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
 * Description: Tests d'intégration pour la modification d'un campus et la relation Campus -> Etudiant.
 * J'ai utiliser l'AI pour ma docummenation des tests 
 */
namespace S14_ProjetSessionTests.Integration.CampusTests
{
    public class ModifierCampusTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private ICampusRepository _campusRepository = new MockCampusRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();

        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();
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

        public ModifierCampusTest(WebApplicationFactory<Program> factory)
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

        // Vérifie qu'une modification valide met à jour le nom du campus
        [Fact(DisplayName = "Modification valide met à jour le campus")]
        public async Task Modifier_Valide_MetAJourCampus()
        {
            _currentUser = _admin;

            Campus campus = _campusRepository.GetById(1)!;
            Assert.NotNull(campus);

            HttpContent form = await GetForm(new Dictionary<string, string>
            {
                { "Id",          campus.Id.ToString() },
                { "Nom",         "Campus Modifié"     },
                { "Abreviation", "CM"                 }
            });

            await _client.PostAsync("/Campus/Modifier", form, TestContext.Current.CancellationToken);

            Assert.Equal("Campus Modifié", _campusRepository.GetById(1)!.Nom);
        }

        // Vérifie qu'une modification invalide (nom vide) ne change pas le campus
        [Fact(DisplayName = "Modification invalide ne modifie pas le campus")]
        public async Task Modifier_Invalide_NeModifiePas()
        {
            _currentUser = _admin;

            Campus campus = _campusRepository.GetById(1)!;
            Assert.NotNull(campus);
            string nomAvant = campus.Nom;

            HttpContent form = await GetForm(new Dictionary<string, string>
            {
                { "Id",          campus.Id.ToString() },
                { "Nom",         ""                   },
                { "Abreviation", ""                   }
            });

            await _client.PostAsync("/Campus/Modifier", form, TestContext.Current.CancellationToken);

            Assert.Equal(nomAvant, _campusRepository.GetById(1)!.Nom);
        }

       
    }
}