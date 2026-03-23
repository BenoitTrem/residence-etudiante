using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IResidenceRepository
    {
        public List<Residence> GetAll();

        public Residence GetById(int id);

        public void Creer(Residence residence);

        public void Modifier(Residence residence);

        public void Supprimer(int id);

     
    }
}
