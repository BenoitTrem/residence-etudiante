using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

/// <author>John Zuleta</author>

namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Gestion des campus (CRUD complet avec rôles).
    /// </summary>
    [Authorize]
    public class CampusController : Controller
    {
        private readonly ICampusRepository _repo;

        public CampusController(ICampusRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Policy = "AdminOuUtilisateur")]
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Create([Bind("Nom,Abreviation")] Campus campus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Creer(campus);
                TempData["Succes"] = "Campus créé ✔️";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Erreur(500, "Erreur lors de la création.");
            }
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Edit(int id)
        {
            try
            {
                var campus = _repo.GetCampus(id);

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
        public IActionResult Edit([Bind("Id,Nom,Abreviation")] Campus campus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(campus);

                _repo.Modifier(campus);
                TempData["Succes"] = "Campus modifié ✔️";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Erreur(500, "Erreur modification.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminUniquement")]
        public IActionResult Delete(int id)
        {
            try
            {
                var campus = _repo.GetCampus(id);

                if (campus == null)
                {
                    TempData["Erreur"] = "Campus introuvable";
                    return RedirectToAction(nameof(Index));
                }

                _repo.Supprimer(campus);
                TempData["Succes"] = "Campus supprimé ✔️";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Erreur(500, "Erreur suppression.");
            }
        }

        private IActionResult Erreur(int code, string message)
        {
            return View("Error", new ErreurViewModel
            {
                StatusCode = code,
                Message = message,
                RequestId = HttpContext.TraceIdentifier
            });
        }
    }
}