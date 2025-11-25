using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Notificaciones.Services
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;
        private readonly string[] _dominiosPermitidos = { "@cuc.cr", "@cuc.ac.cr" };

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        private bool TryCreateEmail(string input, out MailAddress result)
        {
            try
            {
                result = new MailAddress(input.Trim());
                return true;
            }
            catch (FormatException)
            {
            }
            catch (ArgumentException)
            {
            }
            result = null;
            return false;
        }

        private bool ValidarDominio(string email)
        {

            return _dominiosPermitidos.Any(dominio =>
                email.EndsWith(dominio, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<(bool Success, string Message)> EnviarCorreoMultipleAsync(
            string destinatarios, string asunto, string cuerpoHtml)
        {
            try
            {

                var emails = destinatarios.Split(new[] { ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries);

                var emailsValidos = new List<MailAddress>();
                var emailsInvalidos = new List<string>();
                var emailsDominioIncorrecto = new List<string>();

                foreach (var email in emails)
                {
                    var emailLimpio = email.Trim();

                    if (TryCreateEmail(emailLimpio, out var mailAddress))
                    {

                        if (ValidarDominio(emailLimpio))
                        {
                            emailsValidos.Add(mailAddress);
                        }
                        else
                        {
                            emailsDominioIncorrecto.Add(emailLimpio);
                        }
                    }
                    else
                    {
                        emailsInvalidos.Add(emailLimpio);
                    }
                }

                if (emailsInvalidos.Any())
                    return (false, $"Emails con formato inválido: {string.Join(", ", emailsInvalidos)}");

                if (emailsDominioIncorrecto.Any())
                    return (false, $"Solo se permiten dominios @cuc.cr y @cuc.ac.cr. Dominios rechazados: {string.Join(", ", emailsDominioIncorrecto)}");

                if (!emailsValidos.Any())
                    return (false, "No se proporcionaron direcciones de correo válidas");

 
                var smtpSection = _config.GetSection("SmtpSettings");
                string servidor = smtpSection["Server"];
                int puerto = int.Parse(smtpSection["Port"]);
                string usuario = smtpSection["User"];
                string clave = smtpSection["Password"];
                bool enableSsl = bool.Parse(smtpSection["EnableSsl"]);

                using var smtp = new SmtpClient(servidor, puerto)
                {
                    Credentials = new NetworkCredential(usuario, clave),
                    EnableSsl = enableSsl
                };


                var mail = new MailMessage
                {
                    From = new MailAddress(usuario, "Sistema de Notificaciones"),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true
                };


                foreach (var email in emailsValidos)
                {
                    mail.To.Add(email);
                }


                await smtp.SendMailAsync(mail);

                return (true, $"Correo enviado exitosamente a {emailsValidos.Count} destinatario(s)");
            }
            catch (SmtpException ex)
            {
                return (false, $"Error SMTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al enviar correo: {ex.Message}");
            }
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var smtpSection = _config.GetSection("SmtpSettings");
            string servidor = smtpSection["Server"];
            int puerto = int.Parse(smtpSection["Port"]);
            string usuario = smtpSection["User"];
            string clave = smtpSection["Password"];
            bool enableSsl = bool.Parse(smtpSection["EnableSsl"]);

            using var smtp = new SmtpClient(servidor, puerto)
            {
                Credentials = new NetworkCredential(usuario, clave),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(usuario, "Sistema de Notificaciones"),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mail.To.Add(destinatario);
            await smtp.SendMailAsync(mail);
        }
    }
}
