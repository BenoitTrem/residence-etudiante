using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbDemandeRepository : IDemandeRepository
    {
        private ResidencesDbContext _context;
        public List<Demande> Demandes => _context.Demandes.ToList();

        public DbDemandeRepository(ResidencesDbContext context) 
        {
        }
        public void Creer(Demande demande)
        {
            _context.Demandes.Add(demande);
        }

        public Demande? GetDemande(int id)
        {
            return _context.Demandes.FirstOrDefault(d => d.Id == id);
        }

        public void Supprimer(Demande demande)
        {
            _context.Demandes.Remove(demande);
        }
    }
}
