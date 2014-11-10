using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZodiacGlass
{
    class DesignTimeZodiacGlass : IVirtualZodiacGlass
    {
        public int GetEquippedMainHandLightAmount()
        {
            return DateTime.Now.Second / 15 * 15;
        }

        public int GetEquippedOffHandLightAmount()
        {
            return DateTime.Now.Second / 20 * 20;
        }

        public int GetEquippedMainHandID()
        {
            return 7865;
        }

        public int GetEquippedOffHandID()
        {
            return 0;
        }
    }
}
