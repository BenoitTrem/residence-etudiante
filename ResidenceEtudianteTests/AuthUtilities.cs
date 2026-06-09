using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ResidenceEtudianteTests
{
    public static class AuthUtilities
    {
        public static ClaimsPrincipal CreerEtudiant()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "468a4852-42ee-4f45-9be5-41422b589904"),
                new Claim(ClaimTypes.Name, "amin admin"),
                new Claim(ClaimTypes.Email, "amin.admin@exemple.com"),
                new Claim(ClaimTypes.Role, "Utilisateur")
            }, "TestAuth"));
        }

        public static ClaimsPrincipal CreerGestionnaire()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "gestionnaire-id"),
                new Claim(ClaimTypes.Name, "Gestionnaire User"),
                new Claim(ClaimTypes.Email, "gestionnaire@test.com"),
                new Claim(ClaimTypes.Role, "Gestionnaire")
            }, "TestAuth"));
        }

        public static ClaimsPrincipal CreerAdmin()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-id"),
                new Claim(ClaimTypes.Name, "Admin User"),
                new Claim(ClaimTypes.Email, "admin@test.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuth"));
        }

        public static ClaimsPrincipal CreerUtilisateur()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id"),
                new Claim(ClaimTypes.Name, "User"),
                new Claim(ClaimTypes.Email, "user@test.com"),
                new Claim(ClaimTypes.Role, "Utilisateur")
            }, "TestAuth"));
        }

        public static ClaimsPrincipal CreerAnonyme()
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}