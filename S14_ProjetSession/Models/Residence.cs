using System.ComponentModel;

namespace S14_ProjetSession.Models
{
    public class Residence
    {
        public int Id { get; set; }

        public string Nom { get; set; }

        public string Adresse { get; set; }

        public List<Unite> Unites { get; set; } = new();
    }
}
