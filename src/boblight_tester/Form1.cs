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

        private void btnGetLights_Click(object sender, EventArgs e)
        {
            Log("Sending 'get version'...");
            string response = _client.GetLights();
            Log(" sent\r\n");

            Log(response.Replace("\n", "\r\n"));
        }

        private void btnSendSetPriority_Click(object sender, EventArgs e)
        {
            Log("Sending 'set priority'...");
            string response = _client.SetPriority(int.Parse(txtPriority.Text));
            Log(" sent\r\n");

            Log(response.Replace("\n", "\r\n"));
        }

        private void btnSetLightRgb_Click(object sender, EventArgs e)
        {
            string[] rgbPieces = txtSetRgbRgb.Text.Split(',');

            Log("Sending 'set light rgb'...");
            _client.SetLightRgb(txtSetRgbLightName.Text, 
                float.Parse(rgbPieces[0]), 
                float.Parse(rgbPieces[1]), 
                float.Parse(rgbPieces[2]));

            Log(" sent\r\n");
        }

        private void btnSetLightSpeed_Click(object sender, EventArgs e)
        {
            Log("Sending 'set light speed'...");
            _client.SetLightSpeed(txtSetSpeedLightName.Text,
                float.Parse(txtSetSpeedSpeed.Text));

            Log(" sent\r\n");
        }

        private void btnSetLIghtInterpolation_Click(object sender, EventArgs e)
        {
            Log("Sending 'set light interpolation'...");
            _client.SetLightInterpolation(txtSetInterpolationName.Text,
                bool.Parse(txtSetInterpolation.Text));

            Log(" sent\r\n");
        }

        private void btnSetUse_Click(object sender, EventArgs e)
        {
            Log("Sending 'set light use'...");
            _client.SetLightUse(txtSetUseLightName.Text,
                bool.Parse(txtSetUse.Text));

            Log(" sent\r\n");
        }

        private void btnSetSingleChange_Click(object sender, EventArgs e)
        {
            Log("Sending 'set light singlechange'...");
            _client.SetLightSingleChange(txtSetSingleChangeLightName.Text,
                float.Parse(txtSetSingleChangeValue.Text));

            Log(" sent\r\n");
        }
    }
}
