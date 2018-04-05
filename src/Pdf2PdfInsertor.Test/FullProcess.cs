using NUnit.Framework;
using PdfTests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf2PdfInsertor.Test
{
    [TestFixture]
    public class FullProcess
    {
        [Test]
        public void RunFullProcess()
        {
            var formPdfPath = Path.GetFullPath(@"./Pdf2PdfInsertor.Test/TestData/FormCerfa-3265-sd_434.pdf");
            if (!File.Exists(formPdfPath))
                throw new Exception($"File do not exsits: '{formPdfPath}'");

            var inputContentPdfPath1 = Path.GetFullPath(@"./Pdf2PdfInsertor.Test/TestData/content-2pages.pdf");
            if (!File.Exists(inputContentPdfPath1))
                throw new Exception($"File do not exsits: '{inputContentPdfPath1}'");

            var inputContentPdfPath2 = Path.GetFullPath(@"./Pdf2PdfInsertor.Test/TestData/sample.pdf");
            if (!File.Exists(inputContentPdfPath2))
                throw new Exception($"File do not exsits: '{inputContentPdfPath2}'");

            string outputDirPath = Path.GetFullPath("./Pdf2PdfInsertor.Test/TestData/out/");
            var outputDir = new DirectoryInfo(outputDirPath);
            if (!outputDir.Exists)
                outputDir.Create();

            // for test:
            string tmpDirPath = Path.GetFullPath("./Pdf2PdfInsertor.Test/TestData/tmp/");
            var tmpDir = new DirectoryInfo(tmpDirPath);
            if (!tmpDir.Exists)
                tmpDir.Create();
            CerfaInsertor.tempDir = tmpDirPath;

            var actions = new List<PdfActionInsertImage>()
            {
                new PdfActionInsertImage
                {
                    ResultPageIndex = 1,
                    ModelPdfPath = formPdfPath,
                    ModelPageIndex = 1,
                    SourcePdfPath = inputContentPdfPath1,
                    SourcePageIndex = 1
                },
                new PdfActionInsertImage
                {
                    ResultPageIndex = 2,
                    ModelPdfPath = formPdfPath,
                    ModelPageIndex = 2,
                    SourcePdfPath = inputContentPdfPath1,
                    SourcePageIndex = 2
                },
                new PdfActionInsertImage
                {
                    ResultPageIndex = 3,
                    ModelPdfPath = formPdfPath,
                    ModelPageIndex = 3,
                    SourcePdfPath = inputContentPdfPath2,
                    SourcePageIndex = 1
                },
                new PdfActionInsertImage
                {
                    ResultPageIndex = 4,
                    ModelPdfPath = formPdfPath,
                    ModelPageIndex = 2,
                    SourcePdfPath = inputContentPdfPath2,
                    SourcePageIndex = 2
                }
            };
            CerfaInsertor.Run(actions, outputDirPath, "final");
        }
    }
}
