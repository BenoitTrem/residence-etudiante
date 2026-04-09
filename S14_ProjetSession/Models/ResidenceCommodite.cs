using System.ComponentModel.DataAnnotations;

namespace S14_ProjetSession.Models
{
    public class ResidenceCommodite
    {
        public int ResidenceId { get; set; }
        public Residence Residence { get; set; }

        public int CommoditeId { get; set; }
        public Commodite Commodite { get; set; }

        [StringLength(250, ErrorMessage = "La description ne peut pas dépasser 250 caractères.")]
        public string? Description { get; set; }
    }
}
