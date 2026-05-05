using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using S14_ProjetSession;

public class AzureEmailSender : IEmailSender
{
    private readonly AzureEmailSettings _settings;

    public AzureEmailSender(IOptions<AzureEmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            EmailClient emailClient = new EmailClient(_settings.ConnectionString);
            EmailMessage emailMessage = new EmailMessage(
                senderAddress: _settings.SenderAddress,
                content: new EmailContent(subject)
                {
                    Html = htmlMessage,
                    PlainText = "Veuillez ouvrir ce courriel en HTML."
                },
                recipients: new EmailRecipients(new[]
                {
                    new EmailAddress(email)
                }));

            await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
        }
        catch (RequestFailedException ex) when (ex.Status == 401 || ex.Status == 403)
        {
            throw new EmailException("Le service de messagerie est temporairement indisponible. Veuillez réessayer plus tard.");
        }
        catch (RequestFailedException)
        {
            throw new EmailException("L'envoi du courriel a échoué. Veuillez vérifier votre adresse et réessayer.");
        }
        catch (Exception)
        {
            throw new EmailException("Une erreur inattendue est survenue. Veuillez réessayer ou contacter le support.");
        }
    }
}