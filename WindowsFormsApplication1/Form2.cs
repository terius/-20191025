using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TS.Common;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private readonly string HGPath = System.Configuration.ConfigurationManager.AppSettings["HGPath"];
        private readonly string loginUser = System.Configuration.ConfigurationManager.AppSettings["RemotePathLoginName"];
        private readonly string loginPassword = System.Configuration.ConfigurationManager.AppSettings["RemotePathPassword"];
        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(HGPath);
            int len = di.GetFiles().Length;
            MessageBox.Show("获取到" + len + "个文件");
        }

       

        private void Form2_Load(object sender, EventArgs e)
        {
            FileHelper.CreateNetworkShare(HGPath, loginUser, loginPassword);
        }
    }
}
