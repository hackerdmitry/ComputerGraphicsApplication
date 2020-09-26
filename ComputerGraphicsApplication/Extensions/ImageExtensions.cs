using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ComputerGraphicsApplication.Services;

namespace ComputerGraphicsApplication.Extensions
{
    public static class ImageExtensions
    {
        public static async Task SaveToStorage(this Bitmap bitmap, FileService fileService, string fileName)
        {
            await using var memStream = new MemoryStream();
            bitmap.Save(memStream, bitmap.RawFormat);
            await fileService.SaveStreamToFileAsync(memStream, FileService.FilterImagesStorage, fileName);
        }
    }
}