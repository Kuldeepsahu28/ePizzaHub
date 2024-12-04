using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Interfaces
{
    public interface IEmailSenderService
    {
        Task<bool> EmailSend(string email, string subject, string message);

        string GenerateOtp(int length = 6);
    }
}
