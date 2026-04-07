using System.ComponentModel.DataAnnotations;

/*
 * @author Benoit
 * 
 * Description: 
 * Modèle unité.
 * Contient les informations sur le numéro, la capacité, les places occupées,
 * l'accessibilité pour mobilité réduite et la liste des étudiants assignés.
 */
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

        /// Calcul du nombre de places disponibles dans l'unité
        public int PlacesDisponibles => Capacite - PlacesOccupees;

        [Required(ErrorMessage = "Veuillez indiquer si l'unité est adaptée.")]
        [Display(Name = "Adaptée pour mobilité réduite")]
        public bool? AdapteePourMobiliteReduite { get; set; }

        public int ResidenceId { get; set; }
        public Residence? Residence { get; set; }

        public List<Etudiant> Etudiants { get; set; } = new();
    }
}
