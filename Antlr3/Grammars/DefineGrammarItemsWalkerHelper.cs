/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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

namespace Antlr3.Grammars
{
    using System;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using Antlr3.Tool;

    partial class DefineGrammarItemsWalker
    {
        protected Grammar grammar;
        protected GrammarAST root;
        protected String currentRuleName;
        protected GrammarAST currentRewriteBlock;
        protected GrammarAST currentRewriteRule;
        protected int outerAltNum = 0;
        protected int blockLevel = 0;

        public int CountAltsForRule( CommonTree t )
        {
            CommonTree block = (CommonTree)t.GetFirstChildWithType(BLOCK);
            if (block.ChildCount == 0)
                return 0;

            return block.Children.Count(i => i.Type == ALT);
        }

        protected void Finish()
        {
            TrimGrammar();
        }

        /** Remove any lexer rules from a COMBINED; already passed to lexer */
        protected void TrimGrammar()
        {
            if ( grammar.type != GrammarType.Combined )
            {
                return;
            }
            // form is (header ... ) ( grammar ID (scope ...) ... ( rule ... ) ( rule ... ) ... )
            GrammarAST p = root;
            // find the grammar spec
            while ( !p.Text.Equals( "grammar" ) )
            {
                p = (GrammarAST)p.Parent.GetChild(p.ChildIndex + 1);
            }
            for ( int i = 0; i < p.ChildCount; i++ )
            {
                if ( p.GetChild( i ).Type != RULE )
                    continue;

                string ruleName = p.GetChild( i ).GetChild( 0 ).Text;
                //Console.Out.WriteLine( "rule " + ruleName + " prev=" + prev.getText() );
                if (Rule.GetRuleType(ruleName) == RuleType.Lexer)
                {
                    // remove lexer rule
                    p.DeleteChild( i );
                    i--;
                }
            }
            //Console.Out.WriteLine( "root after removal is: " + root.ToStringList() );
        }

        protected void TrackInlineAction( GrammarAST actionAST )
        {
            Rule r = grammar.GetRule( currentRuleName );
            if ( r != null )
            {
                r.TrackInlineAction( actionAST );
            }
        }
    }
}
