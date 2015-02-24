
namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class FFXIVZodiacWeaponLightCalculator
    {
        public const int MaxStones = 12;
        public const int InitialStoneValue = 1;
        public const int FullStoneValue = 80;
        public const int NextStoneIndicator = 500;
        public const int MaxLightAmount = (FullStoneValue - InitialStoneValue) * MaxStones;

        public static int Calculate(int rawLight)
        {
            // 0   = 0 stones
            // 1   = 1 stone and 0 light
            // 80  = fist stone full
            // 501 = 1 stone full and one empty one


            if (rawLight > 0)
            {
                int fullStones = (rawLight / NextStoneIndicator);
                int currentStoneValue = rawLight % NextStoneIndicator;

                if (currentStoneValue > 1)
                {
                    return (fullStones * (FullStoneValue - InitialStoneValue)) + currentStoneValue - InitialStoneValue;
                }
                else
                {
                    return fullStones * (FullStoneValue - InitialStoneValue);
                }
            }

            return 0;
        }
    }
}
