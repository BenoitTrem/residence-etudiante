using S14_ProjetSession.Models;

/*
 * @author John Zuleta
 * Description: Repository simulé pour les tests de Campus.
 */
namespace S14_ProjetSession.Data
{
    public class MockCampusRepository : ICampusRepository
    {
        private readonly List<Campus> _campus;

        /// <summary>
        /// Initialise le repository avec deux campus de test.
        /// </summary>
        public MockCampusRepository()
        {
            _campus = new List<Campus>
            {
                new Campus { Id = 1, Nom = "Campus Test", Abreviation = "CT" },
                new Campus { Id = 2, Nom = "Campus Deux", Abreviation = "CD" }
            };
        }

        public IEnumerable<Campus> Campus => _campus;

        /// <summary>Retourne un campus par son Id.</summary>
        public Campus? GetById(int? id) =>
            _campus.FirstOrDefault(c => c.Id == id);

        /// <summary>Ajoute un campus avec un Id auto-incrémenté.</summary>
        public void Creer(Campus campus)
        {
            campus.Id = _campus.Any() ? _campus.Max(c => c.Id) + 1 : 1;
            _campus.Add(campus);
        }

        /// <summary>Met à jour le nom et l'abréviation du campus existant.</summary>
        public void Modifier(Campus campus)
        {
            Campus? existant = GetById(campus.Id);
            if (existant == null) return;
            existant.Nom = campus.Nom;
            existant.Abreviation = campus.Abreviation;
        }

        /// <summary>Supprime le campus de la liste.</summary>
        public void Supprimer(Campus campus) =>
            _campus.Remove(campus);

        /// <summary>Supprime un campus par son Id.</summary>
        public void SupprimerParID(int id)
        {
            Campus? campus = GetById(id);
            if (campus != null) _campus.Remove(campus);
        }
    }
}