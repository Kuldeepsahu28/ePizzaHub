using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Interfaces
{
    public interface IUtilityService
    {
        string SaveImage(string ContainerName,IFormFile file);

        string EditImage(string ContainerName, IFormFile file, string dbPath);

         void DeleteImage(string ContainerName,string dbPath);
    }
}
