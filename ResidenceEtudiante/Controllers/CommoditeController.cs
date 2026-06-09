using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;

/*
 * @author Benoit
 * 
 * Description: Controller responsable pour la gestion des commodités.
 */
namespace ResidenceEtudiante.Controllers
{
    [Authorize(Policy = "AdminOuGestionnaire")]
    public class CommoditeController : Controller
    {
        private readonly ICommoditeRepository _commoditeRepository;

        public CommoditeController(ICommoditeRepository commoditeRepository)
        {
            _commoditeRepository = commoditeRepository;
        }

        /// <summary>
        /// Affiche la liste paginée des commodités avec filtre et tri.
        /// </summary>
        /// <returns>Vue "Commodites" avec la liste des commodités filtrées et paginées</returns>
        public IActionResult Index(int? id, int page = 1, string? nom = null, bool ascendant = true)
        {
            // Récupère les commodités selon les critères de filtrage et de tri
            List<Commodite> commoditesFiltrer =
               _commoditeRepository.GetCommoditeFiltrer(nom, ascendant)
               ?? new List<Commodite>();

            int nbPage = 10; // Nombre d'éléments par page

            // Calcul du nombre total d'éléments et de pages
            int itemsTotal = commoditesFiltrer.Count;
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

            // Pagination : sélection des commodités de la page courante
            List<Commodite> commodites = commoditesFiltrer
                .Skip((page - 1) * nbPage)
                .Take(nbPage)
                .ToList();

            // Passage des paramètres à la vue
            ViewBag.Nom = nom;
            ViewBag.Ascendant = ascendant;

            // Informations de pagination
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = pagesTotal;

            ViewData["Title"] = "Commodités";
            return View("Commodites", commodites);
        }

        /// <summary>
        /// Traite la soumission du formulaire d'ajout d'une nouvelle commodité.
        /// </summary>
        /// <param name="commodite">
        /// Objet Commodite contenant le nom de la nouvelle commodité.
        /// </param>
        /// <returns>
        /// Redirige vers Index après création.  
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creer([Bind("Nom")] Commodite commodite)
        {
            // Vérifie si le nom de la commodité existe déjà dans le repo.
            if (_commoditeRepository.NomExiste(commodite.Nom))
            {
                TempData["Erreur"] = "Ce nom de commodité existe déjà.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = "Une erreur s'est produite avec l'ajout de la commodité.";
                return RedirectToAction("Index");
            }
            try
            {
                // Ajoute la nouvelle commodité a la DB
                _commoditeRepository.Ajouter(commodite);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la création de la commodité." });
            }

            TempData["Succes"] = $"La commodité {commodite.Nom} a été ajoutée avec succès.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Traite la soumission du formulaire de modification d'une commodité existante.
        /// </summary>
        /// <param name="commodite">
        /// Objet Commodite contenant l'identifiant et le nouveau nom de la commodité.
        /// </param>
        /// <returns>
        /// Redirige vers Index après traitement.  
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modifier([Bind("Id, Nom")] Commodite commodite)
        {
                // Vérifie si le nouveau nom existe déjà pour une autre commodité
            if (_commoditeRepository.NomExiste(commodite.Nom, commodite.Id))
            {
                TempData["Erreur"] = "Ce nom de commodité existe déjà.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Erreur"] = $"Une erreur s'est produite avec la modification de la commodité {commodite.Nom}.";
                return RedirectToAction("Index");
            }
            try
            {
                _commoditeRepository.Modifier(commodite);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la modification de la commodité." });
            }

            TempData["Succes"] = $"La commodité {commodite.Nom} a été modifiée avec succès.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Traite la suppression d'une commodité existante.
        /// </summary>
        /// <param name="Id">
        /// Identifiant de la commodité à supprimer.
        /// </param>
        /// <returns>
        /// Redirige vers Index avec un message de succès ou d'erreur.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            // Récupère la commodité correspondant au ID
            Commodite? commodite = _commoditeRepository.GetCommodite(id);
            if (commodite == null)
            {
                TempData["Erreur"] = $"La commodité avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }
            try
            {
                _commoditeRepository.Supprimer(commodite);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la suppression de la commodité." });
            }

            TempData["Succes"] = $"La commodité {commodite.Nom} a été supprimée avec succès.";
            return RedirectToAction("Index");
        }
    }
}