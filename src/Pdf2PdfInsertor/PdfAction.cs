using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTests
{
    public class PdfActionInsertImage
    {
        public int ResultPageIndex { get; set; }

        public string SourcePdfPath { get; set; }
        public int SourcePageIndex { get; set; }

        public string ModelPdfPath { get; set; }
        public int ModelPageIndex { get; set; }
    }

    public class PdfActionResult
    {
        public int ResultPageIndex { get; set; }
        public string ResultPdfPath { get; set; }
    }
}
