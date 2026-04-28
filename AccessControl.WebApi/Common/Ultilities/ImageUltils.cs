using System.Drawing;
using System.Drawing.Imaging;

namespace AccessControl.WebApi.Common.Ultilities
{
    public static class ImageUltils
    {
        public static byte[] Base64ToJpeg(this byte[] base64)
        {
            if (base64 == null)
            {
                return null;
            }
            using (Image image = Image.FromStream(new MemoryStream(base64)))
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
        }
    }
}
