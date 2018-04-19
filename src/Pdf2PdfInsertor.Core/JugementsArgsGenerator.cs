﻿using Pdf2PdfInsertor.Core.Itf;
using Pdf2PdfInsertor.Core.Itf.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTests
{
    public class JugementsArgsGenerator : IJugementsRepository
    {
        public IEnumerable<JugementArgs> Jugements(string cerfaFormPath, string jugementsDirPath, string outDirPath)
        {
            CheckFile("Cerfa Model", cerfaFormPath);
            CheckDir("Jugements", jugementsDirPath);
            CheckDirOrCreate("output", outDirPath);
            var cerfaFileName = Path.GetFileNameWithoutExtension(cerfaFormPath);

            // for test:
            //string tmpDirPath = Path.GetFullPath($"{testDir}/../../../../data/tmp/");
            //var tmpDir = new DirectoryInfo(tmpDirPath);
            //if (!tmpDir.Exists)
            //    tmpDir.Create();
            //CerfaInsertor.tempDir = tmpDirPath;

            var jugements = new List<JugementArgs>();
            var namePaths = Directory.GetDirectories(jugementsDirPath);

            foreach (var namePath in namePaths)
            {
                var name = Path.GetFileName(namePath);

                var rectoPdfPath = Path.GetFullPath($"{jugementsDirPath}/{name}/2014 RECTO JGT signifié {name}.pdf");
                CheckFile("Recto Pdf", rectoPdfPath);

                var versoPdfPath = Path.GetFullPath($"{jugementsDirPath}/{name}/2014 VERSO JGT signifié {name}.pdf");
                CheckFile("Verso Pdf", versoPdfPath);

                jugements.Add(new JugementArgs()
                {
                    Name = name,
                    FormPdfPath = cerfaFormPath,
                    RectoPdfPath = rectoPdfPath,
                    VersoPdfPath = versoPdfPath,
                    OutputDirPath = outDirPath,
                    OutputFileName = $"{name}-{cerfaFileName}",
                    leftMarginInCm = GetParameterFromFile($"{jugementsDirPath}/{name}", "marge")
                });
            }

            return jugements;
        }

        private double? GetParameterFromFile(string srcDirPath, string parameterName)
        {
            var srcDir = new DirectoryInfo(srcDirPath);
            var files = srcDir.GetFiles($"{parameterName} *.txt", SearchOption.TopDirectoryOnly);
            if (files.Count() > 1)
                throw new Exception($"There is multiple '{parameterName}' parameter files in the directory '{srcDirPath}'");

            if (files.Count() == 1)
            {
                var paramAsString = files.First().Name.ToLower()
                    .Replace($"{parameterName} ", "")
                    .Replace("cm", "")
                    .Replace(".txt", "")
                    .Trim();

                if (Double.TryParse(paramAsString, out double paramValue))
                    return paramValue;
            }

            return null;
        }

        private void CheckDirOrCreate(string dirName, string dirPath)
        {
            var parentDirPath = Path.GetDirectoryName(dirPath);
            if (!Directory.Exists(parentDirPath))
                throw new Exception($"Directory Parent for '{dirName}' do not exsits: '{parentDirPath}'");

            var dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
                dir.Create();
        }

        private void CheckDir(string dirName, string dirPath)
        {
            if (!Directory.Exists(dirPath))
                throw new Exception($"Directory '{dirName}' do not exsits: '{dirPath}'");
        }

        private void CheckFile(string fileName, string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception($"File '{fileName}' do not exsits: '{filePath}'");
        }
    }
}