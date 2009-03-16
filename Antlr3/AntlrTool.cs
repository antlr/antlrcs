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

namespace Antlr3
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Analysis;
    using Antlr3.Codegen;
    using Antlr3.Tool;

    using FileInfo = System.IO.FileInfo;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using Stats = Antlr.Runtime.Misc.Stats;
    using StringReader = System.IO.StringReader;
    using StringWriter = System.IO.StringWriter;
    using TextWriter = System.IO.TextWriter;

    public class AntlrTool
    {
        public const string VERSION = "3.1.2";
        public const string UNINITIALIZED_DIR = "<unset-dir>";

        // Input parameters / option

        protected List<string> grammarFileNames = new List<string>();
        protected bool generate_NFA_dot = false;
        protected bool generate_DFA_dot = false;
        protected string outputDirectory = UNINITIALIZED_DIR;
        protected string libDirectory = ".";
        protected bool debug = false;
        protected bool trace = false;
        protected bool profile = false;
        protected bool report = false;
        protected bool printGrammar = false;
        protected bool depend = false;
        protected bool forceAllFilesToOutputDir = false;
        protected bool deleteTempLexer = true;
        protected bool verbose = false;

        // the internal options are for my use on the command line during dev

        public static bool internalOption_PrintGrammarTree = false;
        public static bool internalOption_PrintDFA = false;
        public static bool internalOption_ShowNFAConfigsInDFA = false;
        public static bool internalOption_watchNFAConversion = false;

        public static void Main( string[] args )
        {
            AntlrTool antlr = new AntlrTool( args );
            if ( antlr.verbose )
                ErrorManager.info( "ANTLR Parser Generator  Version " + VERSION );
            antlr.process();
            Environment.ExitCode = ( ErrorManager.getNumErrors() > 0 ) ? 1 : 0;
        }

        public AntlrTool()
        {
        }

        public AntlrTool( string[] args )
        {
            processArgs( args );
        }

        public virtual void processArgs( string[] args )
        {
            if ( args == null || args.Length == 0 )
            {
                help();
                return;
            }

            for ( int i = 0; i < args.Length; i++ )
            {
                if ( args[i] == "-o" || args[i] == "-fo" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing output directory with -fo/-o option; ignoring" );
                    }
                    else
                    {
                        if ( args[i] == "-fo" )
                            forceAllFilesToOutputDir = true;
                        i++;
                        outputDirectory = args[i];
                        if ( outputDirectory.EndsWith( "/" ) || outputDirectory.EndsWith( "\\" ) )
                            outputDirectory = outputDirectory.Substring( 0, outputDirectory.Length - 1 );
                        if ( System.IO.File.Exists( outputDirectory ) )
                        {
                            ErrorManager.error( ErrorManager.MSG_OUTPUT_DIR_IS_FILE, outputDirectory );
                            libDirectory = ".";
                        }
                    }
                }
                else if ( args[i] == "-lib" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing library directory with -lib option; ignoring" );
                    }
                    else
                    {
                        i++;
                        libDirectory = args[i];
                        if ( libDirectory.EndsWith( "/" ) ||
                             libDirectory.EndsWith( "\\" ) )
                        {
                            libDirectory =
                                libDirectory.Substring( 0, libDirectory.Length - 1 );
                        }
                        //File outDir = new File( libDirectory );
                        if ( !System.IO.Directory.Exists( libDirectory ) )
                        {
                            ErrorManager.error( ErrorManager.MSG_DIR_NOT_FOUND, libDirectory );
                            libDirectory = ".";
                        }
                    }
                }
                else if ( args[i] == "-nfa" )
                {
                    generate_NFA_dot = true;
                }
                else if ( args[i] == "-dfa" )
                {
                    generate_DFA_dot = true;
                }
                else if ( args[i] == "-debug" )
                {
                    debug = true;
                }
                else if ( args[i] == "-trace" )
                {
                    trace = true;
                }
                else if ( args[i] == "-report" )
                {
                    report = true;
                }
                else if ( args[i] == "-profile" )
                {
                    profile = true;
                }
                else if ( args[i] == "-print" )
                {
                    printGrammar = true;
                }
                else if ( args[i] == "-depend" )
                {
                    depend = true;
                }
                else if ( args[i] == "-verbose" )
                {
                    verbose = true;
                }
                else if ( args[i] == "-message-format" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing output format with -message-format option; using default" );
                    }
                    else
                    {
                        i++;
                        ErrorManager.setFormat( args[i] );
                    }
                }
                else if ( args[i] == "-Xgrtree" )
                {
                    internalOption_PrintGrammarTree = true;
                }
                else if ( args[i] == "-Xdfa" )
                {
                    internalOption_PrintDFA = true;
                }
                else if ( args[i] == "-Xnoprune" )
                {
                    DFAOptimizer.PRUNE_EBNF_EXIT_BRANCHES = false;
                }
                else if ( args[i] == "-Xnocollapse" )
                {
                    DFAOptimizer.COLLAPSE_ALL_PARALLEL_EDGES = false;
                }
                else if ( args[i] == "-Xdbgconversion" )
                {
                    NFAToDFAConverter.debug = true;
                }
                else if ( args[i] == "-Xmultithreaded" )
                {
                    NFAToDFAConverter.SINGLE_THREADED_NFA_CONVERSION = false;
                }
                else if ( args[i] == "-Xnomergestopstates" )
                {
                    DFAOptimizer.MERGE_STOP_STATES = false;
                }
                else if ( args[i] == "-Xdfaverbose" )
                {
                    internalOption_ShowNFAConfigsInDFA = true;
                }
                else if ( args[i] == "-Xwatchconversion" )
                {
                    internalOption_watchNFAConversion = true;
                }
                else if ( args[i] == "-XdbgST" )
                {
                    CodeGenerator.EMIT_TEMPLATE_DELIMITERS = true;
                }
                else if ( args[i] == "-Xmaxinlinedfastates" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing max inline dfa states -Xmaxinlinedfastates option; ignoring" );
                    }
                    else
                    {
                        i++;
                        CodeGenerator.MAX_ACYCLIC_DFA_STATES_INLINE = int.Parse( args[i] );
                    }
                }
                else if ( args[i] == "-Xm" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing max recursion with -Xm option; ignoring" );
                    }
                    else
                    {
                        i++;
                        NFAContext.MAX_SAME_RULE_INVOCATIONS_PER_NFA_CONFIG_STACK = int.Parse( args[i] );
                    }
                }
                else if ( args[i] == "-Xmaxdfaedges" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing max number of edges with -Xmaxdfaedges option; ignoring" );
                    }
                    else
                    {
                        i++;
                        DFA.MAX_STATE_TRANSITIONS_FOR_TABLE = int.Parse( args[i] );
                    }
                }
                else if ( args[i] == "-Xconversiontimeout" )
                {
                    if ( i + 1 >= args.Length )
                    {
                        Console.Error.WriteLine( "missing max time in ms -Xconversiontimeout option; ignoring" );
                    }
                    else
                    {
                        i++;
                        DFA.MAX_TIME_PER_DFA_CREATION = TimeSpan.FromMilliseconds( int.Parse( args[i] ) );
                    }
                }
                else if ( args[i] == "-Xnfastates" )
                {
                    DecisionProbe.verbose = true;
                }
                else if ( args[i] == "-X" )
                {
                    Xhelp();
                }
                else
                {
                    if ( args[i][0] != '-' )
                    {
                        // must be the grammar file
                        grammarFileNames.Add( args[i] );
                    }
                }
            }
        }

        /*
            protected void checkForInvalidArguments(String[] args, BitSet cmdLineArgValid) {
                // check for invalid command line args
                for (int a = 0; a < args.length; a++) {
                    if (!cmdLineArgValid.member(a)) {
                        System.err.println("invalid command-line argument: " + args[a] + "; ignored");
                    }
                }
            }
            */

        public virtual void process()
        {
            int numFiles = grammarFileNames.Count;
            bool exceptionWhenWritingLexerFile = false;
            string lexerGrammarFileName = null;		// necessary at this scope to have access in the catch below
            for ( int i = 0; i < numFiles; i++ )
            {
                string grammarFileName = grammarFileNames[i];
                if ( verbose && !depend )
                {
                    Console.Out.WriteLine( grammarFileName );
                }
                try
                {
                    if ( depend )
                    {
                        BuildDependencyGenerator dep = new BuildDependencyGenerator( this, grammarFileName );
                        IList<string> outputFiles = dep.getGeneratedFileList();
                        IList<string> dependents = dep.getDependenciesFileList();
                        //Console.Out.println("output: "+outputFiles);
                        //Console.Out.println("dependents: "+dependents);
                        Console.Out.WriteLine( dep.getDependencies() );
                        continue;
                    }
                    Grammar grammar = getRootGrammar( grammarFileName );
                    // we now have all grammars read in as ASTs
                    // (i.e., root and all delegates)
                    grammar.composite.assignTokenTypes();
                    grammar.composite.defineGrammarSymbols();
                    grammar.composite.createNFAs();

                    generateRecognizer( grammar );

                    if ( printGrammar )
                    {
                        grammar.printGrammar( Console.Out );
                    }

                    if ( report )
                    {
                        GrammarReport report2 = new GrammarReport( grammar );
                        Console.Out.WriteLine( report2.ToString() );
                        // print out a backtracking report too (that is not encoded into log)
                        Console.Out.WriteLine( report2.getBacktrackingReport() );
                        // same for aborted NFA->DFA conversions
                        Console.Out.WriteLine( report2.getAnalysisTimeoutReport() );
                    }
                    if ( profile )
                    {
                        GrammarReport report2 = new GrammarReport( grammar );
                        Stats.WriteReport( GrammarReport.GRAMMAR_STATS_FILENAME,
                                          report2.toNotifyString() );
                    }

                    // now handle the lexer if one was created for a merged spec
                    string lexerGrammarStr = grammar.getLexerGrammar();
                    //JSystem.@out.println("lexer grammar:\n"+lexerGrammarStr);
                    if ( grammar.type == Grammar.COMBINED && lexerGrammarStr != null )
                    {
                        lexerGrammarFileName = grammar.ImplicitlyGeneratedLexerFileName;
                        try
                        {
                            TextWriter w = getOutputFile( grammar, lexerGrammarFileName );
                            w.Write( lexerGrammarStr );
                            w.Close();
                        }
                        catch ( IOException e )
                        {
                            // emit different error message when creating the implicit lexer fails
                            // due to write permission error
                            exceptionWhenWritingLexerFile = true;
                            throw e;
                        }
                        try
                        {
                            StringReader sr = new StringReader( lexerGrammarStr );
                            Grammar lexerGrammar = new Grammar();
                            lexerGrammar.composite.watchNFAConversion = internalOption_watchNFAConversion;
                            lexerGrammar.implicitLexer = true;
                            lexerGrammar.Tool = this;
                            FileInfo lexerGrammarFullFile = new FileInfo( System.IO.Path.Combine( getFileDirectory( lexerGrammarFileName ), lexerGrammarFileName ) );
                            lexerGrammar.FileName = lexerGrammarFullFile.ToString();

                            lexerGrammar.importTokenVocabulary( grammar );
                            lexerGrammar.parseAndBuildAST( sr );

                            sr.Close();

                            lexerGrammar.composite.assignTokenTypes();
                            lexerGrammar.composite.defineGrammarSymbols();
                            lexerGrammar.composite.createNFAs();

                            generateRecognizer( lexerGrammar );
                        }
                        finally
                        {
                            // make sure we clean up
                            if ( deleteTempLexer )
                            {
                                System.IO.DirectoryInfo outputDir = getOutputDirectory( lexerGrammarFileName );
                                FileInfo outputFile = new FileInfo( System.IO.Path.Combine( outputDir.FullName, lexerGrammarFileName ) );
                                outputFile.Delete();
                            }
                        }
                    }
                }
                catch ( IOException e )
                {
                    if ( exceptionWhenWritingLexerFile )
                    {
                        ErrorManager.error( ErrorManager.MSG_CANNOT_WRITE_FILE,
                                           lexerGrammarFileName, e );
                    }
                    else
                    {
                        ErrorManager.error( ErrorManager.MSG_CANNOT_OPEN_FILE,
                                           grammarFileName );
                    }
                }
                catch ( Exception e )
                {
                    ErrorManager.error( ErrorManager.MSG_INTERNAL_ERROR, grammarFileName, e );
                }
                /*
                finally {
                    JSystem.@out.println("creates="+ Interval.creates);
                    JSystem.@out.println("hits="+ Interval.hits);
                    JSystem.@out.println("misses="+ Interval.misses);
                    JSystem.@out.println("outOfRange="+ Interval.outOfRange);
                }
                */
            }
        }

        /** Get a grammar mentioned on the command-line and any delegates */
        public virtual Grammar getRootGrammar( string grammarFileName )
        {
            //StringTemplate.setLintMode(true);
            // grammars mentioned on command line are either roots or single grammars.
            // create the necessary composite in case it's got delegates; even
            // single grammar needs it to get token types.
            CompositeGrammar composite = new CompositeGrammar();
            Grammar grammar = new Grammar( this, grammarFileName, composite );
            composite.setDelegationRoot( grammar );
            //FileReader fr = null;
            //fr = new FileReader( grammarFileName );
            //BufferedReader br = new BufferedReader( fr );
            //grammar.parseAndBuildAST( br );
            StringReader reader = new StringReader( System.IO.File.ReadAllText( grammarFileName ) );
            grammar.parseAndBuildAST( reader );
            composite.watchNFAConversion = internalOption_watchNFAConversion;
            //br.close();
            //fr.close();
            return grammar;
        }

        /** Create NFA, DFA and generate code for grammar.
         *  Create NFA for any delegates first.  Once all NFA are created,
         *  it's ok to create DFA, which must check for left-recursion.  That check
         *  is done by walking the full NFA, which therefore must be complete.
         *  After all NFA, comes DFA conversion for root grammar then code gen for
         *  root grammar.  DFA and code gen for delegates comes next.
         */
        protected virtual void generateRecognizer( Grammar grammar )
        {
            string language = (string)grammar.getOption( "language" );
            if ( language != null )
            {
                CodeGenerator generator = new CodeGenerator( this, grammar, language );
                grammar.setCodeGenerator( generator );
                generator.setDebug( debug );
                generator.setProfile( profile );
                generator.setTrace( trace );

                // generate NFA early in case of crash later (for debugging)
                if ( generate_NFA_dot )
                {
                    generateNFAs( grammar );
                }

                // GENERATE CODE
                generator.genRecognizer();

                if ( generate_DFA_dot )
                {
                    generateDFAs( grammar );
                }

                IList<Grammar> delegates = grammar.getDirectDelegates();
                for ( int i = 0; delegates != null && i < delegates.Count; i++ )
                {
                    Grammar @delegate = (Grammar)delegates[i];
                    if ( @delegate != grammar )
                    { // already processing this one
                        generateRecognizer( @delegate );
                    }
                }
            }
        }

        public virtual void generateDFAs( Grammar g )
        {
            for ( int d = 1; d <= g.NumberOfDecisions; d++ )
            {
                DFA dfa = g.getLookaheadDFA( d );
                if ( dfa == null )
                {
                    continue; // not there for some reason, ignore
                }
                DOTGenerator dotGenerator = new DOTGenerator( g );
                string dot = dotGenerator.getDOT( dfa.startState );
                string dotFileName = g.name + "." + "dec-" + d;
                if ( g.implicitLexer )
                {
                    dotFileName = g.name + Grammar.grammarTypeToFileNameSuffix[g.type] + "." + "dec-" + d;
                }
                try
                {
                    writeDOTFile( g, dotFileName, dot );
                }
                catch ( IOException ioe )
                {
                    ErrorManager.error( ErrorManager.MSG_CANNOT_GEN_DOT_FILE,
                                       dotFileName,
                                       ioe );
                }
            }
        }

        protected virtual void generateNFAs( Grammar g )
        {
            DOTGenerator dotGenerator = new DOTGenerator( g );
            ICollection<Rule> rules = g.getAllImportedRules();
            rules.addAll( g.Rules );

            foreach ( Rule r in rules )
            {
                try
                {
                    string dot = dotGenerator.getDOT( r.startState );
                    if ( dot != null )
                    {
                        writeDOTFile( g, r, dot );
                    }
                }
                catch ( IOException ioe )
                {
                    ErrorManager.error( ErrorManager.MSG_CANNOT_WRITE_FILE, ioe );
                }
            }
        }

        protected virtual void writeDOTFile( Grammar g, Rule r, string dot )
        {
            writeDOTFile( g, r.grammar.name + "." + r.name, dot );
        }

        protected virtual void writeDOTFile( Grammar g, string name, string dot )
        {
            TextWriter fw = getOutputFile( g, name + ".dot" );
            fw.Write( dot );
            fw.Close();
        }

        private static void help()
        {
            Console.Error.WriteLine( "usage: java org.antlr.Tool [args] file.g [file2.g file3.g ...]" );
            Console.Error.WriteLine( "  -o outputDir          specify output directory where all output is generated" );
            Console.Error.WriteLine( "  -fo outputDir         same as -o but force even files with relative paths to dir" );
            Console.Error.WriteLine( "  -lib dir              specify location of token files" );
            Console.Error.WriteLine( "  -depend               generate file dependencies" );
            Console.Error.WriteLine( "  -verbose              generate ANTLR version and other information" );
            Console.Error.WriteLine( "  -report               print out a report about the grammar(s) processed" );
            Console.Error.WriteLine( "  -print                print out the grammar without actions" );
            Console.Error.WriteLine( "  -debug                generate a parser that emits debugging events" );
            Console.Error.WriteLine( "  -profile              generate a parser that computes profiling information" );
            Console.Error.WriteLine( "  -nfa                  generate an NFA for each rule" );
            Console.Error.WriteLine( "  -dfa                  generate a DFA for each decision point" );
            Console.Error.WriteLine( "  -message-format name  specify output style for messages" );
            Console.Error.WriteLine( "  -X                    display extended argument list" );
        }

        private static void Xhelp()
        {
            Console.Error.WriteLine( "  -Xgrtree               print the grammar AST" );
            Console.Error.WriteLine( "  -Xdfa                  print DFA as text " );
            Console.Error.WriteLine( "  -Xnoprune              test lookahead against EBNF block exit branches" );
            Console.Error.WriteLine( "  -Xnocollapse           collapse incident edges into DFA states" );
            Console.Error.WriteLine( "  -Xdbgconversion        dump lots of info during NFA conversion" );
            Console.Error.WriteLine( "  -Xmultithreaded        run the analysis in 2 threads" );
            Console.Error.WriteLine( "  -Xnomergestopstates    do not merge stop states" );
            Console.Error.WriteLine( "  -Xdfaverbose           generate DFA states in DOT with NFA configs" );
            Console.Error.WriteLine( "  -Xwatchconversion      print a message for each NFA before converting" );
            Console.Error.WriteLine( "  -XdbgST                put tags at start/stop of all templates in output" );
            Console.Error.WriteLine( "  -Xm m                  max number of rule invocations during conversion" );
            Console.Error.WriteLine( "  -Xmaxdfaedges m        max \"comfortable\" number of edges for single DFA state" );
            Console.Error.WriteLine( "  -Xconversiontimeout t  set NFA conversion timeout for each decision" );
            Console.Error.WriteLine( "  -Xmaxinlinedfastates m max DFA states before table used rather than inlining" );
            Console.Error.WriteLine( "  -Xnfastates            for nondeterminisms, list NFA states for each path" );
        }

        public virtual void setOutputDirectory( string outputDirectory )
        {
            this.outputDirectory = outputDirectory;
        }

        public virtual TextWriter getOutputFile( Grammar g, string fileName )
        {
            if ( outputDirectory == null )
                return new StringWriter();

            System.IO.DirectoryInfo outputDir = getOutputDirectory( g.FileName );
            FileInfo outputFile = new FileInfo( System.IO.Path.Combine( outputDir.FullName, fileName ) );

            if ( !outputDir.Exists )
                outputDir.Create();

            if ( outputFile.Exists )
                outputFile.Delete();

            return new System.IO.StreamWriter( new System.IO.BufferedStream( outputFile.OpenWrite() ) );
            //if ( outputDirectory == null )
            //{
            //    return new StringWriter();
            //}
            //// output directory is a function of where the grammar file lives
            //// for subdir/T.g, you get subdir here.  Well, depends on -o etc...
            //File outputDir = getOutputDirectory( g.getFileName() );
            //File outputFile = new File( outputDir, fileName );

            //if ( !outputDir.exists() )
            //{
            //    outputDir.mkdirs();
            //}
            //FileWriter fw = new FileWriter( outputFile );
            //return new BufferedWriter( fw );
        }

        public virtual System.IO.DirectoryInfo getOutputDirectory( string fileNameWithPath )
        {
            //File outputDir = new File( outputDirectory );
            //String fileDirectory = getFileDirectory( fileNameWithPath );
            //if ( outputDirectory != UNINITIALIZED_DIR )
            //{
            //    // -o /tmp /var/lib/t.g => /tmp/T.java
            //    // -o subdir/output /usr/lib/t.g => subdir/output/T.java
            //    // -o . /usr/lib/t.g => ./T.java
            //    if ( fileDirectory != null &&
            //         ( new File( fileDirectory ).isAbsolute() ||
            //          fileDirectory.startsWith( "~" ) ) || // isAbsolute doesn't count this :(
            //                                         forceAllFilesToOutputDir
            //        )
            //    {
            //        // somebody set the dir, it takes precendence; write new file there
            //        outputDir = new File( outputDirectory );
            //    }
            //    else
            //    {
            //        // -o /tmp subdir/t.g => /tmp/subdir/t.g
            //        if ( fileDirectory != null )
            //        {
            //            outputDir = new File( outputDirectory, fileDirectory );
            //        }
            //        else
            //        {
            //            outputDir = new File( outputDirectory );
            //        }
            //    }
            //}
            //else
            //{
            //    // they didn't specify a -o dir so just write to location
            //    // where grammar is, absolute or relative
            //    String dir = ".";
            //    if ( fileDirectory != null )
            //    {
            //        dir = fileDirectory;
            //    }
            //    outputDir = new File( dir );
            //}
            //return outputDir;

            //File outputDir = new File( outputDirectory );
            string outputDir = outputDirectory;

            if ( fileNameWithPath.IndexOfAny( System.IO.Path.GetInvalidPathChars() ) >= 0 )
                return new System.IO.DirectoryInfo( outputDir );

            if ( !System.IO.Path.IsPathRooted( fileNameWithPath ) )
                fileNameWithPath = System.IO.Path.GetFullPath( fileNameWithPath );

            string fileDirectory = getFileDirectory( fileNameWithPath );
            if ( outputDirectory != UNINITIALIZED_DIR )
            {
                // -o /tmp /var/lib/t.g => /tmp/T.java
                // -o subdir/output /usr/lib/t.g => subdir/output/T.java
                // -o . /usr/lib/t.g => ./T.java
                if ( fileDirectory != null &&
                     ( System.IO.Path.IsPathRooted( fileDirectory ) ||
                      fileDirectory.StartsWith( "~" ) ) || // isAbsolute doesn't count this :(
                                                     forceAllFilesToOutputDir
                    )
                {
                    // somebody set the dir, it takes precendence; write new file there
                    //outputDir = new File( outputDirectory );
                    outputDir = outputDirectory;
                }
                else
                {
                    // -o /tmp subdir/t.g => /tmp/subdir/t.g
                    if ( fileDirectory != null )
                    {
                        //outputDir = new File( outputDirectory, fileDirectory );
                        outputDir = System.IO.Path.Combine( outputDirectory, fileDirectory );
                    }
                    else
                    {
                        //outputDir = new File( outputDirectory );
                        outputDir = outputDirectory;
                    }
                }
            }
            else
            {
                // they didn't specify a -o dir so just write to location
                // where grammar is, absolute or relative
                string dir = ".";
                if ( fileDirectory != null )
                {
                    dir = fileDirectory;
                }
                //outputDir = new File( dir );
                outputDir = dir;
            }
            return new System.IO.DirectoryInfo( outputDir );
        }

        public virtual string getLibraryFile( string fileName )
        {
            return System.IO.Path.Combine( libDirectory, fileName );
        }

        public virtual string getLibraryDirectory()
        {
            return libDirectory;
        }

        public virtual string getFileDirectory( string fileName )
        {
            return System.IO.Path.GetDirectoryName( fileName );
        }

        public virtual FileInfo getImportedVocabFile( string vocabName )
        {
            string path = System.IO.Path.Combine( getLibraryDirectory(), vocabName + CodeGenerator.VOCAB_FILE_EXTENSION );
            if ( System.IO.File.Exists( path ) )
                return new FileInfo( path );

            return new FileInfo( System.IO.Path.Combine( outputDirectory, vocabName + CodeGenerator.VOCAB_FILE_EXTENSION ) );
            //File f = new File( getLibraryDirectory(),
            //                  File.separator +
            //                  vocabName +
            //                  CodeGenerator.VOCAB_FILE_EXTENSION );
            //if ( f.exists() )
            //{
            //    return f;
            //}
            //
            //return new File( outputDirectory +
            //                File.separator +
            //                vocabName +
            //                CodeGenerator.VOCAB_FILE_EXTENSION );
        }

        public virtual void panic()
        {
            throw new Exception( "ANTLR panic" );
        }

        /// <summary>
        /// Return a time stamp string accurate to sec: yyyy-mm-dd hh:mm:ss
        /// </summary>
        public static string getCurrentTimeStamp()
        {
            return DateTime.Now.ToString( "yyyy\\-MM\\-dd HH\\:mm\\:ss" );
        }
    }
}
