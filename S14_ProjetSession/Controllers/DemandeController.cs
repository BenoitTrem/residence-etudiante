using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.NewFolder;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;
        public int GenreParDefaut = 1;

        public DemandeController(ISemestreRepository semestreRepository, IGenresRepository genreRepository, IEtudiantRepository etudiantRepository, IDemandeRepository demandeRepository)
        {
            
            _semestreRepository = semestreRepository;
            _genreRepository = genreRepository;
            _etudiantRepository = etudiantRepository;
            _demandeRepository = demandeRepository;
        }



        public void AjoutGenreChoisis(Demande demande)
        {
            Genre genre = _genreRepository.GetGenre(1);
        }

        
        public async Task<IActionResult> Index()
        {
            // id du User avec le login je sais pas comment faire donc default a 1 
            // id connecté 
            // get l'identifiant de l'utilisateur
            
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) 
            {
                return Unauthorized();
            }
            Etudiant? etudiantConnecte  = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) 
            {
                return Unauthorized();
            }
            List<Demande> demandes = _demandeRepository.Demandes.Where(E => E.EtudiantId == etudiantConnecte.Id).ToList();
            return View(demandes);
        }



        [Authorize(Policy = "EstEtudiant")]
        public ViewResult Creer()
        {
            ViewBag.Etudiant = GenreParDefaut;
            ViewBag.Genres = _genreRepository.Genres;
            ViewBag.Semestres = _semestreRepository.Semestres;
            return View();
        }

        [Authorize(Policy = "AdminOuGestionnaire")]
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
                TempData["succes"] = "demande supprimée avec succès";
                
            }

            // ajouter tempData
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("demandes");
        }

        // la personne qui possède ou Admin
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
            ViewBag.Semestres = _semestreRepository.Semestres;
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("demandes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Demande demande, List<JumelageViewModel> jumelageNouveau)
        {
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");
            ModelState.Remove("PreferencesGenre");
            ModelState.Remove("jumelages");

            ModelState.Keys
                .Where(k => k.StartsWith("jumelage"))
                .ToList()
                .ForEach(k => ModelState.Remove(k));

            ModelState.Remove("DateNaissanceGarant");
            ModelState.Remove("DateDemande");

            if (!ModelState.IsValid)
            {
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                return View(demande);
            }

            Semestre? semestre = _semestreRepository.GetSemestreParId(demande.SemestreId);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }
            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null)
            {
                return Unauthorized();
            }
            Etudiant? etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id); // temporaire
            Genre? genre = null;

            if (demande.PreferencesGenreId.HasValue)
            {
                genre = _genreRepository.GetGenre(demande.PreferencesGenreId.Value);
            }

            if (semestre == null || etudiant == null || genre == null)
            {
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                ModelState.AddModelError(string.Empty, "Impossible de modifier la demande : données manquantes.");
                return View(demande);
            }

            demande.Semestre = semestre;
            demande.Etudiant = etudiant;
            demande.EtudiantId = etudiant.Id;
            demande.PreferencesGenre = genre;

            AjoutJumelageChoisis(demande, jumelageNouveau);

            Demande? demandeDoubleExiste = _demandeRepository.Demandes
                .FirstOrDefault(d => d.Id != demande.Id
                                  && d.EtudiantId == demande.EtudiantId
                                  && d.SemestreId == demande.SemestreId);

            if (demandeDoubleExiste != null)
            {
                ModelState.AddModelError(string.Empty, "Une demande existe déjà pour cet étudiant et ce semestre.");
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                return View(demande);
            }

            if (!TryValidateModel(demande))
            {
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                return View(demande);
            }

            _demandeRepository.Modifier(demande);
            TempData["Succes"] = "La demande a bien été modifiée";
            return RedirectToAction("Index");
        }



        public void AjoutJumelageChoisis(Demande demande, List<JumelageViewModel>? jumelages)
        {
            if (jumelages == null)
                return;

            // initialiser la liste si null
            if (demande.Jumelages == null)
                demande.Jumelages = new List<Jumelage>();

            // reset
            demande.Jumelages.Clear();

            foreach (JumelageViewModel jumelage in jumelages)
            {
                if (!string.IsNullOrWhiteSpace(jumelage.Nom) &&
                    !string.IsNullOrWhiteSpace(jumelage.Courriel))
                {
                    Jumelage nouveauJumelage = new Jumelage
                    {
                        Nom = jumelage.Nom,
                        Courriel = jumelage.Courriel
                    };

                    demande.Jumelages.Add(nouveauJumelage);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer([Bind("SemestreId,EtudiantId,PreferencesGenreId,PrefDureeBail,AccepteReglements,AccepteTraitementDonnees,ConfirmeSoumission,NomGarant,PrenomGarant,DateNaissanceGarant,CourrielGarant,TelephoneGarant,NomParent,CourrielParent,NomUrgence,LienParenteUrgence,TelephoneUrgence")] Demande demande, List<JumelageViewModel> jumelage)
        {
            ModelState.Remove("Etudiant");
            ModelState.Remove("Semestre");
            ModelState.Remove("PreferencesGenre");
            ModelState.Remove("jumelages");
            // jumelage ne voulait pas s'enlever sinon Fix rapide
            ModelState.Keys
                .Where(k => k.StartsWith("jumelage"))
                .ToList()
                .ForEach(k => ModelState.Remove(k));
            ModelState.Remove("DateNaissanceGarant");
            ModelState.Remove("DateDemande");

            // faire un tempDATA    
            //  regarder la dateDenaissance si bonne 
            // envoyer les Jumelages 

            if (ModelState.IsValid)
            {
                Semestre? semestre= _semestreRepository.GetSemestreParId(demande.SemestreId);
                

                // avec le login 
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
                Etudiant? etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
                
                Genre? genre = _genreRepository.GetGenre(demande.PreferencesGenreId.Value);
                Demande? demandeDoubleExiste = _demandeRepository.Demandes.FirstOrDefault(d => d.EtudiantId == 1 && d.SemestreId == semestre.Id);
                if (demandeDoubleExiste != null) 
                {
                    ModelState.AddModelError(string.Empty, "Une demande existe déjà pour cet étudiant et ce semestre.");
                    ViewBag.Genres = _genreRepository.Genres;
                    ViewBag.Semestres = _semestreRepository.Semestres;
                    return View(demande);
                }
                    if (semestre != null && etudiant != null && genre != null)
                {
                    demande.Semestre = semestre;
                    demande.Etudiant = etudiant;
                    demande.PreferencesGenre = genre;
                    AjoutJumelageChoisis(demande,jumelage);
                    Console.WriteLine(ModelState.IsValid);
                    if (TryValidateModel(demande))
                    {
                        _demandeRepository.Creer(demande);
                        TempData["Succes"] = $"Nouvelle demande Ajouter {demande.Etudiant.Nom} {demande.Semestre.NomSemestre}";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Genres = _genreRepository.Genres;
                        ViewBag.Semestres = _semestreRepository.Semestres;
                        return View(demande);
                    }
                }
                else
                {
                    ViewBag.Genres = _genreRepository.Genres;
                    ViewBag.Semestres = _semestreRepository.Semestres;
                    ModelState.AddModelError(string.Empty, "Impossible de créer la demande : données manquantes (semestre/étudiant/genre).");
                    return View(demande);
                }
            }
            else
            {
                ViewBag.Genres = _genreRepository.Genres;
                ViewBag.Semestres = _semestreRepository.Semestres;
                return View(demande);
            }
        }



    }
}
