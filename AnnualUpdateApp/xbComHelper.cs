using IMIP.UniversalScan.Connector.xBoundConnectorData.Data;
using IMIP.UniversalScan.Data;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Pics.IGR.Service
{
    class xbComHelper
    {
        public static string strBindingType = "Native";
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string[] GetServers()
        {
            string strServerURI0 = ConfigurationSettings.AppSettings["XboundServerURI0"];
            string strServerURI1 = ConfigurationSettings.AppSettings["XboundServerURI1"];
            string strServerURI2 = ConfigurationSettings.AppSettings["XboundServerURI2"];
            string strServerURI3 = ConfigurationSettings.AppSettings["XboundServerURI3"];

            return new string[] { strServerURI0, strServerURI1, strServerURI2, strServerURI3 };
        }

        public static string[] GetServers(string username, string password, string domainname)
        {
            if (String.IsNullOrEmpty(username)) return GetServers();

            string strxBoundServer0 = "";
            string strxBoundServer1 = "";
            string strxBoundServer2 = "";
            string strxBoundServer3 = "";
            string strServerURI0 = ConfigurationSettings.AppSettings["XboundServerURI0"];
            if (strServerURI0 != null && strServerURI0.Length > 6)
            {
                strxBoundServer0 = "tcp://" + domainname + "." + username + ":" + password + "@" + strServerURI0.Substring(6);
            }

            string strServerURI1 = ConfigurationSettings.AppSettings["XboundServerURI1"];
            if (strServerURI1 != null && strServerURI1.Length > 6)
            {
                strxBoundServer1 = "tcp://" + domainname + "." + username + ":" + password + "@" + strServerURI1.Substring(6);
            }

            string strServerURI2 = ConfigurationSettings.AppSettings["XboundServerURI2"];
            if (strServerURI2 != null && strServerURI2.Length > 6)
            {
                strxBoundServer2 = "tcp://" + domainname + "." + username + ":" + password + "@" + strServerURI2.Substring(6);
            }

            string strServerURI3 = ConfigurationSettings.AppSettings["XboundServerURI3"];
            if (strServerURI3 != null && strServerURI3.Length > 6)
            {
                strxBoundServer3 = "tcp://" + domainname + "." + username + ":" + password + "@" + strServerURI3.Substring(6);
            }

            //strxBoundServer0 = "tcp://" + domainname + "." + username + ":" + password + "@" + "xb371x64:4444";
            logger.Debug("strxBoundServer0 = " + strxBoundServer0);

            return new string[] { strxBoundServer0, strxBoundServer1, strxBoundServer2, strxBoundServer3 };
        }

        public static Xbound.Connector.Session LoginToxBound(String strUserName, String strPassword, String strDomainName)
        {
            logger.Debug("Login to xbound ");
            Xbound.Connector.XboundBase mXboundBase = new Xbound.Connector.XboundBase();
            return mXboundBase.Login(GetServers(strUserName, strPassword, strDomainName), "Native", "GetxBoundMetadata", true);
        }

        public static Xbound.Connector.Session LoginToxBound()
        {
            logger.Debug("Login to xbound ");
            Xbound.Connector.XboundBase mXboundBase = new Xbound.Connector.XboundBase();
            return mXboundBase.Login(GetServers(), "Native", "GetxBoundMetadata", true);
        }

        public static xBoundBatch CreateOSHADoc(String token, String callback_url, String fullLocalFilePath)
        {
            Hashtable batchTable = new Hashtable();
            Hashtable docTable = new Hashtable();
            ArrayList docIDs = new ArrayList();

            xBoundBatch oBatch = new xBoundBatch();
            oBatch.ID = System.Guid.NewGuid().ToString();
            oBatch.Priority = 1;
            oBatch.Name = "OSHA" + "_" + DateTime.Now.ToString("MMddyyyy_hh_mm") + "_" + System.Guid.NewGuid().ToString();
            oBatch.xBoundClientName = "PICS";
            oBatch.xBoundProcessName = "Annual";
            oBatch.FormTypeName = "OSHA";
            oBatch.xBoundProcessStepName = "Universal Scan";
            oBatch.ScanUser = "";
            oBatch.ScanStation = "";

            try
            {
                oBatch.xBoundClientName = ConfigurationSettings.AppSettings["OSHAClientName"];
                oBatch.xBoundProcessName = ConfigurationSettings.AppSettings["OSHAProcessName"];
                oBatch.FormTypeName = ConfigurationSettings.AppSettings["OSHAFormTypeName"];
                oBatch.xBoundProcessStepName = ConfigurationSettings.AppSettings["OSHAProcessStepName"];
            }
            catch (Exception oEx)
            {
                logger.Debug("cannot get property value in config file." +oEx.ToString());
            }

            UniField oDocTokenField = new UniField();
            oDocTokenField.Name = "token";
            oDocTokenField.Value = token.ToString();

            UniField oDocCallbackField = new UniField();
            oDocCallbackField.Name = "callback_url";
            oDocCallbackField.Value = callback_url.ToString();

            oBatch.Fields.Add(oDocTokenField);
            oBatch.Fields.Add(oDocCallbackField);

            oBatch.Pages = new List<UniPage>();
            oBatch.Media = new List<UniMedium>();
            UniPage oPage = new UniPage();
            oPage.FullFileName = fullLocalFilePath;
            oBatch.Pages.Add(oPage);

            return oBatch;
        }

        public static xBoundBatch CreateOSHADoc(String token, String callback_url, List<string> fullListLocalFilePath)
        {
            Hashtable batchTable = new Hashtable();
            Hashtable docTable = new Hashtable();
            ArrayList docIDs = new ArrayList();

            xBoundBatch oBatch = new xBoundBatch();
            oBatch.ID = System.Guid.NewGuid().ToString();
            oBatch.Priority = 1;
            oBatch.Name = "OSHA" + "_" + DateTime.Now.ToString("MMddyyyy_hh_mm") + "_" + System.Guid.NewGuid().ToString();
            oBatch.xBoundClientName = "PICS";
            oBatch.xBoundProcessName = "Annual";
            oBatch.FormTypeName = "OSHA";
            oBatch.xBoundProcessStepName = "Universal Scan";
            oBatch.ScanUser = "";
            oBatch.ScanStation = "";

            try
            {
                oBatch.xBoundClientName = ConfigurationSettings.AppSettings["OSHAClientName"];
                oBatch.xBoundProcessName = ConfigurationSettings.AppSettings["OSHAProcessName"];
                oBatch.FormTypeName = ConfigurationSettings.AppSettings["OSHAFormTypeName"];
                oBatch.xBoundProcessStepName = ConfigurationSettings.AppSettings["OSHAProcessStepName"];
            }
            catch (Exception oEx)
            {
                logger.Debug("cannot get property value in config file." + oEx.ToString());
            }

            UniField oDocTokenField = new UniField();
            oDocTokenField.Name = "token";
            oDocTokenField.Value = token.ToString();

            UniField oDocCallbackField = new UniField();
            oDocCallbackField.Name = "callback_url";
            oDocCallbackField.Value = callback_url.ToString();

            oBatch.Fields.Add(oDocTokenField);
            oBatch.Fields.Add(oDocCallbackField);

            oBatch.Pages = new List<UniPage>();
            oBatch.Media = new List<UniMedium>();
            foreach (string file in fullListLocalFilePath)
            {
                UniPage oPage = new UniPage();
                oPage.FullFileName = file;
                oBatch.Pages.Add(oPage);
            }

            return oBatch;
        }
    }
}
