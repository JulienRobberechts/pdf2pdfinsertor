using Pdf2PdfInsertor.Core.Itf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Pdf2PdfInsertor.ConsoleUi
{
    public class MainConsoleUi
    {
        public void Run(IJugementsRepository jugementsRepository, ICerfaInsertor cerfaInsertor)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Pdf 2 Pdf Insertor");
                    Console.WriteLine("");

                    Console.WriteLine("Currrent Directory:");
                    Console.WriteLine("\t" + Directory.GetCurrentDirectory());

                    string jugementsDirPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Jugements"));
                    Console.WriteLine("Jugements Source Directory:");
                    Console.WriteLine("\t" + jugementsDirPath);

                    string cerfaFormPath = Path.GetFullPath($"{jugementsDirPath}/../Cerfa-3265.pdf");
                    Console.WriteLine("Cerfa Model:");
                    Console.WriteLine("\t" + cerfaFormPath);

                    string outDirPath = Path.GetFullPath($"{jugementsDirPath}/../CerfaRemplis/");
                    Console.WriteLine("Output Directory:");
                    Console.WriteLine("\t" + outDirPath);

                    var jugements = jugementsRepository.Jugements(cerfaFormPath, jugementsDirPath, outDirPath);
                    Console.WriteLine($"{jugements.Count()} jugements to process.");

                    if (ShouldQuit())
                        return;

                    foreach (var jugement in jugements)
                    {
                        Console.Write($"Jugement '{jugement.Name}': ...");

                        Stopwatch watch = Stopwatch.StartNew();
                        cerfaInsertor.InsertJudgmentIntoCerfa(jugement);
                        watch.Stop();
                        Console.WriteLine($" {watch.ElapsedMilliseconds} ms");
                    }

                    Console.WriteLine("Done.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Details: {ex}");
                }

                if (ShouldQuit())
                    return;
            }
        }

        public bool ShouldQuit()
        {
            Console.WriteLine("Press Enter to continue or Q to quit.");
            var key = Console.ReadKey().KeyChar;

            if (key == 'Q' || key == 'q')
                return true;

            return false;
        }
    }
}
