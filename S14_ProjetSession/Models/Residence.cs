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

        [Required(ErrorMessage = "L'adresse est requise.")]
        public Adresse Adresse { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un campus.")]
        public int? CampusId { get; set; }
        public Campus? Campus { get; set; }

        public List<Unite> Unites { get; set; } = new();

        public List<ResidenceCommodite> ResidenceCommodites { get; set; } = new();

        /// Retourne le nombre total d'unités associées à cette résidence.
        public int TotalUnites => Unites?.Count ?? 0;

        /// Retourne le nombre d'unités qui ont au moins une place disponible.
        public int UnitesDisponibles => Unites?.Count(u => u.PlacesDisponibles > 0) ?? 0;

        /// Retourne le nombre total de places disponibles dans toutes les unités.
        public int TotalPlacesDisponibles => Unites?.Sum(u => u.PlacesDisponibles) ?? 0;

        /// Retourne l'adresse complète de la résidence sous forme de chaîne de caractères.
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
