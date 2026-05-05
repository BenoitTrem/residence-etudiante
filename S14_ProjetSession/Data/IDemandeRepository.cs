using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    /// <author>Felix</author>
    public interface IDemandeRepository
    {
        
        public List<Demande> Demandes { get; }
        
        public Demande? GetDemande(int id);

        public Demande? GetDerniereDemandeParEtudiant(int etudiantId);
        
        public void Creer(Demande demande);

        public void Modifier(Demande demande);

        public void Supprimer(Demande demande);
    }
}
