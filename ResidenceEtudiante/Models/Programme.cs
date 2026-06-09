using System.ComponentModel.DataAnnotations;

namespace ResidenceEtudiante.Models
{

    /// <summary>
    /// Représente un programme académique.
    /// </summary>
    /// <author>John Zuleta</author>
    public class Programme
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Le Nom est obligatoire")]
        [StringLength(150, ErrorMessage = "Max 50 caractères")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le Code est obligatoire")]
        [StringLength(10, ErrorMessage = "Max 10 caractères")]
        public string Code { get; set; }


        [Display(Name = "Campus")]
        public int? CampusId { get; set; }

        public Campus? Campus { get; set; }


        public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();
    }
}
