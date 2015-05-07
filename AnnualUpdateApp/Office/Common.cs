using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pics.IGR.Office
{
    class Common
    {
        public static void releaseCOMObject(object obj)
        {
            try
            {
                if (System.Runtime.InteropServices.Marshal.IsComObject(obj))
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                }
            }
            catch { }
            finally
            {
                obj = null;
            }
        }
    }
}
