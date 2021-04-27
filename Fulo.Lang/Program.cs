using System;
using FuloMain = Fulo.Lang.Main.Main;

namespace Fulo.Lang
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1) {
                Console.WriteLine("usage fulo [Script.fls]");
            } else if (args.Length == 1) {
                FuloMain.runFile(args[0]);                
            }
        }
    }
}
