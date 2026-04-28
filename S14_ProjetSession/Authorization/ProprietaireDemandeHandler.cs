using Microsoft.AspNetCore.Authorization;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

namespace S14_ProjetSession.Authorization
{
    public class ProprietaireDemandeHandler : AuthorizationHandler<EstProprietaireDemandeRequirement, Demande>
    {
        private readonly IEtudiantRepository _etudiantRepository;
        

        public ProprietaireDemandeHandler(IEtudiantRepository repository)
        {
            _etudiantRepository = repository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EstProprietaireDemandeRequirement requirement, Demande resource)
        {
            
            if (context.User == null) 
            {
                return;
            }

            // si le role est admin ou gestionnaire oui mais si etudiant seulemenet etudiant
            

            if (context.User.IsInRole("Admin") || context.User.IsInRole("Gestionnaire")) 
            {
                context.Succeed(requirement);
                return;
            }

            // get le UserId
            
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null) 
            {
                return;
            }

            Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(userId);
            
            if (etudiant == null) 
            {
                return;
            }

            if (resource.EtudiantId == etudiant.Id)
            {
                context.Succeed(requirement);
            }

        }
    }
}
