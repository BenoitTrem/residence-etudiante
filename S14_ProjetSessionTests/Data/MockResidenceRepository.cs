using S14_ProjetSession.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;


/*
 * @author Benoit
 * 
 * Description: Mock Repository pour Residence.
 */
namespace S14_ProjetSession.Data
{
    public class MockResidenceRepository : IResidenceRepository
    {
        private List<Residence> _residences;

        public MockResidenceRepository()
        {
            _residences = new List<Residence>
{
              new Residence
                {
                    Id = 1,
                    Nom = "Résidence Maple",
                    AdresseLigne = "100 Rue Maple",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 1A1",
                    ResidenceCommodites = new List<ResidenceCommodite>(),
                    Unites = new List<Unite>
                    {
                        new Unite { Id = 1, Capacite = 2 },
                        new Unite { Id = 2, Capacite = 0 }
                    }
                },
                new Residence
                {
                    Id = 2,
                    Nom = "Résidence Oak",
                    AdresseLigne = "200 Rue Oak",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 2B2",
                    ResidenceCommodites = new List<ResidenceCommodite>(),
                    Unites = new List<Unite>
                    {
                        new Unite { Id = 3, Capacite = 2 },
                        new Unite { Id = 4, Capacite = 0 }
                    }
                }
            };
        }

        public List<Residence> GetAll()
        {
            return _residences;
        }

        public Residence GetById(int id)
        {
            return _residences.FirstOrDefault(r => r.Id == id);
        }

        public void Creer(Residence residence)
        {
            residence.Id = _residences.Max(r => r.Id) + 1;
            _residences.Add(residence);
        }

        public void Modifier(Residence residence)
        {
            Residence existing = GetById(residence.Id);

            if (existing != null)
            {
                existing.Nom = residence.Nom;

                existing.AdresseLigne = residence.AdresseLigne;
                existing.Ville = residence.Ville;
                existing.Province = residence.Province;
                existing.CodePostal = residence.CodePostal;

                existing.ResidenceCommodites.Clear();

                foreach (ResidenceCommodite rc in residence.ResidenceCommodites)
                {
                    existing.ResidenceCommodites.Add(new ResidenceCommodite
                    {
                        CommoditeId = rc.CommoditeId,
                        Description = rc.Description,
                        Residence = existing
                    });
                }
            }
        }

        public void Supprimer(Residence residence)
        {
            if (residence != null)
            {
                _residences.Remove(residence);
            }
        }

        public bool NomExiste(string nom)
        {
            return _residences.Any(r => r.Nom == nom);
        }

        public bool NomExiste(string nom, int id)
        {
            return _residences.Any(r => r.Nom == nom && r.Id != id);
        }

        public List<Residence> GetResidenceFiltrer(bool? disponible, string? nom, string? adresseLigne, string? ville, bool ascendant = true)
        {
            return _residences.ToList();
        }
    }
}