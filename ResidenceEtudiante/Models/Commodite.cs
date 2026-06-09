using System.ComponentModel.DataAnnotations;

/*
 * @author Benoit
 * 
 * Description: 
 * Modèle représentant une commodité pouvant être associée à une résidence.
 * Chaque commodité possède un nom et peut être liée à plusieurs résidences.
 */
namespace ResidenceEtudiante.Models
{
    public class Commodite
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la commodité est requis.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; }

        public List<ResidenceCommodite> ResidenceCommodites { get; set; } = new();
    }
}
 