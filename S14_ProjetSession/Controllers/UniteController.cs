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
        /// Affiche la liste paginée des unités, avec filtres optionnels, 
        /// pour une résidence spécifique ou pour l’ensemble des résidences.
        /// </summary>
        /// <param name="id">
        /// Identifiant de la résidence. Si null ou invalide, toutes les unités sont affichées.
        /// </param>
        /// <param name="page">Numéro de la page courante (pagination).</param>
        /// <param name="disponible">Filtre sur la disponibilité des unités.</param>
        /// <param name="capacite">Filtre sur la capacité des unités.</param>
        /// <param name="numero">Filtre sur le numéro de l’unité.</param>
        /// <param name="ascendant">Indique si le tri est ascendant.</param>
        /// <param name="mobiliteReduite">Filtre sur l’accessibilité (mobilité réduite).</param>
        /// <returns>
        /// Vue "Unites" contenant la liste des unités filtrées et paginées.
        /// </returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int? id, int page = 1, bool? disponible = null, int? capacite = null, int? numero = null, bool? ascendant = true, bool? mobiliteReduite = null)
        {
            // Récupère toutes les résidences (pour affichage/filtre dans la vue)
            List<Residence> residences = _residenceRepository.GetAll();

            // Si un id de résidence est fourni, filtre par résidence
            List<Unite> totalUnites;
            if (id.HasValue && id.Value > 0)
            {
                // Récupère la résidence correspondante
                Residence residence = _residenceRepository.GetById(id.Value);

                // Si la résidence n'existe pas, retourne 404
                if (residence == null)
                {
                    return NotFound();
                }
                // Récupère toutes les unités de cette résidence
                totalUnites = _uniteRepository.GetByResidenceId(id.Value);
                ViewBag.ResidenceName = residence.Nom;  // Nom de la résidence pour affichage
            }
            else
            {
                // Sinon, prend toutes les unités
                totalUnites = _uniteRepository.GetAll();
                ViewBag.ResidenceName = "toutes les résidences";
            }

            // Récupère les unités filtrées (avec ou sans résidence)
            List<Unite> unitesFiltrer = _uniteRepository.GetFiltrerByResidenceId(
                id ?? 0, 
                disponible, capacite, numero, ascendant ?? true, mobiliteReduite);

            int nbPage = 10;

            // Calcul du total d'éléments et du nombre de pages
            int itemsTotal = unitesFiltrer.Count;
            int pagesTotal = (int)Math.Ceiling((double)itemsTotal / nbPage);

            // S'assure que la page est au minimum 1
            if (page < 1)
            {
                page = 1;
            }

            // S'assurer qu'il y a au moins une page
            if (pagesTotal == 0)
            {
                pagesTotal = 1;
            }

            // S'assure de ne pas dépasser le nombre total de pages
            if (page > pagesTotal)
            {
                page = pagesTotal;
            }

            // Pagination : sélection des unités de la page courante
            List<Unite> unites = unitesFiltrer
                .Skip((page - 1) * nbPage)
                .Take(nbPage)
                .ToList();

            // Données envoyées à la vue
            ViewBag.Residences = residences;
            ViewBag.ResidenceId = id;

            // Adresse de la résidence sélectionnée ou N/A
            ViewBag.Adresse = id.HasValue ? _residenceRepository.GetById(id.Value)?.AdresseComplete : "N/A";
            ViewBag.NombreTotal = totalUnites.Count;

            // Infos de pagination
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = pagesTotal;

            // Indique si on est en mode "toutes les unités"
            ViewBag.ModeToutesLesUnites = !id.HasValue || id.Value <= 0;

            // Conservation des filtres dans la vue
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
            try
            {
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
                        Numero = numeroSuivant
                    };

                    // Enregistre la nouvelle unité dans la DB
                    _uniteRepository.Creer(nouvelleUnite);
                }
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la création des unités." });
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

            try
            {
                // Met à jour l'unité dans la DB
                _uniteRepository.Modifier(unite);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la modification de l'unité'." });
            }

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

            try
            {
                // Supprime l'unité de la DB
                _uniteRepository.Supprimer(unite);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la suppression de l'unité." });
            }

            TempData["Succes"] = $"L'unité #{unite.Numero} a été supprimé avec succès.";
            return RedirectToAction("Index", new { id = unite.ResidenceId });
        }
    }
}
