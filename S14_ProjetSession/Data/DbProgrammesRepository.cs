using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;
using System;

namespace S14_ProjetSession.Data
{
    public class DbProgrammesRepository : IProgrammesRepository
    {
        private ResidencesDbContext _context;

        public List<Programme> Programmes => _context.Programmes.ToList();

        public Programme? GetProgramme(int id)
        {
            return Programmes.FirstOrDefault(f => f.Id == id);
        }

        public DbProgrammesRepository(ResidencesDbContext contexte)
        {
            _context = contexte;
        }
    }
}
