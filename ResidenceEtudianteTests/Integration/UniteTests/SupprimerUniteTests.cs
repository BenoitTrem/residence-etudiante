using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;
using System.Security.Claims;

/*
 * @author Benoit
 * 
 * Description: Tests d'intégrations pour la suppression d'une unité.
 */
namespace ResidenceEtudianteTests.Integration.UniteTests
{
    public class SupprimerUniteTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IUniteRepository _uniteRepository = new MockUniteRepository();
        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

        private ClaimsPrincipal? _utilisateurActuel;
        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/Unite/AjouterUnite");
            string body = await response.Content.ReadAsStringAsync();
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            HttpContent form = new FormUrlEncodedContent(data);
            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
            return form;
        }

        public SupprimerUniteTests(WebApplicationFactory<Program> factory)
        {
            _campusRepo.Setup(c => c.Campus).Returns(new List<Campus>
            {
                new Campus { Id = 1, Nom = "Test Campus" }
            });

            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>());
            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>())).Returns((Commodite?)null);

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IUniteRepository>(_uniteRepository);
                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);
                    services.AddSingleton<IEtudiantRepository, MockEtudiantRepository>();

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });

                builder.UseSetting("environment", "Test");
            });

            _client = _factory.CreateClient();
            _utilisateurActuel = _admin;
        }

        // Vérifie qu'un admin peut supprimer une unité
        [Fact(DisplayName = "Suppression valide d'une unité")]
        public async Task SupprimerValide()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "id", "1" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Supprimer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant - 1, countApres);
            Assert.Null(_uniteRepository.GetById(1));
        }

       // Vérifie que le compte est bon si l'utilisateur supprime une unité inexistante
        [Fact(DisplayName = "Suppression d'une unité inexistante ne plante pas")]
        public async Task SupprimerInexistant()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "id", "999" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Supprimer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant, countApres);
        }

        // Vérifie qu'un utilisateur non admin ne peut pas suprrimer
        [Fact(DisplayName = "Suppression refusée si non admin")]
        public async Task SupprimerRefuseNonAdmin()
        {
            _utilisateurActuel = null;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "id", "1" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Supprimer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant, countApres);
        }
    }
}
