using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbCampusRepository : ICampusRepository
    {
        private readonly ResidencesDbContext _context;

        public DbCampusRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public List<Campus> GetAll()
        {
            return _context.Campus.ToList();
        }

        public Campus? GetById(int id)
        {
            return _context.Campus.FirstOrDefault(c => c.Id == id);
        }
    }
}
