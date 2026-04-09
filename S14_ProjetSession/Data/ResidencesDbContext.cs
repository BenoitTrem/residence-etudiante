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
        public DbSet<Semestre> Semestres { get; set; }
        public DbSet<Programme> Programmes { get; set; }

        public DbSet<Commodite> Commodites { get; set; } 

        public DbSet<ResidenceCommodite> ResidenceCommodites { get; set; }

        public DbSet<Campus> Campuses { get; set; }
    public DbSet<DemandeGenre> DemandeGenres { get; set; }

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



        // @authorJohn Zuleta
        // Relation Demande <-> Genre (table de jointure N-N)
        // La clé primaire composite est formée de DemandeId et GenreId
        modelBuilder.Entity<DemandeGenre>()
            .HasKey(dg => new { dg.DemandeId, dg.GenreId });

        // @authorJohn Zuleta
        // Si une Demande est supprimée, ses DemandeGenres associés sont supprimés en cascade
        modelBuilder.Entity<DemandeGenre>()
            .HasOne(dg => dg.Demande)
            .WithMany(d => d.DemandeGenres)
            .HasForeignKey(dg => dg.DemandeId)
            .OnDelete(DeleteBehavior.Cascade);

        // @authorJohn Zuleta
        // Un Genre ne peut pas être supprimé s'il est encore référencé par une Demande
        modelBuilder.Entity<DemandeGenre>()
            .HasOne(dg => dg.Genre)
            .WithMany(g => g.DemandeGenres)
            .HasForeignKey(dg => dg.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // @authorJohn Zuleta
        // Relation Étudiant -> Genre (1-N)
        // Un Genre ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Genre)
            .WithMany(g => g.Etudiants)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // @authorJohn Zuleta
        // Relation Étudiant -> Unite (1-N)
        // Si une Unité est supprimée, le champ UniteId de l'Étudiant est mis à null
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Unite)
            .WithMany(u => u.Etudiants)
            .HasForeignKey(e => e.UniteId)
            .OnDelete(DeleteBehavior.SetNull);

        // @authorJohn Zuleta
        // Relation Étudiant -> Campus (1-N)
        // Un Campus ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Campus)
            .WithMany(c => c.Etudiants)
            .HasForeignKey(e => e.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        // @authorJohn Zuleta
        // Relation Étudiant -> Programme (1-N)
        // Un Programme ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Programme)
            .WithMany(p => p.Etudiants)
            .HasForeignKey(e => e.ProgrammeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

