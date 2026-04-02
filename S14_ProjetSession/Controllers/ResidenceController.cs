using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Controllers
{
    [Authorize]
    public class ResidenceController : Controller
    {

        private readonly IResidenceRepository _residenceRepository;
        private readonly ICampusRepository _campusRepository;

        public ResidenceController(IResidenceRepository residenceRepository, ICampusRepository campusRepository)
        {
            _residenceRepository = residenceRepository;
            _campusRepository = campusRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Title"] = "Résidences";
            return View("Residences", _residenceRepository.GetAll());
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterResidence()
        {
            ViewData["Title"] = "Ajout d'une résidence";
            var campusList = _campusRepository.Campus; 

            ViewBag.CampusList = new SelectList(campusList, "Id", "Nom");
            return View(new Residence{Adresse = new Adresse()});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom,CampusId,Adresse")] Residence residence)
        {
            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                TempData["Erreur"] = "Ce nom de résidence existe déjà.";
                ViewData["Title"] = "Ajout d'une résidence";
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom");
                return View("AjouterResidence", residence);
            }

            if (residence.Adresse == null
                || string.IsNullOrWhiteSpace(residence.Adresse.AdresseString)
                || string.IsNullOrWhiteSpace(residence.Adresse.CodePostal))
            {
                TempData["Erreur"] = "Veuillez remplir toutes les informations d'adresse.";
                ViewData["Title"] = "Ajout d'une résidence";
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom");
                return View("AjouterResidence", residence);
            }

            if (residence.CampusId <= 0)
            {
                TempData["Erreur"] = "Veuillez sélectionner un campus.";
                ViewData["Title"] = "Ajout d'une résidence";
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom");
                return View("AjouterResidence", residence);
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajout d'une résidence";
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom");
                return View("AjouterResidence", residence);
            }

            _residenceRepository.Creer(residence);
            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été créée avec succès.";
            return RedirectToAction("Index");
        }


        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier(int id)
        {
            Residence residence = _residenceRepository.GetById(id);

            if (residence == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Modification de la résidence " + residence.Nom;

            ViewBag.CampusList = new SelectList(
                _campusRepository.Campus,
                "Id",
                "Nom",
                residence.CampusId 
            );

            return View("ModifierResidence", residence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id,Nom,CampusId,Adresse")] Residence residence)
        {
            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                TempData["Erreur"] = "Ce nom de résidence existe déjà.";
                ViewData["Title"] = "Modification de la résidence " + residence.Nom;
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom", residence.CampusId);

                return View("ModifierResidence", residence);
            }

            if (residence.Adresse == null
                || string.IsNullOrWhiteSpace(residence.Adresse.AdresseString)
                || string.IsNullOrWhiteSpace(residence.Adresse.CodePostal))
            {
                TempData["Erreur"] = "Veuillez remplir toutes les informations d'adresse.";
                ViewData["Title"] = "Modification de la résidence " + residence.Nom;
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom", residence.CampusId);

                return View("ModifierResidence", residence);
            }
          
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Modification de la résidence " + residence.Nom;
                ViewBag.CampusList = new SelectList(_campusRepository.Campus, "Id", "Nom", residence.CampusId);

                return View("ModifierResidence", residence);
            }

            _residenceRepository.Modifier(residence);

            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été modifiée avec succès.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            Residence? residence = _residenceRepository.GetById(id);    

            if (residence == null)
            {
                TempData["Erreur"] = $"La résidence avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }

            _residenceRepository.Supprimer(residence);
            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été supprimé avec succès.";
            return RedirectToAction("Index");
        }
    }
}
