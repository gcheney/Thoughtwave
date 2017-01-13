using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft​.AspNetCore​.Hosting;

namespace Thoughtwave.Services
{
    public class FileUploader : IFileUploader
    {
        private IHostingEnvironment _environment;

        public FileUploader(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFile(IFormFileCollection uploadedFiles, string destinationPath, 
            IEnumerable<string> validFileFormats)
        {
            if (uploadedFiles.Any())
            {
                foreach (var file in uploadedFiles)
                {
                    var isValidFormat = validFileFormats.Any(s => file.FileName.EndsWith(s));

                    if (file != null && file.Length > 0 && isValidFormat)
                    {
                        string rootPath = Path.Combine(_environment.WebRootPath, destinationPath);
                        var filePath = Path.Combine(rootPath, file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            return $"/{destinationPath}/{file.FileName}";
                        }
                    }
                }
            }

            return null;
        }
    }
}