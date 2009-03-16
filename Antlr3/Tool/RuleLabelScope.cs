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
    using IToken = Antlr.Runtime.IToken;

    public class RuleLabelScope : AttributeScope
    {
        /** Rules have a predefined set of attributes as well as
         *  the return values.  'text' needs to be computed though so.
         */
        public static AttributeScope predefinedRulePropertiesScope;

        public static AttributeScope predefinedTreeRulePropertiesScope;

        public static AttributeScope predefinedLexerRulePropertiesScope;

        public static AttributeScope[] grammarTypeToRulePropertiesScope;

        static RuleLabelScope()
        {
            predefinedRulePropertiesScope = new AttributeScope( "RulePredefined", null );
            predefinedRulePropertiesScope.addAttribute( "text", null );
            predefinedRulePropertiesScope.addAttribute( "start", null );
            predefinedRulePropertiesScope.addAttribute( "stop", null );
            predefinedRulePropertiesScope.addAttribute( "tree", null );
            predefinedRulePropertiesScope.addAttribute( "st", null );
            predefinedRulePropertiesScope.isPredefinedRuleScope = true;

            predefinedTreeRulePropertiesScope = new AttributeScope( "RulePredefined", null );
            predefinedTreeRulePropertiesScope.addAttribute( "text", null );
            predefinedTreeRulePropertiesScope.addAttribute( "start", null ); // note: no stop; not meaningful
            predefinedTreeRulePropertiesScope.addAttribute( "tree", null );
            predefinedTreeRulePropertiesScope.addAttribute( "st", null );
            predefinedTreeRulePropertiesScope.isPredefinedRuleScope = true;

            predefinedLexerRulePropertiesScope = new AttributeScope( "LexerRulePredefined", null );
            predefinedLexerRulePropertiesScope.addAttribute( "text", null );
            predefinedLexerRulePropertiesScope.addAttribute( "type", null );
            predefinedLexerRulePropertiesScope.addAttribute( "line", null );
            predefinedLexerRulePropertiesScope.addAttribute( "index", null );
            predefinedLexerRulePropertiesScope.addAttribute( "pos", null );
            predefinedLexerRulePropertiesScope.addAttribute( "channel", null );
            predefinedLexerRulePropertiesScope.addAttribute( "start", null );
            predefinedLexerRulePropertiesScope.addAttribute( "stop", null );
            predefinedLexerRulePropertiesScope.addAttribute( "int", null );
            predefinedLexerRulePropertiesScope.isPredefinedLexerRuleScope = true;

            grammarTypeToRulePropertiesScope =
                new AttributeScope[]
                {
                    null,
                    predefinedLexerRulePropertiesScope, // LEXER
                    predefinedRulePropertiesScope, // PARSER
                    predefinedTreeRulePropertiesScope, // TREE PARSER
                    predefinedRulePropertiesScope // COMBINED
                };

        }

        public Rule referencedRule;

        public RuleLabelScope( Rule referencedRule, IToken actionToken )
            : base( "ref_" + referencedRule.name, actionToken )
        {
            this.referencedRule = referencedRule;
        }

        /** If you label a rule reference, you can access that rule's
         *  return values as well as any predefined attributes.
         */
        public override Attribute getAttribute( string name )
        {
            AttributeScope rulePropertiesScope =
                RuleLabelScope.grammarTypeToRulePropertiesScope[grammar.type];
            if ( rulePropertiesScope.getAttribute( name ) != null )
            {
                return rulePropertiesScope.getAttribute( name );
            }

            if ( referencedRule.returnScope != null )
            {
                return referencedRule.returnScope.getAttribute( name );
            }
            return null;
        }
    }
}
