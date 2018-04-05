﻿using iTextSharp.awt.geom;
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

            var rectoPageCount = 15;
            var versoPageCount = 15;

            var totalPageCount = rectoPageCount + versoPageCount;

            for (int i = 1; i <= totalPageCount; i++)
            {
                var a = new PdfActionInsertImage()
                {
                    ResultPageIndex = i,
                    ModelPdfPath = formPdfPath,
                };

                var recto = (i % 2 == 1);

                if (i == 1)
                    a.ModelPageIndex = 1;
                else if (recto)
                    a.ModelPageIndex = 3;
                else
                    a.ModelPageIndex = 2;

                a.SourcePdfPath = recto ? rectoPdfPath : versoPdfPath;

                var index = (i + 1) / 2;

                a.SourcePageIndex = recto ? index : versoPageCount + 1 - index;

                actions.Add(a);
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
        //            ModelPageIndex = 1,
        //            SourcePdfPath = rectoPdfPath,
        //            SourcePageIndex = 1
        //        },
        //        new PdfActionInsertImage
        //        {
        //            ResultPageIndex = 2,
        //            ModelPdfPath = formPdfPath,
        //            ModelPageIndex = 2,
        //            SourcePdfPath = versoPdfPath,
        //            SourcePageIndex = 15
        //        },
        //        new PdfActionInsertImage
        //        {
        //            ResultPageIndex = 3,
        //            ModelPdfPath = formPdfPath,
        //            ModelPageIndex = 3,
        //            SourcePdfPath = rectoPdfPath,
        //            SourcePageIndex = 2
        //        },
        //        new PdfActionInsertImage
        //        {
        //            ResultPageIndex = 4,
        //            ModelPdfPath = formPdfPath,
        //            ModelPageIndex = 2,
        //            SourcePdfPath = versoPdfPath,
        //            SourcePageIndex = 14
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
            PdfToJpegConvertor.RenderPage(action.SourcePdfPath, srcPageIndex, imageSize, imgContentPath);
            if (!File.Exists(imgContentPath))
                throw new Exception($"File do not exsits: '{imgContentPath}'");

            // Insert image into Pdf
            var resultPdfPath = tempDir + "page-" + resultPageIndex + ".pdf";
            InsertImageToPdf(action.ModelPdfPath, resultPdfPath, imgContentPath, modelPageIndex, rectDestination);

            return new PdfActionResult { Action = action, ResultPdfPath = resultPdfPath };
        }

        private static Rectangle GetDestinationRect(string pdfPath, int pageIndex)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                reader.SelectPages(pageIndex.ToString());
                AcroFields af = reader.AcroFields;
                var fieldDestination = af.Fields.SingleOrDefault(kv => kv.Key.EndsWith("1"));
                var positions = af.GetFieldPositions(fieldDestination.Key);
                var destRect = positions.First().position;
                return destRect;
            }
        }

        private static void InsertImageToPdf(string inputPdfPath, string outputPdfPath, string imagePathToInsert, int destPageIndex, Rectangle destinationRect)
        {
            using (Stream inputImageStream = new FileStream(imagePathToInsert, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var pdfReader = new PdfReader(inputPdfPath))
            using (var stamper = new PdfStamper(pdfReader, outputPdfStream))
            {
                var pdfContentByte = stamper.GetOverContent(destPageIndex);

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
