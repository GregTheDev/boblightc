using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace boblightc.tests
{
    [TestFixture]
    public class CLightTests
    {
        [Test]
        public void SetRgb_Should_SaveOldValuesAndSetNewValues()
        {
            CLight testLight = new CLight();

            // First change
            testLight.SetRgb(new float[] { 0.1f, 0.2f, 0.3f }, 111);
            
            float[] rgb = testLight.GetRgb();
            Assert.AreEqual(0.1f, rgb[0]);
            Assert.AreEqual(0.2f, rgb[1]);
            Assert.AreEqual(0.3f, rgb[2]);

            // 2nd change
            testLight.SetRgb(new float[] { 0.9f, 0.8f, 0.7f }, 222);
            rgb = testLight.GetRgb();
            Assert.AreEqual(0.9f, rgb[0]);
            Assert.AreEqual(0.8f, rgb[1]);
            Assert.AreEqual(0.7f, rgb[2]);

            rgb = testLight.GetPreviousRgb();
            Assert.AreEqual(0.1f, rgb[0]);
            Assert.AreEqual(0.2f, rgb[1]);
            Assert.AreEqual(0.3f, rgb[2]);
            Assert.AreEqual(111, testLight.PreviousTime);
        }

        [Test]
        public void GetColor()
        {
            CLight testLight = new CLight();
            testLight.AddColor(new CColor {
                Name = "red",
                Rgb = new float[] { 1.0f, 0f, 0f }
            });
            testLight.AddColor(new CColor
            {
                Name = "green",
                Rgb = new float[] { 0f, 1f, 0f }
            });
            testLight.AddColor(new CColor
            {
                Name = "blue",
                Rgb = new float[] { 0f, 0f, 1f }
            });

            // First change
            testLight.SetRgb(new float[] { 0.1f, 0.2f, 0.3f }, 111);

            var thingOne = testLight.GetColorValue(0, 11);
            var thingTwo = testLight.GetColorValue(1, 11);
            var thingThree = testLight.GetColorValue(2, 11);
        }
    }
}
