using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IGenresRepository
    {

  
        public List<Genre> Genres { get; }

        public Genre? GetGenre(int id);

    
      
    }
}
