using ResidenceEtudiante.Models;

/*
 * @author Benoit 
 */
namespace ResidenceEtudiante.Data
{
    public interface ICommoditeRepository
    {
        IEnumerable<Commodite> Commodites { get; }
        Commodite? GetCommodite(int id);
        public List<Commodite> GetAll();
        public List<Commodite> GetCommoditeFiltrer(string? nom, bool ascendant = true);
        void Ajouter(Commodite commodite);               
        void Modifier(Commodite commodite);                 
        void Supprimer(Commodite commodite);         
        bool NomExiste(string nom, int id = 0);
    }
}
