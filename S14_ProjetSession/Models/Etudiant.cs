using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace S14_ProjetSession.Models
{
    public class Etudiant
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, ErrorMessage = "Max 50 caractères")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, ErrorMessage = "Max 50 caractères")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime DateNaissance { get; set; }

        [Required(ErrorMessage = "Le genre est obligatoire")]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        [Required(ErrorMessage = "Le programme est obligatoire")]
        [Display(Name = "Programme")]
        public int ProgrammeId { get; set; }

        public Programme? Programme { get; set; }

        [Required(ErrorMessage = "Le numéro étudiant est obligatoire")]
        [Display(Name = "No étudiant")]
        public string noEtudiant { get; set; }

        [Required(ErrorMessage = "Le numéro d'admission est obligatoire")]
        [Display(Name = "No admission")]
        public string noAdmission { get; set; }

        [Display(Name = "Mobilité réduite")]
        public bool MobiliteReduite { get; set; }

        [Required(ErrorMessage = "L'adresse est obligatoire")]
        [Display(Name = "Adresse permanente")]
        public string AdressePermanente { get; set; }

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Le courriel institutionnel est obligatoire")]
        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [Display(Name = "Courriel institutionnel")]
        public string CourrielInstitutionnel { get; set; }

        [Required(ErrorMessage = "Le courriel personnel est obligatoire")]
        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        [Display(Name = "Courriel personnel")]
        public string CourrielPersonnel { get; set; }

        public int? UniteId { get; set; }
        public Unite Unite { get; set; }

        public List<Demande> Demandes { get; set; } = new();
    }
}