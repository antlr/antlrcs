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
    using Antlr3.Extensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using ArgumentNullException = System.ArgumentNullException;
    using CLSCompliant = System.CLSCompliantAttribute;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using StringComparer = System.StringComparer;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;

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
        public static readonly SemanticContext EmptySemanticContext = new Predicate(Predicate.InvalidPredValue);

        /** Given a semantic context expression tree, return a tree with all
         *  nongated predicates set to true and then reduced.  So p&&(q||r) would
         *  return p&&r if q is nongated but p and r are gated.
         */
        public abstract SemanticContext GatedPredicateContext
        {
            get;
        }

        // user-specified sempred {}? or {}?=>
        public abstract bool HasUserSemanticPredicate
        {
            get;
        }

        public abstract bool IsSyntacticPredicate
        {
            get;
        }

        public sealed override bool Equals(object obj)
        {
            SemanticContext other = obj as SemanticContext;
            if (other == null)
                return false;

            return this.Equals(other);
        }

        public abstract bool Equals(SemanticContext other);

        public abstract override int GetHashCode();

        /** Notify the indicated grammar of any syn preds used within this context */
        public virtual void TrackUseOfSyntacticPredicates(Grammar g)
        {
        }

        /** Generate an expression that will evaluate the semantic context,
         *  given a set of output templates.
         */
        public abstract StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa);

        public class Predicate : SemanticContext
        {
            public const int InvalidPredValue = -2;
            public const int FalsePred = 0;
            public const int TruePred = ~0;

            /** The AST node in tree created from the grammar holding the predicate */
            private readonly GrammarAST _predicateAST;

            /** Is this a {...}?=> gating predicate or a normal disambiguating {..}?
             *  If any predicate in expression is gated, then expression is considered
             *  gated.
             *
             *  The simple Predicate object's predicate AST's type is used to set
             *  gated to true if type==GATED_SEMPRED.
             */
            private readonly bool _gated;

            /** syntactic predicates are converted to semantic predicates
             *  but synpreds are generated slightly differently.
             */
            private readonly bool _synpred;

            /** sometimes predicates are known to be true or false; we need
             *  a way to represent this without resorting to a target language
             *  value like true or TRUE.
             */
            private readonly int _constantValue = InvalidPredValue;

            public Predicate(int constantValue)
            {
                this._predicateAST = new GrammarAST();
                this._constantValue = constantValue;
            }

            public Predicate(GrammarAST predicate)
            {
                this._predicateAST = predicate;
                this._gated =
                    predicate.Type == ANTLRParser.GATED_SEMPRED ||
                    predicate.Type == ANTLRParser.SYN_SEMPRED;
                this._synpred =
                    predicate.Type == ANTLRParser.SYN_SEMPRED ||
                    predicate.Type == ANTLRParser.BACKTRACK_SEMPRED;
            }

            public Predicate(Predicate p)
            {
                this._predicateAST = p._predicateAST;
                this._gated = p._gated;
                this._synpred = p._synpred;
                this._constantValue = p._constantValue;
            }

            public GrammarAST PredicateAST
            {
                get
                {
                    return _predicateAST;
                }
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    if (_gated)
                        return this;

                    return null;
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    // user-specified sempred
                    return _predicateAST != null &&
                           (_predicateAST.Type == ANTLRParser.GATED_SEMPRED ||
                             _predicateAST.Type == ANTLRParser.SEMPRED);
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    return _predicateAST != null &&
                        (_predicateAST.Type == ANTLRParser.SYN_SEMPRED ||
                          _predicateAST.Type == ANTLRParser.BACKTRACK_SEMPRED);
                }
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

                if (this._constantValue != InvalidPredValue)
                    return this._constantValue == p._constantValue;
                else if (p._constantValue != InvalidPredValue)
                    return false;

                return StringComparer.Ordinal.Equals(_predicateAST.Text, p._predicateAST.Text);
            }

            public override int GetHashCode()
            {
                if (_constantValue != InvalidPredValue)
                    return _constantValue.GetHashCode();

                if (_predicateAST == null)
                    return 0;

                return StringComparer.Ordinal.GetHashCode(_predicateAST.Text);
            }

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                StringTemplate eST = null;
                if (templates != null)
                {
                    if (_synpred)
                    {
                        eST = templates.GetInstanceOf("evalSynPredicate");
                    }
                    else
                    {
                        eST = templates.GetInstanceOf("evalPredicate");
                        generator.Grammar.decisionsWhoseDFAsUsesSemPreds.Add(dfa);
                    }

                    string predEnclosingRuleName = _predicateAST.enclosingRuleName;
                    /*
                    String decisionEnclosingRuleName =
                        dfa.getNFADecisionStartState().getEnclosingRule();
                    // if these rulenames are diff, then pred was hoisted out of rule
                    // Currently I don't warn you about this as it could be annoying.
                    // I do the translation anyway.
                    */
                    //eST.setAttribute("pred", this.toString());
                    if (generator != null)
                    {
                        eST.SetAttribute("pred", generator.TranslateAction(predEnclosingRuleName, _predicateAST));
                    }
                }
                else
                {
                    eST = new StringTemplate("<pred>");
                    eST.SetAttribute("pred", this.ToString());
                    return eST;
                }

                if (generator != null)
                {
                    string description = generator.Target.GetTargetStringLiteralFromString(this.ToString());
                    eST.SetAttribute("description", description);
                }

                return eST;
            }

            public override void TrackUseOfSyntacticPredicates(Grammar g)
            {
                if (_synpred)
                    g.synPredNamesUsedInDFA.Add(_predicateAST.Text);
            }

            public override string ToString()
            {
                if (_predicateAST == null)
                    return "<nopred>";

                return _predicateAST.Text;
            }
        }

        public sealed class TruePredicate : Predicate
        {
            public static readonly TruePredicate Instance = new TruePredicate();

            public TruePredicate()
                : base(TruePred)
            {
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return false;
                }
            }

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                if (templates != null)
                    return templates.GetInstanceOf("true_value");

                return new StringTemplate("true");
            }

            public override string ToString()
            {
                return "true"; // not used for code gen, just DOT and print outs
            }
        }

        public sealed class FalsePredicate : Predicate
        {
            public static readonly FalsePredicate Instance = new FalsePredicate();

            public FalsePredicate()
                : base(FalsePred)
            {
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return false;
                }
            }

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                if (templates != null)
                    return templates.GetInstanceOf("false_value");

                return new StringTemplate("false");
            }

            public override string ToString()
            {
                return "false"; // not used for code gen, just DOT and print outs
            }
        }

        public abstract class CommutativePredicate : SemanticContext
        {
            private readonly HashSet<SemanticContext> _operands = new HashSet<SemanticContext>();
            private readonly int _hashcode;

            protected CommutativePredicate(SemanticContext a, SemanticContext b)
            {
                if (a == null)
                    throw new ArgumentNullException("a");
                if (b == null)
                    throw new ArgumentNullException("b");

                if (a.GetType() == this.GetType())
                    _operands.UnionWith(((CommutativePredicate)a).Operands);
                else
                    _operands.Add(a);

                if (b.GetType() == this.GetType())
                    _operands.UnionWith(((CommutativePredicate)b).Operands);
                else
                    _operands.Add(b);

                _hashcode = CalculateHashCode();
            }

            public CommutativePredicate(IEnumerable<SemanticContext> contexts)
            {
                if (contexts == null)
                    throw new ArgumentNullException("contexts");

                foreach (var context in contexts)
                {
                    CommutativePredicate commutative = context as CommutativePredicate;
                    if (commutative != null && commutative.GetType() == this.GetType())
                        _operands.UnionWith(commutative._operands);
                    else if (context != null)
                        _operands.Add(context);
                }

                _hashcode = CalculateHashCode();
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext result = null;
                    foreach (SemanticContext semctx in _operands)
                    {
                        SemanticContext gatedPred = semctx.GatedPredicateContext;
                        if (gatedPred != null)
                            result = CombinePredicates(result, gatedPred);
                    }

                    return result;
                }
            }

            public override bool HasUserSemanticPredicate
            {
                get
                {
                    return _operands.Any(i => i.HasUserSemanticPredicate);
                }
            }

            public override bool IsSyntacticPredicate
            {
                get
                {
                    return _operands.Any(i => i.IsSyntacticPredicate);
                }
            }

            public ICollection<SemanticContext> Operands
            {
                get
                {
                    return _operands;
                }
            }

            protected abstract string OperatorString
            {
                get;
            }

            public override void TrackUseOfSyntacticPredicates(Grammar g)
            {
                foreach (SemanticContext semctx in _operands)
                    semctx.TrackUseOfSyntacticPredicates(g);
            }

            public override bool Equals(SemanticContext other)
            {
                if (object.ReferenceEquals(this, other))
                    return true;

                CommutativePredicate commutative = other as CommutativePredicate;
                if (commutative != null && other.GetType() == this.GetType())
                {
                    ICollection<SemanticContext> otherOperands = commutative.Operands;
                    return _operands.SetEquals(otherOperands);
                }

                NOT not = other as NOT;
                if (not != null)
                {
                    commutative = not.ctx as CommutativePredicate;
                    if (commutative != null && commutative.GetType() != this.GetType())
                    {
                        return _operands.SetEquals(commutative.Operands.Select(i => Not(i)));
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _hashcode;
            }

            public override string ToString()
            {
                return string.Format("({0})", string.Join(OperatorString, _operands.Select(i => i.ToString()).ToArray()));
            }

            protected abstract SemanticContext CombinePredicates(SemanticContext a, SemanticContext b);

            protected abstract int CalculateHashCode();
        }

        public class AND : CommutativePredicate
        {
            public AND(SemanticContext a, SemanticContext b)
                : base(a, b)
            {
            }

            public AND(IEnumerable<SemanticContext> contexts)
                : base(contexts)
            {
            }

            protected override string OperatorString
            {
                get
                {
                    return "&&";
                }
            }

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                StringTemplate result =
                    Operands.Aggregate(default(StringTemplate),
                        (template, operand) =>
                        {
                            if (template == null)
                                return operand.GenExpr(generator, templates, dfa);

                            StringTemplate eST = null;
                            if (templates != null)
                                eST = templates.GetInstanceOf("andPredicates");
                            else
                                eST = new StringTemplate("(<left>&&<right>)");

                            eST.SetAttribute("left", template);
                            eST.SetAttribute("right", operand.GenExpr(generator, templates, dfa));
                            return eST;
                        });

                return result;
            }

            protected override SemanticContext CombinePredicates(SemanticContext a, SemanticContext b)
            {
                return And(a, b);
            }

            protected override int CalculateHashCode()
            {
                return Operands.Aggregate(0, (x, y) => x ^ y.GetHashCode());
            }
        }

        public class OR : CommutativePredicate
        {
            public OR(SemanticContext a, SemanticContext b)
                : base(a, b)
            {
            }

            public OR(IEnumerable<SemanticContext> contexts)
                : base(contexts)
            {
            }

            protected override string OperatorString
            {
                get
                {
                    return "||";
                }
            }

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                StringTemplate eST = null;
                if (templates != null)
                    eST = templates.GetInstanceOf("orPredicates");
                else
                    eST = new StringTemplate("(<first(operands)><rest(operands):{o | ||<o>}>)");

                foreach (SemanticContext semctx in Operands)
                {
                    eST.SetAttribute("operands", semctx.GenExpr(generator, templates, dfa));
                }

                return eST;
            }

            protected override SemanticContext CombinePredicates(SemanticContext a, SemanticContext b)
            {
                return Or(a, b);
            }

            protected override int CalculateHashCode()
            {
                return Operands.Aggregate(0, (x, y) => x ^ ~y.GetHashCode());
            }
        }

        public class NOT : SemanticContext
        {
            protected internal SemanticContext ctx;

            public NOT(SemanticContext ctx)
            {
                this.ctx = ctx;
            }

            public override SemanticContext GatedPredicateContext
            {
                get
                {
                    SemanticContext p = ctx.GatedPredicateContext;
                    if (p == null)
                        return null;

                    return new NOT(p);
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

            public override StringTemplate GenExpr(CodeGenerator generator, TemplateGroup templates, DFA dfa)
            {
                StringTemplate eST = null;
                if (templates != null)
                    eST = templates.GetInstanceOf("notPredicate");
                else
                    eST = new StringTemplate("!(<pred>)");

                eST.SetAttribute("pred", ctx.GenExpr(generator, templates, dfa));
                return eST;
            }

            public override void TrackUseOfSyntacticPredicates(Grammar g)
            {
                ctx.TrackUseOfSyntacticPredicates(g);
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

            public override string ToString()
            {
                return "!(" + ctx + ")";
            }
        }

        [CLSCompliant(false)]
        public static SemanticContext And(SemanticContext a, SemanticContext b)
        {
            if (a is FalsePredicate || b is FalsePredicate)
                return FalsePredicate.Instance;

            SemanticContext commonTerms = FactorOr(ref a, ref b);
            bool factored = commonTerms != null && commonTerms != EmptySemanticContext && !(commonTerms is TruePredicate);
            if (factored)
                return Or(commonTerms, And(a, b));

            //System.Console.Out.WriteLine( "AND: " + a + "&&" + b );
            if (a is FalsePredicate || b is FalsePredicate)
                return FalsePredicate.Instance;

            if (a == EmptySemanticContext || a == null)
                return b;

            if (b == EmptySemanticContext || b == null)
                return a;

            if (a is TruePredicate)
                return b;

            if (b is TruePredicate)
                return a;

            //// Factoring takes care of this case
            //if (a.Equals(b))
            //    return a;

            //System.Console.Out.WriteLine( "## have to AND" );
            return new AND(a, b);
        }

        [CLSCompliant(false)]
        public static SemanticContext Or(SemanticContext a, SemanticContext b)
        {
            if (a is TruePredicate || b is TruePredicate)
                return TruePredicate.Instance;

            SemanticContext commonTerms = FactorAnd(ref a, ref b);
            bool factored = commonTerms != null && commonTerms != EmptySemanticContext && !(commonTerms is FalsePredicate);
            if (factored)
                return And(commonTerms, Or(a, b));

            //System.Console.Out.WriteLine( "OR: " + a + "||" + b );
            if (a == EmptySemanticContext || a == null || a is FalsePredicate)
                return factored ? Or(commonTerms, b) : b;

            if (b == EmptySemanticContext || b == null || b is FalsePredicate)
                return factored ? Or(commonTerms, a) : a;

            if (a is TruePredicate || b is TruePredicate || commonTerms is TruePredicate)
                return TruePredicate.Instance;
            else if (b is FalsePredicate)
                return a;
            else if (a is FalsePredicate)
                return b;

            //// Factoring takes care of this case
            //if (a.Equals(b))
            //    return a;

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
            OR result = new OR(a, b);
            if (result.Operands.Count == 1)
                return result.Operands.First();

            return result;
        }

        [CLSCompliant(false)]
        public static SemanticContext Not(SemanticContext a)
        {
            NOT nota = a as NOT;
            if (nota != null)
                return nota.ctx;

            if (a is TruePredicate)
                return FalsePredicate.Instance;
            else if (a is FalsePredicate)
                return TruePredicate.Instance;

            return new NOT(a);
        }

        // Factor so (a && b) == (result && a && b)
        public static SemanticContext FactorAnd(ref SemanticContext a, ref SemanticContext b)
        {
            if (a == EmptySemanticContext || a == null || a is FalsePredicate)
                return EmptySemanticContext;
            if (b == EmptySemanticContext || b == null || b is FalsePredicate)
                return EmptySemanticContext;

            if (a is TruePredicate || b is TruePredicate)
            {
                a = EmptySemanticContext;
                b = EmptySemanticContext;
                return TruePredicate.Instance;
            }

            HashSet<SemanticContext> opsA = new HashSet<SemanticContext>(GetAndOperands(a));
            HashSet<SemanticContext> opsB = new HashSet<SemanticContext>(GetAndOperands(b));

            HashSet<SemanticContext> result = new HashSet<SemanticContext>(opsA);
            result.IntersectWith(opsB);
            if (result.Count == 0)
                return EmptySemanticContext;

            opsA.ExceptWith(result);
            if (opsA.Count == 0)
                a = TruePredicate.Instance;
            else if (opsA.Count == 1)
                a = opsA.First();
            else
                a = new AND(opsA);

            opsB.ExceptWith(result);
            if (opsB.Count == 0)
                b = TruePredicate.Instance;
            else if (opsB.Count == 1)
                b = opsB.First();
            else
                b = new AND(opsB);

            if (result.Count == 1)
                return result.First();

            return new AND(result);
        }

        // Factor so (a || b) == (result || a || b)
        public static SemanticContext FactorOr(ref SemanticContext a, ref SemanticContext b)
        {
            HashSet<SemanticContext> opsA = new HashSet<SemanticContext>(GetOrOperands(a));
            HashSet<SemanticContext> opsB = new HashSet<SemanticContext>(GetOrOperands(b));

            HashSet<SemanticContext> result = new HashSet<SemanticContext>(opsA);
            result.IntersectWith(opsB);
            if (result.Count == 0)
                return EmptySemanticContext;

            opsA.ExceptWith(result);
            if (opsA.Count == 0)
                a = FalsePredicate.Instance;
            else if (opsA.Count == 1)
                a = opsA.First();
            else
                a = new OR(opsA);

            opsB.ExceptWith(result);
            if (opsB.Count == 0)
                b = FalsePredicate.Instance;
            else if (opsB.Count == 1)
                b = opsB.First();
            else
                b = new OR(opsB);

            if (result.Count == 1)
                return result.First();

            return new OR(result);
        }

        public static IEnumerable<SemanticContext> GetAndOperands(SemanticContext context)
        {
            AND and = context as AND;
            if (and != null)
                return and.Operands;

            NOT not = context as NOT;
            if (not != null)
                return GetOrOperands(not.ctx).Select(Not);

            return new SemanticContext[] { context };
        }

        public static IEnumerable<SemanticContext> GetOrOperands(SemanticContext context)
        {
            OR or = context as OR;
            if (or != null)
                return or.Operands;

            NOT not = context as NOT;
            if (not != null)
                return GetAndOperands(not.ctx).Select(Not);

            return new SemanticContext[] { context };
        }
    }
}
