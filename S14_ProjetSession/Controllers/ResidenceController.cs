using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Controllers
{
    public class ResidenceController : Controller
    {

        private readonly IResidenceRepository _residenceRepository;

        public ResidenceController(IResidenceRepository residenceRepository)
        {
            _residenceRepository = residenceRepository;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Résidences";
            return View("Residences", _residenceRepository.GetAll());
        }

        public IActionResult AjouterResidence()
        {
            ViewData["Title"] = "Ajouter une résidence";
            return View(new Residence());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creer([Bind("Nom, Adresse")] Residence residence)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajouter une résidence";

                return View("AjouterResidence", residence);
            }

            _residenceRepository.Creer(residence);
            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été créé avec succès.";
            return RedirectToAction("Index");
        }

        public IActionResult Modifier(int id)
        {
            Residence residence = _residenceRepository.GetById(id);

            if (residence == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Modifier une résidence";
            ViewBag.Titre = "Modifier la résidence "+ residence.Nom ;
            return View("ModifierResidence", residence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modifier([Bind("Id, Nom, Adresse")] Residence residence)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Modifier un étudiant";

                return View("ModifierResidence", residence);
            }

            _residenceRepository.Modifier(residence);
            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été modifié avec succès.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
