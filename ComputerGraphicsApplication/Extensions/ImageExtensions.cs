using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerGraphicsApplication.Services;

namespace ComputerGraphicsApplication.Extensions
{
    public static class ImageExtensions
    {
        public static async Task SaveToStorage(this Bitmap bitmap,
                                               FileService fileService,
                                               string fileName)
        {
            await using var memStream = new MemoryStream();
            var myImageCodecInfo = GetEncoderInfo("image/png");
            var myEncoder = Encoder.Quality;
            var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            var myEncoderParameters = new EncoderParameters(1) {Param = {[0] = myEncoderParameter}};
            bitmap.Save(memStream, myImageCodecInfo, myEncoderParameters);
            await fileService.SaveStreamToFileAsync(memStream, FileService.FilterImagesStorage, fileName);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();
            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}