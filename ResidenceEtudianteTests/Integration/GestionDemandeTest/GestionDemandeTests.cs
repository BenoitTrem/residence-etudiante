using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace ResidenceEtudianteTests.Integration
{
    public class GestionDemandeTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IDemandeRepository _demandeRepository = new MockDemandeRepository();
        private ISemestreRepository _semestreRepository = new MockSemestreRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private IUniteRepository _uniteRepository = new MockUniteRepository();
        private ClaimsPrincipal? _currentUser;

        public GestionDemandeTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDemandeRepository>(_demandeRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ISemestreRepository>(_semestreRepository);
                    services.AddSingleton<IGenresRepository>(_genresRepository);
                    services.AddSingleton<IUniteRepository>(_uniteRepository);

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
            formData["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(formData);
        }

        [Fact(DisplayName = "Index affiche les demandes pour un gestionnaire")]
        public async Task Index_AfficheLesDemandesPourGestionnaire()
        {
            // Arrange
            _currentUser = AuthUtilities.CreerGestionnaire();

            // Act
            var response = await _client.GetAsync("/GestionDemande", TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("Tremblay", html); // Nom de l'étudiant dans MockDemandeRepository
            Assert.Contains("Gagnon", html);   // Autre nom dans MockDemandeRepository
        }

        [Fact(DisplayName = "TraiterDemande accepte une demande et assigne une unité")]
        public async Task TraiterDemande_Accepter_AssigneUniteCorrectement()
        {
            // Arrange
            _currentUser = AuthUtilities.CreerGestionnaire();
            var formData = new Dictionary<string, string>
            {
                { "demandeId", "1" },
                { "statut", "Acceptee" },
                { "uniteId", "1" }
            };
            var content = await GetForm(formData, "/GestionDemande");

            // Act
            var response = await _client.PostAsync("/GestionDemande/TraiterDemande", content, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var demande = _demandeRepository.GetDemande(1);
            Assert.Equal(StatutDemande.Acceptee, demande.StatutDemande);
            Assert.Equal(1, demande.UniteId);
        }

        [Fact(DisplayName = "TraiterDemande refuse une demande et retire l'unité")]
        public async Task TraiterDemande_Refuser_RetireUnite()
        {
            // Arrange
            _currentUser = AuthUtilities.CreerGestionnaire();
            // On pré-assigne une unité pour vérifier qu'elle est retirée
            var demandeInitiale = _demandeRepository.GetDemande(1);
            demandeInitiale.UniteId = 1;
            demandeInitiale.StatutDemande = StatutDemande.Acceptee;

            var formData = new Dictionary<string, string>
            {
                { "demandeId", "1" },
                { "statut", "Refusee" },
                { "uniteId", "" }
            };
            var content = await GetForm(formData, "/GestionDemande");

            // Act
            var response = await _client.PostAsync("/GestionDemande/TraiterDemande", content, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var demande = _demandeRepository.GetDemande(1);
            Assert.Equal(StatutDemande.Refusee, demande.StatutDemande);
            Assert.Null(demande.UniteId);
        }

        [Fact(DisplayName = "TraiterDemande échoue si aucune unité n'est sélectionnée lors de l'acceptation")]
        public async Task TraiterDemande_AccepterSansUnite_RetourneErreur()
        {
            // Arrange
            _currentUser = AuthUtilities.CreerGestionnaire();
            var formData = new Dictionary<string, string>
            {
                { "demandeId", "1" },
                { "statut", "Acceptee" },
                { "uniteId", "" }
            };
            var content = await GetForm(formData, "/GestionDemande");

            // Act
            var response = await _client.PostAsync("/GestionDemande/TraiterDemande", content, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var demande = _demandeRepository.GetDemande(1);
            Assert.NotEqual(StatutDemande.Acceptee, demande.StatutDemande);
            // Vérifier que TempData contient l'erreur (difficile en intégration pure sans inspecter les cookies, mais on peut vérifier que le statut n'a pas changé)
        }

        [Fact(DisplayName = "TraiterDemande échoue si la capacité de l'unité est dépassée")]
        public async Task TraiterDemande_Accepter_CapaciteDepassee_RetourneErreur()
        {
            // Arrange
            _currentUser = AuthUtilities.CreerGestionnaire();
            
            // On utilise l'unité 3 qui a une capacité de 1 dans le mock
            var unite3 = _uniteRepository.GetById(3);
            Assert.Equal(1, unite3.Capacite);

            // On ajoute un jumelage à la demande 1 pour forcer 2 personnes
            var demande1 = _demandeRepository.GetDemande(1);
            demande1.Jumelages = new List<Jumelage> 
            { 
                new Jumelage { Nom = "Ami", Courriel = "ami@test.com" } 
            };

            var formData = new Dictionary<string, string>
            {
                { "demandeId", "1" },
                { "statut", "Acceptee" },
                { "uniteId", "3" }
            };
            var content = await GetForm(formData, "/GestionDemande");

            // Act
            var response = await _client.PostAsync("/GestionDemande/TraiterDemande", content, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var demandeApres = _demandeRepository.GetDemande(1);
            Assert.NotEqual(StatutDemande.Acceptee, demandeApres.StatutDemande);
            Assert.Null(demandeApres.UniteId);
        }
    }
}
