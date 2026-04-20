using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

/*
 * @author Benoit
 * 
 * Description: Controller responsable pour la gestion des unités.
 */
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

        /// <summary>
        /// Affiche la liste des unités disponibles pour une résidence donnée.
        /// </summary>
        /// <param name="id">Identifiant de la résidence</param>
        /// <returns>Vue Unites avec la liste des unités disponibles de la résidence</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int? id, int page = 1, bool? disponible = null, int? capacite = null, int? numero = null, bool? ascendant = true, bool? mobiliteReduite = null)
        {
            List<Residence> residences = _residenceRepository.GetAll(); 

            List<Unite> totalUnites;
            if (id.HasValue && id.Value > 0)
            {
                Residence residence = _residenceRepository.GetById(id.Value);
                if (residence == null)
                {
                    return NotFound();
                }
                totalUnites = _uniteRepository.GetByResidenceId(id.Value);
                ViewBag.ResidenceName = residence.Nom;
            }
            else
            {
                totalUnites = _uniteRepository.GetAll();
                ViewBag.ResidenceName = "Toutes les résidences";
            }

            List<Unite> unitesFiltrer = _uniteRepository.GetFiltrerByResidenceId(
                id ?? 0, 
                disponible, capacite, numero, ascendant ?? true, mobiliteReduite);

            // Pagination
            int nbPage = 10;
            List<Unite> unites = unitesFiltrer
                .Skip((page - 1) * nbPage)
                .Take(nbPage)
                .ToList();

            ViewBag.Residences = residences;
            ViewBag.ResidenceId = id;
            ViewBag.Adresse = id.HasValue ? _residenceRepository.GetById(id.Value)?.AdresseComplete : "N/A";
            ViewBag.NombreTotal = totalUnites.Count;
            ViewBag.NombreUnites = totalUnites.Count(u => u.EstDisponible);
            ViewBag.TotalPlacesDisponibles = totalUnites.Sum(u => u.PlacesDisponibles);
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)unitesFiltrer.Count / nbPage);
            ViewBag.ModeToutesLesUnites = !id.HasValue || id.Value <= 0;

            ViewBag.Disponible = disponible;
            ViewBag.Capacite = capacite;
            ViewBag.Numero = numero;
            ViewBag.MobiliteReduite = mobiliteReduite;
            ViewBag.Ascendant = ascendant;

            ViewData["Title"] = "Unités de " + ViewBag.ResidenceName;

            return View("Unites", unites);
        }

        /// <summary>
        /// Affiche le formulaire pour ajouter une nouvelle unité à une résidence.
        /// </summary>
        /// <param name="residenceId">Identifiant de la résidence</param>
        /// <returns>Vue AjouteUnite avec un objet Unite initialisé pour la création</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterUnite(int? residenceId)
        {
            List<Residence> residences = _residenceRepository.GetAll();

            ViewBag.Residences = residences;
            ViewBag.ResidenceId = residenceId ?? 0;

            if (residenceId.HasValue && residenceId.Value > 0)
            {
                Residence residence = _residenceRepository.GetById(residenceId.Value);
                ViewData["Title"] = "Ajout des unités";
            }
            else
            {
                ViewData["Title"] = "Ajout des unités";
            }

            Unite unite = new Unite
            {
                ResidenceId = residenceId ?? 0,
                PlacesOccupees = 0
            };

            return View(unite);
        }

        // <summary>
        /// Traite la création d'une nouvelle unité pour une résidence.
        /// </summary>
        /// <param name="unite">Objet Unite contenant les données saisies</param>
        /// <returns>Redirige vers la liste des unités ou retourne la vue en cas d'erreur</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Capacite, ResidenceId, AdapteePourMobiliteReduite")] Unite unite, int nombreUnites)
        {
            // Récupère la résidence associée à l'unité
            Residence residence = _residenceRepository.GetById(unite.ResidenceId);

            if (nombreUnites < 1)
            {
                ModelState.AddModelError("nombreUnites", "Vous devez ajouter au moins 1 unité.");
            }

            if (nombreUnites > 50)
            {
                ModelState.AddModelError("nombreUnites", "Maximum 50 unités à la fois.");
            }

            if (unite.ResidenceId <= 0)
            {
                ModelState.AddModelError("ResidenceId", "Vous devez sélectionner une résidence.");
            }

            // Vérifie si les données du modèle sont valides
            if (!ModelState.IsValid)
            {
                List<Residence> residences = _residenceRepository.GetAll();
                ViewBag.Residences = residences;
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajout des unités";

                return View("AjouterUnite", unite);
            }

            Random random = new Random();
            // Création d'une nouvelle unité pour chaque itération
            for (int i = 0; i < nombreUnites; i++)
            {
                int numeroSuivant = _uniteRepository
                    .GetByResidenceId(unite.ResidenceId)
                    .Select(u => u.Numero ?? 0)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                Unite nouvelleUnite = new Unite
                {
                    Capacite = unite.Capacite,
                    AdapteePourMobiliteReduite = unite.AdapteePourMobiliteReduite,
                    ResidenceId = unite.ResidenceId,
                    PlacesOccupees = 0,
                    Numero = numeroSuivant
                };

                // Enregistre la nouvelle unité dans la DB
                _uniteRepository.Creer(nouvelleUnite);
            }
            TempData["Succes"] = $"{nombreUnites} unité(s) ont été créées avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }

        /// <summary>
        /// Affiche le formulaire de modification d'une unité existante.
        /// </summary>
        /// <param name="id">Identifiant de l'unité à modifier</param>
        /// <returns>Vue ModifierUnite avec les informations de l'unité ou une erreur</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult ModifierUnite(int id)
        {
            // Récupère l'unité à partir de son ID
            Unite unite = _uniteRepository.GetById(id);

            // Vérifie si l'unité existe
            if (unite == null)
            {
                TempData["Erreur"] = "L'unité demandée n'existe pas ou a été supprimée.";
                return RedirectToAction("Index");
            }

            ViewBag.Residences = _residenceRepository.GetAll();
            ViewBag.ResidenceActuelle = _residenceRepository.GetById(unite.ResidenceId)?.Nom;

            ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

            return View("ModifierUnite", unite);
        }

        /// <summary>
        /// Traite la modification d'une unité existante.
        /// </summary>
        /// <param name="unite">Objet Unite contenant les données modifiées</param>
        /// <returns>Redirige vers la liste des unités ou retourne la vue en cas d'erreur</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id, Numero, Capacite, ResidenceId, AdapteePourMobiliteReduite")] Unite unite)
        {

            if (unite.Numero == null)
            {
                ModelState.AddModelError("Numero", "Le numéro est obligatoire.");
            }

            if (unite.ResidenceId <= 0)
            {
                ModelState.AddModelError("ResidenceId", "Vous devez sélectionner une résidence.");
            }

            // Vérifie si une autre unité avec le même numéro existe déjà dans la résidence
            if (_uniteRepository.UniteExiste(unite.Numero, unite.ResidenceId, unite.Id))
            {
                ModelState.AddModelError("Numero", "Ce numéro d'unité existe déjà dans cette résidence.");
            }

            // Vérifie si les données du modèle sont valides
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

                ViewBag.Residences = _residenceRepository.GetAll();

                return View("ModifierUnite", unite);
            }

            // Met à jour l'unité dans la DB
            _uniteRepository.Modifier(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été modifiée avec succès.";

            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }

        /// <summary>
        /// Supprime une unité existante.
        /// </summary>
        /// <param name="id">Identifiant de l'unité à supprimer</param>
        /// <returns>Redirige vers la liste des unités avec un message de succès ou d'erreur</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            // Récupère l'unité à partir de son ID
            Unite? unite =  _uniteRepository.GetById(id);

            // Vérifie si l'unité existe
            if (unite == null)
            {
                TempData["Erreur"] = $"L'unité avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index", "Residence");
            }

            // Supprime l'unité de la DB
            _uniteRepository.Supprimer(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été supprimé avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }
    }
}
