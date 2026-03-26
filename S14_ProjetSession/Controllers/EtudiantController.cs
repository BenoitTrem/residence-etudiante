using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;


namespace S14_ProjetSession.Controllers
{

    [Authorize]
    public class EtudiantController : Controller
    {

        private readonly IEtudiantRepository _etudiantRepository;

        private readonly IGenresRepository _genresRepository;

        private readonly IProgrammesRepository _programmesRepository;



        public EtudiantController(IEtudiantRepository etudiantRepository, IGenresRepository genresRepository, IProgrammesRepository programmesRepository)
        {
            _etudiantRepository = etudiantRepository;
            _genresRepository = genresRepository;
            _programmesRepository = programmesRepository;

        }



        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewData["Title"] = "Etudiant";

            var etudiants = _etudiantRepository.Etudiants
                                                    .OrderBy(e => e.Nom)
                                                    .ThenBy(e => e.Prenom)
                                                    .ToList();

            return View("Etudiants", etudiants);
        }


        public ViewResult Creer()
        {
         
            ViewBag.Genres = _genresRepository.Genres;
            ViewBag.Programmes = _programmesRepository.Programmes;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Creer(Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _etudiantRepository.Creer(etudiant);
                Genre genre = _genresRepository.GetGenre(etudiant.GenreId);
                etudiant.Genre = genre;

                Programme programme = _programmesRepository.GetProgramme(etudiant.ProgrammeId);
                etudiant.Programme = programme;
                TempData.Add("Succes", "L'étudiant " + etudiant.Nom + " a été créer");
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Genres = _genresRepository.Genres;
                ViewBag.Programmes = _programmesRepository.Programmes;
                return View(etudiant);
            }

        }



        public ActionResult Modifier(int id)
        {
            Etudiant? etudiant = _etudiantRepository.GetEtudiant(id);

            if (etudiant == null)
            {
                TempData.Add("Erreur", "L'étudiant " + id + " n'existe pas");
                return RedirectToAction("Index");

            }
               

            ViewBag.Genres = _genresRepository.Genres;
            ViewBag.Programmes = _programmesRepository.Programmes;

            return View(etudiant);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifier(Etudiant etudiant)
        {
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Supprimer(int Id)
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
    }
}
