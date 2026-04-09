using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;

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

        // ─── Helpers ────────────────────────────────────────────────────────────

        /// <summary>
        /// Remplace les DemandeGenres d'une demande par les genres sélectionnés.
        /// </summary>
        private bool AppliquerGenres(Demande demande, List<int> selectedGenreIds, out string erreur)
        {
            erreur = string.Empty;

            if (selectedGenreIds == null || !selectedGenreIds.Any())
            {
                erreur = "Au moins un genre préféré est requis.";
                return false;
            }

            // Valider que tous les IDs existent
            var genres = selectedGenreIds
                .Select(id => _genreRepository.GetGenre(id))
                .ToList();

            if (genres.Any(g => g == null))
            {
                erreur = "Un ou plusieurs genres sélectionnés sont invalides.";
                return false;
            }

            // Remplacer la collection
            demande.DemandeGenres.Clear();
            foreach (var genre in genres)
            {
                demande.DemandeGenres.Add(new DemandeGenre
                {
                    GenreId = genre!.Id,
                    DemandeId = demande.Id   // 0 lors de la création, EF le résout
                });
            }

            return true;
        }

        public void AjoutJumelageChoisis(Demande demande, List<JumelageViewModel>? jumelages)
        {
            if (jumelages == null) return;

            demande.Jumelages ??= new List<Jumelage>();
            demande.Jumelages.Clear();

            foreach (var jumelage in jumelages)
            {
                if (!string.IsNullOrWhiteSpace(jumelage.Nom) &&
                    !string.IsNullOrWhiteSpace(jumelage.Courriel))
                {
                    demande.Jumelages.Add(new Jumelage
                    {
                        Nom = jumelage.Nom,
                        Courriel = jumelage.Courriel
                    });
                }
            }
        }

        // ─── Index ───────────────────────────────────────────────────────────────

        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) return Unauthorized();

            List<Demande> demandes = _demandeRepository.Demandes
                .Where(d => d.EtudiantId == etudiantConnecte.Id)
                .ToList();

            return View(demandes);
        }

        // ─── Creer GET ───────────────────────────────────────────────────────────

        [Authorize(Policy = "EstEtudiant")]
        public IActionResult Creer()
        {
            var vm = new DemandeCreateViewModel
            {
                Genres = _genreRepository.Genres,
                Semestres = _semestreRepository.Semestres,
                SelectedGenreIds = new List<int>()
            };

            return View(vm);
        }

        // ─── Creer POST ──────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Creer(DemandeCreateViewModel vm)
        {
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            if (!ModelState.IsValid)
                return View(vm);

            // Étudiant connecté
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError(string.Empty, "Étudiant introuvable.");
                return View(vm);
            }

            // Valider semestre
            if (!vm.SelectedSemestreId.HasValue)
                ModelState.AddModelError("SelectedSemestreId", "Le semestre est requis.");

            var semestre = _semestreRepository.GetSemestreParId(vm.SelectedSemestreId ?? 0);
            if (semestre == null)
                ModelState.AddModelError("SelectedSemestreId", "Semestre invalide.");

            // Valider genres (plusieurs)
            if (vm.SelectedGenreIds == null || !vm.SelectedGenreIds.Any())
                ModelState.AddModelError("SelectedGenreIds", "Au moins un genre préféré est requis.");

            if (!ModelState.IsValid)
                return View(vm);

            // Doublon
            bool demandeExiste = _demandeRepository.Demandes
                .Any(d => d.EtudiantId == etudiant.Id && d.SemestreId == semestre!.Id);

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

            // Jumelages
            foreach (var j in vm.Jumelages)
            {
                if (!string.IsNullOrWhiteSpace(j.Nom))
                {
                    if (string.IsNullOrWhiteSpace(j.Courriel) ||
                        !new EmailAddressAttribute().IsValid(j.Courriel))
                    {
                        ModelState.AddModelError(string.Empty,
                            $"Courriel invalide pour le jumelage {j.Nom}.");
                        return View(vm);
                    }

                    vm.Demande.Jumelages.Add(new Jumelage
                    {
                        Nom = j.Nom,
                        Courriel = j.Courriel
                    });
                }
            }

            // Appliquer genres (relation N-N)
            if (!AppliquerGenres(vm.Demande, vm.SelectedGenreIds!, out string erreurGenre))
            {
                ModelState.AddModelError("SelectedGenreIds", erreurGenre);
                return View(vm);
            }

            vm.Demande.Etudiant = etudiant;
            vm.Demande.Semestre = semestre;

            _demandeRepository.Creer(vm.Demande);

            TempData["Succes"] = $"Nouvelle demande ajoutée pour {etudiant.Nom} ({semestre!.NomSemestre})";
            return RedirectToAction("Index");
        }

        // ─── Modifier GET ────────────────────────────────────────────────────────

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
                // Pré-cocher les genres déjà associés
                SelectedGenreIds = demande.DemandeGenres.Select(dg => dg.GenreId).ToList(),
                SelectedSemestreId = demande.Semestre?.Id
            };

            while (vm.Jumelages.Count < 3)
                vm.Jumelages.Add(new JumelageViewModel());

            return View(vm);
        }

        // ─── Modifier POST ───────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Modifier(DemandeCreateViewModel vm)
        {
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            if (!ModelState.IsValid)
                return View(vm);

            // Étudiant connecté
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError(string.Empty, "Étudiant introuvable.");
                return View(vm);
            }

            // Valider semestre
            if (!vm.SelectedSemestreId.HasValue)
                ModelState.AddModelError("SelectedSemestreId", "Le semestre est requis.");

            var semestre = _semestreRepository.GetSemestreParId(vm.SelectedSemestreId ?? 0);
            if (semestre == null)
                ModelState.AddModelError("SelectedSemestreId", "Semestre invalide.");

            // Valider genres
            if (vm.SelectedGenreIds == null || !vm.SelectedGenreIds.Any())
                ModelState.AddModelError("SelectedGenreIds", "Au moins un genre préféré est requis.");

            if (!ModelState.IsValid)
                return View(vm);

            // Demande existante
            var demandeEnBase = _demandeRepository.GetDemande(vm.Demande.Id);
            if (demandeEnBase == null) return NotFound();

            // Doublon (hors demande courante)
            bool demandeDoubleExiste = _demandeRepository.Demandes
                .Any(d => d.Id != demandeEnBase.Id &&
                          d.EtudiantId == etudiant.Id &&
                          d.SemestreId == semestre!.Id);

            if (demandeDoubleExiste)
            {
                ModelState.AddModelError(string.Empty,
                    "Une demande existe déjà pour cet étudiant et ce semestre.");
                return View(vm);
            }

            if (!TryValidateModel(vm.Demande))
            {
                ModelState.AddModelError(string.Empty, "Certaines informations de la demande sont invalides.");
                return View(vm);
            }

            // Mise à jour des champs scalaires
            demandeEnBase.SemestreId = semestre!.Id;
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

            // Mise à jour des genres (relation N-N)
            if (!AppliquerGenres(demandeEnBase, vm.SelectedGenreIds!, out string erreurGenre))
            {
                ModelState.AddModelError("SelectedGenreIds", erreurGenre);
                return View(vm);
            }

            // Mise à jour des jumelages
            demandeEnBase.Jumelages.Clear();
            foreach (var j in vm.Jumelages)
            {
                if (!string.IsNullOrWhiteSpace(j.Nom))
                {
                    if (string.IsNullOrWhiteSpace(j.Courriel) ||
                        !new EmailAddressAttribute().IsValid(j.Courriel))
                    {
                        ModelState.AddModelError(string.Empty,
                            $"Courriel invalide pour le jumelage {j.Nom}.");
                        return View(vm);
                    }

                    demandeEnBase.Jumelages.Add(new Jumelage
                    {
                        Nom = j.Nom,
                        Courriel = j.Courriel
                    });
                }
            }

            _demandeRepository.Modifier(demandeEnBase);

            TempData["Succes"] = "La demande a bien été modifiée.";
            return RedirectToAction("Index");
        }

        // ─── Demandes (admin) ────────────────────────────────────────────────────

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
                TempData["succes"] = "Demande supprimée avec succès.";
            }

            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("Demandes");
        }
    }
}