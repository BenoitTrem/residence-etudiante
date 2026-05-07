/*
 * @author John Zuleta
 * Description: Tests d'intégration pour le routage et l'autorisation du contrôleur Programme.
 * Vérifie que les rôles appropriés ont accès aux différentes routes.
 */
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using System.Net;
using System.Security.Claims;

namespace S14_ProjetSessionTests.Integration.ProgrammeTests
{
    public class ProgrammeRoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IProgrammesRepository _programmesRepository = new MockProgrammesRepository();
        private ICampusRepository _campusRepository = new MockCampusRepository();

        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();
        private readonly ClaimsPrincipal _gestionnaire = AuthUtilities.CreerGestionnaire();
        private readonly ClaimsPrincipal _etudiant = AuthUtilities.CreerEtudiant();

        public ProgrammeRoutageTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IProgrammesRepository>(_programmesRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepository);
                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _currentUser = _admin;
        }

        // ── Route / Vue ───────────────────────────────────────────────────────

        [Fact(DisplayName = "Route /Programme existe et retourne 200 pour Admin")]
        public async Task Route_Programme_Existe()
        {
            _currentUser = _admin;
            HttpResponseMessage response = await _client.GetAsync("/Programme", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ── Autorisation : non connecté ────────────────────────────────────────

        [Fact(DisplayName = "Route /Programme refuse un utilisateur non connecté")]
        public async Task Index_RefuseNonConnecte()
        {
            _currentUser = null;
            HttpResponseMessage response = await _client.GetAsync("/Programme", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── Autorisation : rôle normal (Étudiant) ──────────────────────────────

        [Fact(DisplayName = "GET /Programme refusée à un étudiant")]
        public async Task Index_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Programme", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "GET /Programme/Creer refusée à un étudiant")]
        public async Task Creer_Get_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Programme/Creer", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "POST /Programme/Creer refusée à un étudiant")]
        public async Task Creer_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Programme/Creer", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "GET /Programme/Modifier refusée à un étudiant")]
        public async Task Modifier_Get_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpResponseMessage response = await _client.GetAsync("/Programme/Modifier/1", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "POST /Programme/Modifier refusée à un étudiant")]
        public async Task Modifier_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Programme/Modifier/1", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = "POST /Programme/Supprimer refusée à un étudiant")]
        public async Task Supprimer_Post_RefuseEtudiant()
        {
            _currentUser = _etudiant;
            HttpContent form = new FormUrlEncodedContent(new Dictionary<string, string>());
            HttpResponseMessage response = await _client.PostAsync("/Programme/Supprimer/1", form, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
