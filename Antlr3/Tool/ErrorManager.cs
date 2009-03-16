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
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Misc;

    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using CultureInfo = System.Globalization.CultureInfo;
    using DecisionProbe = Antlr3.Analysis.DecisionProbe;
    using DFAState = Antlr3.Analysis.DFAState;
    using FieldAccessException = System.FieldAccessException;
    using FieldInfo = System.Reflection.FieldInfo;
    using ICollection = System.Collections.ICollection;
    using IOException = System.IO.IOException;
    using IStringTemplateErrorListener = Antlr3.ST.IStringTemplateErrorListener;
    using IToken = Antlr.Runtime.IToken;
    using NFAState = Antlr3.Analysis.NFAState;
    using RecognitionException = Antlr.Runtime.RecognitionException;
    using StackFrame = System.Diagnostics.StackFrame;
    using StreamReader = System.IO.StreamReader;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;
    using TargetInvocationException = System.Reflection.TargetInvocationException;
    using Thread = System.Threading.Thread;
    using Tool = Antlr3.AntlrTool;

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
        public static readonly BitSet ERRORS_FORCING_NO_ANALYSIS;
        //public static readonly BitSet ERRORS_FORCING_NO_ANALYSIS = new BitSet() {
        //    {
        //        add(MSG_RULE_REDEFINITION);
        //        add(MSG_UNDEFINED_RULE_REF);
        //        add(MSG_LEFT_RECURSION_CYCLES);
        //        add(MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION);
        //        add(MSG_NO_RULES);
        //        add(MSG_NO_SUCH_GRAMMAR_SCOPE);
        //        add(MSG_NO_SUCH_RULE_IN_SCOPE);
        //        add(MSG_LEXER_RULES_NOT_ALLOWED);
        //        // TODO: ...
        //    }
        //};

        /** Do not do code gen if one of these happens */
        public static readonly BitSet ERRORS_FORCING_NO_CODEGEN;
        //public static readonly BitSet ERRORS_FORCING_NO_CODEGEN = new BitSet() {
        //    {
        //        add(MSG_NONREGULAR_DECISION);
        //        add(MSG_RECURSION_OVERLOW);
        //        add(MSG_UNREACHABLE_ALTS);
        //        add(MSG_FILE_AND_GRAMMAR_NAME_DIFFER);
        //        add(MSG_INVALID_IMPORT);
        //        add(MSG_AST_OP_WITH_NON_AST_OUTPUT_OPTION);
        //        // TODO: ...
        //    }
        //};

        /** Only one error can be emitted for any entry in this table.
         *  Map<String,Set> where the key is a method name like danglingState.
         *  The set is whatever that method accepts or derives like a DFA.
         */
        public static readonly IDictionary<string, ICollection<object>> emitSingleError;
        //public static readonly Map emitSingleError = new HashMap() {
        //    {
        //        put("danglingState", new HashSet());
        //    }
        //};


        /** Messages should be sensitive to the locale. */
        private static CultureInfo locale;
        private static string formatName;

        /** Each thread might need it's own error listener; e.g., a GUI with
         *  multiple window frames holding multiple grammars.
         */
        private static Dictionary<Thread, IANTLRErrorListener> threadToListenerMap = new Dictionary<Thread, IANTLRErrorListener>();

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
        private static IDictionary<Thread, ErrorState> threadToErrorStateMap = new Dictionary<Thread, ErrorState>();

        /** Each thread has its own ptr to a Tool object, which knows how
         *  to panic, for example.  In a GUI, the thread might just throw an Error
         *  to exit rather than the suicide System.exit.
         */
        private static IDictionary<Thread, Tool> threadToToolMap = new Dictionary<Thread, Tool>();

        /** The group of templates that represent all possible ANTLR errors. */
        private static StringTemplateGroup messages;
        /** The group of templates that represent the current message format. */
        private static StringTemplateGroup format;

        /** From a msgID how can I get the name of the template that describes
         *  the error or warning?
         */
        private static String[] idToMessageTemplateName = new String[MAX_MESSAGE_NUMBER + 1];

        class DefaultErrorListener : IANTLRErrorListener
        {
            public virtual void info( String msg )
            {
                if ( formatWantsSingleLineMessage() )
                {
                    msg = msg.replaceAll( "\n", " " );
                }
                Console.Error.WriteLine( msg );
            }

            public virtual void error( Message msg )
            {
                String outputMsg = msg.ToString();
                if ( formatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.replaceAll( "\n", " " );
                }
                Console.Error.WriteLine( outputMsg );
            }

            public virtual void warning( Message msg )
            {
                String outputMsg = msg.ToString();
                if ( formatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.replaceAll( "\n", " " );
                }
                Console.Error.WriteLine( outputMsg );
            }

            public virtual void error( ToolMessage msg )
            {
                String outputMsg = msg.ToString();
                if ( formatWantsSingleLineMessage() )
                {
                    outputMsg = outputMsg.replaceAll( "\n", " " );
                }
                Console.Error.WriteLine( outputMsg );
            }
        }

        static IANTLRErrorListener theDefaultErrorListener = new DefaultErrorListener();
        //static ANTLRErrorListener theDefaultErrorListener = new ANTLRErrorListener() {
        //    public void info(String msg) {
        //        if (formatWantsSingleLineMessage()) {
        //            msg = msg.replaceAll("\n", " ");
        //        }
        //        System.err.println(msg);
        //    }

        //    public void error(Message msg) {
        //        String outputMsg = msg.toString();
        //        if (formatWantsSingleLineMessage()) {
        //            outputMsg = outputMsg.replaceAll("\n", " ");
        //        }
        //        System.err.println(outputMsg);
        //    }

        //    public void warning(Message msg) {
        //        String outputMsg = msg.toString();
        //        if (formatWantsSingleLineMessage()) {
        //            outputMsg = outputMsg.replaceAll("\n", " ");
        //        }
        //        System.err.println(outputMsg);
        //    }

        //    public void error(ToolMessage msg) {
        //        String outputMsg = msg.toString();
        //        if (formatWantsSingleLineMessage()) {
        //            outputMsg = outputMsg.replaceAll("\n", " ");
        //        }
        //        System.err.println(outputMsg);
        //    }
        //};

        class InitSTListener : IStringTemplateErrorListener
        {
            public virtual void Error( String s, Exception e )
            {
                Console.Error.WriteLine( "ErrorManager init error: " + s );
                if ( e != null )
                {
                    Console.Error.WriteLine( "exception: " + e );
                }
                /*
                if ( e!=null ) {
                    e.printStackTrace(System.err);
                }
                */
            }
            public virtual void Warning( String s )
            {
                Console.Error.WriteLine( "ErrorManager init warning: " + s );
            }
            public virtual void Debug( String s )
            {
            }
        }

        /** Handle all ST error listeners here (code gen, Grammar, and this class
         *  use templates.
         */
        static IStringTemplateErrorListener initSTListener = new InitSTListener();
        //static StringTemplateErrorListener initSTListener =
        //    new StringTemplateErrorListener() {
        //        public void error(String s, Throwable e) {
        //            System.err.println("ErrorManager init error: "+s);
        //            if ( e!=null ) {
        //                System.err.println("exception: "+e);
        //            }
        //            /*
        //            if ( e!=null ) {
        //                e.printStackTrace(System.err);
        //            }
        //            */
        //        }
        //        public void warning(String s) {
        //            System.err.println("ErrorManager init warning: "+s);
        //        }
        //        public void debug(String s) {}
        //    };

        class BlankSTListener : IStringTemplateErrorListener
        {
            public virtual void Error( string msg, Exception e )
            {
            }
            public virtual void Warning( string msg )
            {
            }
        }

        /** During verification of the messages group file, don't gen errors.
         *  I'll handle them here.  This is used only after file has loaded ok
         *  and only for the messages STG.
         */
        static IStringTemplateErrorListener blankSTListener = new BlankSTListener();
        //static StringTemplateErrorListener blankSTListener =
        //    new StringTemplateErrorListener() {
        //        public void error(String s, Throwable e) {}
        //        public void warning(String s) {}
        //        public void debug(String s) {}
        //    };

        class DefaultSTListener : IStringTemplateErrorListener
        {
            public virtual void Error( String s, Exception e )
            {
                if ( e is TargetInvocationException )
                {
                    e = e.InnerException ?? e;
                }
                ErrorManager.error( ErrorManager.MSG_INTERNAL_ERROR, s, e );
            }
            public virtual void Warning( String s )
            {
                ErrorManager.warning( ErrorManager.MSG_INTERNAL_WARNING, s );
            }
            public virtual void Debug( String s )
            {
            }
        }

        /** Errors during initialization related to ST must all go to System.err.
         */
        static IStringTemplateErrorListener theDefaultSTListener = new DefaultSTListener();
        //static StringTemplateErrorListener theDefaultSTListener =
        //    new StringTemplateErrorListener() {
        //    public void error(String s, Throwable e) {
        //        if ( e instanceof InvocationTargetException ) {
        //            e = ((InvocationTargetException)e).getTargetException();
        //        }
        //        ErrorManager.error(ErrorManager.MSG_INTERNAL_ERROR, s, e);
        //    }
        //    public void warning(String s) {
        //        ErrorManager.warning(ErrorManager.MSG_INTERNAL_WARNING, s);
        //    }
        //    public void debug(String s) {
        //    }
        //};

        static ErrorManager()
        {
            ERRORS_FORCING_NO_ANALYSIS = new BitSet();
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_RULE_REDEFINITION );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_UNDEFINED_RULE_REF );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_LEFT_RECURSION_CYCLES );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_NO_RULES );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_NO_SUCH_GRAMMAR_SCOPE );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_NO_SUCH_RULE_IN_SCOPE );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_LEXER_RULES_NOT_ALLOWED );
            ERRORS_FORCING_NO_ANALYSIS.add( MSG_WILDCARD_AS_ROOT );
            // TODO: ...

            /** Do not do code gen if one of these happens */
            ERRORS_FORCING_NO_CODEGEN = new BitSet();
            ERRORS_FORCING_NO_CODEGEN.add( MSG_NONREGULAR_DECISION );
            ERRORS_FORCING_NO_CODEGEN.add( MSG_RECURSION_OVERLOW );
            ERRORS_FORCING_NO_CODEGEN.add( MSG_UNREACHABLE_ALTS );
            ERRORS_FORCING_NO_CODEGEN.add( MSG_FILE_AND_GRAMMAR_NAME_DIFFER );
            ERRORS_FORCING_NO_CODEGEN.add( MSG_INVALID_IMPORT );
            ERRORS_FORCING_NO_CODEGEN.add( MSG_AST_OP_WITH_NON_AST_OUTPUT_OPTION );
            // TODO: ...

            /** Only one error can be emitted for any entry in this table.
             *  Map<String,Set> where the key is a method name like danglingState.
             *  The set is whatever that method accepts or derives like a DFA.
             */
            emitSingleError = new Dictionary<string, ICollection<object>>();
            emitSingleError["danglingState"] = new HashSet<object>();

            initIdToMessageNameMapping();
            // it is inefficient to set the default locale here if another
            // piece of code is going to set the locale, but that would
            // require that a user call an init() function or something.  I prefer
            // that this class be ready to go when loaded as I'm absentminded ;)
            setLocale( CultureInfo.CurrentCulture );
            // try to load the message format group
            // the user might have specified one on the command line
            // if not, or if the user has given an illegal value, we will fall back to "antlr"
            setFormat( "antlr" );
        }

        public static IStringTemplateErrorListener getStringTemplateErrorListener()
        {
            return theDefaultSTListener;
        }

        /** We really only need a single locale for entire running ANTLR code
         *  in a single VM.  Only pay attention to the language, not the country
         *  so that French Canadians and French Frenchies all get the same
         *  template file, fr.stg.  Just easier this way.
         */
        public static void setLocale( CultureInfo locale )
        {
            ErrorManager.locale = locale;
            String language = locale.TwoLetterISOLanguageName;
            //String fileName = "org/antlr/tool/templates/messages/languages/"+language+".stg";
            string fileName = @"Tool\Templates\messages\languages\" + language + ".stg";
            string streamName = "Antlr3." + fileName.Replace( '\\', '.' );
            fileName = System.IO.Path.Combine( System.IO.Path.GetDirectoryName( typeof( ErrorManager ).Assembly.Location ), fileName );
            //ClassLoader cl = Thread.currentThread().getContextClassLoader();
            //InputStream @is = cl.getResourceAsStream(fileName);
            System.IO.Stream @is;
            if ( System.IO.File.Exists( fileName ) )
                @is = new System.IO.MemoryStream( System.IO.File.ReadAllBytes( fileName ) );
            else
                @is = typeof( ErrorManager ).Assembly.GetManifestResourceStream( streamName );
            //if ( @is==null ) {
            //    cl = typeof(ErrorManager).getClassLoader();
            //    @is = cl.getResourceAsStream(fileName);
            //}
            if ( @is==null && language.Equals(CultureInfo.GetCultureInfo("en-us").TwoLetterISOLanguageName) ) {
                rawError("ANTLR installation corrupted; cannot find English messages file "+fileName);
                panic();
            }
            else if ( @is==null ) {
                //rawError("no such locale file "+fileName+" retrying with English locale");
                setLocale(CultureInfo.GetCultureInfo("en-us")); // recurse on this rule, trying the US locale
                return;
            }
            StreamReader br = null;
            try {
                br = new StreamReader(new System.IO.BufferedStream( @is ) );
                messages = new StringTemplateGroup(br,
                                                   typeof(AngleBracketTemplateLexer),
                                                   initSTListener);
                br.Close();
            }
            catch (IOException ioe) {
                rawError("error reading message file "+fileName, ioe);
            }
            finally {
                if ( br!=null ) {
                    try {
                        br.Close();
                    }
                    catch (IOException ioe) {
                        rawError("cannot close message file "+fileName, ioe);
                    }
                }
            }

            messages.ErrorListener = blankSTListener;
            bool messagesOK = verifyMessages();
            if ( !messagesOK && language.Equals(CultureInfo.GetCultureInfo("en-us").TwoLetterISOLanguageName) ) {
                rawError("ANTLR installation corrupted; English messages file "+language+".stg incomplete");
                panic();
            }
            else if ( !messagesOK ) {
                setLocale(CultureInfo.GetCultureInfo("en-us")); // try US to see if that will work
            }
        }

        /** The format gets reset either from the Tool if the user supplied a command line option to that effect
         *  Otherwise we just use the default "antlr".
         */
        public static void setFormat( String formatName )
        {
            ErrorManager.formatName = formatName;
            //String fileName = "org/antlr/tool/templates/messages/formats/"+formatName+".stg";
            string fileName = @"Tool\Templates\messages\formats\" + formatName + ".stg";
            string streamName = "Antlr3." + fileName.Replace( '\\', '.' );
            fileName = System.IO.Path.Combine( System.IO.Path.GetDirectoryName( typeof( ErrorManager ).Assembly.Location ), fileName );
            //ClassLoader cl = Thread.currentThread().getContextClassLoader();
            //InputStream is = cl.getResourceAsStream(fileName);
            System.IO.Stream @is;
            if ( System.IO.File.Exists( fileName ) )
                @is = new System.IO.MemoryStream( System.IO.File.ReadAllBytes( fileName ) );
            else
                @is = typeof( ErrorManager ).Assembly.GetManifestResourceStream( streamName );
            //if ( is==null ) {
            //    cl = ErrorManager.class.getClassLoader();
            //    is = cl.getResourceAsStream(fileName);
            //}
            if ( @is == null && formatName.Equals( "antlr" ) )
            {
                rawError( "ANTLR installation corrupted; cannot find ANTLR messages format file " + fileName );
                panic();
            }
            else if ( @is == null )
            {
                rawError( "no such message format file " + fileName + " retrying with default ANTLR format" );
                setFormat( "antlr" ); // recurse on this rule, trying the default message format
                return;
            }
            StreamReader br = null;
            try
            {
                br = new StreamReader( new System.IO.BufferedStream( @is ) );
                format = new StringTemplateGroup( br,
                                                   typeof( AngleBracketTemplateLexer ),
                                                   initSTListener );
            }
            finally
            {
                try
                {
                    if ( br != null )
                    {
                        br.Close();
                    }
                }
                catch ( IOException ioe )
                {
                    rawError( "cannot close message format file " + fileName, ioe );
                }
            }

            format.ErrorListener = blankSTListener;
            bool formatOK = verifyFormat();
            if ( !formatOK && formatName.Equals( "antlr" ) )
            {
                rawError( "ANTLR installation corrupted; ANTLR messages format file " + formatName + ".stg incomplete" );
                panic();
            }
            else if ( !formatOK )
            {
                setFormat( "antlr" ); // recurse on this rule, trying the default message format
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
            if ( is==null && language.equals(Locale.US.getLanguage()) ) {
                return null;
            }
            else if ( is==null ) {
                return getLocaleForValidMessages(Locale.US); // recurse on this rule, trying the US locale
            }

            boolean messagesOK = verifyMessages();
            if ( !messagesOK && language.equals(Locale.US.getLanguage()) ) {
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
        public static void setErrorListener( IANTLRErrorListener listener )
        {
            threadToListenerMap[Thread.CurrentThread] = listener;
        }

        public static void removeErrorListener()
        {
            threadToListenerMap.Remove( Thread.CurrentThread );
        }

        public static void setTool( Tool tool )
        {
            threadToToolMap[Thread.CurrentThread] = tool;
        }

        /** Given a message ID, return a StringTemplate that somebody can fill
         *  with data.  We need to convert the int ID to the name of a template
         *  in the messages ST group.
         */
        public static StringTemplate getMessage( int msgID )
        {
            String msgName = idToMessageTemplateName[msgID];
            return messages.GetInstanceOf( msgName );
        }
        public static String getMessageType( int msgID )
        {
            if ( getErrorState().warningMsgIDs.member( msgID ) )
            {
                return messages.GetInstanceOf( "warning" ).ToString();
            }
            else if ( getErrorState().errorMsgIDs.member( msgID ) )
            {
                return messages.GetInstanceOf( "error" ).ToString();
            }
            assertTrue( false, "Assertion failed! Message ID " + msgID + " created but is not present in errorMsgIDs or warningMsgIDs." );
            return "";
        }

        /** Return a StringTemplate that refers to the current format used for
         * emitting messages.
         */
        public static StringTemplate getLocationFormat()
        {
            return format.GetInstanceOf( "location" );
        }
        public static StringTemplate getReportFormat()
        {
            return format.GetInstanceOf( "report" );
        }
        public static StringTemplate getMessageFormat()
        {
            return format.GetInstanceOf( "message" );
        }
        public static bool formatWantsSingleLineMessage()
        {
            return format.GetInstanceOf( "wantsSingleLineMessage" ).ToString().Equals( "true" );
        }

        public static IANTLRErrorListener getErrorListener()
        {
            IANTLRErrorListener el =
                (IANTLRErrorListener)threadToListenerMap.get( Thread.CurrentThread );
            if ( el == null )
            {
                return theDefaultErrorListener;
            }
            return el;
        }

        public static ErrorState getErrorState()
        {
            ErrorState ec =
                (ErrorState)threadToErrorStateMap.get( Thread.CurrentThread );
            if ( ec == null )
            {
                ec = new ErrorState();
                threadToErrorStateMap[Thread.CurrentThread] = ec;
            }
            return ec;
        }

        public static int getNumErrors()
        {
            return getErrorState().errors;
        }

        public static void resetErrorState()
        {
            threadToListenerMap = new Dictionary<Thread, IANTLRErrorListener>();
            ErrorState ec = new ErrorState();
            threadToErrorStateMap[Thread.CurrentThread] = ec;
        }

        public static void info( String msg )
        {
            getErrorState().infos++;
            getErrorListener().info( msg );
        }

        public static void error( int msgID )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( new ToolMessage( msgID ) );
        }

        public static void error( int msgID, Exception e )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( new ToolMessage( msgID, e ) );
        }

        public static void error( int msgID, Object arg )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( new ToolMessage( msgID, arg ) );
        }

        public static void error( int msgID, Object arg, Object arg2 )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( new ToolMessage( msgID, arg, arg2 ) );
        }

        public static void error( int msgID, Object arg, Exception e )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( new ToolMessage( msgID, arg, e ) );
        }

        public static void warning( int msgID, Object arg )
        {
            getErrorState().warnings++;
            getErrorState().warningMsgIDs.add( msgID );
            getErrorListener().warning( new ToolMessage( msgID, arg ) );
        }

        public static void nondeterminism( DecisionProbe probe,
                                          DFAState d )
        {
            getErrorState().warnings++;
            Message msg = new GrammarNonDeterminismMessage( probe, d );
            getErrorState().warningMsgIDs.add( msg.msgID );
            getErrorListener().warning( msg );
        }

        public static void danglingState( DecisionProbe probe,
                                         DFAState d )
        {
            getErrorState().errors++;
            Message msg = new GrammarDanglingStateMessage( probe, d );
            getErrorState().errorMsgIDs.add( msg.msgID );
            ICollection<object> seen = (ICollection<object>)emitSingleError.get( "danglingState" );
            if ( !seen.Contains( d.dfa.decisionNumber + "|" + d.AltSet ) )
            {
                getErrorListener().error( msg );
                // we've seen this decision and this alt set; never again
                seen.Add( d.dfa.decisionNumber + "|" + d.AltSet );
            }
        }

        public static void analysisAborted( DecisionProbe probe )
        {
            getErrorState().warnings++;
            Message msg = new GrammarAnalysisAbortedMessage( probe );
            getErrorState().warningMsgIDs.add( msg.msgID );
            getErrorListener().warning( msg );
        }

        public static void unreachableAlts( DecisionProbe probe,
                                           IEnumerable<int> alts )
        {
            getErrorState().errors++;
            Message msg = new GrammarUnreachableAltsMessage( probe, alts );
            getErrorState().errorMsgIDs.add( msg.msgID );
            getErrorListener().error( msg );
        }

        public static void insufficientPredicates( DecisionProbe probe,
                                                  DFAState d,
                                                  IDictionary<int, ICollection<IToken>> altToUncoveredLocations )
        {
            getErrorState().warnings++;
            Message msg = new GrammarInsufficientPredicatesMessage( probe, d, altToUncoveredLocations );
            getErrorState().warningMsgIDs.add( msg.msgID );
            getErrorListener().warning( msg );
        }

        public static void nonLLStarDecision( DecisionProbe probe )
        {
            getErrorState().errors++;
            Message msg = new NonRegularDecisionMessage( probe, probe.NonDeterministicAlts );
            getErrorState().errorMsgIDs.add( msg.msgID );
            getErrorListener().error( msg );
        }

        public static void recursionOverflow( DecisionProbe probe,
                                             DFAState sampleBadState,
                                             int alt,
                                             ICollection<string> targetRules,
                                             ICollection<ICollection<NFAState>> callSiteStates )
        {
            getErrorState().errors++;
            Message msg = new RecursionOverflowMessage( probe, sampleBadState, alt,
                                             targetRules, callSiteStates );
            getErrorState().errorMsgIDs.add( msg.msgID );
            getErrorListener().error( msg );
        }

        /*
        // TODO: we can remove I think.  All detected now with cycles check.
        public static void leftRecursion(DecisionProbe probe,
                                         int alt,
                                         Collection targetRules,
                                         Collection callSiteStates)
        {
            getErrorState().warnings++;
            Message msg = new LeftRecursionMessage(probe, alt, targetRules, callSiteStates);
            getErrorState().warningMsgIDs.add(msg.msgID);
            getErrorListener().warning(msg);
        }
        */

        public static void leftRecursionCycles( ICollection cycles )
        {
            getErrorState().errors++;
            Message msg = new LeftRecursionCyclesMessage( cycles );
            getErrorState().errorMsgIDs.add( msg.msgID );
            getErrorListener().warning( msg );
        }

        public static void grammarError( int msgID,
                                        Grammar g,
                                        IToken token,
                                        Object arg,
                                        Object arg2 )
        {
            getErrorState().errors++;
            Message msg = new GrammarSemanticsMessage( msgID, g, token, arg, arg2 );
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error( msg );
        }

        public static void grammarError( int msgID,
                                        Grammar g,
                                        IToken token,
                                        Object arg )
        {
            grammarError( msgID, g, token, arg, null );
        }

        public static void grammarError( int msgID,
                                        Grammar g,
                                        IToken token )
        {
            grammarError( msgID, g, token, null, null );
        }

        public static void grammarWarning( int msgID,
                                          Grammar g,
                                          IToken token,
                                          Object arg,
                                          Object arg2 )
        {
            getErrorState().warnings++;
            Message msg = new GrammarSemanticsMessage( msgID, g, token, arg, arg2 );
            getErrorState().warningMsgIDs.add( msgID );
            getErrorListener().warning( msg );
        }

        public static void grammarWarning( int msgID,
                                          Grammar g,
                                          IToken token,
                                          Object arg )
        {
            grammarWarning( msgID, g, token, arg, null );
        }

        public static void grammarWarning( int msgID,
                                          Grammar g,
                                          IToken token )
        {
            grammarWarning( msgID, g, token, null, null );
        }

        public static void syntaxError( int msgID,
                                       Grammar grammar,
                                       IToken token,
                                       Object arg,
                                       RecognitionException re )
        {
            getErrorState().errors++;
            getErrorState().errorMsgIDs.add( msgID );
            getErrorListener().error(
                new GrammarSyntaxMessage( msgID, grammar, token, arg, re )
            );
        }

        public static void internalError( Object error, Exception e )
        {
            StackFrame location = getLastNonErrorManagerCodeLocation( e );
            String msg = "Exception " + e + "@" + location + ": " + error;
            ErrorManager.error( MSG_INTERNAL_ERROR, msg );
        }

        public static void internalError( Object error )
        {
            StackFrame location =
                getLastNonErrorManagerCodeLocation( new Exception() );
            String msg = location + ": " + error;
            ErrorManager.error( MSG_INTERNAL_ERROR, msg );
        }

        public static bool doNotAttemptAnalysis()
        {
            return !getErrorState().errorMsgIDs.and( ERRORS_FORCING_NO_ANALYSIS ).isNil();
        }

        public static bool doNotAttemptCodeGen()
        {
            return doNotAttemptAnalysis() ||
                   !getErrorState().errorMsgIDs.and( ERRORS_FORCING_NO_CODEGEN ).isNil();
        }

        /** Return first non ErrorManager code location for generating messages */
        private static StackFrame getLastNonErrorManagerCodeLocation( Exception e )
        {
            StackFrame[] stack = e.getStackTrace();
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

        public static void assertTrue( bool condition, String message )
        {
            if ( !condition )
            {
                internalError( message );
            }
        }

        // S U P P O R T  C O D E

        static bool initIdToMessageNameMapping()
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
        static bool verifyMessages()
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
        static bool verifyFormat()
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
        static void rawError( String msg )
        {
            Console.Error.WriteLine( msg );
        }

        static void rawError( String msg, Exception e )
        {
            rawError( msg );
            e.PrintStackTrace( Console.Error );
        }

        /** I *think* this will allow Tool subclasses to exit gracefully
         *  for GUIs etc...
         */
        public static void panic()
        {
            Tool tool = (Tool)threadToToolMap.get( Thread.CurrentThread );
            if ( tool == null )
            {
                // no tool registered, exit
                throw new java.lang.Error( "ANTLR ErrorManager panic" );
            }
            else
            {
                tool.panic();
            }
        }
    }
}
