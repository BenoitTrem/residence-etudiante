using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

/// J'ai utiliser l'AI pour documenter mes méthodes (chatgpt)
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
        private readonly ICampusRepository _campusRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructeur du contrôleur EtudiantController.
        /// </summary>
        /// <param name="etudiantRepository">Repository des étudiants</param>
        /// <param name="genresRepository">Repository des genres</param>
        /// <param name="programmesRepository">Repository des programmes</param>
        /// <param name="userManager">Gestionnaire des utilisateurs</param>
        /// <author>John Zuleta</author>
        public EtudiantController(
            IEtudiantRepository etudiantRepository,
            IGenresRepository genresRepository,
            IProgrammesRepository programmesRepository,
            ICampusRepository campusRepository,
            UserManager<ApplicationUser> userManager)
        {
            _etudiantRepository = etudiantRepository;
            _genresRepository = genresRepository;
            _programmesRepository = programmesRepository;
            _campusRepository = campusRepository;
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
            try
            {
               

                var etudiants = _etudiantRepository.Etudiants
                    .OrderBy(e => e.Nom)
                    .ThenBy(e => e.Prenom)
                    .ToList();

                return View("Etudiants", etudiants);
            }
            catch (Exception)
            {
                return (ViewResult)Erreur(500, "Erreur lors du chargement des étudiants");
            }
        }

        /// <summary>
        /// Affiche le formulaire de création d’un étudiant.
        /// </summary>
        /// <returns>Vue du formulaire</returns>
        /// <author>John Zuleta</author>
        [Authorize(Policy = "AdminOuUtilisateur")]
        public async Task<IActionResult> Creer()
        {
            try
            {
                ViewData["Title"] = "Créer un étudiant";
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                if (User.IsInRole("Utilisateur"))
                {
                    var etudiant = await _etudiantRepository.GetByUserIdAsync(user.Id);
                    if (etudiant != null)
                        return Forbid();
                }

                ViewBag.Genres = _genresRepository.Genres;
                ViewBag.Programmes = _programmesRepository.Programmes;
                ViewBag.Campus = _campusRepository.Campus;
                ViewBag.EmailPerso = user.Email;

                return View();
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors de l'affichage du formulaire");
            }
        }

        /// <summary>
        /// Traite la création d’un étudiant.
        /// </summary>
        /// <param name="etudiant">Données de l’étudiant</param>
        /// <returns>Redirection ou vue avec erreurs</returns>
        /// <author>John Zuleta</author>
        [HttpPost]
        [Authorize(Policy = "AdminOuUtilisateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer(
            [Bind("Nom,Prenom,DateNaissance,GenreId,ProgrammeId,CampusId,noEtudiant,noAdmission,MobiliteReduite,AdressePermanente,Telephone,CourrielInstitutionnel,CourrielPersonnel")] Etudiant etudiant)
        {
            try
            {
                ViewData["Title"] = "Créer un étudiant";
                if (!ModelState.IsValid)
                {
                    ViewBag.Genres = _genresRepository.Genres;
                    ViewBag.Programmes = _programmesRepository.Programmes;
                    ViewBag.Campus = _campusRepository.Campus;
                    return View(etudiant);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                if (User.IsInRole("Utilisateur"))
                {
                    var existe = await _etudiantRepository.GetByUserIdAsync(user.Id);
                    if (existe != null)
                        return Forbid();
                }

                etudiant.Genre = _genresRepository.GetGenre(etudiant.GenreId);
                etudiant.Programme = _programmesRepository.GetProgramme(etudiant.ProgrammeId);
                etudiant.Campus = _campusRepository.GetById(etudiant.CampusId);
                etudiant.ApplicationUserId = user.Id;
                etudiant.User = user;

                _etudiantRepository.Creer(etudiant);

                TempData["Succes"] = $"L'étudiant {etudiant.Nom} a été créé";

                if (User.IsInRole("Admin") || User.IsInRole("Gestionnaire"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Profil");
                }
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors de la création");
            }
        }

        /// <summary>
        /// Affiche le formulaire de modification.
        /// </summary>
        /// <param name="id">Id de l’étudiant</param>
        /// <returns>Vue ou erreur</returns>
        /// <author>John Zuleta</author>
        public async Task<IActionResult> Modifier(int id)
        {
            try
            {
                ViewData["Title"] = "Modifier un étudiant";
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                var etudiant = _etudiantRepository.GetEtudiant(id);
                if (etudiant == null)
                    return Erreur(404, "Étudiant introuvable");

                if (!User.IsInRole("Admin") &&
                    !User.IsInRole("Gestionnaire") &&
                    etudiant.ApplicationUserId != user.Id)
                    return Forbid();

                ViewBag.Genres = _genresRepository.Genres;
                ViewBag.Programmes = _programmesRepository.Programmes;
                ViewBag.Campus = _campusRepository.Campus;


                return View(etudiant);
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors du chargement");
            }
        }

        /// <summary>
        /// Traite la modification d’un étudiant.
        /// </summary>
        /// <param name="etudiant">Données modifiées</param>
        /// <returns>Redirection ou vue</returns>
        /// <author>John Zuleta</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(
            [Bind("Id,ApplicationUserId,Nom,Prenom,DateNaissance,GenreId,ProgrammeId,CampusId,noEtudiant,noAdmission,MobiliteReduite,AdressePermanente,Telephone,CourrielInstitutionnel,CourrielPersonnel,ApplicationUserId")] Etudiant etudiant)
        {
            try
            {

                ViewData["Title"] = "Modifier un étudiant";
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                if (!User.IsInRole("Admin") &&
                    !User.IsInRole("Gestionnaire") &&
                    etudiant.ApplicationUserId != user.Id)
                    return Forbid();

                if (ModelState.IsValid)
                {
                    etudiant.Genre = _genresRepository.GetGenre(etudiant.GenreId);
                    etudiant.Programme = _programmesRepository.GetProgramme(etudiant.ProgrammeId);
                    etudiant.Campus = _campusRepository.GetById(etudiant.CampusId);

                    _etudiantRepository.Modifier(etudiant);

                    TempData["Succes"] = $"L'étudiant {etudiant.Nom} a été modifié";
                    if(User.IsInRole("Admin") || User.IsInRole("Gestionnaire")){
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Profil");
                    }
                    
                }

                ViewBag.Genres = _genresRepository.Genres;
                ViewBag.Programmes = _programmesRepository.Programmes;
                ViewBag.Campus = _campusRepository.Campus;

                return View(etudiant);
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors de la modification");
            }
        }

        /// <summary>
        /// Supprime un étudiant.
        /// </summary>
        /// <param name="Id">Id étudiant</param>
        /// <returns>Redirection</returns>
        /// <author>John Zuleta</author>
        [Authorize(Policy = "AdminUniquement")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Supprimer([Bind("Id")] int Id)
        {
            try
            {
                var etudiant = _etudiantRepository.GetEtudiant(Id);

                if (etudiant == null)
                    return Erreur(404, "Étudiant introuvable");

                _etudiantRepository.Supprimer(etudiant);

                TempData["Succes"] = $"L'étudiant {etudiant.Prenom} {etudiant.Nom} a été supprimé";

          
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors de la suppression");
            }
        }

        /// <summary>
        /// Affiche le profil de l’étudiant connecté.
        /// </summary>
        /// <returns>Vue du profil</returns>
        /// <author>Benoit Tremblay</author>
        public async Task<IActionResult> Profil()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Challenge();

                var etudiant = await _etudiantRepository.GetByUserIdAsync(user.Id);
           

                return View(etudiant);
            }
            catch (Exception)
            {
                return Erreur(500, "Erreur lors du chargement du profil");
            }
        }

        /// <summary>
        /// Génère une vue d’erreur personnalisée avec code HTTP et message.
        /// </summary>
        /// <param name="code">Code HTTP (404, 500, etc.)</param>
        /// <param name="message">Message à afficher à l'utilisateur</param>
        /// <returns>Vue Error avec modèle personnalisé</returns>
        /// <author>John Zuleta</author>
        private IActionResult Erreur(int code, string message)
        {
            return View("Error", new ErreurViewModel
            {
                StatusCode = code,
                Message = message,
            });
        }
    }
}