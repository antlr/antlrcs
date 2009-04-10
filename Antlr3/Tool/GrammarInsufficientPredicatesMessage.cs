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
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.Analysis;

    using IToken = Antlr.Runtime.IToken;
    using StringTemplate = Antlr3.ST.StringTemplate;

    public class GrammarInsufficientPredicatesMessage : Message
    {
        public DecisionProbe probe;
        public IDictionary<int, ICollection<IToken>> altToLocations;
        public DFAState problemState;

        public GrammarInsufficientPredicatesMessage( DecisionProbe probe,
                                                    DFAState problemState,
                                                    IDictionary<int, ICollection<IToken>> altToLocations )
            : base( ErrorManager.MSG_INSUFFICIENT_PREDICATES )
        {
            this.probe = probe;
            this.problemState = problemState;
            this.altToLocations = altToLocations;
        }

        public override string ToString()
        {
            GrammarAST decisionASTNode = probe.dfa.DecisionASTNode;
            line = decisionASTNode.Line;
            charPositionInLine = decisionASTNode.CharPositionInLine;
            string fileName = probe.dfa.nfa.grammar.FileName;
            if ( fileName != null )
            {
                file = fileName;
            }
            StringTemplate st = getMessageTemplate();
            // convert to string key to avoid 3.1 ST bug
            var altToLocationsWithStringKey = new SortedList<string, ICollection<IToken>>();
            List<int> alts = new List<int>();
            alts.addAll( altToLocations.Keys );
            alts.Sort();
            foreach ( int altI in alts )
            {
                altToLocationsWithStringKey[altI.ToString()] = altToLocations.get( altI );
                //List<string> tokens = new List<string>();
                //foreach ( IToken t in altToLocations.get( altI ) )
                //{
                //    tokens.Add( t.ToString() );
                //}
                //tokens.Sort();
                //System.Console.Out.WriteLine( "tokens=\n" + tokens );
            }
            st.SetAttribute( "altToLocations", altToLocationsWithStringKey );

            var sampleInputLabels = problemState.dfa.probe.getSampleNonDeterministicInputSequence( problemState );
            string input = problemState.dfa.probe.getInputSequenceDisplay( sampleInputLabels );
            st.SetAttribute( "upon", input );

            st.SetAttribute( "hasPredicateBlockedByAction", problemState.dfa.hasPredicateBlockedByAction );

            return base.ToString( st );
        }

    }
}
