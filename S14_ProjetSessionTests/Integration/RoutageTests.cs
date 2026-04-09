using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using System.Net;
using System.Security.Claims;

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

            HttpResponseMessage response = await _client.GetAsync("/demande/creer", TestContext.Current.CancellationToken);
            Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task DemandeCreeAvecCompteEstAccessible()
        {
            ClaimsPrincipal? currentUser = AuthUtilities.CreerEtudiant();

            WebApplicationFactory<Program> factoryAuthentifiee = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDemandeRepository>(new MockDemandeRepository());
                    services.AddSingleton<IEtudiantRepository>(new MockEtudiantRepository());
                    services.AddSingleton<ISemestreRepository>(new MockSemestreRepository());
                    services.AddSingleton<IGenresRepository>(new MockGenreRepository());

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", _ => { });
                });
                builder.UseEnvironment("Test");
            });

            HttpClient clientAuthentifie = factoryAuthentifiee.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            HttpResponseMessage response = await clientAuthentifie.GetAsync("/demande/creer", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
