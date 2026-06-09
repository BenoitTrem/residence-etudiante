using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;
using System;

namespace ResidenceEtudiante.Data
{

    public class DbResidenceRepository : IResidenceRepository
    {
        private readonly ResidencesDbContext _context;

        public DbResidenceRepository(ResidencesDbContext context)
        {
            _context = context;
        }

        public List<Residence> GetAll()
        {
            return _context.Residences
                .Include(r => r.Unites)
                .ToList();
        }

        public Residence GetById(int id)
        {
            return _context.Residences
                .Include(r => r.ResidenceCommodites)
                    .ThenInclude(rc => rc.Commodite)
                .Include(r => r.Unites)
                .FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Retourne la liste des résidences filtrées selon plusieurs critères et triées par nom.
        /// </summary>
        /// <param name="disponible">Indique si la résidence doit avoir des unités disponibles</param>
        /// <param name="nom">Nom (ou partie du nom) utilisé pour filtrer</param>
        /// <param name="adresseLigne">Adresse utilisée pour filtrer</param>
        /// <param name="ville">Ville utilisée pour filtrer</param>
        /// <param name="ascendant">Indique si le tri est ascendant (true) ou descendant (false)</param>
        /// <returns>Liste des résidences filtrées et triées</returns>
        public List<Residence> GetResidenceFiltrer(bool? disponible, string? nom, string? adresseLigne, string? ville, bool ascendant = true)
        {
            List<Residence> residences = _context.Residences
            .Include(r => r.Unites) 
            .ToList();

            // Filtre selon la disponibilité des unités si précisée
            if (disponible.HasValue)
            {
                if (disponible.Value)
                {
                    // Résidences ayant au moins une unité disponible
                    residences = residences
                        .Where(r => r.TotalUnites > 0)
                        .ToList();
                }
                else
                {
                    // Résidences sans unité disponible
                    residences = residences
                        .Where(r => r.TotalUnites == 0)
                        .ToList();
                }
            }

            // Filtre par nom si une valeur est fournie
            if (!string.IsNullOrEmpty(nom))
            {
                residences = residences
                    .Where(r => r.Nom != null &&
                                r.Nom.ToLower().Contains(nom.ToLower()))
                    .ToList();
            }

            // Filtre par adresse si une valeur est fournie 
            if (!string.IsNullOrEmpty(adresseLigne))
            {
                residences = residences
                    .Where(r => r.AdresseLigne != null &&
                                r.AdresseLigne.ToLower().Contains(adresseLigne.ToLower()))
                    .ToList();
            }

            // Filtre par ville si une valeur est fournie (insensible à la casse)
            if (!string.IsNullOrEmpty(ville))
            {
                residences = residences
                    .Where(r => r.Ville != null &&
                                r.Ville.ToLower().Contains(ville.ToLower()))
                    .ToList();
            }

            // Trie la liste selon l'ordre demandé (ascendant ou descendant)
            if (ascendant)
            {
                residences = residences.OrderBy(r => r.Nom).ToList();
            }
            else
            {
                residences = residences.OrderByDescending(r => r.Nom).ToList();
            }

            return residences;
        }

        public void Creer(Residence residence)
        {
            _context.Residences.Add(residence);
            _context.SaveChanges();
        }

        public void Modifier(Residence residence)
        {
            _context.Residences.Update(residence);
            _context.SaveChanges();
        }

        public void Supprimer(Residence residence)
        {
            if (residence != null)
            {
                _context.Residences.Remove(residence);
                _context.SaveChanges();
            }
        }

        public bool NomExiste(string nom)
        {
            return _context.Residences.Any(r => r.Nom == nom);
        }
        public bool NomExiste(string nom, int id)
        {
            return _context.Residences
                .Any(r => r.Nom == nom && r.Id != id);
        }
    }
}

