using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace TS.Common.MyConfiguration
{
    public class ConnSectionGroup : System.Configuration.ConfigurationSectionGroup
    {
        public ConnsSection ConnStringList
        {
            get
            {
                return (ConnsSection)base.Sections["conns"];
            }
        }

    


    }
}
