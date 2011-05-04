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
    using Antlr3.Grammars;

    using CLSCompliant = System.CLSCompliantAttribute;
    using Label = Antlr3.Analysis.Label;
    using StringComparer = System.StringComparer;

    /** Move all of the functionality from assign.types.g grammar file. */
    [CLSCompliant(false)]
    public class AssignTokenTypesBehavior : AssignTokenTypesWalker
    {
        private const int Unassigned = -1;
        private const int UnassignedInParserRule = -2;

        private readonly IDictionary<string, int> _stringLiterals = new SortedList<string, int>(StringComparer.Ordinal);
        private readonly IDictionary<string, int> _tokens = new SortedList<string, int>(StringComparer.Ordinal);
        private readonly IDictionary<string, string> _aliases = new SortedList<string, string>(StringComparer.Ordinal);
        private readonly IDictionary<string, string> _aliasesReverseIndex = new Dictionary<string, string>();

        /** Track actual lexer rule defs so we don't get repeated token defs in
         *  generated lexer.
         */
        private readonly HashSet<string> tokenRuleDefs = new HashSet<string>();

        public AssignTokenTypesBehavior()
            : base(null)
        {
        }

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
            if ( currentRuleName == null && grammar.type == GrammarType.Lexer )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_CANNOT_ALIAS_TOKENS_IN_LEXER,
                                          grammar,
                                          t.Token,
                                          t.Text );
                return;
            }
            // in a plain parser grammar rule, cannot reference literals
            // (unless defined previously via tokenVocab option)
            // don't warn until we hit root grammar as may be defined there.
            if ( grammar.IsRoot &&
                 grammar.type == GrammarType.Parser &&
                 grammar.GetTokenType( t.Text ) == Label.INVALID )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_LITERAL_NOT_ASSOCIATED_WITH_LEXER_RULE,
                                          grammar,
                                          t.Token,
                                          t.Text );
            }
            // Don't record literals for lexers, they are things to match not tokens
            if ( grammar.type == GrammarType.Lexer )
            {
                return;
            }
            // otherwise add literal to token types if referenced from parser rule
            // or in the tokens{} section
            if ( ( currentRuleName == null ||
                  Rule.GetRuleType(currentRuleName) == RuleType.Parser) &&
                                                                    grammar.GetTokenType( t.Text ) == Label.INVALID )
            {
                _stringLiterals[t.Text] = UnassignedInParserRule;
            }
        }

        protected override void TrackToken( GrammarAST t )
        {
            // imported token names might exist, only add if new
            // Might have ';'=4 in vocab import and SEMI=';'. Avoid
            // setting to UNASSIGNED if we have loaded ';'/SEMI
            if ( grammar.GetTokenType( t.Text ) == Label.INVALID &&
                 !_tokens.ContainsKey( t.Text ) )
            {
                _tokens[t.Text] = Unassigned;
            }
        }

        protected override void TrackTokenRule( GrammarAST t,
                                      GrammarAST modifier,
                                      GrammarAST block )
        {
            // imported token names might exist, only add if new
            if ( grammar.type == GrammarType.Lexer || grammar.type == GrammarType.Combined )
            {
                if (Rule.GetRuleType(t.Text) == RuleType.Parser)
                {
                    return;
                }
                if ( t.Text.Equals( Grammar.ArtificialTokensRuleName ) )
                {
                    // don't add Tokens rule
                    return;
                }

                // track all lexer rules so we can look for token refs w/o
                // associated lexer rules.
                grammar.composite.LexerRules.Add( t.Text );

                int existing = grammar.GetTokenType( t.Text );
                if ( existing == Label.INVALID )
                {
                    _tokens[t.Text] = Unassigned;
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
                    (parent.type==GrammarType.Lexer||parent.type==GrammarType.Parser);
                    */
                    if ( grammar.type == GrammarType.Combined || grammar.type == GrammarType.Lexer )
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
            string prevAliasLiteralID;
            _aliasesReverseIndex.TryGetValue(literal, out prevAliasLiteralID);
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
                                              t.Token,
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
                _tokens[tokenID] = existingLiteralType;
            }
            string prevAliasTokenID;
            _aliases.TryGetValue(tokenID, out prevAliasTokenID);
            if ( prevAliasTokenID != null )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_TOKEN_ALIAS_REASSIGNMENT,
                                          grammar,
                                          t.Token,
                                          tokenID + "=" + literal,
                                          prevAliasTokenID );
                return; // don't do the alias
            }
            _aliases[tokenID] = literal;
            _aliasesReverseIndex[literal] = tokenID;
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
            if ( grammar.IsRoot && grammar.type == GrammarType.Combined )
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
            foreach ( var literal in _stringLiterals.Where( pair => pair.Value < Label.MIN_TOKEN_TYPE ).ToArray() )
            {
                int type = root.GetNewTokenType();
                _stringLiterals[literal.Key] = type;
                // if string referenced in combined grammar parser rule,
                // automatically define in the generated lexer
                root.DefineLexerRuleForStringLiteral( literal.Key, type );
            }
        }

        protected override void AliasTokenIDsAndLiterals( Grammar root )
        {
            if ( root.type == GrammarType.Lexer )
            {
                return; // strings/chars are never token types in LEXER
            }
            // walk aliases if any and assign types to aliased literals if literal
            // was referenced
            foreach ( var alias in _aliases )
            {
                string tokenID = alias.Key;
                string literal = alias.Value;
                if ( literal[0] == '\'' && _stringLiterals.ContainsKey( literal ) )
                {
                    int token;
                    _tokens.TryGetValue(tokenID, out token);
                    _stringLiterals[literal] = token;
                    // an alias still means you need a lexer rule for it
                    int typeI;
                    _tokens.TryGetValue(tokenID, out typeI);
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
            foreach ( var token in _tokens.Where( pair => pair.Value == Unassigned ).ToArray() )
            {
                _tokens[token.Key] = root.GetNewTokenType();
            }
        }

        protected override void DefineTokenNamesAndLiteralsInGrammar( Grammar root )
        {
            foreach ( var token in _tokens )
            {
                root.DefineToken( token.Key, token.Value );
            }

            foreach ( var lit in _stringLiterals )
            {
                root.DefineToken( lit.Key, lit.Value );
            }
        }

    }
}
