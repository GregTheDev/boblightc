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

            Log("Connected\r\n");
        }

        private void btnSendHello_Click(object sender, EventArgs e)
        {
            Log("Sending 'hello'...\r\n");
            string response = _client.Hello();
            Log("... sent\r\n");

            Log($"Received '{response}' response\r\n");
        }

        private void Log(string message)
        {
            txtLog.AppendText(message);
        }

        private void btnSendPing_Click(object sender, EventArgs e)
        {
            Log("Sending 'ping'...\r\n");
            string response = _client.Ping();
            Log("... sent\r\n");

            Log($"Received '{response}' response\r\n");
        }

        private void btnSendGetVersion_Click(object sender, EventArgs e)
        {
            Log("Sending 'get version'...\r\n");
            string response = _client.GetVersion();
            Log("... sent\r\n");

            Log($"Received '{response}' response\r\n");
        }
    }
}
