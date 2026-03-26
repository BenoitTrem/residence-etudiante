using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;
using System;

namespace S14_ProjetSession.Data
{
    public class DbEtudiantRepository : IEtudiantRepository
    {
        private ResidencesDbContext _context;




        public IEnumerable<Etudiant> Etudiants => _context.Etudiants
            .Include(e => e.Programme)
            .Include(e => e.Genre);

        public DbEtudiantRepository(ResidencesDbContext contexte)
        {
            _context = contexte;
        }

 
        public Etudiant? GetEtudiant(int id)
        {
            return Etudiants.FirstOrDefault(f => f.Id == id);
        }

        public void Creer(Etudiant etudiant)
        {
            _context.Etudiants.Add(etudiant);
            _context.SaveChanges();
        }

        public void Modifier(Etudiant etudiant)
        {
            _context.Etudiants.Update(etudiant);
            _context.SaveChanges();
        }

        public void Supprimer(Etudiant etudiant)
        {
            _context.Etudiants.Remove(etudiant);
            _context.SaveChanges();
        }

     

        public void SupprimerParID(int etudiantID)
        {
            Etudiant etudiant = GetEtudiant(etudiantID);
            _context.Etudiants.Remove(etudiant);
            _context.SaveChanges();
        }
    
    }
}
