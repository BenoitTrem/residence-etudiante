using S14_ProjetSession.Models;

namespace S14_ProjetSession.ViewModels
{
    /// <author>Felix</author>
    public class GestionDemandeIndexViewModel
    {
        public List<Demande> Demandes { get; set; } = new();
        public List<Unite> Unites { get; set; } = new();
        public List<Semestre> Semestres { get; set; } = new();

        /// Semestre sélectionné pour le filtre (nullable = tous)
        public int? SemestreFiltre { get; set; }


        // pour la Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
