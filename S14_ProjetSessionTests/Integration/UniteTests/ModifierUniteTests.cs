using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

/*
 * @author Benoit
 * 
 * Description: Tests d'intégrations pour la modification d'une unité.
 */
namespace S14_ProjetSessionTests.Integration.UniteTests
{
    public class ModifierUniteTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IUniteRepository _uniteRepository = new MockUniteRepository();
        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

        private ClaimsPrincipal? _utilisateurActuel;
        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/Unite/AjouterUnite");
            string body = await response.Content.ReadAsStringAsync();
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            HttpContent form = new FormUrlEncodedContent(data);
            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
            return form;
        }

        public ModifierUniteTests(WebApplicationFactory<Program> factory)
        {
            _campusRepo.Setup(c => c.Campus).Returns(new List<Campus>
            {
                new Campus { Id = 1, Nom = "Test Campus" }
            });

            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>());
            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>())).Returns((Commodite?)null);

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IUniteRepository>(_uniteRepository);
                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);
                    services.AddSingleton<IEtudiantRepository, MockEtudiantRepository>();

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });

                builder.UseSetting("environment", "Test");
            });

            _client = _factory.CreateClient();
            _utilisateurActuel = _admin;
        }


        // Vérifie qu'un admin peut modifier une unité
        [Fact(DisplayName = "Modification valide d'une unité")]
        public async Task ModifierValide()
        {
            _utilisateurActuel = _admin;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Numero", "999" },
                { "Capacite", "3" },
                { "ResidenceId", "1" },
                { "AdapteePourMobiliteReduite", "false" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Modifier", form);

            Unite updated = _uniteRepository.GetById(1);
            Assert.Equal(999, updated.Numero);
            Assert.Equal(3, updated.Capacite);
        }

        // Vérifie qu'un admin ne peut modifier l'unité si le numéro existe déjà
        [Fact(DisplayName = "Modification invalide refusée si numéro déjà utilisé")]
        public async Task ModifierInvalideNumeroExistant()
        {
            _utilisateurActuel = _admin;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Numero", "102" },
                { "Capacite", "2" },
                { "ResidenceId", "1" },
                { "AdapteePourMobiliteReduite", "false" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Modifier", form);

            Unite updated = _uniteRepository.GetById(1);
            Assert.NotEqual(102, updated.Numero);
        }

        // Vérifie qu'un utilisateur non admin est refusé lors de la modification
        [Fact(DisplayName = "Modification refusée si non admin")]
        public async Task ModifierRefuseNonAdmin()
        {
            _utilisateurActuel = null;

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Numero", "888" },
                { "Capacite", "3" },
                { "ResidenceId", "1" },
                { "AdapteePourMobiliteReduite", "false" }
            };

            HttpContent form = await GetForm(data);
            await _client.PostAsync("/Unite/Modifier", form);

            Unite updated = _uniteRepository.GetById(1);
            Assert.NotEqual(888, updated.Numero);
        }
    }
}
