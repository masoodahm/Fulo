using System;
using System.IO;
using System.Text;

namespace Fulo.Lang
{
    class Program
    {
        public static string latestErrorMsg = null;
        private static bool hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1) {
                Console.WriteLine("usage fulo [Script.fls]");
            } else if (args.Length == 1) {
                runFile(args[0]);
            }
        }

        private static void runFile(string path)
        {
            if(path.EndsWith(".fls")) {
                try {
                    byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    string fileName = path;
                    if (path.Contains("/")) {
                        fileName = path.Substring(path.LastIndexOf('/') + 1);
                    }
                    run(bytes.ToString(), fileName);
                    if (hadError) Environment.Exit(65);
                } catch (FileNotFoundException) {
                    printAndStoreError(path + ": File not Found");
                } catch (IOException e) {
                    printAndStoreError(e.Message);
                }
            } else {
                printAndStoreError("Fulo scripts must end with '.fls'.");
            }
        }

        private static void run(string source, string fileName)
        {
            
        }

        public static void printAndStoreError(string errorMsg) {
            latestErrorMsg = errorMsg;
            Console.WriteLine(errorMsg);
        }
    }
}
