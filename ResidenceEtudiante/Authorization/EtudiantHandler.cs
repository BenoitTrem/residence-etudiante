using Microsoft.AspNetCore.Authorization;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Models;
using System.Security.Claims;

/*
 * @author Benoit 
 */
namespace ResidenceEtudiante.Authorization
{
    public class EtudiantHandler : AuthorizationHandler<EtudiantRequirement>
    {
        private readonly IEtudiantRepository _etudiantRepository;

        public EtudiantHandler(IEtudiantRepository etudiantRepository)
        {
            _etudiantRepository = etudiantRepository;
        }

        /// <summary>
        /// Méthode appelée pour vérifier si l'utilisateur satisfait l'exigence EtudiantRequirement.
        /// </summary>
        /// <param name="context">Contexte d'autorisation contenant l'utilisateur</param>
        /// <param name="requirement">Exigence à valider</param>
        /// <returns>Succeed ou non</returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EtudiantRequirement requirement)
        {
            // Récupère le ID de l'utilisateur connecté (AspNetUser) à partir des claims
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vérifie si l'utilisateur est authentifié
            if (userId == null)
            {
                return;
            }

            // Recherche un étudiant correspondant à cet utilisateur dans la base de données
            Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(userId);

            // Si un étudiant est trouvé, l'exigence est satisfaite
            if (etudiant != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}