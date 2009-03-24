/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.ST.Language
{
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;

    /** <summary>
     *  Represents the name of a formal argument
     *  defined in a template:
     *  </summary>
     *
     *  <remarks>
     *  group test;
     *  test(a,b) : "$a$ $b$"
     *  t() : "blort"
     *
     *  Each template has a set of these formal arguments or uses
     *  a placeholder object: UNKNOWN (indicating that no arguments
     *  were specified such as when a template is loaded from a file.st).
     *
     *  Note: originally, I tracked cardinality as well as the name of an
     *  attribute.  I'm leaving the code here as I suspect something may come
     *  of it later.  Currently, though, cardinality is not used.
     *  </remarks>
     */
    public class FormalArgument
    {
        // the following represent bit positions emulating a cardinality bitset.
        public const int OPTIONAL = 1;     // a?
        public const int REQUIRED = 2;     // a
        public const int ZERO_OR_MORE = 4; // a*
        public const int ONE_OR_MORE = 8;  // a+

        public static readonly string[] suffixes =
            {
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

        /** <summary>
         *  When template arguments are not available such as when the user
         *  uses "new StringTemplate(...)", then the list of formal arguments
         *  must be distinguished from the case where a template can specify
         *  args and there just aren't any such as the t() template above.
         *  </summary>
         */
        public static readonly IList<FormalArgument> UNKNOWN = new FormalArgument[0];

        public string name;
        //protected int cardinality = REQUIRED;

        /** <summary>If they specified name="value", store the template here</summary> */
        public StringTemplate defaultValueST;

        public FormalArgument( string name )
            : this( name, null )
        {
        }

        public FormalArgument( string name, StringTemplate defaultValueST )
        {
            this.name = name;
            this.defaultValueST = defaultValueST;
        }

        public static string GetCardinalityName( int cardinality )
        {
            switch ( cardinality )
            {
            case OPTIONAL:
                return "optional";
            case REQUIRED:
                return "exactly one";
            case ZERO_OR_MORE:
                return "zero-or-more";
            case ONE_OR_MORE:
                return "one-or-more";
            default:
                return "unknown";
            }
        }

        public override bool Equals( object o )
        {
            FormalArgument other = o as FormalArgument;
            if ( other == null )
                return false;

            if ( !this.name.Equals( other.name ) )
            {
                return false;
            }
            // only check if there is a default value; that's all
            if ( ( this.defaultValueST != null && other.defaultValueST == null ) ||
                 ( this.defaultValueST == null && other.defaultValueST != null ) )
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.ShiftPrimeXOR(
                ( name != null ) ? name.GetHashCode() : 0,
                ( defaultValueST != null ) ? defaultValueST.GetHashCode() : 0
                );
        }

        public override string ToString()
        {
            if ( defaultValueST != null )
            {
                return name + "=" + defaultValueST;
            }
            return name;
        }
    }

}
