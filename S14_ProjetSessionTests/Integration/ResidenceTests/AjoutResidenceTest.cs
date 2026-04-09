//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using S14_ProjetSession.Data;
//using S14_ProjetSession.Models;
//using System.Net;
//using System.Security.Claims;

///*
// * @author Benoit
// * 
// * Description: Tests d'intégrations pour l'ajout d'une résidence.
// */
//namespace S14_ProjetSessionTests.Integration.ResidenceTests
//{
//    /*
//     * Note :
//     * Seul la configuration de la classe de test (injection des dépendances,
//     * configuration du WebApplicationFactory et de l’authentification simulée)
//     * ci-dessous a été réalisée avec l’aide de ChatGPT.
//     */
//    public class AjoutResidenceTest : IClassFixture<WebApplicationFactory<Program>>
//    {
//        // Factory permettant de créer un serveur de test ASP.NET
//        private readonly WebApplicationFactory<Program> _factory;

//        // Client HTTP utilisé pour envoyer des requêtes au serveur de test
//        private readonly HttpClient _client;

//        // Repository simulé pour les résidences (remplace la base de données)
//        private IResidenceRepository _residenceRepository = new MockResidenceRepository();

//        // Mocks des autres repositories
//        private Mock<ICampusRepository> _campusRepo = new Mock<ICampusRepository>();
//        private Mock<ICommoditeRepository> _commoditeRepo = new Mock<ICommoditeRepository>();

//        // Représente l'utilisateur actuellement connecté (simulation)
//        private ClaimsPrincipal? _utilisateurActuel;

//        // Utilisateur administrateur utilisé pour les tests
//        private ClaimsPrincipal _admin = AuthUtilities.CreerAdmin();

//        private async Task<string> ObtenirToken()
//        {
//            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");
//            string body = await response.Content.ReadAsStringAsync();
//            return Utils.GetToken(body);
//        }

//        private async Task<HttpContent> GetForm(Dictionary<string, string> data)
//        {
//            HttpContent form = new FormUrlEncodedContent(data);
//            form.Headers.Add("RequestVerificationToken", await ObtenirToken());
//            return form;
//        }

//        // Constructeur : configure l’environnement de test et les dépendances
//        public AjoutResidenceTest(WebApplicationFactory<Program> factory)
//        {
//            _campusRepo.Setup(c => c.GetAll()).Returns(new List<Campus>
//            {
//                new Campus { Id = 1, Nom = "Test Campus" }
//            });

//            _commoditeRepo.Setup(c => c.Commodites).Returns(new List<Commodite>
//            {
//                new Commodite { Id = 1, Nom = "Piscine" },
//                new Commodite { Id = 2, Nom = "Salle de sport" }
//            });

//            // Permet de récupérer une commodité par son ID
//            _commoditeRepo.Setup(c => c.GetCommodite(It.IsAny<int>()))
//                          .Returns((int id) => _commoditeRepo.Object.Commodites.FirstOrDefault(c => c.Id == id));

//            // Configuration du serveur de test
//            _factory = factory.WithWebHostBuilder(builder =>
//            {
//                builder.ConfigureTestServices(services =>
//                {
//                    // Injection des dépendances simulées
//                    services.AddSingleton<IResidenceRepository>(_residenceRepository);
//                    services.AddSingleton<ICampusRepository>(_campusRepo.Object);
//                    services.AddSingleton<ICommoditeRepository>(_commoditeRepo.Object);

//                    // Injection de l'utilisateur courant simulé
//                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _utilisateurActuel);

//                    // Configuration d’un système d’authentification de test
//                    services.AddAuthentication("TestAuth")
//                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
//                });

//                builder.UseSetting("environment", "Test");
//            });
//            _client = _factory.CreateClient();
//            _utilisateurActuel = _admin;
//        }

//        // Vérifie qu'un utilisateur non connecté ne peut pas accéder à la page
//        [Fact(DisplayName = "AjouterResidence refuse utilisateur non connecté")]
//        public async Task AjouterRefuseNonConnecte()
//        {
//            _utilisateurActuel = null;

//            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");

//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        // Vérifie qu'un administrateur peut accéder à la page
//        [Fact(DisplayName = "AjouterResidence accessible admin")]
//        public async Task AjouterAccessibleAdmin()
//        {
//            _utilisateurActuel = _admin;

//            HttpResponseMessage response = await _client.GetAsync("/Residence/AjouterResidence");

//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        // Vérifie qu'une création valide ajoute une résidence
//        [Fact(DisplayName = "Création valide redirige")]
//        public async Task CreerValide()
//        {
//            int initial = _residenceRepository.GetAll().Count;

//            Dictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "Nom", "Nouvelle" },
//                { "CampusId", "1" },
//                { "Adresse.AdresseString", "123 rue" },
//                { "Adresse.CodePostal", "J1J1J1" }
//            };

//            HttpContent form = await GetForm(data);

//            await _client.PostAsync("/Residence/Creer", form);

//            // Vérifie qu'une résidence a été ajoutée
//            Assert.Equal(initial + 1, _residenceRepository.GetAll().Count);
//        }

//        // Vérifie qu'une création invalide ne modifie pas les données
//        [Fact(DisplayName = "Création invalide ne fonctionne pas")]
//        public async Task CreerInvalide()
//        {
//            int initial = _residenceRepository.GetAll().Count;

//            // Données invalides (nom vide)
//            Dictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "Nom", "" }, 
//                { "CampusId", "1" }
//            };

//            HttpContent form = await GetForm(data);

//            await _client.PostAsync("/Residence/Creer", form);

//            // Vérifie qu'aucune résidence n'a été ajoutée
//            Assert.Equal(initial, _residenceRepository.GetAll().Count);
//        }
//    }
//}