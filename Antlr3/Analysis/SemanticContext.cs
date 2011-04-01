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
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using ArgumentNullException = System.ArgumentNullException;
    using CLSCompliant = System.CLSCompliantAttribute;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;

    /** A binary tree structure used to record the semantic context in which
     *  an NFA configuration is valid.  It's either a single predicate or
     *  a tree representing an operation tree such as: p1&&p2 or p1||p2.
     *
     *  For NFA o-p1->o-p2->o, create tree AND(p1,p2).
     *  For NFA (1)-p1->(2)
     *           |       ^
     *           |       |
     *          (3)-p2----
     *  we will have to combine p1 and p2 into DFA state as we will be
     *  adding NFA configurations for state 2 with two predicates p1,p2.
     *  So, set context for combined NFA config for state 2: OR(p1,p2).
     *
     *  I have scoped the AND, NOT, OR, and Predicate subclasses of
     *  SemanticContext within the scope of this outer class.
     *
     *  July 7, 2006: TJP altered OR to be set of operands. the Binary tree
     *  made it really hard to reduce complicated || sequences to their minimum.
     *  Got huge repeated || conditions.
     */
    public abstract class SemanticContext : System.IEquatable<SemanticContext>
    {
        /** Create a default value for the semantic context shared among all
         *  NFAConfigurations that do not have an actual semantic context.
         *  This prevents lots of if!=null type checks all over; it represents
         *  just an empty set of predicates.
         */
        public static readonly SemanticContext EmptySemanticContext = new Predicate();

        /** Given a semantic context expression tree, return a tree with all
         *  nongated predicates set to true and then reduced.  So p&&(q||r) would
         *  return p&&r if q is nongated but p and r are gated.
         */
        public abstract SemanticContext GatedPredicateContext
        {
            get;
        }

        /** Generate an expression that will evaluate the semantic context,
         *  given a set of output templates.
         */
        public abstract StringTemplate GenExpr( CodeGenerator generator,
                                               StringTemplateGroup templates,
                                               DFA dfa );

        // user-specified sempred {}? or {}?=>
        public abstract bool HasUserSemanticPredicate
        {
            get;
        }

        public abstract bool IsSyntacticPredicate
        {
            get;
        }

        /** Notify the indicated grammar of any syn preds used within this context */
        public virtual void TrackUseOfSyntacticPredicates( Grammar g )
        {
        }

        protected virtual void ExpandOperands(HashSet<SemanticContext> contexts)
        {
            contexts.Add(this);
        }

        public override sealed bool Equals(object obj)
        {
            SemanticContext other = obj as SemanticContext;
            if (other == null)
                return false;

            return this.Equals(other);
        }

        public virtual bool Equals(SemanticContext other)
        {
            return object.ReferenceEquals(this, other);
        }

        public virtual SemanticContext ReduceOrWith(SemanticContext other)
        {
            return this;
        }

        public virtual SemanticContext ReduceAndWith(SemanticContext other)
        {
            return this;
        }

        public abstract override int GetHashCode();

        public class Predicate : SemanticContext
        {
            /** The AST node in tree created from the grammar holding the predicate */
            public GrammarAST predicateAST;

            /** Is this a {...}?=> gating predicate or a normal disambiguating {..}?
             *  If any predicate in expression is gated, then expression is considered
             *  gated.
             *
             *  The simple Predicate object's predicate AST's type is used to set
             *  gated to true if type==GATED_SEMPRED.
             */
            bool _gated;

            /** syntactic predicates are converted to semantic predicates
             *  but synpreds are generated slightly differently.
             */
            bool _synpred;

            public const int InvalidPredValue = -1;
            public const int FalsePred = 0;
            public const int TruePred = 1;

            /** sometimes predicates are known to be true or false; we need
             *  a way to represent this without resorting to a target language
             *  value like true or TRUE.
             */
            protected int constantValue = InvalidPredValue;

            public Predicate()
            {
                predicateAST = new GrammarAST();
                this._gated = false;
            }

            public Predicate( GrammarAST predicate )
            {
                this.predicateAST = predicate;
                this._gated =
                    predicate.Type == ANTLRParser.GATED_SEMPRED ||
                    predicate.Type == ANTLRParser.SYN_SEMPRED;
                this._synpred =
                    predicate.Type == ANTLRParser.SYN_SEMPRED ||
                    predicate.Type == ANTLRParser.BACKTRACK_SEMPRED;
            }

            public Predicate( Predicate p )
            {
                this.predicateAST = p.predicateAST;
                this._gated = p._gated;
                this._synpred = p._synpred;
                this.constantValue = p.constantValue;
            }

            /** Two predicates are the same if they are literally the same
             *  text rather than same node in the grammar's AST.
             *  Or, if they have the same constant value, return equal.
             *  As of July 2006 I'm not sure these are needed.
             */
            public override bool Equals(SemanticContext other)
            {
                Predicate p = other as Predicate;
                if (p == null)
                    return false;

                return predicateAST.Text.Equals(p.predicateAST.Text);
            }

            public override int GetHashCode()
            {
                if ( predicateAST == null )
                    return 0;

                return predicateAST.Text.GetHashCode();
            }

            public override StringTemplate GenExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    if ( _synpred )
                    {
                        eST = templates.GetInstanceOf( "evalSynPredicate" );
                    }
                    else
                    {
                        eST = templates.GetInstanceOf( "evalPredicate" );
                        generator.grammar.decisionsWhoseDFAsUsesSemPreds.Add( dfa );
                    }
                    string predEnclosingRuleName = predicateAST.enclosingRuleName;
                    /*
                    String decisionEnclosingRuleName =
                        dfa.getNFADecisionStartState().getEnclosingRule();
                    // if these rulenames are diff, then pred was hoisted out of rule
                    // Currently I don't warn you about this as it could be annoying.
                    // I do the translation anyway.
                    */
                    //eST.setAttribute("pred", this.toString());
                    if ( generator != null )
                    {
                        eST.SetAttribute( "pred",
                                         generator.TranslateAction( predEnclosingRuleName, predicateAST ) );
                    }
                }
                else
                {
                    eST = new StringTemplate( "$pred$" );
                    eST.SetAttribute( "pred", this.ToString() );
                    return eST;
                }
                if ( generator != null )
                {
                    string description =
                        generator.target.GetTargetStringLiteralFromString( this.ToString() );
                    eST.SetAttribute( "description", description );
                }
                return eST;
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    if ( _gated )
                    {
                        return this;
                    }
                    return null;
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    // user-specified sempred
                    return predicateAST != null &&
                           (predicateAST.Type == ANTLRParser.GATED_SEMPRED ||
                             predicateAST.Type == ANTLRParser.SEMPRED);
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    return predicateAST != null &&
                        ( predicateAST.Type == ANTLRParser.SYN_SEMPRED ||
                          predicateAST.Type == ANTLRParser.BACKTRACK_SEMPRED );
                }
            }

            public override void TrackUseOfSyntacticPredicates( Grammar g )
            {
                if ( _synpred )
                {
                    g.synPredNamesUsedInDFA.Add( predicateAST.Text );
                }
            }

            public override string ToString()
            {
                if ( predicateAST == null )
                {
                    return "<nopred>";
                }
                return predicateAST.Text;
            }
        }

        public class TruePredicate : Predicate
        {
            public static readonly TruePredicate Instance = new TruePredicate();

            public TruePredicate()
            {
                this.constantValue = TruePred;
            }

            public override StringTemplate GenExpr(CodeGenerator generator, StringTemplateGroup templates, DFA dfa)
            {
                if (templates != null)
                    return templates.GetInstanceOf("true");

                return new StringTemplate("true");
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return ~0;
            }

            public override string ToString()
            {
                return "true"; // not used for code gen, just DOT and print outs
            }
        }

        public class FalsePredicate : Predicate
        {
            public static readonly FalsePredicate Instance = new FalsePredicate();

            public FalsePredicate()
            {
                this.constantValue = FalsePred;
            }

            public override StringTemplate GenExpr(CodeGenerator generator, StringTemplateGroup templates, DFA dfa)
            {
                if (templates != null)
                    return templates.GetInstanceOf("false");

                return new StringTemplate("false");
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override string ToString()
            {
                return "false"; // not used for code gen, just DOT and print outs
            }
        }

        public class AND : SemanticContext
        {
            private readonly SemanticContext _left;
            private readonly SemanticContext _right;
            private readonly int _hashcode;

            public AND( SemanticContext a, SemanticContext b )
            {
                if (a == null)
                    throw new ArgumentNullException("a");
                if (b == null)
                    throw new ArgumentNullException("b");

                this._left = a;
                this._right = b;
                _hashcode = a.GetHashCode() ^ b.GetHashCode();
            }

            public override StringTemplate GenExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.GetInstanceOf( "andPredicates" );
                }
                else
                {
                    eST = new StringTemplate( "($left$&&$right$)" );
                }
                eST.SetAttribute( "left", _left.GenExpr( generator, templates, dfa ) );
                eST.SetAttribute( "right", _right.GenExpr( generator, templates, dfa ) );
                return eST;
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext gatedLeft = _left.GatedPredicateContext;
                    SemanticContext gatedRight = _right.GatedPredicateContext;
                    if ( gatedLeft == null )
                    {
                        return gatedRight;
                    }
                    if ( gatedRight == null )
                    {
                        return gatedLeft;
                    }
                    return new AND( gatedLeft, gatedRight );
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return _left.HasUserSemanticPredicate || _right.HasUserSemanticPredicate;
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    return _left.IsSyntacticPredicate || _right.IsSyntacticPredicate;
                }
            }

            public override void TrackUseOfSyntacticPredicates( Grammar g )
            {
                _left.TrackUseOfSyntacticPredicates( g );
                _right.TrackUseOfSyntacticPredicates( g );
            }

            public override bool Equals(SemanticContext other)
            {
                if (object.ReferenceEquals(this, other))
                    return true;

                HashSet<SemanticContext> operands = new HashSet<SemanticContext>();
                ExpandOperands(operands);

                AND and = other as AND;
                if (and != null)
                {
                    HashSet<SemanticContext> otherOperands = new HashSet<SemanticContext>();
                    and.ExpandOperands(otherOperands);

                    return operands.SetEquals(otherOperands);
                }

                NOT not = other as NOT;
                if (not != null)
                {
                    OR or = not.ctx as OR;
                    if (or != null)
                    {
                        return operands.SetEquals(or.Operands.Select(i => Not(i)));
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _hashcode;
            }

            public override SemanticContext ReduceAndWith(SemanticContext other)
            {
                if (_left.Equals(other))
                    return _right.ReduceAndWith(other);
                else if (_right.Equals(other))
                    return _left.ReduceAndWith(other);

                return this;
            }

            public override SemanticContext ReduceOrWith(SemanticContext other)
            {
                if (_left.Equals(other) || _right.Equals(other))
                    return FalsePredicate.Instance;

                return this;
            }

            public override string ToString()
            {
                return "(" + _left + "&&" + _right + ")";
            }

            protected override void ExpandOperands(HashSet<SemanticContext> contexts)
            {
                AND and = _left as AND;
                if (and != null)
                    and.ExpandOperands(contexts);
                else
                    contexts.Add(_left);

                and = _right as AND;
                if (and != null)
                    and.ExpandOperands(contexts);
                else
                    contexts.Add(_right);
            }
        }

        public class OR : SemanticContext
        {
            private readonly HashSet<SemanticContext> _operands = new HashSet<SemanticContext>();
            private readonly int _hashcode;

            public OR(SemanticContext a, SemanticContext b)
            {
                OR or = a as OR;
                if (or != null)
                    _operands.UnionWith(or._operands);
                else if (a != null)
                    _operands.Add(a);

                or = b as OR;
                if (or != null)
                    _operands.UnionWith(or._operands);
                else if (b != null)
                    _operands.Add(b);

                _hashcode = _operands.Aggregate(0, (x, y) => ~x ^ ~y.GetHashCode());
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext result = null;
                    foreach ( SemanticContext semctx in _operands )
                    {
                        SemanticContext gatedPred = semctx.GatedPredicateContext;
                        if ( gatedPred != null )
                        {
                            result = Or( result, gatedPred );
                            // result = new OR(result, gatedPred);
                        }
                    }
                    return result;
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    for (Iterator it = _operands.iterator(); it.hasNext(); )
                    {
                        SemanticContext semctx = (SemanticContext)it.next();
                        if (semctx.HasUserSemanticPredicate)
                            return true;
                    }

                    return false;
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    foreach ( SemanticContext semctx in _operands )
                    {
                        if ( semctx.IsSyntacticPredicate )
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public ICollection<SemanticContext> Operands
            {
                get
                {
                    return _operands;
                }
            }

            public override StringTemplate GenExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.GetInstanceOf( "orPredicates" );
                }
                else
                {
                    eST = new StringTemplate( "($first(operands)$$rest(operands):{o | ||$o$}$)" );
                }

                foreach ( SemanticContext semctx in _operands )
                {
                    eST.SetAttribute( "operands", semctx.GenExpr( generator, templates, dfa ) );
                }

                return eST;
            }

            public override void TrackUseOfSyntacticPredicates( Grammar g )
            {
                foreach ( SemanticContext semctx in _operands )
                {
                    semctx.TrackUseOfSyntacticPredicates( g );
                }
            }

            public override bool Equals(SemanticContext other)
            {
                if (object.ReferenceEquals(this, other))
                    return true;

                OR or = other as OR;
                if (or != null)
                    return or._operands.SetEquals(this._operands);

                NOT not = other as NOT;
                if (not != null)
                {
                    AND and = not.ctx as AND;
                    if (and != null)
                    {
                        HashSet<SemanticContext> andOperands = new HashSet<SemanticContext>();
                        and.ExpandOperands(andOperands);
                        return _operands.SetEquals(andOperands.Select(i => Not(i)));
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _hashcode;
            }

            public override SemanticContext ReduceAndWith(SemanticContext other)
            {
                if (_operands.Contains(other))
                    return TruePredicate.Instance;

                return base.ReduceAndWith(other);
            }

            public override SemanticContext ReduceOrWith(SemanticContext other)
            {
                return base.ReduceOrWith(other);
            }

            public override string ToString()
            {
                StringBuilder buf = new StringBuilder();
                buf.Append( "(" );
                int i = 0;
                foreach ( SemanticContext semctx in _operands )
                {
                    if ( i > 0 )
                    {
                        buf.Append( "||" );
                    }
                    buf.Append( semctx.ToString() );
                    i++;
                }
                buf.Append( ")" );
                return buf.ToString();
            }

            protected override void ExpandOperands(HashSet<SemanticContext> contexts)
            {
                contexts.UnionWith(_operands);
            }
        }

        public class NOT : SemanticContext
        {
            protected internal SemanticContext ctx;

            public NOT( SemanticContext ctx )
            {
                this.ctx = ctx;
            }
            public override StringTemplate GenExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.GetInstanceOf( "notPredicate" );
                }
                else
                {
                    eST = new StringTemplate( "?!($pred$)" );
                }
                eST.SetAttribute( "pred", ctx.GenExpr( generator, templates, dfa ) );
                return eST;
            }
            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext p = ctx.GatedPredicateContext;
                    if ( p == null )
                    {
                        return null;
                    }
                    return new NOT( p );
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return ctx.HasUserSemanticPredicate;
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    return ctx.IsSyntacticPredicate;
                }
            }

            public override void TrackUseOfSyntacticPredicates( Grammar g )
            {
                ctx.TrackUseOfSyntacticPredicates( g );
            }

            public override bool Equals(SemanticContext other)
            {
                NOT not = other as NOT;
                if (not == null)
                    return false;

                return this.ctx.Equals(not.ctx);
            }

            public override int GetHashCode()
            {
                return ~ctx.GetHashCode();
            }

            public override SemanticContext ReduceAndWith(SemanticContext other)
            {
                if (ctx.Equals(other))
                    return FalsePredicate.Instance;

                return this;
            }

            public override SemanticContext ReduceOrWith(SemanticContext other)
            {
                if (ctx.Equals(other))
                    return TruePredicate.Instance;

                return this;
            }

            public override string ToString()
            {
                return "!(" + ctx + ")";
            }
        }

        [CLSCompliant(false)]
        public static SemanticContext And(SemanticContext a, SemanticContext b)
        {
            //System.Console.Out.WriteLine( "AND: " + a + "&&" + b );
            if (a is FalsePredicate || b is FalsePredicate)
                return FalsePredicate.Instance;

            if (a == EmptySemanticContext || a == null)
                return b;

            if (b == EmptySemanticContext || b == null)
                return a;

            b = b.ReduceAndWith(a);

            if (a is TruePredicate)
                return b;
            if (b is TruePredicate)
                return a;

            if (a.Equals(b))
                return a; // if same, just return left one

            //System.Console.Out.WriteLine( "## have to AND" );
            return new AND(a, b);
        }

        [CLSCompliant(false)]
        public static SemanticContext Or(SemanticContext a, SemanticContext b)
        {
            //System.Console.Out.WriteLine( "OR: " + a + "||" + b );
            if ( a == EmptySemanticContext || a == null )
                return b;

            if ( b == EmptySemanticContext || b == null )
                return a;

            b = b.ReduceOrWith(a);

            if (a is TruePredicate || b is TruePredicate)
                return TruePredicate.Instance;
            if (b is FalsePredicate)
                return a;
            if (a is FalsePredicate)
                return b;

            if (a.Equals(b))
                return a;

            NOT not = a as NOT;
            if (not != null)
            {
                if (not.ctx.Equals(b))
                    return TruePredicate.Instance;
            }
            else
            {
                not = b as NOT;
                if (not != null && not.ctx.Equals(a))
                    return TruePredicate.Instance;
            }

            //System.Console.Out.WriteLine( "## have to OR" );
            OR result = new OR( a, b );
            if (result.Operands.Count == 1)
                return result.Operands.First();

            return result;
        }

        [CLSCompliant(false)]
        public static SemanticContext Not(SemanticContext a)
        {
            NOT nota = a as NOT;
            if ( nota != null )
                return nota.ctx;

            if (a is TruePredicate)
                return FalsePredicate.Instance;
            else if (a is FalsePredicate)
                return TruePredicate.Instance;

            return new NOT( a );
        }
    }
}
