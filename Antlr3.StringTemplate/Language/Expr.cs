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
    /** <summary>
     *  A string template expression embedded within the template.
     *  A template is parsed into a tokenized vector of Expr objects
     *  and then executed after the user sticks in attribute values.
     *  </summary>
     *
     *  <remarks>
     *  This list of Expr objects represents a "program" for the StringTemplate
     *  evaluator.
     *  </remarks>
     */
    public abstract class Expr
    {
        /** <summary>The StringTemplate object surrounding this expr</summary> */
        StringTemplate _enclosingTemplate;

        /** <summary>
         *  Anything spit out as a chunk (even plain text) must be indented
         *  according to whitespace before the action that generated it.  So,
         *  plain text in the outermost template is never indented, but the
         *  text and attribute references in a nested template will all be
         *  indented by the amount seen directly in front of the attribute
         *  reference that initiates construction of the nested template.
         *  </summary>
         */
        string _indentation;

        public Expr( StringTemplate enclosingTemplate )
        {
            _enclosingTemplate = enclosingTemplate;
        }

        /** <summary>How to write this node to output; return how many char written</summary> */
        abstract public int write( StringTemplate self, IStringTemplateWriter writer );

        public StringTemplate EnclosingTemplate
        {
            get
            {
                return _enclosingTemplate;
            }
        }

        public string Indentation
        {
            get
            {
                return _indentation;
            }
            set
            {
                _indentation = value;
            }
        }
    }
}
