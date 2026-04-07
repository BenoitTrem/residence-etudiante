using Microsoft.AspNetCore.Authorization;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

namespace S14_ProjetSession.Authorization
{
    public class EtudiantHandler : AuthorizationHandler<EtudiantRequirement>
    {
        private readonly IEtudiantRepository _etudiantRepository;

        public EtudiantHandler(IEtudiantRepository etudiantRepository)
        {
            _etudiantRepository = etudiantRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EtudiantRequirement requirement)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return;
            }

            Etudiant? etudiant = await _etudiantRepository.GetByUserIdAsync(userId);

            if (etudiant != null)
            {
                context.Succeed(requirement);
            }
        }
    }
}