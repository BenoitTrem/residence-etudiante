using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


/// <author>Felix</author>
/// <author>John Zuleta (logique genres - demandes plusieurs à plusieurs)</author>
namespace S14_ProjetSession.Controllers
{
    public class DemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;

        /// <summary>
        /// Initialise le controleur des demandes avec les referentiels requis.
        /// </summary>
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

        
        private bool ValiderConsentements(Demande demande)
        {
            bool valide = true;

            if (!demande.AccepteReglements)
            {
                ModelState.AddModelError("Demande.AccepteReglements", "Vous devez accepter les règlements.");
                valide = false;
            }

            if (!demande.AccepteTraitementDonnees)
            {
                ModelState.AddModelError("Demande.AccepteTraitementDonnees", "Vous devez accepter le traitement des données.");
                valide = false;
            }

            if (!demande.ConfirmeSoumission)
            {
                ModelState.AddModelError("Demande.ConfirmeSoumission", "Vous devez confirmer la soumission.");
                valide = false;
            }

            return valide;
        }



        private bool ValiderSemestreOuvert(int? semestreId, out Semestre? semestre)
        {
            semestre = null;

            if (!semestreId.HasValue)
            {
                ModelState.AddModelError("SelectedSemestreId", "Le semestre est requis.");
                return false;
            }

            semestre = _semestreRepository.GetSemestreParId(semestreId.Value);
            if (semestre == null)
            {
                ModelState.AddModelError("SelectedSemestreId", "Semestre invalide.");
                return false;
            }

            if (!semestre.InscriptionOuverte)
            {
                ModelState.AddModelError("SelectedSemestreId", "La période d'inscription est fermée pour ce semestre.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retire du ModelState les proprietes de navigation gerees cote serveur.
        /// </summary>
        private void RetirerErreursNavigationDemande()
        {
            string[] cles =
            {
                "Demande.Etudiant",
                "Demande.Semestre",
                "Demande.DemandeGenres",
                "Demande.Jumelages",
                "Demande.Unite",
                "Etudiant",
                "Semestre",
                "DemandeGenres",
                "Jumelages",
                "Unite"
            };

            foreach (string cle in cles)
            {
                foreach (string cleModelState in ModelState.Keys
                    .Where(k => k == cle || k.StartsWith($"{cle}.") || k.StartsWith($"{cle}["))
                    .ToList())
                {
                    ModelState.Remove(cleModelState);
                }
            }
        }

        /// <summary>
        /// Indique si l'utilisateur connecte est le proprietaire de la demande.
        /// </summary>
        private async Task<bool> EstProprietaireConnecte(Demande demande)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return false;

            Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(userId);
            return etudiant != null && demande.EtudiantId == etudiant.Id;
        }

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
            List<Genre> genres = selectedGenreIds
                .Select(id => _genreRepository.GetGenre(id))
                .ToList();

            if (genres.Any(g => g == null))
            {
                erreur = "Un ou plusieurs genres sélectionnés sont invalides.";
                return false;
            }

            // Remplacer la collection
            demande.DemandeGenres ??= new List<Genre>();
            demande.DemandeGenres.Clear();
            foreach (var genre in genres)
            {
                demande.DemandeGenres.Add(genre);
            }

            return true;
        }

        /// <summary>
        /// Ajoute a la demande les jumelages complets fournis dans le formulaire.
        /// </summary>
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

        /// <summary>
        /// Cree une copie editable de la derniere demande, ou d'une demande precise, appartenant a l'etudiant connecte.
        /// </summary>
        public async Task<Demande?> GetDemandeAvecUserId(int? demandeId = null)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            Etudiant? etudiantActuelle = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantActuelle == null) return null;

            // avec etudiant get une demande
            Demande? demande = _demandeRepository.Demandes
                .Where(d => d.EtudiantId == etudiantActuelle.Id)
                .Where(d => !demandeId.HasValue || d.Id == demandeId.Value)
                .OrderByDescending(d => d.DateDemande)
                .FirstOrDefault();

            if (demande != null)
            {
                Demande CopieDemande = new Demande()
                {
                    PrefDureeBail = demande.PrefDureeBail,
                    AccepteReglements = demande.AccepteReglements,
                    AccepteTraitementDonnees = demande.AccepteTraitementDonnees,
                    ConfirmeSoumission = false,
                    NomGarant = demande.NomGarant,
                    PrenomGarant = demande.PrenomGarant,
                    DateNaissanceGarant = demande.DateNaissanceGarant,
                    CourrielGarant = demande.CourrielGarant,
                    TelephoneGarant = demande.TelephoneGarant,
                    NomParent = demande.NomParent,
                    CourrielParent = demande.CourrielParent,
                    NomUrgence = demande.NomUrgence,
                    LienParenteUrgence = demande.LienParenteUrgence,
                    TelephoneUrgence = demande.TelephoneUrgence,
                    DateDebutBail = demande.DateDebutBail,
                    DateFinBail = demande.DateFinBail,
                    Jumelages = demande.Jumelages?
                        .Select(j => new Jumelage
                        {
                            Nom = j.Nom,
                            Courriel = j.Courriel
                        })
                        .ToList() ?? new List<Jumelage>(),
                    DemandeGenres = demande.DemandeGenres?.ToList() ?? new List<Genre>()
                };

                return CopieDemande;
            }
            return null;
        }

        

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

            ViewBag.PeriodeInscriptionOuverte = _semestreRepository.Semestres
                .Any(s => s.InscriptionOuverte);

            return View(demandes);
        }

        /// <summary>
        /// Exporte une demande en PDF pour son proprietaire, un gestionnaire ou un administrateur.
        /// </summary>
        [Authorize]
        public async Task<IActionResult> ExporterPdf(int id)
        {
            Demande? demande = _demandeRepository.GetDemande(id);
            if (demande == null)
            {
                TempData["Erreur"] = "Demande introuvable.";
                return RedirectToAction("Index");
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("Gestionnaire"))
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Etudiant? etudiant = string.IsNullOrEmpty(userId)
                    ? null
                    : await _etudiantRepository.GetByUserIdAsync(userId);

                if (etudiant == null || demande.EtudiantId != etudiant.Id)
                {
                    return Forbid();
                }
            }

            byte[] pdf = DemandePdf.Generate(demande);
            string nomFichier = $"demande-{demande.Id}.pdf";

            return File(pdf, "application/pdf", nomFichier);
        }


        [Authorize(Policy = "EstEtudiant")]
        [HttpGet]
        public async Task<IActionResult> Creer(int? id)
        {
            if (id != null)
            {
                Demande? demande = await GetDemandeAvecUserId(id);
                if (demande == null)
                {
                    TempData["Erreur"] = "Impossible de copier cette demande.";
                    return RedirectToAction("Index");
                }

                List<int> GenreChoisieAnciens = demande.DemandeGenres.Select(g => g.Id).ToList();
                
                // recréation de jumelages
                List<JumelageViewModel> listeJumelages = new();
                foreach (Jumelage jumelage in demande.Jumelages)
                {
                    listeJumelages.Add(new JumelageViewModel() { Courriel = jumelage.Courriel, Nom = jumelage.Nom });
                }

               
                DemandeCreateViewModel vm = new DemandeCreateViewModel
                {
                    Demande = demande,
                    Genres = _genreRepository.Genres,
                    Semestres = _semestreRepository.Semestres,
                    SelectedGenreIds = GenreChoisieAnciens,
                    Jumelages = listeJumelages
                };
                return View(vm);
            }
            else
            {
                DemandeCreateViewModel vm = new DemandeCreateViewModel
                {
                    Genres = _genreRepository.Genres,
                    Semestres = _semestreRepository.Semestres,
                    SelectedGenreIds = new List<int>()
                };
                return View(vm);
            }
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EstEtudiant")]
        public async Task<IActionResult> Creer(DemandeCreateViewModel vm)
        {
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            void AfficherErreursModelState()
            {
                var erreurs = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Distinct()
                    .ToList();

                if (erreurs.Count > 0)
                {
                    TempData["Erreur"] = string.Join(" | ", erreurs);
                }
            }

            // Date trop ancienne: avant 1900, la date semble irréaliste.
            if (vm.Demande.DateNaissanceGarant.HasValue && vm.Demande.DateNaissanceGarant.Value.Year < 1900)
            {
                ModelState.AddModelError("Demande.DateNaissanceGarant", "La date de naissance du garant doit etre apres le 1er janvier 1900.");
                AfficherErreursModelState();
                return View(vm);
            }

            // regarde que il a accepter les regle
            if (!vm.Demande.AccepteReglements)
                ModelState.AddModelError("Demande.AccepteReglements", "Vous devez accepter les règlements.");

            if (!vm.Demande.AccepteTraitementDonnees)
                ModelState.AddModelError("Demande.AccepteTraitementDonnees", "Vous devez accepter le traitement des données.");

            if (!vm.Demande.ConfirmeSoumission)
                ModelState.AddModelError("Demande.ConfirmeSoumission", "Vous devez confirmer la soumission.");


            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;
            Semestre? semestre = null;

             ModelState.Remove("Demande.Etudiant");

            // Étudiant connecté
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError("Formulaire", "Étudiant introuvable.");
                AfficherErreursModelState();
                return View(vm);
            }

            if (!ValiderSemestreOuvert(vm.SelectedSemestreId, out semestre))
            {
                AfficherErreursModelState();
                return View(vm);
            }

            // les genres
            if (vm.SelectedGenreIds == null || !vm.SelectedGenreIds.Any())
                ModelState.AddModelError("SelectedGenreIds", "Au moins un genre préféré est requis.");

            // demande genre ne fonctionne pas ici 
            

            
            // Doublon
            bool demandeExiste = semestre != null && _demandeRepository.Demandes
                .Any(d => d.EtudiantId == etudiant.Id && d.SemestreId == semestre.Id);

            if (demandeExiste)
            {
                ModelState.AddModelError("Formulaire", "Une demande existe déjà pour cet étudiant et ce semestre. Vous pouvez modifier votre demande existante depuis votre tableau de bord.");
                AfficherErreursModelState();
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
                        ModelState.AddModelError("Formulaire",
                            $"Courriel invalide pour le jumelage {j.Nom}.");
                        AfficherErreursModelState();
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
                AfficherErreursModelState();
                return View(vm);
            }

            // Ajout des propriétés pour que Validate fonctionne.
            vm.Demande.StatutDemande = StatutDemande.EnAttente;
            vm.Demande.Etudiant = etudiant;
            vm.Demande.EtudiantId = etudiant.Id;
            vm.Demande.SemestreId = semestre.Id;
            vm.Demande.Semestre = semestre;

            // Propriété problématique à supprimer pour que le ModelState fonctionne.
            RetirerErreursNavigationDemande();

            TryValidateModel(vm.Demande);
            RetirerErreursNavigationDemande();
            ModelState.Remove("");
            if (!ModelState.IsValid)
            {
                AfficherErreursModelState();
                return View(vm);
            }

            _demandeRepository.Creer(vm.Demande);

            TempData["Succes"] = $"Nouvelle demande ajoutée pour {etudiant.Nom} ({semestre.NomSemestre})";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Affiche le formulaire de modification d'une demande appartenant a l'etudiant connecte.
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Modifier(int id)
        {
            var demande = _demandeRepository.GetDemande(id);
            if (demande == null)
            {
                TempData["Erreur"] = "Demande introuvable.";
                return RedirectToAction("Index");
            }

            if (!await EstProprietaireConnecte(demande))
            {
                return Forbid();
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
                // @author John Zuleta : lecture via .Id directement (plus de GenreId)
                SelectedGenreIds = demande.DemandeGenres.Select(g => g.Id).ToList(),
                SelectedSemestreId = demande.Semestre?.Id
            };

            while (vm.Jumelages.Count < 3)
                vm.Jumelages.Add(new JumelageViewModel());

            return View(vm);
        }

        // ─── Modifier POST ───────────────────────────────────────────────────────

        // je ne sais pas encore si je donne le droit à un Admin de modifier. TODO Félix
        /// <summary>
        /// Modifie une demande existante appartenant a l'etudiant connecte.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Modifier(DemandeCreateViewModel vm)
        {
            vm.Genres = _genreRepository.Genres;
            vm.Semestres = _semestreRepository.Semestres;

            // Demande existante
            var demandeEnBase = _demandeRepository.GetDemande(vm.Demande.Id);
            if (demandeEnBase == null) return NotFound();

            if (!await EstProprietaireConnecte(demandeEnBase))
            {
                return Forbid();
            }

            if (!ValiderConsentements(vm.Demande))
            {
                return View(vm);
            }

            // Propriétés de navigation assignées côté serveur
            RetirerErreursNavigationDemande();

            // Étudiant connecté
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Etudiant? etudiantConnecte = await _etudiantRepository.GetByUserIdAsync(userId);
            if (etudiantConnecte == null) return Unauthorized();

            var etudiant = _etudiantRepository.GetEtudiant(etudiantConnecte.Id);
            if (etudiant == null)
            {
                ModelState.AddModelError("Formulaire", "Étudiant introuvable.");
                return View(vm);
            }

            if (!ValiderSemestreOuvert(vm.SelectedSemestreId, out Semestre? semestre))
            {
                return View(vm);
            }

            // Valider genres
            if (vm.SelectedGenreIds == null || !vm.SelectedGenreIds.Any())
                ModelState.AddModelError("SelectedGenreIds", "Au moins un genre préféré est requis.");

            if (!ModelState.IsValid)
                return View(vm);

           
            // Doublon (hors demande courante)
            bool demandeDoubleExiste = _demandeRepository.Demandes
                .Any(d => d.Id != demandeEnBase.Id &&
                          d.EtudiantId == etudiant.Id &&
                          d.SemestreId == semestre.Id);

            if (demandeDoubleExiste)
            {
                ModelState.AddModelError("Formulaire",
                    "Une demande existe déjà pour cet étudiant et ce semestre. veuillez contacté l'admin pour changer cette demande");
                return View(vm);
            }

            // StatutDemande, DateTraitement, UniteId
            demandeEnBase.SemestreId = semestre.Id;
            demandeEnBase.PrefDureeBail = vm.Demande.PrefDureeBail;
            demandeEnBase.AccepteReglements = vm.Demande.AccepteReglements;
            demandeEnBase.AccepteTraitementDonnees = vm.Demande.AccepteTraitementDonnees;
            demandeEnBase.ConfirmeSoumission = vm.Demande.ConfirmeSoumission;
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

            if (User.IsInRole("Admin"))
            {
                demandeEnBase.StatutDemande = vm.Demande.StatutDemande;
                demandeEnBase.DateTraitement = vm.Demande.DateTraitement;
                demandeEnBase.UniteId = vm.Demande.UniteId;
            }

            // Mise à jour des genres (relation N-N)
            if (!AppliquerGenres(demandeEnBase, vm.SelectedGenreIds, out string erreurGenre))
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
                        ModelState.AddModelError("Formulaire",
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

            // Retirer les propriétés de navigation enfants pour éviter les erreurs de validation récursive
            RetirerErreursNavigationDemande();

            TryValidateModel(demandeEnBase);
            RetirerErreursNavigationDemande();
            ModelState.Remove("");
            if (!ModelState.IsValid)
                return View(vm);

            _demandeRepository.Modifier(demandeEnBase);

            TempData["Succes"] = "La demande a bien été modifiée.";
            return RedirectToAction("Index");
        }

        // ─── Demandes (admin) ────────────────────────────────────────────────────

        /// <summary>
        /// Affiche toutes les demandes pour les administrateurs et les gestionnaires.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public ViewResult Demandes()
        {
            ViewBag.Demandes = _demandeRepository.Demandes;
            return View();
        }

        /// <summary>
        /// Supprime une demande si l'utilisateur connecte est autorise a le faire.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Supprimer(int Id)
        {
            Demande demande = _demandeRepository.GetDemande(Id);
            if (demande != null)
            {
                if (!User.IsInRole("Admin") && !await EstProprietaireConnecte(demande))
                {
                    return Forbid();
                }

                _demandeRepository.Supprimer(demande);
                TempData["Succes"] = "Demande supprimée avec succès.";

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Demandes");
                }

                return RedirectToAction("Index");
            }

            ViewBag.Demandes = _demandeRepository.Demandes;
            return View("Demandes");
        }
    }
}
