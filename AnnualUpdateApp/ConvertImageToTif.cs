
using Pics.IGR.Office;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Pics.IGR
{
    public class ConvertImageToTif
    {
          private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<string> Convert(String strInputFilePath, String strOutputDirectoryPath)
          {
              //NetworkCredential writeCredentials = new NetworkCredential(ConvertImageToTif.value("LegacyImagesServerUsername"), ConvertImageToTif.value("LegacyImagesServerPassword"));
              //using (NetworkConnection networkConnection = new NetworkConnection(ConvertImageToTif.value("LegacyImagesServer"), writeCredentials))
              {
                  /*int result = networkConnection.GetConnectionResult();
                  if (result == 267)
                  {
                      logger.Debug("The directory name is invalid: " + strInputFilePath);
                      throw new ApplicationException("The directory name is invalid: " + strInputFilePath);
                  }
                  else if (result == 86)
                  {
                      logger.Debug("The specified username/password is invalid.");
                      throw new ApplicationException("The specified username/password is invalid.");
                  }
                  else if (result != 0 && result != 1219)//result=1219 means existing connection.
                  {
                      logger.Debug("Cannot log in the legacy remote server");
                      throw new ApplicationException("Cannot log in the legacy remote server");
                  }
                  */
                  System.IO.FileInfo input = new FileInfo(strInputFilePath);
                  if (input.Extension.ToLower().Equals(".pdf"))
                  {
                      return GDPicture.GDPictureConverter.ConvertPdfToTiff(strInputFilePath, strOutputDirectoryPath);
                  }
                  else if (input.Extension.ToLower().Equals(".xls") || input.Extension.ToLower().Equals(".xlsx"))
                  {
                      logger.Debug("strInputFilePath:" + strInputFilePath);
                      JODConverter converter = new JODConverter();
                      String strPdfInputFilePath = converter.ConvertOfficeToPDFUsingLib(strInputFilePath, strOutputDirectoryPath);
                      List<string> outputFile = GDPicture.GDPictureConverter.ConvertPdfToTiff(strPdfInputFilePath, strOutputDirectoryPath);
                      try
                      {
                          File.Delete(strPdfInputFilePath);
                      }
                      catch (Exception ex)
                      {
                          logger.Debug("Delete Temp File:" + ex.ToString());
                      }
                      return outputFile;
                  }

                  return null;
              }
          }

        public static string value(string key)
        {
            if ((ConfigurationManager.AppSettings[key] != null))
            {
                return ConfigurationManager.AppSettings[key];
            }
            return "";
        }
    }
    }
