using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Net;
using System.Security.Claims;

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





        private async Task<string> ObtenirToken(string chemin)
        {
            HttpResponseMessage response = await _client.GetAsync(chemin, TestContext.Current.CancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            return Utils.GetToken(responseBody);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> formData, string chemin)
        {
            string token = await ObtenirToken(chemin);
            Console.WriteLine("TOKEN = " + token);

            formData["__RequestVerificationToken"] = token;

            foreach (var kvp in formData)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }

            return new FormUrlEncodedContent(formData);
        }

        // test que le Mock ajout fonctionne bien
        [Fact]
        public void AjouterDemandeAjouteCorrectement()
        {
            MockDemandeRepository repo = new MockDemandeRepository();
            int avant = repo.Demandes.Count;
            Demande demande = new Demande
            {
                Id = 999,
                EtudiantId = 1,
                SemestreId = 1
            };

            repo.Creer(demande);

            Assert.Equal(avant + 1, repo.Demandes.Count);

        }


        [Fact]
        public async Task RequeteIndexFonctionne()
        {
            var response = await _client.GetAsync("/Demande");

            Console.WriteLine(response);
            Assert.True(response.IsSuccessStatusCode);
        }


        [Fact(DisplayName = "RequestVerificationToken est vérifié")]
        public async Task CreerSansTokenRetourne400()
        {
            var formData = new Dictionary<string, string>
                {
                    { "SemestreId", "5" },
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

            //var content = await GetForm(formData, "/demande/creer");
            HttpContent content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Demande/Creer", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }







        //- Un test pour vérifier que la création d’un objet s’effectue avec des données valides

        [Fact]
        public async Task CreationDuneDemande()
        {
            int avant = _demandeRepository.Demandes.Count();
            var formData = new Dictionary<string, string>
            {
                { "SelectedSemestreId", "5" },
                { "SelectedGenreIds[0]", "1" },
                { "SelectedGenreIds[1]", "2" },
                { "Demande.PrefDureeBail", "120" },

                { "Demande.AccepteReglements", "true" },
                { "Demande.AccepteTraitementDonnees", "true" },
                { "Demande.ConfirmeSoumission", "true" },
                { "Demande.DateDemande", DateTime.Today.ToString("yyyy-MM-dd") },

                { "Demande.NomGarant", "Tremblay" },
                { "Demande.PrenomGarant", "Jean" },
                { "Demande.DateNaissanceGarant", "1990-01-01" },
                { "Demande.CourrielGarant", "test@test.com" },
                { "Demande.TelephoneGarant", "8191234567" },

                { "Demande.NomParent", "Parent Test" },
                { "Demande.CourrielParent", "parent@test.com" },

                { "Demande.NomUrgence", "Urgence Test" },
                { "Demande.LienParenteUrgence", "Pere" },
                { "Demande.TelephoneUrgence", "8199999999" },

                { "Jumelages[0].Nom", "Alex" },
                { "Jumelages[0].Courriel", "alex@test.com" }
            };

            string chemin = "/Demande/Creer";
            HttpContent form = await GetForm(formData, chemin);
            HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(avant + 1, _demandeRepository.Demandes.Count());
            Demande nouvelleDemande = _demandeRepository.Demandes.Last();
            Assert.Equal(2, nouvelleDemande.DemandeGenres.Count);
            Assert.Contains(nouvelleDemande.DemandeGenres, dg => dg.GenreId == 1);
            Assert.Contains(nouvelleDemande.DemandeGenres, dg => dg.GenreId == 2);
        }

        // - Un test pour vérifier que la création d’un objet est refusée avec des données invalides
        [Fact(DisplayName = "Une demande en doublon retourne au formulaire et affiche un message d'erreur")]
        public async Task CreerRedirigeVersCreationSiInvalide()
        {
            // Déjà présent dans le mock: EtudiantId=1 + SemestreId=1
            var formData = new Dictionary<string, string>
            {
                { "SelectedSemestreId", "1" },
                { "SelectedGenreIds[0]", "1" },
                { "Demande.PrefDureeBail", "120" },

                { "Demande.AccepteReglements", "true" },
                { "Demande.AccepteTraitementDonnees", "true" },
                { "Demande.ConfirmeSoumission", "true" },
                { "Demande.DateDemande", DateTime.Today.ToString("yyyy-MM-dd") },

                { "Demande.NomGarant", "Tremblay" },
                { "Demande.PrenomGarant", "Jean" },
                { "Demande.DateNaissanceGarant", "1990-01-01" },
                { "Demande.CourrielGarant", "test@test.com" },
                { "Demande.TelephoneGarant", "8191234567" },

                { "Demande.NomParent", "Parent Test" },
                { "Demande.CourrielParent", "parent@test.com" },

                { "Demande.NomUrgence", "Urgence Test" },
                { "Demande.LienParenteUrgence", "Pere" },
                { "Demande.TelephoneUrgence", "8199999999" },

                { "Jumelages[0].Nom", "Alex" },
                { "Jumelages[0].Courriel", "alex@test.com" }
            };

            string chemin = "/Demande/Creer";
            HttpContent form = await GetForm(formData, chemin);
            HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);
            string html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            html = WebUtility.HtmlDecode(html);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Une demande existe déjà pour cet étudiant et ce semestre.", html);
        }



      


        [Fact]
        public async Task LaRouteDemandesPasPourEtudiant()
        {

            HttpResponseMessage response = await _client.GetAsync("/demande/demandes");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden);
        }


        [Fact]
        public async Task IndexAfficherLesDemandes()
        {

            HttpResponseMessage response = await _client.GetAsync("/demande");
            string htmlContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Alex Tremblay", htmlContent);
            Assert.Contains("8191112222", htmlContent);
            Assert.Contains("Martin Jean", htmlContent);

        }





    }
}
