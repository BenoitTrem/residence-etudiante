using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using S14_ProjetSession.ViewModels;

namespace S14_ProjetSession.Controllers
{
    public class GestionDemandeController : Controller
    {
        private readonly ISemestreRepository _semestreRepository;
        private readonly IGenresRepository _genreRepository;
        private readonly IEtudiantRepository _etudiantRepository;
        private readonly IDemandeRepository _demandeRepository;
        private readonly IUniteRepository _uniteRepository;

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

        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult Index(int? semestreFiltre)
        {
            var demandes = _demandeRepository.Demandes;

            // Filtrer par semestre si sélectionné
            if (semestreFiltre.HasValue)
            {
                demandes = demandes
                    .Where(d => d.SemestreId == semestreFiltre.Value)
                    .ToList();
            }

            var vm = new GestionDemandeIndexViewModel
            {
                Demandes = demandes,
                Unites = _uniteRepository.GetAll(),
                Semestres = _semestreRepository.Semestres.ToList(),
                SemestreFiltre = semestreFiltre
            };

            return View(vm);
        }

        /// <summary>
        /// Change le statut d'une demande et optionnellement assigne une unité.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOuGestionnaire")]
        public IActionResult TraiterDemande(int demandeId, StatutDemande statut, int? uniteId)
        {
            var demande = _demandeRepository.GetDemande(demandeId);
            if (demande == null)
            {
                TempData["Erreur"] = "Demande introuvable.";
                return RedirectToAction("Index");
            }

            // Si on accepte, il faut une unité
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

                // Vérifier la capacité pour CE semestre
                int placesOccupees = _demandeRepository.Demandes
                    .Count(d => d.UniteId == unite.Id
                             && d.SemestreId == demande.SemestreId
                             && d.StatutDemande == StatutDemande.Acceptee
                             && d.Id != demande.Id); // exclure la demande courante

                if (placesOccupees >= unite.Capacite)
                {
                    TempData["Erreur"] = $"L'unité {unite.Numero} est pleine pour ce semestre ({placesOccupees}/{unite.Capacite}).";
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
