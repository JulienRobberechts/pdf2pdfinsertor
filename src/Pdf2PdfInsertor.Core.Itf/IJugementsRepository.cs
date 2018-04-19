using Pdf2PdfInsertor.Core.Itf.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf2PdfInsertor.Core.Itf
{
    public interface IJugementsRepository
    {
        IEnumerable<JugementArgs> Jugements(string cerfaFormPath, string jugementsDirPath, string outDirPath);
    }
}
