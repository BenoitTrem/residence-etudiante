using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace S14_ProjetSessionTests.Integration.DemandeTests
{
    public class ModifierDemandeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IDemandeRepository _demandeRepository = new MockDemandeRepository();
        private ISemestreRepository _semestreRepository = new MockSemestreRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private ClaimsPrincipal? _currentUser;
        private ClaimsPrincipal _utilisateur = AuthUtilities.CreerEtudiant();

        public ModifierDemandeTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDemandeRepository>(_demandeRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ISemestreRepository>(_semestreRepository);
                    services.AddSingleton<IGenresRepository>(_genresRepository);

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
            formData["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(formData);
        }

        [Fact(DisplayName = "vérifier que la modification d’un objet s’effectue avec des données valides")]
        public async Task VerifierModificationValide()
        {
            // Créer les données du formulaire
            // deja dans la liste des combinaison semestre 1 et etudiant 1 donc pas de doublon donc devrais retourné avec un texte    

            var formData = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "SemestreId", "10" },
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

                { "jumelage[0].Nom", "Alex" },
                { "jumelage[0].Courriel", "alex@test.com" }
            };

            string chemin = $"/Demande/Modifier/{1}";

            HttpContent form = await GetForm(formData, chemin);

            int demandeAvant = _demandeRepository.GetDemande(1).SemestreId;

            HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);

            Demande demandeApres = _demandeRepository.GetDemande(1);

            Assert.NotEqual(demandeAvant, demandeApres.SemestreId);
        }


        // - Un test pour vérifier que la modification d’un objet est refusée avec des données invalides
        [Fact]
        public async Task VerifierModificationInvalideValide()
        {
            // Créer les données du formulaire
            // deja dans la liste des combinaison Semestre n'est pas un Id Valide Donc ca devrait ne pas changer le Id de l'objet
            int SemestreAvant = _demandeRepository.GetDemande(1).SemestreId;

            var formData = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "SemestreId", "68" },
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

                { "jumelage[0].Nom", "Alex" },
                { "jumelage[0].Courriel", "alex@test.com" }
            };

            string chemin = $"/Demande/Modifier/{1}";

            HttpContent form = await GetForm(formData, chemin);


            HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);

            Demande demandeApres = _demandeRepository.GetDemande(1);

            Assert.Equal(SemestreAvant, demandeApres.SemestreId);
        }
    }
}
