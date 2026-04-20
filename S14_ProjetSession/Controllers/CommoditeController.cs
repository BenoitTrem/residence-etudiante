using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

/*
 * @author Benoit
 * 
 * Description: Controller responsable pour la gestion des commodités.
 */
namespace S14_ProjetSession.Controllers
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
        /// Affiche la liste de toutes les commodités.
        /// </summary>
        /// <returns>Vue Commodites avec la liste complète des commodités</returns>
        public IActionResult Index(int? id, int page = 1, string? nom = null, bool ascendant = true)
        {
            List<Commodite> commoditesFiltrer =
               _commoditeRepository.GetCommoditeFiltrer(nom, ascendant)
               ?? new List<Commodite>();

            // Pagination
            int nbPage = 10;
            List<Commodite> commodites = commoditesFiltrer
                .Skip((page - 1) * nbPage)
                .Take(nbPage)
                .ToList();

            ViewBag.Nom = nom;
            ViewBag.Ascendant = ascendant;
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)commoditesFiltrer.Count / nbPage);
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
            // Ajoute la nouvelle commodité a la DB
            _commoditeRepository.Ajouter(commodite);
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

            if (ModelState.IsValid)
            {
                TempData["Erreur"] = $"Une erreur s'est produite avec la modification de la commodité {commodite.Nom}.";
                return RedirectToAction("Index");
            }

            _commoditeRepository.Modifier(commodite);
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
        public IActionResult Supprimer(int id)
        {
            // Récupère la commodité correspondant au ID
            Commodite? commodite = _commoditeRepository.GetCommodite(id);
            if (commodite == null)
            {
                TempData["Erreur"] = $"La commodité avec l'ID {id} n'existe pas.";
                return RedirectToAction("Index");
            }
           
            _commoditeRepository.Supprimer(commodite);
            TempData["Succes"] = $"La commodité {commodite.Nom} a été supprimée avec succès.";
            return RedirectToAction("Index");
        }
    }
}