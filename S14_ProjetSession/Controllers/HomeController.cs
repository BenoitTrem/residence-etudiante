using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Models;
using System.Diagnostics;

namespace S14_ProjetSession.Controllers
{
    /// <summary>
    /// Contrôleur principal pour les pages générales (Accueil, Confidentialité, Erreur).
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Affiche la page d'accueil.
        /// </summary>
        /// <returns>Vue Index</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Affiche la page de confidentialité.
        /// </summary>
        /// <returns>Vue Confidentialité</returns>
        /// <author>John Zuleta</author>
        public IActionResult Confidentialite()
        {
            return View();
        }

        /// <summary>
        /// Affiche une page d'erreur personnalisée.
        /// </summary>
        /// <param name="statusCode">Code HTTP de l'erreur (optionnel)</param>
        /// <returns>Vue Error avec modèle</returns>
        /// <author>John Zuleta , Par defaut </author>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Erreur(int? statusCode = null, string message = null)
        {
            var model = new ErreurViewModel
            {
                StatusCode = statusCode ?? 500,
                Message = message ?? statusCode switch
                {
                    404 => "Page introuvable",
                    403 => "Accès refusé",
                    500 => "Erreur interne du serveur",
                    _ => "Une erreur inattendue est survenue"
                }
            };
            return View(model);
        }
    }
}