using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

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
                .Include(u => u.Residence)
                .ToList();
        }

        public void Creer(Unite unite)
        {
            throw new NotImplementedException();
        }

        public List<Unite> GetAll()
        {
            throw new NotImplementedException();
        }

        public Unite GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Modifier(Unite unite)
        {
            throw new NotImplementedException();
        }

        public void Supprimer(int id)
        {
            throw new NotImplementedException();
        }
    }
}
