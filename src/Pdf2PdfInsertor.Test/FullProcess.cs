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

            var inputContentPdfPath = Path.GetFullPath(@"./Pdf2PdfInsertor.Test/TestData/content-2pages.pdf");
            if (!File.Exists(inputContentPdfPath))
                throw new Exception($"File do not exsits: '{inputContentPdfPath}'");

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
                    SourcePdfPath = inputContentPdfPath,
                    SourcePageIndex = 1
                }
            };
            CerfaInsertor.Run(actions, outputDirPath, "final");
        }
    }
}
