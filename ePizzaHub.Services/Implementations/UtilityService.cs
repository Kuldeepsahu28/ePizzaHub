using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ePizzaHub.Services.Implementations
{
    public class UtilityService : IUtilityService
    {

        private IWebHostEnvironment _env;
        private IHttpContextAccessor _contextAccessor;

        public UtilityService(IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            _env = env;
            _contextAccessor = contextAccessor;
        }

        public void DeleteImage(string ContainerName, string dbPath)
        {
          

            var fileName=Path.GetFileName(dbPath);
            var completePath = Path.Combine(_env.WebRootPath, ContainerName, fileName);

            if (File.Exists(completePath))
            {
                File.Delete(completePath);
            }

        }

        public  string EditImage(string ContainerName, IFormFile file, string dbPath)
        {
           DeleteImage(ContainerName, dbPath);
            return  SaveImage(ContainerName, file);
        }

        public  string SaveImage(string ContainerName, IFormFile file)
        {
           var extension=Path.GetExtension(file.FileName);
            var fileName=$"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, ContainerName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filePath=Path.Combine(folder, fileName);

            using (var memoryStreem =new MemoryStream())
            {
                 file.CopyToAsync(memoryStreem);
                var content=memoryStreem.ToArray();
                 File.WriteAllBytesAsync(filePath, content);
            }

            var basePath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}";

            var completePath=Path.Combine("/"+ContainerName,fileName).Replace("\\","/");

            return completePath;
        }
    }
}
