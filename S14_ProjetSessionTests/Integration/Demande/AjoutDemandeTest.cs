using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
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

    //    [Fact]
    //    public async Task CreationDuneDemande()
    //    {
    //        int avant = _demandeRepository.Demandes.Count();
    //        var formData = new Dictionary<string, string>
    //{
    //    { "SemestreId", "5" },
    //    { "EtudiantId", "1" },
    //    { "PreferencesGenreId", "1" },
    //    { "PrefDureeBail", "120" },

    //    { "AccepteReglements", "true" },
    //    { "AccepteTraitementDonnees", "true" },
    //    { "ConfirmeSoumission", "true" },

    //    { "NomGarant", "Tremblay" },
    //    { "PrenomGarant", "Jean" },
    //    { "DateNaissanceGarant", "1990-01-01" },
    //    { "CourrielGarant", "test@test.com" },
    //    { "TelephoneGarant", "8191234567" },

    //    { "NomParent", "Parent Test" },
    //    { "CourrielParent", "parent@test.com" },

    //    { "NomUrgence", "Urgence Test" },
    //    { "LienParenteUrgence", "Pere" },
    //    { "TelephoneUrgence", "8199999999" },

    //    { "jumelage[0].Nom", "Alex" },
    //    { "jumelage[0].Courriel", "alex@test.com" }
    //};

    //        string chemin = "/Demande/Creer";
    //        HttpContent form = await GetForm(formData, chemin);
    //        HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);

    //        Assert.Equal(avant + 1, _demandeRepository.Demandes.Count());
    //    }

        // - Un test pour vérifier que la création d’un objet est refusée avec des données invalides
        //[Fact(DisplayName = "Un nom vide retourne au formulaire et affiche message d'erreur")]
        //public async Task CreerRedirigeVersCreationSiInvalide()
        //{
        //    // Créer les données du formulaire
        //    // deja dans la liste des combinaison semestre 1 et etudiant 1 donc pas de doublon donc devrais retourné avec un texte    

        //    var formData = new Dictionary<string, string>
        //    {
        //        { "SemestreId", "1" },
        //        { "EtudiantId", "1" },
        //        { "PreferencesGenreId", "1" },
        //        { "PrefDureeBail", "120" },

        //        { "AccepteReglements", "true" },
        //        { "AccepteTraitementDonnees", "true" },
        //        { "ConfirmeSoumission", "true" },

        //        { "NomGarant", "Tremblay" },
        //        { "PrenomGarant", "Jean" },
        //        { "DateNaissanceGarant", "1990-01-01" },
        //        { "CourrielGarant", "test@test.com" },
        //        { "TelephoneGarant", "8191234567" },

        //        { "NomParent", "Parent Test" },
        //        { "CourrielParent", "parent@test.com" },

        //        { "NomUrgence", "Urgence Test" },
        //        { "LienParenteUrgence", "Pere" },
        //        { "TelephoneUrgence", "8199999999" },

        //        { "jumelage[0].Nom", "Alex" },
        //        { "jumelage[0].Courriel", "alex@test.com" }
        //    };

        //    string chemin = "/Demande/Creer";
        //    HttpContent form = await GetForm(formData, chemin);
        //    HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);
        //    string html = await response.Content.ReadAsStringAsync();
        //    // permet les accents et caractere spécial
        //    html = WebUtility.HtmlDecode(html);

        //    Assert.Contains("Une demande existe déjà pour cet étudiant et ce semestre.", html);
        //}



        // - Un test pour vérifier que la modification d’un objet s’effectue avec des données valides




        // - Un test pour vérifier la modification de la relation d’un objet


        //- Deux tests pour vérifier qu’une route n’est pas accessible aux utilisateurs qui ne sont pas connectés
        //(un test qui vérifie que la route est accessible à l’utilisateur connecté, un test qui vérifie que la même
        //route n’est pas accessible à l’utilisateur qui n’est pas connecté)


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
