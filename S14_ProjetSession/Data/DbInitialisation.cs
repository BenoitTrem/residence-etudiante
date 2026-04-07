using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
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
        {
            await InitialiserRole(roleManager);
            await InitialiserUsers(userManager);

            if (context.Etudiants.Any())
            {
                return; // La DB contient déjà des étudiants, on suppose que tout est initialisé
            }

            // Campus
            var campusList = new Campus[]
            {
        new Campus{ Nom = "Campus Gabrielle-Roy", Abreviation = "CGR" },
        new Campus{ Nom = "Campus Félix-Leclerc", Abreviation = "CFL" }
            };

            foreach (var campus in campusList)
            {
                if (!context.Campus.Any(c => c.Nom == campus.Nom))
                {
                    context.Campus.Add(campus);
                }
            }
            context.SaveChanges();

            var campusDb = context.Campus.ToList();

            // Résidences
            var residences = new Residence[]
            {
        new Residence
        {
            Nom = "Résidence Maple",
            CampusId = campusDb[0].Id,
            Adresse = new Adresse
            {
                AdresseString = "100 Rue Maple",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J8X 1A1"
            }
        },
        new Residence
        {
            Nom = "Résidence Oak",
            CampusId = campusDb[1].Id,
            Adresse = new Adresse
            {
                AdresseString = "200 Rue Oak",
                Ville = "Gatineau",
                Province = "QC",
                CodePostal = "J8X 2B2"
            }
        }
            };

            foreach (var residence in residences)
            {
                if (!context.Residences.Any(r => r.Nom == residence.Nom))
                {
                    context.Residences.Add(residence);
                }
            }
            context.SaveChanges();

            var residencesDb = context.Residences.ToList();

            // Unités
            var unites = new Unite[]
            {
        new Unite { Numero = 101, Capacite = 2, ResidenceId = residencesDb[0].Id, AdapteePourMobiliteReduite = true },
        new Unite { Numero = 102, Capacite = 1, ResidenceId = residencesDb[0].Id, AdapteePourMobiliteReduite = false },
        new Unite { Numero = 201, Capacite = 2, ResidenceId = residencesDb[1].Id, AdapteePourMobiliteReduite = true },
        new Unite { Numero = 202, Capacite = 3, ResidenceId = residencesDb[1].Id, AdapteePourMobiliteReduite = true }
            };

            foreach (var unite in unites)
            {
                if (!context.Unites.Any(u => u.Numero == unite.Numero && u.ResidenceId == unite.ResidenceId))
                {
                    context.Unites.Add(unite);
                }
            }
            context.SaveChanges();

            // Genres
            var genres = new Genre[]
            {
        new Genre{ Nom = "Homme" },
        new Genre{ Nom = "Femme" },
        new Genre{ Nom = "Non-binaire" },
        new Genre{ Nom = "Autre" }
            };

            foreach (var genre in genres)
            {
                if (!context.Genres.Any(g => g.Nom == genre.Nom))
                {
                    context.Genres.Add(genre);
                }
            }
            context.SaveChanges();

            var genreDb = context.Genres.ToList();

            // Programmes
            var programmes = new Programme[]
            {
        new Programme{ Nom = "Techniques de l'informatique", Code = "420.A0", CampusId = campusDb[0].Id },
        new Programme{ Nom = "Sciences de la nature", Code = "200.B0", CampusId = campusDb[0].Id },
        new Programme{ Nom = "Administration des affaires", Code = "410.B0", CampusId = campusDb[1].Id },
        new Programme{ Nom = "Techniques de génie logiciel", Code = "420.B1", CampusId = campusDb[1].Id }
            };

            foreach (var programme in programmes)
            {
                if (!context.Programmes.Any(p => p.Code == programme.Code))
                {
                    context.Programmes.Add(programme);
                }
            }
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
            CampusId = campusDb[0].Id,
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
            CampusId = campusDb[0].Id,
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
            CampusId = campusDb[1].Id,
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
            CampusId = campusDb[1].Id,
            noEtudiant = "20230004",
            noAdmission = "ADM004",
            MobiliteReduite = false,
            AdressePermanente = "90 Rue Quebec",
            Telephone = "6137778888",
            CourrielInstitutionnel = "sophie.bouchard@college.ca",
            CourrielPersonnel = "sophie.bouchard@gmail.com"
        },
        new Etudiant
{
    Nom = "Lefevre",
    Prenom = "Julien",
    DateNaissance = new DateTime(2003, 3, 18),
    GenreId = genreDb[0].Id,
    ProgrammeId = programmeDb[1].Id,
    CampusId = campusDb[0].Id,
    noEtudiant = "20230005",
    noAdmission = "ADM005",
    MobiliteReduite = false,
    AdressePermanente = "150 Rue Laval",
    Telephone = "6139990000",
    CourrielInstitutionnel = "julien.lefevre@college.ca",
    CourrielPersonnel = "julien.lefevre@gmail.com"
}

            };

            foreach (var etudiant in etudiants)
            {
                if (!context.Etudiants.Any(e => e.noEtudiant == etudiant.noEtudiant))
                {
                    context.Etudiants.Add(etudiant);
                }
            }
            context.SaveChanges();

            // Semestres
            List<string> saisons = new List<string> { "printemps", "été", "automne", "hiver" };
            for (int i = 2025; i < 2035; i++)
            {
                foreach (string saison in saisons)
                {
                    if (!context.Semestre.Any(s => s.NomSemestre == $"{saison}-{i}"))
                    {
                        semestres.Add(new Semestre() { NomSemestre = $"{saison}-{i}" });
                    }
                }
            }
            context.Semestre.AddRange(semestres);
            context.SaveChanges();

            // Création des utilisateurs liés aux étudiants
            foreach (Etudiant etudiant in context.Etudiants)
            {
                string email = etudiant.CourrielInstitutionnel;

                ApplicationUser? utiliateurExistant = await userManager.FindByEmailAsync(email);

                if (utiliateurExistant == null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(user, "Password-123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Utilisateur");
                        etudiant.ApplicationUserId = user.Id;
                    }
                }
            }
            context.SaveChanges();
        }

        private static async Task InitialiserRole(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Utilisateur", "Gestionnaire" };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    string roleId = role switch
                    {
                        "Admin" => "ADMIN",
                        "Gestionnaire" => "GESTIONNAIRE",
                        "Utilisateur" => "USER",
                        _ => role.ToUpper()
                    };

                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Id = roleId,
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }

        private static async Task InitialiserUsers(UserManager<ApplicationUser> userManager)
        {
            // L'utilisateur Admin
            string emailAdmin = "admin@gmail.com";
            ApplicationUser? admin = await userManager.FindByEmailAsync(emailAdmin);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, "Password-123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                // L'utilisateur Gestionnaire
                string emailGestionnaire = "gestionnaire@gmail.com";
                ApplicationUser? gestionnaire = await userManager.FindByEmailAsync(emailGestionnaire);

                if (gestionnaire == null)
                {
                    gestionnaire = new ApplicationUser
                    {
                        UserName = emailGestionnaire,
                        Email = emailGestionnaire,
                        EmailConfirmed = true
                    };

                    IdentityResult resultGestionnaire = await userManager.CreateAsync(gestionnaire, "Password-123");

                    if (resultGestionnaire.Succeeded)
                    {
                        await userManager.AddToRoleAsync(gestionnaire, "Gestionnaire");
                    }
                }
            }
        }
    }
}