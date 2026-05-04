using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockSemestreRepository : ISemestreRepository
    {
        
        private List<Semestre> _context;

        public List<Semestre> Semestres => _context;



        public MockSemestreRepository()
        {
            _context = new List<Semestre>();
            List<string> saisons = new List<string>
                {
                    "printemps",
                    "été",
                    "automne",
                    "hiver"
                };
            int Compteur = 0;
            for (int i = 2025; i < 2035; i++)
            {
                foreach (string saison in saisons)
                {
                    Compteur += 1;
                    _context.Add(new Semestre()
                    {
                        Id = Compteur,
                        NomSemestre = $"{saison}-{i}",
                        InscriptionOuverte = i >= 2026
                    });
                    
                }
            }

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
