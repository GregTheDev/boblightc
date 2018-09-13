using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boblight_tester
{
    public partial class Form1 : Form
    {
        private BoblightClient _client;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _client = new BoblightClient(txtServerIp.Text, Int32.Parse(txtServerPort.Text));
            _client.Open();

            txtLog.AppendText("Connected\r\n");
        }
    }
}
