using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface ICampusRepository
    {
        public IEnumerable<Campus> Campus { get; }
        public Campus? GetById(int id);
    
        public void SupprimerParID(int id);


        public void Creer(Campus campus);


        public void Modifier(Campus campus);


        public void Supprimer(Campus campus);
    }
}
