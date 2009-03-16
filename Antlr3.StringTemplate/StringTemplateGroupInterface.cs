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
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.ST.Language;

    using IList = System.Collections.IList;
    using StringBuilder = System.Text.StringBuilder;
    using TextReader = System.IO.TextReader;

    /** <summary>A group interface is like a group without the template implementations.</summary>
     *
     *  <remarks>
     *  There are just template names/argument-lists like this:
     *
     *  interface foo;
     *  class(name,fields);
     *  method(name,args,body);
     *  </remarks>
     */
    public class StringTemplateGroupInterface
    {
        /** <summary>What is the group name</summary> */
        string _name;

        /** <summary>Maps template name to TemplateDefinition object</summary> */
        Dictionary<string, TemplateDefinition> _templates = new Dictionary<string, TemplateDefinition>();

        /** <summary>
         *  Are we derived from another group?  Templates not found in this group
         *  will be searched for in the superGroup recursively.
         *  </summary>
         */
        StringTemplateGroupInterface _superInterface = null;

        /** <summary>
         *  Where to report errors.  All string templates in this group
         *  use this error handler by default.
         *  </summary>
         */
        IStringTemplateErrorListener _listener = DEFAULT_ERROR_LISTENER;

        class DefaultErrorListener : IStringTemplateErrorListener
        {
            #region StringTemplateErrorListener Members

            public void error( string s, Exception e )
            {
                Console.Error.WriteLine( s );
                if ( e != null )
                {
                    e.printStackTrace( Console.Error );
                }
            }

            public void warning( string s )
            {
                Console.Out.WriteLine( s );
            }

            #endregion
        }

        public static IStringTemplateErrorListener DEFAULT_ERROR_LISTENER = new DefaultErrorListener();

        /** <summary>All the info we need to track for a template defined in an interface</summary> */
        protected class TemplateDefinition
        {
            public string name;
            public IDictionary<string, FormalArgument> formalArgs; // LinkedHashMap<FormalArgument>
            public bool optional = false;
            public TemplateDefinition( string name, IDictionary<string, FormalArgument> formalArgs, bool optional )
            {
                this.name = name;
                this.formalArgs = formalArgs;
                this.optional = optional;
            }
        }

        public StringTemplateGroupInterface( TextReader r )
            : this( r, DEFAULT_ERROR_LISTENER, (StringTemplateGroupInterface)null )
        {
        }

        public StringTemplateGroupInterface( TextReader r, IStringTemplateErrorListener errors )
            : this( r, errors, (StringTemplateGroupInterface)null )
        {
        }

        /** <summary>Create an interface from the input stream</summary> */
        public StringTemplateGroupInterface( TextReader r,
                                            IStringTemplateErrorListener errors,
                                            StringTemplateGroupInterface superInterface )
        {
            this._listener = errors;
            setSuperInterface( superInterface );
            parseInterface( r );
        }

        public virtual StringTemplateGroupInterface getSuperInterface()
        {
            return _superInterface;
        }

        public virtual void setSuperInterface( StringTemplateGroupInterface superInterface )
        {
            this._superInterface = superInterface;
        }

        protected virtual void parseInterface( TextReader r )
        {
            try
            {
                InterfaceLexer lexer = new InterfaceLexer( new Antlr.Runtime.ANTLRReaderStream( r ) );
                InterfaceParser parser = new InterfaceParser( new Antlr.Runtime.CommonTokenStream( lexer ) );
                parser.groupInterface( this );
                //System.out.println("read interface\n"+this.toString());
            }
            catch ( Exception e )
            {
                string name = "<unknown>";
                if ( getName() != null )
                {
                    name = getName();
                }
                error( "problem parsing group " + name + ": " + e, e );
            }
        }

        public virtual void defineTemplate( string name, IDictionary<string, FormalArgument> formalArgs, bool optional )
        {
            TemplateDefinition d = new TemplateDefinition( name, formalArgs, optional );
            _templates[d.name] = d;
        }

        /** <summary>
         *  Return a list of all template names missing from group that are defined
         *  in this interface.  Return null if all is well.
         *  </summary>
         */
        public virtual IList getMissingTemplates( StringTemplateGroup group )
        {
            IList missing =
                _templates.Values
                .Where( template => !template.optional && !group.isDefined( template.name ) )
                .Select( template => template.name )
                .ToArray();

            return ( missing.Count == 0 ) ? null : missing;
        }

        /** <summary>
         *  Return a list of all template sigs that are present in the group, but
         *  that have wrong formal argument lists.  Return null if all is well.
         *  </summary>
         */
        public virtual IList getMismatchedTemplates( StringTemplateGroup group )
        {
            IList mismatched = new List<object>();
            foreach ( TemplateDefinition d in _templates.Values )
            {
                if ( group.isDefined( d.name ) )
                {
                    StringTemplate defST = group.getTemplateDefinition( d.name );
                    var formalArgs = defST.getFormalArguments();
                    bool ack = false;
                    if ( ( d.formalArgs != null && formalArgs == null ) ||
                        ( d.formalArgs == null && formalArgs != null ) ||
                        d.formalArgs.Count != formalArgs.Count )
                    {
                        ack = true;
                    }
                    if ( !ack )
                    {
                        foreach ( var arg in formalArgs )
                        {
                            FormalArgument arg2;
                            if ( !d.formalArgs.TryGetValue( arg.name, out arg2 ) || arg2 == null )
                            {
                                ack = true;
                                break;
                            }
                        }
                    }
                    if ( ack )
                    {
                        //System.out.println(d.formalArgs+"!="+formalArgs);
                        mismatched.Add( getTemplateSignature( d ) );
                    }
                }
            }
            if ( mismatched.Count == 0 )
            {
                mismatched = null;
            }
            return mismatched;
        }

        public virtual string getName()
        {
            return _name;
        }

        public virtual void setName( string name )
        {
            this._name = name;
        }

        public virtual void error( string msg )
        {
            error( msg, null );
        }

        public virtual void error( string msg, Exception e )
        {
            if ( _listener != null )
            {
                _listener.error( msg, e );
            }
            else
            {
                Console.Error.WriteLine( "StringTemplate: " + msg );
                if ( e != null )
                {
                    e.printStackTrace();
                }
            }
        }

        string newline = Environment.NewLine;

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( "interface " );
            buf.Append( getName() );
            buf.Append( ";" + newline );
            foreach ( TemplateDefinition d in _templates.Values )
            {
                buf.Append( getTemplateSignature( d ) );
                buf.Append( ";" + newline );
            }
            return buf.ToString();
        }

        protected virtual string getTemplateSignature( TemplateDefinition d )
        {
            StringBuilder buf = new StringBuilder();
            if ( d.optional )
            {
                buf.Append( "optional " );
            }
            buf.Append( d.name );
            if ( d.formalArgs != null )
            {
                StringBuilder args = new StringBuilder();
                args.Append( '(' );
                int i = 1;
                foreach ( string name in d.formalArgs.Keys )
                {
                    if ( i > 1 )
                    {
                        args.Append( ", " );
                    }
                    args.Append( name );
                    i++;
                }
                args.Append( ')' );
                buf.Append( args );
            }
            else
            {
                buf.Append( "()" );
            }
            return buf.ToString();
        }
    }
}
