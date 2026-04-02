using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IGenresRepository
    {

  
        public IEnumerable<Genre> Genres { get; }

        public Genre? GetGenre(int id);



        public void SupprimerParID(int id);


        public void Creer(Genre genre);


        public void Modifier(Genre genre);


        public void Supprimer(Genre genre);



    }
}
