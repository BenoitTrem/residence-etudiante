using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbDemandeRepository : IDemandeRepository
    {
        private ResidencesDbContext _context;
        public List<Demande> Demandes => _context.Demandes.Include(E => E.Etudiant)
            .Include(S => S.Semestre)
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
            return _context.Demandes.FirstOrDefault(d => d.Id == id);
        }

        public void Supprimer(Demande demande)
        {
            _context.Demandes.Remove(demande);
            _context.SaveChanges();
        }

        public void Modifier(Demande demande)
        {
            
            Demande? demandeExistante = GetDemande(demande.Id);
            if (demandeExistante != null) 
            {
                //TODO fix temporaire
                demande.EtudiantId = demandeExistante.EtudiantId;
                _context.Entry(demandeExistante).CurrentValues.SetValues(demande);
            }
            _context.SaveChanges();
        }
    }
}
