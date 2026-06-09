using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ResidenceEtudiante.Data;
using System.Net;
using System.Security.Claims;

namespace ResidenceEtudianteTests.Integration
{
    public class DemandeAutorisationTableauTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private ClaimsPrincipal? _currentUser;

        public DemandeAutorisationTableauTests(WebApplicationFactory<Program> factory)
        {
            WebApplicationFactory<Program> testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDemandeRepository, MockDemandeRepository>();
                    services.AddSingleton<IEtudiantRepository, MockEtudiantRepository>();
                    services.AddSingleton<ISemestreRepository, MockSemestreRepository>();
                    services.AddSingleton<IGenresRepository, MockGenreRepository>();
                    services.AddSingleton<IUniteRepository, MockUniteRepository>();

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", _ => { });
                });

                builder.UseEnvironment("Test");
            });

            _client = testFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }


        // aider par l'IA pour faire tous les Case possible d'autorisation parce que y'en a beaucoup mais logique des utils pour les test fait par felix
        [Theory]
        [InlineData("Admin", "/Demande/Index", HttpStatusCode.Forbidden)]
        [InlineData("Gestionnaire", "/Demande/Index", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", "/Demande/Index", HttpStatusCode.OK)]
        [InlineData("NonConnecte", "/Demande/Index", HttpStatusCode.Unauthorized)]
        [InlineData("Admin", "/Demande/Creer", HttpStatusCode.Forbidden)]
        [InlineData("Gestionnaire", "/Demande/Creer", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", "/Demande/Creer", HttpStatusCode.OK)]
        [InlineData("AutreEtudiant", "/Demande/Creer", HttpStatusCode.OK)]
        [InlineData("NonConnecte", "/Demande/Creer", HttpStatusCode.Unauthorized)]
        [InlineData("Admin", "/Demande/Modifier/1", HttpStatusCode.Forbidden)]
        [InlineData("Gestionnaire", "/Demande/Modifier/1", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", "/Demande/Modifier/1", HttpStatusCode.OK)]
        [InlineData("AutreEtudiant", "/Demande/Modifier/1", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", "/Demande/Modifier/1", HttpStatusCode.Unauthorized)]

        [InlineData("Admin", "/Demande/Demandes", HttpStatusCode.OK)]
        [InlineData("Gestionnaire", "/Demande/Demandes", HttpStatusCode.OK)]
        [InlineData("Proprietaire", "/Demande/Demandes", HttpStatusCode.Forbidden)]
        [InlineData("AutreEtudiant", "/Demande/Demandes", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", "/Demande/Demandes", HttpStatusCode.Unauthorized)]

        [InlineData("Admin", "/GestionDemande/Index", HttpStatusCode.OK)]
        [InlineData("Gestionnaire", "/GestionDemande/Index", HttpStatusCode.OK)]
        [InlineData("Proprietaire", "/GestionDemande/Index", HttpStatusCode.Forbidden)]
        [InlineData("AutreEtudiant", "/GestionDemande/Index", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", "/GestionDemande/Index", HttpStatusCode.Unauthorized)]
        public async Task AutorisationsGetRespectentLeTableau(string utilisateur, string url, HttpStatusCode attendu)
        {
            _currentUser = CreerUtilisateurPourTest(utilisateur);

            HttpResponseMessage response = await _client.GetAsync(url, TestContext.Current.CancellationToken);

            Assert.Equal(attendu, response.StatusCode);
        }

        [Theory]
        [InlineData("Admin", HttpStatusCode.Forbidden)]
        [InlineData("Gestionnaire", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", HttpStatusCode.Redirect)]
        [InlineData("AutreEtudiant", HttpStatusCode.Redirect)]
        [InlineData("NonConnecte", HttpStatusCode.Unauthorized)]
        public async Task DemandeCreerPostRespecteLeTableau(string utilisateur, HttpStatusCode attendu)
        {
            _currentUser = CreerUtilisateurPourTest(utilisateur);
            HttpContent contenu = utilisateur is "Proprietaire" or "AutreEtudiant"
                ? await CreerFormulaireDemandeValide("/Demande/Creer")
                : new FormUrlEncodedContent(new Dictionary<string, string>());

            HttpResponseMessage response = await _client.PostAsync("/Demande/Creer", contenu, TestContext.Current.CancellationToken);

            Assert.Equal(attendu, response.StatusCode);
        }

        [Theory]
        [InlineData("Admin", HttpStatusCode.Forbidden)]
        [InlineData("Gestionnaire", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", HttpStatusCode.OK)]
        [InlineData("AutreEtudiant", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", HttpStatusCode.Unauthorized)]
        public async Task DemandeModifierPostRespecteLeTableau(string utilisateur, HttpStatusCode attendu)
        {
            _currentUser = CreerUtilisateurPourTest(utilisateur);
            HttpContent contenu = utilisateur == "Proprietaire"
                ? await CreerFormulaireModifierValide()
                : await CreerFormulaireAvecToken(CheminTokenPour(utilisateur), new Dictionary<string, string>
                {
                    ["Demande.Id"] = "1"
                });

            HttpResponseMessage response = await _client.PostAsync("/Demande/Modifier", contenu, TestContext.Current.CancellationToken);

            Assert.Equal(attendu, response.StatusCode);
        }

        [Theory]
        [InlineData("Admin", HttpStatusCode.Redirect)]
        [InlineData("Gestionnaire", HttpStatusCode.Forbidden)]
        [InlineData("Proprietaire", HttpStatusCode.Redirect)]
        [InlineData("AutreEtudiant", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", HttpStatusCode.Unauthorized)]
        public async Task DemandeSupprimerPostRespecteLeTableau(string utilisateur, HttpStatusCode attendu)
        {
            _currentUser = CreerUtilisateurPourTest(utilisateur);
            HttpContent contenu = utilisateur switch
            {
                "Admin" => await CreerFormulaireAvecToken("/Demande/Demandes", new Dictionary<string, string>()),
                "Proprietaire" => await CreerFormulaireAvecToken("/Demande/Creer", new Dictionary<string, string>()),
                "Gestionnaire" => await CreerFormulaireAvecToken("/GestionDemande/Index", new Dictionary<string, string>()),
                "AutreEtudiant" => await CreerFormulaireAvecToken("/Demande/Creer", new Dictionary<string, string>()),
                _ => new FormUrlEncodedContent(new Dictionary<string, string>())
            };

            HttpResponseMessage response = await _client.PostAsync("/Demande/Supprimer/1", contenu, TestContext.Current.CancellationToken);

            Assert.Equal(attendu, response.StatusCode);
        }

        [Theory]
        [InlineData("Admin", HttpStatusCode.Redirect)]
        [InlineData("Gestionnaire", HttpStatusCode.Redirect)]
        [InlineData("Proprietaire", HttpStatusCode.Forbidden)]
        [InlineData("AutreEtudiant", HttpStatusCode.Forbidden)]
        [InlineData("NonConnecte", HttpStatusCode.Unauthorized)]
        public async Task GestionDemandeTraiterPostRespecteLeTableau(string utilisateur, HttpStatusCode attendu)
        {
            _currentUser = CreerUtilisateurPourTest(utilisateur);
            HttpContent contenu = utilisateur is "Admin" or "Gestionnaire"
                ? await CreerFormulaireAvecToken("/GestionDemande/Index", new Dictionary<string, string>
                {
                    ["demandeId"] = "1",
                    ["statut"] = "0",
                    ["uniteId"] = ""
                })
                : new FormUrlEncodedContent(new Dictionary<string, string>());

            HttpResponseMessage response = await _client.PostAsync("/GestionDemande/TraiterDemande", contenu, TestContext.Current.CancellationToken);

            Assert.Equal(attendu, response.StatusCode);
        }

        private async Task<HttpContent> CreerFormulaireDemandeValide(string cheminToken)
        {
            return await CreerFormulaireAvecToken(cheminToken, new Dictionary<string, string>
            {
                ["SelectedSemestreId"] = "5",
                ["SelectedGenreIds"] = "1",
                ["Demande.PrefDureeBail"] = "120",
                ["Demande.AccepteReglements"] = "true",
                ["Demande.AccepteTraitementDonnees"] = "true",
                ["Demande.ConfirmeSoumission"] = "true",
                ["Demande.NomGarant"] = "Garant",
                ["Demande.PrenomGarant"] = "Test",
                ["Demande.DateNaissanceGarant"] = "1970-01-01",
                ["Demande.CourrielGarant"] = "garant@test.com",
                ["Demande.TelephoneGarant"] = "819-111-2222",
                ["Demande.NomUrgence"] = "Urgence",
                ["Demande.LienParenteUrgence"] = "Parent",
                ["Demande.TelephoneUrgence"] = "819-333-4444"
            });
        }

        private async Task<HttpContent> CreerFormulaireModifierValide()
        {
            Dictionary<string, string> donnees = new()
            {
                ["Demande.Id"] = "1",
                ["SelectedSemestreId"] = "1",
                ["SelectedGenreIds"] = "1",
                ["Demande.PrefDureeBail"] = "120",
                ["Demande.AccepteReglements"] = "true",
                ["Demande.AccepteTraitementDonnees"] = "true",
                ["Demande.ConfirmeSoumission"] = "true",
                ["Demande.NomGarant"] = "Garant",
                ["Demande.PrenomGarant"] = "Test",
                ["Demande.DateNaissanceGarant"] = "1970-01-01",
                ["Demande.CourrielGarant"] = "garant@test.com",
                ["Demande.TelephoneGarant"] = "819-111-2222",
                ["Demande.NomUrgence"] = "Urgence",
                ["Demande.LienParenteUrgence"] = "Parent",
                ["Demande.TelephoneUrgence"] = "819-333-4444"
            };

            return await CreerFormulaireAvecToken("/Demande/Modifier/1", donnees);
        }

        private async Task<HttpContent> CreerFormulaireAvecToken(string chemin, Dictionary<string, string> donnees)
        {
            HttpResponseMessage response = await _client.GetAsync(chemin, TestContext.Current.CancellationToken);
            string html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            donnees["__RequestVerificationToken"] = Utils.GetToken(html);

            return new FormUrlEncodedContent(donnees);
        }

        private static string CheminTokenPour(string utilisateur)
        {
            return utilisateur switch
            {
                "Admin" => "/Demande/Demandes",
                "Gestionnaire" => "/GestionDemande/Index",
                "AutreEtudiant" => "/Demande/Creer",
                _ => "/Demande/Creer"
            };
        }

        private static ClaimsPrincipal? CreerUtilisateurPourTest(string utilisateur)
        {
            return utilisateur switch
            {
                "Admin" => AuthUtilities.CreerAdmin(),
                "Gestionnaire" => AuthUtilities.CreerGestionnaire(),
                "Proprietaire" => AuthUtilities.CreerEtudiant(),
                "AutreEtudiant" => CreerAutreEtudiant(),
                "NonConnecte" => null,
                _ => throw new ArgumentOutOfRangeException(nameof(utilisateur))
            };
        }

        private static ClaimsPrincipal CreerAutreEtudiant()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "468a4852-42ee-4f45-9be5-41422b589902"),
                new(ClaimTypes.Name, "Autre etudiant"),
                new(ClaimTypes.Email, "autre@test.com"),
                new(ClaimTypes.Role, "Utilisateur")
            }, "TestAuth"));
        }
    }
}
