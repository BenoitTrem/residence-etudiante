using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante.Models;



/// <summary>
/// Interface responsable de la gestion des genres
/// </summary>
/// <author>John Zuleta</author>
namespace ResidenceEtudiante.Data
{
    public interface IGenresRepository
    {

  
        public IEnumerable<Genre> Genres { get; }

        public Genre? GetGenre(int? id);



        public void SupprimerParID(int id);


        public void Creer(Genre genre);


        public void Modifier(Genre genre);


        public void Supprimer(Genre genre);



    }
}
