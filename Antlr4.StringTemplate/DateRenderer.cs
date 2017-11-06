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
    using System;
    using System.Collections.Generic;
    using CultureInfo = System.Globalization.CultureInfo;

    /** A renderer for java.util.Date and Calendar objects. It understands a
     *  variety of format names as shown in formatToInt field.  By default
     *  it assumes "short" format.  A prefix of date: or time: shows only
     *  those components of the time object.
     */
    public class DateRenderer : IAttributeRenderer
    {
        public static readonly IDictionary<string, string> formatToInt =
            new Dictionary<string, string>() {
                {"short", "g"},
                {"medium", "g"},
                {"long", "f"},
                {"full", "F"},

                {"date:short", "d"},
                {"date:medium", "d"},
                {"date:long", "D"},
                {"date:full", "D"},

                {"time:short", "t"},
                {"time:medium", "t"},
                {"time:long", "T"},
                {"time:full", "T"},
            };

        public virtual string ToString(object o, string formatString, CultureInfo locale)
        {
            if (formatString == null)
                formatString = "short";

            DateTimeOffset d;
            if (o is DateTime)
                d = (DateTime)o;
            else if (o is DateTimeOffset)
                d = (DateTimeOffset)o;
            else
                throw new ArgumentException();

            string dateFormat;
            if (!formatToInt.TryGetValue(formatString, out dateFormat))
                return d.ToString(formatString, locale);

            return d.ToString(dateFormat, locale);
        }
    }
}
