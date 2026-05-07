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
    // DbSet<DemandeGenre> supprimé : la relation N-N est maintenant implicite via EF Core

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
            .HasOne(rc => rc.Residence)
            .WithMany(r => r.ResidenceCommodites)
            .HasForeignKey(rc => rc.ResidenceId);

        // @author Benoit
        // Relation plusieurs à plusieurs entre Residence et Commodite via la table de jointure ResidenceCommodite
        modelBuilder.Entity<ResidenceCommodite>()
            .HasOne(rc => rc.Commodite)
            .WithMany(c => c.ResidenceCommodites)
            .HasForeignKey(rc => rc.CommoditeId);

        // @author Benoit
        // Relation un à plusieurs entre Residence et Unites
        modelBuilder.Entity<Residence>()
            .HasMany(r => r.Unites)
            .WithOne(u => u.Residence)
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

        modelBuilder.Entity<Demande>()
        .HasOne(d => d.Unite)
        .WithMany(u => u.Demandes)
        .HasForeignKey(d => d.UniteId)
        .OnDelete(DeleteBehavior.SetNull);

        // @author John Zuleta
        // Relation Demande <-> Genre plusieurs à plusieurs implicite (sans classe de jointure)
        // EF Core génère automatiquement la table de jointure
        modelBuilder.Entity<Demande>()
            .HasMany(d => d.DemandeGenres)
            .WithMany(g => g.DemandeGenres);

        // @author John Zuleta
        // Relation Étudiant -> Genre (1-N)
        // Un Genre ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Genre)
            .WithMany(g => g.Etudiants)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // @author John Zuleta
        // Relation Étudiant -> Unite (1-N)
        // Si une Unité est supprimée, le champ UniteId de l'Étudiant est mis à null
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Unite)
            .WithMany(u => u.Etudiants)
            .HasForeignKey(e => e.UniteId)
            .OnDelete(DeleteBehavior.SetNull);

        // @author John Zuleta
        // Relation Étudiant -> Campus (1-N)
        // Un Campus ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Campus)
            .WithMany(c => c.Etudiants)
            .HasForeignKey(e => e.CampusId)
            .OnDelete(DeleteBehavior.SetNull);

        // @author John Zuleta
        // Relation Étudiant -> Programme (1-N)
        // Un Programme ne peut pas être supprimé s'il est encore associé à un Étudiant
        modelBuilder.Entity<Etudiant>()
            .HasOne(e => e.Programme)
            .WithMany(p => p.Etudiants)
            .HasForeignKey(e => e.ProgrammeId)
            .OnDelete(DeleteBehavior.SetNull);

        // @author Felix
        // Suppression en cascade des demandes lors de la suppression d'un étudiant
        modelBuilder.Entity<Demande>()
            .HasOne(d => d.Etudiant)
            .WithMany(e => e.Demandes)
            .HasForeignKey(d => d.EtudiantId)
            .OnDelete(DeleteBehavior.Cascade);

        // @author Felix
        // Suppression en cascade des jumelages lors de la suppression d'une demande
        modelBuilder.Entity<Jumelage>()
            .HasOne<Demande>()
            .WithMany(d => d.Jumelages)
            .HasForeignKey("DemandeId")
            .OnDelete(DeleteBehavior.Cascade);

        // @author John
        // Mise à null du campus dans les programmes lors de la suppression d'un campus
        modelBuilder.Entity<Programme>()
            .HasOne(p => p.Campus)
            .WithMany(c => c.programmes)
            .HasForeignKey(p => p.CampusId)
            .OnDelete(DeleteBehavior.SetNull);

        // @author Jhon
        // Mise à null du campus dans les résidences lors de la suppression d'un campus
        modelBuilder.Entity<Residence>()
            .HasOne<Campus>()
            .WithMany(c => c.Residences)
            .HasForeignKey("CampusId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}