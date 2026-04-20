namespace S14_ProjetSession.Models
{
    /// <summary>
    /// Modèle utilisé pour afficher les erreurs avec code et message personnalisé.
    /// </summary>
    /// <author>John Zuleta</author>
    public class ErreurViewModel
    {


        /// <summary>
        /// Code de l'erreur HTTP 
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Message personnalisé à afficher à l'utilisateur
        /// </summary>
        public string? Message { get; set; }
    }
}