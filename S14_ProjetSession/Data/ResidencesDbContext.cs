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
        public DbSet<Commodite> Commodites { get; set; }
        public DbSet<ResidenceCommodite> ResidenceCommodites { get; set; }

         public DbSet<Demande> Demandes { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Semestre> Semestre { get; set; }
        public DbSet<Programme> Programmes { get; set; }

        public DbSet<Campus> Campus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResidenceCommodite>()
                .HasKey(rc => new { rc.ResidenceId, rc.CommoditeId });

            modelBuilder.Entity<ResidenceCommodite>()
                .HasOne(rc => rc.Residence)
                .WithMany(r => r.ResidenceCommodites)
                .HasForeignKey(rc => rc.ResidenceId);

            modelBuilder.Entity<ResidenceCommodite>()
                .HasOne(rc => rc.Commodite)
                .WithMany(c => c.ResidenceCommodites)
                .HasForeignKey(rc => rc.CommoditeId);

        modelBuilder.Entity<Residence>()
                 .OwnsOne(r => r.Adresse);

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

            modelBuilder.Entity<Commodite>()
               .HasIndex(c => c.Nom)
               .IsUnique();
            modelBuilder.Entity<Demande>()
                .HasIndex(d => new {
                    d.EtudiantId,
                    d.SemestreId
                })
                .IsUnique();


    }
            
    }

