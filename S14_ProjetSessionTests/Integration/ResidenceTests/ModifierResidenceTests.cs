using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using S14_ProjetSession.Controllers;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;
using System.Security.Claims;

/*
 * @author Benoit
 * 
 * Description: Tests d'intégrations pour la modification d'une résidence.
 */
namespace S14_ProjetSessionTests.Integration.ResidenceTests
{
    /*
     * Note :
     * Seul la configuration de la classe de test (injection des dépendances,
     * configuration du WebApplicationFactory et de l’authentification simulée)
     * ci-dessous a été réalisée avec l’aide de ChatGPT.
     */
    public class ModifierResidenceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private IResidenceRepository _residenceRepository = new MockResidenceRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

        private ClaimsPrincipal? _utilisateurActuel;
        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

        private async Task<string> ObtenirToken()
        {
            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");
            string body = await response.Content.ReadAsStringAsync();
            return Utils.GetToken(body);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
        {
            HttpContent form = new FormUrlEncodedContent(data);
            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
            return form;
        }

        public ModifierResidenceTests(WebApplicationFactory<Program> factory)
        {
            _campusRepo.Setup(c => c.Campus).Returns(new List<Campus>
            {
                new Campus { Id = 1, Nom = "Test Campus" }
            });

            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>
            {
                new Commodite { Id = 1, Nom = "Piscine" },
                new Commodite { Id = 2, Nom = "Salle de sport" }
            });

            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>()))
                          .Returns((int id) => _commoditeRepo.Object.Commodites.FirstOrDefault(c => c.Id == id));

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });

                builder.UseSetting("environment", "Test");
            });
            _client = _factory.CreateClient();
            _utilisateurActuel = _admin;
        }

        // Vérifie qu'une modification valide met à jour les informations
        [Fact(DisplayName = "Modification valide redirige")]
        public async Task ModifierValide()
        {
            _utilisateurActuel = _admin;

            // Récupère une résidence existante
            Residence residence = _residenceRepository.GetById(1);
            Assert.NotNull(residence);

            // Données modifiées
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", residence.Id.ToString() },
                { "Nom", "Résidence Modifiée" },

                { "AdresseLigne", "123 Rue Modifiée" },
                { "Ville", residence.Ville },
                { "Province", residence.Province },
                { "CodePostal", "H0H0H0" }
            };

            HttpContent form = await GetForm(data);

            await _client.PostAsync("/Residence/Modifier", form);

            // Vérifie que les données ont bien été mises à jour
            Residence updatedResidence = _residenceRepository.GetById(1);
            Assert.Equal("Résidence Modifiée", updatedResidence.Nom);
            Assert.Equal("123 Rue Modifiée", updatedResidence.AdresseLigne);
            Assert.Equal("H0H0H0", updatedResidence.CodePostal);
        }

   
        [Fact(DisplayName = "Modification refusée si non admin")]
        public async Task ModificationRefuseNonAdmin()
        {
            _utilisateurActuel = null;

            int initial = _residenceRepository.GetById(1).Nom.GetHashCode();

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Nom", "TentativeHack" },
                { "AdresseLigne", "123 rue" },
                { "Ville", "Ville" },
                { "Province", "QC" },
                { "CodePostal", "H0H0H0" }
            };

            HttpContent form = await GetForm(data);

            await _client.PostAsync("/Residence/Modifier", form);

            Assert.NotEqual("TentativeHack", _residenceRepository.GetById(1).Nom);
        }
    }
}