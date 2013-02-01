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

namespace Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Analysis;
    using Antlr3.Codegen;
    using Antlr3.Misc;
    using Antlr3.Tool;

    using File = System.IO.File;
    using FileInfo = System.IO.FileInfo;
    using IOException = System.IO.IOException;
    using Path = System.IO.Path;
    using Stats = Antlr.Runtime.Misc.Stats;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using StringReader = System.IO.StringReader;
    using StringWriter = System.IO.StringWriter;
    using TextWriter = System.IO.TextWriter;

    public class AntlrTool
    {
        public const string UninitializedDir = "<unset-dir>";

        private IList<string> grammarFileNames = new List<string>();
        private List<string> generatedFiles = new List<string>();
        private bool generate_NFA_dot = false;
        private bool generate_DFA_dot = false;
        private bool _generateDgmlGraphs = false;
        private string outputDirectory = ".";
        private bool haveOutputDir = false;
        private string inputDirectory;
        private string parentGrammarDirectory;
        private string grammarOutputDirectory;
        private bool haveInputDir = false;
        private string libDirectory = ".";
        private bool debug = false;
        private bool trace = false;
        private bool profile = false;
        private bool report = false;
        private bool printGrammar = false;
        private bool depend = false;
        private bool forceAllFilesToOutputDir = false;
        private bool forceRelativeOutput = false;
        private bool deleteTempLexer = true;
        private bool verbose = false;
        /** Don't process grammar file if generated files are newer than grammar */
        private bool make = false;
        private bool showBanner = true;
        // true when we are in a unit test
        private bool testMode = false;
        private bool _showTimer = false;
        private static bool exitNow = false;

        internal static bool EnableTemplateCache = false;

        // The internal options are for my use on the command line during dev
        //
        public static bool internalOption_PrintGrammarTree = false;
        public static bool internalOption_PrintDFA = false;
        public static bool internalOption_ShowNFAConfigsInDFA = false;
        public static bool internalOption_watchNFAConversion = false;

        readonly string[] GrammarExtensions =
            {
                ".g",
                ".g3"
            };

        [STAThread]
        public static void Main( string[] args )
        {
            if (args.Contains("-Xcachetemplates"))
                EnableTemplateCache = true;

            bool repeat = false;
            if (args.Contains("-Xrepeat"))
                repeat = true;

            for (int i = 0; i < (repeat ? 2 : 1); i++)
            {
                if (i == 1)
                    Console.In.ReadLine();

                AntlrTool antlr = new AntlrTool(args);
                if (!exitNow)
                {
                    antlr.Process();
                    Environment.ExitCode = (ErrorManager.GetNumErrors() > 0) ? 1 : 0;
                }
            }
        }

        static AntlrTool()
        {
            ToolPathRoot = Path.GetDirectoryName(typeof(CodeGenerator).Assembly.Location);
        }

        public AntlrTool()
            : this((string)null)
        {
        }

        public AntlrTool(string toolPathRoot)
        {
            if (!string.IsNullOrEmpty(toolPathRoot))
                ToolPathRoot = toolPathRoot;

            TargetsDirectory = Path.Combine(ToolPathRoot, @"Targets");
            TemplatesDirectory = Path.Combine(Path.Combine(ToolPathRoot, @"Codegen"), "Templates");
            ErrorManager.Initialize();
        }

        public AntlrTool( string[] args )
            : this()
        {
            ProcessArgs( args );
        }

        public static Version AssemblyVersion
        {
            get
            {
                var assembly = typeof(AntlrTool).Assembly;
                return assembly.GetName().Version;
            }
        }

        public static string ToolPathRoot
        {
            get;
            set;
        }

        public string TargetsDirectory
        {
            get;
            set;
        }

        public string TemplatesDirectory
        {
            get;
            set;
        }

        public string ForcedLanguageOption
        {
            get;
            set;
        }

        public virtual void ProcessArgs(string[] args)
        {
            if (verbose)
            {
                ErrorManager.Info("ANTLR Parser Generator  Version " + AssemblyVersion.ToString(4));
                showBanner = false;
            }

            if (args == null || args.Length == 0)
            {
                Help();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                case "-o":
                case "-fo":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing output directory with -fo/-o option; ignoring");
                    }
                    else
                    {
                        if (args[i] == "-fo")
                            ForceAllFilesToOutputDir = true;
                        i++;
                        outputDirectory = args[i];
                        if (outputDirectory.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) == outputDirectory.Length - 1)
                            outputDirectory = outputDirectory.Substring(0, OutputDirectory.Length - 1);

                        haveOutputDir = true;
                        if (System.IO.File.Exists(outputDirectory))
                        {
                            ErrorManager.Error(ErrorManager.MSG_OUTPUT_DIR_IS_FILE, outputDirectory);
                            LibraryDirectory = ".";
                        }
                    }

                    break;

                case "-lib":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing library directory with -lib option; ignoring");
                    }
                    else
                    {
                        i++;
                        LibraryDirectory = args[i];
                        if (LibraryDirectory.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) == LibraryDirectory.Length - 1)
                        {
                            LibraryDirectory = LibraryDirectory.Substring(0, LibraryDirectory.Length - 1);
                        }

                        if (!System.IO.Directory.Exists(libDirectory))
                        {
                            ErrorManager.Error(ErrorManager.MSG_DIR_NOT_FOUND, LibraryDirectory);
                            LibraryDirectory = ".";
                        }
                    }

                    break;

                case "-language":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing language name; ignoring");
                    }
                    else
                    {
                        i++;
                        ForcedLanguageOption = args[i];
                    }

                    break;

                case "-nfa":
                    Generate_NFA_dot = true;
                    break;

                case "-dfa":
                    Generate_DFA_dot = true;
                    break;

                case "-dgml":
                    GenerateDgmlGraphs = true;
                    break;

                case "-debug":
                    Debug = true;
                    break;

                case "-trace":
                    Trace = true;
                    break;

                case "-report":
                    Report = true;
                    break;

                case "-profile":
                    Profile = true;
                    break;

                case "-print":
                    PrintGrammar = true;
                    break;

                case "-depend":
                    Depend = true;
                    break;

                case "-testmode":
                    TestMode = true;
                    break;

                case "-verbose":
                    Verbose = true;
                    break;

                case "-version":
                    Version();
                    exitNow = true;
                    break;

                case "-make":
                    Make = true;
                    break;

                case "-message-format":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing output format with -message-format option; using default");
                    }
                    else
                    {
                        i++;
                        ErrorManager.SetFormat(args[i]);
                    }

                    break;

                case "-Xgrtree":
                    internalOption_PrintGrammarTree = true;
                    break;

                case "-Xdfa":
                    internalOption_PrintDFA = true;
                    break;

                case "-Xnoprune":
                    DFAOptimizer.PRUNE_EBNF_EXIT_BRANCHES = false;
                    break;

                case "-Xnocollapse":
                    DFAOptimizer.COLLAPSE_ALL_PARALLEL_EDGES = false;
                    break;

                case "-Xdbgconversion":
                    NFAToDFAConverter.debug = true;
                    break;

                case "-Xmultithreaded":
                    //NFAToDFAConverter.SINGLE_THREADED_NFA_CONVERSION = false;
                    Console.Error.WriteLine("Multithreaded NFA conversion is not currently supported.");
                    break;

                case "-Xnomergestopstates":
                    DFAOptimizer.MergeStopStates = false;
                    break;

                case "-Xdfaverbose":
                    internalOption_ShowNFAConfigsInDFA = true;
                    break;

                case "-Xwatchconversion":
                    internalOption_watchNFAConversion = true;
                    break;

                case "-XdbgST":
                    CodeGenerator.LaunchTemplateInspector = true;
                    break;

                case "-Xmaxinlinedfastates":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing max inline dfa states -Xmaxinlinedfastates option; ignoring");
                    }
                    else
                    {
                        i++;
                        CodeGenerator.MaxAcyclicDfaStatesInline = int.Parse(args[i]);
                    }

                    break;

                case "-Xmaxswitchcaselabels":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing max switch case labels -Xmaxswitchcaselabels option; ignoring");
                    }
                    else
                    {
                        i++;
                        int value;
                        if (int.TryParse(args[i], out value))
                            CodeGenerator.MaxSwitchCaseLabels = value;
                        else
                            Console.Error.WriteLine(string.Format("invalid value '{0}' for max switch case labels -Xmaxswitchcaselabels option; ignoring", args[i]));
                    }

                    break;

                case "-Xminswitchalts":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing min switch alternatives -Xminswitchalts option; ignoring");
                    }
                    else
                    {
                        i++;
                        int value;
                        if (int.TryParse(args[i], out value))
                            CodeGenerator.MinSwitchAlts = value;
                        else
                            Console.Error.WriteLine(string.Format("invalid value '{0}' for min switch alternatives -Xminswitchalts option; ignoring", args[i]));
                    }

                    break;

                case "-Xm":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing max recursion with -Xm option; ignoring");
                    }
                    else
                    {
                        i++;
                        NFAContext.MAX_SAME_RULE_INVOCATIONS_PER_NFA_CONFIG_STACK = int.Parse(args[i]);
                    }

                    break;

                case "-Xmaxdfaedges":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing max number of edges with -Xmaxdfaedges option; ignoring");
                    }
                    else
                    {
                        i++;
                        DFA.MAX_STATE_TRANSITIONS_FOR_TABLE = int.Parse(args[i]);
                    }

                    break;

                case "-Xconversiontimeout":
                    if (i + 1 >= args.Length)
                    {
                        Console.Error.WriteLine("missing max time in ms -Xconversiontimeout option; ignoring");
                    }
                    else
                    {
                        i++;
                        DFA.MAX_TIME_PER_DFA_CREATION = TimeSpan.FromMilliseconds(int.Parse(args[i]));
                    }

                    break;

                case "-Xnfastates":
                    DecisionProbe.verbose = true;
                    break;

                case "-Xsavelexer":
                    deleteTempLexer = false;
                    break;

                case "-Xtimer":
                    _showTimer = true;
                    break;

                case "-Xcachetemplates":
                    EnableTemplateCache = true;
                    break;

                case "-Xrepeat":
                    break;

                case "-X":
                    ExtendedHelp();
                    break;

                default:
                    if (args[i][0] != '-')
                    {
                        // Must be the grammar file
                        AddGrammarFile(args[i]);
                    }

                    break;
                }
            }
        }

#if false
        protected virtual void CheckForInvalidArguments( string[] args, Antlr.Runtime.BitSet cmdLineArgValid )
        {
            // check for invalid command line args
            for ( int a = 0; a < args.Length; a++ )
            {
                if ( !cmdLineArgValid.Member( a ) )
                {
                    Console.Error.WriteLine( "invalid command-line argument: " + args[a] + "; ignored" );
                }
            }
        }
#endif

        /**
         * Checks to see if the list of outputFiles all exist, and have
         * last-modified timestamps which are later than the last-modified
         * timestamp of all the grammar files involved in build the output
         * (imports must be checked). If these conditions hold, the method
         * returns false, otherwise, it returns true.
         *
         * @param grammarFileName The grammar file we are checking
         * @param outputFiles
         * @return
         */
        public virtual bool BuildRequired( string grammarFileName )
        {
            BuildDependencyGenerator bd = new BuildDependencyGenerator( this, grammarFileName );
            IList<string> outputFiles = bd.GetGeneratedFileList();
            IList<string> inputFiles = bd.GetDependenciesFileList();
            DateTime grammarLastModified = File.GetLastWriteTime( grammarFileName );
            foreach ( string outputFile in outputFiles )
            {
                if ( !File.Exists( outputFile ) || grammarLastModified > File.GetLastWriteTime( outputFile ) )
                {
                    // One of the output files does not exist or is out of date, so we must build it
                    if (Verbose)
                    {
                        if (!File.Exists(outputFile))
                            Console.Out.WriteLine("Output file " + outputFile + " does not exist: must build " + grammarFileName);
                        else
                            Console.Out.WriteLine("Output file " + outputFile + " is not up-to-date: must build " + grammarFileName);
                    }

                    return true;
                }

                // Check all of the imported grammars and see if any of these are younger
                // than any of the output files.
                if ( inputFiles != null )
                {
                    foreach ( string inputFile in inputFiles )
                    {
                        if ( File.GetLastWriteTime( inputFile ) > File.GetLastWriteTime( outputFile ) )
                        {
                            // One of the imported grammar files has been updated so we must build
                            if (Verbose)
                                Console.Out.WriteLine("Input file " + inputFile + " is newer than output: must rebuild " + grammarFileName);

                            return true;
                        }
                    }
                }
            }
            if ( Verbose )
            {
                Console.Out.WriteLine( "Grammar " + grammarFileName + " is up to date - build skipped" );
            }
            return false;
        }

        public virtual void Process()
        {
            bool exceptionWhenWritingLexerFile = false;
            string lexerGrammarFileName = null;		// necessary at this scope to have access in the catch below

            Stopwatch timer = Stopwatch.StartNew();

            // Have to be tricky here when Maven or build tools call in and must new Tool()
            // before setting options. The banner won't display that way!
            if ( Verbose && showBanner )
            {
                ErrorManager.Info( "ANTLR Parser Generator  Version " + AssemblyVersion.ToString(4) );
                showBanner = false;
            }

            try
            {
                SortGrammarFiles(); // update grammarFileNames
            }
            catch ( Exception e )
            {
                ErrorManager.Error( ErrorManager.MSG_INTERNAL_ERROR, e );
            }

            foreach ( string grammarFileName in GrammarFileNames )
            {
                // If we are in make mode (to support build tools like Maven) and the
                // file is already up to date, then we do not build it (and in verbose mode
                // we will say so).
                if ( Make )
                {
                    try
                    {
                        if ( !BuildRequired( grammarFileName ) )
                            continue;
                    }
                    catch ( Exception e )
                    {
                        ErrorManager.Error( ErrorManager.MSG_INTERNAL_ERROR, e );
                    }
                }

                if ( Verbose && !Depend )
                {
                    Console.Out.WriteLine( grammarFileName );
                }
                try
                {
                    if ( Depend )
                    {
                        BuildDependencyGenerator dep = new BuildDependencyGenerator( this, grammarFileName );
#if false
                        IList<string> outputFiles = dep.getGeneratedFileList();
                        IList<string> dependents = dep.getDependenciesFileList();
                        Console.Out.WriteLine( "output: " + outputFiles );
                        Console.Out.WriteLine( "dependents: " + dependents );
#endif
                        Console.Out.WriteLine( dep.GetDependencies().Render() );
                        continue;
                    }

                    Grammar rootGrammar = GetRootGrammar( grammarFileName );
                    // we now have all grammars read in as ASTs
                    // (i.e., root and all delegates)
                    rootGrammar.composite.AssignTokenTypes();
                    //rootGrammar.composite.TranslateLeftRecursiveRules();
                    rootGrammar.AddRulesForSyntacticPredicates();
                    rootGrammar.composite.DefineGrammarSymbols();
                    rootGrammar.composite.CreateNFAs();

                    GenerateRecognizer( rootGrammar );

                    if ( PrintGrammar )
                    {
                        rootGrammar.PrintGrammar( Console.Out );
                    }

                    if (Report)
                    {
                        GrammarReport2 greport = new GrammarReport2(rootGrammar);
                        Console.WriteLine(greport.ToString());
                    }

                    if ( Profile )
                    {
                        GrammarReport report = new GrammarReport(rootGrammar);
                        Stats.WriteReport( GrammarReport.GRAMMAR_STATS_FILENAME,
                                          report.ToNotifyString() );
                    }

                    // now handle the lexer if one was created for a merged spec
                    string lexerGrammarStr = rootGrammar.GetLexerGrammar();
                    //JSystem.@out.println("lexer grammar:\n"+lexerGrammarStr);
                    if ( rootGrammar.type == GrammarType.Combined && lexerGrammarStr != null )
                    {
                        lexerGrammarFileName = rootGrammar.ImplicitlyGeneratedLexerFileName;
                        try
                        {
                            TextWriter w = GetOutputFile( rootGrammar, lexerGrammarFileName );
                            w.Write( lexerGrammarStr );
                            w.Close();
                        }
                        catch (IOException)
                        {
                            // emit different error message when creating the implicit lexer fails
                            // due to write permission error
                            exceptionWhenWritingLexerFile = true;
                            throw;
                        }
                        try
                        {
                            StringReader sr = new StringReader( lexerGrammarStr );
                            Grammar lexerGrammar = new Grammar(this);
                            lexerGrammar.composite.WatchNFAConversion = internalOption_watchNFAConversion;
                            lexerGrammar.implicitLexer = true;
                            if ( TestMode )
                                lexerGrammar.DefaultRuleModifier = "public";
                            FileInfo lexerGrammarFullFile = new FileInfo( System.IO.Path.Combine( GetFileDirectory( lexerGrammarFileName ), lexerGrammarFileName ) );
                            lexerGrammar.FileName = lexerGrammarFullFile.ToString();

                            lexerGrammar.ImportTokenVocabulary( rootGrammar );
                            lexerGrammar.ParseAndBuildAST( sr );

                            sr.Close();

                            lexerGrammar.composite.AssignTokenTypes();
                            lexerGrammar.AddRulesForSyntacticPredicates();
                            lexerGrammar.composite.DefineGrammarSymbols();
                            lexerGrammar.composite.CreateNFAs();

                            GenerateRecognizer( lexerGrammar );
                        }
                        finally
                        {
                            // make sure we clean up
                            if ( deleteTempLexer )
                            {
                                System.IO.DirectoryInfo outputDir = GetOutputDirectory( lexerGrammarFileName );
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
                        ErrorManager.Error( ErrorManager.MSG_CANNOT_WRITE_FILE, e );
                    }
                    else
                    {
                        ErrorManager.Error( ErrorManager.MSG_CANNOT_OPEN_FILE,
                                           grammarFileName, e );
                    }
                }
                catch ( Exception e )
                {
                    ErrorManager.Error( ErrorManager.MSG_INTERNAL_ERROR, grammarFileName, e );
                }
#if false
                finally
                {
                    Console.Out.WriteLine( "creates=" + Interval.creates );
                    Console.Out.WriteLine( "hits=" + Interval.hits );
                    Console.Out.WriteLine( "misses=" + Interval.misses );
                    Console.Out.WriteLine( "outOfRange=" + Interval.outOfRange );
                }
#endif
            }

            if (_showTimer)
            {
                Console.WriteLine("Total parse time: {0}ms", timer.ElapsedMilliseconds);
            }
        }

        public virtual void SortGrammarFiles()
        {
            //Console.Out.WriteLine( "Grammar names " + GrammarFileNames );
            Graph<string> g = new Graph<string>();
            foreach ( string gfile in GrammarFileNames )
            {
                GrammarSpelunker grammar = new GrammarSpelunker( inputDirectory, gfile );
                grammar.Parse();
                string vocabName = grammar.TokenVocab;
                string grammarName = grammar.GrammarName;
                // Make all grammars depend on any tokenVocab options
                if ( vocabName != null )
                    g.AddEdge( gfile, vocabName + CodeGenerator.VocabFileExtension );
                // Make all generated tokens files depend on their grammars
                g.AddEdge( grammarName + CodeGenerator.VocabFileExtension, gfile );
            }
            List<string> sorted = g.Sort();
            //Console.Out.WriteLine( "sorted=" + sorted );
            GrammarFileNames.Clear(); // wipe so we can give new ordered list
            for ( int i = 0; i < sorted.Count; i++ )
            {
                string f = (string)sorted[i];
                if ( GrammarExtensions.Any( ext => f.EndsWith( ext, StringComparison.OrdinalIgnoreCase ) ) )
                    AddGrammarFile( f );
            }
            //Console.Out.WriteLine( "new grammars=" + grammarFileNames );
        }

        /** Get a grammar mentioned on the command-line and any delegates */
        public virtual Grammar GetRootGrammar( string grammarFileName )
        {
            //StringTemplate.setLintMode(true);
            // grammars mentioned on command line are either roots or single grammars.
            // create the necessary composite in case it's got delegates; even
            // single grammar needs it to get token types.
            CompositeGrammar composite = new CompositeGrammar();
            Grammar grammar = new Grammar( this, grammarFileName, composite );
            if ( TestMode )
                grammar.DefaultRuleModifier = "public";
            composite.SetDelegationRoot( grammar );

            string f = null;

            if ( haveInputDir )
            {
                f = Path.Combine( inputDirectory, grammarFileName );
            }
            else
            {
                f = grammarFileName;
            }

            // Store the location of this grammar as if we import files, we can then
            // search for imports in the same location as the original grammar as well as in
            // the lib directory.
            //
            parentGrammarDirectory = Path.GetDirectoryName( f );

            if ( grammarFileName.LastIndexOfAny( new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar } ) == -1 )
            {
                grammarOutputDirectory = ".";
            }
            else
            {
                grammarOutputDirectory = grammarFileName.Substring( 0, grammarFileName.LastIndexOfAny( new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar } ) );
            }

            StringReader reader = new StringReader( System.IO.File.ReadAllText( f ) );
            grammar.ParseAndBuildAST( reader );
            composite.WatchNFAConversion = internalOption_watchNFAConversion;
            return grammar;
        }

        /** Create NFA, DFA and generate code for grammar.
         *  Create NFA for any delegates first.  Once all NFA are created,
         *  it's ok to create DFA, which must check for left-recursion.  That check
         *  is done by walking the full NFA, which therefore must be complete.
         *  After all NFA, comes DFA conversion for root grammar then code gen for
         *  root grammar.  DFA and code gen for delegates comes next.
         */
        protected virtual void GenerateRecognizer( Grammar grammar )
        {
            string language = (string)grammar.GetOption( "language" );
            if ( language != null )
            {
                CodeGenerator generator = new CodeGenerator( this, grammar, language );
                grammar.CodeGenerator = generator;
                generator.Debug = Debug;
                generator.Profile = Profile;
                generator.Trace = Trace;

                // generate NFA early in case of crash later (for debugging)
                if ( Generate_NFA_dot )
                {
                    GenerateNFAs( grammar );
                }

                // GENERATE CODE
                generator.GenRecognizer();

                if ( Generate_DFA_dot )
                {
                    GenerateDFAs( grammar );
                }

                IList<Grammar> delegates = grammar.GetDirectDelegates();
                for ( int i = 0; delegates != null && i < delegates.Count; i++ )
                {
                    Grammar @delegate = (Grammar)delegates[i];
                    if ( @delegate != grammar )
                    {
                        // already processing this one
                        GenerateRecognizer( @delegate );
                    }
                }
            }
        }

        public virtual void GenerateDFAs( Grammar g )
        {
            for ( int d = 1; d <= g.NumberOfDecisions; d++ )
            {
                DFA dfa = g.GetLookaheadDFA( d );
                if ( dfa == null )
                {
                    continue; // not there for some reason, ignore
                }

                IGraphGenerator generator;
                if (GenerateDgmlGraphs)
                {
                    generator = new DgmlGenerator(g);
                }
                else
                {
                    generator = new DOTGenerator(g);
                }

                string graph = generator.GenerateGraph( dfa.StartState );
                string graphFileName = g.name + "." + "dec-" + d;
                if ( g.implicitLexer )
                {
                    graphFileName = g.name + Grammar.grammarTypeToFileNameSuffix[(int)g.type] + "." + "dec-" + d;
                }
                try
                {
                    WriteGraphFile( g, graphFileName, graph, generator.FileExtension );
                }
                catch ( IOException ioe )
                {
                    ErrorManager.Error( ErrorManager.MSG_CANNOT_GEN_DOT_FILE,
                                       graphFileName,
                                       ioe );
                }
            }
        }

        protected virtual void GenerateNFAs( Grammar g )
        {
            IGraphGenerator generator = GenerateDgmlGraphs ? (IGraphGenerator)new DgmlGenerator(g) : new DOTGenerator(g);
            HashSet<Rule> rules = g.GetAllImportedRules();
            rules.UnionWith( g.Rules );

            foreach ( Rule r in rules )
            {
                try
                {
                    string dot = generator.GenerateGraph( r.StartState );
                    if ( dot != null )
                    {
                        WriteGraphFile( g, r, dot, generator.FileExtension );
                    }
                }
                catch ( IOException ioe )
                {
                    ErrorManager.Error( ErrorManager.MSG_CANNOT_WRITE_FILE, ioe );
                }
            }
        }

        protected virtual void WriteGraphFile( Grammar g, Rule r, string graph, string formatExtension )
        {
            WriteGraphFile( g, r.Grammar.name + "." + r.Name, graph, formatExtension );
        }

        protected virtual void WriteGraphFile( Grammar g, string name, string graph, string formatExtension )
        {
            TextWriter fw = GetOutputFile( g, name + formatExtension );
            fw.Write( graph );
            fw.Close();
        }

        private static void Version()
        {
            ErrorManager.Info( "ANTLR Parser Generator  Version " + AntlrTool.AssemblyVersion.ToString(4) );
        }

        private static void Help()
        {
            Version();
            Console.Error.WriteLine( "usage: java org.antlr.Tool [args] file.g [file2.g file3.g ...]" );
            Console.Error.WriteLine( "  -o outputDir          specify output directory where all output is generated" );
            Console.Error.WriteLine( "  -fo outputDir         same as -o but force even files with relative paths to dir" );
            Console.Error.WriteLine( "  -lib dir              specify location of token files" );
            Console.Error.WriteLine( "  -depend               generate file dependencies" );
            Console.Error.WriteLine( "  -verbose              generate ANTLR version and other information" );
            Console.Error.WriteLine( "  -report               print out a report about the grammar(s) processed" );
            Console.Error.WriteLine( "  -print                print out the grammar without actions" );
            Console.Error.WriteLine( "  -debug                generate a parser that emits debugging events" );
            Console.Error.WriteLine( "  -trace                generate a recognizer that traces rule entry/exit" );
            Console.Error.WriteLine( "  -profile              generate a parser that computes profiling information" );
            Console.Error.WriteLine( "  -nfa                  generate an NFA for each rule" );
            Console.Error.WriteLine( "  -dfa                  generate a DFA for each decision point" );
            Console.Error.WriteLine( "  -dgml                 generate graphs in DGML format." );
            Console.Error.WriteLine( "  -message-format name  specify output style for messages" );
            Console.Error.WriteLine( "  -verbose              generate ANTLR version and other information" );
            Console.Error.WriteLine( "  -make                 only build if generated files older than grammar" );
            Console.Error.WriteLine( "  -version              print the version of ANTLR and exit." );
            Console.Error.WriteLine( "  -language L           override language grammar option; generate L" );
            Console.Error.WriteLine( "  -X                    display extended argument list" );
        }

        private static void ExtendedHelp()
        {
            Version();
            Console.Error.WriteLine("  -Xgrtree                print the grammar AST");
            Console.Error.WriteLine("  -Xdfa                   print DFA as text ");
            Console.Error.WriteLine("  -Xnoprune               test lookahead against EBNF block exit branches");
            Console.Error.WriteLine("  -Xnocollapse            collapse incident edges into DFA states");
            Console.Error.WriteLine("  -Xdbgconversion         dump lots of info during NFA conversion");
            Console.Error.WriteLine("  -Xmultithreaded         run the analysis in 2 threads");
            Console.Error.WriteLine("  -Xnomergestopstates     do not merge stop states");
            Console.Error.WriteLine("  -Xdfaverbose            generate DFA states in DOT with NFA configs");
            Console.Error.WriteLine("  -Xwatchconversion       print a message for each NFA before converting");
            Console.Error.WriteLine("  -XdbgST                 put tags at start/stop of all templates in output");
            Console.Error.WriteLine("  -Xnfastates             for nondeterminisms, list NFA states for each path");
            Console.Error.WriteLine("  -Xm m                   max number of rule invocations during conversion           [" + NFAContext.MAX_SAME_RULE_INVOCATIONS_PER_NFA_CONFIG_STACK + "]");
            Console.Error.WriteLine("  -Xmaxdfaedges m         max \"comfortable\" number of edges for single DFA state     [" + DFA.MAX_STATE_TRANSITIONS_FOR_TABLE + "]");
            Console.Error.WriteLine("  -Xmaxinlinedfastates m  max DFA states before table used rather than inlining      [" + CodeGenerator.DefaultMaxSwitchCaseLabels + "]");
            Console.Error.WriteLine("  -Xmaxswitchcaselabels m don't generate switch() statements for dfas bigger than m  [" + CodeGenerator.DefaultMaxSwitchCaseLabels + "]");
            Console.Error.WriteLine("  -Xminswitchalts m       don't generate switch() statements for dfas smaller than m [" + CodeGenerator.DefaultMinSwitchAlts + "]");
            Console.Error.WriteLine("  -Xsavelexer             don't delete temporary lexers generated from combined grammars");
        }

        /// <summary>
        /// Set the location (base directory) where output files should be produced by the ANTLR tool.
        /// </summary>
        /// <param name="outputDirectory"></param>
        public virtual void SetOutputDirectory( string outputDirectory )
        {
            haveOutputDir = true;
            this.outputDirectory = outputDirectory;
        }

        /**
         * Used by build tools to force the output files to always be
         * relative to the base output directory, even though the tool
         * had to set the output directory to an absolute path as it
         * cannot rely on the workign directory like command line invocation
         * can.
         *
         * @param forceRelativeOutput true if output files hould always be relative to base output directory
         */
        public virtual void SetForceRelativeOutput( bool forceRelativeOutput )
        {
            this.forceRelativeOutput = forceRelativeOutput;
        }

        /**
         * Set the base location of input files. Normally (when the tool is
         * invoked from the command line), the inputDirectory is not set, but
         * for build tools such as Maven, we need to be able to locate the input
         * files relative to the base, as the working directory could be anywhere and
         * changing workig directories is not a valid concept for JVMs because of threading and
         * so on. Setting the directory just means that the getFileDirectory() method will
         * try to open files relative to this input directory.
         *
         * @param inputDirectory Input source base directory
         */
        public virtual void SetInputDirectory( string inputDirectory )
        {
            this.inputDirectory = inputDirectory;
            haveInputDir = true;
        }

        public virtual TextWriter GetOutputFile( Grammar g, string fileName )
        {
            if ( OutputDirectory == null )
                return new StringWriter();

            // output directory is a function of where the grammar file lives
            // for subdir/T.g, you get subdir here.  Well, depends on -o etc...
            // But, if this is a .tokens file, then we force the output to
            // be the base output directory (or current directory if there is not a -o)
            //
#if false
            System.IO.DirectoryInfo outputDir;
            if ( fileName.EndsWith( CodeGenerator.VOCAB_FILE_EXTENSION ) )
            {
                if ( haveOutputDir )
                {
                    outputDir = new System.IO.DirectoryInfo( OutputDirectory );
                }
                else
                {
                    outputDir = new System.IO.DirectoryInfo( "." );
                }
            }
            else
            {
                outputDir = getOutputDirectory( g.FileName );
            }
#else
            System.IO.DirectoryInfo outputDir = GetOutputDirectory( g.FileName );
#endif
            FileInfo outputFile = new FileInfo( System.IO.Path.Combine( outputDir.FullName, fileName ) );

            if ( !outputDir.Exists )
                outputDir.Create();

            if ( outputFile.Exists )
                outputFile.Delete();

            GeneratedFiles.Add(outputFile.FullName);
            return new System.IO.StreamWriter( new System.IO.BufferedStream( outputFile.OpenWrite() ) );
        }

        /**
         * Return the location where ANTLR will generate output files for a given file. This is a
         * base directory and output files will be relative to here in some cases
         * such as when -o option is used and input files are given relative
         * to the input directory.
         *
         * @param fileNameWithPath path to input source
         * @return
         */
        public virtual System.IO.DirectoryInfo GetOutputDirectory( string fileNameWithPath )
        {
            string outputDir = OutputDirectory;

            if ( fileNameWithPath.IndexOfAny( System.IO.Path.GetInvalidPathChars() ) >= 0 )
                return new System.IO.DirectoryInfo( outputDir );

            if ( !System.IO.Path.IsPathRooted( fileNameWithPath ) )
                fileNameWithPath = System.IO.Path.GetFullPath( fileNameWithPath );

            string fileDirectory;
            // Some files are given to us without a PATH but should should
            // still be written to the output directory in the relative path of
            // the output directory. The file directory is either the set of sub directories
            // or just or the relative path recorded for the parent grammar. This means
            // that when we write the tokens files, or the .java files for imported grammars
            // taht we will write them in the correct place.
            //
            if ( fileNameWithPath.IndexOfAny( new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar } ) == -1 )
            {
                // No path is included in the file name, so make the file
                // directory the same as the parent grammar (which might sitll be just ""
                // but when it is not, we will write the file in the correct place.
                //
                fileDirectory = grammarOutputDirectory;
            }
            else
            {
                fileDirectory = fileNameWithPath.Substring( 0, fileNameWithPath.LastIndexOfAny( new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar } ) );
            }

            if ( haveOutputDir )
            {
                // -o /tmp /var/lib/t.g => /tmp/T.java
                // -o subdir/output /usr/lib/t.g => subdir/output/T.java
                // -o . /usr/lib/t.g => ./T.java
                if ( ( fileDirectory != null && !forceRelativeOutput ) &&
                     ( System.IO.Path.IsPathRooted( fileDirectory ) ||
                        fileDirectory.StartsWith( "~" ) ) || // isAbsolute doesn't count this :(
                        ForceAllFilesToOutputDir )
                {
                    // somebody set the dir, it takes precendence; write new file there
                    outputDir = OutputDirectory;
                }
                else
                {
                    // -o /tmp subdir/t.g => /tmp/subdir/t.g
                    if ( fileDirectory != null )
                    {
                        outputDir = System.IO.Path.Combine( OutputDirectory, fileDirectory );
                    }
                    else
                    {
                        outputDir = OutputDirectory;
                    }
                }
            }
            else
            {
                // they didn't specify a -o dir so just write to location
                // where grammar is, absolute or relative, this will only happen
                // with command line invocation as build tools will always
                // supply an output directory.
                //
                outputDir = fileDirectory;
            }
            return new System.IO.DirectoryInfo( outputDir );
        }

        /**
         * Name a file from the -lib dir.  Imported grammars and .tokens files
         *
         * If we do not locate the file in the library directory, then we try
         * the location of the originating grammar.
         *
         * @param fileName input name we are looking for
         * @return Path to file that we think shuold be the import file
         *
         * @throws java.io.IOException
         */
        public virtual string GetLibraryFile( string fileName )
        {
            // First, see if we can find the file in the library directory
            //
            string f = Path.Combine( LibraryDirectory, fileName );

            if ( File.Exists( f ) )
            {
                // Found in the library directory
                //
                return Path.GetFullPath( f );
            }

            // Need to assume it is in the same location as the input file. Note that
            // this is only relevant for external build tools and when the input grammar
            // was specified relative to the source directory (working directory if using
            // the command line.
            //
            return Path.Combine( parentGrammarDirectory, fileName );
        }

        /** Return the directory containing the grammar file for this grammar.
         *  normally this is a relative path from current directory.  People will
         *  often do "java org.antlr.Tool grammars/*.g3"  So the file will be
         *  "grammars/foo.g3" etc...  This method returns "grammars".
         *
         *  If we have been given a specific input directory as a base, then
         *  we must find the directory relative to this directory, unless the
         *  file name is given to us in absolute terms.
         */
        public virtual string GetFileDirectory( string fileName )
        {
            string f;
            if ( haveInputDir && !( fileName.StartsWith( Path.DirectorySeparatorChar.ToString() ) || fileName.StartsWith( Path.AltDirectorySeparatorChar.ToString() ) ) )
            {
                f = Path.Combine( inputDirectory, fileName );
            }
            else
            {
                f = fileName;
            }

            // And ask .NET what the base directory of this location is
            //
            return Path.GetDirectoryName( f );
        }

        /** Return a File descriptor for vocab file.  Look in library or
         *  in -o output path.  antlr -o foo T.g U.g where U needs T.tokens
         *  won't work unless we look in foo too. If we do not find the
         *  file in the lib directory then must assume that the .tokens file
         *  is going to be generated as part of this build and we have defined
         *  .tokens files so that they ALWAYS are generated in the base output
         *  directory, which means the current directory for the command line tool if there
         *  was no output directory specified.
         */
        public virtual string GetImportedVocabFile( string vocabName )
        {
            // first look at files we're generating
            string path = (from file in GeneratedFiles
                           where Path.GetFileName(file).Equals(vocabName + CodeGenerator.VocabFileExtension)
                             && File.Exists(file)
                           select file)
                          .FirstOrDefault();
            if (path != null)
                return path;

            path = Path.Combine( LibraryDirectory, vocabName + CodeGenerator.VocabFileExtension );
            if ( File.Exists( path ) )
                return path;

            // We did not find the vocab file in the lib directory, so we need
            // to look for it in the output directory which is where .tokens
            // files are generated (in the base, not relative to the input
            // location.)
            //
            if ( haveOutputDir )
            {
                path = Path.Combine( OutputDirectory, vocabName + CodeGenerator.VocabFileExtension );
            }
            else
            {
                path = vocabName + CodeGenerator.VocabFileExtension;
            }
            return path;
        }

        /** If the tool needs to panic/exit, how do we do that?
         */
        public virtual void Panic()
        {
            throw new Exception( "ANTLR panic" );
        }

        /// <summary>
        /// Return a time stamp string accurate to sec: yyyy-mm-dd hh:mm:ss
        /// </summary>
        public static string GetCurrentTimeStamp()
        {
            return DateTime.Now.ToString( "yyyy\\-MM\\-dd HH\\:mm\\:ss" );
        }

        /**
         * Provide the List of all grammar file names that the ANTLR tool will
         * process or has processed.
         *
         * @return the grammarFileNames
         */
        public virtual IList<string> GrammarFileNames
        {
            get
            {
                return grammarFileNames;
            }
        }

        public IList<string> GeneratedFiles
        {
            get
            {
                return generatedFiles;
            }
        }

        /**
         * Indicates whether ANTLR has gnerated or will generate a description of
         * all the NFAs in <a href="http://www.graphviz.org">Dot format</a>
         *
         * @return the generate_NFA_dot
         */
        public virtual bool Generate_NFA_dot
        {
            get
            {
                return generate_NFA_dot;
            }
            set
            {
                this.generate_NFA_dot = value;
            }
        }

        /**
         * Indicates whether ANTLR has generated or will generate a description of
         * all the NFAs in <a href="http://www.graphviz.org">Dot format</a>
         *
         * @return the generate_DFA_dot
         */
        public virtual bool Generate_DFA_dot
        {
            get
            {
                return generate_DFA_dot;
            }
            set
            {
                this.generate_DFA_dot = value;
            }
        }

        public virtual bool GenerateDgmlGraphs
        {
            get
            {
                return _generateDgmlGraphs;
            }

            set
            {
                _generateDgmlGraphs = value;
            }
        }

        /**
         * Return the Path to the base output directory, where ANTLR
         * will generate all the output files for the current language target as
         * well as any ancillary files such as .tokens vocab files.
         * 
         * @return the output Directory
         */
        public virtual string OutputDirectory
        {
            get
            {
                return outputDirectory;
            }
        }

        /**
         * Return the Path to the directory in which ANTLR will search for ancillary
         * files such as .tokens vocab files and imported grammar files.
         *
         * @return the lib Directory
         */
        public virtual string LibraryDirectory
        {
            get
            {
                return libDirectory;
            }
            set
            {
                this.libDirectory = value;
            }
        }

        /**
         * Indicate if ANTLR has generated, or will generate a debug version of the
         * recognizer. Debug versions of a parser communicate with a debugger such
         * as that contained in ANTLRWorks and at start up will 'hang' waiting for
         * a connection on an IP port (49100 by default).
         *
         * @return the debug flag
         */
        public virtual bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }

        /**
         * Indicate whether ANTLR has generated, or will generate a version of the
         * recognizer that prints trace messages on entry and exit of each rule.
         *
         * @return the trace flag
         */
        public virtual bool Trace
        {
            get
            {
                return trace;
            }
            set
            {
                trace = value;
            }
        }

        /**
         * Indicates whether ANTLR has generated or will generate a version of the
         * recognizer that gathers statistics about its execution, which it prints when
         * it terminates.
         *
         * @return the profile
         */
        public virtual bool Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
            }
        }

        /**
         * Indicates whether ANTLR has generated or will generate a report of various
         * elements of the grammar analysis, once it it has finished analyzing a grammar
         * file.
         *
         * @return the report flag
         */
        public virtual bool Report
        {
            get
            {
                return report;
            }
            set
            {
                report = value;
            }
        }

        /**
         * Indicates whether ANTLR has printed, or will print, a version of the input grammar
         * file(s) that is stripped of any action code embedded within.
         *
         * @return the printGrammar flag
         */
        public virtual bool PrintGrammar
        {
            get
            {
                return printGrammar;
            }
            set
            {
                printGrammar = value;
            }
        }

        /**
         * Indicates whether ANTLR has supplied, or will supply, a list of all the things
         * that the input grammar depends upon and all the things that will be generated
         * when that grammar is successfully analyzed.
         *
         * @return the depend flag
         */
        public virtual bool Depend
        {
            get
            {
                return depend;
            }
            set
            {
                depend = value;
            }
        }

        public virtual bool TestMode
        {
            get
            {
                return testMode;
            }
            set
            {
                testMode = value;
            }
        }

        /**
         * Indicates whether ANTLR will force all files to the output directory, even
         * if the input files have relative paths from the input directory.
         *
         * @return the forceAllFilesToOutputDir flag
         */
        public virtual bool ForceAllFilesToOutputDir
        {
            get
            {
                return forceAllFilesToOutputDir;
            }
            set
            {
                forceAllFilesToOutputDir = value;
            }
        }

        /**
         * Indicates whether ANTLR will be verbose when analyzing grammar files, such as
         * displaying the names of the files it is generating and similar information.
         *
         * @return the verbose flag
         */
        public virtual bool Verbose
        {
            get
            {
                return verbose;
            }
            set
            {
                verbose = value;
            }
        }

        /**
         * Gets or sets the current setting of the message format descriptor.
         */
        public virtual string MessageFormat
        {
            get
            {
                return ErrorManager.GetMessageFormat().ToString();
            }
            set
            {
                ErrorManager.SetFormat( value );
            }
        }

        /**
         * Returns the number of errors that the analysis/processing threw up.
         * @return Error count
         */
        public virtual int NumErrors
        {
            get
            {
                return ErrorManager.GetNumErrors();
            }
        }

        /**
         * Indicate whether the tool will analyze the dependencies of the provided grammar
         * file list and ensure that grammars with dependencies are built
         * after any of the other gramamrs in the list that they are dependent on. Setting
         * this option also has the side effect that any grammars that are includes for other
         * grammars in the list are excluded from individual analysis, which allows the caller
         * to invoke the tool via org.antlr.tool -make *.g and not worry about the inclusion
         * of grammars that are just includes for other grammars or what order the grammars
         * appear on the command line.
         *
         * This option was coded to make life easier for tool integration (such as Maven) but
         * may also be useful at the command line.
         *
         * @return true if the tool is currently configured to analyze and sort grammar files.
         */
        public virtual bool Make
        {
            get
            {
                return make;
            }
            set
            {
                make = value;
            }
        }

        public virtual void AddGrammarFile( string grammarFileName )
        {
            if ( !GrammarFileNames.Contains( grammarFileName ) )
                GrammarFileNames.Add( grammarFileName );
        }
    }
}
