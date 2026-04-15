using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;
using System;

namespace S14_ProjetSession.Data
{

    public class DbResidenceRepository : IResidenceRepository
    {
        private readonly ResidencesDbContext _context;

        public DbResidenceRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public List<Residence> GetAll()
        {
            return _context.Residences
                .Include(r => r.Unites)
                .ToList();
        }

        public Residence GetById(int id)
        {
            return _context.Residences
                .Include(r => r.ResidenceCommodites)
                    .ThenInclude(rc => rc.Commodite)
                .Include(r => r.Unites)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Creer(Residence residence)
        {
            _context.Residences.Add(residence);
            _context.SaveChanges();
        }

        public void Modifier(Residence residence)
        {
            _context.Residences.Update(residence);
            _context.SaveChanges();
        }

        public void Supprimer(Residence residence)
        {
            if (residence != null)
            {
                _context.Residences.Remove(residence);
                _context.SaveChanges();
            }
        }

        public bool NomExiste(string nom)
        {
            return _context.Residences.Any(r => r.Nom == nom);
        }
        public bool NomExiste(string nom, int id)
        {
            return _context.Residences
                .Any(r => r.Nom == nom && r.Id != id);
        }
    }
}

