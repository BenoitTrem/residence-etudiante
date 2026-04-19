using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IUniteRepository
    {
        List<Unite> GetByResidenceId(int residenceId);

        List<Unite> GetDisponibleByResidenceId(int residenceId);

        int GetTotalByResidenceId(int residenceId);

        List<Unite> GetFiltrerByResidenceId(int residenceId, bool? disponible, int? capacite, int? numero, bool ascendant = true, bool? mobiliteReduite = null);

        public List<Unite> GetAll();

        public Unite GetById(int id);

        public void Creer(Unite unite);

        public void Modifier(Unite unite);

        public void Supprimer(Unite unite);

        bool UniteExiste(int numero, int residenceId);
        bool UniteExiste(int? numero, int residenceId, int id);

    }
}
