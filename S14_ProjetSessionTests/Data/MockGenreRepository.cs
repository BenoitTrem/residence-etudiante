using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockGenreRepository : IGenresRepository
    {
        private List<Genre> _context;




        public List<Genre> Genres => _context;


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


        public Genre? GetGenre(int id)
        {
            return Genres.FirstOrDefault(f => f.Id == id);
        }

    }
}
