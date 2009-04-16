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

namespace Antlr3.Analysis
{
    using Barrier = Antlr3.Misc.Barrier;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using OperationCanceledException = System.OperationCanceledException;

    /** Convert all decisions i..j inclusive in a thread */
    public class NFAConversionThread //: Runnable
    {
        Grammar grammar;
        int i, j;
        Barrier barrier;
        public NFAConversionThread( Grammar grammar,
                                   Barrier barrier,
                                   int i,
                                   int j )
        {
            this.grammar = grammar;
            this.barrier = barrier;
            this.i = i;
            this.j = j;
        }
        public virtual void Run()
        {
            for ( int decision = i; decision <= j; decision++ )
            {
                NFAState decisionStartState = grammar.getDecisionNFAStartState( decision );
                if ( decisionStartState.NumberOfTransitions > 1 )
                {
                    grammar.createLookaheadDFA( decision, true );
                }
            }
            // now wait for others to finish
            try
            {
                barrier.WaitForRelease();
            }
            catch ( OperationCanceledException e )
            {
                ErrorManager.internalError( "what the hell? DFA interruptus", e );
            }
        }
    }

}
