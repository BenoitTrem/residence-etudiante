using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IEtudiantRepository
    {

  
        public IEnumerable<Etudiant> Etudiants { get; }

        public Etudiant? GetEtudiant(int id);

        Task<Etudiant?> GetByUserIdAsync(string userId);





        public void SupprimerParID(int etudiantID);
   



        public void Creer(Etudiant etudiant);

       
        public void Modifier(Etudiant etudiant);

   
        public void Supprimer(Etudiant etudiant);
    }
}
