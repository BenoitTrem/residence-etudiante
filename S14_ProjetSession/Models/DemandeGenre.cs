namespace S14_ProjetSession.Models
{
    /// <author>John Zuleta</author>
    public class DemandeGenre
    {
        public int DemandeId { get; set; }
        public Demande? Demande { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
