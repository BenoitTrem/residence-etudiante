using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Unite
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le numéro est obligatoire.")]
        [Range(1, 9999, ErrorMessage = "Le numéro doit être entre 1 et 9999.")]
        [Display(Name = "Numéro de l'unité")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "La capacité est obligatoire.")]
        [Range(1, 10, ErrorMessage = "La capacité doit être entre 1 et 10.")]
        [Display(Name = "Capacité")]
        public int Capacite { get; set; }

        public int PlacesOccupees { get; set; }

        public int PlacesDisponibles => Capacite - PlacesOccupees;

        public int ResidenceId { get; set; }
        public Residence? Residence { get; set; }

        public List<Etudiant> Etudiants { get; set; } = new();
    }
}
