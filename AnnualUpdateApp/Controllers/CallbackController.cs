using NLog;
using Pics.IGR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pics.IGR.Controllers
{
    public class CallbackController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // POST api/values
        public void Post(OSHAResponse value)
        {
            logger.Debug("Call post method with token = " + value.IGRForm.token);
            ProcessStatusHolder oProcessStatusHolder = ProcessStatusHolder.getInstance();

            oProcessStatusHolder.setValue(value.IGRForm.token, value);
            oProcessStatusHolder.setStatusStates(value.IGRForm.token, value.IsError ? "Error" : "Ready");
        }
    }
}
