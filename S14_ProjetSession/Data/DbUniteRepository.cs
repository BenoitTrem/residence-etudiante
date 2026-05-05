using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using S14_ProjetSession.Models;
using System.Security.Policy;

namespace S14_ProjetSession.Data
{
    public class DbUniteRepository : IUniteRepository
    {
        private readonly ResidencesDbContext _context;

        public DbUniteRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public List<Unite> GetByResidenceId(int residenceId)
        {
            return _context.Unites
                .Where(u => u.ResidenceId == residenceId)
                .OrderByDescending(u => u.Capacite)
                .ThenBy(u => u.Numero)
                .Include(u => u.Residence)
                .ToList();
        }

        public List<Unite> GetDisponibleByResidenceId(int residenceId)
        {
            return _context.Unites
                .Where(u => u.ResidenceId == residenceId)
                .Include(u => u.Residence)
                .ToList();
        }

        public int GetTotalByResidenceId(int residenceId)
        {
            return _context.Unites.Count(u => u.ResidenceId == residenceId);
        }

        /// <summary>
        /// Retourne la liste des unités filtrées selon différents critères et triées par numéro.
        /// </summary>
        /// <param name="residenceId">Identifiant de la résidence pour filtrer les unités</param>
        /// <param name="disponible">Indique si l'unité doit être disponible (non utilisé ici)</param>
        /// <param name="capacite">Capacité recherchée pour filtrer les unités</param>
        /// <param name="numero">Numéro minimal de l'unité pour filtrer</param>
        /// <param name="ascendant">Indique si le tri est ascendant (true) ou descendant (false)</param>
        /// <param name="mobiliteReduite">Indique si l'unité doit être adaptée pour mobilité réduite</param>
        /// <returns>Liste des unités filtrées et triées</returns>
        public List<Unite> GetFiltrerByResidenceId(int residenceId, bool? disponible, int? capacite, int? numero, bool ascendant = true, bool? mobiliteReduite = null)
        {
            List<Unite> unites;

            // Filtrer par résidence si un identifiant valide est fourni, sinon récupérer toutes les unités
            if (residenceId > 0)
            {
                unites = _context.Unites
                    .Where(u => u.ResidenceId == residenceId)
                    .Include(u => u.Residence)
                    .ToList();
            }
            else
            {
                unites = _context.Unites
                    .Include(u => u.Residence)
                    .ToList();
            }

            // Filtre par capacité si une valeur est fournie
            if (capacite.HasValue)
            {
                unites = unites
                    .Where(u => u.Capacite == capacite.Value)
                    .ToList();
            }

            // Filtre par numéro minimal si une valeur est fournie
            if (numero.HasValue)
            {
                unites = unites
                    .Where(u => u.Numero.HasValue && u.Numero.Value >= numero.Value)
                    .ToList();
            }

            // Filtre selon l'adaptation pour mobilité réduite si précisé
            if (mobiliteReduite.HasValue)
            {
                unites = unites
                    .Where(u => u.AdapteePourMobiliteReduite == mobiliteReduite.Value)
                    .ToList();
            }

            // Trie la liste selon l'ordre demandé (ascendant ou descendant)
            if (ascendant)
            {
                unites = unites.OrderBy(u => u.Numero).ToList();
            }
            else
            {
                unites = unites.OrderByDescending(u => u.Numero).ToList();
            }

            return unites;
        }

        public List<Unite> GetAll()
        {
            return _context.Unites
                .Include(u => u.Residence)
                .ToList();
        }

        public Unite GetById(int id)
        {
            return _context.Unites.Find(id);
        }
        public void Creer(Unite unite)
        {
            _context.Unites.Add(unite);
            _context.SaveChanges();
        }

        public void Modifier(Unite unite)
        {
            _context.Unites.Update(unite);
            _context.SaveChanges();
        }

        public void Supprimer(Unite unite)
        {
            if (unite != null)
            {
                _context.Unites.Remove(unite);
                _context.SaveChanges();
            }
        }

        public bool UniteExiste(int numero, int residenceId)
        {
            return _context.Unites
                .Any(u => u.Numero == numero && u.ResidenceId == residenceId);
        }
        public bool UniteExiste(int? numero, int residenceId, int uniteId)
        {
            return _context.Unites.Any(u =>
                u.Numero == numero &&
                u.ResidenceId == residenceId &&
                u.Id != uniteId
            );
        }
    }
}
