using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;
using System.ComponentModel.DataAnnotations;
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

        [Authorize(Policy = "EstEtudiant")]
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
        public IActionResult Creer()
        {
            var vm = new DemandeCreateViewModel
            {
                Genres = _genreRepository.Genres,
                Semestres = _semestreRepository.Semestres
            };

            return View(vm);
        }
        [Authorize(Policy = "AdminOuGestionnaire")]
        public ViewResult Demandes() 
        {
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
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

        [Authorize(Policy = "EstEtudiant")]
        public IActionResult Modifier(int id)
        {
            var demande = _demandeRepository.GetDemande(id);
            if (demande == null)
            {
                TempData["Erreur"] = "Demande introuvable.";
                return RedirectToAction("Index");
            }

            var vm = new DemandeCreateViewModel
            {
                Demande = demande,
                Jumelages = demande.Jumelages.Select(j => new JumelageViewModel
                {
                    Nom = j.Nom,
                    Courriel = j.Courriel
                }).ToList(),

                Genres = _genreRepository.Genres,
                Semestres = _semestreRepository.Semestres,

                SelectedGenreId = demande.PreferencesGenre?.Id,
                SelectedSemestreId = demande.Semestre?.Id
            };

            // Ajouter des jumelages vides si moins de 3
            while (vm.Jumelages.Count < 3)
                vm.Jumelages.Add(new JumelageViewModel());

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Modifier(DemandeCreateViewModel vm)
        {
            // 1️⃣ Recharger les dropdowns pour la vue
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            // 2️⃣ Validation de base du ViewModel
            if (!ModelState.IsValid)
                return View(vm);

            // 3️⃣ Récupérer l'étudiant connecté
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null)
                return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError(string.Empty, "Étudiant introuvable.");
                return View(vm);
            }

            // 4️⃣ Valider les sélections de semestre et genre
            if (!vm.SelectedSemestreId.HasValue)
                ModelState.AddModelError("SelectedSemestreId", "Le semestre est requis.");

            if (!vm.SelectedGenreId.HasValue)
                ModelState.AddModelError("SelectedGenreId", "Le genre préféré est requis.");

            var semestre = _semestreRepository.GetSemestreParId(vm.SelectedSemestreId ?? 0);
            var genre = _genreRepository.GetGenre(vm.SelectedGenreId ?? 0);

            if (semestre == null)
                ModelState.AddModelError("SelectedSemestreId", "Semestre invalide.");

            if (genre == null)
                ModelState.AddModelError("SelectedGenreId", "Genre invalide.");

            if (!ModelState.IsValid)
                return View(vm);

            // 5️⃣ Récupérer la demande existante depuis la base pour EF
            var demandeEnBase = _demandeRepository.GetDemande(vm.Demande.Id);
            if (demandeEnBase == null)
                return NotFound();

            // 6️⃣ Vérifier doublon (hors demande courante)
            bool demandeDoubleExiste = _demandeRepository.Demandes
                .Any(d => d.Id != demandeEnBase.Id &&
                          d.EtudiantId == etudiant.Id &&
                          d.SemestreId == semestre.Id);

            if (demandeDoubleExiste)
            {
                ModelState.AddModelError(string.Empty, "Une demande existe déjà pour cet étudiant et ce semestre.");
                return View(vm);
            }

            // 7️⃣ Valider le ViewModel Demande
            if (!TryValidateModel(vm.Demande))
            {
                ModelState.AddModelError(string.Empty, "Certaines informations de la demande sont invalides.");
                return View(vm);
            }

            // 8️⃣ Mettre à jour tous les champs modifiables de la demande
            demandeEnBase.SemestreId = semestre.Id;
            demandeEnBase.PreferencesGenreId = genre.Id;
            demandeEnBase.PrefDureeBail = vm.Demande.PrefDureeBail;
            demandeEnBase.AccepteReglements = vm.Demande.AccepteReglements;
            demandeEnBase.AccepteTraitementDonnees = vm.Demande.AccepteTraitementDonnees;
            demandeEnBase.ConfirmeSoumission = vm.Demande.ConfirmeSoumission;
            demandeEnBase.DateDemande = vm.Demande.DateDemande;
            demandeEnBase.NomGarant = vm.Demande.NomGarant;
            demandeEnBase.PrenomGarant = vm.Demande.PrenomGarant;
            demandeEnBase.DateNaissanceGarant = vm.Demande.DateNaissanceGarant;
            demandeEnBase.CourrielGarant = vm.Demande.CourrielGarant;
            demandeEnBase.TelephoneGarant = vm.Demande.TelephoneGarant;
            demandeEnBase.NomParent = vm.Demande.NomParent;
            demandeEnBase.CourrielParent = vm.Demande.CourrielParent;
            demandeEnBase.NomUrgence = vm.Demande.NomUrgence;
            demandeEnBase.LienParenteUrgence = vm.Demande.LienParenteUrgence;
            demandeEnBase.TelephoneUrgence = vm.Demande.TelephoneUrgence;
            demandeEnBase.DateDebutBail = vm.Demande.DateDebutBail;
            demandeEnBase.DateFinBail = vm.Demande.DateFinBail;
            demandeEnBase.StatutDemande = vm.Demande.StatutDemande;
            demandeEnBase.DateTraitement = vm.Demande.DateTraitement;
            demandeEnBase.UniteId = vm.Demande.UniteId;

            // 9️⃣ Mettre à jour les jumelages
            demandeEnBase.Jumelages.Clear();
            foreach (var j in vm.Jumelages)
            {
                if (!string.IsNullOrWhiteSpace(j.Nom))
                {
                    if (string.IsNullOrWhiteSpace(j.Courriel) || !new EmailAddressAttribute().IsValid(j.Courriel))
                    {
                        ModelState.AddModelError(string.Empty, $"Courriel invalide pour le jumelage {j.Nom}.");
                        return View(vm);
                    }

                    demandeEnBase.Jumelages.Add(new Jumelage
                    {
                        Nom = j.Nom,
                        Courriel = j.Courriel
                    });
                }
            }

            // 🔟 Sauvegarder les modifications via le repository
            _demandeRepository.Modifier(demandeEnBase);

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
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Creer(DemandeCreateViewModel vm)
        {
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            if (!ModelState.IsValid)
                return View(vm);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null)
                return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError(string.Empty, "Étudiant introuvable.");
                return View(vm);
            }

            if (!vm.SelectedSemestreId.HasValue)
                ModelState.AddModelError("SelectedSemestreId", "Le semestre est requis.");

            if (!vm.SelectedGenreId.HasValue)
                ModelState.AddModelError("SelectedGenreId", "Le genre préféré est requis.");

            var semestre = _semestreRepository.GetSemestreParId(vm.SelectedSemestreId ?? 0);
            var genre = _genreRepository.GetGenre(vm.SelectedGenreId ?? 0);

            if (semestre == null)
                ModelState.AddModelError("SelectedSemestreId", "Semestre invalide.");

            if (genre == null)
                ModelState.AddModelError("SelectedGenreId", "Genre invalide.");

            if (!ModelState.IsValid)
                return View(vm);

            bool demandeExiste = _demandeRepository.Demandes
                .Any(d => d.EtudiantId == etudiant.Id && d.SemestreId == semestre.Id);

            if (demandeExiste)
            {
                ModelState.AddModelError(string.Empty, "Une demande existe déjà pour cet étudiant et ce semestre.");
                return View(vm);
            }

            if (!TryValidateModel(vm.Demande))
            {
                ModelState.AddModelError(string.Empty, "Certaines informations de la demande sont invalides.");
                return View(vm);
            }

            var demande = vm.Demande;
            demande.Etudiant = etudiant;
            demande.Semestre = semestre;
            demande.PreferencesGenre = genre;

            foreach (var j in vm.Jumelages)
            {
                if (!string.IsNullOrWhiteSpace(j.Nom))
                {
                    if (string.IsNullOrWhiteSpace(j.Courriel) || !new EmailAddressAttribute().IsValid(j.Courriel))
                    {
                        ModelState.AddModelError(string.Empty, $"Courriel invalide pour le jumelage {j.Nom}.");
                        return View(vm);
                    }

                    demande.Jumelages.Add(new Jumelage
                    {
                        Nom = j.Nom,
                        Courriel = j.Courriel
                    });
                }
            }

            _demandeRepository.Creer(demande);

            TempData["Succes"] = $"Nouvelle demande ajoutée pour {etudiant.Nom} ({semestre.NomSemestre})";

            return RedirectToAction("Index");
        }



    }
}
