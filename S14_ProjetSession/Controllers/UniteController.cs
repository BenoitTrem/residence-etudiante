using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Policy;

namespace S14_ProjetSession.Controllers
{

    [Authorize]
    public class UniteController : Controller
    {
        private readonly IUniteRepository _uniteRepository;
        private readonly IResidenceRepository _residenceRepository;

        public UniteController(IUniteRepository uniteRepository, IResidenceRepository residenceRepository)
        {
            _uniteRepository = uniteRepository;
            _residenceRepository = residenceRepository;
        }
 
        [HttpGet("Unite/Residence/{id}")]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int id)
        {
            Residence residence = _residenceRepository.GetById(id);

            List<Unite> unitesDisponibles = _uniteRepository.GetByResidenceId(id);

            if (residence != null)
            {
                ViewBag.ResidenceId = residence.Id;
                ViewBag.Adresse = residence.AdresseString;
                ViewBag.Campus = residence.Campus?.Nom ?? "N/A";
                ViewBag.NombreTotal = residence.TotalUnites; 
                ViewBag.NombreUnites = residence.UnitesDisponibles;
                ViewBag.TotalPlacesDisponibles = residence.TotalPlacesDisponibles;
                ViewData["Title"] = "Unités de la résidence " + residence.Nom;
            }

            return View("Unites", unitesDisponibles);
        }


        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterUnite(int residenceId)
        {
            Residence residence = _residenceRepository.GetById(residenceId);

            ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

            Unite unite = new Unite
            {
                ResidenceId = residenceId,
                PlacesOccupees = 0
            };

            return View(unite);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Numero, Capacite, ResidenceId, AdapteePourMobiliteReduite")] Unite unite)
        {
            unite.PlacesOccupees = 0;

            Residence residence = _residenceRepository.GetById(unite.ResidenceId);

            if (_uniteRepository.UniteExiste(unite.Numero, unite.ResidenceId))
            {
                TempData["Erreur"] = "Ce numéro d'unité existe déjà dans cette résidence.";
                ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

                return View("AjouterUnite", unite);
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

                return View("AjouterUnite", unite);
            }

            _uniteRepository.Creer(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été créé avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult ModifierUnite(int id)
        {
            Unite unite = _uniteRepository.GetById(id);

            if (unite == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

            return View("ModifierUnite", unite);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id, Numero, Capacite, ResidenceId, AdapteePourMobiliteReduite")] Unite unite)
        {

            if (_uniteRepository.UniteExiste(unite.Numero, unite.ResidenceId, unite.Id))
            {
                TempData["Erreur"] = "Ce numéro d'unité existe déjà dans cette résidence.";
                ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

                return View("ModifierUnite", unite);
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

                return View("ModifierUnite", unite);
            }

            _uniteRepository.Modifier(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été modifiée avec succès.";

            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            Unite? unite =  _uniteRepository.GetById(id);

            if (unite == null)
            {
                TempData["Erreur"] = $"L'unité avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }

            _uniteRepository.Supprimer(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été supprimé avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }
    }
}
