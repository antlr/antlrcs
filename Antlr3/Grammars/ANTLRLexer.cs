// $ANTLR 3.1.2 Grammars\\ANTLR.g3 2009-04-16 21:27:00

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

using ErrorManager = Antlr3.Tool.ErrorManager;
using Grammar = Antlr3.Tool.Grammar;
using StringBuffer = System.Text.StringBuilder;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.Grammars
{
public partial class ANTLRLexer : Lexer
{
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

	public ANTLRLexer() {}
	public ANTLRLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ANTLRLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Grammars\\ANTLR.g3"; } }

	// $ANTLR start "CATCH"
	private void mCATCH()
	{
		try
		{
			int _type = CATCH;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:46:9: ( 'catch' )
			// Grammars\\ANTLR.g3:46:9: 'catch'
			{
			Match("catch"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CATCH"

	// $ANTLR start "FINALLY"
	private void mFINALLY()
	{
		try
		{
			int _type = FINALLY;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:47:11: ( 'finally' )
			// Grammars\\ANTLR.g3:47:11: 'finally'
			{
			Match("finally"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "FINALLY"

	// $ANTLR start "FRAGMENT"
	private void mFRAGMENT()
	{
		try
		{
			int _type = FRAGMENT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:48:12: ( 'fragment' )
			// Grammars\\ANTLR.g3:48:12: 'fragment'
			{
			Match("fragment"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "FRAGMENT"

	// $ANTLR start "GRAMMAR"
	private void mGRAMMAR()
	{
		try
		{
			int _type = GRAMMAR;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:49:11: ( 'grammar' )
			// Grammars\\ANTLR.g3:49:11: 'grammar'
			{
			Match("grammar"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "GRAMMAR"

	// $ANTLR start "IMPORT"
	private void mIMPORT()
	{
		try
		{
			int _type = IMPORT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:50:10: ( 'import' )
			// Grammars\\ANTLR.g3:50:10: 'import'
			{
			Match("import"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "IMPORT"

	// $ANTLR start "LEXER"
	private void mLEXER()
	{
		try
		{
			int _type = LEXER;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:51:9: ( 'lexer' )
			// Grammars\\ANTLR.g3:51:9: 'lexer'
			{
			Match("lexer"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LEXER"

	// $ANTLR start "PARSER"
	private void mPARSER()
	{
		try
		{
			int _type = PARSER;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:52:10: ( 'parser' )
			// Grammars\\ANTLR.g3:52:10: 'parser'
			{
			Match("parser"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PARSER"

	// $ANTLR start "PRIVATE"
	private void mPRIVATE()
	{
		try
		{
			int _type = PRIVATE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:53:11: ( 'private' )
			// Grammars\\ANTLR.g3:53:11: 'private'
			{
			Match("private"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PRIVATE"

	// $ANTLR start "PROTECTED"
	private void mPROTECTED()
	{
		try
		{
			int _type = PROTECTED;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:54:13: ( 'protected' )
			// Grammars\\ANTLR.g3:54:13: 'protected'
			{
			Match("protected"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PROTECTED"

	// $ANTLR start "PUBLIC"
	private void mPUBLIC()
	{
		try
		{
			int _type = PUBLIC;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:55:10: ( 'public' )
			// Grammars\\ANTLR.g3:55:10: 'public'
			{
			Match("public"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PUBLIC"

	// $ANTLR start "RETURNS"
	private void mRETURNS()
	{
		try
		{
			int _type = RETURNS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:56:11: ( 'returns' )
			// Grammars\\ANTLR.g3:56:11: 'returns'
			{
			Match("returns"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RETURNS"

	// $ANTLR start "SCOPE"
	private void mSCOPE()
	{
		try
		{
			int _type = SCOPE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:57:9: ( 'scope' )
			// Grammars\\ANTLR.g3:57:9: 'scope'
			{
			Match("scope"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SCOPE"

	// $ANTLR start "THROWS"
	private void mTHROWS()
	{
		try
		{
			int _type = THROWS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:58:10: ( 'throws' )
			// Grammars\\ANTLR.g3:58:10: 'throws'
			{
			Match("throws"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "THROWS"

	// $ANTLR start "TREE"
	private void mTREE()
	{
		try
		{
			int _type = TREE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:59:8: ( 'tree' )
			// Grammars\\ANTLR.g3:59:8: 'tree'
			{
			Match("tree"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TREE"

	// $ANTLR start "STRING_LITERAL"
	private void mSTRING_LITERAL()
	{
		try
		{
			// Grammars\\ANTLR.g3:816:27: ()
			// Grammars\\ANTLR.g3:816:27: 
			{


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "STRING_LITERAL"

	// $ANTLR start "FORCED_ACTION"
	private void mFORCED_ACTION()
	{
		try
		{
			// Grammars\\ANTLR.g3:817:26: ()
			// Grammars\\ANTLR.g3:817:26: 
			{


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "FORCED_ACTION"

	// $ANTLR start "DOC_COMMENT"
	private void mDOC_COMMENT()
	{
		try
		{
			// Grammars\\ANTLR.g3:818:24: ()
			// Grammars\\ANTLR.g3:818:24: 
			{


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "DOC_COMMENT"

	// $ANTLR start "SEMPRED"
	private void mSEMPRED()
	{
		try
		{
			// Grammars\\ANTLR.g3:819:20: ()
			// Grammars\\ANTLR.g3:819:20: 
			{


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "SEMPRED"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:822:4: ( ( ' ' | '\\t' | ( '\\r' )? '\\n' ) )
			// Grammars\\ANTLR.g3:822:4: ( ' ' | '\\t' | ( '\\r' )? '\\n' )
			{
			// Grammars\\ANTLR.g3:822:4: ( ' ' | '\\t' | ( '\\r' )? '\\n' )
			int alt2=3;
			switch ( input.LA(1) )
			{
			case ' ':
				{
				alt2=1;
				}
				break;
			case '\t':
				{
				alt2=2;
				}
				break;
			case '\n':
			case '\r':
				{
				alt2=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

					throw nvae;
				}
			}

			switch ( alt2 )
			{
			case 1:
				// Grammars\\ANTLR.g3:822:6: ' '
				{
				Match(' '); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:823:5: '\\t'
				{
				Match('\t'); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:824:5: ( '\\r' )? '\\n'
				{
				// Grammars\\ANTLR.g3:824:5: ( '\\r' )?
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( (LA1_0=='\r') )
				{
					alt1=1;
				}
				switch ( alt1 )
				{
				case 1:
					// Grammars\\ANTLR.g3:824:6: '\\r'
					{
					Match('\r'); if (state.failed) return ;

					}
					break;

				}

				Match('\n'); if (state.failed) return ;

				}
				break;

			}

			if ( state.backtracking == 0 )
			{
				 _channel = Hidden; 
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "WS"

	// $ANTLR start "COMMENT"
	private void mCOMMENT()
	{
		try
		{
			int _type = COMMENT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:830:4: ( ( SL_COMMENT | ML_COMMENT[ref $type] ) )
			// Grammars\\ANTLR.g3:830:4: ( SL_COMMENT | ML_COMMENT[ref $type] )
			{
			// Grammars\\ANTLR.g3:830:4: ( SL_COMMENT | ML_COMMENT[ref $type] )
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0=='/') )
			{
				int LA3_1 = input.LA(2);

				if ( (LA3_1=='/') )
				{
					alt3=1;
				}
				else if ( (LA3_1=='*') )
				{
					alt3=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 3, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

				throw nvae;
			}
			switch ( alt3 )
			{
			case 1:
				// Grammars\\ANTLR.g3:830:6: SL_COMMENT
				{
				mSL_COMMENT(); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:830:19: ML_COMMENT[ref $type]
				{
				mML_COMMENT(ref _type); if (state.failed) return ;

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							if ( _type != DOC_COMMENT )
								_channel = Hidden;
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "COMMENT"

	// $ANTLR start "SL_COMMENT"
	private void mSL_COMMENT()
	{
		try
		{
			// Grammars\\ANTLR.g3:840:4: ( '//' (=> ' $ANTLR ' SRC ( ( '\\r' )? '\\n' )? | (~ ( '\\r' | '\\n' ) )* ( ( '\\r' )? '\\n' )? ) )
			// Grammars\\ANTLR.g3:840:4: '//' (=> ' $ANTLR ' SRC ( ( '\\r' )? '\\n' )? | (~ ( '\\r' | '\\n' ) )* ( ( '\\r' )? '\\n' )? )
			{
			Match("//"); if (state.failed) return ;

			// Grammars\\ANTLR.g3:841:3: (=> ' $ANTLR ' SRC ( ( '\\r' )? '\\n' )? | (~ ( '\\r' | '\\n' ) )* ( ( '\\r' )? '\\n' )? )
			int alt9=2;
			alt9 = dfa9.Predict(input);
			switch ( alt9 )
			{
			case 1:
				// Grammars\\ANTLR.g3:841:5: => ' $ANTLR ' SRC ( ( '\\r' )? '\\n' )?
				{

				Match(" $ANTLR "); if (state.failed) return ;

				mSRC(); if (state.failed) return ;
				// Grammars\\ANTLR.g3:841:35: ( ( '\\r' )? '\\n' )?
				int alt5=2;
				int LA5_0 = input.LA(1);

				if ( (LA5_0=='\n'||LA5_0=='\r') )
				{
					alt5=1;
				}
				switch ( alt5 )
				{
				case 1:
					// Grammars\\ANTLR.g3:841:36: ( '\\r' )? '\\n'
					{
					// Grammars\\ANTLR.g3:841:36: ( '\\r' )?
					int alt4=2;
					int LA4_0 = input.LA(1);

					if ( (LA4_0=='\r') )
					{
						alt4=1;
					}
					switch ( alt4 )
					{
					case 1:
						// Grammars\\ANTLR.g3:841:37: '\\r'
						{
						Match('\r'); if (state.failed) return ;

						}
						break;

					}

					Match('\n'); if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:842:5: (~ ( '\\r' | '\\n' ) )* ( ( '\\r' )? '\\n' )?
				{
				// Grammars\\ANTLR.g3:842:5: (~ ( '\\r' | '\\n' ) )*
				for ( ; ; )
				{
					int alt6=2;
					int LA6_0 = input.LA(1);

					if ( ((LA6_0>='\u0000' && LA6_0<='\t')||(LA6_0>='\u000B' && LA6_0<='\f')||(LA6_0>='\u000E' && LA6_0<='\uFFFF')) )
					{
						alt6=1;
					}


					switch ( alt6 )
					{
					case 1:
						// Grammars\\ANTLR.g3:
						{
						input.Consume();
						state.failed=false;

						}
						break;

					default:
						goto loop6;
					}
				}

				loop6:
					;


				// Grammars\\ANTLR.g3:842:19: ( ( '\\r' )? '\\n' )?
				int alt8=2;
				int LA8_0 = input.LA(1);

				if ( (LA8_0=='\n'||LA8_0=='\r') )
				{
					alt8=1;
				}
				switch ( alt8 )
				{
				case 1:
					// Grammars\\ANTLR.g3:842:20: ( '\\r' )? '\\n'
					{
					// Grammars\\ANTLR.g3:842:20: ( '\\r' )?
					int alt7=2;
					int LA7_0 = input.LA(1);

					if ( (LA7_0=='\r') )
					{
						alt7=1;
					}
					switch ( alt7 )
					{
					case 1:
						// Grammars\\ANTLR.g3:842:21: '\\r'
						{
						Match('\r'); if (state.failed) return ;

						}
						break;

					}

					Match('\n'); if (state.failed) return ;

					}
					break;

				}


				}
				break;

			}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "SL_COMMENT"

	// $ANTLR start "ML_COMMENT"
	private void mML_COMMENT(ref int type)
	{
		try
		{
			// Grammars\\ANTLR.g3:848:4: ( '/*' ( . )* '*/' )
			// Grammars\\ANTLR.g3:848:4: '/*' ( . )* '*/'
			{
			Match("/*"); if (state.failed) return ;

			if ( state.backtracking == 0 )
			{
				type = (input.LA(1) == '*' && input.LA(2) != '/') ? DOC_COMMENT : ML_COMMENT;
			}
			// Grammars\\ANTLR.g3:850:3: ( . )*
			for ( ; ; )
			{
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( (LA10_0=='*') )
				{
					int LA10_1 = input.LA(2);

					if ( (LA10_1=='/') )
					{
						alt10=2;
					}
					else if ( ((LA10_1>='\u0000' && LA10_1<='.')||(LA10_1>='0' && LA10_1<='\uFFFF')) )
					{
						alt10=1;
					}


				}
				else if ( ((LA10_0>='\u0000' && LA10_0<=')')||(LA10_0>='+' && LA10_0<='\uFFFF')) )
				{
					alt10=1;
				}


				switch ( alt10 )
				{
				case 1:
					// Grammars\\ANTLR.g3:850:0: .
					{
					MatchAny(); if (state.failed) return ;

					}
					break;

				default:
					goto loop10;
				}
			}

			loop10:
				;


			Match("*/"); if (state.failed) return ;


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ML_COMMENT"

	// $ANTLR start "OPEN_ELEMENT_OPTION"
	private void mOPEN_ELEMENT_OPTION()
	{
		try
		{
			int _type = OPEN_ELEMENT_OPTION;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:854:4: ( '<' )
			// Grammars\\ANTLR.g3:854:4: '<'
			{
			Match('<'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OPEN_ELEMENT_OPTION"

	// $ANTLR start "CLOSE_ELEMENT_OPTION"
	private void mCLOSE_ELEMENT_OPTION()
	{
		try
		{
			int _type = CLOSE_ELEMENT_OPTION;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:858:4: ( '>' )
			// Grammars\\ANTLR.g3:858:4: '>'
			{
			Match('>'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CLOSE_ELEMENT_OPTION"

	// $ANTLR start "AMPERSAND"
	private void mAMPERSAND()
	{
		try
		{
			int _type = AMPERSAND;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:861:13: ( '@' )
			// Grammars\\ANTLR.g3:861:13: '@'
			{
			Match('@'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "AMPERSAND"

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:863:9: ( ',' )
			// Grammars\\ANTLR.g3:863:9: ','
			{
			Match(','); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "COMMA"

	// $ANTLR start "QUESTION"
	private void mQUESTION()
	{
		try
		{
			int _type = QUESTION;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:865:12: ( '?' )
			// Grammars\\ANTLR.g3:865:12: '?'
			{
			Match('?'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "QUESTION"

	// $ANTLR start "TREE_BEGIN"
	private void mTREE_BEGIN()
	{
		try
		{
			int _type = TREE_BEGIN;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:867:14: ( '^(' )
			// Grammars\\ANTLR.g3:867:14: '^('
			{
			Match("^("); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TREE_BEGIN"

	// $ANTLR start "LPAREN"
	private void mLPAREN()
	{
		try
		{
			int _type = LPAREN;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:869:9: ( '(' )
			// Grammars\\ANTLR.g3:869:9: '('
			{
			Match('('); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LPAREN"

	// $ANTLR start "RPAREN"
	private void mRPAREN()
	{
		try
		{
			int _type = RPAREN;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:871:9: ( ')' )
			// Grammars\\ANTLR.g3:871:9: ')'
			{
			Match(')'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RPAREN"

	// $ANTLR start "COLON"
	private void mCOLON()
	{
		try
		{
			int _type = COLON;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:873:9: ( ':' )
			// Grammars\\ANTLR.g3:873:9: ':'
			{
			Match(':'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "COLON"

	// $ANTLR start "STAR"
	private void mSTAR()
	{
		try
		{
			int _type = STAR;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:875:7: ( '*' )
			// Grammars\\ANTLR.g3:875:7: '*'
			{
			Match('*'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "STAR"

	// $ANTLR start "PLUS"
	private void mPLUS()
	{
		try
		{
			int _type = PLUS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:877:7: ( '+' )
			// Grammars\\ANTLR.g3:877:7: '+'
			{
			Match('+'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PLUS"

	// $ANTLR start "ASSIGN"
	private void mASSIGN()
	{
		try
		{
			int _type = ASSIGN;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:879:10: ( '=' )
			// Grammars\\ANTLR.g3:879:10: '='
			{
			Match('='); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ASSIGN"

	// $ANTLR start "PLUS_ASSIGN"
	private void mPLUS_ASSIGN()
	{
		try
		{
			int _type = PLUS_ASSIGN;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:881:15: ( '+=' )
			// Grammars\\ANTLR.g3:881:15: '+='
			{
			Match("+="); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PLUS_ASSIGN"

	// $ANTLR start "IMPLIES"
	private void mIMPLIES()
	{
		try
		{
			int _type = IMPLIES;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:883:11: ( '=>' )
			// Grammars\\ANTLR.g3:883:11: '=>'
			{
			Match("=>"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "IMPLIES"

	// $ANTLR start "REWRITE"
	private void mREWRITE()
	{
		try
		{
			int _type = REWRITE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:885:11: ( '->' )
			// Grammars\\ANTLR.g3:885:11: '->'
			{
			Match("->"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "REWRITE"

	// $ANTLR start "SEMI"
	private void mSEMI()
	{
		try
		{
			int _type = SEMI;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:887:7: ( ';' )
			// Grammars\\ANTLR.g3:887:7: ';'
			{
			Match(';'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SEMI"

	// $ANTLR start "ROOT"
	private void mROOT()
	{
		try
		{
			int _type = ROOT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:889:8: ( '^' )
			// Grammars\\ANTLR.g3:889:8: '^'
			{
			Match('^'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				hasASTOperator=true;
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ROOT"

	// $ANTLR start "BANG"
	private void mBANG()
	{
		try
		{
			int _type = BANG;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:891:8: ( '!' )
			// Grammars\\ANTLR.g3:891:8: '!'
			{
			Match('!'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				hasASTOperator=true;
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "BANG"

	// $ANTLR start "OR"
	private void mOR()
	{
		try
		{
			int _type = OR;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:893:6: ( '|' )
			// Grammars\\ANTLR.g3:893:6: '|'
			{
			Match('|'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OR"

	// $ANTLR start "WILDCARD"
	private void mWILDCARD()
	{
		try
		{
			int _type = WILDCARD;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:895:12: ( '.' )
			// Grammars\\ANTLR.g3:895:12: '.'
			{
			Match('.'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "WILDCARD"

	// $ANTLR start "ETC"
	private void mETC()
	{
		try
		{
			int _type = ETC;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:897:7: ( '...' )
			// Grammars\\ANTLR.g3:897:7: '...'
			{
			Match("..."); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ETC"

	// $ANTLR start "RANGE"
	private void mRANGE()
	{
		try
		{
			int _type = RANGE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:899:9: ( '..' )
			// Grammars\\ANTLR.g3:899:9: '..'
			{
			Match(".."); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RANGE"

	// $ANTLR start "NOT"
	private void mNOT()
	{
		try
		{
			int _type = NOT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:901:7: ( '~' )
			// Grammars\\ANTLR.g3:901:7: '~'
			{
			Match('~'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "NOT"

	// $ANTLR start "RCURLY"
	private void mRCURLY()
	{
		try
		{
			int _type = RCURLY;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:903:9: ( '}' )
			// Grammars\\ANTLR.g3:903:9: '}'
			{
			Match('}'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RCURLY"

	// $ANTLR start "DOLLAR"
	private void mDOLLAR()
	{
		try
		{
			int _type = DOLLAR;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:905:10: ( '$' )
			// Grammars\\ANTLR.g3:905:10: '$'
			{
			Match('$'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DOLLAR"

	// $ANTLR start "STRAY_BRACKET"
	private void mSTRAY_BRACKET()
	{
		try
		{
			int _type = STRAY_BRACKET;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:908:4: ( ']' )
			// Grammars\\ANTLR.g3:908:4: ']'
			{
			Match(']'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							ErrorManager.SyntaxError(
								ErrorManager.MSG_SYNTAX_ERROR,
								null,
								state.token,
								"antlr: dangling ']'? make sure to escape with \\]",
								null);
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "STRAY_BRACKET"

	// $ANTLR start "CHAR_LITERAL"
	private void mCHAR_LITERAL()
	{
		try
		{
			int _type = CHAR_LITERAL;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:920:4: ( '\\'' ( ESC |~ ( '\\\\' | '\\'' ) )* '\\'' )
			// Grammars\\ANTLR.g3:920:4: '\\'' ( ESC |~ ( '\\\\' | '\\'' ) )* '\\''
			{
			Match('\''); if (state.failed) return ;
			// Grammars\\ANTLR.g3:921:3: ( ESC |~ ( '\\\\' | '\\'' ) )*
			for ( ; ; )
			{
				int alt11=3;
				int LA11_0 = input.LA(1);

				if ( (LA11_0=='\\') )
				{
					alt11=1;
				}
				else if ( ((LA11_0>='\u0000' && LA11_0<='&')||(LA11_0>='(' && LA11_0<='[')||(LA11_0>=']' && LA11_0<='\uFFFF')) )
				{
					alt11=2;
				}


				switch ( alt11 )
				{
				case 1:
					// Grammars\\ANTLR.g3:921:5: ESC
					{
					mESC(); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:922:5: ~ ( '\\\\' | '\\'' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop11;
				}
			}

			loop11:
				;


			Match('\''); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							StringBuffer s = Grammar.GetUnescapedStringFromGrammarStringLiteral(Text);
							if ( s.Length > 1 )
							{
								_type = STRING_LITERAL;
							}
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CHAR_LITERAL"

	// $ANTLR start "DOUBLE_QUOTE_STRING_LITERAL"
	private void mDOUBLE_QUOTE_STRING_LITERAL()
	{
		try
		{
			int _type = DOUBLE_QUOTE_STRING_LITERAL;
			int _channel = DefaultTokenChannel;
			int c;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Grammars\\ANTLR.g3:939:4: ( '\"' (=> '\\\\' '\"' | '\\\\' c=~ '\"' |c=~ ( '\\\\' | '\"' ) )* '\"' )
			// Grammars\\ANTLR.g3:939:4: '\"' (=> '\\\\' '\"' | '\\\\' c=~ '\"' |c=~ ( '\\\\' | '\"' ) )* '\"'
			{
			Match('\"'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				builder.Append('"');
			}
			// Grammars\\ANTLR.g3:940:3: (=> '\\\\' '\"' | '\\\\' c=~ '\"' |c=~ ( '\\\\' | '\"' ) )*
			for ( ; ; )
			{
				int alt12=4;
				int LA12_0 = input.LA(1);

				if ( (LA12_0=='\\') )
				{
					int LA12_2 = input.LA(2);

					if ( (LA12_2=='\"') && (EvaluatePredicate(synpred2_ANTLR_fragment)))
					{
						alt12=1;
					}
					else if ( ((LA12_2>='\u0000' && LA12_2<='!')||(LA12_2>='#' && LA12_2<='\uFFFF')) )
					{
						alt12=2;
					}


				}
				else if ( ((LA12_0>='\u0000' && LA12_0<='!')||(LA12_0>='#' && LA12_0<='[')||(LA12_0>=']' && LA12_0<='\uFFFF')) )
				{
					alt12=3;
				}


				switch ( alt12 )
				{
				case 1:
					// Grammars\\ANTLR.g3:940:5: => '\\\\' '\"'
					{

					Match('\\'); if (state.failed) return ;
					Match('\"'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						builder.Append('"');
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:941:5: '\\\\' c=~ '\"'
					{
					Match('\\'); if (state.failed) return ;
					c= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append("\\" + (char)c);
					}

					}
					break;
				case 3:
					// Grammars\\ANTLR.g3:942:5: c=~ ( '\\\\' | '\"' )
					{
					c= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append((char)c);
					}

					}
					break;

				default:
					goto loop12;
				}
			}

			loop12:
				;


			Match('\"'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				builder.Append('"');
			}
			if ( state.backtracking == 0 )
			{

							Text = builder.ToString();
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DOUBLE_QUOTE_STRING_LITERAL"

	// $ANTLR start "DOUBLE_ANGLE_STRING_LITERAL"
	private void mDOUBLE_ANGLE_STRING_LITERAL()
	{
		try
		{
			int _type = DOUBLE_ANGLE_STRING_LITERAL;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:951:4: ( '<<' ( . )* '>>' )
			// Grammars\\ANTLR.g3:951:4: '<<' ( . )* '>>'
			{
			Match("<<"); if (state.failed) return ;

			// Grammars\\ANTLR.g3:951:9: ( . )*
			for ( ; ; )
			{
				int alt13=2;
				int LA13_0 = input.LA(1);

				if ( (LA13_0=='>') )
				{
					int LA13_1 = input.LA(2);

					if ( (LA13_1=='>') )
					{
						alt13=2;
					}
					else if ( ((LA13_1>='\u0000' && LA13_1<='=')||(LA13_1>='?' && LA13_1<='\uFFFF')) )
					{
						alt13=1;
					}


				}
				else if ( ((LA13_0>='\u0000' && LA13_0<='=')||(LA13_0>='?' && LA13_0<='\uFFFF')) )
				{
					alt13=1;
				}


				switch ( alt13 )
				{
				case 1:
					// Grammars\\ANTLR.g3:951:0: .
					{
					MatchAny(); if (state.failed) return ;

					}
					break;

				default:
					goto loop13;
				}
			}

			loop13:
				;


			Match(">>"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DOUBLE_ANGLE_STRING_LITERAL"

	// $ANTLR start "ESC"
	private void mESC()
	{
		try
		{
			// Grammars\\ANTLR.g3:957:4: ( '\\\\' ( . ) )
			// Grammars\\ANTLR.g3:957:4: '\\\\' ( . )
			{
			Match('\\'); if (state.failed) return ;
			// Grammars\\ANTLR.g3:958:3: ( . )
			// Grammars\\ANTLR.g3:969:7: .
			{
			MatchAny(); if (state.failed) return ;

			}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ESC"

	// $ANTLR start "DIGIT"
	private void mDIGIT()
	{
		try
		{
			// Grammars\\ANTLR.g3:975:4: ( '0' .. '9' )
			// Grammars\\ANTLR.g3:
			{
			if ( (input.LA(1)>='0' && input.LA(1)<='9') )
			{
				input.Consume();
			state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "DIGIT"

	// $ANTLR start "XDIGIT"
	private void mXDIGIT()
	{
		try
		{
			// Grammars\\ANTLR.g3:980:4: ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )
			// Grammars\\ANTLR.g3:
			{
			if ( (input.LA(1)>='0' && input.LA(1)<='9')||(input.LA(1)>='A' && input.LA(1)<='F')||(input.LA(1)>='a' && input.LA(1)<='f') )
			{
				input.Consume();
			state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "XDIGIT"

	// $ANTLR start "INT"
	private void mINT()
	{
		try
		{
			int _type = INT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:985:4: ( ( '0' .. '9' )+ )
			// Grammars\\ANTLR.g3:985:4: ( '0' .. '9' )+
			{
			// Grammars\\ANTLR.g3:985:4: ( '0' .. '9' )+
			int cnt14=0;
			for ( ; ; )
			{
				int alt14=2;
				int LA14_0 = input.LA(1);

				if ( ((LA14_0>='0' && LA14_0<='9')) )
				{
					alt14=1;
				}


				switch ( alt14 )
				{
				case 1:
					// Grammars\\ANTLR.g3:
					{
					input.Consume();
					state.failed=false;

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

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "INT"

	// $ANTLR start "ARG_ACTION"
	private void mARG_ACTION()
	{
		try
		{
			int _type = ARG_ACTION;
			int _channel = DefaultTokenChannel;

				string text_ = string.Empty;

			// Grammars\\ANTLR.g3:993:4: ( '[' NESTED_ARG_ACTION[out text_] ']' )
			// Grammars\\ANTLR.g3:993:4: '[' NESTED_ARG_ACTION[out text_] ']'
			{
			Match('['); if (state.failed) return ;
			mNESTED_ARG_ACTION(out text_); if (state.failed) return ;
			Match(']'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				 Text = text_; 
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ARG_ACTION"

	// $ANTLR start "NESTED_ARG_ACTION"
	private void mNESTED_ARG_ACTION(out string text_)
	{
		try
		{
			IToken ACTION_STRING_LITERAL1=null;
			IToken ACTION_CHAR_LITERAL2=null;
			int c;


				text_ = string.Empty;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Grammars\\ANTLR.g3:1007:4: ( (=> '\\\\' ']' | '\\\\' c=~ ( ']' ) | ACTION_STRING_LITERAL | ACTION_CHAR_LITERAL |c=~ ( '\\\\' | '\"' | '\\'' | ']' ) )* )
			// Grammars\\ANTLR.g3:1007:4: (=> '\\\\' ']' | '\\\\' c=~ ( ']' ) | ACTION_STRING_LITERAL | ACTION_CHAR_LITERAL |c=~ ( '\\\\' | '\"' | '\\'' | ']' ) )*
			{
			// Grammars\\ANTLR.g3:1007:4: (=> '\\\\' ']' | '\\\\' c=~ ( ']' ) | ACTION_STRING_LITERAL | ACTION_CHAR_LITERAL |c=~ ( '\\\\' | '\"' | '\\'' | ']' ) )*
			for ( ; ; )
			{
				int alt15=6;
				int LA15_0 = input.LA(1);

				if ( (LA15_0=='\\') )
				{
					int LA15_2 = input.LA(2);

					if ( (LA15_2==']') && (EvaluatePredicate(synpred3_ANTLR_fragment)))
					{
						alt15=1;
					}
					else if ( ((LA15_2>='\u0000' && LA15_2<='\\')||(LA15_2>='^' && LA15_2<='\uFFFF')) )
					{
						alt15=2;
					}


				}
				else if ( (LA15_0=='\"') )
				{
					alt15=3;
				}
				else if ( (LA15_0=='\'') )
				{
					alt15=4;
				}
				else if ( ((LA15_0>='\u0000' && LA15_0<='!')||(LA15_0>='#' && LA15_0<='&')||(LA15_0>='(' && LA15_0<='[')||(LA15_0>='^' && LA15_0<='\uFFFF')) )
				{
					alt15=5;
				}


				switch ( alt15 )
				{
				case 1:
					// Grammars\\ANTLR.g3:1007:6: => '\\\\' ']'
					{

					Match('\\'); if (state.failed) return ;
					Match(']'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						builder.Append("]");
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:1008:5: '\\\\' c=~ ( ']' )
					{
					Match('\\'); if (state.failed) return ;
					c= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append("\\" + (char)c);
					}

					}
					break;
				case 3:
					// Grammars\\ANTLR.g3:1009:5: ACTION_STRING_LITERAL
					{
					int ACTION_STRING_LITERAL1Start857 = GetCharIndex();
					mACTION_STRING_LITERAL(); if (state.failed) return ;
					ACTION_STRING_LITERAL1 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ACTION_STRING_LITERAL1Start857, GetCharIndex()-1);
					if ( state.backtracking == 0 )
					{
						builder.Append((ACTION_STRING_LITERAL1!=null?ACTION_STRING_LITERAL1.Text:null));
					}

					}
					break;
				case 4:
					// Grammars\\ANTLR.g3:1010:5: ACTION_CHAR_LITERAL
					{
					int ACTION_CHAR_LITERAL2Start865 = GetCharIndex();
					mACTION_CHAR_LITERAL(); if (state.failed) return ;
					ACTION_CHAR_LITERAL2 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ACTION_CHAR_LITERAL2Start865, GetCharIndex()-1);
					if ( state.backtracking == 0 )
					{
						builder.Append((ACTION_CHAR_LITERAL2!=null?ACTION_CHAR_LITERAL2.Text:null));
					}

					}
					break;
				case 5:
					// Grammars\\ANTLR.g3:1011:5: c=~ ( '\\\\' | '\"' | '\\'' | ']' )
					{
					c= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append((char)c);
					}

					}
					break;

				default:
					goto loop15;
				}
			}

			loop15:
				;


			if ( state.backtracking == 0 )
			{

							text_ = builder.ToString();
						
			}

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "NESTED_ARG_ACTION"

	// $ANTLR start "ACTION"
	private void mACTION()
	{
		try
		{
			int _type = ACTION;
			int _channel = DefaultTokenChannel;

				int actionLine = Line;
				int actionColumn = CharPositionInLine;

			// Grammars\\ANTLR.g3:1023:4: ( NESTED_ACTION ( '?' )? )
			// Grammars\\ANTLR.g3:1023:4: NESTED_ACTION ( '?' )?
			{
			mNESTED_ACTION(); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1024:3: ( '?' )?
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( (LA16_0=='?') )
			{
				alt16=1;
			}
			switch ( alt16 )
			{
			case 1:
				// Grammars\\ANTLR.g3:1024:4: '?'
				{
				Match('?'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					_type = SEMPRED;
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							string action = Text;
							int n = 1; // num delimiter chars
							if ( action.StartsWith("{{") && action.EndsWith("}}") )
							{
								_type = FORCED_ACTION;
								n = 2;
							}
							action = action.Substring(n,action.Length-n - (_type==SEMPRED ? 1 : 0) - n);
							Text = action;
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION"

	// $ANTLR start "NESTED_ACTION"
	private void mNESTED_ACTION()
	{
		try
		{
			// Grammars\\ANTLR.g3:1041:4: ( '{' ( NESTED_ACTION | ACTION_CHAR_LITERAL |=> COMMENT | ACTION_STRING_LITERAL | ACTION_ESC |~ ( '{' | '\\'' | '\"' | '\\\\' | '}' ) )* '}' )
			// Grammars\\ANTLR.g3:1041:4: '{' ( NESTED_ACTION | ACTION_CHAR_LITERAL |=> COMMENT | ACTION_STRING_LITERAL | ACTION_ESC |~ ( '{' | '\\'' | '\"' | '\\\\' | '}' ) )* '}'
			{
			Match('{'); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1042:3: ( NESTED_ACTION | ACTION_CHAR_LITERAL |=> COMMENT | ACTION_STRING_LITERAL | ACTION_ESC |~ ( '{' | '\\'' | '\"' | '\\\\' | '}' ) )*
			for ( ; ; )
			{
				int alt17=7;
				int LA17_0 = input.LA(1);

				if ( (LA17_0=='{') )
				{
					alt17=1;
				}
				else if ( (LA17_0=='\'') )
				{
					alt17=2;
				}
				else if ( (LA17_0=='/') )
				{
					int LA17_4 = input.LA(2);

					if ( (EvaluatePredicate(synpred4_ANTLR_fragment)) )
					{
						alt17=3;
					}
					else if ( (true) )
					{
						alt17=6;
					}


				}
				else if ( (LA17_0=='\"') )
				{
					alt17=4;
				}
				else if ( (LA17_0=='\\') )
				{
					alt17=5;
				}
				else if ( ((LA17_0>='\u0000' && LA17_0<='!')||(LA17_0>='#' && LA17_0<='&')||(LA17_0>='(' && LA17_0<='.')||(LA17_0>='0' && LA17_0<='[')||(LA17_0>=']' && LA17_0<='z')||LA17_0=='|'||(LA17_0>='~' && LA17_0<='\uFFFF')) )
				{
					alt17=6;
				}


				switch ( alt17 )
				{
				case 1:
					// Grammars\\ANTLR.g3:1042:5: NESTED_ACTION
					{
					mNESTED_ACTION(); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:1043:5: ACTION_CHAR_LITERAL
					{
					mACTION_CHAR_LITERAL(); if (state.failed) return ;

					}
					break;
				case 3:
					// Grammars\\ANTLR.g3:1044:5: => COMMENT
					{

					mCOMMENT(); if (state.failed) return ;

					}
					break;
				case 4:
					// Grammars\\ANTLR.g3:1045:5: ACTION_STRING_LITERAL
					{
					mACTION_STRING_LITERAL(); if (state.failed) return ;

					}
					break;
				case 5:
					// Grammars\\ANTLR.g3:1046:5: ACTION_ESC
					{
					mACTION_ESC(); if (state.failed) return ;

					}
					break;
				case 6:
					// Grammars\\ANTLR.g3:1047:5: ~ ( '{' | '\\'' | '\"' | '\\\\' | '}' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop17;
				}
			}

			loop17:
				;


			Match('}'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "NESTED_ACTION"

	// $ANTLR start "ACTION_CHAR_LITERAL"
	private void mACTION_CHAR_LITERAL()
	{
		try
		{
			// Grammars\\ANTLR.g3:1054:4: ( '\\'' ( ACTION_ESC |~ ( '\\\\' | '\\'' ) )* '\\'' )
			// Grammars\\ANTLR.g3:1054:4: '\\'' ( ACTION_ESC |~ ( '\\\\' | '\\'' ) )* '\\''
			{
			Match('\''); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1055:3: ( ACTION_ESC |~ ( '\\\\' | '\\'' ) )*
			for ( ; ; )
			{
				int alt18=3;
				int LA18_0 = input.LA(1);

				if ( (LA18_0=='\\') )
				{
					alt18=1;
				}
				else if ( ((LA18_0>='\u0000' && LA18_0<='&')||(LA18_0>='(' && LA18_0<='[')||(LA18_0>=']' && LA18_0<='\uFFFF')) )
				{
					alt18=2;
				}


				switch ( alt18 )
				{
				case 1:
					// Grammars\\ANTLR.g3:1055:5: ACTION_ESC
					{
					mACTION_ESC(); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:1056:5: ~ ( '\\\\' | '\\'' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop18;
				}
			}

			loop18:
				;


			Match('\''); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION_CHAR_LITERAL"

	// $ANTLR start "ACTION_STRING_LITERAL"
	private void mACTION_STRING_LITERAL()
	{
		try
		{
			// Grammars\\ANTLR.g3:1063:4: ( '\"' ( ACTION_ESC |~ ( '\\\\' | '\"' ) )* '\"' )
			// Grammars\\ANTLR.g3:1063:4: '\"' ( ACTION_ESC |~ ( '\\\\' | '\"' ) )* '\"'
			{
			Match('\"'); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1064:3: ( ACTION_ESC |~ ( '\\\\' | '\"' ) )*
			for ( ; ; )
			{
				int alt19=3;
				int LA19_0 = input.LA(1);

				if ( (LA19_0=='\\') )
				{
					alt19=1;
				}
				else if ( ((LA19_0>='\u0000' && LA19_0<='!')||(LA19_0>='#' && LA19_0<='[')||(LA19_0>=']' && LA19_0<='\uFFFF')) )
				{
					alt19=2;
				}


				switch ( alt19 )
				{
				case 1:
					// Grammars\\ANTLR.g3:1064:5: ACTION_ESC
					{
					mACTION_ESC(); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:1065:5: ~ ( '\\\\' | '\"' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop19;
				}
			}

			loop19:
				;


			Match('\"'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION_STRING_LITERAL"

	// $ANTLR start "ACTION_ESC"
	private void mACTION_ESC()
	{
		try
		{
			// Grammars\\ANTLR.g3:1072:4: ( '\\\\\\'' | '\\\\\\\"' | '\\\\' ~ ( '\\'' | '\"' ) )
			int alt20=3;
			int LA20_0 = input.LA(1);

			if ( (LA20_0=='\\') )
			{
				int LA20_1 = input.LA(2);

				if ( (LA20_1=='\'') )
				{
					alt20=1;
				}
				else if ( (LA20_1=='\"') )
				{
					alt20=2;
				}
				else if ( ((LA20_1>='\u0000' && LA20_1<='!')||(LA20_1>='#' && LA20_1<='&')||(LA20_1>='(' && LA20_1<='\uFFFF')) )
				{
					alt20=3;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 20, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 20, 0, input);

				throw nvae;
			}
			switch ( alt20 )
			{
			case 1:
				// Grammars\\ANTLR.g3:1072:4: '\\\\\\''
				{
				Match("\\'"); if (state.failed) return ;


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:1073:4: '\\\\\\\"'
				{
				Match("\\\""); if (state.failed) return ;


				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:1074:4: '\\\\' ~ ( '\\'' | '\"' )
				{
				Match('\\'); if (state.failed) return ;
				input.Consume();
				state.failed=false;

				}
				break;

			}
		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION_ESC"

	// $ANTLR start "TOKEN_REF"
	private void mTOKEN_REF()
	{
		try
		{
			int _type = TOKEN_REF;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:1077:4: ( 'A' .. 'Z' ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			// Grammars\\ANTLR.g3:1077:4: 'A' .. 'Z' ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			{
			MatchRange('A','Z'); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1078:3: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			for ( ; ; )
			{
				int alt21=2;
				int LA21_0 = input.LA(1);

				if ( ((LA21_0>='0' && LA21_0<='9')||(LA21_0>='A' && LA21_0<='Z')||LA21_0=='_'||(LA21_0>='a' && LA21_0<='z')) )
				{
					alt21=1;
				}


				switch ( alt21 )
				{
				case 1:
					// Grammars\\ANTLR.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop21;
				}
			}

			loop21:
				;



			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TOKEN_REF"

	// $ANTLR start "TOKENS"
	private void mTOKENS()
	{
		try
		{
			int _type = TOKENS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:1083:4: ( 'tokens' WS_LOOP '{' )
			// Grammars\\ANTLR.g3:1083:4: 'tokens' WS_LOOP '{'
			{
			Match("tokens"); if (state.failed) return ;

			mWS_LOOP(); if (state.failed) return ;
			Match('{'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TOKENS"

	// $ANTLR start "OPTIONS"
	private void mOPTIONS()
	{
		try
		{
			int _type = OPTIONS;
			int _channel = DefaultTokenChannel;
			// Grammars\\ANTLR.g3:1087:4: ( 'options' WS_LOOP '{' )
			// Grammars\\ANTLR.g3:1087:4: 'options' WS_LOOP '{'
			{
			Match("options"); if (state.failed) return ;

			mWS_LOOP(); if (state.failed) return ;
			Match('{'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OPTIONS"

	// $ANTLR start "RULE_REF"
	private void mRULE_REF()
	{
		try
		{
			int _type = RULE_REF;
			int _channel = DefaultTokenChannel;

				int t=0;

			// Grammars\\ANTLR.g3:1096:4: ( 'a' .. 'z' ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			// Grammars\\ANTLR.g3:1096:4: 'a' .. 'z' ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			{
			MatchRange('a','z'); if (state.failed) return ;
			// Grammars\\ANTLR.g3:1096:13: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			for ( ; ; )
			{
				int alt22=2;
				int LA22_0 = input.LA(1);

				if ( ((LA22_0>='0' && LA22_0<='9')||(LA22_0>='A' && LA22_0<='Z')||LA22_0=='_'||(LA22_0>='a' && LA22_0<='z')) )
				{
					alt22=1;
				}


				switch ( alt22 )
				{
				case 1:
					// Grammars\\ANTLR.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop22;
				}
			}

			loop22:
				;



			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RULE_REF"

	// $ANTLR start "WS_LOOP"
	private void mWS_LOOP()
	{
		try
		{
			// Grammars\\ANTLR.g3:1102:4: ( ( WS | COMMENT )* )
			// Grammars\\ANTLR.g3:1102:4: ( WS | COMMENT )*
			{
			// Grammars\\ANTLR.g3:1102:4: ( WS | COMMENT )*
			for ( ; ; )
			{
				int alt23=3;
				int LA23_0 = input.LA(1);

				if ( ((LA23_0>='\t' && LA23_0<='\n')||LA23_0=='\r'||LA23_0==' ') )
				{
					alt23=1;
				}
				else if ( (LA23_0=='/') )
				{
					alt23=2;
				}


				switch ( alt23 )
				{
				case 1:
					// Grammars\\ANTLR.g3:1102:6: WS
					{
					mWS(); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:1103:5: COMMENT
					{
					mCOMMENT(); if (state.failed) return ;

					}
					break;

				default:
					goto loop23;
				}
			}

			loop23:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "WS_LOOP"

	// $ANTLR start "WS_OPT"
	private void mWS_OPT()
	{
		try
		{
			// Grammars\\ANTLR.g3:1109:4: ( ( WS )? )
			// Grammars\\ANTLR.g3:1109:4: ( WS )?
			{
			// Grammars\\ANTLR.g3:1109:4: ( WS )?
			int alt24=2;
			int LA24_0 = input.LA(1);

			if ( ((LA24_0>='\t' && LA24_0<='\n')||LA24_0=='\r'||LA24_0==' ') )
			{
				alt24=1;
			}
			switch ( alt24 )
			{
			case 1:
				// Grammars\\ANTLR.g3:1109:5: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "WS_OPT"

	// $ANTLR start "SRC"
	private void mSRC()
	{
		try
		{
			IToken file=null;
			IToken line=null;

			// Grammars\\ANTLR.g3:1122:4: ( 'src' ' ' file= ACTION_STRING_LITERAL ' ' line= INT )
			// Grammars\\ANTLR.g3:1122:4: 'src' ' ' file= ACTION_STRING_LITERAL ' ' line= INT
			{
			Match("src"); if (state.failed) return ;

			Match(' '); if (state.failed) return ;
			int fileStart1270 = GetCharIndex();
			mACTION_STRING_LITERAL(); if (state.failed) return ;
			file = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, fileStart1270, GetCharIndex()-1);
			Match(' '); if (state.failed) return ;
			int lineStart1276 = GetCharIndex();
			mINT(); if (state.failed) return ;
			line = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, lineStart1276, GetCharIndex()-1);
			if ( state.backtracking == 0 )
			{

							Filename = (file!=null?file.Text:null).Substring(1,(file!=null?file.Text:null).Length-2);
							Line = int.Parse((line!=null?line.Text:null)) - 1;  // -1 because SL_COMMENT will increment the line no. KR
						
			}

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "SRC"

	public override void mTokens()
	{
		// Grammars\\ANTLR.g3:1:10: ( CATCH | FINALLY | FRAGMENT | GRAMMAR | IMPORT | LEXER | PARSER | PRIVATE | PROTECTED | PUBLIC | RETURNS | SCOPE | THROWS | TREE | WS | COMMENT | OPEN_ELEMENT_OPTION | CLOSE_ELEMENT_OPTION | AMPERSAND | COMMA | QUESTION | TREE_BEGIN | LPAREN | RPAREN | COLON | STAR | PLUS | ASSIGN | PLUS_ASSIGN | IMPLIES | REWRITE | SEMI | ROOT | BANG | OR | WILDCARD | ETC | RANGE | NOT | RCURLY | DOLLAR | STRAY_BRACKET | CHAR_LITERAL | DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL | INT | ARG_ACTION | ACTION | TOKEN_REF | TOKENS | OPTIONS | RULE_REF )
		int alt25=52;
		alt25 = dfa25.Predict(input);
		switch ( alt25 )
		{
		case 1:
			// Grammars\\ANTLR.g3:1:10: CATCH
			{
			mCATCH(); if (state.failed) return ;

			}
			break;
		case 2:
			// Grammars\\ANTLR.g3:1:16: FINALLY
			{
			mFINALLY(); if (state.failed) return ;

			}
			break;
		case 3:
			// Grammars\\ANTLR.g3:1:24: FRAGMENT
			{
			mFRAGMENT(); if (state.failed) return ;

			}
			break;
		case 4:
			// Grammars\\ANTLR.g3:1:33: GRAMMAR
			{
			mGRAMMAR(); if (state.failed) return ;

			}
			break;
		case 5:
			// Grammars\\ANTLR.g3:1:41: IMPORT
			{
			mIMPORT(); if (state.failed) return ;

			}
			break;
		case 6:
			// Grammars\\ANTLR.g3:1:48: LEXER
			{
			mLEXER(); if (state.failed) return ;

			}
			break;
		case 7:
			// Grammars\\ANTLR.g3:1:54: PARSER
			{
			mPARSER(); if (state.failed) return ;

			}
			break;
		case 8:
			// Grammars\\ANTLR.g3:1:61: PRIVATE
			{
			mPRIVATE(); if (state.failed) return ;

			}
			break;
		case 9:
			// Grammars\\ANTLR.g3:1:69: PROTECTED
			{
			mPROTECTED(); if (state.failed) return ;

			}
			break;
		case 10:
			// Grammars\\ANTLR.g3:1:79: PUBLIC
			{
			mPUBLIC(); if (state.failed) return ;

			}
			break;
		case 11:
			// Grammars\\ANTLR.g3:1:86: RETURNS
			{
			mRETURNS(); if (state.failed) return ;

			}
			break;
		case 12:
			// Grammars\\ANTLR.g3:1:94: SCOPE
			{
			mSCOPE(); if (state.failed) return ;

			}
			break;
		case 13:
			// Grammars\\ANTLR.g3:1:100: THROWS
			{
			mTHROWS(); if (state.failed) return ;

			}
			break;
		case 14:
			// Grammars\\ANTLR.g3:1:107: TREE
			{
			mTREE(); if (state.failed) return ;

			}
			break;
		case 15:
			// Grammars\\ANTLR.g3:1:112: WS
			{
			mWS(); if (state.failed) return ;

			}
			break;
		case 16:
			// Grammars\\ANTLR.g3:1:115: COMMENT
			{
			mCOMMENT(); if (state.failed) return ;

			}
			break;
		case 17:
			// Grammars\\ANTLR.g3:1:123: OPEN_ELEMENT_OPTION
			{
			mOPEN_ELEMENT_OPTION(); if (state.failed) return ;

			}
			break;
		case 18:
			// Grammars\\ANTLR.g3:1:143: CLOSE_ELEMENT_OPTION
			{
			mCLOSE_ELEMENT_OPTION(); if (state.failed) return ;

			}
			break;
		case 19:
			// Grammars\\ANTLR.g3:1:164: AMPERSAND
			{
			mAMPERSAND(); if (state.failed) return ;

			}
			break;
		case 20:
			// Grammars\\ANTLR.g3:1:174: COMMA
			{
			mCOMMA(); if (state.failed) return ;

			}
			break;
		case 21:
			// Grammars\\ANTLR.g3:1:180: QUESTION
			{
			mQUESTION(); if (state.failed) return ;

			}
			break;
		case 22:
			// Grammars\\ANTLR.g3:1:189: TREE_BEGIN
			{
			mTREE_BEGIN(); if (state.failed) return ;

			}
			break;
		case 23:
			// Grammars\\ANTLR.g3:1:200: LPAREN
			{
			mLPAREN(); if (state.failed) return ;

			}
			break;
		case 24:
			// Grammars\\ANTLR.g3:1:207: RPAREN
			{
			mRPAREN(); if (state.failed) return ;

			}
			break;
		case 25:
			// Grammars\\ANTLR.g3:1:214: COLON
			{
			mCOLON(); if (state.failed) return ;

			}
			break;
		case 26:
			// Grammars\\ANTLR.g3:1:220: STAR
			{
			mSTAR(); if (state.failed) return ;

			}
			break;
		case 27:
			// Grammars\\ANTLR.g3:1:225: PLUS
			{
			mPLUS(); if (state.failed) return ;

			}
			break;
		case 28:
			// Grammars\\ANTLR.g3:1:230: ASSIGN
			{
			mASSIGN(); if (state.failed) return ;

			}
			break;
		case 29:
			// Grammars\\ANTLR.g3:1:237: PLUS_ASSIGN
			{
			mPLUS_ASSIGN(); if (state.failed) return ;

			}
			break;
		case 30:
			// Grammars\\ANTLR.g3:1:249: IMPLIES
			{
			mIMPLIES(); if (state.failed) return ;

			}
			break;
		case 31:
			// Grammars\\ANTLR.g3:1:257: REWRITE
			{
			mREWRITE(); if (state.failed) return ;

			}
			break;
		case 32:
			// Grammars\\ANTLR.g3:1:265: SEMI
			{
			mSEMI(); if (state.failed) return ;

			}
			break;
		case 33:
			// Grammars\\ANTLR.g3:1:270: ROOT
			{
			mROOT(); if (state.failed) return ;

			}
			break;
		case 34:
			// Grammars\\ANTLR.g3:1:275: BANG
			{
			mBANG(); if (state.failed) return ;

			}
			break;
		case 35:
			// Grammars\\ANTLR.g3:1:280: OR
			{
			mOR(); if (state.failed) return ;

			}
			break;
		case 36:
			// Grammars\\ANTLR.g3:1:283: WILDCARD
			{
			mWILDCARD(); if (state.failed) return ;

			}
			break;
		case 37:
			// Grammars\\ANTLR.g3:1:292: ETC
			{
			mETC(); if (state.failed) return ;

			}
			break;
		case 38:
			// Grammars\\ANTLR.g3:1:296: RANGE
			{
			mRANGE(); if (state.failed) return ;

			}
			break;
		case 39:
			// Grammars\\ANTLR.g3:1:302: NOT
			{
			mNOT(); if (state.failed) return ;

			}
			break;
		case 40:
			// Grammars\\ANTLR.g3:1:306: RCURLY
			{
			mRCURLY(); if (state.failed) return ;

			}
			break;
		case 41:
			// Grammars\\ANTLR.g3:1:313: DOLLAR
			{
			mDOLLAR(); if (state.failed) return ;

			}
			break;
		case 42:
			// Grammars\\ANTLR.g3:1:320: STRAY_BRACKET
			{
			mSTRAY_BRACKET(); if (state.failed) return ;

			}
			break;
		case 43:
			// Grammars\\ANTLR.g3:1:334: CHAR_LITERAL
			{
			mCHAR_LITERAL(); if (state.failed) return ;

			}
			break;
		case 44:
			// Grammars\\ANTLR.g3:1:347: DOUBLE_QUOTE_STRING_LITERAL
			{
			mDOUBLE_QUOTE_STRING_LITERAL(); if (state.failed) return ;

			}
			break;
		case 45:
			// Grammars\\ANTLR.g3:1:375: DOUBLE_ANGLE_STRING_LITERAL
			{
			mDOUBLE_ANGLE_STRING_LITERAL(); if (state.failed) return ;

			}
			break;
		case 46:
			// Grammars\\ANTLR.g3:1:403: INT
			{
			mINT(); if (state.failed) return ;

			}
			break;
		case 47:
			// Grammars\\ANTLR.g3:1:407: ARG_ACTION
			{
			mARG_ACTION(); if (state.failed) return ;

			}
			break;
		case 48:
			// Grammars\\ANTLR.g3:1:418: ACTION
			{
			mACTION(); if (state.failed) return ;

			}
			break;
		case 49:
			// Grammars\\ANTLR.g3:1:425: TOKEN_REF
			{
			mTOKEN_REF(); if (state.failed) return ;

			}
			break;
		case 50:
			// Grammars\\ANTLR.g3:1:435: TOKENS
			{
			mTOKENS(); if (state.failed) return ;

			}
			break;
		case 51:
			// Grammars\\ANTLR.g3:1:442: OPTIONS
			{
			mOPTIONS(); if (state.failed) return ;

			}
			break;
		case 52:
			// Grammars\\ANTLR.g3:1:450: RULE_REF
			{
			mRULE_REF(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_ANTLR
	public void synpred1_ANTLR_fragment()
	{
		// Grammars\\ANTLR.g3:841:5: ( ' $ANTLR' )
		// Grammars\\ANTLR.g3:841:6: ' $ANTLR'
		{
		Match(" $ANTLR"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred1_ANTLR

	// $ANTLR start synpred2_ANTLR
	public void synpred2_ANTLR_fragment()
	{
		// Grammars\\ANTLR.g3:940:5: ( '\\\\\\\"' )
		// Grammars\\ANTLR.g3:940:6: '\\\\\\\"'
		{
		Match("\\\""); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred2_ANTLR

	// $ANTLR start synpred3_ANTLR
	public void synpred3_ANTLR_fragment()
	{
		// Grammars\\ANTLR.g3:1007:6: ( '\\\\]' )
		// Grammars\\ANTLR.g3:1007:7: '\\\\]'
		{
		Match("\\]"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred3_ANTLR

	// $ANTLR start synpred4_ANTLR
	public void synpred4_ANTLR_fragment()
	{
		// Grammars\\ANTLR.g3:1044:5: ( '//' | '/*' )
		int alt26=2;
		int LA26_0 = input.LA(1);

		if ( (LA26_0=='/') )
		{
			int LA26_1 = input.LA(2);

			if ( (LA26_1=='/') )
			{
				alt26=1;
			}
			else if ( (LA26_1=='*') )
			{
				alt26=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 26, 1, input);

				throw nvae;
			}
		}
		else
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			NoViableAltException nvae = new NoViableAltException("", 26, 0, input);

			throw nvae;
		}
		switch ( alt26 )
		{
		case 1:
			// Grammars\\ANTLR.g3:1044:6: '//'
			{
			Match("//"); if (state.failed) return ;


			}
			break;
		case 2:
			// Grammars\\ANTLR.g3:1044:13: '/*'
			{
			Match("/*"); if (state.failed) return ;


			}
			break;

		}}
	// $ANTLR end synpred4_ANTLR

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
	DFA9 dfa9;
	DFA25 dfa25;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa9 = new DFA9( this, new SpecialStateTransitionHandler( specialStateTransition9 ) );
		dfa25 = new DFA25( this );
	}

	class DFA9 : DFA
	{

		const string DFA9_eotS =
			"\x2\x2\x1\xFFFF\xD\x2\x1\xFFFF\x5\x2\x1\xFFFF\x2\x2\x3\xFFFF\x1\x2\x1"+
			"\xFFFF";
		const string DFA9_eofS =
			"\x1E\xFFFF";
		const string DFA9_minS =
			"\x1\x20\x1\x24\x1\xFFFF\x1\x41\x1\x4E\x1\x54\x1\x4C\x1\x52\x1\x20\x1"+
			"\x73\x1\x72\x1\x63\x1\x20\x1\x22\x3\x0\x1\x20\x7\x0\x3\xFFFF\x1\x30\x1"+
			"\x0";
		const string DFA9_maxS =
			"\x1\x20\x1\x24\x1\xFFFF\x1\x41\x1\x4E\x1\x54\x1\x4C\x1\x52\x1\x20\x1"+
			"\x73\x1\x72\x1\x63\x1\x20\x1\x22\x3\xFFFF\x1\x20\x7\xFFFF\x3\xFFFF\x1"+
			"\x39\x1\x0";
		const string DFA9_acceptS =
			"\x2\xFFFF\x1\x2\x16\xFFFF\x3\x1\x2\xFFFF";
		const string DFA9_specialS =
			"\xE\xFFFF\x1\x0\x1\x1\x1\x2\x1\xFFFF\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1"+
			"\x8\x1\x9\x4\xFFFF\x1\xA}>";
		static readonly string[] DFA9_transitionS =
			{
				"\x1\x1",
				"\x1\x3",
				"",
				"\x1\x4",
				"\x1\x5",
				"\x1\x6",
				"\x1\x7",
				"\x1\x8",
				"\x1\x9",
				"\x1\xA",
				"\x1\xB",
				"\x1\xC",
				"\x1\xD",
				"\x1\xE",
				"\xA\x13\x1\x12\x2\x13\x1\x10\x14\x13\x1\x11\x39\x13\x1\xF\xFFA3\x13",
				"\xA\x18\x1\x17\x2\x18\x1\x16\x14\x18\x1\x15\x4\x18\x1\x14\xFFD8\x18",
				"\xA\x1B\x1\x12\x17\x1B\x1\x19\x39\x1B\x1\x1A\xFFA3\x1B",
				"\x1\x1C",
				"\x22\x1B\x1\x19\x39\x1B\x1\x1A\xFFA3\x1B",
				"\xA\x13\x1\x12\x2\x13\x1\x10\x14\x13\x1\x11\x39\x13\x1\xF\xFFA3\x13",
				"\xA\x13\x1\x12\x2\x13\x1\x10\x14\x13\x1\x11\x39\x13\x1\xF\xFFA3\x13",
				"\xA\x13\x1\x12\x2\x13\x1\x10\x14\x13\x1\x11\x39\x13\x1\xF\xFFA3\x13",
				"\xA\x1B\x1\x12\x17\x1B\x1\x19\x39\x1B\x1\x1A\xFFA3\x1B",
				"\x22\x1B\x1\x19\x39\x1B\x1\x1A\xFFA3\x1B",
				"\xA\x13\x1\x12\x2\x13\x1\x10\x14\x13\x1\x11\x39\x13\x1\xF\xFFA3\x13",
				"",
				"",
				"",
				"\xA\x1D",
				"\x1\xFFFF"
			};

		static readonly short[] DFA9_eot = DFA.UnpackEncodedString(DFA9_eotS);
		static readonly short[] DFA9_eof = DFA.UnpackEncodedString(DFA9_eofS);
		static readonly char[] DFA9_min = DFA.UnpackEncodedStringToUnsignedChars(DFA9_minS);
		static readonly char[] DFA9_max = DFA.UnpackEncodedStringToUnsignedChars(DFA9_maxS);
		static readonly short[] DFA9_accept = DFA.UnpackEncodedString(DFA9_acceptS);
		static readonly short[] DFA9_special = DFA.UnpackEncodedString(DFA9_specialS);
		static readonly short[][] DFA9_transition;

		static DFA9()
		{
			int numStates = DFA9_transitionS.Length;
			DFA9_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA9_transition[i] = DFA.UnpackEncodedString(DFA9_transitionS[i]);
			}
		}

		public DFA9( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 9;
			this.eot = DFA9_eot;
			this.eof = DFA9_eof;
			this.min = DFA9_min;
			this.max = DFA9_max;
			this.accept = DFA9_accept;
			this.special = DFA9_special;
			this.transition = DFA9_transition;
		}
		public override string GetDescription()
		{
			return "841:3: (=> ' $ANTLR ' SRC ( ( '\\r' )? '\\n' )? | (~ ( '\\r' | '\\n' ) )* ( ( '\\r' )? '\\n' )? )";
		}
	}

	int specialStateTransition9( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA9_14 = input.LA(1);

				s = -1;
				if ( (LA9_14=='\\') ) {s = 15;}

				else if ( (LA9_14=='\r') ) {s = 16;}

				else if ( (LA9_14=='\"') ) {s = 17;}

				else if ( (LA9_14=='\n') ) {s = 18;}

				else if ( ((LA9_14>='\u0000' && LA9_14<='\t')||(LA9_14>='\u000B' && LA9_14<='\f')||(LA9_14>='\u000E' && LA9_14<='!')||(LA9_14>='#' && LA9_14<='[')||(LA9_14>=']' && LA9_14<='\uFFFF')) ) {s = 19;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 1:
				int LA9_15 = input.LA(1);

				s = -1;
				if ( (LA9_15=='\'') ) {s = 20;}

				else if ( (LA9_15=='\"') ) {s = 21;}

				else if ( (LA9_15=='\r') ) {s = 22;}

				else if ( (LA9_15=='\n') ) {s = 23;}

				else if ( ((LA9_15>='\u0000' && LA9_15<='\t')||(LA9_15>='\u000B' && LA9_15<='\f')||(LA9_15>='\u000E' && LA9_15<='!')||(LA9_15>='#' && LA9_15<='&')||(LA9_15>='(' && LA9_15<='\uFFFF')) ) {s = 24;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 2:
				int LA9_16 = input.LA(1);


				int index9_16 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_16=='\"') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 25;}

				else if ( (LA9_16=='\\') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 26;}

				else if ( (LA9_16=='\n') ) {s = 18;}

				else if ( ((LA9_16>='\u0000' && LA9_16<='\t')||(LA9_16>='\u000B' && LA9_16<='!')||(LA9_16>='#' && LA9_16<='[')||(LA9_16>=']' && LA9_16<='\uFFFF')) && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 27;}


				input.Seek(index9_16);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA9_18 = input.LA(1);


				int index9_18 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_18=='\"') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 25;}

				else if ( (LA9_18=='\\') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 26;}

				else if ( ((LA9_18>='\u0000' && LA9_18<='!')||(LA9_18>='#' && LA9_18<='[')||(LA9_18>=']' && LA9_18<='\uFFFF')) && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 27;}

				else s = 2;


				input.Seek(index9_18);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA9_19 = input.LA(1);

				s = -1;
				if ( (LA9_19=='\"') ) {s = 17;}

				else if ( (LA9_19=='\\') ) {s = 15;}

				else if ( (LA9_19=='\r') ) {s = 16;}

				else if ( (LA9_19=='\n') ) {s = 18;}

				else if ( ((LA9_19>='\u0000' && LA9_19<='\t')||(LA9_19>='\u000B' && LA9_19<='\f')||(LA9_19>='\u000E' && LA9_19<='!')||(LA9_19>='#' && LA9_19<='[')||(LA9_19>=']' && LA9_19<='\uFFFF')) ) {s = 19;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 5:
				int LA9_20 = input.LA(1);

				s = -1;
				if ( (LA9_20=='\"') ) {s = 17;}

				else if ( (LA9_20=='\\') ) {s = 15;}

				else if ( (LA9_20=='\r') ) {s = 16;}

				else if ( (LA9_20=='\n') ) {s = 18;}

				else if ( ((LA9_20>='\u0000' && LA9_20<='\t')||(LA9_20>='\u000B' && LA9_20<='\f')||(LA9_20>='\u000E' && LA9_20<='!')||(LA9_20>='#' && LA9_20<='[')||(LA9_20>=']' && LA9_20<='\uFFFF')) ) {s = 19;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 6:
				int LA9_21 = input.LA(1);

				s = -1;
				if ( (LA9_21=='\"') ) {s = 17;}

				else if ( (LA9_21=='\\') ) {s = 15;}

				else if ( (LA9_21=='\r') ) {s = 16;}

				else if ( (LA9_21=='\n') ) {s = 18;}

				else if ( ((LA9_21>='\u0000' && LA9_21<='\t')||(LA9_21>='\u000B' && LA9_21<='\f')||(LA9_21>='\u000E' && LA9_21<='!')||(LA9_21>='#' && LA9_21<='[')||(LA9_21>=']' && LA9_21<='\uFFFF')) ) {s = 19;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 7:
				int LA9_22 = input.LA(1);


				int index9_22 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_22=='\"') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 25;}

				else if ( (LA9_22=='\\') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 26;}

				else if ( (LA9_22=='\n') ) {s = 18;}

				else if ( ((LA9_22>='\u0000' && LA9_22<='\t')||(LA9_22>='\u000B' && LA9_22<='!')||(LA9_22>='#' && LA9_22<='[')||(LA9_22>=']' && LA9_22<='\uFFFF')) && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 27;}


				input.Seek(index9_22);
				if ( s>=0 ) return s;
				break;

			case 8:
				int LA9_23 = input.LA(1);


				int index9_23 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_23=='\"') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 25;}

				else if ( (LA9_23=='\\') && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 26;}

				else if ( ((LA9_23>='\u0000' && LA9_23<='!')||(LA9_23>='#' && LA9_23<='[')||(LA9_23>=']' && LA9_23<='\uFFFF')) && (EvaluatePredicate(synpred1_ANTLR_fragment))) {s = 27;}

				else s = 2;


				input.Seek(index9_23);
				if ( s>=0 ) return s;
				break;

			case 9:
				int LA9_24 = input.LA(1);

				s = -1;
				if ( (LA9_24=='\"') ) {s = 17;}

				else if ( (LA9_24=='\\') ) {s = 15;}

				else if ( (LA9_24=='\r') ) {s = 16;}

				else if ( (LA9_24=='\n') ) {s = 18;}

				else if ( ((LA9_24>='\u0000' && LA9_24<='\t')||(LA9_24>='\u000B' && LA9_24<='\f')||(LA9_24>='\u000E' && LA9_24<='!')||(LA9_24>='#' && LA9_24<='[')||(LA9_24>=']' && LA9_24<='\uFFFF')) ) {s = 19;}

				else s = 2;

				if ( s>=0 ) return s;
				break;

			case 10:
				int LA9_29 = input.LA(1);


				int index9_29 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_ANTLR_fragment)) ) {s = 27;}

				else if ( (true) ) {s = 2;}


				input.Seek(index9_29);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 9, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA25 : DFA
	{

		const string DFA25_eotS =
			"\x1\xFFFF\x9\x28\x2\xFFFF\x1\x38\x4\xFFFF\x1\x3A\x4\xFFFF\x1\x3C\x1\x3E"+
			"\x4\xFFFF\x1\x40\xA\xFFFF\x1\x28\x1\xFFFF\xE\x28\x8\xFFFF\x1\x52\x1\xFFFF"+
			"\x10\x28\x2\xFFFF\xE\x28\x1\x71\x2\x28\x1\x74\x4\x28\x1\x79\x5\x28\x1"+
			"\x7F\x1\x28\x1\xFFFF\x2\x28\x1\xFFFF\x3\x28\x1\x86\x1\xFFFF\x1\x87\x2"+
			"\x28\x1\x8A\x1\x28\x1\xFFFF\x1\x8C\x2\x28\x1\x8F\x1\x28\x1\x91\x2\xFFFF"+
			"\x1\x92\x1\x28\x1\xFFFF\x1\x94\x2\xFFFF\x1\x28\x1\xFFFF\x1\x96\x2\xFFFF"+
			"\x1\x28\x3\xFFFF\x1\x98\x1\xFFFF";
		const string DFA25_eofS =
			"\x99\xFFFF";
		const string DFA25_minS =
			"\x1\x9\x1\x61\x1\x69\x1\x72\x1\x6D\x1\x65\x1\x61\x1\x65\x1\x63\x1\x68"+
			"\x2\xFFFF\x1\x3C\x4\xFFFF\x1\x28\x4\xFFFF\x1\x3D\x1\x3E\x4\xFFFF\x1\x2E"+
			"\xA\xFFFF\x1\x70\x1\xFFFF\x1\x74\x1\x6E\x2\x61\x1\x70\x1\x78\x1\x72\x1"+
			"\x69\x1\x62\x1\x74\x1\x6F\x1\x72\x1\x65\x1\x6B\x8\xFFFF\x1\x2E\x1\xFFFF"+
			"\x1\x74\x1\x63\x1\x61\x1\x67\x1\x6D\x1\x6F\x1\x65\x1\x73\x1\x76\x1\x74"+
			"\x1\x6C\x1\x75\x1\x70\x1\x6F\x2\x65\x2\xFFFF\x1\x69\x1\x68\x1\x6C\x2"+
			"\x6D\x2\x72\x1\x65\x1\x61\x1\x65\x1\x69\x1\x72\x1\x65\x1\x77\x1\x30\x1"+
			"\x6E\x1\x6F\x1\x30\x1\x6C\x1\x65\x1\x61\x1\x74\x1\x30\x1\x72\x1\x74\x2"+
			"\x63\x1\x6E\x1\x30\x1\x73\x1\xFFFF\x1\x73\x1\x6E\x1\xFFFF\x1\x79\x1\x6E"+
			"\x1\x72\x1\x30\x1\xFFFF\x1\x30\x1\x65\x1\x74\x1\x30\x1\x73\x1\xFFFF\x1"+
			"\x30\x1\x9\x1\x73\x1\x30\x1\x74\x1\x30\x2\xFFFF\x1\x30\x1\x65\x1\xFFFF"+
			"\x1\x30\x2\xFFFF\x1\x9\x1\xFFFF\x1\x30\x2\xFFFF\x1\x64\x3\xFFFF\x1\x30"+
			"\x1\xFFFF";
		const string DFA25_maxS =
			"\x1\x7E\x1\x61\x2\x72\x1\x6D\x1\x65\x1\x75\x1\x65\x1\x63\x1\x72\x2\xFFFF"+
			"\x1\x3C\x4\xFFFF\x1\x28\x4\xFFFF\x1\x3D\x1\x3E\x4\xFFFF\x1\x2E\xA\xFFFF"+
			"\x1\x70\x1\xFFFF\x1\x74\x1\x6E\x2\x61\x1\x70\x1\x78\x1\x72\x1\x6F\x1"+
			"\x62\x1\x74\x1\x6F\x1\x72\x1\x65\x1\x6B\x8\xFFFF\x1\x2E\x1\xFFFF\x1\x74"+
			"\x1\x63\x1\x61\x1\x67\x1\x6D\x1\x6F\x1\x65\x1\x73\x1\x76\x1\x74\x1\x6C"+
			"\x1\x75\x1\x70\x1\x6F\x2\x65\x2\xFFFF\x1\x69\x1\x68\x1\x6C\x2\x6D\x2"+
			"\x72\x1\x65\x1\x61\x1\x65\x1\x69\x1\x72\x1\x65\x1\x77\x1\x7A\x1\x6E\x1"+
			"\x6F\x1\x7A\x1\x6C\x1\x65\x1\x61\x1\x74\x1\x7A\x1\x72\x1\x74\x2\x63\x1"+
			"\x6E\x1\x7A\x1\x73\x1\xFFFF\x1\x73\x1\x6E\x1\xFFFF\x1\x79\x1\x6E\x1\x72"+
			"\x1\x7A\x1\xFFFF\x1\x7A\x1\x65\x1\x74\x1\x7A\x1\x73\x1\xFFFF\x1\x7A\x1"+
			"\x7B\x1\x73\x1\x7A\x1\x74\x1\x7A\x2\xFFFF\x1\x7A\x1\x65\x1\xFFFF\x1\x7A"+
			"\x2\xFFFF\x1\x7B\x1\xFFFF\x1\x7A\x2\xFFFF\x1\x64\x3\xFFFF\x1\x7A\x1\xFFFF";
		const string DFA25_acceptS =
			"\xA\xFFFF\x1\xF\x1\x10\x1\xFFFF\x1\x12\x1\x13\x1\x14\x1\x15\x1\xFFFF"+
			"\x1\x17\x1\x18\x1\x19\x1\x1A\x2\xFFFF\x1\x1F\x1\x20\x1\x22\x1\x23\x1"+
			"\xFFFF\x1\x27\x1\x28\x1\x29\x1\x2A\x1\x2B\x1\x2C\x1\x2E\x1\x2F\x1\x30"+
			"\x1\x31\x1\xFFFF\x1\x34\xE\xFFFF\x1\x2D\x1\x11\x1\x16\x1\x21\x1\x1D\x1"+
			"\x1B\x1\x1E\x1\x1C\x1\xFFFF\x1\x24\x10\xFFFF\x1\x25\x1\x26\x1E\xFFFF"+
			"\x1\xE\x2\xFFFF\x1\x1\x4\xFFFF\x1\x6\x5\xFFFF\x1\xC\x6\xFFFF\x1\x5\x1"+
			"\x7\x2\xFFFF\x1\xA\x1\xFFFF\x1\xD\x1\x32\x1\xFFFF\x1\x2\x1\xFFFF\x1\x4"+
			"\x1\x8\x1\xFFFF\x1\xB\x1\x33\x1\x3\x1\xFFFF\x1\x9";
		const string DFA25_specialS =
			"\x99\xFFFF}>";
		static readonly string[] DFA25_transitionS =
			{
				"\x2\xA\x2\xFFFF\x1\xA\x12\xFFFF\x1\xA\x1\x1A\x1\x22\x1\xFFFF\x1\x1F"+
				"\x2\xFFFF\x1\x21\x1\x12\x1\x13\x1\x15\x1\x16\x1\xF\x1\x18\x1\x1C\x1"+
				"\xB\xA\x23\x1\x14\x1\x19\x1\xC\x1\x17\x1\xD\x1\x10\x1\xE\x1A\x26\x1"+
				"\x24\x1\xFFFF\x1\x20\x1\x11\x2\xFFFF\x2\x28\x1\x1\x2\x28\x1\x2\x1\x3"+
				"\x1\x28\x1\x4\x2\x28\x1\x5\x2\x28\x1\x27\x1\x6\x1\x28\x1\x7\x1\x8\x1"+
				"\x9\x6\x28\x1\x25\x1\x1B\x1\x1E\x1\x1D",
				"\x1\x29",
				"\x1\x2A\x8\xFFFF\x1\x2B",
				"\x1\x2C",
				"\x1\x2D",
				"\x1\x2E",
				"\x1\x2F\x10\xFFFF\x1\x30\x2\xFFFF\x1\x31",
				"\x1\x32",
				"\x1\x33",
				"\x1\x34\x6\xFFFF\x1\x36\x2\xFFFF\x1\x35",
				"",
				"",
				"\x1\x37",
				"",
				"",
				"",
				"",
				"\x1\x39",
				"",
				"",
				"",
				"",
				"\x1\x3B",
				"\x1\x3D",
				"",
				"",
				"",
				"",
				"\x1\x3F",
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
				"\x1\x41",
				"",
				"\x1\x42",
				"\x1\x43",
				"\x1\x44",
				"\x1\x45",
				"\x1\x46",
				"\x1\x47",
				"\x1\x48",
				"\x1\x49\x5\xFFFF\x1\x4A",
				"\x1\x4B",
				"\x1\x4C",
				"\x1\x4D",
				"\x1\x4E",
				"\x1\x4F",
				"\x1\x50",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x51",
				"",
				"\x1\x53",
				"\x1\x54",
				"\x1\x55",
				"\x1\x56",
				"\x1\x57",
				"\x1\x58",
				"\x1\x59",
				"\x1\x5A",
				"\x1\x5B",
				"\x1\x5C",
				"\x1\x5D",
				"\x1\x5E",
				"\x1\x5F",
				"\x1\x60",
				"\x1\x61",
				"\x1\x62",
				"",
				"",
				"\x1\x63",
				"\x1\x64",
				"\x1\x65",
				"\x1\x66",
				"\x1\x67",
				"\x1\x68",
				"\x1\x69",
				"\x1\x6A",
				"\x1\x6B",
				"\x1\x6C",
				"\x1\x6D",
				"\x1\x6E",
				"\x1\x6F",
				"\x1\x70",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x72",
				"\x1\x73",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x75",
				"\x1\x76",
				"\x1\x77",
				"\x1\x78",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x7A",
				"\x1\x7B",
				"\x1\x7C",
				"\x1\x7D",
				"\x1\x7E",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x80",
				"",
				"\x1\x81",
				"\x1\x82",
				"",
				"\x1\x83",
				"\x1\x84",
				"\x1\x85",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x88",
				"\x1\x89",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x8B",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x2\x8D\x2\xFFFF\x1\x8D\x12\xFFFF\x1\x8D\xE\xFFFF\x1\x8D\x4B\xFFFF\x1"+
				"\x8D",
				"\x1\x8E",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x90",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"\x1\x93",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"",
				"",
				"\x2\x95\x2\xFFFF\x1\x95\x12\xFFFF\x1\x95\xE\xFFFF\x1\x95\x4B\xFFFF\x1"+
				"\x95",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				"",
				"",
				"\x1\x97",
				"",
				"",
				"",
				"\xA\x28\x7\xFFFF\x1A\x28\x4\xFFFF\x1\x28\x1\xFFFF\x1A\x28",
				""
			};

		static readonly short[] DFA25_eot = DFA.UnpackEncodedString(DFA25_eotS);
		static readonly short[] DFA25_eof = DFA.UnpackEncodedString(DFA25_eofS);
		static readonly char[] DFA25_min = DFA.UnpackEncodedStringToUnsignedChars(DFA25_minS);
		static readonly char[] DFA25_max = DFA.UnpackEncodedStringToUnsignedChars(DFA25_maxS);
		static readonly short[] DFA25_accept = DFA.UnpackEncodedString(DFA25_acceptS);
		static readonly short[] DFA25_special = DFA.UnpackEncodedString(DFA25_specialS);
		static readonly short[][] DFA25_transition;

		static DFA25()
		{
			int numStates = DFA25_transitionS.Length;
			DFA25_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA25_transition[i] = DFA.UnpackEncodedString(DFA25_transitionS[i]);
			}
		}

		public DFA25( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 25;
			this.eot = DFA25_eot;
			this.eof = DFA25_eof;
			this.min = DFA25_min;
			this.max = DFA25_max;
			this.accept = DFA25_accept;
			this.special = DFA25_special;
			this.transition = DFA25_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens : ( CATCH | FINALLY | FRAGMENT | GRAMMAR | IMPORT | LEXER | PARSER | PRIVATE | PROTECTED | PUBLIC | RETURNS | SCOPE | THROWS | TREE | WS | COMMENT | OPEN_ELEMENT_OPTION | CLOSE_ELEMENT_OPTION | AMPERSAND | COMMA | QUESTION | TREE_BEGIN | LPAREN | RPAREN | COLON | STAR | PLUS | ASSIGN | PLUS_ASSIGN | IMPLIES | REWRITE | SEMI | ROOT | BANG | OR | WILDCARD | ETC | RANGE | NOT | RCURLY | DOLLAR | STRAY_BRACKET | CHAR_LITERAL | DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL | INT | ARG_ACTION | ACTION | TOKEN_REF | TOKENS | OPTIONS | RULE_REF );";
		}
	}

 
	#endregion

}

} // namespace Antlr3.Grammars
