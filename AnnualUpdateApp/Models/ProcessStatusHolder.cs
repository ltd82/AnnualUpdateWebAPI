using Pics.IGR.Controllers;
using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace Pics.IGR.Models
{
    public class ProcessStatusHolder
    {
        private static ProcessStatusHolder instance = null;
        private Hashtable states = new Hashtable();
        private Hashtable values = new Hashtable();

        public static ProcessStatusHolder getInstance()
        {
            if (instance == null)
            {
                instance = new ProcessStatusHolder();
                instance.states = new Hashtable();
            }

            return instance;
        }
       public String getStatusStates(String token){
           return (String)states[token];
       }

       public OSHAResponse getValue(String token)
       {
           return (OSHAResponse)values[token];
       }

       public void setStatusStates(String token, String stat)
       {
           if (states.ContainsKey(token))
           {
               states[token] = stat;
           }
           else {
               states.Add(token, stat);
           }
       }

       public void setValue(String token, OSHAResponse val)
       {
           if (values.ContainsKey(token))
           {
               values[token] = val;
           }
           else
           {
               values.Add(token, val);
           }
       }

        public void remote(String token) {
            try
            {
                states.Remove(token);
                values.Remove(token);
            }
            catch { }
        }
    }
}