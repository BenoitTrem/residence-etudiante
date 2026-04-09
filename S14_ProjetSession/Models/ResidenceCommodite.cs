using System.ComponentModel.DataAnnotations;

/*
 * @author Benoit
 * 
 * Description: 
 * Modèle représentant l'association entre une résidence et une commodité.
 * Permet de stocker une description spécifique de la commodité pour chaque résidence.
 */
namespace S14_ProjetSession.Models
{
    public class ResidenceCommodite
    {
        public int ResidenceId { get; set; }
        public Residence Residence { get; set; }

        public int CommoditeId { get; set; }
        public Commodite Commodite { get; set; }

        /// Description optionnelle spécifique à la résidence pour cette commodité
        [StringLength(250, ErrorMessage = "La description ne peut pas dépasser 250 caractères.")]
        public string? Description { get; set; }
    }
}
