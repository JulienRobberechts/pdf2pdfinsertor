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
        //public static void ConcatPages(List<PdfActionResult> docs, string outputFilePath)
        //{
        //    Rectangle pageSize;
        //    using (PdfReader inputPdf = new PdfReader(docs.First().ResultPdfPath))
        //        pageSize = inputPdf.GetPageSizeWithRotation(1);

        //    Document resultDoc = new Document(pageSize);

        //    using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
        //    {

        //        PdfWriter outputWriter = PdfWriter.GetInstance(resultDoc, fs);

        //        resultDoc.Open();
        //        PdfContentByte cb1 = outputWriter.DirectContent;

        //        foreach (var info in docs)
        //        {
        //            using (PdfReader inputPdf = new PdfReader(info.ResultPdfPath))
        //            {
        //                resultDoc.SetPageSize(inputPdf.GetPageSizeWithRotation(info.Action.ModelPageIndex));
        //                resultDoc.NewPage();

        //                PdfImportedPage page = outputWriter.GetImportedPage(inputPdf, info.Action.ModelPageIndex);
        //                cb1.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
        //            }
        //        }

        //        resultDoc.Close();
        //    }
        //}

        public static void ConcatPages(List<PdfActionResult> docs, string outputFilePath)
        {
            ExtractPage(docs, outputFilePath, 1);
        }

        public static void ExtractPage(List<PdfActionResult> docs, string outputPdfPath, int pageNumber)
        {
            string sourcePdfPath = docs.First().ResultPdfPath;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                PdfReader reader0 = new PdfReader(sourcePdfPath);
                var pageSize = reader0.GetPageSizeWithRotation(pageNumber);
                reader0.Close();

                // Capture the correct size and orientation for the page:
                Document document = new Document(pageSize);

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                var fs = new FileStream(outputPdfPath, FileMode.Create);
                PdfCopy pdfCopyProvider = new PdfCopy(document, fs);

                document.Open();

                // Extract the desired page number:
                //PdfReader reader1 = new PdfReader(sourcePdfPath);
                //PdfImportedPage importedPage1 = pdfCopyProvider.GetImportedPage(reader1, pageNumber);
                //pdfCopyProvider.AddPage(importedPage1);
                //reader1.Close();

                //PdfReader reader2 = new PdfReader(sourcePdfPath2);
                //PdfImportedPage importedPage2 = pdfCopyProvider.GetImportedPage(reader2, pageNumber+1);
                //pdfCopyProvider.AddPage(importedPage2);
                //reader2.Close();

                foreach (var doc in docs)
                {
                    PdfReader reader = new PdfReader(doc.ResultPdfPath);
                    PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, doc.Action.ModelPageIndex);
                    pdfCopyProvider.AddPage(importedPage);
                    reader.Close();
                }

                document.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
