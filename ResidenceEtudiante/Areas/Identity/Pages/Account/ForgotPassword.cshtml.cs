// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ResidenceEtudiante.Areas.Identity.Data;

namespace ResidenceEtudiante.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "L'adresse courriel est requise.")]
            [EmailAddress(ErrorMessage = "Adresse courriel invalide.")]
            [Display(Name = "Adresse courriel")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Do not reveal if the user does not exist
                    ModelState.AddModelError(string.Empty, "Aucun compte n’est associé à cette adresse courriel.");
                    return Page();
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Vous devez confirmer votre adresse courriel avant de réinitialiser votre mot de passe.");
                    return Page();
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
                try
                {
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Réinitialisation de votre mot de passe – Résidences Étudiantes",
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
                                                <td align='center' style='background-color:#dc3545; padding: 32px 40px;'>
                                                    <h1 style='color:#ffffff; margin:0; font-size:24px;'>Résidences Étudiantes</h1>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style='padding: 40px;'>
                                                    <h2 style='color:#212529; margin-top:0;'>Réinitialisation de votre mot de passe</h2>
                                                    <p style='color:#495057; line-height:1.6;'>Bonjour,</p>
                                                    <p style='color:#495057; line-height:1.6;'>
                                                        Nous avons reçu une demande de réinitialisation du mot de passe associé à votre compte 
                                                        <strong>Résidences Étudiantes</strong>. Cliquez sur le bouton ci-dessous pour choisir un nouveau mot de passe.
                                                    </p>

                                                    <table cellpadding='0' cellspacing='0' style='margin: 32px auto;'>
                                                        <tr>
                                                            <td align='center' style='background-color:#dc3545; border-radius:6px;'>
                                                                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'
                                                                   style='display:inline-block; padding: 14px 32px; color:#ffffff; text-decoration:none; font-size:16px; font-weight:bold;'>
                                                                    Réinitialiser mon mot de passe
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <p style='color:#6c757d; font-size:13px; line-height:1.6;'>
                                                        Si vous n'avez pas demandé de réinitialisation, ignorez ce message. 
                                                        Votre mot de passe restera inchangé et ce lien expirera sous peu.
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

            return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
