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
    using System.Collections.ObjectModel;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.ST.Language;

    using Encoding = System.Text.Encoding;
    using IOException = System.IO.IOException;
    using Stream = System.IO.Stream;
    using StreamReader = System.IO.StreamReader;
    using TextReader = System.IO.TextReader;
    using Path = System.IO.Path;

    /** <summary>
     *  A brain dead loader that looks only in the directory(ies) you
     *  specify in the ctor.
     *  </summary>
     *
     *  <remarks>
     *  You may specify the char encoding.
     *  NOTE: this does not work when you jar things up!  Use
     *  CommonGroupLoader instead in that case
     *  </remarks>
     */
    public class PathGroupLoader : IStringTemplateGroupLoader
    {
        private IStringTemplateErrorListener _errors = null;

        /** <summary>
         *  How are the files encoded (ascii, UTF8, ...)?  You might want to read
         *  UTF8 for example on an ascii machine.
         *  </summary>
         */
        private Encoding _fileCharEncoding = Encoding.Default;

        public PathGroupLoader( IStringTemplateErrorListener errors )
        {
            _errors = errors;
        }

        /** <summary>
         *  Pass a single dir or multiple dirs separated by colons from which
         *  to load groups/interfaces.
         *  </summary>
         */
        public PathGroupLoader( string dirStr, IStringTemplateErrorListener errors )
        {
            _errors = errors;
            Directories = new ReadOnlyCollection<string>(dirStr.Split(':'));
        }

        /** <summary>Gets a list of directories to pull groups from</summary> */
        public ReadOnlyCollection<string> Directories
        {
            get;
            private set;
        }

        /** <summary>
         *  Load a group with a specified superGroup.  Groups with
         *  region definitions must know their supergroup to find templates
         *  during parsing.
         *  </summary>
         */
        public virtual StringTemplateGroup LoadGroup( string groupName,
                                             Type templateLexer,
                                             StringTemplateGroup superGroup )
        {
            StringTemplateGroup group = null;
            TextReader br = null;
            // group file format defaults to <...>
            Type lexer = typeof( AngleBracketTemplateLexer );
            if ( templateLexer != null )
            {
                lexer = templateLexer;
            }
            try
            {
                br = Locate( groupName + ".stg" );
                if ( br == null )
                {
                    Error( "no such group file " + groupName + ".stg" );
                    return null;
                }
                group = new StringTemplateGroup( br, lexer, _errors, superGroup );
                br.Close();
                br = null;
            }
            catch ( IOException ioe )
            {
                Error( "can't load group " + groupName, ioe );
            }
            finally
            {
                if ( br != null )
                {
                    try
                    {
                        br.Close();
                    }
                    catch ( IOException ioe2 )
                    {
                        Error( "Cannot close template group file: " + groupName + ".stg", ioe2 );
                    }
                }
            }
            return group;
        }

        public virtual StringTemplateGroup LoadGroup( string groupName,
                                             StringTemplateGroup superGroup )
        {
            return LoadGroup( groupName, null, superGroup );
        }

        public virtual StringTemplateGroup LoadGroup( string groupName )
        {
            return LoadGroup( groupName, null );
        }

        public virtual StringTemplateGroupInterface LoadInterface( string interfaceName )
        {
            StringTemplateGroupInterface I = null;
            try
            {
                TextReader br = Locate( interfaceName + ".sti" );
                if ( br == null )
                {
                    Error( "no such interface file " + interfaceName + ".sti" );
                    return null;
                }
                I = new StringTemplateGroupInterface( br, _errors );
            }
            catch ( IOException ioe )
            {
                Error( "can't load interface " + interfaceName, ioe );
            }
            return I;
        }

        /** <summary>Look in each directory for the file called 'name'.</summary> */
        protected virtual TextReader Locate( string name )
        {
            foreach (string dir in Directories)
            {
                string fileName = Path.Combine(dir, name);
                if ( System.IO.File.Exists( fileName ) )
                {
                    System.IO.FileStream fis = System.IO.File.OpenRead( fileName );
                    StreamReader isr = GetInputStreamReader( new System.IO.BufferedStream( fis ) );
                    return isr;
                }
            }
            return null;
        }

        protected virtual StreamReader GetInputStreamReader( Stream stream )
        {
            return new StreamReader( stream, _fileCharEncoding );
        }

        public virtual Encoding GetFileCharEncoding()
        {
            return _fileCharEncoding;
        }

        public virtual void SetFileCharEncoding( Encoding fileCharEncoding )
        {
            this._fileCharEncoding = fileCharEncoding ?? Encoding.Default;
        }

        public virtual void Error( string msg )
        {
            Error( msg, null );
        }

        public virtual void Error( string msg, Exception e )
        {
            if ( _errors != null )
            {
                _errors.Error( msg, e );
            }
            else
            {
                Console.Error.WriteLine( "StringTemplate: " + msg );
                if ( e != null )
                {
                    e.PrintStackTrace();
                }
            }
        }
    }
}
