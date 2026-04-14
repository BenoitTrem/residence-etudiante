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
