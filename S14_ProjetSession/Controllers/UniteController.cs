using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

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
        public IActionResult Index(int id, int page = 1)
        {
            // Récupère la résidence
            Residence residence = _residenceRepository.GetById(id);

            if (residence == null)
            {
                return NotFound();
            }

            // Récupère toutes les unités
            List<Unite> totalUnites = _uniteRepository.GetByResidenceId(id);

            // Pagination
            int nbPage = 10;

            List<Unite> unites = totalUnites
                .Skip((page - 1) * nbPage)
                .Take(nbPage)
                .ToList();

            // Infos pour la vue
            ViewBag.ResidenceId = residence.Id;
            ViewBag.Adresse = residence.AdresseComplete;

            ViewBag.NombreTotal = totalUnites.Count;
            ViewBag.NombreUnites = totalUnites.Count(u => u.EstDisponible);
            ViewBag.TotalPlacesDisponibles = totalUnites.Sum(u => u.PlacesDisponibles);

            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUnites.Count / nbPage);

            ViewData["Title"] = "Unités de la résidence " + residence.Nom;

            return View("Unites", unites);
        }

        /// <summary>
        /// Affiche le formulaire pour ajouter une nouvelle unité à une résidence.
        /// </summary>
        /// <param name="residenceId">Identifiant de la résidence</param>
        /// <returns>Vue AjouteUnite avec un objet Unite initialisé pour la création</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterUnite(int residenceId)
        {
            // Récupère la résidence à partir de son ID
            Residence residence = _residenceRepository.GetById(residenceId);

            ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

            // Initialise une nouvelle unité avec les valeurs par défaut
            Unite unite = new Unite
            {
                ResidenceId = residenceId,
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

            // Vérifie si les données du modèle sont valides
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

                return View("AjouterUnite", unite);
            }

            Random random = new Random();

            // Création d'une nouvelle unité pour chaque itération
            for (int i = 0; i < nombreUnites; i++)
            {
                Unite nouvelleUnite = new Unite
                {
                    Capacite = unite.Capacite,
                    AdapteePourMobiliteReduite = unite.AdapteePourMobiliteReduite,
                    ResidenceId = unite.ResidenceId,
                    PlacesOccupees = 0,
                    Numero = random.Next(1, 9999)   // Génération d'un numéro aléatoire pour l'unité
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
                return RedirectToAction("Index", new { id = id });
            }

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
            if (unite != null)
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
