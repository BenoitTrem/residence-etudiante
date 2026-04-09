using System.ComponentModel.DataAnnotations;

/*
 * @author Benoit
 * 
 * Description: 
 * Modèle adresse associée à une résidence.
 */
namespace S14_ProjetSession.Models
{
    public class Adresse
    {
        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        [StringLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        [Display(Name = "Adresse")]
        public string AdresseString { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(50)]
        [Display(Name = "Ville")]
        public string Ville { get; set; } = "Gatineau";

        [Required(ErrorMessage = "Required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "La province doit contenir 2 lettres.")]
        [Display(Name = "Province")]
        public string Province { get; set; } = "QC";

        [Required(ErrorMessage = "Le code postal est obligatoire.")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$",ErrorMessage = "Format invalide (ex: J8X 3X2)")]
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }
    }
}