using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace TS.Common.MyConfiguration
{
    public class ConnsSection : ConfigurationSection
    {

        [ConfigurationProperty("connList", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ConnCollection), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, AddItemName = "conn")]
        public ConnCollection conns
        {
            get
            {
                return (ConnCollection)base["connList"];
            }
            set
            {
                base["connList"] = value;
            }
        }

    }


    public class ConnCollection : ConfigurationElementCollection
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnInfo)element).Name;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnInfo();
        }

        public ConnInfo this[int i]
        {
            get
            {
                return (ConnInfo)base.BaseGet(i);
            }
        }

        public ConnInfo this[string key]
        {
            get
            {
                return (ConnInfo)base.BaseGet(key);
            }
        }

    }


    public class ConnInfo : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("connectionString", IsRequired = false, DefaultValue = "")]
        public string ConnString
        {
            get
            {
                return (string)base["connectionString"];
            }
            set
            {
                base["connectionString"] = value;
            }
        }


        [ConfigurationProperty("table", IsRequired = false, DefaultValue = "")]
        public string Table
        {
            get
            {
                return (string)base["table"];
            }
            set
            {
                base["table"] = value;
            }
        }

        [ConfigurationProperty("changeDecType", IsRequired = false, DefaultValue = 1)]
        public int IsChangeDecType
        {
            get
            {
                return (int)base["changeDecType"];
            }
            set
            {
                base["changeDecType"] = value;
            }
        }

        [ConfigurationProperty("tradeCode", IsRequired = false, DefaultValue = "")]
        public string Trade_Code
        {
            get
            {
                return (string)base["tradeCode"];
            }
            set
            {
                base["tradeCode"] = value;
            }
        }
    }
}
