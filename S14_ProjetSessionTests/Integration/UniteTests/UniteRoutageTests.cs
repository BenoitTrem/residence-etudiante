using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Net;
using System.Security.Claims;

/*
 * @author Benoit
 * 
 * Description: Tests de routage pour les unités.
 * vérifie que les URLs retournent les bons codes HTTP et que les vues affichent les données attendues,
 */
namespace S14_ProjetSessionTests.Integration.UniteTests
{
    public class UniteRoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IUniteRepository _uniteRepository = new MockUniteRepository();
        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

        private ClaimsPrincipal? _utilisateurActuel;
        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        public UniteRoutageTests(WebApplicationFactory<Program> factory)
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

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });

                builder.UseSetting("environment", "Test");
            });

            _client = _factory.CreateClient();
            _utilisateurActuel = _admin;
        }

        // Vérifie que la route Index retournent une réponse OK si admin
        [Fact(DisplayName = "Index unités retourne 200")]
        public async Task IndexRetourneOk()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Unite?id=1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie que la route Index est refusé pour un utilisateur qui n'est pas connecté
        [Fact(DisplayName = "Index unités refusé si non connecté")]
        public async Task IndexRefuseNonConnecte()
        {
            _utilisateurActuel = null;

            HttpResponseMessage response = await _client.GetAsync("/Unite?id=1");

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie que la route pour ajouter une unité retournent une réponse OK si admin
        [Fact(DisplayName = "Page ajout unité retourne 200")]
        public async Task AjouterUniteRetourneOk()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Unite/AjouterUnite");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie que la route pour modifier une unité retournent une réponse OK si admin
        [Fact(DisplayName = "Page modifier unité retourne 200")]
        public async Task ModifierUniteRetourneOk()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Unite/ModifierUnite/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie que la route pour modifier une unité avec un ID invalide retournent une réponse OK
        [Fact(DisplayName = "Page modifier unité inexistante retourne redirection")]
        public async Task ModifierUniteInexistanteRetourneRedirection()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Unite/ModifierUnite/999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        }

        // Vérifie que la route Index contain bien le numéro de l'unité
        [Fact(DisplayName = "Vue affiche le numéro de l'unité")]
        public async Task VueAfficheNumeroUnite()
        {
            _utilisateurActuel = _admin;

            HttpResponseMessage response = await _client.GetAsync("/Unite/ModifierUnite/1");
            string body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("101", body);
        }
    }
}