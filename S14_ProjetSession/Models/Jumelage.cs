using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Jumelage
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nom { get; set; }
        // faire verif de Courriel
        [Required(ErrorMessage = "Le courriel est obligatoire")]
        [EmailAddress(ErrorMessage = "Le courriel doit être valide")]
        [StringLength(255, ErrorMessage = "Maximum 255 caractères")]
        public string Courriel { get; set; }

       
        public Jumelage() { }
    }
}
