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

        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères.")]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        public List<Unite> Unites { get; set; } = new();
    }
}
