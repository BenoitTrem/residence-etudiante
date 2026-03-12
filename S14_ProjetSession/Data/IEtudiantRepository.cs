using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IEtudiantRepository
    {

  
        public List<Etudiant> Etudiants { get; }

        public Etudiant? GetEtudiant(int id);

    
        public List<Etudiant> GetEtudiants(int etudiantId);


        public void SupprimerParEtudiant(int etudiantId);

      
        public void Creer(Etudiant etudiant);

       
        public void Modifier(Etudiant etudiant);

   
        public void Supprimer(Etudiant etudiant);
    }
}
