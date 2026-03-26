using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using S14_ProjetSession.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public static class DbInitialisation
    {
        /**
         * Utilisation de Chatgpt pour generer des données
         */
        private static List<Semestre> semestres = new List<Semestre>();

        public static async Task Initialiser(ResidencesDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)


        public static void Initialiser(ResidencesDbContext context)
        {
            await InitialiserRole(roleManager);
            await InitialiserUsers(userManager);

            context.Database.Migrate();

            if (context.Etudiants.Any())
            {
                return;
            }

            var residences = new Residence[]
            {
                new Residence { Nom = "Résidence Maple", Adresse = "100 Rue Maple" },
                new Residence { Nom = "Résidence Oak", Adresse = "200 Rue Oak" }
            };

            context.Residences.AddRange(residences);
            context.SaveChanges();

            var unites = new Unite[]
            {
                new Unite { Numero = 101, Capacite = 2, ResidenceId = residences[0].Id },
                new Unite { Numero = 102, Capacite = 1, ResidenceId = residences[0].Id },
                new Unite { Numero = 201, Capacite = 2, ResidenceId = residences[1].Id },
                new Unite { Numero = 202, Capacite = 3, ResidenceId = residences[1].Id }
            };

            context.Unites.AddRange(unites);
            context.SaveChanges();

            // Genres
            var genres = new Genre[]
            {
           new Genre{ Nom = "Homme" },
           new Genre{ Nom = "Femme" },
           new Genre{ Nom = "Non-binaire"},
           new Genre{ Nom = "Autre" }
            };

            context.Genres.AddRange(genres);
            context.SaveChanges();

            // Campus
            var campus = new Campus[]
            {
        new Campus{ Nom = "Ottawa", Abreviation = "OTT" },
        new Campus{ Nom = "Gatineau", Abreviation = "GAT" }
            };

            context.Campus.AddRange(campus);
            context.SaveChanges();

            // Récupération DB
            var campusDb = context.Campus.ToList();
            var genreDb = context.Genres.ToList();

            // Programmes (AVEC CampusId)
            var programmes = new Programme[]
            {
        new Programme{ Nom = "Techniques de l'informatique", Code = "420.A0", CampusId = campusDb[0].Id },
        new Programme{ Nom = "Sciences de la nature", Code = "200.B0", CampusId = campusDb[0].Id },
        new Programme{ Nom = "Administration des affaires", Code = "410.B0", CampusId = campusDb[1].Id },
        new Programme{ Nom = "Techniques de génie logiciel", Code = "420.B1", CampusId = campusDb[1].Id }
            };

            context.Programmes.AddRange(programmes);
            context.SaveChanges();

            var programmeDb = context.Programmes.ToList();

            // Étudiants
            var etudiants = new Etudiant[]
            {
        new Etudiant
        {
            Nom = "Tremblay",
            Prenom = "Alex",
            DateNaissance = new DateTime(2003, 5, 14),
            GenreId = genreDb[0].Id,
            ProgrammeId = programmeDb[0].Id,
            noEtudiant = "20230001",
            noAdmission = "ADM001",
            MobiliteReduite = false,
            AdressePermanente = "123 Rue Ottawa",
            Telephone = "6131112222",
            CourrielInstitutionnel = "alex.tremblay@college.ca",
            CourrielPersonnel = "alex.tremblay@gmail.com"
        },
        new Etudiant
        {
            Nom = "Gagnon",
            Prenom = "Marie",
            DateNaissance = new DateTime(2002, 11, 2),
            GenreId = genreDb[1].Id,
            ProgrammeId = programmeDb[1].Id,
            noEtudiant = "20230002",
            noAdmission = "ADM002",
            MobiliteReduite = false,
            AdressePermanente = "45 Rue Montreal",
            Telephone = "6133334444",
            CourrielInstitutionnel = "marie.gagnon@college.ca",
            CourrielPersonnel = "marie.gagnon@gmail.com"
        },
        new Etudiant
        {
            Nom = "Nguyen",
            Prenom = "David",
            DateNaissance = new DateTime(2004, 1, 20),
            GenreId = genreDb[0].Id,
            ProgrammeId = programmeDb[2].Id,
            MobiliteReduite = true,
            noEtudiant = "20230003",
            noAdmission = "ADM003",
            AdressePermanente = "78 Rue Gatineau",
            Telephone = "6135556666",
            CourrielInstitutionnel = "david.nguyen@college.ca",
            CourrielPersonnel = "david.nguyen@gmail.com"
        },
        new Etudiant
        {
            Nom = "Bouchard",
            Prenom = "Sophie",
            DateNaissance = new DateTime(2003, 7, 9),
            GenreId = genreDb[1].Id,
            ProgrammeId = programmeDb[3].Id,
            noEtudiant = "20230004",
            noAdmission = "ADM004",
            MobiliteReduite = false,
            AdressePermanente = "90 Rue Quebec",
            Telephone = "6137778888",
            CourrielInstitutionnel = "sophie.bouchard@college.ca",
            CourrielPersonnel = "sophie.bouchard@gmail.com"
        }
            };
            // Felix
            



            List<string> saisons = new List<string>
                {
                    "printemps",
                    "été",
                    "automne",
                    "hiver"
                };
            for (int i = 2025; i < 2035; i++)
            {
                foreach (string saison in saisons)
                {
                    semestres.Add(new Semestre() { NomSemestre = $"{saison}-{i}" });
                }
            }
            context.Semestre.AddRange(semestres);
            context.Etudiants.AddRange(etudiants);
            context.SaveChanges();
        }

        private static async Task InitialiserRole(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Admin", "Utilisateur", "Gestionnaire"];
            foreach (string role in roles) 
            {
                if (!await roleManager.RoleExistsAsync(role))
                    {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    };
            }
        }

        private static async Task InitialiserUsers(UserManager<ApplicationUser> userManager)
        {
            
        }
    }
}
