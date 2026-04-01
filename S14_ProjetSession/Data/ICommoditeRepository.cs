using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface ICommoditeRepository
    {
        IEnumerable<Commodite> Commodites { get; }
        Commodite? GetCommodite(int id);
    }
}
