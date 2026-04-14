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
        public IActionResult Index()
        {
            ViewData["Title"] = "Commodités";
            return View("Commodites", _commoditeRepository.GetAll());
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
            if (ModelState.IsValid)
            {
                // Vérifie si le nom de la commodité existe déjà dans le repo.
                if (_commoditeRepository.NomExiste(commodite.Nom))
                {
                    TempData["Erreur"] = "Ce nom de commodité existe déjà.";
                }
                else
                {
                    // Sinon, ajoute la nouvelle commodité a la DB
                    _commoditeRepository.Ajouter(commodite);
                    TempData["Succes"] = $"La commodité {commodite.Nom} a été ajoutée avec succès.";
                }
            }
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
            if (ModelState.IsValid)
            {
                // Vérifie si le nouveau nom existe déjà pour une autre commodité
                if (_commoditeRepository.NomExiste(commodite.Nom, commodite.Id))
                {
                    TempData["Erreur"] = "Ce nom de commodité existe déjà.";
                }
                else
                {
                    // Sinon, modifie la commodité
                    _commoditeRepository.Modifier(commodite);
                    TempData["Succes"] = $"La commodité {commodite.Nom} a été modifiée avec succès.";
                }
            }
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
        public IActionResult Supprimer(int Id)
        {
            // Récupère la commodité correspondant au ID
            Commodite? commodite = _commoditeRepository.GetCommodite(Id);
            if (commodite == null)
            {
                TempData["Erreur"] = "La commodité n'existe pas.";
            }
            else
            {
                // Sinon, supprime la commodité de la DB
                _commoditeRepository.Supprimer(commodite);
                TempData["Succes"] = $"La commodité {commodite.Nom} a été supprimée avec succès.";
            }
            return RedirectToAction("Index");
        }
    }
}