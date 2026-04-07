using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.NewFolder;

namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;

        public int GenreParDefaut = 1;

        public DemandeController(
            ISemestreRepository semestreRepository,
            IGenresRepository genreRepository,
            IEtudiantRepository etudiantRepository,
            IDemandeRepository demandeRepository)
        {
            _semestreRepository = semestreRepository;
            _genreRepository = genreRepository;
            _etudiantRepository = etudiantRepository;
            _demandeRepository = demandeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Creer()
        {
            ViewBag.Etudiant = GenreParDefaut;
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

            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("Demandes");
        }

        public ViewResult Modifier(int Id)
        {
            Demande demande = _demandeRepository.GetDemande(Id);

            if (demande != null)
            {
                ViewBag.Etudiant = GenreParDefaut;
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                return View(demande);
            }

            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("Demandes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modifier(Demande demande, List<int> PreferencesGenreIds)
        {
            Demande demandeDb = _demandeRepository.GetDemande(demande.Id);

            if (demandeDb == null)
            {
                return NotFound();
            }

           
            demandeDb.SemestreId = demande.SemestreId;
            demandeDb.PrefDureeBail = demande.PrefDureeBail;
            demandeDb.AccepteReglements = demande.AccepteReglements;
            demandeDb.AccepteTraitementDonnees = demande.AccepteTraitementDonnees;
            demandeDb.ConfirmeSoumission = demande.ConfirmeSoumission;
            demandeDb.DateDemande = demande.DateDemande;

         
            demandeDb.DemandeGenres.Clear();

            if (PreferencesGenreIds != null)
            {
                foreach (int genreId in PreferencesGenreIds)
                {
                    demandeDb.DemandeGenres.Add(new DemandeGenre
                    {
                        DemandeId = demandeDb.Id,
                        GenreId = genreId
                    });
                }
            }

            _demandeRepository.Modifier(demandeDb);

            TempData["Succes"] = "La demande a bien été modifiée";
            return RedirectToAction("Demandes");
        }

        public void AjoutJumelageChoisis(Demande demande, List<JumelageViewModel> jumelages)
        {
            if (demande.Jumelages == null)
            {
                demande.Jumelages = new List<Jumelage>();
            }

            demande.Jumelages.Clear();

            foreach (JumelageViewModel jumelage in jumelages)
            {
                if (!string.IsNullOrEmpty(jumelage.Nom) && !string.IsNullOrEmpty(jumelage.Courriel))
                {
                    demande.Jumelages.Add(new Jumelage
                    {
                        Nom = jumelage.Nom,
                        Courriel = jumelage.Courriel
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creer(
            [Bind("SemestreId,EtudiantId,PrefDureeBail,AccepteReglements,AccepteTraitementDonnees,ConfirmeSoumission,NomGarant,PrenomGarant,DateNaissanceGarant,CourrielGarant,TelephoneGarant,NomParent,CourrielParent,NomUrgence,LienParenteUrgence,TelephoneUrgence")]
            Demande demande,
            List<JumelageViewModel> jumelage,
            List<int> PreferencesGenreIds)
        {
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");
            ModelState.Remove("DemandeGenres");

            ModelState.Keys
                .Where(k => k.StartsWith("jumelage"))
                .ToList()
                .ForEach(k => ModelState.Remove(k));

            ModelState.Remove("DateNaissanceGarant");
            ModelState.Remove("DateDemande");

            if (ModelState.IsValid)
            {
                Semestre? semestre = _semestreRepository.GetSemestreParId(demande.SemestreId);
                Etudiant? etudiant = _etudiantRepository.GetEtudiant(1);

                if (semestre != null && etudiant != null)
                {
                    demande.Semestre = semestre;
                    demande.Etudiant = etudiant;

               
                    demande.DemandeGenres = new List<DemandeGenre>();

                    if (PreferencesGenreIds != null)
                    {
                        foreach (int genreId in PreferencesGenreIds)
                        {
                            demande.DemandeGenres.Add(new DemandeGenre
                            {
                                GenreId = genreId
                            });
                        }
                    }

                    AjoutJumelageChoisis(demande, jumelage);

                    if (TryValidateModel(demande))
                    {
                        _demandeRepository.Creer(demande);
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestres = _semestreRepository.Semestres;
            return View(demande);
        }

        public ViewResult EtapeDemande()
        {
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestres = _semestreRepository.Semestres;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EtapeDemande(Demande demande)
        {
            ModelState.Remove("DemandeGenres");
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");

            if (ModelState.IsValid)
            {
                return View();
            }

            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestres = _semestreRepository.Semestres;
            return View(demande);
        }

        public ViewResult EtapeInformation()
        {
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

            return View(demande);
        }

        public ViewResult EtapeConfirmation()
        {
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

            return View(demande);
        }
    }
}

