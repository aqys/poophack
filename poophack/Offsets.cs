using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poophack
{
    internal class Offsets
    {
        //bases
        public int localPlayer = 0x180DB18;
        public int entityList = 0x181C1D0;
        public int viewMatrix = 0x180DB18;

        // attibutes
        public int teamNum = 0x3c3;
        public int jumpFlag = 0x3cc;
        public int health = 0x324;
        public int origin = 0x0D58;
    }
}
