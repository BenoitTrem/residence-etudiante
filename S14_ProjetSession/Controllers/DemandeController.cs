using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Linq;
using System.Net.Http.Headers;

namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;


        public DemandeController(ISemestreRepository semestreRepository, IGenreRepository genreRepository, IEtudiantRepository etudiantRepository, IDemandeRepository demandeRepository)
        {
            
            _semestreRepository = semestreRepository;
            _genreRepository = genreRepository;
            _etudiantRepository = etudiantRepository;
            _demandeRepository = demandeRepository;
        }



        public void AjoutGenreChoisis(Demande demande)
        {
            Genre genre = _genreRepository.GetGenreParId(1);
        }


        public IActionResult Index()
        {
            return View();
        }

        

        
        public ViewResult Creer()
        {
            ViewBag.Etudiant = 1;
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestres = _semestreRepository.Semestres;
            return View();
        }

        public ViewResult Demandes() 
        {
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Supprimer(int Id) 
        {
            Demande demande = _demandeRepository.GetDemande(Id);
            if (demande != null)
            {
                _demandeRepository.Supprimer(demande);
                TempData["success"] = "demande supprimée avec succès";
                
            }

            // ajouter tempData
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("demandes");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creer([Bind("SemestreId,EtudiantId,PreferencesGenreId,PrefDureeBail,AccepteReglements,AccepteTraitementDonnees,ConfirmeSoumission,NomGarant,PrenomGarant,DateNaissanceGarant,CourrielGarant,TelephoneGarant,NomParent,CourrielParent,NomUrgence,LienParenteUrgence,TelephoneUrgence")] Demande demande)
        {
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");
            ModelState.Remove("PreferencesGenre");
            ModelState.Remove("Jumelages");
            ModelState.Remove("DateNaissanceGarant");
            ModelState.Remove("DateDemande");

            // faire un tempDATA    

            if (ModelState.IsValid)
            {
                Semestre? semestre= _semestreRepository.GetSemestreParId(demande.SemestreId);
                // par défaut en attendant Login

                Etudiant? etudiant = _etudiantRepository.GetEtudiant(1);
                
                Genre? genre = _genreRepository.GetGenreParId(demande.PreferencesGenreId);
                if (semestre != null && etudiant != null && genre != null)
                {
                    demande.Semestre = semestre;
                    demande.Etudiant = etudiant;
                    demande.PreferencesGenre = genre;
                    demande.Jumelages = [];
                    if (TryValidateModel(demande))
                    {
                        _demandeRepository.Creer(demande);
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        return View(demande);
                    }
                    }
                else

                    return View(demande);
            }
            else
            {
                return View(demande);
            }
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
