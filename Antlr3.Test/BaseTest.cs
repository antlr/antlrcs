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

namespace AntlrUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime;
    using Antlr.Runtime.JavaExtensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AntlrTool = Antlr3.AntlrTool;
    using BindingFlags = System.Reflection.BindingFlags;
    using Debugger = System.Diagnostics.Debugger;
    using Directory = System.IO.Directory;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using FieldInfo = System.Reflection.FieldInfo;
    using GrammarSemanticsMessage = Antlr3.Tool.GrammarSemanticsMessage;
    using IANTLRErrorListener = Antlr3.Tool.IANTLRErrorListener;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using Label = Antlr3.Analysis.Label;
    using Message = Antlr3.Tool.Message;
    using Path = System.IO.Path;
    using Registry = Microsoft.Win32.Registry;
    using RegistryKey = Microsoft.Win32.RegistryKey;
    using RegistryValueOptions = Microsoft.Win32.RegistryValueOptions;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;

    [TestClass]
    public abstract class BaseTest
    {
        public readonly string jikes = null;
        public static readonly string pathSep = System.IO.Path.PathSeparator.ToString();
        public readonly string RuntimeJar = Path.Combine( Environment.CurrentDirectory, @"..\..\antlr-runtime-3.3.jar" );
        public readonly string Runtime2Jar = Path.Combine( Environment.CurrentDirectory, @"..\..\antlr-2.7.7.jar" );
        public readonly string StringTemplateJar = Path.Combine( Environment.CurrentDirectory, @"..\..\stringtemplate-3.2.1.jar" );

        private static string javaHome;

        public string tmpdir;

        public TestContext TestContext
        {
            get;
            set;
        }

        /** If error during parser execution, store stderr here; can't return
         *  stdout and stderr.  This doesn't trap errors from running antlr.
         */
        protected string stderrDuringParse;

        public static readonly string NewLine = Environment.NewLine;

        [ClassInitialize]
        public void ClassSetUp( TestContext testContext )
        {
            TestContext = testContext;
        }

        public static long currentTimeMillis()
        {
            return DateTime.Now.ToFileTime() / 10000;
        }

        [TestInitialize]
        public void setUp()
        {
#if DEBUG
            string configuration = "Debug";
#else
            string configuration = "Release";
#endif
            System.IO.DirectoryInfo currentAssemblyDirectory = new System.IO.FileInfo(typeof(BaseTest).Assembly.Location).Directory;
            AntlrTool.ToolPathRoot = Path.Combine(currentAssemblyDirectory.Parent.Parent.Parent.FullName, @"bin\" + configuration);

            // new output dir for each test
            tmpdir = Path.GetFullPath( Path.Combine( Path.GetTempPath(), "antlr-" + currentTimeMillis() ) );

            ErrorManager.ResetErrorState();

            // force reset of static caches
            StringTemplateGroup.ResetNameMaps();

            StringTemplate.ResetTemplateCounter();
            StringTemplate.defaultGroup = new StringTemplateGroup( "defaultGroup", "." );

            // verify token constants in StringTemplate
            VerifyImportedTokens( typeof( Antlr3.ST.Language.ActionParser ), typeof( Antlr3.ST.Language.ActionLexer ) );
            VerifyImportedTokens( typeof( Antlr3.ST.Language.ActionParser ), typeof( Antlr3.ST.Language.ActionEvaluator ) );
            VerifyImportedTokens( typeof( Antlr3.ST.Language.TemplateParser ), typeof( Antlr3.ST.Language.TemplateLexer ) );
            VerifyImportedTokens( typeof( Antlr3.ST.Language.TemplateParser ), typeof( Antlr3.ST.Language.AngleBracketTemplateLexer ) );

            // verify token constants in the ANTLR Tool
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.ANTLRLexer ) );
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.ANTLRTreePrinter ) );
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.AssignTokenTypesWalker ) );
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.CodeGenTreeWalker ) );
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.DefineGrammarItemsWalker ) );
            VerifyImportedTokens( typeof( Antlr3.Grammars.ANTLRParser ), typeof( Antlr3.Grammars.TreeToNFAConverter ) );
        }

        [TestCleanup]
        public void tearDown()
        {
            // remove tmpdir if no error. how?
            if ( TestContext != null && TestContext.CurrentTestOutcome == UnitTestOutcome.Passed )
                eraseTempDir();
        }

        private void VerifyImportedTokens( Type source, Type target )
        {
            FieldInfo namesField = source.GetField( "tokenNames", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            FieldInfo targetNamesField = target.GetField( "tokenNames", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            Assert.IsNotNull( namesField, string.Format( "No tokenNames field was found in grammar {0}.", source.Name ) );

            string[] sourceNames = namesField.GetValue( null ) as string[];
            string[] targetNames = ( targetNamesField != null ) ? targetNamesField.GetValue( null ) as string[] : null;
            Assert.IsNotNull( sourceNames, string.Format( "The tokenNames field in grammar {0} was null.", source.Name ) );

            for ( int i = 0; i < sourceNames.Length; i++ )
            {
                string tokenName = sourceNames[i];
                if ( string.IsNullOrEmpty( tokenName ) || tokenName[0] == '<' )
                    continue;

                if ( tokenName[0] == '\'' )
                {
                    if ( targetNames != null && targetNames.Length > i )
                    {
                        // make sure implicit tokens like 'new' that show up in code as T__45 refer to the same token
                        Assert.AreEqual(
                            sourceNames[i],
                            targetNames[i],
                            string.Format( "Implicit token {0} in grammar {1} doesn't match {2} in grammar {3}.", sourceNames[i], source.Name, targetNames[i], target.Name )
                            );

                        continue;
                    }
                    else
                    {
                        tokenName = "T__" + i.ToString();
                    }
                }

                FieldInfo sourceToken = source.GetField( tokenName );
                FieldInfo targetToken = target.GetField( tokenName );
                if ( source != null && target != null )
                {
                    int sourceValue = (int)sourceToken.GetValue( null );
                    int targetValue = (int)targetToken.GetValue( null );
                    Assert.AreEqual(
                        sourceValue,
                        targetValue,
                        string.Format( "Token {0} with value {1} grammar {2} doesn't match value {3} in grammar {4}.", tokenName, sourceValue, source.Name, targetValue, target.Name )
                        );
                }
            }
        }

        protected AntlrTool newTool(params string[] args)
        {
            AntlrTool tool = (args == null || args.Length == 0) ? new AntlrTool() : new AntlrTool(args);
            tool.SetOutputDirectory( tmpdir );
            tool.TestMode = true;
            return tool;
        }

        protected static string JavaHome
        {
            get
            {
                string home = javaHome;
                bool debugger = Debugger.IsAttached;
                if (home == null || debugger)
                {
                    home = Environment.GetEnvironmentVariable("JAVA_HOME");
                    if (string.IsNullOrEmpty(home) || !Directory.Exists(home))
                    {
                        home = CheckForJavaHome(Registry.CurrentUser);
                        if (home == null)
                            home = CheckForJavaHome(Registry.LocalMachine);
                    }

                    if (home != null && !Directory.Exists(home))
                        home = null;

                    if (!debugger)
                    {
                        javaHome = home;
                    }
                }

                return home;
            }
        }

        protected static string CheckForJavaHome(RegistryKey key)
        {
            using (RegistryKey subkey = key.OpenSubKey(@"SOFTWARE\JavaSoft\Java Development Kit"))
            {
                if (subkey == null)
                    return null;

                object value = subkey.GetValue("CurrentVersion", null, RegistryValueOptions.None);
                if (value != null)
                {
                    using (RegistryKey currentHomeKey = subkey.OpenSubKey(value.ToString()))
                    {
                        if (currentHomeKey == null)
                            return null;

                        value = currentHomeKey.GetValue("JavaHome", null, RegistryValueOptions.None);
                        if (value != null)
                            return value.ToString();
                    }
                }
            }

            return null;
        }

        protected string ClassPath
        {
            get
            {
                return Path.GetFullPath( RuntimeJar )
                    + Path.PathSeparator + Path.GetFullPath( Runtime2Jar )
                    + Path.PathSeparator + Path.GetFullPath( StringTemplateJar );
            }
        }

        protected string Compiler
        {
            get
            {
                return Path.Combine( Path.Combine( JavaHome, "bin" ), "javac.exe" );
            }
        }

        protected string Jvm
        {
            get
            {
                return Path.Combine( Path.Combine( JavaHome, "bin" ), "java.exe" );
            }
        }

        protected bool compile( string fileName )
        {
            //String compiler = "javac";
            string compiler = Compiler;
            string classpathOption = "-classpath";

            if ( jikes != null )
            {
                compiler = jikes;
                classpathOption = "-bootclasspath";
            }

            string inputFile = Path.Combine(tmpdir, fileName);
            string[] args = new string[]
                {
                    /*compiler,*/
                    "-d",
                    tmpdir,
                    classpathOption,
                    tmpdir+pathSep+ClassPath,
                    inputFile
                };
            string cmdLine = compiler + " -d " + tmpdir + " " + classpathOption + " " + '"'+tmpdir + pathSep + ClassPath+'"' + " " + fileName;
            //System.out.println("compile: "+cmdLine);
            //File outputDir = new File( tmpdir );
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.Start( new System.Diagnostics.ProcessStartInfo( compiler, '"' + string.Join( "\" \"", args ) + '"' )
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = tmpdir
                } );

                //Process process =
                //    Runtime.getRuntime().exec( args, null, outputDir );

                StreamVacuum stdout = new StreamVacuum( process.StandardOutput, inputFile );
                StreamVacuum stderr = new StreamVacuum( process.StandardError, inputFile );
                stdout.start();
                stderr.start();
                process.WaitForExit();
                if ( stdout.ToString().Length > 0 )
                {
                    Console.Error.WriteLine( "compile stdout from: " + cmdLine );
                    Console.Error.WriteLine( stdout );
                }
                if ( stderr.ToString().Length > 0 )
                {
                    Console.Error.WriteLine( "compile stderr from: " + cmdLine );
                    Console.Error.WriteLine( stderr );
                }
                int ret = process.ExitCode;
                return ret == 0;
            }
            catch ( Exception e )
            {
                Console.Error.WriteLine( "can't exec compilation" );
                e.PrintStackTrace( Console.Error );
                return false;
            }
        }

        /** Return true if all is ok, no errors */
        protected bool antlr( string fileName, string grammarFileName, string grammarStr, bool debug )
        {
            bool allIsWell = true;
            mkdir( tmpdir );
            writeFile( tmpdir, fileName, grammarStr );
            try
            {
                List<string> options = new List<string>();
                options.Add( "-testmode" );
                if ( debug )
                {
                    options.Add( "-debug" );
                }
                options.Add( "-o" );
                options.Add( tmpdir );
                options.Add( "-lib" );
                options.Add( tmpdir );
                options.Add( Path.Combine( tmpdir, grammarFileName ) );
                options.Add("-language");
                options.Add("Java");
                //String[] optionsA = new String[options.size()];
                //options.toArray( optionsA );
                string[] optionsA = options.ToArray();
                /*
                final ErrorQueue equeue = new ErrorQueue();
                ErrorManager.setErrorListener(equeue);
                */
                AntlrTool antlr = new AntlrTool( optionsA );
                antlr.Process();
                IANTLRErrorListener listener = ErrorManager.GetErrorListener();
                if ( listener is ErrorQueue )
                {
                    ErrorQueue equeue = (ErrorQueue)listener;
                    if ( equeue.errors.Count > 0 )
                    {
                        allIsWell = false;
                        Console.Error.WriteLine( "antlr reports errors from " + options );
                        for ( int i = 0; i < equeue.errors.Count; i++ )
                        {
                            Message msg = (Message)equeue.errors[i];
                            Console.Error.WriteLine( msg );
                        }
                        Console.Out.WriteLine( "!!!\ngrammar:" );
                        Console.Out.WriteLine( grammarStr );
                        Console.Out.WriteLine( "###" );
                    }
                }
            }
            catch ( Exception e )
            {
                allIsWell = false;
                Console.Error.WriteLine( "problems building grammar: " + e );
                e.PrintStackTrace( Console.Error );
            }
            return allIsWell;
        }

        protected string execLexer( string grammarFileName,
                                   string grammarStr,
                                   string lexerName,
                                   string input,
                                   bool debug )
        {
            rawGenerateAndBuildRecognizer( grammarFileName,
                                          grammarStr,
                                          null,
                                          lexerName,
                                          debug );
            writeFile( tmpdir, "input", input );
            return rawExecRecognizer( null,
                                     null,
                                     lexerName,
                                     null,
                                     null,
                                     false,
                                     false,
                                     false,
                                     debug );
        }

        protected string execParser( string grammarFileName,
                                    string grammarStr,
                                    string parserName,
                                    string lexerName,
                                    string startRuleName,
                                    string input, bool debug )
        {
            rawGenerateAndBuildRecognizer( grammarFileName,
                                          grammarStr,
                                          parserName,
                                          lexerName,
                                          debug );
            writeFile( tmpdir, "input", input );
            bool parserBuildsTrees =
                grammarStr.IndexOf( "output=AST" ) >= 0 ||
                grammarStr.IndexOf( "output = AST" ) >= 0;
            bool parserBuildsTemplate =
                grammarStr.IndexOf( "output=template" ) >= 0 ||
                grammarStr.IndexOf( "output = template" ) >= 0;
            return rawExecRecognizer( parserName,
                                     null,
                                     lexerName,
                                     startRuleName,
                                     null,
                                     parserBuildsTrees,
                                     parserBuildsTemplate,
                                     false,
                                     debug );
        }

        protected string execTreeParser( string parserGrammarFileName,
                                        string parserGrammarStr,
                                        string parserName,
                                        string treeParserGrammarFileName,
                                        string treeParserGrammarStr,
                                        string treeParserName,
                                        string lexerName,
                                        string parserStartRuleName,
                                        string treeParserStartRuleName,
                                        string input )
        {
            return execTreeParser( parserGrammarFileName,
                                  parserGrammarStr,
                                  parserName,
                                  treeParserGrammarFileName,
                                  treeParserGrammarStr,
                                  treeParserName,
                                  lexerName,
                                  parserStartRuleName,
                                  treeParserStartRuleName,
                                  input,
                                  false );
        }

        protected string execTreeParser( string parserGrammarFileName,
                                        string parserGrammarStr,
                                        string parserName,
                                        string treeParserGrammarFileName,
                                        string treeParserGrammarStr,
                                        string treeParserName,
                                        string lexerName,
                                        string parserStartRuleName,
                                        string treeParserStartRuleName,
                                        string input,
                                        bool debug )
        {
            // build the parser
            rawGenerateAndBuildRecognizer( parserGrammarFileName,
                                          parserGrammarStr,
                                          parserName,
                                          lexerName,
                                          debug );

            // build the tree parser
            rawGenerateAndBuildRecognizer( treeParserGrammarFileName,
                                          treeParserGrammarStr,
                                          treeParserName,
                                          lexerName,
                                          debug );

            writeFile( tmpdir, "input", input );

            bool parserBuildsTrees =
                parserGrammarStr.IndexOf( "output=AST" ) >= 0 ||
                parserGrammarStr.IndexOf( "output = AST" ) >= 0;
            bool treeParserBuildsTrees =
                treeParserGrammarStr.IndexOf( "output=AST" ) >= 0 ||
                treeParserGrammarStr.IndexOf( "output = AST" ) >= 0;
            bool parserBuildsTemplate =
                parserGrammarStr.IndexOf( "output=template" ) >= 0 ||
                parserGrammarStr.IndexOf( "output = template" ) >= 0;

            return rawExecRecognizer( parserName,
                                     treeParserName,
                                     lexerName,
                                     parserStartRuleName,
                                     treeParserStartRuleName,
                                     parserBuildsTrees,
                                     parserBuildsTemplate,
                                     treeParserBuildsTrees,
                                     debug );
        }

        /** Return true if all is well */
        protected bool rawGenerateAndBuildRecognizer( string grammarFileName,
                                                        string grammarStr,
                                                        string parserName,
                                                        string lexerName,
                                                        bool debug )
        {
            bool allIsWell =
                antlr( grammarFileName, grammarFileName, grammarStr, debug );
            if ( lexerName != null )
            {
                bool ok;
                if ( parserName != null )
                {
                    ok = compile( parserName + ".java" );
                    if ( !ok )
                    {
                        allIsWell = false;
                    }
                }
                ok = compile( lexerName + ".java" );
                if ( !ok )
                {
                    allIsWell = false;
                }
            }
            else
            {
                bool ok = compile( parserName + ".java" );
                if ( !ok )
                {
                    allIsWell = false;
                }
            }
            return allIsWell;
        }

        protected string rawExecRecognizer( string parserName,
                                           string treeParserName,
                                           string lexerName,
                                           string parserStartRuleName,
                                           string treeParserStartRuleName,
                                           bool parserBuildsTrees,
                                           bool parserBuildsTemplate,
                                           bool treeParserBuildsTrees,
                                           bool debug )
        {
            stderrDuringParse = null;
            WriteRecognizerAndCompile(parserName, treeParserName, lexerName, parserStartRuleName, treeParserStartRuleName, parserBuildsTrees, parserBuildsTemplate, treeParserBuildsTrees, debug);

            return execRecognizer();
        }

        public string execRecognizer()
        {
            try
            {
                string inputFile = Path.GetFullPath(Path.Combine(tmpdir, "input"));
                string[] args = new string[] {
                        /*"java",*/ "-classpath", tmpdir+pathSep+ClassPath,
                        "Test", inputFile
                    };
                //String cmdLine = "java -classpath " + CLASSPATH + pathSep + tmpdir + " Test " + Path.GetFullPath( Path.Combine( tmpdir, "input" ) );
                //System.out.println("execParser: "+cmdLine);

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Jvm, '"' + string.Join("\" \"", args) + '"')
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = tmpdir
                });

                //Process process =
                //    Runtime.getRuntime().exec( args, null, new File( tmpdir ) );
                StreamVacuum stdoutVacuum = new StreamVacuum(process.StandardOutput, inputFile);
                StreamVacuum stderrVacuum = new StreamVacuum(process.StandardError, inputFile);
                stdoutVacuum.start();
                stderrVacuum.start();
                process.WaitForExit();
                stdoutVacuum.join();
                stderrVacuum.join();
                string output = null;
                output = stdoutVacuum.ToString();
                if (stderrVacuum.ToString().Length > 0)
                {
                    this.stderrDuringParse = stderrVacuum.ToString();
                    //Console.Error.WriteLine( "exec stderrVacuum: " + stderrVacuum );
                }
                return output;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("can't exec recognizer");
                e.PrintStackTrace(Console.Error);
            }
            return null;
        }

        public void WriteRecognizerAndCompile(
            string parserName,
            string treeParserName,
            string lexerName,
            string parserStartRuleName,
            string treeParserStartRuleName,
            bool parserBuildsTrees,
            bool parserBuildsTemplate,
            bool treeParserBuildsTrees,
            bool debug)
        {
            if (treeParserBuildsTrees && parserBuildsTrees)
            {
                writeTreeAndTreeTestFile( parserName,
                                         treeParserName,
                                         lexerName,
                                         parserStartRuleName,
                                         treeParserStartRuleName,
                                         debug );
            }
            else if ( parserBuildsTrees )
            {
                writeTreeTestFile( parserName,
                                  treeParserName,
                                  lexerName,
                                  parserStartRuleName,
                                  treeParserStartRuleName,
                                  debug );
            }
            else if ( parserBuildsTemplate )
            {
                writeTemplateTestFile( parserName,
                                      lexerName,
                                      parserStartRuleName,
                                      debug );
            }
            else if ( parserName == null )
            {
                writeLexerTestFile( lexerName, debug );
            }
            else
            {
                writeTestFile( parserName,
                              lexerName,
                              parserStartRuleName,
                              debug );
            }

            compile( "Test.java" );
        }

        protected void checkGrammarSemanticsError( ErrorQueue equeue,
                                                  GrammarSemanticsMessage expectedMessage )
        //throws Exception
        {
            /*
                    System.out.println(equeue.infos);
                    System.out.println(equeue.warnings);
                    System.out.println(equeue.errors);
                    Assert.IsTrue("number of errors mismatch", n, equeue.errors.size());
                               */
            Message foundMsg = null;
            for ( int i = 0; i < equeue.errors.Count; i++ )
            {
                Message m = (Message)equeue.errors[i];
                if ( m.msgID == expectedMessage.msgID )
                {
                    foundMsg = m;
                }
            }

            Assert.IsNotNull( foundMsg, "no error; " + expectedMessage.msgID + " expected" );
            Assert.IsTrue( foundMsg is GrammarSemanticsMessage, "error is not a GrammarSemanticsMessage" );
            Assert.AreEqual( expectedMessage.arg, foundMsg.arg );
            if ( equeue.size() != 1 )
            {
                Console.Error.WriteLine( equeue );
            }
        }

        protected void checkGrammarSemanticsWarning( ErrorQueue equeue,
                                                    GrammarSemanticsMessage expectedMessage )
        //throws Exception
        {
            Message foundMsg = null;
            for ( int i = 0; i < equeue.warnings.Count; i++ )
            {
                Message m = (Message)equeue.warnings[i];
                if ( m.msgID == expectedMessage.msgID )
                {
                    foundMsg = m;
                }
            }
            Assert.IsNotNull( foundMsg, "no error; " + expectedMessage.msgID + " expected" );
            Assert.IsTrue( foundMsg is GrammarSemanticsMessage, "error is not a GrammarSemanticsMessage" );
            Assert.AreEqual( expectedMessage.arg, foundMsg.arg );
        }

        protected void checkError( ErrorQueue equeue,
                                  Message expectedMessage )
        //throws Exception
        {
            //System.out.println("errors="+equeue);
            Message foundMsg = null;
            for ( int i = 0; i < equeue.errors.Count; i++ )
            {
                Message m = (Message)equeue.errors[i];
                if ( m.msgID == expectedMessage.msgID )
                {
                    foundMsg = m;
                }
            }
            Assert.IsTrue(equeue.errors.Count > 0, "no error; " + expectedMessage.msgID + " expected");
            Assert.IsTrue(equeue.errors.Count <= 1, "too many errors; " + equeue.errors);
            Assert.IsNotNull(foundMsg, "couldn't find expected error: " + expectedMessage.msgID);
            /*
            Assert.IsTrue("error is not a GrammarSemanticsMessage",
                       foundMsg instanceof GrammarSemanticsMessage);
             */
            Assert.AreEqual( expectedMessage.arg, foundMsg.arg );
            Assert.AreEqual( expectedMessage.arg2, foundMsg.arg2 );
            ErrorManager.ResetErrorState(); // wack errors for next test
        }

        public /*static*/ class StreamVacuum //: Runnable
        {
            StringBuilder buf = new StringBuilder();
            System.IO.TextReader @in;
            System.Threading.Thread sucker;
            string inputFile;
            public StreamVacuum( System.IO.StreamReader @in, string inputFile )
            {
                this.@in = @in;
                this.inputFile = inputFile;
            }
            public void start()
            {
                sucker = new System.Threading.Thread( run );
                sucker.Start();
            }
            public void run()
            {
                try
                {
                    string line = @in.ReadLine();
                    while ( line != null )
                    {
                        if (line.StartsWith(inputFile))
                            line = line.Substring(inputFile.Length + 1);

                        buf.AppendLine( line );
                        //buf.append( '\n' );
                        line = @in.ReadLine();
                    }
                }
                catch ( IOException /*ioe*/ )
                {
                    Console.Error.WriteLine( "can't read output from process" );
                }
            }
            /** wait for the thread to finish */
            public void join() /*throws InterruptedException*/ {
                sucker.Join();
            }
            public override string ToString()
            {
                return buf.ToString();
            }
        }

        protected void writeFile( string dir, string fileName, string content )
        {
            try
            {
                System.IO.File.WriteAllText( Path.Combine( dir, fileName ), content );
                //File f = new File( dir, fileName );
                //FileWriter w = new FileWriter( f );
                //BufferedWriter bw = new BufferedWriter( w );
                //bw.write( content );
                //bw.close();
                //w.close();
            }
            catch ( IOException ioe )
            {
                Console.Error.WriteLine( "can't write file" );
                ioe.PrintStackTrace( Console.Error );
            }
        }

        protected void mkdir( string dir )
        {
            System.IO.Directory.CreateDirectory( dir );
            //File f = new File( dir );
            //f.mkdirs();
        }

        protected void writeTestFile( string parserName,
                                     string lexerName,
                                     string parserStartRuleName,
                                     bool debug )
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        $createParser$\n" +
                "        parser.$parserStartRuleName$();\n" +
                "    }\n" +
                "}"
                );
            StringTemplate createParserST =
                new StringTemplate(
                "        Profiler2 profiler = new Profiler2();\n" +
                "        $parserName$ parser = new $parserName$(tokens,profiler);\n" +
                "        profiler.setParser(parser);\n" );
            if ( !debug )
            {
                createParserST =
                    new StringTemplate(
                    "        $parserName$ parser = new $parserName$(tokens);\n" );
            }
            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            writeFile( tmpdir, "Test.java", outputFileST.ToString() );
        }

        protected void writeLexerTestFile( string lexerName, bool debug )
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        System.out.println(tokens);\n" +
                "    }\n" +
                "}"
                );
            outputFileST.SetAttribute( "lexerName", lexerName );
            writeFile( tmpdir, "Test.java", outputFileST.ToString() );
        }

        protected void writeTreeTestFile( string parserName,
                                         string treeParserName,
                                         string lexerName,
                                         string parserStartRuleName,
                                         string treeParserStartRuleName,
                                         bool debug )
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        TokenRewriteStream tokens = new TokenRewriteStream(lex);\n" +
                "        $createParser$\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        $if(!treeParserStartRuleName)$\n" +
                "        if ( r.tree!=null ) {\n" +
                "            System.out.println(((Tree)r.tree).toStringTree());\n" +
                "            ((CommonTree)r.tree).sanityCheckParentAndChildIndexes();\n" +
                "		 }\n" +
                "        $else$\n" +
                "        CommonTreeNodeStream nodes = new CommonTreeNodeStream((Tree)r.tree);\n" +
                "        nodes.setTokenStream(tokens);\n" +
                "        $treeParserName$ walker = new $treeParserName$(nodes);\n" +
                "        walker.$treeParserStartRuleName$();\n" +
                "        $endif$\n" +
                "    }\n" +
                "}"
                );
            StringTemplate createParserST =
                new StringTemplate(
                "        Profiler2 profiler = new Profiler2();\n" +
                "        $parserName$ parser = new $parserName$(tokens,profiler);\n" +
                "        profiler.setParser(parser);\n" );
            if ( !debug )
            {
                createParserST =
                    new StringTemplate(
                    "        $parserName$ parser = new $parserName$(tokens);\n" );
            }
            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "treeParserName", treeParserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            outputFileST.SetAttribute( "treeParserStartRuleName", treeParserStartRuleName );
            writeFile( tmpdir, "Test.java", outputFileST.ToString() );
        }

        /** Parser creates trees and so does the tree parser */
        protected void writeTreeAndTreeTestFile( string parserName,
                                                string treeParserName,
                                                string lexerName,
                                                string parserStartRuleName,
                                                string treeParserStartRuleName,
                                                bool debug )
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.runtime.tree.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        TokenRewriteStream tokens = new TokenRewriteStream(lex);\n" +
                "        $createParser$\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        ((CommonTree)r.tree).sanityCheckParentAndChildIndexes();\n" +
                "        CommonTreeNodeStream nodes = new CommonTreeNodeStream((Tree)r.tree);\n" +
                "        nodes.setTokenStream(tokens);\n" +
                "        $treeParserName$ walker = new $treeParserName$(nodes);\n" +
                "        $treeParserName$.$treeParserStartRuleName$_return r2 = walker.$treeParserStartRuleName$();\n" +
                "		 CommonTree rt = ((CommonTree)r2.tree);\n" +
                "		 if ( rt!=null ) System.out.println(((CommonTree)r2.tree).toStringTree());\n" +
                "    }\n" +
                "}"
                );
            StringTemplate createParserST =
                new StringTemplate(
                "        Profiler2 profiler = new Profiler2();\n" +
                "        $parserName$ parser = new $parserName$(tokens,profiler);\n" +
                "        profiler.setParser(parser);\n" );
            if ( !debug )
            {
                createParserST =
                    new StringTemplate(
                    "        $parserName$ parser = new $parserName$(tokens);\n" );
            }
            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "treeParserName", treeParserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            outputFileST.SetAttribute( "treeParserStartRuleName", treeParserStartRuleName );
            writeFile( tmpdir, "Test.java", outputFileST.ToString() );
        }

        protected void writeTemplateTestFile( string parserName,
                                             string lexerName,
                                             string parserStartRuleName,
                                             bool debug )
        {
            StringTemplate outputFileST = new StringTemplate(
                "import org.antlr.runtime.*;\n" +
                "import org.antlr.stringtemplate.*;\n" +
                "import org.antlr.stringtemplate.language.*;\n" +
                "import org.antlr.runtime.debug.*;\n" +
                "import java.io.*;\n" +
                "\n" +
                "class Profiler2 extends Profiler {\n" +
                "    public void terminate() { ; }\n" +
                "}\n" +
                "public class Test {\n" +
                "    static String templates =\n" +
                "    		\"group test;\"+" +
                "    		\"foo(x,y) ::= \\\"<x> <y>\\\"\";\n" +
                "    static StringTemplateGroup group =" +
                "    		new StringTemplateGroup(new StringReader(templates)," +
                "					AngleBracketTemplateLexer.class);" +
                "    public static void main(String[] args) throws Exception {\n" +
                "        CharStream input = new ANTLRFileStream(args[0]);\n" +
                "        $lexerName$ lex = new $lexerName$(input);\n" +
                "        CommonTokenStream tokens = new CommonTokenStream(lex);\n" +
                "        $createParser$\n" +
                "		 parser.setTemplateLib(group);\n" +
                "        $parserName$.$parserStartRuleName$_return r = parser.$parserStartRuleName$();\n" +
                "        if ( r.st!=null )\n" +
                "            System.out.print(r.st.toString());\n" +
                "	 	 else\n" +
                "            System.out.print(\"\");\n" +
                "    }\n" +
                "}"
                );
            StringTemplate createParserST =
                new StringTemplate(
                "        Profiler2 profiler = new Profiler2();\n" +
                "        $parserName$ parser = new $parserName$(tokens,profiler);\n" +
                "        profiler.setParser(parser);\n" );
            if ( !debug )
            {
                createParserST =
                    new StringTemplate(
                    "        $parserName$ parser = new $parserName$(tokens);\n" );
            }
            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            writeFile( tmpdir, "Test.java", outputFileST.ToString() );
        }

        protected void eraseFiles( string filesEndingWith )
        {
            string[] files = System.IO.Directory.GetFiles( tmpdir, "*.*", System.IO.SearchOption.TopDirectoryOnly );
            foreach ( string file in files )
            {
                if ( file.EndsWith( filesEndingWith ) )
                    System.IO.File.Delete( file );
            }
            //File tmpdirF = new File( tmpdir );
            //String[] files = tmpdirF.list();
            //for ( int i = 0; files != null && i < files.Length; i++ )
            //{
            //    if ( files[i].endsWith( filesEndingWith ) )
            //    {
            //        new File( tmpdir + "/" + files[i] ).delete();
            //    }
            //}
        }

        protected virtual void eraseFiles()
        {
            string[] files = System.IO.Directory.GetFiles( tmpdir );
            foreach ( string file in files )
                System.IO.File.Delete( file );
        }

        protected virtual void eraseTempDir()
        {
            if ( System.IO.Directory.Exists( tmpdir ) )
            {
                System.IO.Directory.Delete( tmpdir, true );
            }
        }

        public string getFirstLineOfException()
        {
            if ( this.stderrDuringParse == null )
            {
                return null;
            }
            string[] lines =
                stderrDuringParse
                .Split( new string[] { "\r\n" }, StringSplitOptions.None )
                .SelectMany( line => line.Split( '\n' ) )
                .ToArray();
            //String[] lines = this.stderr.Split( '\n' );
            string prefix = "Exception in thread \"main\" ";
            return lines[0].Substring( prefix.Length );
        }

        public IList realElements( IList elements )
        {
            IList n = new List<object>();
            for ( int i = Label.NUM_FAUX_LABELS + Label.MIN_TOKEN_TYPE - 1; i < elements.Count; i++ )
            {
                object o = elements[i];
                if ( o != null )
                {
                    n.Add( o );
                }
            }
            return n;
        }

        public List<string> realElements( IDictionary<string, int> elements )
        {
            List<string> n = new List<string>();
            foreach ( var token in elements )
            {
                if ( token.Value >= Label.MIN_TOKEN_TYPE )
                    n.Add( token.Key + "=" + token.Value );
            }
            n.Sort();
            return n;
        }

        /**
         * When looking at a result set that consists of a Map/HashTable
         * we cannot rely on the output order, as the hashing algorithm or other aspects
         * of the implementation may be different on differnt JDKs or platforms. Hence
         * we take the Map, convert the keys to a List, sort them and Stringify the Map, which is a
         * bit of a hack, but guarantees that we get the same order on all systems. We assume that
         * the keys are strings.
         * 
         * @param m The Map that contains keys we wish to return in sorted order
         * @return A string that represents all the keys in sorted order.
         */
        public string sortMapToString<TKey, TValue>( IDictionary<TKey, TValue> m )
        {
            Console.Out.WriteLine( "Map toString looks like: " + m.ToElementString() );
            // Pass in crap, and get nothing back
            //
            if ( m == null )
            {
                return null;
            }

            // Sort the keys in the Map
            //
            var nset = new SortedList<TKey, TValue>( m );

            Console.Out.WriteLine( "Tree map looks like: " + nset.ToElementString() );
            return nset.ToElementString();
        }

        public class FilteringTokenStream : CommonTokenStream
        {
            private HashSet<int> _hide = new HashSet<int>();

            public FilteringTokenStream(ITokenSource tokenSource)
                : base(tokenSource)
            {
            }

            protected override void Sync(int i)
            {
                base.Sync(i);
                if (_hide.Contains(Get(i).Type))
                    Get(i).Channel = TokenChannels.Hidden;
            }

            public void SetTokenTypeChannel(int ttype, int channel)
            {
                _hide.Add(ttype);
            }
        }
    }
}
