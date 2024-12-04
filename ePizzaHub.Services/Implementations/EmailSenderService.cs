using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using System.Net.Mail;
using System.Net;

using ePizzaHub.Models;
using System.Security.Cryptography;

namespace ePizzaHub.Services.Implementations
{
    public class EmailSenderService : IEmailSenderService
    {
        IConfiguration _configuration;
        IWebHostEnvironment _webHostEnvironment;

        public EmailSenderService(IConfiguration configuration,IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment= webHostEnvironment;
        }
        public async Task<bool> EmailSend(string email, string subject, string message)
        {
            bool status = false;
            try
            {
               EmailSettingsModel getEmailSettings = new EmailSettingsModel()
                {
                    SecretKey =  _configuration.GetValue<string>("EmailSettings:GmailSecretKey"),
                    From =       _configuration.GetValue<string>("EmailSettings:EmailSetting:From"),
                    SmtpServer = _configuration.GetValue<string>("EmailSettings:EmailSetting:SmtpServer"),
                    Port =       _configuration.GetValue<int>("EmailSettings:EmailSetting:port"),
                    EnableSSL =  _configuration.GetValue<bool>("EmailSettings:EmailSetting:EnableSSL"),
                };

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSettings.From),
                    Subject = subject,
                    Body = message,
                    BodyEncoding = System.Text.Encoding.ASCII,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                SmtpClient smtpClient = new SmtpClient(getEmailSettings.SmtpServer)
                {
                    Port = getEmailSettings.Port,
                    Credentials = new NetworkCredential(getEmailSettings.From, getEmailSettings.SecretKey),
                    EnableSsl = getEmailSettings.EnableSSL
                };

                await smtpClient.SendMailAsync(mailMessage);
                status = true;
            }
            catch (Exception ex)
            {

                status = false;
            }
            return status;
        }

        public string GenerateOtp(int length=6)
        {
            if (length <= 0)
                return null;

            byte[] data = new byte[length];
            RandomNumberGenerator.Fill(data); // Fill the byte array with cryptographically strong random values.

            var otp = string.Empty;
            foreach (var b in data)
            {
                otp += (b % 10).ToString(); // Convert each byte to a single digit.
            }

            return otp.Substring(0, length); // Ensure the OTP is exactly the required length.
        }

    }
}
