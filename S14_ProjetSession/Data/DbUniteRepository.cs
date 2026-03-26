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
                .Where(u => u.ResidenceId == residenceId && (u.Capacite - u.PlacesOccupees) > 0)
                .Include(u => u.Residence)
                .ToList();
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
        public bool UniteExiste(int numero, int residenceId, int id)
        {
            return _context.Unites
                .Any(u => u.Numero == numero
                       && u.ResidenceId == residenceId
                       && u.Id != id);
        }
    }
}
