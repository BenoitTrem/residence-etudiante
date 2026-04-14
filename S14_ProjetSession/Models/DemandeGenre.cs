namespace S14_ProjetSession.Models
{

    /*
     * @author John
     * 
     * Description: 
     * Modèle DemandeGenre qui est la classe pivot de la realtion genre et demande plusieurs à plusieurs 
     */
    public class DemandeGenre
    {
        public int DemandeId { get; set; }
        public Demande? Demande { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
