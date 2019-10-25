
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TS.Common;
using TS.DAL;
using TS.Model;
using TS.TranCore;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            //using (SettingHelper setting = new SettingHelper())
            //{
            //    string ServiceName = setting.ServiceName;
            //    string DisplayName = setting.DisplayName;
            //    string Description = setting.Description;
            //}
        }

        
        private void btnReadFile_Click(object sender, EventArgs e)
        {
            HZAction action = new HZAction();
            action.BeginRun();
            label1.Text = "服务已启动";
        }



        private void Form1_Load(object sender, EventArgs e)
        {
           // ConnSectionGroup group = (ConnSectionGroup)ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).SectionGroups["ConnGroup"];
           // ConnsSection simple = group.ConnStringList;
           // foreach (var item in simple.conns)
           // {
                
           // }
        
           //string filename= @"D:\Study2\广州项目2016\0711\报文样例.xml";
           // string xml = XmlHelper.Serializer<EHS_ENTRY_HEAD>(new EHS_ENTRY_HEAD());
           //// EHS_ENTRY_HEAD file = XmlHelper.DeserializeFromFile<EHS_ENTRY_HEAD>(@"D:\Study2\广州项目2016\0711\报文样例.xml");
           // XmlDocument doc = new XmlDocument();
           // doc.Load(filename);
           // XmlNode node = doc.SelectSingleNode("DATA");
           // EHS_ENTRY_HEAD info = XmlHelper.Deserialize<EHS_ENTRY_HEAD>(node.InnerXml);
        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  EncryptConnection.EncryptConnectionString(true);
        }

    }
}
