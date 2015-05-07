using GdPicture10;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pics.IGR.GDPicture
{
    public class GDPictureConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void SetLicense()
        {
            GdPicture10.LicenseManager LM = new GdPicture10.LicenseManager();
            LM.RegisterKEY("4118535557962326260401742");
            LM.RegisterKEY("9122399844896947875252354");
            LM.RegisterKEY("912148297967098731211153528952082");
            LM.RegisterKEY("902103669503678561115111230231234");
        }
        //public static String ConvertPdfToTiff(String strInputFilePath, String strOutputDirectoryPath)
        //{
        //    if (!Directory.Exists(strOutputDirectoryPath))
        //        Directory.CreateDirectory(strOutputDirectoryPath);
        //    System.IO.FileInfo input = new FileInfo(strInputFilePath);
        //    string outputFile = string.Format("{0}\\{1}{2}", strOutputDirectoryPath, input.Name, ".tif");
        //    while (File.Exists(outputFile))
        //    {
        //        outputFile = outputFile.Replace(".tif", string.Format("{1}{0}", ".tif", DateTime.Now.Ticks));
        //    }

        //    GdPicturePDF oGdPicturePDF = new GdPicturePDF();
        //    GdPictureImaging oGdPictureImaging = new GdPictureImaging();
        //    int DPI = 300;
        //    int MultiTiffID = 0;
        //    TiffCompression CompressionScheme = 0;
        //    GdPictureStatus Status = GdPictureStatus.OK;
        //    CompressionScheme = TiffCompression.TiffCompressionCCITT4;
        //    if (oGdPicturePDF.LoadFromFile(strInputFilePath, false) == GdPictureStatus.OK)
        //    {
        //        for (int i = 1; i <= oGdPicturePDF.GetPageCount(); i++)
        //        {
        //            if (Status == GdPictureStatus.OK)
        //            {
        //                oGdPicturePDF.SelectPage(i);
        //                int RasterizedPageID = oGdPicturePDF.RenderPageToGdPictureImageEx(DPI, true);
        //                if (RasterizedPageID == 0)
        //                {
        //                    logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + oGdPicturePDF.GetStat().ToString());
        //                    return null;
        //                }
        //                // CCITT3 & CCITT4 compression support only bitonal (1 bpp) images !!
        //                if (CompressionScheme == TiffCompression.TiffCompressionCCITT3 | CompressionScheme == TiffCompression.TiffCompressionCCITT4)
        //                {
        //                    oGdPictureImaging.ConvertTo1Bpp(RasterizedPageID);
        //                }

        //                if (i == 1)
        //                {
        //                    MultiTiffID = RasterizedPageID; // Warning: this image must be released at the end!!!!
        //                    Status = oGdPictureImaging.TiffSaveAsMultiPageFile(MultiTiffID, outputFile, CompressionScheme);
        //                }
        //                else
        //                {
        //                    Status = oGdPictureImaging.TiffAddToMultiPageFile(MultiTiffID, RasterizedPageID);
        //                    oGdPictureImaging.ReleaseGdPictureImage(RasterizedPageID);
        //                }
        //            }
        //        }

        //        if (oGdPictureImaging.TiffCloseMultiPageFile(MultiTiffID) != GdPictureStatus.OK)
        //        {
        //            logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + oGdPictureImaging.GetStat().ToString());
        //        }
        //        oGdPictureImaging.ReleaseGdPictureImage(MultiTiffID);
        //        oGdPicturePDF.CloseDocument();
        //        if (Status == GdPictureStatus.OK)
        //        {
        //            return outputFile;
        //        }
        //        else
        //        {
        //            logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + Status.ToString());
        //            return null;
        //        }

        //    }
        //    else
        //    {
        //        logger.Debug("Can't open file: " + strInputFilePath);
        //        return null;
        //    }


        //}

        public static List<string> ConvertPdfToTiff(String strInputFilePath, String strOutputDirectoryPath)
        {
            if (!Directory.Exists(strOutputDirectoryPath))
                Directory.CreateDirectory(strOutputDirectoryPath);
            System.IO.FileInfo input = new FileInfo(strInputFilePath);
            string outputFile = String.Empty;
            List<string> fileList = new List<string>();

            GdPicturePDF oGdPicturePDF = new GdPicturePDF();
            GdPictureImaging oGdPictureImaging = new GdPictureImaging();
            int DPI = 300;
            int SingleTiffID = 0;
            TiffCompression CompressionScheme = 0;
            GdPictureStatus Status = GdPictureStatus.OK;
            CompressionScheme = TiffCompression.TiffCompressionCCITT4;
            if (oGdPicturePDF.LoadFromFile(strInputFilePath, false) == GdPictureStatus.OK)
            {
                for (int i = 1; i <= oGdPicturePDF.GetPageCount(); i++)
                {
                    if (Status == GdPictureStatus.OK)
                    {
                        outputFile = string.Format("{0}\\{1}_{2}{3}", strOutputDirectoryPath, input.Name, i, ".tif");
                        oGdPicturePDF.SelectPage(i);
                        int RasterizedPageID = oGdPicturePDF.RenderPageToGdPictureImageEx(DPI, true);
                        if (RasterizedPageID == 0)
                        {
                            logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + oGdPicturePDF.GetStat().ToString());
                            return null;
                        }
                        // CCITT3 & CCITT4 compression support only bitonal (1 bpp) images !!
                        if (CompressionScheme == TiffCompression.TiffCompressionCCITT3 | CompressionScheme == TiffCompression.TiffCompressionCCITT4)
                        {
                            oGdPictureImaging.ConvertTo1Bpp(RasterizedPageID);
                        }

                        SingleTiffID = RasterizedPageID; // Warning: this image must be released at the end!!!!
                        Status = oGdPictureImaging.SaveAsTIFF(SingleTiffID, outputFile, CompressionScheme);
                        oGdPictureImaging.ReleaseGdPictureImage(RasterizedPageID);
                        oGdPictureImaging.ReleaseGdPictureImage(SingleTiffID);
                        if (Status != GdPictureStatus.OK)
                        {
                            logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + Status.ToString());
                            break;
                        }
                        else
                        {
                            fileList.Add(outputFile);
                        }
                    }
                }

                oGdPicturePDF.CloseDocument();
                if (Status == GdPictureStatus.OK)
                {
                    return fileList;
                }
                else
                {
                    logger.Debug("Converter.GDPicture.ConvertPdfToTiff Error:" + Status.ToString());
                    return null;
                }

            }
            else
            {
                logger.Debug("Can't open file: " + strInputFilePath);
                return null;
            }


        }
    }
}