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
    using Antlr3.Codegen;

    using ANTLRStringStream = Antlr.Runtime.ANTLRStringStream;
    using ArrayList = System.Collections.ArrayList;
    using Attribute = Antlr3.Tool.Attribute;
    using AttributeScope = Antlr3.Tool.AttributeScope;
    using CommonToken = Antlr.Runtime.CommonToken;
    using ErrorManager = Antlr3.Tool.ErrorManager;
    using Grammar = Antlr3.Tool.Grammar;
    using GrammarAST = Antlr3.Tool.GrammarAST;
    using IList = System.Collections.IList;
    using IToken = Antlr.Runtime.IToken;
    using Rule = Antlr3.Tool.Rule;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr3.ST.StringTemplate;
    using TokenConstants = Antlr.Runtime.TokenConstants;

    partial class ActionTranslator
    {
        public IList chunks = new ArrayList();
        Rule enclosingRule;
        int outerAltNum;
        Grammar grammar;
        CodeGenerator generator;
        IToken actionToken;

        public ActionTranslator( CodeGenerator generator,
                                     string ruleName,
                                     GrammarAST actionAST )
            : this( new ANTLRStringStream( actionAST.token.Text ) )
        {
            this.generator = generator;
            this.grammar = generator.grammar;
            this.enclosingRule = grammar.getLocallyDefinedRule( ruleName );
            this.actionToken = actionAST.token;
            this.outerAltNum = actionAST.outerAltNum;
        }

        public ActionTranslator( CodeGenerator generator,
                                     string ruleName,
                                     IToken actionToken,
                                     int outerAltNum )
            : this( new ANTLRStringStream( actionToken.Text ) )
        {
            this.generator = generator;
            grammar = generator.grammar;
            this.enclosingRule = grammar.getRule( ruleName );
            this.actionToken = actionToken;
            this.outerAltNum = outerAltNum;
        }

        /** Return a list of strings and StringTemplate objects that
         *  represent the translated action.
         */
        public IList translateToChunks()
        {
            // JSystem.@out.println("###\naction="+action);
            IToken t;
            do
            {
                t = NextToken();
            } while ( t.Type != TokenConstants.Eof );
            return chunks;
        }

        public string translate()
        {
            IList theChunks = translateToChunks();
            //JSystem.@out.println("chunks="+a.chunks);
            StringBuilder buf = new StringBuilder();
            for ( int i = 0; i < theChunks.Count; i++ )
            {
                object o = (object)theChunks[i];
                buf.Append( o );
            }
            //JSystem.@out.println("translated: "+buf.toString());
            return buf.ToString();
        }

        public IList translateAction( string action )
        {
            string rname = null;
            if ( enclosingRule != null )
            {
                rname = enclosingRule.name;
            }
            ActionTranslator translator =
                new ActionTranslator( generator,
                                          rname,
                                          new CommonToken( ANTLRParser.ACTION, action ), outerAltNum );
            return translator.translateToChunks();
        }

        public bool isTokenRefInAlt( string id )
        {
            return enclosingRule.getTokenRefsInAlt( id, outerAltNum ) != null;
        }
        public bool isRuleRefInAlt( string id )
        {
            return enclosingRule.getRuleRefsInAlt( id, outerAltNum ) != null;
        }
        public Grammar.LabelElementPair getElementLabel( string id )
        {
            return enclosingRule.getLabel( id );
        }

        public void checkElementRefUniqueness( string @ref, bool isToken )
        {
            IList refs = null;
            if ( isToken )
            {
                refs = enclosingRule.getTokenRefsInAlt( @ref, outerAltNum );
            }
            else
            {
                refs = enclosingRule.getRuleRefsInAlt( @ref, outerAltNum );
            }
            if ( refs != null && refs.Count > 1 )
            {
                ErrorManager.grammarError( ErrorManager.MSG_NONUNIQUE_REF,
                                          grammar,
                                          actionToken,
                                          @ref );
            }
        }

        /** For \$rulelabel.name, return the Attribute found for name.  It
         *  will be a predefined property or a return value.
         */
        public Attribute getRuleLabelAttribute( string ruleName, string attrName )
        {
            Rule r = grammar.getRule( ruleName );
            AttributeScope scope = r.getLocalAttributeScope( attrName );
            if ( scope != null && !scope.isParameterScope )
            {
                return scope.getAttribute( attrName );
            }
            return null;
        }

        AttributeScope resolveDynamicScope( string scopeName )
        {
            if ( grammar.getGlobalScope( scopeName ) != null )
            {
                return grammar.getGlobalScope( scopeName );
            }
            Rule scopeRule = grammar.getRule( scopeName );
            if ( scopeRule != null )
            {
                return scopeRule.ruleScope;
            }
            return null; // not a valid dynamic scope
        }

        protected StringTemplate template( string name )
        {
            StringTemplate st = generator.Templates.GetInstanceOf( name );
            chunks.Add( st );
            return st;
        }
    }
}
