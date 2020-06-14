using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests.integration
{
    [TestFixture]
    public class CClientsHandlerTests : ProtocolTestsBase
    {
        [Test]
        public void FillChannels_Should_FlagUsedLights()
        {
            var testDevice = _devices[0];
            Type testDeviceType = testDevice.GetType();

            testDeviceType.InvokeMember("SetupDevice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, testDevice, null);

            Send("set priority 128");
            Send("set priority 255");
            Send("set priority 128");
            Send("set light start1 rgb 1.0 0.0 0.0");
            //Send("set light start2 rgb 1.0 1.0 0.0");
            //Send("set light start3 rgb 1.0 1.0 1.0");
            //Send("set light start4 use false");

            SendAndReceive("ping", out byte[] data, out int bytesReceived);
            testDeviceType.InvokeMember("WriteOutput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, testDevice, null);
            testDeviceType.InvokeMember("WriteOutput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, testDevice, null);

            Send("set light start2 rgb 1.0 0.0 0.0");
            SendAndReceive("ping", out data, out bytesReceived);
            testDeviceType.InvokeMember("WriteOutput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, testDevice, null);

            testDeviceType.InvokeMember("CloseDevice", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Instance  | System.Reflection.BindingFlags.InvokeMethod, null, testDevice, null);


            return;

            // Wait for server to finish processing commands
            //string response = SendAndReceive("ping", out byte[] data, out int bytesReceived);

            //IReadOnlyList<CChannel> channels = testDevice.GetChannels();

            //_clientsHandler.FillChannels(testDevice.GetChannels(), 1, testDevice);

            //foreach (var light in _lights)
            //{
            //    if (light.Name == "start4")
            //        Assert.AreEqual(0, light.m_users.Count);
            //    else
            //        Assert.AreEqual(1, light.m_users.Count);
            //}

        }
    }
}
