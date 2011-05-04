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
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime;
    using Antlr3.Analysis;

    using BlankDebugEventListener = Antlr.Runtime.Debug.BlankDebugEventListener;
    using Console = System.Console;
    using DFA = Antlr3.Analysis.DFA;
    using IDebugEventListener = Antlr.Runtime.Debug.IDebugEventListener;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using ParseTree = Antlr.Runtime.Tree.ParseTree;
    using ParseTreeBuilder = Antlr.Runtime.Debug.ParseTreeBuilder;

    /** The recognition interpreter/engine for grammars.  Separated
     *  out of Grammar as it's related, but technically not a Grammar function.
     *  You create an interpreter for a grammar and an input stream.  This object
     *  can act as a TokenSource so that you can hook up two grammars (via
     *  a CommonTokenStream) to lex/parse.  Being a token source only makes sense
     *  for a lexer grammar of course.
     */
    public class Interpreter : ITokenSource
    {
        protected Grammar grammar;
        protected IIntStream input;

        /** A lexer listener that just creates token objects as they
         *  are matched.  scan() use this listener to get a single object.
         *  To get a stream of tokens, you must call scan() multiple times,
         *  recording the token object result after each call.
         */
        private class LexerActionGetTokenType : BlankDebugEventListener
        {
            Interpreter outer;
            public CommonToken token;
            Grammar g;

            public LexerActionGetTokenType( Interpreter outer, Grammar g )
            {
                this.outer = outer;
                this.g = g;
            }

            public override void ExitRule( string grammarFileName, string ruleName )
            {
                if ( !ruleName.Equals( Grammar.ArtificialTokensRuleName ) )
                {
                    int type = g.GetTokenType( ruleName );
                    int channel = TokenChannels.Default;
                    token = new CommonToken( (ICharStream)outer.input, type, channel, 0, 0 );
                }
            }
        }

        public Interpreter( Grammar grammar, IIntStream input )
        {
            this.grammar = grammar;
            this.input = input;
        }

        public string[] TokenNames
        {
            get
            {
                return grammar.TokenIDs.ToArray();
            }
        }

        public virtual IToken NextToken()
        {
            if ( grammar.type != GrammarType.Lexer )
            {
                return null;
            }
            if ( input.LA( 1 ) == CharStreamConstants.EndOfFile )
            {
                return new CommonToken((ICharStream)input, CharStreamConstants.EndOfFile, TokenChannels.Default, input.Index, input.Index);
            }
            int start = input.Index;
            int charPos = ( (ICharStream)input ).CharPositionInLine;
            CommonToken token = null;
            while ( input.LA( 1 ) != CharStreamConstants.EndOfFile )
            {
                try
                {
                    token = Scan( Grammar.ArtificialTokensRuleName, null );
                    break;
                }
                catch ( RecognitionException re )
                {
                    // report a problem and try for another
                    ReportScanError( re );
                    continue;
                }
            }
            // the scan can only set type
            // we must set the line, and other junk here to make it a complete token
            int stop = input.Index - 1;
            if ( token == null )
            {
                return new CommonToken((ICharStream)input, CharStreamConstants.EndOfFile, TokenChannels.Default, start, start);
            }
            token.Line = ( ( (ICharStream)input ).Line );
            token.StartIndex = start;
            token.StopIndex = stop;
            token.CharPositionInLine = charPos;
            return token;
        }

        /** For a given input char stream, try to match against the NFA
         *  starting at startRule.  This is a deterministic parse even though
         *  it is using an NFA because it uses DFAs at each decision point to
         *  predict which alternative will succeed.  This is exactly what the
         *  generated parser will do.
         *
         *  This only does lexer grammars.
         *
         *  Return the token type associated with the final rule end state.
         */
        public virtual void Scan( string startRule,
                         IDebugEventListener actions,
                         IList<NFAState> visitedStates )
        {
            if ( grammar.type != GrammarType.Lexer )
            {
                return;
            }
            ICharStream @in = (ICharStream)this.input;
            //Console.Out.WriteLine( "scan(" + startRule + ",'" + @in.substring( @in.Index, @in.Size() - 1 ) + "')" );
            // Build NFAs/DFAs from the grammar AST if NFAs haven't been built yet
            if ( grammar.GetRuleStartState( startRule ) == null )
            {
                grammar.BuildNFA();
            }

            if ( !grammar.AllDecisionDFAHaveBeenCreated )
            {
                // Create the DFA predictors for each decision
                grammar.CreateLookaheadDFAs();
            }

            // do the parse
            Stack<object> ruleInvocationStack = new Stack<object>();
            NFAState start = grammar.GetRuleStartState( startRule );
            NFAState stop = grammar.GetRuleStopState( startRule );
            ParseEngine( startRule, start, stop, @in, ruleInvocationStack,
                        actions, visitedStates );
        }

        public virtual CommonToken Scan( string startRule )
        {
            return Scan( startRule, null );
        }

        public virtual CommonToken Scan( string startRule,
                                IList<NFAState> visitedStates )
        {
            LexerActionGetTokenType actions = new LexerActionGetTokenType( this, grammar );
            Scan( startRule, actions, visitedStates );
            return actions.token;
        }

        public virtual void Parse( string startRule,
                          IDebugEventListener actions,
                          IList<NFAState> visitedStates )
        {
            //Console.Out.WriteLine( "parse(" + startRule + ")" );
            // Build NFAs/DFAs from the grammar AST if NFAs haven't been built yet
            if ( grammar.GetRuleStartState( startRule ) == null )
            {
                grammar.BuildNFA();
            }
            if ( !grammar.AllDecisionDFAHaveBeenCreated )
            {
                // Create the DFA predictors for each decision
                grammar.CreateLookaheadDFAs();
            }
            // do the parse
            Stack<object> ruleInvocationStack = new Stack<object>();
            NFAState start = grammar.GetRuleStartState( startRule );
            NFAState stop = grammar.GetRuleStopState( startRule );
            ParseEngine( startRule, start, stop, input, ruleInvocationStack,
                        actions, visitedStates );
        }

        public virtual ParseTree Parse( string startRule )
        {
            return Parse( startRule, null );
        }

        public virtual ParseTree Parse( string startRule, IList<NFAState> visitedStates )
        {
            ParseTreeBuilder actions = new ParseTreeBuilder( grammar.name );
            try
            {
                Parse( startRule, actions, visitedStates );
            }
            catch ( RecognitionException /*re*/ )
            {
                // Errors are tracked via the ANTLRDebugInterface
                // Exceptions are used just to blast out of the parse engine
                // The error will be in the parse tree.
            }
            return actions.Tree;
        }

        /** Fill a list of all NFA states visited during the parse */
        protected virtual void ParseEngine( string startRule,
                                   NFAState start,
                                   NFAState stop,
                                   IIntStream input,
                                   Stack<object> ruleInvocationStack,
                                   IDebugEventListener actions,
                                   IList<NFAState> visitedStates )
        {
            NFAState s = start;
            if ( actions != null )
            {
                actions.EnterRule( s.nfa.Grammar.FileName, start.enclosingRule.Name );
            }
            int t = input.LA( 1 );
            while ( s != stop )
            {
                if ( visitedStates != null )
                {
                    visitedStates.Add( s );
                }
                //Console.Out.WriteLine( "parse state " + s.stateNumber + " input=" + s.nfa.Grammar.getTokenDisplayName( t ) );
                // CASE 1: decision state
                if ( s.DecisionNumber > 0 && s.nfa.Grammar.GetNumberOfAltsForDecisionNFA( s ) > 1 )
                {
                    // decision point, must predict and jump to alt
                    DFA dfa = s.nfa.Grammar.GetLookaheadDFA( s.DecisionNumber );
                    //if ( s.nfa.Grammar.type != GrammarType.Lexer )
                    //{
                    //    Console.Out.WriteLine( "decision: " +
                    //                   dfa.getNFADecisionStartState().Description +
                    //                   " input=" + s.nfa.Grammar.getTokenDisplayName( t ) );
                    //}
                    int m = input.Mark();
                    int predictedAlt = Predict( dfa );
                    if ( predictedAlt == NFA.INVALID_ALT_NUMBER )
                    {
                        string description = dfa.NFADecisionStartState.Description;
                        NoViableAltException nvae =
                            new NoViableAltException( description,
                                                          dfa.NfaStartStateDecisionNumber,
                                                          s.StateNumber,
                                                          input );
                        if ( actions != null )
                        {
                            actions.RecognitionException( nvae );
                        }
                        input.Consume(); // recover
                        throw nvae;
                    }
                    input.Rewind( m );
                    int parseAlt =
                        s.TranslateDisplayAltToWalkAlt( predictedAlt );
                    //if ( s.nfa.Grammar.type != GrammarType.Lexer )
                    //{
                    //    Console.Out.WriteLine( "predicted alt " + predictedAlt + ", parseAlt " + parseAlt );
                    //}
                    NFAState alt;
                    if ( parseAlt > s.nfa.Grammar.GetNumberOfAltsForDecisionNFA( s ) )
                    {
                        // implied branch of loop etc...
                        alt = s.nfa.Grammar.nfa.GetState( s.endOfBlockStateNumber );
                    }
                    else
                    {
                        alt = s.nfa.Grammar.GetNFAStateForAltOfDecision( s, parseAlt );
                    }
                    s = (NFAState)alt.transition[0].Target;
                    continue;
                }

                // CASE 2: finished matching a rule
                if ( s.IsAcceptState )
                { // end of rule node
                    if ( actions != null )
                    {
                        actions.ExitRule( s.nfa.Grammar.FileName, s.enclosingRule.Name );
                    }
                    if ( ruleInvocationStack.Count == 0 )
                    {
                        // done parsing.  Hit the start state.
                        //Console.Out.WriteLine( "stack empty in stop state for " + s.enclosingRule );
                        break;
                    }
                    // pop invoking state off the stack to know where to return to
                    NFAState invokingState = (NFAState)ruleInvocationStack.Pop();
                    RuleClosureTransition invokingTransition =
                            (RuleClosureTransition)invokingState.transition[0];
                    // move to node after state that invoked this rule
                    s = invokingTransition.FollowState;
                    continue;
                }

                Transition trans = s.transition[0];
                Label label = trans.Label;
                if ( label.IsSemanticPredicate )
                {
                    FailedPredicateException fpe =
                        new FailedPredicateException( input,
                                                     s.enclosingRule.Name,
                                                     "can't deal with predicates yet" );
                    if ( actions != null )
                    {
                        actions.RecognitionException( fpe );
                    }
                }

                // CASE 3: epsilon transition
                if ( label.IsEpsilon )
                {
                    // CASE 3a: rule invocation state
                    if ( trans is RuleClosureTransition )
                    {
                        ruleInvocationStack.Push( s );
                        s = (NFAState)trans.Target;
                        //Console.Out.WriteLine( "call " + s.enclosingRule.name + " from " + s.nfa.Grammar.getFileName() );
                        if ( actions != null )
                        {
                            actions.EnterRule( s.nfa.Grammar.FileName, s.enclosingRule.Name );
                        }
                        // could be jumping to new grammar, make sure DFA created
                        if ( !s.nfa.Grammar.AllDecisionDFAHaveBeenCreated )
                        {
                            s.nfa.Grammar.CreateLookaheadDFAs();
                        }
                    }
                    // CASE 3b: plain old epsilon transition, just move
                    else
                    {
                        s = (NFAState)trans.Target;
                    }
                }

                // CASE 4: match label on transition
                else if ( label.Matches( t ) )
                {
                    if ( actions != null )
                    {
                        if ( s.nfa.Grammar.type == GrammarType.Parser ||
                             s.nfa.Grammar.type == GrammarType.Combined )
                        {
                            actions.ConsumeToken( ( (ITokenStream)input ).LT( 1 ) );
                        }
                    }
                    s = (NFAState)s.transition[0].Target;
                    input.Consume();
                    t = input.LA( 1 );
                }

                // CASE 5: error condition; label is inconsistent with input
                else
                {
                    if ( label.IsAtom )
                    {
                        MismatchedTokenException mte =
                            new MismatchedTokenException( label.Atom, input );
                        if ( actions != null )
                        {
                            actions.RecognitionException( mte );
                        }
                        input.Consume(); // recover
                        throw mte;
                    }
                    else if ( label.IsSet )
                    {
                        MismatchedSetException mse =
                            new MismatchedSetException( ( (IntervalSet)label.Set ).ToRuntimeBitSet(),
                                                       input );
                        if ( actions != null )
                        {
                            actions.RecognitionException( mse );
                        }
                        input.Consume(); // recover
                        throw mse;
                    }
                    else if ( label.IsSemanticPredicate )
                    {
                        FailedPredicateException fpe =
                            new FailedPredicateException( input,
                                                         s.enclosingRule.Name,
                                                         label.SemanticContext.ToString() );
                        if ( actions != null )
                        {
                            actions.RecognitionException( fpe );
                        }
                        input.Consume(); // recover
                        throw fpe;
                    }
                    else
                    {
                        throw new RecognitionException( input ); // unknown error
                    }
                }
            }
            //Console.Out.WriteLine( "hit stop state for " + stop.enclosingRule );
            if ( actions != null )
            {
                actions.ExitRule( s.nfa.Grammar.FileName, stop.enclosingRule.Name );
            }
        }

        /** Given an input stream, return the unique alternative predicted by
         *  matching the input.  Upon error, return NFA.INVALID_ALT_NUMBER
         *  The first symbol of lookahead is presumed to be primed; that is,
         *  input.lookahead(1) must point at the input symbol you want to start
         *  predicting with.
         */
        public int Predict( DFA dfa )
        {
            DFAState s = dfa.StartState;
            int c = input.LA( 1 );
            Transition eotTransition = null;
        dfaLoop:
            while ( !s.IsAcceptState )
            {
                //Console.Out.WriteLine( "DFA.predict(" + s.stateNumber + ", " + dfa.nfa.Grammar.getTokenDisplayName( c ) + ")" );
                // for each edge of s, look for intersection with current char
                for ( int i = 0; i < s.NumberOfTransitions; i++ )
                {
                    Transition t = s.GetTransition( i );
                    // special case: EOT matches any char
                    if ( t.Label.Matches( c ) )
                    {
                        // take transition i
                        s = (DFAState)t.Target;
                        input.Consume();
                        c = input.LA( 1 );
                        goto dfaLoop;
                    }
                    if ( t.Label.Atom == Label.EOT )
                    {
                        eotTransition = t;
                    }
                }
                if ( eotTransition != null )
                {
                    s = (DFAState)eotTransition.Target;
                    goto dfaLoop;
                }
                /*
                ErrorManager.error(ErrorManager.MSG_NO_VIABLE_DFA_ALT,
                                   s,
                                   dfa.nfa.Grammar.getTokenName(c));
                */
                return NFA.INVALID_ALT_NUMBER;
            }
            // woohoo!  We know which alt to predict
            // nothing emanates from a stop state; must terminate anyway
            //Console.Out.WriteLine( "DFA stop state " + s.stateNumber + " predicts " + s.getUniquelyPredictedAlt() );
            return s.GetUniquelyPredictedAlt();
        }

        public virtual void ReportScanError( RecognitionException re )
        {
            ICharStream cs = (ICharStream)input;
            // print as good of a message as we can, given that we do not have
            // a Lexer object and, hence, cannot call the routine to get a
            // decent error message.
            Console.Error.WriteLine( "problem matching token at " +
                cs.Line + ":" + cs.CharPositionInLine + " " + re );
        }

        public virtual string SourceName
        {
            get
            {
                return input.SourceName;
            }
        }

    }
}
