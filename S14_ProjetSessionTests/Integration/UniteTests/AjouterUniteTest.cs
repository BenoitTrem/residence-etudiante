using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

/*
 * @author Benoit
 * 
 * Description: Tests d'intégrations pour l'ajout d'une unité.
 */
namespace S14_ProjetSessionTests.Integration.UniteTests
{
    public class AjouterUniteTests : IClassFixture<WebApplicationFactory<Program>>
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

        public AjouterUniteTests(WebApplicationFactory<Program> factory)
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

        // Vérifie qu'un admin peut créer une unité
        [Fact(DisplayName = "Création valide d'une unité")]
        public async Task CreerValide()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "ResidenceId", "1" },
                { "Capacite", "2" },
                { "AdapteePourMobiliteReduite", "false" },
                { "nombreUnites", "1" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Creer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant + 1, countApres); 
        }

        // Vérifie qu'un admin peut créer plusieurs unités d'un coup
        [Fact(DisplayName = "Création de plusieurs unités valide")]
        public async Task CreerPlusieursValide()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "ResidenceId", "1" },
                { "Capacite", "2" },
                { "AdapteePourMobiliteReduite", "false" },
                { "nombreUnites", "3" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Creer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant + 3, countApres);
        }

        // Vérifie que la création d'unité est invalide si l'utilisateur ajoute 0 unité à la résidence
        [Fact(DisplayName = "Création invalide refusée si nombreUnites = 0")]
        public async Task CreerInvalideNombreZero()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "ResidenceId", "1" },
                { "Capacite", "2" },
                { "AdapteePourMobiliteReduite", "false" },
                { "nombreUnites", "0" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Creer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant, countApres);
        }

        // Vérifie que la création d'unité est invalide si l'utilisateur ajoute plus de 50 unité à la résidence 
        [Fact(DisplayName = "Création invalide refusée si nombreUnites > 50")]
        public async Task CreerInvalideNombreTropGrand()
        {
            _utilisateurActuel = _admin;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "ResidenceId", "1" },
                { "Capacite", "2" },
                { "AdapteePourMobiliteReduite", "false" },
                { "nombreUnites", "51" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Creer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant, countApres);
        }

        // Vérifie que la création d'unité est refusé si l'utlisateur n'est pas admin
        [Fact(DisplayName = "Création refusée si non admin")]
        public async Task CreerRefuseNonAdmin()
        {
            _utilisateurActuel = null;

            int countAvant = _uniteRepository.GetAll().Count;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "ResidenceId", "1" },
                { "Capacite", "2" },
                { "AdapteePourMobiliteReduite", "false" },
                { "nombreUnites", "1" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Creer", form);

            int countApres = _uniteRepository.GetAll().Count;
            Assert.Equal(countAvant, countApres);
        }
    }
}
