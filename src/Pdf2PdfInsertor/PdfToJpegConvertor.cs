using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PdftoImgoConv
{
    public static class PdfToJpegConvertor
    {
        public static void GetPageAsImage(string pdfPath, int pageNumber, Size size, int marginLeft, int marginTop, int marginRight, int marginBottom, string outputPath)
        {
            int dpi = 150;

            using (var document = PdfDocument.Load(pdfPath))
            using (var image = GetPageImage(pageNumber, size, document, dpi))
            {
                Rectangle cropArea = new Rectangle(
                    marginLeft,
                    marginTop,
                    image.Size.Width - marginLeft - marginRight,
                    image.Size.Height - marginTop - marginBottom);
                Bitmap bmpImage = new Bitmap(image);
                var cropedBmpImage = bmpImage.Clone(cropArea, bmpImage.PixelFormat);

                using (var stream = new FileStream(outputPath, FileMode.Create))
                    cropedBmpImage.Save(stream, ImageFormat.Jpeg);
            }
        }

        private static Image GetPageImage(int pageNumber, Size size, PdfDocument document, int dpi)
        {
            return document.Render(pageNumber - 1, size.Width, size.Height, dpi, dpi, PdfRenderFlags.Annotations);
        }
    }
}
