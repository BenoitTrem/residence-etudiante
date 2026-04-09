using S14_ProjetSession.Models;
using System.ComponentModel.DataAnnotations;

/*
 * @author John Zuleta
 * Description: Tests unitaires pour le modèle Campus.
 *              Vérifie les règles de validation sans dépendance HTTP.
 */
namespace S14_ProjetSessionTests.Unit.CampusTests
{
    public class CampusUnitaireTests
    {
        /// <summary>
        /// Méthode utilitaire — valide un objet et retourne les erreurs.
        /// </summary>
        private static List<ValidationResult> Valider(object model)
        {
            var resultats = new List<ValidationResult>();
            Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                resultats,
                validateAllProperties: true);
            return resultats;
        }

        // Vérifie qu'un campus valide passe la validation
        [Fact(DisplayName = "Campus valide passe la validation")]
        public void Campus_Valide_AucuneErreur()
        {
            var campus = new Campus { Nom = "Campus A", Abreviation = "CA" };
            Assert.Empty(Valider(campus));
        }

        // Vérifie que le modèle Campus est invalide quand le nom dépasse 50 caractères
        [Fact(DisplayName = "Campus invalide si Nom dépasse 50 caractères")]
        public void Campus_NomTropLong_EstInvalide()
        {
            var campus = new Campus { Nom = new string('A', 51), Abreviation = "AB" };
            var erreurs = Valider(campus);
            Assert.Contains(erreurs, r => r.MemberNames.Contains("Nom"));
        }

        // Vérifie que le modèle Campus est invalide quand le nom est vide
        [Fact(DisplayName = "Campus invalide si Nom est vide")]
        public void Campus_NomVide_EstInvalide()
        {
            var campus = new Campus { Nom = "", Abreviation = "AB" };
            var erreurs = Valider(campus);
            Assert.Contains(erreurs, r => r.MemberNames.Contains("Nom"));
        }

        // Vérifie que l'abréviation invalide (trop longue) génère une erreur
        [Fact(DisplayName = "Campus invalide si Abreviation dépasse 10 caractères")]
        public void Campus_AbreivationTropLongue_EstInvalide()
        {
            var campus = new Campus { Nom = "Campus A", Abreviation = new string('X', 11) };
            var erreurs = Valider(campus);
            Assert.Contains(erreurs, r => r.MemberNames.Contains("Abreviation"));
        }
    }
}