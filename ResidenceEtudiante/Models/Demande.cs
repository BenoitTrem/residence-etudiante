using System.ComponentModel.DataAnnotations;

namespace ResidenceEtudiante.Models
{
    /// <summary>
    /// <author>Felix</author>
    /// Représente une demande de résidence soumise par un étudiant.
    /// </summary>
    public class Demande
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le semestre est obligatoire.")]
        public int SemestreId { get; set; }

        public Semestre? Semestre { get; set; }


        public int? EtudiantId { get; set; }

        public Etudiant? Etudiant { get; set; }

        //@author John
        // Relation directe Genre <-> Demande plusieurs à plusieurs (sans table de jointure explicite)
        public List<Genre> DemandeGenres { get; set; } = new List<Genre>();

        // simplement le nombre de jours pour le moment
        [Required(ErrorMessage = "La durée du bail est obligatoire.")]
        [Range(1, 365, ErrorMessage = "La durée du bail doit être entre 1 et 365 jours.")]
        public int PrefDureeBail { get; set; }

        public bool AccepteReglements { get; set; }

        public bool AccepteTraitementDonnees { get; set; }

        public bool ConfirmeSoumission { get; set; }

        [DatePasDansLeFutur]
        [DataType(DataType.Date)]
        public DateTime DateDemande { get; set; } = DateTime.Now;

        public List<Jumelage> Jumelages { get; set; } = new List<Jumelage>();

        [Required(ErrorMessage = "Le nom du garant est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom du garant doit contenir entre 2 et 100 caractères.")]
        public string NomGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom du garant est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le prénom du garant doit contenir entre 2 et 100 caractères.")]
        public string PrenomGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance du garant est obligatoire.")]
        [DatePasDansLeFutur]
        [AgeMinimum(18, ErrorMessage = "Le garant doit être majeur (18 ans et plus).")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateNaissanceGarant { get; set; }

        [Required(ErrorMessage = "Le courriel du garant est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le courriel du garant doit être valide.")]
        [StringLength(255, ErrorMessage = "Le courriel du garant ne peut pas dépasser 255 caractères.")]
        public string CourrielGarant { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone du garant est obligatoire.")]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le format du téléphone doit être 000-000-0000")]
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
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Le format du téléphone doit être 000-000-0000")]
        [StringLength(20, ErrorMessage = "Le téléphone du contact d'urgence ne peut pas dépasser 20 caractères.")]
        public string TelephoneUrgence { get; set; } = string.Empty;

        // L'administrateur gère ces attributs.
        public DateTime? DateDebutBail { get; set; }

        public DateTime? DateFinBail { get; set; }

        public StatutDemande StatutDemande { get; set; } = StatutDemande.EnAttente;

        public DateTime? DateTraitement { get; set; }

        // Attribuer une unité à la demande.
        public int? UniteId { get; set; }
        public Unite? Unite { get; set; }
    }
}