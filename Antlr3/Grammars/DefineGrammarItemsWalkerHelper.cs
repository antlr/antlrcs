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
    using System.Collections.Generic;
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

        private void HandleAttrScope(GrammarAST name, GrammarAST attrs, IDictionary<GrammarAST, GrammarAST> actions)
        {
            AttributeScope scope = grammar.DefineGlobalScope(name.Text, attrs.Token);
            scope.IsDynamicGlobalScope = true;
            scope.AddAttributes(attrs.Text, ';');
            foreach (var action in actions)
                scope.DefineNamedAction(action.Key, action.Value);
        }

        private void HandleAttrScopeAction(GrammarAST id, GrammarAST action, IDictionary<GrammarAST, GrammarAST> actions)
        {
            actions.Add(id, action);
        }

        private void HandleAction(string scope, GrammarAST amp, GrammarAST name, GrammarAST action)
        {
            grammar.DefineNamedAction(amp, scope, name, action);
        }

        private void HandleRuleAfterOptionsSpec(ref string name, ref Rule r, IDictionary<string, object> opts, GrammarAST start, GrammarAST id, string modifier, GrammarAST args, GrammarAST ret, HashSet<string> exceptions)
        {
            name = id.Text;
            currentRuleName = name;
            if (Rule.GetRuleType(name) == RuleType.Lexer && grammar.type == GrammarType.Combined)
            {
                // a merged grammar spec, track lexer rules and send to another grammar
                grammar.DefineLexerRuleFoundInParser(id.Token, start);
            }
            else
            {
                int numAlts = CountAltsForRule(start);
                grammar.DefineRule(id.Token, modifier, opts, start, args, numAlts);
                r = grammar.GetRule(name);
                if (args != null)
                {
                    r.ParameterScope = grammar.CreateParameterScope(name, args.Token);
                    r.ParameterScope.AddAttributes(args.Text, ',');
                }

                if (ret != null)
                {
                    r.ReturnScope = grammar.CreateReturnScope(name, ret.Token);
                    r.ReturnScope.AddAttributes(ret.Text, ',');
                }

                if (exceptions != null)
                {
                    foreach (string exception in exceptions)
                        r.ThrowsSpec.Add(exception);
                }
            }
        }

        private void HandleRuleAfterRuleActions()
        {
            blockLevel = 0;
        }

        private void HandleRuleEnd(GrammarAST blockStart, IDictionary<string, object> opts)
        {
            // copy rule options into the block AST, which is where
            // the analysis will look for k option etc...
            blockStart.BlockOptions = opts;
        }

        private void HandleRuleAction(Rule r, GrammarAST amp, GrammarAST id, GrammarAST action)
        {
            if (r != null)
                r.DefineNamedAction(amp, id, action);
        }

        private void HandleRuleScopeSpecAction(Rule r, GrammarAST attrs, IDictionary<GrammarAST, GrammarAST> actions)
        {
            r.RuleScope = grammar.CreateRuleScope(r.Name, attrs.Token);
            r.RuleScope.IsDynamicRuleScope = true;
            r.RuleScope.AddAttributes(attrs.Text, ';');
            foreach (var action in actions)
                r.RuleScope.DefineNamedAction(action.Key, action.Value);
        }

        private void HandleRuleScopeSpecUses(Rule r, GrammarAST uses)
        {
            if (grammar.GetGlobalScope(uses.Text) == null)
            {
                ErrorManager.GrammarError(ErrorManager.MSG_UNKNOWN_DYNAMIC_SCOPE, grammar, uses.Token, uses.Text);
            }
            else
            {
                if (r.UseScopes == null)
                    r.UseScopes = new List<string>();

                r.UseScopes.Add(uses.Text);
            }
        }

        private void HandleElementAssign(GrammarAST id, GrammarAST elementStart)
        {
            if (elementStart.Type == ANTLRParser.ROOT || elementStart.Type == ANTLRParser.BANG)
            {
                elementStart = (GrammarAST)elementStart.GetChild(0);
            }

            if (elementStart.Type == RULE_REF)
            {
                grammar.DefineRuleRefLabel(currentRuleName, id.Token, elementStart);
            }
            else if (elementStart.Type == WILDCARD && grammar.type == GrammarType.TreeParser)
            {
                grammar.DefineWildcardTreeLabel(currentRuleName, id.Token, elementStart);
            }
            else
            {
                grammar.DefineTokenRefLabel(currentRuleName, id.Token, elementStart);
            }
        }

        private void HandleElementPlusAssign(GrammarAST id, GrammarAST elementStart)
        {
            if (elementStart.Type == ANTLRParser.ROOT || elementStart.Type == ANTLRParser.BANG)
            {
                elementStart = (GrammarAST)elementStart.GetChild(0);
            }

            if (elementStart.Type == RULE_REF)
            {
                grammar.DefineRuleListLabel(currentRuleName, id.Token, elementStart);
            }
            else if (elementStart.Type == WILDCARD && grammar.type == GrammarType.TreeParser)
            {
                grammar.DefineWildcardTreeListLabel(currentRuleName, id.Token, elementStart);
            }
            else
            {
                grammar.DefineTokenListLabel(currentRuleName, id.Token, elementStart);
            }
        }

        private void HandleInlineAction(GrammarAST action)
        {
            action.outerAltNum = outerAltNum;
            TrackInlineAction(action);
        }

        private void HandleDotLoop(GrammarAST start)
        {
            GrammarAST block = (GrammarAST)start.GetChild(0);
            IDictionary<string, object> opts = new Dictionary<string, object>();
            opts["greedy"] = "false";
            if (grammar.type != GrammarType.Lexer)
            {
                // parser grammars assume k=1 for .* loops otherwise they (analysis?) look til EOF!
                opts["k"] = 1;
            }

            block.SetOptions(grammar, opts);
        }

        private void HandleRuleReferenceAtom(GrammarAST scope, GrammarAST ruleReference, GrammarAST action)
        {
            grammar.AltReferencesRule(currentRuleName, scope, ruleReference, this.outerAltNum);
            if (action != null)
            {
                action.outerAltNum = this.outerAltNum;
                TrackInlineAction(action);
            }
        }

        private void HandleTokenReferenceAtom(GrammarAST scope, GrammarAST tokenReference, GrammarAST action)
        {
            if (action != null)
            {
                action.outerAltNum = this.outerAltNum;
                TrackInlineAction(action);
            }

            if (grammar.type == GrammarType.Lexer)
            {
                grammar.AltReferencesRule(currentRuleName, scope, tokenReference, this.outerAltNum);
            }
            else
            {
                grammar.AltReferencesTokenID(currentRuleName, tokenReference, this.outerAltNum);
            }
        }

        private void HandleStringLiteralAtom(GrammarAST literal)
        {
            if (grammar.type != GrammarType.Lexer)
            {
                Rule rule = grammar.GetRule(currentRuleName);
                if (rule != null)
                    rule.TrackTokenReferenceInAlt(literal, outerAltNum);
            }
        }

        private void HandleRewriteAtomStart(GrammarAST start)
        {
            Rule r = grammar.GetRule(currentRuleName);
            var tokenRefsInAlt = r.GetTokenRefsInAlt(outerAltNum);
            bool imaginary = start.Type == TOKEN_REF && !tokenRefsInAlt.Contains(start.Text);
            if (!imaginary && grammar.BuildAST)
            {
                switch (start.Type)
                {
                case RULE_REF:
                case LABEL:
                case TOKEN_REF:
                case CHAR_LITERAL:
                case STRING_LITERAL:
                    // track per block and for entire rewrite rule
                    if (currentRewriteBlock != null)
                    {
                        currentRewriteBlock.rewriteRefsShallow.Add(start);
                        currentRewriteBlock.rewriteRefsDeep.Add(start);
                    }

                    //System.out.println("adding "+$start.Text+" to "+currentRewriteRule.Text);
                    currentRewriteRule.rewriteRefsDeep.Add(start);
                    break;

                default:
                    break;
                }
            }
        }
    }
}
