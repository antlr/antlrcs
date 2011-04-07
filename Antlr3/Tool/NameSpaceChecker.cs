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

namespace Antlr3.Tool
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.JavaExtensions;

    using IToken = Antlr.Runtime.IToken;
    using Label = Antlr3.Analysis.Label;

    public class NameSpaceChecker
    {
        protected Grammar grammar;

        public NameSpaceChecker( Grammar grammar )
        {
            this.grammar = grammar;
        }

        public virtual void CheckConflicts()
        {
            for ( int i = CompositeGrammar.MinRuleIndex; i < grammar.composite.ruleIndexToRuleList.Count; i++ )
            {
                Rule r = grammar.composite.ruleIndexToRuleList[i];
                if ( r == null )
                {
                    continue;
                }
                // walk all labels for Rule r
                if ( r.LabelNameSpace != null )
                {
                    foreach ( Grammar.LabelElementPair pair in r.LabelNameSpace.Values )
                    {
                        CheckForLabelConflict( r, pair.label );
                    }
                }
                // walk rule scope attributes for Rule r
                if ( r.RuleScope != null )
                {
                    var attributes = r.RuleScope.Attributes;
                    for ( int j = 0; j < attributes.Count; j++ )
                    {
                        Attribute attribute = (Attribute)attributes.ElementAt( j );
                        CheckForRuleScopeAttributeConflict( r, attribute );
                    }
                }
                CheckForRuleDefinitionProblems( r );
                CheckForRuleArgumentAndReturnValueConflicts( r );
            }
            // check all global scopes against tokens
            foreach ( AttributeScope scope in grammar.GlobalScopes.Values )
            {
                CheckForGlobalScopeTokenConflict( scope );
            }
            // check for missing rule, tokens
            LookForReferencesToUndefinedSymbols();
        }

        protected virtual void CheckForRuleArgumentAndReturnValueConflicts( Rule r )
        {
            if ( r.ReturnScope != null )
            {
                HashSet<object> conflictingKeys = r.ReturnScope.Intersection( r.ParameterScope );
                if ( conflictingKeys != null )
                {
                    foreach ( string key in conflictingKeys )
                    {
                        ErrorManager.GrammarError(
                            ErrorManager.MSG_ARG_RETVAL_CONFLICT,
                            grammar,
                            r.Tree.Token,
                            key,
                            r.Name );
                    }
                }
            }
        }

        protected virtual void CheckForRuleDefinitionProblems( Rule r )
        {
            string ruleName = r.Name;
            IToken ruleToken = r.Tree.Token;
            int msgID = 0;
            if ( ( grammar.type == GrammarType.Parser || grammar.type == GrammarType.TreeParser ) &&
                 Rule.GetRuleType(ruleName) == RuleType.Lexer)
            {
                msgID = ErrorManager.MSG_LEXER_RULES_NOT_ALLOWED;
            }
            else if ( grammar.type == GrammarType.Lexer &&
                      Rule.GetRuleType(ruleName) == RuleType.Parser &&
                      !r.IsSynPred )
            {
                msgID = ErrorManager.MSG_PARSER_RULES_NOT_ALLOWED;
            }
            else if ( grammar.GetGlobalScope( ruleName ) != null )
            {
                msgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            }
            if ( msgID != 0 )
            {
                ErrorManager.GrammarError( msgID, grammar, ruleToken, ruleName );
            }
        }

        /** If ref to undefined rule, give error at first occurrence.
         * 
         *  Give error if you cannot find the scope override on a rule reference.
         *
         *  If you ref ID in a combined grammar and don't define ID as a lexer rule
         *  it is an error.
         */
        protected virtual void LookForReferencesToUndefinedSymbols()
        {
            // for each rule ref, ask if there is a rule definition
            foreach ( GrammarAST refAST in grammar.ruleRefs )
            {
                IToken tok = refAST.Token;
                string ruleName = tok.Text;
                Rule localRule = grammar.GetLocallyDefinedRule( ruleName );
                Rule rule = grammar.GetRule( ruleName );
                if ( localRule == null && rule != null )
                { // imported rule?
                    grammar.delegatedRuleReferences.Add( rule );
                    rule.Imported = true;
                }
                if ( rule == null && grammar.GetTokenType( ruleName ) != Label.EOF )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_UNDEFINED_RULE_REF,
                                              grammar,
                                              tok,
                                              ruleName );
                }
            }
            if ( grammar.type == GrammarType.Combined )
            {
                // if we're a combined grammar, we know which token IDs have no
                // associated lexer rule.
                foreach ( IToken tok in grammar.tokenIDRefs )
                {
                    string tokenID = tok.Text;
                    if ( !grammar.composite.lexerRules.Contains( tokenID ) &&
                         grammar.GetTokenType( tokenID ) != Label.EOF )
                    {
                        ErrorManager.GrammarWarning( ErrorManager.MSG_NO_TOKEN_DEFINITION,
                                                    grammar,
                                                    tok,
                                                    tokenID );
                    }
                }
            }
            // check scopes and scoped rule refs
            foreach ( GrammarAST scopeAST in grammar.scopedRuleRefs )
            {
                // ^(DOT ID atom)
                Grammar scopeG = grammar.composite.GetGrammar( scopeAST.Text );
                GrammarAST refAST = (GrammarAST)scopeAST.GetChild( 1 );
                string ruleName = refAST.Text;
                if ( scopeG == null )
                {
                    ErrorManager.GrammarError( ErrorManager.MSG_NO_SUCH_GRAMMAR_SCOPE,
                                              grammar,
                                              scopeAST.Token,
                                              scopeAST.Text,
                                              ruleName );
                }
                else
                {
                    Rule rule = grammar.GetRule( scopeG.name, ruleName );
                    if ( rule == null )
                    {
                        ErrorManager.GrammarError( ErrorManager.MSG_NO_SUCH_RULE_IN_SCOPE,
                                                  grammar,
                                                  scopeAST.Token,
                                                  scopeAST.Text,
                                                  ruleName );
                    }
                }
            }
        }

        protected virtual void CheckForGlobalScopeTokenConflict( AttributeScope scope )
        {
            if ( grammar.GetTokenType( scope.Name ) != Label.INVALID )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE,
                                          grammar, null, scope.Name );
            }
        }

        /** Check for collision of a rule-scope dynamic attribute with:
         *  arg, return value, rule name itself.  Labels are checked elsewhere.
         */
        public virtual void CheckForRuleScopeAttributeConflict( Rule r, Attribute attribute )
        {
            int msgID = 0;
            object arg2 = null;
            string attrName = attribute.Name;
            if ( r.Name.Equals( attrName ) )
            {
                msgID = ErrorManager.MSG_ATTRIBUTE_CONFLICTS_WITH_RULE;
                arg2 = r.Name;
            }
            else if ( ( r.ReturnScope != null && r.ReturnScope.GetAttribute( attrName ) != null ) ||
                      ( r.ParameterScope != null && r.ParameterScope.GetAttribute( attrName ) != null ) )
            {
                msgID = ErrorManager.MSG_ATTRIBUTE_CONFLICTS_WITH_RULE_ARG_RETVAL;
                arg2 = r.Name;
            }
            if ( msgID != 0 )
            {
                ErrorManager.GrammarError( msgID, grammar, r.Tree.Token, attrName, arg2 );
            }
        }

        /** Make sure a label doesn't conflict with another symbol.
         *  Labels must not conflict with: rules, tokens, scope names,
         *  return values, parameters, and rule-scope dynamic attributes
         *  defined in surrounding rule.
         */
        protected virtual void CheckForLabelConflict( Rule r, IToken label )
        {
            int msgID = 0;
            object arg2 = null;
            if ( grammar.GetGlobalScope( label.Text ) != null )
            {
                msgID = ErrorManager.MSG_SYMBOL_CONFLICTS_WITH_GLOBAL_SCOPE;
            }
            else if ( grammar.GetRule( label.Text ) != null )
            {
                msgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE;
            }
            else if ( grammar.GetTokenType( label.Text ) != Label.INVALID )
            {
                msgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_TOKEN;
            }
            else if ( r.RuleScope != null && r.RuleScope.GetAttribute( label.Text ) != null )
            {
                msgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE_SCOPE_ATTRIBUTE;
                arg2 = r.Name;
            }
            else if ( ( r.ReturnScope != null && r.ReturnScope.GetAttribute( label.Text ) != null ) ||
                      ( r.ParameterScope != null && r.ParameterScope.GetAttribute( label.Text ) != null ) )
            {
                msgID = ErrorManager.MSG_LABEL_CONFLICTS_WITH_RULE_ARG_RETVAL;
                arg2 = r.Name;
            }
            if ( msgID != 0 )
            {
                ErrorManager.GrammarError( msgID, grammar, label, label.Text, arg2 );
            }
        }

        /** If type of previous label differs from new label's type, that's an error.
         */
        public virtual bool CheckForLabelTypeMismatch( Rule r, IToken label, LabelType type )
        {
            Grammar.LabelElementPair prevLabelPair =
                (Grammar.LabelElementPair)r.LabelNameSpace.get( label.Text );
            if ( prevLabelPair != null )
            {
                // label already defined; if same type, no problem
                if ( prevLabelPair.type != type )
                {
                    string typeMismatchExpr =
                        Grammar.LabelTypeToString[(int)type] + "!=" +
                        Grammar.LabelTypeToString[(int)prevLabelPair.type];
                    ErrorManager.GrammarError(
                        ErrorManager.MSG_LABEL_TYPE_CONFLICT,
                        grammar,
                        label,
                        label.Text,
                        typeMismatchExpr );
                    return true;
                }
            }
            return false;
        }
    }
}
