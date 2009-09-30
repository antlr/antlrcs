// $ANTLR 3.1.2 Language\\Action.g3 2009-09-30 13:22:28

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


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace  Antlr3.ST.Language 
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class ActionLexer : Lexer
{
	public const int EOF=-1;
	public const int ANONYMOUS_TEMPLATE=4;
	public const int APPLY=5;
	public const int ARGS=6;
	public const int ASSIGN=7;
	public const int COLON=8;
	public const int COMMA=9;
	public const int CONDITIONAL=10;
	public const int DOT=11;
	public const int DOTDOTDOT=12;
	public const int ELSEIF=13;
	public const int ESC_CHAR=14;
	public const int FIRST=15;
	public const int FUNCTION=16;
	public const int ID=17;
	public const int INCLUDE=18;
	public const int INT=19;
	public const int LAST=20;
	public const int LBRACK=21;
	public const int LENGTH=22;
	public const int LIST=23;
	public const int LPAREN=24;
	public const int MULTI_APPLY=25;
	public const int NESTED_ANONYMOUS_TEMPLATE=26;
	public const int NEWLINE=27;
	public const int NOT=28;
	public const int NOTHING=29;
	public const int PLUS=30;
	public const int RBRACK=31;
	public const int REST=32;
	public const int RPAREN=33;
	public const int SEMI=34;
	public const int SINGLEVALUEARG=35;
	public const int STRING=36;
	public const int STRIP=37;
	public const int SUPER=38;
	public const int TEMPLATE=39;
	public const int TEMPLATE_ARGS=40;
	public const int TRUNC=41;
	public const int VALUE=42;
	public const int WS=43;
	public const int WS_CHAR=44;

    // delegates
    // delegators

	public ActionLexer() {}
	public ActionLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ActionLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Language\\Action.g3"; } }

	// $ANTLR start "CONDITIONAL"
	private void mCONDITIONAL()
	{
		try
		{
			int _type = CONDITIONAL;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:42:15: ( 'if' )
			// Language\\Action.g3:42:15: 'if'
			{
			Match("if"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CONDITIONAL"

	// $ANTLR start "ELSEIF"
	private void mELSEIF()
	{
		try
		{
			int _type = ELSEIF;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:43:10: ( 'elseif' )
			// Language\\Action.g3:43:10: 'elseif'
			{
			Match("elseif"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ELSEIF"

	// $ANTLR start "FIRST"
	private void mFIRST()
	{
		try
		{
			int _type = FIRST;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:44:9: ( 'first' )
			// Language\\Action.g3:44:9: 'first'
			{
			Match("first"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "FIRST"

	// $ANTLR start "LAST"
	private void mLAST()
	{
		try
		{
			int _type = LAST;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:45:8: ( 'last' )
			// Language\\Action.g3:45:8: 'last'
			{
			Match("last"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LAST"

	// $ANTLR start "LENGTH"
	private void mLENGTH()
	{
		try
		{
			int _type = LENGTH;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:46:10: ( 'length' )
			// Language\\Action.g3:46:10: 'length'
			{
			Match("length"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LENGTH"

	// $ANTLR start "REST"
	private void mREST()
	{
		try
		{
			int _type = REST;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:47:8: ( 'rest' )
			// Language\\Action.g3:47:8: 'rest'
			{
			Match("rest"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "REST"

	// $ANTLR start "STRIP"
	private void mSTRIP()
	{
		try
		{
			int _type = STRIP;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:48:9: ( 'strip' )
			// Language\\Action.g3:48:9: 'strip'
			{
			Match("strip"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "STRIP"

	// $ANTLR start "SUPER"
	private void mSUPER()
	{
		try
		{
			int _type = SUPER;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:49:9: ( 'super' )
			// Language\\Action.g3:49:9: 'super'
			{
			Match("super"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SUPER"

	// $ANTLR start "TRUNC"
	private void mTRUNC()
	{
		try
		{
			int _type = TRUNC;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:50:9: ( 'trunc' )
			// Language\\Action.g3:50:9: 'trunc'
			{
			Match("trunc"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TRUNC"

	// $ANTLR start "ID"
	private void mID()
	{
		try
		{
			int _type = ID;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:335:4: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' | '/' )* )
			// Language\\Action.g3:335:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' | '/' )*
			{
			if ( (input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z') )
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

			// Language\\Action.g3:335:28: ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' | '/' )*
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( ((LA1_0>='/' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')) )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// Language\\Action.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;



			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ID"

	// $ANTLR start "INT"
	private void mINT()
	{
		try
		{
			int _type = INT;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:339:4: ( ( '0' .. '9' )+ )
			// Language\\Action.g3:339:4: ( '0' .. '9' )+
			{
			// Language\\Action.g3:339:4: ( '0' .. '9' )+
			int cnt2=0;
			for ( ; ; )
			{
				int alt2=2;
				int LA2_0 = input.LA(1);

				if ( ((LA2_0>='0' && LA2_0<='9')) )
				{
					alt2=1;
				}


				switch ( alt2 )
				{
				case 1:
					// Language\\Action.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt2 >= 1 )
						goto loop2;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee2 = new EarlyExitException( 2, input );
					throw eee2;
				}
				cnt2++;
			}
			loop2:
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

	// $ANTLR start "STRING"
	private void mSTRING()
	{
		try
		{
			int _type = STRING;
			int _channel = DefaultTokenChannel;
			int ch;


				char uc = '\0';
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Action.g3:348:4: ( '\"' ( ESC_CHAR[out uc, true] |ch=~ ( '\\\\' | '\"' ) )* '\"' )
			// Language\\Action.g3:348:4: '\"' ( ESC_CHAR[out uc, true] |ch=~ ( '\\\\' | '\"' ) )* '\"'
			{
			Match('\"'); if (state.failed) return ;
			// Language\\Action.g3:349:3: ( ESC_CHAR[out uc, true] |ch=~ ( '\\\\' | '\"' ) )*
			for ( ; ; )
			{
				int alt3=3;
				int LA3_0 = input.LA(1);

				if ( (LA3_0=='\\') )
				{
					alt3=1;
				}
				else if ( ((LA3_0>='\u0000' && LA3_0<='!')||(LA3_0>='#' && LA3_0<='[')||(LA3_0>=']' && LA3_0<='\uFFFF')) )
				{
					alt3=2;
				}


				switch ( alt3 )
				{
				case 1:
					// Language\\Action.g3:349:5: ESC_CHAR[out uc, true]
					{
					mESC_CHAR(out uc, true); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						builder.Append(uc);
					}

					}
					break;
				case 2:
					// Language\\Action.g3:350:5: ch=~ ( '\\\\' | '\"' )
					{
					ch= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append((char)ch);
					}

					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;


			Match('\"'); if (state.failed) return ;
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
	// $ANTLR end "STRING"

	// $ANTLR start "ANONYMOUS_TEMPLATE"
	private void mANONYMOUS_TEMPLATE()
	{
		try
		{
			int _type = ANONYMOUS_TEMPLATE;
			int _channel = DefaultTokenChannel;
			CommonToken ESC_CHAR1=null;
			int ch;


				StringTemplateToken t = null;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				List<string> args = new List<string>();
				string subtext = string.Empty;
				char uc = '\0';

			// Language\\Action.g3:367:4: ( '{' (=> TEMPLATE_ARGS[out subtext, args] (=> WS_CHAR )? |) (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '\\\\' | '{' | '}' ) )* '}' )
			// Language\\Action.g3:367:4: '{' (=> TEMPLATE_ARGS[out subtext, args] (=> WS_CHAR )? |) (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '\\\\' | '{' | '}' ) )* '}'
			{
			Match('{'); if (state.failed) return ;
			// Language\\Action.g3:368:3: (=> TEMPLATE_ARGS[out subtext, args] (=> WS_CHAR )? |)
			int alt5=2;
			alt5 = dfa5.Predict(input);
			switch ( alt5 )
			{
			case 1:
				// Language\\Action.g3:368:5: => TEMPLATE_ARGS[out subtext, args] (=> WS_CHAR )?
				{

				mTEMPLATE_ARGS(out subtext, args); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					builder.Append(subtext);
				}
				// Language\\Action.g3:368:82: (=> WS_CHAR )?
				int alt4=2;
				switch ( input.LA(1) )
				{
				case ' ':
					{
					int LA4_1 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_Action_fragment)) )
					{
						alt4=1;
					}
					}
					break;
				case '\t':
					{
					int LA4_2 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_Action_fragment)) )
					{
						alt4=1;
					}
					}
					break;
				case '\r':
					{
					int LA4_3 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_Action_fragment)) )
					{
						alt4=1;
					}
					}
					break;
				case '\n':
					{
					int LA4_4 = input.LA(2);

					if ( (EvaluatePredicate(synpred2_Action_fragment)) )
					{
						alt4=1;
					}
					}
					break;
				}

				switch ( alt4 )
				{
				case 1:
					// Language\\Action.g3:368:83: => WS_CHAR
					{

					mWS_CHAR(); if (state.failed) return ;

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

									// create a special token to track args
									t = new StringTemplateToken(ANONYMOUS_TEMPLATE,Text,args);
									//setToken(t);
									state.token = t;
								
				}

				}
				break;
			case 2:
				// Language\\Action.g3:376:3: 
				{


				}
				break;

			}

			// Language\\Action.g3:377:3: (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '\\\\' | '{' | '}' ) )*
			for ( ; ; )
			{
				int alt6=6;
				alt6 = dfa6.Predict(input);
				switch ( alt6 )
				{
				case 1:
					// Language\\Action.g3:377:5: => '\\\\{'
					{

					Match("\\{"); if (state.failed) return ;

					if ( state.backtracking == 0 )
					{
						builder.Append( '{' );
					}

					}
					break;
				case 2:
					// Language\\Action.g3:378:5: => '\\\\}'
					{

					Match("\\}"); if (state.failed) return ;

					if ( state.backtracking == 0 )
					{
						builder.Append( '}' );
					}

					}
					break;
				case 3:
					// Language\\Action.g3:379:5: ESC_CHAR[out uc, false]
					{
					int ESC_CHAR1Start300 = CharIndex;
					mESC_CHAR(out uc, false); if (state.failed) return ;
					ESC_CHAR1 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ESC_CHAR1Start300, CharIndex-1);
					if ( state.backtracking == 0 )
					{
						builder.Append( (ESC_CHAR1!=null?ESC_CHAR1.Text:null) );
					}

					}
					break;
				case 4:
					// Language\\Action.g3:380:5: NESTED_ANONYMOUS_TEMPLATE[out subtext]
					{
					mNESTED_ANONYMOUS_TEMPLATE(out subtext); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						builder.Append(subtext);
					}

					}
					break;
				case 5:
					// Language\\Action.g3:381:5: ch=~ ( '\\\\' | '{' | '}' )
					{
					ch= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append((char)ch);
					}

					}
					break;

				default:
					goto loop6;
				}
			}

			loop6:
				;


			if ( state.backtracking == 0 )
			{

							Text = builder.ToString();
							if ( t!=null )
								t.Text = Text;
						
			}
			Match('}'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ANONYMOUS_TEMPLATE"

	// $ANTLR start "TEMPLATE_ARGS"
	private void mTEMPLATE_ARGS(out string _text, List<string> args)
	{
		try
		{
			CommonToken a=null;
			CommonToken a2=null;


				_text = string.Empty; // this template is empty text
				args.Clear();

			// Language\\Action.g3:399:4: ( ( WS_CHAR )? a= ID ( ( WS_CHAR )? ',' ( WS_CHAR )? a2= ID )* ( WS_CHAR )? '|' )
			// Language\\Action.g3:399:4: ( WS_CHAR )? a= ID ( ( WS_CHAR )? ',' ( WS_CHAR )? a2= ID )* ( WS_CHAR )? '|'
			{
			// Language\\Action.g3:399:4: ( WS_CHAR )?
			int alt7=2;
			int LA7_0 = input.LA(1);

			if ( ((LA7_0>='\t' && LA7_0<='\n')||LA7_0=='\r'||LA7_0==' ') )
			{
				alt7=1;
			}
			switch ( alt7 )
			{
			case 1:
				// Language\\Action.g3:399:5: WS_CHAR
				{
				mWS_CHAR(); if (state.failed) return ;

				}
				break;

			}

			int aStart368 = CharIndex;
			mID(); if (state.failed) return ;
			a = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, aStart368, CharIndex-1);
			if ( state.backtracking == 0 )
			{
				args.Add((a!=null?a.Text:null));
			}
			// Language\\Action.g3:400:3: ( ( WS_CHAR )? ',' ( WS_CHAR )? a2= ID )*
			for ( ; ; )
			{
				int alt10=2;
				switch ( input.LA(1) )
				{
				case ' ':
					{
					int LA10_1 = input.LA(2);

					if ( (LA10_1==',') )
					{
						alt10=1;
					}


					}
					break;
				case '\t':
					{
					int LA10_2 = input.LA(2);

					if ( (LA10_2==',') )
					{
						alt10=1;
					}


					}
					break;
				case '\r':
					{
					int LA10_3 = input.LA(2);

					if ( (LA10_3=='\n') )
					{
						int LA10_7 = input.LA(3);

						if ( (LA10_7==',') )
						{
							alt10=1;
						}


					}
					else if ( (LA10_3==',') )
					{
						alt10=1;
					}


					}
					break;
				case '\n':
					{
					int LA10_4 = input.LA(2);

					if ( (LA10_4==',') )
					{
						alt10=1;
					}


					}
					break;
				case ',':
					{
					alt10=1;
					}
					break;

				}

				switch ( alt10 )
				{
				case 1:
					// Language\\Action.g3:400:5: ( WS_CHAR )? ',' ( WS_CHAR )? a2= ID
					{
					// Language\\Action.g3:400:5: ( WS_CHAR )?
					int alt8=2;
					int LA8_0 = input.LA(1);

					if ( ((LA8_0>='\t' && LA8_0<='\n')||LA8_0=='\r'||LA8_0==' ') )
					{
						alt8=1;
					}
					switch ( alt8 )
					{
					case 1:
						// Language\\Action.g3:400:6: WS_CHAR
						{
						mWS_CHAR(); if (state.failed) return ;

						}
						break;

					}

					Match(','); if (state.failed) return ;
					// Language\\Action.g3:400:20: ( WS_CHAR )?
					int alt9=2;
					int LA9_0 = input.LA(1);

					if ( ((LA9_0>='\t' && LA9_0<='\n')||LA9_0=='\r'||LA9_0==' ') )
					{
						alt9=1;
					}
					switch ( alt9 )
					{
					case 1:
						// Language\\Action.g3:400:21: WS_CHAR
						{
						mWS_CHAR(); if (state.failed) return ;

						}
						break;

					}

					int a2Start390 = CharIndex;
					mID(); if (state.failed) return ;
					a2 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, a2Start390, CharIndex-1);
					if ( state.backtracking == 0 )
					{
						args.Add((a2!=null?a2.Text:null));
					}

					}
					break;

				default:
					goto loop10;
				}
			}

			loop10:
				;


			// Language\\Action.g3:401:3: ( WS_CHAR )?
			int alt11=2;
			int LA11_0 = input.LA(1);

			if ( ((LA11_0>='\t' && LA11_0<='\n')||LA11_0=='\r'||LA11_0==' ') )
			{
				alt11=1;
			}
			switch ( alt11 )
			{
			case 1:
				// Language\\Action.g3:401:4: WS_CHAR
				{
				mWS_CHAR(); if (state.failed) return ;

				}
				break;

			}

			Match('|'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "TEMPLATE_ARGS"

	// $ANTLR start "NESTED_ANONYMOUS_TEMPLATE"
	private void mNESTED_ANONYMOUS_TEMPLATE(out string _text)
	{
		try
		{
			CommonToken ESC_CHAR2=null;
			int ch;


				_text = string.Empty;
				char uc = '\0';
				string subtext = string.Empty;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Action.g3:413:4: ( '{' (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '{' | '}' | '\\\\' ) )* '}' )
			// Language\\Action.g3:413:4: '{' (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '{' | '}' | '\\\\' ) )* '}'
			{
			Match('{'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				builder.Append('{');
			}
			// Language\\Action.g3:415:3: (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '{' | '}' | '\\\\' ) )*
			for ( ; ; )
			{
				int alt12=6;
				alt12 = dfa12.Predict(input);
				switch ( alt12 )
				{
				case 1:
					// Language\\Action.g3:415:5: => '\\\\{'
					{

					Match("\\{"); if (state.failed) return ;

					if ( state.backtracking == 0 )
					{
						builder.Append('{');
					}

					}
					break;
				case 2:
					// Language\\Action.g3:416:5: => '\\\\}'
					{

					Match("\\}"); if (state.failed) return ;

					if ( state.backtracking == 0 )
					{
						builder.Append('}');
					}

					}
					break;
				case 3:
					// Language\\Action.g3:417:5: ESC_CHAR[out uc, false]
					{
					int ESC_CHAR2Start460 = CharIndex;
					mESC_CHAR(out uc, false); if (state.failed) return ;
					ESC_CHAR2 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ESC_CHAR2Start460, CharIndex-1);
					if ( state.backtracking == 0 )
					{
						builder.Append((ESC_CHAR2!=null?ESC_CHAR2.Text:null));
					}

					}
					break;
				case 4:
					// Language\\Action.g3:418:5: NESTED_ANONYMOUS_TEMPLATE[out subtext]
					{
					mNESTED_ANONYMOUS_TEMPLATE(out subtext); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						builder.Append(subtext);
					}

					}
					break;
				case 5:
					// Language\\Action.g3:419:5: ch=~ ( '{' | '}' | '\\\\' )
					{
					ch= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						builder.Append((char)ch);
					}

					}
					break;

				default:
					goto loop12;
				}
			}

			loop12:
				;


			Match('}'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							builder.Append('}');
							_text = builder.ToString();
						
			}

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "NESTED_ANONYMOUS_TEMPLATE"

	// $ANTLR start "ESC_CHAR"
	private void mESC_CHAR(out char uc, bool doEscape)
	{
		try
		{
			int c;


				uc = '\0';

			// Language\\Action.g3:440:4: ( '\\\\' ( 'n' | 'r' | 't' | 'b' | 'f' |c=~ ( 'n' | 'r' | 't' | 'b' | 'f' ) ) )
			// Language\\Action.g3:440:4: '\\\\' ( 'n' | 'r' | 't' | 'b' | 'f' |c=~ ( 'n' | 'r' | 't' | 'b' | 'f' ) )
			{
			Match('\\'); if (state.failed) return ;
			// Language\\Action.g3:441:3: ( 'n' | 'r' | 't' | 'b' | 'f' |c=~ ( 'n' | 'r' | 't' | 'b' | 'f' ) )
			int alt13=6;
			int LA13_0 = input.LA(1);

			if ( (LA13_0=='n') )
			{
				alt13=1;
			}
			else if ( (LA13_0=='r') )
			{
				alt13=2;
			}
			else if ( (LA13_0=='t') )
			{
				alt13=3;
			}
			else if ( (LA13_0=='b') )
			{
				alt13=4;
			}
			else if ( (LA13_0=='f') )
			{
				alt13=5;
			}
			else if ( ((LA13_0>='\u0000' && LA13_0<='a')||(LA13_0>='c' && LA13_0<='e')||(LA13_0>='g' && LA13_0<='m')||(LA13_0>='o' && LA13_0<='q')||LA13_0=='s'||(LA13_0>='u' && LA13_0<='\uFFFF')) )
			{
				alt13=6;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

				throw nvae;
			}
			switch ( alt13 )
			{
			case 1:
				// Language\\Action.g3:441:5: 'n'
				{
				Match('n'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = '\n'; }
				}

				}
				break;
			case 2:
				// Language\\Action.g3:442:5: 'r'
				{
				Match('r'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = '\r'; }
				}

				}
				break;
			case 3:
				// Language\\Action.g3:443:5: 't'
				{
				Match('t'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = '\t'; }
				}

				}
				break;
			case 4:
				// Language\\Action.g3:444:5: 'b'
				{
				Match('b'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = '\b'; }
				}

				}
				break;
			case 5:
				// Language\\Action.g3:445:5: 'f'
				{
				Match('f'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = '\f'; }
				}

				}
				break;
			case 6:
				// Language\\Action.g3:446:5: c=~ ( 'n' | 'r' | 't' | 'b' | 'f' )
				{
				c= input.LA(1);
				input.Consume();
				state.failed=false;
				if ( state.backtracking == 0 )
				{
					if (doEscape) { uc = (char)c; }
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
	// $ANTLR end "ESC_CHAR"

	// $ANTLR start "LBRACK"
	private void mLBRACK()
	{
		try
		{
			int _type = LBRACK;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:446:11: ( '[' )
			// Language\\Action.g3:446:11: '['
			{
			Match('['); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LBRACK"

	// $ANTLR start "RBRACK"
	private void mRBRACK()
	{
		try
		{
			int _type = RBRACK;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:447:11: ( ']' )
			// Language\\Action.g3:447:11: ']'
			{
			Match(']'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RBRACK"

	// $ANTLR start "LPAREN"
	private void mLPAREN()
	{
		try
		{
			int _type = LPAREN;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:448:11: ( '(' )
			// Language\\Action.g3:448:11: '('
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
			// Language\\Action.g3:449:11: ( ')' )
			// Language\\Action.g3:449:11: ')'
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

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:450:10: ( ',' )
			// Language\\Action.g3:450:10: ','
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

	// $ANTLR start "DOT"
	private void mDOT()
	{
		try
		{
			int _type = DOT;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:451:9: ( '.' )
			// Language\\Action.g3:451:9: '.'
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
	// $ANTLR end "DOT"

	// $ANTLR start "ASSIGN"
	private void mASSIGN()
	{
		try
		{
			int _type = ASSIGN;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:452:11: ( '=' )
			// Language\\Action.g3:452:11: '='
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

	// $ANTLR start "COLON"
	private void mCOLON()
	{
		try
		{
			int _type = COLON;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:453:10: ( ':' )
			// Language\\Action.g3:453:10: ':'
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

	// $ANTLR start "PLUS"
	private void mPLUS()
	{
		try
		{
			int _type = PLUS;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:454:9: ( '+' )
			// Language\\Action.g3:454:9: '+'
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

	// $ANTLR start "SEMI"
	private void mSEMI()
	{
		try
		{
			int _type = SEMI;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:455:9: ( ';' )
			// Language\\Action.g3:455:9: ';'
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

	// $ANTLR start "NOT"
	private void mNOT()
	{
		try
		{
			int _type = NOT;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:456:9: ( '!' )
			// Language\\Action.g3:456:9: '!'
			{
			Match('!'); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "NOT"

	// $ANTLR start "DOTDOTDOT"
	private void mDOTDOTDOT()
	{
		try
		{
			int _type = DOTDOTDOT;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:457:13: ( '...' )
			// Language\\Action.g3:457:13: '...'
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
	// $ANTLR end "DOTDOTDOT"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// Language\\Action.g3:460:4: ( ( ' ' | '\\t' | '\\r' | '\\n' )+ )
			// Language\\Action.g3:460:4: ( ' ' | '\\t' | '\\r' | '\\n' )+
			{
			// Language\\Action.g3:460:4: ( ' ' | '\\t' | '\\r' | '\\n' )+
			int cnt14=0;
			for ( ; ; )
			{
				int alt14=2;
				int LA14_0 = input.LA(1);

				if ( ((LA14_0>='\t' && LA14_0<='\n')||LA14_0=='\r'||LA14_0==' ') )
				{
					alt14=1;
				}


				switch ( alt14 )
				{
				case 1:
					// Language\\Action.g3:
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

	// $ANTLR start "WS_CHAR"
	private void mWS_CHAR()
	{
		try
		{
			// Language\\Action.g3:467:4: ( ' ' | '\\t' | NEWLINE )
			int alt15=3;
			switch ( input.LA(1) )
			{
			case ' ':
				{
				alt15=1;
				}
				break;
			case '\t':
				{
				alt15=2;
				}
				break;
			case '\n':
			case '\r':
				{
				alt15=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 15, 0, input);

					throw nvae;
				}
			}

			switch ( alt15 )
			{
			case 1:
				// Language\\Action.g3:467:4: ' '
				{
				Match(' '); if (state.failed) return ;

				}
				break;
			case 2:
				// Language\\Action.g3:467:8: '\\t'
				{
				Match('\t'); if (state.failed) return ;

				}
				break;
			case 3:
				// Language\\Action.g3:467:13: NEWLINE
				{
				mNEWLINE(); if (state.failed) return ;

				}
				break;

			}
		}
		finally
		{
		}
	}
	// $ANTLR end "WS_CHAR"

	// $ANTLR start "NEWLINE"
	private void mNEWLINE()
	{
		try
		{
			// Language\\Action.g3:472:4: ( '\\r' (=> '\\n' )? | '\\n' )
			int alt17=2;
			int LA17_0 = input.LA(1);

			if ( (LA17_0=='\r') )
			{
				alt17=1;
			}
			else if ( (LA17_0=='\n') )
			{
				alt17=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 17, 0, input);

				throw nvae;
			}
			switch ( alt17 )
			{
			case 1:
				// Language\\Action.g3:472:4: '\\r' (=> '\\n' )?
				{
				Match('\r'); if (state.failed) return ;
				// Language\\Action.g3:472:9: (=> '\\n' )?
				int alt16=2;
				int LA16_0 = input.LA(1);

				if ( (LA16_0=='\n') && (EvaluatePredicate(synpred7_Action_fragment)))
				{
					alt16=1;
				}
				switch ( alt16 )
				{
				case 1:
					// Language\\Action.g3:472:10: => '\\n'
					{

					Match('\n'); if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Language\\Action.g3:473:4: '\\n'
				{
				Match('\n'); if (state.failed) return ;

				}
				break;

			}
		}
		finally
		{
		}
	}
	// $ANTLR end "NEWLINE"

	public override void mTokens()
	{
		// Language\\Action.g3:1:10: ( CONDITIONAL | ELSEIF | FIRST | LAST | LENGTH | REST | STRIP | SUPER | TRUNC | ID | INT | STRING | ANONYMOUS_TEMPLATE | LBRACK | RBRACK | LPAREN | RPAREN | COMMA | DOT | ASSIGN | COLON | PLUS | SEMI | NOT | DOTDOTDOT | WS )
		int alt18=26;
		alt18 = dfa18.Predict(input);
		switch ( alt18 )
		{
		case 1:
			// Language\\Action.g3:1:10: CONDITIONAL
			{
			mCONDITIONAL(); if (state.failed) return ;

			}
			break;
		case 2:
			// Language\\Action.g3:1:22: ELSEIF
			{
			mELSEIF(); if (state.failed) return ;

			}
			break;
		case 3:
			// Language\\Action.g3:1:29: FIRST
			{
			mFIRST(); if (state.failed) return ;

			}
			break;
		case 4:
			// Language\\Action.g3:1:35: LAST
			{
			mLAST(); if (state.failed) return ;

			}
			break;
		case 5:
			// Language\\Action.g3:1:40: LENGTH
			{
			mLENGTH(); if (state.failed) return ;

			}
			break;
		case 6:
			// Language\\Action.g3:1:47: REST
			{
			mREST(); if (state.failed) return ;

			}
			break;
		case 7:
			// Language\\Action.g3:1:52: STRIP
			{
			mSTRIP(); if (state.failed) return ;

			}
			break;
		case 8:
			// Language\\Action.g3:1:58: SUPER
			{
			mSUPER(); if (state.failed) return ;

			}
			break;
		case 9:
			// Language\\Action.g3:1:64: TRUNC
			{
			mTRUNC(); if (state.failed) return ;

			}
			break;
		case 10:
			// Language\\Action.g3:1:70: ID
			{
			mID(); if (state.failed) return ;

			}
			break;
		case 11:
			// Language\\Action.g3:1:73: INT
			{
			mINT(); if (state.failed) return ;

			}
			break;
		case 12:
			// Language\\Action.g3:1:77: STRING
			{
			mSTRING(); if (state.failed) return ;

			}
			break;
		case 13:
			// Language\\Action.g3:1:84: ANONYMOUS_TEMPLATE
			{
			mANONYMOUS_TEMPLATE(); if (state.failed) return ;

			}
			break;
		case 14:
			// Language\\Action.g3:1:103: LBRACK
			{
			mLBRACK(); if (state.failed) return ;

			}
			break;
		case 15:
			// Language\\Action.g3:1:110: RBRACK
			{
			mRBRACK(); if (state.failed) return ;

			}
			break;
		case 16:
			// Language\\Action.g3:1:117: LPAREN
			{
			mLPAREN(); if (state.failed) return ;

			}
			break;
		case 17:
			// Language\\Action.g3:1:124: RPAREN
			{
			mRPAREN(); if (state.failed) return ;

			}
			break;
		case 18:
			// Language\\Action.g3:1:131: COMMA
			{
			mCOMMA(); if (state.failed) return ;

			}
			break;
		case 19:
			// Language\\Action.g3:1:137: DOT
			{
			mDOT(); if (state.failed) return ;

			}
			break;
		case 20:
			// Language\\Action.g3:1:141: ASSIGN
			{
			mASSIGN(); if (state.failed) return ;

			}
			break;
		case 21:
			// Language\\Action.g3:1:148: COLON
			{
			mCOLON(); if (state.failed) return ;

			}
			break;
		case 22:
			// Language\\Action.g3:1:154: PLUS
			{
			mPLUS(); if (state.failed) return ;

			}
			break;
		case 23:
			// Language\\Action.g3:1:159: SEMI
			{
			mSEMI(); if (state.failed) return ;

			}
			break;
		case 24:
			// Language\\Action.g3:1:164: NOT
			{
			mNOT(); if (state.failed) return ;

			}
			break;
		case 25:
			// Language\\Action.g3:1:168: DOTDOTDOT
			{
			mDOTDOTDOT(); if (state.failed) return ;

			}
			break;
		case 26:
			// Language\\Action.g3:1:178: WS
			{
			mWS(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_Action
	public void synpred1_Action_fragment()
	{
		// Language\\Action.g3:368:5: ( ( WS_CHAR )? ID )
		// Language\\Action.g3:368:6: ( WS_CHAR )? ID
		{
		// Language\\Action.g3:368:6: ( WS_CHAR )?
		int alt19=2;
		int LA19_0 = input.LA(1);

		if ( ((LA19_0>='\t' && LA19_0<='\n')||LA19_0=='\r'||LA19_0==' ') )
		{
			alt19=1;
		}
		switch ( alt19 )
		{
		case 1:
			// Language\\Action.g3:368:0: WS_CHAR
			{
			mWS_CHAR(); if (state.failed) return ;

			}
			break;

		}

		mID(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred1_Action

	// $ANTLR start synpred2_Action
	public void synpred2_Action_fragment()
	{
		// Language\\Action.g3:368:83: ( WS_CHAR )
		// Language\\Action.g3:368:84: WS_CHAR
		{
		mWS_CHAR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred2_Action

	// $ANTLR start synpred3_Action
	public void synpred3_Action_fragment()
	{
		// Language\\Action.g3:377:5: ( '\\\\{' )
		// Language\\Action.g3:377:6: '\\\\{'
		{
		Match("\\{"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred3_Action

	// $ANTLR start synpred4_Action
	public void synpred4_Action_fragment()
	{
		// Language\\Action.g3:378:5: ( '\\\\}' )
		// Language\\Action.g3:378:6: '\\\\}'
		{
		Match("\\}"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred4_Action

	// $ANTLR start synpred5_Action
	public void synpred5_Action_fragment()
	{
		// Language\\Action.g3:415:5: ( '\\\\{' )
		// Language\\Action.g3:415:6: '\\\\{'
		{
		Match("\\{"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred5_Action

	// $ANTLR start synpred6_Action
	public void synpred6_Action_fragment()
	{
		// Language\\Action.g3:416:5: ( '\\\\}' )
		// Language\\Action.g3:416:6: '\\\\}'
		{
		Match("\\}"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred6_Action

	// $ANTLR start synpred7_Action
	public void synpred7_Action_fragment()
	{
		// Language\\Action.g3:472:10: ( '\\n' )
		// Language\\Action.g3:472:11: '\\n'
		{
		Match('\n'); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred7_Action

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
	DFA5 dfa5;
	DFA6 dfa6;
	DFA12 dfa12;
	DFA18 dfa18;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa5 = new DFA5( this, new SpecialStateTransitionHandler( specialStateTransition5 ) );
		dfa6 = new DFA6( this, new SpecialStateTransitionHandler( specialStateTransition6 ) );
		dfa12 = new DFA12( this, new SpecialStateTransitionHandler( specialStateTransition12 ) );
		dfa18 = new DFA18( this );
	}

	class DFA5 : DFA
	{

		const string DFA5_eotS =
			"\x1D\xFFFF";
		const string DFA5_eofS =
			"\x1D\xFFFF";
		const string DFA5_minS =
			"\x6\x0\x1\xFFFF\xE\x0\x1\xFFFF\x7\x0";
		const string DFA5_maxS =
			"\x6\xFFFF\x1\xFFFF\x7\xFFFF\x1\x0\x6\xFFFF\x1\xFFFF\x7\xFFFF";
		const string DFA5_acceptS =
			"\x6\xFFFF\x1\x2\xE\xFFFF\x1\x1\x7\xFFFF";
		const string DFA5_specialS =
			"\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\xFFFF\x1\x6\x1\x7\x1\x8\x1\x9"+
			"\x1\xA\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x10\x1\x11\x1\x12\x1\x13\x1\xFFFF"+
			"\x1\x14\x1\x15\x1\x16\x1\x17\x1\x18\x1\x19\x1\x1A}>";
		static readonly string[] DFA5_transitionS =
			{
				"\x9\x6\x1\x2\x1\x4\x2\x6\x1\x3\x12\x6\x1\x1\x20\x6\x1A\x5\x4\x6\x1\x5"+
				"\x1\x6\x1A\x5\xFF85\x6",
				"\x41\x6\x1A\x5\x4\x6\x1\x5\x1\x6\x1A\x5\xFF85\x6",
				"\x41\x6\x1A\x5\x4\x6\x1\x5\x1\x6\x1A\x5\xFF85\x6",
				"\xA\x6\x1\x7\x36\x6\x1A\x5\x4\x6\x1\x5\x1\x6\x1A\x5\xFF85\x6",
				"\x41\x6\x1A\x5\x4\x6\x1\x5\x1\x6\x1A\x5\xFF85\x6",
				"\x9\x6\x1\xA\x1\xC\x2\x6\x1\xB\x12\x6\x1\x9\xB\x6\x1\xD\x2\x6\xB\x8"+
				"\x7\x6\x1A\x8\x4\x6\x1\x8\x1\x6\x1A\x8\x1\x6\x1\xE\xFF83\x6",
				"",
				"\x41\x6\x1A\x5\x4\x6\x1\x5\x1\x6\x1A\x5\xFF85\x6",
				"\x9\x6\x1\xA\x1\xC\x2\x6\x1\xB\x12\x6\x1\x9\xB\x6\x1\xD\x2\x6\xB\x8"+
				"\x7\x6\x1A\x8\x4\x6\x1\x8\x1\x6\x1A\x8\x1\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\xA\x6\x1\xF\x21\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x9\x6\x1\x11\x1\x13\x2\x6\x1\x12\x12\x6\x1\x10\x20\x6\x1A\x14\x4\x6"+
				"\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\x1\xFFFF",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x41\x6\x1A\x14\x4\x6\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\x41\x6\x1A\x14\x4\x6\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\xA\x6\x1\x16\x36\x6\x1A\x14\x4\x6\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\x41\x6\x1A\x14\x4\x6\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\x9\x6\x1\x19\x1\x1B\x2\x6\x1\x1A\x12\x6\x1\x18\xB\x6\x1\xD\x2\x6\xB"+
				"\x17\x7\x6\x1A\x17\x4\x6\x1\x17\x1\x6\x1A\x17\x1\x6\x1\xE\xFF83\x6",
				"",
				"\x41\x6\x1A\x14\x4\x6\x1\x14\x1\x6\x1A\x14\xFF85\x6",
				"\x9\x6\x1\x19\x1\x1B\x2\x6\x1\x1A\x12\x6\x1\x18\xB\x6\x1\xD\x2\x6\xB"+
				"\x17\x7\x6\x1A\x17\x4\x6\x1\x17\x1\x6\x1A\x17\x1\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\xA\x6\x1\x1C\x21\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6",
				"\x2C\x6\x1\xD\x4F\x6\x1\xE\xFF83\x6"
			};

		static readonly short[] DFA5_eot = DFA.UnpackEncodedString(DFA5_eotS);
		static readonly short[] DFA5_eof = DFA.UnpackEncodedString(DFA5_eofS);
		static readonly char[] DFA5_min = DFA.UnpackEncodedStringToUnsignedChars(DFA5_minS);
		static readonly char[] DFA5_max = DFA.UnpackEncodedStringToUnsignedChars(DFA5_maxS);
		static readonly short[] DFA5_accept = DFA.UnpackEncodedString(DFA5_acceptS);
		static readonly short[] DFA5_special = DFA.UnpackEncodedString(DFA5_specialS);
		static readonly short[][] DFA5_transition;

		static DFA5()
		{
			int numStates = DFA5_transitionS.Length;
			DFA5_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA5_transition[i] = DFA.UnpackEncodedString(DFA5_transitionS[i]);
			}
		}

		public DFA5( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 5;
			this.eot = DFA5_eot;
			this.eof = DFA5_eof;
			this.min = DFA5_min;
			this.max = DFA5_max;
			this.accept = DFA5_accept;
			this.special = DFA5_special;
			this.transition = DFA5_transition;
		}
		public override string GetDescription()
		{
			return "368:3: (=> TEMPLATE_ARGS[out subtext, args] (=> WS_CHAR )? |)";
		}
	}

	int specialStateTransition5( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{
			case 0:
				int LA5_0 = input.LA(1);

				s = -1;
				if ( (LA5_0==' ') ) {s = 1;}

				else if ( (LA5_0=='\t') ) {s = 2;}

				else if ( (LA5_0=='\r') ) {s = 3;}

				else if ( (LA5_0=='\n') ) {s = 4;}

				else if ( ((LA5_0>='A' && LA5_0<='Z')||LA5_0=='_'||(LA5_0>='a' && LA5_0<='z')) ) {s = 5;}

				else if ( ((LA5_0>='\u0000' && LA5_0<='\b')||(LA5_0>='\u000B' && LA5_0<='\f')||(LA5_0>='\u000E' && LA5_0<='\u001F')||(LA5_0>='!' && LA5_0<='@')||(LA5_0>='[' && LA5_0<='^')||LA5_0=='`'||(LA5_0>='{' && LA5_0<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 1:
				int LA5_1 = input.LA(1);

				s = -1;
				if ( ((LA5_1>='A' && LA5_1<='Z')||LA5_1=='_'||(LA5_1>='a' && LA5_1<='z')) ) {s = 5;}

				else if ( ((LA5_1>='\u0000' && LA5_1<='@')||(LA5_1>='[' && LA5_1<='^')||LA5_1=='`'||(LA5_1>='{' && LA5_1<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 2:
				int LA5_2 = input.LA(1);

				s = -1;
				if ( ((LA5_2>='A' && LA5_2<='Z')||LA5_2=='_'||(LA5_2>='a' && LA5_2<='z')) ) {s = 5;}

				else if ( ((LA5_2>='\u0000' && LA5_2<='@')||(LA5_2>='[' && LA5_2<='^')||LA5_2=='`'||(LA5_2>='{' && LA5_2<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 3:
				int LA5_3 = input.LA(1);

				s = -1;
				if ( (LA5_3=='\n') ) {s = 7;}

				else if ( ((LA5_3>='A' && LA5_3<='Z')||LA5_3=='_'||(LA5_3>='a' && LA5_3<='z')) ) {s = 5;}

				else if ( ((LA5_3>='\u0000' && LA5_3<='\t')||(LA5_3>='\u000B' && LA5_3<='@')||(LA5_3>='[' && LA5_3<='^')||LA5_3=='`'||(LA5_3>='{' && LA5_3<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 4:
				int LA5_4 = input.LA(1);

				s = -1;
				if ( ((LA5_4>='A' && LA5_4<='Z')||LA5_4=='_'||(LA5_4>='a' && LA5_4<='z')) ) {s = 5;}

				else if ( ((LA5_4>='\u0000' && LA5_4<='@')||(LA5_4>='[' && LA5_4<='^')||LA5_4=='`'||(LA5_4>='{' && LA5_4<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 5:
				int LA5_5 = input.LA(1);

				s = -1;
				if ( ((LA5_5>='/' && LA5_5<='9')||(LA5_5>='A' && LA5_5<='Z')||LA5_5=='_'||(LA5_5>='a' && LA5_5<='z')) ) {s = 8;}

				else if ( (LA5_5==' ') ) {s = 9;}

				else if ( (LA5_5=='\t') ) {s = 10;}

				else if ( (LA5_5=='\r') ) {s = 11;}

				else if ( (LA5_5=='\n') ) {s = 12;}

				else if ( (LA5_5==',') ) {s = 13;}

				else if ( (LA5_5=='|') ) {s = 14;}

				else if ( ((LA5_5>='\u0000' && LA5_5<='\b')||(LA5_5>='\u000B' && LA5_5<='\f')||(LA5_5>='\u000E' && LA5_5<='\u001F')||(LA5_5>='!' && LA5_5<='+')||(LA5_5>='-' && LA5_5<='.')||(LA5_5>=':' && LA5_5<='@')||(LA5_5>='[' && LA5_5<='^')||LA5_5=='`'||LA5_5=='{'||(LA5_5>='}' && LA5_5<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 6:
				int LA5_7 = input.LA(1);

				s = -1;
				if ( ((LA5_7>='A' && LA5_7<='Z')||LA5_7=='_'||(LA5_7>='a' && LA5_7<='z')) ) {s = 5;}

				else if ( ((LA5_7>='\u0000' && LA5_7<='@')||(LA5_7>='[' && LA5_7<='^')||LA5_7=='`'||(LA5_7>='{' && LA5_7<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 7:
				int LA5_8 = input.LA(1);

				s = -1;
				if ( (LA5_8==' ') ) {s = 9;}

				else if ( (LA5_8=='\t') ) {s = 10;}

				else if ( (LA5_8=='\r') ) {s = 11;}

				else if ( (LA5_8=='\n') ) {s = 12;}

				else if ( (LA5_8==',') ) {s = 13;}

				else if ( (LA5_8=='|') ) {s = 14;}

				else if ( ((LA5_8>='/' && LA5_8<='9')||(LA5_8>='A' && LA5_8<='Z')||LA5_8=='_'||(LA5_8>='a' && LA5_8<='z')) ) {s = 8;}

				else if ( ((LA5_8>='\u0000' && LA5_8<='\b')||(LA5_8>='\u000B' && LA5_8<='\f')||(LA5_8>='\u000E' && LA5_8<='\u001F')||(LA5_8>='!' && LA5_8<='+')||(LA5_8>='-' && LA5_8<='.')||(LA5_8>=':' && LA5_8<='@')||(LA5_8>='[' && LA5_8<='^')||LA5_8=='`'||LA5_8=='{'||(LA5_8>='}' && LA5_8<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 8:
				int LA5_9 = input.LA(1);

				s = -1;
				if ( (LA5_9==',') ) {s = 13;}

				else if ( (LA5_9=='|') ) {s = 14;}

				else if ( ((LA5_9>='\u0000' && LA5_9<='+')||(LA5_9>='-' && LA5_9<='{')||(LA5_9>='}' && LA5_9<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 9:
				int LA5_10 = input.LA(1);

				s = -1;
				if ( (LA5_10==',') ) {s = 13;}

				else if ( (LA5_10=='|') ) {s = 14;}

				else if ( ((LA5_10>='\u0000' && LA5_10<='+')||(LA5_10>='-' && LA5_10<='{')||(LA5_10>='}' && LA5_10<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 10:
				int LA5_11 = input.LA(1);

				s = -1;
				if ( (LA5_11=='\n') ) {s = 15;}

				else if ( (LA5_11==',') ) {s = 13;}

				else if ( (LA5_11=='|') ) {s = 14;}

				else if ( ((LA5_11>='\u0000' && LA5_11<='\t')||(LA5_11>='\u000B' && LA5_11<='+')||(LA5_11>='-' && LA5_11<='{')||(LA5_11>='}' && LA5_11<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 11:
				int LA5_12 = input.LA(1);

				s = -1;
				if ( (LA5_12==',') ) {s = 13;}

				else if ( (LA5_12=='|') ) {s = 14;}

				else if ( ((LA5_12>='\u0000' && LA5_12<='+')||(LA5_12>='-' && LA5_12<='{')||(LA5_12>='}' && LA5_12<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 12:
				int LA5_13 = input.LA(1);

				s = -1;
				if ( (LA5_13==' ') ) {s = 16;}

				else if ( (LA5_13=='\t') ) {s = 17;}

				else if ( (LA5_13=='\r') ) {s = 18;}

				else if ( (LA5_13=='\n') ) {s = 19;}

				else if ( ((LA5_13>='A' && LA5_13<='Z')||LA5_13=='_'||(LA5_13>='a' && LA5_13<='z')) ) {s = 20;}

				else if ( ((LA5_13>='\u0000' && LA5_13<='\b')||(LA5_13>='\u000B' && LA5_13<='\f')||(LA5_13>='\u000E' && LA5_13<='\u001F')||(LA5_13>='!' && LA5_13<='@')||(LA5_13>='[' && LA5_13<='^')||LA5_13=='`'||(LA5_13>='{' && LA5_13<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 13:
				int LA5_14 = input.LA(1);


				int index5_14 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_Action_fragment)) ) {s = 21;}

				else if ( (true) ) {s = 6;}


				input.Seek(index5_14);
				if ( s>=0 ) return s;
				break;
			case 14:
				int LA5_15 = input.LA(1);

				s = -1;
				if ( (LA5_15==',') ) {s = 13;}

				else if ( (LA5_15=='|') ) {s = 14;}

				else if ( ((LA5_15>='\u0000' && LA5_15<='+')||(LA5_15>='-' && LA5_15<='{')||(LA5_15>='}' && LA5_15<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 15:
				int LA5_16 = input.LA(1);

				s = -1;
				if ( ((LA5_16>='A' && LA5_16<='Z')||LA5_16=='_'||(LA5_16>='a' && LA5_16<='z')) ) {s = 20;}

				else if ( ((LA5_16>='\u0000' && LA5_16<='@')||(LA5_16>='[' && LA5_16<='^')||LA5_16=='`'||(LA5_16>='{' && LA5_16<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 16:
				int LA5_17 = input.LA(1);

				s = -1;
				if ( ((LA5_17>='A' && LA5_17<='Z')||LA5_17=='_'||(LA5_17>='a' && LA5_17<='z')) ) {s = 20;}

				else if ( ((LA5_17>='\u0000' && LA5_17<='@')||(LA5_17>='[' && LA5_17<='^')||LA5_17=='`'||(LA5_17>='{' && LA5_17<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 17:
				int LA5_18 = input.LA(1);

				s = -1;
				if ( (LA5_18=='\n') ) {s = 22;}

				else if ( ((LA5_18>='A' && LA5_18<='Z')||LA5_18=='_'||(LA5_18>='a' && LA5_18<='z')) ) {s = 20;}

				else if ( ((LA5_18>='\u0000' && LA5_18<='\t')||(LA5_18>='\u000B' && LA5_18<='@')||(LA5_18>='[' && LA5_18<='^')||LA5_18=='`'||(LA5_18>='{' && LA5_18<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 18:
				int LA5_19 = input.LA(1);

				s = -1;
				if ( ((LA5_19>='A' && LA5_19<='Z')||LA5_19=='_'||(LA5_19>='a' && LA5_19<='z')) ) {s = 20;}

				else if ( ((LA5_19>='\u0000' && LA5_19<='@')||(LA5_19>='[' && LA5_19<='^')||LA5_19=='`'||(LA5_19>='{' && LA5_19<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 19:
				int LA5_20 = input.LA(1);

				s = -1;
				if ( ((LA5_20>='/' && LA5_20<='9')||(LA5_20>='A' && LA5_20<='Z')||LA5_20=='_'||(LA5_20>='a' && LA5_20<='z')) ) {s = 23;}

				else if ( (LA5_20==' ') ) {s = 24;}

				else if ( (LA5_20=='\t') ) {s = 25;}

				else if ( (LA5_20=='\r') ) {s = 26;}

				else if ( (LA5_20=='\n') ) {s = 27;}

				else if ( (LA5_20=='|') ) {s = 14;}

				else if ( (LA5_20==',') ) {s = 13;}

				else if ( ((LA5_20>='\u0000' && LA5_20<='\b')||(LA5_20>='\u000B' && LA5_20<='\f')||(LA5_20>='\u000E' && LA5_20<='\u001F')||(LA5_20>='!' && LA5_20<='+')||(LA5_20>='-' && LA5_20<='.')||(LA5_20>=':' && LA5_20<='@')||(LA5_20>='[' && LA5_20<='^')||LA5_20=='`'||LA5_20=='{'||(LA5_20>='}' && LA5_20<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 20:
				int LA5_22 = input.LA(1);

				s = -1;
				if ( ((LA5_22>='A' && LA5_22<='Z')||LA5_22=='_'||(LA5_22>='a' && LA5_22<='z')) ) {s = 20;}

				else if ( ((LA5_22>='\u0000' && LA5_22<='@')||(LA5_22>='[' && LA5_22<='^')||LA5_22=='`'||(LA5_22>='{' && LA5_22<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 21:
				int LA5_23 = input.LA(1);

				s = -1;
				if ( (LA5_23==' ') ) {s = 24;}

				else if ( (LA5_23=='\t') ) {s = 25;}

				else if ( (LA5_23=='\r') ) {s = 26;}

				else if ( (LA5_23=='\n') ) {s = 27;}

				else if ( (LA5_23=='|') ) {s = 14;}

				else if ( (LA5_23==',') ) {s = 13;}

				else if ( ((LA5_23>='/' && LA5_23<='9')||(LA5_23>='A' && LA5_23<='Z')||LA5_23=='_'||(LA5_23>='a' && LA5_23<='z')) ) {s = 23;}

				else if ( ((LA5_23>='\u0000' && LA5_23<='\b')||(LA5_23>='\u000B' && LA5_23<='\f')||(LA5_23>='\u000E' && LA5_23<='\u001F')||(LA5_23>='!' && LA5_23<='+')||(LA5_23>='-' && LA5_23<='.')||(LA5_23>=':' && LA5_23<='@')||(LA5_23>='[' && LA5_23<='^')||LA5_23=='`'||LA5_23=='{'||(LA5_23>='}' && LA5_23<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 22:
				int LA5_24 = input.LA(1);

				s = -1;
				if ( (LA5_24=='|') ) {s = 14;}

				else if ( (LA5_24==',') ) {s = 13;}

				else if ( ((LA5_24>='\u0000' && LA5_24<='+')||(LA5_24>='-' && LA5_24<='{')||(LA5_24>='}' && LA5_24<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 23:
				int LA5_25 = input.LA(1);

				s = -1;
				if ( (LA5_25=='|') ) {s = 14;}

				else if ( (LA5_25==',') ) {s = 13;}

				else if ( ((LA5_25>='\u0000' && LA5_25<='+')||(LA5_25>='-' && LA5_25<='{')||(LA5_25>='}' && LA5_25<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 24:
				int LA5_26 = input.LA(1);

				s = -1;
				if ( (LA5_26=='\n') ) {s = 28;}

				else if ( (LA5_26=='|') ) {s = 14;}

				else if ( (LA5_26==',') ) {s = 13;}

				else if ( ((LA5_26>='\u0000' && LA5_26<='\t')||(LA5_26>='\u000B' && LA5_26<='+')||(LA5_26>='-' && LA5_26<='{')||(LA5_26>='}' && LA5_26<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 25:
				int LA5_27 = input.LA(1);

				s = -1;
				if ( (LA5_27=='|') ) {s = 14;}

				else if ( (LA5_27==',') ) {s = 13;}

				else if ( ((LA5_27>='\u0000' && LA5_27<='+')||(LA5_27>='-' && LA5_27<='{')||(LA5_27>='}' && LA5_27<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
			case 26:
				int LA5_28 = input.LA(1);

				s = -1;
				if ( (LA5_28=='|') ) {s = 14;}

				else if ( (LA5_28==',') ) {s = 13;}

				else if ( ((LA5_28>='\u0000' && LA5_28<='+')||(LA5_28>='-' && LA5_28<='{')||(LA5_28>='}' && LA5_28<='\uFFFF')) ) {s = 6;}

				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 5, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA6 : DFA
	{

		const string DFA6_eotS =
			"\xA\xFFFF";
		const string DFA6_eofS =
			"\xA\xFFFF";
		const string DFA6_minS =
			"\x1\x0\x1\xFFFF\x1\x0\x2\xFFFF\x2\x0\x3\xFFFF";
		const string DFA6_maxS =
			"\x1\xFFFF\x1\xFFFF\x1\xFFFF\x2\xFFFF\x2\x0\x3\xFFFF";
		const string DFA6_acceptS =
			"\x1\xFFFF\x1\x6\x1\xFFFF\x1\x4\x1\x5\x2\xFFFF\x1\x3\x1\x1\x1\x2";
		const string DFA6_specialS =
			"\x1\x0\x1\xFFFF\x1\x1\x2\xFFFF\x1\x2\x1\x3\x3\xFFFF}>";
		static readonly string[] DFA6_transitionS =
			{
				"\x5C\x4\x1\x2\x1E\x4\x1\x3\x1\x4\x1\x1\xFF82\x4",
				"",
				"\x7B\x7\x1\x5\x1\x7\x1\x6\xFF82\x7",
				"",
				"",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				"",
				""
			};

		static readonly short[] DFA6_eot = DFA.UnpackEncodedString(DFA6_eotS);
		static readonly short[] DFA6_eof = DFA.UnpackEncodedString(DFA6_eofS);
		static readonly char[] DFA6_min = DFA.UnpackEncodedStringToUnsignedChars(DFA6_minS);
		static readonly char[] DFA6_max = DFA.UnpackEncodedStringToUnsignedChars(DFA6_maxS);
		static readonly short[] DFA6_accept = DFA.UnpackEncodedString(DFA6_acceptS);
		static readonly short[] DFA6_special = DFA.UnpackEncodedString(DFA6_specialS);
		static readonly short[][] DFA6_transition;

		static DFA6()
		{
			int numStates = DFA6_transitionS.Length;
			DFA6_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA6_transition[i] = DFA.UnpackEncodedString(DFA6_transitionS[i]);
			}
		}

		public DFA6( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 6;
			this.eot = DFA6_eot;
			this.eof = DFA6_eof;
			this.min = DFA6_min;
			this.max = DFA6_max;
			this.accept = DFA6_accept;
			this.special = DFA6_special;
			this.transition = DFA6_transition;
		}
		public override string GetDescription()
		{
			return "()* loopback of 377:3: (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '\\\\' | '{' | '}' ) )*";
		}
	}

	int specialStateTransition6( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{
			case 0:
				int LA6_0 = input.LA(1);

				s = -1;
				if ( (LA6_0=='}') ) {s = 1;}

				else if ( (LA6_0=='\\') ) {s = 2;}

				else if ( (LA6_0=='{') ) {s = 3;}

				else if ( ((LA6_0>='\u0000' && LA6_0<='[')||(LA6_0>=']' && LA6_0<='z')||LA6_0=='|'||(LA6_0>='~' && LA6_0<='\uFFFF')) ) {s = 4;}

				if ( s>=0 ) return s;
				break;
			case 1:
				int LA6_2 = input.LA(1);

				s = -1;
				if ( (LA6_2=='{') ) {s = 5;}

				else if ( (LA6_2=='}') ) {s = 6;}

				else if ( ((LA6_2>='\u0000' && LA6_2<='z')||LA6_2=='|'||(LA6_2>='~' && LA6_2<='\uFFFF')) ) {s = 7;}

				if ( s>=0 ) return s;
				break;
			case 2:
				int LA6_5 = input.LA(1);


				int index6_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred3_Action_fragment)) ) {s = 8;}

				else if ( (true) ) {s = 7;}


				input.Seek(index6_5);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA6_6 = input.LA(1);


				int index6_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred4_Action_fragment)) ) {s = 9;}

				else if ( (true) ) {s = 7;}


				input.Seek(index6_6);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 6, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA12 : DFA
	{

		const string DFA12_eotS =
			"\xA\xFFFF";
		const string DFA12_eofS =
			"\xA\xFFFF";
		const string DFA12_minS =
			"\x1\x0\x1\xFFFF\x1\x0\x2\xFFFF\x2\x0\x3\xFFFF";
		const string DFA12_maxS =
			"\x1\xFFFF\x1\xFFFF\x1\xFFFF\x2\xFFFF\x2\x0\x3\xFFFF";
		const string DFA12_acceptS =
			"\x1\xFFFF\x1\x6\x1\xFFFF\x1\x4\x1\x5\x2\xFFFF\x1\x3\x1\x1\x1\x2";
		const string DFA12_specialS =
			"\x1\x0\x1\xFFFF\x1\x1\x2\xFFFF\x1\x2\x1\x3\x3\xFFFF}>";
		static readonly string[] DFA12_transitionS =
			{
				"\x5C\x4\x1\x2\x1E\x4\x1\x3\x1\x4\x1\x1\xFF82\x4",
				"",
				"\x7B\x7\x1\x5\x1\x7\x1\x6\xFF82\x7",
				"",
				"",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				"",
				""
			};

		static readonly short[] DFA12_eot = DFA.UnpackEncodedString(DFA12_eotS);
		static readonly short[] DFA12_eof = DFA.UnpackEncodedString(DFA12_eofS);
		static readonly char[] DFA12_min = DFA.UnpackEncodedStringToUnsignedChars(DFA12_minS);
		static readonly char[] DFA12_max = DFA.UnpackEncodedStringToUnsignedChars(DFA12_maxS);
		static readonly short[] DFA12_accept = DFA.UnpackEncodedString(DFA12_acceptS);
		static readonly short[] DFA12_special = DFA.UnpackEncodedString(DFA12_specialS);
		static readonly short[][] DFA12_transition;

		static DFA12()
		{
			int numStates = DFA12_transitionS.Length;
			DFA12_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA12_transition[i] = DFA.UnpackEncodedString(DFA12_transitionS[i]);
			}
		}

		public DFA12( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 12;
			this.eot = DFA12_eot;
			this.eof = DFA12_eof;
			this.min = DFA12_min;
			this.max = DFA12_max;
			this.accept = DFA12_accept;
			this.special = DFA12_special;
			this.transition = DFA12_transition;
		}
		public override string GetDescription()
		{
			return "()* loopback of 415:3: (=> '\\\\{' |=> '\\\\}' | ESC_CHAR[out uc, false] | NESTED_ANONYMOUS_TEMPLATE[out subtext] |ch=~ ( '{' | '}' | '\\\\' ) )*";
		}
	}

	int specialStateTransition12( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{
			case 0:
				int LA12_0 = input.LA(1);

				s = -1;
				if ( (LA12_0=='}') ) {s = 1;}

				else if ( (LA12_0=='\\') ) {s = 2;}

				else if ( (LA12_0=='{') ) {s = 3;}

				else if ( ((LA12_0>='\u0000' && LA12_0<='[')||(LA12_0>=']' && LA12_0<='z')||LA12_0=='|'||(LA12_0>='~' && LA12_0<='\uFFFF')) ) {s = 4;}

				if ( s>=0 ) return s;
				break;
			case 1:
				int LA12_2 = input.LA(1);

				s = -1;
				if ( (LA12_2=='{') ) {s = 5;}

				else if ( (LA12_2=='}') ) {s = 6;}

				else if ( ((LA12_2>='\u0000' && LA12_2<='z')||LA12_2=='|'||(LA12_2>='~' && LA12_2<='\uFFFF')) ) {s = 7;}

				if ( s>=0 ) return s;
				break;
			case 2:
				int LA12_5 = input.LA(1);


				int index12_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred5_Action_fragment)) ) {s = 8;}

				else if ( (true) ) {s = 7;}


				input.Seek(index12_5);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA12_6 = input.LA(1);


				int index12_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred6_Action_fragment)) ) {s = 9;}

				else if ( (true) ) {s = 7;}


				input.Seek(index12_6);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 12, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA18 : DFA
	{

		const string DFA18_eotS =
			"\x1\xFFFF\x7\x8\x9\xFFFF\x1\x22\x6\xFFFF\x1\x23\x8\x8\x3\xFFFF\xA\x8"+
			"\x1\x36\x1\x8\x1\x38\x4\x8\x1\x3D\x1\xFFFF\x1\x8\x1\xFFFF\x1\x3F\x1\x40"+
			"\x1\x41\x1\x42\x1\xFFFF\x1\x43\x5\xFFFF";
		const string DFA18_eofS =
			"\x44\xFFFF";
		const string DFA18_minS =
			"\x1\x9\x1\x66\x1\x6C\x1\x69\x1\x61\x1\x65\x1\x74\x1\x72\x9\xFFFF\x1\x2E"+
			"\x6\xFFFF\x1\x2F\x1\x73\x1\x72\x1\x73\x1\x6E\x1\x73\x1\x72\x1\x70\x1"+
			"\x75\x3\xFFFF\x1\x65\x1\x73\x1\x74\x1\x67\x1\x74\x1\x69\x1\x65\x1\x6E"+
			"\x1\x69\x1\x74\x1\x2F\x1\x74\x1\x2F\x1\x70\x1\x72\x1\x63\x1\x66\x1\x2F"+
			"\x1\xFFFF\x1\x68\x1\xFFFF\x4\x2F\x1\xFFFF\x1\x2F\x5\xFFFF";
		const string DFA18_maxS =
			"\x1\x7B\x1\x66\x1\x6C\x1\x69\x2\x65\x1\x75\x1\x72\x9\xFFFF\x1\x2E\x6"+
			"\xFFFF\x1\x7A\x1\x73\x1\x72\x1\x73\x1\x6E\x1\x73\x1\x72\x1\x70\x1\x75"+
			"\x3\xFFFF\x1\x65\x1\x73\x1\x74\x1\x67\x1\x74\x1\x69\x1\x65\x1\x6E\x1"+
			"\x69\x1\x74\x1\x7A\x1\x74\x1\x7A\x1\x70\x1\x72\x1\x63\x1\x66\x1\x7A\x1"+
			"\xFFFF\x1\x68\x1\xFFFF\x4\x7A\x1\xFFFF\x1\x7A\x5\xFFFF";
		const string DFA18_acceptS =
			"\x8\xFFFF\x1\xA\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x10\x1\x11\x1\x12\x1"+
			"\xFFFF\x1\x14\x1\x15\x1\x16\x1\x17\x1\x18\x1\x1A\x9\xFFFF\x1\x19\x1\x13"+
			"\x1\x1\x12\xFFFF\x1\x4\x1\xFFFF\x1\x6\x4\xFFFF\x1\x3\x1\xFFFF\x1\x7\x1"+
			"\x8\x1\x9\x1\x2\x1\x5";
		const string DFA18_specialS =
			"\x44\xFFFF}>";
		static readonly string[] DFA18_transitionS =
			{
				"\x2\x17\x2\xFFFF\x1\x17\x12\xFFFF\x1\x17\x1\x16\x1\xA\x5\xFFFF\x1\xE"+
				"\x1\xF\x1\xFFFF\x1\x14\x1\x10\x1\xFFFF\x1\x11\x1\xFFFF\xA\x9\x1\x13"+
				"\x1\x15\x1\xFFFF\x1\x12\x3\xFFFF\x1A\x8\x1\xC\x1\xFFFF\x1\xD\x1\xFFFF"+
				"\x1\x8\x1\xFFFF\x4\x8\x1\x2\x1\x3\x2\x8\x1\x1\x2\x8\x1\x4\x5\x8\x1\x5"+
				"\x1\x6\x1\x7\x6\x8\x1\xB",
				"\x1\x18",
				"\x1\x19",
				"\x1\x1A",
				"\x1\x1B\x3\xFFFF\x1\x1C",
				"\x1\x1D",
				"\x1\x1E\x1\x1F",
				"\x1\x20",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x21",
				"",
				"",
				"",
				"",
				"",
				"",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\x1\x24",
				"\x1\x25",
				"\x1\x26",
				"\x1\x27",
				"\x1\x28",
				"\x1\x29",
				"\x1\x2A",
				"\x1\x2B",
				"",
				"",
				"",
				"\x1\x2C",
				"\x1\x2D",
				"\x1\x2E",
				"\x1\x2F",
				"\x1\x30",
				"\x1\x31",
				"\x1\x32",
				"\x1\x33",
				"\x1\x34",
				"\x1\x35",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\x1\x37",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\x1\x39",
				"\x1\x3A",
				"\x1\x3B",
				"\x1\x3C",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"",
				"\x1\x3E",
				"",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"",
				"\xB\x8\x7\xFFFF\x1A\x8\x4\xFFFF\x1\x8\x1\xFFFF\x1A\x8",
				"",
				"",
				"",
				"",
				""
			};

		static readonly short[] DFA18_eot = DFA.UnpackEncodedString(DFA18_eotS);
		static readonly short[] DFA18_eof = DFA.UnpackEncodedString(DFA18_eofS);
		static readonly char[] DFA18_min = DFA.UnpackEncodedStringToUnsignedChars(DFA18_minS);
		static readonly char[] DFA18_max = DFA.UnpackEncodedStringToUnsignedChars(DFA18_maxS);
		static readonly short[] DFA18_accept = DFA.UnpackEncodedString(DFA18_acceptS);
		static readonly short[] DFA18_special = DFA.UnpackEncodedString(DFA18_specialS);
		static readonly short[][] DFA18_transition;

		static DFA18()
		{
			int numStates = DFA18_transitionS.Length;
			DFA18_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA18_transition[i] = DFA.UnpackEncodedString(DFA18_transitionS[i]);
			}
		}

		public DFA18( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 18;
			this.eot = DFA18_eot;
			this.eof = DFA18_eof;
			this.min = DFA18_min;
			this.max = DFA18_max;
			this.accept = DFA18_accept;
			this.special = DFA18_special;
			this.transition = DFA18_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens : ( CONDITIONAL | ELSEIF | FIRST | LAST | LENGTH | REST | STRIP | SUPER | TRUNC | ID | INT | STRING | ANONYMOUS_TEMPLATE | LBRACK | RBRACK | LPAREN | RPAREN | COMMA | DOT | ASSIGN | COLON | PLUS | SEMI | NOT | DOTDOTDOT | WS );";
		}
	}

 
	#endregion

}

} // namespace  Antlr3.ST.Language 
