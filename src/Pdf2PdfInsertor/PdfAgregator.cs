using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTests
{
    public static class PdfAgregator
    {
        public static Rectangle GetPageSize(string filePath)
        {
            try
            {
                Rectangle pageSize;
                using (PdfReader reader0 = new PdfReader(filePath))
                {
                    pageSize = reader0.GetPageSizeWithRotation(1);
                    reader0.Close();
                }
                return pageSize;
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible to get the page size", ex);
            }
        }

        public static void ConcatPages(List<PdfActionResult> docs, string outputFilePath, Rectangle pageSize)
        {
            try
            {
                // Capture the correct size and orientation for the page:
                using (var resultDoc = new Document(pageSize))
                using (var fs = new FileStream(outputFilePath, FileMode.Create))
                using (var pdfCopy = new PdfSmartCopy(resultDoc, fs))
                {
                    resultDoc.Open();
                    foreach (var doc in docs)
                    {
                        using (PdfReader reader = new PdfReader(doc.ResultPdfPath))
                        {
                            PdfImportedPage importedPage = pdfCopy.GetImportedPage(reader, doc.Action.ModelPageIndex);
                            pdfCopy.AddPage(importedPage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible to concatenate Pages", ex);
            }
        }
    }
}
