using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
namespace ResidenceEtudianteTests.Integration
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
                    services.AddScoped<IEtudiantRepository, MockEtudiantRepository>();
                });
                builder.UseEnvironment("Test");
            });
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        // felix
        [Fact]
        public async Task DemandeCreeSansCompteRedirigeVersLogin()
        {

            HttpResponseMessage response = await _client.GetAsync("/demande/creer");
            Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
        }

    }
}
