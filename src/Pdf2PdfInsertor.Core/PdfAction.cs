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
        public int SourceMarginLeft { get; set; }

        public string ModelPdfPath { get; set; }
        public int ModelPageIndex { get; set; }

        public string FullPageLabel { get; set; }
    }

    public class PdfActionResult
    {
        public PdfActionInsertImage Action { get; set; }
        public string ResultPdfPath { get; set; }
    }
}
