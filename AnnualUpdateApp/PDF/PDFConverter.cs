using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Converter.PDF
{
    public class PDFConverter
    {
        PDFCommon converter = new PDFCommon();
        public String Convert(string strInputFilePath, string strOutputDirectoryPath)
        {
            if (!System.IO.File.Exists(Application.StartupPath + "\\gsdll32.dll"))
            {
                return null;
            }

            converter.RenderingThreads = -1;
            converter.TextAlphaBit = -1;
            converter.OutputToMultipleFile = false;
            converter.FirstPageToConvert = -1;
            converter.LastPageToConvert = -1;
            converter.FitPage = false;
            converter.OutputFormat = "tifflzw";

            if (!Directory.Exists(strOutputDirectoryPath))
                Directory.CreateDirectory(strOutputDirectoryPath);
            System.IO.FileInfo input = new FileInfo(strInputFilePath);
            string output = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".tif");
            //If the output file exist alrady be sure to add a random name at the end until is unique!
            while (File.Exists(output))
            {
                output = output.Replace(".tif", string.Format("{1}{0}", ".tif", DateTime.Now.Ticks));
            }

            bool r = converter.Convert(input.FullName, output);
            if (r)
                return output;
            else
                return null;
        }

    }
}
