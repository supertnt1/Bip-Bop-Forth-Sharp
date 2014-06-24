using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bip_Bop_Forth_Sharp
{
    class Device
    {
        public int page;
        public int amount;
        public virtual void Init(Forth f)
        {
        }
        public virtual void WriteMem(int address, int value)
        {
        }
        public virtual int ReadMem(int address)
        {
            return 0;
        }
    }
}
