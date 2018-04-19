using Pdf2PdfInsertor.ConsoleUi;
using Pdf2PdfInsertor.Core.Itf;
using PdfTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf2PdfInsertor
{
    class Program
    {
        static void Main(string[] args)
        {
            MainConsoleUi ui = new MainConsoleUi();
            ICerfaInsertor pdfService = new CerfaInsertor();
            IJugementsRepository jugementsRepository = new JugementsArgsGenerator();

            ui.Run(jugementsRepository, pdfService);
        }
    }
}
