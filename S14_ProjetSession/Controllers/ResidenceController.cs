using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;

namespace S14_ProjetSession.Controllers
{
    [Authorize]
    public class ResidenceController : Controller
    {

        private readonly IResidenceRepository _residenceRepository;
        private readonly ICampusRepository _campusRepository;
        private readonly ICommoditeRepository _commoditeRepository;

        public ResidenceController(IResidenceRepository residenceRepository, ICampusRepository campusRepository, ICommoditeRepository commoditeRepository)
        {
            _residenceRepository = residenceRepository;
            _campusRepository = campusRepository;
            _commoditeRepository = commoditeRepository;
        }

        private IEnumerable<ResidenceCommoditeViewModel> GetListeCommodites(List<CommoditeDescriptionViewModel>? commoditesActuelles)
        {
            return _commoditeRepository.Commodites.Select(c =>
            {
                var commodite = commoditesActuelles?.FirstOrDefault(x => x.Id == c.Id);

                return new ResidenceCommoditeViewModel(
                    c,
                    commodite?.IsChecked ?? false,
                    commodite?.Description
                );
            });
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Title"] = "Résidences";
            return View("Residences", _residenceRepository.GetAll());
        }

        [AllowAnonymous]
        public IActionResult ResidenceDetails(int id)
        {
            Residence residence = _residenceRepository.GetById(id); 
            if (residence == null) return NotFound();
            return View(residence);
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterResidence()
        {
            ViewData["Title"] = "Ajout d'une résidence";
            ViewBag.Commodites = GetListeCommodites(new List<CommoditeDescriptionViewModel>());

            ViewBag.CampusList = new SelectList(_campusRepository.GetAll(), "Id", "Nom");
            return View(new Residence{Adresse = new Adresse()});
        }

        private void AjoutCommoditesChoisies(Residence residence, List<CommoditeDescriptionViewModel> commodites)
        {
            residence.ResidenceCommodites.Clear();

            foreach (var c in commodites.Where(x => x.IsChecked))
            {
                var commodite = _commoditeRepository.GetCommodite(c.Id);

                if (commodite != null)
                {
                    residence.ResidenceCommodites.Add(new ResidenceCommodite
                    {
                        Commodite = commodite,
                        Residence = residence,
                        Description = c.Description
                    });
                }
                else
                {
                    throw new Exception("Commodité invalide");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom, CampusId, Adresse")] Residence residence, List<CommoditeDescriptionViewModel> commodites)
        {
            ViewData["Title"] = "Ajout d'une résidence";

            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                ModelState.AddModelError("Nom", "Ce nom de résidence existe déjà.");
            }

            if (residence.Adresse == null
                || string.IsNullOrWhiteSpace(residence.Adresse.AdresseString)
                || string.IsNullOrWhiteSpace(residence.Adresse.CodePostal))
            {
                ModelState.AddModelError("Adresse", "Veuillez remplir toutes les informations d'adresse.");
            }

            if (residence.CampusId <= 0)
            {
                ModelState.AddModelError("CampusId", "Veuillez sélectionner un campus.");
            }

            ModelState.Remove("ResidenceCommodites");

            if (ModelState.IsValid)
            {
                try
                {
                    AjoutCommoditesChoisies(residence, commodites);
                }
                catch
                {
                    ModelState.AddModelError("ResidenceCommodites", "Certaines commodités sont invalides");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Veuillez corriger les erreurs.";

                ViewBag.CampusList = new SelectList(_campusRepository.GetAll(), "Id", "Nom");
                ViewBag.Commodites = GetListeCommodites(commodites); 

                return View("AjouterResidence", residence);
            }
            _residenceRepository.Creer(residence);
            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été créée avec succès.";

            return RedirectToAction("Index");
        }


        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult ModifierResidence(int id)
        {
            Residence? residence = _residenceRepository.GetById(id);

            ViewBag.CampusList = new SelectList(_campusRepository.GetAll(), "Id", "Nom", residence?.CampusId);

            List<CommoditeDescriptionViewModel> commoditesSelectionnees = residence?.ResidenceCommodites
                .Select(rc => new CommoditeDescriptionViewModel
                {
                    Id = rc.CommoditeId,
                    Description = rc.Description,
                    IsChecked = true
                })
                .ToList()
                ?? new List<CommoditeDescriptionViewModel>();

            ViewBag.Commodites = GetListeCommodites(commoditesSelectionnees);

            ViewData["Title"] = "Modification de la résidence " + residence?.Nom;

            return residence is null ? NotFound() : View("ModifierResidence", residence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id, Nom, CampusId, Adresse")] Residence residence, List<CommoditeDescriptionViewModel> commodites)
        {
            Residence residenceDb = _residenceRepository.GetById(residence.Id);

            if (residenceDb == null)
            {
                return NotFound();
            }

            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                ModelState.AddModelError("Nom", "Ce nom de résidence existe déjà.");
            }

            if (residence.Adresse == null
                || string.IsNullOrWhiteSpace(residence.Adresse.AdresseString)
                || string.IsNullOrWhiteSpace(residence.Adresse.CodePostal))
            {
                ModelState.AddModelError("Adresse", "Veuillez remplir toutes les informations d'adresse.");
            }

            ModelState.Remove("ResidenceCommodites");

            if (ModelState.IsValid)
            {
                try
                {
                    residenceDb.Nom = residence.Nom;
                    residenceDb.CampusId = residence.CampusId;
                    residenceDb.Adresse = residence.Adresse;

                    AjoutCommoditesChoisies(residenceDb, commodites);
                }
                catch
                {
                    ModelState.AddModelError("ResidenceCommodites", "Certaines commodités sont invalides");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Modification de la résidence " + residence.Nom;
                ViewBag.CampusList = new SelectList(_campusRepository.GetAll(), "Id", "Nom", residence.CampusId);
                ViewBag.Commodites = GetListeCommodites(commodites);

                return View("ModifierResidence", residence);
            }

            _residenceRepository.Modifier(residenceDb);

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
