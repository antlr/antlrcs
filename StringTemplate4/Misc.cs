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
    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

    internal static class Misc
    {
        public static string Strip(string s, int n)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (n < 0)
                throw new ArgumentOutOfRangeException("n");
            if (s.Length - 2 * n < 0)
                throw new ArgumentException();

            return s.Substring(n, s.Length - 2 * n);
        }

        public static string TrimOneStartingWS(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            // strip newline from front and back, but just one
            if (s.StartsWith("\r\n"))
                s = s.Substring(2);
            else if (s.StartsWith("\n"))
                s = s.Substring(1);
            /*
            if ( s.endsWith("\r\n") ) s = s.substring(0,s.length()-2);
            else if ( s.endsWith("\n") ) s = s.substring(0,s.length()-1);
             */
            return s;
        }

        public static string ReplaceEscapes(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            s = s.Replace("\n", @"\\n");
            s = s.Replace("\r", @"\\r");
            s = s.Replace("\t", @"\\t");
            return s;
        }

        /// <summary>
        /// Given index into string, compute the line and char position in line
        /// </summary>
        public static Coordinate GetLineCharPosition(string s, int index)
        {
            int line = 1;
            int charPos = 0;
            int p = 0;
            while (p < index)
            {
                // don't care about s[index] itself; count before
                if (s[p] == '\n')
                {
                    line++;
                    charPos = 0;
                }
                else
                {
                    charPos++;
                }

                p++;
            }

            return new Coordinate(line, charPos);
        }
    }
}
