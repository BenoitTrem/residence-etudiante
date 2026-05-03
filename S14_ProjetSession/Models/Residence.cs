using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/*
 * @author Benoit
 * 
 * Description: 
 * Modèle résidence.
 * Contient les informations essentielles telles que le nom, l'adresse, le campus associé,
 * les unités de logement disponibles et les commodités offertes.
 * Fournit également le total d'unités, les unités disponibles et 
 * le nombre total de places disponibles.
 */
namespace S14_ProjetSession.Models
{
    public class Residence
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        [Display(Name = "Nom de la résidence")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        [StringLength(150, ErrorMessage = "L'adresse ne peut pas dépasser 150 caractères.")]
        [Display(Name = "Adresse")]
        public string AdresseLigne { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Ville")]
        public string Ville { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "La province doit contenir 2 lettres.")]
        [Display(Name = "Province")]
        public string Province { get; set; } = "QC";

        [Required(ErrorMessage = "Le code postal est obligatoire.")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Format invalide (ex: J8X 3X2)")]
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }

        public List<Unite> Unites { get; set; } = new();

        public List<ResidenceCommodite> ResidenceCommodites { get; set; } = new();

        // Retourne le nombre total d'unités associées à cette résidence.
        public int TotalUnites => Unites?.Count ?? 0;

        // Retourne l'adresse complète de la résidence sous forme de chaîne de caractères.
        public string AdresseComplete =>
           $"{AdresseLigne}, {Ville}, {Province} {CodePostal}";
    }
}
