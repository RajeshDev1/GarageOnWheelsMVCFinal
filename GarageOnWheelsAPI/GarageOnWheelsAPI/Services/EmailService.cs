using GarageOnWheelsAPI.Interfaces.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Security.Authentication;

namespace GarageOnWheelsAPI.Services
{
    public class EmailService 
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }


        public async Task SendOtpEmailAsync(string email, string otpCode)
        {
            _logger.LogInformation("Preparing to send OTP email to: {Email}", email);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GarageOnWheelsApp", "paridajamuna123@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your Verification Code is {otpCode}"
            };

            using var client = new SmtpClient();
            try
            {
                client.CheckCertificateRevocation = false; // Disables certificate revocation check
                client.SslProtocols = SslProtocols.Tls12;  // Specify the SSL/TLS version

                _logger.LogInformation("Connecting to SMTP server...");
                await client.ConnectAsync("smtp.gmail.com", 587);
                _logger.LogInformation("Authenticating...");
                await client.AuthenticateAsync("paridajamuna123@gmail.com", "cwgx dqky qnfv dodm");

                _logger.LogInformation("Sending OTP email...");
                await client.SendAsync(message);
                _logger.LogInformation("OTP email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the OTP email.");
            }
            finally
            {
                _logger.LogInformation("Disconnecting from SMTP server...");
                await client.DisconnectAsync(true);
                _logger.LogInformation("Disconnected from SMTP server.");
            }
        }


    }
}
