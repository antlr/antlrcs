// $ANTLR 3.1.2 Grammars\\AssignTokenTypesWalker.g3 2009-04-17 13:33:46

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


/*
 [The "BSD licence"]
 Copyright (c) 2005-2008 Terence Parr
 All rights reserved.

 Grammar conversion to ANTLR v3 and C#:
 Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
	derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using Antlr3.Analysis;
using Antlr3.Misc;
using Antlr3.Tool;

using BitSet = Antlr.Runtime.BitSet;
using DFA = Antlr.Runtime.DFA;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
using Map = System.Collections.IDictionary;


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Antlr3.Grammars
{
/** [Warning: TJP says that this is probably out of date as of 11/19/2005,
 *   but since it's probably still useful, I'll leave in.  Don't have energy
 *   to update at the moment.]
 *
 *  Compute the token types for all literals and rules etc..  There are
 *  a few different cases to consider for grammar types and a few situations
 *  within.
 *
 *  CASE 1 : pure parser grammar
 *	a) Any reference to a token gets a token type.
 *  b) The tokens section may alias a token name to a string or char
 *
 *  CASE 2 : pure lexer grammar
 *  a) Import token vocabulary if available. Set token types for any new tokens
 *     to values above last imported token type
 *  b) token rule definitions get token types if not already defined
 *  c) literals do NOT get token types
 *
 *  CASE 3 : merged parser / lexer grammar
 *	a) Any char or string literal gets a token type in a parser rule
 *  b) Any reference to a token gets a token type if not referencing
 *     a fragment lexer rule
 *  c) The tokens section may alias a token name to a string or char
 *     which must add a rule to the lexer
 *  d) token rule definitions get token types if not already defined
 *  e) token rule definitions may also alias a token name to a literal.
 *     E.g., Rule 'FOR : "for";' will alias FOR to "for" in the sense that
 *     references to either in the parser grammar will yield the token type
 *
 *  What this pass does:
 *
 *  0. Collects basic info about the grammar like grammar name and type;
 *     Oh, I have go get the options in case they affect the token types.
 *     E.g., tokenVocab option.
 *     Imports any token vocab name/type pairs into a local hashtable.
 *  1. Finds a list of all literals and token names.
 *  2. Finds a list of all token name rule definitions;
 *     no token rules implies pure parser.
 *  3. Finds a list of all simple token rule defs of form "<NAME> : <literal>;"
 *     and aliases them.
 *  4. Walks token names table and assign types to any unassigned
 *  5. Walks aliases and assign types to referenced literals
 *  6. Walks literals, assigning types if untyped
 *  4. Informs the Grammar object of the type definitions such as:
 *     g.defineToken(<charliteral>, ttype);
 *     g.defineToken(<stringliteral>, ttype);
 *     g.defineToken(<tokenID>, ttype);
 *     where some of the ttype values will be the same for aliases tokens.
 */
public partial class AssignTokenTypesWalker : TreeParser
{
	public static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACTION", "ACTION_CHAR_LITERAL", "ACTION_ESC", "ACTION_STRING_LITERAL", "ALT", "AMPERSAND", "ARG", "ARG_ACTION", "ARGLIST", "ASSIGN", "BACKTRACK_SEMPRED", "BANG", "BLOCK", "CATCH", "CHAR_LITERAL", "CHAR_RANGE", "CLOSE_ELEMENT_OPTION", "CLOSURE", "COLON", "COMBINED_GRAMMAR", "COMMA", "COMMENT", "DIGIT", "DOC_COMMENT", "DOLLAR", "DOT", "DOUBLE_ANGLE_STRING_LITERAL", "DOUBLE_QUOTE_STRING_LITERAL", "EOA", "EOB", "EOR", "EPSILON", "ESC", "ETC", "FINALLY", "FORCED_ACTION", "FRAGMENT", "GATED_SEMPRED", "GRAMMAR", "ID", "IMPLIES", "IMPORT", "INITACTION", "INT", "LABEL", "LEXER", "LEXER_GRAMMAR", "LPAREN", "ML_COMMENT", "NESTED_ACTION", "NESTED_ARG_ACTION", "NOT", "OPEN_ELEMENT_OPTION", "OPTIONAL", "OPTIONS", "OR", "PARSER", "PARSER_GRAMMAR", "PLUS", "PLUS_ASSIGN", "POSITIVE_CLOSURE", "PRIVATE", "PROTECTED", "PUBLIC", "QUESTION", "RANGE", "RCURLY", "RET", "RETURNS", "REWRITE", "ROOT", "RPAREN", "RULE", "RULE_REF", "SCOPE", "SEMI", "SEMPRED", "SL_COMMENT", "SRC", "STAR", "STRAY_BRACKET", "STRING_LITERAL", "SYN_SEMPRED", "SYNPRED", "TEMPLATE", "THROWS", "TOKEN_REF", "TOKENS", "TREE", "TREE_BEGIN", "TREE_GRAMMAR", "WILDCARD", "WS", "WS_LOOP", "WS_OPT", "XDIGIT", "CHARSET"
	};
	public const int EOF=-1;
	public const int ACTION=4;
	public const int ACTION_CHAR_LITERAL=5;
	public const int ACTION_ESC=6;
	public const int ACTION_STRING_LITERAL=7;
	public const int ALT=8;
	public const int AMPERSAND=9;
	public const int ARG=10;
	public const int ARG_ACTION=11;
	public const int ARGLIST=12;
	public const int ASSIGN=13;
	public const int BACKTRACK_SEMPRED=14;
	public const int BANG=15;
	public const int BLOCK=16;
	public const int CATCH=17;
	public const int CHAR_LITERAL=18;
	public const int CHAR_RANGE=19;
	public const int CLOSE_ELEMENT_OPTION=20;
	public const int CLOSURE=21;
	public const int COLON=22;
	public const int COMBINED_GRAMMAR=23;
	public const int COMMA=24;
	public const int COMMENT=25;
	public const int DIGIT=26;
	public const int DOC_COMMENT=27;
	public const int DOLLAR=28;
	public const int DOT=29;
	public const int DOUBLE_ANGLE_STRING_LITERAL=30;
	public const int DOUBLE_QUOTE_STRING_LITERAL=31;
	public const int EOA=32;
	public const int EOB=33;
	public const int EOR=34;
	public const int EPSILON=35;
	public const int ESC=36;
	public const int ETC=37;
	public const int FINALLY=38;
	public const int FORCED_ACTION=39;
	public const int FRAGMENT=40;
	public const int GATED_SEMPRED=41;
	public const int GRAMMAR=42;
	public const int ID=43;
	public const int IMPLIES=44;
	public const int IMPORT=45;
	public const int INITACTION=46;
	public const int INT=47;
	public const int LABEL=48;
	public const int LEXER=49;
	public const int LEXER_GRAMMAR=50;
	public const int LPAREN=51;
	public const int ML_COMMENT=52;
	public const int NESTED_ACTION=53;
	public const int NESTED_ARG_ACTION=54;
	public const int NOT=55;
	public const int OPEN_ELEMENT_OPTION=56;
	public const int OPTIONAL=57;
	public const int OPTIONS=58;
	public const int OR=59;
	public const int PARSER=60;
	public const int PARSER_GRAMMAR=61;
	public const int PLUS=62;
	public const int PLUS_ASSIGN=63;
	public const int POSITIVE_CLOSURE=64;
	public const int PRIVATE=65;
	public const int PROTECTED=66;
	public const int PUBLIC=67;
	public const int QUESTION=68;
	public const int RANGE=69;
	public const int RCURLY=70;
	public const int RET=71;
	public const int RETURNS=72;
	public const int REWRITE=73;
	public const int ROOT=74;
	public const int RPAREN=75;
	public const int RULE=76;
	public const int RULE_REF=77;
	public const int SCOPE=78;
	public const int SEMI=79;
	public const int SEMPRED=80;
	public const int SL_COMMENT=81;
	public const int SRC=82;
	public const int STAR=83;
	public const int STRAY_BRACKET=84;
	public const int STRING_LITERAL=85;
	public const int SYN_SEMPRED=86;
	public const int SYNPRED=87;
	public const int TEMPLATE=88;
	public const int THROWS=89;
	public const int TOKEN_REF=90;
	public const int TOKENS=91;
	public const int TREE=92;
	public const int TREE_BEGIN=93;
	public const int TREE_GRAMMAR=94;
	public const int WILDCARD=95;
	public const int WS=96;
	public const int WS_LOOP=97;
	public const int WS_OPT=98;
	public const int XDIGIT=99;
	public const int CHARSET=100;

	// delegates
	// delegators

	public AssignTokenTypesWalker( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public AssignTokenTypesWalker( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return AssignTokenTypesWalker.tokenNames; }
	public override string GrammarFileName { get { return "Grammars\\AssignTokenTypesWalker.g3"; } }


	#region Rules

	// $ANTLR start "grammar_"
	// Grammars\\AssignTokenTypesWalker.g3:139:0: public grammar_[Grammar g] : ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) ;
	public void grammar_( Grammar g )
	{

			if ( state.backtracking == 0 )
				Init(g);

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:145:4: ( ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) )
			// Grammars\\AssignTokenTypesWalker.g3:145:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
			{
			// Grammars\\AssignTokenTypesWalker.g3:145:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
			int alt1=4;
			switch ( input.LA(1) )
			{
			case LEXER_GRAMMAR:
				{
				alt1=1;
				}
				break;
			case PARSER_GRAMMAR:
				{
				alt1=2;
				}
				break;
			case TREE_GRAMMAR:
				{
				alt1=3;
				}
				break;
			case COMBINED_GRAMMAR:
				{
				alt1=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

					throw nvae;
				}
			}

			switch ( alt1 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:145:6: ^( LEXER_GRAMMAR grammarSpec )
				{
				Match(input,LEXER_GRAMMAR,Follow._LEXER_GRAMMAR_in_grammar_68); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._grammarSpec_in_grammar_73);
				grammarSpec();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:146:5: ^( PARSER_GRAMMAR grammarSpec )
				{
				Match(input,PARSER_GRAMMAR,Follow._PARSER_GRAMMAR_in_grammar_83); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._grammarSpec_in_grammar_87);
				grammarSpec();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:147:5: ^( TREE_GRAMMAR grammarSpec )
				{
				Match(input,TREE_GRAMMAR,Follow._TREE_GRAMMAR_in_grammar_97); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._grammarSpec_in_grammar_103);
				grammarSpec();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 4:
				// Grammars\\AssignTokenTypesWalker.g3:148:5: ^( COMBINED_GRAMMAR grammarSpec )
				{
				Match(input,COMBINED_GRAMMAR,Follow._COMBINED_GRAMMAR_in_grammar_113); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._grammarSpec_in_grammar_115);
				grammarSpec();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;

			}


			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "grammar_"


	// $ANTLR start "grammarSpec"
	// Grammars\\AssignTokenTypesWalker.g3:152:0: grammarSpec : id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules ;
	private void grammarSpec(  )
	{
		GrammarAST id=null;
		GrammarAST cmt=null;

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:153:4: (id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules )
			// Grammars\\AssignTokenTypesWalker.g3:153:4: id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules
			{
			id=(GrammarAST)Match(input,ID,Follow._ID_in_grammarSpec134); 
			// Grammars\\AssignTokenTypesWalker.g3:154:3: (cmt= DOC_COMMENT )?
			int alt2=2;
			int LA2_0 = input.LA(1);

			if ( (LA2_0==DOC_COMMENT) )
			{
				alt2=1;
			}
			switch ( alt2 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:154:4: cmt= DOC_COMMENT
				{
				cmt=(GrammarAST)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammarSpec141); 

				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:155:3: ( optionsSpec )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==OPTIONS) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:155:4: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_grammarSpec148);
				optionsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:156:3: ( delegateGrammars )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==IMPORT) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:156:4: delegateGrammars
				{
				PushFollow(Follow._delegateGrammars_in_grammarSpec155);
				delegateGrammars();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:157:3: ( tokensSpec )?
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( (LA5_0==TOKENS) )
			{
				alt5=1;
			}
			switch ( alt5 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:157:4: tokensSpec
				{
				PushFollow(Follow._tokensSpec_in_grammarSpec162);
				tokensSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:158:3: ( attrScope )*
			for ( ; ; )
			{
				int alt6=2;
				int LA6_0 = input.LA(1);

				if ( (LA6_0==SCOPE) )
				{
					alt6=1;
				}


				switch ( alt6 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:158:4: attrScope
					{
					PushFollow(Follow._attrScope_in_grammarSpec169);
					attrScope();

					state._fsp--;


					}
					break;

				default:
					goto loop6;
				}
			}

			loop6:
				;


			// Grammars\\AssignTokenTypesWalker.g3:159:3: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt8=2;
				int LA8_0 = input.LA(1);

				if ( (LA8_0==AMPERSAND) )
				{
					alt8=1;
				}


				switch ( alt8 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:159:5: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_grammarSpec178); 

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); 
						// Grammars\\AssignTokenTypesWalker.g3:159:17: ( . )*
						for ( ; ; )
						{
							int alt7=2;
							int LA7_0 = input.LA(1);

							if ( ((LA7_0>=ACTION && LA7_0<=CHARSET)) )
							{
								alt7=1;
							}
							else if ( (LA7_0==UP) )
							{
								alt7=2;
							}


							switch ( alt7 )
							{
							case 1:
								// Grammars\\AssignTokenTypesWalker.g3:159:0: .
								{
								MatchAny(input); 

								}
								break;

							default:
								goto loop7;
							}
						}

						loop7:
							;



						Match(input, TokenTypes.Up, null); 
					}

					}
					break;

				default:
					goto loop8;
				}
			}

			loop8:
				;


			PushFollow(Follow._rules_in_grammarSpec190);
			rules();

			state._fsp--;


			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "grammarSpec"


	// $ANTLR start "attrScope"
	// Grammars\\AssignTokenTypesWalker.g3:163:0: attrScope : ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) ;
	private void attrScope(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:164:4: ( ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) )
			// Grammars\\AssignTokenTypesWalker.g3:164:4: ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION )
			{
			Match(input,SCOPE,Follow._SCOPE_in_attrScope203); 

			Match(input, TokenTypes.Down, null); 
			Match(input,ID,Follow._ID_in_attrScope205); 
			// Grammars\\AssignTokenTypesWalker.g3:164:18: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( (LA10_0==AMPERSAND) )
				{
					alt10=1;
				}


				switch ( alt10 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:164:20: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_attrScope210); 

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); 
						// Grammars\\AssignTokenTypesWalker.g3:164:32: ( . )*
						for ( ; ; )
						{
							int alt9=2;
							int LA9_0 = input.LA(1);

							if ( ((LA9_0>=ACTION && LA9_0<=CHARSET)) )
							{
								alt9=1;
							}
							else if ( (LA9_0==UP) )
							{
								alt9=2;
							}


							switch ( alt9 )
							{
							case 1:
								// Grammars\\AssignTokenTypesWalker.g3:164:0: .
								{
								MatchAny(input); 

								}
								break;

							default:
								goto loop9;
							}
						}

						loop9:
							;



						Match(input, TokenTypes.Up, null); 
					}

					}
					break;

				default:
					goto loop10;
				}
			}

			loop10:
				;


			Match(input,ACTION,Follow._ACTION_in_attrScope219); 

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "attrScope"


	// $ANTLR start "optionsSpec"
	// Grammars\\AssignTokenTypesWalker.g3:167:0: optionsSpec returns [Map opts=new HashMap()] : ^( OPTIONS ( option[$opts] )+ ) ;
	private Map optionsSpec(  )
	{

		Map opts = new HashMap();

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:168:4: ( ^( OPTIONS ( option[$opts] )+ ) )
			// Grammars\\AssignTokenTypesWalker.g3:168:4: ^( OPTIONS ( option[$opts] )+ )
			{
			Match(input,OPTIONS,Follow._OPTIONS_in_optionsSpec238); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:168:15: ( option[$opts] )+
			int cnt11=0;
			for ( ; ; )
			{
				int alt11=2;
				int LA11_0 = input.LA(1);

				if ( (LA11_0==ASSIGN) )
				{
					alt11=1;
				}


				switch ( alt11 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:168:16: option[$opts]
					{
					PushFollow(Follow._option_in_optionsSpec241);
					option(opts);

					state._fsp--;


					}
					break;

				default:
					if ( cnt11 >= 1 )
						goto loop11;

					EarlyExitException eee11 = new EarlyExitException( 11, input );
					throw eee11;
				}
				cnt11++;
			}
			loop11:
				;



			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return opts;
	}
	// $ANTLR end "optionsSpec"


	// $ANTLR start "option"
	// Grammars\\AssignTokenTypesWalker.g3:171:0: option[Map opts] : ^( ASSIGN ID optionValue ) ;
	private void option( Map opts )
	{
		GrammarAST ID1=null;
		AssignTokenTypesWalker.optionValue_return optionValue2 = default(AssignTokenTypesWalker.optionValue_return);

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:172:4: ( ^( ASSIGN ID optionValue ) )
			// Grammars\\AssignTokenTypesWalker.g3:172:4: ^( ASSIGN ID optionValue )
			{
			Match(input,ASSIGN,Follow._ASSIGN_in_option260); 

			Match(input, TokenTypes.Down, null); 
			ID1=(GrammarAST)Match(input,ID,Follow._ID_in_option262); 
			PushFollow(Follow._optionValue_in_option264);
			optionValue2=optionValue();

			state._fsp--;


			Match(input, TokenTypes.Up, null); 

						string key = (ID1!=null?ID1.Text:null);
						opts[key] = (optionValue2!=null?optionValue2.value:default(Object));
						// check for grammar-level option to import vocabulary
						if ( currentRuleName==null && key.Equals("tokenVocab") )
						{
							grammar.ImportTokenVocabulary(ID1,(string)(optionValue2!=null?optionValue2.value:default(Object)));
						}
					

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "option"

	public class optionValue_return : TreeRuleReturnScope
	{
		public Object value=null;
	}

	// $ANTLR start "optionValue"
	// Grammars\\AssignTokenTypesWalker.g3:184:0: optionValue returns [Object value=null] : ( ID | STRING_LITERAL | CHAR_LITERAL | INT );
	private AssignTokenTypesWalker.optionValue_return optionValue(  )
	{
		AssignTokenTypesWalker.optionValue_return retval = new AssignTokenTypesWalker.optionValue_return();
		retval.start = input.LT(1);

		GrammarAST INT3=null;


			if ( state.backtracking == 0 )
				retval.value = ((GrammarAST)retval.start).Text;

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:190:4: ( ID | STRING_LITERAL | CHAR_LITERAL | INT )
			int alt12=4;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt12=1;
				}
				break;
			case STRING_LITERAL:
				{
				alt12=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt12=3;
				}
				break;
			case INT:
				{
				alt12=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 12, 0, input);

					throw nvae;
				}
			}

			switch ( alt12 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:190:4: ID
				{
				Match(input,ID,Follow._ID_in_optionValue290); 

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:191:4: STRING_LITERAL
				{
				Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_optionValue295); 

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:192:4: CHAR_LITERAL
				{
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_optionValue300); 

				}
				break;
			case 4:
				// Grammars\\AssignTokenTypesWalker.g3:193:4: INT
				{
				INT3=(GrammarAST)Match(input,INT,Follow._INT_in_optionValue305); 
				 retval.value = int.Parse( (INT3!=null?INT3.Text:null) ); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "optionValue"


	// $ANTLR start "charSet"
	// Grammars\\AssignTokenTypesWalker.g3:198:0: charSet : ^( CHARSET charSetElement ) ;
	private void charSet(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:199:4: ( ^( CHARSET charSetElement ) )
			// Grammars\\AssignTokenTypesWalker.g3:199:4: ^( CHARSET charSetElement )
			{
			Match(input,CHARSET,Follow._CHARSET_in_charSet323); 

			Match(input, TokenTypes.Down, null); 
			PushFollow(Follow._charSetElement_in_charSet325);
			charSetElement();

			state._fsp--;


			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "charSet"


	// $ANTLR start "charSetElement"
	// Grammars\\AssignTokenTypesWalker.g3:202:0: charSetElement : ( CHAR_LITERAL | ^( OR CHAR_LITERAL CHAR_LITERAL ) | ^( RANGE CHAR_LITERAL CHAR_LITERAL ) );
	private void charSetElement(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:203:4: ( CHAR_LITERAL | ^( OR CHAR_LITERAL CHAR_LITERAL ) | ^( RANGE CHAR_LITERAL CHAR_LITERAL ) )
			int alt13=3;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt13=1;
				}
				break;
			case OR:
				{
				alt13=2;
				}
				break;
			case RANGE:
				{
				alt13=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

					throw nvae;
				}
			}

			switch ( alt13 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:203:4: CHAR_LITERAL
				{
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_charSetElement338); 

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:204:4: ^( OR CHAR_LITERAL CHAR_LITERAL )
				{
				Match(input,OR,Follow._OR_in_charSetElement345); 

				Match(input, TokenTypes.Down, null); 
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_charSetElement347); 
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_charSetElement349); 

				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:205:4: ^( RANGE CHAR_LITERAL CHAR_LITERAL )
				{
				Match(input,RANGE,Follow._RANGE_in_charSetElement358); 

				Match(input, TokenTypes.Down, null); 
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_charSetElement360); 
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_charSetElement362); 

				Match(input, TokenTypes.Up, null); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "charSetElement"


	// $ANTLR start "delegateGrammars"
	// Grammars\\AssignTokenTypesWalker.g3:208:0: delegateGrammars : ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) ;
	private void delegateGrammars(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:209:4: ( ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) )
			// Grammars\\AssignTokenTypesWalker.g3:209:4: ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ )
			{
			Match(input,IMPORT,Follow._IMPORT_in_delegateGrammars377); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:210:4: ( ^( ASSIGN ID ID ) | ID )+
			int cnt14=0;
			for ( ; ; )
			{
				int alt14=3;
				int LA14_0 = input.LA(1);

				if ( (LA14_0==ASSIGN) )
				{
					alt14=1;
				}
				else if ( (LA14_0==ID) )
				{
					alt14=2;
				}


				switch ( alt14 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:210:6: ^( ASSIGN ID ID )
					{
					Match(input,ASSIGN,Follow._ASSIGN_in_delegateGrammars385); 

					Match(input, TokenTypes.Down, null); 
					Match(input,ID,Follow._ID_in_delegateGrammars387); 
					Match(input,ID,Follow._ID_in_delegateGrammars389); 

					Match(input, TokenTypes.Up, null); 

					}
					break;
				case 2:
					// Grammars\\AssignTokenTypesWalker.g3:211:6: ID
					{
					Match(input,ID,Follow._ID_in_delegateGrammars397); 

					}
					break;

				default:
					if ( cnt14 >= 1 )
						goto loop14;

					EarlyExitException eee14 = new EarlyExitException( 14, input );
					throw eee14;
				}
				cnt14++;
			}
			loop14:
				;



			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "delegateGrammars"


	// $ANTLR start "tokensSpec"
	// Grammars\\AssignTokenTypesWalker.g3:216:0: tokensSpec : ^( TOKENS ( tokenSpec )+ ) ;
	private void tokensSpec(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:217:4: ( ^( TOKENS ( tokenSpec )+ ) )
			// Grammars\\AssignTokenTypesWalker.g3:217:4: ^( TOKENS ( tokenSpec )+ )
			{
			Match(input,TOKENS,Follow._TOKENS_in_tokensSpec420); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:217:14: ( tokenSpec )+
			int cnt15=0;
			for ( ; ; )
			{
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( (LA15_0==ASSIGN||LA15_0==TOKEN_REF) )
				{
					alt15=1;
				}


				switch ( alt15 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:217:0: tokenSpec
					{
					PushFollow(Follow._tokenSpec_in_tokensSpec422);
					tokenSpec();

					state._fsp--;


					}
					break;

				default:
					if ( cnt15 >= 1 )
						goto loop15;

					EarlyExitException eee15 = new EarlyExitException( 15, input );
					throw eee15;
				}
				cnt15++;
			}
			loop15:
				;



			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "tokensSpec"


	// $ANTLR start "tokenSpec"
	// Grammars\\AssignTokenTypesWalker.g3:220:0: tokenSpec : (t= TOKEN_REF | ^( ASSIGN t2= TOKEN_REF (s= STRING_LITERAL |c= CHAR_LITERAL ) ) );
	private void tokenSpec(  )
	{
		GrammarAST t=null;
		GrammarAST t2=null;
		GrammarAST s=null;
		GrammarAST c=null;

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:221:4: (t= TOKEN_REF | ^( ASSIGN t2= TOKEN_REF (s= STRING_LITERAL |c= CHAR_LITERAL ) ) )
			int alt17=2;
			int LA17_0 = input.LA(1);

			if ( (LA17_0==TOKEN_REF) )
			{
				alt17=1;
			}
			else if ( (LA17_0==ASSIGN) )
			{
				alt17=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 17, 0, input);

				throw nvae;
			}
			switch ( alt17 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:221:4: t= TOKEN_REF
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec438); 
				TrackToken(t);

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:222:4: ^( ASSIGN t2= TOKEN_REF (s= STRING_LITERAL |c= CHAR_LITERAL ) )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_tokenSpec458); 

				Match(input, TokenTypes.Down, null); 
				t2=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec465); 
				TrackToken(t2);
				// Grammars\\AssignTokenTypesWalker.g3:224:4: (s= STRING_LITERAL |c= CHAR_LITERAL )
				int alt16=2;
				int LA16_0 = input.LA(1);

				if ( (LA16_0==STRING_LITERAL) )
				{
					alt16=1;
				}
				else if ( (LA16_0==CHAR_LITERAL) )
				{
					alt16=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 16, 0, input);

					throw nvae;
				}
				switch ( alt16 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:224:6: s= STRING_LITERAL
					{
					s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_tokenSpec482); 
					TrackString(s); Alias(t2,s);

					}
					break;
				case 2:
					// Grammars\\AssignTokenTypesWalker.g3:225:6: c= CHAR_LITERAL
					{
					c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_tokenSpec493); 
					TrackString(c); Alias(t2,c);

					}
					break;

				}


				Match(input, TokenTypes.Up, null); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "tokenSpec"


	// $ANTLR start "rules"
	// Grammars\\AssignTokenTypesWalker.g3:230:0: rules : ( rule )+ ;
	private void rules(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:231:4: ( ( rule )+ )
			// Grammars\\AssignTokenTypesWalker.g3:231:4: ( rule )+
			{
			// Grammars\\AssignTokenTypesWalker.g3:231:4: ( rule )+
			int cnt18=0;
			for ( ; ; )
			{
				int alt18=2;
				int LA18_0 = input.LA(1);

				if ( (LA18_0==RULE) )
				{
					alt18=1;
				}


				switch ( alt18 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:231:0: rule
					{
					PushFollow(Follow._rule_in_rules517);
					rule();

					state._fsp--;


					}
					break;

				default:
					if ( cnt18 >= 1 )
						goto loop18;

					EarlyExitException eee18 = new EarlyExitException( 18, input );
					throw eee18;
				}
				cnt18++;
			}
			loop18:
				;



			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "rules"


	// $ANTLR start "rule"
	// Grammars\\AssignTokenTypesWalker.g3:234:0: rule : ^( RULE id= ID (m= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR ) ;
	private void rule(  )
	{
		GrammarAST id=null;
		AssignTokenTypesWalker.modifier_return m = default(AssignTokenTypesWalker.modifier_return);
		AssignTokenTypesWalker.block_return b = default(AssignTokenTypesWalker.block_return);

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:235:4: ( ^( RULE id= ID (m= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR ) )
			// Grammars\\AssignTokenTypesWalker.g3:235:4: ^( RULE id= ID (m= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_rule531); 

			Match(input, TokenTypes.Down, null); 
			id=(GrammarAST)Match(input,ID,Follow._ID_in_rule535); 
			currentRuleName=(id!=null?id.Text:null);
			// Grammars\\AssignTokenTypesWalker.g3:236:4: (m= modifier )?
			int alt19=2;
			int LA19_0 = input.LA(1);

			if ( (LA19_0==FRAGMENT||(LA19_0>=PRIVATE && LA19_0<=PUBLIC)) )
			{
				alt19=1;
			}
			switch ( alt19 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:236:5: m= modifier
				{
				PushFollow(Follow._modifier_in_rule545);
				m=modifier();

				state._fsp--;


				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_rule553); 

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); 
				// Grammars\\AssignTokenTypesWalker.g3:237:10: ( ARG_ACTION )?
				int alt20=2;
				int LA20_0 = input.LA(1);

				if ( (LA20_0==ARG_ACTION) )
				{
					alt20=1;
				}
				switch ( alt20 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:237:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule556); 

					}
					break;

				}


				Match(input, TokenTypes.Up, null); 
			}
			Match(input,RET,Follow._RET_in_rule565); 

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); 
				// Grammars\\AssignTokenTypesWalker.g3:238:10: ( ARG_ACTION )?
				int alt21=2;
				int LA21_0 = input.LA(1);

				if ( (LA21_0==ARG_ACTION) )
				{
					alt21=1;
				}
				switch ( alt21 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:238:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule568); 

					}
					break;

				}


				Match(input, TokenTypes.Up, null); 
			}
			// Grammars\\AssignTokenTypesWalker.g3:239:4: ( throwsSpec )?
			int alt22=2;
			int LA22_0 = input.LA(1);

			if ( (LA22_0==THROWS) )
			{
				alt22=1;
			}
			switch ( alt22 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:239:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule577);
				throwsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:240:4: ( optionsSpec )?
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0==OPTIONS) )
			{
				alt23=1;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:240:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_rule585);
				optionsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:241:4: ( ruleScopeSpec )?
			int alt24=2;
			int LA24_0 = input.LA(1);

			if ( (LA24_0==SCOPE) )
			{
				alt24=1;
			}
			switch ( alt24 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:241:5: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_rule593);
				ruleScopeSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:242:4: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt26=2;
				int LA26_0 = input.LA(1);

				if ( (LA26_0==AMPERSAND) )
				{
					alt26=1;
				}


				switch ( alt26 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:242:6: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_rule603); 

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); 
						// Grammars\\AssignTokenTypesWalker.g3:242:18: ( . )*
						for ( ; ; )
						{
							int alt25=2;
							int LA25_0 = input.LA(1);

							if ( ((LA25_0>=ACTION && LA25_0<=CHARSET)) )
							{
								alt25=1;
							}
							else if ( (LA25_0==UP) )
							{
								alt25=2;
							}


							switch ( alt25 )
							{
							case 1:
								// Grammars\\AssignTokenTypesWalker.g3:242:0: .
								{
								MatchAny(input); 

								}
								break;

							default:
								goto loop25;
							}
						}

						loop25:
							;



						Match(input, TokenTypes.Up, null); 
					}

					}
					break;

				default:
					goto loop26;
				}
			}

			loop26:
				;


			PushFollow(Follow._block_in_rule617);
			b=block();

			state._fsp--;

			// Grammars\\AssignTokenTypesWalker.g3:244:4: ( exceptionGroup )?
			int alt27=2;
			int LA27_0 = input.LA(1);

			if ( (LA27_0==CATCH||LA27_0==FINALLY) )
			{
				alt27=1;
			}
			switch ( alt27 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:244:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_rule623);
				exceptionGroup();

				state._fsp--;


				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_rule630); 
			TrackTokenRule(id,(m!=null?((GrammarAST)m.start):null),(b!=null?((GrammarAST)b.start):null));

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "rule"

	public class modifier_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "modifier"
	// Grammars\\AssignTokenTypesWalker.g3:250:0: modifier : ( 'protected' | 'public' | 'private' | 'fragment' );
	private AssignTokenTypesWalker.modifier_return modifier(  )
	{
		AssignTokenTypesWalker.modifier_return retval = new AssignTokenTypesWalker.modifier_return();
		retval.start = input.LT(1);

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:251:4: ( 'protected' | 'public' | 'private' | 'fragment' )
			// Grammars\\AssignTokenTypesWalker.g3:
			{
			if ( input.LA(1)==FRAGMENT||(input.LA(1)>=PRIVATE && input.LA(1)<=PUBLIC) )
			{
				input.Consume();
				state.errorRecovery=false;
			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				throw mse;
			}


			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "modifier"


	// $ANTLR start "throwsSpec"
	// Grammars\\AssignTokenTypesWalker.g3:257:0: throwsSpec : ^( 'throws' ( ID )+ ) ;
	private void throwsSpec(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:258:4: ( ^( 'throws' ( ID )+ ) )
			// Grammars\\AssignTokenTypesWalker.g3:258:4: ^( 'throws' ( ID )+ )
			{
			Match(input,THROWS,Follow._THROWS_in_throwsSpec677); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:258:15: ( ID )+
			int cnt28=0;
			for ( ; ; )
			{
				int alt28=2;
				int LA28_0 = input.LA(1);

				if ( (LA28_0==ID) )
				{
					alt28=1;
				}


				switch ( alt28 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:258:0: ID
					{
					Match(input,ID,Follow._ID_in_throwsSpec679); 

					}
					break;

				default:
					if ( cnt28 >= 1 )
						goto loop28;

					EarlyExitException eee28 = new EarlyExitException( 28, input );
					throw eee28;
				}
				cnt28++;
			}
			loop28:
				;



			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "throwsSpec"


	// $ANTLR start "ruleScopeSpec"
	// Grammars\\AssignTokenTypesWalker.g3:261:0: ruleScopeSpec : ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) ;
	private void ruleScopeSpec(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:262:4: ( ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) )
			// Grammars\\AssignTokenTypesWalker.g3:262:4: ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* )
			{
			Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec694); 

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); 
				// Grammars\\AssignTokenTypesWalker.g3:262:15: ( ^( AMPERSAND ( . )* ) )*
				for ( ; ; )
				{
					int alt30=2;
					int LA30_0 = input.LA(1);

					if ( (LA30_0==AMPERSAND) )
					{
						alt30=1;
					}


					switch ( alt30 )
					{
					case 1:
						// Grammars\\AssignTokenTypesWalker.g3:262:17: ^( AMPERSAND ( . )* )
						{
						Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleScopeSpec699); 

						if ( input.LA(1)==TokenTypes.Down )
						{
							Match(input, TokenTypes.Down, null); 
							// Grammars\\AssignTokenTypesWalker.g3:262:29: ( . )*
							for ( ; ; )
							{
								int alt29=2;
								int LA29_0 = input.LA(1);

								if ( ((LA29_0>=ACTION && LA29_0<=CHARSET)) )
								{
									alt29=1;
								}
								else if ( (LA29_0==UP) )
								{
									alt29=2;
								}


								switch ( alt29 )
								{
								case 1:
									// Grammars\\AssignTokenTypesWalker.g3:262:0: .
									{
									MatchAny(input); 

									}
									break;

								default:
									goto loop29;
								}
							}

							loop29:
								;



							Match(input, TokenTypes.Up, null); 
						}

						}
						break;

					default:
						goto loop30;
					}
				}

				loop30:
					;


				// Grammars\\AssignTokenTypesWalker.g3:262:36: ( ACTION )?
				int alt31=2;
				int LA31_0 = input.LA(1);

				if ( (LA31_0==ACTION) )
				{
					alt31=1;
				}
				switch ( alt31 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:262:37: ACTION
					{
					Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec709); 

					}
					break;

				}

				// Grammars\\AssignTokenTypesWalker.g3:262:46: ( ID )*
				for ( ; ; )
				{
					int alt32=2;
					int LA32_0 = input.LA(1);

					if ( (LA32_0==ID) )
					{
						alt32=1;
					}


					switch ( alt32 )
					{
					case 1:
						// Grammars\\AssignTokenTypesWalker.g3:262:48: ID
						{
						Match(input,ID,Follow._ID_in_ruleScopeSpec715); 

						}
						break;

					default:
						goto loop32;
					}
				}

				loop32:
					;



				Match(input, TokenTypes.Up, null); 
			}

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "ruleScopeSpec"

	public class block_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "block"
	// Grammars\\AssignTokenTypesWalker.g3:265:0: block : ^( BLOCK ( optionsSpec )? ( alternative rewrite )+ EOB ) ;
	private AssignTokenTypesWalker.block_return block(  )
	{
		AssignTokenTypesWalker.block_return retval = new AssignTokenTypesWalker.block_return();
		retval.start = input.LT(1);

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:266:4: ( ^( BLOCK ( optionsSpec )? ( alternative rewrite )+ EOB ) )
			// Grammars\\AssignTokenTypesWalker.g3:266:4: ^( BLOCK ( optionsSpec )? ( alternative rewrite )+ EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_block733); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:267:4: ( optionsSpec )?
			int alt33=2;
			int LA33_0 = input.LA(1);

			if ( (LA33_0==OPTIONS) )
			{
				alt33=1;
			}
			switch ( alt33 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:267:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_block739);
				optionsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\AssignTokenTypesWalker.g3:268:4: ( alternative rewrite )+
			int cnt34=0;
			for ( ; ; )
			{
				int alt34=2;
				int LA34_0 = input.LA(1);

				if ( (LA34_0==ALT) )
				{
					alt34=1;
				}


				switch ( alt34 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:268:6: alternative rewrite
					{
					PushFollow(Follow._alternative_in_block748);
					alternative();

					state._fsp--;

					PushFollow(Follow._rewrite_in_block750);
					rewrite();

					state._fsp--;


					}
					break;

				default:
					if ( cnt34 >= 1 )
						goto loop34;

					EarlyExitException eee34 = new EarlyExitException( 34, input );
					throw eee34;
				}
				cnt34++;
			}
			loop34:
				;


			Match(input,EOB,Follow._EOB_in_block758); 

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "block"


	// $ANTLR start "alternative"
	// Grammars\\AssignTokenTypesWalker.g3:273:0: alternative : ^( ALT ( element )+ EOA ) ;
	private void alternative(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:274:4: ( ^( ALT ( element )+ EOA ) )
			// Grammars\\AssignTokenTypesWalker.g3:274:4: ^( ALT ( element )+ EOA )
			{
			Match(input,ALT,Follow._ALT_in_alternative775); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:274:11: ( element )+
			int cnt35=0;
			for ( ; ; )
			{
				int alt35=2;
				int LA35_0 = input.LA(1);

				if ( (LA35_0==ACTION||(LA35_0>=ASSIGN && LA35_0<=BLOCK)||(LA35_0>=CHAR_LITERAL && LA35_0<=CHAR_RANGE)||LA35_0==CLOSURE||LA35_0==DOT||LA35_0==EPSILON||LA35_0==FORCED_ACTION||LA35_0==GATED_SEMPRED||LA35_0==NOT||LA35_0==OPTIONAL||(LA35_0>=PLUS_ASSIGN && LA35_0<=POSITIVE_CLOSURE)||LA35_0==RANGE||LA35_0==ROOT||LA35_0==RULE_REF||LA35_0==SEMPRED||(LA35_0>=STRING_LITERAL && LA35_0<=SYNPRED)||LA35_0==TOKEN_REF||LA35_0==TREE_BEGIN||LA35_0==WILDCARD) )
				{
					alt35=1;
				}


				switch ( alt35 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:274:12: element
					{
					PushFollow(Follow._element_in_alternative778);
					element();

					state._fsp--;


					}
					break;

				default:
					if ( cnt35 >= 1 )
						goto loop35;

					EarlyExitException eee35 = new EarlyExitException( 35, input );
					throw eee35;
				}
				cnt35++;
			}
			loop35:
				;


			Match(input,EOA,Follow._EOA_in_alternative782); 

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "alternative"


	// $ANTLR start "exceptionGroup"
	// Grammars\\AssignTokenTypesWalker.g3:277:0: exceptionGroup : ( ( exceptionHandler )+ ( finallyClause )? | finallyClause );
	private void exceptionGroup(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:278:4: ( ( exceptionHandler )+ ( finallyClause )? | finallyClause )
			int alt38=2;
			int LA38_0 = input.LA(1);

			if ( (LA38_0==CATCH) )
			{
				alt38=1;
			}
			else if ( (LA38_0==FINALLY) )
			{
				alt38=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 38, 0, input);

				throw nvae;
			}
			switch ( alt38 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:278:4: ( exceptionHandler )+ ( finallyClause )?
				{
				// Grammars\\AssignTokenTypesWalker.g3:278:4: ( exceptionHandler )+
				int cnt36=0;
				for ( ; ; )
				{
					int alt36=2;
					int LA36_0 = input.LA(1);

					if ( (LA36_0==CATCH) )
					{
						alt36=1;
					}


					switch ( alt36 )
					{
					case 1:
						// Grammars\\AssignTokenTypesWalker.g3:278:6: exceptionHandler
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup797);
						exceptionHandler();

						state._fsp--;


						}
						break;

					default:
						if ( cnt36 >= 1 )
							goto loop36;

						EarlyExitException eee36 = new EarlyExitException( 36, input );
						throw eee36;
					}
					cnt36++;
				}
				loop36:
					;


				// Grammars\\AssignTokenTypesWalker.g3:278:26: ( finallyClause )?
				int alt37=2;
				int LA37_0 = input.LA(1);

				if ( (LA37_0==FINALLY) )
				{
					alt37=1;
				}
				switch ( alt37 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:278:27: finallyClause
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup803);
					finallyClause();

					state._fsp--;


					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:279:4: finallyClause
				{
				PushFollow(Follow._finallyClause_in_exceptionGroup810);
				finallyClause();

				state._fsp--;


				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "exceptionGroup"


	// $ANTLR start "exceptionHandler"
	// Grammars\\AssignTokenTypesWalker.g3:282:0: exceptionHandler : ^( 'catch' ARG_ACTION ACTION ) ;
	private void exceptionHandler(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:283:4: ( ^( 'catch' ARG_ACTION ACTION ) )
			// Grammars\\AssignTokenTypesWalker.g3:283:4: ^( 'catch' ARG_ACTION ACTION )
			{
			Match(input,CATCH,Follow._CATCH_in_exceptionHandler822); 

			Match(input, TokenTypes.Down, null); 
			Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler824); 
			Match(input,ACTION,Follow._ACTION_in_exceptionHandler826); 

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "exceptionHandler"


	// $ANTLR start "finallyClause"
	// Grammars\\AssignTokenTypesWalker.g3:286:0: finallyClause : ^( 'finally' ACTION ) ;
	private void finallyClause(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:287:4: ( ^( 'finally' ACTION ) )
			// Grammars\\AssignTokenTypesWalker.g3:287:4: ^( 'finally' ACTION )
			{
			Match(input,FINALLY,Follow._FINALLY_in_finallyClause839); 

			Match(input, TokenTypes.Down, null); 
			Match(input,ACTION,Follow._ACTION_in_finallyClause841); 

			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "finallyClause"


	// $ANTLR start "rewrite"
	// Grammars\\AssignTokenTypesWalker.g3:290:0: rewrite : ( ^( REWRITE ( . )* ) )* ;
	private void rewrite(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:291:4: ( ( ^( REWRITE ( . )* ) )* )
			// Grammars\\AssignTokenTypesWalker.g3:291:4: ( ^( REWRITE ( . )* ) )*
			{
			// Grammars\\AssignTokenTypesWalker.g3:291:4: ( ^( REWRITE ( . )* ) )*
			for ( ; ; )
			{
				int alt40=2;
				int LA40_0 = input.LA(1);

				if ( (LA40_0==REWRITE) )
				{
					alt40=1;
				}


				switch ( alt40 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:291:6: ^( REWRITE ( . )* )
					{
					Match(input,REWRITE,Follow._REWRITE_in_rewrite856); 

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); 
						// Grammars\\AssignTokenTypesWalker.g3:291:16: ( . )*
						for ( ; ; )
						{
							int alt39=2;
							int LA39_0 = input.LA(1);

							if ( ((LA39_0>=ACTION && LA39_0<=CHARSET)) )
							{
								alt39=1;
							}
							else if ( (LA39_0==UP) )
							{
								alt39=2;
							}


							switch ( alt39 )
							{
							case 1:
								// Grammars\\AssignTokenTypesWalker.g3:291:0: .
								{
								MatchAny(input); 

								}
								break;

							default:
								goto loop39;
							}
						}

						loop39:
							;



						Match(input, TokenTypes.Up, null); 
					}

					}
					break;

				default:
					goto loop40;
				}
			}

			loop40:
				;



			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "rewrite"


	// $ANTLR start "element"
	// Grammars\\AssignTokenTypesWalker.g3:294:0: element : ( ^( ROOT element ) | ^( BANG element ) | atom | ^( NOT element ) | ^( RANGE atom atom ) | ^( CHAR_RANGE atom atom ) | ^( ASSIGN ID element ) | ^( PLUS_ASSIGN ID element ) | ebnf | tree_ | ^( SYNPRED block ) | FORCED_ACTION | ACTION | SEMPRED | SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) | GATED_SEMPRED | EPSILON );
	private void element(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:295:4: ( ^( ROOT element ) | ^( BANG element ) | atom | ^( NOT element ) | ^( RANGE atom atom ) | ^( CHAR_RANGE atom atom ) | ^( ASSIGN ID element ) | ^( PLUS_ASSIGN ID element ) | ebnf | tree_ | ^( SYNPRED block ) | FORCED_ACTION | ACTION | SEMPRED | SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) | GATED_SEMPRED | EPSILON )
			int alt42=18;
			switch ( input.LA(1) )
			{
			case ROOT:
				{
				alt42=1;
				}
				break;
			case BANG:
				{
				alt42=2;
				}
				break;
			case CHAR_LITERAL:
			case DOT:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
			case WILDCARD:
				{
				alt42=3;
				}
				break;
			case NOT:
				{
				alt42=4;
				}
				break;
			case RANGE:
				{
				alt42=5;
				}
				break;
			case CHAR_RANGE:
				{
				alt42=6;
				}
				break;
			case ASSIGN:
				{
				alt42=7;
				}
				break;
			case PLUS_ASSIGN:
				{
				alt42=8;
				}
				break;
			case BLOCK:
			case CLOSURE:
			case OPTIONAL:
			case POSITIVE_CLOSURE:
				{
				alt42=9;
				}
				break;
			case TREE_BEGIN:
				{
				alt42=10;
				}
				break;
			case SYNPRED:
				{
				alt42=11;
				}
				break;
			case FORCED_ACTION:
				{
				alt42=12;
				}
				break;
			case ACTION:
				{
				alt42=13;
				}
				break;
			case SEMPRED:
				{
				alt42=14;
				}
				break;
			case SYN_SEMPRED:
				{
				alt42=15;
				}
				break;
			case BACKTRACK_SEMPRED:
				{
				alt42=16;
				}
				break;
			case GATED_SEMPRED:
				{
				alt42=17;
				}
				break;
			case EPSILON:
				{
				alt42=18;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 42, 0, input);

					throw nvae;
				}
			}

			switch ( alt42 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:295:4: ^( ROOT element )
				{
				Match(input,ROOT,Follow._ROOT_in_element875); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._element_in_element877);
				element();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:296:4: ^( BANG element )
				{
				Match(input,BANG,Follow._BANG_in_element884); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._element_in_element886);
				element();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:297:4: atom
				{
				PushFollow(Follow._atom_in_element892);
				atom();

				state._fsp--;


				}
				break;
			case 4:
				// Grammars\\AssignTokenTypesWalker.g3:298:4: ^( NOT element )
				{
				Match(input,NOT,Follow._NOT_in_element898); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._element_in_element900);
				element();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 5:
				// Grammars\\AssignTokenTypesWalker.g3:299:4: ^( RANGE atom atom )
				{
				Match(input,RANGE,Follow._RANGE_in_element907); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._atom_in_element909);
				atom();

				state._fsp--;

				PushFollow(Follow._atom_in_element911);
				atom();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 6:
				// Grammars\\AssignTokenTypesWalker.g3:300:4: ^( CHAR_RANGE atom atom )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_element918); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._atom_in_element920);
				atom();

				state._fsp--;

				PushFollow(Follow._atom_in_element922);
				atom();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 7:
				// Grammars\\AssignTokenTypesWalker.g3:301:4: ^( ASSIGN ID element )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_element929); 

				Match(input, TokenTypes.Down, null); 
				Match(input,ID,Follow._ID_in_element931); 
				PushFollow(Follow._element_in_element933);
				element();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 8:
				// Grammars\\AssignTokenTypesWalker.g3:302:4: ^( PLUS_ASSIGN ID element )
				{
				Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_element940); 

				Match(input, TokenTypes.Down, null); 
				Match(input,ID,Follow._ID_in_element942); 
				PushFollow(Follow._element_in_element944);
				element();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 9:
				// Grammars\\AssignTokenTypesWalker.g3:303:4: ebnf
				{
				PushFollow(Follow._ebnf_in_element950);
				ebnf();

				state._fsp--;


				}
				break;
			case 10:
				// Grammars\\AssignTokenTypesWalker.g3:304:4: tree_
				{
				PushFollow(Follow._tree__in_element955);
				tree_();

				state._fsp--;


				}
				break;
			case 11:
				// Grammars\\AssignTokenTypesWalker.g3:305:4: ^( SYNPRED block )
				{
				Match(input,SYNPRED,Follow._SYNPRED_in_element962); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._block_in_element964);
				block();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 12:
				// Grammars\\AssignTokenTypesWalker.g3:306:4: FORCED_ACTION
				{
				Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_element971); 

				}
				break;
			case 13:
				// Grammars\\AssignTokenTypesWalker.g3:307:4: ACTION
				{
				Match(input,ACTION,Follow._ACTION_in_element976); 

				}
				break;
			case 14:
				// Grammars\\AssignTokenTypesWalker.g3:308:4: SEMPRED
				{
				Match(input,SEMPRED,Follow._SEMPRED_in_element981); 

				}
				break;
			case 15:
				// Grammars\\AssignTokenTypesWalker.g3:309:4: SYN_SEMPRED
				{
				Match(input,SYN_SEMPRED,Follow._SYN_SEMPRED_in_element986); 

				}
				break;
			case 16:
				// Grammars\\AssignTokenTypesWalker.g3:310:4: ^( BACKTRACK_SEMPRED ( . )* )
				{
				Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_element992); 

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); 
					// Grammars\\AssignTokenTypesWalker.g3:310:24: ( . )*
					for ( ; ; )
					{
						int alt41=2;
						int LA41_0 = input.LA(1);

						if ( ((LA41_0>=ACTION && LA41_0<=CHARSET)) )
						{
							alt41=1;
						}
						else if ( (LA41_0==UP) )
						{
							alt41=2;
						}


						switch ( alt41 )
						{
						case 1:
							// Grammars\\AssignTokenTypesWalker.g3:310:0: .
							{
							MatchAny(input); 

							}
							break;

						default:
							goto loop41;
						}
					}

					loop41:
						;



					Match(input, TokenTypes.Up, null); 
				}

				}
				break;
			case 17:
				// Grammars\\AssignTokenTypesWalker.g3:311:4: GATED_SEMPRED
				{
				Match(input,GATED_SEMPRED,Follow._GATED_SEMPRED_in_element1001); 

				}
				break;
			case 18:
				// Grammars\\AssignTokenTypesWalker.g3:312:4: EPSILON
				{
				Match(input,EPSILON,Follow._EPSILON_in_element1006); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "element"


	// $ANTLR start "ebnf"
	// Grammars\\AssignTokenTypesWalker.g3:315:0: ebnf : ( block | ^( OPTIONAL block ) | ^( CLOSURE block ) | ^( POSITIVE_CLOSURE block ) );
	private void ebnf(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:316:4: ( block | ^( OPTIONAL block ) | ^( CLOSURE block ) | ^( POSITIVE_CLOSURE block ) )
			int alt43=4;
			switch ( input.LA(1) )
			{
			case BLOCK:
				{
				alt43=1;
				}
				break;
			case OPTIONAL:
				{
				alt43=2;
				}
				break;
			case CLOSURE:
				{
				alt43=3;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt43=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 43, 0, input);

					throw nvae;
				}
			}

			switch ( alt43 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:316:4: block
				{
				PushFollow(Follow._block_in_ebnf1017);
				block();

				state._fsp--;


				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:317:4: ^( OPTIONAL block )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_ebnf1024); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._block_in_ebnf1026);
				block();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:318:4: ^( CLOSURE block )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_ebnf1035); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._block_in_ebnf1037);
				block();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;
			case 4:
				// Grammars\\AssignTokenTypesWalker.g3:319:4: ^( POSITIVE_CLOSURE block )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_ebnf1046); 

				Match(input, TokenTypes.Down, null); 
				PushFollow(Follow._block_in_ebnf1048);
				block();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "ebnf"


	// $ANTLR start "tree_"
	// Grammars\\AssignTokenTypesWalker.g3:322:0: tree_ : ^( TREE_BEGIN ( element )+ ) ;
	private void tree_(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:323:4: ( ^( TREE_BEGIN ( element )+ ) )
			// Grammars\\AssignTokenTypesWalker.g3:323:4: ^( TREE_BEGIN ( element )+ )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_1062); 

			Match(input, TokenTypes.Down, null); 
			// Grammars\\AssignTokenTypesWalker.g3:323:17: ( element )+
			int cnt44=0;
			for ( ; ; )
			{
				int alt44=2;
				int LA44_0 = input.LA(1);

				if ( (LA44_0==ACTION||(LA44_0>=ASSIGN && LA44_0<=BLOCK)||(LA44_0>=CHAR_LITERAL && LA44_0<=CHAR_RANGE)||LA44_0==CLOSURE||LA44_0==DOT||LA44_0==EPSILON||LA44_0==FORCED_ACTION||LA44_0==GATED_SEMPRED||LA44_0==NOT||LA44_0==OPTIONAL||(LA44_0>=PLUS_ASSIGN && LA44_0<=POSITIVE_CLOSURE)||LA44_0==RANGE||LA44_0==ROOT||LA44_0==RULE_REF||LA44_0==SEMPRED||(LA44_0>=STRING_LITERAL && LA44_0<=SYNPRED)||LA44_0==TOKEN_REF||LA44_0==TREE_BEGIN||LA44_0==WILDCARD) )
				{
					alt44=1;
				}


				switch ( alt44 )
				{
				case 1:
					// Grammars\\AssignTokenTypesWalker.g3:323:0: element
					{
					PushFollow(Follow._element_in_tree_1064);
					element();

					state._fsp--;


					}
					break;

				default:
					if ( cnt44 >= 1 )
						goto loop44;

					EarlyExitException eee44 = new EarlyExitException( 44, input );
					throw eee44;
				}
				cnt44++;
			}
			loop44:
				;



			Match(input, TokenTypes.Up, null); 

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "tree_"


	// $ANTLR start "atom"
	// Grammars\\AssignTokenTypesWalker.g3:326:0: atom : ( ^( RULE_REF ( ARG_ACTION )? ) | ^(t= TOKEN_REF ( ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL | WILDCARD | ^( DOT ID atom ) );
	private void atom(  )
	{
		GrammarAST t=null;
		GrammarAST c=null;
		GrammarAST s=null;

		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:327:4: ( ^( RULE_REF ( ARG_ACTION )? ) | ^(t= TOKEN_REF ( ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL | WILDCARD | ^( DOT ID atom ) )
			int alt47=6;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt47=1;
				}
				break;
			case TOKEN_REF:
				{
				alt47=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt47=3;
				}
				break;
			case STRING_LITERAL:
				{
				alt47=4;
				}
				break;
			case WILDCARD:
				{
				alt47=5;
				}
				break;
			case DOT:
				{
				alt47=6;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 47, 0, input);

					throw nvae;
				}
			}

			switch ( alt47 )
			{
			case 1:
				// Grammars\\AssignTokenTypesWalker.g3:327:4: ^( RULE_REF ( ARG_ACTION )? )
				{
				Match(input,RULE_REF,Follow._RULE_REF_in_atom1079); 

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); 
					// Grammars\\AssignTokenTypesWalker.g3:327:16: ( ARG_ACTION )?
					int alt45=2;
					int LA45_0 = input.LA(1);

					if ( (LA45_0==ARG_ACTION) )
					{
						alt45=1;
					}
					switch ( alt45 )
					{
					case 1:
						// Grammars\\AssignTokenTypesWalker.g3:327:17: ARG_ACTION
						{
						Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1082); 

						}
						break;

					}


					Match(input, TokenTypes.Up, null); 
				}

				}
				break;
			case 2:
				// Grammars\\AssignTokenTypesWalker.g3:328:4: ^(t= TOKEN_REF ( ARG_ACTION )? )
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom1095); 

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); 
					// Grammars\\AssignTokenTypesWalker.g3:328:19: ( ARG_ACTION )?
					int alt46=2;
					int LA46_0 = input.LA(1);

					if ( (LA46_0==ARG_ACTION) )
					{
						alt46=1;
					}
					switch ( alt46 )
					{
					case 1:
						// Grammars\\AssignTokenTypesWalker.g3:328:20: ARG_ACTION
						{
						Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1098); 

						}
						break;

					}


					Match(input, TokenTypes.Up, null); 
				}
				TrackToken(t);

				}
				break;
			case 3:
				// Grammars\\AssignTokenTypesWalker.g3:329:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom1112); 
				TrackString(c);

				}
				break;
			case 4:
				// Grammars\\AssignTokenTypesWalker.g3:330:4: s= STRING_LITERAL
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_atom1123); 
				TrackString(s);

				}
				break;
			case 5:
				// Grammars\\AssignTokenTypesWalker.g3:331:4: WILDCARD
				{
				Match(input,WILDCARD,Follow._WILDCARD_in_atom1130); 

				}
				break;
			case 6:
				// Grammars\\AssignTokenTypesWalker.g3:332:4: ^( DOT ID atom )
				{
				Match(input,DOT,Follow._DOT_in_atom1136); 

				Match(input, TokenTypes.Down, null); 
				Match(input,ID,Follow._ID_in_atom1138); 
				PushFollow(Follow._atom_in_atom1140);
				atom();

				state._fsp--;


				Match(input, TokenTypes.Up, null); 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "atom"


	// $ANTLR start "ast_suffix"
	// Grammars\\AssignTokenTypesWalker.g3:335:0: ast_suffix : ( ROOT | BANG );
	private void ast_suffix(  )
	{
		try
		{
			// Grammars\\AssignTokenTypesWalker.g3:336:4: ( ROOT | BANG )
			// Grammars\\AssignTokenTypesWalker.g3:
			{
			if ( input.LA(1)==BANG||input.LA(1)==ROOT )
			{
				input.Consume();
				state.errorRecovery=false;
			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				throw mse;
			}


			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
		}
		return ;
	}
	// $ANTLR end "ast_suffix"
	#endregion Rules


	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _LEXER_GRAMMAR_in_grammar_68 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_73 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PARSER_GRAMMAR_in_grammar_83 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_87 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_GRAMMAR_in_grammar_97 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_103 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _COMBINED_GRAMMAR_in_grammar_113 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_115 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_grammarSpec134 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammarSpec141 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _optionsSpec_in_grammarSpec148 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _delegateGrammars_in_grammarSpec155 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _tokensSpec_in_grammarSpec162 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _attrScope_in_grammarSpec169 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _AMPERSAND_in_grammarSpec178 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rules_in_grammarSpec190 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SCOPE_in_attrScope203 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScope205 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _AMPERSAND_in_attrScope210 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_attrScope219 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _OPTIONS_in_optionsSpec238 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _option_in_optionsSpec241 = new BitSet(new ulong[]{0x2008UL});
		public static readonly BitSet _ASSIGN_in_option260 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_option262 = new BitSet(new ulong[]{0x880000040000UL,0x200000UL});
		public static readonly BitSet _optionValue_in_option264 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_optionValue290 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_optionValue295 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_optionValue300 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_optionValue305 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHARSET_in_charSet323 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _charSetElement_in_charSet325 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_charSetElement338 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OR_in_charSetElement345 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_charSetElement347 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_charSetElement349 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RANGE_in_charSetElement358 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_charSetElement360 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_charSetElement362 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _IMPORT_in_delegateGrammars377 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ASSIGN_in_delegateGrammars385 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_delegateGrammars387 = new BitSet(new ulong[]{0x80000000000UL});
		public static readonly BitSet _ID_in_delegateGrammars389 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_delegateGrammars397 = new BitSet(new ulong[]{0x80000002008UL});
		public static readonly BitSet _TOKENS_in_tokensSpec420 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _tokenSpec_in_tokensSpec422 = new BitSet(new ulong[]{0x2008UL,0x4000000UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec438 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ASSIGN_in_tokenSpec458 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec465 = new BitSet(new ulong[]{0x40000UL,0x200000UL});
		public static readonly BitSet _STRING_LITERAL_in_tokenSpec482 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_tokenSpec493 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rule_in_rules517 = new BitSet(new ulong[]{0x400200008000202UL,0x8005000UL});
		public static readonly BitSet _RULE_in_rule531 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rule535 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_rule545 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_rule553 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule556 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RET_in_rule565 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule568 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _throwsSpec_in_rule577 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _optionsSpec_in_rule585 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _ruleScopeSpec_in_rule593 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _AMPERSAND_in_rule603 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_rule617 = new BitSet(new ulong[]{0x4400020000UL});
		public static readonly BitSet _exceptionGroup_in_rule623 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_rule630 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_modifier650 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec677 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_throwsSpec679 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec694 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _AMPERSAND_in_ruleScopeSpec699 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec709 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _ID_in_ruleScopeSpec715 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _BLOCK_in_block733 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _optionsSpec_in_block739 = new BitSet(new ulong[]{0x100UL});
		public static readonly BitSet _alternative_in_block748 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _rewrite_in_block750 = new BitSet(new ulong[]{0x200000100UL});
		public static readonly BitSet _EOB_in_block758 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_alternative775 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_alternative778 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _EOA_in_alternative782 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup797 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup803 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup810 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler822 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler824 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler826 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FINALLY_in_finallyClause839 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_finallyClause841 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REWRITE_in_rewrite856 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ROOT_in_element875 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element877 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BANG_in_element884 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element886 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _atom_in_element892 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_element898 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element900 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RANGE_in_element907 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element909 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element911 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_RANGE_in_element918 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element920 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element922 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_element929 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element931 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element933 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_ASSIGN_in_element940 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element942 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element944 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ebnf_in_element950 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_element955 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYNPRED_in_element962 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_element964 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FORCED_ACTION_in_element971 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_element976 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_element981 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYN_SEMPRED_in_element986 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_element992 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _GATED_SEMPRED_in_element1001 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EPSILON_in_element1006 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_ebnf1017 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_ebnf1024 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1026 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_ebnf1035 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1037 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_ebnf1046 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1048 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_1062 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_tree_1064 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _RULE_REF_in_atom1079 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1082 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom1095 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1098 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom1112 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_atom1123 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _WILDCARD_in_atom1130 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_atom1136 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_atom1138 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_atom1140 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_ast_suffix1153 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
