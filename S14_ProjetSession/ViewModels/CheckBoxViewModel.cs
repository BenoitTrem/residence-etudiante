/*
 * @author Benoit
 */
namespace S14_ProjetSession.ViewModels
{
    /// <summary>
    /// ViewModel représentant une checkbox dans un formulaire.
    /// Permet de conserver l'état coché/décoché et le label associé.
    /// </summary>
    public class CheckBoxViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
    }
}
