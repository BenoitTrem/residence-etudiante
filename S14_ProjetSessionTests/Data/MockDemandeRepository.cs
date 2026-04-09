using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public class MockDemandeRepository : IDemandeRepository
    {
        private List<Demande> _demandes = new List<Demande>();
        public List<Demande> Demandes => _demandes;

        public MockDemandeRepository()
        {
            _demandes.Add(new Demande
            {
                Id = 1,
                SemestreId = 1,
                EtudiantId = 1,
                Etudiant = new Etudiant
                {
                    Id = 1,
                    Nom = "Tremblay",
                    Prenom = "Alex",
                    DateNaissance = new DateTime(2003, 5, 14),
                    noEtudiant = "20230001",
                    noAdmission = "ADM001",
                    ApplicationUserId = "468a4852-42ee-4f45-9be5-41422b589904"
                },
                Semestre = new Semestre
                {
                    Id = 1,
                    NomSemestre = "hivers-2025"
                },
                // Relation N-N : DemandeGenres
                DemandeGenres = new List<DemandeGenre>
                {
                    new DemandeGenre { DemandeId = 1, GenreId = 1 },
                    new DemandeGenre { DemandeId = 1, GenreId = 2 }
                },
                PrefDureeBail = 120,
                AccepteReglements = true,
                AccepteTraitementDonnees = true,
                ConfirmeSoumission = true,
                DateDemande = DateTime.Now,
                NomGarant = "Martin",
                PrenomGarant = "Jean",
                DateNaissanceGarant = new DateTime(1970, 5, 12),
                CourrielGarant = "jean.martin@email.com",
                TelephoneGarant = "8191112222",
                NomParent = "Luc Martin",
                CourrielParent = "luc.martin@email.com",
                NomUrgence = "Marie Martin",
                LienParenteUrgence = "Mère",
                TelephoneUrgence = "8193334444",
                Jumelages = new List<Jumelage>()
            });

            _demandes.Add(new Demande
            {
                Id = 2,
                SemestreId = 2,
                EtudiantId = 2,
                Etudiant = new Etudiant
                {
                    Id = 2,
                    Nom = "Gagnon",
                    Prenom = "Marie",
                    DateNaissance = new DateTime(2002, 11, 2),
                    noEtudiant = "20230002",
                    noAdmission = "ADM002"
                },
                Semestre = new Semestre
                {
                    Id = 2,
                    NomSemestre = "ete-2025"
                },
                // Relation N-N : DemandeGenres
                DemandeGenres = new List<DemandeGenre>
                {
                    new DemandeGenre { DemandeId = 2, GenreId = 1 }
                },
                PrefDureeBail = 90,
                AccepteReglements = true,
                AccepteTraitementDonnees = true,
                ConfirmeSoumission = true,
                DateDemande = DateTime.Now.AddDays(-5),
                NomGarant = "Robert",
                PrenomGarant = "Paul",
                DateNaissanceGarant = new DateTime(1968, 8, 20),
                CourrielGarant = "paul.robert@email.com",
                TelephoneGarant = "8195556666",
                NomParent = "Julie Robert",
                CourrielParent = "julie.robert@email.com",
                NomUrgence = "Marc Robert",
                LienParenteUrgence = "Frère",
                TelephoneUrgence = "8197778888",
                Jumelages = new List<Jumelage>()
            });
        }

        public Demande? GetDemande(int id)
        {
            return _demandes.FirstOrDefault(x => x.Id == id);
        }

        public void Creer(Demande demande)
        {
            demande.Id = _demandes.Any() ? _demandes.Max(d => d.Id) + 1 : 1;

            // S'assurer que DemandeId est bien assigné dans chaque DemandeGenre
            foreach (var dg in demande.DemandeGenres)
                dg.DemandeId = demande.Id;

            _demandes.Add(demande);
        }

        public void Modifier(Demande demande)
        {
            Demande? demandeExistante = GetDemande(demande.Id);
            if (demandeExistante == null) return;

            demandeExistante.SemestreId = demande.SemestreId;
            demandeExistante.PrefDureeBail = demande.PrefDureeBail;
            demandeExistante.AccepteReglements = demande.AccepteReglements;
            demandeExistante.AccepteTraitementDonnees = demande.AccepteTraitementDonnees;
            demandeExistante.ConfirmeSoumission = demande.ConfirmeSoumission;
            demandeExistante.DateDemande = demande.DateDemande;
            demandeExistante.NomGarant = demande.NomGarant;
            demandeExistante.PrenomGarant = demande.PrenomGarant;
            demandeExistante.DateNaissanceGarant = demande.DateNaissanceGarant;
            demandeExistante.CourrielGarant = demande.CourrielGarant;
            demandeExistante.TelephoneGarant = demande.TelephoneGarant;
            demandeExistante.NomParent = demande.NomParent;
            demandeExistante.CourrielParent = demande.CourrielParent;
            demandeExistante.NomUrgence = demande.NomUrgence;
            demandeExistante.LienParenteUrgence = demande.LienParenteUrgence;
            demandeExistante.TelephoneUrgence = demande.TelephoneUrgence;
            demandeExistante.StatutDemande = demande.StatutDemande;
            demandeExistante.DateTraitement = demande.DateTraitement;
            demandeExistante.DateDebutBail = demande.DateDebutBail;
            demandeExistante.DateFinBail = demande.DateFinBail;
            demandeExistante.UniteId = demande.UniteId;
            demandeExistante.Jumelages = demande.Jumelages;

            // Remplacer les genres (relation N-N)
            demandeExistante.DemandeGenres.Clear();
            foreach (var dg in demande.DemandeGenres)
            {
                demandeExistante.DemandeGenres.Add(new DemandeGenre
                {
                    DemandeId = demandeExistante.Id,
                    GenreId = dg.GenreId
                });
            }
        }

        public void Supprimer(Demande demande)
        {
            _demandes.Remove(demande);
        }
    }
}