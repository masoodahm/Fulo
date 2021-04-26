using System;

namespace Fulo.Lang.Tokens
{
    public class Token
    {
        public readonly TokenType type;
        public readonly string lexeme;
        public readonly object literal;
        public readonly int line;

        public string file = "~unknown file~";

        public Token(TokenType type, string lexeme, object literal, int line) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public string toString() {
            return type + " " + lexeme + " " + literal;
        }
    }
}
