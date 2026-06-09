namespace ResidenceEtudiante.Models
{
    /// <summary>
    /// @author Felix
    /// Représente une période scolaire pour laquelle une demande de résidence peut être faite.
    /// </summary>
    public class Semestre
    {
        public int Id { get; set; }
        public string NomSemestre { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool InscriptionOuverte { get; set; }
    }
}
