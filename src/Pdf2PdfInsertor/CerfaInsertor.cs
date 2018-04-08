using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdftoImgoConv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfTests
{
    public static class CerfaInsertor
    {
        public static string tempDir = Path.GetTempPath();
        public static int imgPixelFactor = 3;

        public static void InsertJudgmentIntoCerfa(string formPdfPath, string rectoPdfPath, string versoPdfPath, string outputDirPath, string outputFileName)
        {
            var actions = BuildActions(formPdfPath, rectoPdfPath, versoPdfPath);
            RunPdfActions(actions, outputDirPath, outputFileName);
        }

        private static IEnumerable<PdfActionInsertImage> BuildActions(string formPdfPath, string rectoPdfPath, string versoPdfPath)
        {
            var actions = new List<PdfActionInsertImage>();

            var pageCountToskipAtThaEndOfVerso = 2;

            var rectoPageCount = 15;
            var versoPageCount = 15;

            var totalPageCount = rectoPageCount + versoPageCount;

            var pageIndexDisplay = 1;

            for (int i = 1; i <= totalPageCount; i++)
            {
                var a = new PdfActionInsertImage()
                {
                    ResultPageIndex = pageIndexDisplay,
                    ModelPdfPath = formPdfPath,
                    FullPageLabel = pageIndexDisplay.ToString() // +"/"+ totalPageCount // It's too large !
                };

                var recto = (i % 2 == 1);

                if (i == 1)
                    a.ModelPageIndex = 1;
                else
                    a.ModelPageIndex = 2;

                a.SourcePdfPath = recto ? rectoPdfPath : versoPdfPath;

                var index = (i + 1) / 2;

                a.SourcePageIndex = recto ? index : versoPageCount + 1 - index;

                if (!recto && a.SourcePageIndex <= pageCountToskipAtThaEndOfVerso)
                    continue;

                actions.Add(a);
                pageIndexDisplay++;
            }

            return actions;
        }

        //private static IEnumerable<PdfActionInsertImage> BuildActionsTEST(string formPdfPath, string rectoPdfPath, string versoPdfPath)
        //{
        //    var actions = new List<PdfActionInsertImage>
        //    {
        //        new PdfActionInsertImage
        //        {
        //            ResultPageIndex = 1,
        //            ModelPdfPath = formPdfPath,
        //            ModelPageIndex = 5,
        //            SourcePdfPath = rectoPdfPath,
        //            SourcePageIndex = 3,
        //            FullPageLabel = "XX"
        //        }
        //    };

        //    return actions;
        //}

        public static void RunPdfActions(IEnumerable<PdfActionInsertImage> actions, string outputDirPath, string outputFileName)
        {
            var results = actions.Select(a => CreatePdfPage(a)).ToList();

            // Concat each page into a single file....
            var pageSize = PdfAgregator.GetPageSize(results.First().Action.ModelPdfPath);

            var finalPdfPath = outputDirPath + outputFileName + ".pdf";
            PdfAgregator.ConcatPages(results, finalPdfPath, pageSize);
        }

        public static PdfActionResult CreatePdfPage(PdfActionInsertImage action)
        {
            // const int imgPixelFactor = 3;

            var srcPageIndex = action.SourcePageIndex;
            var modelPageIndex = action.ModelPageIndex;
            var resultPageIndex = action.ResultPageIndex;

            // Calculate Destination rectangle in the form
            var rectDestination = GetDestinationRect(action.ModelPdfPath, modelPageIndex);

            // calculate image size
            var rect = GetPageSize(action.SourcePdfPath, srcPageIndex);
            var imageSize = new System.Drawing.Size((int)(rect.Width * imgPixelFactor), (int)(rect.Height * imgPixelFactor));

            // generate image
            var imgContentPath = tempDir + Path.GetFileNameWithoutExtension(action.SourcePdfPath) + "-p" + srcPageIndex + ".jpeg";
            PdfToJpegConvertor.GetPageAsImage(action.SourcePdfPath, srcPageIndex, imageSize, imgContentPath);
            if (!File.Exists(imgContentPath))
                throw new Exception($"File do not exsits: '{imgContentPath}'");

            // Insert image into Pdf
            var resultPdfPathA = tempDir + "page-" + resultPageIndex + "-a.pdf";
            InsertImageToPdf(action.ModelPdfPath, resultPdfPathA, imgContentPath, modelPageIndex, rectDestination, action.FullPageLabel);

            var resultPdfPathB = tempDir + "page-" + resultPageIndex + "-b.pdf";
            PdfReplacer.FixPageNumberOnPage(resultPdfPathA, resultPdfPathB, modelPageIndex, resultPageIndex.ToString());

            return new PdfActionResult { Action = action, ResultPdfPath = resultPdfPathB };
        }

        private static Rectangle GetDestinationRect(string pdfPath, int pageIndex)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                reader.SelectPages(pageIndex.ToString());
                AcroFields af = reader.AcroFields;
                var fieldDestination = af.Fields.SingleOrDefault(kv => kv.Key.Length == 2 && Char.IsNumber(kv.Key[1]));

                if (fieldDestination.Key == null)
                    throw new Exception("No box found on the page");

                var positions = af.GetFieldPositions(fieldDestination.Key);
                var destRect = positions.First().position;
                return destRect;
            }
        }

        private static void InsertImageToPdf(string inputPdfPath, string outputPdfPath, string imagePathToInsert, int destPageIndex, Rectangle destinationRect, string fullpageLabel)
        {
            using (Stream inputImageStream = new FileStream(imagePathToInsert, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var pdfReader = new PdfReader(inputPdfPath))
            using (var stamper = new PdfStamper(pdfReader, outputPdfStream))
            {
                var pdfContentByte = stamper.GetOverContent(destPageIndex);

                // Insert Image
                Image image = Image.GetInstance(inputImageStream);
                image.ScaleToFit(destinationRect);
                image.SetAbsolutePosition(destinationRect.GetLeft(0), destinationRect.GetTop(0) - image.PlainHeight);
                pdfContentByte.AddImage(image);

                // Change Page Number
                var af = stamper.AcroFields;
                var field = af.Fields.SingleOrDefault(kv => kv.Value.GetPage(0) == destPageIndex && kv.Key.Length == 2 && kv.Key[0] == 'L');

                if (field.Key == null)
                    throw new Exception("No page number found on the page");

                af.SetField(field.Key, fullpageLabel);

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
