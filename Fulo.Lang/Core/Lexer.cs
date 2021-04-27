using System.Collections.Generic;
using Fulo.Lang.Tokens;
using static Fulo.Lang.Tokens.TokenType;
using static Fulo.Lang.Main.Main;
using System;
using System.Collections;

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
        private char lastStringChar = 'E';

        private static IDictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "STRICT", STRICT },
            { "and", AND },
            { "internal", INTERNAL },
            { "deprecated", DEPRECATED },
            { "class", CLASS },
            { "for", FOR },
            { "else", ELSE },
            { "false", FALSE },
            { "fun", FUN },
            { "if", IF },
            { "switch", SWITCH },
            { "nil", NIL },
            { "print", PRINT },
            { "or", OR },
            { "return", RETURN },
            { "super", SUPER },
            { "this", THIS },
            { "true", TRUE },
            { "while", WHILE },
            { "let", LET },
            { "break", BREAK },
            { "loop", LOOP },
            { "load", LOAD },
            { "is", IS },
            { "as", AS },
            { "match", MATCH },
            { "case", CASE },
            { "nameset", NAMESET },
            { "default", DEFAULT }
        };

        public Lexer(string source, string fileName)
        {
            this.source = source;
            this.fileName = fileName;
        }

        public IList<Token> ScanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }
            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(LEFT_PAREN); break;
                case ')': addToken(RIGHT_PAREN); break;
                case '{': addToken(LEFT_BRACE); break;
                case '}': addToken(RIGHT_BRACE); break;
                case '[': addToken(LEFT_SQUARE); break;
                case ']': addToken(RIGHT_SQUARE); break;
                case ',': addToken(COMMA); break;
                case '.': addToken(DOT); break;
                case ';': addToken(SEMICOLON); break;
                case '-': addToken(match('=') ? MINUS_ASSIGN : match('-') ? MINUS_MINUS : match('>') ? ASSIGN_ARROW : MINUS); break;
                case '+': addToken(match('=') ? PLUS_ASSIGN : match('+') ? PLUS_PLUS : PLUS); break;
                case '*': addToken(match('=') ? STAR_ASSIGN : match('*') ? STAR_STAR : STAR); break;
                case '%': addToken(match('=') ? PERCENT_ASSIGN : PERCENT); break;
                case '!': addToken(match('=') ? BANG_EQUAL : BANG); break;
                case '=': addToken(match('=') ? EQUAL_EQUAL : EQUAL); break;
                case '>': addToken(match('=') ? GREATER_EQUAL : GREATER); break;
                case '<': addToken(match('=') ? LESS_EQUAL : LESS); break;
                case ':': addToken(COLON); break;
                case '#': addToken(IMPORT); break;
                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else if (match('*'))
                    {
                        do
                        {
                            if (peek() == ('\n')) line++;
                            advance();
                        } while (!(match('*') && match('/') && !isAtEnd()));
                    }
                    else
                    {
                        addToken(match('=') ? SLASH_ASSIGN : SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                case '\'':
                    this.lastStringChar = (this.lastStringChar == 'E' ? '\'' : this.lastStringChar);
                    consumeString();
                    break;
                case '"':
                    this.lastStringChar = (this.lastStringChar == 'E' ? '"' : this.lastStringChar);
                    consumeString();
                    break;
                default:
                    if (isDigit(c))
                    {
                        consumeNumber();
                    } else if (isAlpha(c)) {
                        consumeIdentifier();
                    }
                    break;


            }
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool isAlpha(char c) {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private bool isAlphaNumeric(char c) {
            return isAlpha(c) || isDigit(c);
        }

        private void consumeString()
        {
            while ((peek() != this.lastStringChar) && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }
            this.lastStringChar = 'E';
            if (isAtEnd())
            {
                Error(line, fileName, "Unterminated string");
                return;
            }
            advance();
            string value = source.Substring(start + 1, current - 1);
            addToken(STRING, value);
        }

        private void consumeIdentifier() {
            while ( isAlphaNumeric(peek())) advance();
            string text = source.Substring(start, current);
            TokenType type;
            if (!keywords.TryGetValue(text, out type)) {
                type = IDENTIFIER;
            }
            addToken(type);
        }

        private void consumeNumber() {
            while(isDigit(peek())) advance();
            if (peek() == '.' && isDigit(peekNext())) {
                advance();
            }

            while(isDigit(peek())) advance();
            addToken(NUMBER, Double.Parse(source.Substring(start, current)));
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source[current];
        }

        private char peekNext() {
            if (current + 1 > source.Length) return '\0';
            return source[current + 1];
        }

        private char advance()
        {
            current++;
            return source[current - 1];
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current);
            Token tokenToAdd = new Token(type, text, literal, line);
            tokenToAdd.file = fileName;
            tokens.Add(tokenToAdd);
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;
            current++;
            return true;
        }
    }
}
