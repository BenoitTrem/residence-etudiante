namespace S14_ProjetSession.Models
{
    public class Campus
    {

        public int Id { get; set; }

        public string Nom { get; set; }


        public string Abreviation { get; set; }


        public List<Programme> programmes { get; set; } = new List<Programme>();
    }
}
