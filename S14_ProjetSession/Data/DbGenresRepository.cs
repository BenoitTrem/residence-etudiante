using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;
using System;

namespace S14_ProjetSession.Data
{
    public class DbGenresRepository : IGenresRepository
    {
        private ResidencesDbContext _context;




        public List<Genre> Genres => _context.Genres.ToList();
           

        public DbGenresRepository(ResidencesDbContext contexte)
        {
            _context = contexte;
        }

 
        public Genre? GetGenre(int id)
        {
            return Genres.FirstOrDefault(f => f.Id == id);
        }

     
    
    }
}
