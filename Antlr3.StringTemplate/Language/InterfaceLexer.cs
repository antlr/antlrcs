// $ANTLR 3.1.2 Language\\Interface.g3 2009-03-23 17:53:08

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
public partial class InterfaceLexer : Lexer
{
	public const int EOF=-1;
	public const int COLON=4;
	public const int COMMA=5;
	public const int ID=6;
	public const int INTERFACE=7;
	public const int LPAREN=8;
	public const int ML_COMMENT=9;
	public const int OPTIONAL=10;
	public const int RPAREN=11;
	public const int SEMI=12;
	public const int SL_COMMENT=13;
	public const int WS=14;

    // delegates
    // delegators

	public InterfaceLexer() {}
	public InterfaceLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public InterfaceLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Language\\Interface.g3"; } }

	// $ANTLR start "INTERFACE"
	private void mINTERFACE()
	{
		try
		{
			int _type = INTERFACE;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:42:13: ( 'interface' )
			// Language\\Interface.g3:42:13: 'interface'
			{
			Match("interface"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "INTERFACE"

	// $ANTLR start "OPTIONAL"
	private void mOPTIONAL()
	{
		try
		{
			int _type = OPTIONAL;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:43:12: ( 'optional' )
			// Language\\Interface.g3:43:12: 'optional'
			{
			Match("optional"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OPTIONAL"

	// $ANTLR start "ID"
	private void mID()
	{
		try
		{
			int _type = ID;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:156:6: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )* )
			// Language\\Interface.g3:156:6: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )*
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

			// Language\\Interface.g3:156:30: ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '-' | '_' )*
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
					// Language\\Interface.g3:
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

	// $ANTLR start "LPAREN"
	private void mLPAREN()
	{
		try
		{
			int _type = LPAREN;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:159:10: ( '(' )
			// Language\\Interface.g3:159:10: '('
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
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:160:10: ( ')' )
			// Language\\Interface.g3:160:10: ')'
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

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:161:9: ( ',' )
			// Language\\Interface.g3:161:9: ','
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

	// $ANTLR start "SEMI"
	private void mSEMI()
	{
		try
		{
			int _type = SEMI;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:162:8: ( ';' )
			// Language\\Interface.g3:162:8: ';'
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
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:163:9: ( ':' )
			// Language\\Interface.g3:163:9: ':'
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

	// $ANTLR start "SL_COMMENT"
	private void mSL_COMMENT()
	{
		try
		{
			int _type = SL_COMMENT;
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:167:4: ( '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )? )
			// Language\\Interface.g3:167:4: '//' (~ ( '\\n' | '\\r' ) )* ( ( '\\r' )? '\\n' )?
			{
			Match("//"); 

			// Language\\Interface.g3:168:3: (~ ( '\\n' | '\\r' ) )*
			for ( ; ; )
			{
				int alt2=2;
				int LA2_0 = input.LA(1);

				if ( ((LA2_0>='\u0000' && LA2_0<='\t')||(LA2_0>='\u000B' && LA2_0<='\f')||(LA2_0>='\u000E' && LA2_0<='\uFFFF')) )
				{
					alt2=1;
				}


				switch ( alt2 )
				{
				case 1:
					// Language\\Interface.g3:
					{
					input.Consume();


					}
					break;

				default:
					goto loop2;
				}
			}

			loop2:
				;


			// Language\\Interface.g3:168:19: ( ( '\\r' )? '\\n' )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0=='\n'||LA4_0=='\r') )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Language\\Interface.g3:168:20: ( '\\r' )? '\\n'
				{
				// Language\\Interface.g3:168:20: ( '\\r' )?
				int alt3=2;
				int LA3_0 = input.LA(1);

				if ( (LA3_0=='\r') )
				{
					alt3=1;
				}
				switch ( alt3 )
				{
				case 1:
					// Language\\Interface.g3:168:21: '\\r'
					{
					Match('\r'); 

					}
					break;

				}

				Match('\n'); 

				}
				break;

			}

			 _channel = Hidden; 

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
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:173:4: ( '/*' ( . )* '*/' )
			// Language\\Interface.g3:173:4: '/*' ( . )* '*/'
			{
			Match("/*"); 

			// Language\\Interface.g3:173:9: ( . )*
			for ( ; ; )
			{
				int alt5=2;
				int LA5_0 = input.LA(1);

				if ( (LA5_0=='*') )
				{
					int LA5_1 = input.LA(2);

					if ( (LA5_1=='/') )
					{
						alt5=2;
					}
					else if ( ((LA5_1>='\u0000' && LA5_1<='.')||(LA5_1>='0' && LA5_1<='\uFFFF')) )
					{
						alt5=1;
					}


				}
				else if ( ((LA5_0>='\u0000' && LA5_0<=')')||(LA5_0>='+' && LA5_0<='\uFFFF')) )
				{
					alt5=1;
				}


				switch ( alt5 )
				{
				case 1:
					// Language\\Interface.g3:173:0: .
					{
					MatchAny(); 

					}
					break;

				default:
					goto loop5;
				}
			}

			loop5:
				;


			Match("*/"); 

			 _channel = Hidden; 

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
			int _channel = DefaultTokenChannel;
			// Language\\Interface.g3:178:4: ( ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+ )
			// Language\\Interface.g3:178:4: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			{
			// Language\\Interface.g3:178:4: ( ' ' | '\\t' | '\\f' | ( '\\r' )? '\\n' )+
			int cnt7=0;
			for ( ; ; )
			{
				int alt7=5;
				switch ( input.LA(1) )
				{
				case ' ':
					{
					alt7=1;
					}
					break;
				case '\t':
					{
					alt7=2;
					}
					break;
				case '\f':
					{
					alt7=3;
					}
					break;
				case '\n':
				case '\r':
					{
					alt7=4;
					}
					break;

				}

				switch ( alt7 )
				{
				case 1:
					// Language\\Interface.g3:178:6: ' '
					{
					Match(' '); 

					}
					break;
				case 2:
					// Language\\Interface.g3:179:5: '\\t'
					{
					Match('\t'); 

					}
					break;
				case 3:
					// Language\\Interface.g3:180:5: '\\f'
					{
					Match('\f'); 

					}
					break;
				case 4:
					// Language\\Interface.g3:181:5: ( '\\r' )? '\\n'
					{
					// Language\\Interface.g3:181:5: ( '\\r' )?
					int alt6=2;
					int LA6_0 = input.LA(1);

					if ( (LA6_0=='\r') )
					{
						alt6=1;
					}
					switch ( alt6 )
					{
					case 1:
						// Language\\Interface.g3:181:6: '\\r'
						{
						Match('\r'); 

						}
						break;

					}

					Match('\n'); 

					}
					break;

				default:
					if ( cnt7 >= 1 )
						goto loop7;

					EarlyExitException eee7 = new EarlyExitException( 7, input );
					throw eee7;
				}
				cnt7++;
			}
			loop7:
				;


			 _channel = Hidden; 

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
		// Language\\Interface.g3:1:10: ( INTERFACE | OPTIONAL | ID | LPAREN | RPAREN | COMMA | SEMI | COLON | SL_COMMENT | ML_COMMENT | WS )
		int alt8=11;
		alt8 = dfa8.Predict(input);
		switch ( alt8 )
		{
		case 1:
			// Language\\Interface.g3:1:10: INTERFACE
			{
			mINTERFACE(); 

			}
			break;
		case 2:
			// Language\\Interface.g3:1:20: OPTIONAL
			{
			mOPTIONAL(); 

			}
			break;
		case 3:
			// Language\\Interface.g3:1:29: ID
			{
			mID(); 

			}
			break;
		case 4:
			// Language\\Interface.g3:1:32: LPAREN
			{
			mLPAREN(); 

			}
			break;
		case 5:
			// Language\\Interface.g3:1:39: RPAREN
			{
			mRPAREN(); 

			}
			break;
		case 6:
			// Language\\Interface.g3:1:46: COMMA
			{
			mCOMMA(); 

			}
			break;
		case 7:
			// Language\\Interface.g3:1:52: SEMI
			{
			mSEMI(); 

			}
			break;
		case 8:
			// Language\\Interface.g3:1:57: COLON
			{
			mCOLON(); 

			}
			break;
		case 9:
			// Language\\Interface.g3:1:63: SL_COMMENT
			{
			mSL_COMMENT(); 

			}
			break;
		case 10:
			// Language\\Interface.g3:1:74: ML_COMMENT
			{
			mML_COMMENT(); 

			}
			break;
		case 11:
			// Language\\Interface.g3:1:85: WS
			{
			mWS(); 

			}
			break;

		}

	}


	#region DFA
	DFA8 dfa8;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa8 = new DFA8( this );
	}

	class DFA8 : DFA
	{

		const string DFA8_eotS =
			"\x1\xFFFF\x2\x3\x8\xFFFF\x2\x3\x2\xFFFF\xB\x3\x1\x1C\x1\x1D\x2\xFFFF";
		const string DFA8_eofS =
			"\x1E\xFFFF";
		const string DFA8_minS =
			"\x1\x9\x1\x6E\x1\x70\x6\xFFFF\x1\x2A\x1\xFFFF\x2\x74\x2\xFFFF\x1\x65"+
			"\x1\x69\x1\x72\x1\x6F\x1\x66\x1\x6E\x2\x61\x1\x63\x1\x6C\x1\x65\x2\x2D"+
			"\x2\xFFFF";
		const string DFA8_maxS =
			"\x1\x7A\x1\x6E\x1\x70\x6\xFFFF\x1\x2F\x1\xFFFF\x2\x74\x2\xFFFF\x1\x65"+
			"\x1\x69\x1\x72\x1\x6F\x1\x66\x1\x6E\x2\x61\x1\x63\x1\x6C\x1\x65\x2\x7A"+
			"\x2\xFFFF";
		const string DFA8_acceptS =
			"\x3\xFFFF\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\xFFFF\x1\xB\x2\xFFFF"+
			"\x1\x9\x1\xA\xD\xFFFF\x1\x2\x1\x1";
		const string DFA8_specialS =
			"\x1E\xFFFF}>";
		static readonly string[] DFA8_transitionS =
			{
				"\x2\xA\x1\xFFFF\x2\xA\x12\xFFFF\x1\xA\x7\xFFFF\x1\x4\x1\x5\x2\xFFFF"+
				"\x1\x6\x2\xFFFF\x1\x9\xA\xFFFF\x1\x8\x1\x7\x5\xFFFF\x1A\x3\x4\xFFFF"+
				"\x1\x3\x1\xFFFF\x8\x3\x1\x1\x5\x3\x1\x2\xB\x3",
				"\x1\xB",
				"\x1\xC",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xE\x4\xFFFF\x1\xD",
				"",
				"\x1\xF",
				"\x1\x10",
				"",
				"",
				"\x1\x11",
				"\x1\x12",
				"\x1\x13",
				"\x1\x14",
				"\x1\x15",
				"\x1\x16",
				"\x1\x17",
				"\x1\x18",
				"\x1\x19",
				"\x1\x1A",
				"\x1\x1B",
				"\x1\x3\x2\xFFFF\xA\x3\x7\xFFFF\x1A\x3\x4\xFFFF\x1\x3\x1\xFFFF\x1A\x3",
				"\x1\x3\x2\xFFFF\xA\x3\x7\xFFFF\x1A\x3\x4\xFFFF\x1\x3\x1\xFFFF\x1A\x3",
				"",
				""
			};

		static readonly short[] DFA8_eot = DFA.UnpackEncodedString(DFA8_eotS);
		static readonly short[] DFA8_eof = DFA.UnpackEncodedString(DFA8_eofS);
		static readonly char[] DFA8_min = DFA.UnpackEncodedStringToUnsignedChars(DFA8_minS);
		static readonly char[] DFA8_max = DFA.UnpackEncodedStringToUnsignedChars(DFA8_maxS);
		static readonly short[] DFA8_accept = DFA.UnpackEncodedString(DFA8_acceptS);
		static readonly short[] DFA8_special = DFA.UnpackEncodedString(DFA8_specialS);
		static readonly short[][] DFA8_transition;

		static DFA8()
		{
			int numStates = DFA8_transitionS.Length;
			DFA8_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA8_transition[i] = DFA.UnpackEncodedString(DFA8_transitionS[i]);
			}
		}

		public DFA8( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 8;
			this.eot = DFA8_eot;
			this.eof = DFA8_eof;
			this.min = DFA8_min;
			this.max = DFA8_max;
			this.accept = DFA8_accept;
			this.special = DFA8_special;
			this.transition = DFA8_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens : ( INTERFACE | OPTIONAL | ID | LPAREN | RPAREN | COMMA | SEMI | COLON | SL_COMMENT | ML_COMMENT | WS );";
		}
	}

 
	#endregion

}

} // namespace Antlr3.ST.Language
