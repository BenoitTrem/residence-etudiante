using MailKit.Net.Smtp;
using MailKit.Security;
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
        try
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Residences etudiantes", _settings.Email));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using SmtpClient smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
        catch (AuthenticationException)
        {
            throw new EmailException("Les identifiants de messagerie sont invalides. Veuillez vérifier le nom d'utilisateur et le mot de passe.");
        }
        catch (SmtpCommandException)
        {
            throw new EmailException("Le serveur de messagerie a rejeté la requête. Veuillez vérifier la configuration SMTP.");
        }
        catch (SmtpProtocolException)
        {
            throw new EmailException("Une erreur de protocole SMTP est survenue. Veuillez vérifier les paramètres de connexion.");
        }
        catch (Exception)
        {
            throw new EmailException("Une erreur inattendue est survenue lors de l'envoi du courriel.");
        }
    }
}

public class EmailException : Exception
{
    public EmailException(string message) : base(message) { }
}