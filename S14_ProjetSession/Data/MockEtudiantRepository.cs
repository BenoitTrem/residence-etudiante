using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockEtudiantRepository : IEtudiantRepository
    {
        private readonly List<Etudiant> _etudiants;

        public MockEtudiantRepository()
        {
            _etudiants = new List<Etudiant>
{
    new Etudiant
    {
        Id = 1,
        Nom = "Tremblay",
        Prenom = "Alex",
        DateNaissance = new DateTime(2003, 5, 14),
        GenreId = 1,
        ProgrammeId = 1,
        noEtudiant = "20230001",
        noAdmission = "ADM001",
        MobiliteReduite = false,
        AdressePermanente = "123 rue Principale, Gatineau, QC",
        Telephone = "8191234567",
        CourrielInstitutionnel = "alex.tremblay@cegepoutaouais.qc.ca",
        CourrielPersonnel = "alex.tremblay@gmail.com",
        ApplicationUserId = "468a4852-42ee-4f45-9be5-41422b589904"
    },
    new Etudiant
    {
        Id = 2,
        Nom = "Gagnon",
        Prenom = "Marie",
        DateNaissance = new DateTime(2002, 11, 2),
        GenreId = 1,
        ProgrammeId = 1,
        noEtudiant = "20230002",
        noAdmission = "ADM002",
        MobiliteReduite = false,
        AdressePermanente = "456 avenue du Parc, Ottawa, ON",
        Telephone = "8199876543",
        CourrielInstitutionnel = "marie.gagnon@cegepoutaouais.qc.ca",
        CourrielPersonnel = "marie.gagnon@gmail.com",
        ApplicationUserId = "468a4852-42ee-4f45-9be5-41422b589902"
    }
};
        }

        public IEnumerable<Etudiant> Etudiants
        {
            get
            {
                return _etudiants;
            }
        }

        public Etudiant? GetEtudiant(int id)
        {
            Etudiant? etudiant = _etudiants.FirstOrDefault(e => e.Id == id);
            return etudiant;
        }

        public Task<Etudiant?> GetByUserIdAsync(string userId)
        {
            Etudiant? etudiant = _etudiants.FirstOrDefault(e => e.ApplicationUserId == userId);
            return Task.FromResult(etudiant);
        }

        public void Creer(Etudiant etudiant)
        {
            _etudiants.Add(etudiant);
        }

        public void Modifier(Etudiant etudiant)
        {
            Etudiant? existing = GetEtudiant(etudiant.Id);
            if (existing != null)
            {
                _etudiants.Remove(existing);
                _etudiants.Add(etudiant);
            }
        }

        public void Supprimer(Etudiant etudiant)
        {
            _etudiants.Remove(etudiant);
        }

        public void SupprimerParID(int etudiantID)
        {
            Etudiant? etudiant = GetEtudiant(etudiantID);
            if (etudiant != null)
            {
                _etudiants.Remove(etudiant);
            }
        }
    }
}