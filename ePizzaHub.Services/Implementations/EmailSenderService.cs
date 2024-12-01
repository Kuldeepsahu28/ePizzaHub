using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ePizzaHub.Models;

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

        public string GetEmailBody(string email,string Template,string Title)
        {
            //string url = _configuration.GetValue<string>("Urls:LoginUrl");
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates/"+Template);
            string htmlString = System.IO.File.ReadAllText(path);
            htmlString = htmlString.Replace("{{title}}",Title);
            htmlString = htmlString.Replace("{{Username}}", email);
            //htmlString = htmlString.Replace("{{url}}", url);
            return htmlString;
        }
    }
}
