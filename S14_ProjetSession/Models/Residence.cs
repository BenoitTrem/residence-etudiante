using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Residence
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        [Display(Name = "Nom de la résidence")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'adresse est requise.")]
        public Adresse Adresse { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un campus.")]
        public int? CampusId { get; set; }
        public Campus? Campus { get; set; }

        public List<Unite> Unites { get; set; } = new();

        public List<ResidenceCommodite> ResidenceCommodites { get; set; } = new();

        public string AdresseString
        {
            get
            {
                if (Adresse == null) return "";
                return $"{Adresse.AdresseString}, {Adresse.Ville}, {Adresse.Province} {Adresse.CodePostal}";
            }
        }
    }
}
