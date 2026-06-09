using ResidenceEtudiante.Models;
using System.Collections.Generic;
using System.Linq;

namespace ResidenceEtudiante.Data
{
    public class MockUniteRepository : IUniteRepository
    {
        private List<Unite> _unites = new List<Unite>();

        public MockUniteRepository()
        {
            _unites.Add(new Unite { Id = 1, Numero = 101, ResidenceId = 1, Capacite = 2 });
            _unites.Add(new Unite { Id = 2, Numero = 102, ResidenceId = 1, Capacite = 4 });
            _unites.Add(new Unite { Id = 3, Numero = 201, ResidenceId = 2, Capacite = 1 });
        }

        public List<Unite> GetByResidenceId(int residenceId)
        {
            return _unites.Where(u => u.ResidenceId == residenceId).ToList();
        }

        public List<Unite> GetDisponibleByResidenceId(int residenceId)
        {
            return _unites.Where(u => u.ResidenceId == residenceId).ToList(); // Simplifié pour le mock
        }

        public int GetTotalByResidenceId(int residenceId)
        {
            return _unites.Count(u => u.ResidenceId == residenceId);
        }

        public List<Unite> GetFiltrerByResidenceId(int residenceId, bool? disponible, int? capacite, int? numero, bool ascendant = true, bool? mobiliteReduite = null)
        {
            return _unites.Where(u => u.ResidenceId == residenceId).ToList();
        }

        public List<Unite> GetAll()
        {
            return _unites;
        }

        public Unite GetById(int id)
        {
            return _unites.FirstOrDefault(u => u.Id == id);
        }

        public void Creer(Unite unite)
        {
            unite.Id = _unites.Any() ? _unites.Max(u => u.Id) + 1 : 1;
            _unites.Add(unite);
        }

        public void Modifier(Unite unite)
        {
            var existing = GetById(unite.Id);
            if (existing != null)
            {
                existing.Numero = unite.Numero;
                existing.Capacite = unite.Capacite;
                existing.ResidenceId = unite.ResidenceId;
            }
        }

        public void Supprimer(Unite unite)
        {
            _unites.Remove(unite);
        }

        public bool UniteExiste(int numero, int residenceId)
        {
            return _unites.Any(u => u.Numero == numero && u.ResidenceId == residenceId);
        }

        public bool UniteExiste(int? numero, int residenceId, int id)
        {
            return _unites.Any(u => u.Numero == numero && u.ResidenceId == residenceId && u.Id != id);
        }

        public bool UniteExiste(int numero, int residenceId, int id)
        {
            return _unites.Any(u => u.Numero == numero && u.ResidenceId == residenceId && u.Id != id);
        }
    }
}
