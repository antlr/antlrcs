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

namespace StringTemplate.Compiler
{
    using Antlr.Runtime;
    using System.Collections.Generic;

    public class FormalArgument
    {
        // the following represent bit positions emulating a cardinality bitset.
#if false
        public static readonly int OPTIONAL = 1;     // a?
        public static readonly int REQUIRED = 2;     // a
        public static readonly int ZERO_OR_MORE = 4; // a*
        public static readonly int ONE_OR_MORE = 8;  // a+

        public static readonly string[] suffixes = {
        null,
        "?",
        "",
        null,
        "*",
        null,
        null,
        null,
        "+"
    };
        //protected int cardinality = REQUIRED;
#endif

        /** When template arguments are not available such as when the user
         *  uses "new ST(...)", then the list of formal arguments
         *  must be distinguished from the case where a template can specify
         *  args and there just aren't any such as the t() template above.
         */
        public static readonly IDictionary<string, FormalArgument> Unknown = new Dictionary<string, FormalArgument>();

        public string name;

        /** If they specified name="value", store the template here */
        public IToken defaultValueToken;
        public CompiledTemplate compiledDefaultValue;

        public FormalArgument(string name)
        {
            this.name = name;
        }

        public FormalArgument(string name, IToken defaultValueToken)
        {
            this.name = name;
            this.defaultValueToken = defaultValueToken;
        }

        /*
        public static String getCardinalityName(int cardinality) {
            switch (cardinality) {
                case OPTIONAL : return "optional";
                case REQUIRED : return "exactly one";
                case ZERO_OR_MORE : return "zero-or-more";
                case ONE_OR_MORE : return "one-or-more";
                default : return "unknown";
            }
        }
        */

        public override int GetHashCode()
        {
            return name.GetHashCode() + defaultValueToken.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is FormalArgument))
            {
                return false;
            }
            FormalArgument other = (FormalArgument)o;
            if (!this.name.Equals(other.name))
            {
                return false;
            }
            // only check if there is a default value; that's all
            if ((this.defaultValueToken != null && other.defaultValueToken == null) ||
                 (this.defaultValueToken == null && other.defaultValueToken != null))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            if (defaultValueToken != null)
            {
                return name + "=" + defaultValueToken.Text;
            }
            return name;
        }
    }
}
