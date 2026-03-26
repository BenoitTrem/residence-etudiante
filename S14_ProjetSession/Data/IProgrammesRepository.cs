using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Data
{
    public interface IProgrammesRepository
    {


        public List<Programme> Programmes { get; }

        public Programme? GetProgramme(int id);

    }
}
