using Microsoft.AspNetCore.Authorization;

/*
 * @author Benoit 
 */
namespace S14_ProjetSession.Authorization
{
    /// <summary>
    /// Représente une exigence d'autorisation utilisée dans la policy "EstEtudiant".
    /// </summary>
    public class EtudiantRequirement : IAuthorizationRequirement
    {

    }
}
