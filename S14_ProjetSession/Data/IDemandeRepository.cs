using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IDemandeRepository
    {
        
        public List<Demande> Demandes { get; }
        
        public Demande? GetDemande(int id);
        
        public void Creer(Demande demande);

        public void Supprimer(Demande demande);
    }
}
