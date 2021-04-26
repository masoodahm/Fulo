using System.Collections.Generic;
using Fulo.Lang.Tokens;
using static Fulo.Lang.Tokens.TokenType;

namespace Fulo.Lang.Core
{
    public class Lexer
    {
        private readonly string source;
        private string fileName;
        private int current = 0;
        private int start = 0;

        private int line = 1;

        private IList<Token> tokens = new List<Token>();

        public Lexer(string source, string fileName)
        {
            this.source = source;
            this.fileName = fileName;
        }

        public IList<Token> ScanTokens() {
            while (! isAtEnd()) {
                start = current;
                scanToken();
            }
            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        private bool isAtEnd() {
            return current >= source.Length;
        }
    }
}
