using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = new DirectoryInfo(".");
            var pdfs = files.GetFiles("*.pdf").ToList();
            pdfs.ForEach(pdf =>
             {
                 var pages = new List<string>();
                 using (var pdfReader = new PdfReader(pdf.FullName))
                 {
                     for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                     {
                         var strategy = new SimpleTextExtractionStrategy();
                         var currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                         pages.Add(currentPageText);
                     }
                 }
                 var output = pdf.Name + ".txt";
                 File.Delete(output);
                 using (var writer = new StreamWriter(File.OpenWrite(output)))
                 {
                     pages.ForEach(page => writer.WriteLine(page));
                 }
             });
                

        }
    }
}
