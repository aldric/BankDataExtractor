using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace BankDataExtractor
{
    class Program
    {
        public class RateLocator
        {
            public string Label { get; set; }
            public string RegexLocator { get; set; }

            public string GetValue(string text)
            {
                var r = new Regex(RegexLocator, RegexOptions.IgnoreCase);
                return r.Match(text).Groups[0].Value;
            }
        }

        public class BankRateConfiguration
        {
            public string BankName { get; set; }
            public string FileName { get; set; }
            public List<RateLocator> Rates { get; set; }

            public BankRateConfiguration()
            {
                Rates = new List<RateLocator>();
            }
        }


        static void Main(string[] args)
        {
            var conf = new BankRateConfiguration
            {
                BankName = "Monabanq",
                FileName = "monabanq_tarification.pdf",
                Rates = new List<RateLocator>
                {
                    new RateLocator
                    {
                        Label =
                            "Abonnement à des services de banque à distance (internet - hors du coût du fournisseur d'accès internet)",
                        RegexLocator = @"^.*?Abonnement à des services de banque à distance.*?\.*\s(.*?)$"
                    }
                }
            };

            var files = new DirectoryInfo(".");
            var pdfs = files.GetFiles(conf.FileName).ToList();
            pdfs.ForEach(pdf =>
             {
                 var content = string.Empty;
                 var pdfReader = new PdfReader(pdf.FullName);
                 var doc = new PdfDocument(pdfReader);
                 var pages = new List<string>();

                 for (var page = 1; page <= doc.GetNumberOfPages(); page++)
                 {
                     var strategy = new SimpleTextExtractionStrategy();
                     var currentPageText = PdfTextExtractor.GetTextFromPage(doc.GetPage(page), strategy);
                     content += currentPageText;
                     pages.Add(content);
                 }
                 conf.Rates.ForEach(locator =>
                 {
                     Console.WriteLine(locator.Label + " = " + locator.GetValue(content));
                 });
                 var output = pdf.Name + ".txt";
                 File.Delete(output);
                 using (var writer = new StreamWriter(File.OpenWrite(output)))
                 {
                     pages.ForEach(page => writer.WriteLine(page));
                 }
             });
            Console.ReadLine();

        }
    }
}
