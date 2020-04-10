using boblightc.Device;
using boblightc.tests.integration.Mocks;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace boblightc.tests.integration
{
    public class LiveRS232Tests
    {
        private readonly string _configFileFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SampleConfigFiles");

        private CConfig config;
        private List<CDevice> devices;
        private List<CLight> lights;
        private MockServer mockServer;
        private CDeviceRS232 rs232Device;

        [SetUp]
        public void Setup()
        {
            config = new CConfig();
            devices = new List<CDevice>();
            lights = new List<CLight>();
            mockServer = new MockServer();
        }

        [TearDown]
        public void TearDown()
        {
            if (rs232Device != null)
            {
                rs232Device.StopThread();
                rs232Device = null;
            }

            lights = null;
            devices = null;
            config = null;
        }

        [Test]
        public void SolidColorTest()
        {
            IChannelDataProvider solidColorCycler = new CycleSolidColorChannelProvider();

            rs232Device = CreateRS232Device(solidColorCycler);

            rs232Device.Run(255);
        }

        [Test]
        public void WalkingTest()
        {
            WalkingChannelProvider solidColorCycler = new WalkingChannelProvider();

            rs232Device = CreateRS232Device(solidColorCycler);

            solidColorCycler.NumberOfLights = lights.Count;

            rs232Device.Run(255);
        }

        private CDeviceRS232 CreateRS232Device(IChannelDataProvider clients)
        {
            bool result;

            result = config.LoadConfigFromFile(Path.Combine(_configFileFolderPath, "boblight.conf"));
            Assert.IsTrue(result);

            result = config.CheckConfig();
            Assert.IsTrue(result);

            result = config.BuildConfig(mockServer, clients, devices, lights);
            Assert.IsTrue(result);
            Assert.AreEqual(1, devices.Count);
            Assert.IsInstanceOf<CDeviceRS232>(devices[0]);

            return (CDeviceRS232)devices[0];
        }
    }
}