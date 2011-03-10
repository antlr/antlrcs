// $ANTLR 3.1.2 FastSimpleExpression.g3 2009-10-18 19:39:56

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using Lexer = Antlr.Runtime.SlimLexer;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class FastSimpleExpressionLexer : Lexer
{
	public const int EOF=-1;
	public const int T__7=7;
	public const int T__8=8;
	public const int T__9=9;
	public const int T__10=10;
	public const int T__11=11;
	public const int IDENTIFIER=4;
	public const int NUMBER=5;
	public const int WS=6;

    // delegates
    // delegators

	public FastSimpleExpressionLexer() {}
	public FastSimpleExpressionLexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}
	public FastSimpleExpressionLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{

	}
	public override string GrammarFileName { get { return "FastSimpleExpression.g3"; } }

	// $ANTLR start "T__7"
	private void mT__7()
	{
		try
		{
			int _type = T__7;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:11:8: ( '-' )
			// FastSimpleExpression.g3:11:8: '-'
			{
			Match('-'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "T__7"

	// $ANTLR start "T__8"
	private void mT__8()
	{
		try
		{
			int _type = T__8;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:12:8: ( '%' )
			// FastSimpleExpression.g3:12:8: '%'
			{
			Match('%'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "T__8"

	// $ANTLR start "T__9"
	private void mT__9()
	{
		try
		{
			int _type = T__9;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:13:8: ( '*' )
			// FastSimpleExpression.g3:13:8: '*'
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
	// $ANTLR end "T__9"

	// $ANTLR start "T__10"
	private void mT__10()
	{
		try
		{
			int _type = T__10;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:14:9: ( '/' )
			// FastSimpleExpression.g3:14:9: '/'
			{
			Match('/'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "T__10"

	// $ANTLR start "T__11"
	private void mT__11()
	{
		try
		{
			int _type = T__11;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:15:9: ( '+' )
			// FastSimpleExpression.g3:15:9: '+'
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
	// $ANTLR end "T__11"

	// $ANTLR start "IDENTIFIER"
	private void mIDENTIFIER()
	{
		try
		{
			int _type = IDENTIFIER;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:78:4: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			// FastSimpleExpression.g3:78:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			{
			if ((input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}

			// FastSimpleExpression.g3:79:3: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			while (true)
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if (((LA1_0>='0' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')))
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// FastSimpleExpression.g3:
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
	// $ANTLR end "IDENTIFIER"

	// $ANTLR start "NUMBER"
	private void mNUMBER()
	{
		try
		{
			int _type = NUMBER;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:83:4: ( ( '0' .. '9' )+ )
			// FastSimpleExpression.g3:83:4: ( '0' .. '9' )+
			{
			// FastSimpleExpression.g3:83:4: ( '0' .. '9' )+
			int cnt2=0;
			while (true)
			{
				int alt2=2;
				int LA2_0 = input.LA(1);

				if (((LA2_0>='0' && LA2_0<='9')))
				{
					alt2=1;
				}


				switch (alt2)
				{
				case 1:
					// FastSimpleExpression.g3:
					{
					input.Consume();


					}
					break;

				default:
					if (cnt2 >= 1)
						goto loop2;

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
	// $ANTLR end "NUMBER"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// FastSimpleExpression.g3:87:4: ( ( ' ' | '\\t' | '\\n' | '\\r' | '\\f' ) )
			// FastSimpleExpression.g3:87:4: ( ' ' | '\\t' | '\\n' | '\\r' | '\\f' )
			{
			if ((input.LA(1)>='\t' && input.LA(1)<='\n')||(input.LA(1)>='\f' && input.LA(1)<='\r')||input.LA(1)==' ')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}

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
		// FastSimpleExpression.g3:1:10: ( T__7 | T__8 | T__9 | T__10 | T__11 | IDENTIFIER | NUMBER | WS )
		int alt3=8;
		switch (input.LA(1))
		{
		case '-':
			{
			alt3=1;
			}
			break;
		case '%':
			{
			alt3=2;
			}
			break;
		case '*':
			{
			alt3=3;
			}
			break;
		case '/':
			{
			alt3=4;
			}
			break;
		case '+':
			{
			alt3=5;
			}
			break;
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
		case 'G':
		case 'H':
		case 'I':
		case 'J':
		case 'K':
		case 'L':
		case 'M':
		case 'N':
		case 'O':
		case 'P':
		case 'Q':
		case 'R':
		case 'S':
		case 'T':
		case 'U':
		case 'V':
		case 'W':
		case 'X':
		case 'Y':
		case 'Z':
		case '_':
		case 'a':
		case 'b':
		case 'c':
		case 'd':
		case 'e':
		case 'f':
		case 'g':
		case 'h':
		case 'i':
		case 'j':
		case 'k':
		case 'l':
		case 'm':
		case 'n':
		case 'o':
		case 'p':
		case 'q':
		case 'r':
		case 's':
		case 't':
		case 'u':
		case 'v':
		case 'w':
		case 'x':
		case 'y':
		case 'z':
			{
			alt3=6;
			}
			break;
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			{
			alt3=7;
			}
			break;
		case '\t':
		case '\n':
		case '\f':
		case '\r':
		case ' ':
			{
			alt3=8;
			}
			break;
		default:
			{
				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

				throw nvae;
			}
		}

		switch (alt3)
		{
		case 1:
			// FastSimpleExpression.g3:1:10: T__7
			{
			mT__7(); 

			}
			break;
		case 2:
			// FastSimpleExpression.g3:1:15: T__8
			{
			mT__8(); 

			}
			break;
		case 3:
			// FastSimpleExpression.g3:1:20: T__9
			{
			mT__9(); 

			}
			break;
		case 4:
			// FastSimpleExpression.g3:1:25: T__10
			{
			mT__10(); 

			}
			break;
		case 5:
			// FastSimpleExpression.g3:1:31: T__11
			{
			mT__11(); 

			}
			break;
		case 6:
			// FastSimpleExpression.g3:1:37: IDENTIFIER
			{
			mIDENTIFIER(); 

			}
			break;
		case 7:
			// FastSimpleExpression.g3:1:48: NUMBER
			{
			mNUMBER(); 

			}
			break;
		case 8:
			// FastSimpleExpression.g3:1:55: WS
			{
			mWS(); 

			}
			break;

		}

	}


	#region DFA

	protected override void InitDFAs()
	{
		base.InitDFAs();
	}

 
	#endregion

}
