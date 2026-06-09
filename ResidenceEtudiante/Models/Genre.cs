using System.ComponentModel.DataAnnotations;



/*
 * @author John
 * 
 * Description: 
 * Modèle Genre 
 */
namespace ResidenceEtudiante.Models
{
    public class Genre
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Le Nom est obligatoire")]
        [StringLength(50, ErrorMessage = "Max 50 caractères")]
        public string Nom { get; set; }


        public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();

        //ajout de la realtion demande et genre plsuieurs à plusieurs
        public List<Demande> DemandeGenres { get; set; } = new List<Demande>();


    }

}
