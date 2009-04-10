// $ANTLR 3.1.2 Grammars\\CodeGenTreeWalker.g3 2009-04-10 15:22:10

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
using StringTemplate = Antlr3.ST.StringTemplate;
using StringTemplateGroup = Antlr3.ST.StringTemplateGroup;
using TokenWithIndex = Antlr.Runtime.CommonToken;


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
/** Walk a grammar and generate code by gradually building up
 *  a bigger and bigger StringTemplate.
 *
 *  Terence Parr
 *  University of San Francisco
 *  June 15, 2004
 */
public partial class CodeGenTreeWalker : TreeParser
{
	public static readonly string[] tokenNames = new string[] {
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

	public CodeGenTreeWalker( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public CodeGenTreeWalker( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return CodeGenTreeWalker.tokenNames; }
	public override string GrammarFileName { get { return "Grammars\\CodeGenTreeWalker.g3"; } }


	#region Rules

	// $ANTLR start "grammar_"
	// Grammars\\CodeGenTreeWalker.g3:97:0: public grammar_[Grammar g,\r\n\t\tStringTemplate recognizerST,\r\n\t\tStringTemplate outputFileST,\r\n\t\tStringTemplate headerFileST] : ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) ;
	public void grammar_( Grammar g, StringTemplate recognizerST, StringTemplate outputFileST, StringTemplate headerFileST )
	{

			if ( state.backtracking == 0 )
			{
				init(g);
				this.recognizerST = recognizerST;
				this.outputFileST = outputFileST;
				this.headerFileST = headerFileST;
				string superClass = (string)g.getOption("superClass");
				outputOption = (string)g.getOption("output");
				recognizerST.SetAttribute("superClass", superClass);
				if ( g.type!=Grammar.LEXER ) {
					recognizerST.SetAttribute("ASTLabelType", g.getOption("ASTLabelType"));
				}
				if ( g.type==Grammar.TREE_PARSER && g.getOption("ASTLabelType")==null ) {
					ErrorManager.grammarWarning(ErrorManager.MSG_MISSING_AST_TYPE_IN_TREE_GRAMMAR,
											   g,
											   null,
											   g.name);
				}
				if ( g.type!=Grammar.TREE_PARSER ) {
					recognizerST.SetAttribute("labelType", g.getOption("TokenLabelType"));
				}
				recognizerST.SetAttribute("numRules", grammar.Rules.Count);
				outputFileST.SetAttribute("numRules", grammar.Rules.Count);
				headerFileST.SetAttribute("numRules", grammar.Rules.Count);
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:129:4: ( ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) ) )
			// Grammars\\CodeGenTreeWalker.g3:129:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
			{
			// Grammars\\CodeGenTreeWalker.g3:129:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
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
				// Grammars\\CodeGenTreeWalker.g3:129:6: ^( LEXER_GRAMMAR grammarSpec )
				{
				Match(input,LEXER_GRAMMAR,Follow._LEXER_GRAMMAR_in_grammar_66); if (state.failed) return ;

				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_68);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenConstants.Up, null); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:130:5: ^( PARSER_GRAMMAR grammarSpec )
				{
				Match(input,PARSER_GRAMMAR,Follow._PARSER_GRAMMAR_in_grammar_78); if (state.failed) return ;

				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_80);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenConstants.Up, null); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:131:5: ^( TREE_GRAMMAR grammarSpec )
				{
				Match(input,TREE_GRAMMAR,Follow._TREE_GRAMMAR_in_grammar_90); if (state.failed) return ;

				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_92);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenConstants.Up, null); if (state.failed) return ;

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:132:5: ^( COMBINED_GRAMMAR grammarSpec )
				{
				Match(input,COMBINED_GRAMMAR,Follow._COMBINED_GRAMMAR_in_grammar_102); if (state.failed) return ;

				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				PushFollow(Follow._grammarSpec_in_grammar_104);
				grammarSpec();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenConstants.Up, null); if (state.failed) return ;

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


	// $ANTLR start "attrScope"
	// Grammars\\CodeGenTreeWalker.g3:136:0: attrScope : ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) ;
	private void attrScope(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:137:4: ( ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION ) )
			// Grammars\\CodeGenTreeWalker.g3:137:4: ^( 'scope' ID ( ^( AMPERSAND ( . )* ) )* ACTION )
			{
			Match(input,SCOPE,Follow._SCOPE_in_attrScope123); if (state.failed) return ;

			Match(input, TokenConstants.Down, null); if (state.failed) return ;
			Match(input,ID,Follow._ID_in_attrScope125); if (state.failed) return ;
			// Grammars\\CodeGenTreeWalker.g3:137:18: ( ^( AMPERSAND ( . )* ) )*
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
					// Grammars\\CodeGenTreeWalker.g3:137:20: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_attrScope130); if (state.failed) return ;

					if ( input.LA(1)==TokenConstants.Down )
					{
						Match(input, TokenConstants.Down, null); if (state.failed) return ;
						// Grammars\\CodeGenTreeWalker.g3:137:32: ( . )*
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
								// Grammars\\CodeGenTreeWalker.g3:137:0: .
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



						Match(input, TokenConstants.Up, null); if (state.failed) return ;
					}

					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;


			Match(input,ACTION,Follow._ACTION_in_attrScope139); if (state.failed) return ;

			Match(input, TokenConstants.Up, null); if (state.failed) return ;

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
	// Grammars\\CodeGenTreeWalker.g3:140:0: grammarSpec : name= ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules[recognizerST] ;
	private void grammarSpec(  )
	{
		GrammarAST name=null;
		GrammarAST cmt=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:141:6: (name= ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules[recognizerST] )
			// Grammars\\CodeGenTreeWalker.g3:141:6: name= ID (cmt= DOC_COMMENT )? ( ^( OPTIONS ( . )* ) )? ( ^( IMPORT ( . )* ) )? ( ^( TOKENS ( . )* ) )? ( attrScope )* ( ^( AMPERSAND ( . )* ) )* rules[recognizerST]
			{
			name=(GrammarAST)Match(input,ID,Follow._ID_in_grammarSpec156); if (state.failed) return ;
			// Grammars\\CodeGenTreeWalker.g3:142:3: (cmt= DOC_COMMENT )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==DOC_COMMENT) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:142:5: cmt= DOC_COMMENT
				{
				cmt=(GrammarAST)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammarSpec164); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

									outputFileST.SetAttribute("docComment", (cmt!=null?cmt.Text:null));
									headerFileST.SetAttribute("docComment", (cmt!=null?cmt.Text:null));
								
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							recognizerST.SetAttribute("name", grammar.getRecognizerName());
							outputFileST.SetAttribute("name", grammar.getRecognizerName());
							headerFileST.SetAttribute("name", grammar.getRecognizerName());
							recognizerST.SetAttribute("scopes", grammar.GlobalScopes);
							headerFileST.SetAttribute("scopes", grammar.GlobalScopes);
						
			}
			// Grammars\\CodeGenTreeWalker.g3:155:3: ( ^( OPTIONS ( . )* ) )?
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==OPTIONS) )
			{
				alt6=1;
			}
			switch ( alt6 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:155:5: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_grammarSpec185); if (state.failed) return ;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return ;
					// Grammars\\CodeGenTreeWalker.g3:155:15: ( . )*
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
							// Grammars\\CodeGenTreeWalker.g3:155:0: .
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



					Match(input, TokenConstants.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:156:3: ( ^( IMPORT ( . )* ) )?
			int alt8=2;
			int LA8_0 = input.LA(1);

			if ( (LA8_0==IMPORT) )
			{
				alt8=1;
			}
			switch ( alt8 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:156:5: ^( IMPORT ( . )* )
				{
				Match(input,IMPORT,Follow._IMPORT_in_grammarSpec199); if (state.failed) return ;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return ;
					// Grammars\\CodeGenTreeWalker.g3:156:14: ( . )*
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
							// Grammars\\CodeGenTreeWalker.g3:156:0: .
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



					Match(input, TokenConstants.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:157:3: ( ^( TOKENS ( . )* ) )?
			int alt10=2;
			int LA10_0 = input.LA(1);

			if ( (LA10_0==TOKENS) )
			{
				alt10=1;
			}
			switch ( alt10 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:157:5: ^( TOKENS ( . )* )
				{
				Match(input,TOKENS,Follow._TOKENS_in_grammarSpec213); if (state.failed) return ;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return ;
					// Grammars\\CodeGenTreeWalker.g3:157:14: ( . )*
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
							// Grammars\\CodeGenTreeWalker.g3:157:0: .
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



					Match(input, TokenConstants.Up, null); if (state.failed) return ;
				}

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:158:3: ( attrScope )*
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
					// Grammars\\CodeGenTreeWalker.g3:158:4: attrScope
					{
					PushFollow(Follow._attrScope_in_grammarSpec225);
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


			// Grammars\\CodeGenTreeWalker.g3:159:3: ( ^( AMPERSAND ( . )* ) )*
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
					// Grammars\\CodeGenTreeWalker.g3:159:5: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_grammarSpec234); if (state.failed) return ;

					if ( input.LA(1)==TokenConstants.Down )
					{
						Match(input, TokenConstants.Down, null); if (state.failed) return ;
						// Grammars\\CodeGenTreeWalker.g3:159:17: ( . )*
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
								// Grammars\\CodeGenTreeWalker.g3:159:0: .
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



						Match(input, TokenConstants.Up, null); if (state.failed) return ;
					}

					}
					break;

				default:
					goto loop13;
				}
			}

			loop13:
				;


			PushFollow(Follow._rules_in_grammarSpec245);
			rules(recognizerST);

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
	// Grammars\\CodeGenTreeWalker.g3:163:0: rules[StringTemplate recognizerST] : ( ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) ) )+ ;
	private void rules( StringTemplate recognizerST )
	{
		CodeGenTreeWalker.rule_return rST = default(CodeGenTreeWalker.rule_return);


			string ruleName = ((GrammarAST)input.LT(1)).GetChild(0).Text;
			bool generated = grammar.generateMethodForRule(ruleName);

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:169:4: ( ( ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) ) )+ )
			// Grammars\\CodeGenTreeWalker.g3:169:4: ( ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) ) )+
			{
			// Grammars\\CodeGenTreeWalker.g3:169:4: ( ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) ) )+
			int cnt16=0;
			for ( ; ; )
			{
				int alt16=2;
				int LA16_0 = input.LA(1);

				if ( (LA16_0==RULE) )
				{
					alt16=1;
				}


				switch ( alt16 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:169:6: ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) )
					{
					// Grammars\\CodeGenTreeWalker.g3:169:6: ( options {k=1; } :{...}? =>rST= rule | ^( RULE ( . )* ) )
					int alt15=2;
					int LA15_0 = input.LA(1);

					if ( (LA15_0==RULE) )
					{
						int LA15_1 = input.LA(2);

						if ( ((generated)) )
						{
							alt15=1;
						}
						else if ( (true) )
						{
							alt15=2;
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return ;}
							NoViableAltException nvae = new NoViableAltException("", 15, 1, input);

							throw nvae;
						}
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 15, 0, input);

						throw nvae;
					}
					switch ( alt15 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:170:5: {...}? =>rST= rule
						{
						if ( !((generated)) )
						{
							if (state.backtracking>0) {state.failed=true; return ;}
							throw new FailedPredicateException(input, "rules", "generated");
						}
						PushFollow(Follow._rule_in_rules290);
						rST=rule();

						state._fsp--;
						if (state.failed) return ;
						if ( state.backtracking == 0 )
						{

												if ( (rST!=null?rST.code:default(StringTemplate)) != null )
												{
													recognizerST.SetAttribute("rules", (rST!=null?rST.code:default(StringTemplate)));
													outputFileST.SetAttribute("rules", (rST!=null?rST.code:default(StringTemplate)));
													headerFileST.SetAttribute("rules", (rST!=null?rST.code:default(StringTemplate)));
												}
											
						}

						}
						break;
					case 2:
						// Grammars\\CodeGenTreeWalker.g3:180:6: ^( RULE ( . )* )
						{
						Match(input,RULE,Follow._RULE_in_rules304); if (state.failed) return ;

						if ( input.LA(1)==TokenConstants.Down )
						{
							Match(input, TokenConstants.Down, null); if (state.failed) return ;
							// Grammars\\CodeGenTreeWalker.g3:180:13: ( . )*
							for ( ; ; )
							{
								int alt14=2;
								int LA14_0 = input.LA(1);

								if ( ((LA14_0>=ACTION && LA14_0<=XDIGIT)) )
								{
									alt14=1;
								}
								else if ( (LA14_0==UP) )
								{
									alt14=2;
								}


								switch ( alt14 )
								{
								case 1:
									// Grammars\\CodeGenTreeWalker.g3:180:0: .
									{
									MatchAny(input); if (state.failed) return ;

									}
									break;

								default:
									goto loop14;
								}
							}

							loop14:
								;



							Match(input, TokenConstants.Up, null); if (state.failed) return ;
						}

						}
						break;

					}


									if ( input.LA(1) == RULE )
									{
										ruleName = ((GrammarAST)input.LT(1)).GetChild(0).Text;
										//System.Diagnostics.Debug.Assert( ruleName == ((GrammarAST)input.LT(1)).enclosingRuleName );
										generated = grammar.generateMethodForRule(ruleName);
									}
								

					}
					break;

				default:
					if ( cnt16 >= 1 )
						goto loop16;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee16 = new EarlyExitException( 16, input );
					throw eee16;
				}
				cnt16++;
			}
			loop16:
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
		public StringTemplate code=null;
	}

	// $ANTLR start "rule"
	// Grammars\\CodeGenTreeWalker.g3:193:0: rule returns [StringTemplate code=null] : ^( RULE id= ID (mod= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block[\"ruleBlock\", dfa] ( exceptionGroup[$code] )? EOR ) ;
	private CodeGenTreeWalker.rule_return rule(  )
	{
		CodeGenTreeWalker.rule_return retval = new CodeGenTreeWalker.rule_return();
		retval.start = input.LT(1);

		GrammarAST id=null;
		CodeGenTreeWalker.modifier_return mod = default(CodeGenTreeWalker.modifier_return);
		CodeGenTreeWalker.block_return b = default(CodeGenTreeWalker.block_return);


			string initAction = null;
			// get the dfa for the BLOCK
			GrammarAST block2=(GrammarAST)((GrammarAST)retval.start).GetFirstChildWithType(BLOCK);
			Antlr3.Analysis.DFA dfa=block2.LookaheadDFA;
			// init blockNestingLevel so it's block level RULE_BLOCK_NESTING_LEVEL
			// for alts of rule
			blockNestingLevel = RULE_BLOCK_NESTING_LEVEL-1;
			Rule ruleDescr = grammar.getRule(((GrammarAST)retval.start).GetChild(0).Text);
			currentRuleName = ((GrammarAST)retval.start).GetChild(0).Text;

			// For syn preds, we don't want any AST code etc... in there.
			// Save old templates ptr and restore later.  Base templates include Dbg.
			StringTemplateGroup saveGroup = templates;
			if ( ruleDescr.isSynPred )
			{
				templates = generator.getBaseTemplates();
			}

			string description = string.Empty;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:216:4: ( ^( RULE id= ID (mod= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block[\"ruleBlock\", dfa] ( exceptionGroup[$code] )? EOR ) )
			// Grammars\\CodeGenTreeWalker.g3:216:4: ^( RULE id= ID (mod= modifier )? ^( ARG ( ARG_ACTION )? ) ^( RET ( ARG_ACTION )? ) ( throwsSpec )? ( ^( OPTIONS ( . )* ) )? ( ruleScopeSpec )? ( ^( AMPERSAND ( . )* ) )* b= block[\"ruleBlock\", dfa] ( exceptionGroup[$code] )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_rule345); if (state.failed) return retval;

			Match(input, TokenConstants.Down, null); if (state.failed) return retval;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_rule349); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{
				 System.Diagnostics.Debug.Assert( currentRuleName == (id!=null?id.Text:null) ); 
			}
			// Grammars\\CodeGenTreeWalker.g3:218:4: (mod= modifier )?
			int alt17=2;
			int LA17_0 = input.LA(1);

			if ( (LA17_0==FRAGMENT||(LA17_0>=PRIVATE && LA17_0<=PUBLIC)) )
			{
				alt17=1;
			}
			switch ( alt17 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:218:5: mod= modifier
				{
				PushFollow(Follow._modifier_in_rule362);
				mod=modifier();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_rule370); if (state.failed) return retval;

			if ( input.LA(1)==TokenConstants.Down )
			{
				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				// Grammars\\CodeGenTreeWalker.g3:219:10: ( ARG_ACTION )?
				int alt18=2;
				int LA18_0 = input.LA(1);

				if ( (LA18_0==ARG_ACTION) )
				{
					alt18=1;
				}
				switch ( alt18 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:219:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule373); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
			}
			Match(input,RET,Follow._RET_in_rule382); if (state.failed) return retval;

			if ( input.LA(1)==TokenConstants.Down )
			{
				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				// Grammars\\CodeGenTreeWalker.g3:220:10: ( ARG_ACTION )?
				int alt19=2;
				int LA19_0 = input.LA(1);

				if ( (LA19_0==ARG_ACTION) )
				{
					alt19=1;
				}
				switch ( alt19 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:220:11: ARG_ACTION
					{
					Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule385); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
			}
			// Grammars\\CodeGenTreeWalker.g3:221:4: ( throwsSpec )?
			int alt20=2;
			int LA20_0 = input.LA(1);

			if ( (LA20_0==THROWS) )
			{
				alt20=1;
			}
			switch ( alt20 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:221:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule394);
				throwsSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:222:4: ( ^( OPTIONS ( . )* ) )?
			int alt22=2;
			int LA22_0 = input.LA(1);

			if ( (LA22_0==OPTIONS) )
			{
				alt22=1;
			}
			switch ( alt22 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:222:6: ^( OPTIONS ( . )* )
				{
				Match(input,OPTIONS,Follow._OPTIONS_in_rule404); if (state.failed) return retval;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:222:16: ( . )*
					for ( ; ; )
					{
						int alt21=2;
						int LA21_0 = input.LA(1);

						if ( ((LA21_0>=ACTION && LA21_0<=XDIGIT)) )
						{
							alt21=1;
						}
						else if ( (LA21_0==UP) )
						{
							alt21=2;
						}


						switch ( alt21 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:222:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop21;
						}
					}

					loop21:
						;



					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				}

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:223:4: ( ruleScopeSpec )?
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0==SCOPE) )
			{
				alt23=1;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:223:5: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_rule417);
				ruleScopeSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\CodeGenTreeWalker.g3:224:4: ( ^( AMPERSAND ( . )* ) )*
			for ( ; ; )
			{
				int alt25=2;
				int LA25_0 = input.LA(1);

				if ( (LA25_0==AMPERSAND) )
				{
					alt25=1;
				}


				switch ( alt25 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:224:6: ^( AMPERSAND ( . )* )
					{
					Match(input,AMPERSAND,Follow._AMPERSAND_in_rule427); if (state.failed) return retval;

					if ( input.LA(1)==TokenConstants.Down )
					{
						Match(input, TokenConstants.Down, null); if (state.failed) return retval;
						// Grammars\\CodeGenTreeWalker.g3:224:18: ( . )*
						for ( ; ; )
						{
							int alt24=2;
							int LA24_0 = input.LA(1);

							if ( ((LA24_0>=ACTION && LA24_0<=XDIGIT)) )
							{
								alt24=1;
							}
							else if ( (LA24_0==UP) )
							{
								alt24=2;
							}


							switch ( alt24 )
							{
							case 1:
								// Grammars\\CodeGenTreeWalker.g3:224:0: .
								{
								MatchAny(input); if (state.failed) return retval;

								}
								break;

							default:
								goto loop24;
							}
						}

						loop24:
							;



						Match(input, TokenConstants.Up, null); if (state.failed) return retval;
					}

					}
					break;

				default:
					goto loop25;
				}
			}

			loop25:
				;


			PushFollow(Follow._block_in_rule441);
			b=block("ruleBlock", dfa);

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								description =
									grammar.grammarTreeToString((GrammarAST)((GrammarAST)retval.start).GetFirstChildWithType(BLOCK),
																false);
								description =
									generator.target.getTargetStringLiteralFromString(description);
								(b!=null?b.code:default(StringTemplate)).SetAttribute("description", description);
								// do not generate lexer rules in combined grammar
								string stName = null;
								if ( ruleDescr.isSynPred )
								{
									stName = "synpredRule";
								}
								else if ( grammar.type==Grammar.LEXER )
								{
									if ( currentRuleName.Equals(Grammar.ARTIFICIAL_TOKENS_RULENAME) )
									{
										stName = "tokensRule";
									}
									else
									{
										stName = "lexerRule";
									}
								}
								else
								{
									if ( !(grammar.type==Grammar.COMBINED &&
										 char.IsUpper(currentRuleName[0])) )
									{
										stName = "rule";
									}
								}
								retval.code = templates.GetInstanceOf(stName);
								if ( retval.code.Name.Equals("rule") )
								{
									retval.code.SetAttribute("emptyRule", grammar.isEmptyRule(block2));
								}
								retval.code.SetAttribute("ruleDescriptor", ruleDescr);
								string memo = (string)grammar.getBlockOption(((GrammarAST)retval.start),"memoize");
								if ( memo==null )
								{
									memo = (string)grammar.getOption("memoize");
								}
								if ( memo!=null && memo.Equals("true") &&
									 (stName.Equals("rule")||stName.Equals("lexerRule")) )
								{
									retval.code.SetAttribute("memoize", memo!=null && memo.Equals("true"));
								}
							
			}
			// Grammars\\CodeGenTreeWalker.g3:276:4: ( exceptionGroup[$code] )?
			int alt26=2;
			int LA26_0 = input.LA(1);

			if ( (LA26_0==CATCH||LA26_0==FINALLY) )
			{
				alt26=1;
			}
			switch ( alt26 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:276:5: exceptionGroup[$code]
				{
				PushFollow(Follow._exceptionGroup_in_rule454);
				exceptionGroup(retval.code);

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_rule462); if (state.failed) return retval;

			Match(input, TokenConstants.Up, null); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

							if ( retval.code!=null )
							{
								if ( grammar.type==Grammar.LEXER )
								{
									bool naked =
										currentRuleName.Equals(Grammar.ARTIFICIAL_TOKENS_RULENAME) ||
										((mod!=null?((GrammarAST)mod.start):null)!=null&&(mod!=null?((GrammarAST)mod.start):null).Text.Equals(Grammar.FRAGMENT_RULE_MODIFIER));
									retval.code.SetAttribute("nakedBlock", naked);
								}
								else
								{
									description = grammar.grammarTreeToString(((GrammarAST)retval.start),false);
									description = generator.target.getTargetStringLiteralFromString(description);
									retval.code.SetAttribute("description", description);
								}
								Rule theRule = grammar.getRule(currentRuleName);
								generator.translateActionAttributeReferencesForSingleScope(
									theRule,
									theRule.Actions
								);
								retval.code.SetAttribute("ruleName", currentRuleName);
								retval.code.SetAttribute("block", (b!=null?b.code:default(StringTemplate)));
								if ( initAction!=null )
								{
									retval.code.SetAttribute("initAction", initAction);
								}
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
			 templates = saveGroup; 
		}
		return retval;
	}
	// $ANTLR end "rule"

	public class modifier_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "modifier"
	// Grammars\\CodeGenTreeWalker.g3:311:0: modifier : ( 'protected' | 'public' | 'private' | 'fragment' );
	private CodeGenTreeWalker.modifier_return modifier(  )
	{
		CodeGenTreeWalker.modifier_return retval = new CodeGenTreeWalker.modifier_return();
		retval.start = input.LT(1);

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:312:4: ( 'protected' | 'public' | 'private' | 'fragment' )
			// Grammars\\CodeGenTreeWalker.g3:
			{
			if ( input.LA(1)==FRAGMENT||(input.LA(1)>=PRIVATE && input.LA(1)<=PUBLIC) )
			{
				input.Consume();
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
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
	// Grammars\\CodeGenTreeWalker.g3:318:0: throwsSpec : ^( 'throws' ( ID )+ ) ;
	private void throwsSpec(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:319:4: ( ^( 'throws' ( ID )+ ) )
			// Grammars\\CodeGenTreeWalker.g3:319:4: ^( 'throws' ( ID )+ )
			{
			Match(input,THROWS,Follow._THROWS_in_throwsSpec512); if (state.failed) return ;

			Match(input, TokenConstants.Down, null); if (state.failed) return ;
			// Grammars\\CodeGenTreeWalker.g3:319:15: ( ID )+
			int cnt27=0;
			for ( ; ; )
			{
				int alt27=2;
				int LA27_0 = input.LA(1);

				if ( (LA27_0==ID) )
				{
					alt27=1;
				}


				switch ( alt27 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:319:0: ID
					{
					Match(input,ID,Follow._ID_in_throwsSpec514); if (state.failed) return ;

					}
					break;

				default:
					if ( cnt27 >= 1 )
						goto loop27;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee27 = new EarlyExitException( 27, input );
					throw eee27;
				}
				cnt27++;
			}
			loop27:
				;



			Match(input, TokenConstants.Up, null); if (state.failed) return ;

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
	// Grammars\\CodeGenTreeWalker.g3:322:0: ruleScopeSpec : ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) ;
	private void ruleScopeSpec(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:323:4: ( ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* ) )
			// Grammars\\CodeGenTreeWalker.g3:323:4: ^( 'scope' ( ^( AMPERSAND ( . )* ) )* ( ACTION )? ( ID )* )
			{
			Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec529); if (state.failed) return ;

			if ( input.LA(1)==TokenConstants.Down )
			{
				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				// Grammars\\CodeGenTreeWalker.g3:323:15: ( ^( AMPERSAND ( . )* ) )*
				for ( ; ; )
				{
					int alt29=2;
					int LA29_0 = input.LA(1);

					if ( (LA29_0==AMPERSAND) )
					{
						alt29=1;
					}


					switch ( alt29 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:323:17: ^( AMPERSAND ( . )* )
						{
						Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleScopeSpec534); if (state.failed) return ;

						if ( input.LA(1)==TokenConstants.Down )
						{
							Match(input, TokenConstants.Down, null); if (state.failed) return ;
							// Grammars\\CodeGenTreeWalker.g3:323:29: ( . )*
							for ( ; ; )
							{
								int alt28=2;
								int LA28_0 = input.LA(1);

								if ( ((LA28_0>=ACTION && LA28_0<=XDIGIT)) )
								{
									alt28=1;
								}
								else if ( (LA28_0==UP) )
								{
									alt28=2;
								}


								switch ( alt28 )
								{
								case 1:
									// Grammars\\CodeGenTreeWalker.g3:323:0: .
									{
									MatchAny(input); if (state.failed) return ;

									}
									break;

								default:
									goto loop28;
								}
							}

							loop28:
								;



							Match(input, TokenConstants.Up, null); if (state.failed) return ;
						}

						}
						break;

					default:
						goto loop29;
					}
				}

				loop29:
					;


				// Grammars\\CodeGenTreeWalker.g3:323:36: ( ACTION )?
				int alt30=2;
				int LA30_0 = input.LA(1);

				if ( (LA30_0==ACTION) )
				{
					alt30=1;
				}
				switch ( alt30 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:323:37: ACTION
					{
					Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec544); if (state.failed) return ;

					}
					break;

				}

				// Grammars\\CodeGenTreeWalker.g3:323:46: ( ID )*
				for ( ; ; )
				{
					int alt31=2;
					int LA31_0 = input.LA(1);

					if ( (LA31_0==ID) )
					{
						alt31=1;
					}


					switch ( alt31 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:323:48: ID
						{
						Match(input,ID,Follow._ID_in_ruleScopeSpec550); if (state.failed) return ;

						}
						break;

					default:
						goto loop31;
					}
				}

				loop31:
					;



				Match(input, TokenConstants.Up, null); if (state.failed) return ;
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
		public StringTemplate code=null;
	}

	// $ANTLR start "block"
	// Grammars\\CodeGenTreeWalker.g3:326:0: block[string blockTemplateName, Antlr3.Analysis.DFA dfa] returns [StringTemplate code=null] options {k=1; } : ({...}? => setBlock | ^( BLOCK ( ^( OPTIONS ( . )* ) )? (alt= alternative rew= rewrite )+ EOB ) );
	private CodeGenTreeWalker.block_return block( string blockTemplateName, Antlr3.Analysis.DFA dfa )
	{
		CodeGenTreeWalker.block_return retval = new CodeGenTreeWalker.block_return();
		retval.start = input.LT(1);

		CodeGenTreeWalker.alternative_return alt = default(CodeGenTreeWalker.alternative_return);
		CodeGenTreeWalker.rewrite_return rew = default(CodeGenTreeWalker.rewrite_return);
		CodeGenTreeWalker.setBlock_return setBlock1 = default(CodeGenTreeWalker.setBlock_return);


			int altNum = 0;

			blockNestingLevel++;
			if ( state.backtracking == 0 )
			{
				StringTemplate decision = null;
				if ( dfa != null )
				{
					retval.code = templates.GetInstanceOf(blockTemplateName);
					decision = generator.genLookaheadDecision(recognizerST,dfa);
					retval.code.SetAttribute("decision", decision);
					retval.code.SetAttribute("decisionNumber", dfa.DecisionNumber);
					retval.code.SetAttribute("maxK",dfa.MaxLookaheadDepth);
					retval.code.SetAttribute("maxAlt",dfa.NumberOfAlts);
				}
				else
				{
					retval.code = templates.GetInstanceOf(blockTemplateName+"SingleAlt");
				}
				retval.code.SetAttribute("blockLevel", blockNestingLevel);
				retval.code.SetAttribute("enclosingBlockLevel", blockNestingLevel-1);
				altNum = 1;
				if ( this.blockNestingLevel==RULE_BLOCK_NESTING_LEVEL ) {
					this.outerAltNum=1;
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:358:4: ({...}? => setBlock | ^( BLOCK ( ^( OPTIONS ( . )* ) )? (alt= alternative rew= rewrite )+ EOB ) )
			int alt35=2;
			int LA35_0 = input.LA(1);

			if ( (LA35_0==BLOCK) )
			{
				int LA35_1 = input.LA(2);

				if ( ((((GrammarAST)retval.start).SetValue!=null)) )
				{
					alt35=1;
				}
				else if ( (true) )
				{
					alt35=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 35, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 35, 0, input);

				throw nvae;
			}
			switch ( alt35 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:358:4: {...}? => setBlock
				{
				if ( !((((GrammarAST)retval.start).SetValue!=null)) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "block", "$start.SetValue!=null");
				}
				PushFollow(Follow._setBlock_in_block591);
				setBlock1=setBlock();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								retval.code.SetAttribute("alts",(setBlock1!=null?setBlock1.code:default(StringTemplate)));
							
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:363:4: ^( BLOCK ( ^( OPTIONS ( . )* ) )? (alt= alternative rew= rewrite )+ EOB )
				{
				Match(input,BLOCK,Follow._BLOCK_in_block604); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				// Grammars\\CodeGenTreeWalker.g3:364:4: ( ^( OPTIONS ( . )* ) )?
				int alt33=2;
				int LA33_0 = input.LA(1);

				if ( (LA33_0==OPTIONS) )
				{
					alt33=1;
				}
				switch ( alt33 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:364:6: ^( OPTIONS ( . )* )
					{
					Match(input,OPTIONS,Follow._OPTIONS_in_block612); if (state.failed) return retval;

					if ( input.LA(1)==TokenConstants.Down )
					{
						Match(input, TokenConstants.Down, null); if (state.failed) return retval;
						// Grammars\\CodeGenTreeWalker.g3:364:16: ( . )*
						for ( ; ; )
						{
							int alt32=2;
							int LA32_0 = input.LA(1);

							if ( ((LA32_0>=ACTION && LA32_0<=XDIGIT)) )
							{
								alt32=1;
							}
							else if ( (LA32_0==UP) )
							{
								alt32=2;
							}


							switch ( alt32 )
							{
							case 1:
								// Grammars\\CodeGenTreeWalker.g3:364:0: .
								{
								MatchAny(input); if (state.failed) return retval;

								}
								break;

							default:
								goto loop32;
							}
						}

						loop32:
							;



						Match(input, TokenConstants.Up, null); if (state.failed) return retval;
					}

					}
					break;

				}

				// Grammars\\CodeGenTreeWalker.g3:365:4: (alt= alternative rew= rewrite )+
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
						// Grammars\\CodeGenTreeWalker.g3:365:6: alt= alternative rew= rewrite
						{
						PushFollow(Follow._alternative_in_block629);
						alt=alternative();

						state._fsp--;
						if (state.failed) return retval;
						PushFollow(Follow._rewrite_in_block633);
						rew=rewrite();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 )
						{

												if ( this.blockNestingLevel==RULE_BLOCK_NESTING_LEVEL )
												{
													this.outerAltNum++;
												}
												// add the rewrite code as just another element in the alt :)
												// (unless it's a " -> ..." rewrite
												// ( -> ... )
												bool etc =
													(rew!=null?((GrammarAST)rew.start):null).Type==REWRITE &&
													(rew!=null?((GrammarAST)rew.start):null).GetChild(0)!=null &&
													(rew!=null?((GrammarAST)rew.start):null).GetChild(0).Type==ETC;
												if ( (rew!=null?rew.code:default(StringTemplate))!=null && !etc )
												{
													(alt!=null?alt.code:default(StringTemplate)).SetAttribute("rew", (rew!=null?rew.code:default(StringTemplate)));
												}
												// add this alt to the list of alts for this block
												retval.code.SetAttribute("alts",(alt!=null?alt.code:default(StringTemplate)));
												(alt!=null?alt.code:default(StringTemplate)).SetAttribute("altNum", altNum);
												(alt!=null?alt.code:default(StringTemplate)).SetAttribute("outerAlt", blockNestingLevel==RULE_BLOCK_NESTING_LEVEL);
												altNum++;
											
						}

						}
						break;

					default:
						if ( cnt34 >= 1 )
							goto loop34;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee34 = new EarlyExitException( 34, input );
						throw eee34;
					}
					cnt34++;
				}
				loop34:
					;


				Match(input,EOB,Follow._EOB_in_block650); if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;

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
			 blockNestingLevel--; 
		}
		return retval;
	}
	// $ANTLR end "block"

	public class setBlock_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "setBlock"
	// Grammars\\CodeGenTreeWalker.g3:394:0: setBlock returns [StringTemplate code=null] : ^(s= BLOCK ( . )* ) ;
	private CodeGenTreeWalker.setBlock_return setBlock(  )
	{
		CodeGenTreeWalker.setBlock_return retval = new CodeGenTreeWalker.setBlock_return();
		retval.start = input.LT(1);

		GrammarAST s=null;


			StringTemplate setcode = null;
			if ( state.backtracking == 0 )
			{
				if ( blockNestingLevel==RULE_BLOCK_NESTING_LEVEL && grammar.BuildAST )
				{
					Rule r = grammar.getRule(currentRuleName);
					currentAltHasASTRewrite = r.hasRewrite(outerAltNum);
					if ( currentAltHasASTRewrite )
					{
						r.trackTokenReferenceInAlt(((GrammarAST)retval.start), outerAltNum);
					}
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:411:4: ( ^(s= BLOCK ( . )* ) )
			// Grammars\\CodeGenTreeWalker.g3:411:4: ^(s= BLOCK ( . )* )
			{
			s=(GrammarAST)Match(input,BLOCK,Follow._BLOCK_in_setBlock682); if (state.failed) return retval;

			if ( input.LA(1)==TokenConstants.Down )
			{
				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				// Grammars\\CodeGenTreeWalker.g3:411:14: ( . )*
				for ( ; ; )
				{
					int alt36=2;
					int LA36_0 = input.LA(1);

					if ( ((LA36_0>=ACTION && LA36_0<=XDIGIT)) )
					{
						alt36=1;
					}
					else if ( (LA36_0==UP) )
					{
						alt36=2;
					}


					switch ( alt36 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:411:0: .
						{
						MatchAny(input); if (state.failed) return retval;

						}
						break;

					default:
						goto loop36;
					}
				}

				loop36:
					;



				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
			}
			if ( state.backtracking == 0 )
			{

							int i = ((TokenWithIndex)s.Token).TokenIndex;
							if ( blockNestingLevel==RULE_BLOCK_NESTING_LEVEL )
							{
								setcode = getTokenElementST("matchRuleBlockSet", "set", s, null, null);
							}
							else
							{
								setcode = getTokenElementST("matchSet", "set", s, null, null);
							}
							setcode.SetAttribute("elementIndex", i);
							if ( grammar.type!=Grammar.LEXER )
							{
								generator.generateLocalFOLLOW(s,"set",currentRuleName,i);
							}
							setcode.SetAttribute("s",
								generator.genSetExpr(templates,s.SetValue,1,false));
							StringTemplate altcode=templates.GetInstanceOf("alt");
							altcode.SetAttribute("elements.{el,line,pos}",
												 setcode,
												 s.Line,
												 s.CharPositionInLine
												);
							altcode.SetAttribute("altNum", 1);
							altcode.SetAttribute("outerAlt", blockNestingLevel==RULE_BLOCK_NESTING_LEVEL);
							if ( !currentAltHasASTRewrite && grammar.BuildAST )
							{
								altcode.SetAttribute("autoAST", true);
							}
							altcode.SetAttribute("treeLevel", rewriteTreeNestingLevel);
							retval.code = altcode;
						
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
	// $ANTLR end "setBlock"


	// $ANTLR start "setAlternative"
	// Grammars\\CodeGenTreeWalker.g3:446:0: setAlternative : ^( ALT ( setElement )+ EOA ) ;
	private void setAlternative(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:447:4: ( ^( ALT ( setElement )+ EOA ) )
			// Grammars\\CodeGenTreeWalker.g3:447:4: ^( ALT ( setElement )+ EOA )
			{
			Match(input,ALT,Follow._ALT_in_setAlternative702); if (state.failed) return ;

			Match(input, TokenConstants.Down, null); if (state.failed) return ;
			// Grammars\\CodeGenTreeWalker.g3:447:10: ( setElement )+
			int cnt37=0;
			for ( ; ; )
			{
				int alt37=2;
				int LA37_0 = input.LA(1);

				if ( ((LA37_0>=CHAR_LITERAL && LA37_0<=CHAR_RANGE)||LA37_0==STRING_LITERAL||LA37_0==TOKEN_REF) )
				{
					alt37=1;
				}


				switch ( alt37 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:447:0: setElement
					{
					PushFollow(Follow._setElement_in_setAlternative704);
					setElement();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					if ( cnt37 >= 1 )
						goto loop37;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee37 = new EarlyExitException( 37, input );
					throw eee37;
				}
				cnt37++;
			}
			loop37:
				;


			Match(input,EOA,Follow._EOA_in_setAlternative707); if (state.failed) return ;

			Match(input, TokenConstants.Up, null); if (state.failed) return ;

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
	// $ANTLR end "setAlternative"


	// $ANTLR start "exceptionGroup"
	// Grammars\\CodeGenTreeWalker.g3:450:0: exceptionGroup[StringTemplate ruleST] : ( ( exceptionHandler[$ruleST] )+ ( finallyClause[$ruleST] )? | finallyClause[$ruleST] );
	private void exceptionGroup( StringTemplate ruleST )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:451:4: ( ( exceptionHandler[$ruleST] )+ ( finallyClause[$ruleST] )? | finallyClause[$ruleST] )
			int alt40=2;
			int LA40_0 = input.LA(1);

			if ( (LA40_0==CATCH) )
			{
				alt40=1;
			}
			else if ( (LA40_0==FINALLY) )
			{
				alt40=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 40, 0, input);

				throw nvae;
			}
			switch ( alt40 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:451:4: ( exceptionHandler[$ruleST] )+ ( finallyClause[$ruleST] )?
				{
				// Grammars\\CodeGenTreeWalker.g3:451:4: ( exceptionHandler[$ruleST] )+
				int cnt38=0;
				for ( ; ; )
				{
					int alt38=2;
					int LA38_0 = input.LA(1);

					if ( (LA38_0==CATCH) )
					{
						alt38=1;
					}


					switch ( alt38 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:451:6: exceptionHandler[$ruleST]
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup722);
						exceptionHandler(ruleST);

						state._fsp--;
						if (state.failed) return ;

						}
						break;

					default:
						if ( cnt38 >= 1 )
							goto loop38;

						if (state.backtracking>0) {state.failed=true; return ;}
						EarlyExitException eee38 = new EarlyExitException( 38, input );
						throw eee38;
					}
					cnt38++;
				}
				loop38:
					;


				// Grammars\\CodeGenTreeWalker.g3:451:35: ( finallyClause[$ruleST] )?
				int alt39=2;
				int LA39_0 = input.LA(1);

				if ( (LA39_0==FINALLY) )
				{
					alt39=1;
				}
				switch ( alt39 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:451:36: finallyClause[$ruleST]
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup729);
					finallyClause(ruleST);

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:452:4: finallyClause[$ruleST]
				{
				PushFollow(Follow._finallyClause_in_exceptionGroup737);
				finallyClause(ruleST);

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
	// Grammars\\CodeGenTreeWalker.g3:455:0: exceptionHandler[StringTemplate ruleST] : ^( 'catch' ARG_ACTION ACTION ) ;
	private void exceptionHandler( StringTemplate ruleST )
	{
		GrammarAST ACTION2=null;
		GrammarAST ARG_ACTION3=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:456:4: ( ^( 'catch' ARG_ACTION ACTION ) )
			// Grammars\\CodeGenTreeWalker.g3:456:4: ^( 'catch' ARG_ACTION ACTION )
			{
			Match(input,CATCH,Follow._CATCH_in_exceptionHandler751); if (state.failed) return ;

			Match(input, TokenConstants.Down, null); if (state.failed) return ;
			ARG_ACTION3=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler753); if (state.failed) return ;
			ACTION2=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_exceptionHandler755); if (state.failed) return ;

			Match(input, TokenConstants.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							List chunks = generator.translateAction(currentRuleName,ACTION2);
							ruleST.SetAttribute("exceptions.{decl,action}",(ARG_ACTION3!=null?ARG_ACTION3.Text:null),chunks);
						
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
	// $ANTLR end "exceptionHandler"


	// $ANTLR start "finallyClause"
	// Grammars\\CodeGenTreeWalker.g3:463:0: finallyClause[StringTemplate ruleST] : ^( 'finally' ACTION ) ;
	private void finallyClause( StringTemplate ruleST )
	{
		GrammarAST ACTION4=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:464:4: ( ^( 'finally' ACTION ) )
			// Grammars\\CodeGenTreeWalker.g3:464:4: ^( 'finally' ACTION )
			{
			Match(input,FINALLY,Follow._FINALLY_in_finallyClause773); if (state.failed) return ;

			Match(input, TokenConstants.Down, null); if (state.failed) return ;
			ACTION4=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_finallyClause775); if (state.failed) return ;

			Match(input, TokenConstants.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							List chunks = generator.translateAction(currentRuleName,ACTION4);
							ruleST.SetAttribute("finally",chunks);
						
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
	// $ANTLR end "finallyClause"

	public class alternative_return : TreeRuleReturnScope
	{
		public StringTemplate code;
	}

	// $ANTLR start "alternative"
	// Grammars\\CodeGenTreeWalker.g3:471:0: alternative returns [StringTemplate code] : ^(a= ALT (e= element[null,null] )+ EOA ) ;
	private CodeGenTreeWalker.alternative_return alternative(  )
	{
		CodeGenTreeWalker.alternative_return retval = new CodeGenTreeWalker.alternative_return();
		retval.start = input.LT(1);

		GrammarAST a=null;
		CodeGenTreeWalker.element_return e = default(CodeGenTreeWalker.element_return);


			if ( state.backtracking == 0 )
			{
				retval.code = templates.GetInstanceOf("alt");
				/*
				// TODO: can we use Rule.altsWithRewrites???
				if ( blockNestingLevel==RULE_BLOCK_NESTING_LEVEL ) {
					GrammarAST aRewriteNode = #alternative.findFirstType(REWRITE);
					if ( grammar.buildAST() &&
						 (aRewriteNode!=null||
						 (#alternative.getNextSibling()!=null &&
						  #alternative.getNextSibling().getType()==REWRITE)) )
					{
						currentAltHasASTRewrite = true;
					}
					else {
						currentAltHasASTRewrite = false;
					}
				}
				*/
				if ( blockNestingLevel==RULE_BLOCK_NESTING_LEVEL && grammar.BuildAST )
				{
					Rule r = grammar.getRule(currentRuleName);
					currentAltHasASTRewrite = r.hasRewrite(outerAltNum);
				}
				string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
				description = generator.target.getTargetStringLiteralFromString(description);
				retval.code.SetAttribute("description", description);
				retval.code.SetAttribute("treeLevel", rewriteTreeNestingLevel);
				if ( !currentAltHasASTRewrite && grammar.BuildAST )
				{
					retval.code.SetAttribute("autoAST", true);
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:508:4: ( ^(a= ALT (e= element[null,null] )+ EOA ) )
			// Grammars\\CodeGenTreeWalker.g3:508:4: ^(a= ALT (e= element[null,null] )+ EOA )
			{
			a=(GrammarAST)Match(input,ALT,Follow._ALT_in_alternative804); if (state.failed) return retval;

			Match(input, TokenConstants.Down, null); if (state.failed) return retval;
			// Grammars\\CodeGenTreeWalker.g3:509:4: (e= element[null,null] )+
			int cnt41=0;
			for ( ; ; )
			{
				int alt41=2;
				int LA41_0 = input.LA(1);

				if ( (LA41_0==ACTION||(LA41_0>=ASSIGN && LA41_0<=BLOCK)||(LA41_0>=CHAR_LITERAL && LA41_0<=CHAR_RANGE)||LA41_0==CLOSURE||LA41_0==DOT||LA41_0==EPSILON||LA41_0==FORCED_ACTION||LA41_0==GATED_SEMPRED||LA41_0==NOT||LA41_0==OPTIONAL||(LA41_0>=PLUS_ASSIGN && LA41_0<=POSITIVE_CLOSURE)||LA41_0==ROOT||LA41_0==RULE_REF||LA41_0==SEMPRED||(LA41_0>=STRING_LITERAL && LA41_0<=SYNPRED)||LA41_0==TOKEN_REF||LA41_0==TREE_BEGIN||LA41_0==WILDCARD) )
				{
					alt41=1;
				}


				switch ( alt41 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:510:5: e= element[null,null]
					{
					PushFollow(Follow._element_in_alternative817);
					e=element(null, null);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

											if ( e!=null )
											{
												retval.code.SetAttribute("elements.{el,line,pos}",
																  (e!=null?e.code:default(StringTemplate)),
																  (e!=null?((GrammarAST)e.start):null).Line,
																  (e!=null?((GrammarAST)e.start):null).CharPositionInLine
																 );
											}
										
					}

					}
					break;

				default:
					if ( cnt41 >= 1 )
						goto loop41;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee41 = new EarlyExitException( 41, input );
					throw eee41;
				}
				cnt41++;
			}
			loop41:
				;


			Match(input,EOA,Follow._EOA_in_alternative835); if (state.failed) return retval;

			Match(input, TokenConstants.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "alternative"

	public class element_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "element"
	// Grammars\\CodeGenTreeWalker.g3:526:0: element[GrammarAST label, GrammarAST astSuffix] returns [StringTemplate code=null] options {k=1; } : ( ^( ROOT e= element[label,$ROOT] ) | ^( BANG e= element[label,$BANG] ) | ^(n= NOT notElement[$n, $label, $astSuffix] ) | ^( ASSIGN alabel= ID e= element[$alabel,$astSuffix] ) | ^( PLUS_ASSIGN label2= ID e= element[$label2,$astSuffix] ) | ^( CHAR_RANGE a= CHAR_LITERAL b= CHAR_LITERAL ) |=> ebnf | atom[null, $label, $astSuffix] | tree_ | element_action | (sp= SEMPRED |sp= GATED_SEMPRED ) | SYN_SEMPRED | ^( SYNPRED ( . )* ) | ^( BACKTRACK_SEMPRED ( . )* ) | EPSILON );
	private CodeGenTreeWalker.element_return element( GrammarAST label, GrammarAST astSuffix )
	{
		CodeGenTreeWalker.element_return retval = new CodeGenTreeWalker.element_return();
		retval.start = input.LT(1);

		GrammarAST n=null;
		GrammarAST alabel=null;
		GrammarAST label2=null;
		GrammarAST a=null;
		GrammarAST b=null;
		GrammarAST sp=null;
		GrammarAST ROOT5=null;
		GrammarAST BANG6=null;
		CodeGenTreeWalker.element_return e = default(CodeGenTreeWalker.element_return);
		StringTemplate notElement7 = default(StringTemplate);
		CodeGenTreeWalker.ebnf_return ebnf8 = default(CodeGenTreeWalker.ebnf_return);
		CodeGenTreeWalker.atom_return atom9 = default(CodeGenTreeWalker.atom_return);
		CodeGenTreeWalker.tree__return tree_10 = default(CodeGenTreeWalker.tree__return);
		CodeGenTreeWalker.element_action_return element_action11 = default(CodeGenTreeWalker.element_action_return);


			IIntSet elements=null;
			GrammarAST ast = null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:533:4: ( ^( ROOT e= element[label,$ROOT] ) | ^( BANG e= element[label,$BANG] ) | ^(n= NOT notElement[$n, $label, $astSuffix] ) | ^( ASSIGN alabel= ID e= element[$alabel,$astSuffix] ) | ^( PLUS_ASSIGN label2= ID e= element[$label2,$astSuffix] ) | ^( CHAR_RANGE a= CHAR_LITERAL b= CHAR_LITERAL ) |=> ebnf | atom[null, $label, $astSuffix] | tree_ | element_action | (sp= SEMPRED |sp= GATED_SEMPRED ) | SYN_SEMPRED | ^( SYNPRED ( . )* ) | ^( BACKTRACK_SEMPRED ( . )* ) | EPSILON )
			int alt45=15;
			alt45 = dfa45.Predict(input);
			switch ( alt45 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:533:4: ^( ROOT e= element[label,$ROOT] )
				{
				ROOT5=(GrammarAST)Match(input,ROOT,Follow._ROOT_in_element870); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element874);
				e=element(label, ROOT5);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (e!=null?e.code:default(StringTemplate)); 
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:536:4: ^( BANG e= element[label,$BANG] )
				{
				BANG6=(GrammarAST)Match(input,BANG,Follow._BANG_in_element887); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element891);
				e=element(label, BANG6);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (e!=null?e.code:default(StringTemplate)); 
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:539:4: ^(n= NOT notElement[$n, $label, $astSuffix] )
				{
				n=(GrammarAST)Match(input,NOT,Follow._NOT_in_element907); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._notElement_in_element909);
				notElement7=notElement(n, label, astSuffix);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = notElement7; 
				}

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:542:4: ^( ASSIGN alabel= ID e= element[$alabel,$astSuffix] )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_element924); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				alabel=(GrammarAST)Match(input,ID,Follow._ID_in_element928); if (state.failed) return retval;
				PushFollow(Follow._element_in_element932);
				e=element(alabel, astSuffix);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (e!=null?e.code:default(StringTemplate)); 
				}

				}
				break;
			case 5:
				// Grammars\\CodeGenTreeWalker.g3:545:4: ^( PLUS_ASSIGN label2= ID e= element[$label2,$astSuffix] )
				{
				Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_element947); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				label2=(GrammarAST)Match(input,ID,Follow._ID_in_element951); if (state.failed) return retval;
				PushFollow(Follow._element_in_element955);
				e=element(label2, astSuffix);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (e!=null?e.code:default(StringTemplate)); 
				}

				}
				break;
			case 6:
				// Grammars\\CodeGenTreeWalker.g3:548:4: ^( CHAR_RANGE a= CHAR_LITERAL b= CHAR_LITERAL )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_element969); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				a=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_element973); if (state.failed) return retval;
				b=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_element977); if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								retval.code = templates.GetInstanceOf("charRangeRef");
								string low = generator.target.getTargetCharLiteralFromANTLRCharLiteral(generator,(a!=null?a.Text:null));
								string high = generator.target.getTargetCharLiteralFromANTLRCharLiteral(generator,(b!=null?b.Text:null));
								retval.code.SetAttribute("a", low);
								retval.code.SetAttribute("b", high);
								if ( label!=null )
								{
									retval.code.SetAttribute("label", label.Text);
								}
							
				}

				}
				break;
			case 7:
				// Grammars\\CodeGenTreeWalker.g3:561:4: => ebnf
				{

				PushFollow(Follow._ebnf_in_element1006);
				ebnf8=ebnf();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (ebnf8!=null?ebnf8.code:default(StringTemplate)); 
				}

				}
				break;
			case 8:
				// Grammars\\CodeGenTreeWalker.g3:564:4: atom[null, $label, $astSuffix]
				{
				PushFollow(Follow._atom_in_element1016);
				atom9=atom(null, label, astSuffix);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (atom9!=null?atom9.code:default(StringTemplate)); 
				}

				}
				break;
			case 9:
				// Grammars\\CodeGenTreeWalker.g3:567:4: tree_
				{
				PushFollow(Follow._tree__in_element1027);
				tree_10=tree_();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (tree_10!=null?tree_10.code:default(StringTemplate)); 
				}

				}
				break;
			case 10:
				// Grammars\\CodeGenTreeWalker.g3:570:4: element_action
				{
				PushFollow(Follow._element_action_in_element1037);
				element_action11=element_action();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (element_action11!=null?element_action11.code:default(StringTemplate)); 
				}

				}
				break;
			case 11:
				// Grammars\\CodeGenTreeWalker.g3:573:6: (sp= SEMPRED |sp= GATED_SEMPRED )
				{
				// Grammars\\CodeGenTreeWalker.g3:573:6: (sp= SEMPRED |sp= GATED_SEMPRED )
				int alt42=2;
				int LA42_0 = input.LA(1);

				if ( (LA42_0==SEMPRED) )
				{
					alt42=1;
				}
				else if ( (LA42_0==GATED_SEMPRED) )
				{
					alt42=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 42, 0, input);

					throw nvae;
				}
				switch ( alt42 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:573:7: sp= SEMPRED
					{
					sp=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_element1052); if (state.failed) return retval;

					}
					break;
				case 2:
					// Grammars\\CodeGenTreeWalker.g3:573:18: sp= GATED_SEMPRED
					{
					sp=(GrammarAST)Match(input,GATED_SEMPRED,Follow._GATED_SEMPRED_in_element1056); if (state.failed) return retval;

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

								retval.code = templates.GetInstanceOf("validateSemanticPredicate");
								retval.code.SetAttribute("pred", generator.translateAction(currentRuleName,sp));
								string description = generator.target.getTargetStringLiteralFromString((sp!=null?sp.Text:null));
								retval.code.SetAttribute("description", description);
							
				}

				}
				break;
			case 12:
				// Grammars\\CodeGenTreeWalker.g3:581:4: SYN_SEMPRED
				{
				Match(input,SYN_SEMPRED,Follow._SYN_SEMPRED_in_element1067); if (state.failed) return retval;

				}
				break;
			case 13:
				// Grammars\\CodeGenTreeWalker.g3:583:4: ^( SYNPRED ( . )* )
				{
				Match(input,SYNPRED,Follow._SYNPRED_in_element1075); if (state.failed) return retval;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:583:14: ( . )*
					for ( ; ; )
					{
						int alt43=2;
						int LA43_0 = input.LA(1);

						if ( ((LA43_0>=ACTION && LA43_0<=XDIGIT)) )
						{
							alt43=1;
						}
						else if ( (LA43_0==UP) )
						{
							alt43=2;
						}


						switch ( alt43 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:583:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop43;
						}
					}

					loop43:
						;



					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				}

				}
				break;
			case 14:
				// Grammars\\CodeGenTreeWalker.g3:585:4: ^( BACKTRACK_SEMPRED ( . )* )
				{
				Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_element1086); if (state.failed) return retval;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:585:24: ( . )*
					for ( ; ; )
					{
						int alt44=2;
						int LA44_0 = input.LA(1);

						if ( ((LA44_0>=ACTION && LA44_0<=XDIGIT)) )
						{
							alt44=1;
						}
						else if ( (LA44_0==UP) )
						{
							alt44=2;
						}


						switch ( alt44 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:585:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop44;
						}
					}

					loop44:
						;



					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				}

				}
				break;
			case 15:
				// Grammars\\CodeGenTreeWalker.g3:587:6: EPSILON
				{
				Match(input,EPSILON,Follow._EPSILON_in_element1098); if (state.failed) return retval;

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

	public class element_action_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "element_action"
	// Grammars\\CodeGenTreeWalker.g3:590:0: element_action returns [StringTemplate code=null] : (act= ACTION |act2= FORCED_ACTION );
	private CodeGenTreeWalker.element_action_return element_action(  )
	{
		CodeGenTreeWalker.element_action_return retval = new CodeGenTreeWalker.element_action_return();
		retval.start = input.LT(1);

		GrammarAST act=null;
		GrammarAST act2=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:591:4: (act= ACTION |act2= FORCED_ACTION )
			int alt46=2;
			int LA46_0 = input.LA(1);

			if ( (LA46_0==ACTION) )
			{
				alt46=1;
			}
			else if ( (LA46_0==FORCED_ACTION) )
			{
				alt46=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 46, 0, input);

				throw nvae;
			}
			switch ( alt46 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:591:4: act= ACTION
				{
				act=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_element_action1115); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								retval.code = templates.GetInstanceOf("execAction");
								retval.code.SetAttribute("action", generator.translateAction(currentRuleName,act));
							
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:596:4: act2= FORCED_ACTION
				{
				act2=(GrammarAST)Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_element_action1126); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								retval.code = templates.GetInstanceOf("execForcedAction");
								retval.code.SetAttribute("action", generator.translateAction(currentRuleName,act2));
							
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
	// $ANTLR end "element_action"


	// $ANTLR start "notElement"
	// Grammars\\CodeGenTreeWalker.g3:603:0: notElement[GrammarAST n, GrammarAST label, GrammarAST astSuffix] returns [StringTemplate code=null] : (assign_c= CHAR_LITERAL |assign_s= STRING_LITERAL |assign_t= TOKEN_REF | ^(assign_st= BLOCK ( . )* ) ) ;
	private StringTemplate notElement( GrammarAST n, GrammarAST label, GrammarAST astSuffix )
	{

		StringTemplate code = null;

		GrammarAST assign_c=null;
		GrammarAST assign_s=null;
		GrammarAST assign_t=null;
		GrammarAST assign_st=null;


			IIntSet elements=null;
			string labelText = null;
			if ( label!=null )
			{
				labelText = label.Text;
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:613:4: ( (assign_c= CHAR_LITERAL |assign_s= STRING_LITERAL |assign_t= TOKEN_REF | ^(assign_st= BLOCK ( . )* ) ) )
			// Grammars\\CodeGenTreeWalker.g3:613:4: (assign_c= CHAR_LITERAL |assign_s= STRING_LITERAL |assign_t= TOKEN_REF | ^(assign_st= BLOCK ( . )* ) )
			{
			// Grammars\\CodeGenTreeWalker.g3:613:4: (assign_c= CHAR_LITERAL |assign_s= STRING_LITERAL |assign_t= TOKEN_REF | ^(assign_st= BLOCK ( . )* ) )
			int alt48=4;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt48=1;
				}
				break;
			case STRING_LITERAL:
				{
				alt48=2;
				}
				break;
			case TOKEN_REF:
				{
				alt48=3;
				}
				break;
			case BLOCK:
				{
				alt48=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 48, 0, input);

					throw nvae;
				}
			}

			switch ( alt48 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:613:6: assign_c= CHAR_LITERAL
				{
				assign_c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_notElement1155); if (state.failed) return code;
				if ( state.backtracking == 0 )
				{

									int ttype=0;
									if ( grammar.type==Grammar.LEXER )
									{
										ttype = Grammar.getCharValueFromGrammarCharLiteral((assign_c!=null?assign_c.Text:null));
									}
									else
									{
										ttype = grammar.getTokenType((assign_c!=null?assign_c.Text:null));
									}
									elements = grammar.complement(ttype);
								
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:626:5: assign_s= STRING_LITERAL
				{
				assign_s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_notElement1168); if (state.failed) return code;
				if ( state.backtracking == 0 )
				{

									int ttype=0;
									if ( grammar.type==Grammar.LEXER )
									{
										// TODO: error!
									}
									else
									{
										ttype = grammar.getTokenType((assign_s!=null?assign_s.Text:null));
									}
									elements = grammar.complement(ttype);
								
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:639:5: assign_t= TOKEN_REF
				{
				assign_t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_notElement1181); if (state.failed) return code;
				if ( state.backtracking == 0 )
				{

									int ttype = grammar.getTokenType((assign_t!=null?assign_t.Text:null));
									elements = grammar.complement(ttype);
								
				}

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:644:5: ^(assign_st= BLOCK ( . )* )
				{
				assign_st=(GrammarAST)Match(input,BLOCK,Follow._BLOCK_in_notElement1195); if (state.failed) return code;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return code;
					// Grammars\\CodeGenTreeWalker.g3:644:23: ( . )*
					for ( ; ; )
					{
						int alt47=2;
						int LA47_0 = input.LA(1);

						if ( ((LA47_0>=ACTION && LA47_0<=XDIGIT)) )
						{
							alt47=1;
						}
						else if ( (LA47_0==UP) )
						{
							alt47=2;
						}


						switch ( alt47 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:644:0: .
							{
							MatchAny(input); if (state.failed) return code;

							}
							break;

						default:
							goto loop47;
						}
					}

					loop47:
						;



					Match(input, TokenConstants.Up, null); if (state.failed) return code;
				}
				if ( state.backtracking == 0 )
				{

									elements = assign_st.SetValue;
									elements = grammar.complement(elements);
								
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							code = getTokenElementST("matchSet",
													 "set",
													 (GrammarAST)n.GetChild(0),
													 astSuffix,
													 labelText);
							code.SetAttribute("s",generator.genSetExpr(templates,elements,1,false));
							int i = ((TokenWithIndex)n.Token).TokenIndex;
							code.SetAttribute("elementIndex", i);
							if ( grammar.type!=Grammar.LEXER )
							{
								generator.generateLocalFOLLOW(n,"set",currentRuleName,i);
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
		return code;
	}
	// $ANTLR end "notElement"

	public class ebnf_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "ebnf"
	// Grammars\\CodeGenTreeWalker.g3:666:0: ebnf returns [StringTemplate code=null] : (blk= block[\"block\", dfa] | ^( OPTIONAL blk= block[\"optionalBlock\", dfa] ) | ^( CLOSURE blk= block[\"closureBlock\", dfa] ) | ^( POSITIVE_CLOSURE blk= block[\"positiveClosureBlock\", dfa] ) ) ;
	private CodeGenTreeWalker.ebnf_return ebnf(  )
	{
		CodeGenTreeWalker.ebnf_return retval = new CodeGenTreeWalker.ebnf_return();
		retval.start = input.LT(1);

		CodeGenTreeWalker.block_return blk = default(CodeGenTreeWalker.block_return);


			Antlr3.Analysis.DFA dfa=null;
			GrammarAST b = (GrammarAST)((GrammarAST)retval.start).GetChild(0);
			GrammarAST eob = (GrammarAST)b.getLastChild(); // loops will use EOB DFA

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:673:4: ( (blk= block[\"block\", dfa] | ^( OPTIONAL blk= block[\"optionalBlock\", dfa] ) | ^( CLOSURE blk= block[\"closureBlock\", dfa] ) | ^( POSITIVE_CLOSURE blk= block[\"positiveClosureBlock\", dfa] ) ) )
			// Grammars\\CodeGenTreeWalker.g3:673:4: (blk= block[\"block\", dfa] | ^( OPTIONAL blk= block[\"optionalBlock\", dfa] ) | ^( CLOSURE blk= block[\"closureBlock\", dfa] ) | ^( POSITIVE_CLOSURE blk= block[\"positiveClosureBlock\", dfa] ) )
			{
			// Grammars\\CodeGenTreeWalker.g3:673:4: (blk= block[\"block\", dfa] | ^( OPTIONAL blk= block[\"optionalBlock\", dfa] ) | ^( CLOSURE blk= block[\"closureBlock\", dfa] ) | ^( POSITIVE_CLOSURE blk= block[\"positiveClosureBlock\", dfa] ) )
			int alt49=4;
			switch ( input.LA(1) )
			{
			case BLOCK:
				{
				alt49=1;
				}
				break;
			case OPTIONAL:
				{
				alt49=2;
				}
				break;
			case CLOSURE:
				{
				alt49=3;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt49=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 49, 0, input);

					throw nvae;
				}
			}

			switch ( alt49 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:673:6: blk= block[\"block\", dfa]
				{
				if ( state.backtracking == 0 )
				{
					 dfa = ((GrammarAST)retval.start).LookaheadDFA; 
				}
				PushFollow(Follow._block_in_ebnf1241);
				blk=block("block", dfa);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (blk!=null?blk.code:default(StringTemplate)); 
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:676:5: ^( OPTIONAL blk= block[\"optionalBlock\", dfa] )
				{
				if ( state.backtracking == 0 )
				{
					 dfa = ((GrammarAST)retval.start).LookaheadDFA; 
				}
				Match(input,OPTIONAL,Follow._OPTIONAL_in_ebnf1260); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf1264);
				blk=block("optionalBlock", dfa);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (blk!=null?blk.code:default(StringTemplate)); 
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:679:5: ^( CLOSURE blk= block[\"closureBlock\", dfa] )
				{
				if ( state.backtracking == 0 )
				{
					 dfa = eob.LookaheadDFA; 
				}
				Match(input,CLOSURE,Follow._CLOSURE_in_ebnf1285); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf1289);
				blk=block("closureBlock", dfa);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (blk!=null?blk.code:default(StringTemplate)); 
				}

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:682:5: ^( POSITIVE_CLOSURE blk= block[\"positiveClosureBlock\", dfa] )
				{
				if ( state.backtracking == 0 )
				{
					 dfa = eob.LookaheadDFA; 
				}
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_ebnf1310); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_ebnf1314);
				blk=block("positiveClosureBlock", dfa);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (blk!=null?blk.code:default(StringTemplate)); 
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
							description = generator.target.getTargetStringLiteralFromString(description);
							retval.code.SetAttribute("description", description);
						
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
	// $ANTLR end "ebnf"

	public class tree__return : TreeRuleReturnScope
	{
		public StringTemplate code;
	}

	// $ANTLR start "tree_"
	// Grammars\\CodeGenTreeWalker.g3:693:0: tree_ returns [StringTemplate code] : ^( TREE_BEGIN el= element[null,rootSuffix] (=>act= element_action )* (el= element[null,null] )* ) ;
	private CodeGenTreeWalker.tree__return tree_(  )
	{
		CodeGenTreeWalker.tree__return retval = new CodeGenTreeWalker.tree__return();
		retval.start = input.LT(1);

		CodeGenTreeWalker.element_return el = default(CodeGenTreeWalker.element_return);
		CodeGenTreeWalker.element_action_return act = default(CodeGenTreeWalker.element_action_return);


			rewriteTreeNestingLevel++;
			GrammarAST rootSuffix = null;
			if ( state.backtracking == 0 )
			{
				retval.code = templates.GetInstanceOf("tree");
				NFAState afterDOWN = (NFAState)((GrammarAST)retval.start).NFATreeDownState.getTransition(0).target;
				LookaheadSet s = grammar.LOOK(afterDOWN);
				if ( s.member(Label.UP) ) {
					// nullable child list if we can see the UP as the next token
					// we need an "if ( input.LA(1)==Token.DOWN )" gate around
					// the child list.
					retval.code.SetAttribute("nullableChildList", "true");
				}
				retval.code.SetAttribute("enclosingTreeLevel", rewriteTreeNestingLevel-1);
				retval.code.SetAttribute("treeLevel", rewriteTreeNestingLevel);
				Rule r = grammar.getRule(currentRuleName);
				if ( grammar.BuildAST && !r.hasRewrite(outerAltNum) ) {
					rootSuffix = new GrammarAST(ROOT,"ROOT");
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:717:4: ( ^( TREE_BEGIN el= element[null,rootSuffix] (=>act= element_action )* (el= element[null,null] )* ) )
			// Grammars\\CodeGenTreeWalker.g3:717:4: ^( TREE_BEGIN el= element[null,rootSuffix] (=>act= element_action )* (el= element[null,null] )* )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_1352); if (state.failed) return retval;

			Match(input, TokenConstants.Down, null); if (state.failed) return retval;
			PushFollow(Follow._element_in_tree_1359);
			el=element(null, rootSuffix);

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								retval.code.SetAttribute("root.{el,line,pos}",
												  (el!=null?el.code:default(StringTemplate)),
												  (el!=null?((GrammarAST)el.start):null).Line,
												  (el!=null?((GrammarAST)el.start):null).CharPositionInLine
												  );
							
			}
			// Grammars\\CodeGenTreeWalker.g3:729:4: (=>act= element_action )*
			for ( ; ; )
			{
				int alt50=2;
				int LA50_0 = input.LA(1);

				if ( (LA50_0==ACTION) )
				{
					int LA50_2 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_CodeGenTreeWalker_fragment)) )
					{
						alt50=1;
					}


				}
				else if ( (LA50_0==FORCED_ACTION) )
				{
					int LA50_3 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_CodeGenTreeWalker_fragment)) )
					{
						alt50=1;
					}


				}


				switch ( alt50 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:729:6: =>act= element_action
					{

					PushFollow(Follow._element_action_in_tree_1396);
					act=element_action();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

											retval.code.SetAttribute("actionsAfterRoot.{el,line,pos}",
															  (act!=null?act.code:default(StringTemplate)),
															  (act!=null?((GrammarAST)act.start):null).Line,
															  (act!=null?((GrammarAST)act.start):null).CharPositionInLine
															);
										
					}

					}
					break;

				default:
					goto loop50;
				}
			}

			loop50:
				;


			// Grammars\\CodeGenTreeWalker.g3:739:4: (el= element[null,null] )*
			for ( ; ; )
			{
				int alt51=2;
				int LA51_0 = input.LA(1);

				if ( (LA51_0==ACTION||(LA51_0>=ASSIGN && LA51_0<=BLOCK)||(LA51_0>=CHAR_LITERAL && LA51_0<=CHAR_RANGE)||LA51_0==CLOSURE||LA51_0==DOT||LA51_0==EPSILON||LA51_0==FORCED_ACTION||LA51_0==GATED_SEMPRED||LA51_0==NOT||LA51_0==OPTIONAL||(LA51_0>=PLUS_ASSIGN && LA51_0<=POSITIVE_CLOSURE)||LA51_0==ROOT||LA51_0==RULE_REF||LA51_0==SEMPRED||(LA51_0>=STRING_LITERAL && LA51_0<=SYNPRED)||LA51_0==TOKEN_REF||LA51_0==TREE_BEGIN||LA51_0==WILDCARD) )
				{
					alt51=1;
				}


				switch ( alt51 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:739:7: el= element[null,null]
					{
					PushFollow(Follow._element_in_tree_1418);
					el=element(null, null);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

										 retval.code.SetAttribute("children.{el,line,pos}",
														  (el!=null?el.code:default(StringTemplate)),
														  (el!=null?((GrammarAST)el.start):null).Line,
														  (el!=null?((GrammarAST)el.start):null).CharPositionInLine
														  );
										 
					}

					}
					break;

				default:
					goto loop51;
				}
			}

			loop51:
				;



			Match(input, TokenConstants.Up, null); if (state.failed) return retval;

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			 rewriteTreeNestingLevel--; 
		}
		return retval;
	}
	// $ANTLR end "tree_"

	public class atom_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "atom"
	// Grammars\\CodeGenTreeWalker.g3:752:0: atom[GrammarAST scope, GrammarAST label, GrammarAST astSuffix] returns [StringTemplate code=null] : ( ^(r= RULE_REF (rarg= ARG_ACTION )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL |w= WILDCARD | ^( DOT ID a= atom[$ID, label, astSuffix] ) | set[label,astSuffix] );
	private CodeGenTreeWalker.atom_return atom( GrammarAST scope, GrammarAST label, GrammarAST astSuffix )
	{
		CodeGenTreeWalker.atom_return retval = new CodeGenTreeWalker.atom_return();
		retval.start = input.LT(1);

		GrammarAST r=null;
		GrammarAST rarg=null;
		GrammarAST t=null;
		GrammarAST targ=null;
		GrammarAST c=null;
		GrammarAST s=null;
		GrammarAST w=null;
		GrammarAST ID12=null;
		CodeGenTreeWalker.atom_return a = default(CodeGenTreeWalker.atom_return);
		StringTemplate set13 = default(StringTemplate);


			string labelText=null;
			if ( state.backtracking == 0 )
			{
				if ( label!=null )
				{
					labelText = label.Text;
				}
				if ( grammar.type!=Grammar.LEXER &&
					 (((GrammarAST)retval.start).Type==RULE_REF||((GrammarAST)retval.start).Type==TOKEN_REF||
					  ((GrammarAST)retval.start).Type==CHAR_LITERAL||((GrammarAST)retval.start).Type==STRING_LITERAL) )
				{
					Rule encRule = grammar.getRule(((GrammarAST)((GrammarAST)retval.start)).enclosingRuleName);
					if ( encRule!=null && encRule.hasRewrite(outerAltNum) && astSuffix!=null )
					{
						ErrorManager.grammarError(ErrorManager.MSG_AST_OP_IN_ALT_WITH_REWRITE,
												  grammar,
												  ((GrammarAST)((GrammarAST)retval.start)).Token,
												  ((GrammarAST)((GrammarAST)retval.start)).enclosingRuleName,
												  outerAltNum);
						astSuffix = null;
					}
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:780:6: ( ^(r= RULE_REF (rarg= ARG_ACTION )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL |w= WILDCARD | ^( DOT ID a= atom[$ID, label, astSuffix] ) | set[label,astSuffix] )
			int alt54=7;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt54=1;
				}
				break;
			case TOKEN_REF:
				{
				alt54=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt54=3;
				}
				break;
			case STRING_LITERAL:
				{
				alt54=4;
				}
				break;
			case WILDCARD:
				{
				alt54=5;
				}
				break;
			case DOT:
				{
				alt54=6;
				}
				break;
			case BLOCK:
				{
				alt54=7;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 54, 0, input);

					throw nvae;
				}
			}

			switch ( alt54 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:780:6: ^(r= RULE_REF (rarg= ARG_ACTION )? )
				{
				r=(GrammarAST)Match(input,RULE_REF,Follow._RULE_REF_in_atom1469); if (state.failed) return retval;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:780:20: (rarg= ARG_ACTION )?
					int alt52=2;
					int LA52_0 = input.LA(1);

					if ( (LA52_0==ARG_ACTION) )
					{
						alt52=1;
					}
					switch ( alt52 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:780:21: rarg= ARG_ACTION
						{
						rarg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1474); if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								grammar.checkRuleReference(scope, r, rarg, currentRuleName);
								string scopeName = null;
								if ( scope!=null ) {
									scopeName = scope.Text;
								}
								Rule rdef = grammar.getRule(scopeName, (r!=null?r.Text:null));
								// don't insert label=r() if label.attr not used, no ret value, ...
								if ( !rdef.HasReturnValue ) {
									labelText = null;
								}
								retval.code = getRuleElementST("ruleRef", (r!=null?r.Text:null), r, astSuffix, labelText);
								retval.code.SetAttribute("rule", rdef);
								if ( scope!=null ) { // scoped rule ref
									Grammar scopeG = grammar.composite.getGrammar(scope.Text);
									retval.code.SetAttribute("scope", scopeG);
								}
								else if ( rdef.grammar != this.grammar ) { // nonlocal
									// if rule definition is not in this grammar, it's nonlocal
									IList<Grammar> rdefDelegates = rdef.grammar.getDelegates();
									if ( rdefDelegates.Contains(this.grammar) ) {
										retval.code.SetAttribute("scope", rdef.grammar);
									}
									else {
										// defining grammar is not a delegate, scope all the
										// back to root, which has delegate methods for all
										// rules.  Don't use scope if we are root.
										if ( this.grammar != rdef.grammar.composite.delegateGrammarTreeRoot.grammar ) {
											retval.code.SetAttribute("scope",
															  rdef.grammar.composite.delegateGrammarTreeRoot.grammar);
										}
									}
								}

								if ( rarg!=null ) {
									List args = generator.translateAction(currentRuleName,rarg);
									retval.code.SetAttribute("args", args);
								}
								int i = ((TokenWithIndex)r.Token).TokenIndex;
								retval.code.SetAttribute("elementIndex", i);
								generator.generateLocalFOLLOW(r,(r!=null?r.Text:null),currentRuleName,i);
								r.code = retval.code;
							
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:825:4: ^(t= TOKEN_REF (targ= ARG_ACTION )? )
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom1492); if (state.failed) return retval;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:825:19: (targ= ARG_ACTION )?
					int alt53=2;
					int LA53_0 = input.LA(1);

					if ( (LA53_0==ARG_ACTION) )
					{
						alt53=1;
					}
					switch ( alt53 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:825:20: targ= ARG_ACTION
						{
						targ=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1497); if (state.failed) return retval;

						}
						break;

					}


					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				}
				if ( state.backtracking == 0 )
				{

								if ( currentAltHasASTRewrite && t.terminalOptions!=null &&
									t.terminalOptions[Grammar.defaultTokenOption]!=null )
								{
									ErrorManager.grammarError(ErrorManager.MSG_HETERO_ILLEGAL_IN_REWRITE_ALT,
															grammar,
															((GrammarAST)(t)).Token,
															(t!=null?t.Text:null));
								}
								grammar.checkRuleReference(scope, t, targ, currentRuleName);
								if ( grammar.type==Grammar.LEXER )
								{
									if ( grammar.getTokenType((t!=null?t.Text:null))==Label.EOF )
									{
										retval.code = templates.GetInstanceOf("lexerMatchEOF");
									}
									else
									{
										retval.code = templates.GetInstanceOf("lexerRuleRef");
										if ( isListLabel(labelText) )
										{
											retval.code = templates.GetInstanceOf("lexerRuleRefAndListLabel");
										}
										string scopeName = null;
										if ( scope!=null )
										{
											scopeName = scope.Text;
										}
										Rule rdef2 = grammar.getRule(scopeName, (t!=null?t.Text:null));
										retval.code.SetAttribute("rule", rdef2);
										if ( scope!=null )
										{ // scoped rule ref
											Grammar scopeG = grammar.composite.getGrammar(scope.Text);
											retval.code.SetAttribute("scope", scopeG);
										}
										else if ( rdef2.grammar != this.grammar )
										{ // nonlocal
											// if rule definition is not in this grammar, it's nonlocal
											retval.code.SetAttribute("scope", rdef2.grammar);
										}
										if ( targ!=null )
										{
											List args = generator.translateAction(currentRuleName,targ);
											retval.code.SetAttribute("args", args);
										}
									}
									int i = ((TokenWithIndex)t.Token).TokenIndex;
									retval.code.SetAttribute("elementIndex", i);
									if ( label!=null )
										retval.code.SetAttribute("label", labelText);
								}
								else
								{
									retval.code = getTokenElementST("tokenRef", (t!=null?t.Text:null), t, astSuffix, labelText);
									string tokenLabel =
										generator.getTokenTypeAsTargetLabel(grammar.getTokenType(t.Text));
									retval.code.SetAttribute("token",tokenLabel);
									if ( !currentAltHasASTRewrite && t.terminalOptions!=null )
									{ 
										retval.code.SetAttribute("hetero",t.terminalOptions[Grammar.defaultTokenOption]);
									}
									int i = ((TokenWithIndex)t.Token).TokenIndex;
									retval.code.SetAttribute("elementIndex", i);
									generator.generateLocalFOLLOW(t,tokenLabel,currentRuleName,i);
								}
								t.code = retval.code;
							
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:894:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom1513); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==Grammar.LEXER )
								{
									retval.code = templates.GetInstanceOf("charRef");
									retval.code.SetAttribute("char",
									   generator.target.getTargetCharLiteralFromANTLRCharLiteral(generator,(c!=null?c.Text:null)));
									if ( label!=null )
									{
										retval.code.SetAttribute("label", labelText);
									}
								}
								else { // else it's a token type reference
									retval.code = getTokenElementST("tokenRef", "char_literal", c, astSuffix, labelText);
									string tokenLabel = generator.getTokenTypeAsTargetLabel(grammar.getTokenType((c!=null?c.Text:null)));
									retval.code.SetAttribute("token",tokenLabel);
									if ( c.terminalOptions!=null ) {
										retval.code.SetAttribute("hetero",c.terminalOptions[Grammar.defaultTokenOption]);
									}
									int i = ((TokenWithIndex)c.Token).TokenIndex;
									retval.code.SetAttribute("elementIndex", i);
									generator.generateLocalFOLLOW(c,tokenLabel,currentRuleName,i);
								}
							
				}

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:919:4: s= STRING_LITERAL
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_atom1526); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type==Grammar.LEXER )
								{
									retval.code = templates.GetInstanceOf("lexerStringRef");
									retval.code.SetAttribute("string",
										generator.target.getTargetStringLiteralFromANTLRStringLiteral(generator,(s!=null?s.Text:null)));
									if ( label!=null )
									{
										retval.code.SetAttribute("label", labelText);
									}
								}
								else
								{ // else it's a token type reference
									retval.code = getTokenElementST("tokenRef", "string_literal", s, astSuffix, labelText);
									string tokenLabel =
										generator.getTokenTypeAsTargetLabel(grammar.getTokenType((s!=null?s.Text:null)));
									retval.code.SetAttribute("token",tokenLabel);
									if ( s.terminalOptions!=null )
									{
										retval.code.SetAttribute("hetero",s.terminalOptions[Grammar.defaultTokenOption]);
									}
									int i = ((TokenWithIndex)s.Token).TokenIndex;
									retval.code.SetAttribute("elementIndex", i);
									generator.generateLocalFOLLOW(s,tokenLabel,currentRuleName,i);
								}
							
				}

				}
				break;
			case 5:
				// Grammars\\CodeGenTreeWalker.g3:947:4: w= WILDCARD
				{
				w=(GrammarAST)Match(input,WILDCARD,Follow._WILDCARD_in_atom1538); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								retval.code = getWildcardST(w,astSuffix,labelText);
								retval.code.SetAttribute("elementIndex", ((TokenWithIndex)w.Token).TokenIndex);
							
				}

				}
				break;
			case 6:
				// Grammars\\CodeGenTreeWalker.g3:953:4: ^( DOT ID a= atom[$ID, label, astSuffix] )
				{
				Match(input,DOT,Follow._DOT_in_atom1549); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				ID12=(GrammarAST)Match(input,ID,Follow._ID_in_atom1551); if (state.failed) return retval;
				PushFollow(Follow._atom_in_atom1555);
				a=atom(ID12, label, astSuffix);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (a!=null?a.code:default(StringTemplate)); 
				}

				}
				break;
			case 7:
				// Grammars\\CodeGenTreeWalker.g3:956:4: set[label,astSuffix]
				{
				PushFollow(Follow._set_in_atom1568);
				set13=set(label, astSuffix);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = set13; 
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
	// $ANTLR end "atom"


	// $ANTLR start "ast_suffix"
	// Grammars\\CodeGenTreeWalker.g3:960:0: ast_suffix : ( ROOT | BANG );
	private void ast_suffix(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:961:4: ( ROOT | BANG )
			// Grammars\\CodeGenTreeWalker.g3:
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


	// $ANTLR start "set"
	// Grammars\\CodeGenTreeWalker.g3:965:0: set[GrammarAST label, GrammarAST astSuffix] returns [StringTemplate code=null] : ^(s= BLOCK ( . )* ) ;
	private StringTemplate set( GrammarAST label, GrammarAST astSuffix )
	{

		StringTemplate code = null;

		GrammarAST s=null;


			string labelText=null;
			if ( label!=null )
			{
				labelText = label.Text;
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:974:4: ( ^(s= BLOCK ( . )* ) )
			// Grammars\\CodeGenTreeWalker.g3:974:4: ^(s= BLOCK ( . )* )
			{
			s=(GrammarAST)Match(input,BLOCK,Follow._BLOCK_in_set1613); if (state.failed) return code;

			if ( input.LA(1)==TokenConstants.Down )
			{
				Match(input, TokenConstants.Down, null); if (state.failed) return code;
				// Grammars\\CodeGenTreeWalker.g3:974:14: ( . )*
				for ( ; ; )
				{
					int alt55=2;
					int LA55_0 = input.LA(1);

					if ( ((LA55_0>=ACTION && LA55_0<=XDIGIT)) )
					{
						alt55=1;
					}
					else if ( (LA55_0==UP) )
					{
						alt55=2;
					}


					switch ( alt55 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:974:0: .
						{
						MatchAny(input); if (state.failed) return code;

						}
						break;

					default:
						goto loop55;
					}
				}

				loop55:
					;



				Match(input, TokenConstants.Up, null); if (state.failed) return code;
			}
			if ( state.backtracking == 0 )
			{

							code = getTokenElementST("matchSet", "set", s, astSuffix, labelText);
							int i = ((TokenWithIndex)s.Token).TokenIndex;
							code.SetAttribute("elementIndex", i);
							if ( grammar.type!=Grammar.LEXER )
							{
								generator.generateLocalFOLLOW(s,"set",currentRuleName,i);
							}
							code.SetAttribute("s", generator.genSetExpr(templates,s.SetValue,1,false));
						
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
		return code;
	}
	// $ANTLR end "set"


	// $ANTLR start "setElement"
	// Grammars\\CodeGenTreeWalker.g3:987:0: setElement : ( CHAR_LITERAL | TOKEN_REF | STRING_LITERAL | ^( CHAR_RANGE CHAR_LITERAL CHAR_LITERAL ) );
	private void setElement(  )
	{
		try
		{
			// Grammars\\CodeGenTreeWalker.g3:988:4: ( CHAR_LITERAL | TOKEN_REF | STRING_LITERAL | ^( CHAR_RANGE CHAR_LITERAL CHAR_LITERAL ) )
			int alt56=4;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt56=1;
				}
				break;
			case TOKEN_REF:
				{
				alt56=2;
				}
				break;
			case STRING_LITERAL:
				{
				alt56=3;
				}
				break;
			case CHAR_RANGE:
				{
				alt56=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 56, 0, input);

					throw nvae;
				}
			}

			switch ( alt56 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:988:4: CHAR_LITERAL
				{
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1633); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:989:4: TOKEN_REF
				{
				Match(input,TOKEN_REF,Follow._TOKEN_REF_in_setElement1638); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:990:4: STRING_LITERAL
				{
				Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_setElement1643); if (state.failed) return ;

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:991:4: ^( CHAR_RANGE CHAR_LITERAL CHAR_LITERAL )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_setElement1649); if (state.failed) return ;

				Match(input, TokenConstants.Down, null); if (state.failed) return ;
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1651); if (state.failed) return ;
				Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_setElement1653); if (state.failed) return ;

				Match(input, TokenConstants.Up, null); if (state.failed) return ;

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

	public class rewrite_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "rewrite"
	// Grammars\\CodeGenTreeWalker.g3:996:0: rewrite returns [StringTemplate code=null] : ( ^(r= REWRITE (pred= SEMPRED )? alt= rewrite_alternative ) )* ;
	private CodeGenTreeWalker.rewrite_return rewrite(  )
	{
		CodeGenTreeWalker.rewrite_return retval = new CodeGenTreeWalker.rewrite_return();
		retval.start = input.LT(1);

		GrammarAST r=null;
		GrammarAST pred=null;
		StringTemplate alt = default(StringTemplate);


			if ( state.backtracking == 0 )
			{
				if ( ((GrammarAST)retval.start).Type==REWRITE )
				{
					if ( generator.grammar.BuildTemplate )
					{
						retval.code = templates.GetInstanceOf("rewriteTemplate");
					}
					else
					{
						retval.code = templates.GetInstanceOf("rewriteCode");
						retval.code.SetAttribute("treeLevel", OUTER_REWRITE_NESTING_LEVEL);
						retval.code.SetAttribute("rewriteBlockLevel", OUTER_REWRITE_NESTING_LEVEL);
						retval.code.SetAttribute("referencedElementsDeep",
										  getTokenTypesAsTargetLabels(((GrammarAST)retval.start).rewriteRefsDeep));
						HashSet<string> tokenLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.TOKEN_LABEL);
						HashSet<string> tokenListLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.TOKEN_LIST_LABEL);
						HashSet<string> ruleLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.RULE_LABEL);
						HashSet<string> ruleListLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.RULE_LIST_LABEL);
						HashSet<string> wildcardLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.WILDCARD_TREE_LABEL);
						HashSet<string> wildcardListLabels =
							grammar.getLabels(((GrammarAST)retval.start).rewriteRefsDeep, Grammar.WILDCARD_TREE_LIST_LABEL);
						// just in case they ref r for "previous value", make a stream
						// from retval.tree
						StringTemplate retvalST = templates.GetInstanceOf("prevRuleRootRef");
						ruleLabels.Add(retvalST.ToString());
						retval.code.SetAttribute("referencedTokenLabels", tokenLabels);
						retval.code.SetAttribute("referencedTokenListLabels", tokenListLabels);
						retval.code.SetAttribute("referencedRuleLabels", ruleLabels);
						retval.code.SetAttribute("referencedRuleListLabels", ruleListLabels);
						retval.code.SetAttribute("referencedWildcardLabels", wildcardLabels);
						retval.code.SetAttribute("referencedWildcardListLabels", wildcardListLabels);
					}
				}
				else
				{
						retval.code = templates.GetInstanceOf("noRewrite");
						retval.code.SetAttribute("treeLevel", OUTER_REWRITE_NESTING_LEVEL);
						retval.code.SetAttribute("rewriteBlockLevel", OUTER_REWRITE_NESTING_LEVEL);
				}
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1046:4: ( ( ^(r= REWRITE (pred= SEMPRED )? alt= rewrite_alternative ) )* )
			// Grammars\\CodeGenTreeWalker.g3:1046:4: ( ^(r= REWRITE (pred= SEMPRED )? alt= rewrite_alternative ) )*
			{
			// Grammars\\CodeGenTreeWalker.g3:1046:4: ( ^(r= REWRITE (pred= SEMPRED )? alt= rewrite_alternative ) )*
			for ( ; ; )
			{
				int alt58=2;
				int LA58_0 = input.LA(1);

				if ( (LA58_0==REWRITE) )
				{
					alt58=1;
				}


				switch ( alt58 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1047:4: ^(r= REWRITE (pred= SEMPRED )? alt= rewrite_alternative )
					{
					if ( state.backtracking == 0 )
					{
						rewriteRuleRefs = new HashSet<object>();
					}
					r=(GrammarAST)Match(input,REWRITE,Follow._REWRITE_in_rewrite1690); if (state.failed) return retval;

					Match(input, TokenConstants.Down, null); if (state.failed) return retval;
					// Grammars\\CodeGenTreeWalker.g3:1048:17: (pred= SEMPRED )?
					int alt57=2;
					int LA57_0 = input.LA(1);

					if ( (LA57_0==SEMPRED) )
					{
						alt57=1;
					}
					switch ( alt57 )
					{
					case 1:
						// Grammars\\CodeGenTreeWalker.g3:1048:18: pred= SEMPRED
						{
						pred=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_rewrite1695); if (state.failed) return retval;

						}
						break;

					}

					PushFollow(Follow._rewrite_alternative_in_rewrite1701);
					alt=rewrite_alternative();

					state._fsp--;
					if (state.failed) return retval;

					Match(input, TokenConstants.Up, null); if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

										rewriteBlockNestingLevel = OUTER_REWRITE_NESTING_LEVEL;
										List predChunks = null;
										if ( pred!=null )
										{
											//predText = #pred.getText();
											predChunks = generator.translateAction(currentRuleName,pred);
										}
										string description =
											grammar.grammarTreeToString(r,false);
										description = generator.target.getTargetStringLiteralFromString(description);
										retval.code.SetAttribute("alts.{pred,alt,description}",
														  predChunks,
														  alt,
														  description);
										pred=null;
									
					}

					}
					break;

				default:
					goto loop58;
				}
			}

			loop58:
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


	// $ANTLR start "rewrite_block"
	// Grammars\\CodeGenTreeWalker.g3:1069:0: rewrite_block[string blockTemplateName] returns [StringTemplate code=null] : ^( BLOCK alt= rewrite_alternative EOB ) ;
	private StringTemplate rewrite_block( string blockTemplateName )
	{

		StringTemplate code = null;

		GrammarAST BLOCK14=null;
		StringTemplate alt = default(StringTemplate);


			rewriteBlockNestingLevel++;
			StringTemplate save_currentBlockST = currentBlockST;
			if ( state.backtracking == 0 )
			{
				code = templates.GetInstanceOf(blockTemplateName);
				currentBlockST = code;
				code.SetAttribute("rewriteBlockLevel", rewriteBlockNestingLevel);
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1081:4: ( ^( BLOCK alt= rewrite_alternative EOB ) )
			// Grammars\\CodeGenTreeWalker.g3:1081:4: ^( BLOCK alt= rewrite_alternative EOB )
			{
			BLOCK14=(GrammarAST)Match(input,BLOCK,Follow._BLOCK_in_rewrite_block1736); if (state.failed) return code;

			if ( state.backtracking == 0 )
			{

								currentBlockST.SetAttribute("referencedElementsDeep",
									getTokenTypesAsTargetLabels(BLOCK14.rewriteRefsDeep));
								currentBlockST.SetAttribute("referencedElements",
									getTokenTypesAsTargetLabels(BLOCK14.rewriteRefsShallow));
							
			}

			Match(input, TokenConstants.Down, null); if (state.failed) return code;
			PushFollow(Follow._rewrite_alternative_in_rewrite_block1748);
			alt=rewrite_alternative();

			state._fsp--;
			if (state.failed) return code;
			Match(input,EOB,Follow._EOB_in_rewrite_block1753); if (state.failed) return code;

			Match(input, TokenConstants.Up, null); if (state.failed) return code;
			if ( state.backtracking == 0 )
			{

							code.SetAttribute("alt", alt);
						
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
			 rewriteBlockNestingLevel--; currentBlockST = save_currentBlockST; 
		}
		return code;
	}
	// $ANTLR end "rewrite_block"


	// $ANTLR start "rewrite_alternative"
	// Grammars\\CodeGenTreeWalker.g3:1097:0: rewrite_alternative returns [StringTemplate code=null] : ({...}? ^(a= ALT ( (el= rewrite_element )+ | EPSILON ) EOA ) |{...}? rewrite_template | ETC );
	private StringTemplate rewrite_alternative(  )
	{

		StringTemplate code = null;

		GrammarAST a=null;
		CodeGenTreeWalker.rewrite_element_return el = default(CodeGenTreeWalker.rewrite_element_return);
		StringTemplate rewrite_template15 = default(StringTemplate);

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1098:4: ({...}? ^(a= ALT ( (el= rewrite_element )+ | EPSILON ) EOA ) |{...}? rewrite_template | ETC )
			int alt61=3;
			switch ( input.LA(1) )
			{
			case ALT:
				{
				int LA61_1 = input.LA(2);

				if ( (LA61_1==DOWN) )
				{
					int LA61_4 = input.LA(3);

					if ( (LA61_4==EPSILON) )
					{
						int LA61_5 = input.LA(4);

						if ( (LA61_5==EOA) )
						{
							int LA61_7 = input.LA(5);

							if ( (LA61_7==UP) )
							{
								int LA61_8 = input.LA(6);

								if ( ((generator.grammar.BuildAST)) )
								{
									alt61=1;
								}
								else if ( ((generator.grammar.BuildTemplate)) )
								{
									alt61=2;
								}
								else
								{
									if (state.backtracking>0) {state.failed=true; return code;}
									NoViableAltException nvae = new NoViableAltException("", 61, 8, input);

									throw nvae;
								}
							}
							else
							{
								if (state.backtracking>0) {state.failed=true; return code;}
								NoViableAltException nvae = new NoViableAltException("", 61, 7, input);

								throw nvae;
							}
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return code;}
							NoViableAltException nvae = new NoViableAltException("", 61, 5, input);

							throw nvae;
						}
					}
					else if ( (LA61_4==ACTION||LA61_4==CHAR_LITERAL||LA61_4==CLOSURE||LA61_4==LABEL||LA61_4==OPTIONAL||LA61_4==POSITIVE_CLOSURE||LA61_4==RULE_REF||LA61_4==STRING_LITERAL||LA61_4==TOKEN_REF||LA61_4==TREE_BEGIN) )
					{
						alt61=1;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return code;}
						NoViableAltException nvae = new NoViableAltException("", 61, 4, input);

						throw nvae;
					}
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 61, 1, input);

					throw nvae;
				}
				}
				break;
			case ACTION:
			case TEMPLATE:
				{
				alt61=2;
				}
				break;
			case ETC:
				{
				alt61=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 61, 0, input);

					throw nvae;
				}
			}

			switch ( alt61 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:1098:4: {...}? ^(a= ALT ( (el= rewrite_element )+ | EPSILON ) EOA )
				{
				if ( !((generator.grammar.BuildAST)) )
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					throw new FailedPredicateException(input, "rewrite_alternative", "generator.grammar.BuildAST");
				}
				a=(GrammarAST)Match(input,ALT,Follow._ALT_in_rewrite_alternative1788); if (state.failed) return code;

				if ( state.backtracking == 0 )
				{
					code =templates.GetInstanceOf("rewriteElementList");
				}

				Match(input, TokenConstants.Down, null); if (state.failed) return code;
				// Grammars\\CodeGenTreeWalker.g3:1100:4: ( (el= rewrite_element )+ | EPSILON )
				int alt60=2;
				int LA60_0 = input.LA(1);

				if ( (LA60_0==ACTION||LA60_0==CHAR_LITERAL||LA60_0==CLOSURE||LA60_0==LABEL||LA60_0==OPTIONAL||LA60_0==POSITIVE_CLOSURE||LA60_0==RULE_REF||LA60_0==STRING_LITERAL||LA60_0==TOKEN_REF||LA60_0==TREE_BEGIN) )
				{
					alt60=1;
				}
				else if ( (LA60_0==EPSILON) )
				{
					alt60=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 60, 0, input);

					throw nvae;
				}
				switch ( alt60 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1100:6: (el= rewrite_element )+
					{
					// Grammars\\CodeGenTreeWalker.g3:1100:6: (el= rewrite_element )+
					int cnt59=0;
					for ( ; ; )
					{
						int alt59=2;
						int LA59_0 = input.LA(1);

						if ( (LA59_0==ACTION||LA59_0==CHAR_LITERAL||LA59_0==CLOSURE||LA59_0==LABEL||LA59_0==OPTIONAL||LA59_0==POSITIVE_CLOSURE||LA59_0==RULE_REF||LA59_0==STRING_LITERAL||LA59_0==TOKEN_REF||LA59_0==TREE_BEGIN) )
						{
							alt59=1;
						}


						switch ( alt59 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:1101:6: el= rewrite_element
							{
							PushFollow(Follow._rewrite_element_in_rewrite_alternative1806);
							el=rewrite_element();

							state._fsp--;
							if (state.failed) return code;
							if ( state.backtracking == 0 )
							{
								code.SetAttribute("elements.{el,line,pos}",
																		(el!=null?el.code:default(StringTemplate)),
																		(el!=null?((GrammarAST)el.start):null).Line,
																		(el!=null?((GrammarAST)el.start):null).CharPositionInLine
																		);
													
							}

							}
							break;

						default:
							if ( cnt59 >= 1 )
								goto loop59;

							if (state.backtracking>0) {state.failed=true; return code;}
							EarlyExitException eee59 = new EarlyExitException( 59, input );
							throw eee59;
						}
						cnt59++;
					}
					loop59:
						;



					}
					break;
				case 2:
					// Grammars\\CodeGenTreeWalker.g3:1109:6: EPSILON
					{
					Match(input,EPSILON,Follow._EPSILON_in_rewrite_alternative1827); if (state.failed) return code;
					if ( state.backtracking == 0 )
					{
						code.SetAttribute("elements.{el,line,pos}",
														   templates.GetInstanceOf("rewriteEmptyAlt"),
														   a.Line,
														   a.CharPositionInLine
														   );
										
					}

					}
					break;

				}

				Match(input,EOA,Follow._EOA_in_rewrite_alternative1843); if (state.failed) return code;

				Match(input, TokenConstants.Up, null); if (state.failed) return code;

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:1120:4: {...}? rewrite_template
				{
				if ( !((generator.grammar.BuildTemplate)) )
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					throw new FailedPredicateException(input, "rewrite_alternative", "generator.grammar.BuildTemplate");
				}
				PushFollow(Follow._rewrite_template_in_rewrite_alternative1856);
				rewrite_template15=rewrite_template();

				state._fsp--;
				if (state.failed) return code;
				if ( state.backtracking == 0 )
				{
					 code = rewrite_template15; 
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:1124:3: ETC
				{
				Match(input,ETC,Follow._ETC_in_rewrite_alternative1869); if (state.failed) return code;

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
		return code;
	}
	// $ANTLR end "rewrite_alternative"

	public class rewrite_element_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "rewrite_element"
	// Grammars\\CodeGenTreeWalker.g3:1127:0: rewrite_element returns [StringTemplate code=null] : ( rewrite_atom[false] | rewrite_ebnf | rewrite_tree );
	private CodeGenTreeWalker.rewrite_element_return rewrite_element(  )
	{
		CodeGenTreeWalker.rewrite_element_return retval = new CodeGenTreeWalker.rewrite_element_return();
		retval.start = input.LT(1);

		CodeGenTreeWalker.rewrite_atom_return rewrite_atom16 = default(CodeGenTreeWalker.rewrite_atom_return);
		CodeGenTreeWalker.rewrite_ebnf_return rewrite_ebnf17 = default(CodeGenTreeWalker.rewrite_ebnf_return);
		CodeGenTreeWalker.rewrite_tree_return rewrite_tree18 = default(CodeGenTreeWalker.rewrite_tree_return);


			IIntSet elements=null;
			GrammarAST ast = null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1133:4: ( rewrite_atom[false] | rewrite_ebnf | rewrite_tree )
			int alt62=3;
			switch ( input.LA(1) )
			{
			case ACTION:
			case CHAR_LITERAL:
			case LABEL:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
				{
				alt62=1;
				}
				break;
			case CLOSURE:
			case OPTIONAL:
			case POSITIVE_CLOSURE:
				{
				alt62=2;
				}
				break;
			case TREE_BEGIN:
				{
				alt62=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 62, 0, input);

					throw nvae;
				}
			}

			switch ( alt62 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:1133:4: rewrite_atom[false]
				{
				PushFollow(Follow._rewrite_atom_in_rewrite_element1889);
				rewrite_atom16=rewrite_atom(false);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (rewrite_atom16!=null?rewrite_atom16.code:default(StringTemplate)); 
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:1135:4: rewrite_ebnf
				{
				PushFollow(Follow._rewrite_ebnf_in_rewrite_element1899);
				rewrite_ebnf17=rewrite_ebnf();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (rewrite_ebnf17!=null?rewrite_ebnf17.code:default(StringTemplate)); 
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:1137:4: rewrite_tree
				{
				PushFollow(Follow._rewrite_tree_in_rewrite_element1908);
				rewrite_tree18=rewrite_tree();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = (rewrite_tree18!=null?rewrite_tree18.code:default(StringTemplate)); 
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
	// $ANTLR end "rewrite_element"

	public class rewrite_ebnf_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "rewrite_ebnf"
	// Grammars\\CodeGenTreeWalker.g3:1141:0: rewrite_ebnf returns [StringTemplate code=null] : ( ^( OPTIONAL rewrite_block[\"rewriteOptionalBlock\"] ) | ^( CLOSURE rewrite_block[\"rewriteClosureBlock\"] ) | ^( POSITIVE_CLOSURE rewrite_block[\"rewritePositiveClosureBlock\"] ) );
	private CodeGenTreeWalker.rewrite_ebnf_return rewrite_ebnf(  )
	{
		CodeGenTreeWalker.rewrite_ebnf_return retval = new CodeGenTreeWalker.rewrite_ebnf_return();
		retval.start = input.LT(1);

		StringTemplate rewrite_block19 = default(StringTemplate);
		StringTemplate rewrite_block20 = default(StringTemplate);
		StringTemplate rewrite_block21 = default(StringTemplate);

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1142:4: ( ^( OPTIONAL rewrite_block[\"rewriteOptionalBlock\"] ) | ^( CLOSURE rewrite_block[\"rewriteClosureBlock\"] ) | ^( POSITIVE_CLOSURE rewrite_block[\"rewritePositiveClosureBlock\"] ) )
			int alt63=3;
			switch ( input.LA(1) )
			{
			case OPTIONAL:
				{
				alt63=1;
				}
				break;
			case CLOSURE:
				{
				alt63=2;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt63=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 63, 0, input);

					throw nvae;
				}
			}

			switch ( alt63 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:1142:4: ^( OPTIONAL rewrite_block[\"rewriteOptionalBlock\"] )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_rewrite_ebnf1929); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1931);
				rewrite_block19=rewrite_block("rewriteOptionalBlock");

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = rewrite_block19; 
				}
				if ( state.backtracking == 0 )
				{

								string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
								description = generator.target.getTargetStringLiteralFromString(description);
								retval.code.SetAttribute("description", description);
							
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:1149:4: ^( CLOSURE rewrite_block[\"rewriteClosureBlock\"] )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_rewrite_ebnf1949); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1951);
				rewrite_block20=rewrite_block("rewriteClosureBlock");

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = rewrite_block20; 
				}
				if ( state.backtracking == 0 )
				{

								string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
								description = generator.target.getTargetStringLiteralFromString(description);
								retval.code.SetAttribute("description", description);
							
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:1156:4: ^( POSITIVE_CLOSURE rewrite_block[\"rewritePositiveClosureBlock\"] )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_rewrite_ebnf1969); if (state.failed) return retval;

				Match(input, TokenConstants.Down, null); if (state.failed) return retval;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1971);
				rewrite_block21=rewrite_block("rewritePositiveClosureBlock");

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenConstants.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{
					 retval.code = rewrite_block21; 
				}
				if ( state.backtracking == 0 )
				{

								string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
								description = generator.target.getTargetStringLiteralFromString(description);
								retval.code.SetAttribute("description", description);
							
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
	// $ANTLR end "rewrite_ebnf"

	public class rewrite_tree_return : TreeRuleReturnScope
	{
		public StringTemplate code;
	}

	// $ANTLR start "rewrite_tree"
	// Grammars\\CodeGenTreeWalker.g3:1165:0: rewrite_tree returns [StringTemplate code] : ^( TREE_BEGIN r= rewrite_atom[true] (el= rewrite_element )* ) ;
	private CodeGenTreeWalker.rewrite_tree_return rewrite_tree(  )
	{
		CodeGenTreeWalker.rewrite_tree_return retval = new CodeGenTreeWalker.rewrite_tree_return();
		retval.start = input.LT(1);

		CodeGenTreeWalker.rewrite_atom_return r = default(CodeGenTreeWalker.rewrite_atom_return);
		CodeGenTreeWalker.rewrite_element_return el = default(CodeGenTreeWalker.rewrite_element_return);


			rewriteTreeNestingLevel++;
			if ( state.backtracking == 0 )
			{
				retval.code = templates.GetInstanceOf("rewriteTree");
				retval.code.SetAttribute("treeLevel", rewriteTreeNestingLevel);
				retval.code.SetAttribute("enclosingTreeLevel", rewriteTreeNestingLevel-1);
			}

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1176:4: ( ^( TREE_BEGIN r= rewrite_atom[true] (el= rewrite_element )* ) )
			// Grammars\\CodeGenTreeWalker.g3:1176:4: ^( TREE_BEGIN r= rewrite_atom[true] (el= rewrite_element )* )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_rewrite_tree2004); if (state.failed) return retval;

			Match(input, TokenConstants.Down, null); if (state.failed) return retval;
			PushFollow(Follow._rewrite_atom_in_rewrite_tree2011);
			r=rewrite_atom(true);

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								retval.code.SetAttribute("root.{el,line,pos}",
												   (r!=null?r.code:default(StringTemplate)),
												   (r!=null?((GrammarAST)r.start):null).Line,
												   (r!=null?((GrammarAST)r.start):null).CharPositionInLine
												  );
							
			}
			// Grammars\\CodeGenTreeWalker.g3:1185:4: (el= rewrite_element )*
			for ( ; ; )
			{
				int alt64=2;
				int LA64_0 = input.LA(1);

				if ( (LA64_0==ACTION||LA64_0==CHAR_LITERAL||LA64_0==CLOSURE||LA64_0==LABEL||LA64_0==OPTIONAL||LA64_0==POSITIVE_CLOSURE||LA64_0==RULE_REF||LA64_0==STRING_LITERAL||LA64_0==TOKEN_REF||LA64_0==TREE_BEGIN) )
				{
					alt64=1;
				}


				switch ( alt64 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1186:6: el= rewrite_element
					{
					PushFollow(Follow._rewrite_element_in_rewrite_tree2031);
					el=rewrite_element();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

										retval.code.SetAttribute("children.{el,line,pos}",
															(el!=null?el.code:default(StringTemplate)),
															(el!=null?((GrammarAST)el.start):null).Line,
															(el!=null?((GrammarAST)el.start):null).CharPositionInLine
															);
									  
					}

					}
					break;

				default:
					goto loop64;
				}
			}

			loop64:
				;



			Match(input, TokenConstants.Up, null); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

							string description = grammar.grammarTreeToString(((GrammarAST)retval.start), false);
							description = generator.target.getTargetStringLiteralFromString(description);
							retval.code.SetAttribute("description", description);
						
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
			 rewriteTreeNestingLevel--; 
		}
		return retval;
	}
	// $ANTLR end "rewrite_tree"

	public class rewrite_atom_return : TreeRuleReturnScope
	{
		public StringTemplate code=null;
	}

	// $ANTLR start "rewrite_atom"
	// Grammars\\CodeGenTreeWalker.g3:1204:0: rewrite_atom[bool isRoot] returns [StringTemplate code=null] : (r= RULE_REF | ( ^(tk= TOKEN_REF (arg= ARG_ACTION )? ) |cl= CHAR_LITERAL |sl= STRING_LITERAL ) | LABEL | ACTION );
	private CodeGenTreeWalker.rewrite_atom_return rewrite_atom( bool isRoot )
	{
		CodeGenTreeWalker.rewrite_atom_return retval = new CodeGenTreeWalker.rewrite_atom_return();
		retval.start = input.LT(1);

		GrammarAST r=null;
		GrammarAST tk=null;
		GrammarAST arg=null;
		GrammarAST cl=null;
		GrammarAST sl=null;
		GrammarAST LABEL22=null;
		GrammarAST ACTION23=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1205:6: (r= RULE_REF | ( ^(tk= TOKEN_REF (arg= ARG_ACTION )? ) |cl= CHAR_LITERAL |sl= STRING_LITERAL ) | LABEL | ACTION )
			int alt67=4;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt67=1;
				}
				break;
			case CHAR_LITERAL:
			case STRING_LITERAL:
			case TOKEN_REF:
				{
				alt67=2;
				}
				break;
			case LABEL:
				{
				alt67=3;
				}
				break;
			case ACTION:
				{
				alt67=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 67, 0, input);

					throw nvae;
				}
			}

			switch ( alt67 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:1205:6: r= RULE_REF
				{
				r=(GrammarAST)Match(input,RULE_REF,Follow._RULE_REF_in_rewrite_atom2076); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								string ruleRefName = (r!=null?r.Text:null);
								string stName = "rewriteRuleRef";
								if ( isRoot )
								{
									stName += "Root";
								}
								retval.code = templates.GetInstanceOf(stName);
								retval.code.SetAttribute("rule", ruleRefName);
								if ( grammar.getRule(ruleRefName)==null )
								{
									ErrorManager.grammarError(ErrorManager.MSG_UNDEFINED_RULE_REF,
															  grammar,
															  ((GrammarAST)(r)).Token,
															  ruleRefName);
									retval.code = new StringTemplate(); // blank; no code gen
								}
								else if ( grammar.getRule(currentRuleName)
											 .getRuleRefsInAlt(ruleRefName,outerAltNum)==null )
								{
									ErrorManager.grammarError(ErrorManager.MSG_REWRITE_ELEMENT_NOT_PRESENT_ON_LHS,
															  grammar,
															  ((GrammarAST)(r)).Token,
															  ruleRefName);
									retval.code = new StringTemplate(); // blank; no code gen
								}
								else
								{
									// track all rule refs as we must copy 2nd ref to rule and beyond
									if ( !rewriteRuleRefs.Contains(ruleRefName) )
									{
										rewriteRuleRefs.Add(ruleRefName);
									}
								}
							
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:1243:3: ( ^(tk= TOKEN_REF (arg= ARG_ACTION )? ) |cl= CHAR_LITERAL |sl= STRING_LITERAL )
				{
				// Grammars\\CodeGenTreeWalker.g3:1243:3: ( ^(tk= TOKEN_REF (arg= ARG_ACTION )? ) |cl= CHAR_LITERAL |sl= STRING_LITERAL )
				int alt66=3;
				switch ( input.LA(1) )
				{
				case TOKEN_REF:
					{
					alt66=1;
					}
					break;
				case CHAR_LITERAL:
					{
					alt66=2;
					}
					break;
				case STRING_LITERAL:
					{
					alt66=3;
					}
					break;
				default:
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 66, 0, input);

						throw nvae;
					}
				}

				switch ( alt66 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1243:5: ^(tk= TOKEN_REF (arg= ARG_ACTION )? )
					{
					tk=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_rewrite_atom2093); if (state.failed) return retval;

					if ( input.LA(1)==TokenConstants.Down )
					{
						Match(input, TokenConstants.Down, null); if (state.failed) return retval;
						// Grammars\\CodeGenTreeWalker.g3:1243:20: (arg= ARG_ACTION )?
						int alt65=2;
						int LA65_0 = input.LA(1);

						if ( (LA65_0==ARG_ACTION) )
						{
							alt65=1;
						}
						switch ( alt65 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:1243:21: arg= ARG_ACTION
							{
							arg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rewrite_atom2098); if (state.failed) return retval;

							}
							break;

						}


						Match(input, TokenConstants.Up, null); if (state.failed) return retval;
					}

					}
					break;
				case 2:
					// Grammars\\CodeGenTreeWalker.g3:1244:5: cl= CHAR_LITERAL
					{
					cl=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_rewrite_atom2109); if (state.failed) return retval;

					}
					break;
				case 3:
					// Grammars\\CodeGenTreeWalker.g3:1245:5: sl= STRING_LITERAL
					{
					sl=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_rewrite_atom2117); if (state.failed) return retval;

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

								GrammarAST term = (tk) ?? (cl) ?? (sl);
								string tokenName = ((GrammarAST)retval.start).Token.Text;
								string stName = "rewriteTokenRef";
								Rule rule = grammar.getRule(currentRuleName);
								ICollection<string> tokenRefsInAlt = rule.getTokenRefsInAlt(outerAltNum);
								bool createNewNode = !tokenRefsInAlt.Contains(tokenName) || arg!=null;
								object hetero = null;
								if ( term.terminalOptions!=null )
								{
									hetero = term.terminalOptions[Grammar.defaultTokenOption];
								}
								if ( createNewNode )
								{
									stName = "rewriteImaginaryTokenRef";
								}
								if ( isRoot )
								{
									stName += "Root";
								}
								retval.code = templates.GetInstanceOf(stName);
								retval.code.SetAttribute("hetero", hetero);
								if ( arg!=null )
								{
									List args = generator.translateAction(currentRuleName,arg);
									retval.code.SetAttribute("args", args);
								}
								retval.code.SetAttribute("elementIndex", ((TokenWithIndex)((GrammarAST)retval.start).Token).TokenIndex);
								int ttype = grammar.getTokenType(tokenName);
								string tok = generator.getTokenTypeAsTargetLabel(ttype);
								retval.code.SetAttribute("token", tok);
								if ( grammar.getTokenType(tokenName)==Label.INVALID )
								{
									ErrorManager.grammarError(ErrorManager.MSG_UNDEFINED_TOKEN_REF_IN_REWRITE,
															  grammar,
															  ((GrammarAST)(((GrammarAST)retval.start))).Token,
															  tokenName);
									retval.code = new StringTemplate(); // blank; no code gen
								}
							
				}

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:1288:4: LABEL
				{
				LABEL22=(GrammarAST)Match(input,LABEL,Follow._LABEL_in_rewrite_atom2131); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								string labelName = (LABEL22!=null?LABEL22.Text:null);
								Rule rule = grammar.getRule(currentRuleName);
								Grammar.LabelElementPair pair = rule.getLabel(labelName);
								if ( labelName.Equals(currentRuleName) )
								{
									// special case; ref to old value via $ rule
									if ( rule.hasRewrite(outerAltNum) &&
										 rule.getRuleRefsInAlt(outerAltNum).Contains(labelName) )
									{
										ErrorManager.grammarError(ErrorManager.MSG_RULE_REF_AMBIG_WITH_RULE_IN_ALT,
																  grammar,
																  ((GrammarAST)(LABEL22)).Token,
																  labelName);
									}
									StringTemplate labelST = templates.GetInstanceOf("prevRuleRootRef");
									retval.code = templates.GetInstanceOf("rewriteRuleLabelRef"+(isRoot?"Root":""));
									retval.code.SetAttribute("label", labelST);
								}
								else if ( pair==null )
								{
									ErrorManager.grammarError(ErrorManager.MSG_UNDEFINED_LABEL_REF_IN_REWRITE,
															  grammar,
															  ((GrammarAST)(LABEL22)).Token,
															  labelName);
									retval.code = new StringTemplate();
								}
								else
								{
									string stName = null;
									switch ( pair.type )
									{
									case Grammar.TOKEN_LABEL :
										stName = "rewriteTokenLabelRef";
										break;
									case Grammar.WILDCARD_TREE_LABEL :
										stName = "rewriteWildcardLabelRef";
										break;
									case Grammar.WILDCARD_TREE_LIST_LABEL:
										stName = "rewriteRuleListLabelRef"; // acts like rule ref list for ref
										break;
									case Grammar.RULE_LABEL :
										stName = "rewriteRuleLabelRef";
										break;
									case Grammar.TOKEN_LIST_LABEL :
										stName = "rewriteTokenListLabelRef";
										break;
									case Grammar.RULE_LIST_LABEL :
										stName = "rewriteRuleListLabelRef";
										break;
									}
									if ( isRoot )
									{
										stName += "Root";
									}
									retval.code = templates.GetInstanceOf(stName);
									retval.code.SetAttribute("label", labelName);
								}
							
				}

				}
				break;
			case 4:
				// Grammars\\CodeGenTreeWalker.g3:1349:4: ACTION
				{
				ACTION23=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_atom2141); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								// actions in rewrite rules yield a tree object
								string actText = (ACTION23!=null?ACTION23.Text:null);
								List chunks = generator.translateAction(currentRuleName,ACTION23);
								retval.code = templates.GetInstanceOf("rewriteNodeAction"+(isRoot?"Root":""));
								retval.code.SetAttribute("action", chunks);
							
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
	// $ANTLR end "rewrite_atom"


	// $ANTLR start "rewrite_template"
	// Grammars\\CodeGenTreeWalker.g3:1360:0: public rewrite_template returns [StringTemplate code=null] : ( ^( ALT EPSILON EOA ) | ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) |act= ACTION );
	public StringTemplate rewrite_template(  )
	{

		StringTemplate code = null;

		GrammarAST id=null;
		GrammarAST ind=null;
		GrammarAST arg=null;
		GrammarAST a=null;
		GrammarAST act=null;
		GrammarAST DOUBLE_QUOTE_STRING_LITERAL24=null;
		GrammarAST DOUBLE_ANGLE_STRING_LITERAL25=null;

		try
		{
			// Grammars\\CodeGenTreeWalker.g3:1361:4: ( ^( ALT EPSILON EOA ) | ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) |act= ACTION )
			int alt71=3;
			switch ( input.LA(1) )
			{
			case ALT:
				{
				alt71=1;
				}
				break;
			case TEMPLATE:
				{
				alt71=2;
				}
				break;
			case ACTION:
				{
				alt71=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 71, 0, input);

					throw nvae;
				}
			}

			switch ( alt71 )
			{
			case 1:
				// Grammars\\CodeGenTreeWalker.g3:1361:4: ^( ALT EPSILON EOA )
				{
				Match(input,ALT,Follow._ALT_in_rewrite_template2164); if (state.failed) return code;

				Match(input, TokenConstants.Down, null); if (state.failed) return code;
				Match(input,EPSILON,Follow._EPSILON_in_rewrite_template2166); if (state.failed) return code;
				Match(input,EOA,Follow._EOA_in_rewrite_template2168); if (state.failed) return code;

				Match(input, TokenConstants.Up, null); if (state.failed) return code;
				if ( state.backtracking == 0 )
				{
					code =templates.GetInstanceOf("rewriteEmptyTemplate");
				}

				}
				break;
			case 2:
				// Grammars\\CodeGenTreeWalker.g3:1362:4: ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? )
				{
				Match(input,TEMPLATE,Follow._TEMPLATE_in_rewrite_template2179); if (state.failed) return code;

				Match(input, TokenConstants.Down, null); if (state.failed) return code;
				// Grammars\\CodeGenTreeWalker.g3:1362:16: (id= ID |ind= ACTION )
				int alt68=2;
				int LA68_0 = input.LA(1);

				if ( (LA68_0==ID) )
				{
					alt68=1;
				}
				else if ( (LA68_0==ACTION) )
				{
					alt68=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return code;}
					NoViableAltException nvae = new NoViableAltException("", 68, 0, input);

					throw nvae;
				}
				switch ( alt68 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1362:17: id= ID
					{
					id=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template2184); if (state.failed) return code;

					}
					break;
				case 2:
					// Grammars\\CodeGenTreeWalker.g3:1362:23: ind= ACTION
					{
					ind=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template2188); if (state.failed) return code;

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

									if ( id!=null && (id!=null?id.Text:null).Equals("template") )
									{
											code = templates.GetInstanceOf("rewriteInlineTemplate");
									}
									else if ( id!=null )
									{
											code = templates.GetInstanceOf("rewriteExternalTemplate");
											code.SetAttribute("name", (id!=null?id.Text:null));
									}
									else if ( ind!=null )
									{ // must be %({expr})(args)
										code = templates.GetInstanceOf("rewriteIndirectTemplate");
										List chunks=generator.translateAction(currentRuleName,ind);
										code.SetAttribute("expr", chunks);
									}
								
				}
				Match(input,ARGLIST,Follow._ARGLIST_in_rewrite_template2201); if (state.failed) return code;

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); if (state.failed) return code;
					// Grammars\\CodeGenTreeWalker.g3:1381:5: ( ^( ARG arg= ID a= ACTION ) )*
					for ( ; ; )
					{
						int alt69=2;
						int LA69_0 = input.LA(1);

						if ( (LA69_0==ARG) )
						{
							alt69=1;
						}


						switch ( alt69 )
						{
						case 1:
							// Grammars\\CodeGenTreeWalker.g3:1381:7: ^( ARG arg= ID a= ACTION )
							{
							Match(input,ARG,Follow._ARG_in_rewrite_template2211); if (state.failed) return code;

							Match(input, TokenConstants.Down, null); if (state.failed) return code;
							arg=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template2215); if (state.failed) return code;
							a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template2219); if (state.failed) return code;
							if ( state.backtracking == 0 )
							{

														// must set alt num here rather than in define.g
														// because actions like %foo(name={$ID.text}) aren't
														// broken up yet into trees.
														a.outerAltNum = this.outerAltNum;
														List chunks = generator.translateAction(currentRuleName,a);
														code.SetAttribute("args.{name,value}", (arg!=null?arg.Text:null), chunks);
													
							}

							Match(input, TokenConstants.Up, null); if (state.failed) return code;

							}
							break;

						default:
							goto loop69;
						}
					}

					loop69:
						;



					Match(input, TokenConstants.Up, null); if (state.failed) return code;
				}
				// Grammars\\CodeGenTreeWalker.g3:1393:4: ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )?
				int alt70=3;
				int LA70_0 = input.LA(1);

				if ( (LA70_0==DOUBLE_QUOTE_STRING_LITERAL) )
				{
					alt70=1;
				}
				else if ( (LA70_0==DOUBLE_ANGLE_STRING_LITERAL) )
				{
					alt70=2;
				}
				switch ( alt70 )
				{
				case 1:
					// Grammars\\CodeGenTreeWalker.g3:1393:6: DOUBLE_QUOTE_STRING_LITERAL
					{
					DOUBLE_QUOTE_STRING_LITERAL24=(GrammarAST)Match(input,DOUBLE_QUOTE_STRING_LITERAL,Follow._DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template2252); if (state.failed) return code;
					if ( state.backtracking == 0 )
					{

											string sl = (DOUBLE_QUOTE_STRING_LITERAL24!=null?DOUBLE_QUOTE_STRING_LITERAL24.Text:null);
											string t = sl.Substring( 1, sl.Length - 2 ); // strip quotes
											t = generator.target.getTargetStringLiteralFromString(t);
											code.SetAttribute("template",t);
										
					}

					}
					break;
				case 2:
					// Grammars\\CodeGenTreeWalker.g3:1400:6: DOUBLE_ANGLE_STRING_LITERAL
					{
					DOUBLE_ANGLE_STRING_LITERAL25=(GrammarAST)Match(input,DOUBLE_ANGLE_STRING_LITERAL,Follow._DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template2265); if (state.failed) return code;
					if ( state.backtracking == 0 )
					{

											string sl = (DOUBLE_ANGLE_STRING_LITERAL25!=null?DOUBLE_ANGLE_STRING_LITERAL25.Text:null);
											string t = sl.Substring( 2, sl.Length - 4 ); // strip double angle quotes
											t = generator.target.getTargetStringLiteralFromString(t);
											code.SetAttribute("template",t);
										
					}

					}
					break;

				}


				Match(input, TokenConstants.Up, null); if (state.failed) return code;

				}
				break;
			case 3:
				// Grammars\\CodeGenTreeWalker.g3:1410:4: act= ACTION
				{
				act=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template2289); if (state.failed) return code;
				if ( state.backtracking == 0 )
				{

								// set alt num for same reason as ARGLIST above
								act.outerAltNum = this.outerAltNum;
								code =templates.GetInstanceOf("rewriteAction");
								code.SetAttribute("action",
												  generator.translateAction(currentRuleName,act));
							
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
		return code;
	}
	// $ANTLR end "rewrite_template"

	// $ANTLR start synpred1_CodeGenTreeWalker
	public void synpred1_CodeGenTreeWalker_fragment()
	{
		// Grammars\\CodeGenTreeWalker.g3:561:4: ({...}? ( BLOCK | OPTIONAL | CLOSURE | POSITIVE_CLOSURE ) )
		// Grammars\\CodeGenTreeWalker.g3:561:5: {...}? ( BLOCK | OPTIONAL | CLOSURE | POSITIVE_CLOSURE )
		{
		if ( !((((GrammarAST)input.LT(1)).SetValue==null)) )
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			throw new FailedPredicateException(input, "synpred1_CodeGenTreeWalker", "((GrammarAST)input.LT(1)).SetValue==null");
		}
		if ( input.LA(1)==BLOCK||input.LA(1)==CLOSURE||input.LA(1)==OPTIONAL||input.LA(1)==POSITIVE_CLOSURE )
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
	// $ANTLR end synpred1_CodeGenTreeWalker

	// $ANTLR start synpred2_CodeGenTreeWalker
	public void synpred2_CodeGenTreeWalker_fragment()
	{
		// Grammars\\CodeGenTreeWalker.g3:729:6: ( element_action )
		// Grammars\\CodeGenTreeWalker.g3:729:7: element_action
		{
		PushFollow(Follow._element_action_in_synpred2_CodeGenTreeWalker1385);
		element_action();

		state._fsp--;
		if (state.failed) return ;

		}
	}
	// $ANTLR end synpred2_CodeGenTreeWalker
	#endregion Rules

	#region Synpreds
	bool EvaluatePredicate( System.Action fragment )
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			fragment();
		}
		catch ( RecognitionException re )
		{
			System.Console.Error.WriteLine("impossible: "+re);
		}
		bool success = !state.failed;
		input.Rewind(start);
		state.backtracking--;
		state.failed=false;
		return success;
	}
	#endregion Synpreds


	#region DFA
	DFA45 dfa45;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa45 = new DFA45( this, new SpecialStateTransitionHandler( specialStateTransition45 ) );
	}

	class DFA45 : DFA
	{

		const string DFA45_eotS =
			"\x1A\xFFFF";
		const string DFA45_eofS =
			"\x1A\xFFFF";
		const string DFA45_minS =
			"\x1\x4\x6\xFFFF\x1\x0\x12\xFFFF";
		const string DFA45_maxS =
			"\x1\x5F\x6\xFFFF\x1\x0\x12\xFFFF";
		const string DFA45_acceptS =
			"\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\xFFFF\x3\x7\x1\x8\x5"+
			"\xFFFF\x1\x9\x1\xA\x1\xFFFF\x1\xB\x1\xFFFF\x1\xC\x1\xD\x1\xE\x1\xF";
		const string DFA45_specialS =
			"\x1\x0\x6\xFFFF\x1\x1\x12\xFFFF}>";
		static readonly string[] DFA45_transitionS =
			{
				"\x1\x12\x8\xFFFF\x1\x4\x1\x18\x1\x2\x1\x7\x1\xFFFF\x1\xB\x1\x6\x1\xFFFF"+
				"\x1\x9\x7\xFFFF\x1\xB\x5\xFFFF\x1\x19\x3\xFFFF\x1\x12\x1\xFFFF\x1\x14"+
				"\xD\xFFFF\x1\x3\x1\xFFFF\x1\x8\x5\xFFFF\x1\x5\x1\xA\x9\xFFFF\x1\x1\x2"+
				"\xFFFF\x1\xB\x2\xFFFF\x1\x14\x4\xFFFF\x1\xB\x1\x16\x1\x17\x2\xFFFF\x1"+
				"\xB\x2\xFFFF\x1\x11\x1\xFFFF\x1\xB",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xFFFF",
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
				"",
				""
			};

		static readonly short[] DFA45_eot = DFA.UnpackEncodedString(DFA45_eotS);
		static readonly short[] DFA45_eof = DFA.UnpackEncodedString(DFA45_eofS);
		static readonly char[] DFA45_min = DFA.UnpackEncodedStringToUnsignedChars(DFA45_minS);
		static readonly char[] DFA45_max = DFA.UnpackEncodedStringToUnsignedChars(DFA45_maxS);
		static readonly short[] DFA45_accept = DFA.UnpackEncodedString(DFA45_acceptS);
		static readonly short[] DFA45_special = DFA.UnpackEncodedString(DFA45_specialS);
		static readonly short[][] DFA45_transition;

		static DFA45()
		{
			int numStates = DFA45_transitionS.Length;
			DFA45_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA45_transition[i] = DFA.UnpackEncodedString(DFA45_transitionS[i]);
			}
		}

		public DFA45( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 45;
			this.eot = DFA45_eot;
			this.eof = DFA45_eof;
			this.min = DFA45_min;
			this.max = DFA45_max;
			this.accept = DFA45_accept;
			this.special = DFA45_special;
			this.transition = DFA45_transition;
		}
		public override string GetDescription()
		{
			return "526:0: element[GrammarAST label, GrammarAST astSuffix] returns [StringTemplate code=null] options {k=1; } : ( ^( ROOT e= element[label,$ROOT] ) | ^( BANG e= element[label,$BANG] ) | ^(n= NOT notElement[$n, $label, $astSuffix] ) | ^( ASSIGN alabel= ID e= element[$alabel,$astSuffix] ) | ^( PLUS_ASSIGN label2= ID e= element[$label2,$astSuffix] ) | ^( CHAR_RANGE a= CHAR_LITERAL b= CHAR_LITERAL ) |=> ebnf | atom[null, $label, $astSuffix] | tree_ | element_action | (sp= SEMPRED |sp= GATED_SEMPRED ) | SYN_SEMPRED | ^( SYNPRED ( . )* ) | ^( BACKTRACK_SEMPRED ( . )* ) | EPSILON );";
		}
	}

	int specialStateTransition45( DFA dfa, int s, IIntStream _input )
	{
		ITreeNodeStream input = (ITreeNodeStream)_input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA45_0 = input.LA(1);


				int index45_0 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA45_0==ROOT) ) {s = 1;}

				else if ( (LA45_0==BANG) ) {s = 2;}

				else if ( (LA45_0==NOT) ) {s = 3;}

				else if ( (LA45_0==ASSIGN) ) {s = 4;}

				else if ( (LA45_0==PLUS_ASSIGN) ) {s = 5;}

				else if ( (LA45_0==CHAR_RANGE) ) {s = 6;}

				else if ( (LA45_0==BLOCK) ) {s = 7;}

				else if ( (LA45_0==OPTIONAL) && (EvaluatePredicate(synpred1_CodeGenTreeWalker_fragment))) {s = 8;}

				else if ( (LA45_0==CLOSURE) && (EvaluatePredicate(synpred1_CodeGenTreeWalker_fragment))) {s = 9;}

				else if ( (LA45_0==POSITIVE_CLOSURE) && (EvaluatePredicate(synpred1_CodeGenTreeWalker_fragment))) {s = 10;}

				else if ( (LA45_0==CHAR_LITERAL||LA45_0==DOT||LA45_0==RULE_REF||LA45_0==STRING_LITERAL||LA45_0==TOKEN_REF||LA45_0==WILDCARD) ) {s = 11;}

				else if ( (LA45_0==TREE_BEGIN) ) {s = 17;}

				else if ( (LA45_0==ACTION||LA45_0==FORCED_ACTION) ) {s = 18;}

				else if ( (LA45_0==GATED_SEMPRED||LA45_0==SEMPRED) ) {s = 20;}

				else if ( (LA45_0==SYN_SEMPRED) ) {s = 22;}

				else if ( (LA45_0==SYNPRED) ) {s = 23;}

				else if ( (LA45_0==BACKTRACK_SEMPRED) ) {s = 24;}

				else if ( (LA45_0==EPSILON) ) {s = 25;}


				input.Seek(index45_0);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA45_7 = input.LA(1);


				int index45_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_CodeGenTreeWalker_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index45_7);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 45, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}

	#endregion DFA

	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _LEXER_GRAMMAR_in_grammar_66 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_68 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PARSER_GRAMMAR_in_grammar_78 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_80 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_GRAMMAR_in_grammar_90 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_92 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _COMBINED_GRAMMAR_in_grammar_102 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_104 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SCOPE_in_attrScope123 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScope125 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _AMPERSAND_in_attrScope130 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_attrScope139 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_grammarSpec156 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammarSpec164 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _OPTIONS_in_grammarSpec185 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _IMPORT_in_grammarSpec199 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _TOKENS_in_grammarSpec213 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _attrScope_in_grammarSpec225 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _AMPERSAND_in_grammarSpec234 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rules_in_grammarSpec245 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rule_in_rules290 = new BitSet(new ulong[]{0x2UL,0x1000UL});
		public static readonly BitSet _RULE_in_rules304 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _RULE_in_rule345 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rule349 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_rule362 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_rule370 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule373 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RET_in_rule382 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule385 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _throwsSpec_in_rule394 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _OPTIONS_in_rule404 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ruleScopeSpec_in_rule417 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _AMPERSAND_in_rule427 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_rule441 = new BitSet(new ulong[]{0x4400020000UL});
		public static readonly BitSet _exceptionGroup_in_rule454 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_rule462 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_modifier485 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec512 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_throwsSpec514 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec529 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _AMPERSAND_in_ruleScopeSpec534 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec544 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _ID_in_ruleScopeSpec550 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _setBlock_in_block591 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BLOCK_in_block604 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _OPTIONS_in_block612 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _alternative_in_block629 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _rewrite_in_block633 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _EOB_in_block650 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BLOCK_in_setBlock682 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_setAlternative702 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _setElement_in_setAlternative704 = new BitSet(new ulong[]{0x1000C0000UL,0x4200000UL});
		public static readonly BitSet _EOA_in_setAlternative707 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup722 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup729 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup737 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler751 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler753 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler755 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FINALLY_in_finallyClause773 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_finallyClause775 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_alternative804 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_alternative817 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16401UL});
		public static readonly BitSet _EOA_in_alternative835 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ROOT_in_element870 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element874 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BANG_in_element887 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element891 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _NOT_in_element907 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _notElement_in_element909 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_element924 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element928 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16401UL});
		public static readonly BitSet _element_in_element932 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_ASSIGN_in_element947 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element951 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16401UL});
		public static readonly BitSet _element_in_element955 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_RANGE_in_element969 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_element973 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_element977 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ebnf_in_element1006 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _atom_in_element1016 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_element1027 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _element_action_in_element1037 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_element1052 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _GATED_SEMPRED_in_element1056 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYN_SEMPRED_in_element1067 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYNPRED_in_element1075 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_element1086 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _EPSILON_in_element1098 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_element_action1115 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FORCED_ACTION_in_element_action1126 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_notElement1155 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_notElement1168 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_notElement1181 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BLOCK_in_notElement1195 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1241 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_ebnf1260 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1264 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_ebnf1285 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1289 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_ebnf1310 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1314 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_1352 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_tree_1359 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16401UL});
		public static readonly BitSet _element_action_in_tree_1396 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16401UL});
		public static readonly BitSet _element_in_tree_1418 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16401UL});
		public static readonly BitSet _RULE_REF_in_atom1469 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1474 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom1492 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1497 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom1513 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_atom1526 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _WILDCARD_in_atom1538 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_atom1549 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_atom1551 = new BitSet(new ulong[]{0x20050000UL,0x84202000UL});
		public static readonly BitSet _atom_in_atom1555 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_atom1568 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_ast_suffix1584 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BLOCK_in_set1613 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1633 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_setElement1638 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_setElement1643 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_RANGE_in_setElement1649 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1651 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_setElement1653 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REWRITE_in_rewrite1690 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _SEMPRED_in_rewrite1695 = new BitSet(new ulong[]{0x2000000110UL,0x1000000UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite1701 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BLOCK_in_rewrite_block1736 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite_block1748 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _EOB_in_rewrite_block1753 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_rewrite_alternative1788 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_element_in_rewrite_alternative1806 = new BitSet(new ulong[]{0x201000100240010UL,0x24202001UL});
		public static readonly BitSet _EPSILON_in_rewrite_alternative1827 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_rewrite_alternative1843 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rewrite_template_in_rewrite_alternative1856 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ETC_in_rewrite_alternative1869 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_element1889 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_ebnf_in_rewrite_element1899 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_tree_in_rewrite_element1908 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_rewrite_ebnf1929 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1931 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_rewrite_ebnf1949 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1951 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_rewrite_ebnf1969 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1971 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_rewrite_tree2004 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_tree2011 = new BitSet(new ulong[]{0x201000100240018UL,0x24202001UL});
		public static readonly BitSet _rewrite_element_in_rewrite_tree2031 = new BitSet(new ulong[]{0x201000100240018UL,0x24202001UL});
		public static readonly BitSet _RULE_REF_in_rewrite_atom2076 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_rewrite_atom2093 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rewrite_atom2098 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_rewrite_atom2109 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_rewrite_atom2117 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LABEL_in_rewrite_atom2131 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_rewrite_atom2141 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ALT_in_rewrite_template2164 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _EPSILON_in_rewrite_template2166 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_rewrite_template2168 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_rewrite_template2179 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template2184 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ACTION_in_rewrite_template2188 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ARGLIST_in_rewrite_template2201 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_in_rewrite_template2211 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template2215 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_rewrite_template2219 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template2252 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template2265 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_rewrite_template2289 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_synpred1_CodeGenTreeWalker991 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _element_action_in_synpred2_CodeGenTreeWalker1385 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
