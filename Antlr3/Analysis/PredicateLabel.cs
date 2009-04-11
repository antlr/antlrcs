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

namespace Antlr3.Analysis
{
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;

    public class PredicateLabel : Label
    {
        /** A tree of semantic predicates from the grammar AST if label==SEMPRED.
         *  In the NFA, labels will always be exactly one predicate, but the DFA
         *  may have to combine a bunch of them as it collects predicates from
         *  multiple NFA configurations into a single DFA state.
         */
        SemanticContext _semanticContext;

        /** Make a semantic predicate label */
        public PredicateLabel( GrammarAST predicateASTNode )
            : base( SEMPRED )
        {
            this._semanticContext = new SemanticContext.Predicate( predicateASTNode );
        }

        /** Make a semantic predicates label */
        public PredicateLabel( SemanticContext semCtx )
            : base( SEMPRED )
        {
            this._semanticContext = semCtx;
        }

        public override int GetHashCode()
        {
            return _semanticContext.GetHashCode();
        }

        public override bool Equals( object o )
        {
            if ( o == null )
            {
                return false;
            }
            if ( object.ReferenceEquals( this, o ) )
            {
                return true; // equals if same object
            }
            PredicateLabel pl = o as PredicateLabel;
            if ( pl == null )
            {
                return false;
            }
            return _semanticContext.Equals( pl.SemanticContext );
        }

        public override bool IsSemanticPredicate
        {
            get
            {
                return true;
            }
        }

        public override SemanticContext SemanticContext
        {
            get
            {
                return _semanticContext;
            }
        }

        public override string ToString()
        {
            return "{" + _semanticContext + "}?";
        }

        public override string ToString( Grammar g )
        {
            return ToString();
        }
    }

}
