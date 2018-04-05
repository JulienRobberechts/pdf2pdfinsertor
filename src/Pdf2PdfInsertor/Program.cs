using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdftoImgoConv;
using System;
using System.IO;
using System.Linq;

namespace PdfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pdf 2 Pdf Insertor");
            Console.WriteLine("");
            Insertor.Insert();
            Console.WriteLine("Done.");
            // Console.ReadLine();
        }
    }
}
