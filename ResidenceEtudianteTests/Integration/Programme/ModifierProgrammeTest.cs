using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;
using System.Security.Claims;


/*
 * @author John Zuleta
 * Description: Tests Programme
 * J'ai utiliser l'AI pour ma docummenation des tests 
 */

namespace ResidenceEtudianteTests.Integration.ProgrammeTests
{
    public class ModifierProgrammeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IProgrammesRepository _programmesRepository = new MockProgrammesRepository();
        private ICampusRepository _campusRepository = new MockCampusRepository();

        private ClaimsPrincipal? _currentUser;
        private readonly ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync(
                "/Programme/Creer", TestContext.Current.CancellationToken);
            string body = await response.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            string token = await ObtenirToken();
            data["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(data);
        }

        public ModifierProgrammeTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IProgrammesRepository>(_programmesRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepository);
                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });
                builder.UseEnvironment("Test");
            });

            _client = _factory.CreateClient();
            _currentUser = _admin;
        }

        [Fact(DisplayName = "Modification du CampusId d'un programme met à jour la relation")]
        public async Task Modifier_RelationProgrammeCampus_MiseAJour()
        {
            _currentUser = _admin;

            ResidenceEtudiante.Models.Programme? programmeAvant = _programmesRepository.GetProgramme(1);
            Assert.NotNull(programmeAvant);
            Assert.Equal(1, programmeAvant.CampusId);

            HttpContent form = await GetForm(new Dictionary<string, string>
            {
                { "Id",       "1"               },
                { "Nom",      "Informatique"    },
                { "Code",     "420"             },
                { "CampusId", "2"               }
            });

            await _client.PostAsync("/Programme/Modifier", form, TestContext.Current.CancellationToken);

            ResidenceEtudiante.Models.Programme? programmeApres = _programmesRepository.GetProgramme(1);
            Assert.NotNull(programmeApres);
            Assert.Equal(2, programmeApres.CampusId);
        }
    }
}
