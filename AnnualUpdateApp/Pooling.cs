using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Pics.IGR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace Pics.IGR
{
    public class Pooling
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static String strConnectionString_Legacy = ConfigurationManager.AppSettings["legacy_db"];
        public static String strConnectionString_IGR = ConfigurationManager.AppSettings["igr_db"];

        public static Boolean CheckDocsFromLegacy(MySqlConnection connection,
            String auditId, String questionId)
        {
            logger.Debug("Check questionId: " + questionId);
            String strSql = @"select 1 from pqfdata a where a.auditID = '" + auditId + "' and a.questionID = " + questionId + ";";
            DataTable dt = new DataTable();

            
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(strSql, connection))
            {
                dataAdapter.Fill(dt);
            }

            if (dt.Rows.Count > 0)
            {

                return true;
            }
            else
            {

                return false;
            }

        }

        public static void UpdateDocument_OSHA(JObject ob, OSHA osha) // Pending
        {
            // Validation "ob" parameter
            String fileName = ob.GetValue("fileName").ToString();
            String auditID = ob.GetValue("auditID").ToString();
            String incident = ob.GetValue("Incident").ToString();
            String tag = ob.GetValue("Tag").ToString();
            if (String.IsNullOrEmpty(fileName) || String.IsNullOrEmpty(auditID))
            {
                logger.Debug("Missing fileName or auditID");
                return;
            }
            int iTemp = 0;
            try
            {
                iTemp = Convert.ToInt32(auditID);
            }
            catch { }
            if (iTemp == 0)
            {
                logger.Debug("Invalid auditID");
                return;
            }
            ob.Property("fileName").Remove();
            ob.Property("auditID").Remove();
            ob.Property("Tag").Remove();
            if (String.IsNullOrEmpty(incident)) ob.Property("Incident").Remove();
            // End validation "ob" parameter

            String temp = ob.ToString();
            logger.Debug("Template of Questions: " + temp);
            logger.Debug("Update a new OSHA doc");
            logger.Debug("auditId: " + auditID);
            OSHA oshaTemp = JsonConvert.DeserializeObject<OSHA>(temp);
            PropertyInfo[] properties = osha.GetType().GetProperties();

            String strSql = "";
            Boolean haveIncident = false;
            Boolean updNAifNull = false;
            Boolean haveNA = false;

            using (MySqlConnection connect = new MySqlConnection(strConnectionString_Legacy))
            {
                try
                {
                    String pattern = @"[^0-9\.]";
                    Regex rgx = new Regex(pattern);
                    connect.Open();
                    for (int i = 1; i < properties.Length; i++)
                    {
                        strSql = "";
                        iTemp = 0;
                        updNAifNull = false;
                        String questionID = oshaTemp.getValues(i);
                        String answer = osha.getValues(i);
                        if (String.IsNullOrEmpty(answer))
                        {
                            if ("1" == tag)
                            {
                                updNAifNull = true;
                                haveNA = true;
                            }
                            else continue;
                        }
                        try
                        {
                            if (properties[i].Name != "NumberOfEmployees" && properties[i].Name != "NumOfHWorked" && properties[i].Name != "NAICSCode")
                            {
                                string result = rgx.Replace(answer, "");
                                iTemp = Convert.ToInt32(result);
                            }
                        }
                        catch { }
                        if (iTemp > 0) haveIncident = true;
                        logger.Debug("Question name: " + properties[i].Name);
                        if (CheckDocsFromLegacy(connect, auditID, questionID))
                        {
                            logger.Debug("questionID is exist");
                            if (updNAifNull)
                                strSql = @"Update pqfdata set answer='N/A' where auditID=" + auditID + " and questionID=" + questionID + ";";
                            else strSql = @"Update pqfdata set answer='" + answer + "' where auditID=" + auditID + " and questionID=" + questionID + ";";
                        }
                        else
                        {
                            logger.Debug("questionID is not exist");
                            if (updNAifNull)
                                strSql = @"insert into pqfdata(auditID, questionID, answer) values (" + auditID + "," + questionID + ",'N/A')";
                            else strSql = @"insert into pqfdata(auditID, questionID, answer) values (" + auditID + "," + questionID + ",'" + answer + "')";
                        }
                        if (String.IsNullOrEmpty(strSql)) continue;
                        using (MySqlCommand cmd = new MySqlCommand(strSql, connect))
                        {
                            logger.Debug("Start set value osha: auditId=" + auditID + ", questionId=" + questionID + ", answer=" + answer);
                            cmd.ExecuteNonQuery();
                            logger.Debug("Finish set value osha: auditId=" + auditID + ", questionId=" + questionID + ", answer=" + answer);
                        }

                    }

                    if (haveIncident || haveNA)
                    {
                        strSql = "";
                        logger.Debug("Updates incident: " + incident);
                        if (CheckDocsFromLegacy(connect, auditID, incident))
                        {
                            logger.Debug("questionID is exist");
                            strSql = @"Update pqfdata set answer='Yes' where auditID=" + auditID + " and questionID=" + incident + ";";
                        }
                        else
                        {
                            logger.Debug("questionID is not exist");
                            strSql = @"insert into pqfdata(auditID, questionID, answer) values (" + auditID + "," + incident + ",'Yes')";
                        }
                        using (MySqlCommand cmd = new MySqlCommand(strSql, connect))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception oEx)
                {
                    logger.Debug("Exception with strSql: " + strSql);
                    logger.Debug("Exception detail: " + oEx.ToString());
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public static int UpdateDocument_OSHA1(JObject ob, OSHA osha) // Pending
        {
            String temp = ConfigurationManager.AppSettings["TempObjectQuestions"];
            logger.Debug("update a new doc in OSHA: ");
            String auditID = ob.GetValue("auditID").ToString();
            logger.Debug("auditId: " +auditID);
            OSHA oshaTemp = JsonConvert.DeserializeObject<OSHA>(temp);
            PropertyInfo[] properties = osha.GetType().GetProperties();
            
            String strSql = "";
            String strSql1 = "";
            int newId = 0;

            using (MySqlConnection connect = new MySqlConnection(strConnectionString_Legacy))
            {
                try
                {
                    connect.Open();
                    for (int i = 1; i < 10; i++)
                    {
                        if (CheckDocsFromLegacy(connect, auditID, oshaTemp.getValues(i)))
                        {
                            logger.Debug("Ton tai: ");
                            strSql = @"Update pqfdata set answer='" + osha.getValues(i) + "'where auditID='" + auditID + "' and questionID='" + oshaTemp.getValues(i) + "';";
                            if (i == 3 && Convert.ToInt32(osha.getValues(3))>0)
                            {
                                strSql1 = @"Update pqfdata set answer='Yes' where auditID='" + auditID + "' and questionID='8838';";
                            }
                        }
                        else {
                            logger.Debug("ko Ton tai: ");
                            strSql = @"insert into pqfdata(auditID, questionID, answer) values (" + auditID + ", " + oshaTemp.getValues(i) + ",'" + osha.getValues(i) + "')";
                            if (i == 3 && Convert.ToInt32(osha.getValues(3)) > 0)
                            {
                                strSql1 = @"insert into pqfdata(auditID, questionID, answer) values (" + auditID+ ", '8838','Yes')";
                            }
                        }
                            
                        using (MySqlCommand cmd = new MySqlCommand(strSql, connect))
                        {
                            logger.Debug("Start Update Id = " + newId + " auditId = " + auditID + " questionId = " + oshaTemp.getValues(i) + " answer = " + osha.getValues(i));
                            cmd.ExecuteNonQuery();
                            newId = (int)cmd.LastInsertedId;
                            logger.Debug("Finish Update Id = " + newId + " auditId = " + auditID + " questionId = " + oshaTemp.getValues(i) + " answer = " + osha.getValues(i));

                        }
                       
                    }
                    //if (!strSql1.Equals(""))//check truong hop Fatalities>1
                    //{
                    //    using (MySqlCommand cmd = new MySqlCommand(strSql1, connect))
                    //    {
                    //        logger.Debug("check truong hop Fatalities>1");

                    //        cmd.ExecuteNonQuery();
                    //        newId = (int)cmd.LastInsertedId;
                    //        logger.Debug("Finish Update pqfdata set answer='Yes' where auditID='" + auditID + "' and questionID='8838'");

                    //    }
                    //}
                }
                catch (Exception oEx)
                {
                    logger.Debug("strSql = " + strSql);
                    logger.Debug("ex = " + oEx.ToString());
                }
                finally
                {
                    connect.Close();

                }

                return newId;
            }
        }
    }
}