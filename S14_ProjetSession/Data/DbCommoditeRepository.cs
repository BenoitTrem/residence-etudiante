using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbCommoditeRepository : ICommoditeRepository
    {
        private readonly ResidencesDbContext _context;

        public DbCommoditeRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Commodite> Commodites => _context.Commodites;

        public Commodite? GetCommodite(int id)
        {
            return Commodites.FirstOrDefault(c => c.Id == id);
        }

        public List<Commodite> GetAll()
        {
            return _context.Commodites.ToList();
        }
        public List<Commodite> GetCommoditeFiltrer(string? nom, bool ascendant = true)
        {
            List<Commodite> commodites = _context.Commodites
            .ToList();

            if (!string.IsNullOrEmpty(nom))
            {
                commodites = commodites
                    .Where(r => r.Nom != null &&
                                r.Nom.ToLower().Contains(nom.ToLower()))
                    .ToList();
            }
            if (ascendant)
            {
                commodites = commodites.OrderBy(r => r.Nom).ToList();
            }
            else
            {
                commodites = commodites.OrderByDescending(r => r.Nom).ToList();
            }

            return commodites;
        }

        public void Ajouter(Commodite commodite)
        {
            _context.Commodites.Add(commodite);
            _context.SaveChanges();
        }

        public void Modifier(Commodite commodite)
        {
            _context.Commodites.Update(commodite);
            _context.SaveChanges();
        }

        public void Supprimer(Commodite commodite)
        {
            _context.Commodites.Remove(commodite);
            _context.SaveChanges();
        }

        public bool NomExiste(string nom, int id = 0)
        {
            return _context.Commodites.Any(c => c.Nom == nom && c.Id != id);
        }
    }
}
