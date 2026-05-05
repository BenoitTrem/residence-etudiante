// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using S14_ProjetSession.Areas.Identity.Data;

namespace S14_ProjetSession.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le courriel est requis.")]
            [EmailAddress(ErrorMessage = "Format de courriel invalide.")]
            [Display(Name = "Courriel")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [StringLength(100, ErrorMessage = "Le {0} doit contenir entre {2} et {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le mot de passe")]
            [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // cree un utilisateur
                    await _userManager.AddToRoleAsync(user, "Utilisateur");
                    _logger.LogInformation("Un utilisateur a créé un nouveau compte avec mot de passe.");

                    // par defaut etait vrai mais devrais etre false
                    user.EmailConfirmed = false;
                    await _userManager.UpdateAsync(user);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    try
                    {
                        await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Confirmez votre courriel – Résidences Étudiantes",
                        $@"
                        <!DOCTYPE html>
                        <html lang='fr'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        </head>
                        <body style='margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;'>

                            <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f4f4; padding: 40px 0;'>
                                <tr>
                                    <td align='center'>
                                        <table width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                        
                                            <tr>
                                                <td align='center' style='background-color:#0d6efd; padding: 32px 40px;'>
                                                    <h1 style='color:#ffffff; margin:0; font-size:24px;'>Résidences Étudiantes</h1>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style='padding: 40px;'>
                                                    <h2 style='color:#212529; margin-top:0;'>Confirmation de votre courriel</h2>
                                                    <p style='color:#495057; line-height:1.6;'>Bonjour,</p>
                                                    <p style='color:#495057; line-height:1.6;'>
                                                        Merci de vous être inscrit sur la plateforme de <strong>Résidences Étudiantes</strong>. 
                                                        Pour activer votre compte, veuillez confirmer votre adresse courriel en cliquant sur le bouton ci-dessous.
                                                    </p>

                                                    <table cellpadding='0' cellspacing='0' style='margin: 32px auto;'>
                                                        <tr>
                                                            <td align='center' style='background-color:#0d6efd; border-radius:6px;'>
                                                                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'
                                                                   style='display:inline-block; padding: 14px 32px; color:#ffffff; text-decoration:none; font-size:16px; font-weight:bold;'>
                                                                     Confirmer mon courriel
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <p style='color:#6c757d; font-size:13px; line-height:1.6;'>
                                                        Si vous n'avez pas créé de compte, vous pouvez ignorer ce message. 
                                                        Ce lien expirera sous peu.
                                                    </p>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td align='center' style='background-color:#f8f9fa; padding: 24px 40px; border-top: 1px solid #dee2e6;'>
                                                    <p style='color:#6c757d; font-size:13px; margin:0;'>
                                                        © {DateTime.Now.Year} Résidences Étudiantes — Tous droits réservés
                                                    </p>
                                                </td>
                                            </tr>

                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>
                        "
                    );
                    }
                    catch (EmailException ex)
                    {
                        return Redirect($"/Home/Erreur?statusCode=500&message={Uri.EscapeDataString(ex.Message)}");
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Impossible de créer une instance de '{nameof(ApplicationUser)}'. " +
                    $"Assurez-vous que '{nameof(ApplicationUser)}' n'est pas une classe abstraite et possède un constructeur sans paramètre, ou remplacez la page d'inscription.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("L'interface par défaut nécessite un magasin d'utilisateurs avec support du courriel.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
