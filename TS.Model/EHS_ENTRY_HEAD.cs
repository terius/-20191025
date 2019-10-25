using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace TS.Model
{
    [Serializable]
    [XmlRoot("ENTRY_HEAD")]
    public  class EHS_ENTRY_HEAD
    {
        public string SHIP_ID { get; set; }
        public string VOYAGE_NO { get; set; }
        public string BILL_NO { get; set; }
        public string I_E_FLAG { get; set; }
        public string I_E_PORT { get; set; }
        public string L_D_PORT { get; set; }
        public string TRAF_NAME { get; set; }
        public string SEND_NAME { get; set; }
        public string OWNER_NAME { get; set; }
        public string SEND_COUNTRY { get; set; }
        public string SEND_CITY { get; set; }
        public string SEND_ID { get; set; }
        public string TRADE_CODE { get; set; }
        public string TRADE_NAME { get; set; }
        public string MAIN_G_NAME { get; set; }
        public Nullable<decimal> PACK_NO { get; set; }
        public Nullable<decimal> GROSS_WT { get; set; }
        public Nullable<decimal> TOTAL_VALUE { get; set; }
        public string CURR_CODE { get; set; }
        public Nullable<System.DateTime> I_E_DATE { get; set; }
        public Nullable<System.DateTime> D_DATE { get; set; }
        public string ENTRY_TYPE { get; set; }
        public string DEC_TYPE { get; set; }
        public string DEC_ER { get; set; }
        public Nullable<System.DateTime> DEC_DATE { get; set; }
        public Nullable<System.DateTime> READ_DATE { get; set; }
        public string READ_FLAG { get; set; }
        public string NOTE { get; set; }
        public string CHK_MARK { get; set; }
    }
}
