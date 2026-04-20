using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IResidenceRepository
    {
        public List<Residence> GetAll();

        public Residence GetById(int id);

        public List<Residence> GetResidenceFiltrer(bool? disponible, string? nom, string? adresseLigne, string? ville, bool ascendant = true);

        public void Creer(Residence residence);

        public void Modifier(Residence residence);

        public void Supprimer(Residence residence);

        public bool NomExiste(string nom);
        public bool NomExiste(string nom, int id);
    }
}
