using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

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

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Nouveau() 
        {
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestre = _semestreRepository.Semestres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nouveau(Demande demande)
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
