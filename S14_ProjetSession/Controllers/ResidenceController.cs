using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

/*
 * @author Benoit
 * 
 * Description: Controller responsable pour la gestion des résidences.
 */
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

        /// <summary>
        /// Génère une liste de commodités,
        /// en tenant compte de leurs résidence.
        /// </summary>
        /// <param name="commoditesActuelles">
        /// Liste des commodités déjà associées à la résidence.
        /// </param>
        /// <returns>
        /// Une collection de ResidenceCommoditeViewModel représentant toutes les commodités,
        /// avec l'état "checked" et la description si elles existent déjà.
        /// </returns>
        private IEnumerable<ResidenceCommoditeViewModel> GetListeCommodites(List<CommoditeDescriptionViewModel>? commoditesActuelles)
        {
            return _commoditeRepository.Commodites.Select(c =>
            {
                // Cherche la commodité actuelle
                CommoditeDescriptionViewModel? commoditeActuelle = commoditesActuelles?.FirstOrDefault(x => x.Id == c.Id);

                // Crée un ViewModel pour cette commodité
                return new ResidenceCommoditeViewModel(
                    c,
                    commoditeActuelle?.IsChecked ?? false,
                    commoditeActuelle?.Description
                );
            });
        }

        /// <summary>
        /// Affiche une liste paginée des résidences avec possibilité de filtrage et de tri.
        /// </summary>
        /// <param name="id">Identifiant optionnel (non utilisé actuellement).</param>
        /// <param name="page">Numéro de la page à afficher (par défaut : 1).</param>
        /// <param name="disponible">Filtre sur la disponibilité des résidences.</param>
        /// <param name="nom">Filtre sur le nom de la résidence.</param>
        /// <param name="adresseLigne">Filtre sur l'adresse.</param>
        /// <param name="ville">Filtre sur la ville.</param>
        /// <param name="ascendant">Indique si le tri est ascendant (true) ou descendant (false).</param>
        /// <returns>
        /// Vue "Residences" contenant la liste paginée des résidences correspondant aux critères.
        /// </returns>
        [AllowAnonymous]
        public IActionResult Index(int? id, int page = 1, bool? disponible = null, string? nom = null, string? adresseLigne = null, string? ville = null, bool ascendant = true)
        {
            ViewData["Title"] = "Résidences";

            // Récupère les résidences selon les critères de filtrage
            List<Residence> residencesFiltrer =
                _residenceRepository.GetResidenceFiltrer(disponible, nom, adresseLigne, ville, ascendant)
                ?? new List<Residence>();

            int nbPage = 10; // Nombre d'éléments par page

            // Calcul du nombre total d'éléments et de pages
            int itemsTotal = residencesFiltrer.Count;
            int pagesTotal = (int)Math.Ceiling((double)itemsTotal / nbPage);

            // S'assure que la page est au minimum 1
            if (page < 1)
            {
                page = 1;
            }

            // S'assure qu'il y a au moins une page
            if (pagesTotal == 0)
            {
                pagesTotal = 1;
            }

            // empêche de dépasser le nombre total de pages
            if (page > pagesTotal)
            {
                page = pagesTotal;
            }

            // Pagination : sélectionne uniquement les éléments de la page courante
            List<Residence> residences = residencesFiltrer
                .Skip((page - 1) * nbPage) // Ignore les éléments des pages précédentes
                .Take(nbPage)  // Prend seulement les éléments de la page courante
                .ToList();

            // Passage des paramètres à la vue pour conserver les filtres et la pagination
            ViewBag.Disponible = disponible;
            ViewBag.Nom = nom;
            ViewBag.AdresseLigne = adresseLigne;
            ViewBag.Ville = ville;
            ViewBag.Ascendant = ascendant;
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = pagesTotal;

            return View("Residences", residences);
        }

        /// <summary>
        /// Affiche les détails d'une résidence spécifique.
        /// </summary>
        /// <param name="id">Identifiant de la résidence à afficher</param>
        /// <returns>
        /// Vue contenant les détails de la résidence si elle existe,
        /// sinon retourne NotFound() si l'identifiant n'est pas valide
        /// </returns>
        [AllowAnonymous]
        public IActionResult ResidenceDetails(int id)
        {
            Residence residence = _residenceRepository.GetById(id); 
            if (residence == null)
            {
                TempData["Erreur"] = "La résidence demandée n’existe pas ou a été supprimée.";
                return RedirectToAction("Index");
            }
               
            return View(residence);
        }

        /// <summary>
        /// Affiche le formulaire pour ajouter une nouvelle résidence.
        /// Accessible uniquement aux utilisateurs ayant la politique "AdminOuGestionnaire".
        /// </summary>
        /// <returns>
        /// La vue AjouterResidence.
        /// </returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult AjouterResidence()
        {
            ViewData["Title"] = "Ajout d'une résidence";

            // Envoie les commodités à la vue
            ViewBag.Commodites = GetListeCommodites(new List<CommoditeDescriptionViewModel>());

            // Initialisation du modèle avec valeurs par défaut
            Residence residence = new Residence
            {
                Province = "QC"
            };

            return View(residence);
        }

        /// <summary>
        /// Ajoute les commodités sélectionnées à une résidence.
        /// </summary>
        /// <param name="residence">La résidence à laquelle ajouter les commodités</param>
        /// <param name="commodites">Liste des commodités avec leur état sélectionné et description</param>
        /// <exception cref="Exception">Lancée si une commodité n'existe pas dans le repo</exception>
        private void AjoutCommoditesChoisies(Residence residence, List<CommoditeDescriptionViewModel> commodites)
        {
            // Vide les commodités existantes de la résidence
            residence.ResidenceCommodites.Clear();

            // Parcourt toutes les commodités sélectionnées par l'utilisateur
            foreach (CommoditeDescriptionViewModel c in commodites.Where(x => x.IsChecked))
            {
                // Récupère l'objet Commodite correspondant depuis le repo
                Commodite? commodite = _commoditeRepository.GetCommodite(c.Id);

                if (commodite != null)
                {
                    // Ajoute la commodité à la résidence avec la description fournie
                    residence.ResidenceCommodites.Add(new ResidenceCommodite
                    {
                        Commodite = commodite,
                        Residence = residence,
                        Description = c.Description
                    });
                }
                else
                {
                    // Si la commodité n'existe pas dans le repo
                    throw new Exception("Commodité invalide");
                }
            }
        }

        /// <summary>
        /// Traite la soumission du formulaire d'ajout d'une nouvelle résidence.
        /// Accessible uniquement aux utilisateurs ayant la politique "AdminOuGestionnaire".
        /// </summary>
        /// <param name="residence">
        /// Objet Residence contenant le nom, l'identifiant du campus et l'adresse saisie par l'utilisateur.
        /// </param>
        /// <param name="commodites">
        /// Liste des commodités sélectionnées par l'utilisateur pour cette résidence.
        /// </param>
        /// <returns>
        /// Redirige vers Index si la création réussit,
        /// sinon retourne la vue "AjouterResidence" avec les erreurs.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom, AdresseLigne, Ville, Province, CodePostal")] Residence residence,
            List<CommoditeDescriptionViewModel> commodites)
        {
            ViewData["Title"] = "Ajout d'une résidence";

            // Vérifie si le nom existe déjà
            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                ModelState.AddModelError("Nom", "Ce nom de résidence existe déjà.");
            }

            // Vérifie que l'adresse est complète
            if (string.IsNullOrWhiteSpace(residence.AdresseLigne) || string.IsNullOrWhiteSpace(residence.CodePostal))
            {
                ModelState.AddModelError("AdresseLigne", "Veuillez remplir toutes les informations d'adresse.");
            }

            ModelState.Remove("ResidenceCommodites");

            // Ajout des commodités si valide
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

            // Si erreurs, retourner la vue
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";

                ViewBag.Commodites = GetListeCommodites(commodites);

                return View("AjouterResidence", residence);
            }

            try
            {
                // Sauvegarde
                _residenceRepository.Creer(residence);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la création de la résidence." });
            }

            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été créée avec succès.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Affiche le formulaire de modification d'une résidence existante.
        /// Accessible uniquement aux utilisateurs ayant la politique "AdminOuGestionnaire".
        /// </summary>
        /// <param name="id">Identifiant de la résidence à modifier</param>
        /// <returns>
        /// Vue "ModifierResidence" avec les données de la résidence et les listes de campus et commodités.
        /// Retourne NotFound() si la résidence n'existe pas.
        /// </returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult ModifierResidence(int id)
        {
            // Récupère la résidence correspondant au ID
            Residence? residence = _residenceRepository.GetById(id);

            if (residence == null)
            {
                TempData["Erreur"] = "La résidence demandée est introuvable.";
                return RedirectToAction("Index");
            }

            // Récupère la liste des commodités déjà associées à la résidence
            List<CommoditeDescriptionViewModel> commoditesSelectionnees = residence?.ResidenceCommodites
                .Select(rc => new CommoditeDescriptionViewModel
                {
                    Id = rc.CommoditeId,
                    Description = rc.Description,
                    IsChecked = true // marque les commodités existantes comme True
                })
                .ToList()
                ?? new List<CommoditeDescriptionViewModel>(); // si la résidence n'a pas de commodités, crée une liste vide

            // Génère la liste complète des commodités, avec les sélections actuelles
            ViewBag.Commodites = GetListeCommodites(commoditesSelectionnees);

            ViewData["Title"] = "Modification de la résidence " + residence?.Nom;

            return View("ModifierResidence", residence);
        }

        /// <summary>
        /// Traite la soumission du formulaire de modification d'une résidence existante.
        /// Accessible uniquement aux utilisateurs ayant la politique "AdminOuGestionnaire".
        /// </summary>
        /// <param name="residence">
        /// Objet Residence contenant l'identifiant, le nom, le campus et l'adresse modifiés.
        /// </param>
        /// <param name="commodites">
        /// Liste des commodités sélectionnées pour cette résidence.
        /// </param>
        /// <returns>
        /// Redirige vers Index si la modification réussit,
        /// sinon retourne la vue "ModifierResidence" avec les erreurs de validation.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id, Nom, AdresseLigne, Ville, Province, CodePostal")] Residence residence,
            List<CommoditeDescriptionViewModel> commodites)
        {
            // Récupère la résidence existante
            Residence residenceDb = _residenceRepository.GetById(residence.Id);

            if (residenceDb == null)
            {
                TempData["Erreur"] = "La résidence demandée est introuvable.";
                return RedirectToAction("Index");
            }

            // Vérifie que le nom de la résidence n'est pas déjà utilisé par une autre résidence
            if (_residenceRepository.NomExiste(residence.Nom, residence.Id))
            {
                ModelState.AddModelError("Nom", "Ce nom de résidence existe déjà.");
            }

            // Vérifie que l'adresse est correctement remplie
            if (string.IsNullOrWhiteSpace(residence.AdresseLigne) || string.IsNullOrWhiteSpace(residence.CodePostal))
            {
                ModelState.AddModelError("AdresseLigne", "Veuillez remplir toutes les informations d'adresse.");
            }

            ModelState.Remove("ResidenceCommodites"); // Supprime l'état du modèle pour les commodités avant validation

            // Si le modèle est valide, applique les modifications
            if (ModelState.IsValid)
            {
                try
                {
                    residenceDb.Nom = residence.Nom;
                    residenceDb.AdresseLigne = residence.AdresseLigne;
                    residenceDb.Ville = residence.Ville;
                    residenceDb.Province = residence.Province;
                    residenceDb.CodePostal = residence.CodePostal;

                    AjoutCommoditesChoisies(residenceDb, commodites);
                }
                catch
                {
                    ModelState.AddModelError("ResidenceCommodites", "Certaines commodités sont invalides");
                }
            }

            // Si le modèle n'est pas valide, retourne le formulaire avec les erreurs
            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite.";
                ViewData["Title"] = "Modification de la résidence " + residence.Nom;
                ViewBag.Commodites = GetListeCommodites(commodites);

                return View("ModifierResidence", residenceDb);
            }
            try
            {
                // Sauvegarde les modifications dans la base de données
                _residenceRepository.Modifier(residenceDb);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la modification de la résidence." });
            }

            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été modifiée avec succès.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Supprime une résidence existante.
        /// Accessible uniquement aux utilisateurs ayant la politique "AdminUniquement".
        /// </summary>
        /// <param name="id">Identifiant de la résidence à supprimer</param>
        /// <returns>
        /// Redirige vers Index avec un message de succès ou d'erreur.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            // Récupère la résidence correspondant au ID
            Residence? residence = _residenceRepository.GetById(id);

            // Si la résidence n'existe pas
            if (residence == null)
            {
                TempData["Erreur"] = $"La résidence avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }

            try
            {
                // Supprime la résidence de la base de données
                _residenceRepository.Supprimer(residence);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la suppression de la résidence." });
            }

            TempData["Succes"] = $"La résidence {residence.Nom ?? ""} a été supprimé avec succès.";
            return RedirectToAction("Index");
        }
    }
}
