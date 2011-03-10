// $ANTLR 3.1.2 SimpleExpression.g3 2009-10-18 19:39:54

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;


using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class SimpleExpressionParser : Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "IDENTIFIER", "NUMBER", "WS", "'-'", "'%'", "'*'", "'/'", "'+'"
	};
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

	public SimpleExpressionParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public SimpleExpressionParser(ITokenStream input, RecognizerSharedState state)
		: base(input, state)
	{
		InitializeTreeAdaptor();
		if ( TreeAdaptor == null )
			TreeAdaptor = new CommonTreeAdaptor();
	}
		
	// Implement this function in your helper file to use a custom tree adaptor
	partial void InitializeTreeAdaptor();
	ITreeAdaptor adaptor;

	public ITreeAdaptor TreeAdaptor
	{
		get
		{
			return adaptor;
		}
		set
		{
			this.adaptor = value;
		}
	}

	public override string[] TokenNames { get { return SimpleExpressionParser.tokenNames; } }
	public override string GrammarFileName { get { return "SimpleExpression.g3"; } }


	#region Rules
	public class expression_return : ParserRuleReturnScope
	{
		internal CommonTree tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "expression"
	// SimpleExpression.g3:43:0: public expression : additive_expression EOF ;
	public SimpleExpressionParser.expression_return expression()
	{
		SimpleExpressionParser.expression_return retval = new SimpleExpressionParser.expression_return();
		retval.start = input.LT(1);

		CommonTree root_0 = null;

		IToken EOF2=null;
		SimpleExpressionParser.additive_expression_return additive_expression1 = default(SimpleExpressionParser.additive_expression_return);

		CommonTree EOF2_tree=null;

		try
		{
			// SimpleExpression.g3:44:4: ( additive_expression EOF )
			// SimpleExpression.g3:44:4: additive_expression EOF
			{
			root_0 = (CommonTree)adaptor.Nil();

			PushFollow(Follow._additive_expression_in_expression39);
			additive_expression1=additive_expression();
			PopFollow();

			adaptor.AddChild(root_0, additive_expression1.Tree);
			EOF2=(IToken)Match(input,EOF,Follow._EOF_in_expression43); 
			EOF2_tree = (CommonTree)adaptor.Create(EOF2);
			adaptor.AddChild(root_0, EOF2_tree);


			}

			retval.stop = input.LT(-1);

			retval.tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (CommonTree)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;

	}
	// $ANTLR end "expression"

	public class additive_expression_return : ParserRuleReturnScope
	{
		internal CommonTree tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "additive_expression"
	// SimpleExpression.g3:48:0: additive_expression : multiplicative_expression ( ( '+' | '-' ) multiplicative_expression )* ;
	private SimpleExpressionParser.additive_expression_return additive_expression()
	{
		SimpleExpressionParser.additive_expression_return retval = new SimpleExpressionParser.additive_expression_return();
		retval.start = input.LT(1);

		CommonTree root_0 = null;

		IToken char_literal4=null;
		IToken char_literal5=null;
		SimpleExpressionParser.multiplicative_expression_return multiplicative_expression3 = default(SimpleExpressionParser.multiplicative_expression_return);
		SimpleExpressionParser.multiplicative_expression_return multiplicative_expression6 = default(SimpleExpressionParser.multiplicative_expression_return);

		CommonTree char_literal4_tree=null;
		CommonTree char_literal5_tree=null;

		try
		{
			// SimpleExpression.g3:49:4: ( multiplicative_expression ( ( '+' | '-' ) multiplicative_expression )* )
			// SimpleExpression.g3:49:4: multiplicative_expression ( ( '+' | '-' ) multiplicative_expression )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			PushFollow(Follow._multiplicative_expression_in_additive_expression54);
			multiplicative_expression3=multiplicative_expression();
			PopFollow();

			adaptor.AddChild(root_0, multiplicative_expression3.Tree);
			// SimpleExpression.g3:50:3: ( ( '+' | '-' ) multiplicative_expression )*
			while (true)
			{
				int alt2=2;
				int LA2_0 = input.LA(1);

				if ((LA2_0==7||LA2_0==11))
				{
					alt2=1;
				}


				switch ( alt2 )
				{
				case 1:
					// SimpleExpression.g3:50:5: ( '+' | '-' ) multiplicative_expression
					{
					// SimpleExpression.g3:50:5: ( '+' | '-' )
					int alt1=2;
					int LA1_0 = input.LA(1);

					if ((LA1_0==11))
					{
						alt1=1;
					}
					else if ((LA1_0==7))
					{
						alt1=2;
					}
					else
					{
						NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

						throw nvae;
					}
					switch (alt1)
					{
					case 1:
						// SimpleExpression.g3:50:6: '+'
						{
						char_literal4=(IToken)Match(input,11,Follow._11_in_additive_expression61); 
						char_literal4_tree = (CommonTree)adaptor.Create(char_literal4);
						root_0 = (CommonTree)adaptor.BecomeRoot(char_literal4_tree, root_0);


						}
						break;
					case 2:
						// SimpleExpression.g3:50:13: '-'
						{
						char_literal5=(IToken)Match(input,7,Follow._7_in_additive_expression66); 
						char_literal5_tree = (CommonTree)adaptor.Create(char_literal5);
						root_0 = (CommonTree)adaptor.BecomeRoot(char_literal5_tree, root_0);


						}
						break;

					}

					PushFollow(Follow._multiplicative_expression_in_additive_expression73);
					multiplicative_expression6=multiplicative_expression();
					PopFollow();

					adaptor.AddChild(root_0, multiplicative_expression6.Tree);

					}
					break;

				default:
					goto loop2;
				}
			}

			loop2:
				;



			}

			retval.stop = input.LT(-1);

			retval.tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (CommonTree)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;

	}
	// $ANTLR end "additive_expression"

	public class multiplicative_expression_return : ParserRuleReturnScope
	{
		internal CommonTree tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "multiplicative_expression"
	// SimpleExpression.g3:55:0: multiplicative_expression : atom ( ( '*' | '/' | '%' ) atom )* ;
	private SimpleExpressionParser.multiplicative_expression_return multiplicative_expression()
	{
		SimpleExpressionParser.multiplicative_expression_return retval = new SimpleExpressionParser.multiplicative_expression_return();
		retval.start = input.LT(1);

		CommonTree root_0 = null;

		IToken char_literal8=null;
		IToken char_literal9=null;
		IToken char_literal10=null;
		SimpleExpressionParser.atom_return atom7 = default(SimpleExpressionParser.atom_return);
		SimpleExpressionParser.atom_return atom11 = default(SimpleExpressionParser.atom_return);

		CommonTree char_literal8_tree=null;
		CommonTree char_literal9_tree=null;
		CommonTree char_literal10_tree=null;

		try
		{
			// SimpleExpression.g3:56:4: ( atom ( ( '*' | '/' | '%' ) atom )* )
			// SimpleExpression.g3:56:4: atom ( ( '*' | '/' | '%' ) atom )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			PushFollow(Follow._atom_in_multiplicative_expression89);
			atom7=atom();
			PopFollow();

			adaptor.AddChild(root_0, atom7.Tree);
			// SimpleExpression.g3:57:3: ( ( '*' | '/' | '%' ) atom )*
			while (true)
			{
				int alt4=2;
				int LA4_0 = input.LA(1);

				if (((LA4_0>=8 && LA4_0<=10)))
				{
					alt4=1;
				}


				switch ( alt4 )
				{
				case 1:
					// SimpleExpression.g3:57:5: ( '*' | '/' | '%' ) atom
					{
					// SimpleExpression.g3:57:5: ( '*' | '/' | '%' )
					int alt3=3;
					switch (input.LA(1))
					{
					case 9:
						{
						alt3=1;
						}
						break;
					case 10:
						{
						alt3=2;
						}
						break;
					case 8:
						{
						alt3=3;
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
						// SimpleExpression.g3:57:6: '*'
						{
						char_literal8=(IToken)Match(input,9,Follow._9_in_multiplicative_expression96); 
						char_literal8_tree = (CommonTree)adaptor.Create(char_literal8);
						root_0 = (CommonTree)adaptor.BecomeRoot(char_literal8_tree, root_0);


						}
						break;
					case 2:
						// SimpleExpression.g3:57:13: '/'
						{
						char_literal9=(IToken)Match(input,10,Follow._10_in_multiplicative_expression101); 
						char_literal9_tree = (CommonTree)adaptor.Create(char_literal9);
						root_0 = (CommonTree)adaptor.BecomeRoot(char_literal9_tree, root_0);


						}
						break;
					case 3:
						// SimpleExpression.g3:57:20: '%'
						{
						char_literal10=(IToken)Match(input,8,Follow._8_in_multiplicative_expression106); 
						char_literal10_tree = (CommonTree)adaptor.Create(char_literal10);
						root_0 = (CommonTree)adaptor.BecomeRoot(char_literal10_tree, root_0);


						}
						break;

					}

					PushFollow(Follow._atom_in_multiplicative_expression113);
					atom11=atom();
					PopFollow();

					adaptor.AddChild(root_0, atom11.Tree);

					}
					break;

				default:
					goto loop4;
				}
			}

			loop4:
				;



			}

			retval.stop = input.LT(-1);

			retval.tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (CommonTree)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;

	}
	// $ANTLR end "multiplicative_expression"

	public class atom_return : ParserRuleReturnScope
	{
		internal CommonTree tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "atom"
	// SimpleExpression.g3:62:0: atom : ( IDENTIFIER | NUMBER );
	private SimpleExpressionParser.atom_return atom()
	{
		SimpleExpressionParser.atom_return retval = new SimpleExpressionParser.atom_return();
		retval.start = input.LT(1);

		CommonTree root_0 = null;

		IToken set12=null;

		CommonTree set12_tree=null;

		try
		{
			// SimpleExpression.g3:63:4: ( IDENTIFIER | NUMBER )
			// SimpleExpression.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			set12=(IToken)input.LT(1);
			if ((input.LA(1)>=IDENTIFIER && input.LA(1)<=NUMBER))
			{
				input.Consume();
				adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set12));
				state.errorRecovery=false;
			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				throw mse;
			}


			}

			retval.stop = input.LT(-1);

			retval.tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (CommonTree)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;

	}
	// $ANTLR end "atom"
	#endregion Rules


	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _additive_expression_in_expression39 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _EOF_in_expression43 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _multiplicative_expression_in_additive_expression54 = new BitSet(new ulong[]{0x882UL});
		public static readonly BitSet _11_in_additive_expression61 = new BitSet(new ulong[]{0x30UL});
		public static readonly BitSet _7_in_additive_expression66 = new BitSet(new ulong[]{0x30UL});
		public static readonly BitSet _multiplicative_expression_in_additive_expression73 = new BitSet(new ulong[]{0x882UL});
		public static readonly BitSet _atom_in_multiplicative_expression89 = new BitSet(new ulong[]{0x702UL});
		public static readonly BitSet _9_in_multiplicative_expression96 = new BitSet(new ulong[]{0x30UL});
		public static readonly BitSet _10_in_multiplicative_expression101 = new BitSet(new ulong[]{0x30UL});
		public static readonly BitSet _8_in_multiplicative_expression106 = new BitSet(new ulong[]{0x30UL});
		public static readonly BitSet _atom_in_multiplicative_expression113 = new BitSet(new ulong[]{0x702UL});
		public static readonly BitSet _set_in_atom129 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}
