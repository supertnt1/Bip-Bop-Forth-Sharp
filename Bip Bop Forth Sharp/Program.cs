using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
namespace Bip_Bop_Forth_Sharp
{
    class Program
    {
        static bool running;
        static void Main(string[] args)
        {
            running = true;
            Forth f = new Forth();
            f.registerFunc("saveword", saveWord);
            f.registerFunc("exit", exit);
            f.registerFunc("loadfile", loadfile);
            f.RegisterDevice(new DiskDrive(), 1, 2); //register devices
            if (File.Exists("autorun.txt"))
            {
                Console.WriteLine("Autorun file exists, now running it");
                runAuto("autorun.txt", f);

            }
            while (running)
            {
                List<string> code = f.Compile();
                f.execute(code.ToArray());
            }
        }
        static void runAuto(string file,Forth f)
        {
            string contents;
            using (StreamReader sr = new StreamReader(file))
            {
                contents = sr.ReadToEnd();
            }

            char[] delimiters = new char[] { '\r', '\n', ' ','\t' };

            string[] i = contents.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            f.words.AddRange(i);
            List<string> t = f.Compile();
            f.execute(t.ToArray());
        }
        static bool saveWord(Forth f)
        {
            Console.WriteLine("Save Word");
            Console.Write("Type the forth word you want saving");
            string word;
            while (true)
            {
               Console.Write("/nType the forth word you wish to be saved\n\t");
               word = Console.ReadLine().ToLower();
                if (!(word == ""))
                {
                    if (f.createdDict.ContainsKey(word))
                    {
                        using (StreamWriter sw = new StreamWriter(word+".txt"))
                        {
                            sw.WriteLine(":");
                            sw.WriteLine(word);
                            List<string> t = f.createdDict[word];
                            foreach (string i in t)
                            {
                                sw.WriteLine(i);
                            }
                            sw.WriteLine(";");
                        }
                        Console.WriteLine("Saved " + word + "to" + word + ".txt");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Word not found: " + word);
                    }
                }
            }
            return true;
        }
        static bool exit(Forth f)
        {
            running = false;
            Console.WriteLine("Now quitting");
            return true;
        }
        static bool loadfile(Forth f)
        {
            Console.Write("\nEnter file to load and run: ");
            string file = Console.ReadLine().ToLower();
            string contents;
            using (StreamReader sr = new StreamReader(file + ".txt"))
            {
                contents = sr.ReadToEnd();
            }

            char[] delimiters = new char[] { '\r', '\n' };

            string[] i = contents.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            f.words.AddRange(i);
            return false;

        }
    }
}
