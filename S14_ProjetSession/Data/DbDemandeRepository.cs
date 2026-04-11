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
            .Include(d => d.Jumelages)
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
                .Include(d => d.Jumelages)
                .Include(d => d.DemandeGenres)
                    .ThenInclude(dg => dg.Genre)
                .FirstOrDefault(d => d.Id == id);
        }

        public void Supprimer(Demande demande)
        {
            List<Jumelage> jumelages = _context.Set<Jumelage>()
                .Where(j => EF.Property<int?>(j, "DemandeId") == demande.Id)
                .ToList();

            if (jumelages.Count > 0)
            {
                _context.RemoveRange(jumelages);
            }

            _context.Demandes.Remove(demande);
            _context.SaveChanges();
        }

        public void Modifier(Demande demande)
        {
            // Le contrôleur modifie déjà l'entité trackée (demandeEnBase) directement,
            // donc il suffit de sauvegarder les changements.
            _context.SaveChanges();
        }
    }
}
