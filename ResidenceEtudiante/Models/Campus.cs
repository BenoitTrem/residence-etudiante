using System.ComponentModel.DataAnnotations;



/*
 * @author John
 * 
 * Description: 
 * Modèle Campus
 */
namespace ResidenceEtudiante.Models
{
    public class Campus
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Le Nom est obligatoire")]
        [StringLength(50, ErrorMessage = "Max 50 caractères")]
        public string Nom { get; set; }


        [Required(ErrorMessage = "L'abreviation est obligatoire")]
        [StringLength(10, ErrorMessage = "Max 5 caractères")]
        public string Abreviation { get; set; }



        public int Priorite { get; set; } = 0;




        public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();
        public List<Residence> Residences { get; set; } = new List<Residence>();
        public List<Programme> programmes { get; set; } = new List<Programme>();
    }
}
