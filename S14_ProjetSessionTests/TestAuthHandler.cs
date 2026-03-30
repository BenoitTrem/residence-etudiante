using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace S14_ProjetSessionTests
{
    /// <summary>
    /// Gestion de l'authentification personnalisée pour les tests d'intégration.
    /// Permet de simuler l'authentification d'utilisateurs avec des ClaimsPrincipal configurables dans les tests.
    /// Aide de Claude Sonnet 4.5 pour l'implémentation et la documentation
    /// </summary>
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Func<ClaimsPrincipal?> _userProvider;

        /// <summary>
        /// Initialise une nouvelle instance du handler d'authentification de test.
        /// </summary>
        /// <param name="options">Options de configuration du schéma d'authentification (requis par la classe de base).</param>
        /// <param name="logger">Factory de création de loggers pour le diagnostic (requis par la classe de base).</param>
        /// <param name="encoder">Encodeur d'URL utilisé pour l'encodage des redirections (requis par la classe de base).</param>
        /// <param name="userProvider">Fonction qui retourne l'utilisateur courant; permet de choisir un utilisateur au moment du test.</param>
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, Func<ClaimsPrincipal?> userProvider) : base(options, logger, encoder)
        {
            _userProvider = userProvider;
        }

        /// <summary>
        /// Gère l'authentification pour chaque requête HTTP.
        /// Récupère l'utilisateur via la fonction _userProvider et crée un ticket d'authentification si l'utilisateur existe.
        /// Cette méthode est appelée automatiquement par ASP.NET Core lors du traitement de chaque requête.
        /// </summary>
        /// <returns>
        /// AuthenticateResult.NoResult() si aucun utilisateur n'est défini (non authentifié).
        /// AuthenticateResult.Success(ticket) si un utilisateur est présent (authentifié avec succès).
        /// </returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            ClaimsPrincipal? user = _userProvider();
            // Si aucun utilisateur connecté
            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            AuthenticationTicket ticket = new AuthenticationTicket(user, "TestAuth");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

}
