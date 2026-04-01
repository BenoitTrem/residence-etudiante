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
                PreferencesGenreId = 1,

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
                PreferencesGenreId = 2,

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
            return Demandes.FirstOrDefault(x => x.Id == id);
        }

        public void Creer(Demande demande)
        {
            _demandes.Add(demande);
        }

        public void Modifier(Demande demande)
        {
            var existante = _demandes.FirstOrDefault(x => x.Id == demande.Id);

            if (existante != null)
            {
                _demandes.Remove(existante);
                _demandes.Add(demande);
            }
        }

        public void Supprimer(Demande demande)
        {
            _demandes.Remove(demande);
        }
    }
}