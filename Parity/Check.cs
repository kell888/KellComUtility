using System;
using System.Collections.Generic;
using System.Text;

namespace Parity
{
    public static class Check
    {
        public static bool CheckParity(object raw, object parity)
        {
            ushort r = Convert.ToUInt16(raw);
            ushort p = Convert.ToUInt16(parity);
            return r == p;
        }
    }
}
