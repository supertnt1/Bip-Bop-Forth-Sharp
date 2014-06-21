using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bip_Bop_Forth_Sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Forth f = new Forth();
            while (true)
            {
                List<string> code = f.Compile();
                f.execute(code.ToArray());
            }
        }
    }
}
