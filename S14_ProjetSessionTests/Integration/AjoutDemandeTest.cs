using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace S14_ProjetSessionTests.Integration
{
    public class AjoutDemandeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IDemandeRepository _demandeRepository = new MockDemandeRepository();
        private ISemestreRepository _semestreRepository = new MockSemestreRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private ClaimsPrincipal? _currentUser;
        private ClaimsPrincipal _utilisateur = AuthUtilities.CreerEtudiant();


        public AjoutDemandeTest(WebApplicationFactory<Program> factory)
        {

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Juste pour les tests, comme on n'utilise pas une base de données
                    services.AddSingleton<IDemandeRepository>(_demandeRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ISemestreRepository>(_semestreRepository);
                    services.AddSingleton<IGenresRepository>(_genresRepository);


                    // Fonction qui retourne l'utilisateur courant; utilisée par TestAuthHandler
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
            _currentUser = _utilisateur;

        }





        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/demande/creer", TestContext.Current.CancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            return Utils.GetToken(responseBody);
        }

        // comprendre ca fait quoi CA
        private async Task<HttpContent> GetForm(Dictionary<string, string> formData)
        {
            HttpContent form = new FormUrlEncodedContent(formData);
            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
            return form;
        }

        [Fact(DisplayName = "Le formulaire de création comporte token antiforgery")]
        public async Task CreerComporteToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/demande/creer");
            string body = await response.Content.ReadAsStringAsync();

            Assert.Contains("__RequestVerificationToken", body);
        }

        [Fact(DisplayName = "RequestVerificationToken est vérifié")]
        public async Task CreerSansTokenRetourne400()
        {
            var formData = new Dictionary<string, string>
                {
                    { "SemestreId", "1" },
                    { "EtudiantId", "1" },
                    { "PreferencesGenreId", "1" },
                    { "PrefDureeBail", "120" },

                    { "AccepteReglements", "true" },
                    { "AccepteTraitementDonnees", "true" },
                    { "ConfirmeSoumission", "true" },

                    { "NomGarant", "Tremblay" },
                    { "PrenomGarant", "Jean" },
                    { "DateNaissanceGarant", "1990-01-01" },
                    { "CourrielGarant", "test@test.com" },
                    { "TelephoneGarant", "8191234567" },

                    { "NomParent", "Parent Test" },
                    { "CourrielParent", "parent@test.com" },

                    { "NomUrgence", "Urgence Test" },
                    { "LienParenteUrgence", "Pere" },
                    { "TelephoneUrgence", "8199999999" },

                    // Jumelage list
                    { "jumelage[0].Nom", "Alex" },
                    { "jumelage[0].Courriel", "alex@test.com" }
                };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Demande/Creer", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Un nom vide retourne au formulaire et affiche message d'erreur")]
        public async Task CreerRedirigeVersCreationSiInvalide()
        {
            // Créer les données du formulaire
            var formData = new Dictionary<string, string>
                {
                    { "SemestreId", "1" },
                    { "EtudiantId", "1" },
                    { "PreferencesGenreId", "1" },
                    { "PrefDureeBail", "120" },

                    //{ "AccepteReglements", "true" },
                    //{ "AccepteTraitementDonnees", "true" },
                    //{ "ConfirmeSoumission", "true" },

                    { "NomGarant", "Tremblay" },
                    { "PrenomGarant", "Jean" },
                    { "DateNaissanceGarant", "1990-01-01" },
                    { "CourrielGarant", "test@test.com" },
                    { "TelephoneGarant", "8191234567" },

                    { "NomParent", "Parent Test" },
                    { "CourrielParent", "parent@test.com" },

                    { "NomUrgence", "Urgence Test" },
                    { "LienParenteUrgence", "Pere" },
                    { "TelephoneUrgence", "8199999999" },

                    // Jumelage list
                    { "jumelage[0].Nom", "Alex" },
                    { "jumelage[0].Courriel", "alex@test.com" }
                };
            HttpContent form = await GetForm(formData);
            HttpResponseMessage response = await _client.PostAsync("/Demande/Creer", form, TestContext.Current.CancellationToken);
            
            string responseBody = HttpUtility.HtmlDecode(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            Assert.Contains(("Veuillez spécifier un nom pour votre merveilleuse recette"), responseBody);
        }





    }
}
