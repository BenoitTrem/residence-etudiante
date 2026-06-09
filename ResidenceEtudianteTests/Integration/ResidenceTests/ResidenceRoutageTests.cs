using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ResidenceEtudiante.Data;
using System.Net;

/*
 * @author Benoit
 * 
 * Description: Tests de routage pour les résidences.
 * vérifie que les URLs retournent les bons codes HTTP et que les vues affichent les données attendues,
 */
namespace ResidenceEtudianteTests.Integration.ResidenceTests
{

    public class ResidenceRoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ResidenceRoutageTests(WebApplicationFactory<Program> factory)
        {
            var mockRepo = new MockResidenceRepository();

           _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IResidenceRepository>(mockRepo);
                });
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();
        }

        // Vérifie que certaines routes valides retournent une réponse OK
        [InlineData("/Residence")]
        [InlineData("/Residence/ResidenceDetails/1")]
        [Theory]
        public async Task RouteExiste(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            string body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(string.IsNullOrEmpty(body)); // Vérifie que la réponse contient du contenu
        }

        // Vérifie que les routes invalides retournent une erreur 404
        [InlineData("/Residence/InvalidRoute")]
        [Theory]
        public async Task RouteNexistePas(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            
            // Vérifie que la route n'existe pas (code 404)
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Vérifie qu'une propriété de l'objet s'affiche dans la vue
        [Fact(DisplayName = "Vue affiche le nom de la résidence")]
        public async Task VueAffichePropriete()
        {
            HttpResponseMessage response = await _client.GetAsync("/Residence/ResidenceDetails/1");
            string body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("R&#xE9;sidence Maple", body);
        }
    }
}