//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using S14_ProjetSession.Data;
//using S14_ProjetSession.Models;
//using System.Net;
//using System.Security.Claims;

///*
// * @author Benoit
// * 
// * Description: Tests d'intégrations pour la suppression d'une résidence.
// */
//namespace S14_ProjetSessionTests.Integration.ResidenceTests
//{
//    public class SupprimerResidenceTest : IClassFixture<WebApplicationFactory<Program>>
//    {
//        private readonly WebApplicationFactory<Program> _factory;
//        private readonly HttpClient _client;

//        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
//        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
//        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

//        private ClaimsPrincipal? _utilisateurActuel;
//        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

//        private async Task<string> ObtenirToken()
//        {
//            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");
//            string body = await response.Content.ReadAsStringAsync();
//            return Utils.GetToken(body);
//        }

//        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
//        {
//            HttpContent form = new FormUrlEncodedContent(data);
//            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
//            return form;
//        }

//        public SupprimerResidenceTest(WebApplicationFactory<Program> factory)
//        {
//            _campusRepo.Setup(c => c.GetAll()).Returns(new List<Campus>
//            {
//                new Campus { Id = 1, Nom = "Test Campus" }
//            });

//            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>
//            {
//                new Commodite { Id = 1, Nom = "Piscine" },
//                new Commodite { Id = 2, Nom = "Salle de sport" }
//            });

//            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>()))
//                          .Returns((int id) => _commoditeRepo.Object.Commodites.FirstOrDefault(c => c.Id == id));

//            _factory = factory.WithWebHostBuilder(builder =>
//            {
//                builder.ConfigureTestServices(services =>
//                {
//                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
//                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
//                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);

//                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

//                    services.AddAuthentication("TestAuth")
//                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
//                });

//                builder.UseSetting("environment", "Test");
//            });
//            _client = _factory.CreateClient();
//            _utilisateurActuel = _admin;
//        }

//        // Vérifie que la suppression d'une résidence fonctionne correctement
//        [Fact(DisplayName = "Suppression fonctionne")]
//        public async Task SupprimerFonctionne()
//        {
//            int initial = _residenceRepository.GetAll().Count; // Nombre initial de résidences

//            Dictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "id", "1" }
//            };

//            HttpContent form = await GetForm(data);

//            await _client.PostAsync("/Residence/Supprimer", form);

//            // Vérifie qu'une résidence a été supprimée
//            Assert.Equal(initial - 1, _residenceRepository.GetAll().Count);
//        }
//    }
//}