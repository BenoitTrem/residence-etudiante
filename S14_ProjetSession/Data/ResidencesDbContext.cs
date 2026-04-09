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

    public DbSet<Campus> Campus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Adresse est une propriété possédée
        modelBuilder.Entity<Residence>()
            .OwnsOne(r => r.Adresse);

        // Résidence -> Unites
        modelBuilder.Entity<Residence>()
            .HasMany(r => r.Unites)
            .WithOne(u => u.Residence)
            .HasForeignKey(u => u.ResidenceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Nom unique pour résidence
        modelBuilder.Entity<Residence>()
            .HasIndex(r => r.Nom)
            .IsUnique();

        // Numéro unique par résidence pour l'unité
        modelBuilder.Entity<Unite>()
            .HasIndex(u => new { u.Numero, u.ResidenceId })
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

        // ------------------------
        // Étudiants
        // ------------------------

        // Étudiant -> Campus
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Campus)
            .WithMany(c => c.Etudiants)
            .HasForeignKey(e => e.CampusId)
            .OnDelete(DeleteBehavior.SetNull); 

        // Étudiant -> Programme
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Programme)
            .WithMany(p => p.Etudiants)
            .HasForeignKey(e => e.ProgrammeId)
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

