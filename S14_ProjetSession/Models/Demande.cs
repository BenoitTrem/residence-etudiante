using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Demande
    {
        public int Id { get; set; }


        public int session {  get; set; }
        // devrait etre Genre
        []
        public String preferencesGenre  { get; set; }
        public bool JumelageVolontaire { get; set; }

        public String nomJumelage {  get; set; }

        // faire verif de email avec regex


        [Required(ErrorMessage = "adresse email est un champ obligatoire")]
        [EmailAddress(ErrorMessage = "adresse email invalide.")]
        [Display(Name = "adresse email")]
        public String courrielJumelage { get; set; }

        public int prefDureeBail { get; set; }

        public bool accepteReglements { get; set; }

        public bool accepteTraitementDonnees { get; set; }

        public bool confirmeSoumission {  get; set; }

        public DateTime DateDemande { get; set; }
    }
}
