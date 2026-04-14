using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockGenreRepository : IGenresRepository
    {
        private List<Genre> _context;




        public List<Genre> Genres => _context;

        IEnumerable<Genre> IGenresRepository.Genres => Genres;

        public MockGenreRepository()
        {
            _context = new List<Genre>()
            {
                new Genre
                {
                    Nom = "HOMME",
                    Id = 1
                },
                new Genre
                {
                    Nom = "Femme",
                    Id = 2
                },
                new Genre
                {
                    Nom = "Autre",
                    Id = 3
                }
            };
        }


        public Genre? GetGenre(int? id)
        {
            return Genres.FirstOrDefault(f => f.Id == id);
        }

        void IGenresRepository.SupprimerParID(int id)
        {
            throw new NotImplementedException();
        }

        void IGenresRepository.Creer(Genre genre)
        {
            throw new NotImplementedException();
        }

        void IGenresRepository.Modifier(Genre genre)
        {
            throw new NotImplementedException();
        }

        void IGenresRepository.Supprimer(Genre genre)
        {
            throw new NotImplementedException();
        }
    }
}
