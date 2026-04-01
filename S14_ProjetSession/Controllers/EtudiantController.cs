using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using Microsoft.VisualBasic;


namespace S14_ProjetSession.Controllers
{

    [Authorize]
    public class EtudiantController : Controller
    {

        private readonly IEtudiantRepository _etudiantRepository;

        private readonly IGenresRepository _genresRepository;

        private readonly IProgrammesRepository _programmesRepository;

        private readonly UserManager<ApplicationUser> _userManager;


        public EtudiantController(IEtudiantRepository etudiantRepository, IGenresRepository genresRepository, IProgrammesRepository programmesRepository, UserManager<ApplicationUser> userManager)
        {
            _etudiantRepository = etudiantRepository;
            _genresRepository = genresRepository;
            _programmesRepository = programmesRepository;
            _userManager = userManager;
        }




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
