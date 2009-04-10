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
    using Grammar = Antlr3.Tool.Grammar;
    using ICloneable = System.ICloneable;
    using IComparable = System.IComparable;
    using IIntSet = Antlr3.Misc.IIntSet;
    using InvalidOperationException = System.InvalidOperationException;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using TokenConstants = Antlr.Runtime.TokenConstants;

    /** A state machine transition label.  A label can be either a simple
     *  label such as a token or character.  A label can be a set of char or
     *  tokens.  It can be an epsilon transition.  It can be a semantic predicate
     *  (which assumes an epsilon transition) or a tree of predicates (in a DFA).
     */
    public class Label : IComparable, ICloneable
    {
        public const int INVALID = -7;

        public const int ACTION = -6;

        public const int EPSILON = -5;

        public const string EPSILON_STR = "<EPSILON>";

        /** label is a semantic predicate; implies label is epsilon also */
        public const int SEMPRED = -4;

        /** label is a set of tokens or char */
        public const int SET = -3;

        /** End of Token is like EOF for lexer rules.  It implies that no more
         *  characters are available and that NFA conversion should terminate
         *  for this path.  For example
         *
         *  A : 'a' 'b' | 'a' ;
         *
         *  yields a DFA predictor:
         *
         *  o-a->o-b->1   predict alt 1
         *       |
         *       |-EOT->o predict alt 2
         *
         *  To generate code for EOT, treat it as the "default" path, which
         *  implies there is no way to mismatch a char for the state from
         *  which the EOT emanates.
         */
        public const int EOT = -2;

        public const int EOF = -1;

        /** We have labels like EPSILON that are below 0; it's hard to
         *  store them in an array with negative index so use this
         *  constant as an index shift when accessing arrays based upon
         *  token type.  If real token type is i, then array index would be
         *  NUM_FAUX_LABELS + i.
         */
        public const int NUM_FAUX_LABELS = -INVALID;

        /** Anything at this value or larger can be considered a simple atom int
         *  for easy comparison during analysis only; faux labels are not used
         *  during parse time for real token types or char values.
         */
        public const int MIN_ATOM_VALUE = EOT;

        // TODO: is 0 a valid unicode char? max is FFFF -1, right?
        public const int MIN_CHAR_VALUE = '\u0000';
        public const int MAX_CHAR_VALUE = '\uFFFF';

        /** End of rule token type; imaginary token type used only for
         *  local, partial FOLLOW sets to indicate that the local FOLLOW
         *  hit the end of rule.  During error recovery, the local FOLLOW
         *  of a token reference may go beyond the end of the rule and have
         *  to use FOLLOW(rule).  I have to just shift the token types to 2..n
         *  rather than 1..n to accommodate this imaginary token in my bitsets.
         *  If I didn't use a bitset implementation for runtime sets, I wouldn't
         *  need this.  EOF is another candidate for a run time token type for
         *  parsers.  Follow sets are not computed for lexers so we do not have
         *  this issue.
         */
        public const int EOR_TOKEN_TYPE =
            TokenConstants.EorTokenType;

        public const int DOWN = TokenConstants.Down;
        public const int UP = TokenConstants.Up;

        /** tokens and char range overlap; tokens are MIN_TOKEN_TYPE..n */
        public const int MIN_TOKEN_TYPE =
            TokenConstants.MinTokenType;

        /** The wildcard '.' char atom implies all valid characters==UNICODE */
        //public static final IntSet ALLCHAR = IntervalSet.of(MIN_CHAR_VALUE,MAX_CHAR_VALUE);

        /** The token type or character value; or, signifies special label. */
        protected internal int label;

        /** A set of token types or character codes if label==SET */
        // TODO: try IntervalSet for everything
        protected IIntSet labelSet;

        public Label( int label )
        {
            this.label = label;
        }

        /** Make a set label */
        public Label( IIntSet labelSet )
        {
            if ( labelSet == null )
            {
                this.label = SET;
                this.labelSet = IntervalSet.of( INVALID );
                return;
            }
            int singleAtom = labelSet.getSingleElement();
            if ( singleAtom != INVALID )
            {
                // convert back to a single atomic element if |labelSet|==1
                label = singleAtom;
                return;
            }
            this.label = SET;
            this.labelSet = labelSet;
        }

        #region Properties
        /** return the single atom label or INVALID if not a single atom */
        public virtual int Atom
        {
            get
            {
                if ( IsAtom )
                {
                    return label;
                }
                return INVALID;
            }
        }
        public virtual bool IsAction
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsAtom
        {
            get
            {
                return label >= MIN_ATOM_VALUE;
            }
        }
        public virtual bool IsEpsilon
        {
            get
            {
                return label == EPSILON;
            }
        }
        public virtual bool IsSemanticPredicate
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsSet
        {
            get
            {
                return label == SET;
            }
        }
        public virtual SemanticContext SemanticContext
        {
            get
            {
                return null;
            }
        }
        public virtual IIntSet Set
        {
            get
            {
                if ( label != SET )
                {
                    // convert single element to a set if they ask for it.
                    return IntervalSet.of( label );
                }
                return labelSet;
            }
            set
            {
                label = SET;
                labelSet = value;
            }
        }
        #endregion

        public virtual object Clone()
        {
            Label l;
            //try
            //{
                //l = (Label)base.clone();
                l = new Label( label );
                l.label = this.label;
                l.labelSet = new IntervalSet();
                l.labelSet.addAll( this.labelSet );
            //}
            //catch ( CloneNotSupportedException e )
            //{
            //    throw new InternalError();
            //}
            return l;
        }

        public virtual void add( Label a )
        {
            if ( IsAtom )
            {
                labelSet = IntervalSet.of( label );
                label = SET;
                if ( a.IsAtom )
                {
                    labelSet.Add( a.Atom );
                }
                else if ( a.IsSet )
                {
                    labelSet.addAll( a.Set );
                }
                else
                {
                    throw new InvalidOperationException( "can't add element to Label of type " + label );
                }
                return;
            }
            if ( IsSet )
            {
                if ( a.IsAtom )
                {
                    labelSet.Add( a.Atom );
                }
                else if ( a.IsSet )
                {
                    labelSet.addAll( a.Set );
                }
                else
                {
                    throw new InvalidOperationException( "can't add element to Label of type " + label );
                }
                return;
            }
            throw new InvalidOperationException( "can't add element to Label of type " + label );
        }

        public virtual bool matches( int atom )
        {
            if ( label == atom )
            {
                return true; // handle the single atom case efficiently
            }
            if ( IsSet )
            {
                return labelSet.member( atom );
            }
            return false;
        }

        public virtual bool matches( IIntSet set )
        {
            if ( IsAtom )
            {
                return set.member( Atom );
            }
            if ( IsSet )
            {
                // matches if intersection non-nil
                return !Set.and( set ).isNil();
            }
            return false;
        }


        public virtual bool matches( Label other )
        {
            if ( other.IsSet )
            {
                return matches( other.Set );
            }
            if ( other.IsAtom )
            {
                return matches( other.Atom );
            }
            return false;
        }

        public override int GetHashCode()
        {
            if ( label == SET )
            {
                return labelSet.GetHashCode();
            }
            else
            {
                return label;
            }
        }

        // TODO: do we care about comparing set {A} with atom A? Doesn't now.
        public override bool Equals( object o )
        {
            if ( o == null )
            {
                return false;
            }
            if ( this == o )
            {
                return true; // equals if same object
            }
            // labels must be the same even if epsilon or set or sempred etc...
            if ( label != ( (Label)o ).label )
            {
                return false;
            }
            if ( label == SET )
            {
                return this.labelSet.Equals( ( (Label)o ).labelSet );
            }
            return true;  // label values are same, so true
        }

        public virtual int CompareTo( object o )
        {
            return this.label - ( (Label)o ).label;
        }

#if false
        /** Predicates are lists of AST nodes from the NFA created from the
         *  grammar, but the same predicate could be cut/paste into multiple
         *  places in the grammar.  I must compare the text of all the
         *  predicates to truly answer whether {p1,p2} .equals {p1,p2}.
         *  Unfortunately, I cannot rely on the AST.equals() to work properly
         *  so I must do a brute force O(n^2) nested traversal of the Set
         *  doing a String compare.
         *
         *  At this point, Labels are not compared for equals when they are
         *  predicates, but here's the code for future use.
         */
        protected boolean predicatesEquals(Set others) {
            Iterator iter = semanticContext.iterator();
            while (iter.hasNext()) {
                AST predAST = (AST) iter.next();
                Iterator inner = semanticContext.iterator();
                while (inner.hasNext()) {
                    AST otherPredAST = (AST) inner.next();
                    if ( !predAST.getText().equals(otherPredAST.getText()) ) {
                        return false;
                    }
                }
            }
            return true;
        }
#endif

        public override string ToString()
        {
            switch ( label )
            {
            case SET:
                return labelSet.ToString();
            default:
                return label.ToString(); //String.valueOf( label );
            }
        }

        public virtual string ToString( Grammar g )
        {
            switch ( label )
            {
            case SET:
                return labelSet.ToString( g );
            default:
                return g.getTokenDisplayName( label );
            }
        }

#if false
        public String predicatesToString() {
            if ( semanticContext==NFAConfiguration.DEFAULT_CLAUSE_SEMANTIC_CONTEXT ) {
                return "!other preds";
            }
            StringBuffer buf = new StringBuffer();
            Iterator iter = semanticContext.iterator();
            while (iter.hasNext()) {
                AST predAST = (AST) iter.next();
                buf.append(predAST.getText());
                if ( iter.hasNext() ) {
                    buf.append("&");
                }
            }
            return buf.toString();
        }
#endif

        public static bool intersect( Label label, Label edgeLabel )
        {
            bool hasIntersection = false;
            bool labelIsSet = label.IsSet;
            bool edgeIsSet = edgeLabel.IsSet;
            if ( !labelIsSet && !edgeIsSet && edgeLabel.label == label.label )
            {
                hasIntersection = true;
            }
            else if ( labelIsSet && edgeIsSet &&
                      !edgeLabel.Set.and( label.Set ).isNil() )
            {
                hasIntersection = true;
            }
            else if ( labelIsSet && !edgeIsSet &&
                      label.Set.member( edgeLabel.label ) )
            {
                hasIntersection = true;
            }
            else if ( !labelIsSet && edgeIsSet &&
                      edgeLabel.Set.member( label.label ) )
            {
                hasIntersection = true;
            }
            return hasIntersection;
        }
    }
}
