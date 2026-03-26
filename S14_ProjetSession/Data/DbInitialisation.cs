using System;
using System.Linq;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public static class DbInitialisation
    {
        /**
         * Utilisation de Chatgpt pour generer des données
         */

        public static void Initialiser(ResidencesDbContext context)
        {
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

            // Programmes
            var programmes = new Programme[]
            {
                new Programme{ Nom = "Techniques de l'informatique", Code = "420.A0"},
                new Programme{ Nom = "Sciences de la nature", Code = "200.B0"},
                new Programme{ Nom = "Administration des affaires", Code = "410.B0"},
                new Programme{ Nom = "Techniques de génie logiciel", Code = "420.B1"}
            };

            context.Programmes.AddRange(programmes);
            context.SaveChanges();

            // Étudiants
            var etudiants = new Etudiant[]
            {
                new Etudiant
                {
                    Nom = "Tremblay",
                    Prenom = "Alex",
                    DateNaissance = new DateTime(2003, 5, 14),
                    Genreid = genres[0].Id,
                    ProgrammeId = programmes[0].Id,
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
                    Genreid = genres[1].Id,
                    ProgrammeId = programmes[1].Id,
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
                    Genreid = genres[0].Id,
                    ProgrammeId = programmes[2].Id,
                    noEtudiant = "20230003",
                    noAdmission = "ADM003",
                    MobiliteReduite = true,
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
                    Genreid = genres[1].Id,
                    ProgrammeId = programmes[3].Id,
                    noEtudiant = "20230004",
                    noAdmission = "ADM004",
                    MobiliteReduite = false,
                    AdressePermanente = "90 Rue Quebec",
                    Telephone = "6137778888",
                    CourrielInstitutionnel = "sophie.bouchard@college.ca",
                    CourrielPersonnel = "sophie.bouchard@gmail.com"
                }
            };

            context.Etudiants.AddRange(etudiants);
            context.SaveChanges();
        }
    }
}
