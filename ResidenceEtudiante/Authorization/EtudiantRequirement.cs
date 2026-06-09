using Microsoft.AspNetCore.Authorization;

/*
 * @author Benoit 
 */
namespace ResidenceEtudiante.Authorization
{
    /// <summary>
    /// Représente une exigence d'autorisation utilisée dans la policy "EstEtudiant".
    /// </summary>
    public class EtudiantRequirement : IAuthorizationRequirement
    {

    }
}
