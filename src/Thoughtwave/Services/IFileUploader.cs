using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Thoughtwave.Services
{
    /// <summary>
    /// Uploads the first valid file of an IFormFileCollection 
    /// </summary>
    /// <param name="uploadedFiles">Files that have been sent with the HTTP request</param>
    /// <param name="destinationPath">The file path destination for the file</param>
    /// <param name="validFileFormats">The file extensions to accept</param>
    /// <returns>
    /// The relative path to the uploaded file
    /// </returns>
    public interface IFileUploader
    {
        Task<string> UploadFile(IFormFileCollection uploadedFiles, string destinationPath, 
            IEnumerable<string> validFileFormats);
    }
}
