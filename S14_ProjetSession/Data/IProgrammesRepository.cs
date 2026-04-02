using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IProgrammesRepository
    {


        public IEnumerable<Programme> Programmes { get; }

        public Programme? GetProgramme(int id);


        public void SupprimerParID(int id);


        public void Creer(Programme programme);


        public void Modifier(Programme programme);


        public void Supprimer(Programme programme);

    }
}
