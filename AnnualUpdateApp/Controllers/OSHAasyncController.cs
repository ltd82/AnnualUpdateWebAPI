
using IMIP.UniversalScan.Connector.xBoundConnectorData.Data;
using IMIP.UniversalScan.Connector.xBoundConnectorShared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using PIC.IGR;
using Pics.IGR.Models;
using Pics.IGR.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;

namespace Pics.IGR.Controllers
{
    public class OSHAasyncController : ApiController
    {


       private static Logger logger = LogManager.GetCurrentClassLogger();
       private String token = "";
       private OSHA osha = new OSHA();
       private static JObject ob = new JObject();
        // POST api/<controller>
       public void Post([FromBody]JObject value)
        {
            try
            {
                ob = value;
                Thread t = new Thread(new ThreadStart(initialImporting));
                t.Start();
            }
            catch (Exception oEx)
            {
                logger.Debug(oEx.ToString());
            }
        }


       private void initialImporting()
       {
           try
           {
               logger.Debug("Call  OSHAasyncController");
               String strCallbackURL = ConfigurationManager.AppSettings["WebServerURL"] + "api/Callback";
               String strDomain = ConfigurationManager.AppSettings["xBoundDomainName"];
               String strUserName = ConfigurationManager.AppSettings["xBoundUserName"];
               String strPassword = ConfigurationManager.AppSettings["xBoundPassword"];
               String strImgLegacyUserName = ConfigurationManager.AppSettings["ImgLegacyUserName"];
               String strImgLegacyPassword = ConfigurationManager.AppSettings["ImgLegacyPassword"];

               String strLegacyImagesFolderPath = ConfigurationSettings.AppSettings["LegacyImagesFolderPathB"];

               String strLocalImagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

               String fileName = ob.GetValue("fileName").ToString();
               String extension = Path.GetExtension(fileName);
               token = fileName + "_" + System.Guid.NewGuid().ToString();
               fileName = Path.GetFileNameWithoutExtension(fileName);
               String fullLegacyFilePath = GetLegacyDocFilePath(strLegacyImagesFolderPath, fileName, extension);
               List<string> fullLocalFilePath = null;

               logger.Debug("strLocalImagesFolderPath = " + strLocalImagesFolderPath);
               if (!Directory.Exists(strLocalImagesFolderPath))
               {
                   Directory.CreateDirectory(strLocalImagesFolderPath);
               }
               // get physical path of image on Pics server
               NetworkCredential writeCredentials = new NetworkCredential(strImgLegacyUserName, strImgLegacyPassword);
               using (NetworkConnection networkConnection = new NetworkConnection(strLegacyImagesFolderPath, writeCredentials))
               {
                   int result = networkConnection.GetConnectionResult();
                   if (result == 267)
                   {
                       logger.Debug("The directory name is invalid: " + fullLegacyFilePath);
                       throw new ApplicationException("The directory name is invalid: " + fullLegacyFilePath);
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
                   logger.Debug("networkConnection:" + networkConnection.ToString());
                   if (ConfigurationSettings.AppSettings["extSepcial"].ToString().Contains(extension))
                   {
                       if (!File.Exists(fullLegacyFilePath))
                       {
                           logger.Debug("The file: " + fullLegacyFilePath + " is not found.");
                           throw new Exception("The file: " + fullLegacyFilePath + " is not found.");

                       }
                       fullLocalFilePath = new List<string> {fullLegacyFilePath};
                   }
                   else if (ConfigurationSettings.AppSettings["Allow_ext"].ToString().Contains(extension))
                   {


                       if (!File.Exists(fullLegacyFilePath))
                       {
                           logger.Debug("The file: " + fullLegacyFilePath + " is not found.");
                           throw new Exception("The file: " + fullLegacyFilePath + " is not found.");

                       }

                       // check if image file is existing
                       if (File.Exists(fullLegacyFilePath))
                       {
                           // convert image to tiff and save to a share folder 

                           fullLocalFilePath = ConvertDocToTiffFile(strLocalImagesFolderPath, fullLegacyFilePath);
                           foreach (string file in fullLocalFilePath) 
                           {
                               if (!File.Exists(file))
                               {
                                   throw new Exception("Cannot convert the file " + strLocalImagesFolderPath);
                               }
                               logger.Debug("fullLocalFilePath: " + file);
                           }
                       }
                   }
                   else
                   {
                       throw new Exception("Do not support convert: " + extension);
                   }
                   logger.Debug("Current user:" + WindowsIdentity.GetCurrent().Name);
                   if (!fullLocalFilePath.Equals(null))
                   {
                       // import doc to xBound
                       try
                       {
                           String xBoundDocName = importDocToXBound(token, strCallbackURL, fullLocalFilePath);
                           //check Holder
                           ProcessStatusHolder oProcessStatusHolder = ProcessStatusHolder.getInstance();
                           oProcessStatusHolder.setStatusStates(token, "Pending");

                           Thread t = new Thread(new ThreadStart(run));
                           t.Start();
                       }
                       catch (Exception exImportDocToXBound)
                       {
                           logger.Error("Unexpected exception occured when importDocToXBound " + exImportDocToXBound.ToString());
                           throw new Exception("Unexpected exception occured when importDocToXBound");
                       }
                   }
               }
           }
           catch (Exception oEx)
           {
               logger.Debug(oEx.ToString());
           }
       }
        private void run()
        {
            int i=0;
            ProcessStatusHolder oProcessStatusHolder = ProcessStatusHolder.getInstance();
            String status = oProcessStatusHolder.getStatusStates(token);
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutOptB"]);
            while (i < timeout)//webconfig
            {
                status = oProcessStatusHolder.getStatusStates(token);
                logger.Debug("thread check holder ");
              
                if (status.Equals("Ready"))
                {
                    OSHAResponse oshaResponse = oProcessStatusHolder.getValue(token);
                    osha = oshaResponse.IGRForm;
                    try
                    {
                        Pooling.UpdateDocument_OSHA(ob, osha);
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("Update db pics fail! " + ex.ToString());
                        throw new Exception("Update db pics fail! "+ex.ToString());
                    }
                    logger.Debug("Update db pics success!");
                    Thread.CurrentThread.Abort();
                }
                else if (status.Equals("Error"))
                {
                    logger.Debug("Xbound Read file Error!");
                    throw new Exception("Xbound Read file Error!");
                }
                i = i + 1000;
                Thread.Sleep(1000);
            }
            if (status.Equals("Pending"))
            {
                logger.Debug("Request time out!");
                Thread.CurrentThread.Abort();
                throw new Exception("Request time out!");
            }
        }
        private static string GetLegacyDocFilePath(String strLegacyImagesFolderPath, String certID, String fileType)
        {
            String cert = certID.Substring(ConfigurationSettings.AppSettings["PO_Prefix_Filename"].ToString().Length);
            String folderNameL1 = cert.Substring(0, 3);
            String folderNameL2 = cert.Substring(3, 3);
            String fileName = certID + fileType;
            fileName = Path.Combine(folderNameL1, folderNameL2, fileName);
            String fullLegacyFilePath = Path.Combine(strLegacyImagesFolderPath, fileName);

            logger.Debug("Legacy Document File Path: " + fullLegacyFilePath);
            return fullLegacyFilePath;
        }
        private static String importDocToXBound(String token, String callback_url, List<string> fullLocalFilePath)
        {
            String strDomain = ConfigurationManager.AppSettings["xBoundDomainName"];
            String strUserName = ConfigurationManager.AppSettings["xBoundUserName"];
            String strPassword = ConfigurationManager.AppSettings["xBoundPassword"];

            xBoundBatch oDoc = xbComHelper.CreateOSHADoc(token, callback_url, fullLocalFilePath);
            xBoundBatchProcess oImporter = new xBoundBatchProcess();
            logger.Debug("Start create doc " + oDoc.Name);
            if (String.IsNullOrEmpty(strDomain) || String.IsNullOrEmpty(strUserName))
            {
                oImporter.InitializeSession(xbComHelper.GetServers(), true, "Native");
            }
            else
            {
                oImporter.InitializeSession(xbComHelper.GetServers(strUserName, strPassword, strDomain), true, "Native");
            }

            oImporter.CreateXboundBatch(oDoc);
            logger.Debug("Finish create doc " + oDoc.Name);
            return oDoc.Name;
        }
        private List<string> ConvertDocToTiffFile(string strLocalImagesFolderPath, string fullLegacyFilePath)
        {
            ConvertImageToTif convert = new ConvertImageToTif();
            List<string> fullLocalFilePath = convert.Convert(fullLegacyFilePath, strLocalImagesFolderPath);
            return fullLocalFilePath;
            // return @"C:\pdfs\Certs_251498.PDF.tif";
        }
    }
}