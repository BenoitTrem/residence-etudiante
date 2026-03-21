using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class Demande
    {
        public int Id { get; set; }


        public int SemestreId { get; set; }
        public Semestre  Semestre { get; set; }
       
        public int EtudiantId { get; set; }
        public Etudiant Etudiant { get; set; }
        
        
        public String PreferencesGenre  { get; set; }
        
        public bool JumelageVolontaire { get; set; }
        
        // simplement le nombre de jours pour le moment
        public int PrefDureeBail { get; set; }

        public bool AccepteReglements { get; set; }

        public bool AccepteTraitementDonnees { get; set; }

        public bool ConfirmeSoumission {  get; set; }

        public DateTime DateDemande { get; set; }

        // jumelage n'est pas forcément relié A un étudiant mais
        // y'a c'est information
        public List<Jumelage> Jumelages { get; set; }

    }
}
