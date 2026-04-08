/*
 * @author Benoit
 */
namespace S14_ProjetSession.ViewModels
{

    /// <summary>
    /// ViewModel pour gérer la description d'une commodité et l'état de la checkbox dans un formulaire.
    /// </summary>
    public class CommoditeDescriptionViewModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }   // Description personnalisée de la commodité
        public bool IsChecked { get; set; } // Indique si la checkbox associée doit rester cochée
    }
}
