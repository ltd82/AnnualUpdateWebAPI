using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pics.IGR.Office
{
    public class Word
    {
        public String Convert(String strInputFilePath, String strOutputDirectoryPath)
        {
            Document doc = new Document();
            Application app = new Application();

            doc = app.Documents.Open(strInputFilePath);

            app.ActivePrinter = "Fax";
            app.Visible = false;
            object copies = "1";
            object pages = "";
            object range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
            object items = Microsoft.Office.Interop.Word.WdPrintOutItem.wdPrintDocumentContent;
            object pageType = Microsoft.Office.Interop.Word.WdPrintOutPages.wdPrintAllPages;
            object oTrue = true;
            object oFalse = false;
            object missing = Type.Missing;
            if (!Directory.Exists(strOutputDirectoryPath))
                Directory.CreateDirectory(strOutputDirectoryPath);
            System.IO.FileInfo input = new FileInfo(strInputFilePath);
            string output = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".tif");
            //If the output file exist alrady be sure to add a random name at the end until is unique!
            while (File.Exists(output))
            {
                output = output.Replace(".tif", string.Format("{1}{0}", ".tif", DateTime.Now.Ticks));
            }
            object outputFile = output;

            doc.PrintOut(ref oTrue, ref oFalse, ref range, ref outputFile, ref missing, ref missing,
                ref items, ref copies, ref pages, ref pageType, ref oTrue, ref oTrue,
                ref missing, ref oFalse, ref missing, ref missing, ref missing, ref missing);
            doc.Close();
            app.Quit();
            return output;
        }
    }
}
