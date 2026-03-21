using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Linq;

namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenreRepository _genreRepository;



        public DemandeController(ISemestreRepository semestreRepository, IGenreRepository genreRepository)
        {
            _semestreRepository = semestreRepository;
            _genreRepository = genreRepository;
        }



        public void AjoutGenreChoisis(Demande demande)
        {
            Genre genre = _genreRepository.GetGenreParId(1);
        }


        public IActionResult Index()
        {
            return View();
        }

        public ViewResult EtapeDemande() 
        {
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestre = _semestreRepository.Semestres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EtapeDemande(Demande demande, string[] nom, string[] email)
        {
            ModelState.Remove("PreferencesGenre");
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");


            if (ModelState.IsValid)
            {
                return View();
            }
            else
            {
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestre = _semestreRepository.Semestres;
                return View(demande);
            }
        }



        public ViewResult EtapeInformation()
        {
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestre = _semestreRepository.Semestres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EtapeInformation(Demande demande)
        {
            if (ModelState.IsValid)
            {
                return View();
            }
            else
            {
                return View(demande);
            }
        }




        public ViewResult EtapeConfirmation()
        {
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestre = _semestreRepository.Semestres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EtapeConfirmation(Demande demande)
        {
            if (ModelState.IsValid)
            {
                return View();
            }
            else
            {
                return View(demande);
            }
        }


    }
}
