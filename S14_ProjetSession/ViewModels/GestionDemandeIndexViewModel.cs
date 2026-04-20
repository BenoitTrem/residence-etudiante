using S14_ProjetSession.Models;

namespace S14_ProjetSession.ViewModels
{
    public class GestionDemandeIndexViewModel
    {
        public List<Demande> Demandes { get; set; } = new();
        public List<Unite> Unites { get; set; } = new();
        public List<Semestre> Semestres { get; set; } = new();

        /// Semestre sélectionné pour le filtre (nullable = tous)
        public int? SemestreFiltre { get; set; }
    }
}
