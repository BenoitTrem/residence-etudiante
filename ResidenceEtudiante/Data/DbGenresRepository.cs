using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;


/// <summary>
/// Repository responsable de la gestion des genres
/// </summary>
/// <author>John Zuleta</author>
namespace ResidenceEtudiante.Data
{
    public class DbGenresRepository : IGenresRepository
    {
        private readonly ResidencesDbContext _context;

        public DbGenresRepository(ResidencesDbContext contexte)
        {
            _context = contexte;
        }

        public IEnumerable<Genre> Genres => _context.Genres;

        public Genre? GetGenre(int? id)
        {
            return _context.Genres.FirstOrDefault(g => g.Id == id);
        }

        public void Creer(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void Modifier(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();
        }

        public void Supprimer(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }

        public void SupprimerParID(int id)
        {
            Genre genre = GetGenre(id);
            if (genre != null)
            {
                Supprimer(genre);
            }
        }
    }
}