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
            predefinedRulePropertiesScope.AddAttribute( "text", null );
            predefinedRulePropertiesScope.AddAttribute( "start", null );
            predefinedRulePropertiesScope.AddAttribute( "stop", null );
            predefinedRulePropertiesScope.AddAttribute( "tree", null );
            predefinedRulePropertiesScope.AddAttribute( "st", null );
            predefinedRulePropertiesScope.isPredefinedRuleScope = true;

            predefinedTreeRulePropertiesScope = new AttributeScope( "RulePredefined", null );
            predefinedTreeRulePropertiesScope.AddAttribute( "text", null );
            predefinedTreeRulePropertiesScope.AddAttribute( "start", null ); // note: no stop; not meaningful
            predefinedTreeRulePropertiesScope.AddAttribute( "tree", null );
            predefinedTreeRulePropertiesScope.AddAttribute( "st", null );
            predefinedTreeRulePropertiesScope.isPredefinedRuleScope = true;

            predefinedLexerRulePropertiesScope = new AttributeScope( "LexerRulePredefined", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "text", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "type", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "line", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "index", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "pos", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "channel", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "start", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "stop", null );
            predefinedLexerRulePropertiesScope.AddAttribute( "int", null );
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
        public override Attribute GetAttribute( string name )
        {
            AttributeScope rulePropertiesScope =
                RuleLabelScope.grammarTypeToRulePropertiesScope[grammar.type];
            if ( rulePropertiesScope.GetAttribute( name ) != null )
            {
                return rulePropertiesScope.GetAttribute( name );
            }

            if ( referencedRule.returnScope != null )
            {
                return referencedRule.returnScope.GetAttribute( name );
            }
            return null;
        }
    }
}
