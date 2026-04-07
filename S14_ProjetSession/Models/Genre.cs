using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Genre
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Le Nom est obligatoire")]
        [StringLength(50, ErrorMessage = "Max 50 caractères")]
        public string Nom { get; set; }


        public List<Etudiant> Etudiants { get; set; } = new List<Etudiant>();

        public List<DemandeGenre> DemandeGenres { get; set; } = new();


    }

}
