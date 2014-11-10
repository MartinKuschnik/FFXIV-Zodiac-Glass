using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZodiacGlass
{
    interface IVirtualZodiacGlass
    {
        int GetEquippedMainHandLightAmount();

        int GetEquippedOffHandLightAmount();

        int GetEquippedMainHandID();

        int GetEquippedOffHandID();

    }
}
