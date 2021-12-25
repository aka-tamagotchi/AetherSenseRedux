using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux
{
    public enum ButtplugStatus
    {
        Error,
        Uninitialized,
        Connected,
        Connecting,
        Disconnected
    }

    public enum WaitType
    {
        Slow_Timer,
        Use_Sleep,
        Use_Delay
    }

}
