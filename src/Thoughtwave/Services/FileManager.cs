using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft​.AspNetCore​.Hosting;

namespace Thoughtwave.Services
{
    public class FileManager : IFileManager
    {
        private IHostingEnvironment _environment;

        public FileManager(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFileCollection files, string destinationPath, 
            IEnumerable<string> validFileFormats)
        {
            if (files.Any())
            {
                var file = files.FirstOrDefault();
                var isValidFormat = validFileFormats.Any(s => file.FileName.EndsWith(s));

                if (file != null && file.Length > 0 && isValidFormat)
                {
                    var rootPath = Path.Combine(_environment.WebRootPath, destinationPath);
                    var filePath = Path.Combine(rootPath, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        return $"/{destinationPath}/{file.FileName}";
                    }
                }
            }

            return null;
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = _environment.WebRootPath + filePath;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}