using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;

/*
 * @author Benoit 
 */
namespace ResidenceEtudiante.Data
{
    public class DbCommoditeRepository : ICommoditeRepository
    {
        private readonly ResidencesDbContext _context;

        public DbCommoditeRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Commodite> Commodites => _context.Commodites;

        public Commodite? GetCommodite(int id)
        {
            return Commodites.FirstOrDefault(c => c.Id == id);
        }

        public List<Commodite> GetAll()
        {
            return _context.Commodites.ToList();
        }

        /// <summary>
        /// Retourne la liste des commodités avec filtrage par nom et tri (ascendant ou descendant).
        /// </summary>
        /// <param name="nom">Nom (ou partie du nom) utilisé pour filtrer les commodités</param>
        /// <param name="ascendant">Indique si le tri est ascendant (true) ou descendant (false)</param>
        /// <returns>Liste des commodités filtrées et triées</returns>
        public List<Commodite> GetCommoditeFiltrer(string? nom, bool ascendant = true)
        {
            List<Commodite> commodites = _context.Commodites
            .ToList();

            // Filtre par nom si un nom est fourni
            if (!string.IsNullOrEmpty(nom))
            {
                // Vérifie que le nom n'est pas nul et contient la valeur recherchée
                commodites = commodites
                    .Where(r => r.Nom != null && r.Nom.ToLower().Contains(nom.ToLower()))
                    .ToList();
            }

            // Trie la liste selon l'ordre demandé (ascendant ou descendant)
            if (ascendant)
            {
                commodites = commodites.OrderBy(r => r.Nom).ToList();
            }
            else
            {
                commodites = commodites.OrderByDescending(r => r.Nom).ToList();
            }

            return commodites;
        }

        public void Ajouter(Commodite commodite)
        {
            _context.Commodites.Add(commodite);
            _context.SaveChanges();
        }

        public void Modifier(Commodite commodite)
        {
            _context.Commodites.Update(commodite);
            _context.SaveChanges();
        }

        public void Supprimer(Commodite commodite)
        {
            _context.Commodites.Remove(commodite);
            _context.SaveChanges();
        }

        public bool NomExiste(string nom, int id = 0)
        {
            return _context.Commodites.Any(c => c.Nom == nom && c.Id != id);
        }
    }
}
