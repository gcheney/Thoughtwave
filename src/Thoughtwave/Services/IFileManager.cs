using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Thoughtwave.Services
{
    public interface IFileManager
    {
        /// <summary>
        /// Uploads the first file of an form collection
        /// </summary>
        /// <param name="uploadedFiles">Files that have been sent with the HTTP request</param>
        /// <param name="destinationPath">The file path destination for the file</param>
        /// <param name="validFileFormats">The file extensions to accept</param>
        /// <returns>
        /// The relative path to the uploaded file
        /// </returns>
        Task<string> UploadFileAsync(IFormFileCollection files, string destinationPath, 
            IEnumerable<string> validFileFormats);

        /// <summary>
        /// Removes the specified file from the system
        /// </summary>
        /// <param name="filePath">The relative path of the file to be deleted</param>
        void DeleteFile(string filePath);
    }
}
