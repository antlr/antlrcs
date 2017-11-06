/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr3.Tool;

    partial class AssignTokenTypesWalker
    {
        protected internal Grammar grammar;
        protected String currentRuleName;

        protected static GrammarAST stringAlias;
        protected static GrammarAST charAlias;
        protected static GrammarAST stringAlias2;
        protected static GrammarAST charAlias2;

        public override void ReportError( RecognitionException ex )
        {
            IToken token = null;
            if ( ex is MismatchedTokenException )
            {
                token = ( (MismatchedTokenException)ex ).Token;
            }
            else if ( ex is NoViableAltException )
            {
                token = ( (NoViableAltException)ex ).Token;
            }
            ErrorManager.SyntaxError(
                ErrorManager.MSG_SYNTAX_ERROR,
                grammar,
                token,
                "assign.types: " + ex.ToString(),
                ex );
        }


        protected void InitASTPatterns()
        {
            ITreeAdaptor adaptor = new ANTLRParser.grammar_Adaptor( null );

            /*
             * stringAlias = ^(BLOCK[] ^(ALT[] STRING_LITERAL[] EOA[]) EOB[])
             */
            stringAlias = (GrammarAST)adaptor.Create( BLOCK, "BLOCK" );
            {
                GrammarAST alt = (GrammarAST)adaptor.Create( ALT, "ALT" );
                adaptor.AddChild( alt, adaptor.Create( STRING_LITERAL, "STRING_LITERAL" ) );
                adaptor.AddChild( alt, adaptor.Create( EOA, "EOA" ) );
                adaptor.AddChild( stringAlias, alt );
            }
            adaptor.AddChild( stringAlias, adaptor.Create( EOB, "EOB" ) );

            /*
             * charAlias = ^(BLOCK[] ^(ALT[] CHAR_LITERAL[] EOA[]) EOB[])
             */
            charAlias = (GrammarAST)adaptor.Create( BLOCK, "BLOCK" );
            {
                GrammarAST alt = (GrammarAST)adaptor.Create( ALT, "ALT" );
                adaptor.AddChild( alt, adaptor.Create( CHAR_LITERAL, "CHAR_LITERAL" ) );
                adaptor.AddChild( alt, adaptor.Create( EOA, "EOA" ) );
                adaptor.AddChild( charAlias, alt );
            }
            adaptor.AddChild( charAlias, adaptor.Create( EOB, "EOB" ) );

            /*
             * stringAlias2 = ^(BLOCK[] ^(ALT[] STRING_LITERAL[] ACTION[] EOA[]) EOB[])
             */
            stringAlias2 = (GrammarAST)adaptor.Create( BLOCK, "BLOCK" );
            {
                GrammarAST alt = (GrammarAST)adaptor.Create( ALT, "ALT" );
                adaptor.AddChild( alt, adaptor.Create( STRING_LITERAL, "STRING_LITERAL" ) );
                adaptor.AddChild( alt, adaptor.Create( ACTION, "ACTION" ) );
                adaptor.AddChild( alt, adaptor.Create( EOA, "EOA" ) );
                adaptor.AddChild( stringAlias2, alt );
            }
            adaptor.AddChild( stringAlias2, adaptor.Create( EOB, "EOB" ) );

            /*
             * charAlias = ^(BLOCK[] ^(ALT[] CHAR_LITERAL[] ACTION[] EOA[]) EOB[])
             */
            charAlias2 = (GrammarAST)adaptor.Create( BLOCK, "BLOCK" );
            {
                GrammarAST alt = (GrammarAST)adaptor.Create( ALT, "ALT" );
                adaptor.AddChild( alt, adaptor.Create( CHAR_LITERAL, "CHAR_LITERAL" ) );
                adaptor.AddChild( alt, adaptor.Create( ACTION, "ACTION" ) );
                adaptor.AddChild( alt, adaptor.Create( EOA, "EOA" ) );
                adaptor.AddChild( charAlias2, alt );
            }
            adaptor.AddChild( charAlias2, adaptor.Create( EOB, "EOB" ) );
        }

        // Behavior moved to AssignTokenTypesBehavior
        protected virtual void TrackString( GrammarAST t )
        {
        }
        protected virtual void TrackToken( GrammarAST t )
        {
        }
        protected virtual void TrackTokenRule( GrammarAST t, GrammarAST modifier, GrammarAST block )
        {
        }
        protected virtual void Alias( GrammarAST t, GrammarAST s )
        {
        }
        protected internal virtual void DefineTokens( Grammar root )
        {
        }
        protected virtual void DefineStringLiteralsFromDelegates()
        {
        }
        protected virtual void AssignStringTypes( Grammar root )
        {
        }
        protected virtual void AliasTokenIDsAndLiterals( Grammar root )
        {
        }
        protected virtual void AssignTokenIDTypes( Grammar root )
        {
        }
        protected virtual void DefineTokenNamesAndLiteralsInGrammar( Grammar root )
        {
        }
        protected virtual void Init( Grammar root )
        {
        }
    }
}
