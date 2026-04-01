using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using Microsoft.VisualBasic;

/// J'ai utiliser l'AI pour documenter mes méthodes
/// <author>John Zuleta</author>

namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Contrôleur responsable de la gestion des étudiants (CRUD et profil).
    /// </summary>
    /// <author>John Zuleta</author>
    [Authorize]
    public class EtudiantController : Controller
    {

        private readonly IEtudiantRepository _etudiantRepository;

        private readonly IGenresRepository _genresRepository;

        private readonly IProgrammesRepository _programmesRepository;

        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructeur du contrôleur EtudiantController.
        /// </summary>
        /// <param name="etudiantRepository">Repository des étudiants</param>
        /// <param name="genresRepository">Repository des genres</param>
        /// <param name="programmesRepository">Repository des programmes</param>
        /// <param name="userManager">Gestionnaire des utilisateurs</param>
        /// <author>John Zuleta</author>
        public EtudiantController(IEtudiantRepository etudiantRepository, IGenresRepository genresRepository, IProgrammesRepository programmesRepository, UserManager<ApplicationUser> userManager)
        {
            _etudiantRepository = etudiantRepository;
            _genresRepository = genresRepository;
            _programmesRepository = programmesRepository;
            _userManager = userManager;
        }



        /// <summary>
        /// Affiche la liste des étudiants triés par nom et prénom.
        /// </summary>
        /// <returns>Vue contenant la liste des étudiants</returns>
        /// <author>John Zuleta</author>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public ViewResult Index()
        {
            ViewData["Title"] = "Etudiant";

            var etudiants = _etudiantRepository.Etudiants
                                                    .OrderBy(e => e.Nom)
                                                    .ThenBy(e => e.Prenom)
                                                    .ToList();

            return View("Etudiants", etudiants);
        }



        /// <summary>
        /// Affiche le formulaire de création d’un étudiant.
        /// </summary>
        /// <returns>Vue du formulaire de création</returns>
        /// <author>John Zuleta</author>
        [Authorize(Policy = "AdminOuUtilisateur")]
        public async Task<IActionResult> Creer()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

     
            if (User.IsInRole("Utilisateur"))
            {
                Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(user.Id);

                if (etudiant != null)
                {
                    return Forbid();
                }
            }

            ViewBag.Genres = _genresRepository.Genres;
            ViewBag.Programmes = _programmesRepository.Programmes;
            ViewBag.EmailPerso = user.Email;

            return View();
        }

        /// <summary>
        /// Traite la création d’un étudiant.
        /// </summary>
        /// <param name="etudiant">Données de l’étudiant à créer</param>
        /// <returns>Redirection vers la liste ou réaffichage du formulaire en cas d’erreur</returns>
        /// <author>John Zuleta</author>
        [HttpPost]
        [Authorize(Policy = "AdminOuUtilisateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer([Bind("Nom,Prenom,DateNaissance,GenreId,ProgrammeId,noEtudiant,noAdmission,MobiliteReduite,AdressePermanente,Telephone,CourrielInstitutionnel,CourrielPersonnel")] Etudiant etudiant)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _genresRepository.Genres;
                ViewBag.Programmes = _programmesRepository.Programmes;
                return View(etudiant);
            }

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

        
       
            if (User.IsInRole("Utilisateur"))
            {
                Etudiant? existe = await _etudiantRepository.GetByUserIdAsync(user.Id);
                if (existe != null)
                {
                    return Forbid();
                }

            }

         
            Genre genre = _genresRepository.GetGenre(etudiant.GenreId);
            Programme programme = _programmesRepository.GetProgramme(etudiant.ProgrammeId);

            etudiant.Genre = genre;
            etudiant.Programme = programme;
            etudiant.ApplicationUserId = user.Id;
            etudiant.User = user;

            _etudiantRepository.Creer(etudiant);

            TempData["Succes"] = "L'étudiant " + etudiant.Nom + " a été créé";

            return RedirectToAction("Index");
        }


        /// <summary>
        /// Affiche le formulaire de modification d’un étudiant.
        /// </summary>
        /// <param name="id">Identifiant de l’étudiant</param>
        /// <returns>Vue de modification ou redirection si non trouvé</returns>
        /// <author>John Zuleta</author>
        public async Task<IActionResult> Modifier(int id)
        {

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            Etudiant? etudiant = _etudiantRepository.GetEtudiant(id);
            if (etudiant == null)
            {
                TempData.Add("Erreur", "L'étudiant " + id + " n'existe pas");
                return RedirectToAction("Index");

            }
       
            if (!User.IsInRole("Admin") &&
               !User.IsInRole("Gestionnaire") &&
               etudiant.ApplicationUserId != user.Id)
            {
                return Forbid();
            }


            ViewBag.Genres = _genresRepository.Genres;
            ViewBag.Programmes = _programmesRepository.Programmes;

            return View(etudiant);
        }



        /// <summary>
        /// Traite la modification d’un étudiant.
        /// </summary>
        /// <param name="etudiant">Données mises à jour de l’étudiant</param>
        /// <returns>Redirection ou réaffichage du formulaire</returns>
        /// <author>John Zuleta</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier([Bind("Id,Nom,Prenom,DateNaissance,GenreId,ProgrammeId,noEtudiant,noAdmission,MobiliteReduite,AdressePermanente,Telephone,CourrielInstitutionnel,CourrielPersonnel,ApplicationUserId")] Etudiant etudiant)
        {

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (!User.IsInRole("Admin") &&
               !User.IsInRole("Gestionnaire") &&
               etudiant.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                Genre genre = _genresRepository.GetGenre(etudiant.GenreId);
                etudiant.Genre = genre;

                Programme programme = _programmesRepository.GetProgramme(etudiant.ProgrammeId);
                etudiant.Programme = programme;

                _etudiantRepository.Modifier(etudiant);
                TempData.Add("Succes", "L'étudiant " + etudiant.Nom + " a été modifié");
                return RedirectToAction("Index");
            }

            ViewBag.Genres = _genresRepository.Genres;
            ViewBag.Programmes = _programmesRepository.Programmes;
            return View(etudiant);
        }

        /// <summary>
        /// Supprime un étudiant.
        /// </summary>
        /// <param name="Id">Identifiant de l’étudiant à supprimer</param>
        /// <returns>Redirection vers la liste des étudiants</returns>
        /// <author>John Zuleta</author>
        [Authorize(Policy = "AdminUniquement")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Supprimer([Bind("Id")] int Id)
        {
            Etudiant etudiant = _etudiantRepository.GetEtudiant(Id);
            if (etudiant is null)
            {
                TempData.Add("Erreur", "L'étudiant " + Id + " n'existe pas");
                return RedirectToAction("Index");
            }
            else
            {
                _etudiantRepository.Supprimer(etudiant);
                TempData.Add("Succes", "L'étudiant " + etudiant.Prenom + " "+ etudiant.Nom + " a été supprimé");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Affiche le profil de l’étudiant connecté.
        /// </summary>
        /// <returns>Vue du profil de l’étudiant</returns>
        /// <author>Benoit Tremblay</author>
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Profil()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(user.Id);
            if (etudiant is null)
            {
                return Forbid();
            }

            return View(etudiant); 
        }
    }
}
