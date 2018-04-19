using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTests
{
    public static class PdfReplacer
    {
        public static void FixPageNumberOnPage(String src, String dest, int pageIndex, string newPageLabel)
        {
            using (PdfReader reader = new PdfReader(src))
            {
                PdfDictionary dict = reader.GetPageN(pageIndex);
                PdfObject o = dict.GetDirectObject(PdfName.CONTENTS);
                var streams = SearchAllStreams(reader, o);
                foreach (var stream in streams)
                    FixPageNumberOnStream(stream as PRStream, pageIndex.ToString(), newPageLabel);

                using (PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create)))
                    stamper.Close();

                reader.Close();
            }
        }

        private static List<PdfObject> SearchAllStreams(PdfReader reader, PdfObject o)
        {
            List<PdfObject> results = new List<PdfObject>();
            if (o.IsArray())
            {
                var array = (o as PdfArray);
                // for (int i = 0; i < array.ArrayList; i++)

                foreach (var item in array.ArrayList)
                {
                    results.AddRange(SearchAllStreams(reader, item));
                }
            }
            else if (o.IsIndirect())
            {
                var indirect = o as PdfIndirectReference;

                int num = indirect.Number;
                var child = reader.GetPdfObject(num);
                results.AddRange(SearchAllStreams(reader, child));
            }
            else if (o.IsStream())
            {
                PRStream stream = (PRStream)o;
                results.Add(stream);
            }

            return results;
        }

        private static void FixPageNumberOnStream(PRStream stream, string initialLabel, string newPageLabel)
        {
            if (stream == null)
                throw new Exception("The stream is null");

            byte[] data = PdfReader.GetStreamBytes(stream);
            var utf8 = new UTF8Encoding();
            string originalString = utf8.GetString(data);

            if (originalString.Contains($"({initialLabel})Tj"))
            {
                string newString = originalString
                    .Replace($"({initialLabel})Tj", $"({newPageLabel})Tj")
                    .Replace("/F2 1", "/F1 1");
                byte[] newData = utf8.GetBytes(newString);
                stream.SetData(newData);
            }
        }
    }
}
