using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

/*
 * @author John Zuleta
 * Description: Tests d'intégration pour la création d'un campus.
 */
namespace S14_ProjetSessionTests.Integration.CampusTests
{
    public class AjoutCampusTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private ICampusRepository _campusRepository = new MockCampusRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();


        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        // /Campus/Creer contient un formulaire avec token CSRF
        // accessible aux rôles Admin et Gestionnaire
        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync(
                "/Campus/Creer", TestContext.Current.CancellationToken);
            string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            string token = await ObtenirToken();
            data["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(data);
        }

        public AjoutCampusTest(WebApplicationFactory<Program> factory)
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

            _client = _factory.CreateClient();

            _currentUser = _admin;
        }

        // Vérifie qu'un campus valide est bien ajouté au repository
        [Fact(DisplayName = "Création valide ajoute le campus")]
        public async Task Creer_Valide_AjouteCampus()
        {
            _currentUser = _admin;
            int avant = _campusRepository.Campus.Count();

            HttpContent form = await GetForm(new Dictionary<string, string>
            {
                { "Nom",         "Nouveau Campus" },
                { "Abreviation", "NC"             }
            });

            await _client.PostAsync("/Campus/Creer", form, TestContext.Current.CancellationToken);

            Assert.Equal(avant + 1, _campusRepository.Campus.Count());
        }

        // Vérifie qu'un campus avec données invalides (nom vide) n'est pas ajouté
        [Fact(DisplayName = "Création invalide ne modifie pas le repository")]
        public async Task Creer_Invalide_NAjoutePas()
        {
            _currentUser = _admin;
            int avant = _campusRepository.Campus.Count();

            HttpContent form = await GetForm(new Dictionary<string, string>
            {
                { "Nom",         "" },
                { "Abreviation", "" }
            });

            await _client.PostAsync("/Campus/Creer", form, TestContext.Current.CancellationToken);

            Assert.Equal(avant, _campusRepository.Campus.Count());
        }
    }
}