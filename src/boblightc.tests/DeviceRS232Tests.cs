using boblightc.Device;
using boblightc.tests.Mocks;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace boblightc.tests
{
    public class DeviceRS232Tests
    {
        private readonly string _configFileFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SampleConfigFiles");
        private readonly string _resultsFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SampleResults");
        private const int START_WRITES = 2;
        private const int CLOSE_WRITES = 1;

        private CConfig config;
        private List<CDevice> devices;
        private List<CLight> lights;
        private MockEmptyClientsHandler clients;
        private CDeviceRS232 rs232Device;
        private MockServer mockServer;

        [SetUp]
        public void Setup()
        {
            config = new CConfig();
            devices = new List<CDevice>();
            lights = new List<CLight>();
            mockServer = new MockServer();
            //clients = new MockClientsHandler(lights, new Dictionary<int, float>());
            //clients = new CClientsHandler(lights);
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
        public void Device_Should_BeConfiguredCorrectly()
        {
            rs232Device = CreateRS232Device(new CClientsHandler(lights));

            Assert.AreEqual("AmbiLight", rs232Device.Name);
            Assert.AreEqual(CDevice.MOMO, rs232Device.Type);
            Assert.AreEqual("SerialPortMock", rs232Device.Output);
            Assert.AreEqual(3, rs232Device.NrChannels);
            Assert.AreEqual(new int[] { 0x41, 0x64, 0x61, 0x00, 0x18, 0x4D }, rs232Device.m_prefix);
            Assert.AreEqual(new int[] { 0xF, 0xE, 0xD, 0xC, 0xB, 0xA }, rs232Device.m_postfix);
            Assert.AreEqual(20000, rs232Device.Interval);
            Assert.AreEqual(115200, rs232Device.Rate);
            Assert.AreEqual(false, rs232Device.Debug);
            Assert.AreEqual(100000, rs232Device.DelayAfterOpen);
        }

        [Test]
        public void Device_Should_WriteCorrectData_WhenNoClientsAreConnected()
        {
            CClientsHandler clients = new CClientsHandler(lights);

            rs232Device = CreateRS232Device(clients);

            rs232Device.Run(2);

            MockSerialPort mockSerialPort = MockSerialPort.Instance;
            Assert.AreEqual(START_WRITES + 2 + CLOSE_WRITES, mockSerialPort.Writes.Count);

            byte[] expectedBytes = { 0x41, 0x64, 0x61, 0x00, 0x18, 0x4D, 0, 0, 0, 0xF, 0xE, 0xD, 0xC, 0xB, 0xA };

            Assert.AreEqual(expectedBytes, mockSerialPort.Writes[3]);
            Assert.AreEqual(expectedBytes, mockSerialPort.Writes[4]);
        }

        [Test]
        public void Device_Should_WriteCorrectData_WhenClientIsConnected()
        {
            MockEmptyClientsHandler clients = new MockEmptyClientsHandler(lights, new Dictionary<int, float>
            {
                { 1, 0xFF0000 },
                { 2, 0x00FF00 },
                { 3, 0x0000FF }
            });
            rs232Device = CreateRS232Device(clients);

            rs232Device.Run(2);

            MockSerialPort mockSerialPort = MockSerialPort.Instance;
            Assert.AreEqual(START_WRITES + 2 + CLOSE_WRITES, mockSerialPort.Writes.Count);

            byte[] expectedBytes = { 0x41, 0x64, 0x61, 0x00, 0x18, 0x4D, 0xFF, 0xFF, 0xFF, 0xF, 0xE, 0xD, 0xC, 0xB, 0xA };

            int targetDataRow = START_WRITES + 2;

            Assert.AreEqual(expectedBytes, mockSerialPort.Writes[targetDataRow - 1]);
        }

        [Test]
        public void Device_Should_WriteCorrectData_WhenClientCycles()
        {
            MockIncrementingClientsHandler clients = new MockIncrementingClientsHandler(lights);
            rs232Device = CreateRS232Device(clients);

            rs232Device.Run(255);
            rs232Device.StopThread();

            MockSerialPort mockSerialPort = MockSerialPort.Instance;
            string[] expectedResults = File.ReadAllLines(Path.Combine(_resultsFolderPath, "RS232CycleResults.txt"));
            
            Assert.AreEqual(expectedResults.Length, mockSerialPort.Writes.Count);

            for (int i = 0; i < expectedResults.Length; i++)
            {
                string[] resultPieces = expectedResults[i].Split(" ");

                Assert.AreEqual(resultPieces.Length, mockSerialPort.Writes[i].Length);
                
                for (int j = 0; j < resultPieces.Length; j++)
                {
                    Assert.AreEqual(int.Parse(resultPieces[j], System.Globalization.NumberStyles.HexNumber), mockSerialPort.Writes[i][j]);
                }
            }
        }

        private CDeviceRS232 CreateRS232Device(IChannelDataProvider clients)
        {
            bool result;

            result = config.LoadConfigFromFile(Path.Combine(_configFileFolderPath, "RS232.conf"));
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