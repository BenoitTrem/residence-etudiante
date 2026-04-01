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
    }
}
