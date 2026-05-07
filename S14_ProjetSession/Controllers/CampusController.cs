using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

/// <author>John Zuleta</author>
/// J'ai utilisé ChatGPT pour ma documentation
namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des campus.
    /// Permet de lister, créer, modifier et supprimer des campus.
    /// </summary>
    [Authorize]
    public class CampusController : Controller
    {
        private readonly ICampusRepository _repo;

        /// <summary>
        /// Constructeur du CampusController.
        /// </summary>
        /// <param name="repo">Le repository des campus pour les opérations CRUD.</param>
        public CampusController(ICampusRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Affiche la liste des campus triés par nom.
        /// </summary>
        /// <returns>Une vue avec la liste des campus ou une vue d'erreur si un problème survient.</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        // Utilisation du code de pagination fourni par le professeur
        public IActionResult Index(string ordre = "", int noPage = 1)
        {
            try
            {
                ViewData["OrdreActuel"] = ordre;
                ViewData["OrdreTri"] = ordre == "desc" ? "asc" : "desc";
                List<Campus> campus;
                int itemsParPage = 6;
                noPage = (noPage > 0 && noPage < int.MaxValue) ? noPage : 1;

                if (ordre == "desc")
                {
                    campus = _repo.Campus.OrderByDescending(c => c.Nom).ToList();
                }
                else
                {
                    campus = _repo.Campus.OrderBy(c => c.Nom).ToList();
                }

                return View(PaginatedList<Campus>.Create(campus, noPage, itemsParPage));
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors du chargement des campus." });
            }
        }

        /// <summary>
        /// Affiche le formulaire de création d'un campus.
        /// Accessible uniquement aux administrateurs ou gestionnaires.
        /// </summary>
        /// <returns>Une vue avec le formulaire de création.</returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer()
        {
            ViewData["Title"] = "Créer un campus";
            return View();
        }

        /// <summary>
        /// Traite la création d'un nouveau campus.
        /// </summary>
        /// <param name="campus">L'objet Campus contenant les informations saisies par l'utilisateur.</param>
        /// <returns>
        /// Redirige vers l'index en cas de succès, 
        /// ou renvoie la vue avec les erreurs de validation.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom,Abreviation")] Campus campus)
        {
            try
            {
                ViewData["Title"] = "Créer un campus";
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Creer(campus);
                TempData["Succes"] = $"Campus créé : {campus.Nom}";

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la création." });
            }
        }

        /// <summary>
        /// Affiche le formulaire de modification d'un campus existant.
        /// </summary>
        /// <param name="id">L'identifiant du campus à modifier.</param>
        /// <returns>
        /// Une vue avec les informations du campus ou une vue d'erreur si le campus est introuvable.
        /// </returns>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier(int id)
        {
            try
            {
                ViewData["Title"] = "Modifier un campus";
                Campus campus = _repo.GetById(id);

                if (campus == null)
                    return View("Erreur", new ErreurViewModel { StatusCode = 404, Message = "Campus introuvable." });

                return View(campus);
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur chargement." });
            }
        }

        /// <summary>
        /// Traite la modification d'un campus existant.
        /// </summary>
        /// <param name="campus">L'objet Campus avec les modifications effectuées par l'utilisateur.</param>
        /// <returns>
        /// Redirige vers l'index en cas de succès,
        /// ou renvoie la vue avec les erreurs de validation.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id,Nom,Abreviation")] Campus campus)
        {
            try
            {
                ViewData["Title"] = "Modifier un campus";
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Modifier(campus);
                TempData["Succes"] = $"Campus modifié : {campus.Nom}";

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = $"Erreur modification du campus {campus.Nom}." });
            }
        }

        /// <summary>
        /// Supprime un campus existant.
        /// </summary>
        /// <param name="id">L'identifiant du campus à supprimer.</param>
        /// <returns>
        /// Redirige vers l'index avec un message de succès,
        /// ou affiche un message d'erreur si le campus est introuvable ou si une erreur survient.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            try
            {
                Campus campus = _repo.GetById(id);

                if (campus == null)
                {
                    TempData["Erreur"] = $"Campus introuvable";
                    return RedirectToAction("Index");
                }

                _repo.Supprimer(campus);
                TempData["Succes"] = $"Campus supprimé : {campus.Nom}";

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = $"Erreur suppression du campus {id}." });
            }
        }
    }
}