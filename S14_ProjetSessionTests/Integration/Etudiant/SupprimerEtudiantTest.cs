using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using System.Net;
using System.Security.Claims;

/*
 * @author John Zuleta
 * Description: Tests d'intégration pour la suppression d'un étudiant.
 *              La suppression est réservée au rôle Admin uniquement (policy AdminUniquement).
 *              Un gestionnaire ne peut pas supprimer — seulement un admin.
 */
namespace S14_ProjetSessionTests.Integration.Etudiant
{
    public class SupprimerEtudiantTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private ICampusRepository _campusRepository = new MockCampusRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private IProgrammesRepository _programmesRepository = new MockProgrammesRepository();

        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();
        private readonly ClaimsPrincipal _gestionnaire = AuthUtilities.CreerGestionnaire();
        private readonly ClaimsPrincipal _etudiant = AuthUtilities.CreerEtudiant();

        // /Campus/Creer contient un formulaire avec token CSRF
        // Accessible aux admins et gestionnaires (AdminOuGestionnaire)
        // Pour l'étudiant on utilise /Etudiant/Modifier/1 car il a accès à son propre profil
        private async Task<string> ObtenirToken()
        {
            string chemin = (_currentUser == _etudiant)
                ? "/Etudiant/Modifier/1"
                : "/Campus/Creer";

            HttpResponseMessage response = await _client.GetAsync(
                chemin, TestContext.Current.CancellationToken);
            string body = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);
            return Utils.GetToken(body);
        }


        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            string token = await ObtenirToken();
            data["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(data);
        }

        public SupprimerEtudiantTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepository);
                    services.AddSingleton<IGenresRepository>(_genresRepository);
                    services.AddSingleton<IProgrammesRepository>(_programmesRepository);
                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();

            _currentUser = _admin;
        }

        // Vérifie qu'un admin peut supprimer un étudiant (AdminUniquement)
        [Fact(DisplayName = "Suppression étudiant réussie pour un admin")]
        public async Task Supprimer_Admin_Reussit()
        {
            _currentUser = _admin;
            int avant = _etudiantRepository.Etudiants.Count();

            HttpContent form = await GetForm(
                new Dictionary<string, string> { { "Id", "2" } });

            await _client.PostAsync("/Etudiant/Supprimer", form, TestContext.Current.CancellationToken);

            Assert.Equal(avant - 1, _etudiantRepository.Etudiants.Count());
        }

        // Vérifie qu'un gestionnaire ne peut PAS supprimer un étudiant (AdminUniquement)
        [Fact(DisplayName = "Suppression étudiant refusée à un gestionnaire")]
        public async Task Supprimer_Gestionnaire_Refuse()
        {
            _currentUser = _gestionnaire;
            int avant = _etudiantRepository.Etudiants.Count();

            HttpContent form = await GetForm(
                new Dictionary<string, string> { { "Id", "2" } });

            HttpResponseMessage response = await _client.PostAsync(
                "/Etudiant/Supprimer", form, TestContext.Current.CancellationToken);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden);

            Assert.Equal(avant, _etudiantRepository.Etudiants.Count());
        }

        // Vérifie qu'un étudiant ne peut PAS supprimer (AdminUniquement)
        [Fact(DisplayName = "Suppression étudiant refusée à un étudiant")]
        public async Task Supprimer_Etudiant_Refuse()
        {
            _currentUser = _etudiant;
            int avant = _etudiantRepository.Etudiants.Count();

            HttpContent form = await GetForm(
                new Dictionary<string, string> { { "Id", "2" } });

            HttpResponseMessage response = await _client.PostAsync(
                "/Etudiant/Supprimer", form, TestContext.Current.CancellationToken);

            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden);

            Assert.Equal(avant, _etudiantRepository.Etudiants.Count());
        }
    }
}