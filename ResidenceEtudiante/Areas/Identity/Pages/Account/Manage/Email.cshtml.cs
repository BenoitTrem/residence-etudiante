// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ResidenceEtudiante.Areas.Identity.Data;

namespace ResidenceEtudiante.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Le courriel est requis.")]
            [EmailAddress(ErrorMessage = "Veuillez entrer une adresse courriel valide.")]
            [Display(Name = "Nouveau courriel")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec l'ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec l'ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                try
                {
                    await _emailSender.SendEmailAsync(
                        Input.NewEmail,
                        "Confirmez votre nouvelle adresse courriel – Résidences Étudiantes",
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
                                                    <h2 style='color:#212529; margin-top:0;'>Confirmation de votre nouvelle adresse courriel</h2>
                                                    <p style='color:#495057; line-height:1.6;'>Bonjour,</p>
                                                    <p style='color:#495057; line-height:1.6;'>
                                                        Vous avez demandé à modifier l'adresse courriel associée à votre compte
                                                        <strong>Résidences Étudiantes</strong>.
                                                        Veuillez confirmer votre nouvelle adresse en cliquant sur le bouton ci-dessous.
                                                    </p>

                                                    <table cellpadding='0' cellspacing='0' style='margin: 32px auto;'>
                                                        <tr>
                                                            <td align='center' style='background-color:#0d6efd; border-radius:6px;'>
                                                                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'
                                                                   style='display:inline-block; padding: 14px 32px; color:#ffffff; text-decoration:none; font-size:16px; font-weight:bold;'>
                                                                    Confirmer ma nouvelle adresse
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <p style='color:#6c757d; font-size:13px; line-height:1.6;'>
                                                        Si vous n'avez pas demandé ce changement, ignorez ce message.
                                                        Votre ancienne adresse courriel restera active et ce lien expirera sous peu.
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

                StatusMessage = "Lien de confirmation envoyé pour modifier le courriel. Veuillez vérifier votre boîte.";
                return RedirectToPage();
            }

            StatusMessage = "Votre courriel est inchangé.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec l'ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirmez votre courriel",
                $"Veuillez confirmer votre compte en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");

            StatusMessage = "Courriel de vérification envoyé. Veuillez vérifier votre boîte.";
            return RedirectToPage();
        }
    }
}
