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

namespace Antlr3.Codegen
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Analysis;
    using Antlr3.Extensions;
    using Antlr3.Grammars;
    using Antlr4.StringTemplate.Visualizer.Extensions;

    using Activator = System.Activator;
    using ANTLRLexer = Antlr3.Grammars.ANTLRLexer;
    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using AntlrTool = Antlr3.AntlrTool;
    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
    using AttributeScope = Antlr3.Tool.AttributeScope;
    using BitSet = Antlr3.Misc.BitSet;
    using CLSCompliant = System.CLSCompliantAttribute;
    using CommonToken = Antlr.Runtime.CommonToken;
    using DFA = Antlr3.Analysis.DFA;
    using DFAOptimizer = Antlr3.Analysis.DFAOptimizer;
    using DFAState = Antlr3.Analysis.DFAState;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Exception = System.Exception;
    using File = System.IO.File;
    using FileNotFoundException = System.IO.FileNotFoundException;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using GrammarType = Antlr3.Tool.GrammarType;
    using IIntSet = Antlr3.Misc.IIntSet;
    using Interval = Antlr3.Misc.Interval;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using IOException = System.IO.IOException;
    using ITemplateWriter = Antlr4.StringTemplate.ITemplateWriter;
    using IToken = Antlr.Runtime.IToken;
    using Label = Antlr3.Analysis.Label;
    using LookaheadSet = Antlr3.Analysis.LookaheadSet;
    using NFAState = Antlr3.Analysis.NFAState;
    using Path = System.IO.Path;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using Regex = System.Text.RegularExpressions.Regex;
    using Rule = Antlr3.Tool.Rule;
    using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;
    using ToolTemplateGroupFile = Antlr3.Tool.ToolTemplateGroupFile;
    using TextWriter = System.IO.TextWriter;
    using TimeSpan = System.TimeSpan;
    using Antlr3.Misc;

    /** ANTLR's code generator.
     *
     *  Generate recognizers derived from grammars.  Language independence
     *  achieved through the use of TemplateGroup objects.  All output
     *  strings are completely encapsulated in the group files such as Java.stg.
     *  Some computations are done that are unused by a particular language.
     *  This generator just computes and sets the values into the templates;
     *  the templates are free to use or not use the information.
     *
     *  To make a new code generation target, define X.stg for language X
     *  by copying from existing Y.stg most closely releated to your language;
     *  e.g., to do CSharp.stg copy Java.stg.  The template group file has a
     *  bunch of templates that are needed by the code generator.  You can add
     *  a new target w/o even recompiling ANTLR itself.  The language=X option
     *  in a grammar file dictates which templates get loaded/used.
     *
     *  Some language like C need both parser files and header files.  Java needs
     *  to have a separate file for the cyclic DFA as ANTLR generates bytecodes
     *  directly (which cannot be in the generated parser Java file).  To facilitate
     *  this,
     *
     * cyclic can be in same file, but header, output must be searpate.  recognizer
     *  is in outptufile.
     */
    public class CodeGenerator
    {
        /** When generating SWITCH statements, some targets might need to limit
         *  the size (based upon the number of case labels).  Generally, this
         *  limit will be hit only for lexers where wildcard in a UNICODE
         *  vocabulary environment would generate a SWITCH with 65000 labels.
         */

        public static readonly int DefaultMaxSwitchCaseLabels = 300;
        public static readonly int DefaultMinSwitchAlts = 3;
        public static readonly int DefaultMaxAcyclicDfaStatesInline = 60;

        public static int MaxSwitchCaseLabels = DefaultMaxSwitchCaseLabels;
        public static int MinSwitchAlts = DefaultMinSwitchAlts;
        public static int MaxAcyclicDfaStatesInline = DefaultMaxAcyclicDfaStatesInline;
        public static bool LaunchTemplateInspector = false;

        public bool GenerateSwitchesWhenPossible = true;

        /** Which grammar are we generating code for?  Each generator
         *  is attached to a specific grammar.
         */
        private readonly Grammar _grammar;

        /** What language are we generating? */
        private readonly string _language;

        /** The target specifies how to write out files and do other language
         *  specific actions.
         */
        private readonly Target _target;

        private static readonly Dictionary<string, Target> _targets = new Dictionary<string, Target>();

        /** Where are the templates this generator should use to generate code? */
        private TemplateGroup _templates;

        /** The basic output templates without AST or templates stuff; this will be
         *  the templates loaded for the language such as Java.stg *and* the Dbg
         *  stuff if turned on.  This is used for generating syntactic predicates.
         */
        private TemplateGroup baseTemplates;

        private StringTemplate recognizerST;
        private StringTemplate outputFileST;
        private StringTemplate headerFileST;

        /** Used to create unique labels */
        private int uniqueLabelNumber = 1;

        /** A reference to the ANTLR tool so we can learn about output directories
         *  and such.
         */
        private readonly AntlrTool tool;

        /** Generate debugging event method calls */
        private bool debug;

        /** Create a Tracer object and make the recognizer invoke this. */
        private bool trace;

        /** Track runtime parsing information about decisions etc...
         *  This requires the debugging event mechanism to work.
         */
        private bool profile;

        private int lineWidth = 72;

        /** I have factored out the generation of acyclic DFAs to separate class */
        private readonly ACyclicDFACodeGenerator _acyclicDFAGenerator;

        /** I have factored out the generation of cyclic DFAs to separate class */
        /*
        public CyclicDFACodeGenerator cyclicDFAGenerator =
            new CyclicDFACodeGenerator(this);
            */

        public static readonly string VocabFileExtension = ".tokens";

        private const string vocabFilePattern =
            "<tokens:{it|<it.name>=<it.type>\n}>" +
            "<literals:{it|<it.name>=<it.type>\n}>";

        public CodeGenerator( AntlrTool tool, Grammar grammar, string language )
        {
            if (tool == null)
                throw new ArgumentNullException("tool");
            if (grammar == null)
                throw new ArgumentNullException("grammar");

            this.tool = tool;
            this._grammar = grammar;
            this._language = language;

            _acyclicDFAGenerator = new ACyclicDFACodeGenerator( this );

            _target = LoadLanguageTarget( language, tool.TargetsDirectory );
        }

        #region Properties

        public Grammar Grammar
        {
            get
            {
                return _grammar;
            }
        }

        public string Language
        {
            get
            {
                return _language;
            }
        }

        public Target Target
        {
            get
            {
                return _target;
            }
        }

        [CLSCompliant(false)]
        public TemplateGroup BaseTemplates
        {
            get
            {
                return baseTemplates;
            }
        }

        [CLSCompliant(false)]
        public StringTemplate RecognizerST
        {
            get
            {
                return outputFileST;
            }
        }

        [CLSCompliant(false)]
        public TemplateGroup Templates
        {
            get
            {
                return _templates;
            }
        }

        public string VocabFileName
        {
            get
            {
                return GetVocabFileName();
            }
        }

        [CLSCompliant(false)]
        public bool Debug
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

        [CLSCompliant(false)]
        public bool Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
                if ( profile )
                {
                    // requires debug events
                    Debug = true;
                }
            }
        }

        [CLSCompliant(false)]
        public bool Trace
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

        #endregion

        public static Target LoadLanguageTarget( string language, string targetsDirectory )
        {
            lock (_targets)
            {
                Target target;
                if (!_targets.TryGetValue(language, out target))
                {
                    // first try to load the target via a satellite DLL
                    string assembly = "Antlr3.Targets." + language + ".dll";
                    string[] paths = { targetsDirectory };

                    System.Reflection.Assembly targetAssembly = null;
                    System.Type targetType = null;
                    string targetName = "Antlr3.Targets." + language + "Target";

                    foreach (string path in paths)
                    {
                        string filename = System.IO.Path.Combine(path, assembly);
                        if (System.IO.File.Exists(filename))
                        {
                            try
                            {
                                targetAssembly = System.Reflection.Assembly.LoadFrom(filename);
                                targetType = targetAssembly.GetType(targetName, false);
                            }
                            catch
                            {
                            }
                        }
                    }

                    // then try to load from the current file
                    if (targetType == null)
                    {
                        targetType = System.Type.GetType(targetName);

                        if (targetType == null)
                        {
                            ErrorManager.Error(ErrorManager.MSG_CANNOT_CREATE_TARGET_GENERATOR, targetName);
                            return null;
                        }
                    }

                    target = (Target)Activator.CreateInstance(targetType);
                    _targets[language] = target;
                }

                return target;
            }
        }

        /** load the main language.stg template group file */
        public virtual void LoadTemplates( string language )
        {
            string outputOption = (string)this._grammar.GetOption("output") ?? string.Empty;
            LoadTemplates(tool, language, _grammar.type, outputOption, debug, out baseTemplates, out _templates);
        }

        private static readonly Dictionary<string, TemplateGroup> _coreTemplates =
            new Dictionary<string, TemplateGroup>();

#if AGGREGATE_TEMPLATE_GROUPS
        private static readonly Dictionary<string, Dictionary<string, TemplateGroup>> _languageBaseTemplates =
            new Dictionary<string, Dictionary<string, TemplateGroup>>();
#endif

        private static readonly Dictionary<TemplateGroup, Dictionary<string, TemplateGroup>> _languageTemplates =
            new Dictionary<TemplateGroup, Dictionary<string, TemplateGroup>>(ObjectReferenceEqualityComparer<TemplateGroup>.Default);

        private sealed class ObjectReferenceEqualityComparer<T> : EqualityComparer<T>
            where T : class
        {
            private static readonly ObjectReferenceEqualityComparer<T> _default = new ObjectReferenceEqualityComparer<T>();

            private ObjectReferenceEqualityComparer()
            {
            }

            public static new ObjectReferenceEqualityComparer<T> Default
            {
                get
                {
                    return _default;
                }
            }

            public override bool Equals(T x, T y)
            {
                return object.ReferenceEquals(x, y);
            }

            public override int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private static void LoadTemplates(AntlrTool tool, string language, GrammarType grammarType, string outputOption, bool debug, out TemplateGroup baseTemplates, out TemplateGroup templates)
        {
            // first load main language template
            TemplateGroup coreTemplates = GetOrCacheTemplateGroup(tool, language, null, null);
            baseTemplates = coreTemplates;

            outputOption = outputOption ?? string.Empty;
            // dynamically add subgroups that act like filters to apply to
            // their supergroup.  E.g., Java:Dbg:AST:ASTParser::ASTDbg.
            if (outputOption.Equals("AST"))
            {
                if (debug && grammarType != GrammarType.Lexer)
                {
                    TemplateGroup dbgTemplates = GetOrCacheTemplateGroup(tool, language, "Dbg", coreTemplates);
                    baseTemplates = dbgTemplates;
                    TemplateGroup astTemplates = GetOrCacheTemplateGroup(tool, language, "AST", dbgTemplates);
                    TemplateGroup astParserTemplates = astTemplates;
                    if (grammarType == GrammarType.TreeParser)
                    {
                        astParserTemplates = GetOrCacheTemplateGroup(tool, language, "ASTTreeParser", astTemplates);
                    }
                    else
                    {
                        astParserTemplates = GetOrCacheTemplateGroup(tool, language, "ASTParser", astTemplates);
                    }

                    TemplateGroup astDbgTemplates = GetOrCacheTemplateGroup(tool, language, "ASTDbg", astParserTemplates);
                    templates = astDbgTemplates;
                }
                else
                {
                    TemplateGroup astTemplates = GetOrCacheTemplateGroup(tool, language, "AST", coreTemplates);
                    TemplateGroup astParserTemplates = astTemplates;
                    if (grammarType == GrammarType.TreeParser)
                    {
                        astParserTemplates = GetOrCacheTemplateGroup(tool, language, "ASTTreeParser", astTemplates);
                    }
                    else
                    {
                        astParserTemplates = GetOrCacheTemplateGroup(tool, language, "ASTParser", astTemplates);
                    }

                    templates = astParserTemplates;
                }
            }
            else if (outputOption.Equals("template"))
            {
                if (debug && grammarType != GrammarType.Lexer)
                {
                    TemplateGroup dbgTemplates = GetOrCacheTemplateGroup(tool, language, "Dbg", coreTemplates);
                    baseTemplates = dbgTemplates;
                    TemplateGroup stTemplates = GetOrCacheTemplateGroup(tool, language, "ST", dbgTemplates);
                    templates = stTemplates;
                }
                else
                {
                    templates = GetOrCacheTemplateGroup(tool, language, "ST", coreTemplates);
                }
            }
            else if (debug && grammarType != GrammarType.Lexer)
            {
                templates = GetOrCacheTemplateGroup(tool, language, "Dbg", coreTemplates);
                baseTemplates = templates;
            }
            else
            {
                templates = coreTemplates;
            }
        }

        private static TemplateGroup GetOrCacheTemplateGroup(AntlrTool tool, string language, string name, TemplateGroup superGroup)
        {
            if (string.IsNullOrEmpty(name) && superGroup == null)
            {
                TemplateGroup group;
                if (_coreTemplates.TryGetValue(language, out group))
                    return group;
            }
            else
            {
                Dictionary<string, TemplateGroup> languageTemplates;
                if (_languageTemplates.TryGetValue(superGroup, out languageTemplates))
                {
                    TemplateGroup group;
                    if (languageTemplates.TryGetValue(name, out group))
                        return group;
                }
            }

            string[] templateDirectories =
                {
                    tool.TemplatesDirectory,
                    Path.Combine(tool.TemplatesDirectory, language)
                };

            return CacheTemplateGroup(templateDirectories, language, name, superGroup);
        }

#if AGGREGATE_TEMPLATE_GROUPS
        private static TemplateGroup CacheTemplateGroup(string[] templateDirectories, string language, string name, TemplateGroup superGroup)
        {
            TemplateGroup baseTemplateGroup;
            if (string.IsNullOrEmpty(name))
            {
                _coreTemplates.TryGetValue(language, out baseTemplateGroup);
            }
            else
            {
                Dictionary<string, TemplateGroup> baseTemplates;
                if (!_languageBaseTemplates.TryGetValue(language, out baseTemplates))
                {
                    baseTemplates = new Dictionary<string, TemplateGroup>();
                    _languageBaseTemplates[language] = baseTemplates;
                }

                baseTemplates.TryGetValue(name, out baseTemplateGroup);
            }

            if (baseTemplateGroup == null)
            {
                baseTemplateGroup = CacheBaseTemplateGroup(templateDirectories, language, name);
            }

            if (superGroup == null)
                return baseTemplateGroup;

            TemplateGroup group = new TemplateGroup();
            group.TrackCreationEvents = CodeGenerator.LaunchTemplateInspector;
            group.IterateAcrossValues = true;
            group.ImportTemplates(baseTemplateGroup);
            group.ImportTemplates(superGroup);

            Dictionary<string, TemplateGroup> languageTemplates;
            if (!_languageTemplates.TryGetValue(superGroup, out languageTemplates))
            {
                languageTemplates = new Dictionary<string, TemplateGroup>();
                _languageTemplates[superGroup] = languageTemplates;
            }

            languageTemplates[name] = group;
            return group;
        }

        private static TemplateGroup CacheBaseTemplateGroup(string[] templateDirectories, string language, string name)
        {
            string groupFileName;
            if (string.IsNullOrEmpty(name))
                groupFileName = FindTemplateFile(templateDirectories, string.Format("{0}.stg", language));
            else
                groupFileName = FindTemplateFile(templateDirectories, string.Format("{0}.stg", name));

            if (string.IsNullOrEmpty(name))
            {
                TemplateGroup group = new ToolTemplateGroupFile(groupFileName);
                group.TrackCreationEvents = CodeGenerator.LaunchTemplateInspector;
                group.IterateAcrossValues = true;
                _coreTemplates[language] = group;
                return group;
            }
            else
            {
                TemplateGroup group = new ToolTemplateGroupFile(groupFileName);
                group.TrackCreationEvents = CodeGenerator.LaunchTemplateInspector;
                group.IterateAcrossValues = true;

                Dictionary<string, TemplateGroup> groups;
                if (!_languageBaseTemplates.TryGetValue(language, out groups))
                {
                    groups = new Dictionary<string, TemplateGroup>();
                    _languageBaseTemplates[language] = groups;
                }

                groups[name] = group;
                return group;
            }
        }
#else
        private static TemplateGroup CacheTemplateGroup(string[] templateDirectories, string language, string name, TemplateGroup superGroup)
        {
            string groupFileName;
            if (string.IsNullOrEmpty(name))
                groupFileName = FindTemplateFile(templateDirectories, string.Format("{0}.stg", language));
            else
                groupFileName = FindTemplateFile(templateDirectories, string.Format("{0}.stg", name));

            if (string.IsNullOrEmpty(name) && superGroup == null)
            {
                TemplateGroup group = new ToolTemplateGroupFile(groupFileName);
                group.TrackCreationEvents = CodeGenerator.LaunchTemplateInspector;
                group.IterateAcrossValues = true;
                _coreTemplates[language] = group;
                return group;
            }
            else
            {
                TemplateGroup group = new ToolTemplateGroupFile(groupFileName);
                group.TrackCreationEvents = CodeGenerator.LaunchTemplateInspector;
                group.IterateAcrossValues = true;
                group.ImportTemplates(superGroup);

                Dictionary<string, TemplateGroup> groups;
                if (!_languageTemplates.TryGetValue(superGroup, out groups))
                {
                    groups = new Dictionary<string, TemplateGroup>();
                    _languageTemplates[superGroup] = groups;
                }

                groups[name] = group;
                return group;
            }
        }
#endif

        internal static string FindTemplateFile(string[] templateDirectories, string fileName)
        {
            foreach (var directory in templateDirectories)
            {
                string templateFileName = Path.Combine(directory, fileName);
                if (File.Exists(templateFileName))
                    return templateFileName;
            }

            throw new FileNotFoundException();
        }

        /** Given the grammar to which we are attached, walk the AST associated
         *  with that grammar to create NFAs.  Then create the DFAs for all
         *  decision points in the grammar by converting the NFAs to DFAs.
         *  Finally, walk the AST again to generate code.
         *
         *  Either 1 or 2 files are written:
         *
         * 		recognizer: the main parser/lexer/treewalker item
         * 		header file: language like C/C++ need extern definitions
         *
         *  The target, such as JavaTarget, dictates which files get written.
         */
        public virtual StringTemplate GenRecognizer()
        {
            //JSystem.@out.println("### generate "+grammar.name+" recognizer");
            // LOAD OUTPUT TEMPLATES
            LoadTemplates( _language );
            if ( _templates == null )
            {
                return null;
            }

            // CREATE NFA FROM GRAMMAR, CREATE DFA FROM NFA
            if ( ErrorManager.DoNotAttemptAnalysis() )
            {
                return null;
            }
            _target.PerformGrammarAnalysis( this, _grammar );


            // some grammar analysis errors will not yield reliable DFA
            if ( ErrorManager.DoNotAttemptCodeGen() )
            {
                return null;
            }

            // OPTIMIZE DFA
            DFAOptimizer optimizer = new DFAOptimizer( _grammar );
            optimizer.Optimize();

            // OUTPUT FILE (contains recognizerST)
            outputFileST = _templates.GetInstanceOf( "outputFile" );

            // HEADER FILE
            if ( _templates.IsDefined( "headerFile" ) )
            {
                headerFileST = _templates.GetInstanceOf( "headerFile" );
            }
            else
            {
                // create a dummy to avoid null-checks all over code generator
                headerFileST = new StringTemplate( _templates, string.Empty );
                // it normally sees this from outputFile
                headerFileST.Add("cyclicDFAs", null);
                headerFileST.impl.Name = "dummy-header-file";
            }

            bool filterMode = _grammar.GetOption( "filter" ) != null &&
                                  _grammar.GetOption( "filter" ).Equals( "true" );
            bool canBacktrack = _grammar.composite.RootGrammar.atLeastOneBacktrackOption ||
                                   _grammar.SyntacticPredicates != null ||
                                   filterMode;

            // TODO: move this down further because generating the recognizer
            // alters the model with info on who uses predefined properties etc...
            // The actions here might refer to something.

            // The only two possible output files are available at this point.
            // Verify action scopes are ok for target and dump actions into output
            // Templates can say <actions.parser.header> for example.
            var actions = _grammar.Actions;
            VerifyActionScopesOkForTarget( actions );
            // translate $x::y references
            TranslateActionAttributeReferences( actions );
            StringTemplate gateST = _templates.GetInstanceOf( "actionGate" );
            if ( filterMode )
            {
                // if filtering, we need to set actions to execute at backtracking
                // level 1 not 0.
                gateST = _templates.GetInstanceOf( "filteringActionGate" );
            }
            _grammar.SetSynPredGateIfNotAlready( gateST );

            headerFileST.SetAttribute( "actions", actions );
            outputFileST.SetAttribute( "actions", actions );

            headerFileST.SetAttribute( "buildTemplate", _grammar.BuildTemplate );
            outputFileST.SetAttribute( "buildTemplate", _grammar.BuildTemplate );
            headerFileST.SetAttribute( "buildAST", _grammar.BuildAST );
            outputFileST.SetAttribute( "buildAST", _grammar.BuildAST );

            outputFileST.SetAttribute( "rewriteMode", _grammar.RewriteMode );
            headerFileST.SetAttribute( "rewriteMode", _grammar.RewriteMode );

            outputFileST.SetAttribute( "backtracking", canBacktrack );
            headerFileST.SetAttribute( "backtracking", canBacktrack );
            // turn on memoize attribute at grammar level so we can create ruleMemo.
            // each rule has memoize attr that hides this one, indicating whether
            // it needs to save results
            string memoize = (string)_grammar.GetOption( "memoize" );
            outputFileST.SetAttribute( "memoize",
                                      ( _grammar.atLeastOneRuleMemoizes ||
                                      ( memoize != null && memoize.Equals( "true" ) ) &&
                                                  canBacktrack ) );
            headerFileST.SetAttribute( "memoize",
                                      ( _grammar.atLeastOneRuleMemoizes ||
                                      ( memoize != null && memoize.Equals( "true" ) ) &&
                                                  canBacktrack ) );


            outputFileST.SetAttribute( "trace", trace );
            headerFileST.SetAttribute( "trace", trace );

            outputFileST.SetAttribute( "profile", profile );
            headerFileST.SetAttribute( "profile", profile );

            // RECOGNIZER
            if ( _grammar.type == GrammarType.Lexer )
            {
                recognizerST = _templates.GetInstanceOf( "lexer" );
                outputFileST.SetAttribute( "LEXER", true );
                headerFileST.SetAttribute( "LEXER", true );
                recognizerST.SetAttribute( "filterMode", filterMode );
            }
            else if ( _grammar.type == GrammarType.Parser ||
                _grammar.type == GrammarType.Combined )
            {
                recognizerST = _templates.GetInstanceOf( "parser" );
                outputFileST.SetAttribute( "PARSER", true );
                headerFileST.SetAttribute( "PARSER", true );
            }
            else
            {
                recognizerST = _templates.GetInstanceOf( "treeParser" );
                outputFileST.SetAttribute( "TREE_PARSER", true );
                headerFileST.SetAttribute( "TREE_PARSER", true );
                recognizerST.SetAttribute( "filterMode", filterMode );
            }
            outputFileST.SetAttribute( "recognizer", recognizerST );
            headerFileST.SetAttribute( "recognizer", recognizerST );
            outputFileST.SetAttribute( "actionScope",
                                      _grammar.GetDefaultActionScope( _grammar.type ) );
            headerFileST.SetAttribute( "actionScope",
                                      _grammar.GetDefaultActionScope( _grammar.type ) );

            string targetAppropriateFileNameString =
                _target.GetTargetStringLiteralFromString( _grammar.FileName );
            outputFileST.SetAttribute( "fileName", targetAppropriateFileNameString );
            headerFileST.SetAttribute( "fileName", targetAppropriateFileNameString );
            outputFileST.SetAttribute( "ANTLRVersion", AntlrTool.AssemblyVersion.ToString(4) );
            headerFileST.SetAttribute( "ANTLRVersion", AntlrTool.AssemblyVersion.ToString(4) );
            outputFileST.SetAttribute( "generatedTimestamp", AntlrTool.GetCurrentTimeStamp() );
            headerFileST.SetAttribute( "generatedTimestamp", AntlrTool.GetCurrentTimeStamp() );

            {
                // GENERATE RECOGNIZER
                // Walk the AST holding the input grammar, this time generating code
                // Decisions are generated by using the precomputed DFAs
                // Fill in the various templates with data
                CodeGenTreeWalker gen = new CodeGenTreeWalker( new Antlr.Runtime.Tree.CommonTreeNodeStream( _grammar.Tree ) );
                try
                {
                    gen.grammar_( _grammar,
                                recognizerST,
                                outputFileST,
                                headerFileST );
                }
                catch ( RecognitionException re )
                {
                    ErrorManager.Error( ErrorManager.MSG_BAD_AST_STRUCTURE,
                                       re );
                }
            }

            GenTokenTypeConstants( recognizerST );
            GenTokenTypeConstants( outputFileST );
            GenTokenTypeConstants( headerFileST );

            if ( _grammar.type != GrammarType.Lexer )
            {
                GenTokenTypeNames( recognizerST );
                GenTokenTypeNames( outputFileST );
                GenTokenTypeNames( headerFileST );
            }

            // Now that we know what synpreds are used, we can set into template
            HashSet<string> synpredNames = null;
            if ( _grammar.synPredNamesUsedInDFA.Count > 0 )
            {
                synpredNames = _grammar.synPredNamesUsedInDFA;
            }
            outputFileST.SetAttribute( "synpreds", synpredNames );
            headerFileST.SetAttribute( "synpreds", synpredNames );

            // all recognizers can see Grammar object
            recognizerST.SetAttribute( "grammar", _grammar );

            // do not render templates to disk if errors occurred
            if (ErrorManager.GetErrorState().errors > 0)
                return null;

            if (CodeGenerator.LaunchTemplateInspector)
            {
                outputFileST.Visualize();
                if (_templates.IsDefined("headerFile"))
                    headerFileST.Visualize();
            }

            // WRITE FILES
            try
            {
                _target.GenRecognizerFile( tool, this, _grammar, outputFileST );
                if ( _templates.IsDefined( "headerFile" ) )
                {
                    StringTemplate extST = _templates.GetInstanceOf( "headerFileExtension" );
                    _target.GenRecognizerHeaderFile( tool, this, _grammar, headerFileST, extST.Render() );
                }
                // write out the vocab interchange file; used by antlr,
                // does not change per target
                StringTemplate tokenVocabSerialization = GenTokenVocabOutput();
                string vocabFileName = VocabFileName;
                if ( vocabFileName != null )
                {
                    Write( tokenVocabSerialization, vocabFileName );
                }
                //JSystem.@out.println(outputFileST.getDOTForDependencyGraph(false));
            }
            catch ( IOException ioe )
            {
                ErrorManager.Error( ErrorManager.MSG_CANNOT_WRITE_FILE, ioe );
            }
            /*
            JSystem.@out.println("num obj.prop refs: "+ ASTExpr.totalObjPropRefs);
            JSystem.@out.println("num reflection lookups: "+ ASTExpr.totalReflectionLookups);
            */

            return outputFileST;
        }

        /** Some targets will have some extra scopes like C++ may have
         *  '@headerfile:name {action}' or something.  Make sure the
         *  target likes the scopes in action table.
         */
        protected virtual void VerifyActionScopesOkForTarget( IDictionary<string, IDictionary<string, object>> actions )
        {
            foreach ( var action in actions )
            {
                string scope = action.Key;
                if ( !_target.IsValidActionScope( _grammar.type, scope ) )
                {
                    // get any action from the scope to get error location
                    var scopeActions = action.Value;
                    GrammarAST actionAST = scopeActions.Values.Cast<GrammarAST>().First();
                    ErrorManager.GrammarError(
                        ErrorManager.MSG_INVALID_ACTION_SCOPE, _grammar,
                        actionAST.Token, scope,
                        _grammar.GrammarTypeString );
                }
            }
        }

        /** Actions may reference $x::y attributes, call translateAction on
         *  each action and replace that action in the Map.
         */
        protected virtual void TranslateActionAttributeReferences( IDictionary<string, IDictionary<string, object>> actions )
        {
            foreach ( var action in actions )
            {
                string scope = action.Key;
                var scopeActions = action.Value;
                TranslateActionAttributeReferencesForSingleScope( null, scopeActions );
            }
        }

        /** Use for translating rule @init{...} actions that have no scope */
        protected internal virtual void TranslateActionAttributeReferencesForSingleScope(
            Rule r,
            IDictionary<string,object> scopeActions )
        {
            string ruleName = null;
            if ( r != null )
            {
                ruleName = r.Name;
            }

            foreach ( var scopeAction in scopeActions.ToArray() )
            {
                string name = scopeAction.Key;
                object action = scopeAction.Value;
                GrammarAST actionAST = action as GrammarAST;
                IList<object> chunks = TranslateAction( ruleName, actionAST );
                scopeActions[name] = chunks; // replace with translation
            }
        }

        /** Error recovery in ANTLR recognizers.
         *
         *  Based upon original ideas:
         *
         *  Algorithms + Data Structures = Programs by Niklaus Wirth
         *
         *  and
         *
         *  A note on error recovery in recursive descent parsers:
         *  http://portal.acm.org/citation.cfm?id=947902.947905
         *
         *  Later, Josef Grosch had some good ideas:
         *  Efficient and Comfortable Error Recovery in Recursive Descent Parsers:
         *  ftp://www.cocolab.com/products/cocktail/doca4.ps/ell.ps.zip
         *
         *  Like Grosch I implemented local FOLLOW sets that are combined at run-time
         *  upon error to avoid parsing overhead.
         */
        public virtual void GenerateLocalFollow( GrammarAST referencedElementNode,
                                        string referencedElementName,
                                        string enclosingRuleName,
                                        int elementIndex )
        {
            if (elementIndex < 0)
            {
                throw new ArgumentOutOfRangeException("elementIndex", "elementIndex cannot be less than zero.");
            }

            /*
            JSystem.@out.println("compute FOLLOW "+grammar.name+"."+referencedElementNode.toString()+
                             " for "+referencedElementName+"#"+elementIndex +" in "+
                             enclosingRuleName+
                             " line="+referencedElementNode.getLine());
                             */
            NFAState followingNFAState = referencedElementNode.followingNFAState;
            LookaheadSet follow = null;
            if ( followingNFAState != null )
            {
                // compute follow for this element and, as side-effect, track
                // the rule LOOK sensitivity.
                follow = _grammar.First( followingNFAState );
            }

            if ( follow == null )
            {
                ErrorManager.InternalError( "no follow state or cannot compute follow" );
                follow = new LookaheadSet();
            }
            if ( follow.Member( Label.EOF ) )
            {
                // TODO: can we just remove?  Seems needed here:
                // compilation_unit : global_statement* EOF
                // Actually i guess we resync to EOF regardless
                follow.Remove( Label.EOF );
            }
            //JSystem.@out.println(" "+follow);

            IList<int> tokenTypeList = null;
            ulong[] words = null;
            if ( follow.TokenTypeSet == null )
            {
                words = new ulong[1];
                tokenTypeList = new List<int>();
            }
            else
            {
                BitSet bits = BitSet.Of( follow.TokenTypeSet );
                words = bits.ToPackedArray();
                tokenTypeList = follow.TokenTypeSet.ToList();
            }
            // use the target to convert to hex strings (typically)
            string[] wordStrings = new string[words.Length];
            for ( int j = 0; j < words.Length; j++ )
            {
                ulong w = words[j];
                wordStrings[j] = _target.GetTarget64BitStringFromValue( w );
            }
            recognizerST.SetAttribute( "bitsets.{name,inName,bits,tokenTypes,tokenIndex}",
                    referencedElementName,
                    enclosingRuleName,
                    wordStrings,
                    tokenTypeList,
                    elementIndex );
            outputFileST.SetAttribute( "bitsets.{name,inName,bits,tokenTypes,tokenIndex}",
                    referencedElementName,
                    enclosingRuleName,
                    wordStrings,
                    tokenTypeList,
                    elementIndex );
            headerFileST.SetAttribute( "bitsets.{name,inName,bits,tokenTypes,tokenIndex}",
                    referencedElementName,
                    enclosingRuleName,
                    wordStrings,
                    tokenTypeList,
                    elementIndex );
        }

        // L O O K A H E A D  D E C I S I O N  G E N E R A T I O N

        /** Generate code that computes the predicted alt given a DFA.  The
         *  recognizerST can be either the main generated recognizerTemplate
         *  for storage in the main parser file or a separate file.  It's up to
         *  the code that ultimately invokes the codegen.g grammar rule.
         *
         *  Regardless, the output file and header file get a copy of the DFAs.
         */
        public virtual StringTemplate GenLookaheadDecision( StringTemplate recognizerST,
                                                   DFA dfa )
        {
            StringTemplate decisionST;
            // If we are doing inline DFA and this one is acyclic and LL(*)
            // I have to check for is-non-LL(*) because if non-LL(*) the cyclic
            // check is not done by DFA.verify(); that is, verify() avoids
            // doesStateReachAcceptState() if non-LL(*)
            if ( dfa.CanInlineDecision )
            {
                decisionST =
                    _acyclicDFAGenerator.GenFixedLookaheadDecision( Templates, dfa );
            }
            else
            {
                // generate any kind of DFA here (cyclic or acyclic)
                dfa.CreateStateTables( this );
                outputFileST.SetAttribute( "cyclicDFAs", dfa );
                headerFileST.SetAttribute( "cyclicDFAs", dfa );
                decisionST = _templates.GetInstanceOf( "dfaDecision" );
                string description = dfa.NFADecisionStartState.Description;
                description = _target.GetTargetStringLiteralFromString( description );
                if ( description != null )
                {
                    decisionST.SetAttribute( "description", description );
                }
                decisionST.SetAttribute( "decisionNumber",
                                        dfa.NfaStartStateDecisionNumber );
            }
            return decisionST;
        }

        /** A special state is huge (too big for state tables) or has a predicated
         *  edge.  Generate a simple if-then-else.  Cannot be an accept state as
         *  they have no emanating edges.  Don't worry about switch vs if-then-else
         *  because if you get here, the state is super complicated and needs an
         *  if-then-else.  This is used by the new DFA scheme created June 2006.
         */
        public virtual StringTemplate GenerateSpecialState( DFAState s )
        {
            StringTemplate stateST;
            stateST = _templates.GetInstanceOf( "cyclicDFAState" );
            stateST.SetAttribute( "needErrorClause", true );
            stateST.SetAttribute( "semPredState", s.IsResolvedWithPredicates );
            stateST.SetAttribute( "stateNumber", s.StateNumber );
            stateST.SetAttribute( "decisionNumber", s.Dfa.DecisionNumber );

            bool foundGatedPred = false;
            StringTemplate eotST = null;
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.GetTransition( i );
                StringTemplate edgeST;
                if ( edge.Label.Atom == Label.EOT )
                {
                    // this is the default clause; has to held until last
                    edgeST = _templates.GetInstanceOf( "eotDFAEdge" );
                    stateST.RemoveAttribute( "needErrorClause" );
                    eotST = edgeST;
                }
                else
                {
                    edgeST = _templates.GetInstanceOf( "cyclicDFAEdge" );
                    StringTemplate exprST =
                        GenLabelExpr( _templates, edge, 1 );
                    edgeST.SetAttribute( "labelExpr", exprST );
                }
                edgeST.SetAttribute( "edgeNumber", i + 1 );
                edgeST.SetAttribute( "targetStateNumber",
                                     edge.Target.StateNumber );
                // stick in any gated predicates for any edge if not already a pred
                if ( !edge.Label.IsSemanticPredicate )
                {
                    DFAState t = (DFAState)edge.Target;
                    SemanticContext preds = t.GetGatedPredicatesInNFAConfigurations();
                    if ( preds != null )
                    {
                        foundGatedPred = true;
                        StringTemplate predST = preds.GenExpr( this,
                                                              Templates,
                                                              t.Dfa );
                        edgeST.SetAttribute( "predicates", predST.Render() );
                    }
                }
                if ( edge.Label.Atom != Label.EOT )
                {
                    stateST.SetAttribute( "edges", edgeST );
                }
            }
            if ( foundGatedPred )
            {
                // state has >= 1 edge with a gated pred (syn or sem)
                // must rewind input first, set flag.
                stateST.SetAttribute( "semPredState", foundGatedPred );
            }
            if ( eotST != null )
            {
                stateST.SetAttribute( "edges", eotST );
            }
            return stateST;
        }

        /** Generate an expression for traversing an edge. */
        protected internal virtual StringTemplate GenLabelExpr( TemplateGroup templates,
                                              Transition edge,
                                              int k )
        {
            Label label = edge.Label;
            if ( label.IsSemanticPredicate )
            {
                return GenSemanticPredicateExpr( templates, edge );
            }
            if ( label.IsSet )
            {
                return GenSetExpr( templates, label.Set, k, true );
            }
            // must be simple label
            StringTemplate eST = templates.GetInstanceOf( "lookaheadTest" );
            eST.SetAttribute( "atom", GetTokenTypeAsTargetLabel( label.Atom ) );
            eST.SetAttribute( "atomAsInt", label.Atom );
            eST.SetAttribute( "k", k );
            return eST;
        }

        protected internal virtual StringTemplate GenSemanticPredicateExpr( TemplateGroup templates,
                                                          Transition edge )
        {
            DFA dfa = ( (DFAState)edge.Target ).Dfa; // which DFA are we in
            Label label = edge.Label;
            SemanticContext semCtx = label.SemanticContext;
            return semCtx.GenExpr( this, templates, dfa );
        }

        /** For intervals such as [3..3, 30..35], generate an expression that
         *  tests the lookahead similar to LA(1)==3 || (LA(1)>=30&&LA(1)<=35)
         */
        public virtual StringTemplate GenSetExpr( TemplateGroup templates,
                                         IIntSet set,
                                         int k,
                                         bool partOfDFA )
        {
            if ( !( set is IntervalSet ) )
            {
                throw new ArgumentException( "unable to generate expressions for non IntervalSet objects" );
            }
            IntervalSet iset = (IntervalSet)set;
            if ( iset.Intervals == null || iset.Intervals.Count == 0 )
            {
                StringTemplate emptyST = new StringTemplate( templates, "" );
                emptyST.impl.Name = "empty-set-expr";
                return emptyST;
            }
            string testSTName = "lookaheadTest";
            string testRangeSTName = "lookaheadRangeTest";
            string testSetSTName = "lookaheadSetTest";
            string varSTName = "lookaheadVarName";
            if (!partOfDFA)
            {
                testSTName = "isolatedLookaheadTest";
                testRangeSTName = "isolatedLookaheadRangeTest";
                testSetSTName = "isolatedLookaheadSetTest";
                varSTName = "isolatedLookaheadVarName";
            }

            StringTemplate setST = templates.GetInstanceOf( "setTest" );
            // If the SetTest template exists, separate the ranges:
            // flatten the small ones into one list and make that a range,
            // and leave the others as they are.
            if (templates.IsDefined(testSetSTName))
            {
                // Flatten the IntervalSet into a list of integers.
                StringTemplate sST = templates.GetInstanceOf(testSetSTName);
                int rangeNumber2 = 1;
                foreach (Interval I in iset.Intervals)
                {
                    int a = I.a;
                    int b = I.b;
                    // Not flattening the large ranges helps us avoid making a
                    // set that contains 90% of Unicode when we could just use
                    // a simple range like (LA(1)>=123 && LA(1)<=65535).
                    // This flattens all ranges of length 4 or less.
                    if (b - a < 4)
                    {
                        for (int i = a; i <= b; i++)
                        {
                            sST.Add("values", GetTokenTypeAsTargetLabel(i));
                            sST.Add("valuesAsInt", i);
                        }
                    }
                    else
                    {
                        StringTemplate eST = templates.GetInstanceOf(testRangeSTName);
                        eST.Add("lower", GetTokenTypeAsTargetLabel(a));
                        eST.Add("lowerAsInt", a);
                        eST.Add("upper", GetTokenTypeAsTargetLabel(b));
                        eST.Add("upperAsInt", b);
                        eST.Add("rangeNumber", rangeNumber2);
                        eST.Add("k", k);
                        setST.Add("ranges", eST);
                        rangeNumber2++;
                    }
                }

                sST.Add("k", k);
                setST.Add("ranges", sST);
                return setST;
            }

            int rangeNumber = 1;
            foreach ( Interval I in iset.Intervals )
            {
                int a = I.a;
                int b = I.b;
                StringTemplate eST;
                if ( a == b )
                {
                    eST = templates.GetInstanceOf( testSTName );
                    eST.SetAttribute( "atom", GetTokenTypeAsTargetLabel( a ) );
                    eST.SetAttribute( "atomAsInt", a );
                    //eST.setAttribute("k",Utils.integer(k));
                }
                else
                {
                    eST = templates.GetInstanceOf( testRangeSTName );
                    eST.SetAttribute( "lower", GetTokenTypeAsTargetLabel( a ) );
                    eST.SetAttribute( "lowerAsInt", a );
                    eST.SetAttribute( "upper", GetTokenTypeAsTargetLabel( b ) );
                    eST.SetAttribute( "upperAsInt", b );
                    eST.SetAttribute( "rangeNumber", rangeNumber );
                }
                eST.SetAttribute( "k", k );
                setST.SetAttribute( "ranges", eST );
                rangeNumber++;
            }
            return setST;
        }

        // T O K E N  D E F I N I T I O N  G E N E R A T I O N

        /** Set attributes tokens and literals attributes in the incoming
         *  code template.  This is not the token vocab interchange file, but
         *  rather a list of token type ID needed by the recognizer.
         */
        protected virtual void GenTokenTypeConstants( StringTemplate code )
        {
            // make constants for the token types
            foreach (var token in _grammar.composite.TokenIDToTypeMap.OrderBy(i => i.Value))
            {
                if (token.Value == Label.EOF || token.Value >= Label.MIN_TOKEN_TYPE)
                    code.SetAttribute("tokens.{name,type}", token.Key, token.Value);
            }
        }

        /** Generate a token names table that maps token type to a printable
         *  name: either the label like INT or the literal like "begin".
         */
        protected virtual void GenTokenTypeNames( StringTemplate code )
        {
            for ( int t = Label.MIN_TOKEN_TYPE; t <= _grammar.MaxTokenType; t++ )
            {
                string tokenName = _grammar.GetTokenDisplayName( t );
                if ( tokenName != null )
                {
                    tokenName = _target.GetTargetStringLiteralFromString( tokenName, true );
                    code.SetAttribute( "tokenNames", tokenName );
                }
            }
        }

        /** Get a meaningful name for a token type useful during code generation.
         *  Literals without associated names are converted to the string equivalent
         *  of their integer values. Used to generate x==ID and x==34 type comparisons
         *  etc...  Essentially we are looking for the most obvious way to refer
         *  to a token type in the generated code.  If in the lexer, return the
         *  char literal translated to the target language.  For example, ttype=10
         *  will yield '\n' from the getTokenDisplayName method.  That must
         *  be converted to the target languages literals.  For most C-derived
         *  languages no translation is needed.
         */
        public virtual string GetTokenTypeAsTargetLabel( int ttype )
        {
            if ( _grammar.type == GrammarType.Lexer )
            {
                string name = _grammar.GetTokenDisplayName( ttype );
                return _target.GetTargetCharLiteralFromANTLRCharLiteral( this, name );
            }
            return _target.GetTokenTypeAsTargetLabel( this, ttype );
        }

        /** Generate a token vocab file with all the token names/types.  For example:
         *  ID=7
         *  FOR=8
         *  'for'=8
         *
         *  This is independent of the target language; used by antlr internally
         */
        protected virtual StringTemplate GenTokenVocabOutput()
        {
            StringTemplate vocabFileST = new StringTemplate( vocabFilePattern );
            // "define" literals arg
            vocabFileST.Add("literals", null);
            vocabFileST.Add("tokens", null);
            vocabFileST.impl.Name = "vocab-file";
            // make constants for the token names
            foreach ( string tokenID in _grammar.TokenIDs )
            {
                int tokenType = _grammar.GetTokenType( tokenID );
                if ( tokenType >= Label.MIN_TOKEN_TYPE )
                {
                    vocabFileST.SetAttribute( "tokens.{name,type}", tokenID, tokenType );
                }
            }

            // now dump the strings
            foreach ( string literal in _grammar.StringLiterals )
            {
                int tokenType = _grammar.GetTokenType( literal );
                if ( tokenType >= Label.MIN_TOKEN_TYPE )
                {
                    vocabFileST.SetAttribute( "tokens.{name,type}", literal, tokenType );
                }
            }

            return vocabFileST;
        }

        public virtual IList<object> TranslateAction( string ruleName,
                                    GrammarAST actionTree )
        {
            if ( actionTree.Type == ANTLRParser.ARG_ACTION )
            {
                return TranslateArgAction( ruleName, actionTree );
            }
            ActionTranslator translator = new ActionTranslator( this, ruleName, actionTree );
            IList<object> chunks = translator.TranslateToChunks();
            chunks = _target.PostProcessAction( chunks, actionTree.Token );
            return chunks;
        }

        /** Translate an action like [3,"foo",a[3]] and return a List of the
         *  translated actions.  Because actions are themselves translated to a list
         *  of chunks, must cat together into a StringTemplate>.  Don't translate
         *  to strings early as we need to eval templates in context.
         */
        public virtual List<object> TranslateArgAction( string ruleName,
                                               GrammarAST actionTree )
        {
            string actionText = actionTree.Token.Text;
            List<string> args = GetListOfArgumentsFromAction( actionText, ',' );
            List<object> translatedArgs = new List<object>();
            foreach ( string arg in args )
            {
                if ( arg != null )
                {
                    IToken actionToken =
                        new CommonToken( ANTLRParser.ACTION, arg );
                    ActionTranslator translator =
                        new ActionTranslator( this, ruleName,
                                                  actionToken,
                                                  actionTree.outerAltNum );
                    IList<object> chunks = translator.TranslateToChunks();
                    chunks = _target.PostProcessAction( chunks, actionToken );
                    StringTemplate catST = new StringTemplate( _templates, "<chunks>" );
                    catST.SetAttribute( "chunks", chunks );
                    translatedArgs.Add( catST );
                }
            }
            if ( translatedArgs.Count == 0 )
            {
                return null;
            }
            return translatedArgs;
        }

        public static List<string> GetListOfArgumentsFromAction( string actionText,
                                                                int separatorChar )
        {
            List<string> args = new List<string>();
            GetListOfArgumentsFromAction( actionText, 0, -1, separatorChar, args );
            return args;
        }

        /** Given an arg action like
         *
         *  [x, (*a).foo(21,33), 3.2+1, '\n',
         *  "a,oo\nick", {bl, "fdkj"eck}, ["cat\n,", x, 43]]
         *
         *  convert to a list of arguments.  Allow nested square brackets etc...
         *  Set separatorChar to ';' or ',' or whatever you want.
         */
        public static int GetListOfArgumentsFromAction( string actionText,
                                                       int start,
                                                       int targetChar,
                                                       int separatorChar,
                                                       IList<string> args )
        {
            if ( actionText == null )
            {
                return -1;
            }
            actionText = Regex.Replace(actionText, "//.*\n", "" );
            int n = actionText.Length;
            //JSystem.@out.println("actionText@"+start+"->"+(char)targetChar+"="+actionText.substring(start,n));
            int p = start;
            int last = p;
            while ( p < n && actionText[p] != targetChar )
            {
                int c = actionText[p];
                switch ( c )
                {
                case '\'':
                    p++;
                    while ( p < n && actionText[p] != '\'' )
                    {
                        if ( actionText[p] == '\\' && ( p + 1 ) < n &&
                             actionText[p + 1] == '\'' )
                        {
                            p++; // skip escaped quote
                        }
                        p++;
                    }
                    p++;
                    break;
                case '"':
                    p++;
                    while ( p < n && actionText[p] != '\"' )
                    {
                        if ( actionText[p] == '\\' && ( p + 1 ) < n &&
                             actionText[p + 1] == '\"' )
                        {
                            p++; // skip escaped quote
                        }
                        p++;
                    }
                    p++;
                    break;
                case '(':
                    p = GetListOfArgumentsFromAction( actionText, p + 1, ')', separatorChar, args );
                    break;
                case '{':
                    p = GetListOfArgumentsFromAction( actionText, p + 1, '}', separatorChar, args );
                    break;
                case '<':
                    if ( actionText.IndexOf( '>', p + 1 ) >= p )
                    {
                        // do we see a matching '>' ahead?  if so, hope it's a generic
                        // and not less followed by expr with greater than
                        p = GetListOfArgumentsFromAction( actionText, p + 1, '>', separatorChar, args );
                    }
                    else
                    {
                        p++; // treat as normal char
                    }
                    break;
                case '[':
                    p = GetListOfArgumentsFromAction( actionText, p + 1, ']', separatorChar, args );
                    break;
                default:
                    if ( c == separatorChar && targetChar == -1 )
                    {
                        string arg = actionText.Substring( last, p - last );
                        //JSystem.@out.println("arg="+arg);
                        args.Add( arg.Trim() );
                        last = p + 1;
                    }
                    p++;
                    break;
                }
            }
            if ( targetChar == -1 && p <= n )
            {
                string arg = actionText.Substring( last, p - last ).Trim();
                //JSystem.@out.println("arg="+arg);
                if ( arg.Length > 0 )
                {
                    args.Add( arg.Trim() );
                }
            }
            p++;
            return p;
        }

        /** Given a template constructor action like %foo(a={...}) in
         *  an action, translate it to the appropriate template constructor
         *  from the templateLib. This translates a *piece* of the action.
         */
        public virtual StringTemplate TranslateTemplateConstructor( string ruleName,
                                                           int outerAltNum,
                                                           IToken actionToken,
                                                           string templateActionText )
        {
            GrammarAST rewriteTree = null;

            {
                // first, parse with antlr.g
                //JSystem.@out.println("translate template: "+templateActionText);
                ANTLRLexer lexer = new ANTLRLexer( new Antlr.Runtime.ANTLRStringStream( templateActionText ) );
                lexer.Filename = _grammar.FileName;
                //lexer.setTokenObjectClass( "antlr.TokenWithIndex" );
                //TokenStreamRewriteEngine tokenBuffer = new TokenStreamRewriteEngine( lexer );
                //tokenBuffer.discard( ANTLRParser.WS );
                //tokenBuffer.discard( ANTLRParser.ML_COMMENT );
                //tokenBuffer.discard( ANTLRParser.COMMENT );
                //tokenBuffer.discard( ANTLRParser.SL_COMMENT );
                ANTLRParser parser = new ANTLRParser( new Antlr.Runtime.CommonTokenStream( lexer ) );
                parser.FileName = _grammar.FileName;
                //parser.setASTNodeClass( "org.antlr.tool.GrammarAST" );
                try
                {
                    Antlr.Runtime.IAstRuleReturnScope<GrammarAST> result = parser.rewrite_template();
                    rewriteTree = result.Tree;
                }
                catch ( RecognitionException /*re*/ )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_INVALID_TEMPLATE_ACTION,
                                                  _grammar,
                                                  actionToken,
                                                  templateActionText );
                }
                catch ( Exception tse )
                {
                    ErrorManager.InternalError( "can't parse template action", tse );
                }
            }

            {
                // then translate via codegen.g
                CodeGenTreeWalker gen = new CodeGenTreeWalker( new Antlr.Runtime.Tree.CommonTreeNodeStream( rewriteTree ) );
                gen.Init( _grammar );
                gen.currentRuleName = ruleName;
                gen.outerAltNum = outerAltNum;
                StringTemplate st = null;
                try
                {
                    st = gen.rewrite_template();
                }
                catch ( RecognitionException re )
                {
                    ErrorManager.Error( ErrorManager.MSG_BAD_AST_STRUCTURE,
                                       re );
                }
                return st;
            }
        }


        public virtual void IssueInvalidScopeError( string x,
                                           string y,
                                           Rule enclosingRule,
                                           IToken actionToken,
                                           int outerAltNum )
        {
            //JSystem.@out.println("error $"+x+"::"+y);
            Rule r = _grammar.GetRule( x );
            AttributeScope scope = _grammar.GetGlobalScope( x );
            if ( scope == null )
            {
                if ( r != null )
                {
                    scope = r.RuleScope; // if not global, might be rule scope
                }
            }
            if ( scope == null )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_UNKNOWN_DYNAMIC_SCOPE,
                                              _grammar,
                                              actionToken,
                                              x );
            }
            else if ( scope.GetAttribute( y ) == null )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_UNKNOWN_DYNAMIC_SCOPE_ATTRIBUTE,
                                              _grammar,
                                              actionToken,
                                              x,
                                              y );
            }
        }

        public virtual void IssueInvalidAttributeError( string x,
                                               string y,
                                               Rule enclosingRule,
                                               IToken actionToken,
                                               int outerAltNum )
        {
            //JSystem.@out.println("error $"+x+"."+y);
            if ( enclosingRule == null )
            {
                // action not in a rule
                ErrorManager.GrammarError( ErrorManager.MSG_ATTRIBUTE_REF_NOT_IN_RULE,
                                              _grammar,
                                              actionToken,
                                              x,
                                              y );
                return;
            }

            // action is in a rule
            Grammar.LabelElementPair label = enclosingRule.GetRuleLabel( x );

            if ( label != null || enclosingRule.GetRuleRefsInAlt( x, outerAltNum ) != null )
            {
                // $rulelabel.attr or $ruleref.attr; must be unknown attr
                string refdRuleName = x;
                if ( label != null )
                {
                    refdRuleName = enclosingRule.GetRuleLabel( x ).referencedRuleName;
                }
                Rule refdRule = _grammar.GetRule( refdRuleName );
                AttributeScope scope = refdRule.GetAttributeScope( y );
                if ( scope == null )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_UNKNOWN_RULE_ATTRIBUTE,
                                              _grammar,
                                              actionToken,
                                              refdRuleName,
                                              y );
                }
                else if ( scope.IsParameterScope )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_INVALID_RULE_PARAMETER_REF,
                                              _grammar,
                                              actionToken,
                                              refdRuleName,
                                              y );
                }
                else if ( scope.IsDynamicRuleScope )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_INVALID_RULE_SCOPE_ATTRIBUTE_REF,
                                              _grammar,
                                              actionToken,
                                              refdRuleName,
                                              y );
                }
            }

        }

        public virtual void IssueInvalidAttributeError( string x,
                                               Rule enclosingRule,
                                               IToken actionToken,
                                               int outerAltNum )
        {
            //JSystem.@out.println("error $"+x);
            if ( enclosingRule == null )
            {
                // action not in a rule
                ErrorManager.GrammarError( ErrorManager.MSG_ATTRIBUTE_REF_NOT_IN_RULE,
                                              _grammar,
                                              actionToken,
                                              x );
                return;
            }

            // action is in a rule
            Grammar.LabelElementPair label = enclosingRule.GetRuleLabel( x );
            AttributeScope scope = enclosingRule.GetAttributeScope( x );

            if ( label != null ||
                 enclosingRule.GetRuleRefsInAlt( x, outerAltNum ) != null ||
                 enclosingRule.Name.Equals( x ) )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_ISOLATED_RULE_SCOPE,
                                              _grammar,
                                              actionToken,
                                              x );
            }
            else if ( scope != null && scope.IsDynamicRuleScope )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_ISOLATED_RULE_ATTRIBUTE,
                                              _grammar,
                                              actionToken,
                                              x );
            }
            else
            {
                ErrorManager.GrammarError( ErrorManager.MSG_UNKNOWN_SIMPLE_ATTRIBUTE,
                                          _grammar,
                                          actionToken,
                                          x );
            }
        }

        // M I S C

        /** Generate TParser.java and TLexer.java from T.g if combined, else
         *  just use T.java as output regardless of type.
         */
        public virtual string GetRecognizerFileName( string name, GrammarType type )
        {
            StringTemplate extST = _templates.GetInstanceOf( "codeFileExtension" );
            string recognizerName = _grammar.GetRecognizerName();
            return recognizerName + extST.Render();
            /*
            String suffix = "";
            if ( type==GrammarType.Combined ||
                 (type==GrammarType.Lexer && !grammar.implicitLexer) )
            {
                suffix = Grammar.grammarTypeToFileNameSuffix[type];
            }
            return name+suffix+extST.toString();
            */
        }

        /** What is the name of the vocab file generated for this grammar?
         *  Returns null if no .tokens file should be generated.
         */
        public virtual string GetVocabFileName()
        {
            if ( _grammar.IsBuiltFromString )
            {
                return null;
            }
            return _grammar.name + VocabFileExtension;
        }

        public virtual void Write( StringTemplate code, string fileName )
        {
            Stopwatch watch = Stopwatch.StartNew();
            TextWriter w = tool.GetOutputFile( _grammar, fileName );
            // Write the output to a StringWriter
            ITemplateWriter wr = _templates.GetStringTemplateWriter( w );
            wr.LineWidth = lineWidth;
            code.Write( wr );
            w.Close();
            TimeSpan duration = watch.Elapsed;
            //JSystem.@out.println("render time for "+fileName+": "+(int)(stop-start)+"ms");
        }

        /** You can generate a switch rather than if-then-else for a DFA state
         *  if there are no semantic predicates and the number of edge label
         *  values is small enough; e.g., don't generate a switch for a state
         *  containing an edge label such as 20..52330 (the resulting byte codes
         *  would overflow the method 65k limit probably).
         */
        protected internal virtual bool CanGenerateSwitch( DFAState s )
        {
            if ( !GenerateSwitchesWhenPossible )
            {
                return false;
            }
            int size = 0;
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.GetTransition( i );
                if ( edge.Label.IsSemanticPredicate )
                {
                    return false;
                }
                // can't do a switch if the edges are going to require predicates
                if ( edge.Label.Atom == Label.EOT )
                {
                    int EOTPredicts = ( (DFAState)edge.Target ).GetUniquelyPredictedAlt();
                    if ( EOTPredicts == NFA.INVALID_ALT_NUMBER )
                    {
                        // EOT target has to be a predicate then; no unique alt
                        return false;
                    }
                }
                // if target is a state with gated preds, we need to use preds on
                // this edge then to reach it.
                if ( ( (DFAState)edge.Target ).GetGatedPredicatesInNFAConfigurations() != null )
                {
                    return false;
                }
                size += edge.Label.Set.Count;
            }
            if ( s.NumberOfTransitions < MinSwitchAlts ||
                 size > MaxSwitchCaseLabels )
            {
                return false;
            }
            return true;
        }

        /** Create a label to track a token / rule reference's result.
         *  Technically, this is a place where I break model-view separation
         *  as I am creating a variable name that could be invalid in a
         *  target language, however, label ::= <ID><INT> is probably ok in
         *  all languages we care about.
         */
        public virtual string CreateUniqueLabel( string name )
        {
            return name + ( uniqueLabelNumber++ );
            //return new StringBuffer()
            //    .append( name ).append( uniqueLabelNumber++ ).toString();
        }
    }
}
