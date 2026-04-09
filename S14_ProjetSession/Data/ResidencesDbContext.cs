using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Diagnostics;
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

    public DbSet<Commodite> Commodites { get; set; } 

    public DbSet<ResidenceCommodite> ResidenceCommodites { get; set; }

    public DbSet<Campus> Campus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

            // @author Benoit
            // Configuration de la table de jointure ResidenceCommodite
            modelBuilder.Entity<ResidenceCommodite>()
                .HasKey(rc => new { rc.ResidenceId, rc.CommoditeId });

            // @author Benoit
            // Relation plusieurs à plusieurs entre Residence et Commodite via ResidenceCommodite
            modelBuilder.Entity<ResidenceCommodite>()
                .HasOne(rc => rc.Residence)   // Chaque entrée de ResidenceCommodite est liée à une seule Residence
                .WithMany(r => r.ResidenceCommodites) // Une Residence peut apparaître dans plusieurs ResidenceCommodites
                .HasForeignKey(rc => rc.ResidenceId);

            // @author Benoit
            // Relation plusieurs à plusieurs entre Residence et Commodite via la table de jointure ResidenceCommodite
            modelBuilder.Entity<ResidenceCommodite>()
                .HasOne(rc => rc.Commodite)  // Chaque entrée de ResidenceCommodite est liée à une seule Commodite
                .WithMany(c => c.ResidenceCommodites) // Une Commodite peut apparaître dans plusieurs ResidenceCommodites
                .HasForeignKey(rc => rc.CommoditeId);

            // @author Benoit
            //Permet de regrouper les champs liés à l'adresse dans l'entité Residence.
            modelBuilder.Entity<Residence>()
                 .OwnsOne(r => r.Adresse);

            // @author Benoit
            //Relation un à plusieurs entre Residence et Unites
            modelBuilder.Entity<Residence>()
                    .HasMany(r => r.Unites) // Une résidence possède plusieurs unités
                    .WithOne(u => u.Residence) // Chaque unité appartient à une seule résidence
                    .HasForeignKey(u => u.ResidenceId)
                    .OnDelete(DeleteBehavior.Cascade);

            // @author Benoit
            // Nom de la résidence doit être unique
            modelBuilder.Entity<Residence>()
                .HasIndex(r => r.Nom)
                .IsUnique();

            // @author Benoit
            // Numéro d'unité unique par résidence
            modelBuilder.Entity<Unite>()
                .HasIndex(u => new { u.Numero, u.ResidenceId })
                .IsUnique();

            // @author Benoit
            // Nom de la commodité doit être unique
            modelBuilder.Entity<Commodite>()
               .HasIndex(c => c.Nom)
               .IsUnique();

            modelBuilder.Entity<Demande>()
                .HasIndex(d => new {
                    d.EtudiantId,
                    d.SemestreId
                })
                .IsUnique();

        // Relation Demande <-> Genre (table de jointure)
        modelBuilder.Entity<DemandeGenre>()
            .HasKey(dg => new { dg.DemandeId, dg.GenreId });

        modelBuilder.Entity<DemandeGenre>()
            .HasOne(dg => dg.Demande)
            .WithMany(d => d.DemandeGenres)
            .HasForeignKey(dg => dg.DemandeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DemandeGenre>()
            .HasOne(dg => dg.Genre)
            .WithMany(g => g.DemandeGenres)
            .HasForeignKey(dg => dg.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Étudiant -> Genre
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Genre)
            .WithMany(g => g.Etudiants)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Étudiant -> Unite
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Unite)
            .WithMany(u => u.Etudiants)
            .HasForeignKey(e => e.UniteId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}

