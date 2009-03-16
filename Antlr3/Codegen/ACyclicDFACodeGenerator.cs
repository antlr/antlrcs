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

namespace Antlr3.Codegen
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr3.Analysis;

    using StringTemplate = Antlr3.ST.StringTemplate;
    using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;

    public class ACyclicDFACodeGenerator
    {
        protected CodeGenerator parentGenerator;

        public ACyclicDFACodeGenerator( CodeGenerator parent )
        {
            this.parentGenerator = parent;
        }

        public virtual StringTemplate genFixedLookaheadDecision( StringTemplateGroup templates,
                                                        DFA dfa )
        {
            return walkFixedDFAGeneratingStateMachine( templates, dfa, dfa.startState, 1 );
        }

        protected virtual StringTemplate walkFixedDFAGeneratingStateMachine(
                StringTemplateGroup templates,
                DFA dfa,
                DFAState s,
                int k )
        {
            //System.out.println("walk "+s.stateNumber+" in dfa for decision "+dfa.decisionNumber);
            if ( s.IsAcceptState )
            {
                StringTemplate dfaST2 = templates.GetInstanceOf( "dfaAcceptState" );
                dfaST2.SetAttribute( "alt", s.getUniquelyPredictedAlt() );
                return dfaST2;
            }

            // the default templates for generating a state and its edges
            // can be an if-then-else structure or a switch
            string dfaStateName = "dfaState";
            string dfaLoopbackStateName = "dfaLoopbackState";
            string dfaOptionalBlockStateName = "dfaOptionalBlockState";
            string dfaEdgeName = "dfaEdge";
            if ( parentGenerator.canGenerateSwitch( s ) )
            {
                dfaStateName = "dfaStateSwitch";
                dfaLoopbackStateName = "dfaLoopbackStateSwitch";
                dfaOptionalBlockStateName = "dfaOptionalBlockStateSwitch";
                dfaEdgeName = "dfaEdgeSwitch";
            }

            StringTemplate dfaST = templates.GetInstanceOf( dfaStateName );
            if ( dfa.NFADecisionStartState.decisionStateType == NFAState.LOOPBACK )
            {
                dfaST = templates.GetInstanceOf( dfaLoopbackStateName );
            }
            else if ( dfa.NFADecisionStartState.decisionStateType == NFAState.OPTIONAL_BLOCK_START )
            {
                dfaST = templates.GetInstanceOf( dfaOptionalBlockStateName );
            }
            dfaST.SetAttribute( "k", k );
            dfaST.SetAttribute( "stateNumber", s.stateNumber );
            dfaST.SetAttribute( "semPredState",
                               s.IsResolvedWithPredicates );
            /*
            String description = dfa.getNFADecisionStartState().getDescription();
            description = parentGenerator.target.getTargetStringLiteralFromString(description);
            //System.out.println("DFA: "+description+" associated with AST "+dfa.getNFADecisionStartState());
            if ( description!=null ) {
                dfaST.setAttribute("description", description);
            }
            */
            int EOTPredicts = NFA.INVALID_ALT_NUMBER;
            DFAState EOTTarget = null;
            //System.out.println("DFA state "+s.stateNumber);
            for ( int i = 0; i < s.NumberOfTransitions; i++ )
            {
                Transition edge = (Transition)s.transition( i );
                //System.out.println("edge "+s.stateNumber+"-"+edge.label.toString()+"->"+edge.target.stateNumber);
                if ( edge.label.Atom == Label.EOT )
                {
                    // don't generate a real edge for EOT; track alt EOT predicts
                    // generate that prediction in the else clause as default case
                    EOTTarget = (DFAState)edge.target;
                    EOTPredicts = EOTTarget.getUniquelyPredictedAlt();
                    /*
                    System.out.println("DFA s"+s.stateNumber+" EOT goes to s"+
                                       edge.target.stateNumber+" predicates alt "+
                                       EOTPredicts);
                    */
                    continue;
                }
                StringTemplate edgeST = templates.GetInstanceOf( dfaEdgeName );
                // If the template wants all the label values delineated, do that
                if ( edgeST.GetFormalArgument( "labels" ) != null )
                {
                    List<string> labels = edge.Label.Set.Select( value => parentGenerator.getTokenTypeAsTargetLabel( value ) ).ToList();
                    edgeST.SetAttribute( "labels", labels );
                }
                else
                { // else create an expression to evaluate (the general case)
                    edgeST.SetAttribute( "labelExpr",
                                        parentGenerator.genLabelExpr( templates, edge, k ) );
                }

                // stick in any gated predicates for any edge if not already a pred
                if ( !edge.label.IsSemanticPredicate )
                {
                    DFAState target = (DFAState)edge.target;
                    SemanticContext preds =
                        target.getGatedPredicatesInNFAConfigurations();
                    if ( preds != null )
                    {
                        //System.out.println("preds="+target.getGatedPredicatesInNFAConfigurations());
                        StringTemplate predST = preds.genExpr( parentGenerator,
                                                              parentGenerator.Templates,
                                                              dfa );
                        edgeST.SetAttribute( "predicates", predST );
                    }
                }

                StringTemplate targetST =
                    walkFixedDFAGeneratingStateMachine( templates,
                                                       dfa,
                                                       (DFAState)edge.target,
                                                       k + 1 );
                edgeST.SetAttribute( "targetState", targetST );
                dfaST.SetAttribute( "edges", edgeST );
                /*
                System.out.println("back to DFA "+
                                   dfa.decisionNumber+"."+s.stateNumber);
                                   */
            }

            // HANDLE EOT EDGE
            if ( EOTPredicts != NFA.INVALID_ALT_NUMBER )
            {
                // EOT unique predicts an alt
                dfaST.SetAttribute( "eotPredictsAlt", EOTPredicts );
            }
            else if ( EOTTarget != null && EOTTarget.NumberOfTransitions > 0 )
            {
                // EOT state has transitions so must split on predicates.
                // Generate predicate else-if clauses and then generate
                // NoViableAlt exception as else clause.
                // Note: these predicates emanate from the EOT target state
                // rather than the current DFAState s so the error message
                // might be slightly misleading if you are looking at the
                // state number.  Predicates emanating from EOT targets are
                // hoisted up to the state that has the EOT edge.
                for ( int i = 0; i < EOTTarget.NumberOfTransitions; i++ )
                {
                    Transition predEdge = (Transition)EOTTarget.transition( i );
                    StringTemplate edgeST = templates.GetInstanceOf( dfaEdgeName );
                    edgeST.SetAttribute( "labelExpr",
                                        parentGenerator.genSemanticPredicateExpr( templates, predEdge ) );
                    // the target must be an accept state
                    //System.out.println("EOT edge");
                    StringTemplate targetST =
                        walkFixedDFAGeneratingStateMachine( templates,
                                                           dfa,
                                                           (DFAState)predEdge.target,
                                                           k + 1 );
                    edgeST.SetAttribute( "targetState", targetST );
                    dfaST.SetAttribute( "edges", edgeST );
                }
            }
            return dfaST;
        }
    }
}
