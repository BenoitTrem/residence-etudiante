using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data;
    public class ResidencesDbContext : IdentityDbContext<ApplicationUser>
    {
        public ResidencesDbContext(DbContextOptions<ResidencesDbContext> options) : base(options)
        {
        }

        public DbSet<Residence> Residences { get; set; }
        public DbSet<Unite> Unites { get; set; }

        public DbSet<Demande> Demandes { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Semestre> Semestre { get; set; }
        public DbSet<Programme> Programmes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Residence>()
                .HasMany(r => r.Unites)
                .WithOne(u => u.Residence)
                .HasForeignKey(u => u.ResidenceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Residence>()
                .HasIndex(r => r.Nom)
                .IsUnique();

            modelBuilder.Entity<Unite>()
                .HasIndex(u => new { u.Numero, u.ResidenceId })
                .IsUnique();
        }
    }

