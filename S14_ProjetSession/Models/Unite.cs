namespace S14_ProjetSession.Models
{
    public class Unite
    {
        public int Id { get; set; }

        public int Capacite { get; set; }

        public int Numero { get; set; }

        public int ResidenceId { get; set; }
        public Residence Residence { get; set; }

        public List<Etudiant> Etudiants { get; set; } = new();
    }
}
