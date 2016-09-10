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

namespace Antlr3.Analysis
{
    using ArgumentNullException = System.ArgumentNullException;
    using StringBuilder = System.Text.StringBuilder;

    /** An NFA state, predicted alt, and syntactic/semantic context.
     *  The syntactic context is a pointer into the rule invocation
     *  chain used to arrive at the state.  The semantic context is
     *  the unordered set semantic predicates encountered before reaching
     *  an NFA state.
     */
    public class NFAConfiguration : System.IEquatable<NFAConfiguration>
    {
        /** The NFA state associated with this configuration */
        private readonly int _state;

        /** What alt is predicted by this configuration */
        private readonly int _alt;

        /** What is the stack of rule invocations that got us to state? */
        private readonly NFAContext _context;

        /** The set of semantic predicates associated with this NFA
         *  configuration.  The predicates were found on the way to
         *  the associated NFA state in this syntactic context.
         *  Set<AST>: track nodes in grammar containing the predicate
         *  for error messages and such (nice to know where the predicate
         *  came from in case of duplicates etc...).  By using a set,
         *  the equals() method will correctly show {pred1,pred2} as equals()
         *  to {pred2,pred1}.
         */
        private SemanticContext _semanticContext = SemanticContext.EmptySemanticContext;

        /** Indicate that this configuration has been resolved and no further
         *  DFA processing should occur with it.  Essentially, this is used
         *  as an "ignore" bit so that upon a set of nondeterministic configurations
         *  such as (s|2) and (s|3), I can set (s|3) to resolved=true (and any
         *  other configuration associated with alt 3).
         */
        private bool _resolved;

        /** This bit is used to indicate a semantic predicate will be
         *  used to resolve the conflict.  Method
         *  DFA.findNewDFAStatesAndAddDFATransitions will add edges for
         *  the predicates after it performs the reach operation.  The
         *  nondeterminism resolver sets this when it finds a set of
         *  nondeterministic configurations (as it does for "resolved" field)
         *  that have enough predicates to resolve the conflit.
         */
        private bool _resolveWithPredicate;

        ///// <summary>
        ///// Lots of NFA states have only epsilon edges (1 or 2).  We can
        ///// safely consider only n>0 during closure.
        ///// </summary>
        //int _numberEpsilonTransitionsEmanatingFromState;

        /** Indicates that the NFA state associated with this configuration
         *  has exactly one transition and it's an atom (not epsilon etc...).
         */
        private bool _singleAtomTransitionEmanating;

        //protected boolean addedDuringClosure = true;

        public NFAConfiguration( int state,
                                int alt,
                                NFAContext context,
                                SemanticContext semanticContext )
        {
            this._state = state;
            this._alt = alt;
            this._context = context;
            this._semanticContext = semanticContext;
        }

        public int State
        {
            get
            {
                return _state;
            }
        }

        public int Alt
        {
            get
            {
                return _alt;
            }
        }

        public NFAContext Context
        {
            get
            {
                return _context;
            }
        }

        public SemanticContext SemanticContext
        {
            get
            {
                return _semanticContext;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _semanticContext = value;
            }
        }

        protected internal bool Resolved
        {
            get
            {
                return _resolved;
            }

            set
            {
                _resolved = value;
            }
        }

        protected internal bool ResolveWithPredicate
        {
            get
            {
                return _resolveWithPredicate;
            }

            set
            {
                _resolveWithPredicate = value;
            }
        }

        protected internal bool SingleAtomTransitionEmanating
        {
            get
            {
                return _singleAtomTransitionEmanating;
            }

            set
            {
                _singleAtomTransitionEmanating = value;
            }
        }

        /** An NFA configuration is equal to another if both have
         *  the same state, the predict the same alternative, and
         *  syntactic/semantic contexts are the same.  I don't think
         *  the state|alt|ctx could be the same and have two different
         *  semantic contexts, but might as well define equals to be
         *  everything.
         */
        public override bool Equals(object obj)
        {
            NFAConfiguration other = obj as NFAConfiguration;
            if (other == null)
                return false;

            return this.Equals(other);
        }

        public bool Equals(NFAConfiguration other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            return this._state == other._state &&
                   this._alt == other._alt &&
                   this._context.Equals(other._context) &&
                   this._semanticContext.Equals(other._semanticContext);
        }

        public override int GetHashCode()
        {
            int h = _state ^ _alt ^ _context.GetHashCode();
            return h;
        }

        public override string ToString()
        {
            return ToString( true );
        }

        public string ToString( bool showAlt )
        {
            StringBuilder buf = new StringBuilder();
            buf.Append( _state );
            if ( showAlt )
            {
                buf.Append( "|" );
                buf.Append( _alt );
            }

            if ( _context.Parent != null )
            {
                buf.Append( "|" );
                buf.Append( _context );
            }

            if ( _semanticContext != null &&
                 _semanticContext != SemanticContext.EmptySemanticContext )
            {
                buf.Append( "|" );
                string escQuote = _semanticContext.ToString().Replace( "\"", "\\\"" );
                buf.Append( escQuote );
            }

            if ( _resolved )
            {
                buf.Append( "|resolved" );
            }

            if ( _resolveWithPredicate )
            {
                buf.Append( "|resolveWithPredicate" );
            }

            return buf.ToString();
        }
    }
}
