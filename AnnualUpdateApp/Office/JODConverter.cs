using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Pics.IGR.Office
{
    public class JODConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public String ConvertOfficeToPDFUsingService(String strInputFilePath, String strOutputDirectoryPath)
        {
            if (!Directory.Exists(strOutputDirectoryPath))
                Directory.CreateDirectory(strOutputDirectoryPath);
            System.IO.FileInfo input = new FileInfo(strInputFilePath);
            String contentType = "";
            if (input.Extension.ToLower().Equals(".doc"))
            {
                contentType = "application/msword";
            }
            else if (input.Extension.ToLower().Equals(".docx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (input.Extension.ToLower().Equals(".xls"))
            {
                contentType = "application/vnd.ms-excel";
            }
            else if (input.Extension.ToLower().Equals(".xlsx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (input.Extension.ToLower().Equals(".ppt"))
            {
                contentType = "application/vnd.ms-powerpoint";
            }
            else if (input.Extension.ToLower().Equals(".pptx"))
            {
                contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            }

            string outputFile = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".pdf");
            while (File.Exists(outputFile))
            {
                outputFile = outputFile.Replace(".pdf", string.Format("{1}{0}", ".pdf", DateTime.Now.Ticks));
            }

            string urlJODConverterService = ConfigurationManager.AppSettings["JODConverterService"];
            WebClient webClient = new WebClient();
            webClient.Headers.Set("Content-Type", contentType);
            webClient.Headers.Set("Accept", "application/pdf");
            FileStream inputStream = File.OpenRead(strInputFilePath);
            BinaryReader reader = new BinaryReader(inputStream);
            byte[] inputData = reader.ReadBytes((int)inputStream.Length);
            reader.Close();

            byte[] outputData = webClient.UploadData(urlJODConverterService, "POST", inputData);

            FileStream outputStream = File.Create(outputFile);
            BinaryWriter writer = new BinaryWriter(outputStream);
            writer.Write(outputData);
            writer.Close();

            return outputFile;
        }

        public String ConvertOfficeToPDFUsingLib(String strInputFilePath, String strOutputDirectoryPath)
        {
            if (!Directory.Exists(strOutputDirectoryPath))
                Directory.CreateDirectory(strOutputDirectoryPath);
            System.IO.FileInfo input = new FileInfo(strInputFilePath);

            //Copy remote file to local
            string localFile = string.Format("{0}\\{1}", strOutputDirectoryPath, input.Name);
            while (File.Exists(localFile))
            {
                localFile = localFile.Replace(input.Extension, string.Format("{1}{0}", input.Extension, DateTime.Now.Ticks));
            }
            File.Copy(strInputFilePath, localFile, true);

            string outputFile = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".pdf");
            while (File.Exists(outputFile))
            {
                outputFile = outputFile.Replace(".pdf", string.Format("{1}{0}", ".pdf", DateTime.Now.Ticks));
            }

            com.jod.converter.Converter converter = new com.jod.converter.Converter();
            logger.Debug("ConvertToPDF localFile:" + localFile);
            converter.ConvertToPDF(localFile, outputFile);

            try
            {
                File.Delete(localFile);
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Temp File:" + ex.ToString());
            }
            return outputFile;
        }

        //public String ConvertOfficeToTIFF(String strInputFilePath, String strOutputDirectoryPath)
        //{
        //    String tempPdfPath = ConvertOfficeToPDFUsingLib(strInputFilePath, strOutputDirectoryPath);
        //    ConvertImageToTif converter = new ConvertImageToTif();
        //    String tiffPath = converter.Convert(tempPdfPath, strOutputDirectoryPath);

        //    try
        //    {
        //        File.Delete(tempPdfPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("Delete Temp File:" + ex.ToString());
        //    }

        //    return tiffPath;
        //}
    }
}
