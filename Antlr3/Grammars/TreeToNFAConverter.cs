// $ANTLR 3.1.2 Grammars\\TreeToNFAConverter.g3 2009-09-30 13:28:47

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

using Antlr3.Analysis;
using Antlr3.Misc;
using Antlr3.Tool;

using BitSet = Antlr.Runtime.BitSet;
using DFA = Antlr.Runtime.DFA;


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.Grammars
{
/** Build an NFA from a tree representing an ANTLR grammar. */
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class TreeToNFAConverter : TreeParser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACTION", "ACTION_CHAR_LITERAL", "ACTION_ESC", "ACTION_STRING_LITERAL", "ALT", "AMPERSAND", "ARG", "ARG_ACTION", "ARGLIST", "ASSIGN", "BACKTRACK_SEMPRED", "BANG", "BLOCK", "CATCH", "CHAR_LITERAL", "CHAR_RANGE", "CLOSE_ELEMENT_OPTION", "CLOSURE", "COLON", "COMBINED_GRAMMAR", "COMMA", "COMMENT", "DIGIT", "DOC_COMMENT", "DOLLAR", "DOT", "DOUBLE_ANGLE_STRING_LITERAL", "DOUBLE_QUOTE_STRING_LITERAL", "EOA", "EOB", "EOR", "EPSILON", "ESC", "ETC", "FINALLY", "FORCED_ACTION", "FRAGMENT", "GATED_SEMPRED", "GRAMMAR", "ID", "IMPLIES", "IMPORT", "INITACTION", "INT", "LABEL", "LEXER", "LEXER_GRAMMAR", "LPAREN", "ML_COMMENT", "NESTED_ACTION", "NESTED_ARG_ACTION", "NOT", "OPEN_ELEMENT_OPTION", "OPTIONAL", "OPTIONS", "OR", "PARSER", "PARSER_GRAMMAR", "PLUS", "PLUS_ASSIGN", "POSITIVE_CLOSURE", "PRIVATE", "PROTECTED", "PUBLIC", "QUESTION", "RANGE", "RCURLY", "RET", "RETURNS", "REWRITE", "ROOT", "RPAREN", "RULE", "RULE_REF", "SCOPE", "SEMI", "SEMPRED", "SL_COMMENT", "SRC", "STAR", "STRAY_BRACKET", "STRING_LITERAL", "SYN_SEMPRED", "SYNPRED", "TEMPLATE", "THROWS", "TOKEN_REF", "TOKENS", "TREE", "TREE_BEGIN", "TREE_GRAMMAR", "WILDCARD", "WS", "WS_LOOP", "WS_OPT", "XDIGIT"
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

	// delegates
	// delegators

	public TreeToNFAConverter( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public TreeToNFAConverter( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] TokenNames { get { return TreeToNFAConverter.tokenNames; } }
	public override string GrammarFileName { get { return "Grammars\\TreeToNFAConverter.g3"; } }


	#region Rules

	// $ANTLR start "grammar_"
	// Grammars\\TreeToNFAConverter.g3:88:0: public grammar_ : ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) ;
	public void grammar_(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:93:4: ( ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) )
			// Grammars\\TreeToNFAConverter.g3:93:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
			{
			// Grammars\\TreeToNFAConverter.g3:93:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
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
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

					throw nvae;
				}
			}

			switch ( alt1 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:93:6: ^( LEXER_GRAMMAR grammarSpec )
				{
				Match(input,LEXER_GRAMMAR,Follow._LEXER_GRAMMAR_in_grammar_67); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_69);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:94:5: ^( PARSER_GRAMMAR grammarSpec )
				{
				Match(input,PARSER_GRAMMAR,Follow._PARSER_GRAMMAR_in_grammar_79); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_81);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:95:5: ^( TREE_GRAMMAR grammarSpec )
				{
				Match(input,TREE_GRAMMAR,Follow._TREE_GRAMMAR_in_grammar_91); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_93);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:96:5: ^( COMBINED_GRAMMAR grammarSpec )
				{
				Match(input,COMBINED_GRAMMAR,Follow._COMBINED_GRAMMAR_in_grammar_103); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_105);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;

			}


			}

			if ( state.backtracking == 0 )
			{

					Finish();

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


	// $ANTLR start "attrScope"
	// Grammars\\TreeToNFAConverter.g3:100:0: attrScope : ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) ;
	private void attrScope(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:101:4: ( ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) )
			// Grammars\\TreeToNFAConverter.g3:101:4: ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION )
			{
			Match(input,SCOPE,Follow._SCOPE_in_attrScope124); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,ID,Follow._ID_in_attrScope126); if (state.failed) return ;
			// Grammars\\TreeToNFAConverter.g3:101:18: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt3=2;
				int LA3_0 = input.LA(1);

				if ( (LA3_0==AMPERSAND) )
				{
					alt3=1;
				}


				switch ( alt3 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:101:20: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_attrScope131); if (state.failed) return ;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return ;
						// Grammars\\TreeToNFAConverter.g3:101:32: ( . )*
						for ( ; ; )
						{
							int alt2=2;
							int LA2_0 = input.LA(1);

							if ( ((LA2_0>=ACTION && LA2_0<=XDIGIT)) )
							{
								alt2=1;
							}
							else if ( (LA2_0==UP) )
							{
								alt2=2;
							}


							switch ( alt2 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:101:0: .
								{
								MatchAny(input); if (state.failed) return ;

								}
								break;

							default:
								goto loop2;
							}
						}

						loop2:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return ;
					}

					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;


			Match(input,ACTION,Follow._ACTION_in_attrScope140); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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


	// $ANTLR start "grammarSpec"
	// Grammars\\TreeToNFAConverter.g3:104:0: grammarSpec : ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules ;
	private void grammarSpec(  )
	{
		GrammarAST cmt=null;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:105:4: ( ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules )
			// Grammars\\TreeToNFAConverter.g3:105:4: ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules
			{
			Match(input,ID,Follow._ID_in_grammarSpec153); if (state.failed) return ;
			// Grammars\\TreeToNFAConverter.g3:106:3: (cmt= DOC_COMMENT )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==DOC_COMMENT) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:106:4: cmt= DOC_COMMENT
				{
				cmt=(GrammarAST)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammarSpec160); if (state.failed) return ;

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:107:3: ( ^( OPTIONS ( . )* ) )?
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==OPTIONS) )
			{
				alt6=1;
			}
			switch ( alt6 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:107:5: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_grammarSpec169); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\TreeToNFAConverter.g3:107:15: ( . )*
					for ( ; ; )
					{
						int alt5=2;
						int LA5_0 = input.LA(1);

						if ( ((LA5_0>=ACTION && LA5_0<=XDIGIT)) )
						{
							alt5=1;
						}
						else if ( (LA5_0==UP) )
						{
							alt5=2;
						}


						switch ( alt5 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:107:0: .
							{
							MatchAny(input); if (state.failed) return ;

							}
							break;

						default:
							goto loop5;
						}
					}

					loop5:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:108:3: ( ^( IMPORT ( . )* ) )?
			int alt8=2;
			int LA8_0 = input.LA(1);

			if ( (LA8_0==IMPORT) )
			{
				alt8=1;
			}
			switch ( alt8 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:108:5: ^( IMPORT ( . )* )
				{
				Match(input,IMPORT,Follow._IMPORT_in_grammarSpec183); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\TreeToNFAConverter.g3:108:14: ( . )*
					for ( ; ; )
					{
						int alt7=2;
						int LA7_0 = input.LA(1);

						if ( ((LA7_0>=ACTION && LA7_0<=XDIGIT)) )
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
							// Grammars\\TreeToNFAConverter.g3:108:0: .
							{
							MatchAny(input); if (state.failed) return ;

							}
							break;

						default:
							goto loop7;
						}
					}

					loop7:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:109:3: ( ^( TOKENS ( . )* ) )?
			int alt10=2;
			int LA10_0 = input.LA(1);

			if ( (LA10_0==TOKENS) )
			{
				alt10=1;
			}
			switch ( alt10 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:109:5: ^( TOKENS ( . )* )
				{
				Match(input,TOKENS,Follow._TOKENS_in_grammarSpec197); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\TreeToNFAConverter.g3:109:14: ( . )*
					for ( ; ; )
					{
						int alt9=2;
						int LA9_0 = input.LA(1);

						if ( ((LA9_0>=ACTION && LA9_0<=XDIGIT)) )
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
							// Grammars\\TreeToNFAConverter.g3:109:0: .
							{
							MatchAny(input); if (state.failed) return ;

							}
							break;

						default:
							goto loop9;
						}
					}

					loop9:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:110:3: ( attrScope )*
			for ( ; ; )
			{
				int alt11=2;
				int LA11_0 = input.LA(1);

				if ( (LA11_0==SCOPE) )
				{
					alt11=1;
				}


				switch ( alt11 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:110:4: attrScope
					{
					PushFollow(Follow._attrScope_in_grammarSpec209);
					attrScope();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					goto loop11;
				}
			}

			loop11:
				;


			// Grammars\\TreeToNFAConverter.g3:111:3: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt13=2;
				int LA13_0 = input.LA(1);

				if ( (LA13_0==AMPERSAND) )
				{
					alt13=1;
				}


				switch ( alt13 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:111:5: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_grammarSpec218); if (state.failed) return ;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return ;
						// Grammars\\TreeToNFAConverter.g3:111:17: ( . )*
						for ( ; ; )
						{
							int alt12=2;
							int LA12_0 = input.LA(1);

							if ( ((LA12_0>=ACTION && LA12_0<=XDIGIT)) )
							{
								alt12=1;
							}
							else if ( (LA12_0==UP) )
							{
								alt12=2;
							}


							switch ( alt12 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:111:0: .
								{
								MatchAny(input); if (state.failed) return ;

								}
								break;

							default:
								goto loop12;
							}
						}

						loop12:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return ;
					}

					}
					break;

				default:
					goto loop13;
				}
			}

			loop13:
				;


			PushFollow(Follow._rules_in_grammarSpec230);
			rules();

			state._fsp--;
			if (state.failed) return ;

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


	// $ANTLR start "rules"
	// Grammars\\TreeToNFAConverter.g3:115:0: rules : ( rule )+ ;
	private void rules(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:116:4: ( ( rule )+ )
			// Grammars\\TreeToNFAConverter.g3:116:4: ( rule )+
			{
			// Grammars\\TreeToNFAConverter.g3:116:4: ( rule )+
			int cnt14=0;
			for ( ; ; )
			{
				int alt14=2;
				int LA14_0 = input.LA(1);

				if ( (LA14_0==RULE) )
				{
					alt14=1;
				}


				switch ( alt14 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:116:0: rule
					{
					PushFollow(Follow._rule_in_rules241);
					rule();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					if ( cnt14 >= 1 )
						goto loop14;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee14 = new EarlyExitException( 14, input );
					throw eee14;
				}
				cnt14++;
			}
			loop14:
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

	public class rule_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rule"
	// Grammars\\TreeToNFAConverter.g3:119:0: rule : ^( RULE id= ID ( modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR ) ;
	private TreeToNFAConverter.rule_return rule(  )
	{
		TreeToNFAConverter.rule_return retval = new TreeToNFAConverter.rule_return();
		retval.start = input.LT(1);

		GrammarAST id=null;
		TreeToNFAConverter.block_return b = default(TreeToNFAConverter.block_return);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:120:4: ( ^( RULE id= ID ( modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR ) )
			// Grammars\\TreeToNFAConverter.g3:120:4: ^( RULE id= ID ( modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block ( exceptionGroup )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_rule255); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_rule259); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								currentRuleName = (id!=null?id.Text:null);
								factory.CurrentRule = grammar.GetLocallyDefinedRule( currentRuleName );
							
			}
			// Grammars\\TreeToNFAConverter.g3:125:4: ( modifier )?
			int alt15=2;
			int LA15_0 = input.LA(1);

			if ( (LA15_0==FRAGMENT||(LA15_0>=PRIVATE && LA15_0<=PUBLIC)) )
			{
				alt15=1;
			}
			switch ( alt15 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:125:5: modifier
				{
				PushFollow(Follow._modifier_in_rule270);
				modifier();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_rule278); if (state.failed) return retval;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				// Grammars\\TreeToNFAConverter.g3:126:10: ( ARG_ACTION )?
				int alt16=2;
				int LA16_0 = input.LA(1);

				if ( (LA16_0==ARG_ACTION) )
				{
					alt16=1;
				}
				switch ( alt16 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:126:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule281); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			}
			Match(input,RET,Follow._RET_in_rule290); if (state.failed) return retval;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				// Grammars\\TreeToNFAConverter.g3:127:10: ( ARG_ACTION )?
				int alt17=2;
				int LA17_0 = input.LA(1);

				if ( (LA17_0==ARG_ACTION) )
				{
					alt17=1;
				}
				switch ( alt17 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:127:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule293); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			}
			// Grammars\\TreeToNFAConverter.g3:128:4: ( throwsSpec )?
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( (LA18_0==THROWS) )
			{
				alt18=1;
			}
			switch ( alt18 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:128:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule302);
				throwsSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:129:4: ( ^( OPTIONS ( . )* ) )?
			int alt20=2;
			int LA20_0 = input.LA(1);

			if ( (LA20_0==OPTIONS) )
			{
				alt20=1;
			}
			switch ( alt20 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:129:6: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_rule312); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:129:16: ( . )*
					for ( ; ; )
					{
						int alt19=2;
						int LA19_0 = input.LA(1);

						if ( ((LA19_0>=ACTION && LA19_0<=XDIGIT)) )
						{
							alt19=1;
						}
						else if ( (LA19_0==UP) )
						{
							alt19=2;
						}


						switch ( alt19 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:129:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop19;
						}
					}

					loop19:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:130:4: ( ruleScopeSpec )?
			int alt21=2;
			int LA21_0 = input.LA(1);

			if ( (LA21_0==SCOPE) )
			{
				alt21=1;
			}
			switch ( alt21 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:130:6: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_rule326);
				ruleScopeSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:131:4: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt23=2;
				int LA23_0 = input.LA(1);

				if ( (LA23_0==AMPERSAND) )
				{
					alt23=1;
				}


				switch ( alt23 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:131:6: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_rule337); if (state.failed) return retval;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return retval;
						// Grammars\\TreeToNFAConverter.g3:131:18: ( . )*
						for ( ; ; )
						{
							int alt22=2;
							int LA22_0 = input.LA(1);

							if ( ((LA22_0>=ACTION && LA22_0<=XDIGIT)) )
							{
								alt22=1;
							}
							else if ( (LA22_0==UP) )
							{
								alt22=2;
							}


							switch ( alt22 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:131:0: .
								{
								MatchAny(input); if (state.failed) return retval;

								}
								break;

							default:
								goto loop22;
							}
						}

						loop22:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return retval;
					}

					}
					break;

				default:
					goto loop23;
				}
			}

			loop23:
				;


			PushFollow(Follow._block_in_rule351);
			b=block();

			state._fsp--;
			if (state.failed) return retval;
			// Grammars\\TreeToNFAConverter.g3:133:4: ( exceptionGroup )?
			int alt24=2;
			int LA24_0 = input.LA(1);

			if ( (LA24_0==CATCH||LA24_0==FINALLY) )
			{
				alt24=1;
			}
			switch ( alt24 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:133:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_rule357);
				exceptionGroup();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_rule364); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								StateCluster g = (b!=null?b.g:default(StateCluster));
								if ( (b!=null?((GrammarAST)b.Start):null).SetValue!=null )
								{
									// if block comes back as a set not BLOCK, make it
									// a single ALT block
									g = factory.BuildAlternativeBlockFromSet(g);
								}
								if ( char.IsLower(currentRuleName[0]) ||
									 grammar.type==GrammarType.Lexer )
								{
									// attach start node to block for this rule
									Rule thisR = grammar.GetLocallyDefinedRule(currentRuleName);
									NFAState start = thisR.startState;
									start.associatedASTNode = id;
									start.AddTransition(new Transition(Label.EPSILON, g.left));

									// track decision if > 1 alts
									if ( grammar.GetNumberOfAltsForDecisionNFA(g.left)>1 )
									{
										g.left.Description = grammar.GrammarTreeToString(((GrammarAST)retval.Start),false);
										g.left.SetDecisionASTNode((b!=null?((GrammarAST)b.Start):null));
										int d = grammar.AssignDecisionNumber( g.left );
										grammar.SetDecisionNFA( d, g.left );
										grammar.SetDecisionBlockAST(d, (b!=null?((GrammarAST)b.Start):null));
									}

									// hook to end of rule node
									NFAState end = thisR.stopState;
									g.right.AddTransition(new Transition(Label.EPSILON,end));
								}
							
			}

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "rule"


	// $ANTLR start "modifier"
	// Grammars\\TreeToNFAConverter.g3:170:0: modifier : ( 'protected' | 'public' | 'private' | 'fragment' );
	private void modifier(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:171:4: ( 'protected' | 'public' | 'private' | 'fragment' )
			// Grammars\\TreeToNFAConverter.g3:
			{
			if ( input.LA(1)==FRAGMENT||(input.LA(1)>=PRIVATE && input.LA(1)<=PUBLIC) )
			{
				input.Consume();
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
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
	// $ANTLR end "modifier"


	// $ANTLR start "throwsSpec"
	// Grammars\\TreeToNFAConverter.g3:177:0: throwsSpec : ^( 'throws' ( ID )+ ) ;
	private void throwsSpec(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:178:4: ( ^( 'throws' ( ID )+ ) )
			// Grammars\\TreeToNFAConverter.g3:178:4: ^( 'throws' ( ID )+ )
			{
			Match(input,THROWS,Follow._THROWS_in_throwsSpec411); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			// Grammars\\TreeToNFAConverter.g3:178:15: ( ID )+
			int cnt25=0;
			for ( ; ; )
			{
				int alt25=2;
				int LA25_0 = input.LA(1);

				if ( (LA25_0==ID) )
				{
					alt25=1;
				}


				switch ( alt25 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:178:0: ID
					{
					Match(input,ID,Follow._ID_in_throwsSpec413); if (state.failed) return ;

					}
					break;

				default:
					if ( cnt25 >= 1 )
						goto loop25;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee25 = new EarlyExitException( 25, input );
					throw eee25;
				}
				cnt25++;
			}
			loop25:
				;



			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// Grammars\\TreeToNFAConverter.g3:181:0: ruleScopeSpec : ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) ;
	private void ruleScopeSpec(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:182:4: ( ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) )
			// Grammars\\TreeToNFAConverter.g3:182:4: ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* )
			{
			Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec428); if (state.failed) return ;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				// Grammars\\TreeToNFAConverter.g3:182:15: ( ^( AMPERSAND ( . )* ) )*
				for ( ; ; )
				{
					int alt27=2;
					int LA27_0 = input.LA(1);

					if ( (LA27_0==AMPERSAND) )
					{
						alt27=1;
					}


					switch ( alt27 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:182:17: ^( AMPERSAND ( . )* )
						{
						Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleScopeSpec433); if (state.failed) return ;

						if ( input.LA(1)==TokenTypes.Down )
						{
							Match(input, TokenTypes.Down, null); if (state.failed) return ;
							// Grammars\\TreeToNFAConverter.g3:182:29: ( . )*
							for ( ; ; )
							{
								int alt26=2;
								int LA26_0 = input.LA(1);

								if ( ((LA26_0>=ACTION && LA26_0<=XDIGIT)) )
								{
									alt26=1;
								}
								else if ( (LA26_0==UP) )
								{
									alt26=2;
								}


								switch ( alt26 )
								{
								case 1:
									// Grammars\\TreeToNFAConverter.g3:182:0: .
									{
									MatchAny(input); if (state.failed) return ;

									}
									break;

								default:
									goto loop26;
								}
							}

							loop26:
								;



							Match(input, TokenTypes.Up, null); if (state.failed) return ;
						}

						}
						break;

					default:
						goto loop27;
					}
				}

				loop27:
					;


				// Grammars\\TreeToNFAConverter.g3:182:36: ( ACTION )?
				int alt28=2;
				int LA28_0 = input.LA(1);

				if ( (LA28_0==ACTION) )
				{
					alt28=1;
				}
				switch ( alt28 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:182:37: ACTION
					{
					Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec443); if (state.failed) return ;

					}
					break;

				}

				// Grammars\\TreeToNFAConverter.g3:182:46: ( ID )*
				for ( ; ; )
				{
					int alt29=2;
					int LA29_0 = input.LA(1);

					if ( (LA29_0==ID) )
					{
						alt29=1;
					}


					switch ( alt29 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:182:48: ID
						{
						Match(input,ID,Follow._ID_in_ruleScopeSpec449); if (state.failed) return ;

						}
						break;

					default:
						goto loop29;
					}
				}

				loop29:
					;



				Match(input, TokenTypes.Up, null); if (state.failed) return ;
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
		public StateCluster g = null;
	}

	// $ANTLR start "block"
	// Grammars\\TreeToNFAConverter.g3:185:0: block returns [StateCluster g = null] : ({...}? => set | ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative rewrite )+ EOB ) );
	private TreeToNFAConverter.block_return block(  )
	{
		TreeToNFAConverter.block_return retval = new TreeToNFAConverter.block_return();
		retval.start = input.LT(1);

		StateCluster a = default(StateCluster);
		TreeToNFAConverter.set_return set1 = default(TreeToNFAConverter.set_return);


			List<StateCluster> alts = new List<StateCluster>();
			this.blockLevel++;
			if ( this.blockLevel==1 )
				this.outerAltNum=1;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:193:4: ({...}? => set | ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative rewrite )+ EOB ) )
			int alt33=2;
			int LA33_0 = input.LA(1);

			if ( (LA33_0==BLOCK) )
			{
				int LA33_1 = input.LA(2);

				if ( ((grammar.IsValidSet(this,((GrammarAST)retval.Start)) &&
						 !currentRuleName.Equals(Grammar.ArtificialTokensRuleName))) )
				{
					alt33=1;
				}
				else if ( (true) )
				{
					alt33=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 33, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 33, 0, input);

				throw nvae;
			}
			switch ( alt33 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:193:4: {...}? => set
				{
				if ( !((grammar.IsValidSet(this,((GrammarAST)retval.Start)) &&
						 !currentRuleName.Equals(Grammar.ArtificialTokensRuleName))) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "block", "grammar.IsValidSet(this,$start) &&\r\n\t\t !currentRuleName.Equals(Grammar.ArtificialTokensRuleName)");
				}
				PushFollow(Follow._set_in_block480);
				set1=set();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (set1!=null?set1.g:default(StateCluster));
				}

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:197:4: ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative rewrite )+ EOB )
				{
				Match(input,BLOCK,Follow._BLOCK_in_block490); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				// Grammars\\TreeToNFAConverter.g3:197:13: ( ^( OPTIONS ( . )* ) )?
				int alt31=2;
				int LA31_0 = input.LA(1);

				if ( (LA31_0==OPTIONS) )
				{
					alt31=1;
				}
				switch ( alt31 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:197:15: ^( OPTIONS ( . )* )
					{
					Match(input,OPTIONS,Follow._OPTIONS_in_block495); if (state.failed) return retval;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return retval;
						// Grammars\\TreeToNFAConverter.g3:197:25: ( . )*
						for ( ; ; )
						{
							int alt30=2;
							int LA30_0 = input.LA(1);

							if ( ((LA30_0>=ACTION && LA30_0<=XDIGIT)) )
							{
								alt30=1;
							}
							else if ( (LA30_0==UP) )
							{
								alt30=2;
							}


							switch ( alt30 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:197:0: .
								{
								MatchAny(input); if (state.failed) return retval;

								}
								break;

							default:
								goto loop30;
							}
						}

						loop30:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return retval;
					}

					}
					break;

				}

				// Grammars\\TreeToNFAConverter.g3:198:4: (a= alternative rewrite )+
				int cnt32=0;
				for ( ; ; )
				{
					int alt32=2;
					int LA32_0 = input.LA(1);

					if ( (LA32_0==ALT) )
					{
						alt32=1;
					}


					switch ( alt32 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:198:6: a= alternative rewrite
						{
						PushFollow(Follow._alternative_in_block511);
						a=alternative();

						state._fsp--;
						if (state.failed) return retval;
						PushFollow(Follow._rewrite_in_block513);
						rewrite();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 )
						{

												alts.Add(a);
											
						}

											if ( blockLevel == 1 )
												outerAltNum++;
										

						}
						break;

					default:
						if ( cnt32 >= 1 )
							goto loop32;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee32 = new EarlyExitException( 32, input );
						throw eee32;
					}
					cnt32++;
				}
				loop32:
					;


				Match(input,EOB,Follow._EOB_in_block536); if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildAlternativeBlock(alts);
				}

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
			 blockLevel--; 
		}
		return retval;
	}
	// $ANTLR end "block"


	// $ANTLR start "alternative"
	// Grammars\\TreeToNFAConverter.g3:213:0: alternative returns [StateCluster g=null] : ^( ALT (e= element )+ EOA ) ;
	private StateCluster alternative(  )
	{
		StateCluster g = null;

		TreeToNFAConverter.element_return e = default(TreeToNFAConverter.element_return);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:214:4: ( ^( ALT (e= element )+ EOA ) )
			// Grammars\\TreeToNFAConverter.g3:214:4: ^( ALT (e= element )+ EOA )
			{
			Match(input,ALT,Follow._ALT_in_alternative565); if (state.failed) return g;

			Match(input, TokenTypes.Down, null); if (state.failed) return g;
			// Grammars\\TreeToNFAConverter.g3:214:11: (e= element )+
			int cnt34=0;
			for ( ; ; )
			{
				int alt34=2;
				int LA34_0 = input.LA(1);

				if ( (LA34_0==ACTION||(LA34_0>=ASSIGN && LA34_0<=BLOCK)||(LA34_0>=CHAR_LITERAL && LA34_0<=CHAR_RANGE)||LA34_0==CLOSURE||LA34_0==DOT||LA34_0==EPSILON||LA34_0==FORCED_ACTION||LA34_0==GATED_SEMPRED||LA34_0==NOT||LA34_0==OPTIONAL||(LA34_0>=PLUS_ASSIGN && LA34_0<=POSITIVE_CLOSURE)||LA34_0==RANGE||LA34_0==ROOT||LA34_0==RULE_REF||LA34_0==SEMPRED||(LA34_0>=STRING_LITERAL && LA34_0<=SYNPRED)||LA34_0==TOKEN_REF||LA34_0==TREE_BEGIN||LA34_0==WILDCARD) )
				{
					alt34=1;
				}


				switch ( alt34 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:214:12: e= element
					{
					PushFollow(Follow._element_in_alternative570);
					e=element();

					state._fsp--;
					if (state.failed) return g;
					if ( state.backtracking == 0 )
					{
						g = factory.BuildAB(g,(e!=null?e.g:default(StateCluster)));
					}

					}
					break;

				default:
					if ( cnt34 >= 1 )
						goto loop34;

					if (state.backtracking>0) {state.failed=true; return g;}
					EarlyExitException eee34 = new EarlyExitException( 34, input );
					throw eee34;
				}
				cnt34++;
			}
			loop34:
				;


			Match(input,EOA,Follow._EOA_in_alternative577); if (state.failed) return g;

			Match(input, TokenTypes.Up, null); if (state.failed) return g;
			if ( state.backtracking == 0 )
			{

							if (g==null) { // if alt was a list of actions or whatever
								g = factory.BuildEpsilon();
							}
							else {
								factory.OptimizeAlternative(g);
							}
						
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
		return g;
	}
	// $ANTLR end "alternative"


	// $ANTLR start "exceptionGroup"
	// Grammars\\TreeToNFAConverter.g3:225:0: exceptionGroup : ( ( exceptionHandler )+ ( finallyClause )? | finallyClause );
	private void exceptionGroup(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:226:4: ( ( exceptionHandler )+ ( finallyClause )? | finallyClause )
			int alt37=2;
			int LA37_0 = input.LA(1);

			if ( (LA37_0==CATCH) )
			{
				alt37=1;
			}
			else if ( (LA37_0==FINALLY) )
			{
				alt37=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 37, 0, input);

				throw nvae;
			}
			switch ( alt37 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:226:4: ( exceptionHandler )+ ( finallyClause )?
				{
				// Grammars\\TreeToNFAConverter.g3:226:4: ( exceptionHandler )+
				int cnt35=0;
				for ( ; ; )
				{
					int alt35=2;
					int LA35_0 = input.LA(1);

					if ( (LA35_0==CATCH) )
					{
						alt35=1;
					}


					switch ( alt35 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:226:6: exceptionHandler
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup596);
						exceptionHandler();

						state._fsp--;
						if (state.failed) return ;

						}
						break;

					default:
						if ( cnt35 >= 1 )
							goto loop35;

						if (state.backtracking>0) {state.failed=true; return ;}
						EarlyExitException eee35 = new EarlyExitException( 35, input );
						throw eee35;
					}
					cnt35++;
				}
				loop35:
					;


				// Grammars\\TreeToNFAConverter.g3:226:26: ( finallyClause )?
				int alt36=2;
				int LA36_0 = input.LA(1);

				if ( (LA36_0==FINALLY) )
				{
					alt36=1;
				}
				switch ( alt36 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:226:27: finallyClause
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup602);
					finallyClause();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:227:4: finallyClause
				{
				PushFollow(Follow._finallyClause_in_exceptionGroup609);
				finallyClause();

				state._fsp--;
				if (state.failed) return ;

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
	// Grammars\\TreeToNFAConverter.g3:230:0: exceptionHandler : ^( 'catch' ARG_ACTION ACTION ) ;
	private void exceptionHandler(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:231:7: ( ^( 'catch' ARG_ACTION ACTION ) )
			// Grammars\\TreeToNFAConverter.g3:231:7: ^( 'catch' ARG_ACTION ACTION )
			{
			Match(input,CATCH,Follow._CATCH_in_exceptionHandler624); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler626); if (state.failed) return ;
			Match(input,ACTION,Follow._ACTION_in_exceptionHandler628); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// Grammars\\TreeToNFAConverter.g3:234:0: finallyClause : ^( 'finally' ACTION ) ;
	private void finallyClause(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:235:7: ( ^( 'finally' ACTION ) )
			// Grammars\\TreeToNFAConverter.g3:235:7: ^( 'finally' ACTION )
			{
			Match(input,FINALLY,Follow._FINALLY_in_finallyClause644); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,ACTION,Follow._ACTION_in_finallyClause646); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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

	public class rewrite_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rewrite"
	// Grammars\\TreeToNFAConverter.g3:238:0: rewrite : ( ^( REWRITE ( . )* ) )* ;
	private TreeToNFAConverter.rewrite_return rewrite(  )
	{
		TreeToNFAConverter.rewrite_return retval = new TreeToNFAConverter.rewrite_return();
		retval.start = input.LT(1);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:239:4: ( ( ^( REWRITE ( . )* ) )* )
			// Grammars\\TreeToNFAConverter.g3:239:4: ( ^( REWRITE ( . )* ) )*
			{
			// Grammars\\TreeToNFAConverter.g3:239:4: ( ^( REWRITE ( . )* ) )*
			for ( ; ; )
			{
				int alt39=2;
				int LA39_0 = input.LA(1);

				if ( (LA39_0==REWRITE) )
				{
					alt39=1;
				}


				switch ( alt39 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:240:4: ^( REWRITE ( . )* )
					{
					if ( state.backtracking == 0 )
					{

										if ( grammar.GetOption("output")==null )
										{
											ErrorManager.GrammarError(ErrorManager.MSG_REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION,
																	  grammar, ((GrammarAST)retval.Start).Token, currentRuleName);
										}
									
					}
					Match(input,REWRITE,Follow._REWRITE_in_rewrite669); if (state.failed) return retval;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return retval;
						// Grammars\\TreeToNFAConverter.g3:247:14: ( . )*
						for ( ; ; )
						{
							int alt38=2;
							int LA38_0 = input.LA(1);

							if ( ((LA38_0>=ACTION && LA38_0<=XDIGIT)) )
							{
								alt38=1;
							}
							else if ( (LA38_0==UP) )
							{
								alt38=2;
							}


							switch ( alt38 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:247:0: .
								{
								MatchAny(input); if (state.failed) return retval;

								}
								break;

							default:
								goto loop38;
							}
						}

						loop38:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return retval;
					}

					}
					break;

				default:
					goto loop39;
				}
			}

			loop39:
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
		return retval;
	}
	// $ANTLR end "rewrite"

	public class element_return : TreeRuleReturnScope
	{
		public StateCluster g=null;
	}

	// $ANTLR start "element"
	// Grammars\\TreeToNFAConverter.g3:251:0: element returns [StateCluster g=null] : ( ^( ROOT e= element ) | ^( BANG e= element ) | ^( ASSIGN ID e= element ) | ^( PLUS_ASSIGN ID e= element ) | ^( RANGE a= atom[null] b= atom[null] ) | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) | atom_or_notatom | ebnf | tree_ | ^( SYNPRED block ) | ACTION | FORCED_ACTION |pred= SEMPRED |spred= SYN_SEMPRED | ^(bpred= BACKTRACK_SEMPRED ( . )* ) |gpred= GATED_SEMPRED | EPSILON );
	private TreeToNFAConverter.element_return element(  )
	{
		TreeToNFAConverter.element_return retval = new TreeToNFAConverter.element_return();
		retval.start = input.LT(1);

		GrammarAST c1=null;
		GrammarAST c2=null;
		GrammarAST pred=null;
		GrammarAST spred=null;
		GrammarAST bpred=null;
		GrammarAST gpred=null;
		GrammarAST ACTION5=null;
		GrammarAST FORCED_ACTION6=null;
		TreeToNFAConverter.element_return e = default(TreeToNFAConverter.element_return);
		TreeToNFAConverter.atom_return a = default(TreeToNFAConverter.atom_return);
		TreeToNFAConverter.atom_return b = default(TreeToNFAConverter.atom_return);
		StateCluster atom_or_notatom2 = default(StateCluster);
		TreeToNFAConverter.ebnf_return ebnf3 = default(TreeToNFAConverter.ebnf_return);
		TreeToNFAConverter.tree__return tree_4 = default(TreeToNFAConverter.tree__return);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:252:6: ( ^( ROOT e= element ) | ^( BANG e= element ) | ^( ASSIGN ID e= element ) | ^( PLUS_ASSIGN ID e= element ) | ^( RANGE a= atom[null] b= atom[null] ) | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) | atom_or_notatom | ebnf | tree_ | ^( SYNPRED block ) | ACTION | FORCED_ACTION |pred= SEMPRED |spred= SYN_SEMPRED | ^(bpred= BACKTRACK_SEMPRED ( . )* ) |gpred= GATED_SEMPRED | EPSILON )
			int alt41=17;
			alt41 = dfa41.Predict(input);
			switch ( alt41 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:252:6: ^( ROOT e= element )
				{
				Match(input,ROOT,Follow._ROOT_in_element696); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element700);
				e=element();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (e!=null?e.g:default(StateCluster));
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:253:6: ^( BANG e= element )
				{
				Match(input,BANG,Follow._BANG_in_element711); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element715);
				e=element();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (e!=null?e.g:default(StateCluster));
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:254:4: ^( ASSIGN ID e= element )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_element724); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				Match(input,ID,Follow._ID_in_element726); if (state.failed) return retval;
				PushFollow(Follow._element_in_element730);
				e=element();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (e!=null?e.g:default(StateCluster));
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:255:4: ^( PLUS_ASSIGN ID e= element )
				{
				Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_element739); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				Match(input,ID,Follow._ID_in_element741); if (state.failed) return retval;
				PushFollow(Follow._element_in_element745);
				e=element();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (e!=null?e.g:default(StateCluster));
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 5:
				// Grammars\\TreeToNFAConverter.g3:256:6: ^( RANGE a= atom[null] b= atom[null] )
				{
				Match(input,RANGE,Follow._RANGE_in_element756); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._atom_in_element760);
				a=atom(null);

				state._fsp--;
				if (state.failed) return retval;
				PushFollow(Follow._atom_in_element765);
				b=atom(null);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildRange(grammar.GetTokenType((a!=null?(input.TokenStream.ToString(
					  input.TreeAdaptor.GetTokenStartIndex(a.Start),
					  input.TreeAdaptor.GetTokenStopIndex(a.Start))):null)),
													 grammar.GetTokenType((b!=null?(input.TokenStream.ToString(
					  input.TreeAdaptor.GetTokenStartIndex(b.Start),
					  input.TreeAdaptor.GetTokenStopIndex(b.Start))):null)));
				}

				}
				break;
			case 6:
				// Grammars\\TreeToNFAConverter.g3:259:6: ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_element779); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				c1=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_element783); if (state.failed) return retval;
				c2=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_element787); if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

							if ( grammar.type==GrammarType.Lexer ) {
								retval.g = factory.BuildCharRange((c1!=null?c1.Text:null), (c2!=null?c2.Text:null));
							}
							
				}

				}
				break;
			case 7:
				// Grammars\\TreeToNFAConverter.g3:265:6: atom_or_notatom
				{
				PushFollow(Follow._atom_or_notatom_in_element799);
				atom_or_notatom2=atom_or_notatom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = atom_or_notatom2;
				}

				}
				break;
			case 8:
				// Grammars\\TreeToNFAConverter.g3:266:6: ebnf
				{
				PushFollow(Follow._ebnf_in_element808);
				ebnf3=ebnf();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (ebnf3!=null?ebnf3.g:default(StateCluster));
				}

				}
				break;
			case 9:
				// Grammars\\TreeToNFAConverter.g3:267:6: tree_
				{
				PushFollow(Follow._tree__in_element817);
				tree_4=tree_();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (tree_4!=null?tree_4.g:default(StateCluster));
				}

				}
				break;
			case 10:
				// Grammars\\TreeToNFAConverter.g3:268:6: ^( SYNPRED block )
				{
				Match(input,SYNPRED,Follow._SYNPRED_in_element828); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_element830);
				block();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 11:
				// Grammars\\TreeToNFAConverter.g3:269:6: ACTION
				{
				ACTION5=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_element839); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildAction(ACTION5);
				}

				}
				break;
			case 12:
				// Grammars\\TreeToNFAConverter.g3:270:6: FORCED_ACTION
				{
				FORCED_ACTION6=(GrammarAST)Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_element848); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildAction(FORCED_ACTION6);
				}

				}
				break;
			case 13:
				// Grammars\\TreeToNFAConverter.g3:271:6: pred= SEMPRED
				{
				pred=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_element859); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildSemanticPredicate(pred);
				}

				}
				break;
			case 14:
				// Grammars\\TreeToNFAConverter.g3:272:6: spred= SYN_SEMPRED
				{
				spred=(GrammarAST)Match(input,SYN_SEMPRED,Follow._SYN_SEMPRED_in_element870); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildSemanticPredicate(spred);
				}

				}
				break;
			case 15:
				// Grammars\\TreeToNFAConverter.g3:273:6: ^(bpred= BACKTRACK_SEMPRED ( . )* )
				{
				bpred=(GrammarAST)Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_element882); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:273:32: ( . )*
					for ( ; ; )
					{
						int alt40=2;
						int LA40_0 = input.LA(1);

						if ( ((LA40_0>=ACTION && LA40_0<=XDIGIT)) )
						{
							alt40=1;
						}
						else if ( (LA40_0==UP) )
						{
							alt40=2;
						}


						switch ( alt40 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:273:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop40;
						}
					}

					loop40:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildSemanticPredicate(bpred);
				}

				}
				break;
			case 16:
				// Grammars\\TreeToNFAConverter.g3:274:6: gpred= GATED_SEMPRED
				{
				gpred=(GrammarAST)Match(input,GATED_SEMPRED,Follow._GATED_SEMPRED_in_element897); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildSemanticPredicate(gpred);
				}

				}
				break;
			case 17:
				// Grammars\\TreeToNFAConverter.g3:275:6: EPSILON
				{
				Match(input,EPSILON,Follow._EPSILON_in_element906); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = factory.BuildEpsilon();
				}

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
	// $ANTLR end "element"

	public class ebnf_return : TreeRuleReturnScope
	{
		public StateCluster g=null;
	}

	// $ANTLR start "ebnf"
	// Grammars\\TreeToNFAConverter.g3:278:0: ebnf returns [StateCluster g=null] : ({...}? => set |b= block | ^( OPTIONAL b= block ) | ^( CLOSURE b= block ) | ^( POSITIVE_CLOSURE b= block ) );
	private TreeToNFAConverter.ebnf_return ebnf(  )
	{
		TreeToNFAConverter.ebnf_return retval = new TreeToNFAConverter.ebnf_return();
		retval.start = input.LT(1);

		TreeToNFAConverter.block_return b = default(TreeToNFAConverter.block_return);
		TreeToNFAConverter.set_return set7 = default(TreeToNFAConverter.set_return);


			GrammarAST blk = ((GrammarAST)retval.Start);
			if ( blk.Type!=BLOCK ) {
				blk = (GrammarAST)blk.GetChild(0);
			}
			GrammarAST eob = blk.LastChild;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:287:4: ({...}? => set |b= block | ^( OPTIONAL b= block ) | ^( CLOSURE b= block ) | ^( POSITIVE_CLOSURE b= block ) )
			int alt42=5;
			switch ( input.LA(1) )
			{
			case BLOCK:
				{
				int LA42_1 = input.LA(2);

				if ( ((grammar.IsValidSet(this,((GrammarAST)retval.Start)))) )
				{
					alt42=1;
				}
				else if ( (true) )
				{
					alt42=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 42, 1, input);

					throw nvae;
				}
				}
				break;
			case OPTIONAL:
				{
				alt42=3;
				}
				break;
			case CLOSURE:
				{
				alt42=4;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt42=5;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 42, 0, input);

					throw nvae;
				}
			}

			switch ( alt42 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:287:4: {...}? => set
				{
				if ( !((grammar.IsValidSet(this,((GrammarAST)retval.Start)))) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "ebnf", "grammar.IsValidSet(this,$start)");
				}
				PushFollow(Follow._set_in_ebnf932);
				set7=set();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (set7!=null?set7.g:default(StateCluster));
				}

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:289:4: b= block
				{
				PushFollow(Follow._block_in_ebnf942);
				b=block();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								// track decision if > 1 alts
								if ( grammar.GetNumberOfAltsForDecisionNFA((b!=null?b.g:default(StateCluster)).left)>1 )
								{
									(b!=null?b.g:default(StateCluster)).left.Description = grammar.GrammarTreeToString(blk,false);
									(b!=null?b.g:default(StateCluster)).left.SetDecisionASTNode(blk);
									int d = grammar.AssignDecisionNumber( (b!=null?b.g:default(StateCluster)).left );
									grammar.SetDecisionNFA( d, (b!=null?b.g:default(StateCluster)).left );
									grammar.SetDecisionBlockAST(d, blk);
								}
								retval.g = (b!=null?b.g:default(StateCluster));
							
				}

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:302:4: ^( OPTIONAL b= block )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_ebnf953); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf957);
				b=block();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								StateCluster bg = (b!=null?b.g:default(StateCluster));
								if ( blk.SetValue!=null )
								{
									// if block comes back SET not BLOCK, make it
									// a single ALT block
									bg = factory.BuildAlternativeBlockFromSet(bg);
								}
								retval.g = factory.BuildAoptional(bg);
								retval.g.left.Description = grammar.GrammarTreeToString(((GrammarAST)retval.Start),false);
								// there is always at least one alt even if block has just 1 alt
								int d = grammar.AssignDecisionNumber( retval.g.left );
								grammar.SetDecisionNFA(d, retval.g.left);
								grammar.SetDecisionBlockAST(d, blk);
								retval.g.left.SetDecisionASTNode(((GrammarAST)retval.Start));
							
				}

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:319:4: ^( CLOSURE b= block )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_ebnf970); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf974);
				b=block();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								StateCluster bg = (b!=null?b.g:default(StateCluster));
								if ( blk.SetValue!=null )
								{
									bg = factory.BuildAlternativeBlockFromSet(bg);
								}
								retval.g = factory.BuildAstar(bg);
								// track the loop back / exit decision point
								bg.right.Description = "()* loopback of "+grammar.GrammarTreeToString(((GrammarAST)retval.Start),false);
								int d = grammar.AssignDecisionNumber( bg.right );
								grammar.SetDecisionNFA(d, bg.right);
								grammar.SetDecisionBlockAST(d, blk);
								bg.right.SetDecisionASTNode(eob);
								// make block entry state also have same decision for interpreting grammar
								NFAState altBlockState = (NFAState)retval.g.left.GetTransition(0).target;
								altBlockState.SetDecisionASTNode(((GrammarAST)retval.Start));
								altBlockState.DecisionNumber = d;
								retval.g.left.DecisionNumber = d; // this is the bypass decision (2 alts)
								retval.g.left.SetDecisionASTNode(((GrammarAST)retval.Start));
							
				}

				}
				break;
			case 5:
				// Grammars\\TreeToNFAConverter.g3:340:4: ^( POSITIVE_CLOSURE b= block )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_ebnf987); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf991);
				b=block();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								StateCluster bg = (b!=null?b.g:default(StateCluster));
								if ( blk.SetValue!=null )
								{
									bg = factory.BuildAlternativeBlockFromSet(bg);
								}
								retval.g = factory.BuildAplus(bg);
								// don't make a decision on left edge, can reuse loop end decision
								// track the loop back / exit decision point
								bg.right.Description = "()+ loopback of "+grammar.GrammarTreeToString(((GrammarAST)retval.Start),false);
								int d = grammar.AssignDecisionNumber( bg.right );
								grammar.SetDecisionNFA(d, bg.right);
								grammar.SetDecisionBlockAST(d, blk);
								bg.right.SetDecisionASTNode(eob);
								// make block entry state also have same decision for interpreting grammar
								NFAState altBlockState = (NFAState)retval.g.left.GetTransition(0).target;
								altBlockState.SetDecisionASTNode(((GrammarAST)retval.Start));
								altBlockState.DecisionNumber = d;
							
				}

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
	// $ANTLR end "ebnf"

	public class tree__return : TreeRuleReturnScope
	{
		public StateCluster g=null;
	}

	// $ANTLR start "tree_"
	// Grammars\\TreeToNFAConverter.g3:362:0: tree_ returns [StateCluster g=null] : ^( TREE_BEGIN e= element (e= element )* ) ;
	private TreeToNFAConverter.tree__return tree_(  )
	{
		TreeToNFAConverter.tree__return retval = new TreeToNFAConverter.tree__return();
		retval.start = input.LT(1);

		TreeToNFAConverter.element_return e = default(TreeToNFAConverter.element_return);


			StateCluster down=null, up=null;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:367:4: ( ^( TREE_BEGIN e= element (e= element )* ) )
			// Grammars\\TreeToNFAConverter.g3:367:4: ^( TREE_BEGIN e= element (e= element )* )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_1019); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			PushFollow(Follow._element_in_tree_1026);
			e=element();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{
				 retval.g = (e!=null?e.g:default(StateCluster)); 
			}
			if ( state.backtracking == 0 )
			{

								down = factory.BuildAtom(Label.DOWN, (e!=null?((GrammarAST)e.Start):null));
								// TODO set following states for imaginary nodes?
								//el.followingNFAState = down.right;
								retval.g = factory.BuildAB(retval.g,down);
							
			}
			// Grammars\\TreeToNFAConverter.g3:375:4: (e= element )*
			for ( ; ; )
			{
				int alt43=2;
				int LA43_0 = input.LA(1);

				if ( (LA43_0==ACTION||(LA43_0>=ASSIGN && LA43_0<=BLOCK)||(LA43_0>=CHAR_LITERAL && LA43_0<=CHAR_RANGE)||LA43_0==CLOSURE||LA43_0==DOT||LA43_0==EPSILON||LA43_0==FORCED_ACTION||LA43_0==GATED_SEMPRED||LA43_0==NOT||LA43_0==OPTIONAL||(LA43_0>=PLUS_ASSIGN && LA43_0<=POSITIVE_CLOSURE)||LA43_0==RANGE||LA43_0==ROOT||LA43_0==RULE_REF||LA43_0==SEMPRED||(LA43_0>=STRING_LITERAL && LA43_0<=SYNPRED)||LA43_0==TOKEN_REF||LA43_0==TREE_BEGIN||LA43_0==WILDCARD) )
				{
					alt43=1;
				}


				switch ( alt43 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:375:6: e= element
					{
					PushFollow(Follow._element_in_tree_1042);
					e=element();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{
						retval.g = factory.BuildAB(retval.g,(e!=null?e.g:default(StateCluster)));
					}

					}
					break;

				default:
					goto loop43;
				}
			}

			loop43:
				;


			if ( state.backtracking == 0 )
			{

								up = factory.BuildAtom(Label.UP, (e!=null?((GrammarAST)e.Start):null));
								//el.followingNFAState = up.right;
								retval.g = factory.BuildAB(retval.g,up);
								// tree roots point at right edge of DOWN for LOOK computation later
								((GrammarAST)retval.Start).NFATreeDownState = down.left;
							
			}

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "tree_"


	// $ANTLR start "atom_or_notatom"
	// Grammars\\TreeToNFAConverter.g3:386:0: atom_or_notatom returns [StateCluster g=null] : ( atom[null] | ^(n= NOT (c= CHAR_LITERAL (ast1= ast_suffix )? |t= TOKEN_REF (ast3= ast_suffix )? | set ) ) );
	private StateCluster atom_or_notatom(  )
	{
		StateCluster g = null;

		GrammarAST n=null;
		GrammarAST c=null;
		GrammarAST t=null;
		TreeToNFAConverter.set_return set9 = default(TreeToNFAConverter.set_return);
		TreeToNFAConverter.atom_return atom8 = default(TreeToNFAConverter.atom_return);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:387:4: ( atom[null] | ^(n= NOT (c= CHAR_LITERAL (ast1= ast_suffix )? |t= TOKEN_REF (ast3= ast_suffix )? | set ) ) )
			int alt47=2;
			int LA47_0 = input.LA(1);

			if ( (LA47_0==CHAR_LITERAL||LA47_0==DOT||LA47_0==RULE_REF||LA47_0==STRING_LITERAL||LA47_0==TOKEN_REF||LA47_0==WILDCARD) )
			{
				alt47=1;
			}
			else if ( (LA47_0==NOT) )
			{
				alt47=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return g;}
				NoViableAltException nvae = new NoViableAltException("", 47, 0, input);

				throw nvae;
			}
			switch ( alt47 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:387:4: atom[null]
				{
				PushFollow(Follow._atom_in_atom_or_notatom1071);
				atom8=atom(null);

				state._fsp--;
				if (state.failed) return g;
				if ( state.backtracking == 0 )
				{
					g = (atom8!=null?atom8.g:default(StateCluster));
				}

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:388:4: ^(n= NOT (c= CHAR_LITERAL (ast1= ast_suffix )? |t= TOKEN_REF (ast3= ast_suffix )? | set ) )
				{
				n=(GrammarAST)Match(input,NOT,Follow._NOT_in_atom_or_notatom1083); if (state.failed) return g;

				Match(input, TokenTypes.Down, null); if (state.failed) return g;
				// Grammars\\TreeToNFAConverter.g3:389:4: (c= CHAR_LITERAL (ast1= ast_suffix )? |t= TOKEN_REF (ast3= ast_suffix )? | set )
				int alt46=3;
				switch ( input.LA(1) )
				{
				case CHAR_LITERAL:
					{
					alt46=1;
					}
					break;
				case TOKEN_REF:
					{
					alt46=2;
					}
					break;
				case BLOCK:
					{
					alt46=3;
					}
					break;
				default:
					{
						if (state.backtracking>0) {state.failed=true; return g;}
						NoViableAltException nvae = new NoViableAltException("", 46, 0, input);

						throw nvae;
					}
				}

				switch ( alt46 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:389:6: c= CHAR_LITERAL (ast1= ast_suffix )?
					{
					c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom_or_notatom1092); if (state.failed) return g;
					// Grammars\\TreeToNFAConverter.g3:389:21: (ast1= ast_suffix )?
					int alt44=2;
					int LA44_0 = input.LA(1);

					if ( (LA44_0==BANG||LA44_0==ROOT) )
					{
						alt44=1;
					}
					switch ( alt44 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:389:22: ast1= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom_or_notatom1097);
						ast_suffix();

						state._fsp--;
						if (state.failed) return g;

						}
						break;

					}

					if ( state.backtracking == 0 )
					{

											int ttype=0;
											if ( grammar.type==GrammarType.Lexer )
											{
												ttype = Grammar.GetCharValueFromGrammarCharLiteral((c!=null?c.Text:null));
											}
											else
											{
												ttype = grammar.GetTokenType((c!=null?c.Text:null));
											}
											IIntSet notAtom = grammar.Complement(ttype);
											if ( notAtom.IsNil )
											{
												ErrorManager.GrammarError(
													ErrorManager.MSG_EMPTY_COMPLEMENT,
													grammar,
													c.Token,
													(c!=null?c.Text:null));
											}
											g =factory.BuildSet(notAtom,n);
										
					}

					}
					break;
				case 2:
					// Grammars\\TreeToNFAConverter.g3:411:6: t= TOKEN_REF (ast3= ast_suffix )?
					{
					t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom_or_notatom1114); if (state.failed) return g;
					// Grammars\\TreeToNFAConverter.g3:411:18: (ast3= ast_suffix )?
					int alt45=2;
					int LA45_0 = input.LA(1);

					if ( (LA45_0==BANG||LA45_0==ROOT) )
					{
						alt45=1;
					}
					switch ( alt45 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:411:19: ast3= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom_or_notatom1119);
						ast_suffix();

						state._fsp--;
						if (state.failed) return g;

						}
						break;

					}

					if ( state.backtracking == 0 )
					{

											int ttype=0;
											IIntSet notAtom = null;
											if ( grammar.type==GrammarType.Lexer )
											{
												notAtom = grammar.GetSetFromRule(this,(t!=null?t.Text:null));
												if ( notAtom==null )
												{
													ErrorManager.GrammarError(
														ErrorManager.MSG_RULE_INVALID_SET,
														grammar,
														t.Token,
														(t!=null?t.Text:null));
												}
												else
												{
													notAtom = grammar.Complement(notAtom);
												}
											}
											else
											{
												ttype = grammar.GetTokenType((t!=null?t.Text:null));
												notAtom = grammar.Complement(ttype);
											}
											if ( notAtom==null || notAtom.IsNil )
											{
												ErrorManager.GrammarError(
													ErrorManager.MSG_EMPTY_COMPLEMENT,
													grammar,
													t.Token,
													(t!=null?t.Text:null));
											}
											g =factory.BuildSet(notAtom,n);
										
					}

					}
					break;
				case 3:
					// Grammars\\TreeToNFAConverter.g3:446:6: set
					{
					PushFollow(Follow._set_in_atom_or_notatom1134);
					set9=set();

					state._fsp--;
					if (state.failed) return g;
					if ( state.backtracking == 0 )
					{
						g = (set9!=null?set9.g:default(StateCluster));
					}
					if ( state.backtracking == 0 )
					{

											GrammarAST stNode = (GrammarAST)n.GetChild(0);
											//IIntSet notSet = grammar.Complement(stNode.SetValue);
											// let code generator complement the sets
											IIntSet s = stNode.SetValue;
											stNode.SetValue = s;
											// let code gen do the complement again; here we compute
											// for NFA construction
											s = grammar.Complement(s);
											if ( s.IsNil )
											{
												ErrorManager.GrammarError(
													ErrorManager.MSG_EMPTY_COMPLEMENT,
													grammar,
													n.Token);
											}
											g =factory.BuildSet(s,n);
										
					}

					}
					break;

				}

				if ( state.backtracking == 0 )
				{
					n.followingNFAState = g.right;
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return g;

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
		return g;
	}
	// $ANTLR end "atom_or_notatom"

	public class atom_return : TreeRuleReturnScope
	{
		public StateCluster g=null;
	}

	// $ANTLR start "atom"
	// Grammars\\TreeToNFAConverter.g3:470:0: atom[string scopeName] returns [StateCluster g=null] : ( ^(r= RULE_REF (rarg= ARG_ACTION )? (as1= ast_suffix )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? (as2= ast_suffix )? ) | ^(c= CHAR_LITERAL (as3= ast_suffix )? ) | ^(s= STRING_LITERAL (as4= ast_suffix )? ) | ^(w= WILDCARD (as5= ast_suffix )? ) | ^( DOT scope_= ID a= atom[$scope_.text] ) );
	private TreeToNFAConverter.atom_return atom( string scopeName )
	{
		TreeToNFAConverter.atom_return retval = new TreeToNFAConverter.atom_return();
		retval.start = input.LT(1);

		GrammarAST r=null;
		GrammarAST rarg=null;
		GrammarAST t=null;
		GrammarAST targ=null;
		GrammarAST c=null;
		GrammarAST s=null;
		GrammarAST w=null;
		GrammarAST scope_=null;
		TreeToNFAConverter.atom_return a = default(TreeToNFAConverter.atom_return);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:471:4: ( ^(r= RULE_REF (rarg= ARG_ACTION )? (as1= ast_suffix )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? (as2= ast_suffix )? ) | ^(c= CHAR_LITERAL (as3= ast_suffix )? ) | ^(s= STRING_LITERAL (as4= ast_suffix )? ) | ^(w= WILDCARD (as5= ast_suffix )? ) | ^( DOT scope_= ID a= atom[$scope_.text] ) )
			int alt55=6;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt55=1;
				}
				break;
			case TOKEN_REF:
				{
				alt55=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt55=3;
				}
				break;
			case STRING_LITERAL:
				{
				alt55=4;
				}
				break;
			case WILDCARD:
				{
				alt55=5;
				}
				break;
			case DOT:
				{
				alt55=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 55, 0, input);

					throw nvae;
				}
			}

			switch ( alt55 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:471:4: ^(r= RULE_REF (rarg= ARG_ACTION )? (as1= ast_suffix )? )
				{
				r=(GrammarAST)Match(input,RULE_REF,Follow._RULE_REF_in_atom1176); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:471:18: (rarg= ARG_ACTION )?
					int alt48=2;
					int LA48_0 = input.LA(1);

					if ( (LA48_0==ARG_ACTION) )
					{
						alt48=1;
					}
					switch ( alt48 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:471:19: rarg= ARG_ACTION
						{
						rarg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1181); if (state.failed) return retval;

						}
						break;

					}

					// Grammars\\TreeToNFAConverter.g3:471:37: (as1= ast_suffix )?
					int alt49=2;
					int LA49_0 = input.LA(1);

					if ( (LA49_0==BANG||LA49_0==ROOT) )
					{
						alt49=1;
					}
					switch ( alt49 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:471:38: as1= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom1188);
						ast_suffix();

						state._fsp--;
						if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								NFAState start = grammar.GetRuleStartState(scopeName,(r!=null?r.Text:null));
								if ( start!=null )
								{
									Rule rr = grammar.GetRule(scopeName,(r!=null?r.Text:null));
									retval.g = factory.BuildRuleRef(rr, start);
									r.followingNFAState = retval.g.right;
									r._nfaStartState = retval.g.left;
									if ( retval.g.left.GetTransition(0) is RuleClosureTransition
										&& grammar.type!=GrammarType.Lexer )
									{
										AddFollowTransition((r!=null?r.Text:null), retval.g.right);
									}
									// else rule ref got inlined to a set
								}
							
				}

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:489:4: ^(t= TOKEN_REF (targ= ARG_ACTION )? (as2= ast_suffix )? )
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom1206); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:489:20: (targ= ARG_ACTION )?
					int alt50=2;
					int LA50_0 = input.LA(1);

					if ( (LA50_0==ARG_ACTION) )
					{
						alt50=1;
					}
					switch ( alt50 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:489:21: targ= ARG_ACTION
						{
						targ=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1212); if (state.failed) return retval;

						}
						break;

					}

					// Grammars\\TreeToNFAConverter.g3:489:39: (as2= ast_suffix )?
					int alt51=2;
					int LA51_0 = input.LA(1);

					if ( (LA51_0==BANG||LA51_0==ROOT) )
					{
						alt51=1;
					}
					switch ( alt51 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:489:40: as2= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom1219);
						ast_suffix();

						state._fsp--;
						if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									NFAState start = grammar.GetRuleStartState(scopeName,(t!=null?t.Text:null));
									if ( start!=null )
									{
										Rule rr = grammar.GetRule(scopeName,t.Text);
										retval.g = factory.BuildRuleRef(rr, start);
										t._nfaStartState = retval.g.left;
										// don't add FOLLOW transitions in the lexer;
										// only exact context should be used.
									}
								}
								else
								{
									retval.g = factory.BuildAtom(t);
									t.followingNFAState = retval.g.right;
								}
							
				}

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:510:4: ^(c= CHAR_LITERAL (as3= ast_suffix )? )
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom1237); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:510:23: (as3= ast_suffix )?
					int alt52=2;
					int LA52_0 = input.LA(1);

					if ( (LA52_0==BANG||LA52_0==ROOT) )
					{
						alt52=1;
					}
					switch ( alt52 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:510:24: as3= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom1243);
						ast_suffix();

						state._fsp--;
						if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									retval.g = factory.BuildCharLiteralAtom(c);
								}
								else
								{
									retval.g = factory.BuildAtom(c);
									c.followingNFAState = retval.g.right;
								}
							
				}

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:523:4: ^(s= STRING_LITERAL (as4= ast_suffix )? )
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_atom1261); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:523:25: (as4= ast_suffix )?
					int alt53=2;
					int LA53_0 = input.LA(1);

					if ( (LA53_0==BANG||LA53_0==ROOT) )
					{
						alt53=1;
					}
					switch ( alt53 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:523:26: as4= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom1267);
						ast_suffix();

						state._fsp--;
						if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									retval.g = factory.BuildStringLiteralAtom(s);
								}
								else
								{
									retval.g = factory.BuildAtom(s);
									s.followingNFAState = retval.g.right;
								}
							
				}

				}
				break;
			case 5:
				// Grammars\\TreeToNFAConverter.g3:536:4: ^(w= WILDCARD (as5= ast_suffix )? )
				{
				w=(GrammarAST)Match(input,WILDCARD,Follow._WILDCARD_in_atom1285); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:536:18: (as5= ast_suffix )?
					int alt54=2;
					int LA54_0 = input.LA(1);

					if ( (LA54_0==BANG||LA54_0==ROOT) )
					{
						alt54=1;
					}
					switch ( alt54 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:536:19: as5= ast_suffix
						{
						PushFollow(Follow._ast_suffix_in_atom1290);
						ast_suffix();

						state._fsp--;
						if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

									if ( nfa.grammar.type == GrammarType.TreeParser
										&& (w.ChildIndex > 0 || w.Parent.GetChild(1).Type == EOA) )
									{
										retval.g = factory.BuildWildcardTree( w );
									}
									else
									{
										retval.g = factory.BuildWildcard( w );
									}
								
				}

				}
				break;
			case 6:
				// Grammars\\TreeToNFAConverter.g3:549:4: ^( DOT scope_= ID a= atom[$scope_.text] )
				{
				Match(input,DOT,Follow._DOT_in_atom1307); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				scope_=(GrammarAST)Match(input,ID,Follow._ID_in_atom1311); if (state.failed) return retval;
				PushFollow(Follow._atom_in_atom1315);
				a=atom((scope_!=null?scope_.Text:null));

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					retval.g = (a!=null?a.g:default(StateCluster));
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "atom"


	// $ANTLR start "ast_suffix"
	// Grammars\\TreeToNFAConverter.g3:552:0: ast_suffix : ( ROOT | BANG );
	private void ast_suffix(  )
	{
		try
		{
			// Grammars\\TreeToNFAConverter.g3:553:4: ( ROOT | BANG )
			// Grammars\\TreeToNFAConverter.g3:
			{
			if ( input.LA(1)==BANG||input.LA(1)==ROOT )
			{
				input.Consume();
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
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

	public class set_return : TreeRuleReturnScope
	{
		public StateCluster g=null;
	}

	// $ANTLR start "set"
	// Grammars\\TreeToNFAConverter.g3:557:0: set returns [StateCluster g=null] : ^(b= BLOCK ( ^( ALT ( ^( BACKTRACK_SEMPRED ( . )* ) )? setElement[elements] EOA ) )+ EOB ) ;
	private TreeToNFAConverter.set_return set(  )
	{
		TreeToNFAConverter.set_return retval = new TreeToNFAConverter.set_return();
		retval.start = input.LT(1);

		GrammarAST b=null;


			IIntSet elements=new IntervalSet();
			if ( state.backtracking == 0 )
				((GrammarAST)retval.Start).SetValue = elements; // track set for use by code gen

		try
		{
			// Grammars\\TreeToNFAConverter.g3:564:4: ( ^(b= BLOCK ( ^( ALT ( ^( BACKTRACK_SEMPRED ( . )* ) )? setElement[elements] EOA ) )+ EOB ) )
			// Grammars\\TreeToNFAConverter.g3:564:4: ^(b= BLOCK ( ^( ALT ( ^( BACKTRACK_SEMPRED ( . )* ) )? setElement[elements] EOA ) )+ EOB )
			{
			b=(GrammarAST)Match(input,BLOCK,Follow._BLOCK_in_set1361); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			// Grammars\\TreeToNFAConverter.g3:565:6: ( ^( ALT ( ^( BACKTRACK_SEMPRED ( . )* ) )? setElement[elements] EOA ) )+
			int cnt58=0;
			for ( ; ; )
			{
				int alt58=2;
				int LA58_0 = input.LA(1);

				if ( (LA58_0==ALT) )
				{
					alt58=1;
				}


				switch ( alt58 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:565:7: ^( ALT ( ^( BACKTRACK_SEMPRED ( . )* ) )? setElement[elements] EOA )
					{
					Match(input,ALT,Follow._ALT_in_set1370); if (state.failed) return retval;

					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\TreeToNFAConverter.g3:565:13: ( ^( BACKTRACK_SEMPRED ( . )* ) )?
					int alt57=2;
					int LA57_0 = input.LA(1);

					if ( (LA57_0==BACKTRACK_SEMPRED) )
					{
						alt57=1;
					}
					switch ( alt57 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:565:15: ^( BACKTRACK_SEMPRED ( . )* )
						{
						Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_set1375); if (state.failed) return retval;

						if ( input.LA(1)==TokenTypes.Down )
						{
							Match(input, TokenTypes.Down, null); if (state.failed) return retval;
							// Grammars\\TreeToNFAConverter.g3:565:35: ( . )*
							for ( ; ; )
							{
								int alt56=2;
								int LA56_0 = input.LA(1);

								if ( ((LA56_0>=ACTION && LA56_0<=XDIGIT)) )
								{
									alt56=1;
								}
								else if ( (LA56_0==UP) )
								{
									alt56=2;
								}


								switch ( alt56 )
								{
								case 1:
									// Grammars\\TreeToNFAConverter.g3:565:0: .
									{
									MatchAny(input); if (state.failed) return retval;

									}
									break;

								default:
									goto loop56;
								}
							}

							loop56:
								;



							Match(input, TokenTypes.Up, null); if (state.failed) return retval;
						}

						}
						break;

					}

					PushFollow(Follow._setElement_in_set1384);
					setElement(elements);

					state._fsp--;
					if (state.failed) return retval;
					Match(input,EOA,Follow._EOA_in_set1387); if (state.failed) return retval;

					Match(input, TokenTypes.Up, null); if (state.failed) return retval;

					}
					break;

				default:
					if ( cnt58 >= 1 )
						goto loop58;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee58 = new EarlyExitException( 58, input );
					throw eee58;
				}
				cnt58++;
			}
			loop58:
				;


			Match(input,EOB,Follow._EOB_in_set1397); if (state.failed) return retval;

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

						retval.g = factory.BuildSet(elements,b);
						b.followingNFAState = retval.g.right;
						b.SetValue = elements; // track set value of this block
						
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
	// $ANTLR end "set"


	// $ANTLR start "setRule"
	// Grammars\\TreeToNFAConverter.g3:576:0: setRule returns [IIntSet elements=new IntervalSet()] : ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( OPTIONS ( . )* ) )? ( ^( ALT ( BACKTRACK_SEMPRED )? setElement[elements] EOA ) )+ EOB ) ( exceptionGroup )? EOR ) ;
	private IIntSet setRule(  )
	{
		IIntSet elements = new IntervalSet();

		GrammarAST id=null;


			IIntSet s=null;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:581:4: ( ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( OPTIONS ( . )* ) )? ( ^( ALT ( BACKTRACK_SEMPRED )? setElement[elements] EOA ) )+ EOB ) ( exceptionGroup )? EOR ) )
			// Grammars\\TreeToNFAConverter.g3:581:4: ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( OPTIONS ( . )* ) )? ( ^( ALT ( BACKTRACK_SEMPRED )? setElement[elements] EOA ) )+ EOB ) ( exceptionGroup )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_setRule1431); if (state.failed) return elements;

			Match(input, TokenTypes.Down, null); if (state.failed) return elements;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_setRule1435); if (state.failed) return elements;
			// Grammars\\TreeToNFAConverter.g3:581:18: ( modifier )?
			int alt59=2;
			int LA59_0 = input.LA(1);

			if ( (LA59_0==FRAGMENT||(LA59_0>=PRIVATE && LA59_0<=PUBLIC)) )
			{
				alt59=1;
			}
			switch ( alt59 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:581:19: modifier
				{
				PushFollow(Follow._modifier_in_setRule1438);
				modifier();

				state._fsp--;
				if (state.failed) return elements;

				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_setRule1442); if (state.failed) return elements;
			Match(input,RET,Follow._RET_in_setRule1444); if (state.failed) return elements;
			// Grammars\\TreeToNFAConverter.g3:581:38: ( ^( OPTIONS ( . )* ) )?
			int alt61=2;
			int LA61_0 = input.LA(1);

			if ( (LA61_0==OPTIONS) )
			{
				alt61=1;
			}
			switch ( alt61 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:581:40: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_setRule1449); if (state.failed) return elements;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return elements;
					// Grammars\\TreeToNFAConverter.g3:581:50: ( . )*
					for ( ; ; )
					{
						int alt60=2;
						int LA60_0 = input.LA(1);

						if ( ((LA60_0>=ACTION && LA60_0<=XDIGIT)) )
						{
							alt60=1;
						}
						else if ( (LA60_0==UP) )
						{
							alt60=2;
						}


						switch ( alt60 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:581:0: .
							{
							MatchAny(input); if (state.failed) return elements;

							}
							break;

						default:
							goto loop60;
						}
					}

					loop60:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return elements;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:581:57: ( ruleScopeSpec )?
			int alt62=2;
			int LA62_0 = input.LA(1);

			if ( (LA62_0==SCOPE) )
			{
				alt62=1;
			}
			switch ( alt62 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:581:59: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_setRule1460);
				ruleScopeSpec();

				state._fsp--;
				if (state.failed) return elements;

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:582:4: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt64=2;
				int LA64_0 = input.LA(1);

				if ( (LA64_0==AMPERSAND) )
				{
					alt64=1;
				}


				switch ( alt64 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:582:6: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_setRule1471); if (state.failed) return elements;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return elements;
						// Grammars\\TreeToNFAConverter.g3:582:18: ( . )*
						for ( ; ; )
						{
							int alt63=2;
							int LA63_0 = input.LA(1);

							if ( ((LA63_0>=ACTION && LA63_0<=XDIGIT)) )
							{
								alt63=1;
							}
							else if ( (LA63_0==UP) )
							{
								alt63=2;
							}


							switch ( alt63 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:582:0: .
								{
								MatchAny(input); if (state.failed) return elements;

								}
								break;

							default:
								goto loop63;
							}
						}

						loop63:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return elements;
					}

					}
					break;

				default:
					goto loop64;
				}
			}

			loop64:
				;


			Match(input,BLOCK,Follow._BLOCK_in_setRule1485); if (state.failed) return elements;

			Match(input, TokenTypes.Down, null); if (state.failed) return elements;
			// Grammars\\TreeToNFAConverter.g3:583:13: ( ^( OPTIONS ( . )* ) )?
			int alt66=2;
			int LA66_0 = input.LA(1);

			if ( (LA66_0==OPTIONS) )
			{
				alt66=1;
			}
			switch ( alt66 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:583:15: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_setRule1490); if (state.failed) return elements;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return elements;
					// Grammars\\TreeToNFAConverter.g3:583:25: ( . )*
					for ( ; ; )
					{
						int alt65=2;
						int LA65_0 = input.LA(1);

						if ( ((LA65_0>=ACTION && LA65_0<=XDIGIT)) )
						{
							alt65=1;
						}
						else if ( (LA65_0==UP) )
						{
							alt65=2;
						}


						switch ( alt65 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:583:0: .
							{
							MatchAny(input); if (state.failed) return elements;

							}
							break;

						default:
							goto loop65;
						}
					}

					loop65:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return elements;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:584:7: ( ^( ALT ( BACKTRACK_SEMPRED )? setElement[elements] EOA ) )+
			int cnt68=0;
			for ( ; ; )
			{
				int alt68=2;
				int LA68_0 = input.LA(1);

				if ( (LA68_0==ALT) )
				{
					alt68=1;
				}


				switch ( alt68 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:584:9: ^( ALT ( BACKTRACK_SEMPRED )? setElement[elements] EOA )
					{
					Match(input,ALT,Follow._ALT_in_setRule1508); if (state.failed) return elements;

					Match(input, TokenTypes.Down, null); if (state.failed) return elements;
					// Grammars\\TreeToNFAConverter.g3:584:15: ( BACKTRACK_SEMPRED )?
					int alt67=2;
					int LA67_0 = input.LA(1);

					if ( (LA67_0==BACKTRACK_SEMPRED) )
					{
						alt67=1;
					}
					switch ( alt67 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:584:16: BACKTRACK_SEMPRED
						{
						Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_setRule1511); if (state.failed) return elements;

						}
						break;

					}

					PushFollow(Follow._setElement_in_setRule1515);
					setElement(elements);

					state._fsp--;
					if (state.failed) return elements;
					Match(input,EOA,Follow._EOA_in_setRule1518); if (state.failed) return elements;

					Match(input, TokenTypes.Up, null); if (state.failed) return elements;

					}
					break;

				default:
					if ( cnt68 >= 1 )
						goto loop68;

					if (state.backtracking>0) {state.failed=true; return elements;}
					EarlyExitException eee68 = new EarlyExitException( 68, input );
					throw eee68;
				}
				cnt68++;
			}
			loop68:
				;


			Match(input,EOB,Follow._EOB_in_setRule1530); if (state.failed) return elements;

			Match(input, TokenTypes.Up, null); if (state.failed) return elements;
			// Grammars\\TreeToNFAConverter.g3:587:4: ( exceptionGroup )?
			int alt69=2;
			int LA69_0 = input.LA(1);

			if ( (LA69_0==CATCH||LA69_0==FINALLY) )
			{
				alt69=1;
			}
			switch ( alt69 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:587:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_setRule1542);
				exceptionGroup();

				state._fsp--;
				if (state.failed) return elements;

				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_setRule1549); if (state.failed) return elements;

			Match(input, TokenTypes.Up, null); if (state.failed) return elements;

			}

		}
		catch ( RecognitionException re )
		{
			 throw re; 
		}
		finally
		{
		}
		return elements;
	}
	// $ANTLR end "setRule"


	// $ANTLR start "setElement"
	// Grammars\\TreeToNFAConverter.g3:593:0: setElement[IIntSet elements] : (c= CHAR_LITERAL |t= TOKEN_REF |s= STRING_LITERAL | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) |gset= set | ^( NOT setElement[ns] ) );
	private void setElement( IIntSet elements )
	{
		GrammarAST c=null;
		GrammarAST t=null;
		GrammarAST s=null;
		GrammarAST c1=null;
		GrammarAST c2=null;
		TreeToNFAConverter.set_return gset = default(TreeToNFAConverter.set_return);


			int ttype;
			IIntSet ns=null;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:599:4: (c= CHAR_LITERAL |t= TOKEN_REF |s= STRING_LITERAL | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) |gset= set | ^( NOT setElement[ns] ) )
			int alt70=6;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt70=1;
				}
				break;
			case TOKEN_REF:
				{
				alt70=2;
				}
				break;
			case STRING_LITERAL:
				{
				alt70=3;
				}
				break;
			case CHAR_RANGE:
				{
				alt70=4;
				}
				break;
			case BLOCK:
				{
				alt70=5;
				}
				break;
			case NOT:
				{
				alt70=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 70, 0, input);

					throw nvae;
				}
			}

			switch ( alt70 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:599:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1578); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									ttype = Grammar.GetCharValueFromGrammarCharLiteral((c!=null?c.Text:null));
								}
								else
								{
									ttype = grammar.GetTokenType((c!=null?c.Text:null));
								}
								if ( elements.Contains(ttype) )
								{
									ErrorManager.GrammarError(
										ErrorManager.MSG_DUPLICATE_SET_ENTRY,
										grammar,
										c.Token,
										(c!=null?c.Text:null));
								}
								elements.Add(ttype);
							
				}

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:619:4: t= TOKEN_REF
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_setElement1589); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									// recursively will invoke this rule to match elements in target rule ref
									IIntSet ruleSet = grammar.GetSetFromRule(this,(t!=null?t.Text:null));
									if ( ruleSet==null )
									{
										ErrorManager.GrammarError(
											ErrorManager.MSG_RULE_INVALID_SET,
											grammar,
											t.Token,
											(t!=null?t.Text:null));
									}
									else
									{
										elements.AddAll(ruleSet);
									}
								}
								else
								{
									ttype = grammar.GetTokenType((t!=null?t.Text:null));
									if ( elements.Contains(ttype) )
									{
										ErrorManager.GrammarError(
											ErrorManager.MSG_DUPLICATE_SET_ENTRY,
											grammar,
											t.Token,
											(t!=null?t.Text:null));
									}
									elements.Add(ttype);
								}
							
				}

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:653:4: s= STRING_LITERAL
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_setElement1601); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								ttype = grammar.GetTokenType((s!=null?s.Text:null));
								if ( elements.Contains(ttype) )
								{
									ErrorManager.GrammarError(
										ErrorManager.MSG_DUPLICATE_SET_ENTRY,
										grammar,
										s.Token,
										(s!=null?s.Text:null));
								}
								elements.Add(ttype);
							
				}

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:666:4: ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_setElement1611); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				c1=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1615); if (state.failed) return ;
				c2=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1619); if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==GrammarType.Lexer )
								{
									int a = Grammar.GetCharValueFromGrammarCharLiteral((c1!=null?c1.Text:null));
									int b = Grammar.GetCharValueFromGrammarCharLiteral((c2!=null?c2.Text:null));
									elements.AddAll(IntervalSet.Of(a,b));
								}
							
				}

				}
				break;
			case 5:
				// Grammars\\TreeToNFAConverter.g3:676:4: gset= set
				{
				PushFollow(Follow._set_in_setElement1632);
				gset=set();

				state._fsp--;
				if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								Transition setTrans = (gset!=null?gset.g:default(StateCluster)).left.GetTransition(0);
								elements.AddAll(setTrans.label.Set);
							
				}

				}
				break;
			case 6:
				// Grammars\\TreeToNFAConverter.g3:682:4: ^( NOT setElement[ns] )
				{
				Match(input,NOT,Follow._NOT_in_setElement1644); if (state.failed) return ;

				if ( state.backtracking == 0 )
				{
					ns=new IntervalSet();
				}

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._setElement_in_setElement1651);
				setElement(ns);

				state._fsp--;
				if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

									IIntSet not = grammar.Complement(ns);
									elements.AddAll(not);
								
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "setElement"


	// $ANTLR start "testBlockAsSet"
	// Grammars\\TreeToNFAConverter.g3:698:0: testBlockAsSet returns [int alts=0] options {backtrack=true; } : ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB ) ;
	private int testBlockAsSet(  )
	{
		int alts = 0;

		int testSetElement10 = default(int);


			inTest++;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:704:4: ( ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB ) )
			// Grammars\\TreeToNFAConverter.g3:704:4: ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_testBlockAsSet1696); if (state.failed) return alts;

			Match(input, TokenTypes.Down, null); if (state.failed) return alts;
			// Grammars\\TreeToNFAConverter.g3:705:4: ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+
			int cnt72=0;
			for ( ; ; )
			{
				int alt72=2;
				int LA72_0 = input.LA(1);

				if ( (LA72_0==ALT) )
				{
					alt72=1;
				}


				switch ( alt72 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:705:6: ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA )
					{
					Match(input,ALT,Follow._ALT_in_testBlockAsSet1704); if (state.failed) return alts;

					Match(input, TokenTypes.Down, null); if (state.failed) return alts;
					// Grammars\\TreeToNFAConverter.g3:705:12: ( BACKTRACK_SEMPRED )?
					int alt71=2;
					int LA71_0 = input.LA(1);

					if ( (LA71_0==BACKTRACK_SEMPRED) )
					{
						alt71=1;
					}
					switch ( alt71 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:705:13: BACKTRACK_SEMPRED
						{
						Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_testBlockAsSet1707); if (state.failed) return alts;

						}
						break;

					}

					PushFollow(Follow._testSetElement_in_testBlockAsSet1711);
					testSetElement10=testSetElement();

					state._fsp--;
					if (state.failed) return alts;
					alts += testSetElement10;
					Match(input,EOA,Follow._EOA_in_testBlockAsSet1715); if (state.failed) return alts;

					Match(input, TokenTypes.Up, null); if (state.failed) return alts;

					}
					break;

				default:
					if ( cnt72 >= 1 )
						goto loop72;

					if (state.backtracking>0) {state.failed=true; return alts;}
					EarlyExitException eee72 = new EarlyExitException( 72, input );
					throw eee72;
				}
				cnt72++;
			}
			loop72:
				;


			Match(input,EOB,Follow._EOB_in_testBlockAsSet1727); if (state.failed) return alts;

			Match(input, TokenTypes.Up, null); if (state.failed) return alts;

			}

		}
		catch ( RecognitionException re )
		{
			 throw re; 
		}
		finally
		{
			 inTest--; 
		}
		return alts;
	}
	// $ANTLR end "testBlockAsSet"


	// $ANTLR start "testSetRule"
	// Grammars\\TreeToNFAConverter.g3:713:0: testSetRule returns [int alts=0] : ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB ) ( exceptionGroup )? EOR ) ;
	private int testSetRule(  )
	{
		int alts = 0;

		GrammarAST id=null;
		int testSetElement11 = default(int);


			inTest++;

		try
		{
			// Grammars\\TreeToNFAConverter.g3:718:4: ( ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB ) ( exceptionGroup )? EOR ) )
			// Grammars\\TreeToNFAConverter.g3:718:4: ^( RULE id= ID ( modifier )? ARG RET ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* ^( BLOCK ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+ EOB ) ( exceptionGroup )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_testSetRule1762); if (state.failed) return alts;

			Match(input, TokenTypes.Down, null); if (state.failed) return alts;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_testSetRule1766); if (state.failed) return alts;
			// Grammars\\TreeToNFAConverter.g3:718:18: ( modifier )?
			int alt73=2;
			int LA73_0 = input.LA(1);

			if ( (LA73_0==FRAGMENT||(LA73_0>=PRIVATE && LA73_0<=PUBLIC)) )
			{
				alt73=1;
			}
			switch ( alt73 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:718:19: modifier
				{
				PushFollow(Follow._modifier_in_testSetRule1769);
				modifier();

				state._fsp--;
				if (state.failed) return alts;

				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_testSetRule1773); if (state.failed) return alts;
			Match(input,RET,Follow._RET_in_testSetRule1775); if (state.failed) return alts;
			// Grammars\\TreeToNFAConverter.g3:718:38: ( ^( OPTIONS ( . )* ) )?
			int alt75=2;
			int LA75_0 = input.LA(1);

			if ( (LA75_0==OPTIONS) )
			{
				alt75=1;
			}
			switch ( alt75 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:718:40: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_testSetRule1780); if (state.failed) return alts;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return alts;
					// Grammars\\TreeToNFAConverter.g3:718:50: ( . )*
					for ( ; ; )
					{
						int alt74=2;
						int LA74_0 = input.LA(1);

						if ( ((LA74_0>=ACTION && LA74_0<=XDIGIT)) )
						{
							alt74=1;
						}
						else if ( (LA74_0==UP) )
						{
							alt74=2;
						}


						switch ( alt74 )
						{
						case 1:
							// Grammars\\TreeToNFAConverter.g3:718:0: .
							{
							MatchAny(input); if (state.failed) return alts;

							}
							break;

						default:
							goto loop74;
						}
					}

					loop74:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return alts;
				}

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:718:57: ( ruleScopeSpec )?
			int alt76=2;
			int LA76_0 = input.LA(1);

			if ( (LA76_0==SCOPE) )
			{
				alt76=1;
			}
			switch ( alt76 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:718:59: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_testSetRule1791);
				ruleScopeSpec();

				state._fsp--;
				if (state.failed) return alts;

				}
				break;

			}

			// Grammars\\TreeToNFAConverter.g3:719:4: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt78=2;
				int LA78_0 = input.LA(1);

				if ( (LA78_0==AMPERSAND) )
				{
					alt78=1;
				}


				switch ( alt78 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:719:6: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_testSetRule1802); if (state.failed) return alts;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return alts;
						// Grammars\\TreeToNFAConverter.g3:719:18: ( . )*
						for ( ; ; )
						{
							int alt77=2;
							int LA77_0 = input.LA(1);

							if ( ((LA77_0>=ACTION && LA77_0<=XDIGIT)) )
							{
								alt77=1;
							}
							else if ( (LA77_0==UP) )
							{
								alt77=2;
							}


							switch ( alt77 )
							{
							case 1:
								// Grammars\\TreeToNFAConverter.g3:719:0: .
								{
								MatchAny(input); if (state.failed) return alts;

								}
								break;

							default:
								goto loop77;
							}
						}

						loop77:
							;



						Match(input, TokenTypes.Up, null); if (state.failed) return alts;
					}

					}
					break;

				default:
					goto loop78;
				}
			}

			loop78:
				;


			Match(input,BLOCK,Follow._BLOCK_in_testSetRule1816); if (state.failed) return alts;

			Match(input, TokenTypes.Down, null); if (state.failed) return alts;
			// Grammars\\TreeToNFAConverter.g3:721:5: ( ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA ) )+
			int cnt80=0;
			for ( ; ; )
			{
				int alt80=2;
				int LA80_0 = input.LA(1);

				if ( (LA80_0==ALT) )
				{
					alt80=1;
				}


				switch ( alt80 )
				{
				case 1:
					// Grammars\\TreeToNFAConverter.g3:721:7: ^( ALT ( BACKTRACK_SEMPRED )? testSetElement EOA )
					{
					Match(input,ALT,Follow._ALT_in_testSetRule1825); if (state.failed) return alts;

					Match(input, TokenTypes.Down, null); if (state.failed) return alts;
					// Grammars\\TreeToNFAConverter.g3:721:13: ( BACKTRACK_SEMPRED )?
					int alt79=2;
					int LA79_0 = input.LA(1);

					if ( (LA79_0==BACKTRACK_SEMPRED) )
					{
						alt79=1;
					}
					switch ( alt79 )
					{
					case 1:
						// Grammars\\TreeToNFAConverter.g3:721:14: BACKTRACK_SEMPRED
						{
						Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_testSetRule1828); if (state.failed) return alts;

						}
						break;

					}

					PushFollow(Follow._testSetElement_in_testSetRule1832);
					testSetElement11=testSetElement();

					state._fsp--;
					if (state.failed) return alts;
					alts += testSetElement11;
					Match(input,EOA,Follow._EOA_in_testSetRule1836); if (state.failed) return alts;

					Match(input, TokenTypes.Up, null); if (state.failed) return alts;

					}
					break;

				default:
					if ( cnt80 >= 1 )
						goto loop80;

					if (state.backtracking>0) {state.failed=true; return alts;}
					EarlyExitException eee80 = new EarlyExitException( 80, input );
					throw eee80;
				}
				cnt80++;
			}
			loop80:
				;


			Match(input,EOB,Follow._EOB_in_testSetRule1850); if (state.failed) return alts;

			Match(input, TokenTypes.Up, null); if (state.failed) return alts;
			// Grammars\\TreeToNFAConverter.g3:725:4: ( exceptionGroup )?
			int alt81=2;
			int LA81_0 = input.LA(1);

			if ( (LA81_0==CATCH||LA81_0==FINALLY) )
			{
				alt81=1;
			}
			switch ( alt81 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:725:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_testSetRule1861);
				exceptionGroup();

				state._fsp--;
				if (state.failed) return alts;

				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_testSetRule1868); if (state.failed) return alts;

			Match(input, TokenTypes.Up, null); if (state.failed) return alts;

			}

		}
		catch ( RecognitionException re )
		{
			 throw re; 
		}
		finally
		{
			 inTest--; 
		}
		return alts;
	}
	// $ANTLR end "testSetRule"


	// $ANTLR start "testSetElement"
	// Grammars\\TreeToNFAConverter.g3:733:0: testSetElement returns [int alts=1] : (c= CHAR_LITERAL |t= TOKEN_REF |{...}? =>s= STRING_LITERAL | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) | testBlockAsSet | ^( NOT tse= testSetElement ) );
	private int testSetElement(  )
	{
		int alts = 1;

		GrammarAST c=null;
		GrammarAST t=null;
		GrammarAST s=null;
		GrammarAST c1=null;
		GrammarAST c2=null;
		int tse = default(int);
		int testBlockAsSet12 = default(int);

		try
		{
			// Grammars\\TreeToNFAConverter.g3:734:4: (c= CHAR_LITERAL |t= TOKEN_REF |{...}? =>s= STRING_LITERAL | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) | testBlockAsSet | ^( NOT tse= testSetElement ) )
			int alt82=6;
			int LA82_0 = input.LA(1);

			if ( (LA82_0==CHAR_LITERAL) )
			{
				alt82=1;
			}
			else if ( (LA82_0==TOKEN_REF) )
			{
				alt82=2;
			}
			else if ( (LA82_0==STRING_LITERAL) && ((grammar.type!=GrammarType.Lexer)))
			{
				alt82=3;
			}
			else if ( (LA82_0==CHAR_RANGE) )
			{
				alt82=4;
			}
			else if ( (LA82_0==BLOCK) )
			{
				alt82=5;
			}
			else if ( (LA82_0==NOT) )
			{
				alt82=6;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return alts;}
				NoViableAltException nvae = new NoViableAltException("", 82, 0, input);

				throw nvae;
			}
			switch ( alt82 )
			{
			case 1:
				// Grammars\\TreeToNFAConverter.g3:734:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_testSetElement1900); if (state.failed) return alts;

				}
				break;
			case 2:
				// Grammars\\TreeToNFAConverter.g3:735:4: t= TOKEN_REF
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_testSetElement1907); if (state.failed) return alts;

							if ( grammar.type==GrammarType.Lexer )
							{
								Rule rule = grammar.GetRule((t!=null?t.Text:null));
								if ( rule==null )
								{
									//throw new RecognitionException("invalid rule");
									throw new RecognitionException();
								}
								// recursively will invoke this rule to match elements in target rule ref
								alts += TestSetRule(rule.tree);
							}
						

				}
				break;
			case 3:
				// Grammars\\TreeToNFAConverter.g3:749:6: {...}? =>s= STRING_LITERAL
				{
				if ( !((grammar.type!=GrammarType.Lexer)) )
				{
					if (state.backtracking>0) {state.failed=true; return alts;}
					throw new FailedPredicateException(input, "testSetElement", "grammar.type!=GrammarType.Lexer");
				}
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_testSetElement1924); if (state.failed) return alts;

				}
				break;
			case 4:
				// Grammars\\TreeToNFAConverter.g3:750:4: ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_testSetElement1930); if (state.failed) return alts;

				Match(input, TokenTypes.Down, null); if (state.failed) return alts;
				c1=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_testSetElement1934); if (state.failed) return alts;
				c2=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_testSetElement1938); if (state.failed) return alts;

				Match(input, TokenTypes.Up, null); if (state.failed) return alts;
				 alts = IntervalSet.Of( Grammar.GetCharValueFromGrammarCharLiteral((c1!=null?c1.Text:null)), Grammar.GetCharValueFromGrammarCharLiteral((c2!=null?c2.Text:null)) ).Count; 

				}
				break;
			case 5:
				// Grammars\\TreeToNFAConverter.g3:752:6: testBlockAsSet
				{
				PushFollow(Follow._testBlockAsSet_in_testSetElement1950);
				testBlockAsSet12=testBlockAsSet();

				state._fsp--;
				if (state.failed) return alts;
				 alts = testBlockAsSet12; 

				}
				break;
			case 6:
				// Grammars\\TreeToNFAConverter.g3:754:6: ^( NOT tse= testSetElement )
				{
				Match(input,NOT,Follow._NOT_in_testSetElement1963); if (state.failed) return alts;

				Match(input, TokenTypes.Down, null); if (state.failed) return alts;
				PushFollow(Follow._testSetElement_in_testSetElement1967);
				tse=testSetElement();

				state._fsp--;
				if (state.failed) return alts;

				Match(input, TokenTypes.Up, null); if (state.failed) return alts;
				 alts = grammar.TokenTypes.Count - tse; 

				}
				break;

			}
		}
		catch ( RecognitionException re )
		{
			 throw re; 
		}
		finally
		{
		}
		return alts;
	}
	// $ANTLR end "testSetElement"
	#endregion Rules


	#region DFA
	DFA41 dfa41;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa41 = new DFA41( this );
	}

	class DFA41 : DFA
	{

		const string DFA41_eotS =
			"\x12\xFFFF";
		const string DFA41_eofS =
			"\x12\xFFFF";
		const string DFA41_minS =
			"\x1\x4\x11\xFFFF";
		const string DFA41_maxS =
			"\x1\x5F\x11\xFFFF";
		const string DFA41_acceptS =
			"\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9\x1\xA"+
			"\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x10\x1\x11";
		const string DFA41_specialS =
			"\x12\xFFFF}>";
		static readonly string[] DFA41_transitionS =
			{
				"\x1\xB\x8\xFFFF\x1\x3\x1\xF\x1\x2\x1\x8\x1\xFFFF\x1\x7\x1\x6\x1\xFFFF"+
				"\x1\x8\x7\xFFFF\x1\x7\x5\xFFFF\x1\x11\x3\xFFFF\x1\xC\x1\xFFFF\x1\x10"+
				"\xD\xFFFF\x1\x7\x1\xFFFF\x1\x8\x5\xFFFF\x1\x4\x1\x8\x4\xFFFF\x1\x5\x4"+
				"\xFFFF\x1\x1\x2\xFFFF\x1\x7\x2\xFFFF\x1\xD\x4\xFFFF\x1\x7\x1\xE\x1\xA"+
				"\x2\xFFFF\x1\x7\x2\xFFFF\x1\x9\x1\xFFFF\x1\x7",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};

		static readonly short[] DFA41_eot = DFA.UnpackEncodedString(DFA41_eotS);
		static readonly short[] DFA41_eof = DFA.UnpackEncodedString(DFA41_eofS);
		static readonly char[] DFA41_min = DFA.UnpackEncodedStringToUnsignedChars(DFA41_minS);
		static readonly char[] DFA41_max = DFA.UnpackEncodedStringToUnsignedChars(DFA41_maxS);
		static readonly short[] DFA41_accept = DFA.UnpackEncodedString(DFA41_acceptS);
		static readonly short[] DFA41_special = DFA.UnpackEncodedString(DFA41_specialS);
		static readonly short[][] DFA41_transition;

		static DFA41()
		{
			int numStates = DFA41_transitionS.Length;
			DFA41_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA41_transition[i] = DFA.UnpackEncodedString(DFA41_transitionS[i]);
			}
		}

		public DFA41( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 41;
			this.eot = DFA41_eot;
			this.eof = DFA41_eof;
			this.min = DFA41_min;
			this.max = DFA41_max;
			this.accept = DFA41_accept;
			this.special = DFA41_special;
			this.transition = DFA41_transition;
		}
		public override string GetDescription()
		{
			return "251:0: element returns [StateCluster g=null] : ( ^( ROOT e= element ) | ^( BANG e= element ) | ^( ASSIGN ID e= element ) | ^( PLUS_ASSIGN ID e= element ) | ^( RANGE a= atom[null] b= atom[null] ) | ^( CHAR_RANGE c1= CHAR_LITERAL c2= CHAR_LITERAL ) | atom_or_notatom | ebnf | tree_ | ^( SYNPRED block ) | ACTION | FORCED_ACTION |pred= SEMPRED |spred= SYN_SEMPRED | ^(bpred= BACKTRACK_SEMPRED ( . )* ) |gpred= GATED_SEMPRED | EPSILON );";
		}
	}


	#endregion DFA

	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _LEXER_GRAMMAR_in_grammar_67 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_69 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PARSER_GRAMMAR_in_grammar_79 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_81 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_GRAMMAR_in_grammar_91 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_93 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _COMBINED_GRAMMAR_in_grammar_103 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_105 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SCOPE_in_attrScope124 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScope126 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _AMPERSAND_in_attrScope131 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_attrScope140 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_grammarSpec153 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammarSpec160 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _OPTIONS_in_grammarSpec169 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _IMPORT_in_grammarSpec183 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _TOKENS_in_grammarSpec197 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _attrScope_in_grammarSpec209 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _AMPERSAND_in_grammarSpec218 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rules_in_grammarSpec230 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rule_in_rules241 = new BitSet(new ulong[]{0x400200008000202UL,0x8005000UL});
		public static readonly BitSet _RULE_in_rule255 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rule259 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_rule270 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_rule278 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule281 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RET_in_rule290 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule293 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _throwsSpec_in_rule302 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _OPTIONS_in_rule312 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ruleScopeSpec_in_rule326 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _AMPERSAND_in_rule337 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_rule351 = new BitSet(new ulong[]{0x4400020000UL});
		public static readonly BitSet _exceptionGroup_in_rule357 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_rule364 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_modifier384 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec411 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_throwsSpec413 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec428 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _AMPERSAND_in_ruleScopeSpec433 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec443 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _ID_in_ruleScopeSpec449 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _set_in_block480 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BLOCK_in_block490 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _OPTIONS_in_block495 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _alternative_in_block511 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _rewrite_in_block513 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _EOB_in_block536 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_alternative565 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_alternative570 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _EOA_in_alternative577 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup596 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup602 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup609 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler624 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler626 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler628 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FINALLY_in_finallyClause644 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_finallyClause646 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REWRITE_in_rewrite669 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ROOT_in_element696 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element700 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BANG_in_element711 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element715 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_element724 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element726 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element730 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_ASSIGN_in_element739 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element741 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element745 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RANGE_in_element756 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element760 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element765 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_RANGE_in_element779 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_element783 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_element787 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _atom_or_notatom_in_element799 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ebnf_in_element808 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_element817 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYNPRED_in_element828 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_element830 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_element839 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FORCED_ACTION_in_element848 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_element859 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYN_SEMPRED_in_element870 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_element882 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _GATED_SEMPRED_in_element897 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EPSILON_in_element906 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_ebnf932 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_ebnf942 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_ebnf953 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf957 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_ebnf970 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf974 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_ebnf987 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf991 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_1019 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_tree_1026 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _element_in_tree_1042 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _atom_in_atom_or_notatom1071 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_atom_or_notatom1083 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom_or_notatom1092 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom_or_notatom1097 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom_or_notatom1114 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom_or_notatom1119 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_atom_or_notatom1134 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RULE_REF_in_atom1176 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1181 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom1188 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom1206 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1212 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom1219 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom1237 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1243 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRING_LITERAL_in_atom1261 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1267 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _WILDCARD_in_atom1285 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1290 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOT_in_atom1307 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_atom1311 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_atom1315 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_ast_suffix1332 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BLOCK_in_set1361 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_set1370 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_set1375 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _setElement_in_set1384 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_set1387 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _EOB_in_set1397 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RULE_in_setRule1431 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_setRule1435 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_setRule1438 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_setRule1442 = new BitSet(new ulong[]{0x0UL,0x80UL});
		public static readonly BitSet _RET_in_setRule1444 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _OPTIONS_in_setRule1449 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ruleScopeSpec_in_setRule1460 = new BitSet(new ulong[]{0x10200UL});
		public static readonly BitSet _AMPERSAND_in_setRule1471 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BLOCK_in_setRule1485 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _OPTIONS_in_setRule1490 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_setRule1508 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_setRule1511 = new BitSet(new ulong[]{0x800000000D0000UL,0x4200000UL});
		public static readonly BitSet _setElement_in_setRule1515 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_setRule1518 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _EOB_in_setRule1530 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionGroup_in_setRule1542 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_setRule1549 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1578 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_setElement1589 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_setElement1601 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_RANGE_in_setElement1611 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1615 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1619 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_setElement1632 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_setElement1644 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _setElement_in_setElement1651 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BLOCK_in_testBlockAsSet1696 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_testBlockAsSet1704 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_testBlockAsSet1707 = new BitSet(new ulong[]{0x800000000D0000UL,0x4200000UL});
		public static readonly BitSet _testSetElement_in_testBlockAsSet1711 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_testBlockAsSet1715 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _EOB_in_testBlockAsSet1727 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RULE_in_testSetRule1762 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_testSetRule1766 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_testSetRule1769 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_testSetRule1773 = new BitSet(new ulong[]{0x0UL,0x80UL});
		public static readonly BitSet _RET_in_testSetRule1775 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _OPTIONS_in_testSetRule1780 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ruleScopeSpec_in_testSetRule1791 = new BitSet(new ulong[]{0x10200UL});
		public static readonly BitSet _AMPERSAND_in_testSetRule1802 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BLOCK_in_testSetRule1816 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_testSetRule1825 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_testSetRule1828 = new BitSet(new ulong[]{0x800000000D0000UL,0x4200000UL});
		public static readonly BitSet _testSetElement_in_testSetRule1832 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_testSetRule1836 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _EOB_in_testSetRule1850 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionGroup_in_testSetRule1861 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_testSetRule1868 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_testSetElement1900 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_testSetElement1907 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_testSetElement1924 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_RANGE_in_testSetElement1930 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_testSetElement1934 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_testSetElement1938 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _testBlockAsSet_in_testSetElement1950 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_testSetElement1963 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _testSetElement_in_testSetElement1967 = new BitSet(new ulong[]{0x8UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
