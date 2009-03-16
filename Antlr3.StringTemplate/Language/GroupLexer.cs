// $ANTLR 3.1.2 Language\\Group.g3 2009-03-07 08:51:18

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
namespace Antlr3.ST.Language
{
public partial class GroupLexer : Lexer
{
	public const int EOF=-1;
	public const int ANONYMOUS_TEMPLATE=4;
	public const int ANYCHAR=5;
	public const int ASSIGN=6;
	public const int AT=7;
	public const int BIGSTRING=8;
	public const int CLOSE_ANON_TEMPLATE=9;
	public const int CLOSE_BIG_STRING=10;
	public const int CLOSE_BLOCK_COMMENT=11;
	public const int COLON=12;
	public const int COMMA=13;
	public const int DEFINED_TO_BE=14;
	public const int DOT=15;
	public const int ID=16;
	public const int KWDEFAULT=17;
	public const int KWGROUP=18;
	public const int KWIMPLEMENTS=19;
	public const int LBRACK=20;
	public const int LPAREN=21;
	public const int ML_COMMENT=22;
	public const int OPTIONAL=23;
	public const int PLUS=24;
	public const int RBRACK=25;
	public const int RPAREN=26;
	public const int SEMI=27;
	public const int SL_COMMENT=28;
	public const int STAR=29;
	public const int STRING=30;
	public const int WS=31;

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
			Match("default"); if (state.failed) return ;


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
			Match("group"); if (state.failed) return ;


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
			Match("implements"); if (state.failed) return ;


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
			state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
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

	// $ANTLR start "STRING"
	private void mSTRING()
	{
		try
		{
			int _type = STRING;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			int ch;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Group.g3:298:4: ( '\"' ( '\\\\' ( '\"' |ch=~ '\"' ) |ch=~ ( '\\\\' | '\"' ) )* ({...}? => ( '\"' )? | '\"' ) )
			// Language\\Group.g3:298:4: '\"' ( '\\\\' ( '\"' |ch=~ '\"' ) |ch=~ ( '\\\\' | '\"' ) )* ({...}? => ( '\"' )? | '\"' )
			{
			Match('\"'); if (state.failed) return ;
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
					Match('\\'); if (state.failed) return ;
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
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

						throw nvae;
					}
					switch ( alt2 )
					{
					case 1:
						// Language\\Group.g3:300:6: '\"'
						{
						Match('\"'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							builder.Append('"');
						}

						}
						break;
					case 2:
						// Language\\Group.g3:301:6: ch=~ '\"'
						{
						ch= input.LA(1);
						input.Consume();
						state.failed=false;
						if ( state.backtracking == 0 )
						{
							builder.Append("\\" + (char)ch);
						}

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


			// Language\\Group.g3:305:3: ({...}? => ( '\"' )? | '\"' )
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( (LA5_0=='\"') )
			{
				int LA5_1 = input.LA(2);

				if ( ((InColorizer)) )
				{
					alt5=1;
				}
				else if ( (true) )
				{
					alt5=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 5, 1, input);

					throw nvae;
				}
			}
			else
			{
				alt5=1;}
			switch ( alt5 )
			{
			case 1:
				// Language\\Group.g3:305:5: {...}? => ( '\"' )?
				{
				if ( !((InColorizer)) )
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					throw new FailedPredicateException(input, "STRING", "InColorizer");
				}
				// Language\\Group.g3:305:23: ( '\"' )?
				int alt4=2;
				int LA4_0 = input.LA(1);

				if ( (LA4_0=='\"') )
				{
					alt4=1;
				}
				switch ( alt4 )
				{
				case 1:
					// Language\\Group.g3:305:0: '\"'
					{
					Match('\"'); if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Language\\Group.g3:306:5: '\"'
				{
				Match('\"'); if (state.failed) return ;

				}
				break;

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
	// $ANTLR end "STRING"

	// $ANTLR start "BIGSTRING"
	private void mBIGSTRING()
	{
		try
		{
			int _type = BIGSTRING;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			int ch;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Group.g3:318:4: ( ( '<<' (=> ( '\\r' )? '\\n' )? (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )* ( '>>' )? ) )
			// Language\\Group.g3:318:4: ( '<<' (=> ( '\\r' )? '\\n' )? (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )* ( '>>' )? )
			{
			// Language\\Group.g3:318:4: ( '<<' (=> ( '\\r' )? '\\n' )? (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )* ( '>>' )? )
			// Language\\Group.g3:318:6: '<<' (=> ( '\\r' )? '\\n' )? (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )* ( '>>' )?
			{
			Match("<<"); if (state.failed) return ;

			if ( state.backtracking == 0 )
			{

								if ( !InAnonymousTemplate && !InBlockComment )
									InBigString=true;
							
			}
			// Language\\Group.g3:323:4: (=> ( '\\r' )? '\\n' )?
			int alt7=2;
			int LA7_0 = input.LA(1);

			if ( (LA7_0=='\r') )
			{
				int LA7_1 = input.LA(2);

				if ( (LA7_1=='\n') )
				{
					int LA7_4 = input.LA(3);

					if ( (synpred1_Group()) )
					{
						alt7=1;
					}
				}
			}
			else if ( (LA7_0=='\n') )
			{
				int LA7_2 = input.LA(2);

				if ( (synpred1_Group()) )
				{
					alt7=1;
				}
			}
			switch ( alt7 )
			{
			case 1:
				// Language\\Group.g3:323:6: => ( '\\r' )? '\\n'
				{

				// Language\\Group.g3:323:21: ( '\\r' )?
				int alt6=2;
				int LA6_0 = input.LA(1);

				if ( (LA6_0=='\r') )
				{
					alt6=1;
				}
				switch ( alt6 )
				{
				case 1:
					// Language\\Group.g3:323:22: '\\r'
					{
					Match('\r'); if (state.failed) return ;

					}
					break;

				}

				Match('\n'); if (state.failed) return ;

				}
				break;

			}

			// Language\\Group.g3:324:4: (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )*
			for ( ; ; )
			{
				int alt9=6;
				alt9 = dfa9.Predict(input);
				switch ( alt9 )
				{
				case 1:
					// Language\\Group.g3:324:6: => '\\r' '\\n'
					{

					Match('\r'); if (state.failed) return ;
					Match('\n'); if (state.failed) return ;

					}
					break;
				case 2:
					// Language\\Group.g3:325:6: => '\\n'
					{

					Match('\n'); if (state.failed) return ;

					}
					break;
				case 3:
					// Language\\Group.g3:326:6: => ( '\\r' )? '\\n'
					{

					// Language\\Group.g3:326:21: ( '\\r' )?
					int alt8=2;
					int LA8_0 = input.LA(1);

					if ( (LA8_0=='\r') )
					{
						alt8=1;
					}
					switch ( alt8 )
					{
					case 1:
						// Language\\Group.g3:326:22: '\\r'
						{
						Match('\r'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							 builder.Append('\r'); 
						}

						}
						break;

					}

					Match('\n'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						 builder.Append('\n'); 
					}

					}
					break;
				case 4:
					// Language\\Group.g3:327:6: => '\\\\>'
					{

					Match("\\>"); if (state.failed) return ;

					if ( state.backtracking == 0 )
					{
						 builder.Append('>'); 
					}

					}
					break;
				case 5:
					// Language\\Group.g3:328:6: =>ch= .
					{

					ch = input.LA(1);
					MatchAny(); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						 builder.Append((char)ch); 
					}

					}
					break;

				default:
					goto loop9;
				}
			}

			loop9:
				;


			// Language\\Group.g3:330:4: ( '>>' )?
			int alt10=2;
			int LA10_0 = input.LA(1);

			if ( (LA10_0=='>') )
			{
				alt10=1;
			}
			switch ( alt10 )
			{
			case 1:
				// Language\\Group.g3:330:6: '>>'
				{
				Match(">>"); if (state.failed) return ;

				if ( state.backtracking == 0 )
				{
					 InBigString = false; 
				}

				}
				break;

			}


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
	// $ANTLR end "BIGSTRING"

	// $ANTLR start "ANONYMOUS_TEMPLATE"
	private void mANONYMOUS_TEMPLATE()
	{
		try
		{
			int _type = ANONYMOUS_TEMPLATE;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			int ch;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\Group.g3:344:4: ( ( '{' ( ( '\\r' )? '\\n' |=> '\\\\' '}' |ch=~ ( '\\r' | '\\n' | '}' ) )* ( '}' )? ) )
			// Language\\Group.g3:344:4: ( '{' ( ( '\\r' )? '\\n' |=> '\\\\' '}' |ch=~ ( '\\r' | '\\n' | '}' ) )* ( '}' )? )
			{
			// Language\\Group.g3:344:4: ( '{' ( ( '\\r' )? '\\n' |=> '\\\\' '}' |ch=~ ( '\\r' | '\\n' | '}' ) )* ( '}' )? )
			// Language\\Group.g3:344:6: '{' ( ( '\\r' )? '\\n' |=> '\\\\' '}' |ch=~ ( '\\r' | '\\n' | '}' ) )* ( '}' )?
			{
			Match('{'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

								if ( !InBigString && !InBlockComment )
									InAnonymousTemplate=true;
							
			}
			// Language\\Group.g3:349:4: ( ( '\\r' )? '\\n' |=> '\\\\' '}' |ch=~ ( '\\r' | '\\n' | '}' ) )*
			for ( ; ; )
			{
				int alt12=4;
				int LA12_0 = input.LA(1);

				if ( (LA12_0=='\n'||LA12_0=='\r') )
				{
					alt12=1;
				}
				else if ( (LA12_0=='\\') )
				{
					int LA12_3 = input.LA(2);

					if ( (LA12_3=='}') )
					{
						int LA12_5 = input.LA(3);

						if ( (synpred7_Group()) )
						{
							alt12=2;
						}
						else if ( (true) )
						{
							alt12=3;
						}


					}

					else
					{
						alt12=3;
					}

				}
				else if ( ((LA12_0>='\u0000' && LA12_0<='\t')||(LA12_0>='\u000B' && LA12_0<='\f')||(LA12_0>='\u000E' && LA12_0<='[')||(LA12_0>=']' && LA12_0<='|')||(LA12_0>='~' && LA12_0<='\uFFFF')) )
				{
					alt12=3;
				}


				switch ( alt12 )
				{
				case 1:
					// Language\\Group.g3:349:6: ( '\\r' )? '\\n'
					{
					// Language\\Group.g3:349:6: ( '\\r' )?
					int alt11=2;
					int LA11_0 = input.LA(1);

					if ( (LA11_0=='\r') )
					{
						alt11=1;
					}
					switch ( alt11 )
					{
					case 1:
						// Language\\Group.g3:349:7: '\\r'
						{
						Match('\r'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							 builder.Append('\r'); 
						}

						}
						break;

					}

					Match('\n'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						 builder.Append('\n'); 
					}

					}
					break;
				case 2:
					// Language\\Group.g3:350:6: => '\\\\' '}'
					{

					Match('\\'); if (state.failed) return ;
					Match('}'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						 builder.Append('}'); 
					}

					}
					break;
				case 3:
					// Language\\Group.g3:351:6: ch=~ ( '\\r' | '\\n' | '}' )
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


			// Language\\Group.g3:353:4: ( '}' )?
			int alt13=2;
			int LA13_0 = input.LA(1);

			if ( (LA13_0=='}') )
			{
				alt13=1;
			}
			switch ( alt13 )
			{
			case 1:
				// Language\\Group.g3:353:6: '}'
				{
				Match('}'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					 InAnonymousTemplate = false; 
				}

				}
				break;

			}


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
	// $ANTLR end "ANONYMOUS_TEMPLATE"

	// $ANTLR start "AT"
	private void mAT()
	{
		try
		{
			int _type = AT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:363:6: ( '@' )
			// Language\\Group.g3:363:6: '@'
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
	// $ANTLR end "AT"

	// $ANTLR start "LPAREN"
	private void mLPAREN()
	{
		try
		{
			int _type = LPAREN;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:364:9: ( '(' )
			// Language\\Group.g3:364:9: '('
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:365:9: ( ')' )
			// Language\\Group.g3:365:9: ')'
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

	// $ANTLR start "LBRACK"
	private void mLBRACK()
	{
		try
		{
			int _type = LBRACK;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:366:9: ( '[' )
			// Language\\Group.g3:366:9: '['
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:367:9: ( ']' )
			// Language\\Group.g3:367:9: ']'
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

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:368:9: ( ',' )
			// Language\\Group.g3:368:9: ','
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:369:7: ( '.' )
			// Language\\Group.g3:369:7: '.'
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

	// $ANTLR start "DEFINED_TO_BE"
	private void mDEFINED_TO_BE()
	{
		try
		{
			int _type = DEFINED_TO_BE;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:370:17: ( '::=' )
			// Language\\Group.g3:370:17: '::='
			{
			Match("::="); if (state.failed) return ;


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
			// Language\\Group.g3:371:9: ( ';' )
			// Language\\Group.g3:371:9: ';'
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

	// $ANTLR start "COLON"
	private void mCOLON()
	{
		try
		{
			int _type = COLON;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:372:9: ( ':' )
			// Language\\Group.g3:372:9: ':'
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:373:9: ( '*' )
			// Language\\Group.g3:373:9: '*'
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:374:9: ( '+' )
			// Language\\Group.g3:374:9: '+'
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
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:375:11: ( '=' )
			// Language\\Group.g3:375:11: '='
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

	// $ANTLR start "OPTIONAL"
	private void mOPTIONAL()
	{
		try
		{
			int _type = OPTIONAL;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:376:12: ( '?' )
			// Language\\Group.g3:376:12: '?'
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
	// $ANTLR end "OPTIONAL"

	// $ANTLR start "CLOSE_BIG_STRING"
	private void mCLOSE_BIG_STRING()
	{
		try
		{
			int _type = CLOSE_BIG_STRING;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:378:20: ( '>>' )
			// Language\\Group.g3:378:20: '>>'
			{
			Match(">>"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CLOSE_BIG_STRING"

	// $ANTLR start "CLOSE_BLOCK_COMMENT"
	private void mCLOSE_BLOCK_COMMENT()
	{
		try
		{
			int _type = CLOSE_BLOCK_COMMENT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:379:23: ( '*/' )
			// Language\\Group.g3:379:23: '*/'
			{
			Match("*/"); if (state.failed) return ;


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CLOSE_BLOCK_COMMENT"

	// $ANTLR start "CLOSE_ANON_TEMPLATE"
	private void mCLOSE_ANON_TEMPLATE()
	{
		try
		{
			int _type = CLOSE_ANON_TEMPLATE;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:380:23: ( '}' )
			// Language\\Group.g3:380:23: '}'
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
	// $ANTLR end "CLOSE_ANON_TEMPLATE"

	// $ANTLR start "SL_COMMENT"
	private void mSL_COMMENT()
	{
		try
		{
			int _type = SL_COMMENT;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:384:4: ( '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )? )
			// Language\\Group.g3:384:4: '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )?
			{
			Match("//"); if (state.failed) return ;

			// Language\\Group.g3:385:3: (~ ( '\\n' | '\\r' ) )*
			for ( ; ; )
			{
				int alt14=2;
				int LA14_0 = input.LA(1);

				if ( ((LA14_0>='\u0000' && LA14_0<='\t')||(LA14_0>='\u000B' && LA14_0<='\f')||(LA14_0>='\u000E' && LA14_0<='\uFFFF')) )
				{
					alt14=1;
				}


				switch ( alt14 )
				{
				case 1:
					// Language\\Group.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop14;
				}
			}

			loop14:
				;


			// Language\\Group.g3:385:19: ( ( '\\r' )? '\\n' )?
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( (LA16_0=='\n'||LA16_0=='\r') )
			{
				alt16=1;
			}
			switch ( alt16 )
			{
			case 1:
				// Language\\Group.g3:385:20: ( '\\r' )? '\\n'
				{
				// Language\\Group.g3:385:20: ( '\\r' )?
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( (LA15_0=='\r') )
				{
					alt15=1;
				}
				switch ( alt15 )
				{
				case 1:
					// Language\\Group.g3:385:21: '\\r'
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
				 _channel = HIDDEN; 
			}

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
			// Language\\Group.g3:391:4: ( '/*' (~ ( '*' ) |=> '*' )* ( '*/' )? )
			// Language\\Group.g3:391:4: '/*' (~ ( '*' ) |=> '*' )* ( '*/' )?
			{
			Match("/*"); if (state.failed) return ;

			if ( state.backtracking == 0 )
			{

							if ( !InAnonymousTemplate && !InBigString )
								InBlockComment=true;

							_channel = HIDDEN;
						
			}
			// Language\\Group.g3:398:3: (~ ( '*' ) |=> '*' )*
			for ( ; ; )
			{
				int alt17=3;
				int LA17_0 = input.LA(1);

				if ( (LA17_0=='*') )
				{
					int LA17_1 = input.LA(2);

					if ( (LA17_1=='/') )
					{
						int LA17_4 = input.LA(3);

						if ( (synpred8_Group()) )
						{
							alt17=2;
						}


					}
					else if ( (LA17_1=='*') && (synpred8_Group()))
					{
						alt17=2;
					}
					else if ( ((LA17_1>='\u0000' && LA17_1<=')')||(LA17_1>='+' && LA17_1<='.')||(LA17_1>='0' && LA17_1<='\uFFFF')) && (synpred8_Group()))
					{
						alt17=2;
					}

					else
					{
						alt17=2;
					}

				}
				else if ( ((LA17_0>='\u0000' && LA17_0<=')')||(LA17_0>='+' && LA17_0<='\uFFFF')) )
				{
					alt17=1;
				}


				switch ( alt17 )
				{
				case 1:
					// Language\\Group.g3:398:5: ~ ( '*' )
					{
					input.Consume();
					state.failed=false;

					}
					break;
				case 2:
					// Language\\Group.g3:399:6: => '*'
					{

					Match('*'); if (state.failed) return ;

					}
					break;

				default:
					goto loop17;
				}
			}

			loop17:
				;


			// Language\\Group.g3:401:3: ( '*/' )?
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( (LA18_0=='*') )
			{
				alt18=1;
			}
			switch ( alt18 )
			{
			case 1:
				// Language\\Group.g3:401:4: '*/'
				{
				Match("*/"); if (state.failed) return ;

				if ( state.backtracking == 0 )
				{
					InBlockComment = false;
				}

				}
				break;

			}


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
			// Language\\Group.g3:405:9: ( ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+ )
			// Language\\Group.g3:405:9: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			{
			// Language\\Group.g3:405:9: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			int cnt20=0;
			for ( ; ; )
			{
				int alt20=5;
				switch ( input.LA(1) )
				{
				case ' ':
					{
					alt20=1;
					}
					break;
				case '\t':
					{
					alt20=2;
					}
					break;
				case '\f':
					{
					alt20=3;
					}
					break;
				case '\n':
				case '\r':
					{
					alt20=4;
					}
					break;

				}

				switch ( alt20 )
				{
				case 1:
					// Language\\Group.g3:405:13: ' '
					{
					Match(' '); if (state.failed) return ;

					}
					break;
				case 2:
					// Language\\Group.g3:406:13: '\\t'
					{
					Match('\t'); if (state.failed) return ;

					}
					break;
				case 3:
					// Language\\Group.g3:407:13: '\\f'
					{
					Match('\f'); if (state.failed) return ;

					}
					break;
				case 4:
					// Language\\Group.g3:408:13: ( '\\r' )? '\\n'
					{
					// Language\\Group.g3:408:13: ( '\\r' )?
					int alt19=2;
					int LA19_0 = input.LA(1);

					if ( (LA19_0=='\r') )
					{
						alt19=1;
					}
					switch ( alt19 )
					{
					case 1:
						// Language\\Group.g3:408:14: '\\r'
						{
						Match('\r'); if (state.failed) return ;

						}
						break;

					}

					Match('\n'); if (state.failed) return ;

					}
					break;

				default:
					if ( cnt20 >= 1 )
						goto loop20;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee20 = new EarlyExitException( 20, input );
					throw eee20;
				}
				cnt20++;
			}
			loop20:
				;


			if ( state.backtracking == 0 )
			{
				 _channel = HIDDEN; 
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

	// $ANTLR start "ANYCHAR"
	private void mANYCHAR()
	{
		try
		{
			int _type = ANYCHAR;
			int _channel = DEFAULT_TOKEN_CHANNEL;
			// Language\\Group.g3:414:4: ( . )
			// Language\\Group.g3:414:4: .
			{
			MatchAny(); if (state.failed) return ;

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ANYCHAR"

	public override void mTokens()
	{
		// Language\\Group.g3:1:10: ( KWDEFAULT | KWGROUP | KWIMPLEMENTS | ID | STRING | BIGSTRING | ANONYMOUS_TEMPLATE | AT | LPAREN | RPAREN | LBRACK | RBRACK | COMMA | DOT | DEFINED_TO_BE | SEMI | COLON | STAR | PLUS | ASSIGN | OPTIONAL | CLOSE_BIG_STRING | CLOSE_BLOCK_COMMENT | CLOSE_ANON_TEMPLATE | SL_COMMENT | ML_COMMENT | WS | ANYCHAR )
		int alt21=28;
		alt21 = dfa21.Predict(input);
		switch ( alt21 )
		{
		case 1:
			// Language\\Group.g3:1:10: KWDEFAULT
			{
			mKWDEFAULT(); if (state.failed) return ;

			}
			break;
		case 2:
			// Language\\Group.g3:1:20: KWGROUP
			{
			mKWGROUP(); if (state.failed) return ;

			}
			break;
		case 3:
			// Language\\Group.g3:1:28: KWIMPLEMENTS
			{
			mKWIMPLEMENTS(); if (state.failed) return ;

			}
			break;
		case 4:
			// Language\\Group.g3:1:41: ID
			{
			mID(); if (state.failed) return ;

			}
			break;
		case 5:
			// Language\\Group.g3:1:44: STRING
			{
			mSTRING(); if (state.failed) return ;

			}
			break;
		case 6:
			// Language\\Group.g3:1:51: BIGSTRING
			{
			mBIGSTRING(); if (state.failed) return ;

			}
			break;
		case 7:
			// Language\\Group.g3:1:61: ANONYMOUS_TEMPLATE
			{
			mANONYMOUS_TEMPLATE(); if (state.failed) return ;

			}
			break;
		case 8:
			// Language\\Group.g3:1:80: AT
			{
			mAT(); if (state.failed) return ;

			}
			break;
		case 9:
			// Language\\Group.g3:1:83: LPAREN
			{
			mLPAREN(); if (state.failed) return ;

			}
			break;
		case 10:
			// Language\\Group.g3:1:90: RPAREN
			{
			mRPAREN(); if (state.failed) return ;

			}
			break;
		case 11:
			// Language\\Group.g3:1:97: LBRACK
			{
			mLBRACK(); if (state.failed) return ;

			}
			break;
		case 12:
			// Language\\Group.g3:1:104: RBRACK
			{
			mRBRACK(); if (state.failed) return ;

			}
			break;
		case 13:
			// Language\\Group.g3:1:111: COMMA
			{
			mCOMMA(); if (state.failed) return ;

			}
			break;
		case 14:
			// Language\\Group.g3:1:117: DOT
			{
			mDOT(); if (state.failed) return ;

			}
			break;
		case 15:
			// Language\\Group.g3:1:121: DEFINED_TO_BE
			{
			mDEFINED_TO_BE(); if (state.failed) return ;

			}
			break;
		case 16:
			// Language\\Group.g3:1:135: SEMI
			{
			mSEMI(); if (state.failed) return ;

			}
			break;
		case 17:
			// Language\\Group.g3:1:140: COLON
			{
			mCOLON(); if (state.failed) return ;

			}
			break;
		case 18:
			// Language\\Group.g3:1:146: STAR
			{
			mSTAR(); if (state.failed) return ;

			}
			break;
		case 19:
			// Language\\Group.g3:1:151: PLUS
			{
			mPLUS(); if (state.failed) return ;

			}
			break;
		case 20:
			// Language\\Group.g3:1:156: ASSIGN
			{
			mASSIGN(); if (state.failed) return ;

			}
			break;
		case 21:
			// Language\\Group.g3:1:163: OPTIONAL
			{
			mOPTIONAL(); if (state.failed) return ;

			}
			break;
		case 22:
			// Language\\Group.g3:1:172: CLOSE_BIG_STRING
			{
			mCLOSE_BIG_STRING(); if (state.failed) return ;

			}
			break;
		case 23:
			// Language\\Group.g3:1:189: CLOSE_BLOCK_COMMENT
			{
			mCLOSE_BLOCK_COMMENT(); if (state.failed) return ;

			}
			break;
		case 24:
			// Language\\Group.g3:1:209: CLOSE_ANON_TEMPLATE
			{
			mCLOSE_ANON_TEMPLATE(); if (state.failed) return ;

			}
			break;
		case 25:
			// Language\\Group.g3:1:229: SL_COMMENT
			{
			mSL_COMMENT(); if (state.failed) return ;

			}
			break;
		case 26:
			// Language\\Group.g3:1:240: ML_COMMENT
			{
			mML_COMMENT(); if (state.failed) return ;

			}
			break;
		case 27:
			// Language\\Group.g3:1:251: WS
			{
			mWS(); if (state.failed) return ;

			}
			break;
		case 28:
			// Language\\Group.g3:1:254: ANYCHAR
			{
			mANYCHAR(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_Group
	public void synpred1_Group_fragment()
	{
		// Language\\Group.g3:323:6: ( '\\r' | '\\n' )
		// Language\\Group.g3:
		{
		if ( input.LA(1)=='\n'||input.LA(1)=='\r' )
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
	// $ANTLR end synpred1_Group

	// $ANTLR start synpred2_Group
	public void synpred2_Group_fragment()
	{
		// Language\\Group.g3:324:6: ( '\\r\\n>>' )
		// Language\\Group.g3:324:7: '\\r\\n>>'
		{
		Match("\r\n>>"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred2_Group

	// $ANTLR start synpred3_Group
	public void synpred3_Group_fragment()
	{
		// Language\\Group.g3:325:6: ( '\\n>>' )
		// Language\\Group.g3:325:7: '\\n>>'
		{
		Match("\n>>"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred3_Group

	// $ANTLR start synpred4_Group
	public void synpred4_Group_fragment()
	{
		// Language\\Group.g3:326:6: ( '\\r' | '\\n' )
		// Language\\Group.g3:
		{
		if ( input.LA(1)=='\n'||input.LA(1)=='\r' )
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
	// $ANTLR end synpred4_Group

	// $ANTLR start synpred5_Group
	public void synpred5_Group_fragment()
	{
		// Language\\Group.g3:327:6: ( '\\\\>' )
		// Language\\Group.g3:327:7: '\\\\>'
		{
		Match("\\>"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred5_Group

	// $ANTLR start synpred6_Group
	public void synpred6_Group_fragment()
	{
		// Language\\Group.g3:328:6: ( '>' ~ '>' |~ '>' )
		int alt22=2;
		int LA22_0 = input.LA(1);

		if ( (LA22_0=='>') )
		{
			alt22=1;
		}
		else if ( ((LA22_0>='\u0000' && LA22_0<='=')||(LA22_0>='?' && LA22_0<='\uFFFF')) )
		{
			alt22=2;
		}
		else
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			NoViableAltException nvae = new NoViableAltException("", 22, 0, input);

			throw nvae;
		}
		switch ( alt22 )
		{
		case 1:
			// Language\\Group.g3:328:7: '>' ~ '>'
			{
			Match('>'); if (state.failed) return ;
			if ( (input.LA(1)>='\u0000' && input.LA(1)<='=')||(input.LA(1)>='?' && input.LA(1)<='\uFFFF') )
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
			break;
		case 2:
			// Language\\Group.g3:328:18: ~ '>'
			{
			if ( (input.LA(1)>='\u0000' && input.LA(1)<='=')||(input.LA(1)>='?' && input.LA(1)<='\uFFFF') )
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
			break;

		}}
	// $ANTLR end synpred6_Group

	// $ANTLR start synpred7_Group
	public void synpred7_Group_fragment()
	{
		// Language\\Group.g3:350:6: ( '\\\\}' )
		// Language\\Group.g3:350:7: '\\\\}'
		{
		Match("\\}"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred7_Group

	// $ANTLR start synpred8_Group
	public void synpred8_Group_fragment()
	{
		// Language\\Group.g3:399:6: ( '*' ~ '/' )
		// Language\\Group.g3:399:7: '*' ~ '/'
		{
		Match('*'); if (state.failed) return ;
		if ( (input.LA(1)>='\u0000' && input.LA(1)<='.')||(input.LA(1)>='0' && input.LA(1)<='\uFFFF') )
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
	// $ANTLR end synpred8_Group

	public bool synpred1_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred1_Group_fragment(); // can never throw exception
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
	public bool synpred4_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred4_Group_fragment(); // can never throw exception
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
	public bool synpred6_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred6_Group_fragment(); // can never throw exception
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
	public bool synpred3_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred3_Group_fragment(); // can never throw exception
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
	public bool synpred5_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred5_Group_fragment(); // can never throw exception
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
	public bool synpred2_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred2_Group_fragment(); // can never throw exception
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
	public bool synpred7_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred7_Group_fragment(); // can never throw exception
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
	public bool synpred8_Group()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred8_Group_fragment(); // can never throw exception
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


	#region DFA
	DFA9 dfa9;
	DFA21 dfa21;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa9 = new DFA9( this, new SpecialStateTransitionHandler( specialStateTransition9 ) );
		dfa21 = new DFA21( this, new SpecialStateTransitionHandler( specialStateTransition21 ) );
	}

	class DFA9 : DFA
	{

		const string DFA9_eotS =
			"\x1\x2\x1\x8\x3\xFFFF\x1\x8\xB\xFFFF";
		const string DFA9_eofS =
			"\x11\xFFFF";
		const string DFA9_minS =
			"\x2\x0\x1\xFFFF\x1\xA\x2\x0\x1\xFFFF\x1\x0\x7\xFFFF\x1\x0\x1\xFFFF";
		const string DFA9_maxS =
			"\x2\xFFFF\x1\xFFFF\x1\xA\x1\x0\x1\xFFFF\x1\xFFFF\x1\x0\x7\xFFFF\x1\x0"+
			"\x1\xFFFF";
		const string DFA9_acceptS =
			"\x2\xFFFF\x1\x6\x3\xFFFF\x1\x5\x1\xFFFF\x4\x5\x1\x1\x1\x3\x1\x2\x1\xFFFF"+
			"\x1\x4";
		const string DFA9_specialS =
			"\x1\x0\x1\x1\x1\xFFFF\x1\x2\x1\x3\x1\x4\x1\xFFFF\x1\x5\x7\xFFFF\x1\x6"+
			"\x1\xFFFF}>";
		static readonly string[] DFA9_transitionS =
			{
				"\xA\x6\x1\x4\x2\x6\x1\x3\x30\x6\x1\x1\x1D\x6\x1\x5\xFFA3\x6",
				"\xA\x6\x1\xA\x2\x6\x1\x9\x30\x6\x1\x7\x1D\x6\x1\xB\xFFA3\x6",
				"",
				"\x1\xC",
				"\x1\xFFFF",
				"\xA\x6\x1\xA\x2\x6\x1\x9\x30\x6\x1\xF\x1D\x6\x1\xB\xFFA3\x6",
				"",
				"\x1\xFFFF",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xFFFF",
				""
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
			return "()* loopback of 324:4: (=> '\\r' '\\n' |=> '\\n' |=> ( '\\r' )? '\\n' |=> '\\\\>' |=>ch= . )*";
		}
	}

	int specialStateTransition9( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA9_0 = input.LA(1);


				int index9_0 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_0=='>') ) {s = 1;}

				else if ( (LA9_0=='\r') ) {s = 3;}

				else if ( (LA9_0=='\n') ) {s = 4;}

				else if ( (LA9_0=='\\') ) {s = 5;}

				else if ( ((LA9_0>='\u0000' && LA9_0<='\t')||(LA9_0>='\u000B' && LA9_0<='\f')||(LA9_0>='\u000E' && LA9_0<='=')||(LA9_0>='?' && LA9_0<='[')||(LA9_0>=']' && LA9_0<='\uFFFF')) && (synpred6_Group())) {s = 6;}

				else s = 2;


				input.Seek(index9_0);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA9_1 = input.LA(1);


				int index9_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_1=='>') ) {s = 7;}

				else if ( (LA9_1=='\r') && (synpred6_Group())) {s = 9;}

				else if ( (LA9_1=='\n') && (synpred6_Group())) {s = 10;}

				else if ( (LA9_1=='\\') && (synpred6_Group())) {s = 11;}

				else if ( ((LA9_1>='\u0000' && LA9_1<='\t')||(LA9_1>='\u000B' && LA9_1<='\f')||(LA9_1>='\u000E' && LA9_1<='=')||(LA9_1>='?' && LA9_1<='[')||(LA9_1>=']' && LA9_1<='\uFFFF')) && (synpred6_Group())) {s = 6;}

				else s = 8;


				input.Seek(index9_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA9_3 = input.LA(1);


				int index9_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_3=='\n') && (synpred2_Group())) {s = 12;}

				else if ( (synpred4_Group()) ) {s = 13;}

				else if ( (synpred6_Group()) ) {s = 11;}


				input.Seek(index9_3);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA9_4 = input.LA(1);


				int index9_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred3_Group()) ) {s = 14;}

				else if ( (synpred4_Group()) ) {s = 13;}

				else if ( (synpred6_Group()) ) {s = 11;}


				input.Seek(index9_4);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA9_5 = input.LA(1);


				int index9_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA9_5=='>') ) {s = 15;}

				else if ( (LA9_5=='\r') && (synpred6_Group())) {s = 9;}

				else if ( (LA9_5=='\n') && (synpred6_Group())) {s = 10;}

				else if ( (LA9_5=='\\') && (synpred6_Group())) {s = 11;}

				else if ( ((LA9_5>='\u0000' && LA9_5<='\t')||(LA9_5>='\u000B' && LA9_5<='\f')||(LA9_5>='\u000E' && LA9_5<='=')||(LA9_5>='?' && LA9_5<='[')||(LA9_5>=']' && LA9_5<='\uFFFF')) && (synpred6_Group())) {s = 6;}

				else s = 8;


				input.Seek(index9_5);
				if ( s>=0 ) return s;
				break;

			case 5:
				int LA9_7 = input.LA(1);


				int index9_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred6_Group()) ) {s = 11;}

				else if ( (true) ) {s = 2;}


				input.Seek(index9_7);
				if ( s>=0 ) return s;
				break;

			case 6:
				int LA9_15 = input.LA(1);


				int index9_15 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred5_Group()) ) {s = 16;}

				else if ( (synpred6_Group()) ) {s = 11;}


				input.Seek(index9_15);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 9, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA21 : DFA
	{

		const string DFA21_eotS =
			"\x1\xFFFF\x3\x1F\x2\xFFFF\x1\x1D\x8\xFFFF\x1\x2D\x1\xFFFF\x1\x30\x3\xFFFF"+
			"\x1\x1D\x1\xFFFF\x1\x1D\x3\xFFFF\x1\x1D\x2\xFFFF\x1\x1F\x1\xFFFF\x2\x1F"+
			"\x17\xFFFF\x7\x1F\x1\x43\x2\x1F\x1\xFFFF\x1\x1F\x1\x47\x1\x1F\x1\xFFFF"+
			"\x2\x1F\x1\x4B\x1\xFFFF";
		const string DFA21_eofS =
			"\x4C\xFFFF";
		const string DFA21_minS =
			"\x1\x0\x1\x65\x1\x72\x1\x6D\x2\xFFFF\x1\x3C\x8\xFFFF\x1\x3A\x1\xFFFF"+
			"\x1\x2F\x3\xFFFF\x1\x3E\x1\xFFFF\x1\x2A\x3\xFFFF\x1\xA\x2\xFFFF\x1\x66"+
			"\x1\xFFFF\x1\x6F\x1\x70\x17\xFFFF\x1\x61\x1\x75\x1\x6C\x1\x75\x1\x70"+
			"\x1\x65\x1\x6C\x1\x2D\x1\x6D\x1\x74\x1\xFFFF\x1\x65\x1\x2D\x1\x6E\x1"+
			"\xFFFF\x1\x74\x1\x73\x1\x2D\x1\xFFFF";
		const string DFA21_maxS =
			"\x1\xFFFF\x1\x65\x1\x72\x1\x6D\x2\xFFFF\x1\x3C\x8\xFFFF\x1\x3A\x1\xFFFF"+
			"\x1\x2F\x3\xFFFF\x1\x3E\x1\xFFFF\x1\x2F\x3\xFFFF\x1\xA\x2\xFFFF\x1\x66"+
			"\x1\xFFFF\x1\x6F\x1\x70\x17\xFFFF\x1\x61\x1\x75\x1\x6C\x1\x75\x1\x70"+
			"\x1\x65\x1\x6C\x1\x7A\x1\x6D\x1\x74\x1\xFFFF\x1\x65\x1\x7A\x1\x6E\x1"+
			"\xFFFF\x1\x74\x1\x73\x1\x7A\x1\xFFFF";
		const string DFA21_acceptS =
			"\x4\xFFFF\x1\x4\x1\x5\x1\xFFFF\x1\x7\x1\x8\x1\x9\x1\xA\x1\xB\x1\xC\x1"+
			"\xD\x1\xE\x1\xFFFF\x1\x10\x1\xFFFF\x1\x13\x1\x14\x1\x15\x1\xFFFF\x1\x18"+
			"\x1\xFFFF\x3\x1B\x1\xFFFF\x1\x1B\x1\x1C\x1\xFFFF\x1\x4\x2\xFFFF\x1\x5"+
			"\x1\x6\x1\x7\x1\x8\x1\x9\x1\xA\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x11\x1"+
			"\x10\x1\x17\x1\x12\x1\x13\x1\x14\x1\x15\x1\x16\x1\x18\x1\x19\x1\x1A\x1"+
			"\x1B\xA\xFFFF\x1\x2\x3\xFFFF\x1\x1\x3\xFFFF\x1\x3";
		const string DFA21_specialS =
			"\x1\x0\x4B\xFFFF}>";
		static readonly string[] DFA21_transitionS =
			{
				"\x9\x1D\x1\x19\x1\x1C\x1\x1D\x1\x1A\x1\x1B\x12\x1D\x1\x18\x1\x1D\x1"+
				"\x5\x5\x1D\x1\x9\x1\xA\x1\x11\x1\x12\x1\xD\x1\x1D\x1\xE\x1\x17\xA\x1D"+
				"\x1\xF\x1\x10\x1\x6\x1\x13\x1\x15\x1\x14\x1\x8\x1A\x4\x1\xB\x1\x1D\x1"+
				"\xC\x1\x1D\x1\x4\x1\x1D\x3\x4\x1\x1\x2\x4\x1\x2\x1\x4\x1\x3\x11\x4\x1"+
				"\x7\x1\x1D\x1\x16\xFF82\x1D",
				"\x1\x1E",
				"\x1\x20",
				"\x1\x21",
				"",
				"",
				"\x1\x23",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x2C",
				"",
				"\x1\x2F",
				"",
				"",
				"",
				"\x1\x34",
				"",
				"\x1\x37\x4\xFFFF\x1\x36",
				"",
				"",
				"",
				"\x1\x38",
				"",
				"",
				"\x1\x39",
				"",
				"\x1\x3A",
				"\x1\x3B",
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
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x3C",
				"\x1\x3D",
				"\x1\x3E",
				"\x1\x3F",
				"\x1\x40",
				"\x1\x41",
				"\x1\x42",
				"\x1\x1F\x2\xFFFF\xA\x1F\x7\xFFFF\x1A\x1F\x4\xFFFF\x1\x1F\x1\xFFFF\x1A"+
				"\x1F",
				"\x1\x44",
				"\x1\x45",
				"",
				"\x1\x46",
				"\x1\x1F\x2\xFFFF\xA\x1F\x7\xFFFF\x1A\x1F\x4\xFFFF\x1\x1F\x1\xFFFF\x1A"+
				"\x1F",
				"\x1\x48",
				"",
				"\x1\x49",
				"\x1\x4A",
				"\x1\x1F\x2\xFFFF\xA\x1F\x7\xFFFF\x1A\x1F\x4\xFFFF\x1\x1F\x1\xFFFF\x1A"+
				"\x1F",
				""
			};

		static readonly short[] DFA21_eot = DFA.UnpackEncodedString(DFA21_eotS);
		static readonly short[] DFA21_eof = DFA.UnpackEncodedString(DFA21_eofS);
		static readonly char[] DFA21_min = DFA.UnpackEncodedStringToUnsignedChars(DFA21_minS);
		static readonly char[] DFA21_max = DFA.UnpackEncodedStringToUnsignedChars(DFA21_maxS);
		static readonly short[] DFA21_accept = DFA.UnpackEncodedString(DFA21_acceptS);
		static readonly short[] DFA21_special = DFA.UnpackEncodedString(DFA21_specialS);
		static readonly short[][] DFA21_transition;

		static DFA21()
		{
			int numStates = DFA21_transitionS.Length;
			DFA21_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA21_transition[i] = DFA.UnpackEncodedString(DFA21_transitionS[i]);
			}
		}

		public DFA21( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 21;
			this.eot = DFA21_eot;
			this.eof = DFA21_eof;
			this.min = DFA21_min;
			this.max = DFA21_max;
			this.accept = DFA21_accept;
			this.special = DFA21_special;
			this.transition = DFA21_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens : ( KWDEFAULT | KWGROUP | KWIMPLEMENTS | ID | STRING | BIGSTRING | ANONYMOUS_TEMPLATE | AT | LPAREN | RPAREN | LBRACK | RBRACK | COMMA | DOT | DEFINED_TO_BE | SEMI | COLON | STAR | PLUS | ASSIGN | OPTIONAL | CLOSE_BIG_STRING | CLOSE_BLOCK_COMMENT | CLOSE_ANON_TEMPLATE | SL_COMMENT | ML_COMMENT | WS | ANYCHAR );";
		}
	}

	int specialStateTransition21( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA21_0 = input.LA(1);

				s = -1;
				if ( (LA21_0=='d') ) {s = 1;}

				else if ( (LA21_0=='g') ) {s = 2;}

				else if ( (LA21_0=='i') ) {s = 3;}

				else if ( ((LA21_0>='A' && LA21_0<='Z')||LA21_0=='_'||(LA21_0>='a' && LA21_0<='c')||(LA21_0>='e' && LA21_0<='f')||LA21_0=='h'||(LA21_0>='j' && LA21_0<='z')) ) {s = 4;}

				else if ( (LA21_0=='\"') ) {s = 5;}

				else if ( (LA21_0=='<') ) {s = 6;}

				else if ( (LA21_0=='{') ) {s = 7;}

				else if ( (LA21_0=='@') ) {s = 8;}

				else if ( (LA21_0=='(') ) {s = 9;}

				else if ( (LA21_0==')') ) {s = 10;}

				else if ( (LA21_0=='[') ) {s = 11;}

				else if ( (LA21_0==']') ) {s = 12;}

				else if ( (LA21_0==',') ) {s = 13;}

				else if ( (LA21_0=='.') ) {s = 14;}

				else if ( (LA21_0==':') ) {s = 15;}

				else if ( (LA21_0==';') ) {s = 16;}

				else if ( (LA21_0=='*') ) {s = 17;}

				else if ( (LA21_0=='+') ) {s = 18;}

				else if ( (LA21_0=='=') ) {s = 19;}

				else if ( (LA21_0=='?') ) {s = 20;}

				else if ( (LA21_0=='>') ) {s = 21;}

				else if ( (LA21_0=='}') ) {s = 22;}

				else if ( (LA21_0=='/') ) {s = 23;}

				else if ( (LA21_0==' ') ) {s = 24;}

				else if ( (LA21_0=='\t') ) {s = 25;}

				else if ( (LA21_0=='\f') ) {s = 26;}

				else if ( (LA21_0=='\r') ) {s = 27;}

				else if ( (LA21_0=='\n') ) {s = 28;}

				else if ( ((LA21_0>='\u0000' && LA21_0<='\b')||LA21_0=='\u000B'||(LA21_0>='\u000E' && LA21_0<='\u001F')||LA21_0=='!'||(LA21_0>='#' && LA21_0<='\'')||LA21_0=='-'||(LA21_0>='0' && LA21_0<='9')||LA21_0=='\\'||LA21_0=='^'||LA21_0=='`'||LA21_0=='|'||(LA21_0>='~' && LA21_0<='\uFFFF')) ) {s = 29;}

				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 21, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
 
	#endregion

}

} // namespace Antlr3.ST.Language
