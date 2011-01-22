/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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

namespace Antlr4.StringTemplate.Compiler
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Misc;
    using Exception = System.Exception;
    using NumberStyles = System.Globalization.NumberStyles;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparison = System.StringComparison;

    /** This class represents the tokenizer for templates. It operates in two modes:
     *  inside and outside of expressions. It behaves like an ANTLR TokenSource,
     *  implementing nextToken().  Outside of expressions, we can return these
     *  token types: TEXT, INDENT, LDELIM (start of expr), RCURLY (end of subtemplate),
     *  and NEWLINE.  Inside of an expression, this lexer returns all of the tokens
     *  needed by the STParser. From the parser's point of view, it can treat a
     *  template as a simple stream of elements.
     *
     *  This class defines the token types and communicates these values to STParser.g
     *  via STLexer.tokens file (which must remain consistent).
     */
    public class STLexer : ITokenSource
    {
        public const char EOF = char.MaxValue;            // EOF char
        public const int EOF_TYPE = CharStreamConstants.EndOfFile;  // EOF token type

        /** We build STToken tokens instead of relying on CommonToken so we
         *  can override toString(). It just converts token types to
         *  token names like 23 to LDELIM.
         */
        public class STToken : CommonToken
        {
            public STToken(ICharStream input, int type, int start, int stop)
                : base(input, type, TokenChannels.Default, start, stop)
            {
            }

            public STToken(int type, string text)
                : base(type, text)
            {
            }

            public override string ToString()
            {
                string channelStr = "";
                if (Channel > 0)
                    channelStr = ",channel=" + Channel;

                string txt = Text;
                if (txt != null)
                    txt = Utility.replaceEscapes(txt);
                else
                    txt = "<no text>";

                return "[@" + TokenIndex + "," + StartIndex + ":" + StopIndex + "='" + txt + "',<" + TemplateParser.tokenNames[Type] + ">" + channelStr + "," + Line + ":" + CharPositionInLine + "]";
            }
        }

        public static readonly IToken SKIP = new STToken(-1, "<skip>");

        // must follow STLexer.tokens file that STParser.g loads
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

        /** What char starts an expression? */
        char delimiterStartChar = '<';
        char delimiterStopChar = '>';

        /** This keep track of the mode of the lexer. Are we inside or outside
         *  an ST expression?
         */
        bool scanningInsideExpr = false;

        /** To be able to properly track the inside/outside mode, we need to
         *  track how deeply nested we are in some templates. Otherwise, we
         *  know whether a '}' and the outermost subtemplate to send this back to
         *  outside mode.
         */
        public int subtemplateDepth = 0; // start out *not* in a {...} subtemplate

        ErrorManager errMgr;

        IToken templateToken; // template embedded in a group file? this is the template

        ICharStream input;
        char c;        // current character

        /** When we started token, track initial coordinates so we can properly
         *  build token objects.
         */
        int startCharIndex;
        int startLine;
        int startCharPositionInLine;

        /** Our lexer routines might have to emit more than a single token. We
         *  buffer everything through this list.
         */
        private readonly Queue<IToken> tokens = new Queue<IToken>();

        public STLexer(ICharStream input) : this(STGroup.DEFAULT_ERR_MGR, input, null, '<', '>')
        {
        }

        public STLexer(ErrorManager errMgr, ICharStream input, IToken templateToken)
            : this(errMgr, input, templateToken, '<', '>')
        {
        }

        public STLexer(ErrorManager errMgr,
                       ICharStream input,
                       IToken templateToken,
                       char delimiterStartChar,
                       char delimiterStopChar)
        {
            this.errMgr = errMgr;
            this.input = input;
            c = (char)input.LA(1); // prime lookahead
            this.templateToken = templateToken;
            this.delimiterStartChar = delimiterStartChar;
            this.delimiterStopChar = delimiterStopChar;
        }

        public virtual IToken NextToken()
        {
            if (tokens.Count > 0)
                return tokens.Dequeue();

            return _nextToken();
        }

        /** Ensure x is next character on the input stream */
        public virtual void match(char x)
        {
            if (c == x)
                consume();
            else
                throw new Exception(getErrorHeader() + ": expecting " + x + "; found " + c);
        }

        protected virtual void consume()
        {
            input.Consume();
            c = (char)input.LA(1);
        }

        public virtual void emit(IToken token)
        {
            tokens.Enqueue(token);
        }

        public virtual IToken _nextToken()
        {
            //System.out.println("nextToken: c="+(char)c+"@"+input.index());
            while (true)
            { // lets us avoid recursion when skipping stuff
                startCharIndex = input.Index;
                startLine = input.Line;
                startCharPositionInLine = input.CharPositionInLine;

                if (c == EOF)
                    return newToken(EOF_TYPE);

                IToken t;
                if (scanningInsideExpr)
                    t = inside();
                else
                    t = outside();

                if (t != SKIP)
                    return t;
            }
        }

        protected virtual IToken outside()
        {
            if (input.CharPositionInLine == 0 && (c == ' ' || c == '\t'))
            {
                while (c == ' ' || c == '\t')
                    consume(); // scarf indent

                if (c != EOF)
                    return newToken(INDENT);

                return newToken(TEXT);
            }

            if (c == delimiterStartChar)
            {
                consume();
                if (c == '!')
                {
                    COMMENT();
                    return SKIP;
                }

                if (c == '\\')
                    return ESCAPE(); // <\\> <\uFFFF> <\n> etc...

                scanningInsideExpr = true;
                return newToken(LDELIM);
            }

            if (c == '\r')
            {
                consume();
                consume();
                return newToken(NEWLINE);
            } // \r\n -> \n

            if (c == '\n')
            {
                consume();
                return newToken(NEWLINE);
            }

            if (c == '}' && subtemplateDepth > 0)
            {
                scanningInsideExpr = true;
                subtemplateDepth--;
                consume();
                return newTokenFromPreviousChar(RCURLY);
            }

            return mTEXT();
        }

        protected virtual IToken inside()
        {
            while (true)
            {
                switch (c)
                {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    consume();
                    continue;

                case '.':
                    consume();
                    if (input.LA(1) == '.' && input.LA(2) == '.')
                    {
                        consume();
                        match('.');
                        return newToken(ELLIPSIS);
                    }
                    return newToken(DOT);

                case ',':
                    consume();
                    return newToken(COMMA);

                case ':':
                    consume();
                    return newToken(COLON);

                case ';':
                    consume();
                    return newToken(SEMI);

                case '(':
                    consume();
                    return newToken(LPAREN);

                case ')':
                    consume();
                    return newToken(RPAREN);

                case '[':
                    consume();
                    return newToken(LBRACK);

                case ']':
                    consume();
                    return newToken(RBRACK);

                case '=':
                    consume();
                    return newToken(EQUALS);

                case '!':
                    consume();
                    return newToken(BANG);

                case '@':
                    consume();
                    if (c == 'e' && input.LA(2) == 'n' && input.LA(3) == 'd')
                    {
                        consume();
                        consume();
                        consume();
                        return newToken(REGION_END);
                    }
                    return newToken(AT);

                case '"':
                    return mSTRING();

                case '&':
                    consume();
                    match('&');
                    return newToken(AND); // &&

                case '|':
                    consume();
                    match('|');
                    return newToken(OR); // ||

                case '{':
                    return subTemplate();

                default:
                    if (c == delimiterStopChar)
                    {
                        consume();
                        scanningInsideExpr = false;
                        return newToken(RDELIM);
                    }

                    if (isIDStartLetter(c))
                    {
                        IToken id = mID();
                        string name = id.Text;

                        if (name.Equals("if", StringComparison.Ordinal))
                            return newToken(IF);
                        else if (name.Equals("endif", StringComparison.Ordinal))
                            return newToken(ENDIF);
                        else if (name.Equals("else", StringComparison.Ordinal))
                            return newToken(ELSE);
                        else if (name.Equals("elseif", StringComparison.Ordinal))
                            return newToken(ELSEIF);
                        else if (name.Equals("super", StringComparison.Ordinal))
                            return newToken(SUPER);

                        return id;
                    }

                    RecognitionException re = new NoViableAltException("", 0, 0, input);
                    re.Line = startLine;
                    re.CharPositionInLine = startCharPositionInLine;
                    if (c == EOF)
                    {
                        throw new STException("EOF inside ST expression at " + re.Line + ":" + re.CharPositionInLine, re);
                    }

                    errMgr.lexerError(input.SourceName, "invalid character '" + c + "'", templateToken, re);
                    consume();
                    break;
                }
            }
        }

        private IToken subTemplate()
        {
            // look for "{ args ID (',' ID)* '|' ..."
            subtemplateDepth++;
            int m = input.Mark();
            int curlyStartChar = startCharIndex;
            int curlyLine = startLine;
            int curlyPos = startCharPositionInLine;
            List<IToken> argTokens = new List<IToken>();
            consume();
            IToken curly = newTokenFromPreviousChar(LCURLY);
            WS();
            argTokens.Add(mID());
            WS();
            while (c == ',')
            {
                consume();
                argTokens.Add(newTokenFromPreviousChar(COMMA));
                WS();
                argTokens.Add(mID());
                WS();
            }

            WS();
            if (c == '|')
            {
                consume();
                argTokens.Add(newTokenFromPreviousChar(PIPE));
                if (isWS(c))
                    consume(); // ignore a single whitespace after |

                //System.out.println("matched args: "+argTokens);
                foreach (IToken t in argTokens)
                    emit(t);

                input.Release(m);
                scanningInsideExpr = false;
                startCharIndex = curlyStartChar; // reset state
                startLine = curlyLine;
                startCharPositionInLine = curlyPos;
                return curly;
            }

            input.Rewind(m);
            startCharIndex = curlyStartChar; // reset state
            startLine = curlyLine;
            startCharPositionInLine = curlyPos;
            consume();
            scanningInsideExpr = false;
            return curly;
        }

        private IToken ESCAPE()
        {
            consume(); // kill \\
            IToken t = null;
            switch (c)
            {
            case '\\':
                LINEBREAK();
                return SKIP;

            case 'n':
                t = newToken(TEXT, "\n", input.CharPositionInLine - 2);
                break;

            case 't':
                t = newToken(TEXT, "\t", input.CharPositionInLine - 2);
                break;

            case ' ':
                t = newToken(TEXT, " ", input.CharPositionInLine - 2);
                break;

            case 'u':
                t = UNICODE();
                break;

            default:
                NoViableAltException e = new NoViableAltException("", 0, 0, input);
                errMgr.lexerError(input.SourceName, "invalid escaped char: '" + c + "'", templateToken, e);
                break;
            }
            consume();
            match(delimiterStopChar);
            return t;
        }

        private IToken UNICODE()
        {
            consume();
            char[] chars = new char[4];
            if (!isUnicodeLetter(c))
            {
                NoViableAltException e = new NoViableAltException("", 0, 0, input);
                errMgr.lexerError(input.SourceName, "invalid unicode char: '" + c + "'", templateToken, e);
            }

            chars[0] = c;
            consume();
            if (!isUnicodeLetter(c))
            {
                NoViableAltException e = new NoViableAltException("", 0, 0, input);
                errMgr.lexerError(input.SourceName, "invalid unicode char: '" + c + "'", templateToken, e);
            }

            chars[1] = c;
            consume();
            if (!isUnicodeLetter(c))
            {
                NoViableAltException e = new NoViableAltException("", 0, 0, input);
                errMgr.lexerError(input.SourceName, "invalid unicode char: '" + c + "'", templateToken, e);
            }

            chars[2] = c;
            consume();
            if (!isUnicodeLetter(c))
            {
                NoViableAltException e = new NoViableAltException("", 0, 0, input);
                errMgr.lexerError(input.SourceName, "invalid unicode char: '" + c + "'", templateToken, e);
            }

            chars[3] = c;
            // ESCAPE kills final char and >
            char uc = (char)int.Parse(new string(chars), NumberStyles.HexNumber);
            return newToken(TEXT, uc.ToString(), input.CharPositionInLine - 6);
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
                    if (input.LA(2) == '\\')
                    {
                        // convert \\ to \
                        consume();
                        consume();
                        buf.Append('\\');
                        modifiedText = true;
                        continue;
                    }

                    if (input.LA(2) == delimiterStartChar || input.LA(2) == '}')
                    {
                        modifiedText = true;
                        // toss out \ char
                        consume();
                        buf.Append(c);
                        consume();
                    }
                    else
                    {
                        buf.Append(c);
                        consume();
                    }

                    continue;
                }

                buf.Append(c);
                consume();
            }

            if (modifiedText)
                return newToken(TEXT, buf.ToString());
            else
                return newToken(TEXT);
        }

        /** ID  :   ('a'..'z'|'A'..'Z'|'_'|'/') ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|'/')* ; */
        private IToken mID()
        {
            // called from subTemplate; so keep resetting position during speculation
            startCharIndex = input.Index;
            startLine = input.Line;
            startCharPositionInLine = input.CharPositionInLine;
            consume();
            while (isIDLetter(c))
            {
                consume();
            }
            return newToken(ID);
        }

        /** STRING : '"' ( '\\' '"' | '\\' ~'"' | ~('\\'|'"') )* '"' ; */
        private IToken mSTRING()
        {
            //{setText(getText().substring(1, getText().length()-1));}
            bool sawEscape = false;
            StringBuilder buf = new StringBuilder();
            buf.Append(c);
            consume();
            while (c != '"')
            {
                if (c == '\\')
                {
                    sawEscape = true;
                    consume();
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

                    consume();
                    continue;
                }

                buf.Append(c);
                consume();
                if (c == EOF)
                {
                    RecognitionException re = new MismatchedTokenException((int)'"', input);
                    re.Line = input.Line;
                    re.CharPositionInLine = input.CharPositionInLine;
                    throw new STException("EOF inside string/template at " + startLine + ":" + startCharPositionInLine, re);
                }
            }

            buf.Append(c);
            consume();

            if (sawEscape)
                return newToken(STRING, buf.ToString());
            else
                return newToken(STRING);
        }

        private void WS()
        {
            while (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                consume();
        }

        private void COMMENT()
        {
            match('!');
            while (!(c == '!' && input.LA(2) == delimiterStopChar))
                consume();
            consume();
            consume(); // kill !>
        }

        private void LINEBREAK()
        {
            match('\\'); // only kill 2nd \ as outside() kills first one
            match(delimiterStopChar);
            while (c == ' ' || c == '\t')
                consume(); // scarf WS after <\\>
            if (c == '\r')
                consume();
            match('\n');
            while (c == ' ' || c == '\t')
                consume(); // scarf any indent
        }

        public static bool isIDStartLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
        }

        public static bool isIDLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '/';
        }

        public static bool isWS(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        public static bool isUnicodeLetter(char c)
        {
            return c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F' || c >= '0' && c <= '9';
        }

        public virtual IToken newToken(int ttype)
        {
            STToken t = new STToken(input, ttype, startCharIndex, input.Index - 1);
            t.Line = startLine;
            t.CharPositionInLine = startCharPositionInLine;
            return t;
        }

        public virtual IToken newTokenFromPreviousChar(int ttype)
        {
            STToken t = new STToken(input, ttype, input.Index - 1, input.Index - 1);
            t.Line = input.Line;
            t.CharPositionInLine = input.CharPositionInLine - 1;
            return t;
        }

        public virtual IToken newToken(int ttype, string text, int pos)
        {
            STToken t = new STToken(ttype, text);
            t.Line = input.Line;
            t.CharPositionInLine = pos;
            return t;
        }

        public virtual IToken newToken(int ttype, string text)
        {
            STToken t = new STToken(ttype, text);
            t.StartIndex = startCharIndex;
            t.StopIndex = input.Index - 1;
            t.Line = startLine;
            t.CharPositionInLine = startCharPositionInLine;
            return t;
        }

        public virtual string getErrorHeader()
        {
            return startLine + ":" + startCharPositionInLine;
        }

        public virtual string SourceName
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
