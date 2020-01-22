using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace pdfmergeC
{
    internal class Program
    {
        private static string dataDir = @"C:\PdfMergeData\";

        private static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            using (TextReader reader = new System.IO.StreamReader(dataDir + "Divisions.csv", System.Text.Encoding.UTF8))
            {
                string line = reader.ReadLine(); // skip header
                while ((line = reader.ReadLine()) != null)
                {
                    var lineContent = line.Split(';');
                    string divisionName = lineContent[0];
                    string personName = lineContent[1];
                    if (divisionName == "none")
                    {
                        File.Copy(dataDir + "general.pdf", dataDir + $"Output\\{personName}.pdf", true);
                    }
                    else
                    {
                        MergeAndName(divisionName, personName);
                    }
                }
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        static private void MergeAndName(string divisionName, string personName)
        {
            if (!divisionName.Contains("+"))
            {
                divisionName += "+";
            }

            string[] divisions = divisionName.Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (string division in divisions)
                {
                    using (PdfDocument divisionFile = PdfReader.Open(dataDir + $"{division}.pdf", PdfDocumentOpenMode.Import))
                    {
                        CopyPages(divisionFile, outPdf);
                    }
                }

                using (PdfDocument generalFile = PdfReader.Open(dataDir + "general.pdf", PdfDocumentOpenMode.Import))
                {
                    CopyPages(generalFile, outPdf);
                }

                Console.WriteLine($"Saving {personName}.pdf");
                outPdf.Save(dataDir + $"Output\\{personName}.pdf");
            }
        }

        static private void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}