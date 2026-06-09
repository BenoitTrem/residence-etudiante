using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;



/// <summary>
/// Interface responsable de la gestion des programmes
/// </summary>
/// <author>John Zuleta</author>
namespace ResidenceEtudiante.Data
{
    public interface IProgrammesRepository
    {


        public IEnumerable<Programme> Programmes { get; }

        public Programme? GetProgramme(int? id);


        public void SupprimerParID(int id);


        public void Creer(Programme programme);


        public void Modifier(Programme programme);


        public void Supprimer(Programme programme);

    }
}
