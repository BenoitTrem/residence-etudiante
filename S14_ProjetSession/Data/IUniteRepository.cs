using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IUniteRepository
    {
        List<Unite> GetByResidenceId(int residenceId);

        public List<Unite> GetAll();

        public Unite GetById(int id);

        public void Creer(Unite unite);

        public void Modifier(Unite unite);

        public void Supprimer(int id);
        
    }
}
