using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bip_Bop_Forth_Sharp
{
    static class functions
    {
        #region forth run time functions
        //run time functions
        public static int fAdd(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(a + b);
            return -1;
        }
        public static int fSub(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(a - b);
            return -1;
        }
        public static int fDiv(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(a / b);
            return -1;
        }
        public static int fMul(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(a * b);
            return -1;
        }
        public static int fMod(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(a % b);
            return -1;
        }
        public static int fSwap(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            f.fStack.Push(b);
            f.fStack.Push(a);
            return -1;
        }
        public static int fEQ(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            int c = a == b ? 1 : 0;
            f.fStack.Push(c);
            return -1;
        }
        public static int fGT(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            int c = a > b ? 1 : 0;
            f.fStack.Push(c);
            return -1;
        }
        public static int fLT(string[] code, int p, Forth f)
        {
            int b = f.fStack.Pop();
            int a = f.fStack.Pop();
            int c = a < b ? 1 : 0;
            f.fStack.Push(c);
            return -1;
        }
        public static int fDup(string[] code, int p, Forth f)
        {
            int a = f.fStack.Peek();
            f.fStack.Push(a);
            return -1;
        }
        public static int fDot(string[] code, int p, Forth f)
        {
            int a = f.fStack.Pop();
            Console.WriteLine(a.ToString());
            return -1;
        }
        public static int fEmit(string[] code, int p, Forth f)
        {
            int a = f.fStack.Pop();
            Console.Write((char)a);
            return -1;
        }

        public static int fCR(string[] code, int p, Forth f)
        {
            Console.Write("\n\r");
            return -1;
        }

        public static int fPush(string[] code, int p, Forth f)
        {
            string t = code[p];
            int i;
            int.TryParse(t, out i);
            f.fStack.Push(i);
            return p + 1;

        }
        public static int fRun(string[] code, int p, Forth f)
        {
            string t = code[p];
            f.execute(f.createdDict[t].ToArray());
            return p + 1;

        }
        public static int fNumin(string[] code, int p, Forth f)
        {
            string t = Console.ReadLine();
            int i;
            if (int.TryParse(t, out i))
            {
                f.fStack.Push(i);
            }
            else
            {
                f.fStack.Push(0);
            }
            return -1;
        }
        public static int fJZ(string[] code, int p, Forth f)
        {
            if (f.fStack.Count > 0)
            {
                int t = f.fStack.Pop();
                if (t == 0)
                {
                    string slot = code[p];
                    int i;
                    int.TryParse(slot, out i);
                    return i;
                }
                else
                {
                    return p + 1;
                }
            }
            return -1;
        }
        public static int fJMP(string[] code, int p, Forth f)
        {
            int t;
            int.TryParse(code[p], out t);
            return t;
        }
        public static int fDump(string[] code, int p, Forth f)
        {
            Console.Write("Stack contains: [");
            for (int i = 0; i < f.fStack.Count; i++)
            {
                Console.Write(f.fStack.ElementAt(i).ToString() + ", ");
            }
            Console.WriteLine("]");
            return -1;
        }
        public static int fCreate(string[] code, int p, Forth f)
        {
            for (int i = 0; i < 512; i++) //hard code it at 512 variables atm
            {
                if (!f.fMemNames.ContainsKey(i))
                {
                    string s = f.getWord("...").ToLower();
                    int t;
                    if (!int.TryParse(s, out t)) //make sure number is not a variable
                    {
                        if (!f.fMemNames.ContainsValue(s))
                        {
                            f.fMemNames[i] = s;
                            return -1;
                        }
                    }
                }
            }
            return -1;
        }
        public static int fMCreate(string[] code, int p, Forth f)
        {
            for (int i = 0; i < 512; i++) //hard code it at 512 variables atm
            {
                if (!f.fMemNames.ContainsKey(i))
                {
                    string s = f.getWord("...").ToLower();
                    int t;
                    if (!int.TryParse(s, out t)) //make sure variable is not a number
                    {
                        int addr = f.fStack.Pop();
                        f.fMemNames[addr] = s;
                        return p + 1;
                    }

                }
            }
            return -1;
        }
        public static int fString(string[] code, int p, Forth f)
        {
            for (int i = p; i < code.Length; i++)
            {
                string word = code[i];
                if (word == "STOPPRINT")
                {
                    return i + 1;
                }
                else
                {
                    Console.Write(word + " ");
                }
            }
            return -1;
        }
        public static int fStore(string[] code, int p, Forth f)
        {
            int addr = f.fStack.Pop();
            int val = f.fStack.Pop();
            Device d = f.checkPage(addr);
            if (d != null)
            {
                d.WriteMem(addr, val);
            }
            else
            {
                f.fMemory[addr] = val;
            }
            
            return -1;
        }
        public static int fFetch(string[] code, int p, Forth f)
        {
            int addr = f.fStack.Pop();
            int val = f.fMemory[addr];
            Device d = f.checkPage(addr);
            if (d != null)
            {
                val = d.ReadMem(addr);
            }
            else
            {
                f.fStack.Push(f.fMemory[addr]);
            }
            return -1;
        }
        public static int fRand(string[] code, int p, Forth f)
        {
            f.fStack.Push(f.r.Next());
            return -1;
        }
        #endregion



        //compile time functions

        public static bool cColon (List<string> code,Forth f)
        {
            if (f.cStack.Count == 0)
            {
                string label = f.getWord("...");
                f.cStack.Push(label);
                f.cStack.Push("COLON");
            }
            return true;
        }
        public static bool cSemi(List<string> code, Forth f)
        {
             if (f.cStack.Count > 0)
            {
                string ccode = f.cStack.Pop();
                if (ccode == "COLON")
                {
                    string label = f.cStack.Pop();
                    f.addWord(ref f.currentCode, label);
                }
                else
                {
                    Console.WriteLine("Expected : , got: " + ccode); 
                }
            }
            else
            {
                Console.WriteLine("No : to match ; with.");
             }
            return true;
        }
        public static bool cBegin(List<string> code, Forth f)
        {  
            f.cStack.Push(code.Count.ToString());
                f.cStack.Push("BEGIN");
            
            return true;
        }
        public static bool cUntil(List<string> code, Forth f)
        {
            if (f.cStack.Count > 0)
            {
                string ccode = f.cStack.Pop();
                if (ccode == "BEGIN")
                {
                    code.Add("JZ");
                    string slot = f.cStack.Pop();
                    code.Add(slot);
                }
            }
            return true;
        }


        public static bool cIF(List<string> code, Forth f)
        {
            code.Add("JZ");
            code.Add("peanut");
            f.cStack.Push(code.Count.ToString());
            f.cStack.Push("IF");
            return true;
        }
        public static bool CElse(List<string> code, Forth f)
        {
            if (f.cStack.Count > 0)
            {
                string ccode = f.cStack.Pop();
                if (ccode == "IF")
                {
                    code.Add("JMP");
                    string slot = f.cStack.Pop();
                    f.cStack.Push(code.Count.ToString());
                    f.cStack.Push("ELSE");
                    code.Add("hazelnut");
                    int t;
                    int.TryParse(slot, out t);
                    code[t-1] = code.Count.ToString();
                }
            }
            return true;
        }
        public static bool CThen(List<string> code, Forth f)
        {
            if (f.cStack.Count > 0)
            {
                string ccode = f.cStack.Pop();
                if (ccode == "IF" || ccode == "ELSE")
                {
                    string slot = f.cStack.Pop();
                    int t;
                    int.TryParse(slot, out t);
                    if (ccode == "IF")
                    {
                        code[t-1] = code.Count.ToString();
                    }
                    else
                    {
                        code[t] = code.Count.ToString();
                    }
                }
            }
            return true;
        }
        public static bool Cstring(List<string> code, Forth f)
        {
            f.cStack.Push("STRING");
            code.Add("PRINT");
            return true;
        }
        public static bool cEndString(List<string> code, Forth f)
        {
            if (f.cStack.Count > 0)
            {
                string ccode = f.cStack.Pop();
                if (ccode == "STRING")
                {
                    code.Add("STOPPRINT");
                }
            }
            else
            {
                Console.WriteLine("No matching start of string");
            }
            return true;
        }
    }
}
