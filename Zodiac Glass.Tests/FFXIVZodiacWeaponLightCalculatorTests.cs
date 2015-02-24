
namespace ZodiacGlass.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ZodiacGlass.FFXIV;

    [TestClass]
    public class FFXIVZodiacWeaponLightCalculatorTests
    {
        [TestMethod]
        public void TestNonCurrentStoneValues()
        {
            int expectedValue = 0;

            for (int i = 0; i <= FFXIVZodiacWeaponLightCalculator.MaxStones; i++)
            {
                int rawLight = i * FFXIVZodiacWeaponLightCalculator.NextStoneIndicator;

                Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(rawLight), expectedValue);

                if (i < FFXIVZodiacWeaponLightCalculator.MaxStones)
                {
                    expectedValue += (FFXIVZodiacWeaponLightCalculator.FullStoneValue - FFXIVZodiacWeaponLightCalculator.InitialStoneValue);                    
                }
            }

            Assert.AreEqual(expectedValue, FFXIVZodiacWeaponLightCalculator.MaxLightAmount);
        }

        [TestMethod]
        public void TestEmptyCurrentStoneValues()
        {
            int expectedValue = 0;

            for (int i = 0; i <= FFXIVZodiacWeaponLightCalculator.MaxStones; i++)
            {
                int rawLight = (i * FFXIVZodiacWeaponLightCalculator.NextStoneIndicator) + FFXIVZodiacWeaponLightCalculator.InitialStoneValue;

                Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(rawLight), expectedValue);

                if (i < FFXIVZodiacWeaponLightCalculator.MaxStones)
                {
                    expectedValue += (FFXIVZodiacWeaponLightCalculator.FullStoneValue - FFXIVZodiacWeaponLightCalculator.InitialStoneValue);
                }
            }

            Assert.AreEqual(expectedValue, FFXIVZodiacWeaponLightCalculator.MaxLightAmount);
        }

        [TestMethod]
        public void TestFullCurrentStoneValues()
        {
            int expectedValue = 0;

            for (int i = 1; i <= FFXIVZodiacWeaponLightCalculator.MaxStones; i++)
            {
                expectedValue += FFXIVZodiacWeaponLightCalculator.FullStoneValue - FFXIVZodiacWeaponLightCalculator.InitialStoneValue;

                int rawLight = ((i - 1) * FFXIVZodiacWeaponLightCalculator.NextStoneIndicator) + FFXIVZodiacWeaponLightCalculator.FullStoneValue;

                Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(rawLight), expectedValue);
            }

            Assert.AreEqual(expectedValue, FFXIVZodiacWeaponLightCalculator.MaxLightAmount);
        }


        [TestMethod]
        public void TestRandomStoneValues()
        {
            Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(505), 79 + 4);
            Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(1000), 79 * 2);
            Assert.AreEqual(FFXIVZodiacWeaponLightCalculator.Calculate(6000), FFXIVZodiacWeaponLightCalculator.MaxLightAmount);
        }
    }
}
