using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Areas.Identity.Data;
using ResidenceEtudiante.Models;
using System;
using System.Linq;
using System.Net;


namespace ResidenceEtudiante.Data
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
                new Campus{ Nom = "Campus Gabrielle-Roy", Abreviation = "CGR" , Priorite = 1 }, 
                new Campus{ Nom = "Campus Félix-Leclerc", Abreviation = "CFL" ,  Priorite = 2 },
                new Campus{ Nom = "Campus Louis-Reboul", Abreviation = "CLR", Priorite = 3 },
                new Campus{ Nom = "Campus de Papineauville", Abreviation = "CP" , Priorite = 2 },
                new Campus{ Nom = "Centre de formation continue", Abreviation = "CFC" ,  Priorite =3 }
            };

            foreach (var campus in campusList)
            {
                if (!context.Campuses.Any(c => c.Nom == campus.Nom))
                {
                    context.Campuses.Add(campus);
                }
            }
            context.SaveChanges();

            var campusDb = context.Campuses.ToList();

            // Résidences
            var residences = new Residence[]
            {
                new Residence
                {
                    Nom = "Résidence Maple",
                    AdresseLigne = "100 Rue Maple",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 1A1"
                },
                new Residence
                {
                    Nom = "Résidence Oak",
                    AdresseLigne = "200 Rue Oak",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 2B2"
                },
                new Residence
                {
                    Nom = "Résidence Pine",
                    AdresseLigne = "300 Rue Pine",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 3C3"
                },
                new Residence
                {
                    Nom = "Résidence Cedar",
                    AdresseLigne = "400 Rue Cedar",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 4D4"
                },
                new Residence
                {
                    Nom = "Résidence Birch",
                    AdresseLigne = "500 Rue Birch",
                    Ville = "Gatineau",
                    Province = "QC",
                    CodePostal = "J8X 5E5"
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
            var unites = new List<Unite>();
            var random = new Random();

            // Nombre d’unités par résidence 
            var nbUnitesParResidence = new Dictionary<int, int>
            {
                { 0, 15 }, // Résidence Maple
                { 1, 5 },  // Résidence Oak
                { 2, 8 },  // Résidence Pine
                { 3, 6 },  // Résidence Cedar
                { 4, 10 }  // Résidence Birch
            };

            foreach (var entry in nbUnitesParResidence)
            {
                int index = entry.Key;
                int nbUnites = entry.Value;

                int numero = 1;

                for (int i = 0; i < nbUnites; i++)
                {
                    unites.Add(new Unite
                    {
                        Numero = numero++,
                        Capacite = random.Next(1, 6),
                        ResidenceId = residencesDb[index].Id,
                        AdapteePourMobiliteReduite = random.Next(0, 5) == 0
                    });
                }
            }

            // Ajout en base (évite les doublons)
            foreach (var unite in unites)
            {
                if (!context.Unites.Any(u =>
                    u.Numero == unite.Numero &&
                    u.ResidenceId == unite.ResidenceId))
                {
                    context.Unites.Add(unite);
                }
            }

            context.SaveChanges();

            // Genres
            var genres = new Genre[]
            {
                new Genre { Nom = "Homme" },
                new Genre { Nom = "Femme" },
                new Genre { Nom = "Autre" }
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
                // Campus Gabrielle-Roy (0) - Arts, lettres, sciences
                new Programme{ Nom = "Arts visuels - Approche Matière", Code = "510.AM", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts visuels - Technologie de l'image", Code = "510.T1", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts, lettres et communication - Option Cinéma", Code = "500.AG", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts, lettres et communication - Option Langues", Code = "500.AL", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts, lettres et communication - Option Littérature", Code = "500.AH", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts, lettres et communication - Option Médias", Code = "500.AJ", CampusId = campusDb[0].Id },
                new Programme{ Nom = "Arts, lettres et communication - Option Théâtre", Code = "500.AK", CampusId = campusDb[0].Id },

              // Campus Félix-Leclerc (1) - Sciences & techniques de santé
              new Programme{ Nom = "Sciences de la nature", Code = "200.B1", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Sciences humaines", Code = "300.M0", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Sciences, lettres et arts (SLA)", Code = "700.A1", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Soins infirmiers", Code = "180.A0", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Soins préhospitaliers d'urgence", Code = "181.A1", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Technique d'éducation spécialisée", Code = "351.A0", CampusId = campusDb[1].Id },
              new Programme{ Nom = "Technique de la documentation", Code = "393.B0", CampusId = campusDb[1].Id },

               // Campus Louis-Reboul (2) - Techniques sociales & santé
               new Programme{ Nom = "Techniques d'éducation à l'enfance", Code = "322.A1", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques d'hygiène dentaire", Code = "111.B0", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques d'inhalothérapie", Code = "141.A0", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques d'intégration multimédia", Code = "582.A1", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques d'administration et de gestion", Code = "410.G0", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques de design d'intérieur", Code = "570.E0", CampusId = campusDb[2].Id },
               new Programme{ Nom = "Techniques de diététique", Code = "120.A0", CampusId = campusDb[2].Id },

               // Campus de Papineauville (3) - Techniques industrielles & juridiques
               new Programme{ Nom = "Techniques de génie mécanique", Code = "241.A0", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques de gestion et d'intervention en loisir", Code = "391.A0", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques de l'informatique", Code = "420.B0", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques de laboratoire (biotechnologies)", Code = "210.AA", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques en travail social", Code = "388.A1", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques juridiques", Code = "310.C0", CampusId = campusDb[3].Id },
               new Programme{ Nom = "Techniques policières", Code = "310.A0", CampusId = campusDb[3].Id },

               // Centre de formation continue (4) - Technologies du bâtiment & génie
               new Programme{ Nom = "Technologie d'analyses biomédicales", Code = "140.C0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie de génie électrique - électronique programmable", Code = "243.G0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie de l'architecture", Code = "221.A0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie de la géomatique", Code = "230.B0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie de radiodiagnostic", Code = "142.H0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie du génie civil", Code = "221.B0", CampusId = campusDb[4].Id },
               new Programme{ Nom = "Technologie du génie du bâtiment", Code = "221.C0", CampusId = campusDb[4].Id },
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
                    if (!context.Semestres.Any(s => s.NomSemestre == $"{saison}-{i}"))
                    {
                        DateTime debut, fin;
                        switch (saison)
                        {
                            case "hiver":
                                debut = new DateTime(i, 1, 1);
                                fin = new DateTime(i, 4, 30);
                                break;
                            case "printemps":
                                debut = new DateTime(i, 5, 1);
                                fin = new DateTime(i, 6, 30);
                                break;
                            case "été":
                                debut = new DateTime(i, 7, 1);
                                fin = new DateTime(i, 8, 31);
                                break;
                            case "automne":
                                debut = new DateTime(i, 9, 1);
                                fin = new DateTime(i, 12, 31);
                                break;
                            default:
                                debut = DateTime.Now;
                                fin = DateTime.Now;
                                break;
                        }
                        semestres.Add(new Semestre() 
                        { 
                            NomSemestre = $"{saison}-{i}",
                            DateDebut = debut,
                            DateFin = fin,
                            InscriptionOuverte = saison == "printemps" && i == 2026
                        });
                    }
                }
            }

            context.Semestres.AddRange(semestres);
            context.SaveChanges();

            // Parcourt une liste d'étudiants et crée un compte utilisateur pour chacun
            // dans ASP.NET Identity si aucun compte n'existe déjà.
            // Associe ensuite l'utilisateur au rôle "Utilisateur".
            foreach (Etudiant etudiant in etudiants)
            {
                // Récupère l'email institutionnel de l'étudiant
                string email = etudiant.CourrielInstitutionnel;

                // Vérifie si un utilisateur avec cet email existe déjà
                ApplicationUser? utiliateurExistant = await userManager.FindByEmailAsync(email);

                if (utiliateurExistant == null)
                {
                    // Si aucun utilisateur n'existe, crée un nouveau compte
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = email, // Nom d'utilisateur = email
                        Email = email, // Email de l'utilisateur
                        EmailConfirmed = true
                    };

                    IdentityResult result = await userManager.CreateAsync(user, "Password-123");

                    if (result.Succeeded)
                    {
                        // Ajoute l'utilisateur au rôle "Utilisateur"
                        await userManager.AddToRoleAsync(user, "Utilisateur");

                        // Lie l'utilisateur à l'entité Etudiant
                        etudiant.ApplicationUserId = user.Id;
                    }
                }
            }
            context.SaveChanges();

            var commodites = new Commodite[]
            {
                new Commodite { Nom = "WiFi" },
                new Commodite { Nom = "Salle de sport" },
                new Commodite { Nom = "Stationnement" },
                new Commodite { Nom = "Buanderie" },
                new Commodite { Nom = "Piscine" },
                new Commodite { Nom = "Sécurité 24h" }
            };

            context.Commodites.AddRange(commodites);
            context.SaveChanges();

          
            var residenceCommodites = new ResidenceCommodite[]
            {
                new ResidenceCommodite { ResidenceId = residencesDb[0].Id, CommoditeId = commodites[0].Id, Description = "WiFi rapide dans toutes les chambres" },
                new ResidenceCommodite { ResidenceId = residencesDb[0].Id, CommoditeId = commodites[1].Id, Description = "Salle de sport ouverte 6h-22h" },
                new ResidenceCommodite { ResidenceId = residencesDb[0].Id, CommoditeId = commodites[3].Id, Description = "Buanderie avec 4 machines" },

                new ResidenceCommodite { ResidenceId = residencesDb[1].Id, CommoditeId = commodites[0].Id, Description = "WiFi illimité" },
                new ResidenceCommodite { ResidenceId = residencesDb[1].Id, CommoditeId = commodites[2].Id, Description = "Stationnement extérieur gratuit" },
                new ResidenceCommodite { ResidenceId = residencesDb[1].Id, CommoditeId = commodites[5].Id}
            };

            context.ResidenceCommodites.AddRange(residenceCommodites);
            context.SaveChanges();

            // Seed des demandes pour démonstration
            List<Etudiant> etudiantsDb = context.Etudiants.ToList();
            List<Semestre> semestresDb = context.Semestres.ToList();
            List<Genre> genresDb = context.Genres.ToList();
            List<Unite> unitesDb = context.Unites.ToList();

            var demandes = new Demande[]
            {
                new Demande
                {
                    EtudiantId = etudiantsDb[0].Id, // Alex Tremblay
                    SemestreId = semestresDb[0].Id, // printemps-2025
                    PrefDureeBail = 365,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-10),
                    NomGarant = "Tremblay Père",
                    PrenomGarant = "Jean",
                    DateNaissanceGarant = new DateTime(1970, 1, 1),
                    CourrielGarant = "jean.tremblay@gmail.com",
                    TelephoneGarant = "6131112222",
                    NomParent = "Tremblay Mère",
                    CourrielParent = "mere.tremblay@gmail.com",
                    NomUrgence = "Gagnon Marie",
                    LienParenteUrgence = "Soeur",
                    TelephoneUrgence = "6133334444",
                    StatutDemande = StatutDemande.EnAttente,
                    Jumelages = new List<Jumelage>
                    {
                        new Jumelage { Nom = "Marie Gagnon", Courriel = "marie.gagnon@college.ca" }
                    },
                    DemandeGenres = new List<Genre>
                    {
                       genresDb[0]
                    }
                },
                new Demande
                {
                    EtudiantId = etudiantsDb[1].Id, // Marie Gagnon
                    SemestreId = semestresDb[1].Id, // été-2025
                    PrefDureeBail = 180,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-5),
                    NomGarant = "Gagnon Père",
                    PrenomGarant = "Pierre",
                    DateNaissanceGarant = new DateTime(1965, 5, 10),
                    CourrielGarant = "pierre.gagnon@gmail.com",
                    TelephoneGarant = "6133334444",
                    NomParent = "Gagnon Mère",
                    CourrielParent = "mere.gagnon@gmail.com",
                    NomUrgence = "Tremblay Alex",
                    LienParenteUrgence = "Frère",
                    TelephoneUrgence = "6131112222",
                    StatutDemande = StatutDemande.Acceptee,
                    DateTraitement = DateTime.Now.AddDays(-2),
                    UniteId = unitesDb[0].Id, // Assignée à une unité
                    Jumelages = new List<Jumelage>
                    {
                        new Jumelage { Nom = "Sophie Bouchard", Courriel = "sophie.bouchard@college.ca" }
                    },
                    // Demande 2 - Marie Gagnon
                    DemandeGenres = new List<Genre> { genresDb[1] } // Femme
                },
                new Demande
                {
                    EtudiantId = etudiantsDb[2].Id, // David Nguyen
                    SemestreId = semestresDb[2].Id, // automne-2025
                    PrefDureeBail = 270,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-7),
                    NomGarant = "Nguyen Père",
                    PrenomGarant = "Viet",
                    DateNaissanceGarant = new DateTime(1975, 3, 15),
                    CourrielGarant = "viet.nguyen@gmail.com",
                    TelephoneGarant = "6135556666",
                    NomUrgence = "Bouchard Sophie",
                    LienParenteUrgence = "Amie",
                    TelephoneUrgence = "6137778888",
                    StatutDemande = StatutDemande.Refusee,
                    DateTraitement = DateTime.Now.AddDays(-1),
                    Jumelages = new List<Jumelage>(),
                     // Demande 3 - David Nguyen
                    DemandeGenres = new List<Genre> { genresDb[0] } // Homme
                },
                new Demande
                {
                    EtudiantId = etudiantsDb[3].Id, // Sophie Bouchard
                    SemestreId = semestresDb[3].Id, // hiver-2025
                    PrefDureeBail = 90,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-3),
                    NomGarant = "Bouchard Père",
                    PrenomGarant = "Michel",
                    DateNaissanceGarant = new DateTime(1968, 7, 20),
                    CourrielGarant = "michel.bouchard@gmail.com",
                    TelephoneGarant = "6137778888",
                    NomParent = "Bouchard Mère",
                    CourrielParent = "mere.bouchard@gmail.com",
                    NomUrgence = "Nguyen David",
                    LienParenteUrgence = "Ami",
                    TelephoneUrgence = "6135556666",
                    StatutDemande = StatutDemande.EnAttente,
                    Jumelages = new List<Jumelage>
                    {
                        new Jumelage { Nom = "Alex Tremblay", Courriel = "alex.tremblay@college.ca" },
                        new Jumelage { Nom = "Marie Gagnon", Courriel = "marie.gagnon@college.ca" }
                    },
                    DemandeGenres = new List<Genre> { genresDb[1] } // Femme
                },
                new Demande
                {
                    EtudiantId = etudiantsDb[4].Id, // Julien Lefevre
                    SemestreId = semestresDb[4].Id, // printemps-2026
                    PrefDureeBail = 365,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-1),
                    NomGarant = "Lefevre Père",
                    PrenomGarant = "Paul",
                    DateNaissanceGarant = new DateTime(1972, 9, 5),
                    CourrielGarant = "paul.lefevre@gmail.com",
                    TelephoneGarant = "6139990000",
                    NomUrgence = "Tremblay Alex",
                    LienParenteUrgence = "Cousin",
                    TelephoneUrgence = "6131112222",
                    StatutDemande = StatutDemande.EnAttente,
                    Jumelages = new List<Jumelage>(),
                    DemandeGenres = new List<Genre> { genresDb[0] } // Homme
                },
                // Nouvelles demandes pour le premier semestre (printemps-2025)
                new Demande
                {
                    EtudiantId = etudiantsDb[3].Id, // Sophie Bouchard
                    SemestreId = semestresDb[0].Id, // printemps-2025
                    PrefDureeBail = 180,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-2),
                    NomGarant = "Bouchard Père",
                    PrenomGarant = "Michel",
                    DateNaissanceGarant = new DateTime(1968, 7, 20),
                    CourrielGarant = "michel.bouchard@gmail.com",
                    TelephoneGarant = "6137778888",
                    NomUrgence = "Nguyen David",
                    LienParenteUrgence = "Ami",
                    TelephoneUrgence = "6135556666",
                    StatutDemande = StatutDemande.EnAttente,
                    Jumelages = new List<Jumelage>
                    {
                        new Jumelage { Nom = "David Nguyen", Courriel = "david.nguyen@college.ca" }
                    },
                    DemandeGenres = new List<Genre> { genresDb[1] } // Femme
                },
                new Demande
                {
                    EtudiantId = etudiantsDb[2].Id, // David Nguyen
                    SemestreId = semestresDb[0].Id, // printemps-2025
                    PrefDureeBail = 270,
                    AccepteReglements = true,
                    AccepteTraitementDonnees = true,
                    ConfirmeSoumission = true,
                    DateDemande = DateTime.Now.AddDays(-3),
                    NomGarant = "Nguyen Père",
                    PrenomGarant = "Viet",
                    DateNaissanceGarant = new DateTime(1975, 3, 15),
                    CourrielGarant = "viet.nguyen@gmail.com",
                    TelephoneGarant = "6135556666",
                    NomUrgence = "Bouchard Sophie",
                    LienParenteUrgence = "Amie",
                    TelephoneUrgence = "6137778888",
                    StatutDemande = StatutDemande.EnAttente,
                    Jumelages = new List<Jumelage>(),
                    DemandeGenres = new List<Genre> { genresDb[0] } // Homme
                }
            };

            context.Demandes.AddRange(demandes);
            context.SaveChanges();

        }

        /// <summary>
        /// @author Benoit, Felix, John
        /// Initialise les rôles par défaut dans l'application Identity.
        /// Crée les rôles "Admin", "Utilisateur" et "Gestionnaire" si ils n'existent pas.
        /// </summary>
        /// <param name="roleManager">RoleManager pour gérer les rôles</param>
        /// <returns>Une tâche asynchrone</returns>
        private static async Task InitialiserRole(RoleManager<IdentityRole> roleManager)
        {
            // Tableau des rôles à créer
            string[] roles = { "Admin", "Utilisateur", "Gestionnaire" };

            foreach (string role in roles)
            {
                // Vérifie si le rôle existe déjà
                if (!await roleManager.RoleExistsAsync(role))
                {
                    // Définit un identifiant personnalisé pour les rôles
                    string roleId = role switch
                    {
                        "Admin" => "ADMIN",
                        "Gestionnaire" => "GESTIONNAIRE",
                        "Utilisateur" => "USER",
                        _ => role.ToUpper()
                    };

                    // Création du rôle dans la base
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Id = roleId,
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }

        /// <summary>
        /// @author Benoit
        /// Initialise les utilisateurs par défaut dans l'application Identity.
        /// Crée un utilisateur Admin et un utilisateur Gestionnaire si ils n'existent pas.
        /// </summary>
        /// <param name="userManager">UserManager pour gérer les utilisateurs</param>
        /// <returns>Une tâche asynchrone</returns>
        private static async Task InitialiserUsers(UserManager<ApplicationUser> userManager)
        {
            // L'utilisateur Admin
            string emailAdmin = "admin@gmail.com";

            // Vérifie si l'utilisateur existe déjà
            ApplicationUser? admin = await userManager.FindByEmailAsync(emailAdmin);

            if (admin == null)
            {
                // Si l'utilisateur n'existe pas, il est crée
                admin = new ApplicationUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true
                };

                // Création de l'utilisateur avec un mot de passe par défaut
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
