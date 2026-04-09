using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;


/// <summary>
/// Repository responsable de la gestion des campus
/// </summary>
/// <author>John Zuleta</author>
namespace S14_ProjetSession.Data
{
    public class DbCampusRepository : ICampusRepository
    {
        private readonly ResidencesDbContext _context;

        public DbCampusRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        IEnumerable<Campus> ICampusRepository.Campus => _context.Campus;

        public Campus? GetById(int? id)
        {
            return _context.Campus.FirstOrDefault(c => c.Id == id);
        }

        void ICampusRepository.Creer(Campus campus)
        {
            _context.Campus.Add(campus);
            _context.SaveChanges();
        }

        void ICampusRepository.Modifier(Campus campus)
        {
            _context.Campus.Update(campus);
            _context.SaveChanges();
        }

        void ICampusRepository.Supprimer(Campus campus)
        {
            _context.Campus.Remove(campus);
            _context.SaveChanges();
        }

        void ICampusRepository.SupprimerParID(int id)
        {
            Campus campus = GetById(id);
            _context.Campus.Remove(campus);
            _context.SaveChanges();
        }
    }
}
