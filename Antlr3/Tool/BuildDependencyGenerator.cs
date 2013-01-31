/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Extensions;

    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using FileInfo = System.IO.FileInfo;
    using Path = System.IO.Path;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;
    using Tool = Antlr3.AntlrTool;

    /** Given a grammar file, show the dependencies on .tokens etc...
     *  Using ST, emit a simple "make compatible" list of dependencies.
     *  For example, combined grammar T.g (no token import) generates:
     *
     *		TParser.java : T.g
     * 		T.tokens : T.g
     * 		T__g : T.g
     *
     *  For tree grammar TP with import of T.tokens:
     *
     * 		TP.g : T.tokens
     * 		TP.java : TP.g
     *
     *  If "-lib libdir" is used on command-line with -depend, then include the
     *  path like
     *
     * 		TP.g : libdir/T.tokens
     *
     *  Pay attention to -o as well:
     *
     * 		outputdir/TParser.java : T.g
     *
     *  So this output shows what the grammar depends on *and* what it generates.
     *
     *  Operate on one grammar file at a time.  If given a list of .g on the
     *  command-line with -depend, just emit the dependencies.  The grammars
     *  may depend on each other, but the order doesn't matter.  Build tools,
     *  reading in this output, will know how to organize it.
     *
     *  This is a wee bit slow probably because the code generator has to load
     *  all of its template files in order to figure out the file extension
     *  for the generated recognizer.
     *
     *  This code was obvious until I removed redundant "./" on front of files
     *  and had to escape spaces in filenames :(
     */
    public class BuildDependencyGenerator
    {
        private readonly string grammarFileName;
        private readonly Tool tool;
        private readonly Grammar grammar;
        private readonly CodeGenerator generator;
        private string tokenVocab;
#pragma warning disable 649 // Field 'field' is never assigned to, and will always have its default value 'value'
        private TemplateGroup templates;
#pragma warning restore 649

        public BuildDependencyGenerator( Tool tool, string grammarFileName )
        {
            this.tool = tool;
            this.grammarFileName = grammarFileName;
            this.grammar = tool.GetRootGrammar( grammarFileName );

            string language = (string)grammar.GetOption( "language" );
            this.generator = new CodeGenerator( tool, grammar, language );
            this.generator.LoadTemplates( language );
        }

        public virtual string TokenVocab
        {
            get
            {
                return tokenVocab;
            }
        }

        public virtual CodeGenerator Generator
        {
            get
            {
                return generator;
            }
        }

        /** From T.g return a list of File objects that
         *  name files ANTLR will emit from T.g.
         */
        public virtual IList<string> GetGeneratedFileList()
        {
            List<FileInfo> files = new List<FileInfo>();
            System.IO.DirectoryInfo outputDir = tool.GetOutputDirectory( grammarFileName );
            if ( outputDir.Name.Equals( "." ) )
            {
                outputDir = outputDir.Parent;
            }
            else if ( outputDir.Name.IndexOf( ' ' ) >= 0 )
            { // has spaces?
                string escSpaces = outputDir.ToString().Replace(
                                                 " ",
                                                 "\\ " );
                outputDir = new System.IO.DirectoryInfo( escSpaces );
            }
            // add generated recognizer; e.g., TParser.java
            string recognizer =
                generator.GetRecognizerFileName( grammar.name, grammar.type );
            files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, recognizer ) ) );
            // add output vocab file; e.g., T.tokens. This is always generated to
            // the base output directory, which will be just . if there is no -o option
            //
            files.Add( new FileInfo( System.IO.Path.Combine( tool.OutputDirectory, generator.VocabFileName ) ) );
            // are we generating a .h file?
            StringTemplate headerExtST = null;
            StringTemplate extST = generator.Templates.GetInstanceOf( "codeFileExtension" );
            if ( generator.Templates.IsDefined( "headerFile" ) )
            {
                headerExtST = generator.Templates.GetInstanceOf( "headerFileExtension" );
                string suffix = Grammar.grammarTypeToFileNameSuffix[(int)grammar.type];
                string fileName = grammar.name + suffix + headerExtST.Render();
                files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, fileName ) ) );
            }
            if ( grammar.type == GrammarType.Combined )
            {
                // add autogenerated lexer; e.g., TLexer.java TLexer.h TLexer.tokens
                // don't add T__.g (just a temp file)
                string suffix = Grammar.grammarTypeToFileNameSuffix[(int)GrammarType.Lexer];
                string lexer = grammar.name + suffix + extST.Render();
                files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, lexer ) ) );

                // TLexer.h
                if ( headerExtST != null )
                {
                    string header = grammar.name + suffix + headerExtST.Render();
                    files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, header ) ) );
                }
                // for combined, don't generate TLexer.tokens
            }

            // handle generated files for imported grammars
            IList<Grammar> imports =
                grammar.composite.GetDelegates( grammar.composite.RootGrammar );
            foreach ( Grammar g in imports )
            {
                outputDir = tool.GetOutputDirectory( g.FileName );
                string fname = GroomQualifiedFileName( outputDir.ToString(), g.GetRecognizerName() + extST.Render() );
                files.Add( new FileInfo( fname ) );
            }

            if ( files.Count == 0 )
            {
                return null;
            }

            return files.Select( info => info.FullName ).ToArray();
        }

        /** Return a list of File objects that name files ANTLR will read
         *  to process T.g; This can be .tokens files if the grammar uses the tokenVocab option
         *  as well as any imported grammar files.
         */
        public virtual IList<string> GetDependenciesFileList()
        {
            // Find all things other than imported grammars
            //
            IList<string> files = GetNonImportDependenciesFileList();

            // Handle imported grammars
            //
            IList<Grammar> imports =
                grammar.composite.GetDelegates( grammar.composite.RootGrammar );
            foreach ( Grammar g in imports )
            {
                string libdir = tool.LibraryDirectory;
                string fileName = GroomQualifiedFileName( libdir, g.fileName );
                files.Add( fileName );
            }

            if ( files.Count == 0 )
            {
                return null;
            }
            return files;
        }

        /**
         * Return a list of File objects that name files ANTLR will read
         * to process T.g; This can only be .tokens files and only
         * if they use the tokenVocab option.
         *
         * @return List of dependencies other than imported grammars
         */
        public virtual List<string> GetNonImportDependenciesFileList()
        {
            List<string> files = new List<string>();

            // handle token vocabulary loads
            tokenVocab = (string)grammar.GetOption( "tokenVocab" );
            if ( tokenVocab != null )
            {
                string vocabFile = tool.GetImportedVocabFile( tokenVocab );
                files.Add( Path.GetFullPath( vocabFile ) );
            }

            return files;
        }

        public virtual StringTemplate GetDependencies()
        {
            LoadDependencyTemplates();
            StringTemplate dependenciesST = templates.GetInstanceOf( "dependencies" );
            dependenciesST.SetAttribute( "in", GetDependenciesFileList() );
            dependenciesST.SetAttribute( "out", GetGeneratedFileList() );
            dependenciesST.SetAttribute( "grammarFileName", grammar.fileName );
            return dependenciesST;
        }

        public virtual void LoadDependencyTemplates()
        {
            throw new NotImplementedException();
            //if ( templates != null )
            //{
            //    return;
            //}
            //String fileName = "org/antlr/tool/templates/depend.stg";
            //ClassLoader cl = Thread.currentThread().getContextClassLoader();
            //InputStream @is = cl.getResourceAsStream( fileName );
            //if ( @is == null )
            //{
            //    cl = typeof( ErrorManager ).getClassLoader();
            //    @is = cl.getResourceAsStream( fileName );
            //}
            //if ( @is == null )
            //{
            //    ErrorManager.internalError( "Can't load dependency templates: " + fileName );
            //    return;
            //}
            //BufferedReader br = null;
            //try
            //{
            //    br = new BufferedReader( new InputStreamReader( @is ) );
            //    templates = new StringTemplateGroup( br,
            //                                        typeof( AngleBracketTemplateLexer ) );
            //    br.close();
            //}
            //catch ( IOException ioe )
            //{
            //    ErrorManager.internalError( "error reading dependency templates file " + fileName, ioe );
            //}
            //finally
            //{
            //    if ( br != null )
            //    {
            //        try
            //        {
            //            br.close();
            //        }
            //        catch ( IOException ioe )
            //        {
            //            ErrorManager.internalError( "cannot close dependency templates file " + fileName, ioe );
            //        }
            //    }
            //}
        }

        public virtual string GroomQualifiedFileName( string outputDir, string fileName )
        {
            if ( outputDir.Equals( "." ) )
            {
                return fileName;
            }
            else if ( outputDir.IndexOf( ' ' ) >= 0 )
            { // has spaces?
                string escSpaces = outputDir.ToString().Replace(
                                                 " ",
                                                 "\\ " );
                return System.IO.Path.Combine( escSpaces, fileName );
            }
            else
            {
                return System.IO.Path.Combine( outputDir, fileName );
            }
        }
    }
}
