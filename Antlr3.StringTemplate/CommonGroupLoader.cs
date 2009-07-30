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

namespace Antlr3.ST
{
    using System;
    using System.Linq;

    using TextReader = System.IO.TextReader;

    /** <summary>
     *  A simple loader that looks only in the directory(ies) you
     *  specify in the ctor, but it uses the classpath rather than
     *  absolute dirs so it can be used when the ST application is jar'd up.
     *  You may specify the char encoding.
     *  </summary>
     */
    public class CommonGroupLoader : PathGroupLoader
    {

        public CommonGroupLoader( IStringTemplateErrorListener errors )
            : base( errors )
        {
        }

        /** <summary>
         *  Pass a single dir or multiple dirs separated by colons from which
         *  to load groups/interfaces.  These are interpreted as relative
         *  paths to be used with CLASSPATH to locate groups.  E.g.,
         *  If you pass in "org/antlr/codegen/templates" and ask to load
         *  group "foo" it will try to load via classpath as
         *  "org/antlr/codegen/templates/foo".
         *  </summary>
         */
        public CommonGroupLoader( string dirStr, IStringTemplateErrorListener errors )
            : base( dirStr, errors )
        {
        }

        /** <summary>
         *  Look in each relative directory for the file called 'name'.
         *  </summary>
         */
        protected override TextReader Locate( string name )
        {
            // check for templates on disk first
            foreach ( string dir in Directories )
            {
                string fileName = System.IO.Path.Combine( dir, name );
                if ( System.IO.File.Exists( fileName ) )
                    return new System.IO.StreamReader( fileName );
            }

            var assemblies = ( from frame in ( new System.Diagnostics.StackTrace().GetFrames() )
                               select frame.GetMethod().DeclaringType.Assembly )
                             .Distinct();

            foreach (string dir in Directories)
            {
                string fileName = dir + "." + name;

                System.IO.Stream @is =
                    assemblies
                    .Select( assembly => assembly.GetManifestResourceStream( fileName ) )
                    .Where( stream => stream != null )
                    .FirstOrDefault();

                if ( @is == null )
                {
                    @is = GetType().Assembly.GetManifestResourceStream( fileName );
                }

                if ( @is != null )
                {
                    return new System.IO.StreamReader( new System.IO.BufferedStream( @is ) );
                }
            }

            return null;
        }

    }
}
