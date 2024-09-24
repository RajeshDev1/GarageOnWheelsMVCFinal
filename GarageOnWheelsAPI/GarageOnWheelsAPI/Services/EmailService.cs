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
                client.CheckCertificateRevocation = false; 
                client.SslProtocols = SslProtocols.Tls12;  
                //Connect to the smtp server
                await client.ConnectAsync("smtp.gmail.com", 587);
                //authenticate
                await client.AuthenticateAsync("paridajamuna123@gmail.com", "cwgx dqky qnfv dodm");
                //Send Email
                await client.SendAsync(message);              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the OTP email.");
            }
            finally
            {
                // Disconnect 
                await client.DisconnectAsync(true);
            }
        }
    }
}
