using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace S14_ProjetSessionTests.Integration
{
    public class SupprimerDemandeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IDemandeRepository _demandeRepository = new MockDemandeRepository();
        private ISemestreRepository _semestreRepository = new MockSemestreRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private ClaimsPrincipal? _currentUser;
        private ClaimsPrincipal _utilisateur = AuthUtilities.CreerAdmin();


        public SupprimerDemandeTest(WebApplicationFactory<Program> factory)
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
        // - Un test pour vérifier la suppression d’un objet

        [Fact]
        public async Task LaRouteDemandesFonctionneAvecAdmin()
        {

            HttpResponseMessage response = await _client.GetAsync("/demande/demandes");
            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }
        ///demande/demandes
        ///
        [Fact(DisplayName = "Un nom vide retourne au formulaire et affiche message d'erreur")]
        public async Task SuppressionDemandeFonctionnel()
        {

            string chemin = $"demande/demandes";
            Demande demande = _demandeRepository.GetDemande(1);

            Dictionary<string, string> formData = new Dictionary<string, string>()
                    {
                        { "id", "1" }
                    };

            HttpContent form = await GetForm(formData, chemin);

            HttpResponseMessage response = await _client.PostAsync(
                "demande/Supprimer",
                form,
                TestContext.Current.CancellationToken
            );
            Assert.False(_demandeRepository.Demandes.Contains(demande));
        }

    }
}
