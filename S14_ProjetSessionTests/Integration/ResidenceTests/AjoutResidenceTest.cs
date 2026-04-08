using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Net;
using System.Security.Claims;

namespace S14_ProjetSessionTests.Integration.ResidenceTests
{
    public class AjoutResidenceTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

        private ClaimsPrincipal? _utilisateurActuel;
        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");
            string body = await response.Content.ReadAsStringAsync();
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            HttpContent form = new FormUrlEncodedContent(data);
            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
            return form;
        }

        public AjoutResidenceTest(WebApplicationFactory<Program> factory)
        {
            _campusRepo.Setup(c => c.GetAll()).Returns(new List<Campus>
            {
                new Campus { Id = 1, Nom = "Test Campus" }
            });

            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>
            {
                new Commodite { Id = 1, Nom = "Piscine" },
                new Commodite { Id = 2, Nom = "Salle de sport" }
            });

            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>()))
                          .Returns((int id) => _commoditeRepo.Object.Commodites.FirstOrDefault(c => c.Id == id));

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });

                builder.UseSetting("environment", "Test");
            });
            _client = _factory.CreateClient();
            _utilisateurActuel = _admin;
        }


        [Fact(DisplayName = "AjouterResidence refuse utilisateur non connecté")]
        public async Task AjouterRefuseNonConnecte()
        {
            _utilisateurActuel = null;

            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact(DisplayName = "AjouterResidence accessible admin")]
        public async Task AjouterAccessibleAdmin()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact(DisplayName = "Création valide redirige")]
        public async Task CreerValide()
        {
            int initial = _residenceRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Nom", "Nouvelle" },
                { "CampusId", "1" },
                { "Adresse.AdresseString", "123 rue" },
                { "Adresse.CodePostal", "J1J1J1" }
            };

            HttpContent form = await GetForm(data);

            await _client.PostAsync("/Residence/Creer", form);

            Assert.Equal(initial + 1, _residenceRepository.GetAll().Count);
        }


        [Fact(DisplayName = "Création invalide ne fonctionne pas")]
        public async Task CreerInvalide()
        {
            int initial = _residenceRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Nom", "" }, 
                { "CampusId", "1" }
            };

            HttpContent form = await GetForm(data);

            await _client.PostAsync("/Residence/Creer", form);

            Assert.Equal(initial, _residenceRepository.GetAll().Count);
        }
    }
}