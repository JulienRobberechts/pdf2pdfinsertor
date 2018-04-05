using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdftoImgoConv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTests
{
    public class Insertor
    {
        public static void Insert()
        {
            var imgPixelFactor = 3;
            var formPdfPath = @"D:\temp\pdf\3265-sd_434.pdf";
            var inputPdfPath = @"D:\temp\pdf\1.pdf";
            var outputPdfPath = @"D:\temp\pdf\1-out.pdf";

            for (int pageIndex = 1; pageIndex <= 1; pageIndex++)
            {
                // Calculate Destination rectangle in the form
                var rectDestination = GetDestinationRect(formPdfPath, pageIndex);

                // calculate image size
                var rect = GetPageSize(inputPdfPath, pageIndex);
                var imageSize = new System.Drawing.Size((int)(rect.Width * imgPixelFactor), (int)(rect.Height * imgPixelFactor));

                // generate image
                var img1PdfPath = @"D:\temp\pdf\" + $"1-{pageIndex}-img.jpeg";
                PdfToJpegConvertor.RenderPage(inputPdfPath, pageIndex, imageSize, img1PdfPath);

                // Insert image into Pdf
                InsertImageToPdf(formPdfPath, outputPdfPath, img1PdfPath, pageIndex, rectDestination);
            }
        }

        private static Rectangle GetDestinationRect(string pdfPath, int pageIndex)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                reader.SelectPages(pageIndex.ToString());
                // var pageSize = reader.GetPageSize(1);
                AcroFields af = reader.AcroFields;
                var fieldDestination = af.Fields.SingleOrDefault(kv => kv.Key.EndsWith("1"));
                var positions = af.GetFieldPositions(fieldDestination.Key);
                var destRect = positions.First().position;
                return destRect;
            }
        }

        private static void InsertImageToPdf(string inputPdfPath, string outputPdfPath, string imagePath, int pageIndex, Rectangle destinationRect)
        {
            using (Stream inputPdfStream = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream inputImageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new PdfReader(inputPdfStream);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(pageIndex);

                Image image = Image.GetInstance(inputImageStream);

                image.ScaleToFit(destinationRect);
                image.SetAbsolutePosition(destinationRect.GetLeft(0), destinationRect.GetBottom(0));

                pdfContentByte.AddImage(image);
                stamper.Close();
            }
        }

        private static Rectangle GetPageSize(string pdfPath, int pageIndex)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                reader.SelectPages(pageIndex.ToString());
                var pageSize = reader.GetPageSize(1);
                return pageSize;
            }
        }
    }
}
