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
}
