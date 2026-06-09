using ResidenceEtudiante.Models;

/*
 * @author Benoit
 */
namespace ResidenceEtudiante.ViewModels
{
    /// <summary>
    /// ViewModel combiné pour une résidence avec ses commodités,
    /// permettant de gérer à la fois la checkbox et la description associée.
    /// </summary>
    public class ResidenceCommoditeViewModel
    {
        // ViewModel de la checkbox (cochée ou non)
        public CheckBoxViewModel CheckBoxViewModel { get; set; } = new CheckBoxViewModel();

        // ViewModel pour la description de la commodité
        public CommoditeDescriptionViewModel DescriptionViewModel { get; set; } = new CommoditeDescriptionViewModel();

        public ResidenceCommoditeViewModel() { }


        /// <summary>
        /// Initialise le ViewModel avec une commodité spécifique,
        /// son état de sélection et sa description si disponible.
        /// </summary>
        /// <param name="commodite">La commodité concernée</param>
        /// <param name="selectionne">Indique si la checkbox doit être cochée</param>
        /// <param name="description">Description personnalisée de la commodité</param>
        public ResidenceCommoditeViewModel(Commodite commodite, bool selectionne = false, string? description = null)
        {
            // Initialise la checkbox avec l'état sélectionné et le label
            CheckBoxViewModel = new CheckBoxViewModel
            {
                Value = commodite.Id.ToString(),
                Label = commodite.Nom ?? "",
                IsChecked = selectionne
            };

            // Initialise la description de la commodité
            DescriptionViewModel = new CommoditeDescriptionViewModel
            {
                Id = commodite.Id,
                Description = description
            };
        }
    }
}
