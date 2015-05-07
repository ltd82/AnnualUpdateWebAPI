using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pics.IGR.Office
{
    public class PowerPoint
    {
        public String Convert(String strInputFilePath, String strOutputDirectoryPath)
        {
            Microsoft.Office.Interop.PowerPoint.Application xlApp = new Microsoft.Office.Interop.PowerPoint.Application();

            Presentation presentation = xlApp.Presentations.Open(strInputFilePath,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoFalse
                , Microsoft.Office.Core.MsoTriState.msoFalse);

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
            string outputFile = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".tif");
            //If the output file exist alrady be sure to add a random name at the end until is unique!
            while (File.Exists(outputFile))
            {
                outputFile = outputFile.Replace(".tif", string.Format("{1}{0}", ".tif", DateTime.Now.Ticks));
            }

            presentation.PrintOptions.PrintInBackground = Microsoft.Office.Core.MsoTriState.msoTrue;
            presentation.PrintOut(1, presentation.Slides.Count, outputFile, 1, Microsoft.Office.Core.MsoTriState.msoFalse);
            //Worksheet ws = (Worksheet)wb.Worksheets[1];
            presentation.Close();
            xlApp.Quit();
            return outputFile;
        }
    }
}
