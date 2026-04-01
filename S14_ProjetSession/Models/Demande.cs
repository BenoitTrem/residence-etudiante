using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Demande
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le semestre est obligatoire.")]
        public int SemestreId { get; set; }

        public Semestre? Semestre { get; set; }

        [Required(ErrorMessage = "L'étudiant est obligatoire.")]
        public int EtudiantId { get; set; }

        public Etudiant? Etudiant { get; set; }

        public Genre? PreferencesGenre { get; set; }

        public int? PreferencesGenreId { get; set; }

        [Required(ErrorMessage = "La durée du bail est obligatoire.")]
        [Range(1, 3650, ErrorMessage = "La durée du bail doit être supérieure à 0.")]
        public int PrefDureeBail { get; set; }

        public bool AccepteReglements { get; set; }

        public bool AccepteTraitementDonnees { get; set; }

        public bool ConfirmeSoumission { get; set; }

        [Required(ErrorMessage = "La date de la demande est obligatoire.")]
        public DateTime DateDemande { get; set; }

        public List<Jumelage> Jumelages { get; set; } = new List<Jumelage>();

        [Required(ErrorMessage = "Le nom du garant est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom du garant doit contenir entre 2 et 100 caractères.")]
        public string NomGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom du garant est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le prénom du garant doit contenir entre 2 et 100 caractères.")]
        public string PrenomGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance du garant est obligatoire.")]
        public DateTime? DateNaissanceGarant { get; set; }

        [Required(ErrorMessage = "Le courriel du garant est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le courriel du garant doit être valide.")]
        [StringLength(255, ErrorMessage = "Le courriel du garant ne peut pas dépasser 255 caractères.")]
        public string CourrielGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone du garant est obligatoire.")]
        [Phone(ErrorMessage = "Le numéro de téléphone du garant doit être valide.")]
        [StringLength(20, ErrorMessage = "Le téléphone du garant ne peut pas dépasser 20 caractères.")]
        public string TelephoneGarant { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom du parent doit contenir entre 2 et 100 caractères.")]
        public string? NomParent { get; set; }

        [EmailAddress(ErrorMessage = "Le courriel du parent doit être valide.")]
        [StringLength(255, ErrorMessage = "Le courriel du parent ne peut pas dépasser 255 caractères.")]
        public string? CourrielParent { get; set; }

        [Required(ErrorMessage = "Le nom du contact d'urgence est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom du contact d'urgence doit contenir entre 2 et 100 caractères.")]
        public string NomUrgence { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le lien de parenté est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le lien de parenté doit contenir entre 2 et 100 caractères.")]
        public string LienParenteUrgence { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone du contact d'urgence est obligatoire.")]
        [Phone(ErrorMessage = "Le numéro de téléphone du contact d'urgence doit être valide.")]
        [StringLength(20, ErrorMessage = "Le téléphone du contact d'urgence ne peut pas dépasser 20 caractères.")]
        public string TelephoneUrgence { get; set; } = string.Empty;
    }
}