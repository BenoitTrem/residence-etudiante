using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Text;
using S14_ProjetSession.Data;
namespace S14_ProjetSessionTests.Integration
{
    public class RoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private readonly HttpClient _client;

        public RoutageTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IDemandeRepository, MockDemandeRepository>();
                });
                builder.UseEnvironment("Test");
            });
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task DemandeControllerEtat() 
        {
            HttpResponseMessage responseMessage = await _client.GetAsync("/demande", TestContext.Current.CancellationToken);
            Assert.True(responseMessage.IsSuccessStatusCode);
        }
        [Fact]
        public async Task DemandeCreeSansCompteRedirigeVersLogin()
        {

            HttpResponseMessage response = await _client.GetAsync("/demande/creer");
            Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
        }
        // s'assurer que les demande crée sont bien afficher Dans /demande/demandes
        [Fact]
        public async Task DemandesAfficheBienDemande() 
        {
            // il ne sent rend pas faut un mock etudiant etc ...
            HttpResponseMessage response = await _client.GetAsync("/demande/demandes");
            Console.Write(response.Content);
            Console.Write(response.Content);
        }

    }
}
