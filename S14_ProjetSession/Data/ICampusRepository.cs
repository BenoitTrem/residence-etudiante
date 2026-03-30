using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface ICampusRepository
    {
        public List<Campus> GetAll();
        public Campus? GetById(int id);
    }
}
