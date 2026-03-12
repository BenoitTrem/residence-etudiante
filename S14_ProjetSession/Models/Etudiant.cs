using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace S14_ProjetSession.Models
{
    public class Etudiant
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        public string Prenom {  get; set; }

        public DateTime DateNaissance { get; set; }

        public int Genreid { get; set; }

        public Genre Genre { get; set; }


        public int ProgrammeId { get; set; }

        public Programme Programme { get; set; }

        public string noEtudiant { get; set; }

        public string noAdmission { get; set; }


        public bool MobiliteReduite { get; set; }

        public string AdressePermanente { get; set; }

        public string Telephone { get; set; }

        public string CourrielInstitutionnel { get; set; }

        public string CourrielPersonnel { get; set; }










    }
}
