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

namespace Antlr4.StringTemplate
{
    using StringBuilder = System.Text.StringBuilder;
    using CultureInfo = System.Globalization.CultureInfo;
    using NotImplementedException = System.NotImplementedException;

    /** This render knows to perform a few operations on String objects:
     *  upper, lower, cap, url-encode, xml-encode.
     */
    public class StringRenderer : AttributeRenderer
    {
        // trim(s) and strlen(s) built-in funcs; these are format options
        public virtual string toString(object o, string formatString, CultureInfo locale)
        {
            string s = (string)o;
            if (formatString == null)
                return s;

            if (formatString.Equals("upper"))
                return s.ToUpper(locale);

            if (formatString.Equals("lower"))
                return s.ToLower(locale);

            if (formatString.Equals("cap"))
                return char.ToUpper(s[0], locale) + s.Substring(1);

            if (formatString.Equals("url-encode"))
                throw new NotImplementedException();
#if false
            if (formatString.Equals("url-encode"))
                return URLEncoder.encode(s);
#endif

            if (formatString.Equals("xml-encode"))
                return escapeHTML(s);

            return string.Format(formatString, s);
        }

        public static string escapeHTML(string s)
        {
            if (s == null)
                return null;

            StringBuilder buf = new StringBuilder(s.Length);
            int len = s.Length;
            for (int i = 0; i < len; i++)
            {
                char c = s[i];
                switch (c)
                {
                case '&':
                    buf.Append("&amp;");
                    break;

                case '<':
                    buf.Append("&lt;");
                    break;

                case '>':
                    buf.Append("&gt;");
                    break;

                case '\r':
                case '\n':
                case '\t':
                    buf.Append(c);
                    break;

                default:
                    bool control = c < ' '; // 32
                    bool aboveASCII = c > 126;
                    if (control || aboveASCII)
                    {
                        buf.Append("&#");
                        buf.Append((int)c);
                        buf.Append(";");
                    }
                    else
                    {
                        buf.Append(c);
                    }
                    break;
                }
            }

            return buf.ToString();
        }
    }
}
