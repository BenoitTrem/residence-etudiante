using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
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
 