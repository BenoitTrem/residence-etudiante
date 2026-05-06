using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Models;
using S14_ProjetSession.Data;

namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Contrôleur responsable de la gestion des programmes (CRUD).
    /// </summary>
    /// <author>John Zuleta</author>
    [Authorize]
    public class ProgrammeController : Controller
    {
        private readonly IProgrammesRepository _programmesRepository;
        private readonly ICampusRepository _campusRepository;

        /// <summary>
        /// Constructeur du contrôleur ProgrammeController.
        /// </summary>
        public ProgrammeController(
            IProgrammesRepository programmesRepository,
            ICampusRepository campusRepository)
        {
            _programmesRepository = programmesRepository;
            _campusRepository = campusRepository;
        }

        /// <summary>
        /// Affiche la liste des programmes.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index()
        {
            try
            {
                List<Programme> programmes = _programmesRepository.Programmes
                    .OrderBy(p => p.Nom)
                    .ToList();

                return View(programmes);
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors du chargement des programmes" });
            }
        }

        /// <summary>
        /// Affiche le formulaire de création.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer()
        {
            try
            {
                ViewData["Title"] = "Créer un programme";
                ViewBag.Campus = _campusRepository.Campus;
                return View();
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors du formulaire" });
            }
        }

        /// <summary>
        /// Crée un programme.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Creer([Bind("Nom,Code,CampusId")] Programme programme)
        {
            try
            {
                ViewData["Title"] = "Créer un programme";
                if (!ModelState.IsValid)
                {
                    ViewBag.Campus = _campusRepository.Campus;
                    return View(programme);
                }

                programme.Campus = _campusRepository.GetById(programme.CampusId);

                _programmesRepository.Creer(programme);

                TempData["Succes"] = "Programme créé avec succès";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la création" });
            }
        }

        /// <summary>
        /// Affiche le formulaire de modification.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier(int id)
        {
            try
            {
                ViewData["Title"] = "Modifier un programme";
                Programme programme = _programmesRepository.GetProgramme(id);

                if (programme == null)
                    return View("Erreur", new ErreurViewModel { StatusCode = 404, Message = "Programme introuvable" });

                ViewBag.Campus = _campusRepository.Campus;

                return View(programme);
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors du chargement" });
            }
        }

        /// <summary>
        /// Modifie un programme.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Modifier([Bind("Id,Nom,Code,CampusId")] Programme programme)
        {
            try
            {
                ViewData["Title"] = "Modifier un programme";
                if (!ModelState.IsValid)
                {
                    ViewBag.Campus = _campusRepository.Campus;
                    return View(programme);
                }

                programme.Campus = _campusRepository.GetById(programme.CampusId);

                _programmesRepository.Modifier(programme);

                TempData["Succes"] = "Programme modifié";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la modification" });
            }
        }

        /// <summary>
        /// Supprime un programme.
        /// </summary>
        [Authorize(Policy = "AdminUniquement")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Supprimer(int id)
        {
            try
            {
                Programme programme = _programmesRepository.GetProgramme(id);

                if (programme == null)
                    return View("Erreur", new ErreurViewModel { StatusCode = 404, Message = "Programme introuvable" });

                _programmesRepository.Supprimer(programme);

                TempData["Succes"] = "Programme supprimé";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Erreur", new ErreurViewModel { StatusCode = 500, Message = "Erreur lors de la suppression" });
            }
        }
    }
}