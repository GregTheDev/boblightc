using boblightc.Device;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace boblightc.tests.integration
{
    [TestFixture]
    public class ProtocolTests : ProtocolTestsBase
    {
        [Test]
        public void Hello_Should_ReturnCorrectData()
        {
            string response = SendAndReceive("hello", out byte[] data, out int bytesReceived);

            Assert.AreEqual(LINE_FEED, data[bytesReceived - 1]);
            Assert.AreEqual("hello\n", response);
        }

        [Test]
        public void Ping_Should_ReturnCorrectData_WhenNoLightsAreUsed()
        {
            string response = SendAndReceive("ping", out byte[] data, out int bytesReceived);

            Assert.AreEqual(LINE_FEED, data[bytesReceived - 1]);
            Assert.AreEqual("ping 0\n", response);
        }

        [Test]
        public void GetVersion_Should_ReturnCorrectData()
        {
            string response = SendAndReceive("get version", out byte[] data, out int bytesReceived);

            Assert.AreEqual(LINE_FEED, data[bytesReceived - 1]);
            Assert.AreEqual("version 5\n", response);
        }

        [Test]
        public void GetLights_Should_ReturnCorrectData()
        {
            string expectedResults = File.ReadAllText(Path.Combine(_resultsFolderPath, "GetLightsResults.txt"));

            string response = SendAndReceive("get lights", out byte[] data, out int bytesReceived);
            Assert.AreEqual(LINE_FEED, data[bytesReceived - 1]);
            Assert.AreEqual(expectedResults, response);
        }

        [Test]
        public void SetLightRgb_Should_SetLightValues()
        {
            Send("set light start1 rgb 0.1 0.2 0.3");

            // Give the server time to process the request
            Thread.Sleep(2 * 1000);

            var targetLight = _clientsHandler.Clients[0].Lights.Where(x => x.Name == "start1").First();

            float[] rgb = targetLight.GetRgb();

            Assert.AreEqual(0.1f, rgb[0]);
            Assert.AreEqual(0.2f, rgb[1]);
            Assert.AreEqual(0.3f, rgb[2]);
        }
    }
}
