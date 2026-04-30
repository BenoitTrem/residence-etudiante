namespace S14_ProjetSession.Models
{
    public class Semestre
    {
        public int Id { get; set; }
        public string NomSemestre { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public DateTime DebutInscriptionDisponible { get; set; }
        public DateTime FinInscriptionDisponible { get; set; }

        public bool InscriptionOuverte => DebutInscriptionDisponible.Date <= DateTime.Today && FinInscriptionDisponible.Date >= DateTime.Today;


    }
}
