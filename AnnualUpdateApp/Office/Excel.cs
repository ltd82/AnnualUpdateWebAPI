using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pics.IGR.Office
{
    public class Excel
    {
        public String Convert(String strInputFilePath, String strOutputDirectoryPath)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            xlApp.Visible = false;
            Workbook wb = xlApp.Workbooks.Open(strInputFilePath,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);

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

            wb.PrintOutEx(missing, missing, copies, oFalse, "Fax", oTrue, missing, outputFile, missing);
            //Worksheet ws = (Worksheet)wb.Worksheets[1];
            wb.Close(oFalse, oFalse, oFalse);
            xlApp.Quit();
            return output;
        }
    }
}
