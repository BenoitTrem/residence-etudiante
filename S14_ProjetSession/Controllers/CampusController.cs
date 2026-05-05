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
        public IActionResult Index()
        {
            try
            {
                List<Campus> campus = _repo.Campus.OrderBy(c => c.Nom).ToList();
                return View(campus);
            }
            catch
            {
                return Erreur(500, "Erreur lors du chargement des campus.");
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
                return Erreur(500, "Erreur lors de la création.");
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
                    return Erreur(404, "Campus introuvable.");

                return View(campus);
            }
            catch
            {
                return Erreur(500, "Erreur chargement.");
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
                return Erreur(500, $"Erreur modification du campus {campus.Nom}.");
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
                return Erreur(500, $"Erreur suppression du campus {id}.");
            }
        }

       

        /// <summary>
        /// Méthode privée pour gérer les erreurs et afficher une vue dédiée.
        /// </summary>
        /// <param name="code">Le code HTTP de l'erreur (ex: 404, 500).</param>
        /// <param name="message">Le message d'erreur à afficher.</param>
        /// <returns>Une vue d'erreur contenant les détails du problème.</returns>
        private IActionResult Erreur(int code, string message)
        {
            return View("Erreur", new ErreurViewModel
            {
                StatusCode = code,
                Message = message,
                
            });
        }
    }
}