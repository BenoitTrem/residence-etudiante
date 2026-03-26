using Microsoft.AspNetCore.Identity;

namespace S14_ProjetSession.Areas.Identity.Data
{
    public class ApplicationUser: IdentityUser
    {
        [PersonalData]
        public DateOnly? DateNaissance {  get; set; }

        public string? Autre {  get; set; }
    }
}
