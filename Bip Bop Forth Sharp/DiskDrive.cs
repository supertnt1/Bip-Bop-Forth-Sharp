using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bip_Bop_Forth_Sharp
{
    class DiskDrive : Device
    {
        //disk drive allows the forth environment to interact with a binary file as if it where a disk drive
        public override void Init(Forth f)
        {
            Console.WriteLine("Disk Drive v0.01");
            f.registerFunc("dinfo", dinfo);
        }
        static bool SMount(Forth f)
        {
            return true;
        }
        static bool dinfo(Forth f)
        {
            Console.WriteLine("Disk Drive v0.01");
            Console.Write("Pages used");
            Console.WriteLine();
            return true;
        }
    }
}
