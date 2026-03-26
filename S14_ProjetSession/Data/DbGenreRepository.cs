using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbGenreRepository : IGenreRepository
    {

        private ResidencesDbContext _context;

        public DbGenreRepository(ResidencesDbContext contexte) 
        {
        _context = contexte;
        }

        public List<Genre> Genres => _context.Genres.ToList();

        public void Creer(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public Genre? GetGenreParId(int? GenreId)
        {
            if (GenreId == null)
            {
                return null;
            }
            Genre genre = _context.Genres.FirstOrDefault(G => G.Id == GenreId);
            return genre;
        }

        public void Modifier(Genre genre)
        {
            throw new NotImplementedException();
        }

        public void Supprimer(Genre genre)
        {
            throw new NotImplementedException();
        }

        public void SupprimerParID(int GenreId)
        {
            throw new NotImplementedException();
        }
    }
}
