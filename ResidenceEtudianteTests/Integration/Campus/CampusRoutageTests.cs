using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ResidenceEtudiante.Data;
using System.Net;
using System.Security.Claims;

/*
 * @author John Zuleta
 * Description: Tests de routage, de vue et d'autorisation pour le contrôleur Campus.
 * J'ai utliser chatgpt pour la documention
 */
namespace ResidenceEtudianteTests.Integration.CampusTests
{
    public class CampusRoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private ICampusRepository _campusRepository = new MockCampusRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();

        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();
        private readonly ClaimsPrincipal _gestionnaire = AuthUtilities.CreerGestionnaire();
        private readonly ClaimsPrincipal _etudiant = AuthUtilities.CreerEtudiant();

 
        public CampusRoutageTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ICampusRepository>(_campusRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                // doit etre mis pour que le test fonctionne
                AllowAutoRedirect = false
            });

            _currentUser = _admin;
        }

        // ── Route ──────────────────────────────────────────────────────────────

        // Vérifie que la route /Campus retourne HTTP 200
        [Fact(DisplayName = "Route /Campus existe et retourne 200")]
        public async Task Route_Campus_Existe()
        {
            _currentUser = _admin;
            HttpResponseMessage response = await _client.GetAsync("/Campus", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── Vue ────────────────────────────────────────────────────────────────

        // Vérifie que le nom du campus s'affiche dans la vue Index
        [Fact(DisplayName = "Vue Index affiche le nom du campus")]
        public async Task Vue_Index_AfficheNomCampus()
        {
            _currentUser = _admin;
            HttpResponseMessage response = await _client.GetAsync("/Campus", TestContext.Current.CancellationToken);
            string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Contains("Campus Test", body);
        }

        // ── Autorisation : non connecté ────────────────────────────────────────

        // Vérifie que /Campus redirige un utilisateur non connecté vers la connexion
        [Fact(DisplayName = "Route /Campus refuse un utilisateur non connecté")]
        public async Task Index_RefuseNonConnecte()
        {
            _currentUser = null;
            HttpResponseMessage response = await _client.GetAsync("/Campus", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Vérifie que /Campus refuse un étudiant, car la page est réservée aux admins et gestionnaires
        [Fact(DisplayName = "Route /Campus refusée à un étudiant")]
        public async Task Index_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Campus", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ── Autorisation : rôle AdminOuGestionnaire (Creer) ───────────────────

        // Vérifie qu'un gestionnaire peut accéder à /Campus/Creer
        [Fact(DisplayName = "Route /Campus/Creer accessible à un gestionnaire")]
        public async Task Creer_AccessibleGestionnaire()
        {
            _currentUser = _gestionnaire;
            HttpResponseMessage response = await _client.GetAsync("/Campus/Creer", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie qu'un simple étudiant ne peut pas accéder à /Campus/Creer
        [Fact(DisplayName = "Route /Campus/Creer refusée à un étudiant")]
        public async Task Creer_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Campus/Creer", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ── Autorisation : rôle AdminOuGestionnaire (Modifier) ────────────────

        // Vérifie qu'un gestionnaire peut accéder à /Campus/Modifier
        [Fact(DisplayName = "Route /Campus/Modifier accessible à un gestionnaire")]
        public async Task Modifier_AccessibleGestionnaire()
        {
            _currentUser = _gestionnaire;
            HttpResponseMessage response = await _client.GetAsync("/Campus/Modifier/1", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Vérifie qu'un simple étudiant ne peut pas accéder à /Campus/Modifier
        [Fact(DisplayName = "Route /Campus/Modifier refusée à un étudiant")]
        public async Task Modifier_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Campus/Modifier/1", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ── Autorisation : rôle normal (Étudiant) pour actions de modification (POST) ──

        [Fact(DisplayName = "POST /Campus/Creer refusée à un étudiant")]
        public async Task Creer_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Campus/Creer", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "POST /Campus/Modifier refusée à un étudiant")]
        public async Task Modifier_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Campus/Modifier/1", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "POST /Campus/Supprimer refusée à un étudiant")]
        public async Task Supprimer_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Campus/Supprimer/1", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
