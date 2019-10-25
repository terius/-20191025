using System;
using System.Collections.Generic;
using System.Text;

namespace TS.Common
{
    public class TimeHelper
    {
        private static object ob = new object();
        public static bool CheckDataIsNewer(string oldTime,string newTime)
        {
            lock (ob)
            {
                DateTime dtTempOld = DateTime.MinValue;
                if (DateTime.TryParse(oldTime, out dtTempOld))
                {
                    DateTime dtTempNew = DateTime.MinValue;
                    if (DateTime.TryParse(newTime, out dtTempNew))
                    {
                        return dtTempNew > dtTempOld;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

        }
    }
}
