using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IGenreRepository
    {
        public List<Genre> Genres { get; }


        public Genre GetGenreParId(int GenreId);




        public void SupprimerParID(int GenreId);




        public void Creer(Genre genre);


        public void Modifier(Genre genre);


        public void Supprimer(Genre genre);
    }
}
