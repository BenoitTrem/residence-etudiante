using Microsoft.EntityFrameworkCore;
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
            context.Database.Migrate();

            if (context.Etudiants.Any())
            {
                return;
            }

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

            context.Etudiants.AddRange(etudiants);
            context.SaveChanges();
        }
    }
}
