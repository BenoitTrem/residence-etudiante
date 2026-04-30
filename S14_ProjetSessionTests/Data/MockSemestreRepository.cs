using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockSemestreRepository : ISemestreRepository
    {
        
        private List<Semestre> _context;

        public List<Semestre> Semestres => _context;



        public MockSemestreRepository()
        {
            DateTime today = DateTime.Today;
            _context = new List<Semestre>
            {
                new Semestre { Id = 1, NomSemestre = "Hiver test", DateDebut = today.AddDays(-5), DateFin = today.AddDays(5), DebutInscriptionDisponible = today.AddDays(-10), FinInscriptionDisponible = today.AddDays(10) },
                new Semestre { Id = 2, NomSemestre = "Printemps fermé", DateDebut = today.AddMonths(-3), DateFin = today.AddMonths(-2), DebutInscriptionDisponible = today.AddMonths(-4), FinInscriptionDisponible = today.AddMonths(-3) },
                new Semestre { Id = 3, NomSemestre = "Été test", DateDebut = today.AddDays(-2), DateFin = today.AddDays(10), DebutInscriptionDisponible = today.AddDays(-1), FinInscriptionDisponible = today.AddDays(15) },
                new Semestre { Id = 4, NomSemestre = "Automne fermé", DateDebut = today.AddMonths(-6), DateFin = today.AddMonths(-5), DebutInscriptionDisponible = today.AddMonths(-7), FinInscriptionDisponible = today.AddMonths(-6) }
            };

        }

        public void Creer(Semestre semestre)
        {
            _context.Add(semestre);
        }



        public Semestre? GetSemestreParId(int semestreId)
        {
            return Semestres.FirstOrDefault(s => s.Id == semestreId);
        }

        public void Modifier(Semestre semestre)
        {
            //_context.Update(semestre);
        }

        public void Supprimer(Semestre semestre)
        {
            _context.Remove(semestre);
        }

        public void SupprimerParID(int semestreID)
        {

        }
    }
}
