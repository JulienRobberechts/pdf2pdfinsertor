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
        [TestCase("FormCerfa-3265-01", "ROBBERECHTS")]
        [TestCase("FormCerfa-3265-01", "THOMAS N")]
        public void InsertJudgmentIntoCerfa(string cerfa, string name)
        {
            var testDir = TestContext.CurrentContext.TestDirectory;

            var formPdfPath = Path.GetFullPath($"{testDir}/../../../../data/ModeleCerfa/{cerfa}.pdf");
            if (!File.Exists(formPdfPath))
                throw new Exception($"File do not exsits: '{formPdfPath}'");

            var rectoPdfPath = Path.GetFullPath($"{testDir}/../../../../data/Jugements/{name}/2014 RECTO JGT signifié {name}.pdf");
            if (!File.Exists(rectoPdfPath))
                throw new Exception($"File do not exsits: '{rectoPdfPath}'");

            var versoPdfPath = Path.GetFullPath($"{testDir}/../../../../data/Jugements/{name}/2014 VERSO JGT signifié {name}.pdf");
            if (!File.Exists(versoPdfPath))
                throw new Exception($"File do not exsits: '{versoPdfPath}'");

            string outputDirPath = Path.GetFullPath($"{testDir}/../../../../data/CerfaRemplis/");
            var outputDir = new DirectoryInfo(outputDirPath);
            if (!outputDir.Exists)
                outputDir.Create();

            // for test:
            string tmpDirPath = Path.GetFullPath($"{testDir}/../../../../data/tmp/");
            var tmpDir = new DirectoryInfo(tmpDirPath);
            if (!tmpDir.Exists)
                tmpDir.Create();
            CerfaInsertor.tempDir = tmpDirPath;

            // Extract Margin from a Config file
            double? leftMarginInCm = null;

            var srcDirPath = Path.GetDirectoryName(rectoPdfPath);
            var srcDir = new DirectoryInfo(srcDirPath);
            var files = srcDir.GetFiles("Marge *.txt", SearchOption.TopDirectoryOnly);
            if (files.Count() > 1)
                throw new Exception("Multiple margin parameter files");

            if (files.Count() == 1)
            {
                var marginString = files.First().Name.ToLower().Replace("marge ","").Replace("cm", "").Replace(".txt", "").Trim();
                if (Double.TryParse(marginString, out double margin))
                    leftMarginInCm = margin;
            }

            CerfaInsertor.InsertJudgmentIntoCerfa(formPdfPath, rectoPdfPath, versoPdfPath, outputDirPath, $"{cerfa}-{name}", leftMarginInCm);
        }
    }
}
