using System;
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
                    var fileName = GenerateUniqueFileName(file.FileName);
                    var filePath = Path.Combine(rootPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        return $"/{destinationPath}/{fileName}";
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

        /// <summary>
        /// Generates a unique file name using the provided file name, current timestamp and Guid
        /// </summary>
        /// <param name="fileName">The files current name</param>
        /// <returns>
        /// The newly generated, unique file name
        /// </returns>
        private string GenerateUniqueFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var context = Path.GetFileNameWithoutExtension(fileName);
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var guid = Guid.NewGuid().ToString("N");

            return $"{context}_{timeStamp}_{guid}{extension}";
        }
    }
}