using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace S14_ProjetSessionTests.Integration
{
    public class DemandeGestionnaireAutorisation : IClassFixture<WebApplicationFactory<Program>>
        {
            private readonly WebApplicationFactory<Program> _factory;
            private readonly HttpClient _client;
            private IDemandeRepository _demandeRepository = new MockDemandeRepository();
            private ISemestreRepository _semestreRepository = new MockSemestreRepository();
            private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
            private IGenresRepository _genresRepository = new MockGenreRepository();
            private ClaimsPrincipal? _currentUser;
            private ClaimsPrincipal _utilisateur = AuthUtilities.CreerGestionnaire();


            public DemandeGestionnaireAutorisation(WebApplicationFactory<Program> factory)
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
        [InlineData("/GestionDemande/Index")]
        [InlineData("/Demande/Demandes")]
        [Theory]
        public async Task GestionnairePeutAcceder(String url)
        {

            HttpResponseMessage response = await _client.GetAsync(url);
            Assert.True(response.IsSuccessStatusCode);
        }



    }
}
