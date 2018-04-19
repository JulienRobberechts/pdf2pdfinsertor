using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf2PdfInsertor.Core.Itf.Model
{
    public struct JugementArgs
    {
        public string Name { get; set; }

        public string FormPdfPath { get; set; }
        public string RectoPdfPath { get; set; }
        public string VersoPdfPath { get; set; }
        public string OutputDirPath { get; set; }
        public string OutputFileName { get; set; }

        public double? leftMarginInCm { get; set; }
    }
}
