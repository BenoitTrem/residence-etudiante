using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbDemandeRepository : IDemandeRepository
    {
        private ResidencesDbContext _context;

        public List<Demande> Demandes => _context.Demandes
            .Include(d => d.Etudiant)
            .Include(d => d.Semestre)
            .Include(d => d.DemandeGenres)
                .ThenInclude(dg => dg.Genre)
            .ToList();

        public DbDemandeRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public void Creer(Demande demande)
        {
            _context.Demandes.Add(demande);
            _context.SaveChanges();
        }

        public Demande? GetDemande(int id)
        {
            return _context.Demandes
                .Include(d => d.Etudiant)
                .Include(d => d.Semestre)
                .Include(d => d.DemandeGenres)
                    .ThenInclude(dg => dg.Genre)
                .FirstOrDefault(d => d.Id == id);
        }

        public void Supprimer(Demande demande)
        {
            _context.Demandes.Remove(demande);
            _context.SaveChanges();
        }

        public void Modifier(Demande demande)
        {
            _context.Demandes.Update(demande);
            _context.SaveChanges();
        }
    }
}
