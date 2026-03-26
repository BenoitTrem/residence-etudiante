using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Demande
    {
        public int Id { get; set; }


        public int Session {  get; set; }
        // devrait etre Genre
        
        public String PreferencesGenre  { get; set; }
        
        public bool JumelageVolontaire { get; set; }
        
        public String NomJumelage {  get; set; }

        [Required(ErrorMessage = "adresse email est un champ obligatoire")]
        [EmailAddress(ErrorMessage = "adresse email invalide.")]
        [Display(Name = "adresse email")]
        public String CourrielJumelage { get; set; }

        public int PrefDureeBail { get; set; }

        public bool AccepteReglements { get; set; }

        public bool AccepteTraitementDonnees { get; set; }

        public bool ConfirmeSoumission {  get; set; }

        public DateTime DateDemande { get; set; }
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }

    }
}
