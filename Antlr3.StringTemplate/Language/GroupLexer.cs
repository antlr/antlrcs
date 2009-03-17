// $ANTLR 3.1.2 Language\\Group.g3 2009-03-16 20:26:39

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

namespace Antlr3.ST.Language
{
public partial class GroupLexer : Lexer
{
	public const int EOF=-1;
	public const int ANONYMOUS_TEMPLATE=4;
	public const int ASSIGN=5;
	public const int AT=6;
	public const int BIGSTRING=7;
	public const int COLON=8;
	public const int COMMA=9;
	public const int DEFINED_TO_BE=10;
	public const int DOT=11;
	public const int ID=12;
	public const int KWDEFAULT=13;
	public const int KWGROUP=14;
	public const int KWIMPLEMENTS=15;
	public const int LBRACK=16;
	public const int LPAREN=17;
	public const int ML_COMMENT=18;
	public const int OPTIONAL=19;
	public const int PLUS=20;
	public const int RBRACK=21;
	public const int RPAREN=22;
	public const int SEMI=23;
	public const int SL_COMMENT=24;
	public const int STAR=25;
	public const int STRING=26;
	public const int WS=27;

    // delegates
    // delegators

	public GroupLexer() {}
	public GroupLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public GroupLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Language\\Group.g3"; } }

	// $ANTLR start "KWDEFAULT"
	private void mKWDEFAULT()
	{
		try
		{
			int _type = KWDEFAULT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:42:13: ( 'default' )
			// Language\\Group.g3:42:13: 'default'
			{
			Match("default"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "KWDEFAULT"

	// $ANTLR start "KWGROUP"
	private void mKWGROUP()
	{
		try
		{
			int _type = KWGROUP;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:43:11: ( 'group' )
			// Language\\Group.g3:43:11: 'group'
			{
			Match("group"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "KWGROUP"

	// $ANTLR start "KWIMPLEMENTS"
	private void mKWIMPLEMENTS()
	{
		try
		{
			int _type = KWIMPLEMENTS;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:44:16: ( 'implements' )
			// Language\\Group.g3:44:16: 'implements'
			{
			Match("implements"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "KWIMPLEMENTS"

	// $ANTLR start "ID"
	private void mID()
	{
		try
		{
			int _type = ID;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:290:4: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )* )
			// Language\\Group.g3:290:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )*
			{
			if ( (input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z') )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}

			// Language\\Group.g3:290:28: ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )*
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( (LA1_0=='-'||(LA1_0>='0' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')) )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// Language\\Group.g3:
					{
					input.Consume();


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

	// $ANTLR start "STRING"
	private void mSTRING()
	{
		try
		{
			int _type = STRING;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			int ch;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Group.g3:298:4: ( '\"' ( '\\\\' ( '\"' |ch=~ '\"' ) |ch=~ ( '\\\\' | '\"' ) )* '\"' )
			// Language\\Group.g3:298:4: '\"' ( '\\\\' ( '\"' |ch=~ '\"' ) |ch=~ ( '\\\\' | '\"' ) )* '\"'
			{
			Match('\"'); 
			// Language\\Group.g3:299:3: ( '\\\\' ( '\"' |ch=~ '\"' ) |ch=~ ( '\\\\' | '\"' ) )*
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
					// Language\\Group.g3:299:5: '\\\\' ( '\"' |ch=~ '\"' )
					{
					Match('\\'); 
					// Language\\Group.g3:300:4: ( '\"' |ch=~ '\"' )
					int alt2=2;
					int LA2_0 = input.LA(1);

					if ( (LA2_0=='\"') )
					{
						alt2=1;
					}
					else if ( ((LA2_0>='\u0000' && LA2_0<='!')||(LA2_0>='#' && LA2_0<='\uFFFF')) )
					{
						alt2=2;
					}
					else
					{
						NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

						throw nvae;
					}
					switch ( alt2 )
					{
					case 1:
						// Language\\Group.g3:300:6: '\"'
						{
						Match('\"'); 
						builder.Append('"');

						}
						break;
					case 2:
						// Language\\Group.g3:301:6: ch=~ '\"'
						{
						ch= input.LA(1);
						input.Consume();

						builder.Append("\\" + (char)ch);

						}
						break;

					}


					}
					break;
				case 2:
					// Language\\Group.g3:303:5: ch=~ ( '\\\\' | '\"' )
					{
					ch= input.LA(1);
					input.Consume();

					builder.Append((char)ch);

					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;


			Match('\"'); 

						Text = builder.ToString();
					

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "STRING"

	// $ANTLR start "BIGSTRING"
	private void mBIGSTRING()
	{
		try
		{
			int _type = BIGSTRING;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:312:4: ( '<<' ( '\\\\>' | '\\\\' ~ '>' | '>' ~ '>' |~ ( '\\\\' | '>' ) )* '>>' )
			// Language\\Group.g3:312:4: '<<' ( '\\\\>' | '\\\\' ~ '>' | '>' ~ '>' |~ ( '\\\\' | '>' ) )* '>>'
			{
			Match("<<"); 

			// Language\\Group.g3:312:9: ( '\\\\>' | '\\\\' ~ '>' | '>' ~ '>' |~ ( '\\\\' | '>' ) )*
			for ( ; ; )
			{
				int alt4=5;
				int LA4_0 = input.LA(1);

				if ( (LA4_0=='>') )
				{
					int LA4_1 = input.LA(2);

					if ( ((LA4_1>='\u0000' && LA4_1<='=')||(LA4_1>='?' && LA4_1<='\uFFFF')) )
					{
						alt4=3;
					}


				}
				else if ( (LA4_0=='\\') )
				{
					int LA4_2 = input.LA(2);

					if ( (LA4_2=='>') )
					{
						alt4=1;
					}
					else if ( ((LA4_2>='\u0000' && LA4_2<='=')||(LA4_2>='?' && LA4_2<='\uFFFF')) )
					{
						alt4=2;
					}


				}
				else if ( ((LA4_0>='\u0000' && LA4_0<='=')||(LA4_0>='?' && LA4_0<='[')||(LA4_0>=']' && LA4_0<='\uFFFF')) )
				{
					alt4=4;
				}


				switch ( alt4 )
				{
				case 1:
					// Language\\Group.g3:312:10: '\\\\>'
					{
					Match("\\>"); 


					}
					break;
				case 2:
					// Language\\Group.g3:312:18: '\\\\' ~ '>'
					{
					Match('\\'); 
					input.Consume();


					}
					break;
				case 3:
					// Language\\Group.g3:312:30: '>' ~ '>'
					{
					Match('>'); 
					input.Consume();


					}
					break;
				case 4:
					// Language\\Group.g3:312:41: ~ ( '\\\\' | '>' )
					{
					input.Consume();


					}
					break;

				default:
					goto loop4;
				}
			}

			loop4:
				;


			Match(">>"); 


						System.Text.StringBuilder builder = new System.Text.StringBuilder( input.substring( state.tokenStartCharIndex + 2, GetCharIndex() - 3 ) );
						Text = ProcessBigString( builder );
					

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "BIGSTRING"

	// $ANTLR start "ANONYMOUS_TEMPLATE"
	private void mANONYMOUS_TEMPLATE()
	{
		try
		{
			int _type = ANONYMOUS_TEMPLATE;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:320:4: ( '{' ( '\\\\}' | '\\\\' ~ '}' |~ ( '\\\\' | '}' ) )* '}' )
			// Language\\Group.g3:320:4: '{' ( '\\\\}' | '\\\\' ~ '}' |~ ( '\\\\' | '}' ) )* '}'
			{
			Match('{'); 
			// Language\\Group.g3:321:3: ( '\\\\}' | '\\\\' ~ '}' |~ ( '\\\\' | '}' ) )*
			for ( ; ; )
			{
				int alt5=4;
				int LA5_0 = input.LA(1);

				if ( (LA5_0=='\\') )
				{
					int LA5_2 = input.LA(2);

					if ( (LA5_2=='}') )
					{
						alt5=1;
					}
					else if ( ((LA5_2>='\u0000' && LA5_2<='|')||(LA5_2>='~' && LA5_2<='\uFFFF')) )
					{
						alt5=2;
					}


				}
				else if ( ((LA5_0>='\u0000' && LA5_0<='[')||(LA5_0>=']' && LA5_0<='|')||(LA5_0>='~' && LA5_0<='\uFFFF')) )
				{
					alt5=3;
				}


				switch ( alt5 )
				{
				case 1:
					// Language\\Group.g3:321:5: '\\\\}'
					{
					Match("\\}"); 


					}
					break;
				case 2:
					// Language\\Group.g3:322:5: '\\\\' ~ '}'
					{
					Match('\\'); 
					input.Consume();


					}
					break;
				case 3:
					// Language\\Group.g3:323:5: ~ ( '\\\\' | '}' )
					{
					input.Consume();


					}
					break;

				default:
					goto loop5;
				}
			}

			loop5:
				;


			Match('}'); 

						System.Text.StringBuilder builder = new System.Text.StringBuilder( input.substring( state.tokenStartCharIndex + 1, GetCharIndex() - 2 ) );
						Text = ProcessAnonymousTemplate( builder );
					

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ANONYMOUS_TEMPLATE"

	// $ANTLR start "AT"
	private void mAT()
	{
		try
		{
			int _type = AT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:332:6: ( '@' )
			// Language\\Group.g3:332:6: '@'
			{
			Match('@'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "AT"

	// $ANTLR start "LPAREN"
	private void mLPAREN()
	{
		try
		{
			int _type = LPAREN;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:333:9: ( '(' )
			// Language\\Group.g3:333:9: '('
			{
			Match('('); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:334:9: ( ')' )
			// Language\\Group.g3:334:9: ')'
			{
			Match(')'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RPAREN"

	// $ANTLR start "LBRACK"
	private void mLBRACK()
	{
		try
		{
			int _type = LBRACK;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:335:9: ( '[' )
			// Language\\Group.g3:335:9: '['
			{
			Match('['); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:336:9: ( ']' )
			// Language\\Group.g3:336:9: ']'
			{
			Match(']'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "RBRACK"

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:337:9: ( ',' )
			// Language\\Group.g3:337:9: ','
			{
			Match(','); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:338:7: ( '.' )
			// Language\\Group.g3:338:7: '.'
			{
			Match('.'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DOT"

	// $ANTLR start "DEFINED_TO_BE"
	private void mDEFINED_TO_BE()
	{
		try
		{
			int _type = DEFINED_TO_BE;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:339:17: ( '::=' )
			// Language\\Group.g3:339:17: '::='
			{
			Match("::="); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DEFINED_TO_BE"

	// $ANTLR start "SEMI"
	private void mSEMI()
	{
		try
		{
			int _type = SEMI;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:340:9: ( ';' )
			// Language\\Group.g3:340:9: ';'
			{
			Match(';'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SEMI"

	// $ANTLR start "COLON"
	private void mCOLON()
	{
		try
		{
			int _type = COLON;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:341:9: ( ':' )
			// Language\\Group.g3:341:9: ':'
			{
			Match(':'); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:342:9: ( '*' )
			// Language\\Group.g3:342:9: '*'
			{
			Match('*'); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:343:9: ( '+' )
			// Language\\Group.g3:343:9: '+'
			{
			Match('+'); 

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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:344:11: ( '=' )
			// Language\\Group.g3:344:11: '='
			{
			Match('='); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ASSIGN"

	// $ANTLR start "OPTIONAL"
	private void mOPTIONAL()
	{
		try
		{
			int _type = OPTIONAL;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:345:12: ( '?' )
			// Language\\Group.g3:345:12: '?'
			{
			Match('?'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OPTIONAL"

	// $ANTLR start "SL_COMMENT"
	private void mSL_COMMENT()
	{
		try
		{
			int _type = SL_COMMENT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:349:4: ( '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )? )
			// Language\\Group.g3:349:4: '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )?
			{
			Match("//"); 

			// Language\\Group.g3:350:3: (~ ( '\\n' | '\\r' ) )*
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
					// Language\\Group.g3:
					{
					input.Consume();


					}
					break;

				default:
					goto loop6;
				}
			}

			loop6:
				;


			// Language\\Group.g3:350:19: ( ( '\\r' )? '\\n' )?
			int alt8=2;
			int LA8_0 = input.LA(1);

			if ( (LA8_0=='\n'||LA8_0=='\r') )
			{
				alt8=1;
			}
			switch ( alt8 )
			{
			case 1:
				// Language\\Group.g3:350:20: ( '\\r' )? '\\n'
				{
				// Language\\Group.g3:350:20: ( '\\r' )?
				int alt7=2;
				int LA7_0 = input.LA(1);

				if ( (LA7_0=='\r') )
				{
					alt7=1;
				}
				switch ( alt7 )
				{
				case 1:
					// Language\\Group.g3:350:21: '\\r'
					{
					Match('\r'); 

					}
					break;

				}

				Match('\n'); 

				}
				break;

			}

			 _channel = HIDDEN; 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SL_COMMENT"

	// $ANTLR start "ML_COMMENT"
	private void mML_COMMENT()
	{
		try
		{
			int _type = ML_COMMENT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:356:4: ( '/*' ( . )* '*/' )
			// Language\\Group.g3:356:4: '/*' ( . )* '*/'
			{
			Match("/*"); 

			// Language\\Group.g3:356:9: ( . )*
			for ( ; ; )
			{
				int alt9=2;
				int LA9_0 = input.LA(1);

				if ( (LA9_0=='*') )
				{
					int LA9_1 = input.LA(2);

					if ( (LA9_1=='/') )
					{
						alt9=2;
					}
					else if ( ((LA9_1>='\u0000' && LA9_1<='.')||(LA9_1>='0' && LA9_1<='\uFFFF')) )
					{
						alt9=1;
					}


				}
				else if ( ((LA9_0>='\u0000' && LA9_0<=')')||(LA9_0>='+' && LA9_0<='\uFFFF')) )
				{
					alt9=1;
				}


				switch ( alt9 )
				{
				case 1:
					// Language\\Group.g3:356:0: .
					{
					MatchAny(); 

					}
					break;

				default:
					goto loop9;
				}
			}

			loop9:
				;


			Match("*/"); 

			 _channel = HIDDEN; 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ML_COMMENT"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			int _type = WS;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:361:9: ( ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+ )
			// Language\\Group.g3:361:9: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			{
			// Language\\Group.g3:361:9: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			int cnt11=0;
			for ( ; ; )
			{
				int alt11=5;
				switch ( input.LA(1) )
				{
				case ' ':
					{
					alt11=1;
					}
					break;
				case '\t':
					{
					alt11=2;
					}
					break;
				case '\f':
					{
					alt11=3;
					}
					break;
				case '\n':
				case '\r':
					{
					alt11=4;
					}
					break;

				}

				switch ( alt11 )
				{
				case 1:
					// Language\\Group.g3:361:13: ' '
					{
					Match(' '); 

					}
					break;
				case 2:
					// Language\\Group.g3:362:13: '\\t'
					{
					Match('\t'); 

					}
					break;
				case 3:
					// Language\\Group.g3:363:13: '\\f'
					{
					Match('\f'); 

					}
					break;
				case 4:
					// Language\\Group.g3:364:13: ( '\\r' )? '\\n'
					{
					// Language\\Group.g3:364:13: ( '\\r' )?
					int alt10=2;
					int LA10_0 = input.LA(1);

					if ( (LA10_0=='\r') )
					{
						alt10=1;
					}
					switch ( alt10 )
					{
					case 1:
						// Language\\Group.g3:364:14: '\\r'
						{
						Match('\r'); 

						}
						break;

					}

					Match('\n'); 

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


			 _channel = HIDDEN; 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "WS"

	public override void mTokens()
	{
		// Language\\Group.g3:1:10: ( KWDEFAULT | KWGROUP | KWIMPLEMENTS | ID | STRING | BIGSTRING | ANONYMOUS_TEMPLATE | AT | LPAREN | RPAREN | LBRACK | RBRACK | COMMA | DOT | DEFINED_TO_BE | SEMI | COLON | STAR | PLUS | ASSIGN | OPTIONAL | SL_COMMENT | ML_COMMENT | WS )
		int alt12=24;
		alt12 = dfa12.Predict(input);
		switch ( alt12 )
		{
		case 1:
			// Language\\Group.g3:1:10: KWDEFAULT
			{
			mKWDEFAULT(); 

			}
			break;
		case 2:
			// Language\\Group.g3:1:20: KWGROUP
			{
			mKWGROUP(); 

			}
			break;
		case 3:
			// Language\\Group.g3:1:28: KWIMPLEMENTS
			{
			mKWIMPLEMENTS(); 

			}
			break;
		case 4:
			// Language\\Group.g3:1:41: ID
			{
			mID(); 

			}
			break;
		case 5:
			// Language\\Group.g3:1:44: STRING
			{
			mSTRING(); 

			}
			break;
		case 6:
			// Language\\Group.g3:1:51: BIGSTRING
			{
			mBIGSTRING(); 

			}
			break;
		case 7:
			// Language\\Group.g3:1:61: ANONYMOUS_TEMPLATE
			{
			mANONYMOUS_TEMPLATE(); 

			}
			break;
		case 8:
			// Language\\Group.g3:1:80: AT
			{
			mAT(); 

			}
			break;
		case 9:
			// Language\\Group.g3:1:83: LPAREN
			{
			mLPAREN(); 

			}
			break;
		case 10:
			// Language\\Group.g3:1:90: RPAREN
			{
			mRPAREN(); 

			}
			break;
		case 11:
			// Language\\Group.g3:1:97: LBRACK
			{
			mLBRACK(); 

			}
			break;
		case 12:
			// Language\\Group.g3:1:104: RBRACK
			{
			mRBRACK(); 

			}
			break;
		case 13:
			// Language\\Group.g3:1:111: COMMA
			{
			mCOMMA(); 

			}
			break;
		case 14:
			// Language\\Group.g3:1:117: DOT
			{
			mDOT(); 

			}
			break;
		case 15:
			// Language\\Group.g3:1:121: DEFINED_TO_BE
			{
			mDEFINED_TO_BE(); 

			}
			break;
		case 16:
			// Language\\Group.g3:1:135: SEMI
			{
			mSEMI(); 

			}
			break;
		case 17:
			// Language\\Group.g3:1:140: COLON
			{
			mCOLON(); 

			}
			break;
		case 18:
			// Language\\Group.g3:1:146: STAR
			{
			mSTAR(); 

			}
			break;
		case 19:
			// Language\\Group.g3:1:151: PLUS
			{
			mPLUS(); 

			}
			break;
		case 20:
			// Language\\Group.g3:1:156: ASSIGN
			{
			mASSIGN(); 

			}
			break;
		case 21:
			// Language\\Group.g3:1:163: OPTIONAL
			{
			mOPTIONAL(); 

			}
			break;
		case 22:
			// Language\\Group.g3:1:172: SL_COMMENT
			{
			mSL_COMMENT(); 

			}
			break;
		case 23:
			// Language\\Group.g3:1:183: ML_COMMENT
			{
			mML_COMMENT(); 

			}
			break;
		case 24:
			// Language\\Group.g3:1:194: WS
			{
			mWS(); 

			}
			break;

		}

	}


	#region DFA
	DFA12 dfa12;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa12 = new DFA12( this );
	}

	class DFA12 : DFA
	{

		const string DFA12_eotS =
			"\x1\xFFFF\x3\x4\xB\xFFFF\x1\x1B\x7\xFFFF\x3\x4\x4\xFFFF\x7\x4\x1\x28"+
			"\x2\x4\x1\xFFFF\x1\x4\x1\x2C\x1\x4\x1\xFFFF\x2\x4\x1\x30\x1\xFFFF";
		const string DFA12_eofS =
			"\x31\xFFFF";
		const string DFA12_minS =
			"\x1\x9\x1\x65\x1\x72\x1\x6D\xB\xFFFF\x1\x3A\x5\xFFFF\x1\x2A\x1\xFFFF"+
			"\x1\x66\x1\x6F\x1\x70\x4\xFFFF\x1\x61\x1\x75\x1\x6C\x1\x75\x1\x70\x1"+
			"\x65\x1\x6C\x1\x2D\x1\x6D\x1\x74\x1\xFFFF\x1\x65\x1\x2D\x1\x6E\x1\xFFFF"+
			"\x1\x74\x1\x73\x1\x2D\x1\xFFFF";
		const string DFA12_maxS =
			"\x1\x7B\x1\x65\x1\x72\x1\x6D\xB\xFFFF\x1\x3A\x5\xFFFF\x1\x2F\x1\xFFFF"+
			"\x1\x66\x1\x6F\x1\x70\x4\xFFFF\x1\x61\x1\x75\x1\x6C\x1\x75\x1\x70\x1"+
			"\x65\x1\x6C\x1\x7A\x1\x6D\x1\x74\x1\xFFFF\x1\x65\x1\x7A\x1\x6E\x1\xFFFF"+
			"\x1\x74\x1\x73\x1\x7A\x1\xFFFF";
		const string DFA12_acceptS =
			"\x4\xFFFF\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9\x1\xA\x1\xB\x1\xC\x1\xD"+
			"\x1\xE\x1\xFFFF\x1\x10\x1\x12\x1\x13\x1\x14\x1\x15\x1\xFFFF\x1\x18\x3"+
			"\xFFFF\x1\xF\x1\x11\x1\x16\x1\x17\xA\xFFFF\x1\x2\x3\xFFFF\x1\x1\x3\xFFFF"+
			"\x1\x3";
		const string DFA12_specialS =
			"\x31\xFFFF}>";
		static readonly string[] DFA12_transitionS =
			{
				"\x2\x16\x1\xFFFF\x2\x16\x12\xFFFF\x1\x16\x1\xFFFF\x1\x5\x5\xFFFF\x1"+
				"\x9\x1\xA\x1\x11\x1\x12\x1\xD\x1\xFFFF\x1\xE\x1\x15\xA\xFFFF\x1\xF\x1"+
				"\x10\x1\x6\x1\x13\x1\xFFFF\x1\x14\x1\x8\x1A\x4\x1\xB\x1\xFFFF\x1\xC"+
				"\x1\xFFFF\x1\x4\x1\xFFFF\x3\x4\x1\x1\x2\x4\x1\x2\x1\x4\x1\x3\x11\x4"+
				"\x1\x7",
				"\x1\x17",
				"\x1\x18",
				"\x1\x19",
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
				"\x1\x1A",
				"",
				"",
				"",
				"",
				"",
				"\x1\x1D\x4\xFFFF\x1\x1C",
				"",
				"\x1\x1E",
				"\x1\x1F",
				"\x1\x20",
				"",
				"",
				"",
				"",
				"\x1\x21",
				"\x1\x22",
				"\x1\x23",
				"\x1\x24",
				"\x1\x25",
				"\x1\x26",
				"\x1\x27",
				"\x1\x4\x2\xFFFF\xA\x4\x7\xFFFF\x1A\x4\x4\xFFFF\x1\x4\x1\xFFFF\x1A\x4",
				"\x1\x29",
				"\x1\x2A",
				"",
				"\x1\x2B",
				"\x1\x4\x2\xFFFF\xA\x4\x7\xFFFF\x1A\x4\x4\xFFFF\x1\x4\x1\xFFFF\x1A\x4",
				"\x1\x2D",
				"",
				"\x1\x2E",
				"\x1\x2F",
				"\x1\x4\x2\xFFFF\xA\x4\x7\xFFFF\x1A\x4\x4\xFFFF\x1\x4\x1\xFFFF\x1A\x4",
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

		public DFA12( BaseRecognizer recognizer )
		{
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
			return "1:0: Tokens : ( KWDEFAULT | KWGROUP | KWIMPLEMENTS | ID | STRING | BIGSTRING | ANONYMOUS_TEMPLATE | AT | LPAREN | RPAREN | LBRACK | RBRACK | COMMA | DOT | DEFINED_TO_BE | SEMI | COLON | STAR | PLUS | ASSIGN | OPTIONAL | SL_COMMENT | ML_COMMENT | WS );";
		}
	}

 
	#endregion

}

} // namespace Antlr3.ST.Language
