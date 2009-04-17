/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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

namespace Antlr3.Tool
{
    using Antlr.Runtime.JavaExtensions;

    /** Track the names of attributes define in arg lists, return values,
     *  scope blocks etc...
     */
    public class Attribute
    {
        /** The entire declaration such as "String foo;" */
        public string Decl
        {
            get;
            set;
        }

        /** The type; might be empty such as for Python which has no static typing */
        public string Type
        {
            get;
            set;
        }

        /** The name of the attribute "foo" */
        public string Name
        {
            get;
            set;
        }

        /** The optional attribute intialization expression */
        public string InitValue
        {
            get;
            set;
        }

        public Attribute( string decl )
        {
            ExtractAttribute( decl );
        }

        public Attribute( string name, string decl )
        {
            this.Name = name;
            this.Decl = decl;
        }

        /** For decls like "String foo" or "char *foo32[3]" compute the ID
         *  and type declarations.  Also handle "int x=3" and 'T t = new T("foo")'
         *  but if the separator is ',' you cannot use ',' in the initvalue.
         *  AttributeScope.addAttributes takes care of the separation so we are
         *  free here to use from '=' to end of string as the expression.
         *
         *  Set name, type, initvalue, and full decl instance vars.
         */
        protected virtual void ExtractAttribute( string decl )
        {
            if ( decl == null )
            {
                return;
            }
            bool inID = false;
            int start = -1;
            int rightEdgeOfDeclarator = decl.Length - 1;
            int equalsIndex = decl.IndexOf( '=' );
            if ( equalsIndex > 0 )
            {
                // everything after the '=' is the init value
                this.InitValue = decl.Substring( equalsIndex + 1 );
                rightEdgeOfDeclarator = equalsIndex - 1;
            }
            // walk backwards looking for start of an ID
            for ( int i = rightEdgeOfDeclarator; i >= 0; i-- )
            {
                // if we haven't found the end yet, keep going
                if ( !inID && char.IsLetterOrDigit( decl[i] ) )
                {
                    inID = true;
                }
                else if ( inID &&
                          !( char.IsLetterOrDigit( decl[i] ) ||
                           decl[i] == '_' ) )
                {
                    start = i + 1;
                    break;
                }
            }
            if ( start < 0 && inID )
            {
                start = 0;
            }
            if ( start < 0 )
            {
                ErrorManager.Error( ErrorManager.MSG_CANNOT_FIND_ATTRIBUTE_NAME_IN_DECL, decl );
            }
            // walk forwards looking for end of an ID
            int stop = -1;
            for ( int i = start; i <= rightEdgeOfDeclarator; i++ )
            {
                // if we haven't found the end yet, keep going
                if ( !( char.IsLetterOrDigit( decl[i] ) ||
                    decl[i] == '_' ) )
                {
                    stop = i;
                    break;
                }
                if ( i == rightEdgeOfDeclarator )
                {
                    stop = i + 1;
                }
            }

            // the name is the last ID
            this.Name = decl.Substring( start, stop - start );

            // the type is the decl minus the ID (could be empty)
            this.Type = decl.Substring( 0, start );
            if ( stop <= rightEdgeOfDeclarator )
            {
                this.Type += decl.Substring( stop, rightEdgeOfDeclarator + 1 - stop );
            }
            this.Type = Type.Trim();
            if ( this.Type.Length == 0 )
            {
                this.Type = null;
            }

            this.Decl = decl;
        }

        public override string ToString()
        {
            if ( InitValue != null )
            {
                return Type + " " + Name + "=" + InitValue;
            }
            return Type + " " + Name;
        }
    }
}
