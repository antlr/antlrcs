/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace StringTemplate
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Console = System.Console;
    using Exception = System.Exception;
    using NumberStyles = System.Globalization.NumberStyles;
    using StringBuilder = System.Text.StringBuilder;

    public class TemplateLexer : ITokenSource
    {
        public const char EOF = unchecked((char)(-1));            // EOF char
        public const int EOF_TYPE = CharStreamConstants.EndOfFile;  // EOF token type

        public class TemplateToken : CommonToken
        {
            public TemplateToken(ICharStream input, int type, int channel, int start, int stop)
                : base(input, type, channel, start, stop)
            {
            }

            public TemplateToken(int type, string text)
                : base(type, text)
            {
            }

            public override string ToString()
            {
                string channelStr = "";
                if (Channel > 0)
                {
                    channelStr = ",channel=" + Channel;
                }
                string txt = Text;
                if (txt != null)
                {
                    txt = Misc.ReplaceEscapes(txt);
                }
                else
                {
                    txt = "<no text>";
                }
                return "[@" + TokenIndex + "," + StartIndex + ":" + StopIndex + "='" + txt + "',<" + TemplateParser.tokenNames[Type] + ">" + channelStr + "," + Line + ":" + CharPositionInLine + "]";
            }
        }

        public static readonly IToken SKIP = new TemplateToken(-1, "<skip>");

        // TODO: enum?
        // pasted from STParser
        public const int RBRACK = 17;
        public const int LBRACK = 16;
        public const int ELSE = 5;
        public const int ELLIPSIS = 11;
        public const int LCURLY = 20;
        public const int BANG = 10;
        public const int EQUALS = 12;
        public const int TEXT = 22;
        public const int ID = 25;
        public const int SEMI = 9;
        public const int LPAREN = 14;
        public const int IF = 4;
        public const int ELSEIF = 6;
        public const int COLON = 13;
        public const int RPAREN = 15;
        public const int WS = 27;
        public const int COMMA = 18;
        public const int RCURLY = 21;
        public const int ENDIF = 7;
        public const int RDELIM = 24;
        public const int SUPER = 8;
        public const int DOT = 19;
        public const int LDELIM = 23;
        public const int STRING = 26;
        public const int PIPE = 28;
        public const int OR = 29;
        public const int AND = 30;
        public const int INDENT = 31;
        public const int NEWLINE = 32;
        public const int AT = 33;
        public const int REGION_END = 34;

        char delimiterStartChar = '<';
        char delimiterStopChar = '>';

        bool scanningInsideExpr = false;
        internal int subtemplateDepth = 0; // start out *not* in a {...} subtemplate 

        ICharStream input;
        char c;        // current character
        int startCharIndex;
        int startLine;
        int startCharPositionInLine;

        List<IToken> tokens = new List<IToken>();

        public IToken NextToken()
        {
            if (tokens.Count > 0)
            {
                var result = tokens[0];
                tokens.RemoveAt(0);
                return result;
            }
            return _nextToken();
        }

        public TemplateLexer(ANTLRStringStream input)
            : this(input, '<', '>')
        {
        }

        public TemplateLexer(ICharStream input, char delimiterStartChar, char delimiterStopChar)
        {
            this.input = input;
            c = (char)input.LA(1); // prime lookahead
            this.delimiterStartChar = delimiterStartChar;
            this.delimiterStopChar = delimiterStopChar;
        }

        /** Ensure x is next character on the input stream */
        public void Match(char x)
        {
            if (c == x)
                Consume();
            else
                throw new Exception("expecting " + x + "; found " + c);
        }

        protected void Consume()
        {
            input.Consume();
            c = (char)input.LA(1);
        }

        public void Emit(IToken token)
        {
            tokens.Add(token);
        }

        public IToken _nextToken()
        {
            //System.out.println("nextToken: c="+(char)c+"@"+input.index());
            while (true)
            { // lets us avoid recursion when skipping stuff
                startCharIndex = input.Index;
                startLine = input.Line;
                startCharPositionInLine = input.CharPositionInLine;

                if (c == EOF)
                    return NewToken(EOF_TYPE);
                IToken t = null;
                if (scanningInsideExpr)
                    t = Inside();
                else
                    t = Outside();
                if (t != SKIP)
                    return t;
            }
        }

        protected IToken Outside()
        {
            if (input.CharPositionInLine == 0 && (c == ' ' || c == '\t'))
            {
                while (c == ' ' || c == '\t')
                    Consume(); // scarf indent
                return NewToken(INDENT);
            }
            if (c == delimiterStartChar)
            {
                Consume();
                if (c == '!')
                {
                    mCOMMENT();
                    return SKIP;
                }
                if (c == '\\')
                    return ESCAPE(); // <\\> <\uFFFF> <\n> etc...
                scanningInsideExpr = true;
                return NewToken(LDELIM);
            }
            if (c == '\r')
            {
                // \r\n -> \n
                Consume();
                Consume();
                return NewToken(NEWLINE);
            }
            if (c == '\n')
            {
                Consume();
                return NewToken(NEWLINE);
            }
            if (c == '}' && subtemplateDepth > 0)
            {
                scanningInsideExpr = true;
                subtemplateDepth--;
                Consume();
                return NewTokenFromPreviousChar(RCURLY);
            }
            return mTEXT();
        }

        protected IToken Inside()
        {
            while (true)
            {
                switch (c)
                {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    Consume();
                    continue;
                case '.':
                    Consume();
                    if (input.LA(1) == '.' && input.LA(2) == '.')
                    {
                        Consume();
                        Match('.');
                        return NewToken(ELLIPSIS);
                    }
                    return NewToken(DOT);
                case ',':
                    Consume();
                    return NewToken(COMMA);
                case ':':
                    Consume();
                    return NewToken(COLON);
                case ';':
                    Consume();
                    return NewToken(SEMI);
                case '(':
                    Consume();
                    return NewToken(LPAREN);
                case ')':
                    Consume();
                    return NewToken(RPAREN);
                case '[':
                    Consume();
                    return NewToken(LBRACK);
                case ']':
                    Consume();
                    return NewToken(RBRACK);
                case '=':
                    Consume();
                    return NewToken(EQUALS);
                case '!':
                    Consume();
                    return NewToken(BANG);
                case '@':
                    Consume();
                    if (c == 'e' && input.LA(2) == 'n' && input.LA(3) == 'd')
                    {
                        Consume();
                        Consume();
                        Consume();
                        return NewToken(REGION_END);
                    }
                    return NewToken(AT);
                case '"':
                    return mSTRING();
                case '&':
                    Consume();
                    Match('&');
                    return NewToken(AND); // &&
                case '|':
                    Consume();
                    Match('|');
                    return NewToken(OR); // ||
                case '{':
                    return SubTemplate();
                default:
                    if (c == delimiterStopChar)
                    {
                        Consume();
                        scanningInsideExpr = false;
                        return NewToken(RDELIM);
                    }
                    if (IsIDStartLetter(c))
                    {
                        IToken id = mID();
                        string name = id.Text;
                        if (name.Equals("if"))
                            return NewToken(IF);
                        else if (name.Equals("endif"))
                            return NewToken(ENDIF);
                        else if (name.Equals("else"))
                            return NewToken(ELSE);
                        else if (name.Equals("elseif"))
                            return NewToken(ELSEIF);
                        else if (name.Equals("super"))
                            return NewToken(SUPER);
                        return id;
                    }
                    RecognitionException re = new NoViableAltException("", 0, 0, input);
                    if (c == EOF)
                    {
                        throw new TemplateRecognitionException("EOF inside ST expression", re);
                    }
                    throw new TemplateRecognitionException("invalid character: " + c, re);
                }
            }
        }

        private IToken SubTemplate()
        {
            // look for "{ args ID (',' ID)* '|' ..."
            subtemplateDepth++;
            int m = input.Mark();
            int curlyStartChar = startCharIndex;
            int curlyLine = startLine;
            int curlyPos = startCharPositionInLine;
            List<IToken> argTokens = new List<IToken>();
            Consume();
            IToken curly = NewTokenFromPreviousChar(LCURLY);
            mWS();
            argTokens.Add(mID());
            mWS();
            while (c == ',')
            {
                Consume();
                argTokens.Add(NewTokenFromPreviousChar(COMMA));
                mWS();
                argTokens.Add(mID());
                mWS();
            }
            mWS();
            if (c == '|')
            {
                Consume();
                argTokens.Add(NewTokenFromPreviousChar(PIPE));
                if (IsWS(c))
                    Consume(); // ignore a single whitespace after |
                //System.out.println("matched args: "+argTokens);
                foreach (IToken t in argTokens)
                    Emit(t);
                input.Release(m);
                scanningInsideExpr = false;
                startCharIndex = curlyStartChar; // reset state
                startLine = curlyLine;
                startCharPositionInLine = curlyPos;
                return curly;
            }
            //System.out.println("no match rewind");
            input.Rewind(m);
            startCharIndex = curlyStartChar; // reset state
            startLine = curlyLine;
            startCharPositionInLine = curlyPos;
            Consume();
            scanningInsideExpr = false;
            return curly;
        }

        private IToken ESCAPE()
        {
            Consume(); // kill \\
            IToken t = null;
            switch (c)
            {
            case '\\':
                mLINEBREAK();
                return SKIP;
            case 'n':
                t = NewToken(TEXT, "\n", input.CharPositionInLine - 2);
                break;
            case 't':
                t = NewToken(TEXT, "\t", input.CharPositionInLine - 2);
                break;
            case ' ':
                t = NewToken(TEXT, " ", input.CharPositionInLine - 2);
                break;
            case 'u':
                t = UNICODE();
                break;
            default:
                Console.Error.WriteLine("bad \\ char");
                break;
            }
            Consume();
            Match(delimiterStopChar);
            return t;
        }

        private IToken UNICODE()
        {
            Consume();
            char[] chars = new char[4];
            if (!IsUnicodeLetter(c))
                Console.Error.WriteLine("bad unicode char: " + c);
            chars[0] = c;
            Consume();
            if (!IsUnicodeLetter(c))
                Console.Error.WriteLine("bad unicode char: " + c);
            chars[1] = c;
            Consume();
            if (!IsUnicodeLetter(c))
                Console.Error.WriteLine("bad unicode char: " + c);
            chars[2] = c;
            Consume();
            if (!IsUnicodeLetter(c))
                Console.Error.WriteLine("bad unicode char: " + c);
            chars[3] = c;
            // ESCAPE kills final char and >
            char uc = (char)int.Parse(new string(chars), NumberStyles.HexNumber);
            return NewToken(TEXT, uc.ToString(), input.CharPositionInLine - 6);
        }

        private IToken mTEXT()
        {
            bool modifiedText = false;
            StringBuilder buf = new StringBuilder();
            while (c != EOF && c != delimiterStartChar)
            {
                if (c == '\r' || c == '\n')
                    break;
                if (c == '}' && subtemplateDepth > 0)
                    break;
                if (c == '\\')
                {
                    if (input.LA(2) == delimiterStartChar ||
                         input.LA(2) == '}')
                    {
                        modifiedText = true;
                        Consume(); // toss out \ char
                        buf.Append(c);
                        Consume();
                    }
                    else
                    {
                        Consume();
                    }
                    continue;
                }
                buf.Append(c);
                Consume();
            }
            if (modifiedText)
                return NewToken(TEXT, buf.ToString());
            else
                return NewToken(TEXT);
        }

        /** ID  :   ('a'..'z'|'A'..'Z'|'_'|'/') ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|'/')* ; */
        private IToken mID()
        {
            // called from subTemplate; so keep resetting position during speculation
            startCharIndex = input.Index;
            startLine = input.Line;
            startCharPositionInLine = input.CharPositionInLine;
            Consume();
            while (IsIDLetter(c))
            {
                Consume();
            }
            return NewToken(ID);
        }

        /** STRING : '"' ( '\\' '"' | '\\' ~'"' | ~('\\'|'"') )* '"' ; */
        private IToken mSTRING()
        {
            //{setText(getText().substring(1, getText().length()-1));}
            bool sawEscape = false;
            StringBuilder buf = new StringBuilder();
            buf.Append(c);
            Consume();
            while (c != '"')
            {
                if (c == '\\')
                {
                    sawEscape = true;
                    Consume();
                    switch (c)
                    {
                    case 'n':
                        buf.Append('\n');
                        break;
                    case 'r':
                        buf.Append('\r');
                        break;
                    case 't':
                        buf.Append('\t');
                        break;
                    default:
                        buf.Append(c);
                        break;
                    }
                    Consume();
                    continue;
                }
                buf.Append(c);
                Consume();
            }
            buf.Append(c);
            Consume();
            if (sawEscape)
                return NewToken(STRING, buf.ToString());
            else
                return NewToken(STRING);
        }

        private void mWS()
        {
            while (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                Consume();
        }

        private void mCOMMENT()
        {
            Match('!');
            while (!(c == '!' && input.LA(2) == delimiterStopChar))
                Consume();
            Consume();
            Consume(); // kill !>
        }

        private void mLINEBREAK()
        {
            Match('\\'); // only kill 2nd \ as outside() kills first one
            Match(delimiterStopChar);
            while (c == ' ' || c == '\t')
                Consume(); // scarf WS after <\\>
            if (c == '\r')
                Consume();
            Match('\n');
            while (c == ' ' || c == '\t')
                Consume(); // scarf any indent
            return;
        }

        public static bool IsIDStartLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '/';
        }

        public static bool IsIDLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '/';
        }

        public static bool IsWS(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        public static bool IsUnicodeLetter(char c)
        {
            return c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F' || c >= '0' && c <= '9';
        }

        public IToken NewToken(int ttype)
        {
            TemplateToken t = new TemplateToken(input, ttype, Lexer.DefaultTokenChannel,
                    startCharIndex, input.Index - 1);
            t.Line = startLine;
            t.CharPositionInLine = startCharPositionInLine;
            return t;
        }

        public IToken NewTokenFromPreviousChar(int ttype)
        {
            TemplateToken t =
                new TemplateToken(input, ttype, Lexer.DefaultTokenChannel,
                    input.Index - 1, input.Index - 1);
            t.StartIndex = input.Index - 1;
            t.Line = input.Line;
            t.CharPositionInLine = input.CharPositionInLine - 1;
            return t;
        }

        public IToken NewToken(int ttype, string text, int pos)
        {
            TemplateToken t = new TemplateToken(ttype, text);
            t.Line = input.Line;
            t.CharPositionInLine = pos;
            return t;
        }

        public IToken NewToken(int ttype, string text)
        {
            TemplateToken t = new TemplateToken(ttype, text);
            t.StartIndex = startCharIndex;
            t.Line = startLine;
            t.CharPositionInLine = startCharPositionInLine;
            return t;
        }

        public string SourceName
        {
            get
            {
                return "no idea";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return TemplateParser.tokenNames;
            }
        }
    }
}
