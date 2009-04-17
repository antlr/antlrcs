/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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
    using Antlr.Runtime.Tree;
    using Antlr3.Grammars;

    using Label = Antlr3.Analysis.Label;

    /** Move all of the functionality from assign.types.g grammar file. */
    public class AssignTokenTypesBehavior : AssignTokenTypesWalker
    {
        protected const int UNASSIGNED = -1;
        protected const int UNASSIGNED_IN_PARSER_RULE = -2;

        protected IDictionary<string, int> stringLiterals = new SortedList<string, int>();
        protected IDictionary<string, int> tokens = new SortedList<string, int>();
        protected IDictionary<string, string> aliases = new SortedList<string, string>();
        protected IDictionary<string, string> aliasesReverseIndex = new Dictionary<string, string>();

        public AssignTokenTypesBehavior( ITreeNodeStream input )
            : base( input )
        {
        }

        /** Track actual lexer rule defs so we don't get repeated token defs in
         *  generated lexer.
         */
        protected HashSet<string> tokenRuleDefs = new HashSet<string>();

        protected override void Init( Grammar g )
        {
            this.grammar = g;
            currentRuleName = null;
            if ( stringAlias == null )
            {
                // only init once; can't statically init since we need astFactory
                InitASTPatterns();
            }
        }

        /** Track string literals (could be in tokens{} section) */
        protected override void TrackString( GrammarAST t )
        {
            // if lexer, don't allow aliasing in tokens section
            if ( currentRuleName == null && grammar.type == Grammar.LEXER )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_CANNOT_ALIAS_TOKENS_IN_LEXER,
                                          grammar,
                                          t.token,
                                          t.Text );
                return;
            }
            // in a plain parser grammar rule, cannot reference literals
            // (unless defined previously via tokenVocab option)
            // don't warn until we hit root grammar as may be defined there.
            if ( grammar.IsRoot &&
                 grammar.type == Grammar.PARSER &&
                 grammar.GetTokenType( t.Text ) == Label.INVALID )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE,
                                          grammar,
                                          t.token,
                                          t.Text );
            }
            // Don't record literals for lexers, they are things to match not tokens
            if ( grammar.type == Grammar.LEXER )
            {
                return;
            }
            // otherwise add literal to token types if referenced from parser rule
            // or in the tokens{} section
            if ( ( currentRuleName == null ||
                  char.IsLower( currentRuleName[0] ) ) &&
                                                                    grammar.GetTokenType( t.Text ) == Label.INVALID )
            {
                stringLiterals[t.Text] = UNASSIGNED_IN_PARSER_RULE;
            }
        }

        protected override void TrackToken( GrammarAST t )
        {
            // imported token names might exist, only add if new
            // Might have ';'=4 in vocab import and SEMI=';'. Avoid
            // setting to UNASSIGNED if we have loaded ';'/SEMI
            if ( grammar.GetTokenType( t.Text ) == Label.INVALID &&
                 !tokens.ContainsKey( t.Text ) )
            {
                tokens[t.Text] = UNASSIGNED;
            }
        }

        protected override void TrackTokenRule( GrammarAST t,
                                      GrammarAST modifier,
                                      GrammarAST block )
        {
            // imported token names might exist, only add if new
            if ( grammar.type == Grammar.LEXER || grammar.type == Grammar.COMBINED )
            {
                if ( !char.IsUpper( t.Text[0] ) )
                {
                    return;
                }
                if ( t.Text.Equals( Grammar.ARTIFICIAL_TOKENS_RULENAME ) )
                {
                    // don't add Tokens rule
                    return;
                }

                // track all lexer rules so we can look for token refs w/o
                // associated lexer rules.
                grammar.composite.lexerRules.Add( t.Text );

                int existing = grammar.GetTokenType( t.Text );
                if ( existing == Label.INVALID )
                {
                    tokens[t.Text] = UNASSIGNED;
                }
                // look for "<TOKEN> : <literal> ;" pattern
                // (can have optional action last)
                if ( block.HasSameTreeStructure( charAlias ) ||
                     block.HasSameTreeStructure( stringAlias ) ||
                     block.HasSameTreeStructure( charAlias2 ) ||
                     block.HasSameTreeStructure( stringAlias2 ) )
                {
                    tokenRuleDefs.Add( t.Text );
                    /*
                Grammar parent = grammar.composite.getDelegator(grammar);
                boolean importedByParserOrCombined =
                    parent!=null &&
                    (parent.type==Grammar.LEXER||parent.type==Grammar.PARSER);
                    */
                    if ( grammar.type == Grammar.COMBINED || grammar.type == Grammar.LEXER )
                    {
                        // only call this rule an alias if combined or lexer
                        Alias( t, (GrammarAST)block.GetChild( 0 ).GetChild( 0 ) );
                    }
                }
            }
            // else error
        }

        protected override void Alias( GrammarAST t, GrammarAST s )
        {
            string tokenID = t.Text;
            string literal = s.Text;
            string prevAliasLiteralID = aliasesReverseIndex.get( literal );
            if ( prevAliasLiteralID != null )
            { // we've seen this literal before
                if ( tokenID.Equals( prevAliasLiteralID ) )
                {
                    // duplicate but identical alias; might be tokens {A='a'} and
                    // lexer rule A : 'a' ;  Is ok, just return
                    return;
                }

                // give error unless both are rules (ok if one is in tokens section)
                if ( !( tokenRuleDefs.Contains( tokenID ) && tokenRuleDefs.Contains( prevAliasLiteralID ) ) )
                {
                    // don't allow alias if A='a' in tokens section and B : 'a'; is rule.
                    // Allow if both are rules.  Will get DFA nondeterminism error later.
                    ErrorManager.GrammarError( ErrorManager.MSG_TOKEN_ALIAS_CONFLICT,
                                              grammar,
                                              t.token,
                                              tokenID + "=" + literal,
                                              prevAliasLiteralID );
                }
                return; // don't do the alias
            }
            int existingLiteralType = grammar.GetTokenType( literal );
            if ( existingLiteralType != Label.INVALID )
            {
                // we've seen this before from a tokenVocab most likely
                // don't assign a new token type; use existingLiteralType.
                tokens[tokenID] = existingLiteralType;
            }
            string prevAliasTokenID = aliases.get( tokenID );
            if ( prevAliasTokenID != null )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_TOKEN_ALIAS_REASSIGNMENT,
                                          grammar,
                                          t.token,
                                          tokenID + "=" + literal,
                                          prevAliasTokenID );
                return; // don't do the alias
            }
            aliases[tokenID] = literal;
            aliasesReverseIndex[literal] = tokenID;
        }

        protected internal override void DefineTokens( Grammar root )
        {
            //System.Console.Out.WriteLine( "stringLiterals=" + stringLiterals );
            //System.Console.Out.WriteLine( "tokens=" + tokens );
            //System.Console.Out.WriteLine( "aliases=" + aliases );
            //System.Console.Out.WriteLine( "aliasesReverseIndex=" + aliasesReverseIndex );

            AssignTokenIDTypes( root );

            AliasTokenIDsAndLiterals( root );

            AssignStringTypes( root );

            //System.Console.Out.WriteLine( "stringLiterals=" + stringLiterals );
            //System.Console.Out.WriteLine( "tokens=" + tokens );
            //System.Console.Out.WriteLine( "aliases=" + aliases );
            DefineTokenNamesAndLiteralsInGrammar( root );
        }

#if false
        protected virtual void defineStringLiteralsFromDelegates()
        {
            if ( grammar.IsRoot && grammar.type == Grammar.COMBINED )
            {
                IList<Grammar> delegates = grammar.getDelegates();
                System.Console.Out.WriteLine( "delegates in master combined: " + delegates );
                for ( int i = 0; i < delegates.size(); i++ )
                {
                    Grammar d = (Grammar)delegates.get( i );
                    var literals = d.StringLiterals;
                    foreach ( string literal in literals )
                    {
                        System.Console.Out.WriteLine( "literal " + literal );
                        int ttype = grammar.getTokenType( literal );
                        grammar.defineLexerRuleForStringLiteral( literal, ttype );
                    }
                }
            }
        }
#endif

        protected override void AssignStringTypes( Grammar root )
        {
            // walk string literals assigning types to unassigned ones
            foreach ( var literal in stringLiterals.Where( pair => pair.Value < Label.MIN_TOKEN_TYPE ).ToArray() )
            {
                int type = root.GetNewTokenType();
                stringLiterals[literal.Key] = type;
                // if string referenced in combined grammar parser rule,
                // automatically define in the generated lexer
                root.DefineLexerRuleForStringLiteral( literal.Key, type );
            }
        }

        protected override void AliasTokenIDsAndLiterals( Grammar root )
        {
            if ( root.type == Grammar.LEXER )
            {
                return; // strings/chars are never token types in LEXER
            }
            // walk aliases if any and assign types to aliased literals if literal
            // was referenced
            foreach ( var alias in aliases )
            {
                string tokenID = alias.Key;
                string literal = alias.Value;
                if ( literal[0] == '\'' && stringLiterals.ContainsKey( literal ) )
                {
                    stringLiterals[literal] = tokens.get( tokenID );
                    // an alias still means you need a lexer rule for it
                    int typeI = (int)tokens.get( tokenID );
                    if ( !tokenRuleDefs.Contains( tokenID ) )
                    {
                        root.DefineLexerRuleForAliasedStringLiteral( tokenID, literal, typeI );
                    }
                }
            }
        }

        protected override void AssignTokenIDTypes( Grammar root )
        {
            // walk token names, assigning values if unassigned
            foreach ( var token in tokens.Where( pair => pair.Value == UNASSIGNED ).ToArray() )
            {
                tokens[token.Key] = root.GetNewTokenType();
            }
        }

        protected override void DefineTokenNamesAndLiteralsInGrammar( Grammar root )
        {
            foreach ( var token in tokens )
            {
                root.DefineToken( token.Key, token.Value );
            }

            foreach ( var lit in stringLiterals )
            {
                root.DefineToken( lit.Key, lit.Value );
            }
        }

    }
}
