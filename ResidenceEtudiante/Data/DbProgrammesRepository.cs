using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;



/// <summary>
/// Repository responsable de la gestion des programmes
/// </summary>
/// <author>John Zuleta</author>
namespace ResidenceEtudiante.Data
{
    public class DbProgrammesRepository : IProgrammesRepository
    {
        private readonly ResidencesDbContext _context;

        public DbProgrammesRepository(ResidencesDbContext contexte)
        {
            _context = contexte;
        }

        public IEnumerable<Programme> Programmes => _context.Programmes.Include(p => p.Campus);

        public Programme? GetProgramme(int? id)
        {
            return _context.Programmes.FirstOrDefault(p => p.Id == id);
        }

        public void Creer(Programme programme)
        {
            _context.Programmes.Add(programme);
            _context.SaveChanges();
        }

        public void Modifier(Programme programme)
        {
            _context.Programmes.Update(programme);
            _context.SaveChanges();
        }

        public void Supprimer(Programme programme)
        {
            _context.Programmes.Remove(programme);
            _context.SaveChanges();
        }

        public void SupprimerParID(int id)
        {
            Programme programme = GetProgramme(id);
            if (programme != null)
            {
                Supprimer(programme);
            }
        }
    }
}