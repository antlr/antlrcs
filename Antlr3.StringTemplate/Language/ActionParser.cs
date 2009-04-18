// $ANTLR 3.1.2 Language\\Action.g3 2009-04-18 02:24:05

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

using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;

namespace  Antlr3.ST.Language 
{
public partial class ActionParser : Parser
{
	public static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ANONYMOUS_TEMPLATE", "APPLY", "ARGS", "ASSIGN", "COLON", "COMMA", "CONDITIONAL", "DOT", "DOTDOTDOT", "ELSEIF", "ESC_CHAR", "FIRST", "FUNCTION", "ID", "INCLUDE", "INT", "LAST", "LBRACK", "LENGTH", "LIST", "LPAREN", "MULTI_APPLY", "NESTED_ANONYMOUS_TEMPLATE", "NEWLINE", "NOT", "NOTHING", "PLUS", "RBRACK", "REST", "RPAREN", "SEMI", "SINGLEVALUEARG", "STRING", "STRIP", "SUPER", "TEMPLATE", "TEMPLATE_ARGS", "TRUNC", "VALUE", "WS", "WS_CHAR"
	};
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

	public ActionParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ActionParser( ITokenStream input, RecognizerSharedState state )
		: base( input, state )
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

	public override string[] TokenNames { get { return ActionParser.tokenNames; } }
	public override string GrammarFileName { get { return "Language\\Action.g3"; } }


	#region Rules
	public class action_return : ParserRuleReturnScope
	{
		public IDictionary<string, object> opts=null;
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "action"
	// Language\\Action.g3:145:0: public action returns [IDictionary<string, object> opts=null] : ( templatesExpr ( SEMI optionList )? | 'if' LPAREN ifCondition RPAREN | 'elseif' LPAREN ifCondition RPAREN ) EOF ;
	public ActionParser.action_return action(  )
	{
		ActionParser.action_return retval = new ActionParser.action_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken SEMI2=null;
		IToken string_literal4=null;
		IToken LPAREN5=null;
		IToken RPAREN7=null;
		IToken string_literal8=null;
		IToken LPAREN9=null;
		IToken RPAREN11=null;
		IToken EOF12=null;
		ActionParser.templatesExpr_return templatesExpr1 = default(ActionParser.templatesExpr_return);
		ActionParser.optionList_return optionList3 = default(ActionParser.optionList_return);
		ActionParser.ifCondition_return ifCondition6 = default(ActionParser.ifCondition_return);
		ActionParser.ifCondition_return ifCondition10 = default(ActionParser.ifCondition_return);

		StringTemplateAST SEMI2_tree=null;
		StringTemplateAST string_literal4_tree=null;
		StringTemplateAST LPAREN5_tree=null;
		StringTemplateAST RPAREN7_tree=null;
		StringTemplateAST string_literal8_tree=null;
		StringTemplateAST LPAREN9_tree=null;
		StringTemplateAST RPAREN11_tree=null;
		StringTemplateAST EOF12_tree=null;

		try
		{
			// Language\\Action.g3:146:4: ( ( templatesExpr ( SEMI optionList )? | 'if' LPAREN ifCondition RPAREN | 'elseif' LPAREN ifCondition RPAREN ) EOF )
			// Language\\Action.g3:146:4: ( templatesExpr ( SEMI optionList )? | 'if' LPAREN ifCondition RPAREN | 'elseif' LPAREN ifCondition RPAREN ) EOF
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			// Language\\Action.g3:146:4: ( templatesExpr ( SEMI optionList )? | 'if' LPAREN ifCondition RPAREN | 'elseif' LPAREN ifCondition RPAREN )
			int alt2=3;
			switch ( input.LA(1) )
			{
			case ANONYMOUS_TEMPLATE:
			case FIRST:
			case ID:
			case INT:
			case LAST:
			case LBRACK:
			case LENGTH:
			case LPAREN:
			case REST:
			case STRING:
			case STRIP:
			case SUPER:
			case TRUNC:
				{
				alt2=1;
				}
				break;
			case CONDITIONAL:
				{
				alt2=2;
				}
				break;
			case ELSEIF:
				{
				alt2=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

					throw nvae;
				}
			}

			switch ( alt2 )
			{
			case 1:
				// Language\\Action.g3:146:6: templatesExpr ( SEMI optionList )?
				{
				PushFollow(Follow._templatesExpr_in_action192);
				templatesExpr1=templatesExpr();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, templatesExpr1.Tree);
				// Language\\Action.g3:146:20: ( SEMI optionList )?
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( (LA1_0==SEMI) )
				{
					alt1=1;
				}
				switch ( alt1 )
				{
				case 1:
					// Language\\Action.g3:146:21: SEMI optionList
					{
					SEMI2=(IToken)Match(input,SEMI,Follow._SEMI_in_action195); if (state.failed) return retval;
					PushFollow(Follow._optionList_in_action198);
					optionList3=optionList();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, optionList3.Tree);
					if ( state.backtracking == 0 )
					{
						retval.opts = (optionList3!=null?optionList3.opts:default(IDictionary<string, object>));
					}

					}
					break;

				}


				}
				break;
			case 2:
				// Language\\Action.g3:147:5: 'if' LPAREN ifCondition RPAREN
				{
				string_literal4=(IToken)Match(input,CONDITIONAL,Follow._CONDITIONAL_in_action208); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				string_literal4_tree = (StringTemplateAST)adaptor.Create(string_literal4);
				root_0 = (StringTemplateAST)adaptor.BecomeRoot(string_literal4_tree, root_0);
				}
				LPAREN5=(IToken)Match(input,LPAREN,Follow._LPAREN_in_action211); if (state.failed) return retval;
				PushFollow(Follow._ifCondition_in_action214);
				ifCondition6=ifCondition();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ifCondition6.Tree);
				RPAREN7=(IToken)Match(input,RPAREN,Follow._RPAREN_in_action216); if (state.failed) return retval;

				}
				break;
			case 3:
				// Language\\Action.g3:148:5: 'elseif' LPAREN ifCondition RPAREN
				{
				string_literal8=(IToken)Match(input,ELSEIF,Follow._ELSEIF_in_action223); if (state.failed) return retval;
				LPAREN9=(IToken)Match(input,LPAREN,Follow._LPAREN_in_action226); if (state.failed) return retval;
				PushFollow(Follow._ifCondition_in_action229);
				ifCondition10=ifCondition();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ifCondition10.Tree);
				RPAREN11=(IToken)Match(input,RPAREN,Follow._RPAREN_in_action231); if (state.failed) return retval;

				}
				break;

			}

			EOF12=(IToken)Match(input,EOF,Follow._EOF_in_action241); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			EOF12_tree = (StringTemplateAST)adaptor.Create(EOF12);
			adaptor.AddChild(root_0, EOF12_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "action"

	public class optionList_return : ParserRuleReturnScope
	{
		public IDictionary<string, object> opts=new Dictionary<string, object>();
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "optionList"
	// Language\\Action.g3:153:0: optionList returns [IDictionary<string, object> opts=new Dictionary<string, object>()] : option[$opts] ( COMMA option[$opts] )* ;
	private ActionParser.optionList_return optionList(  )
	{
		ActionParser.optionList_return retval = new ActionParser.optionList_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken COMMA14=null;
		ActionParser.option_return option13 = default(ActionParser.option_return);
		ActionParser.option_return option15 = default(ActionParser.option_return);

		StringTemplateAST COMMA14_tree=null;

		try
		{
			// Language\\Action.g3:154:4: ( option[$opts] ( COMMA option[$opts] )* )
			// Language\\Action.g3:154:4: option[$opts] ( COMMA option[$opts] )*
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			PushFollow(Follow._option_in_optionList257);
			option13=option(retval.opts);

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, option13.Tree);
			// Language\\Action.g3:154:18: ( COMMA option[$opts] )*
			for ( ; ; )
			{
				int alt3=2;
				int LA3_0 = input.LA(1);

				if ( (LA3_0==COMMA) )
				{
					alt3=1;
				}


				switch ( alt3 )
				{
				case 1:
					// Language\\Action.g3:154:19: COMMA option[$opts]
					{
					COMMA14=(IToken)Match(input,COMMA,Follow._COMMA_in_optionList261); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					COMMA14_tree = (StringTemplateAST)adaptor.Create(COMMA14);
					adaptor.AddChild(root_0, COMMA14_tree);
					}
					PushFollow(Follow._option_in_optionList263);
					option15=option(retval.opts);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, option15.Tree);

					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "optionList"

	public class option_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "option"
	// Language\\Action.g3:157:0: option[IDictionary<string, object> opts] : ID ( ASSIGN nonAlternatingTemplateExpr |) ;
	private ActionParser.option_return option( IDictionary<string, object> opts )
	{
		ActionParser.option_return retval = new ActionParser.option_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken ID16=null;
		IToken ASSIGN17=null;
		ActionParser.nonAlternatingTemplateExpr_return nonAlternatingTemplateExpr18 = default(ActionParser.nonAlternatingTemplateExpr_return);

		StringTemplateAST ID16_tree=null;
		StringTemplateAST ASSIGN17_tree=null;


		object v=null;

		try
		{
			// Language\\Action.g3:162:4: ( ID ( ASSIGN nonAlternatingTemplateExpr |) )
			// Language\\Action.g3:162:4: ID ( ASSIGN nonAlternatingTemplateExpr |)
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			ID16=(IToken)Match(input,ID,Follow._ID_in_option283); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ID16_tree = (StringTemplateAST)adaptor.Create(ID16);
			adaptor.AddChild(root_0, ID16_tree);
			}
			// Language\\Action.g3:163:3: ( ASSIGN nonAlternatingTemplateExpr |)
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==ASSIGN) )
			{
				alt4=1;
			}
			else if ( (LA4_0==EOF||LA4_0==COMMA) )
			{
				alt4=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 4, 0, input);

				throw nvae;
			}
			switch ( alt4 )
			{
			case 1:
				// Language\\Action.g3:163:5: ASSIGN nonAlternatingTemplateExpr
				{
				ASSIGN17=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_option289); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ASSIGN17_tree = (StringTemplateAST)adaptor.Create(ASSIGN17);
				adaptor.AddChild(root_0, ASSIGN17_tree);
				}
				PushFollow(Follow._nonAlternatingTemplateExpr_in_option291);
				nonAlternatingTemplateExpr18=nonAlternatingTemplateExpr();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, nonAlternatingTemplateExpr18.Tree);
				if ( state.backtracking == 0 )
				{
					v=(nonAlternatingTemplateExpr18!=null?((StringTemplateAST)nonAlternatingTemplateExpr18.tree):null);
				}

				}
				break;
			case 2:
				// Language\\Action.g3:164:5: 
				{
				if ( state.backtracking == 0 )
				{
					v=ASTExpr.EmptyOption;
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{
				opts[(ID16!=null?ID16.Text:null)] = v;
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "option"

	public class templatesExpr_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "templatesExpr"
	// Language\\Action.g3:169:0: templatesExpr : expr ( ( COMMA expr )+ colon= COLON anonymousTemplate -> ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate ) | ( -> expr ) (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )* ) ;
	private ActionParser.templatesExpr_return templatesExpr(  )
	{
		ActionParser.templatesExpr_return retval = new ActionParser.templatesExpr_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken colon=null;
		IToken COMMA20=null;
		ActionParser.expr_return expr19 = default(ActionParser.expr_return);
		ActionParser.expr_return expr21 = default(ActionParser.expr_return);
		ActionParser.anonymousTemplate_return anonymousTemplate22 = default(ActionParser.anonymousTemplate_return);
		ActionParser.templateList_return templateList23 = default(ActionParser.templateList_return);

		StringTemplateAST colon_tree=null;
		StringTemplateAST COMMA20_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleSubtreeStream stream_expr=new RewriteRuleSubtreeStream(adaptor,"rule expr");
		RewriteRuleSubtreeStream stream_anonymousTemplate=new RewriteRuleSubtreeStream(adaptor,"rule anonymousTemplate");
		RewriteRuleSubtreeStream stream_templateList=new RewriteRuleSubtreeStream(adaptor,"rule templateList");
		try
		{
			// Language\\Action.g3:171:3: ( expr ( ( COMMA expr )+ colon= COLON anonymousTemplate -> ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate ) | ( -> expr ) (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )* ) )
			// Language\\Action.g3:171:3: expr ( ( COMMA expr )+ colon= COLON anonymousTemplate -> ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate ) | ( -> expr ) (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )* )
			{
			PushFollow(Follow._expr_in_templatesExpr321);
			expr19=expr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expr.Add(expr19.Tree);
			// Language\\Action.g3:172:3: ( ( COMMA expr )+ colon= COLON anonymousTemplate -> ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate ) | ( -> expr ) (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )* )
			int alt7=2;
			int LA7_0 = input.LA(1);

			if ( (LA7_0==COMMA) )
			{
				alt7=1;
			}
			else if ( (LA7_0==EOF||LA7_0==COLON||(LA7_0>=RPAREN && LA7_0<=SEMI)) )
			{
				alt7=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

				throw nvae;
			}
			switch ( alt7 )
			{
			case 1:
				// Language\\Action.g3:172:5: ( COMMA expr )+ colon= COLON anonymousTemplate
				{
				// Language\\Action.g3:172:5: ( COMMA expr )+
				int cnt5=0;
				for ( ; ; )
				{
					int alt5=2;
					int LA5_0 = input.LA(1);

					if ( (LA5_0==COMMA) )
					{
						alt5=1;
					}


					switch ( alt5 )
					{
					case 1:
						// Language\\Action.g3:172:6: COMMA expr
						{
						COMMA20=(IToken)Match(input,COMMA,Follow._COMMA_in_templatesExpr328); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA20);

						PushFollow(Follow._expr_in_templatesExpr330);
						expr21=expr();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_expr.Add(expr21.Tree);

						}
						break;

					default:
						if ( cnt5 >= 1 )
							goto loop5;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee5 = new EarlyExitException( 5, input );
						throw eee5;
					}
					cnt5++;
				}
				loop5:
					;


				colon=(IToken)Match(input,COLON,Follow._COLON_in_templatesExpr336); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COLON.Add(colon);

				PushFollow(Follow._anonymousTemplate_in_templatesExpr338);
				anonymousTemplate22=anonymousTemplate();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_anonymousTemplate.Add(anonymousTemplate22.Tree);


				{
				// AST REWRITE
				// elements: expr, colon, anonymousTemplate
				// token labels: colon
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleITokenStream stream_colon=new RewriteRuleITokenStream(adaptor,"token colon",colon);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 173:4: -> ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate )
				{
					// Language\\Action.g3:173:7: ^( MULTI_APPLY[\"MULTI_APPLY\"] ( expr )+ $colon anonymousTemplate )
					{
					StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
					root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(MULTI_APPLY, "MULTI_APPLY"), root_1);

					if ( !(stream_expr.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_expr.HasNext )
					{
						adaptor.AddChild(root_1, stream_expr.NextTree());

					}
					stream_expr.Reset();
					adaptor.AddChild(root_1, stream_colon.NextNode());
					adaptor.AddChild(root_1, stream_anonymousTemplate.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Language\\Action.g3:174:5: ( -> expr ) (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )*
				{
				// Language\\Action.g3:174:5: ( -> expr )
				// Language\\Action.g3:174:7: 
				{



				{
				// AST REWRITE
				// elements: expr
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 174:7: -> expr
				{
					adaptor.AddChild(root_0, stream_expr.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}

				// Language\\Action.g3:176:4: (colon= COLON templateList -> ^( APPLY[$colon] $templatesExpr templateList ) )*
				for ( ; ; )
				{
					int alt6=2;
					int LA6_0 = input.LA(1);

					if ( (LA6_0==COLON) )
					{
						alt6=1;
					}


					switch ( alt6 )
					{
					case 1:
						// Language\\Action.g3:176:6: colon= COLON templateList
						{
						colon=(IToken)Match(input,COLON,Follow._COLON_in_templatesExpr380); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COLON.Add(colon);

						PushFollow(Follow._templateList_in_templatesExpr382);
						templateList23=templateList();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_templateList.Add(templateList23.Tree);


						{
						// AST REWRITE
						// elements: templatesExpr, templateList
						// token labels: 
						// rule labels: retval
						// token list labels: 
						// rule list labels: 
						// wildcard labels: 
						if ( state.backtracking == 0 ) {
						retval.tree = root_0;
						RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

						root_0 = (StringTemplateAST)adaptor.Nil();
						// 177:5: -> ^( APPLY[$colon] $templatesExpr templateList )
						{
							// Language\\Action.g3:177:8: ^( APPLY[$colon] $templatesExpr templateList )
							{
							StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
							root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(APPLY, colon), root_1);

							adaptor.AddChild(root_1, stream_retval.NextTree());
							adaptor.AddChild(root_1, stream_templateList.NextTree());

							adaptor.AddChild(root_0, root_1);
							}

						}

						retval.tree = root_0;
						}
						}

						}
						break;

					default:
						goto loop6;
					}
				}

				loop6:
					;



				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "templatesExpr"

	public class templateList_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "templateList"
	// Language\\Action.g3:182:0: templateList : template ( COMMA template )* ;
	private ActionParser.templateList_return templateList(  )
	{
		ActionParser.templateList_return retval = new ActionParser.templateList_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken COMMA25=null;
		ActionParser.template_return template24 = default(ActionParser.template_return);
		ActionParser.template_return template26 = default(ActionParser.template_return);

		StringTemplateAST COMMA25_tree=null;

		try
		{
			// Language\\Action.g3:183:4: ( template ( COMMA template )* )
			// Language\\Action.g3:183:4: template ( COMMA template )*
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			PushFollow(Follow._template_in_templateList421);
			template24=template();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, template24.Tree);
			// Language\\Action.g3:183:13: ( COMMA template )*
			for ( ; ; )
			{
				int alt8=2;
				int LA8_0 = input.LA(1);

				if ( (LA8_0==COMMA) )
				{
					alt8=1;
				}


				switch ( alt8 )
				{
				case 1:
					// Language\\Action.g3:183:14: COMMA template
					{
					COMMA25=(IToken)Match(input,COMMA,Follow._COMMA_in_templateList424); if (state.failed) return retval;
					PushFollow(Follow._template_in_templateList427);
					template26=template();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, template26.Tree);

					}
					break;

				default:
					goto loop8;
				}
			}

			loop8:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "templateList"

	public class ifCondition_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ifCondition"
	// Language\\Action.g3:186:0: ifCondition : ( ifAtom | NOT ifAtom );
	private ActionParser.ifCondition_return ifCondition(  )
	{
		ActionParser.ifCondition_return retval = new ActionParser.ifCondition_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken NOT28=null;
		ActionParser.ifAtom_return ifAtom27 = default(ActionParser.ifAtom_return);
		ActionParser.ifAtom_return ifAtom29 = default(ActionParser.ifAtom_return);

		StringTemplateAST NOT28_tree=null;

		try
		{
			// Language\\Action.g3:187:4: ( ifAtom | NOT ifAtom )
			int alt9=2;
			int LA9_0 = input.LA(1);

			if ( (LA9_0==ANONYMOUS_TEMPLATE||LA9_0==FIRST||LA9_0==ID||(LA9_0>=INT && LA9_0<=LENGTH)||LA9_0==LPAREN||LA9_0==REST||(LA9_0>=STRING && LA9_0<=SUPER)||LA9_0==TRUNC) )
			{
				alt9=1;
			}
			else if ( (LA9_0==NOT) )
			{
				alt9=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 9, 0, input);

				throw nvae;
			}
			switch ( alt9 )
			{
			case 1:
				// Language\\Action.g3:187:4: ifAtom
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._ifAtom_in_ifCondition440);
				ifAtom27=ifAtom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ifAtom27.Tree);

				}
				break;
			case 2:
				// Language\\Action.g3:188:4: NOT ifAtom
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				NOT28=(IToken)Match(input,NOT,Follow._NOT_in_ifCondition445); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				NOT28_tree = (StringTemplateAST)adaptor.Create(NOT28);
				root_0 = (StringTemplateAST)adaptor.BecomeRoot(NOT28_tree, root_0);
				}
				PushFollow(Follow._ifAtom_in_ifCondition448);
				ifAtom29=ifAtom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ifAtom29.Tree);

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ifCondition"

	public class ifAtom_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ifAtom"
	// Language\\Action.g3:191:0: ifAtom : templatesExpr ;
	private ActionParser.ifAtom_return ifAtom(  )
	{
		ActionParser.ifAtom_return retval = new ActionParser.ifAtom_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		ActionParser.templatesExpr_return templatesExpr30 = default(ActionParser.templatesExpr_return);


		try
		{
			// Language\\Action.g3:192:4: ( templatesExpr )
			// Language\\Action.g3:192:4: templatesExpr
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			PushFollow(Follow._templatesExpr_in_ifAtom459);
			templatesExpr30=templatesExpr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, templatesExpr30.Tree);

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ifAtom"

	public class expr_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "expr"
	// Language\\Action.g3:195:0: expr : primaryExpr ( PLUS primaryExpr )* ;
	private ActionParser.expr_return expr(  )
	{
		ActionParser.expr_return retval = new ActionParser.expr_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken PLUS32=null;
		ActionParser.primaryExpr_return primaryExpr31 = default(ActionParser.primaryExpr_return);
		ActionParser.primaryExpr_return primaryExpr33 = default(ActionParser.primaryExpr_return);

		StringTemplateAST PLUS32_tree=null;

		try
		{
			// Language\\Action.g3:196:4: ( primaryExpr ( PLUS primaryExpr )* )
			// Language\\Action.g3:196:4: primaryExpr ( PLUS primaryExpr )*
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			PushFollow(Follow._primaryExpr_in_expr470);
			primaryExpr31=primaryExpr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, primaryExpr31.Tree);
			// Language\\Action.g3:196:16: ( PLUS primaryExpr )*
			for ( ; ; )
			{
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( (LA10_0==PLUS) )
				{
					alt10=1;
				}


				switch ( alt10 )
				{
				case 1:
					// Language\\Action.g3:196:17: PLUS primaryExpr
					{
					PLUS32=(IToken)Match(input,PLUS,Follow._PLUS_in_expr473); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					PLUS32_tree = (StringTemplateAST)adaptor.Create(PLUS32);
					root_0 = (StringTemplateAST)adaptor.BecomeRoot(PLUS32_tree, root_0);
					}
					PushFollow(Follow._primaryExpr_in_expr476);
					primaryExpr33=primaryExpr();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, primaryExpr33.Tree);

					}
					break;

				default:
					goto loop10;
				}
			}

			loop10:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "expr"

	public class primaryExpr_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "primaryExpr"
	// Language\\Action.g3:199:0: primaryExpr : (=> templateInclude | atom ( DOT ( ID | valueExpr ) )* | function ( DOT ( ID | valueExpr ) )* | valueExpr | list );
	private ActionParser.primaryExpr_return primaryExpr(  )
	{
		ActionParser.primaryExpr_return retval = new ActionParser.primaryExpr_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken DOT36=null;
		IToken ID37=null;
		IToken DOT40=null;
		IToken ID41=null;
		ActionParser.templateInclude_return templateInclude34 = default(ActionParser.templateInclude_return);
		ActionParser.atom_return atom35 = default(ActionParser.atom_return);
		ActionParser.valueExpr_return valueExpr38 = default(ActionParser.valueExpr_return);
		ActionParser.function_return function39 = default(ActionParser.function_return);
		ActionParser.valueExpr_return valueExpr42 = default(ActionParser.valueExpr_return);
		ActionParser.valueExpr_return valueExpr43 = default(ActionParser.valueExpr_return);
		ActionParser.list_return list44 = default(ActionParser.list_return);

		StringTemplateAST DOT36_tree=null;
		StringTemplateAST ID37_tree=null;
		StringTemplateAST DOT40_tree=null;
		StringTemplateAST ID41_tree=null;

		try
		{
			// Language\\Action.g3:200:4: (=> templateInclude | atom ( DOT ( ID | valueExpr ) )* | function ( DOT ( ID | valueExpr ) )* | valueExpr | list )
			int alt15=5;
			alt15 = dfa15.Predict(input);
			switch ( alt15 )
			{
			case 1:
				// Language\\Action.g3:200:4: => templateInclude
				{
				root_0 = (StringTemplateAST)adaptor.Nil();


				PushFollow(Follow._templateInclude_in_primaryExpr493);
				templateInclude34=templateInclude();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, templateInclude34.Tree);

				}
				break;
			case 2:
				// Language\\Action.g3:201:4: atom ( DOT ( ID | valueExpr ) )*
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._atom_in_primaryExpr500);
				atom35=atom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, atom35.Tree);
				// Language\\Action.g3:202:3: ( DOT ( ID | valueExpr ) )*
				for ( ; ; )
				{
					int alt12=2;
					int LA12_0 = input.LA(1);

					if ( (LA12_0==DOT) )
					{
						alt12=1;
					}


					switch ( alt12 )
					{
					case 1:
						// Language\\Action.g3:202:5: DOT ( ID | valueExpr )
						{
						DOT36=(IToken)Match(input,DOT,Follow._DOT_in_primaryExpr506); if (state.failed) return retval;
						if ( state.backtracking == 0 ) {
						DOT36_tree = (StringTemplateAST)adaptor.Create(DOT36);
						root_0 = (StringTemplateAST)adaptor.BecomeRoot(DOT36_tree, root_0);
						}
						// Language\\Action.g3:203:4: ( ID | valueExpr )
						int alt11=2;
						int LA11_0 = input.LA(1);

						if ( (LA11_0==ID) )
						{
							alt11=1;
						}
						else if ( (LA11_0==LPAREN) )
						{
							alt11=2;
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return retval;}
							NoViableAltException nvae = new NoViableAltException("", 11, 0, input);

							throw nvae;
						}
						switch ( alt11 )
						{
						case 1:
							// Language\\Action.g3:203:6: ID
							{
							ID37=(IToken)Match(input,ID,Follow._ID_in_primaryExpr515); if (state.failed) return retval;
							if ( state.backtracking==0 ) {
							ID37_tree = (StringTemplateAST)adaptor.Create(ID37);
							adaptor.AddChild(root_0, ID37_tree);
							}

							}
							break;
						case 2:
							// Language\\Action.g3:204:6: valueExpr
							{
							PushFollow(Follow._valueExpr_in_primaryExpr522);
							valueExpr38=valueExpr();

							state._fsp--;
							if (state.failed) return retval;
							if ( state.backtracking == 0 ) adaptor.AddChild(root_0, valueExpr38.Tree);

							}
							break;

						}


						}
						break;

					default:
						goto loop12;
					}
				}

				loop12:
					;



				}
				break;
			case 3:
				// Language\\Action.g3:207:4: function ( DOT ( ID | valueExpr ) )*
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._function_in_primaryExpr537);
				function39=function();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, function39.Tree);
				// Language\\Action.g3:208:3: ( DOT ( ID | valueExpr ) )*
				for ( ; ; )
				{
					int alt14=2;
					int LA14_0 = input.LA(1);

					if ( (LA14_0==DOT) )
					{
						alt14=1;
					}


					switch ( alt14 )
					{
					case 1:
						// Language\\Action.g3:208:5: DOT ( ID | valueExpr )
						{
						DOT40=(IToken)Match(input,DOT,Follow._DOT_in_primaryExpr543); if (state.failed) return retval;
						if ( state.backtracking == 0 ) {
						DOT40_tree = (StringTemplateAST)adaptor.Create(DOT40);
						root_0 = (StringTemplateAST)adaptor.BecomeRoot(DOT40_tree, root_0);
						}
						// Language\\Action.g3:209:4: ( ID | valueExpr )
						int alt13=2;
						int LA13_0 = input.LA(1);

						if ( (LA13_0==ID) )
						{
							alt13=1;
						}
						else if ( (LA13_0==LPAREN) )
						{
							alt13=2;
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return retval;}
							NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

							throw nvae;
						}
						switch ( alt13 )
						{
						case 1:
							// Language\\Action.g3:209:6: ID
							{
							ID41=(IToken)Match(input,ID,Follow._ID_in_primaryExpr551); if (state.failed) return retval;
							if ( state.backtracking==0 ) {
							ID41_tree = (StringTemplateAST)adaptor.Create(ID41);
							adaptor.AddChild(root_0, ID41_tree);
							}

							}
							break;
						case 2:
							// Language\\Action.g3:210:6: valueExpr
							{
							PushFollow(Follow._valueExpr_in_primaryExpr558);
							valueExpr42=valueExpr();

							state._fsp--;
							if (state.failed) return retval;
							if ( state.backtracking == 0 ) adaptor.AddChild(root_0, valueExpr42.Tree);

							}
							break;

						}


						}
						break;

					default:
						goto loop14;
					}
				}

				loop14:
					;



				}
				break;
			case 4:
				// Language\\Action.g3:213:4: valueExpr
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._valueExpr_in_primaryExpr573);
				valueExpr43=valueExpr();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, valueExpr43.Tree);

				}
				break;
			case 5:
				// Language\\Action.g3:214:4: list
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._list_in_primaryExpr578);
				list44=list();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, list44.Tree);

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "primaryExpr"

	public class valueExpr_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "valueExpr"
	// Language\\Action.g3:217:0: valueExpr : LPAREN templatesExpr RPAREN -> ^( VALUE[$LPAREN,\"value\"] templatesExpr ) ;
	private ActionParser.valueExpr_return valueExpr(  )
	{
		ActionParser.valueExpr_return retval = new ActionParser.valueExpr_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken LPAREN45=null;
		IToken RPAREN47=null;
		ActionParser.templatesExpr_return templatesExpr46 = default(ActionParser.templatesExpr_return);

		StringTemplateAST LPAREN45_tree=null;
		StringTemplateAST RPAREN47_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_templatesExpr=new RewriteRuleSubtreeStream(adaptor,"rule templatesExpr");
		try
		{
			// Language\\Action.g3:218:4: ( LPAREN templatesExpr RPAREN -> ^( VALUE[$LPAREN,\"value\"] templatesExpr ) )
			// Language\\Action.g3:218:4: LPAREN templatesExpr RPAREN
			{
			LPAREN45=(IToken)Match(input,LPAREN,Follow._LPAREN_in_valueExpr589); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN45);

			PushFollow(Follow._templatesExpr_in_valueExpr591);
			templatesExpr46=templatesExpr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_templatesExpr.Add(templatesExpr46.Tree);
			RPAREN47=(IToken)Match(input,RPAREN,Follow._RPAREN_in_valueExpr593); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN47);



			{
			// AST REWRITE
			// elements: templatesExpr
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 219:3: -> ^( VALUE[$LPAREN,\"value\"] templatesExpr )
			{
				// Language\\Action.g3:219:6: ^( VALUE[$LPAREN,\"value\"] templatesExpr )
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(VALUE, LPAREN45, "value"), root_1);

				adaptor.AddChild(root_1, stream_templatesExpr.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "valueExpr"

	public class nonAlternatingTemplateExpr_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "nonAlternatingTemplateExpr"
	// Language\\Action.g3:222:0: nonAlternatingTemplateExpr : ( expr -> expr ) ( COLON template -> ^( APPLY[$COLON] $nonAlternatingTemplateExpr template ) )* ;
	private ActionParser.nonAlternatingTemplateExpr_return nonAlternatingTemplateExpr(  )
	{
		ActionParser.nonAlternatingTemplateExpr_return retval = new ActionParser.nonAlternatingTemplateExpr_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken COLON49=null;
		ActionParser.expr_return expr48 = default(ActionParser.expr_return);
		ActionParser.template_return template50 = default(ActionParser.template_return);

		StringTemplateAST COLON49_tree=null;
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleSubtreeStream stream_expr=new RewriteRuleSubtreeStream(adaptor,"rule expr");
		RewriteRuleSubtreeStream stream_template=new RewriteRuleSubtreeStream(adaptor,"rule template");
		try
		{
			// Language\\Action.g3:223:4: ( ( expr -> expr ) ( COLON template -> ^( APPLY[$COLON] $nonAlternatingTemplateExpr template ) )* )
			// Language\\Action.g3:223:4: ( expr -> expr ) ( COLON template -> ^( APPLY[$COLON] $nonAlternatingTemplateExpr template ) )*
			{
			// Language\\Action.g3:223:4: ( expr -> expr )
			// Language\\Action.g3:223:5: expr
			{
			PushFollow(Follow._expr_in_nonAlternatingTemplateExpr616);
			expr48=expr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expr.Add(expr48.Tree);


			{
			// AST REWRITE
			// elements: expr
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 223:10: -> expr
			{
				adaptor.AddChild(root_0, stream_expr.NextTree());

			}

			retval.tree = root_0;
			}
			}

			}

			// Language\\Action.g3:223:19: ( COLON template -> ^( APPLY[$COLON] $nonAlternatingTemplateExpr template ) )*
			for ( ; ; )
			{
				int alt16=2;
				int LA16_0 = input.LA(1);

				if ( (LA16_0==COLON) )
				{
					alt16=1;
				}


				switch ( alt16 )
				{
				case 1:
					// Language\\Action.g3:223:21: COLON template
					{
					COLON49=(IToken)Match(input,COLON,Follow._COLON_in_nonAlternatingTemplateExpr625); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COLON.Add(COLON49);

					PushFollow(Follow._template_in_nonAlternatingTemplateExpr627);
					template50=template();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_template.Add(template50.Tree);


					{
					// AST REWRITE
					// elements: nonAlternatingTemplateExpr, template
					// token labels: 
					// rule labels: retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.tree = root_0;
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

					root_0 = (StringTemplateAST)adaptor.Nil();
					// 223:36: -> ^( APPLY[$COLON] $nonAlternatingTemplateExpr template )
					{
						// Language\\Action.g3:223:39: ^( APPLY[$COLON] $nonAlternatingTemplateExpr template )
						{
						StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
						root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(APPLY, COLON49), root_1);

						adaptor.AddChild(root_1, stream_retval.NextTree());
						adaptor.AddChild(root_1, stream_template.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop16;
				}
			}

			loop16:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "nonAlternatingTemplateExpr"

	public class function_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "function"
	// Language\\Action.g3:226:0: function : ( 'first' | 'rest' | 'last' | 'length' | 'strip' | 'trunc' ) singleArg -> ^( FUNCTION ( 'first' )? ( 'rest' )? ( 'last' )? ( 'length' )? ( 'strip' )? ( 'trunc' )? singleArg ) ;
	private ActionParser.function_return function(  )
	{
		ActionParser.function_return retval = new ActionParser.function_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken string_literal51=null;
		IToken string_literal52=null;
		IToken string_literal53=null;
		IToken string_literal54=null;
		IToken string_literal55=null;
		IToken string_literal56=null;
		ActionParser.singleArg_return singleArg57 = default(ActionParser.singleArg_return);

		StringTemplateAST string_literal51_tree=null;
		StringTemplateAST string_literal52_tree=null;
		StringTemplateAST string_literal53_tree=null;
		StringTemplateAST string_literal54_tree=null;
		StringTemplateAST string_literal55_tree=null;
		StringTemplateAST string_literal56_tree=null;
		RewriteRuleITokenStream stream_FIRST=new RewriteRuleITokenStream(adaptor,"token FIRST");
		RewriteRuleITokenStream stream_REST=new RewriteRuleITokenStream(adaptor,"token REST");
		RewriteRuleITokenStream stream_LAST=new RewriteRuleITokenStream(adaptor,"token LAST");
		RewriteRuleITokenStream stream_LENGTH=new RewriteRuleITokenStream(adaptor,"token LENGTH");
		RewriteRuleITokenStream stream_STRIP=new RewriteRuleITokenStream(adaptor,"token STRIP");
		RewriteRuleITokenStream stream_TRUNC=new RewriteRuleITokenStream(adaptor,"token TRUNC");
		RewriteRuleSubtreeStream stream_singleArg=new RewriteRuleSubtreeStream(adaptor,"rule singleArg");
		try
		{
			// Language\\Action.g3:227:4: ( ( 'first' | 'rest' | 'last' | 'length' | 'strip' | 'trunc' ) singleArg -> ^( FUNCTION ( 'first' )? ( 'rest' )? ( 'last' )? ( 'length' )? ( 'strip' )? ( 'trunc' )? singleArg ) )
			// Language\\Action.g3:227:4: ( 'first' | 'rest' | 'last' | 'length' | 'strip' | 'trunc' ) singleArg
			{
			// Language\\Action.g3:227:4: ( 'first' | 'rest' | 'last' | 'length' | 'strip' | 'trunc' )
			int alt17=6;
			switch ( input.LA(1) )
			{
			case FIRST:
				{
				alt17=1;
				}
				break;
			case REST:
				{
				alt17=2;
				}
				break;
			case LAST:
				{
				alt17=3;
				}
				break;
			case LENGTH:
				{
				alt17=4;
				}
				break;
			case STRIP:
				{
				alt17=5;
				}
				break;
			case TRUNC:
				{
				alt17=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 17, 0, input);

					throw nvae;
				}
			}

			switch ( alt17 )
			{
			case 1:
				// Language\\Action.g3:227:6: 'first'
				{
				string_literal51=(IToken)Match(input,FIRST,Follow._FIRST_in_function655); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_FIRST.Add(string_literal51);


				}
				break;
			case 2:
				// Language\\Action.g3:228:5: 'rest'
				{
				string_literal52=(IToken)Match(input,REST,Follow._REST_in_function661); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_REST.Add(string_literal52);


				}
				break;
			case 3:
				// Language\\Action.g3:229:5: 'last'
				{
				string_literal53=(IToken)Match(input,LAST,Follow._LAST_in_function667); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LAST.Add(string_literal53);


				}
				break;
			case 4:
				// Language\\Action.g3:230:5: 'length'
				{
				string_literal54=(IToken)Match(input,LENGTH,Follow._LENGTH_in_function673); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LENGTH.Add(string_literal54);


				}
				break;
			case 5:
				// Language\\Action.g3:231:5: 'strip'
				{
				string_literal55=(IToken)Match(input,STRIP,Follow._STRIP_in_function679); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STRIP.Add(string_literal55);


				}
				break;
			case 6:
				// Language\\Action.g3:232:5: 'trunc'
				{
				string_literal56=(IToken)Match(input,TRUNC,Follow._TRUNC_in_function685); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TRUNC.Add(string_literal56);


				}
				break;

			}

			PushFollow(Follow._singleArg_in_function693);
			singleArg57=singleArg();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_singleArg.Add(singleArg57.Tree);


			{
			// AST REWRITE
			// elements: FIRST, REST, LAST, LENGTH, STRIP, TRUNC, singleArg
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 235:3: -> ^( FUNCTION ( 'first' )? ( 'rest' )? ( 'last' )? ( 'length' )? ( 'strip' )? ( 'trunc' )? singleArg )
			{
				// Language\\Action.g3:235:6: ^( FUNCTION ( 'first' )? ( 'rest' )? ( 'last' )? ( 'length' )? ( 'strip' )? ( 'trunc' )? singleArg )
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

				// Language\\Action.g3:235:17: ( 'first' )?
				if ( stream_FIRST.HasNext )
				{
					adaptor.AddChild(root_1, stream_FIRST.NextNode());

				}
				stream_FIRST.Reset();
				// Language\\Action.g3:235:26: ( 'rest' )?
				if ( stream_REST.HasNext )
				{
					adaptor.AddChild(root_1, stream_REST.NextNode());

				}
				stream_REST.Reset();
				// Language\\Action.g3:235:34: ( 'last' )?
				if ( stream_LAST.HasNext )
				{
					adaptor.AddChild(root_1, stream_LAST.NextNode());

				}
				stream_LAST.Reset();
				// Language\\Action.g3:235:42: ( 'length' )?
				if ( stream_LENGTH.HasNext )
				{
					adaptor.AddChild(root_1, stream_LENGTH.NextNode());

				}
				stream_LENGTH.Reset();
				// Language\\Action.g3:235:52: ( 'strip' )?
				if ( stream_STRIP.HasNext )
				{
					adaptor.AddChild(root_1, stream_STRIP.NextNode());

				}
				stream_STRIP.Reset();
				// Language\\Action.g3:235:61: ( 'trunc' )?
				if ( stream_TRUNC.HasNext )
				{
					adaptor.AddChild(root_1, stream_TRUNC.NextNode());

				}
				stream_TRUNC.Reset();
				adaptor.AddChild(root_1, stream_singleArg.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "function"

	public class template_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "template"
	// Language\\Action.g3:238:0: template : ( namedTemplate | anonymousTemplate ) -> ^( TEMPLATE ( namedTemplate )? ( anonymousTemplate )? ) ;
	private ActionParser.template_return template(  )
	{
		ActionParser.template_return retval = new ActionParser.template_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		ActionParser.namedTemplate_return namedTemplate58 = default(ActionParser.namedTemplate_return);
		ActionParser.anonymousTemplate_return anonymousTemplate59 = default(ActionParser.anonymousTemplate_return);

		RewriteRuleSubtreeStream stream_namedTemplate=new RewriteRuleSubtreeStream(adaptor,"rule namedTemplate");
		RewriteRuleSubtreeStream stream_anonymousTemplate=new RewriteRuleSubtreeStream(adaptor,"rule anonymousTemplate");
		try
		{
			// Language\\Action.g3:239:4: ( ( namedTemplate | anonymousTemplate ) -> ^( TEMPLATE ( namedTemplate )? ( anonymousTemplate )? ) )
			// Language\\Action.g3:239:4: ( namedTemplate | anonymousTemplate )
			{
			// Language\\Action.g3:239:4: ( namedTemplate | anonymousTemplate )
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( (LA18_0==ID||LA18_0==LPAREN||LA18_0==SUPER) )
			{
				alt18=1;
			}
			else if ( (LA18_0==ANONYMOUS_TEMPLATE) )
			{
				alt18=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 18, 0, input);

				throw nvae;
			}
			switch ( alt18 )
			{
			case 1:
				// Language\\Action.g3:239:6: namedTemplate
				{
				PushFollow(Follow._namedTemplate_in_template734);
				namedTemplate58=namedTemplate();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_namedTemplate.Add(namedTemplate58.Tree);

				}
				break;
			case 2:
				// Language\\Action.g3:240:5: anonymousTemplate
				{
				PushFollow(Follow._anonymousTemplate_in_template743);
				anonymousTemplate59=anonymousTemplate();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_anonymousTemplate.Add(anonymousTemplate59.Tree);

				}
				break;

			}



			{
			// AST REWRITE
			// elements: namedTemplate, anonymousTemplate
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 242:3: -> ^( TEMPLATE ( namedTemplate )? ( anonymousTemplate )? )
			{
				// Language\\Action.g3:242:6: ^( TEMPLATE ( namedTemplate )? ( anonymousTemplate )? )
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(TEMPLATE, "TEMPLATE"), root_1);

				// Language\\Action.g3:242:17: ( namedTemplate )?
				if ( stream_namedTemplate.HasNext )
				{
					adaptor.AddChild(root_1, stream_namedTemplate.NextTree());

				}
				stream_namedTemplate.Reset();
				// Language\\Action.g3:242:32: ( anonymousTemplate )?
				if ( stream_anonymousTemplate.HasNext )
				{
					adaptor.AddChild(root_1, stream_anonymousTemplate.NextTree());

				}
				stream_anonymousTemplate.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "template"

	public class namedTemplate_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "namedTemplate"
	// Language\\Action.g3:245:0: namedTemplate : ( ID argList -> ID argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate );
	private ActionParser.namedTemplate_return namedTemplate(  )
	{
		ActionParser.namedTemplate_return retval = new ActionParser.namedTemplate_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken qid=null;
		IToken ID60=null;
		IToken string_literal62=null;
		IToken DOT63=null;
		ActionParser.argList_return argList61 = default(ActionParser.argList_return);
		ActionParser.argList_return argList64 = default(ActionParser.argList_return);
		ActionParser.indirectTemplate_return indirectTemplate65 = default(ActionParser.indirectTemplate_return);

		StringTemplateAST qid_tree=null;
		StringTemplateAST ID60_tree=null;
		StringTemplateAST string_literal62_tree=null;
		StringTemplateAST DOT63_tree=null;
		RewriteRuleITokenStream stream_ID=new RewriteRuleITokenStream(adaptor,"token ID");
		RewriteRuleITokenStream stream_SUPER=new RewriteRuleITokenStream(adaptor,"token SUPER");
		RewriteRuleITokenStream stream_DOT=new RewriteRuleITokenStream(adaptor,"token DOT");
		RewriteRuleSubtreeStream stream_argList=new RewriteRuleSubtreeStream(adaptor,"rule argList");
		RewriteRuleSubtreeStream stream_indirectTemplate=new RewriteRuleSubtreeStream(adaptor,"rule indirectTemplate");
		try
		{
			// Language\\Action.g3:246:4: ( ID argList -> ID argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate )
			int alt19=3;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt19=1;
				}
				break;
			case SUPER:
				{
				alt19=2;
				}
				break;
			case LPAREN:
				{
				alt19=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 19, 0, input);

					throw nvae;
				}
			}

			switch ( alt19 )
			{
			case 1:
				// Language\\Action.g3:246:4: ID argList
				{
				ID60=(IToken)Match(input,ID,Follow._ID_in_namedTemplate774); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ID.Add(ID60);

				PushFollow(Follow._argList_in_namedTemplate776);
				argList61=argList();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_argList.Add(argList61.Tree);


				{
				// AST REWRITE
				// elements: ID, argList
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 247:3: -> ID argList
				{
					adaptor.AddChild(root_0, stream_ID.NextNode());
					adaptor.AddChild(root_0, stream_argList.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Language\\Action.g3:248:4: 'super' DOT qid= ID argList
				{
				string_literal62=(IToken)Match(input,SUPER,Follow._SUPER_in_namedTemplate789); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_SUPER.Add(string_literal62);

				DOT63=(IToken)Match(input,DOT,Follow._DOT_in_namedTemplate791); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_DOT.Add(DOT63);

				qid=(IToken)Match(input,ID,Follow._ID_in_namedTemplate795); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ID.Add(qid);

				PushFollow(Follow._argList_in_namedTemplate797);
				argList64=argList();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_argList.Add(argList64.Tree);


				{
				// AST REWRITE
				// elements: ID, argList
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 249:3: -> ID[$qid,\"super.\"+$qid.text] argList
				{
					adaptor.AddChild(root_0, (StringTemplateAST)adaptor.Create(ID, qid, "super."+(qid!=null?qid.Text:null)));
					adaptor.AddChild(root_0, stream_argList.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Language\\Action.g3:250:4: indirectTemplate
				{
				PushFollow(Follow._indirectTemplate_in_namedTemplate811);
				indirectTemplate65=indirectTemplate();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_indirectTemplate.Add(indirectTemplate65.Tree);


				{
				// AST REWRITE
				// elements: indirectTemplate
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 251:3: -> indirectTemplate
				{
					adaptor.AddChild(root_0, stream_indirectTemplate.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "namedTemplate"

	public class anonymousTemplate_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "anonymousTemplate"
	// Language\\Action.g3:254:0: anonymousTemplate : t= ANONYMOUS_TEMPLATE ;
	private ActionParser.anonymousTemplate_return anonymousTemplate(  )
	{
		ActionParser.anonymousTemplate_return retval = new ActionParser.anonymousTemplate_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken t=null;

		StringTemplateAST t_tree=null;


			StringTemplate anonymous = null;

		try
		{
			// Language\\Action.g3:263:4: (t= ANONYMOUS_TEMPLATE )
			// Language\\Action.g3:263:4: t= ANONYMOUS_TEMPLATE
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			t=(IToken)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_anonymousTemplate840); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			t_tree = (StringTemplateAST)adaptor.Create(t);
			adaptor.AddChild(root_0, t_tree);
			}
			if ( state.backtracking == 0 )
			{

							anonymous = new StringTemplate();
							anonymous.Group = self.Group;
							anonymous.EnclosingInstance = self;
							anonymous.Template = (t!=null?t.Text:null);
							if ( t is StringTemplateToken )
								anonymous.DefineFormalArguments(((StringTemplateToken)t).args);
							else
								anonymous.DefineFormalArguments(new string[0]);
						
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
			if ( state.backtracking == 0 )
			{

					((StringTemplateAST)retval.tree).StringTemplate = anonymous;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "anonymousTemplate"

	public class atom_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "atom"
	// Language\\Action.g3:276:0: atom : ( ID | STRING | INT | ANONYMOUS_TEMPLATE );
	private ActionParser.atom_return atom(  )
	{
		ActionParser.atom_return retval = new ActionParser.atom_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken set66=null;

		StringTemplateAST set66_tree=null;

		try
		{
			// Language\\Action.g3:277:4: ( ID | STRING | INT | ANONYMOUS_TEMPLATE )
			// Language\\Action.g3:
			{
			root_0 = (StringTemplateAST)adaptor.Nil();

			set66=(IToken)input.LT(1);
			if ( input.LA(1)==ANONYMOUS_TEMPLATE||input.LA(1)==ID||input.LA(1)==INT||input.LA(1)==STRING )
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (StringTemplateAST)adaptor.Create(set66));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				throw mse;
			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "atom"

	public class list_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "list"
	// Language\\Action.g3:283:0: list : lb= LBRACK listElement ( COMMA listElement )* RBRACK -> ^( LIST[$lb,\"value\"] ( listElement )+ ) ;
	private ActionParser.list_return list(  )
	{
		ActionParser.list_return retval = new ActionParser.list_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken lb=null;
		IToken COMMA68=null;
		IToken RBRACK70=null;
		ActionParser.listElement_return listElement67 = default(ActionParser.listElement_return);
		ActionParser.listElement_return listElement69 = default(ActionParser.listElement_return);

		StringTemplateAST lb_tree=null;
		StringTemplateAST COMMA68_tree=null;
		StringTemplateAST RBRACK70_tree=null;
		RewriteRuleITokenStream stream_LBRACK=new RewriteRuleITokenStream(adaptor,"token LBRACK");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_RBRACK=new RewriteRuleITokenStream(adaptor,"token RBRACK");
		RewriteRuleSubtreeStream stream_listElement=new RewriteRuleSubtreeStream(adaptor,"rule listElement");
		try
		{
			// Language\\Action.g3:284:4: (lb= LBRACK listElement ( COMMA listElement )* RBRACK -> ^( LIST[$lb,\"value\"] ( listElement )+ ) )
			// Language\\Action.g3:284:4: lb= LBRACK listElement ( COMMA listElement )* RBRACK
			{
			lb=(IToken)Match(input,LBRACK,Follow._LBRACK_in_list883); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACK.Add(lb);

			PushFollow(Follow._listElement_in_list887);
			listElement67=listElement();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_listElement.Add(listElement67.Tree);
			// Language\\Action.g3:285:15: ( COMMA listElement )*
			for ( ; ; )
			{
				int alt20=2;
				int LA20_0 = input.LA(1);

				if ( (LA20_0==COMMA) )
				{
					alt20=1;
				}


				switch ( alt20 )
				{
				case 1:
					// Language\\Action.g3:285:16: COMMA listElement
					{
					COMMA68=(IToken)Match(input,COMMA,Follow._COMMA_in_list890); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA68);

					PushFollow(Follow._listElement_in_list892);
					listElement69=listElement();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_listElement.Add(listElement69.Tree);

					}
					break;

				default:
					goto loop20;
				}
			}

			loop20:
				;


			RBRACK70=(IToken)Match(input,RBRACK,Follow._RBRACK_in_list898); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACK.Add(RBRACK70);



			{
			// AST REWRITE
			// elements: listElement
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 287:3: -> ^( LIST[$lb,\"value\"] ( listElement )+ )
			{
				// Language\\Action.g3:287:6: ^( LIST[$lb,\"value\"] ( listElement )+ )
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(LIST, lb, "value"), root_1);

				if ( !(stream_listElement.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_listElement.HasNext )
				{
					adaptor.AddChild(root_1, stream_listElement.NextTree());

				}
				stream_listElement.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "list"

	public class listElement_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "listElement"
	// Language\\Action.g3:290:0: listElement : ( nonAlternatingTemplateExpr | -> NOTHING[\"NOTHING\"] );
	private ActionParser.listElement_return listElement(  )
	{
		ActionParser.listElement_return retval = new ActionParser.listElement_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		ActionParser.nonAlternatingTemplateExpr_return nonAlternatingTemplateExpr71 = default(ActionParser.nonAlternatingTemplateExpr_return);


		try
		{
			// Language\\Action.g3:291:4: ( nonAlternatingTemplateExpr | -> NOTHING[\"NOTHING\"] )
			int alt21=2;
			int LA21_0 = input.LA(1);

			if ( (LA21_0==ANONYMOUS_TEMPLATE||LA21_0==FIRST||LA21_0==ID||(LA21_0>=INT && LA21_0<=LENGTH)||LA21_0==LPAREN||LA21_0==REST||(LA21_0>=STRING && LA21_0<=SUPER)||LA21_0==TRUNC) )
			{
				alt21=1;
			}
			else if ( (LA21_0==COMMA||LA21_0==RBRACK) )
			{
				alt21=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 21, 0, input);

				throw nvae;
			}
			switch ( alt21 )
			{
			case 1:
				// Language\\Action.g3:291:4: nonAlternatingTemplateExpr
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._nonAlternatingTemplateExpr_in_listElement921);
				nonAlternatingTemplateExpr71=nonAlternatingTemplateExpr();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, nonAlternatingTemplateExpr71.Tree);

				}
				break;
			case 2:
				// Language\\Action.g3:293:3: 
				{



				{
				// AST REWRITE
				// elements: 
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 293:3: -> NOTHING[\"NOTHING\"]
				{
					adaptor.AddChild(root_0, (StringTemplateAST)adaptor.Create(NOTHING, "NOTHING"));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "listElement"

	public class templateInclude_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "templateInclude"
	// Language\\Action.g3:296:0: templateInclude : (id= ID argList -> $id argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate ) -> ^( INCLUDE[\"include\"] $templateInclude) ;
	private ActionParser.templateInclude_return templateInclude(  )
	{
		ActionParser.templateInclude_return retval = new ActionParser.templateInclude_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken id=null;
		IToken qid=null;
		IToken string_literal73=null;
		IToken DOT74=null;
		ActionParser.argList_return argList72 = default(ActionParser.argList_return);
		ActionParser.argList_return argList75 = default(ActionParser.argList_return);
		ActionParser.indirectTemplate_return indirectTemplate76 = default(ActionParser.indirectTemplate_return);

		StringTemplateAST id_tree=null;
		StringTemplateAST qid_tree=null;
		StringTemplateAST string_literal73_tree=null;
		StringTemplateAST DOT74_tree=null;
		RewriteRuleITokenStream stream_ID=new RewriteRuleITokenStream(adaptor,"token ID");
		RewriteRuleITokenStream stream_SUPER=new RewriteRuleITokenStream(adaptor,"token SUPER");
		RewriteRuleITokenStream stream_DOT=new RewriteRuleITokenStream(adaptor,"token DOT");
		RewriteRuleSubtreeStream stream_argList=new RewriteRuleSubtreeStream(adaptor,"rule argList");
		RewriteRuleSubtreeStream stream_indirectTemplate=new RewriteRuleSubtreeStream(adaptor,"rule indirectTemplate");
		try
		{
			// Language\\Action.g3:297:4: ( (id= ID argList -> $id argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate ) -> ^( INCLUDE[\"include\"] $templateInclude) )
			// Language\\Action.g3:297:4: (id= ID argList -> $id argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate )
			{
			// Language\\Action.g3:297:4: (id= ID argList -> $id argList | 'super' DOT qid= ID argList -> ID[$qid,\"super.\"+$qid.text] argList | indirectTemplate -> indirectTemplate )
			int alt22=3;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt22=1;
				}
				break;
			case SUPER:
				{
				alt22=2;
				}
				break;
			case LPAREN:
				{
				alt22=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 22, 0, input);

					throw nvae;
				}
			}

			switch ( alt22 )
			{
			case 1:
				// Language\\Action.g3:297:6: id= ID argList
				{
				id=(IToken)Match(input,ID,Follow._ID_in_templateInclude946); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ID.Add(id);

				PushFollow(Follow._argList_in_templateInclude948);
				argList72=argList();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_argList.Add(argList72.Tree);


				{
				// AST REWRITE
				// elements: id, argList
				// token labels: id
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleITokenStream stream_id=new RewriteRuleITokenStream(adaptor,"token id",id);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 297:20: -> $id argList
				{
					adaptor.AddChild(root_0, stream_id.NextNode());
					adaptor.AddChild(root_0, stream_argList.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Language\\Action.g3:298:5: 'super' DOT qid= ID argList
				{
				string_literal73=(IToken)Match(input,SUPER,Follow._SUPER_in_templateInclude961); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_SUPER.Add(string_literal73);

				DOT74=(IToken)Match(input,DOT,Follow._DOT_in_templateInclude963); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_DOT.Add(DOT74);

				qid=(IToken)Match(input,ID,Follow._ID_in_templateInclude967); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ID.Add(qid);

				PushFollow(Follow._argList_in_templateInclude969);
				argList75=argList();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_argList.Add(argList75.Tree);


				{
				// AST REWRITE
				// elements: ID, argList
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 298:32: -> ID[$qid,\"super.\"+$qid.text] argList
				{
					adaptor.AddChild(root_0, (StringTemplateAST)adaptor.Create(ID, qid, "super."+(qid!=null?qid.Text:null)));
					adaptor.AddChild(root_0, stream_argList.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Language\\Action.g3:299:5: indirectTemplate
				{
				PushFollow(Follow._indirectTemplate_in_templateInclude982);
				indirectTemplate76=indirectTemplate();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_indirectTemplate.Add(indirectTemplate76.Tree);


				{
				// AST REWRITE
				// elements: indirectTemplate
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 299:22: -> indirectTemplate
				{
					adaptor.AddChild(root_0, stream_indirectTemplate.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}



			{
			// AST REWRITE
			// elements: templateInclude
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 301:3: -> ^( INCLUDE[\"include\"] $templateInclude)
			{
				// Language\\Action.g3:301:6: ^( INCLUDE[\"include\"] $templateInclude)
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(INCLUDE, "include"), root_1);

				adaptor.AddChild(root_1, stream_retval.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "templateInclude"

	public class indirectTemplate_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "indirectTemplate"
	// Language\\Action.g3:305:0: indirectTemplate : LPAREN e= templatesExpr RPAREN args= argList -> ^( VALUE[\"value\"] $e $args) ;
	private ActionParser.indirectTemplate_return indirectTemplate(  )
	{
		ActionParser.indirectTemplate_return retval = new ActionParser.indirectTemplate_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken LPAREN77=null;
		IToken RPAREN78=null;
		ActionParser.templatesExpr_return e = default(ActionParser.templatesExpr_return);
		ActionParser.argList_return args = default(ActionParser.argList_return);

		StringTemplateAST LPAREN77_tree=null;
		StringTemplateAST RPAREN78_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_templatesExpr=new RewriteRuleSubtreeStream(adaptor,"rule templatesExpr");
		RewriteRuleSubtreeStream stream_argList=new RewriteRuleSubtreeStream(adaptor,"rule argList");
		try
		{
			// Language\\Action.g3:306:4: ( LPAREN e= templatesExpr RPAREN args= argList -> ^( VALUE[\"value\"] $e $args) )
			// Language\\Action.g3:306:4: LPAREN e= templatesExpr RPAREN args= argList
			{
			LPAREN77=(IToken)Match(input,LPAREN,Follow._LPAREN_in_indirectTemplate1016); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN77);

			PushFollow(Follow._templatesExpr_in_indirectTemplate1020);
			e=templatesExpr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_templatesExpr.Add(e.Tree);
			RPAREN78=(IToken)Match(input,RPAREN,Follow._RPAREN_in_indirectTemplate1022); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN78);

			PushFollow(Follow._argList_in_indirectTemplate1026);
			args=argList();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_argList.Add(args.Tree);


			{
			// AST REWRITE
			// elements: e, args
			// token labels: 
			// rule labels: e, args, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_e=new RewriteRuleSubtreeStream(adaptor,"rule e",e!=null?e.tree:null);
			RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 307:3: -> ^( VALUE[\"value\"] $e $args)
			{
				// Language\\Action.g3:307:6: ^( VALUE[\"value\"] $e $args)
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(VALUE, "value"), root_1);

				adaptor.AddChild(root_1, stream_e.NextTree());
				adaptor.AddChild(root_1, stream_args.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "indirectTemplate"

	public class argList_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "argList"
	// Language\\Action.g3:310:0: argList : ( LPAREN ( argumentAssignment ( COMMA argumentAssignment )* )? RPAREN -> ^( ARGS[\"ARGS\"] ( argumentAssignment )* ) | singleArg );
	private ActionParser.argList_return argList(  )
	{
		ActionParser.argList_return retval = new ActionParser.argList_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken LPAREN79=null;
		IToken COMMA81=null;
		IToken RPAREN83=null;
		ActionParser.argumentAssignment_return argumentAssignment80 = default(ActionParser.argumentAssignment_return);
		ActionParser.argumentAssignment_return argumentAssignment82 = default(ActionParser.argumentAssignment_return);
		ActionParser.singleArg_return singleArg84 = default(ActionParser.singleArg_return);

		StringTemplateAST LPAREN79_tree=null;
		StringTemplateAST COMMA81_tree=null;
		StringTemplateAST RPAREN83_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_argumentAssignment=new RewriteRuleSubtreeStream(adaptor,"rule argumentAssignment");
		try
		{
			// Language\\Action.g3:311:4: ( LPAREN ( argumentAssignment ( COMMA argumentAssignment )* )? RPAREN -> ^( ARGS[\"ARGS\"] ( argumentAssignment )* ) | singleArg )
			int alt25=2;
			int LA25_0 = input.LA(1);

			if ( (LA25_0==LPAREN) )
			{
				switch ( input.LA(2) )
				{
				case ID:
					{
					int LA25_2 = input.LA(3);

					if ( (LA25_2==ASSIGN) )
					{
						alt25=1;
					}
					else if ( (LA25_2==COLON||LA25_2==DOT||LA25_2==LPAREN||LA25_2==PLUS||LA25_2==RPAREN) )
					{
						alt25=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 25, 2, input);

						throw nvae;
					}
					}
					break;
				case DOTDOTDOT:
				case RPAREN:
					{
					alt25=1;
					}
					break;
				case ANONYMOUS_TEMPLATE:
				case FIRST:
				case INT:
				case LAST:
				case LBRACK:
				case LENGTH:
				case LPAREN:
				case REST:
				case STRING:
				case STRIP:
				case SUPER:
				case TRUNC:
					{
					alt25=2;
					}
					break;
				default:
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 25, 1, input);

						throw nvae;
					}
				}

			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 25, 0, input);

				throw nvae;
			}
			switch ( alt25 )
			{
			case 1:
				// Language\\Action.g3:311:4: LPAREN ( argumentAssignment ( COMMA argumentAssignment )* )? RPAREN
				{
				LPAREN79=(IToken)Match(input,LPAREN,Follow._LPAREN_in_argList1053); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN79);

				// Language\\Action.g3:312:3: ( argumentAssignment ( COMMA argumentAssignment )* )?
				int alt24=2;
				int LA24_0 = input.LA(1);

				if ( (LA24_0==DOTDOTDOT||LA24_0==ID) )
				{
					alt24=1;
				}
				switch ( alt24 )
				{
				case 1:
					// Language\\Action.g3:312:5: argumentAssignment ( COMMA argumentAssignment )*
					{
					PushFollow(Follow._argumentAssignment_in_argList1059);
					argumentAssignment80=argumentAssignment();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_argumentAssignment.Add(argumentAssignment80.Tree);
					// Language\\Action.g3:312:24: ( COMMA argumentAssignment )*
					for ( ; ; )
					{
						int alt23=2;
						int LA23_0 = input.LA(1);

						if ( (LA23_0==COMMA) )
						{
							alt23=1;
						}


						switch ( alt23 )
						{
						case 1:
							// Language\\Action.g3:312:25: COMMA argumentAssignment
							{
							COMMA81=(IToken)Match(input,COMMA,Follow._COMMA_in_argList1062); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA81);

							PushFollow(Follow._argumentAssignment_in_argList1064);
							argumentAssignment82=argumentAssignment();

							state._fsp--;
							if (state.failed) return retval;
							if ( state.backtracking == 0 ) stream_argumentAssignment.Add(argumentAssignment82.Tree);

							}
							break;

						default:
							goto loop23;
						}
					}

					loop23:
						;



					}
					break;

				}

				RPAREN83=(IToken)Match(input,RPAREN,Follow._RPAREN_in_argList1075); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN83);



				{
				// AST REWRITE
				// elements: argumentAssignment
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (StringTemplateAST)adaptor.Nil();
				// 315:3: -> ^( ARGS[\"ARGS\"] ( argumentAssignment )* )
				{
					// Language\\Action.g3:315:6: ^( ARGS[\"ARGS\"] ( argumentAssignment )* )
					{
					StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
					root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(ARGS, "ARGS"), root_1);

					// Language\\Action.g3:315:21: ( argumentAssignment )*
					while ( stream_argumentAssignment.HasNext )
					{
						adaptor.AddChild(root_1, stream_argumentAssignment.NextTree());

					}
					stream_argumentAssignment.Reset();

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Language\\Action.g3:316:4: singleArg
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				PushFollow(Follow._singleArg_in_argList1092);
				singleArg84=singleArg();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, singleArg84.Tree);

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "argList"

	public class singleArg_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "singleArg"
	// Language\\Action.g3:319:0: singleArg : LPAREN nonAlternatingTemplateExpr RPAREN -> ^( SINGLEVALUEARG[\"SINGLEVALUEARG\"] nonAlternatingTemplateExpr ) ;
	private ActionParser.singleArg_return singleArg(  )
	{
		ActionParser.singleArg_return retval = new ActionParser.singleArg_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken LPAREN85=null;
		IToken RPAREN87=null;
		ActionParser.nonAlternatingTemplateExpr_return nonAlternatingTemplateExpr86 = default(ActionParser.nonAlternatingTemplateExpr_return);

		StringTemplateAST LPAREN85_tree=null;
		StringTemplateAST RPAREN87_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_nonAlternatingTemplateExpr=new RewriteRuleSubtreeStream(adaptor,"rule nonAlternatingTemplateExpr");
		try
		{
			// Language\\Action.g3:320:4: ( LPAREN nonAlternatingTemplateExpr RPAREN -> ^( SINGLEVALUEARG[\"SINGLEVALUEARG\"] nonAlternatingTemplateExpr ) )
			// Language\\Action.g3:320:4: LPAREN nonAlternatingTemplateExpr RPAREN
			{
			LPAREN85=(IToken)Match(input,LPAREN,Follow._LPAREN_in_singleArg1104); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN85);

			PushFollow(Follow._nonAlternatingTemplateExpr_in_singleArg1106);
			nonAlternatingTemplateExpr86=nonAlternatingTemplateExpr();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_nonAlternatingTemplateExpr.Add(nonAlternatingTemplateExpr86.Tree);
			RPAREN87=(IToken)Match(input,RPAREN,Follow._RPAREN_in_singleArg1108); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN87);



			{
			// AST REWRITE
			// elements: nonAlternatingTemplateExpr
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (StringTemplateAST)adaptor.Nil();
			// 321:3: -> ^( SINGLEVALUEARG[\"SINGLEVALUEARG\"] nonAlternatingTemplateExpr )
			{
				// Language\\Action.g3:321:6: ^( SINGLEVALUEARG[\"SINGLEVALUEARG\"] nonAlternatingTemplateExpr )
				{
				StringTemplateAST root_1 = (StringTemplateAST)adaptor.Nil();
				root_1 = (StringTemplateAST)adaptor.BecomeRoot((StringTemplateAST)adaptor.Create(SINGLEVALUEARG, "SINGLEVALUEARG"), root_1);

				adaptor.AddChild(root_1, stream_nonAlternatingTemplateExpr.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "singleArg"

	public class argumentAssignment_return : ParserRuleReturnScope
	{
		public StringTemplateAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "argumentAssignment"
	// Language\\Action.g3:324:0: argumentAssignment : ( ID ASSIGN nonAlternatingTemplateExpr | DOTDOTDOT );
	private ActionParser.argumentAssignment_return argumentAssignment(  )
	{
		ActionParser.argumentAssignment_return retval = new ActionParser.argumentAssignment_return();
		retval.start = input.LT(1);

		StringTemplateAST root_0 = null;

		IToken ID88=null;
		IToken ASSIGN89=null;
		IToken DOTDOTDOT91=null;
		ActionParser.nonAlternatingTemplateExpr_return nonAlternatingTemplateExpr90 = default(ActionParser.nonAlternatingTemplateExpr_return);

		StringTemplateAST ID88_tree=null;
		StringTemplateAST ASSIGN89_tree=null;
		StringTemplateAST DOTDOTDOT91_tree=null;

		try
		{
			// Language\\Action.g3:325:4: ( ID ASSIGN nonAlternatingTemplateExpr | DOTDOTDOT )
			int alt26=2;
			int LA26_0 = input.LA(1);

			if ( (LA26_0==ID) )
			{
				alt26=1;
			}
			else if ( (LA26_0==DOTDOTDOT) )
			{
				alt26=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 26, 0, input);

				throw nvae;
			}
			switch ( alt26 )
			{
			case 1:
				// Language\\Action.g3:325:4: ID ASSIGN nonAlternatingTemplateExpr
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				ID88=(IToken)Match(input,ID,Follow._ID_in_argumentAssignment1130); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ID88_tree = (StringTemplateAST)adaptor.Create(ID88);
				adaptor.AddChild(root_0, ID88_tree);
				}
				ASSIGN89=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_argumentAssignment1132); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				ASSIGN89_tree = (StringTemplateAST)adaptor.Create(ASSIGN89);
				root_0 = (StringTemplateAST)adaptor.BecomeRoot(ASSIGN89_tree, root_0);
				}
				PushFollow(Follow._nonAlternatingTemplateExpr_in_argumentAssignment1135);
				nonAlternatingTemplateExpr90=nonAlternatingTemplateExpr();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, nonAlternatingTemplateExpr90.Tree);

				}
				break;
			case 2:
				// Language\\Action.g3:326:4: DOTDOTDOT
				{
				root_0 = (StringTemplateAST)adaptor.Nil();

				DOTDOTDOT91=(IToken)Match(input,DOTDOTDOT,Follow._DOTDOTDOT_in_argumentAssignment1140); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				DOTDOTDOT91_tree = (StringTemplateAST)adaptor.Create(DOTDOTDOT91);
				adaptor.AddChild(root_0, DOTDOTDOT91_tree);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (StringTemplateAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (StringTemplateAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "argumentAssignment"

	// $ANTLR start synpred1_Action
	public void synpred1_Action_fragment()
	{
		// Language\\Action.g3:200:4: ( templateInclude )
		// Language\\Action.g3:200:5: templateInclude
		{
		PushFollow(Follow._templateInclude_in_synpred1_Action490);
		templateInclude();

		state._fsp--;
		if (state.failed) return ;

		}
	}
	// $ANTLR end synpred1_Action
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
	DFA15 dfa15;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa15 = new DFA15( this, new SpecialStateTransitionHandler( specialStateTransition15 ) );
	}

	class DFA15 : DFA
	{

		const string DFA15_eotS =
			"\xD\xFFFF";
		const string DFA15_eofS =
			"\xD\xFFFF";
		const string DFA15_minS =
			"\x1\x4\x1\x0\x1\xFFFF\x1\x0\x9\xFFFF";
		const string DFA15_maxS =
			"\x1\x29\x1\x0\x1\xFFFF\x1\x0\x9\xFFFF";
		const string DFA15_acceptS =
			"\x2\xFFFF\x1\x1\x1\xFFFF\x1\x2\x1\x3\x5\xFFFF\x1\x5\x1\x4";
		const string DFA15_specialS =
			"\x1\x0\x1\x1\x1\xFFFF\x1\x2\x9\xFFFF}>";
		static readonly string[] DFA15_transitionS =
			{
				"\x1\x4\xA\xFFFF\x1\x5\x1\xFFFF\x1\x1\x1\xFFFF\x1\x4\x1\x5\x1\xB\x1\x5"+
				"\x1\xFFFF\x1\x3\x7\xFFFF\x1\x5\x3\xFFFF\x1\x4\x1\x5\x1\x2\x2\xFFFF\x1"+
				"\x5",
				"\x1\xFFFF",
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
				""
			};

		static readonly short[] DFA15_eot = DFA.UnpackEncodedString(DFA15_eotS);
		static readonly short[] DFA15_eof = DFA.UnpackEncodedString(DFA15_eofS);
		static readonly char[] DFA15_min = DFA.UnpackEncodedStringToUnsignedChars(DFA15_minS);
		static readonly char[] DFA15_max = DFA.UnpackEncodedStringToUnsignedChars(DFA15_maxS);
		static readonly short[] DFA15_accept = DFA.UnpackEncodedString(DFA15_acceptS);
		static readonly short[] DFA15_special = DFA.UnpackEncodedString(DFA15_specialS);
		static readonly short[][] DFA15_transition;

		static DFA15()
		{
			int numStates = DFA15_transitionS.Length;
			DFA15_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA15_transition[i] = DFA.UnpackEncodedString(DFA15_transitionS[i]);
			}
		}

		public DFA15( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 15;
			this.eot = DFA15_eot;
			this.eof = DFA15_eof;
			this.min = DFA15_min;
			this.max = DFA15_max;
			this.accept = DFA15_accept;
			this.special = DFA15_special;
			this.transition = DFA15_transition;
		}
		public override string GetDescription()
		{
			return "199:0: primaryExpr : (=> templateInclude | atom ( DOT ( ID | valueExpr ) )* | function ( DOT ( ID | valueExpr ) )* | valueExpr | list );";
		}
	}

	int specialStateTransition15( DFA dfa, int s, IIntStream _input )
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA15_0 = input.LA(1);


				int index15_0 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA15_0==ID) ) {s = 1;}

				else if ( (LA15_0==SUPER) && (EvaluatePredicate(synpred1_Action_fragment))) {s = 2;}

				else if ( (LA15_0==LPAREN) ) {s = 3;}

				else if ( (LA15_0==ANONYMOUS_TEMPLATE||LA15_0==INT||LA15_0==STRING) ) {s = 4;}

				else if ( (LA15_0==FIRST||LA15_0==LAST||LA15_0==LENGTH||LA15_0==REST||LA15_0==STRIP||LA15_0==TRUNC) ) {s = 5;}

				else if ( (LA15_0==LBRACK) ) {s = 11;}


				input.Seek(index15_0);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA15_1 = input.LA(1);


				int index15_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_Action_fragment)) ) {s = 2;}

				else if ( (true) ) {s = 4;}


				input.Seek(index15_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA15_3 = input.LA(1);


				int index15_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_Action_fragment)) ) {s = 2;}

				else if ( (true) ) {s = 12;}


				input.Seek(index15_3);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 15, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}

	#endregion DFA

	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _templatesExpr_in_action192 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _SEMI_in_action195 = new BitSet(new ulong[]{0x20000UL});
		public static readonly BitSet _optionList_in_action198 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _CONDITIONAL_in_action208 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _LPAREN_in_action211 = new BitSet(new ulong[]{0x271117A8010UL});
		public static readonly BitSet _ifCondition_in_action214 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _RPAREN_in_action216 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _ELSEIF_in_action223 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _LPAREN_in_action226 = new BitSet(new ulong[]{0x271117A8010UL});
		public static readonly BitSet _ifCondition_in_action229 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _RPAREN_in_action231 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _EOF_in_action241 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _option_in_optionList257 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _COMMA_in_optionList261 = new BitSet(new ulong[]{0x20000UL});
		public static readonly BitSet _option_in_optionList263 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _ID_in_option283 = new BitSet(new ulong[]{0x82UL});
		public static readonly BitSet _ASSIGN_in_option289 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _nonAlternatingTemplateExpr_in_option291 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expr_in_templatesExpr321 = new BitSet(new ulong[]{0x302UL});
		public static readonly BitSet _COMMA_in_templatesExpr328 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _expr_in_templatesExpr330 = new BitSet(new ulong[]{0x300UL});
		public static readonly BitSet _COLON_in_templatesExpr336 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _anonymousTemplate_in_templatesExpr338 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _COLON_in_templatesExpr380 = new BitSet(new ulong[]{0x4001020010UL});
		public static readonly BitSet _templateList_in_templatesExpr382 = new BitSet(new ulong[]{0x102UL});
		public static readonly BitSet _template_in_templateList421 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _COMMA_in_templateList424 = new BitSet(new ulong[]{0x4001020010UL});
		public static readonly BitSet _template_in_templateList427 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _ifAtom_in_ifCondition440 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_ifCondition445 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _ifAtom_in_ifCondition448 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templatesExpr_in_ifAtom459 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _primaryExpr_in_expr470 = new BitSet(new ulong[]{0x40000002UL});
		public static readonly BitSet _PLUS_in_expr473 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _primaryExpr_in_expr476 = new BitSet(new ulong[]{0x40000002UL});
		public static readonly BitSet _templateInclude_in_primaryExpr493 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _atom_in_primaryExpr500 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _DOT_in_primaryExpr506 = new BitSet(new ulong[]{0x1020000UL});
		public static readonly BitSet _ID_in_primaryExpr515 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _valueExpr_in_primaryExpr522 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _function_in_primaryExpr537 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _DOT_in_primaryExpr543 = new BitSet(new ulong[]{0x1020000UL});
		public static readonly BitSet _ID_in_primaryExpr551 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _valueExpr_in_primaryExpr558 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _valueExpr_in_primaryExpr573 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _list_in_primaryExpr578 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_valueExpr589 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _templatesExpr_in_valueExpr591 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _RPAREN_in_valueExpr593 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expr_in_nonAlternatingTemplateExpr616 = new BitSet(new ulong[]{0x102UL});
		public static readonly BitSet _COLON_in_nonAlternatingTemplateExpr625 = new BitSet(new ulong[]{0x4001020010UL});
		public static readonly BitSet _template_in_nonAlternatingTemplateExpr627 = new BitSet(new ulong[]{0x102UL});
		public static readonly BitSet _FIRST_in_function655 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _REST_in_function661 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _LAST_in_function667 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _LENGTH_in_function673 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _STRIP_in_function679 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _TRUNC_in_function685 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _singleArg_in_function693 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _namedTemplate_in_template734 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _anonymousTemplate_in_template743 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_namedTemplate774 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _argList_in_namedTemplate776 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SUPER_in_namedTemplate789 = new BitSet(new ulong[]{0x800UL});
		public static readonly BitSet _DOT_in_namedTemplate791 = new BitSet(new ulong[]{0x20000UL});
		public static readonly BitSet _ID_in_namedTemplate795 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _argList_in_namedTemplate797 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _indirectTemplate_in_namedTemplate811 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_anonymousTemplate840 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_atom855 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LBRACK_in_list883 = new BitSet(new ulong[]{0x271817A8210UL});
		public static readonly BitSet _listElement_in_list887 = new BitSet(new ulong[]{0x80000200UL});
		public static readonly BitSet _COMMA_in_list890 = new BitSet(new ulong[]{0x271817A8210UL});
		public static readonly BitSet _listElement_in_list892 = new BitSet(new ulong[]{0x80000200UL});
		public static readonly BitSet _RBRACK_in_list898 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _nonAlternatingTemplateExpr_in_listElement921 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_templateInclude946 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _argList_in_templateInclude948 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SUPER_in_templateInclude961 = new BitSet(new ulong[]{0x800UL});
		public static readonly BitSet _DOT_in_templateInclude963 = new BitSet(new ulong[]{0x20000UL});
		public static readonly BitSet _ID_in_templateInclude967 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _argList_in_templateInclude969 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _indirectTemplate_in_templateInclude982 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_indirectTemplate1016 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _templatesExpr_in_indirectTemplate1020 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _RPAREN_in_indirectTemplate1022 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _argList_in_indirectTemplate1026 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_argList1053 = new BitSet(new ulong[]{0x200021000UL});
		public static readonly BitSet _argumentAssignment_in_argList1059 = new BitSet(new ulong[]{0x200000200UL});
		public static readonly BitSet _COMMA_in_argList1062 = new BitSet(new ulong[]{0x21000UL});
		public static readonly BitSet _argumentAssignment_in_argList1064 = new BitSet(new ulong[]{0x200000200UL});
		public static readonly BitSet _RPAREN_in_argList1075 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _singleArg_in_argList1092 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_singleArg1104 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _nonAlternatingTemplateExpr_in_singleArg1106 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _RPAREN_in_singleArg1108 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_argumentAssignment1130 = new BitSet(new ulong[]{0x80UL});
		public static readonly BitSet _ASSIGN_in_argumentAssignment1132 = new BitSet(new ulong[]{0x271017A8010UL});
		public static readonly BitSet _nonAlternatingTemplateExpr_in_argumentAssignment1135 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOTDOTDOT_in_argumentAssignment1140 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templateInclude_in_synpred1_Action490 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace  Antlr3.ST.Language 
