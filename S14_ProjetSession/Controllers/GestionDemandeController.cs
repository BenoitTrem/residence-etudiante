using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;

/// <author>Felix</author>
namespace S14_ProjetSession.Controllers
{
    public class GestionDemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;
        private readonly IUniteRepository _uniteRepository;

        /// <summary>
        /// Initialise le controleur de gestion des demandes avec les referentiels requis.
        /// </summary>
        public GestionDemandeController(
            ISemestreRepository semestreRepository,
            IGenresRepository genreRepository,
            IEtudiantRepository etudiantRepository,
            IDemandeRepository demandeRepository,
            IUniteRepository uniteRepository
            )
        {
            _semestreRepository = semestreRepository;
            _genreRepository = genreRepository;
            _etudiantRepository = etudiantRepository;
            _demandeRepository = demandeRepository;
            _uniteRepository = uniteRepository;
        }

        /// <summary>
        /// Affiche les demandes a traiter avec filtre par semestre et pagination.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int? semestreFiltre, int page = 1)
        {
            int pageSize = 6;

            List<Demande> demandes = _demandeRepository.Demandes
                .OrderByDescending(d => d.Semestre.DateDebut)
                .ToList();

            if (semestreFiltre.HasValue)
            {
                demandes = demandes
                    .Where(d => d.SemestreId == semestreFiltre.Value)
                    .ToList();
            }

            // pagination
            int totalDemandes = demandes.Count;
            int totalPages = (int)Math.Ceiling((double)totalDemandes / pageSize);
            demandes = demandes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            GestionDemandeIndexViewModel vm = new GestionDemandeIndexViewModel
            {
                Demandes = demandes,
                Unites = _uniteRepository.GetAll(),
                Semestres = _semestreRepository.Semestres.ToList(),
                SemestreFiltre = semestreFiltre,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        /// <summary>
        /// Exporte en PDF les demandes visibles, avec filtre optionnel par semestre.
        /// </summary>
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult ExporterToutesPdf(int? semestreFiltre)
        {
            List<Demande> demandes = _demandeRepository.Demandes
                .OrderByDescending(d => d.Semestre.DateDebut)
                .ToList();

            if (semestreFiltre.HasValue)
            {
                demandes = demandes
                    .Where(d => d.SemestreId == semestreFiltre.Value)
                    .ToList();
            }

            byte[] pdf = DemandePdf.GenerateToutes(demandes);
            string suffixe = semestreFiltre.HasValue ? $"-semestre-{semestreFiltre.Value}" : string.Empty;

            return File(pdf, "application/pdf", $"demandes{suffixe}.pdf");
        }

        /// <summary>
        /// Change le statut d'une demande et optionnellement assigne une unité.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult TraiterDemande(int demandeId, StatutDemande statut, int? uniteId)
        {
            Demande demande = _demandeRepository.GetDemande(demandeId);

            if (demande == null)
            {
                TempData["Erreur"] = "Demande introuvable.";
                return RedirectToAction("Index");
            }
                
            // Si on accepte, il faut une unité.
            if (statut == StatutDemande.Acceptee)
            {
                if (!uniteId.HasValue)
                {
                    TempData["Erreur"] = "Vous devez sélectionner une unité pour accepter la demande.";
                    return RedirectToAction("Index", new { semestreFiltre = demande.SemestreId });
                }
                    
                var unite = _uniteRepository.GetById(uniteId.Value);
                if (unite == null)
                {
                    TempData["Erreur"] = "Unité introuvable.";
                    return RedirectToAction("Index", new { semestreFiltre = demande.SemestreId });

                }
                
                // Vérifier la capacité pour CE semestre en incluant les jumelages
                var demandesDejaAcceptees = _demandeRepository.Demandes
                    .Where(d => d.UniteId == unite.Id
                             && d.SemestreId == demande.SemestreId
                             && d.StatutDemande == StatutDemande.Acceptee
                             && d.Id != demande.Id)
                    .ToList();

                int placesOccupees = demandesDejaAcceptees.Count;

                // On évite de compter en double les colocataires qui sont déjà acceptés dans l'unité
                var emailsDejaAcceptes = demandesDejaAcceptees
                    .Select(d => d.Etudiant?.CourrielInstitutionnel)
                    .Where(e => e != null)
                    .ToList();


                // etudiant lui meme
                int placesAajouter = 1;
                foreach (var jumelage in demande.Jumelages)
                {
                    if (!emailsDejaAcceptes.Contains(jumelage.Courriel))
                    {
                        placesAajouter++;
                    }
                }

                if (placesOccupees + placesAajouter > unite.Capacite)
                {
                    TempData["Erreur"] = $"L'unité {unite.Numero} n'a pas assez de place pour accueillir le demandeur et ses colocataires ({placesOccupees + placesAajouter}/{unite.Capacite}).";
                    return RedirectToAction("Index", new { semestreFiltre = demande.SemestreId });
                }

                demande.UniteId = unite.Id;

            }

            else if (statut == StatutDemande.Refusee)
            {
                // Si refusée, retirer l'unité assignée
                demande.UniteId = null;
            }

            demande.StatutDemande = statut;
            demande.DateTraitement = DateTime.Now;

            _demandeRepository.Modifier(demande);

            string statutTexte = statut switch
            {
                StatutDemande.Acceptee => "acceptée",
                StatutDemande.Refusee => "refusée",
                _ => "mise en attente"
            };

            TempData["Succes"] = $"La demande de {demande.Etudiant?.Prenom} {demande.Etudiant?.Nom} a été {statutTexte}.";
            return RedirectToAction("Index", new { semestreFiltre = demande.SemestreId });
        }
    }
}
