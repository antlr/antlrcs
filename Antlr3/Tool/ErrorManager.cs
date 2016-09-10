/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr3.Extensions;

    using BitSet = Antlr3.Misc.BitSet;
    using CultureInfo = System.Globalization.CultureInfo;
    using DecisionProbe = Antlr3.Analysis.DecisionProbe;
    using DFAState = Antlr3.Analysis.DFAState;
    using FieldAccessException = System.FieldAccessException;
    using FieldInfo = System.Reflection.FieldInfo;
    using File = System.IO.File;
    using ICollection = System.Collections.ICollection;
    using ITemplateErrorListener = Antlr4.StringTemplate.ITemplateErrorListener;
    using IToken = Antlr.Runtime.IToken;
    using NFAState = Antlr3.Analysis.NFAState;
    using Path = System.IO.Path;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using StackFrame = System.Diagnostics.StackFrame;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;
    using TemplateGroupFile = Antlr4.StringTemplate.TemplateGroupFile;
    using TemplateMessage = Antlr4.StringTemplate.Misc.TemplateMessage;
    using Thread = System.Threading.Thread;
    using Tool = Antlr3.AntlrTool;
    using TraceListener = System.Diagnostics.TraceListener;

    public static class ErrorManager
    {
        // TOOL ERRORS
        // file errors
        public const int MSG_CANNOT_WRITE_FILE = 1;
        public const int MSG_CANNOT_CLOSE_FILE = 2;
        public const int MSG_CANNOT_FIND_TOKENS_FILE = 3;
        public const int MSG_ERROR_READING_TOKENS_FILE = 4;
        public const int MSG_DIR_NOT_FOUND = 5;
        public const int MSG_OUTPUT_DIR_IS_FILE = 6;
        public const int MSG_CANNOT_OPEN_FILE = 7;
        public const int MSG_FILE_AND_GRAMMAR_NAME_DIFFER = 8;
        public const int MSG_FILENAME_EXTENSION_ERROR = 9;

        public const int MSG_INTERNAL_ERROR = 10;
        public const int MSG_INTERNAL_WARNING = 11;
        public const int MSG_ERROR_CREATING_ARTIFICIAL_RULE = 12;
        public const int MSG_TOKENS_FILE_SYNTAX_ERROR = 13;
        public const int MSG_CANNOT_GEN_DOT_FILE = 14;
        public const int MSG_BAD_AST_STRUCTURE = 15;
        public const int MSG_BAD_ACTION_AST_STRUCTURE = 16;

        // code gen errors
        public const int MSG_MISSING_CODE_GEN_TEMPLATES = 20;
        public const int MSG_MISSING_CYCLIC_DFA_CODE_GEN_TEMPLATES = 21;
        public const int MSG_CODE_GEN_TEMPLATES_INCOMPLETE = 22;
        public const int MSG_CANNOT_CREATE_TARGET_GENERATOR = 23;
        //public const int MSG_CANNOT_COMPUTE_SAMPLE_INPUT_SEQ = 24;

        // GRAMMAR ERRORS
        public const int MSG_SYNTAX_ERROR = 100;
        public const int MSG_RULE_REDEFINITION = 101;
        public const int MSG_LEXER_RULES_NOT_ALLOWED = 102;
        public const int MSG_PARSER_RULES_NOT_ALLOWED = 103;
        public const int MSG_CANNOT_FIND_ATTRIBUTE_NAME_IN_DECL = 104;
        public const int MSG_NO_TOKEN_DEFINITION = 105;
        public const int MSG_UNDEFINED_RULE_REF = 106;
        public const int MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE = 107;
        public const int MSG_CANNOT_ALIAS_TOKENS_IN_LEXER = 108;
        public const int MSG_ATTRIBUTE_REF_NOT_IN_RULE = 111;
        public const int MSG_INVALID_RULE_SCOPE_ATTRIBUTE_REF = 112;
        public const int MSG_UNKNOWN_ATTRIBUTE_IN_SCOPE = 113;
        public const int MSG_UNKNOWN_SIMPLE_ATTRIBUTE = 114;
        public const int MSG_INVALID_RULE_PARAMETER_REF = 115;
        public const int MSG_UNKNOWN_RULE_ATTRIBUTE = 116;
        public const int MSG_ISOLATED_RULE_SCOPE = 117;
        public const int MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE = 118;
        public const int MSG_LABEL_CONFLICTS_WITH_RULE = 119;
        public const int MSG_LABEL_CONFLICTS_WITH_TOKEN = 120;
        public const int MSG_LABEL_CONFLICTS_WITH_RULE_SCOPE_ATTRIBUTE = 121;
        public const int MSG_LABEL_CONFLICTS_WITH_RULE_ARG_RETVAL = 122;
        public const int MSG_ATTRIBUTE_CONFLICTS_WITH_RULE = 123;
        public const int MSG_ATTRIBUTE_CONFLICTS_WITH_RULE_ARG_RETVAL = 124;
        public const int MSG_LABEL_TYPE_CONFLICT = 125;
        public const int MSG_ARG_RETVAL_CONFLICT = 126;
        public const int MSG_NONUNIQUE_REF = 127;
        public const int MSG_FORWARD_ELEMENT_REF = 128;
        public const int MSG_MISSING_RULE_ARGS = 129;
        public const int MSG_RULE_HAS_NO_ARGS = 130;
        public const int MSG_ARGS_ON_TOKEN_REF = 131;
        public const int MSG_RULE_REF_AMBIG_WITH_RULE_IN_ALT = 132;
        public const int MSG_ILLEGAL_OPTION = 133;
        public const int MSG_LIST_LABEL_INVALID_UNLESS_RETVAL_STRUCT = 134;
        public const int MSG_UNDEFINED_TOKEN_REF_IN_REWRITE = 135;
        public const int MSG_REWRITE_ELEMENT_NOT_PRESENT_ON_LHS = 136;
        public const int MSG_UNDEFINED_LABEL_REF_IN_REWRITE = 137;
        public const int MSG_NO_GRAMMAR_START_RULE = 138;
        public const int MSG_EMPTY_COMPLEMENT = 139;
        public const int MSG_UNKNOWN_DYNAMIC_SCOPE = 140;
        public const int MSG_UNKNOWN_DYNAMIC_SCOPE_ATTRIBUTE = 141;
        public const int MSG_ISOLATED_RULE_ATTRIBUTE = 142;
        public const int MSG_INVALID_ACTION_SCOPE = 143;
        public const int MSG_ACTION_REDEFINITION = 144;
        public const int MSG_DOUBLE_QUOTES_ILLEGAL = 145;
        public const int MSG_INVALID_TEMPLATE_ACTION = 146;
        public const int MSG_MISSING_ATTRIBUTE_NAME = 147;
        public const int MSG_ARG_INIT_VALUES_ILLEGAL = 148;
        public const int MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION = 149;
        public const int MSG_NO_RULES = 150;
        public const int MSG_WRITE_TO_READONLY_ATTR = 151;
        public const int MSG_MISSING_AST_TYPE_IN_TREE_GRAMMAR = 152;
        public const int MSG_REWRITE_FOR_MULTI_ELEMENT_ALT = 153;
        public const int MSG_RULE_INVALID_SET = 154;
        public const int MSG_HETERO_ILLEGAL_IN_REWRITE_ALT = 155;
        public const int MSG_NO_SUCH_GRAMMAR_SCOPE = 156;
        public const int MSG_NO_SUCH_RULE_IN_SCOPE = 157;
        public const int MSG_TOKEN_ALIAS_CONFLICT = 158;
        public const int MSG_TOKEN_ALIAS_REASSIGNMENT = 159;
        public const int MSG_TOKEN_VOCAB_IN_DELEGATE = 160;
        public const int MSG_INVALID_IMPORT = 161;
        public const int MSG_IMPORTED_TOKENS_RULE_EMPTY = 162;
        public const int MSG_IMPORT_NAME_CLASH = 163;
        public const int MSG_AST_OP_WITH_NON_AST_OUTPUT_OPTION = 164;
        public const int MSG_AST_OP_IN_ALT_WITH_REWRITE = 165;
        public const int MSG_WILDCARD_AS_ROOT = 166;
        public const int MSG_CONFLICTING_OPTION_IN_TREE_FILTER = 167;
        public const int MSG_ILLEGAL_OPTION_VALUE = 168;
        public const int MSG_ALL_OPS_NEED_SAME_ASSOC = 169;
        public const int MSG_RANGE_OP_ILLEGAL = 170;

        // GRAMMAR WARNINGS
        public const int MSG_GRAMMAR_NONDETERMINISM = 200; // A predicts alts 1,2
        public const int MSG_UNREACHABLE_ALTS = 201;       // nothing predicts alt i
        public const int MSG_DANGLING_STATE = 202;        // no edges out of state
        public const int MSG_INSUFFICIENT_PREDICATES = 203;
        public const int MSG_DUPLICATE_SET_ENTRY = 204;    // (A|A)
        public const int MSG_ANALYSIS_ABORTED = 205;
        public const int MSG_RECURSION_OVERLOW = 206;
        public const int MSG_LEFT_RECURSION = 207;
        public const int MSG_UNREACHABLE_TOKENS = 208; // nothing predicts token
        public const int MSG_TOKEN_NONDETERMINISM = 209; // alts of Tokens rule
        public const int MSG_LEFT_RECURSION_CYCLES = 210;
        public const int MSG_NONREGULAR_DECISION = 211;

        public const int MAX_MESSAGE_NUMBER = 211;

        /** Do not do perform analysis if one of these happens */
        public static readonly BitSet ErrorsForcingNoAnalysis =
            new BitSet()
            {
                MSG_CANNOT_CREATE_TARGET_GENERATOR,
                MSG_RULE_REDEFINITION,
                MSG_UNDEFINED_RULE_REF,
                MSG_LEFT_RECURSION_CYCLES,
                MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION,
                MSG_NO_RULES,
                MSG_NO_SUCH_GRAMMAR_SCOPE,
                MSG_NO_SUCH_RULE_IN_SCOPE,
                MSG_LEXER_RULES_NOT_ALLOWED,
                MSG_WILDCARD_AS_ROOT
            };

        /** Do not do code gen if one of these happens */
        public static readonly BitSet ErrorsForcingNoCodegen =
            new BitSet()
            {
                MSG_NONREGULAR_DECISION,
                MSG_RECURSION_OVERLOW,
                MSG_UNREACHABLE_ALTS,
                MSG_FILE_AND_GRAMMAR_NAME_DIFFER,
                MSG_INVALID_IMPORT,
                MSG_AST_OP_WITH_NON_AST_OUTPUT_OPTION
            };

        /** Only one error can be emitted for any entry in this table.
         *  Map from string to a set where the key is a method name like danglingState.
         *  The set is whatever that method accepts or derives like a DFA.
         */
        public static readonly IDictionary<string, ICollection<object>> emitSingleError =
            new Dictionary<string, ICollection<object>>()
            {
                { "danglingState", new HashSet<object>() }
            };

        /** Messages should be sensitive to the locale. */
        private static CultureInfo locale;
        private static string formatName;

        /** Each thread might need it's own error listener; e.g., a GUI with
         *  multiple window frames holding multiple grammars.
         */
        [ThreadStatic]
        private static IANTLRErrorListener _listener;

        public class ErrorState
        {
            public int errors;
            public int warnings;
            public int infos;

            public BitSet errorMsgIDs = new BitSet();
            public BitSet warningMsgIDs = new BitSet();
        }

        /** Track the number of errors regardless of the listener but track
         *  per thread.
         */
        [ThreadStatic]
        private static ErrorState _errorState;

        /** Each thread has its own ptr to a Tool object, which knows how
         *  to panic, for example.  In a GUI, the thread might just throw an Error
         *  to exit rather than the suicide System.exit.
         */
        [ThreadStatic]
        private static Tool _tool;

        /** The group of templates that represent all possible ANTLR errors. */
        private static TemplateGroup messages;
        /** The group of templates that represent the current message format. */
        private static TemplateGroup format;

        /** From a msgID how can I get the name of the template that describes
         *  the error or warning?
         */
        private static readonly String[] idToMessageTemplateName = new String[MAX_MESSAGE_NUMBER + 1];

        static ErrorManager()
        {
            InitIdToMessageNameMapping();
        }

        public static TraceListener ExternalListener
        {
            get;
            set;
        }

        private class DefaultErrorListener : IANTLRErrorListener
        {
            public virtual void Info( String msg )
            {
                if ( FormatWantsSingleLineMessage() )
                {
                    msg = msg.Replace( '\n', ' ' );
                }
                Console.Error.WriteLine( msg );

                if (ExternalListener != null)
                    ExternalListener.WriteLine(msg);
            }

            public virtual void Error( Message msg )
            {
                String outputMsg = msg.ToString();
                if ( FormatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.Replace( '\n', ' ' );
                }
                Console.Error.WriteLine( outputMsg );

                if (ExternalListener != null)
                    ExternalListener.WriteLine(outputMsg);
            }

            public virtual void Warning( Message msg )
            {
                String outputMsg = msg.ToString();
                if ( FormatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.Replace( '\n', ' ' );
                }
                Console.Error.WriteLine( outputMsg );

                if (ExternalListener != null)
                    ExternalListener.WriteLine(outputMsg);
            }

            public virtual void Error( ToolMessage msg )
            {
                String outputMsg = msg.ToString();
                if ( FormatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.Replace( '\n', ' ' );
                }
                Console.Error.WriteLine( outputMsg );

                if (ExternalListener != null)
                    ExternalListener.WriteLine(outputMsg);
            }
        }

        private static IANTLRErrorListener theDefaultErrorListener = new DefaultErrorListener();

        private class InitSTListener : ITemplateErrorListener
        {
            public void CompiletimeError(TemplateMessage msg)
            {
                Console.Error.WriteLine("ErrorManager init error: " + msg);
            }

            public void RuntimeError(TemplateMessage msg)
            {
                Console.Error.WriteLine("ErrorManager init error: " + msg);
            }

            public void IOError(TemplateMessage msg)
            {
                Console.Error.WriteLine("ErrorManager init error: " + msg);
            }

            public void InternalError(TemplateMessage msg)
            {
                Console.Error.WriteLine("ErrorManager init error: " + msg);
            }
        }

        /** Handle all ST error listeners here (code gen, Grammar, and this class
         *  use templates.
         */
        private static ITemplateErrorListener initSTListener = new InitSTListener();

        private sealed class BlankSTListener : ITemplateErrorListener
        {
            public void CompiletimeError(TemplateMessage msg)
            {
            }

            public void RuntimeError(TemplateMessage msg)
            {
            }

            public void IOError(TemplateMessage msg)
            {
            }

            public void InternalError(TemplateMessage msg)
            {
            }
        }

        /** During verification of the messages group file, don't gen errors.
         *  I'll handle them here.  This is used only after file has loaded ok
         *  and only for the messages STG.
         */
        private static ITemplateErrorListener blankSTListener = new BlankSTListener();

        private class DefaultSTListener : ITemplateErrorListener
        {
            public void CompiletimeError(TemplateMessage msg)
            {
                ErrorManager.Error(ErrorManager.MSG_INTERNAL_ERROR, msg.ToString(), msg.Cause);
            }

            public void RuntimeError(TemplateMessage msg)
            {
                ErrorManager.Error(ErrorManager.MSG_INTERNAL_ERROR, msg.ToString(), msg.Cause);
            }

            public void IOError(TemplateMessage msg)
            {
                ErrorManager.Error(ErrorManager.MSG_INTERNAL_ERROR, msg.ToString(), msg.Cause);
            }

            public void InternalError(TemplateMessage msg)
            {
                ErrorManager.Error(ErrorManager.MSG_INTERNAL_ERROR, msg.ToString(), msg.Cause);
            }
        }

        /** Errors during initialization related to ST must all go to System.err.
         */
        private static ITemplateErrorListener theDefaultSTListener = new DefaultSTListener();

        internal static void Initialize()
        {
            // it is inefficient to set the default locale here if another
            // piece of code is going to set the locale, but that would
            // require that a user call an init() function or something.  I prefer
            // that this class be ready to go when loaded as I'm absentminded ;)
            SetLocale( CultureInfo.CurrentCulture );

            // try to load the message format group
            // the user might have specified one on the command line
            // if not, or if the user has given an illegal value, we will fall back to "antlr"
            SetFormat( "antlr" );
        }

        public static ITemplateErrorListener GetStringTemplateErrorListener()
        {
            return theDefaultSTListener;
        }

        /** We really only need a single locale for entire running ANTLR code
         *  in a single VM.  Only pay attention to the language, not the country
         *  so that French Canadians and French Frenchies all get the same
         *  template file, fr.stg.  Just easier this way.
         */
        public static void SetLocale( CultureInfo locale )
        {
            if (ErrorManager.locale == locale)
                return;

            ErrorManager.locale = locale;
            string language = locale.TwoLetterISOLanguageName;
            string fileName = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(AntlrTool.ToolPathRoot, "Tool"), "Templates"), "messages"), "languages"), language + ".stg");
            if (!File.Exists(fileName) && locale.TwoLetterISOLanguageName != CultureInfo.GetCultureInfo("en-us").TwoLetterISOLanguageName)
            {
                SetLocale(CultureInfo.GetCultureInfo("en-us"));
                return;
            }

            messages = new TemplateGroupFile(fileName);
            messages.EnableCache = AntlrTool.EnableTemplateCache;
            messages.Listener = initSTListener;
            if (!messages.IsDefined("INTERNAL_ERROR"))
            {
                // pick random msg to load
                if (language.Equals(CultureInfo.GetCultureInfo("en-us").TwoLetterISOLanguageName))
                {
                    RawError("ANTLR installation corrupted; cannot find English messages file " + fileName);
                    Panic();
                }
                else
                {
                    // recurse on this rule, trying the US locale
                    SetLocale(CultureInfo.GetCultureInfo("en-us"));
                }
            }

            messages.Listener = blankSTListener;
            bool messagesOK = VerifyMessages();
            if ( !messagesOK && language.Equals(CultureInfo.GetCultureInfo("en-us").TwoLetterISOLanguageName) ) {
                RawError("ANTLR installation corrupted; English messages file "+language+".stg incomplete");
                Panic();
            }
            else if ( !messagesOK ) {
                SetLocale(CultureInfo.GetCultureInfo("en-us")); // try US to see if that will work
            }
        }

        /** The format gets reset either from the Tool if the user supplied a command line option to that effect
         *  Otherwise we just use the default "antlr".
         */
        public static void SetFormat( String formatName )
        {
            if (ErrorManager.formatName == formatName)
                return;

            ErrorManager.formatName = formatName;
            string fileName = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(AntlrTool.ToolPathRoot, "Tool"), "Templates"), "messages"), "formats"), formatName + ".stg");
            format = new TemplateGroupFile(fileName);
            if (!File.Exists(fileName) && formatName != "antlr")
            {
                SetFormat("antlr");
                return;
            }

            format.EnableCache = AntlrTool.EnableTemplateCache;
            format.Listener = initSTListener;
            if (!format.IsDefined("message"))
            {
                // pick random msg to load
                if (formatName.Equals("antlr"))
                {
                    RawError("no such message format file " + fileName + " retrying with default ANTLR format");
                    // recurse on this rule, trying the default message format
                    SetFormat("antlr");
                    return;
                }
                else
                {
                    // recurse on this rule, trying the default message format
                    SetFormat("antlr");
                }
            }

            format.Listener = blankSTListener;
            bool formatOK = VerifyFormat();
            if ( !formatOK && formatName.Equals( "antlr" ) )
            {
                RawError( "ANTLR installation corrupted; ANTLR messages format file " + formatName + ".stg incomplete" );
                Panic();
            }
            else if ( !formatOK )
            {
                SetFormat( "antlr" ); // recurse on this rule, trying the default message format
            }
        }

        /** Encodes the error handling found in setLocale, but does not trigger
         *  panics, which would make GUI tools die if ANTLR's installation was
         *  a bit screwy.  Duplicated code...ick.
        public static Locale getLocaleForValidMessages(Locale locale) {
            ErrorManager.locale = locale;
            String language = locale.getLanguage();
            String fileName = "org/antlr/tool/templates/messages/"+language+".stg";
            ClassLoader cl = Thread.currentThread().getContextClassLoader();
            InputStream is = cl.getResourceAsStream(fileName);
            if ( is==null &amp;&amp; language.equals(Locale.US.getLanguage()) ) {
                return null;
            }
            else if ( is==null ) {
                return getLocaleForValidMessages(Locale.US); // recurse on this rule, trying the US locale
            }

            boolean messagesOK = verifyMessages();
            if ( !messagesOK &amp;&amp; language.equals(Locale.US.getLanguage()) ) {
                return null;
            }
            else if ( !messagesOK ) {
                return getLocaleForValidMessages(Locale.US); // try US to see if that will work
            }
            return true;
        }
         */

        /** In general, you'll want all errors to go to a single spot.
         *  However, in a GUI, you might have two frames up with two
         *  different grammars.  Two threads might launch to process the
         *  grammars--you would want errors to go to different objects
         *  depending on the thread.  I store a single listener per
         *  thread.
         */
        public static void SetErrorListener( IANTLRErrorListener listener )
        {
            ErrorManager._listener = listener;
        }

        public static void RemoveErrorListener()
        {
            ErrorManager._listener = null;
        }

        public static Tool GetTool()
        {
            return _tool;
        }

        public static void SetTool( Tool tool )
        {
            _tool = tool;
        }

        /** Given a message ID, return a StringTemplate that somebody can fill
         *  with data.  We need to convert the int ID to the name of a template
         *  in the messages ST group.
         */
        public static StringTemplate GetMessage( int msgID )
        {
            String msgName = idToMessageTemplateName[msgID];
            return messages.GetInstanceOf( msgName );
        }
        public static String GetMessageType( int msgID )
        {
            if ( GetErrorState().warningMsgIDs.Contains( msgID ) )
            {
                return messages.GetInstanceOf( "warning" ).Render();
            }
            else if ( GetErrorState().errorMsgIDs.Contains( msgID ) )
            {
                return messages.GetInstanceOf( "error" ).Render();
            }
            AssertTrue( false, "Assertion failed! Message ID " + msgID + " created but is not present in errorMsgIDs or warningMsgIDs." );
            return "";
        }

        /** Return a StringTemplate that refers to the current format used for
         * emitting messages.
         */
        public static StringTemplate GetLocationFormat()
        {
            return format.GetInstanceOf( "location" );
        }
        public static StringTemplate GetReportFormat()
        {
            return format.GetInstanceOf( "report" );
        }
        public static StringTemplate GetMessageFormat()
        {
            return format.GetInstanceOf( "message" );
        }
        public static bool FormatWantsSingleLineMessage()
        {
            return format.GetInstanceOf( "wantsSingleLineMessage" ).Render().Equals( "true" );
        }

        public static IANTLRErrorListener GetErrorListener()
        {
            IANTLRErrorListener el = _listener;
            if ( el == null )
                return theDefaultErrorListener;

            return el;
        }

        public static ErrorState GetErrorState()
        {
            ErrorState ec = _errorState;
            if ( ec == null )
            {
                ec = new ErrorState();
                _errorState = ec;
            }

            return ec;
        }

        public static int GetNumErrors()
        {
            return GetErrorState().errors;
        }

        public static void ResetErrorState()
        {
            _errorState = new ErrorState();
        }

        public static void Info( String msg )
        {
            GetErrorState().infos++;
            GetErrorListener().Info( msg );
        }

        public static void Error( int msgID )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( new ToolMessage( msgID ) );
        }

        public static void Error( int msgID, Exception e )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( new ToolMessage( msgID, e ) );
        }

        public static void Error( int msgID, Object arg )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( new ToolMessage( msgID, arg ) );
        }

        public static void Error( int msgID, Object arg, Object arg2 )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( new ToolMessage( msgID, arg, arg2 ) );
        }

        public static void Error( int msgID, Object arg, Exception e )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( new ToolMessage( msgID, arg, e ) );
        }

        public static void Warning( int msgID, Object arg )
        {
            GetErrorState().warnings++;
            GetErrorState().warningMsgIDs.Add( msgID );
            GetErrorListener().Warning( new ToolMessage( msgID, arg ) );
        }

        public static void Nondeterminism( DecisionProbe probe,
                                          DFAState d )
        {
            GetErrorState().warnings++;
            Message msg = new GrammarNonDeterminismMessage( probe, d );
            GetErrorState().warningMsgIDs.Add( msg.msgID );
            GetErrorListener().Warning( msg );
        }

        public static void DanglingState( DecisionProbe probe,
                                         DFAState d )
        {
            GetErrorState().errors++;
            Message msg = new GrammarDanglingStateMessage( probe, d );
            GetErrorState().errorMsgIDs.Add( msg.msgID );
            ICollection<object> seen;
            emitSingleError.TryGetValue("danglingState", out seen);
            if ( !seen.Contains( d.Dfa.DecisionNumber + "|" + d.AltSet ) )
            {
                GetErrorListener().Error( msg );
                // we've seen this decision and this alt set; never again
                seen.Add( d.Dfa.DecisionNumber + "|" + d.AltSet );
            }
        }

        public static void AnalysisAborted( DecisionProbe probe )
        {
            GetErrorState().warnings++;
            Message msg = new GrammarAnalysisAbortedMessage( probe );
            GetErrorState().warningMsgIDs.Add( msg.msgID );
            GetErrorListener().Warning( msg );
        }

        public static void UnreachableAlts( DecisionProbe probe,
                                           IEnumerable<int> alts )
        {
            GetErrorState().errors++;
            Message msg = new GrammarUnreachableAltsMessage( probe, alts );
            GetErrorState().errorMsgIDs.Add( msg.msgID );
            GetErrorListener().Error( msg );
        }

        public static void InsufficientPredicates( DecisionProbe probe,
                                                  DFAState d,
                                                  IDictionary<int, ICollection<IToken>> altToUncoveredLocations )
        {
            GetErrorState().warnings++;
            Message msg = new GrammarInsufficientPredicatesMessage( probe, d, altToUncoveredLocations );
            GetErrorState().warningMsgIDs.Add( msg.msgID );
            GetErrorListener().Warning( msg );
        }

        public static void NonLLStarDecision( DecisionProbe probe )
        {
            GetErrorState().errors++;
            Message msg = new NonRegularDecisionMessage( probe, probe.NonDeterministicAlts );
            GetErrorState().errorMsgIDs.Add( msg.msgID );
            GetErrorListener().Error( msg );
        }

        public static void RecursionOverflow( DecisionProbe probe,
                                             DFAState sampleBadState,
                                             int alt,
                                             ICollection<string> targetRules,
                                             ICollection<ICollection<NFAState>> callSiteStates )
        {
            GetErrorState().errors++;
            Message msg = new RecursionOverflowMessage( probe, sampleBadState, alt,
                                             targetRules, callSiteStates );
            GetErrorState().errorMsgIDs.Add( msg.msgID );
            GetErrorListener().Error( msg );
        }

#if false
        // TODO: we can remove I think.  All detected now with cycles check.
        public static void LeftRecursion(DecisionProbe probe,
                                         int alt,
                                         ICollection targetRules,
                                         ICollection callSiteStates)
        {
            getErrorState().warnings++;
            Message msg = new LeftRecursionMessage(probe, alt, targetRules, callSiteStates);
            getErrorState().warningMsgIDs.add(msg.msgID);
            getErrorListener().Warning(msg);
        }
#endif

        public static void LeftRecursionCycles( ICollection cycles )
        {
            GetErrorState().errors++;
            Message msg = new LeftRecursionCyclesMessage( cycles );
            GetErrorState().errorMsgIDs.Add( msg.msgID );
            GetErrorListener().Error( msg );
        }

        public static void GrammarError( int msgID,
                                        Grammar g,
                                        IToken token,
                                        Object arg,
                                        Object arg2 )
        {
            GetErrorState().errors++;
            Message msg = new GrammarSemanticsMessage( msgID, g, token, arg, arg2 );
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error( msg );
        }

        public static void GrammarError( int msgID,
                                        Grammar g,
                                        IToken token,
                                        Object arg )
        {
            GrammarError( msgID, g, token, arg, null );
        }

        public static void GrammarError( int msgID,
                                        Grammar g,
                                        IToken token )
        {
            GrammarError( msgID, g, token, null, null );
        }

        public static void GrammarWarning( int msgID,
                                          Grammar g,
                                          IToken token,
                                          Object arg,
                                          Object arg2 )
        {
            GetErrorState().warnings++;
            Message msg = new GrammarSemanticsMessage( msgID, g, token, arg, arg2 );
            GetErrorState().warningMsgIDs.Add( msgID );
            GetErrorListener().Warning( msg );
        }

        public static void GrammarWarning( int msgID,
                                          Grammar g,
                                          IToken token,
                                          Object arg )
        {
            GrammarWarning( msgID, g, token, arg, null );
        }

        public static void GrammarWarning( int msgID,
                                          Grammar g,
                                          IToken token )
        {
            GrammarWarning( msgID, g, token, null, null );
        }

        public static void SyntaxError( int msgID,
                                       Grammar grammar,
                                       IToken token,
                                       Object arg,
                                       RecognitionException re )
        {
            GetErrorState().errors++;
            GetErrorState().errorMsgIDs.Add( msgID );
            GetErrorListener().Error(
                new GrammarSyntaxMessage( msgID, grammar, token, arg, re )
            );
        }

        public static void InternalError( Object error, Exception e )
        {
            StackFrame location = GetLastNonErrorManagerCodeLocation( e );
            String msg = "Exception " + e + "@" + location + ": " + error;
            ErrorManager.Error( MSG_INTERNAL_ERROR, msg );
        }

        public static void InternalError( Object error )
        {
            StackFrame location =
                GetLastNonErrorManagerCodeLocation( new Exception() );
            String msg = location + ": " + error;
            ErrorManager.Error( MSG_INTERNAL_ERROR, msg );
        }

        public static bool DoNotAttemptAnalysis()
        {
            return !GetErrorState().errorMsgIDs.And( ErrorsForcingNoAnalysis ).IsNil;
        }

        public static bool DoNotAttemptCodeGen()
        {
            return DoNotAttemptAnalysis() ||
                   !GetErrorState().errorMsgIDs.And( ErrorsForcingNoCodegen ).IsNil;
        }

        /** Return first non ErrorManager code location for generating messages */
        private static StackFrame GetLastNonErrorManagerCodeLocation( Exception e )
        {
            StackFrame[] stack = e.GetStackTrace();
            int i = 0;
            for ( ; i < stack.Length; i++ )
            {
                StackFrame t = stack[i];
                if ( t.ToString().IndexOf( "ErrorManager" ) < 0 )
                {
                    break;
                }
            }
            StackFrame location = stack[i];
            return location;
        }

        // A S S E R T I O N  C O D E

        public static void AssertTrue( bool condition, String message )
        {
            if ( !condition )
            {
                InternalError( message );
            }
        }

        // S U P P O R T  C O D E

        static bool InitIdToMessageNameMapping()
        {
            // make sure a message exists, even if it's just to indicate a problem
            for ( int i = 0; i < idToMessageTemplateName.Length; i++ )
            {
                idToMessageTemplateName[i] = "INVALID MESSAGE ID: " + i;
            }
            // get list of fields and use it to fill in idToMessageTemplateName mapping
            FieldInfo[] fields = typeof( ErrorManager ).GetFields();
            for ( int i = 0; i < fields.Length; i++ )
            {
                FieldInfo f = fields[i];
                String fieldName = f.Name;
                if ( !fieldName.StartsWith( "MSG_" ) )
                {
                    continue;
                }
                String templateName =
                    fieldName.Substring( "MSG_".Length );
                int msgID = 0;
                try
                {
                    // get the constant value from this class object
                    msgID = (int)f.GetValue( null );
                    //msgID = f.getInt( typeof( ErrorManager ) );
                }
                catch ( FieldAccessException /*iae*/ )
                {
                    Console.Out.WriteLine( "cannot get const value for " + f.Name );
                    continue;
                }
                if ( fieldName.StartsWith( "MSG_" ) )
                {
                    idToMessageTemplateName[msgID] = templateName;
                }
            }
            return true;
        }

        /** Use reflection to find list of MSG_ fields and then verify a
         *  template exists for each one from the locale's group.
         */
        static bool VerifyMessages()
        {
            bool ok = true;
            FieldInfo[] fields = typeof( ErrorManager ).GetFields();
            for ( int i = 0; i < fields.Length; i++ )
            {
                FieldInfo f = fields[i];
                String fieldName = f.Name;
                String templateName =
                    fieldName.Substring( "MSG_".Length );
                if ( fieldName.StartsWith( "MSG_" ) )
                {
                    if ( !messages.IsDefined( templateName ) )
                    {
                        Console.Out.WriteLine( "Message " + templateName + " in locale " +
                                           locale + " not found" );
                        ok = false;
                    }
                }
            }
            // check for special templates
            if ( !messages.IsDefined( "warning" ) )
            {
                Console.Error.WriteLine( "Message template 'warning' not found in locale " + locale );
                ok = false;
            }
            if ( !messages.IsDefined( "error" ) )
            {
                Console.Error.WriteLine( "Message template 'error' not found in locale " + locale );
                ok = false;
            }
            return ok;
        }

        /** Verify the message format template group */
        static bool VerifyFormat()
        {
            bool ok = true;
            if ( !format.IsDefined( "location" ) )
            {
                Console.Error.WriteLine( "Format template 'location' not found in " + formatName );
                ok = false;
            }
            if ( !format.IsDefined( "message" ) )
            {
                Console.Error.WriteLine( "Format template 'message' not found in " + formatName );
                ok = false;
            }
            if ( !format.IsDefined( "report" ) )
            {
                Console.Error.WriteLine( "Format template 'report' not found in " + formatName );
                ok = false;
            }
            return ok;
        }

        /** If there are errors during ErrorManager init, we have no choice
         *  but to go to System.err.
         */
        static void RawError( String msg )
        {
            Console.Error.WriteLine( msg );
        }

        static void RawError( String msg, Exception e )
        {
            RawError( msg );
            e.PrintStackTrace( Console.Error );
        }

        /** I *think* this will allow Tool subclasses to exit gracefully
         *  for GUIs etc...
         */
        public static void Panic()
        {
            Tool tool = _tool;
            if ( tool == null )
            {
                // no tool registered, exit
                throw new Exception( "ANTLR ErrorManager panic" );
            }
            else
            {
                tool.Panic();
            }
        }
    }
}
