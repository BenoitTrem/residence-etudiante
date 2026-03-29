using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

namespace S14_ProjetSession.Authorization
{
    public class EtudiantHandler : AuthorizationHandler<EtudiantRequirement>
    {
        private readonly ResidencesDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EtudiantHandler(ResidencesDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EtudiantRequirement requirement)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(context.User);

            if (user == null)
                return;

            Etudiant? etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

            if (etudiant != null)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "NotEtudiant"));
            }
        }
    }
}