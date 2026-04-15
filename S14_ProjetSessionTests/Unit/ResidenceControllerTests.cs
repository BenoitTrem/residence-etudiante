using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using S14_ProjetSession.Controllers;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;

/*
 * @author Benoit
 * 
 * Description: Tests Unitaires pour le controller Residence.
 */
namespace S14_ProjetSessionTests.Unit
{
    public class ResidenceControllerTests
    {
        private ResidenceController _controller;
        private Mock<IResidenceRepository> _residenceRepoMock;
        private Mock<ICampusRepository> _campusRepoMock;
        private Mock<ICommoditeRepository> _commoditeRepoMock;

        public ResidenceControllerTests()
        {
            _residenceRepoMock = new Mock<IResidenceRepository>();
            _campusRepoMock = new Mock<ICampusRepository>();
            _commoditeRepoMock = new Mock<ICommoditeRepository>();

            List<Residence> residences = new List<Residence>();

            _residenceRepoMock.Setup(r => r.GetAll()).Returns(residences);
            _residenceRepoMock.Setup(r => r.GetById(It.IsAny<int>()))
                 .Returns(new Residence
                 {
                     Id = 1,
                     Nom = "Test",
                     AdresseLigne = "Test",
                     Ville = "Gatineau",
                     Province = "QC",
                     CodePostal = "J1J1J1",
                     ResidenceCommodites = new List<ResidenceCommodite>()
                 });

            _campusRepoMock.Setup(c => c.Campus).Returns(new List<Campus>());
            _commoditeRepoMock.Setup(c => c.Commodites).Returns(new List<Commodite>());

            _controller = new ResidenceController(
                _residenceRepoMock.Object,
                _campusRepoMock.Object,
                _commoditeRepoMock.Object
            );

            Mock<ITempDataProvider> tempDataProvider = new Mock<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider.Object);
        }

        // Vérifie que l'action Index retourne la bonne vue
        [Fact(DisplayName = "Index retourne la vue Residences")]
        public void IndexRetourneVueResidences()
        {
            ViewResult resultat = _controller.Index() as ViewResult;
            Assert.Equal("Residences", resultat.ViewName);
        }

        // Vérifie que le titre de la page est correctement défini
        [Fact(DisplayName = "Index met le bon titre")]
        public void IndexMetBonTitre()
        {
            ViewResult resultat = _controller.Index() as ViewResult;
            Assert.Equal("Résidences", resultat.ViewData["Title"]);
        }

        // Vérifie que la création redirige vers Index si les données sont valides
        [Fact(DisplayName = "Creer redirige vers Index si valide")]
        public void CreerRedirigeIndex()
        {
            Residence residence = new Residence
            {
                Nom = "Test",

                AdresseLigne = "Test",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J1J1J1"
            };

            RedirectToActionResult resultat =
                _controller.Creer(residence, new List<CommoditeDescriptionViewModel>()) as RedirectToActionResult;

            Assert.Equal("Index", resultat.ActionName);
        }

        // Vérifie que le repo est appelé lors d'une création valide
        [Fact(DisplayName = "Creer appelle le repository si valide")]
        public void CreerAppelleRepository()
        {
            Residence residence = new Residence
            {
                Nom = "Test",

                AdresseLigne = "Test",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J1J1J1"
            };

            _controller.Creer(residence, new List<CommoditeDescriptionViewModel>());

            _residenceRepoMock.Verify(r => r.Creer(It.IsAny<Residence>()), Times.Once);
        }

        // Vérifie que la création échoue si le nom existe déjà
        [Fact(DisplayName = "Creer n'appelle pas repository si nom existe déjà")]
        public void CreerNomExiste()
        {
            Residence residence = new Residence
            {
                Nom = "Test",
                AdresseLigne = "Test street",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J1J1J1"
            };

            // Simule qu'un nom existe déjà
            _residenceRepoMock
                .Setup(r => r.NomExiste("Test", 0))
                .Returns(true);

            _controller.Creer(residence, new List<CommoditeDescriptionViewModel>());

            // Vérifie que la méthode Creer n'est jamais appelée
            _residenceRepoMock.Verify(r => r.Creer(It.IsAny<Residence>()), Times.Never);
        }

        // Vérifie que la modification redirige vers Index si valide
        [Fact(DisplayName = "Modifier redirige vers Index si valide")]
        public void ModifierPostRedirige()
        {
            Residence residence = new Residence
            {
                Id = 1,
                Nom = "Test",
                AdresseLigne = "Test street",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J1J1J1"
            };

            RedirectToActionResult resultat = _controller.Modifier(residence, new List<CommoditeDescriptionViewModel>()) as RedirectToActionResult;

            Assert.Equal("Index", resultat.ActionName);
        }

        // Vérifie que le repo est appelé lors d'une modification
        [Fact(DisplayName = "Modifier appelle repository")]
        public void ModifierAppelleRepository()
        {
            Residence residence = new Residence
            {
                Id = 1,
                Nom = "Test",
                AdresseLigne = "Test",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J1J1J1"
            };

            _controller.Modifier(residence, new List<CommoditeDescriptionViewModel>());

            _residenceRepoMock.Verify(r => r.Modifier(It.IsAny<Residence>()), Times.Once);
        }

        // Vérifie que la suppression appelle le repository
        [Fact(DisplayName = "Supprimer appelle repository")]
        public void SupprimerAppelleRepository()
        {
            _controller.Supprimer(1);

            _residenceRepoMock.Verify(r => r.Supprimer(It.IsAny<Residence>()), Times.Once);
        }
    }
}