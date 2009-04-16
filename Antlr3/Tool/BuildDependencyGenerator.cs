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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ArrayList = System.Collections.Generic.List<object>;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using FileInfo = System.IO.FileInfo;
    using IList = System.Collections.IList;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;
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
        protected string grammarFileName;
        protected string tokenVocab;
        protected Tool tool;
        protected Grammar grammar;
        protected CodeGenerator generator;
        protected StringTemplateGroup templates;

        public BuildDependencyGenerator( Tool tool, string grammarFileName )
        {
            this.tool = tool;
            this.grammarFileName = grammarFileName;
            grammar = tool.getRootGrammar( grammarFileName );
            string language = (string)grammar.getOption( "language" );
            generator = new CodeGenerator( tool, grammar, language );
            generator.LoadTemplates( language );
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
        public virtual IList<string> getGeneratedFileList()
        {
            List<FileInfo> files = new List<FileInfo>();
            System.IO.DirectoryInfo outputDir = tool.getOutputDirectory( grammarFileName );
            if ( outputDir.Name.Equals( "." ) )
            {
                outputDir = null;
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
                string suffix = Grammar.grammarTypeToFileNameSuffix[grammar.type];
                string fileName = grammar.name + suffix + headerExtST.ToString();
                files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, fileName ) ) );
            }
            if ( grammar.type == Grammar.COMBINED )
            {
                // add autogenerated lexer; e.g., TLexer.java TLexer.h TLexer.tokens
                // don't add T__.g (just a temp file)
                string suffix = Grammar.grammarTypeToFileNameSuffix[Grammar.LEXER];
                string lexer = grammar.name + suffix + extST.ToString();
                files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, lexer ) ) );

                // TLexer.h
                if ( headerExtST != null )
                {
                    string header = grammar.name + suffix + headerExtST.ToString();
                    files.Add( new FileInfo( System.IO.Path.Combine( outputDir.FullName, header ) ) );
                }
                // for combined, don't generate TLexer.tokens
            }

            // handle generated files for imported grammars
            IList<Grammar> imports =
                grammar.composite.getDelegates( grammar.composite.RootGrammar );
            foreach ( Grammar g in imports )
            {
                outputDir = tool.getOutputDirectory( g.FileName );
                string fname = groomQualifiedFileName( outputDir.ToString(), g.getRecognizerName() + extST.ToString() );
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
        public virtual IList<string> getDependenciesFileList()
        {
            // Find all things other than imported grammars
            //
            IList<string> files = getNonImportDependenciesFileList();

            // Handle imported grammars
            //
            IList<Grammar> imports =
                grammar.composite.getDelegates( grammar.composite.RootGrammar );
            foreach ( Grammar g in imports )
            {
                string libdir = tool.LibraryDirectory;
                string fileName = groomQualifiedFileName( libdir, g.fileName );
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
        public virtual List<string> getNonImportDependenciesFileList()
        {
            List<string> files = new List<string>();

            // handle token vocabulary loads
            tokenVocab = (string)grammar.getOption( "tokenVocab" );
            if ( tokenVocab != null )
            {
                FileInfo vocabFile = tool.getImportedVocabFile( tokenVocab );
                files.Add( vocabFile.FullName );
            }

            return files;
        }

        public virtual StringTemplate getDependencies()
        {
            loadDependencyTemplates();
            StringTemplate dependenciesST = templates.GetInstanceOf( "dependencies" );
            dependenciesST.SetAttribute( "in", getDependenciesFileList() );
            dependenciesST.SetAttribute( "out", getGeneratedFileList() );
            dependenciesST.SetAttribute( "grammarFileName", grammar.fileName );
            return dependenciesST;
        }

        public virtual void loadDependencyTemplates()
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

        public virtual string groomQualifiedFileName( string outputDir, string fileName )
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
