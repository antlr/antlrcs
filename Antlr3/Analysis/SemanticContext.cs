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
    using Antlr.Runtime.JavaExtensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
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
    public abstract class SemanticContext
    {
        /** Create a default value for the semantic context shared among all
         *  NFAConfigurations that do not have an actual semantic context.
         *  This prevents lots of if!=null type checks all over; it represents
         *  just an empty set of predicates.
         */
        public static readonly SemanticContext EMPTY_SEMANTIC_CONTEXT = new Predicate();

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
        public abstract StringTemplate genExpr( CodeGenerator generator,
                                               StringTemplateGroup templates,
                                               DFA dfa );

        public abstract bool IsSyntacticPredicate
        {
            get;
        }

        /** Notify the indicated grammar of any syn preds used within this context */
        public virtual void trackUseOfSyntacticPredicates( Grammar g )
        {
        }

        public /*static*/ class Predicate : SemanticContext
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
            protected bool gated = false;

            /** syntactic predicates are converted to semantic predicates
             *  but synpreds are generated slightly differently.
             */
            protected bool synpred = false;

            public const int INVALID_PRED_VALUE = -1;
            public const int FALSE_PRED = 0;
            public const int TRUE_PRED = 1;

            /** sometimes predicates are known to be true or false; we need
             *  a way to represent this without resorting to a target language
             *  value like true or TRUE.
             */
            protected int constantValue = INVALID_PRED_VALUE;

            public Predicate()
            {
                predicateAST = new GrammarAST();
                this.gated = false;
            }

            public Predicate( GrammarAST predicate )
            {
                this.predicateAST = predicate;
                this.gated =
                    predicate.Type == ANTLRParser.GATED_SEMPRED ||
                    predicate.Type == ANTLRParser.SYN_SEMPRED;
                this.synpred =
                    predicate.Type == ANTLRParser.SYN_SEMPRED ||
                    predicate.Type == ANTLRParser.BACKTRACK_SEMPRED;
            }

            public Predicate( Predicate p )
            {
                this.predicateAST = p.predicateAST;
                this.gated = p.gated;
                this.synpred = p.synpred;
                this.constantValue = p.constantValue;
            }

            /** Two predicates are the same if they are literally the same
             *  text rather than same node in the grammar's AST.
             *  Or, if they have the same constant value, return equal.
             *  As of July 2006 I'm not sure these are needed.
             */
            public override bool Equals( object o )
            {
                Predicate p = o as Predicate;
                if ( p == null )
                {
                    return false;
                }
                return predicateAST.Text.Equals( p.predicateAST.Text );
            }

            public override int GetHashCode()
            {
                if ( predicateAST == null )
                {
                    return 0;
                }
                return predicateAST.Text.GetHashCode();
            }

            public override StringTemplate genExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    if ( synpred )
                    {
                        eST = templates.getInstanceOf( "evalSynPredicate" );
                    }
                    else
                    {
                        eST = templates.getInstanceOf( "evalPredicate" );
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
                        eST.setAttribute( "pred",
                                         generator.translateAction( predEnclosingRuleName, predicateAST ) );
                    }
                }
                else
                {
                    eST = new StringTemplate( "$pred$" );
                    eST.setAttribute( "pred", this.ToString() );
                    return eST;
                }
                if ( generator != null )
                {
                    string description =
                        generator.target.getTargetStringLiteralFromString( this.ToString() );
                    eST.setAttribute( "description", description );
                }
                return eST;
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    if ( gated )
                    {
                        return this;
                    }
                    return null;
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

            public override void trackUseOfSyntacticPredicates( Grammar g )
            {
                if ( synpred )
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

        public /*static*/ class TruePredicate : Predicate
        {
            public TruePredicate()
            {
                this.constantValue = TRUE_PRED;
            }

            public override StringTemplate genExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                if ( templates != null )
                {
                    return templates.getInstanceOf( "true" );
                }
                return new StringTemplate( "true" );
            }

            public override string ToString()
            {
                return "true"; // not used for code gen, just DOT and print outs
            }
        }

        /*
        public static class FalsePredicate extends Predicate {
            public FalsePredicate() {
                super();
                this.constantValue = FALSE_PRED;
            }
            public StringTemplate genExpr(CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa)
            {
                if ( templates!=null ) {
                    return templates.getInstanceOf("false");
                }
                return new StringTemplate("false");
            }
            public String toString() {
                return "false"; // not used for code gen, just DOT and print outs
            }
        }
        */

        public /*static*/ class AND : SemanticContext
        {
            protected SemanticContext left, right;
            public AND( SemanticContext a, SemanticContext b )
            {
                this.left = a;
                this.right = b;
            }
            public override StringTemplate genExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.getInstanceOf( "andPredicates" );
                }
                else
                {
                    eST = new StringTemplate( "($left$&&$right$)" );
                }
                eST.setAttribute( "left", left.genExpr( generator, templates, dfa ) );
                eST.setAttribute( "right", right.genExpr( generator, templates, dfa ) );
                return eST;
            }
            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext gatedLeft = left.GatedPredicateContext;
                    SemanticContext gatedRight = right.GatedPredicateContext;
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
            public override bool IsSyntacticPredicate
            {
                get
                {
                    return left.IsSyntacticPredicate || right.IsSyntacticPredicate;
                }
            }
            public override void trackUseOfSyntacticPredicates( Grammar g )
            {
                left.trackUseOfSyntacticPredicates( g );
                right.trackUseOfSyntacticPredicates( g );
            }
            public override string ToString()
            {
                return "(" + left + "&&" + right + ")";
            }
        }

        public /*static*/ class OR : SemanticContext
        {
            protected HashSet<object> operands;
            public OR( SemanticContext a, SemanticContext b )
            {
                operands = new HashSet<object>();
                if ( a is OR )
                {
                    operands.addAll( ( (OR)a ).operands );
                }
                else if ( a != null )
                {
                    operands.Add( a );
                }
                if ( b is OR )
                {
                    operands.addAll( ( (OR)b ).operands );
                }
                else if ( b != null )
                {
                    operands.Add( b );
                }
            }
            public override StringTemplate genExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.getInstanceOf( "orPredicates" );
                }
                else
                {
                    eST = new StringTemplate( "($first(operands)$$rest(operands):{o | ||$o$}$)" );
                }
                foreach ( SemanticContext semctx in operands )
                {
                    eST.setAttribute( "operands", semctx.genExpr( generator, templates, dfa ) );
                }
                return eST;
            }
            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext result = null;
                    foreach ( SemanticContext semctx in operands )
                    {
                        SemanticContext gatedPred = semctx.GatedPredicateContext;
                        if ( gatedPred != null )
                        {
                            result = or( result, gatedPred );
                            // result = new OR(result, gatedPred);
                        }
                    }
                    return result;
                }
            }
            public override bool IsSyntacticPredicate
            {
                get
                {
                    foreach ( SemanticContext semctx in operands )
                    {
                        if ( semctx.IsSyntacticPredicate )
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            public override void trackUseOfSyntacticPredicates( Grammar g )
            {
                foreach ( SemanticContext semctx in operands )
                {
                    semctx.trackUseOfSyntacticPredicates( g );
                }
            }
            public override string ToString()
            {
                StringBuilder buf = new StringBuilder();
                buf.Append( "(" );
                int i = 0;
                foreach ( SemanticContext semctx in operands )
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
        }

        public /*static*/ class NOT : SemanticContext
        {
            protected internal SemanticContext ctx;
            public NOT( SemanticContext ctx )
            {
                this.ctx = ctx;
            }
            public override StringTemplate genExpr( CodeGenerator generator,
                                          StringTemplateGroup templates,
                                          DFA dfa )
            {
                StringTemplate eST = null;
                if ( templates != null )
                {
                    eST = templates.getInstanceOf( "notPredicate" );
                }
                else
                {
                    eST = new StringTemplate( "?!($pred$)" );
                }
                eST.setAttribute( "pred", ctx.genExpr( generator, templates, dfa ) );
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
            public override bool IsSyntacticPredicate
            {
                get
                {
                    return ctx.IsSyntacticPredicate;
                }
            }
            public override void trackUseOfSyntacticPredicates( Grammar g )
            {
                ctx.trackUseOfSyntacticPredicates( g );
            }

            public override bool Equals( object @object )
            {
                if ( !( @object is NOT ) )
                {
                    return false;
                }
                return this.ctx.Equals( ( (NOT)@object ).ctx );
            }

            public override int GetHashCode()
            {
                return ctx.GetHashCode();
            }

            public override string ToString()
            {
                return "!(" + ctx + ")";
            }
        }

        public static SemanticContext and( SemanticContext a, SemanticContext b )
        {
            //JSystem.@out.println("AND: "+a+"&&"+b);
            if ( a == EMPTY_SEMANTIC_CONTEXT || a == null )
            {
                return b;
            }
            if ( b == EMPTY_SEMANTIC_CONTEXT || b == null )
            {
                return a;
            }
            if ( a.Equals( b ) )
            {
                return a; // if same, just return left one
            }
            //JSystem.@out.println("## have to AND");
            return new AND( a, b );
        }

        public static SemanticContext or( SemanticContext a, SemanticContext b )
        {
            //JSystem.@out.println("OR: "+a+"||"+b);
            if ( a == EMPTY_SEMANTIC_CONTEXT || a == null )
            {
                return b;
            }
            if ( b == EMPTY_SEMANTIC_CONTEXT || b == null )
            {
                return a;
            }
            if ( a is TruePredicate )
            {
                return a;
            }
            if ( b is TruePredicate )
            {
                return b;
            }
            if ( a is NOT && b is Predicate )
            {
                NOT n = (NOT)a;
                // check for !p||p
                if ( n.ctx.Equals( b ) )
                {
                    return new TruePredicate();
                }
            }
            else if ( b is NOT && a is Predicate )
            {
                NOT n = (NOT)b;
                // check for p||!p
                if ( n.ctx.Equals( a ) )
                {
                    return new TruePredicate();
                }
            }
            else if ( a.Equals( b ) )
            {
                return a;
            }
            //JSystem.@out.println("## have to OR");
            return new OR( a, b );
        }

        public static SemanticContext not( SemanticContext a )
        {
            return new NOT( a );
        }

    }
}
