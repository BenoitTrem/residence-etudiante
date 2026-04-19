using Microsoft.EntityFrameworkCore;
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
                .OrderByDescending(u => u.Capacite > u.PlacesOccupees)
                .ThenBy(u => u.Numero)
                .Include(u => u.Residence)
                .ToList();
        }

        public List<Unite> GetDisponibleByResidenceId(int residenceId)
        {
            return _context.Unites
                .Where(u => u.ResidenceId == residenceId && u.EstDisponible)
                .Include(u => u.Residence)
                .ToList();
        }

        public int GetTotalByResidenceId(int residenceId)
        {
            return _context.Unites.Count(u => u.ResidenceId == residenceId);
        }

        public List<Unite> GetFiltrerByResidenceId(int residenceId, bool? disponible, int? capacite, int? numero, bool ascendant = true, bool? mobiliteReduite = null)
        {
            List<Unite> unites = _context.Unites
                .Where(u => u.ResidenceId == residenceId)
                .Include(u => u.Residence)
                .ToList();

            if (disponible.HasValue)
            {
                unites = unites
                    .Where(u => u.EstDisponible == disponible.Value)
                    .ToList();
            }

            if (capacite.HasValue)
            {
                unites = unites
                    .Where(u => u.Capacite >= capacite.Value)
                    .ToList();
            }

            if (numero.HasValue)
            {
                unites = unites
                    .Where(u => u.Numero.HasValue &&
                                u.Numero.Value >= numero.Value)
                    .ToList();
            }

            if (mobiliteReduite.HasValue)
            {
                unites = unites
                    .Where(u => u.AdapteePourMobiliteReduite == mobiliteReduite.Value)
                    .ToList();
            }

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

        public void Creer(Unite unite)
        {
            _context.Unites.Add(unite);
            _context.SaveChanges();
        }

        public List<Unite> GetAll()
        {
            return _context.Unites.ToList();
        }

        public Unite GetById(int id)
        {
            return _context.Unites.Find(id);
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
