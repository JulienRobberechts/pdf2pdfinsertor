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
    public class InsertJudgmentIntoCerfaTest
    {
        [Test]
        public void InsertJudgmentIntoCerfa()
        {
            var formPdfPath = Path.GetFullPath(@"./../data/ModeleCerfa/FormCerfa-3265-sd_434.pdf");
            if (!File.Exists(formPdfPath))
                throw new Exception($"File do not exsits: '{formPdfPath}'");

            var rectoPdfPath = Path.GetFullPath(@"./../data/Jugements/ROBBERECHTS/2014 RECTO JGT signifié ROBBERECHTS.pdf");
            if (!File.Exists(rectoPdfPath))
                throw new Exception($"File do not exsits: '{rectoPdfPath}'");

            var versoPdfPath = Path.GetFullPath(@"./../data/Jugements/ROBBERECHTS/2014 VERSO JGT signifié ROBBERECHTS.pdf");
            if (!File.Exists(versoPdfPath))
                throw new Exception($"File do not exsits: '{versoPdfPath}'");

            string outputDirPath = Path.GetFullPath("./../data/CerfaRemplies/");
            var outputDir = new DirectoryInfo(outputDirPath);
            if (!outputDir.Exists)
                outputDir.Create();

            CerfaInsertor.InsertJudgmentIntoCerfa(formPdfPath, rectoPdfPath, versoPdfPath, outputDirPath, "CERFA-ROBBERECHTS");
        }
    }
}
