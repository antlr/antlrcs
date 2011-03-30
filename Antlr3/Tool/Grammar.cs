/*
 * [The "BSD licence"]
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Analysis;
    using Antlr3.Grammars;
    using Antlr3.Misc;

    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using CLSCompliant = System.CLSCompliantAttribute;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using CommonToken = Antlr.Runtime.CommonToken;
    using Console = System.Console;
    using DateTime = System.DateTime;
    using Exception = System.Exception;
    using File = System.IO.File;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using IToken = Antlr.Runtime.IToken;
    using ITree = Antlr.Runtime.Tree.ITree;
    using Math = System.Math;
    using Path = System.IO.Path;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using StringBuilder = System.Text.StringBuilder;
    using StringComparer = System.StringComparer;
    using StringComparison = System.StringComparison;
    using StringReader = System.IO.StringReader;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using Target = Antlr3.Codegen.Target;
    using TextReader = System.IO.TextReader;
    using TextWriter = System.IO.TextWriter;
    using TimeSpan = System.TimeSpan;
    using Tool = Antlr3.AntlrTool;

    /** Represents a grammar in memory. */
    public class Grammar
    {
        public const string SynpredRulePrefix = "synpred";

        public const string GrammarFileExtension = ".g";

        /** used for generating lexer temp files */
        public const string LexerGrammarFileExtension = ".g";

        public const int InitialDecisionListSize = 300;
        public const int InvalidRuleIndex = -1;

        public static string[] LabelTypeToString = { "<invalid>", "rule", "token", "rule-list", "token-list", "wildcard-tree", "wildcard-tree-list" };

        public const string ArtificialTokensRuleName = "Tokens";
        public const string FragmentRuleModifier = "fragment";

        public const string SynpredGateActionName = "synpredgate";

        /** When converting ANTLR char and string literals, here is the
         *  value set of escape chars.
         */
        public static int[] AntlrLiteralEscapedCharValue = new int[255];

        /** Given a char, we need to be able to show as an ANTLR literal.
         */
        public static string[] AntlrLiteralCharValueEscape = new string[255];

        static Grammar()
        {
            AntlrLiteralEscapedCharValue['n'] = '\n';
            AntlrLiteralEscapedCharValue['r'] = '\r';
            AntlrLiteralEscapedCharValue['t'] = '\t';
            AntlrLiteralEscapedCharValue['b'] = '\b';
            AntlrLiteralEscapedCharValue['f'] = '\f';
            AntlrLiteralEscapedCharValue['\\'] = '\\';
            AntlrLiteralEscapedCharValue['\''] = '\'';
            AntlrLiteralEscapedCharValue['"'] = '"';
            AntlrLiteralCharValueEscape['\n'] = "\\n";
            AntlrLiteralCharValueEscape['\r'] = "\\r";
            AntlrLiteralCharValueEscape['\t'] = "\\t";
            AntlrLiteralCharValueEscape['\b'] = "\\b";
            AntlrLiteralCharValueEscape['\f'] = "\\f";
            AntlrLiteralCharValueEscape['\\'] = "\\\\";
            AntlrLiteralCharValueEscape['\''] = "\\'";
        }

        public static readonly string[] grammarTypeToString =
            new string[]
            {
		        "<invalid>",
		        "lexer",
		        "parser",
		        "tree",
		        "combined"
	        };

        public static readonly string[] grammarTypeToFileNameSuffix =
            new string[]
            {
                "<invalid>",
                "Lexer",
                "Parser",
                "", // no suffix for tree grammars
                "Parser" // if combined grammar, gen Parser and Lexer will be done later
            };

        /** Set of valid imports.  E.g., can only import a tree parser into
         *  another tree parser.  Maps delegate to set of delegator grammar types.
         *  validDelegations.get(LEXER) gives list of the kinds of delegators
         *  that can import lexers.
         */
        public static MultiMap<GrammarType, GrammarType> validDelegations =
            new MultiMap<GrammarType, GrammarType>()
            {
                { GrammarType.Lexer, GrammarType.Lexer },
                { GrammarType.Lexer, GrammarType.Parser },
                { GrammarType.Lexer, GrammarType.Combined },

                { GrammarType.Parser, GrammarType.Parser },
                { GrammarType.Parser, GrammarType.Combined },

                { GrammarType.TreeParser, GrammarType.TreeParser },
                // allow GrammarType.Combined
                //{ GrammarType.Combined, GrammarType.Combined }
            };

        /** This is the buffer of *all* tokens found in the grammar file
         *  including whitespace tokens etc...  I use this to extract
         *  lexer rules from combined grammars.
         */
        internal Antlr.Runtime.CommonTokenStream tokenBuffer;
        public const string IGNORE_STRING_IN_GRAMMAR_FILE_NAME = "__";
        public const string AUTO_GENERATED_TOKEN_NAME_PREFIX = "T__";

        public class Decision
        {
            public Grammar grammar;
            public int decision;
            public NFAState startState;
            public GrammarAST blockAST;
            public DFA dfa;
        }

        public class LabelElementPair
        {
            [CLSCompliant(false)]
            public Grammar _outer;
            public IToken label;
            public GrammarAST elementRef;
            public string referencedRuleName;
            /** Has an action referenced the label?  Set by ActionAnalysis.g
             *  Currently only set for rule labels.
             */
            public bool actionReferencesLabel;
            public LabelType type; // in {RULE_LABEL,TOKEN_LABEL,RULE_LIST_LABEL,TOKEN_LIST_LABEL}
            public LabelElementPair( Grammar outer, IToken label, GrammarAST elementRef )
            {
                this._outer = outer;
                this.label = label;
                this.elementRef = elementRef;
                this.referencedRuleName = elementRef.Text;
            }
            public Rule GetReferencedRule()
            {
                return _outer.GetRule( referencedRuleName );
            }
            public override string ToString()
            {
                return elementRef.ToString();
            }
        }

        /** What name did the user provide for this grammar? */
        public string name;

        /** What type of grammar is this: lexer, parser, tree walker */
        public GrammarType type;

        /** A list of options specified at the grammar level such as language=Java.
         *  The value can be an AST for complicated values such as character sets.
         *  There may be code generator specific options in here.  I do no
         *  interpretation of the key/value pairs...they are simply available for
         *  who wants them.
         */
        protected IDictionary<string, object> options;

        public static readonly HashSet<string> legalLexerOptions =
            new HashSet<string>()
            {
                "language",
                "tokenVocab",
                "TokenLabelType",
                "superClass",
                "filter",
                "k",
                "backtrack",
                "memoize"
            };

        public static readonly HashSet<string> legalParserOptions =
            new HashSet<string>()
            {
                "language",
                "tokenVocab",
                "output",
                "rewrite",
                "ASTLabelType",
                "TokenLabelType",
                "superClass",
                "k",
                "backtrack",
                "memoize"
            };

        public static readonly HashSet<string> legalTreeParserOptions =
            new HashSet<string>()
            {
                "language",
                "tokenVocab",
                "output",
                "rewrite",
                "ASTLabelType",
                "TokenLabelType",
                "superClass",
                "k",
                "backtrack",
                "memoize",
                "filter",
            };

        public static readonly HashSet<string> doNotCopyOptionsToLexer =
            new HashSet<string>()
            {
                "output",
                "ASTLabelType",
                "superClass",
                "k",
                "backtrack",
                "memoize",
                "rewrite"
            };

        public static readonly IDictionary<string, object> defaultOptions =
            new Dictionary<string, object>()
            {
                { "language", "CSharp3" }
            };

        public static readonly HashSet<string> legalBlockOptions =
            new HashSet<string>()
            {
                "k",
                "greedy",
                "backtrack",
                "memoize"
            };

        /** What are the default options for a subrule? */
        public static readonly IDictionary<string, object> defaultBlockOptions =
            new Dictionary<string, object>()
            {
                { "greedy", "true" }
            };

        public static readonly IDictionary<string, object> defaultLexerBlockOptions =
            new Dictionary<string, object>()
            {
                { "greedy", "true" }
            };

        // Token options are here to avoid contaminating Token object in runtime

        /** Legal options for terminal refs like ID&lt;node=MyVarNode&gt; */
        public static readonly HashSet<string> legalTokenOptions =
            new HashSet<string>()
            {
                "node",
                "type",
                "text",
                "assoc",
            };

        public const string defaultTokenOption = "type";

        /** Is there a global fixed lookahead set for this grammar?
         *  If 0, nothing specified.  -1 implies we have not looked at
         *  the options table yet to set k.
         */
        protected int global_k = -1;

        /** Map a scope to a map of name:action pairs.
         *  Map<String, Map<String,GrammarAST>>
         *  The code generator will use this to fill holes in the output files.
         *  I track the AST node for the action in case I need the line number
         *  for errors.
         */
        protected Dictionary<string, IDictionary<string, object>> actions = new Dictionary<string, IDictionary<string, object>>();

        /** The NFA that represents the grammar with edges labelled with tokens
         *  or epsilon.  It is more suitable to analysis than an AST representation.
         */
        public NFA nfa;

        protected NFAFactory factory;

        /** If this grammar is part of a larger composite grammar via delegate
         *  statement, then this points at the composite.  The composite holds
         *  a global list of rules, token types, decision numbers, etc...
         */
        public CompositeGrammar composite;

        /** A pointer back into grammar tree.  Needed so we can add delegates. */
        public CompositeGrammarTree compositeTreeNode;

        /** If this is a delegate of another grammar, this is the label used
         *  as an instance var by that grammar to point at this grammar. null
         *  if no label was specified in the delegate statement.
         */
        public string label;

        /** TODO: hook this to the charVocabulary option */
        protected IIntSet charVocabulary = null;

        /** For ANTLRWorks, we want to be able to map a line:col to a specific
         *  decision DFA so it can display DFA.
         */
        Dictionary<string, DFA> lineColumnToLookaheadDFAMap = new Dictionary<string, DFA>();

        public Tool tool;

        /** The unique set of all rule references in any rule; set of tree node
         *  objects so two refs to same rule can exist but at different line/position.
         */
        protected internal HashSet<GrammarAST> ruleRefs = new HashSet<GrammarAST>(GrammarAST.TreeTokenEqualityComparer.Default);

        protected internal HashSet<GrammarAST> scopedRuleRefs = new HashSet<GrammarAST>(GrammarAST.TreeTokenEqualityComparer.Default);

        /** The unique set of all token ID references in any rule */
        protected internal HashSet<IToken> tokenIDRefs = new HashSet<IToken>();

        /** Be able to assign a number to every decision in grammar;
         *  decisions in 1..n
         */
        protected int decisionCount = 0;

        /** A list of all rules that are in any left-recursive cycle.  There
         *  could be multiple cycles, but this is a flat list of all problematic
         *  rules. This is stuff we couldn't refactor to precedence rule.
         */
        protected internal HashSet<Rule> leftRecursiveRules;

        /** An external tool requests that DFA analysis abort prematurely.  Stops
         *  at DFA granularity, which are limited to a DFA size and time computation
         *  as failsafe.
         */
        protected bool externalAnalysisAbort;

        public int numNonLLStar = 0; // hack to track for -report	

        /** When we read in a grammar, we track the list of syntactic predicates
         *  and build faux rules for them later.  See my blog entry Dec 2, 2005:
         *  http://www.antlr.org/blog/antlr3/lookahead.tml
         *  This maps the name (we make up) for a pred to the AST grammar fragment.
         */
        protected List<KeyValuePair<string, GrammarAST>> nameToSynpredASTMap;

        /** Each left-recursive precedence rule must define precedence array
         *  for binary operators like:
         *
         *  	static int[] e_prec = new int[tokenNames.length];
         *  	static {
         *  		e_prec[75] = 1;
         *  	}
         *  Track and we push into parser later; this is computed
         *  early when we look for prec rules.
         */
        public List<string> precRuleInitCodeBlocks = new List<string>();

        /** At least one rule has memoize=true */
        public bool atLeastOneRuleMemoizes;

        /** At least one backtrack=true in rule or decision or grammar. */
        public bool atLeastOneBacktrackOption;

        /** Was this created from a COMBINED grammar? */
        public bool implicitLexer;

        string defaultRuleModifier;

        /** Map a rule to it's Rule object */
        protected readonly SortedList<string, Rule> nameToRuleMap = new SortedList<string, Rule>(StringComparer.Ordinal);

        /** If this rule is a delegate, some rules might be overridden; don't
         *  want to gen code for them.
         */
        public HashSet<string> overriddenRules = new HashSet<string>();

        /** The list of all rules referenced in this grammar, not defined here,
         *  and defined in a delegate grammar.  Not all of these will be generated
         *  in the recognizer for this file; only those that are affected by rule
         *  definitions in this grammar.  I am not sure the Java target will need
         *  this but I'm leaving in case other targets need it.
         *  see NameSpaceChecker.lookForReferencesToUndefinedSymbols()
         */
        protected internal HashSet<Rule> delegatedRuleReferences = new HashSet<Rule>();

        /** The ANTLRParser tracks lexer rules when reading combined grammars
         *  so we can build the Tokens rule.
         */
        public IList<string> lexerRuleNamesInCombined = new List<string>();

        /** Track the scopes defined outside of rules and the scopes associated
         *  with all rules (even if empty).
         */
        protected IDictionary<string, AttributeScope> scopes = new Dictionary<string, AttributeScope>();

        /** An AST that records entire input grammar with all rules.  A simple
         *  grammar with one rule, "grammar t; a : A | B ;", looks like:
         * ( grammar t ( rule a ( BLOCK ( ALT A ) ( ALT B ) ) <end-of-rule> ) )
         */
        protected GrammarAST grammarTree = null;

        /** Each subrule/rule is a decision point and we must track them so we
         *  can go back later and build DFA predictors for them.  This includes
         *  all the rules, subrules, optional blocks, ()+, ()* etc...
         */
        protected List<Decision> indexToDecision =
            new List<Decision>( InitialDecisionListSize );

        /** If non-null, this is the code generator we will use to generate
         *  recognizers in the target language.
         */
        protected internal CodeGenerator generator;

        public NameSpaceChecker nameSpaceChecker;

        public LL1Analyzer ll1Analyzer;

        /** For merged lexer/parsers, we must construct a separate lexer spec.
         *  This is the template for lexer; put the literals first then the
         *  regular rules.  We don't need to specify a token vocab import as
         *  I make the new grammar import from the old all in memory; don't want
         *  to force it to read from the disk.  Lexer grammar will have same
         *  name as original grammar but will be in different filename.  Foo.g
         *  with combined grammar will have FooParser.java generated and
         *  Foo__.g with again Foo inside.  It will however generate FooLexer.java
         *  as it's a lexer grammar.  A bit odd, but autogenerated.  Can tweak
         *  later if we want.
         */
        StringTemplate lexerGrammarST;
        protected StringTemplate LexerGrammarST
        {
            get
            {
                if ( lexerGrammarST == null )
                {
                    lexerGrammarST = new StringTemplate(
                        "lexer grammar <name>;\n" +
                        "<if(options)>" +
                        "options {\n" +
                        "  <options:{<it.name>=<it.value>;<\\n>}>\n" +
                        "}<\\n>\n" +
                        "<endif>\n" +
                        "<if(imports)>import <imports; separator=\", \">;<endif>\n" +
                        "<actionNames,actions:{n,a|@<n> {<a>}\n}>\n" +
                        "<literals:{<it.ruleName> : <it.literal> ;\n}>\n" +
                        "<rules>",
                        typeof( AngleBracketTemplateLexer )
                        );
                }

                return lexerGrammarST;
            }
        }

        /** What file name holds this grammar? */
        protected internal string fileName;

        /** How long in ms did it take to build DFAs for this grammar?
         *  If this grammar is a combined grammar, it only records time for
         *  the parser grammar component.  This only records the time to
         *  do the LL(*) work; NFA->DFA conversion.
         */
        public TimeSpan DFACreationWallClockTimeInMS;

        public int numberOfSemanticPredicates = 0;
        public int numberOfManualLookaheadOptions = 0;
        public HashSet<int> setOfNondeterministicDecisionNumbers = new HashSet<int>();
        public HashSet<int> setOfNondeterministicDecisionNumbersResolvedWithPredicates =
            new HashSet<int>();

        /** Track decisions with syn preds specified for reporting.
         *  This is the a set of BLOCK type AST nodes.
         */
        public HashSet<GrammarAST> blocksWithSynPreds = new HashSet<GrammarAST>(GrammarAST.TreeTokenEqualityComparer.Default);

        /** Track decisions that actually use the syn preds in the DFA.
         *  Computed during NFA to DFA conversion.
         */
        public HashSet<DFA> decisionsWhoseDFAsUsesSynPreds = new HashSet<DFA>();

        /** Track names of preds so we can avoid generating preds that aren't used
         *  Computed during NFA to DFA conversion.  Just walk accept states
         *  and look for synpreds because that is the only state target whose
         *  incident edges can have synpreds.  Same is try for
         *  decisionsWhoseDFAsUsesSynPreds.
         */
        public HashSet<string> synPredNamesUsedInDFA = new HashSet<string>();

        /** Track decisions with syn preds specified for reporting.
         *  This is the a set of BLOCK type AST nodes.
         */
        public HashSet<GrammarAST> blocksWithSemPreds = new HashSet<GrammarAST>(GrammarAST.TreeTokenEqualityComparer.Default);

        /** Track decisions that actually use the syn preds in the DFA. */
        public HashSet<DFA> decisionsWhoseDFAsUsesSemPreds = new HashSet<DFA>();

        protected bool allDecisionDFACreated = false;

        /** We need a way to detect when a lexer grammar is autogenerated from
         *  another grammar or we are just sending in a string representing a
         *  grammar.  We don't want to generate a .tokens file, for example,
         *  in such cases.
         */
        protected bool builtFromString = false;

        /** Factored out the sanity checking code; delegate to it. */
        internal GrammarSanity sanity;

        /** Useful for asking questions about target during analysis */
        private Target target;

        /** Create a grammar from file name.  */
        public Grammar( Tool tool, string fileName, CompositeGrammar composite )
        {
            nameSpaceChecker = new NameSpaceChecker( this );
            ll1Analyzer = new LL1Analyzer( this );
            sanity = new GrammarSanity( this );

            this.composite = composite;
            Tool = tool;
            FileName = fileName;
            // ensure we have the composite set to something
            if ( composite.delegateGrammarTreeRoot == null )
            {
                composite.SetDelegationRoot( this );
            }
            else
            {
                defaultRuleModifier = composite.delegateGrammarTreeRoot.grammar.DefaultRuleModifier;
            }
            target = CodeGenerator.LoadLanguageTarget((string)GetOption("language"), tool.TargetsDirectory);
        }

        public Grammar()
            : this(default(Tool))
        {
        }

        /** Useful for when you are sure that you are not part of a composite
         *  already.  Used in Interp/RandomPhrase and testing.
         */
        public Grammar(Tool tool)
        {
            nameSpaceChecker = new NameSpaceChecker( this );
            ll1Analyzer = new LL1Analyzer( this );
            sanity = new GrammarSanity( this );

            builtFromString = true;
            composite = new CompositeGrammar( this );
            Tool = tool;
            string targetsDirectory = Path.Combine(AntlrTool.ToolPathRoot, "Targets");
            target = CodeGenerator.LoadLanguageTarget((string)GetOption("language"), targetsDirectory);
        }

        /** Used for testing; only useful on noncomposite grammars.*/
        public Grammar( string grammarString )
            : this( null, grammarString )
        {
        }

        /** Used for testing and Interp/RandomPhrase.  Only useful on
         *  noncomposite grammars.
         */
        public Grammar( Tool tool, string grammarString )
            : this(tool)
        {
            Tool = tool;
            FileName = "<string>";
            StringReader r = new StringReader( grammarString );
            ParseAndBuildAST( r );
            composite.AssignTokenTypes();
            //composite.TranslateLeftRecursiveRules();
            AddRulesForSyntacticPredicates();
            composite.DefineGrammarSymbols();
            //composite.CreateNFAs();
            CheckNameSpaceAndActions();
        }

        #region Properties
        [CLSCompliant(false)]
        public IDictionary<string, IDictionary<string, object>> Actions
        {
            get
            {
                return actions;
            }
        }
        /** If there is a char vocabulary, use it; else return min to max char
         *  as defined by the target.  If no target, use max unicode char value.
         */
        public IIntSet AllCharValues
        {
            get
            {
                if ( charVocabulary != null )
                {
                    return charVocabulary;
                }
                IIntSet allChar = IntervalSet.Of( Label.MIN_CHAR_VALUE, MaxCharValue );
                return allChar;
            }
        }
        public bool AllDecisionDFAHaveBeenCreated
        {
            get
            {
                return allDecisionDFACreated;
            }
        }
        public bool BuildAST
        {
            get
            {
                string outputType = (string)GetOption( "output" );
                if ( outputType != null )
                {
                    return outputType.Equals( "AST" );
                }
                return false;
            }
        }
        public bool BuildTemplate
        {
            get
            {
                string outputType = (string)GetOption( "output" );
                if ( outputType != null )
                {
                    return outputType.Equals( "template" );
                }
                return false;
            }
        }
        public CodeGenerator CodeGenerator
        {
            get
            {
                return generator;
            }
            set
            {
                generator = value;
            }
        }

        public virtual ReadOnlyCollection<Decision> Decisions
        {
            get
            {
                return indexToDecision.AsReadOnly();
            }
        }

        public string DefaultRuleModifier
        {
            get
            {
                return defaultRuleModifier;
            }
            set
            {
                defaultRuleModifier = value;
            }
        }
        [CLSCompliant(false)]
        public ICollection<Rule> DelegatedRuleReferences
        {
            get
            {
                return GetDelegatedRuleReferences();
            }
        }
        public ICollection<Grammar> Delegates
        {
            get
            {
                return GetDelegates();
            }
        }
        /** Who's my direct parent grammar? */
        public Grammar Delegator
        {
            get
            {
                return composite.GetDelegator( this );
            }
        }
        public ICollection<Grammar> Delegators
        {
            get
            {
                return GetDelegators();
            }
        }
        public ICollection<Grammar> DirectDelegates
        {
            get
            {
                return GetDirectDelegates();
            }
        }
        [CLSCompliant(false)]
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }
        public IDictionary<string, AttributeScope> GlobalScopes
        {
            get
            {
                return scopes;
            }
        }
        public string GrammarTypeString
        {
            get
            {
                return grammarTypeToString[(int)type];
            }
        }
        public bool HasDelegates
        {
            get
            {
                return GetDelegates().Any();
            }
        }
        public string ImplicitlyGeneratedLexerFileName
        {
            get
            {
                return name +
                       IGNORE_STRING_IN_GRAMMAR_FILE_NAME +
                       LexerGrammarFileExtension;
            }
        }
        public ICollection<Grammar> IndirectDelegates
        {
            get
            {
                return GetIndirectDelegates();
            }
        }
        public bool IsBuiltFromString
        {
            get
            {
                return builtFromString;
            }
        }
        public bool IsRoot
        {
            get
            {
                return composite.delegateGrammarTreeRoot.grammar == this;
            }
        }

        // this is used by the codegen templates
        [System.Obsolete]
        public bool GrammarIsRoot
        {
            get
            {
                return IsRoot;
            }
        }

        public IDictionary<string, DFA> LineColumnToLookaheadDFAMap
        {
            get
            {
                return lineColumnToLookaheadDFAMap;
            }
        }
        /** What is the max char value possible for this grammar's target?  Use
         *  unicode max if no target defined.
         */
        public int MaxCharValue
        {
            get
            {
                if ( generator != null )
                {
                    return generator.target.GetMaxCharValue( generator );
                }
                else
                {
                    return Label.MAX_CHAR_VALUE;
                }
            }
        }
        /** How many token types have been allocated so far? */
        public int MaxTokenType
        {
            get
            {
                return composite.maxTokenType;
            }
        }
        public int MaxLookahead
        {
            get
            {
                return GetGrammarMaxLookahead();
            }
        }
        public int NumberOfDecisions
        {
            get
            {
                return decisionCount;
            }
        }
        public string RecognizerName
        {
            get
            {
                return GetRecognizerName();
            }
        }
        public bool RewriteMode
        {
            get
            {
                string outputType = (string)GetOption( "rewrite" );
                if ( outputType != null )
                {
                    return outputType.Equals( "true" );
                }
                return false;
            }
        }
        public ICollection<Rule> Rules
        {
            get
            {
                return nameToRuleMap.Values;
            }
        }
        /** Get the list of ANTLR String literals */
        public ICollection<string> StringLiterals
        {
            get
            {
                return composite.stringLiteralToTypeMap.Keys;
            }
        }
        public IList<KeyValuePair<string, GrammarAST>> SyntacticPredicates
        {
            get
            {
                return nameToSynpredASTMap;
            }
        }
        /** Get the list of tokens that are IDs like BLOCK and LPAREN */
        public ICollection<string> TokenIDs
        {
            get
            {
                return composite.tokenIDToTypeMap.Keys;
            }
        }
        /** Return a set of all possible token or char types for this grammar */
        public IIntSet TokenTypes
        {
            get
            {
                if ( type == GrammarType.Lexer )
                {
                    return AllCharValues;
                }
                return IntervalSet.Of( Label.MIN_TOKEN_TYPE, MaxTokenType );
            }
        }
        [CLSCompliant(false)]
        public AntlrTool Tool
        {
            get
            {
                return tool;
            }
            set
            {
                tool = value;
            }
        }
        public GrammarAST Tree
        {
            get
            {
                return grammarTree;
            }
        }
        #endregion

        public virtual void SetName( string name )
        {
            if ( name == null )
            {
                return;
            }
            // don't error check autogenerated files (those with '__' in them)
            //String saneFile = fileName.replace( '\\', '/' );
            //int lastSlash = saneFile.lastIndexOf( '/' );
            //String onlyFileName = saneFile.substring( lastSlash + 1, fileName.length() );
            //string onlyFileName = fileName;
            //if ( builtFromString )
            //{
            //    string saneFile = fileName.Replace( '\\', '/' );
            //    onlyFileName = saneFile.Split( '/' ).LastOrDefault();
            //}
            if ( !builtFromString )
            {
                string onlyFileName = System.IO.Path.GetFileName( fileName );
                string onlyFileNameNoSuffix = System.IO.Path.GetFileNameWithoutExtension( onlyFileName );

                //int lastDot = onlyFileName.lastIndexOf( '.' );
                //String onlyFileNameNoSuffix = null;
                if ( onlyFileNameNoSuffix == onlyFileName )
                {
                    ErrorManager.Error( ErrorManager.MSG_FILENAME_EXTENSION_ERROR, fileName );
                    onlyFileNameNoSuffix = onlyFileName + GrammarFileExtension;
                }
                else
                {
                    //onlyFileNameNoSuffix = onlyFileName.substring( 0, lastDot );
                }
                if ( !name.Equals( onlyFileNameNoSuffix ) )
                {
                    ErrorManager.Error( ErrorManager.MSG_FILE_AND_GRAMMAR_NAME_DIFFER,
                                       name,
                                       fileName );
                }
            }
            this.name = name;
        }

        public virtual void SetGrammarContent( string grammarString )
        {
            StringReader r = new StringReader( grammarString );
            ParseAndBuildAST( r );
            composite.AssignTokenTypes();
            composite.DefineGrammarSymbols();
        }

        public virtual void ParseAndBuildAST()
        {
            using ( System.IO.TextReader reader = System.IO.File.OpenText( fileName ) )
            {
                ParseAndBuildAST( reader );
            }
        }

        class TokenStreamRewriteEngine : Antlr.Runtime.ITokenSource
        {
            Antlr.Runtime.ITokenSource _source;
            int[] _discard;

            public TokenStreamRewriteEngine( Antlr.Runtime.ITokenSource source )
            {
                _source = source;
            }

            public virtual void Discard( params int[] tokenKinds )
            {
                _discard = (int[])tokenKinds.Clone();
            }

            public IToken NextToken()
            {
                IToken next = _source.NextToken();
                while ( next != null && _discard.Contains( next.Type ) )
                    next = _source.NextToken();

                return next;
            }

            public string SourceName
            {
                get
                {
                    return _source.SourceName;
                }
            }

            public string[] TokenNames
            {
                get
                {
                    return _source.TokenNames;
                }
            }
        }

        public virtual void ParseAndBuildAST( TextReader r )
        {
            // BUILD AST FROM GRAMMAR
            ANTLRLexer lexer = new ANTLRLexer( new Antlr.Runtime.ANTLRReaderStream( r ) );
            lexer.Filename = this.FileName;
            // use the rewrite engine because we want to buffer up all tokens
            // in case they have a merged lexer/parser, send lexer rules to
            // new grammar.
            //lexer.setTokenObjectClass( "antlr.TokenWithIndex" );
            tokenBuffer = new Antlr.Runtime.CommonTokenStream( lexer );
            //tokenBuffer = new TokenStreamRewriteEngine( lexer );
            //tokenBuffer.Discard( ANTLRParser.WS, ANTLRParser.ML_COMMENT, ANTLRParser.COMMENT, ANTLRParser.SL_COMMENT );
            //tokenBuffer.discard( ANTLRParser.WS );
            //tokenBuffer.discard( ANTLRParser.ML_COMMENT );
            //tokenBuffer.discard( ANTLRParser.COMMENT );
            //tokenBuffer.discard( ANTLRParser.SL_COMMENT );
            ANTLRParser parser = new ANTLRParser( tokenBuffer );
            parser.FileName = this.FileName;
            ANTLRParser.grammar__return result = null;
            try
            {
                result = parser.grammar_( this );
            }
            //catch ( TokenStreamException tse )
            //{
            //    ErrorManager.internalError( "unexpected stream error from parsing " + fileName, tse );
            //}
            catch ( RecognitionException re )
            {
                ErrorManager.InternalError( "unexpected parser recognition error from " + fileName, re );
            }

            DealWithTreeFilterMode(); // tree grammar and filter=true?

            if ( lexer.hasASTOperator && !BuildAST )
            {
                object value = GetOption( "output" );
                if ( value == null )
                {
                    ErrorManager.GrammarWarning( ErrorManager.MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION,
                                                this, null );
                    SetOption( "output", "AST", null );
                }
                else
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_AST_OP_WITH_NON_AST_OUTPUT_OPTION,
                                              this, null, value );
                }
            }

            grammarTree = result.Tree;
            if (grammarTree != null && AntlrTool.internalOption_PrintGrammarTree)
                Console.WriteLine("grammar tree: " + grammarTree.ToStringTree());

            grammarTree.SetUnknownTokenBoundaries();

            FileName = lexer.Filename; // the lexer #src might change name
            if ( grammarTree == null || grammarTree.FindFirstType( ANTLRParser.RULE ) == null )
            {
                ErrorManager.Error( ErrorManager.MSG_NO_RULES, FileName );
                return;
            }
        }

        protected virtual void DealWithTreeFilterMode()
        {
            object filterMode = (string)GetOption( "filter" );
            if ( type == GrammarType.TreeParser && filterMode != null && filterMode.ToString().Equals( "true" ) )
            {
                // check for conflicting options
                // filter => backtrack=true
                // filter&&output=AST => rewrite=true
                // filter&&output!=AST => error
                // any deviation from valid option set is an error
                object backtrack = (string)GetOption( "backtrack" );
                object output = GetOption( "output" );
                object rewrite = GetOption( "rewrite" );
                if ( backtrack != null && !backtrack.ToString().Equals( "true" ) )
                {
                    ErrorManager.Error( ErrorManager.MSG_CONFLICTING_OPTION_IN_TREE_FILTER,
                                       "backtrack", backtrack );
                }
                if ( output != null && !output.ToString().Equals( "AST" ) )
                {
                    ErrorManager.Error( ErrorManager.MSG_CONFLICTING_OPTION_IN_TREE_FILTER,
                                       "output", output );
                    SetOption( "output", "", null );
                }
                if ( rewrite != null && !rewrite.ToString().Equals( "true" ) )
                {
                    ErrorManager.Error( ErrorManager.MSG_CONFLICTING_OPTION_IN_TREE_FILTER,
                                       "rewrite", rewrite );
                }
                // set options properly
                SetOption( "backtrack", "true", null );
                if ( output != null && output.ToString().Equals( "AST" ) )
                {
                    SetOption( "rewrite", "true", null );
                }
                // @synpredgate set to state.backtracking==1 by code gen when filter=true
                // superClass set in template target::treeParser
            }
        }

        public void TranslateLeftRecursiveRule(GrammarAST ruleAST)
        {
            var input = new Antlr.Runtime.Tree.CommonTreeNodeStream(ruleAST);
            LeftRecursiveRuleAnalyzer leftRecursiveRuleWalker = new LeftRecursiveRuleAnalyzer(input, this, ruleAST.enclosingRuleName);
            bool isLeftRec = false;
            try
            {
                //System.out.println("TESTING "+ruleAST.enclosingRuleName);
                isLeftRec = leftRecursiveRuleWalker.rec_rule(this);
            }
            catch (RecognitionException re)
            {
                ErrorManager.Error(ErrorManager.MSG_BAD_AST_STRUCTURE, re);
            }

            if (!isLeftRec)
                return;

            List<string> rules = new List<string>();
            rules.Add(leftRecursiveRuleWalker.GetArtificialPrecStartRule());
            rules.Add(leftRecursiveRuleWalker.GetArtificialOpPrecRule());
            rules.Add(leftRecursiveRuleWalker.GetArtificialPrimaryRule());
            foreach (string r in rules)
            {
                GrammarAST t = ParseArtificialRule(r);
                AddRule(grammarTree, t);
                //System.out.println(t.toStringTree());
            }

            //precRuleInitCodeBlocks.add( precRuleWalker.getOpPrecJavaCode() );
        }

        public virtual void DefineGrammarSymbols()
        {
            if ( Tool.internalOption_PrintGrammarTree )
            {
                Console.Out.WriteLine( grammarTree.ToStringList() );
            }

            // DEFINE RULES
            //JSystem.@out.println("### define "+name+" rules");
            DefineGrammarItemsWalker defineItemsWalker = new DefineGrammarItemsWalker( new Antlr.Runtime.Tree.CommonTreeNodeStream( grammarTree ) );
            //defineItemsWalker.setASTNodeClass( "org.antlr.tool.GrammarAST" );
            try
            {
                defineItemsWalker.grammar_( this );
            }
            catch ( RecognitionException re )
            {
                ErrorManager.Error( ErrorManager.MSG_BAD_AST_STRUCTURE,
                                   re );
            }
        }

        /** ANALYZE ACTIONS, LOOKING FOR LABEL AND ATTR REFS, sanity check */
        public virtual void CheckNameSpaceAndActions()
        {
            ExamineAllExecutableActions();
            CheckAllRulesForUselessLabels();

            nameSpaceChecker.CheckConflicts();
        }

        /** Many imports are illegal such as lexer into a tree grammar */
        public virtual bool ValidImport( Grammar @delegate )
        {
            IList<GrammarType> validDelegators = validDelegations.get( @delegate.type );
            return validDelegators != null && validDelegators.Contains( this.type );
        }

        /** If the grammar is a combined grammar, return the text of the implicit
         *  lexer grammar.
         */
        public virtual string GetLexerGrammar()
        {
            if ( LexerGrammarST.GetAttribute( "literals" ) == null &&
                 LexerGrammarST.GetAttribute( "rules" ) == null )
            {
                // if no rules, return nothing
                return null;
            }
            LexerGrammarST.SetAttribute( "name", name );
            // if there are any actions set for lexer, pass them in
            if ( actions.get( "lexer" ) != null )
            {
                LexerGrammarST.SetAttribute( "actionNames",
                                            ( (IDictionary)actions.get( "lexer" ) ).Keys );
                LexerGrammarST.SetAttribute( "actions",
                                            ( (IDictionary)actions.get( "lexer" ) ).Values );
            }
            // make sure generated grammar has the same options
            if ( options != null )
            {
                foreach ( var option in options )
                {
                    string optionName = option.Key;
                    if ( !doNotCopyOptionsToLexer.Contains( optionName ) )
                    {
                        object value = option.Value;
                        LexerGrammarST.SetAttribute( "options.{name,value}", optionName, value );
                    }
                }
            }
            return LexerGrammarST.ToString();
        }

        /** Get the name of the generated recognizer; may or may not be same
         *  as grammar name.
         *  Recognizer is TParser and TLexer from T if combined, else
         *  just use T regardless of grammar type.
         */
        public virtual string GetRecognizerName()
        {
            string suffix = "";
            IList<Grammar> grammarsFromRootToMe = composite.GetDelegators( this );
            //JSystem.@out.println("grammarsFromRootToMe="+grammarsFromRootToMe);
            string qualifiedName = name;
            if ( grammarsFromRootToMe != null )
            {
                StringBuilder buf = new StringBuilder();
                foreach ( Grammar g in grammarsFromRootToMe )
                {
                    buf.Append( g.name );
                    buf.Append( '_' );
                }
                buf.Append( name );
                qualifiedName = buf.ToString();
            }
            if ( type == GrammarType.Combined ||
                 ( type == GrammarType.Lexer && implicitLexer ) )
            {
                suffix = Grammar.grammarTypeToFileNameSuffix[(int)type];
            }
            return qualifiedName + suffix;
        }

        /** Parse a rule we add artificially that is a list of the other lexer
         *  rules like this: "Tokens : ID | INT | SEMI ;"  nextToken() will invoke
         *  this to set the current token.  Add char literals before
         *  the rule references.
         *
         *  If in filter mode, we want every alt to backtrack and we need to
         *  do k=1 to force the "first token def wins" rule.  Otherwise, the
         *  longest-match rule comes into play with LL(*).
         *
         *  The ANTLRParser antlr.g file now invokes this when parsing a lexer
         *  grammar, which I think is proper even though it peeks at the info
         *  that later phases will (re)compute.  It gets a list of lexer rules
         *  and builds a string representing the rule; then it creates a parser
         *  and adds the resulting tree to the grammar's tree.
         */
        public GrammarAST AddArtificialMatchTokensRule( GrammarAST grammarAST,
                                                       IList<string> ruleNames,
                                                       IList<string> delegateNames,
                                                       bool filterMode )
        {
            StringTemplate matchTokenRuleST = null;
            if ( filterMode )
            {
                matchTokenRuleST = new StringTemplate(
                        ArtificialTokensRuleName +
                        " options {k=1; backtrack=true;} : <rules; separator=\"|\">;",
                        typeof( AngleBracketTemplateLexer ) );
            }
            else
            {
                matchTokenRuleST = new StringTemplate(
                        ArtificialTokensRuleName + " : <rules; separator=\"|\">;",
                        typeof( AngleBracketTemplateLexer ) );
            }

            // Now add token rule references
            for ( int i = 0; i < ruleNames.Count; i++ )
            {
                string rname = (string)ruleNames[i];
                matchTokenRuleST.SetAttribute( "rules", rname );
            }
            for ( int i = 0; i < delegateNames.Count; i++ )
            {
                string dname = (string)delegateNames[i];
                matchTokenRuleST.SetAttribute( "rules", dname + ".Tokens" );
            }
            //JSystem.@out.println("tokens rule: "+matchTokenRuleST.toString());

            ////ANTLRLexer lexer = new ANTLRLexer( new StringReader( matchTokenRuleST.toString() ) );
            ////lexer.setTokenObjectClass( "antlr.TokenWithIndex" );
            ////TokenStreamRewriteEngine tokbuf =
            ////    new TokenStreamRewriteEngine( lexer );
            ////tokbuf.discard( ANTLRParser.WS );
            ////tokbuf.discard( ANTLRParser.ML_COMMENT );
            ////tokbuf.discard( ANTLRParser.COMMENT );
            ////tokbuf.discard( ANTLRParser.SL_COMMENT );
            ////ANTLRParser parser = new ANTLRParser( tokbuf );
            //ANTLRLexer lexer = new ANTLRLexer( new Antlr.Runtime.ANTLRStringStream( matchTokenRuleST.ToString() ) );
            //TokenStreamRewriteEngine tokbuf = new TokenStreamRewriteEngine( lexer );
            //tokbuf.Discard( ANTLRParser.WS, ANTLRParser.ML_COMMENT, ANTLRParser.COMMENT, ANTLRParser.SL_COMMENT );
            //ANTLRParser parser = new ANTLRParser( new Antlr.Runtime.CommonTokenStream( tokbuf ) );

            //parser.Grammar = this;
            //parser.GrammarType = GrammarType.Lexer;
            //ANTLRParser.rule_return result = null;
            //try
            //{
            //    result = parser.rule();
            //    if ( Tool.internalOption_PrintGrammarTree )
            //    {
            //        Console.Out.WriteLine( "Tokens rule: " + ( (ITree)result.Tree ).ToStringTree() );
            //    }
            //    GrammarAST p = grammarAST;
            //    while ( p.Type != ANTLRParser.LEXER_GRAMMAR )
            //    {
            //        p = (GrammarAST)p.getNextSibling();
            //    }
            //    p.AddChild( (Antlr.Runtime.Tree.ITree)result.Tree );
            //}
            //catch ( Exception e )
            //{
            //    ErrorManager.Error( ErrorManager.MSG_ERROR_CREATING_ARTIFICIAL_RULE,
            //                       e );
            //}
            //return (GrammarAST)result.Tree;

            GrammarAST r = ParseArtificialRule(matchTokenRuleST.ToString());
            AddRule(grammarAST, r);
            //addRule((GrammarAST)parser.getAST());
            //return (GrammarAST)parser.getAST();
            return r;
        }

        public GrammarAST ParseArtificialRule(string ruleText)
        {
            ANTLRLexer lexer = new ANTLRLexer(new Antlr.Runtime.ANTLRStringStream(ruleText));
            TokenStreamRewriteEngine tokbuf = new TokenStreamRewriteEngine(lexer);
            tokbuf.Discard(ANTLRParser.WS, ANTLRParser.ML_COMMENT, ANTLRParser.COMMENT, ANTLRParser.SL_COMMENT);
            ANTLRParser parser = new ANTLRParser(new Antlr.Runtime.CommonTokenStream(tokbuf));
            parser.Grammar = this;
            parser.GrammarType = this.type;
            try
            {
                ANTLRParser.rule_return result = parser.rule();
                return result.Tree;
            }
            catch (Exception e)
            {
                ErrorManager.Error(ErrorManager.MSG_ERROR_CREATING_ARTIFICIAL_RULE, e);
                return null;
            }
        }

        public void AddRule(GrammarAST grammarTree, GrammarAST t)
        {
            GrammarAST p = null;
            for (int i = 0; i < grammarTree.ChildCount; i++)
            {
                p = (GrammarAST)grammarTree.GetChild(i);
                if (p == null || p.Type == ANTLRParser.RULE || p.Type == ANTLRParser.PREC_RULE)
                    break;
            }

            if (p != null)
                grammarTree.AddChild(t);
        }

        /** for any syntactic predicates, we need to define rules for them; they will get
         *  defined automatically like any other rule. :)
         */
        [CLSCompliant(false)]
        protected virtual IList<GrammarAST> GetArtificialRulesForSyntacticPredicates(IEnumerable<KeyValuePair<string, GrammarAST>> nameToSynpredASTMap)
        {
            IList<GrammarAST> rules = new List<GrammarAST>();
            if (nameToSynpredASTMap == null)
            {
                return rules;
            }

            bool isLexer = grammarTree.Type == ANTLRParser.LEXER_GRAMMAR;
            foreach (var synpred in nameToSynpredASTMap)
            {
                string synpredName = synpred.Key;
                GrammarAST fragmentAST = synpred.Value;
                GrammarAST ruleAST = ANTLRParser.CreateSimpleRuleAST(synpredName, fragmentAST, isLexer);
                rules.Add(ruleAST);
            }

            return rules;
        }

        public void AddRulesForSyntacticPredicates()
        {
            // Get syn pred rules and add to existing tree
            IList<GrammarAST> synpredRules = GetArtificialRulesForSyntacticPredicates(nameToSynpredASTMap);
            for (int i = 0; i < synpredRules.Count; i++)
            {
                GrammarAST rAST = synpredRules[i];
                grammarTree.AddChild(rAST);
            }
        }

#if false
        /** Walk the list of options, altering this Grammar object according
         *  to any I recognize.
         */
        protected virtual void ProcessOptions()
        {
            foreach ( var option in options )
            {
                string optionName = option.Key;
                object value = option.Value;
                if ( optionName.Equals( "tokenVocab" ) )
                {
                }
            }
        }
#endif

        /** Define all the rule begin/end NFAStates to solve forward reference
         *  issues.  Critical for composite grammars too.
         *  This is normally called on all root/delegates manually and then
         *  buildNFA() is called afterwards because the NFA construction needs
         *  to see rule start/stop states from potentially every grammar. Has
         *  to be have these created a priori.  Testing routines will often
         *  just call buildNFA(), which forces a call to this method if not
         *  done already. Works ONLY for single noncomposite grammars.
         */
        public virtual void CreateRuleStartAndStopNFAStates()
        {
            //JSystem.@out.println("### createRuleStartAndStopNFAStates "+getGrammarTypeString()+" grammar "+name+" NFAs");
            if ( nfa != null )
            {
                return;
            }
            nfa = new NFA( this );
            factory = new NFAFactory( nfa );

            foreach ( Rule r in Rules )
            {
                string ruleName = r.Name;
                NFAState ruleBeginState = factory.NewState();
                ruleBeginState.Description = "rule " + ruleName + " start";
                ruleBeginState.enclosingRule = r;
                r.startState = ruleBeginState;
                NFAState ruleEndState = factory.NewState();
                ruleEndState.Description = "rule " + ruleName + " end";
                ruleEndState.IsAcceptState = true;
                ruleEndState.enclosingRule = r;
                r.stopState = ruleEndState;
            }
        }

        public virtual void BuildNFA()
        {
            if ( nfa == null )
            {
                CreateRuleStartAndStopNFAStates();
            }
            if ( nfa.complete )
            {
                // don't let it create more than once; has side-effects
                return;
            }
            //JSystem.@out.println("### build "+getGrammarTypeString()+" grammar "+name+" NFAs");
            if ( Rules.Count == 0 )
            {
                return;
            }

            Antlr.Runtime.Tree.ITreeNodeStream input = new Antlr.Runtime.Tree.CommonTreeNodeStream( grammarTree );
            TreeToNFAConverter nfaBuilder = new TreeToNFAConverter( this, nfa, factory, input );
            try
            {
                nfaBuilder.grammar_();
            }
            catch ( RecognitionException re )
            {
                ErrorManager.Error( ErrorManager.MSG_BAD_AST_STRUCTURE,
                                   name,
                                   re );
            }
            nfa.complete = true;
        }

        /** For each decision in this grammar, compute a single DFA using the
         *  NFA states associated with the decision.  The DFA construction
         *  determines whether or not the alternatives in the decision are
         *  separable using a regular lookahead language.
         *
         *  Store the lookahead DFAs in the AST created from the user's grammar
         *  so the code generator or whoever can easily access it.
         *
         *  This is a separate method because you might want to create a
         *  Grammar without doing the expensive analysis.
         */
        public virtual void CreateLookaheadDFAs()
        {
            CreateLookaheadDFAs( true );
        }

        public virtual void CreateLookaheadDFAs( bool wackTempStructures )
        {
            if ( nfa == null )
            {
                BuildNFA();
            }

            // CHECK FOR LEFT RECURSION; Make sure we can actually do analysis
            CheckAllRulesForLeftRecursion();

            /*
            // was there a severe problem while sniffing the grammar?
            if ( ErrorManager.doNotAttemptAnalysis() ) {
                return;
            }
            */

            DateTime start = DateTime.Now;

            //JSystem.@out.println("### create DFAs");
            int numDecisions = NumberOfDecisions;
            if ( NFAToDFAConverter.SINGLE_THREADED_NFA_CONVERSION )
            {
                for ( int decision = 1; decision <= numDecisions; decision++ )
                {
                    NFAState decisionStartState = GetDecisionNFAStartState( decision );
                    if ( leftRecursiveRules.Contains( decisionStartState.enclosingRule ) )
                    {
                        // don't bother to process decisions within left recursive rules.
                        if ( composite.watchNFAConversion )
                        {
                            Console.Out.WriteLine( "ignoring decision " + decision +
                                               " within left-recursive rule " + decisionStartState.enclosingRule.Name );
                        }
                        continue;
                    }
                    if ( !externalAnalysisAbort && decisionStartState.NumberOfTransitions > 1 )
                    {
                        Rule r = decisionStartState.enclosingRule;
                        if ( r.isSynPred && !synPredNamesUsedInDFA.Contains( r.Name ) )
                        {
                            continue;
                        }
                        DFA dfa = null;
                        // if k=* or k=1, try LL(1)
                        if ( GetUserMaxLookahead( decision ) == 0 ||
                             GetUserMaxLookahead( decision ) == 1 )
                        {
                            dfa = CreateLL_1_LookaheadDFA( decision );
                        }
                        if ( dfa == null )
                        {
                            if ( composite.watchNFAConversion )
                            {
                                Console.Out.WriteLine( "decision " + decision +
                                                   " not suitable for LL(1)-optimized DFA analysis" );
                            }
                            dfa = CreateLookaheadDFA( decision, wackTempStructures );
                        }
                        if ( dfa.startState == null )
                        {
                            // something went wrong; wipe out DFA
                            SetLookaheadDFA( decision, null );
                        }
                        if ( Tool.internalOption_PrintDFA )
                        {
                            Console.Out.WriteLine( "DFA d=" + decision );
                            FASerializer serializer = new FASerializer( nfa.grammar );
                            string result = serializer.Serialize( dfa.startState );
                            Console.Out.WriteLine( result );
                        }
                    }
                }
            }
            else
            {
                ErrorManager.Info( "two-threaded DFA conversion" );
                // create a barrier expecting n DFA and this main creation thread
                Barrier barrier = new Barrier( 3 );
                // assume 2 CPU for now
                int midpoint = numDecisions / 2;
                NFAConversionThread t1 =
                    new NFAConversionThread( this, barrier, 1, midpoint );
                new System.Threading.Thread( t1.Run ).Start();
                //new Thread( t1 ).start();
                if ( midpoint == ( numDecisions / 2 ) )
                {
                    midpoint++;
                }
                NFAConversionThread t2 =
                    new NFAConversionThread( this, barrier, midpoint, numDecisions );
                new System.Threading.Thread( t2.Run ).Start();
                //new Thread( t2 ).start();
                // wait for these two threads to finish
                try
                {
                    barrier.WaitForRelease();
                }
                //catch ( InterruptedException e )
                //{
                //    ErrorManager.internalError( "what the hell? DFA interruptus", e );
                //}
                catch
                {
                    throw new System.NotImplementedException();
                }
            }

            DateTime stop = DateTime.Now;
            DFACreationWallClockTimeInMS = stop - start;

            // indicate that we've finished building DFA (even if #decisions==0)
            allDecisionDFACreated = true;
        }

        public virtual DFA CreateLL_1_LookaheadDFA( int decision )
        {
            Decision d = GetDecision( decision );
            string enclosingRule = d.startState.enclosingRule.Name;
            Rule r = d.startState.enclosingRule;
            NFAState decisionStartState = GetDecisionNFAStartState( decision );

            if ( composite.watchNFAConversion )
            {
                Console.Out.WriteLine( "--------------------\nattempting LL(1) DFA (d="
                                   + decisionStartState.DecisionNumber + ") for " +
                                   decisionStartState.Description );
            }

            if ( r.isSynPred && !synPredNamesUsedInDFA.Contains( enclosingRule ) )
            {
                return null;
            }

            // compute lookahead for each alt
            int numAlts = GetNumberOfAltsForDecisionNFA( decisionStartState );
            LookaheadSet[] altLook = new LookaheadSet[numAlts + 1];
            for ( int alt = 1; alt <= numAlts; alt++ )
            {
                int walkAlt =
                    decisionStartState.TranslateDisplayAltToWalkAlt( alt );
                NFAState altLeftEdge = GetNFAStateForAltOfDecision( decisionStartState, walkAlt );
                NFAState altStartState = (NFAState)altLeftEdge.transition[0].Target;
                //JSystem.@out.println("alt "+alt+" start state = "+altStartState.stateNumber);
                altLook[alt] = ll1Analyzer.Look( altStartState );
                //JSystem.@out.println("alt "+alt+": "+altLook[alt].toString(this));
            }

            // compare alt i with alt j for disjointness
            bool decisionIsLL_1 = true;
            for ( int i = 1; i <= numAlts; i++ )
            {
                for ( int j = i + 1; j <= numAlts; j++ )
                {
                    /*
                    JSystem.@out.println("compare "+i+", "+j+": "+
                                       altLook[i].toString(this)+" with "+
                                       altLook[j].toString(this));
                    */
                    LookaheadSet collision = altLook[i].Intersection( altLook[j] );
                    if ( !collision.IsNil )
                    {
                        //JSystem.@out.println("collision (non-LL(1)): "+collision.toString(this));
                        decisionIsLL_1 = false;
                        goto outer;
                    }
                }
            }

        outer:
            bool foundConfoundingPredicate =
                ll1Analyzer.DetectConfoundingPredicates( decisionStartState );
            if ( decisionIsLL_1 && !foundConfoundingPredicate )
            {
                // build an LL(1) optimized DFA with edge for each altLook[i]
                if ( NFAToDFAConverter.debug )
                {
                    Console.Out.WriteLine( "decision " + decision + " is simple LL(1)" );
                }
                DFA lookaheadDFA2 = new LL1DFA( decision, decisionStartState, altLook );
                SetLookaheadDFA( decision, lookaheadDFA2 );
                UpdateLineColumnToLookaheadDFAMap( lookaheadDFA2 );
                return lookaheadDFA2;
            }

            // not LL(1) but perhaps we can solve with simplified predicate search
            // even if k=1 set manually, only resolve here if we have preds; i.e.,
            // don't resolve etc...

            /*
            SemanticContext visiblePredicates =
                ll1Analyzer.getPredicates(decisionStartState);
            boolean foundConfoundingPredicate =
                ll1Analyzer.detectConfoundingPredicates(decisionStartState);
                */

            // exit if not forced k=1 or we found a predicate situation we
            // can't handle: predicates in rules invoked from this decision.
            if ( GetUserMaxLookahead( decision ) != 1 || // not manually set to k=1
                 !GetAutoBacktrackMode( decision ) ||
                 foundConfoundingPredicate )
            {
                //JSystem.@out.println("trying LL(*)");
                return null;
            }

            IList<IIntSet> edges = new List<IIntSet>();
            for ( int i = 1; i < altLook.Length; i++ )
            {
                LookaheadSet s = altLook[i];
                edges.Add( (IntervalSet)s.tokenTypeSet );
            }
            IList<IIntSet> disjoint = MakeEdgeSetsDisjoint( edges );
            //JSystem.@out.println("disjoint="+disjoint);

            MultiMap<IntervalSet, int> edgeMap = new MultiMap<IntervalSet, int>();
            for ( int i = 0; i < disjoint.Count; i++ )
            {
                IntervalSet ds = (IntervalSet)disjoint[i];
                for ( int alt = 1; alt < altLook.Length; alt++ )
                {
                    LookaheadSet look = altLook[alt];
                    if ( !ds.And( look.tokenTypeSet ).IsNil )
                    {
                        edgeMap.Map( ds, alt );
                    }
                }
            }
            //JSystem.@out.println("edge map: "+edgeMap);

            // TODO: how do we know we covered stuff?

            // build an LL(1) optimized DFA with edge for each altLook[i]
            DFA lookaheadDFA = new LL1DFA( decision, decisionStartState, edgeMap );
            SetLookaheadDFA( decision, lookaheadDFA );

            // create map from line:col to decision DFA (for ANTLRWorks)
            UpdateLineColumnToLookaheadDFAMap( lookaheadDFA );

            return lookaheadDFA;
        }

        private void UpdateLineColumnToLookaheadDFAMap( DFA lookaheadDFA )
        {
            GrammarAST decisionAST = nfa.grammar.GetDecisionBlockAST( lookaheadDFA.decisionNumber );
            int line = decisionAST.Line;
            int col = decisionAST.CharPositionInLine;
            lineColumnToLookaheadDFAMap[line + ":" + col] = lookaheadDFA;
        }

        protected virtual IList<IIntSet> MakeEdgeSetsDisjoint( IList<IIntSet> edges )
        {
            OrderedHashSet<IIntSet> disjointSets = new OrderedHashSet<IIntSet>();
            // walk each incoming edge label/set and add to disjoint set
            int numEdges = edges.Count;
            for ( int e = 0; e < numEdges; e++ )
            {
                IntervalSet t = (IntervalSet)edges[e];
                if ( disjointSets.Contains( t ) )
                { // exact set present
                    continue;
                }

                // compare t with set i for disjointness
                IntervalSet remainder = t; // remainder starts out as whole set to add
                int numDisjointElements = disjointSets.Size();
                for ( int i = 0; i < numDisjointElements; i++ )
                {
                    IntervalSet s_i = (IntervalSet)disjointSets.Get( i );

                    if ( t.And( s_i ).IsNil )
                    { // nothing in common
                        continue;
                    }
                    //JSystem.@out.println(label+" collides with "+rl);

                    // For any (s_i, t) with s_i&t!=nil replace with (s_i-t, s_i&t)
                    // (ignoring s_i-t if nil; don't put in list)

                    // Replace existing s_i with intersection since we
                    // know that will always be a non nil character class
                    IntervalSet intersection = (IntervalSet)s_i.And( t );
                    disjointSets.Set( i, intersection );

                    // Compute s_i-t to see what is in current set and not in incoming
                    IIntSet existingMinusNewElements = s_i.Subtract( t );
                    //JSystem.@out.println(s_i+"-"+t+"="+existingMinusNewElements);
                    if ( !existingMinusNewElements.IsNil )
                    {
                        // found a new character class, add to the end (doesn't affect
                        // outer loop duration due to n computation a priori.
                        disjointSets.Add( existingMinusNewElements );
                    }

                    // anything left to add to the reachableLabels?
                    remainder = (IntervalSet)t.Subtract( s_i );
                    if ( remainder.IsNil )
                    {
                        break; // nothing left to add to set.  done!
                    }

                    t = remainder;
                }
                if ( !remainder.IsNil )
                {
                    disjointSets.Add( remainder );
                }
            }
            return disjointSets.GetElements();
        }

        public virtual DFA CreateLookaheadDFA( int decision, bool wackTempStructures )
        {
            Decision d = GetDecision( decision );
            string enclosingRule = d.startState.enclosingRule.Name;
            Rule r = d.startState.enclosingRule;

            //JSystem.@out.println("createLookaheadDFA(): "+enclosingRule+" dec "+decision+"; synprednames prev used "+synPredNamesUsedInDFA);
            NFAState decisionStartState = GetDecisionNFAStartState( decision );
            DateTime startDFA = DateTime.MinValue;
            DateTime stopDFA = DateTime.MinValue;
            if ( composite.watchNFAConversion )
            {
                Console.Out.WriteLine( "--------------------\nbuilding lookahead DFA (d="
                                   + decisionStartState.DecisionNumber + ") for " +
                                   decisionStartState.Description );
                startDFA = DateTime.Now;
            }

            DFA lookaheadDFA = new DFA( decision, decisionStartState );
            // Retry to create a simpler DFA if analysis failed (non-LL(*),
            // recursion overflow, or time out).
            bool failed =
                lookaheadDFA.probe.IsNonLLStarDecision ||
                lookaheadDFA.probe.AnalysisOverflowed;
            if ( failed && lookaheadDFA.OkToRetryWithK1 )
            {
                // set k=1 option and try again.
                // First, clean up tracking stuff
                decisionsWhoseDFAsUsesSynPreds.Remove( lookaheadDFA );
                // TODO: clean up synPredNamesUsedInDFA also (harder)
                d.blockAST.SetBlockOption( this, "k", 1 );
                if ( composite.watchNFAConversion )
                {
                    Console.Out.Write( "trying decision " + decision +
                                     " again with k=1; reason: " +
                                     lookaheadDFA.ReasonForFailure );
                }
                lookaheadDFA = null; // make sure other memory is "free" before redoing
                lookaheadDFA = new DFA( decision, decisionStartState );
            }

            SetLookaheadDFA( decision, lookaheadDFA );

            if ( wackTempStructures )
            {
                foreach ( DFAState s in lookaheadDFA.UniqueStates.Values )
                {
                    s.Reset();
                }
            }

            // create map from line:col to decision DFA (for ANTLRWorks)
            UpdateLineColumnToLookaheadDFAMap( lookaheadDFA );

            if ( composite.watchNFAConversion )
            {
                stopDFA = DateTime.Now;
                Console.Out.WriteLine( "cost: " + lookaheadDFA.NumberOfStates +
                                   " states, " + (int)( stopDFA - startDFA ).TotalMilliseconds + " ms" );
            }
            //JSystem.@out.println("after create DFA; synPredNamesUsedInDFA="+synPredNamesUsedInDFA);
            return lookaheadDFA;
        }

        /** Terminate DFA creation (grammar analysis).
         */
        public virtual void ExternallyAbortNFAToDFAConversion()
        {
            externalAnalysisAbort = true;
        }

        public virtual bool NFAToDFAConversionExternallyAborted()
        {
            return externalAnalysisAbort;
        }

        /** Return a new unique integer in the token type space */
        public virtual int GetNewTokenType()
        {
            composite.maxTokenType++;
            return composite.maxTokenType;
        }

        /** Define a token at a particular token type value.  Blast an
         *  old value with a new one.  This is called normal grammar processsing
         *  and during import vocab operations to set tokens with specific values.
         */
        public virtual void DefineToken( string text, int tokenType )
        {
            //JSystem.@out.println("defineToken("+text+", "+tokenType+")");
            if (composite.tokenIDToTypeMap.ContainsKey( text ))
            {
                // already defined?  Must be predefined one like EOF;
                // do nothing
                return;
            }
            // the index in the typeToTokenList table is actually shifted to
            // hold faux labels as you cannot have negative indices.
            if ( text[0] == '\'' )
            {
                composite.stringLiteralToTypeMap[text] = tokenType;
                // track in reverse index too
                if ( tokenType >= composite.typeToStringLiteralList.Count )
                {
                    composite.typeToStringLiteralList.setSize( tokenType + 1 );
                }
                composite.typeToStringLiteralList[tokenType] = text;
            }
            else
            {
                // must be a label like ID
                composite.tokenIDToTypeMap[text] = tokenType;
            }
            int index = Label.NUM_FAUX_LABELS + tokenType - 1;
            //JSystem.@out.println("defining "+name+" token "+text+" at type="+tokenType+", index="+index);
            composite.maxTokenType = Math.Max( composite.maxTokenType, tokenType );
            if ( index >= composite.typeToTokenList.Count )
            {
                composite.typeToTokenList.setSize( index + 1 );
            }
            string prevToken = (string)composite.typeToTokenList[index];
            if ( prevToken == null || prevToken[0] == '\'' )
            {
                // only record if nothing there before or if thing before was a literal
                composite.typeToTokenList[index] = text;
            }
        }

        /** Define a new rule.  A new rule index is created by incrementing
         *  ruleIndex.
         */
        public virtual void DefineRule( IToken ruleToken,
                               string modifier,
                               IDictionary<string,object> options,
                               GrammarAST tree,
                               GrammarAST argActionAST,
                               int numAlts )
        {
            string ruleName = ruleToken.Text;
            if ( GetLocallyDefinedRule( ruleName ) != null )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_RULE_REDEFINITION,
                                          this, ruleToken, ruleName );
                return;
            }

            if ( ( type == GrammarType.Parser || type == GrammarType.TreeParser ) &&
                 Rule.GetRuleType(ruleName) == RuleType.Lexer)
            {
                ErrorManager.GrammarError( ErrorManager.MSG_LEXER_RULES_NOT_ALLOWED,
                                          this, ruleToken, ruleName );
                return;
            }

            Rule r = new Rule( this, ruleName, composite.ruleIndex, numAlts );
            /*
            JSystem.@out.println("defineRule("+ruleName+",modifier="+modifier+
                               "): index="+r.index+", nalts="+numAlts);
            */
            r.modifier = modifier ?? DefaultRuleModifier;
            nameToRuleMap[ruleName] = r;
            SetRuleAST( ruleName, tree );
            r.SetOptions( options, ruleToken );
            r.argActionAST = argActionAST;
            composite.ruleIndexToRuleList.setSize( composite.ruleIndex + 1 );
            composite.ruleIndexToRuleList[composite.ruleIndex] = r;
            composite.ruleIndex++;
            if ( ruleName.StartsWith( SynpredRulePrefix ) )
            {
                r.isSynPred = true;
            }
        }

        /** Define a new predicate and get back its name for use in building
         *  a semantic predicate reference to the syn pred.
         */
        public virtual string DefineSyntacticPredicate( GrammarAST blockAST,
                                               string currentRuleName )
        {
            if ( nameToSynpredASTMap == null )
            {
                nameToSynpredASTMap = new List<KeyValuePair<string, GrammarAST>>();
            }
            string predName =
                SynpredRulePrefix + ( nameToSynpredASTMap.Count + 1 ) + "_" + name;
            blockAST.SetTreeEnclosingRuleNameDeeply( predName );
            nameToSynpredASTMap.Add(new KeyValuePair<string, GrammarAST>(predName, blockAST));
            return predName;
        }

        public virtual GrammarAST GetSyntacticPredicate( string name )
        {
            if ( nameToSynpredASTMap == null )
            {
                return null;
            }

            KeyValuePair<string, GrammarAST> pred = nameToSynpredASTMap.FirstOrDefault(i => string.Equals(i.Key, name, StringComparison.Ordinal));
            return pred.Value;
        }

        public virtual void SynPredUsedInDFA( DFA dfa, SemanticContext semCtx )
        {
            decisionsWhoseDFAsUsesSynPreds.Add( dfa );
            semCtx.TrackUseOfSyntacticPredicates( this ); // walk ctx looking for preds
        }

        /** Given @scope::name {action} define it for this grammar.  Later,
         *  the code generator will ask for the actions table.  For composite
         *  grammars, make sure header action propogates down to all delegates.
         */
        public virtual void DefineNamedAction( GrammarAST ampersandAST,
                                      string scope,
                                      GrammarAST nameAST,
                                      GrammarAST actionAST )
        {
            if ( scope == null )
            {
                scope = GetDefaultActionScope( type );
            }
            //JSystem.@out.println("@"+scope+"::"+nameAST.getText()+"{"+actionAST.getText()+"}");
            string actionName = nameAST.Text;
            var scopeActions = actions.get( scope );
            if ( scopeActions == null )
            {
                scopeActions = new Dictionary<string, object>();
                actions[scope] = scopeActions;
            }
            GrammarAST a = (GrammarAST)scopeActions.get( actionName );
            if ( a != null )
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_ACTION_REDEFINITION, this,
                    nameAST.Token, nameAST.Text );
            }
            else
            {
                scopeActions[actionName] = actionAST;
            }
            // propogate header (regardless of scope (lexer, parser, ...) ?
            if ( this == composite.RootGrammar && actionName.Equals( "header" ) )
            {
                IList<Grammar> allgrammars = composite.RootGrammar.GetDelegates();
                foreach ( Grammar @delegate in allgrammars )
                {
                    if (target.IsValidActionScope(@delegate.type, scope))
                    {
                        //System.out.println("propogate to "+delegate.name);
                        @delegate.DefineNamedAction(ampersandAST, scope, nameAST, actionAST);
                    }
                }
            }
        }

        public virtual void SetSynPredGateIfNotAlready( StringTemplate gateST )
        {
            string scope = GetDefaultActionScope( type );
            var actionsForGrammarScope = actions.get( scope );
            // if no synpredgate action set by user then set
            if ( actionsForGrammarScope == null || !actionsForGrammarScope.ContainsKey( Grammar.SynpredGateActionName ) )
            {
                if ( actionsForGrammarScope == null )
                {
                    actionsForGrammarScope = new Dictionary<string, object>();
                    actions[scope] = actionsForGrammarScope;
                }
                actionsForGrammarScope[Grammar.SynpredGateActionName] = gateST;
            }
        }

        /** Given a grammar type, what should be the default action scope?
         *  If I say @members in a COMBINED grammar, for example, the
         *  default scope should be "parser".
         */
        public virtual string GetDefaultActionScope( GrammarType grammarType )
        {
            switch ( grammarType )
            {
            case GrammarType.Lexer:
                return "lexer";
            case GrammarType.Parser:
            case GrammarType.Combined:
                return "parser";
            case GrammarType.TreeParser:
                return "treeparser";
            }
            return null;
        }

        public virtual void DefineLexerRuleFoundInParser( IToken ruleToken,
                                                 GrammarAST ruleAST )
        {
            //JSystem.@out.println("rule tree is:\n"+ruleAST.toStringTree());
            /*
            String ruleText = tokenBuffer.toOriginalString(ruleAST.ruleStartTokenIndex,
                                                   ruleAST.ruleStopTokenIndex);
            */
            // first, create the text of the rule
            StringBuilder buf = new StringBuilder();
            buf.Append( "// $ANTLR src \"" );
            buf.Append( FileName );
            buf.Append( "\" " );
            buf.Append( ruleAST.Line );
            buf.AppendLine();
            for ( int i = ruleAST.TokenStartIndex;
                 i <= ruleAST.TokenStopIndex && i < tokenBuffer.Count;
                 i++ )
            {
                CommonToken t = (CommonToken)tokenBuffer.Get( i );
                // undo the text deletions done by the lexer (ugh)
                if ( t.Type == ANTLRParser.BLOCK )
                {
                    buf.Append( "(" );
                }
                else if ( t.Type == ANTLRParser.ACTION )
                {
                    buf.Append( "{" );
                    buf.Append( t.Text );
                    buf.Append( "}" );
                }
                else if ( t.Type == ANTLRParser.SEMPRED ||
                          t.Type == ANTLRParser.SYN_SEMPRED ||
                          t.Type == ANTLRParser.GATED_SEMPRED ||
                          t.Type == ANTLRParser.BACKTRACK_SEMPRED )
                {
                    buf.Append( "{" );
                    buf.Append( t.Text );
                    buf.Append( "}?" );
                }
                else if ( t.Type == ANTLRParser.ARG_ACTION )
                {
                    buf.Append( "[" );
                    buf.Append( t.Text );
                    buf.Append( "]" );
                }
                else
                {
                    buf.Append( t.Text );
                }
            }
            string ruleText = buf.ToString();
            //JSystem.@out.println("[["+ruleText+"]]");
            // now put the rule into the lexer grammar template
            if ( IsRoot )
            { // don't build lexers for delegates
                LexerGrammarST.SetAttribute( "rules", ruleText );
            }
            // track this lexer rule's name
            composite.lexerRules.Add( ruleToken.Text );
        }

        /** If someone does PLUS='+' in the parser, must make sure we get
         *  "PLUS : '+' ;" in lexer not "T73 : '+';"
         */
        public virtual void DefineLexerRuleForAliasedStringLiteral( string tokenID,
                                                           string literal,
                                                           int tokenType )
        {
            if ( IsRoot )
            { // don't build lexers for delegates
                //JSystem.@out.println("defineLexerRuleForAliasedStringLiteral: "+literal+" "+tokenType);
                LexerGrammarST.SetAttribute( "literals.{ruleName,type,literal}",
                                            tokenID,
                                            tokenType,
                                            literal );
            }
            // track this lexer rule's name
            composite.lexerRules.Add( tokenID );
        }

        public virtual void DefineLexerRuleForStringLiteral( string literal, int tokenType )
        {
            //JSystem.@out.println("defineLexerRuleForStringLiteral: "+literal+" "+tokenType);
            // compute new token name like T237 and define it as having tokenType
            string tokenID = ComputeTokenNameFromLiteral( tokenType, literal );
            DefineToken( tokenID, tokenType );
            // tell implicit lexer to define a rule to match the literal
            if ( IsRoot )
            { // don't build lexers for delegates
                LexerGrammarST.SetAttribute( "literals.{ruleName,type,literal}",
                                            tokenID,
                                            tokenType,
                                            literal );
            }
        }

        public virtual Rule GetLocallyDefinedRule( string ruleName )
        {
            Rule r;
            if ( nameToRuleMap.TryGetValue( ruleName ?? string.Empty, out r ) )
                return r;

            return null;
        }

        public virtual Rule GetRule( string ruleName )
        {
            Rule r = composite.GetRule( ruleName );
            /*
            if ( r!=null && r.grammar != this ) {
                JSystem.@out.println(name+".getRule("+ruleName+")="+r);
            }
            */
            return r;
        }

        public virtual Rule GetRule( string scopeName, string ruleName )
        {
            if ( scopeName != null )
            { // scope override
                Grammar scope = composite.GetGrammar( scopeName );
                if ( scope == null )
                {
                    return null;
                }
                return scope.GetLocallyDefinedRule( ruleName );
            }
            return GetRule( ruleName );
        }

        public virtual int GetRuleIndex( string scopeName, string ruleName )
        {
            Rule r = GetRule( scopeName, ruleName );
            if ( r != null )
            {
                return r.index;
            }
            return InvalidRuleIndex;
        }

        public virtual int GetRuleIndex( string ruleName )
        {
            return GetRuleIndex( null, ruleName );
        }

        public virtual string GetRuleName( int ruleIndex )
        {
            Rule r = composite.ruleIndexToRuleList[ruleIndex];
            if ( r != null )
            {
                return r.Name;
            }
            return null;
        }

        /** Should codegen.g gen rule for ruleName?
         * 	If synpred, only gen if used in a DFA.
         *  If regular rule, only gen if not overridden in delegator
         *  Always gen Tokens rule though.
         */
        public virtual bool GenerateMethodForRule( string ruleName )
        {
            if ( ruleName.Equals( ArtificialTokensRuleName ) )
            {
                // always generate Tokens rule to satisfy lexer interface
                // but it may have no alternatives.
                return true;
            }
            if ( overriddenRules.Contains( ruleName ) )
            {
                // don't generate any overridden rules
                return false;
            }
            // generate if non-synpred or synpred used in a DFA
            Rule r = GetLocallyDefinedRule( ruleName );
            return !r.isSynPred ||
                   ( r.isSynPred && synPredNamesUsedInDFA.Contains( ruleName ) );
        }

        public virtual AttributeScope DefineGlobalScope( string name, IToken scopeAction )
        {
            AttributeScope scope = new AttributeScope( this, name, scopeAction );
            scopes[name] = scope;
            return scope;
        }

        public virtual AttributeScope CreateReturnScope( string ruleName, IToken retAction )
        {
            AttributeScope scope = new AttributeScope( this, ruleName, retAction );
            scope.isReturnScope = true;
            return scope;
        }

        public virtual AttributeScope CreateRuleScope( string ruleName, IToken scopeAction )
        {
            AttributeScope scope = new AttributeScope( this, ruleName, scopeAction );
            scope.isDynamicRuleScope = true;
            return scope;
        }

        public virtual AttributeScope CreateParameterScope( string ruleName, IToken argAction )
        {
            AttributeScope scope = new AttributeScope( this, ruleName, argAction );
            scope.isParameterScope = true;
            return scope;
        }

        /** Get a global scope */
        public virtual AttributeScope GetGlobalScope( string name )
        {
            return (AttributeScope)scopes.get( name );
        }

        /** Define a label defined in a rule r; check the validity then ask the
         *  Rule object to actually define it.
         */
        protected virtual void DefineLabel( Rule r, IToken label, GrammarAST element, LabelType type )
        {
            bool err = nameSpaceChecker.CheckForLabelTypeMismatch( r, label, type );
            if ( err )
            {
                return;
            }
            r.DefineLabel( label, element, type );
        }

        public virtual void DefineTokenRefLabel( string ruleName,
                                        IToken label,
                                        GrammarAST tokenRef )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                if ( type == GrammarType.Lexer &&
                     ( tokenRef.Type == ANTLRParser.CHAR_LITERAL ||
                      tokenRef.Type == ANTLRParser.BLOCK ||
                      tokenRef.Type == ANTLRParser.NOT ||
                      tokenRef.Type == ANTLRParser.CHAR_RANGE ||
                      tokenRef.Type == ANTLRParser.WILDCARD ) )
                {
                    DefineLabel( r, label, tokenRef, LabelType.Char );
                }
                else
                {
                    DefineLabel( r, label, tokenRef, LabelType.Token );
                }
            }
        }

        public virtual void DefineWildcardTreeLabel( string ruleName,
                                               IToken label,
                                               GrammarAST tokenRef )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                DefineLabel( r, label, tokenRef, LabelType.WildcardTree );
            }
        }

        public virtual void DefineWildcardTreeListLabel( string ruleName, IToken label, GrammarAST tokenRef )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
                DefineLabel( r, label, tokenRef, LabelType.WildcardTreeList );
        }

        public virtual void DefineRuleRefLabel( string ruleName,
                                       IToken label,
                                       GrammarAST ruleRef )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                DefineLabel( r, label, ruleRef, LabelType.Rule );
            }
        }

        public virtual void DefineTokenListLabel( string ruleName,
                                         IToken label,
                                         GrammarAST element )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                if (type == GrammarType.Lexer &&
                     (element.Type == ANTLRParser.CHAR_LITERAL ||
                      element.Type == ANTLRParser.BLOCK ||
                      element.Type == ANTLRParser.NOT ||
                      element.Type == ANTLRParser.CHAR_RANGE ||
                      element.Type == ANTLRParser.WILDCARD))
                {
                    DefineLabel(r, label, element, LabelType.CharList);
                }
                else
                {
                    DefineLabel( r, label, element, LabelType.TokenList );
                }
            }
        }

        public virtual void DefineRuleListLabel( string ruleName,
                                        IToken label,
                                        GrammarAST element )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                if ( !r.HasMultipleReturnValues )
                {
                    ErrorManager.GrammarError(
                        ErrorManager.MSG_LIST_LABEL_INVALID_UNLESS_RETVAL_STRUCT, this,
                        label, label.Text );
                }
                DefineLabel( r, label, element, LabelType.RuleList );
            }
        }

        /** Given a set of all rewrite elements on right of ->, filter for
         *  label types such as Grammar.TOKEN_LABEL, Grammar.TOKEN_LIST_LABEL, ...
         *  Return a displayable token type name computed from the GrammarAST.
         */
        public virtual HashSet<string> GetLabels( HashSet<GrammarAST> rewriteElements, LabelType labelType )
        {
            HashSet<string> labels = new HashSet<string>();
            foreach ( GrammarAST el in rewriteElements )
            {
                if ( el.Type == ANTLRParser.LABEL )
                {
                    string labelName = el.Text;
                    Rule enclosingRule = GetLocallyDefinedRule( el.enclosingRuleName );
                    if (enclosingRule == null)
                        continue;

                    LabelElementPair pair = enclosingRule.GetLabel( labelName );
                    /*
                    // if tree grammar and we have a wildcard, only notice it
                    // when looking for rule labels not token label. x=. should
                    // look like a rule ref since could be subtree.
                    if ( type==TREE_PARSER && pair!=null &&
                         pair.elementRef.getType()==ANTLRParser.WILDCARD )
                    {
                        if ( labelType==WILDCARD_TREE_LABEL ) {
                            labels.add(labelName);
                            continue;
                        }
                        else continue;
                    }
                     */
                    // if valid label and type is what we're looking for
                    // and not ref to old value val $rule, add to list
                    if ( pair != null && pair.type == labelType &&
                         !labelName.Equals( el.enclosingRuleName ) )
                    {
                        labels.Add( labelName );
                    }
                }
            }
            return labels;
        }

        /** Before generating code, we examine all actions that can have
         *  $x.y and $y stuff in them because some code generation depends on
         *  Rule.referencedPredefinedRuleAttributes.  I need to remove unused
         *  rule labels for example.
         */
        protected virtual void ExamineAllExecutableActions()
        {
            foreach ( Rule r in Rules )
            {
                // walk all actions within the rule elements, args, and exceptions
                var actions = r.InlineActions;
                foreach ( GrammarAST actionAST in actions )
                {
                    ActionAnalysisLexer sniffer = new ActionAnalysisLexer( this, r.Name, actionAST );
                    sniffer.Analyze();
                }
                // walk any named actions like @init, @after
                IEnumerable<GrammarAST> namedActions = r.Actions.Values.Cast<GrammarAST>();
                foreach ( GrammarAST actionAST in namedActions )
                {
                    ActionAnalysisLexer sniffer = new ActionAnalysisLexer( this, r.Name, actionAST );
                    sniffer.Analyze();
                }
            }
        }

        /** Remove all labels on rule refs whose target rules have no return value.
         *  Do this for all rules in grammar.
         */
        public virtual void CheckAllRulesForUselessLabels()
        {
            if ( type == GrammarType.Lexer )
                return;

            foreach ( string ruleName in nameToRuleMap.Keys )
            {
                Rule r = GetRule( ruleName );
                RemoveUselessLabels( r.RuleLabels );
                RemoveUselessLabels( r.RuleListLabels );
            }
        }

        /** A label on a rule is useless if the rule has no return value, no
         *  tree or template output, and it is not referenced in an action.
         */
        protected virtual void RemoveUselessLabels( IDictionary<string, LabelElementPair> ruleToElementLabelPairMap )
        {
            if ( ruleToElementLabelPairMap == null )
                return;

            var tokill = from pair in ruleToElementLabelPairMap.Values
                         let rule = GetRule( pair.elementRef.Text )
                         where rule != null && !rule.HasReturnValue && !pair.actionReferencesLabel
                         select pair.label.Text;

            foreach ( string label in tokill.ToArray() )
                ruleToElementLabelPairMap.Remove( label );
        }

        /** Track a rule reference within an outermost alt of a rule.  Used
         *  at the moment to decide if $ruleref refers to a unique rule ref in
         *  the alt.  Rewrite rules force tracking of all rule AST results.
         *
         *  This data is also used to verify that all rules have been defined.
         */
        public virtual void AltReferencesRule( string enclosingRuleName,
                                      GrammarAST refScopeAST,
                                      GrammarAST refAST,
                                      int outerAltNum )
        {
            /* Do nothing for now; not sure need; track S.x as x
            String scope = null;
            Grammar scopeG = null;
            if ( refScopeAST!=null ) {
                if ( !scopedRuleRefs.contains(refScopeAST) ) {
                    scopedRuleRefs.add(refScopeAST);
                }
                scope = refScopeAST.getText();
            }
            */
            Rule r = GetRule( enclosingRuleName );
            if ( r == null )
            {
                return; // no error here; see NameSpaceChecker
            }
            r.TrackRuleReferenceInAlt( refAST, outerAltNum );
            IToken refToken = refAST.Token;
            if ( !ruleRefs.Contains( refAST ) )
            {
                ruleRefs.Add( refAST );
            }
        }

        /** Track a token reference within an outermost alt of a rule.  Used
         *  to decide if $tokenref refers to a unique token ref in
         *  the alt. Does not track literals!
         *
         *  Rewrite rules force tracking of all tokens.
         */
        public virtual void AltReferencesTokenID( string ruleName, GrammarAST refAST, int outerAltNum )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r == null )
            {
                return;
            }
            r.TrackTokenReferenceInAlt( refAST, outerAltNum );
            if ( !tokenIDRefs.Contains( refAST.Token ) )
            {
                tokenIDRefs.Add( refAST.Token );
            }
        }

        /** To yield smaller, more readable code, track which rules have their
         *  predefined attributes accessed.  If the rule has no user-defined
         *  return values, then don't generate the return value scope classes
         *  etc...  Make the rule have void return value.  Don't track for lexer
         *  rules.
         */
        public virtual void ReferenceRuleLabelPredefinedAttribute( string ruleName )
        {
            Rule r = GetRule( ruleName );
            if ( r != null && type != GrammarType.Lexer )
            {
                // indicate that an action ref'd an attr unless it's in a lexer
                // so that $ID.text refs don't force lexer rules to define
                // return values...Token objects are created by the caller instead.
                r.referencedPredefinedRuleAttributes = true;
            }
        }

        public virtual IList<HashSet<Rule>> CheckAllRulesForLeftRecursion()
        {
            return sanity.CheckAllRulesForLeftRecursion();
        }

        /** Return a list of left-recursive rules; no analysis can be done
         *  successfully on these.  Useful to skip these rules then and also
         *  for ANTLRWorks to highlight them.
         */
        public virtual HashSet<Rule> GetLeftRecursiveRules()
        {
            if ( nfa == null )
            {
                BuildNFA();
            }
            if ( leftRecursiveRules != null )
            {
                return leftRecursiveRules;
            }
            sanity.CheckAllRulesForLeftRecursion();
            return leftRecursiveRules;
        }

        public virtual void CheckRuleReference( GrammarAST scopeAST,
                                       GrammarAST refAST,
                                       GrammarAST argsAST,
                                       string currentRuleName )
        {
            sanity.CheckRuleReference( scopeAST, refAST, argsAST, currentRuleName );
        }

        /** Rules like "a : ;" and "a : {...} ;" should not generate
         *  try/catch blocks for RecognitionException.  To detect this
         *  it's probably ok to just look for any reference to an atom
         *  that can match some input.  W/o that, the rule is unlikey to have
         *  any else.
         */
        public virtual bool IsEmptyRule( GrammarAST block )
        {
            foreach ( ITree node in GrammarAST.Descendants(block) )
            {
                switch ( node.Type )
                {
                case ANTLRParser.TOKEN_REF:
                case ANTLRParser.STRING_LITERAL:
                case ANTLRParser.CHAR_LITERAL:
                case ANTLRParser.WILDCARD:
                case ANTLRParser.RULE_REF:
                    return false;

                default:
                    continue;
                }
            }

            return false;
        }

        public virtual bool IsAtomTokenType( int ttype )
        {
            return ttype == ANTLRParser.WILDCARD ||
                   ttype == ANTLRParser.CHAR_LITERAL ||
                   ttype == ANTLRParser.CHAR_RANGE ||
                   ttype == ANTLRParser.STRING_LITERAL ||
                   ttype == ANTLRParser.NOT ||
                   ( type != GrammarType.Lexer && ttype == ANTLRParser.TOKEN_REF );
        }

        public virtual int GetTokenType( string tokenName )
        {
            int i;
            if ( tokenName[0] == '\'' )
            {
                if ( composite.stringLiteralToTypeMap.TryGetValue( tokenName, out i ) )
                    return i;
            }
            else
            {
                // must be a label like ID
                if ( composite.tokenIDToTypeMap.TryGetValue( tokenName, out i ) )
                    return i;
            }

            return Label.INVALID;
        }

        /** Return an ordered integer list of token types that have no
         *  corresponding token ID like INT or KEYWORD_BEGIN; for stuff
         *  like 'begin'.
         */
        public virtual ICollection<int> GetTokenTypesWithoutID()
        {
            IList<int> types = new List<int>();
            for ( int t = Label.MIN_TOKEN_TYPE; t <= MaxTokenType; t++ )
            {
                string name = GetTokenDisplayName( t );
                if ( name[0] == '\'' )
                {
                    types.Add( t );
                }
            }
            return types;
        }

        /** Get a list of all token IDs and literals that have an associated
         *  token type.
         */
        public virtual HashSet<string> GetTokenDisplayNames()
        {
            HashSet<string> names = new HashSet<string>();
            for ( int t = Label.MIN_TOKEN_TYPE; t <= MaxTokenType; t++ )
            {
                names.Add( GetTokenDisplayName( t ) );
            }
            return names;
        }

        /** Given a literal like (the 3 char sequence with single quotes) 'a',
         *  return the int value of 'a'. Convert escape sequences here also.
         *  ANTLR's antlr.g parser does not convert escape sequences.
         *
         *  11/26/2005: I changed literals to always be '...' even for strings.
         *  This routine still works though.
         */
        public static int GetCharValueFromGrammarCharLiteral( string literal )
        {
            switch ( literal.Length )
            {
            case 3:
                // 'x'
                return literal[1]; // no escape char
            case 4:
                // '\x'  (antlr lexer will catch invalid char)
                if ( char.IsDigit( literal[2] ) )
                {
                    ErrorManager.Error( ErrorManager.MSG_SYNTAX_ERROR,
                                       "invalid char literal: " + literal );
                    return -1;
                }
                int escChar = literal[2];
                int charVal = AntlrLiteralEscapedCharValue[escChar];
                if ( charVal == 0 )
                {
                    // Unnecessary escapes like '\{' should just yield {
                    return escChar;
                }
                return charVal;
            case 8:
                // '\u1234'
                string unicodeChars = literal.Substring( 3, literal.Length - 1 - 3 );
                //return Integer.parseInt( unicodeChars, 16 );
                return int.Parse( unicodeChars, System.Globalization.NumberStyles.AllowHexSpecifier );
            default:
                ErrorManager.Error( ErrorManager.MSG_SYNTAX_ERROR,
                                   "invalid char literal: " + literal );
                return -1;
            }
        }

        /** ANTLR does not convert escape sequences during the parse phase because
         *  it could not know how to print String/char literals back out when
         *  printing grammars etc...  Someone in China might use the real unicode
         *  char in a literal as it will display on their screen; when printing
         *  back out, I could not know whether to display or use a unicode escape.
         *
         *  This routine converts a string literal with possible escape sequences
         *  into a pure string of 16-bit char values.  Escapes and unicode \u0000
         *  specs are converted to pure chars.  return in a buffer; people may
         *  want to walk/manipulate further.
         *
         *  The NFA construction routine must know the actual char values.
         */
        public static StringBuilder GetUnescapedStringFromGrammarStringLiteral( string literal )
        {
            //JSystem.@out.println("escape: ["+literal+"]");
            StringBuilder buf = new StringBuilder();
            int last = literal.Length - 1; // skip quotes on outside
            for ( int i = 1; i < last; i++ )
            {
                char c = literal[i];
                if ( c == '\\' )
                {
                    i++;
                    c = literal[i];
                    if ( char.ToUpperInvariant( c ) == 'U' )
                    {
                        // \u0000
                        i++;
                        string unicodeChars = literal.Substring( i, 4 );
                        // parse the unicode 16 bit hex value
                        //int val = Integer.parseInt( unicodeChars, 16 );
                        int val = int.Parse( unicodeChars, System.Globalization.NumberStyles.AllowHexSpecifier );
                        i += 4 - 1; // loop will inc by 1; only jump 3 then
                        buf.Append( (char)val );
                    }
                    else if ( char.IsDigit( c ) )
                    {
                        ErrorManager.Error( ErrorManager.MSG_SYNTAX_ERROR,
                                           "invalid char literal: " + literal );
                        buf.Append( "\\" + (char)c );
                    }
                    else
                    {
                        buf.Append( (char)AntlrLiteralEscapedCharValue[c] ); // normal \x escape
                    }
                }
                else
                {
                    buf.Append( c ); // simple char x
                }
            }
            //JSystem.@out.println("string: ["+buf.toString()+"]");
            return buf;
        }

        /** Pull your token definitions from an existing grammar in memory.
         *  You must use Grammar() ctor then this method then setGrammarContent()
         *  to make this work.  This was useful primarily for testing and
         *  interpreting grammars until I added import grammar functionality.
         *  When you import a grammar you implicitly import its vocabulary as well
         *  and keep the same token type values.
         *
         *  Returns the max token type found.
         */
        public virtual int ImportTokenVocabulary( Grammar importFromGr )
        {
            var importedTokenIDs = importFromGr.TokenIDs;
            foreach ( string tokenID in importedTokenIDs )
            {
                int tokenType = importFromGr.GetTokenType( tokenID );
                composite.maxTokenType = Math.Max( composite.maxTokenType, tokenType );
                if ( tokenType >= Label.MIN_TOKEN_TYPE )
                {
                    //JSystem.@out.println("import token from grammar "+tokenID+"="+tokenType);
                    DefineToken( tokenID, tokenType );
                }
            }
            return composite.maxTokenType; // return max found
        }

        /** Import the rules/tokens of a delegate grammar. All delegate grammars are
         *  read during the ctor of first Grammar created.
         *
         *  Do not create NFA here because NFA construction needs to hook up with
         *  overridden rules in delegation root grammar.
         */
        public virtual void ImportGrammar( GrammarAST grammarNameAST, string label )
        {
            string grammarName = grammarNameAST.Text;
            //JSystem.@out.println("import "+gfile.getName());
            string gname = grammarName + GrammarFileExtension;
            TextReader br = null;
            try
            {
                string fullName = tool.GetLibraryFile( gname );
                //FileReader fr = new FileReader( fullName );
                //br = new BufferedReader( fr );
                br = new StringReader( System.IO.File.ReadAllText( fullName ) );
                Grammar delegateGrammar = null;
                delegateGrammar = new Grammar( tool, gname, composite );
                delegateGrammar.label = label;

                AddDelegateGrammar( delegateGrammar );

                delegateGrammar.ParseAndBuildAST( br );
                delegateGrammar.AddRulesForSyntacticPredicates();
                if ( !ValidImport( delegateGrammar ) )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_INVALID_IMPORT,
                                              this,
                                              grammarNameAST.Token,
                                              this,
                                              delegateGrammar );
                    return;
                }
                if ( this.type == GrammarType.Combined &&
                     ( delegateGrammar.name.Equals( this.name + grammarTypeToFileNameSuffix[(int)GrammarType.Lexer] ) ||
                      delegateGrammar.name.Equals( this.name + grammarTypeToFileNameSuffix[(int)GrammarType.Parser] ) ) )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_IMPORT_NAME_CLASH,
                                              this,
                                              grammarNameAST.Token,
                                              this,
                                              delegateGrammar );
                    return;
                }
                if ( delegateGrammar.grammarTree != null )
                {
                    // we have a valid grammar
                    // deal with combined grammars
                    if ( delegateGrammar.type == GrammarType.Lexer && this.type == GrammarType.Combined )
                    {
                        // ooops, we wasted some effort; tell lexer to read it in
                        // later
                        LexerGrammarST.SetAttribute( "imports", grammarName );
                        // but, this parser grammar will need the vocab
                        // so add to composite anyway so we suck in the tokens later
                    }
                }
                //JSystem.@out.println("Got grammar:\n"+delegateGrammar);
            }
            catch ( IOException ioe )
            {
                ErrorManager.Error( ErrorManager.MSG_CANNOT_OPEN_FILE,
                                   gname,
                                   ioe );
            }
            finally
            {
                if ( br != null )
                {
                    try
                    {
                        br.Close();
                    }
                    catch ( IOException ioe )
                    {
                        ErrorManager.Error( ErrorManager.MSG_CANNOT_CLOSE_FILE,
                                           gname,
                                           ioe );
                    }
                }
            }
        }

        /** add new delegate to composite tree */
        protected virtual void AddDelegateGrammar( Grammar delegateGrammar )
        {
            CompositeGrammarTree t = composite.delegateGrammarTreeRoot.FindNode( this );
            t.AddChild( new CompositeGrammarTree( delegateGrammar ) );
            // make sure new grammar shares this composite
            delegateGrammar.composite = this.composite;
        }

        /** Load a vocab file <vocabName>.tokens and return max token type found. */
        public virtual int ImportTokenVocabulary( GrammarAST tokenVocabOptionAST,
                                         string vocabName )
        {
            if ( !IsRoot )
            {
                ErrorManager.GrammarWarning( ErrorManager.MSG_TOKEN_VOCAB_IN_DELEGATE,
                                            this,
                                            tokenVocabOptionAST.Token,
                                            name );
                return composite.maxTokenType;
            }

            Regex vocabLine = new Regex( @"^(?<tokenID>'(?:\\'|.)+'|\w+)\s*=\s*(?<tokenType>\d+)$", RegexOptions.Compiled );
            string fileName = Path.GetFullPath( tool.GetImportedVocabFile( vocabName ) );
            if ( File.Exists( fileName ) )
            {
                try
                {
                    string[] lines = File.ReadAllLines( fileName );
                    for ( int lineNum = 0; lineNum < lines.Length; lineNum++ )
                    {
                        string line = lines[lineNum].Trim();
                        int commentStart = line.IndexOf( "//" );
                        if ( commentStart >= 0 )
                            line = line.Substring( 0, commentStart );

                        var match = vocabLine.Match( line );
                        if ( !match.Success )
                        {
                            ErrorManager.Error( ErrorManager.MSG_TOKENS_FILE_SYNTAX_ERROR,
                                               vocabName + CodeGenerator.VocabFileExtension,
                                               lineNum );
                            continue;
                        }

                        DefineToken( match.Groups["tokenID"].Value, int.Parse( match.Groups["tokenType"].Value ) );
                    }
                }
                catch ( IOException ioe )
                {
                    ErrorManager.Error( ErrorManager.MSG_ERROR_READING_TOKENS_FILE, fileName, ioe );
                }
                catch ( Exception e )
                {
                    ErrorManager.Error( ErrorManager.MSG_ERROR_READING_TOKENS_FILE, fileName, e );
                }
            }
            else
            {
                ErrorManager.Error( ErrorManager.MSG_CANNOT_FIND_TOKENS_FILE, fileName );
            }

            return composite.maxTokenType;
        }

        /** Given a token type, get a meaningful name for it such as the ID
         *  or string literal.  If this is a lexer and the ttype is in the
         *  char vocabulary, compute an ANTLR-valid (possibly escaped) char literal.
         */
        public virtual string GetTokenDisplayName( int ttype )
        {
            string tokenName = null;
            int index = 0;
            // inside any target's char range and is lexer grammar?
            if ( this.type == GrammarType.Lexer &&
                 ttype >= Label.MIN_CHAR_VALUE && ttype <= Label.MAX_CHAR_VALUE )
            {
                return GetANTLRCharLiteralForChar( ttype );
            }
            // faux label?
            else if ( ttype < 0 )
            {
                tokenName = (string)composite.typeToTokenList[Label.NUM_FAUX_LABELS + ttype];
            }
            else
            {
                // compute index in typeToTokenList for ttype
                index = ttype - 1; // normalize to 0..n-1
                index += Label.NUM_FAUX_LABELS;     // jump over faux tokens

                if ( index < composite.typeToTokenList.Count )
                {
                    tokenName = (string)composite.typeToTokenList[index];
                    if ( tokenName != null &&
                         tokenName.StartsWith( AUTO_GENERATED_TOKEN_NAME_PREFIX ) )
                    {
                        tokenName = composite.typeToStringLiteralList[ttype];
                    }
                }
                else
                {
                    tokenName = ttype.ToString(); // String.valueOf( ttype );
                }
            }
            //JSystem.@out.println("getTokenDisplayName ttype="+ttype+", index="+index+", name="+tokenName);
            return tokenName;
        }

        public virtual int GetGrammarMaxLookahead()
        {
            if ( global_k >= 0 )
            {
                return global_k;
            }
            object k = GetOption( "k" );
            if ( k == null )
            {
                global_k = 0;
            }
            else if ( k is int )
            {
                int kI = (int)k;
                global_k = kI;
            }
            else
            {
                // must be String "*"
                if ( k.Equals( "*" ) )
                {  // this the default anyway
                    global_k = 0;
                }
            }
            return global_k;
        }

        /** Save the option key/value pair and process it; return the key
         *  or null if invalid option.
         */
        public virtual string SetOption( string key, object value, IToken optionsStartToken )
        {
            if ( LegalOption( key ) )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_ILLEGAL_OPTION,
                                          this,
                                          optionsStartToken,
                                          key );
                return null;
            }
            if ( !OptionIsValid( key, value ) )
            {
                return null;
            }
            if ( key == "backtrack" && value.ToString() == "true" )
            {
                composite.GetRootGrammar().atLeastOneBacktrackOption = true;
            }
            if ( options == null )
            {
                options = new Dictionary<string, object>();
            }
            options[key] = value;
            return key;
        }

        public virtual bool LegalOption( string key )
        {
            switch ( type )
            {
            case GrammarType.Lexer:
                return !legalLexerOptions.Contains( key );
            case GrammarType.Parser:
                return !legalParserOptions.Contains( key );
            case GrammarType.TreeParser:
                return !legalTreeParserOptions.Contains( key );
            default:
                return !legalParserOptions.Contains( key );
            }
        }

        public virtual void SetOptions( IDictionary<string, object> options, IToken optionsStartToken )
        {
            if ( options == null )
            {
                this.options = null;
                return;
            }
            foreach ( var option in options.ToArray() )
            {
                string optionName = option.Key;
                object optionValue = option.Value;
                string stored = SetOption( optionName, optionValue, optionsStartToken );
                if ( stored == null )
                    options.Remove( optionName );
            }
        }

        public virtual object GetOption( string key )
        {
            return composite.GetOption( key );
        }

        public virtual object GetLocallyDefinedOption( string key )
        {
            object value = null;
            if ( options != null )
            {
                value = options.get( key );
            }
            if ( value == null )
            {
                value = defaultOptions.get( key );
            }
            return value;
        }

        public virtual object GetBlockOption( GrammarAST blockAST, string key )
        {
            string v = (string)blockAST.GetBlockOption( key );
            if ( v != null )
            {
                return v;
            }
            if ( type == GrammarType.Lexer )
            {
                return defaultLexerBlockOptions.get( key );
            }
            return defaultBlockOptions.get( key );
        }

        public virtual int GetUserMaxLookahead( int decision )
        {
            int user_k = 0;
            GrammarAST blockAST = nfa.grammar.GetDecisionBlockAST( decision );
            object k = blockAST.GetBlockOption( "k" );
            if ( k == null )
            {
                user_k = nfa.grammar.MaxLookahead;
                return user_k;
            }
            if ( k is int )
            {
                int kI = (int)k;
                user_k = kI;
            }
            else
            {
                // must be String "*"
                if ( k.Equals( "*" ) )
                {
                    user_k = 0;
                }
            }
            return user_k;
        }

        public virtual bool GetAutoBacktrackMode( int decision )
        {
            NFAState decisionNFAStartState = GetDecisionNFAStartState( decision );
            string autoBacktrack =
                (string)GetBlockOption( decisionNFAStartState.associatedASTNode, "backtrack" );

            if ( autoBacktrack == null )
            {
                autoBacktrack = (string)nfa.grammar.GetOption( "backtrack" );
            }
            return autoBacktrack != null && autoBacktrack.Equals( "true" );
        }

        public virtual bool OptionIsValid( string key, object value )
        {
            return true;
        }

        /** Get the set of Rules that need to have manual delegations
         *  like "void rule() { importedGrammar.rule(); }"
         *
         *  If this grammar is master, get list of all rule definitions from all
         *  delegate grammars.  Only master has complete interface from combined
         *  grammars...we will generated delegates as helper objects.
         *
         *  Composite grammars that are not the root/master do not have complete
         *  interfaces.  It is not my intention that people use subcomposites.
         *  Only the outermost grammar should be used from outside code.  The
         *  other grammar components are specifically generated to work only
         *  with the master/root. 
         *
         *  delegatedRules = imported - overridden
         */
        public virtual HashSet<Rule> GetDelegatedRules()
        {
            return composite.GetDelegatedRules( this );
        }

        /** Get set of all rules imported from all delegate grammars even if
         *  indirectly delegated.
         */
        public virtual HashSet<Rule> GetAllImportedRules()
        {
            return composite.GetAllImportedRules( this );
        }

        /** Get list of all delegates from all grammars directly or indirectly
         *  imported into this grammar.
         */
        public virtual IList<Grammar> GetDelegates()
        {
            return composite.GetDelegates( this );
        }

        public virtual IList<string> GetDelegateNames()
        {
            // compute delegates:{Grammar g | return g.name;}
            IList<string> names = new List<string>();
            IList<Grammar> delegates = composite.GetDelegates( this );
            if ( delegates != null )
            {
                foreach ( Grammar g in delegates )
                {
                    names.Add( g.name );
                }
            }
            return names;
        }

        public virtual IList<Grammar> GetDirectDelegates()
        {
            return composite.GetDirectDelegates( this );
        }

        /** Get delegates below direct delegates */
        public virtual IList<Grammar> GetIndirectDelegates()
        {
            return composite.GetIndirectDelegates( this );
        }

        /** Get list of all delegators.  This amounts to the grammars on the path
         *  to the root of the delegation tree.
         */
        public virtual IList<Grammar> GetDelegators()
        {
            return composite.GetDelegators( this );
        }

        public virtual HashSet<Rule> GetDelegatedRuleReferences()
        {
            return delegatedRuleReferences;
        }

        public virtual void SetRuleAST( string ruleName, GrammarAST t )
        {
            Rule r = GetLocallyDefinedRule( ruleName );
            if ( r != null )
            {
                r.tree = t;
                r.EORNode = t.LastChild;
            }
        }

        public virtual NFAState GetRuleStartState( string ruleName )
        {
            return GetRuleStartState( null, ruleName );
        }

        public virtual NFAState GetRuleStartState( string scopeName, string ruleName )
        {
            Rule r = GetRule( scopeName, ruleName );
            if ( r != null )
            {
                //JSystem.@out.println("getRuleStartState("+scopeName+", "+ruleName+")="+r.startState);
                return r.startState;
            }
            //JSystem.@out.println("getRuleStartState("+scopeName+", "+ruleName+")=null");
            return null;
        }

        public virtual string GetRuleModifier( string ruleName )
        {
            Rule r = GetRule( ruleName );
            if ( r != null )
            {
                return r.modifier;
            }
            return null;
        }

        public virtual NFAState GetRuleStopState( string ruleName )
        {
            Rule r = GetRule( ruleName );
            if ( r != null )
            {
                return r.stopState;
            }
            return null;
        }

        public virtual int AssignDecisionNumber( NFAState state )
        {
            decisionCount++;
            state.DecisionNumber = decisionCount;
            return decisionCount;
        }

        protected internal virtual Decision GetDecision( int decision )
        {
            int index = decision - 1;
            if ( index >= indexToDecision.Count )
            {
                return null;
            }
            Decision d = indexToDecision[index];
            return d;
        }

        protected virtual Decision CreateDecision( int decision )
        {
            int index = decision - 1;
            if ( index < indexToDecision.Count )
            {
                return GetDecision( decision ); // don't recreate
            }
            Decision d = new Decision();
            d.decision = decision;
            d.grammar = this;
            indexToDecision.setSize( NumberOfDecisions );
            indexToDecision[index] = d;
            return d;
        }

        public virtual IList<NFAState> GetDecisionNFAStartStateList()
        {
            var states = new List<NFAState>( 100 );
            for ( int d = 0; d < indexToDecision.Count; d++ )
            {
                Decision dec = (Decision)indexToDecision[d];
                states.Add( dec.startState );
            }
            return states;
        }

        public virtual NFAState GetDecisionNFAStartState( int decision )
        {
            Decision d = GetDecision( decision );
            if ( d == null )
            {
                return null;
            }
            return d.startState;
        }

        public virtual DFA GetLookaheadDFA( int decision )
        {
            Decision d = GetDecision( decision );
            if ( d == null )
            {
                return null;
            }
            return d.dfa;
        }

        public virtual GrammarAST GetDecisionBlockAST( int decision )
        {
            Decision d = GetDecision( decision );
            if ( d == null )
            {
                return null;
            }
            return d.blockAST;
        }

        /** returns a list of column numbers for all decisions
         *  on a particular line so ANTLRWorks choose the decision
         *  depending on the location of the cursor (otherwise,
         *  ANTLRWorks has to give the *exact* location which
         *  is not easy from the user point of view).
         *
         *  This is not particularly fast as it walks entire line:col->DFA map
         *  looking for a prefix of "line:".
         */
        public virtual IList<int> GetLookaheadDFAColumnsForLineInFile( int line )
        {
            string prefix = line + ":";
            var columns = new List<int>();
            foreach ( string key in lineColumnToLookaheadDFAMap.Keys )
            {
                if ( key.StartsWith( prefix ) )
                {
                    columns.Add( int.Parse( key.Substring( prefix.Length ) ) );
                }
            }
            return columns;
        }

        /** Useful for ANTLRWorks to map position in file to the DFA for display */
        public virtual DFA GetLookaheadDFAFromPositionInFile( int line, int col )
        {
            return (DFA)lineColumnToLookaheadDFAMap.get( line.ToString() + ":" + col.ToString() );
        }

#if false
        public virtual void SetDecisionOptions( int decision, IDictionary options )
        {
            Decision d = createDecision( decision );
            d.options = options;
        }

        public virtual void SetDecisionOption( int decision, string name, object value )
        {
            Decision d = getDecision( decision );
            if ( d != null )
            {
                if ( d.options == null )
                {
                    d.options = new Dictionary<object, object>();
                }
                d.options.put( name, value );
            }
        }

        public virtual IDictionary GetDecisionOptions( int decision )
        {
            Decision d = getDecision( decision );
            if ( d == null )
            {
                return null;
            }
            return d.options;
        }
#endif

        public virtual int GetNumberOfCyclicDecisions()
        {
            int n = 0;
            for ( int i = 1; i <= NumberOfDecisions; i++ )
            {
                Decision d = GetDecision( i );
                if ( d.dfa != null && d.dfa.IsCyclic )
                {
                    n++;
                }
            }
            return n;
        }

        /** Set the lookahead DFA for a particular decision.  This means
         *  that the appropriate AST node must updated to have the new lookahead
         *  DFA.  This method could be used to properly set the DFAs without
         *  using the createLookaheadDFAs() method.  You could do this
         *
         *    Grammar g = new Grammar("...");
         *    g.setLookahead(1, dfa1);
         *    g.setLookahead(2, dfa2);
         *    ...
         */
        public virtual void SetLookaheadDFA( int decision, DFA lookaheadDFA )
        {
            Decision d = CreateDecision( decision );
            d.dfa = lookaheadDFA;
            GrammarAST ast = d.startState.associatedASTNode;
            ast.LookaheadDFA = lookaheadDFA;
        }

        public virtual void SetDecisionNFA( int decision, NFAState state )
        {
            Decision d = CreateDecision( decision );
            d.startState = state;
        }

        public virtual void SetDecisionBlockAST( int decision, GrammarAST blockAST )
        {
            //JSystem.@out.println("setDecisionBlockAST("+decision+", "+blockAST.token);
            Decision d = CreateDecision( decision );
            d.blockAST = blockAST;
        }

        /** Return a string representing the escaped char for code c.  E.g., If c
         *  has value 0x100, you will get "\u0100".  ASCII gets the usual
         *  char (non-hex) representation.  Control characters are spit out
         *  as unicode.  While this is specially set up for returning Java strings,
         *  it can be used by any language target that has the same syntax. :)
         *
         *  11/26/2005: I changed this to use double quotes, consistent with antlr.g
         *  12/09/2005: I changed so everything is single quotes
         */
        public static string GetANTLRCharLiteralForChar( int c )
        {
            if ( c < Label.MIN_CHAR_VALUE )
            {
                ErrorManager.InternalError( "invalid char value " + c );
                return "'<INVALID>'";
            }
            if ( c < AntlrLiteralCharValueEscape.Length && AntlrLiteralCharValueEscape[c] != null )
            {
                return '\'' + AntlrLiteralCharValueEscape[c] + '\'';
            }
            if ( c <= 0x7f && !char.IsControl( (char)c ) )
            {
                if ( c == '\\' )
                {
                    return "'\\\\'";
                }
                if ( c == '\'' )
                {
                    return "'\\''";
                }
                return "'" + (char)c + "'";
            }
            // turn on the bit above max "\uFFFF" value so that we pad with zeros
            // then only take last 4 digits
            string hex = c.ToString( "X4" ); //Integer.toHexString( c | 0x10000 ).toUpperCase().substring( 1, 5 );
            string unicodeStr = "'\\u" + hex + "'";
            return unicodeStr;
        }

        /** For lexer grammars, return everything in unicode not in set.
         *  For parser and tree grammars, return everything in token space
         *  from MIN_TOKEN_TYPE to last valid token type or char value.
         */
        public virtual IIntSet Complement( IIntSet set )
        {
            //JSystem.@out.println("complement "+set.toString(this));
            //JSystem.@out.println("vocabulary "+getTokenTypes().toString(this));
            IIntSet c = set.Complement( TokenTypes );
            //JSystem.@out.println("result="+c.toString(this));
            return c;
        }

        public virtual IIntSet Complement( int atom )
        {
            return Complement( IntervalSet.Of( atom ) );
        }

        /** Given set tree like ( SET A B ), check that A and B
         *  are both valid sets themselves, else we must tree like a BLOCK
         */
        [CLSCompliant(false)]
        public virtual bool IsValidSet(TreeToNFAConverter nfabuilder, GrammarAST t)
        {
            bool valid = true;
            try
            {
                //JSystem.@out.println("parse BLOCK as set tree: "+t.toStringTree());
                int alts = nfabuilder.TestBlockAsSet( t );
                valid = ( alts > 1 );
            }
            catch ( RecognitionException /*re*/ )
            {
                // The rule did not parse as a set, return false; ignore exception
                valid = false;
            }
            //JSystem.@out.println("valid? "+valid);
            return valid;
        }

        /** Get the set equivalent (if any) of the indicated rule from this
         *  grammar.  Mostly used in the lexer to do ~T for some fragment rule
         *  T.  If the rule AST has a SET use that.  If the rule is a single char
         *  convert it to a set and return.  If rule is not a simple set (w/o actions)
         *  then return null.
         *  Rules have AST form:
         *
         *		^( RULE ID modifier ARG RET SCOPE block EOR )
         */
        [CLSCompliant(false)]
        public virtual IIntSet GetSetFromRule(TreeToNFAConverter nfabuilder, string ruleName)
        {
            Rule r = GetRule( ruleName );
            if ( r == null )
            {
                return null;
            }
            IIntSet elements = null;
            //JSystem.@out.println("parsed tree: "+r.tree.toStringTree());
            elements = nfabuilder.SetRule( r.tree );
            //JSystem.@out.println("elements="+elements);
            return elements;
        }

        /** Decisions are linked together with transition(1).  Count how
         *  many there are.  This is here rather than in NFAState because
         *  a grammar decides how NFAs are put together to form a decision.
         */
        public virtual int GetNumberOfAltsForDecisionNFA( NFAState decisionState )
        {
            if ( decisionState == null )
            {
                return 0;
            }
            int n = 1;
            NFAState p = decisionState;
            while ( p.transition[1] != null )
            {
                n++;
                p = (NFAState)p.transition[1].Target;
            }
            return n;
        }

        /** Get the ith alternative (1..n) from a decision; return null when
         *  an invalid alt is requested.  I must count in to find the right
         *  alternative number.  For (A|B), you get NFA structure (roughly):
         *
         *  o->o-A->o
         *  |
         *  o->o-B->o
         *
         *  This routine returns the leftmost state for each alt.  So alt=1, returns
         *  the upperleft most state in this structure.
         */
        public virtual NFAState GetNFAStateForAltOfDecision( NFAState decisionState, int alt )
        {
            if ( decisionState == null || alt <= 0 )
            {
                return null;
            }
            int n = 1;
            NFAState p = decisionState;
            while ( p != null )
            {
                if ( n == alt )
                {
                    return p;
                }
                n++;
                Transition next = p.transition[1];
                p = null;
                if ( next != null )
                {
                    p = (NFAState)next.Target;
                }
            }
            return null;
        }

#if false
        public virtual void ComputeRuleFOLLOWSets()
        {
            if ( getNumberOfDecisions() == 0 )
            {
                createNFAs();
            }
            for ( Iterator it = getRules().iterator(); it.hasNext(); )
            {
                Rule r = (Rule)it.next();
                if ( r.isSynPred )
                {
                    continue;
                }
                LookaheadSet s = ll1Analyzer.FOLLOW( r );
                JSystem.@out.println( "FOLLOW(" + r.name + ")=" + s );
            }
        }
#endif

        public virtual LookaheadSet First( NFAState s )
        {
            return ll1Analyzer.First( s );
        }

        public virtual LookaheadSet Look( NFAState s )
        {
            return ll1Analyzer.Look( s );
        }

        /** given a token type and the text of the literal, come up with a
         *  decent token type label.  For now it's just T<type>.  Actually,
         *  if there is an aliased name from tokens like PLUS='+', use it.
         */
        public virtual string ComputeTokenNameFromLiteral( int tokenType, string literal )
        {
            return AUTO_GENERATED_TOKEN_NAME_PREFIX + tokenType;
        }

        public override string ToString()
        {
            return GrammarTreeToString( grammarTree );
        }

        public virtual string GrammarTreeToString( GrammarAST t )
        {
            return GrammarTreeToString( t, true );
        }

        public virtual string GrammarTreeToString( GrammarAST t, bool showActions )
        {
            string s = null;
            try
            {
                s = t.Line + ":" + (t.CharPositionInLine+1) + ": ";
                s += new ANTLRTreePrinter( new Antlr.Runtime.Tree.CommonTreeNodeStream( t ) ).toString( this, showActions );
            }
            catch ( Exception /*e*/ )
            {
                s = "<invalid or missing tree structure>";
            }
            return s;
        }

        public virtual void PrintGrammar( TextWriter output )
        {
            ANTLRTreePrinter printer = new ANTLRTreePrinter( new Antlr.Runtime.Tree.CommonTreeNodeStream( grammarTree ) );
            //printer.setASTNodeClass( "org.antlr.tool.GrammarAST" );
            try
            {
                string g = printer.toString( this, false );
                output.WriteLine( g );
            }
            catch ( RecognitionException re )
            {
                ErrorManager.Error( ErrorManager.MSG_SYNTAX_ERROR, re );
            }
        }

    }
}
