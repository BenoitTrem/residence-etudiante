using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data;
    public class ResidencesDbContext : DbContext
    {
        public ResidencesDbContext(DbContextOptions<ResidencesDbContext> options) : base(options)
        {
        }

        public DbSet<Residence> Residences { get; set; }
        public DbSet<Unite> Unites { get; set; }

        public DbSet<Demande> Demandes { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<PrefDureeBails> PrefDurees { get; set; }
        public DbSet<Programme> Programmes { get; set; }



    }

