using Pdf2PdfInsertor.Core.Itf.Model;

namespace Pdf2PdfInsertor.Core.Itf
{
    public interface ICerfaInsertor
    {
        void InsertJudgmentIntoCerfa(JugementArgs jugementArgs);
    }
}