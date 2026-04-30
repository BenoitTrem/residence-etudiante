using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using S14_ProjetSession;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;

    public EmailSender(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Nom application", _settings.Email));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;

        BodyBuilder builder = new BodyBuilder();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        using (SmtpClient smtp = new SmtpClient())
        {
            await smtp.ConnectAsync(_settings.Host, _settings.Port,
                MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}