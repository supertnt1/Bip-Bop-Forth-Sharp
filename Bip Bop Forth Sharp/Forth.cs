using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Bip_Bop_Forth_Sharp
{
    class Forth
    {
        public List<string> words = new List<string>();
        public Stack<int> fStack = new Stack<int>();
        public Stack<string> cStack = new Stack<string>();
        public List<string> currentCode = new List<string>(); //stores the current code so forth functions can use it.
        Dictionary<string, Func<string[], int,Forth, int>> runDict = new Dictionary<string, Func<string[], int,Forth, int>>() //runtime forth functions
        { 
            {"+",functions.fAdd},
            {"-",functions.fSub},
            {"*",functions.fMul},
            {"/",functions.fDiv},
            {"=",functions.fEQ},
            {"<",functions.fLT},
            {">",functions.fGT},
            {"dup",functions.fDup},
            {".",functions.fDot},
            {"emit",functions.fEmit},
            {"cr",functions.fCR},
            {"dump",functions.fDump},
            {"create",functions.fCreate},
            {"mcreate",functions.fMCreate},
            
            {"!",functions.fStore},
            {"@",functions.fFetch},
            {"rand",functions.fRand},
            {"swap",functions.fSwap},
            {"%",functions.fMod},
            {"innumber",functions.fNumin},

            {"PRINT",functions.fString},
            {"PUSH",functions.fPush}, //not accessible to the programmer
            {"RUN",functions.fRun},
            {"JZ",functions.fJZ},
            {"JMP",functions.fJMP},
        };
        Dictionary<string, Func<Forth,bool>> shellDict = new Dictionary<string, Func<Forth,bool>>() //shell functions
        {
        };
        Dictionary<string,Func<List<string>,Forth,bool>> compDict = new Dictionary<string,Func<List<string>,Forth,bool>>() //compile time functions
        {
            {":",functions.cColon},
            {";",functions.cSemi},
            {"begin",functions.cBegin},
            {"until",functions.cUntil},
            {"if",functions.cIF},
            {"else",functions.CElse},
            {"then",functions.CThen},
            {".\"",functions.Cstring},
            {"\"",functions.cEndString}
        };

        public Dictionary<int, int> fMemory = new Dictionary<int, int>();
        public Dictionary<int, string> fMemNames = new Dictionary<int, string>();
        public Dictionary<string, List<string>> createdDict = new Dictionary<string, List<string>>();
        Dictionary<int, Device> Devices = new Dictionary<int, Device>();
        public Random r = new Random();
        public Forth()
        {
            Console.WriteLine("Forth interpreter V0.01");
        }
        public void RegisterDevice(Device name, int page,int amount)
        {
            for (int i = page; i < page + amount;i++)
            {
                Devices[i] = name;
                Devices[i].Init(this);
                Devices[i].page = i;
                Devices[i].amount = r.Next();
            }
        }
        public Device checkPage(int memAddress)
        {
            Device d = null;
            int mem = memAddress / 512;
            if (Devices.ContainsKey(mem))
            {
                d = Devices[mem];
            }
            return d;
        }
        public void registerFunc(string name, Func<Forth, bool> callback)
        {
            shellDict.Add(name, callback);
        }

        public void addWord(ref List<string> code,string name)
        {
            List<string> templist = new List<string>();
            while (code.Count > 0)
            {
                templist.Add(code[0]);
                code.RemoveAt(0);
            }
            string namel = name.ToLower();
            if (!(runDict.ContainsKey(namel)||compDict.ContainsKey(namel)||shellDict.ContainsKey(namel)))
            {
                 createdDict[namel] = templist;
            }
        }
        private void emptyCompStack()
        {
            if (cStack.Count > 0)
            {
                while (cStack.Count > 0)
                {
                    cStack.Pop();
                }
            }
        }
        private void emptyCode()
        {
            currentCode.RemoveRange(0, currentCode.Count);
        }

        public List<string> Compile()
        {
            emptyCompStack();
            emptyCode();
            List<string> compiled = new List<string>();
            string prmpt = "Forth>";
            while (true)
            {
                string word = getWord(prmpt).ToLower();
                Func<Forth,bool> sAct;
                Func<List<string>,Forth, bool> cAct;
                shellDict.TryGetValue(word, out sAct);
                compDict.TryGetValue(word,out cAct);

                if (sAct != null)
                {
                    if (words.Count == 0) //shell commands must only begin at the start or the end of a forth statement
                    {
                        sAct.Invoke(this);
                    }
                    else
                    {
                        Console.WriteLine("Shell commands must not be in the middle of a forth statement.");
                    }
                }
                else if (cAct != null)
                {
                    cAct.Invoke(currentCode, this);
                }
                else if (runDict.ContainsKey(word))
                {
                    currentCode.Add(word); //word was a run time word, add it to the compiled code;
                }
                else if (createdDict.ContainsKey(word)) // word was found in the created dictonary, add the necessary info to run it
                {
                    currentCode.Add("RUN");
                    currentCode.Add(word);
                }
                else //could be a number
                {
                    if (fMemNames.ContainsValue(word)) //word could be a variable
                    {
                        string f = "nothing";
                        if (cStack.Count != 0)
                        {
                            f = cStack.Peek();
                        }
                        if (f == "STRING")
                        {
                            currentCode.Add(word);
                        }
                        else
                        {
                            KeyValuePair<int, string> t = fMemNames.FirstOrDefault(x => x.Value == word);
                            currentCode.Add("PUSH"); //add necessary code to push it
                            currentCode.Add(t.Key.ToString());
                        }
                    } else {
                    
                        int t = 0;
                        if (int.TryParse(word, out t)) //word was a number push it on
                        {
                            //add the number to the code if it is in the middle of a string

                            string f ="nothing";
                            if (cStack.Count != 0)
                            {
                                f = cStack.Peek();
                            }
                            if (f == "STRING")
                            {
                                currentCode.Add(word);
                            }
                            else
                            {
                                currentCode.Add("PUSH");
                                currentCode.Add(word);
                            }
                        }
                        else
                        {
                            //add the word to the code if it is in the middle of a string
                            string f="nothing";
                            if (cStack.Count != 0)
                            {
                                f = cStack.Peek();
                            }
                            if (f == "STRING")
                            {
                                currentCode.Add(word);
                            }
                            else
                            {
                                Console.WriteLine("Unknown word: " + word);
                            }
                        }
                    }
                }
                compiled = currentCode;

                if (cStack.Count == 0) return compiled;
                prmpt = "...";
            }
        }
        public void execute(string[] code)
        {
            int p = 0;
            while (p < code.Length)
            {
                string word = code[p];
                p += 1;
                Func<string[], int, Forth, int> t = runDict[word];
                int next = t.Invoke(code, p, this);
                if (next != -1) p = next;
                
            }
        }
       public string getWord(string prompt = "... ") //get next space seperated word
        {
            if (words.Count == 0)
            {
                while (words.Count == 0)
                {
                    Console.Write(prompt);
                    string line = Console.ReadLine();
                    tokenise(line);
                }
            }
            string word = words[0];
            words.RemoveAt(0);
            
           return word;
        }
        void tokenise(string input) //split a string into its tokens
        {
            string[] a = input.Split(new char[] {' ','\n'});
            foreach (string t in a)
            {
                words.Add(t);
            }
        }
    }
}
