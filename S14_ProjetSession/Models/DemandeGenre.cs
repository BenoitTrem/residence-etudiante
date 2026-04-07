namespace S14_ProjetSession.Models
{
    public class DemandeGenre
    {
        public int DemandeId { get; set; }
        public Demande Demande { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
