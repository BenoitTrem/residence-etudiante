using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;



/// J'ai utiliser l'AI pour documenter mes méthodes (chatgpt)
/// <author>John Zuleta</author>

namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Gestion des campus 
    /// </summary>
    [Authorize]
    public class CampusController : Controller
    {
        private readonly ICampusRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager; 

        public CampusController(ICampusRepository repo, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _repo = repo;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
                var campus = _repo.Campus.OrderBy(c => c.Nom).ToList();
                return View(campus);
            }
            catch
            {
                return Erreur(500, "Erreur lors du chargement des campus.");
            }
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom,Abreviation")] Campus campus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Creer(campus);
                TempData["Succes"] = "Campus créé";

                return RedirectToAction("Index");
            }
            catch
            {
                return Erreur(500, "Erreur lors de la création.");
            }
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier(int id)
        {
            try
            {
                var campus = _repo.GetById(id);

                if (campus == null)
                    return Erreur(404, "Campus introuvable.");

                return View(campus);
            }
            catch
            {
                return Erreur(500, "Erreur chargement.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id,Nom,Abreviation")] Campus campus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Modifier(campus);
                TempData["Succes"] = "Campus modifié";

                return RedirectToAction("Index");
            }
            catch
            {
                return Erreur(500, "Erreur modification.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Supprimer(int id)
        {
            try
            {
                var campus = _repo.GetById(id);

                if (campus == null)
                {
                    TempData["Erreur"] = "Campus introuvable";
                    return RedirectToAction("Index");
                }

                _repo.Supprimer(campus);
                TempData["Succes"] = "Campus supprimé";

                return RedirectToAction("Index");
            }
            catch
            {
                return Erreur(500, "Erreur suppression.");
            }
        }

        private IActionResult Erreur(int code, string message)
        {
            return View("Erreur", new ErreurViewModel
            {
                StatusCode = code,
                Message = message,
                RequestId = HttpContext.TraceIdentifier
            });
        }
    }
}