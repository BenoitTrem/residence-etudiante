using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    /// <author>Felix</author>
    public class DbSemestreRepository : ISemestreRepository
    {
        private ResidencesDbContext _context;


        

        public List<Semestre> Semestres => _context.Semestres.ToList();

        public DbSemestreRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public void Creer(Semestre semestre)
        {
            _context.Semestres.Add(semestre);
            _context.SaveChanges();
        }



        public Semestre? GetSemestreParId(int semestreId)
        {
            return Semestres.FirstOrDefault(s => s.Id == semestreId);
        }

        public void Modifier(Semestre semestre)
        {
            _context.Semestres.Update(semestre);
            _context.SaveChanges();
        }

        public void Supprimer(Semestre semestre)
        {
            _context.Semestres.Remove(semestre);
            _context.SaveChanges();
        }

        public void SupprimerParID(int semestreID)
        {
           
        }

        
    }
}
