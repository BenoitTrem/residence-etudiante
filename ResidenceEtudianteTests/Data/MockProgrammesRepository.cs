using ResidenceEtudiante.Models;

/*
 * @author John Zuleta
 * Description: Repository simulé pour les tests de Programme.
 */
namespace ResidenceEtudiante.Data
{
    public class MockProgrammesRepository : IProgrammesRepository
    {
        private readonly List<Programme> _programmes;

        /// <summary>
        /// Initialise le repository avec deux programmes de test.
        /// </summary>
        public MockProgrammesRepository()
        {
            _programmes = new List<Programme>
            {
                new Programme { Id = 1, Nom = "Informatique", Code = "420", CampusId = 1 },
                new Programme { Id = 2, Nom = "Administration", Code = "410", CampusId = 1 }
            };
        }

        public IEnumerable<Programme> Programmes => _programmes;

        /// <summary>Retourne un programme par son Id.</summary>
        public Programme? GetProgramme(int? id) =>
            _programmes.FirstOrDefault(p => p.Id == id);

        /// <summary>Ajoute un programme avec un Id auto-incrémenté.</summary>
        public void Creer(Programme programme)
        {
            programme.Id = _programmes.Any() ? _programmes.Max(p => p.Id) + 1 : 1;
            _programmes.Add(programme);
        }

        public void Modifier(Programme programme)
        {
            Programme? existant = GetProgramme(programme.Id);
            if (existant == null) return;
            existant.Nom = programme.Nom;
            existant.Code = programme.Code;
            existant.CampusId = programme.CampusId;
        }

        /// <summary>Supprime le programme de la liste.</summary>
        public void Supprimer(Programme programme) =>
            _programmes.Remove(programme);

        /// <summary>Supprime un programme par son Id.</summary>
        public void SupprimerParID(int id)
        {
            Programme? programme = GetProgramme(id);
            if (programme != null) _programmes.Remove(programme);
        }
    }
}