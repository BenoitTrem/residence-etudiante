using S14_ProjetSession.Models;


namespace S14_ProjetSession.Data
{
    public interface ISemestreRepository
    {

        public List<Semestre> Semestre { get; }


        public List<Etudiant> GetSemestreParId(int semestreId);




        public void SupprimerParID(int semestreID);




        public void Creer(Semestre semestre);


        public void Modifier(Semestre semestre);


        public void Supprimer(Semestre semestre);

    }
}
