using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ComputerGraphicsApplication.Models.Common
{
    public class UploadFileModel
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public string StorageName { get; set; }
        public string ContentType { get; set; }

        public static async Task<UploadFileModel> CreateAsync(IFormFile formFile, string storageName)
        {
            var file = new UploadFileModel
            {
                FileName = formFile.FileName,
                Length = formFile.Length,
                Stream = new MemoryStream(),
                StorageName = storageName,
                ContentType = formFile.ContentType
            };
            await formFile.CopyToAsync(file.Stream);

            return file;
        }
    }
}