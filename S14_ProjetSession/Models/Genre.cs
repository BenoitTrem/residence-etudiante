namespace S14_ProjetSession.Models
{
    public class Genre
    {

        public int Id { get; set; }

        public string Nom { get; set; }


        public List<Etudiant> etudiants { get; set; } = new List<Etudiant>();


    }

}
