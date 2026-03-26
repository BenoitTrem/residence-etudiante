using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class DbSemestreRepository : ISemestreRepository
    {
        private ResidencesDbContext _context;


        

        public List<Semestre> Semestres => _context.Semestre.ToList();

        public DbSemestreRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public void Creer(Semestre semestre)
        {
            _context.Semestre.Add(semestre);
            _context.SaveChanges();
        }



        public Semestre? GetSemestreParId(int semestreId)
        {
            return Semestres.FirstOrDefault(s => s.Id == semestreId);
        }

        public void Modifier(Semestre semestre)
        {
            _context.Semestre.Update(semestre);
            _context.SaveChanges();
        }

        public void Supprimer(Semestre semestre)
        {
            _context.Semestre.Remove(semestre);
            _context.SaveChanges();
        }

        public void SupprimerParID(int semestreID)
        {
           
        }

        
    }
}
