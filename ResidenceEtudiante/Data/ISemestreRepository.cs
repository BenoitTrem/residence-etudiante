using ResidenceEtudiante.Models;


namespace ResidenceEtudiante.Data
{
    /// <author>Felix</author>
    public interface ISemestreRepository
    {

        public List<Semestre> Semestres { get; }


        public Semestre GetSemestreParId(int semestreId);




        public void SupprimerParID(int semestreID);




        public void Creer(Semestre semestre);


        public void Modifier(Semestre semestre);


        public void Supprimer(Semestre semestre);

    }
}
