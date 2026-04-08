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
        [HttpGet("Unite/Residence/{id}")]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int id)
        {
            // Récupère la résidence à partir de son ID
            Residence residence = _residenceRepository.GetById(id);

            // Récupère la liste des unités disponibles associées à la résidence
            List<Unite> unitesDisponibles = _uniteRepository.GetByResidenceId(id);

            // Vérifie si la résidence existe et passe les informations de la résidence à la vue via ViewBag
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
        public IActionResult Creer([Bind("Numero, Capacite, ResidenceId, AdapteePourMobiliteReduite")] Unite unite)
        {
            unite.PlacesOccupees = 0;

            // Récupère la résidence associée à l'unité
            Residence residence = _residenceRepository.GetById(unite.ResidenceId);

            // Vérifie si une unité avec le même numéro existe déjà dans la résidence
            if (_uniteRepository.UniteExiste(unite.Numero, unite.ResidenceId))
            {
                TempData["Erreur"] = "Ce numéro d'unité existe déjà dans cette résidence.";
                ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

                return View("AjouterUnite", unite);
            }

            // Vérifie si les données du modèle sont valides
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Ajout d'une unité à la résidence " + residence.Nom;

                return View("AjouterUnite", unite);
            }

            // Enregistre la nouvelle unité dans la DB
            _uniteRepository.Creer(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été créé avec succès.";
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
                return NotFound();
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
            // Vérifie si une autre unité avec le même numéro existe déjà dans la résidence
            if (_uniteRepository.UniteExiste(unite.Numero, unite.ResidenceId, unite.Id))
            {
                TempData["Erreur"] = "Ce numéro d'unité existe déjà dans cette résidence.";
                ViewData["Title"] = "Modification de l'unité #" + unite.Numero;

                return View("ModifierUnite", unite);
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
            if (unite == null)
            {
                TempData["Erreur"] = $"L'unité avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }

            // Supprime l'unité de la DB
            _uniteRepository.Supprimer(unite);
            TempData["Succes"] = $"L'unité #{unite.Numero} a été supprimé avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }
    }
}
