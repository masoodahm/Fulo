using System;
using System.Collections.Generic;
using System.IO;
using Fulo.Lang.Core;
using Fulo.Lang.Tokens;

namespace Fulo.Lang.Main
{
    public class Main
    {
        public static string latestErrorMsg = null;
        private static bool hadError = false;

        public static void runFile(string path)
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
                    PrintAndStoreError(path + ": File not Found.");
                } catch (IOException e) {
                    PrintAndStoreError(e.Message);
                }
            } else {
                PrintAndStoreError("Fulo scripts must end with '.fls'.");
            }
        }

        private static void run(string source, string fileName)
        {
            Lexer lexer = new Lexer(source, fileName);
            IList<Token> tokens = lexer.ScanTokens();
        }

        public static void PrintAndStoreError(string errorMsg) {
            latestErrorMsg = errorMsg;
            Console.WriteLine(errorMsg);
        }

        public static void Error(int line, string where, string message) {
            report(line, where, message);
        }

        public static void Error(Token token, string message) {
            if (token.type == TokenType.EOF) {
                report(token.line, "at end of " + token.file, message);
            } else {
                report(token.line, "at '" + token.lexeme + "' " + token.file, message);
            }
        }

        private static void report(int line, string where, string message) {
            PrintAndStoreError("[line " + line + "] Error " + where + " : " + message);
            hadError = true;
        }
        
    }
}
