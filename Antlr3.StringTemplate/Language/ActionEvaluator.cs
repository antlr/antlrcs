// $ANTLR 3.1.2 Language\\ActionEvaluator.g3 2009-04-11 17:05:21

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

using System.Linq;
using Antlr.Runtime.JavaExtensions;

using Map = System.Collections.IDictionary;
using Set = System.Collections.Generic.HashSet<object>;
using StringWriter = System.IO.StringWriter;
using Vector = System.Collections.Generic.List<object>;


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Antlr3.ST.Language
{
public partial class ActionEvaluator : TreeParser
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

	public ActionEvaluator( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ActionEvaluator( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return ActionEvaluator.tokenNames; }
	public override string GrammarFileName { get { return "Language\\ActionEvaluator.g3"; } }


	#region Rules

	// $ANTLR start "action"
	// Language\\ActionEvaluator.g3:87:0: public action returns [int numCharsWritten=0] : expr ;
	public int action(  )
	{

		int numCharsWritten = 0;

		object expr1 = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:88:4: ( expr )
			// Language\\ActionEvaluator.g3:88:4: expr
			{
			PushFollow(Follow._expr_in_action56);
			expr1=expr();

			state._fsp--;

			numCharsWritten = chunk.WriteAttribute(self,expr1,writer);

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
		return numCharsWritten;
	}
	// $ANTLR end "action"


	// $ANTLR start "expr"
	// Language\\ActionEvaluator.g3:91:0: expr returns [object value] : ( ^( PLUS a= expr b= expr ) | templateApplication | attribute | templateInclude | function | list | ^( VALUE e= expr ) );
	private object expr(  )
	{

		object value = default(object);

		object a = default(object);
		object b = default(object);
		object e = default(object);
		object templateApplication2 = default(object);
		object attribute3 = default(object);
		object templateInclude4 = default(object);
		object function5 = default(object);
		object list6 = default(object);


			Map argumentContext=null;
			value = null;

		try
		{
			// Language\\ActionEvaluator.g3:97:4: ( ^( PLUS a= expr b= expr ) | templateApplication | attribute | templateInclude | function | list | ^( VALUE e= expr ) )
			int alt1=7;
			switch ( input.LA(1) )
			{
			case PLUS:
				{
				alt1=1;
				}
				break;
			case APPLY:
			case MULTI_APPLY:
				{
				alt1=2;
				}
				break;
			case ANONYMOUS_TEMPLATE:
			case DOT:
			case ID:
			case INT:
			case STRING:
				{
				alt1=3;
				}
				break;
			case INCLUDE:
				{
				alt1=4;
				}
				break;
			case FUNCTION:
				{
				alt1=5;
				}
				break;
			case LIST:
				{
				alt1=6;
				}
				break;
			case VALUE:
				{
				alt1=7;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

					throw nvae;
				}
			}

			switch ( alt1 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:97:4: ^( PLUS a= expr b= expr )
				{
				Match(input,PLUS,Follow._PLUS_in_expr79); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_expr83);
				a=expr();

				state._fsp--;

				PushFollow(Follow._expr_in_expr87);
				b=expr();

				state._fsp--;

				value = chunk.Add(a,b);

				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:98:4: templateApplication
				{
				PushFollow(Follow._templateApplication_in_expr95);
				templateApplication2=templateApplication();

				state._fsp--;

				value = templateApplication2;

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:99:4: attribute
				{
				PushFollow(Follow._attribute_in_expr102);
				attribute3=attribute();

				state._fsp--;

				value = attribute3;

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:100:4: templateInclude
				{
				PushFollow(Follow._templateInclude_in_expr109);
				templateInclude4=templateInclude();

				state._fsp--;

				value = templateInclude4;

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:101:4: function
				{
				PushFollow(Follow._function_in_expr116);
				function5=function();

				state._fsp--;

				value = function5;

				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:102:4: list
				{
				PushFollow(Follow._list_in_expr123);
				list6=list();

				state._fsp--;

				value = list6;

				}
				break;
			case 7:
				// Language\\ActionEvaluator.g3:103:4: ^( VALUE e= expr )
				{
				Match(input,VALUE,Follow._VALUE_in_expr131); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_expr135);
				e=expr();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

							StringWriter buf = new StringWriter();
							IStringTemplateWriter sw = self.Group.GetStringTemplateWriter(buf);
							int n = chunk.WriteAttribute(self,e,sw);
							if ( n > 0 )
							{
								value = buf.ToString();
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
		return value;
	}
	// $ANTLR end "expr"


	// $ANTLR start "list"
	// Language\\ActionEvaluator.g3:117:0: list returns [object value=null] : ^( LIST ( expr | NOTHING )+ ) ;
	private object list(  )
	{

		object value = null;

		object expr7 = default(object);


			List elements = new ArrayList();

		try
		{
			// Language\\ActionEvaluator.g3:122:4: ( ^( LIST ( expr | NOTHING )+ ) )
			// Language\\ActionEvaluator.g3:122:4: ^( LIST ( expr | NOTHING )+ )
			{
			Match(input,LIST,Follow._LIST_in_list167); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:123:4: ( expr | NOTHING )+
			int cnt2=0;
			for ( ; ; )
			{
				int alt2=3;
				int LA2_0 = input.LA(1);

				if ( ((LA2_0>=ANONYMOUS_TEMPLATE && LA2_0<=APPLY)||LA2_0==DOT||(LA2_0>=FUNCTION && LA2_0<=INT)||LA2_0==LIST||LA2_0==MULTI_APPLY||LA2_0==PLUS||LA2_0==STRING||LA2_0==VALUE) )
				{
					alt2=1;
				}
				else if ( (LA2_0==NOTHING) )
				{
					alt2=2;
				}


				switch ( alt2 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:123:6: expr
					{
					PushFollow(Follow._expr_in_list174);
					expr7=expr();

					state._fsp--;


										if ( expr7!=null )
										{
											elements.Add(expr7);
										}
									

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:130:6: NOTHING
					{
					Match(input,NOTHING,Follow._NOTHING_in_list187); 

										List nullSingleton = new ArrayList( new object[] {null} );
										elements.Add(nullSingleton.iterator()); // add a blank
									

					}
					break;

				default:
					if ( cnt2 >= 1 )
						goto loop2;

					EarlyExitException eee2 = new EarlyExitException( 2, input );
					throw eee2;
				}
				cnt2++;
			}
			loop2:
				;



			Match(input, TokenConstants.Up, null); 
			value = new Cat(elements);

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
		return value;
	}
	// $ANTLR end "list"


	// $ANTLR start "templateInclude"
	// Language\\ActionEvaluator.g3:140:0: templateInclude returns [object value=null] : ^( INCLUDE (id= ID a1= . | ^( VALUE n= expr a2= . ) ) ) ;
	private object templateInclude(  )
	{

		object value = null;

		StringTemplateAST id=null;
		StringTemplateAST a1=null;
		StringTemplateAST a2=null;
		object n = default(object);


			StringTemplateAST args = null;
			string name = null;

		try
		{
			// Language\\ActionEvaluator.g3:146:4: ( ^( INCLUDE (id= ID a1= . | ^( VALUE n= expr a2= . ) ) ) )
			// Language\\ActionEvaluator.g3:146:4: ^( INCLUDE (id= ID a1= . | ^( VALUE n= expr a2= . ) ) )
			{
			Match(input,INCLUDE,Follow._INCLUDE_in_templateInclude229); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:148:4: (id= ID a1= . | ^( VALUE n= expr a2= . ) )
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==ID) )
			{
				alt3=1;
			}
			else if ( (LA3_0==VALUE) )
			{
				alt3=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

				throw nvae;
			}
			switch ( alt3 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:148:6: id= ID a1= .
				{
				id=(StringTemplateAST)Match(input,ID,Follow._ID_in_templateInclude242); 
				a1=(StringTemplateAST)input.LT(1);
				MatchAny(input); 
				name=(id!=null?id.Text:null); args=a1;

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:151:6: ^( VALUE n= expr a2= . )
				{
				Match(input,VALUE,Follow._VALUE_in_templateInclude262); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_templateInclude266);
				n=expr();

				state._fsp--;

				a2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				Match(input, TokenConstants.Up, null); 
				if (n!=null) {name=n.ToString();} args=a2;

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

						if ( name!=null )
						{
							value = chunk.GetTemplateInclude(self, name, args);
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
		return value;
	}
	// $ANTLR end "templateInclude"


	// $ANTLR start "templateApplication"
	// Language\\ActionEvaluator.g3:167:0: templateApplication returns [object value] : ( ^( APPLY a= expr ( template[templatesToApply] )+ ) | ^( MULTI_APPLY (a= expr )+ COLON anon= ANONYMOUS_TEMPLATE ) );
	private object templateApplication(  )
	{

		object value = default(object);

		StringTemplateAST anon=null;
		object a = default(object);


			var templatesToApply = new System.Collections.Generic.List<StringTemplate>();
			List attributes = new ArrayList();

		try
		{
			// Language\\ActionEvaluator.g3:173:4: ( ^( APPLY a= expr ( template[templatesToApply] )+ ) | ^( MULTI_APPLY (a= expr )+ COLON anon= ANONYMOUS_TEMPLATE ) )
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==APPLY) )
			{
				alt6=1;
			}
			else if ( (LA6_0==MULTI_APPLY) )
			{
				alt6=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 6, 0, input);

				throw nvae;
			}
			switch ( alt6 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:173:4: ^( APPLY a= expr ( template[templatesToApply] )+ )
				{
				Match(input,APPLY,Follow._APPLY_in_templateApplication316); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_templateApplication320);
				a=expr();

				state._fsp--;

				// Language\\ActionEvaluator.g3:174:4: ( template[templatesToApply] )+
				int cnt4=0;
				for ( ; ; )
				{
					int alt4=2;
					int LA4_0 = input.LA(1);

					if ( (LA4_0==TEMPLATE) )
					{
						alt4=1;
					}


					switch ( alt4 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:174:5: template[templatesToApply]
						{
						PushFollow(Follow._template_in_templateApplication326);
						template(templatesToApply);

						state._fsp--;


						}
						break;

					default:
						if ( cnt4 >= 1 )
							goto loop4;

						EarlyExitException eee4 = new EarlyExitException( 4, input );
						throw eee4;
					}
					cnt4++;
				}
				loop4:
					;


				value = chunk.ApplyListOfAlternatingTemplates(self,a,templatesToApply);

				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:177:4: ^( MULTI_APPLY (a= expr )+ COLON anon= ANONYMOUS_TEMPLATE )
				{
				Match(input,MULTI_APPLY,Follow._MULTI_APPLY_in_templateApplication345); 

				Match(input, TokenConstants.Down, null); 
				// Language\\ActionEvaluator.g3:177:19: (a= expr )+
				int cnt5=0;
				for ( ; ; )
				{
					int alt5=2;
					int LA5_0 = input.LA(1);

					if ( ((LA5_0>=ANONYMOUS_TEMPLATE && LA5_0<=APPLY)||LA5_0==DOT||(LA5_0>=FUNCTION && LA5_0<=INT)||LA5_0==LIST||LA5_0==MULTI_APPLY||LA5_0==PLUS||LA5_0==STRING||LA5_0==VALUE) )
					{
						alt5=1;
					}


					switch ( alt5 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:177:20: a= expr
						{
						PushFollow(Follow._expr_in_templateApplication350);
						a=expr();

						state._fsp--;

						attributes.Add(a);

						}
						break;

					default:
						if ( cnt5 >= 1 )
							goto loop5;

						EarlyExitException eee5 = new EarlyExitException( 5, input );
						throw eee5;
					}
					cnt5++;
				}
				loop5:
					;


				Match(input,COLON,Follow._COLON_in_templateApplication357); 
				anon=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_templateApplication364); 

								StringTemplate anonymous = anon.StringTemplate;
								templatesToApply.Add(anonymous);
								value = chunk.ApplyTemplateToListOfAttributes(self,
																			  attributes,
																			  anon.StringTemplate);
							

				Match(input, TokenConstants.Up, null); 

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
		return value;
	}
	// $ANTLR end "templateApplication"


	// $ANTLR start "function"
	// Language\\ActionEvaluator.g3:189:0: function returns [object value] : ^( FUNCTION ( 'first' a= singleFunctionArg | 'rest' a= singleFunctionArg | 'last' a= singleFunctionArg | 'length' a= singleFunctionArg | 'strip' a= singleFunctionArg | 'trunc' a= singleFunctionArg ) ) ;
	private object function(  )
	{

		object value = default(object);

		object a = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:190:4: ( ^( FUNCTION ( 'first' a= singleFunctionArg | 'rest' a= singleFunctionArg | 'last' a= singleFunctionArg | 'length' a= singleFunctionArg | 'strip' a= singleFunctionArg | 'trunc' a= singleFunctionArg ) ) )
			// Language\\ActionEvaluator.g3:190:4: ^( FUNCTION ( 'first' a= singleFunctionArg | 'rest' a= singleFunctionArg | 'last' a= singleFunctionArg | 'length' a= singleFunctionArg | 'strip' a= singleFunctionArg | 'trunc' a= singleFunctionArg ) )
			{
			Match(input,FUNCTION,Follow._FUNCTION_in_function390); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:191:4: ( 'first' a= singleFunctionArg | 'rest' a= singleFunctionArg | 'last' a= singleFunctionArg | 'length' a= singleFunctionArg | 'strip' a= singleFunctionArg | 'trunc' a= singleFunctionArg )
			int alt7=6;
			switch ( input.LA(1) )
			{
			case FIRST:
				{
				alt7=1;
				}
				break;
			case REST:
				{
				alt7=2;
				}
				break;
			case LAST:
				{
				alt7=3;
				}
				break;
			case LENGTH:
				{
				alt7=4;
				}
				break;
			case STRIP:
				{
				alt7=5;
				}
				break;
			case TRUNC:
				{
				alt7=6;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

					throw nvae;
				}
			}

			switch ( alt7 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:191:6: 'first' a= singleFunctionArg
				{
				Match(input,FIRST,Follow._FIRST_in_function397); 
				PushFollow(Follow._singleFunctionArg_in_function402);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.First(a);

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:192:6: 'rest' a= singleFunctionArg
				{
				Match(input,REST,Follow._REST_in_function411); 
				PushFollow(Follow._singleFunctionArg_in_function416);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.Rest(a);

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:193:6: 'last' a= singleFunctionArg
				{
				Match(input,LAST,Follow._LAST_in_function425); 
				PushFollow(Follow._singleFunctionArg_in_function430);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.Last(a);

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:194:6: 'length' a= singleFunctionArg
				{
				Match(input,LENGTH,Follow._LENGTH_in_function439); 
				PushFollow(Follow._singleFunctionArg_in_function443);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.Length(a);

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:195:6: 'strip' a= singleFunctionArg
				{
				Match(input,STRIP,Follow._STRIP_in_function452); 
				PushFollow(Follow._singleFunctionArg_in_function457);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.Strip(a);

				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:196:6: 'trunc' a= singleFunctionArg
				{
				Match(input,TRUNC,Follow._TRUNC_in_function466); 
				PushFollow(Follow._singleFunctionArg_in_function471);
				a=singleFunctionArg();

				state._fsp--;

				value =chunk.Trunc(a);

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
		return value;
	}
	// $ANTLR end "function"


	// $ANTLR start "singleFunctionArg"
	// Language\\ActionEvaluator.g3:201:0: singleFunctionArg returns [object value=null] : ^( SINGLEVALUEARG expr ) ;
	private object singleFunctionArg(  )
	{

		object value = null;

		object expr8 = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:202:4: ( ^( SINGLEVALUEARG expr ) )
			// Language\\ActionEvaluator.g3:202:4: ^( SINGLEVALUEARG expr )
			{
			Match(input,SINGLEVALUEARG,Follow._SINGLEVALUEARG_in_singleFunctionArg499); 

			Match(input, TokenConstants.Down, null); 
			PushFollow(Follow._expr_in_singleFunctionArg501);
			expr8=expr();

			state._fsp--;

			value = expr8;

			Match(input, TokenConstants.Up, null); 

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
		return value;
	}
	// $ANTLR end "singleFunctionArg"


	// $ANTLR start "template"
	// Language\\ActionEvaluator.g3:205:0: template[System.Collections.Generic.List<StringTemplate> templatesToApply] : ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) ) ;
	private void template( System.Collections.Generic.List<StringTemplate> templatesToApply )
	{
		StringTemplateAST anon=null;
		StringTemplateAST ID9=null;
		StringTemplateAST args=null;
		StringTemplateAST args2=null;
		object n = default(object);


			Map argumentContext = null;

		try
		{
			// Language\\ActionEvaluator.g3:210:4: ( ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) ) )
			// Language\\ActionEvaluator.g3:210:4: ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) )
			{
			Match(input,TEMPLATE,Follow._TEMPLATE_in_template524); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:211:4: ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) )
			int alt8=3;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt8=1;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt8=2;
				}
				break;
			case VALUE:
				{
				alt8=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 8, 0, input);

					throw nvae;
				}
			}

			switch ( alt8 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:211:6: ID args= .
				{
				ID9=(StringTemplateAST)Match(input,ID,Follow._ID_in_template531); 
				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

									string templateName = (ID9!=null?ID9.Text:null);
									StringTemplateGroup group = self.Group;
									StringTemplate embedded = group.GetEmbeddedInstanceOf(self, templateName);
									if ( embedded!=null )
									{
										embedded.ArgumentsAST = args;
										templatesToApply.Add(embedded);
									}
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:223:6: anon= ANONYMOUS_TEMPLATE
				{
				anon=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_template552); 

									StringTemplate anonymous = anon.StringTemplate;
									// to properly see overridden templates, always set
									// anonymous' group to be self's group
									anonymous.Group = self.Group;
									templatesToApply.Add(anonymous);
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:232:6: ^( VALUE n= expr args2= . )
				{
				Match(input,VALUE,Follow._VALUE_in_template568); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_template572);
				n=expr();

				state._fsp--;

				args2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

										StringTemplate embedded = null;
										if ( n!=null )
										{
											string templateName = n.ToString();
											StringTemplateGroup group = self.Group;
											embedded = group.GetEmbeddedInstanceOf(self, templateName);
											if ( embedded!=null )
											{
												embedded.ArgumentsAST = args2;
												templatesToApply.Add(embedded);
											}
										}
									

				Match(input, TokenConstants.Up, null); 

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "template"


	// $ANTLR start "ifCondition"
	// Language\\ActionEvaluator.g3:253:0: public ifCondition returns [bool value] : (a= ifAtom | ^( NOT a= ifAtom ) );
	public bool ifCondition(  )
	{

		bool value = default(bool);

		object a = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:254:4: (a= ifAtom | ^( NOT a= ifAtom ) )
			int alt9=2;
			int LA9_0 = input.LA(1);

			if ( ((LA9_0>=ANONYMOUS_TEMPLATE && LA9_0<=APPLY)||LA9_0==DOT||(LA9_0>=FUNCTION && LA9_0<=INT)||LA9_0==LIST||LA9_0==MULTI_APPLY||LA9_0==PLUS||LA9_0==STRING||LA9_0==VALUE) )
			{
				alt9=1;
			}
			else if ( (LA9_0==NOT) )
			{
				alt9=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 9, 0, input);

				throw nvae;
			}
			switch ( alt9 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:254:4: a= ifAtom
				{
				PushFollow(Follow._ifAtom_in_ifCondition617);
				a=ifAtom();

				state._fsp--;

				value = chunk.TestAttributeTrue(a);

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:255:4: ^( NOT a= ifAtom )
				{
				Match(input,NOT,Follow._NOT_in_ifCondition625); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._ifAtom_in_ifCondition629);
				a=ifAtom();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 
				value = !chunk.TestAttributeTrue(a);

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
		return value;
	}
	// $ANTLR end "ifCondition"


	// $ANTLR start "ifAtom"
	// Language\\ActionEvaluator.g3:258:0: ifAtom returns [object value] : expr ;
	private object ifAtom(  )
	{

		object value = default(object);

		object expr10 = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:259:4: ( expr )
			// Language\\ActionEvaluator.g3:259:4: expr
			{
			PushFollow(Follow._expr_in_ifAtom647);
			expr10=expr();

			state._fsp--;

			value = expr10;

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
		return value;
	}
	// $ANTLR end "ifAtom"


	// $ANTLR start "attribute"
	// Language\\ActionEvaluator.g3:262:0: attribute returns [object value] : ( ^( DOT obj= expr (prop= ID | ^( VALUE e= expr ) ) ) |i3= ID |i= INT |s= STRING |at= ANONYMOUS_TEMPLATE );
	private object attribute(  )
	{

		object value = default(object);

		StringTemplateAST prop=null;
		StringTemplateAST i3=null;
		StringTemplateAST i=null;
		StringTemplateAST s=null;
		StringTemplateAST at=null;
		object obj = default(object);
		object e = default(object);


			object propName = null;

		try
		{
			// Language\\ActionEvaluator.g3:267:4: ( ^( DOT obj= expr (prop= ID | ^( VALUE e= expr ) ) ) |i3= ID |i= INT |s= STRING |at= ANONYMOUS_TEMPLATE )
			int alt11=5;
			switch ( input.LA(1) )
			{
			case DOT:
				{
				alt11=1;
				}
				break;
			case ID:
				{
				alt11=2;
				}
				break;
			case INT:
				{
				alt11=3;
				}
				break;
			case STRING:
				{
				alt11=4;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt11=5;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 11, 0, input);

					throw nvae;
				}
			}

			switch ( alt11 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:267:4: ^( DOT obj= expr (prop= ID | ^( VALUE e= expr ) ) )
				{
				Match(input,DOT,Follow._DOT_in_attribute671); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_attribute675);
				obj=expr();

				state._fsp--;

				// Language\\ActionEvaluator.g3:268:4: (prop= ID | ^( VALUE e= expr ) )
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( (LA10_0==ID) )
				{
					alt10=1;
				}
				else if ( (LA10_0==VALUE) )
				{
					alt10=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 10, 0, input);

					throw nvae;
				}
				switch ( alt10 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:268:6: prop= ID
					{
					prop=(StringTemplateAST)Match(input,ID,Follow._ID_in_attribute684); 
					propName = (prop!=null?prop.Text:null);

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:273:6: ^( VALUE e= expr )
					{
					Match(input,VALUE,Follow._VALUE_in_attribute714); 

					Match(input, TokenConstants.Down, null); 
					PushFollow(Follow._expr_in_attribute718);
					e=expr();

					state._fsp--;


					Match(input, TokenConstants.Up, null); 
					if (e!=null) {propName=e;}

					}
					break;

				}


				Match(input, TokenConstants.Up, null); 
				value = chunk.GetObjectProperty(self,obj,propName);

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:278:4: i3= ID
				{
				i3=(StringTemplateAST)Match(input,ID,Follow._ID_in_attribute742); 

						value =self.GetAttribute((i3!=null?i3.Text:null));
						

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:283:4: i= INT
				{
				i=(StringTemplateAST)Match(input,INT,Follow._INT_in_attribute754); 
				value =int.Parse((i!=null?i.Text:null));

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:285:4: s= STRING
				{
				s=(StringTemplateAST)Match(input,STRING,Follow._STRING_in_attribute764); 

						value =(s!=null?s.Text:null);
						

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:290:4: at= ANONYMOUS_TEMPLATE
				{
				at=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_attribute776); 

							value =(at!=null?at.Text:null);
							if ( (at!=null?at.Text:null)!=null )
							{
								StringTemplate valueST =new StringTemplate(self.Group, (at!=null?at.Text:null));
								valueST.EnclosingInstance = self;
								valueST.Name = "<anonymous template argument>";
								value = valueST;
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
		return value;
	}
	// $ANTLR end "attribute"


	// $ANTLR start "argList"
	// Language\\ActionEvaluator.g3:309:0: public argList[StringTemplate embedded, System.Collections.Generic.Dictionary<string, object> initialContext] returns [System.Collections.Generic.Dictionary<string, object> argumentContext=null] : ( ^( ARGS ( argumentAssignment[$embedded,$argumentContext] )* ) | singleTemplateArg[$embedded,$argumentContext] );
	public System.Collections.Generic.Dictionary<string, object> argList( StringTemplate embedded, System.Collections.Generic.Dictionary<string, object> initialContext )
	{

		System.Collections.Generic.Dictionary<string, object> argumentContext = null;


			argumentContext = initialContext;
			if ( argumentContext==null )
			{
				argumentContext =new System.Collections.Generic.Dictionary<string, object>();
			}

		try
		{
			// Language\\ActionEvaluator.g3:319:4: ( ^( ARGS ( argumentAssignment[$embedded,$argumentContext] )* ) | singleTemplateArg[$embedded,$argumentContext] )
			int alt13=2;
			int LA13_0 = input.LA(1);

			if ( (LA13_0==ARGS) )
			{
				alt13=1;
			}
			else if ( (LA13_0==SINGLEVALUEARG) )
			{
				alt13=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

				throw nvae;
			}
			switch ( alt13 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:319:4: ^( ARGS ( argumentAssignment[$embedded,$argumentContext] )* )
				{
				Match(input,ARGS,Follow._ARGS_in_argList808); 

				if ( input.LA(1)==TokenConstants.Down )
				{
					Match(input, TokenConstants.Down, null); 
					// Language\\ActionEvaluator.g3:319:12: ( argumentAssignment[$embedded,$argumentContext] )*
					for ( ; ; )
					{
						int alt12=2;
						int LA12_0 = input.LA(1);

						if ( (LA12_0==ASSIGN||LA12_0==DOTDOTDOT) )
						{
							alt12=1;
						}


						switch ( alt12 )
						{
						case 1:
							// Language\\ActionEvaluator.g3:319:13: argumentAssignment[$embedded,$argumentContext]
							{
							PushFollow(Follow._argumentAssignment_in_argList811);
							argumentAssignment(embedded, argumentContext);

							state._fsp--;


							}
							break;

						default:
							goto loop12;
						}
					}

					loop12:
						;



					Match(input, TokenConstants.Up, null); 
				}

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:320:4: singleTemplateArg[$embedded,$argumentContext]
				{
				PushFollow(Follow._singleTemplateArg_in_argList821);
				singleTemplateArg(embedded, argumentContext);

				state._fsp--;


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
		return argumentContext;
	}
	// $ANTLR end "argList"


	// $ANTLR start "singleTemplateArg"
	// Language\\ActionEvaluator.g3:323:0: singleTemplateArg[StringTemplate embedded, Map argumentContext] : ^( SINGLEVALUEARG e= expr ) ;
	private void singleTemplateArg( StringTemplate embedded, Map argumentContext )
	{
		object e = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:324:4: ( ^( SINGLEVALUEARG e= expr ) )
			// Language\\ActionEvaluator.g3:324:4: ^( SINGLEVALUEARG e= expr )
			{
			Match(input,SINGLEVALUEARG,Follow._SINGLEVALUEARG_in_singleTemplateArg836); 

			Match(input, TokenConstants.Down, null); 
			PushFollow(Follow._expr_in_singleTemplateArg840);
			e=expr();

			state._fsp--;


			Match(input, TokenConstants.Up, null); 

						if ( e!=null )
						{
							string soleArgName = null;
							// find the sole defined formal argument for embedded
							bool error = false;
							var formalArgs = embedded.FormalArguments;
							if ( formalArgs!=null )
							{
								var argNames = formalArgs.Select( fa => fa.name ).ToArray();
								if ( argNames.Length==1 )
								{
									soleArgName = (string)argNames.ToArray()[0];
									//System.out.println("sole formal arg of "+embedded.Name+" is "+soleArgName);
								}
								else
								{
									error=true;
								}
							}
							else
							{
								error=true;
							}
							if ( error )
							{
								self.Error("template "+embedded.Name+
										   " must have exactly one formal arg in template context "+
										   self.GetEnclosingInstanceStackString());
							}
							else
							{
								self.RawSetArgumentAttribute(embedded,argumentContext,soleArgName,e);
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
		return ;
	}
	// $ANTLR end "singleTemplateArg"


	// $ANTLR start "argumentAssignment"
	// Language\\ActionEvaluator.g3:363:0: argumentAssignment[StringTemplate embedded, Map argumentContext] : ( ^( ASSIGN arg= ID expr ) | DOTDOTDOT );
	private void argumentAssignment( StringTemplate embedded, Map argumentContext )
	{
		StringTemplateAST arg=null;
		object expr11 = default(object);

		try
		{
			// Language\\ActionEvaluator.g3:364:4: ( ^( ASSIGN arg= ID expr ) | DOTDOTDOT )
			int alt14=2;
			int LA14_0 = input.LA(1);

			if ( (LA14_0==ASSIGN) )
			{
				alt14=1;
			}
			else if ( (LA14_0==DOTDOTDOT) )
			{
				alt14=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

				throw nvae;
			}
			switch ( alt14 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:364:4: ^( ASSIGN arg= ID expr )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_argumentAssignment860); 

				Match(input, TokenConstants.Down, null); 
				arg=(StringTemplateAST)Match(input,ID,Follow._ID_in_argumentAssignment864); 
				PushFollow(Follow._expr_in_argumentAssignment866);
				expr11=expr();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

							if ( expr11 != null )
							{
								self.RawSetArgumentAttribute(embedded,argumentContext,(arg!=null?arg.Text:null),expr11);
							}
						

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:371:4: DOTDOTDOT
				{
				Match(input,DOTDOTDOT,Follow._DOTDOTDOT_in_argumentAssignment877); 
				embedded.SetPassThroughAttributes(true);

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
	// $ANTLR end "argumentAssignment"


	// $ANTLR start "actionCompiled"
	// Language\\ActionEvaluator.g3:375:0: public actionCompiled[System.Reflection.Emit.ILGenerator gen] : exprCompiled[$gen] ;
	public void actionCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:376:4: ( exprCompiled[$gen] )
			// Language\\ActionEvaluator.g3:376:4: exprCompiled[$gen]
			{
			PushFollow(Follow._exprCompiled_in_actionCompiled893);
			exprCompiled(gen);

			state._fsp--;


			#if COMPILE_EXPRESSIONS
						EmitWriteAttribute(gen);
			#endif
					

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
	// $ANTLR end "actionCompiled"


	// $ANTLR start "actionFunctional"
	// Language\\ActionEvaluator.g3:385:0: public actionFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,int> func] : exprFunctional ;
	public System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,int> actionFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,int> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,int>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional12 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:386:4: ( exprFunctional )
			// Language\\ActionEvaluator.g3:386:4: exprFunctional
			{
			PushFollow(Follow._exprFunctional_in_actionFunctional915);
			exprFunctional12=exprFunctional();

			state._fsp--;


			#if COMPILE_EXPRESSIONS
						func = (chunk,self,writer) => chunk.WriteAttribute(self,exprFunctional12(chunk,self,writer),writer);
			#endif
					

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
		return func;
	}
	// $ANTLR end "actionFunctional"


	// $ANTLR start "ifConditionCompiled"
	// Language\\ActionEvaluator.g3:395:0: public ifConditionCompiled[System.Reflection.Emit.ILGenerator gen] : ( ifAtomCompiled[$gen] | ^( NOT ifAtomCompiled[$gen] ) );
	public void ifConditionCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:396:4: ( ifAtomCompiled[$gen] | ^( NOT ifAtomCompiled[$gen] ) )
			int alt15=2;
			int LA15_0 = input.LA(1);

			if ( ((LA15_0>=ANONYMOUS_TEMPLATE && LA15_0<=APPLY)||LA15_0==DOT||(LA15_0>=FUNCTION && LA15_0<=INT)||LA15_0==LIST||LA15_0==MULTI_APPLY||LA15_0==PLUS||LA15_0==STRING||LA15_0==VALUE) )
			{
				alt15=1;
			}
			else if ( (LA15_0==NOT) )
			{
				alt15=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 15, 0, input);

				throw nvae;
			}
			switch ( alt15 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:396:4: ifAtomCompiled[$gen]
				{
				PushFollow(Follow._ifAtomCompiled_in_ifConditionCompiled933);
				ifAtomCompiled(gen);

				state._fsp--;


				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:397:4: ^( NOT ifAtomCompiled[$gen] )
				{
				Match(input,NOT,Follow._NOT_in_ifConditionCompiled940); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._ifAtomCompiled_in_ifConditionCompiled942);
				ifAtomCompiled(gen);

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							EmitNot(gen);
				#endif
						

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
	// $ANTLR end "ifConditionCompiled"


	// $ANTLR start "ifConditionFunctional"
	// Language\\ActionEvaluator.g3:406:0: public ifConditionFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> func] : ( ifAtomFunctional | ^( NOT ifAtomFunctional ) );
	public System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> ifConditionFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> ifAtomFunctional13 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> ifAtomFunctional14 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool>);

		try
		{
			// Language\\ActionEvaluator.g3:407:4: ( ifAtomFunctional | ^( NOT ifAtomFunctional ) )
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( ((LA16_0>=ANONYMOUS_TEMPLATE && LA16_0<=APPLY)||LA16_0==DOT||(LA16_0>=FUNCTION && LA16_0<=INT)||LA16_0==LIST||LA16_0==MULTI_APPLY||LA16_0==PLUS||LA16_0==STRING||LA16_0==VALUE) )
			{
				alt16=1;
			}
			else if ( (LA16_0==NOT) )
			{
				alt16=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 16, 0, input);

				throw nvae;
			}
			switch ( alt16 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:407:4: ifAtomFunctional
				{
				PushFollow(Follow._ifAtomFunctional_in_ifConditionFunctional965);
				ifAtomFunctional13=ifAtomFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = ifAtomFunctional13;
				#endif
						

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:413:4: ^( NOT ifAtomFunctional )
				{
				Match(input,NOT,Follow._NOT_in_ifConditionFunctional975); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._ifAtomFunctional_in_ifConditionFunctional977);
				ifAtomFunctional14=ifAtomFunctional();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							func = (chunk,self,writer) => !(ifAtomFunctional14(chunk,self,writer));
				#endif
						

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
		return func;
	}
	// $ANTLR end "ifConditionFunctional"


	// $ANTLR start "ifAtomCompiled"
	// Language\\ActionEvaluator.g3:421:0: ifAtomCompiled[System.Reflection.Emit.ILGenerator gen] : exprCompiled[$gen] ;
	private void ifAtomCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:422:4: ( exprCompiled[$gen] )
			// Language\\ActionEvaluator.g3:422:4: exprCompiled[$gen]
			{
			PushFollow(Follow._exprCompiled_in_ifAtomCompiled994);
			exprCompiled(gen);

			state._fsp--;


			#if COMPILE_EXPRESSIONS
						EmitTest(gen);
			#endif
					

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
	// $ANTLR end "ifAtomCompiled"


	// $ANTLR start "ifAtomFunctional"
	// Language\\ActionEvaluator.g3:430:0: ifAtomFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> func] : exprFunctional ;
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> ifAtomFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,bool>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional15 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:431:4: ( exprFunctional )
			// Language\\ActionEvaluator.g3:431:4: exprFunctional
			{
			PushFollow(Follow._exprFunctional_in_ifAtomFunctional1014);
			exprFunctional15=exprFunctional();

			state._fsp--;


			#if COMPILE_EXPRESSIONS
						func = (chunk,self,writer) => chunk.TestAttributeTrue(exprFunctional15(chunk,self,writer));
			#endif
					

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
		return func;
	}
	// $ANTLR end "ifAtomFunctional"


	// $ANTLR start "exprCompiled"
	// Language\\ActionEvaluator.g3:439:0: exprCompiled[System.Reflection.Emit.ILGenerator gen] : ( ^( PLUS exprCompiled[$gen] exprCompiled[$gen] ) | templateApplicationCompiled[$gen] | attributeCompiled[$gen] | templateIncludeCompiled[$gen] | functionCompiled[$gen] | listCompiled[$gen] | ^( VALUE exprCompiled[$gen] ) );
	private void exprCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:440:4: ( ^( PLUS exprCompiled[$gen] exprCompiled[$gen] ) | templateApplicationCompiled[$gen] | attributeCompiled[$gen] | templateIncludeCompiled[$gen] | functionCompiled[$gen] | listCompiled[$gen] | ^( VALUE exprCompiled[$gen] ) )
			int alt17=7;
			switch ( input.LA(1) )
			{
			case PLUS:
				{
				alt17=1;
				}
				break;
			case APPLY:
			case MULTI_APPLY:
				{
				alt17=2;
				}
				break;
			case ANONYMOUS_TEMPLATE:
			case DOT:
			case ID:
			case INT:
			case STRING:
				{
				alt17=3;
				}
				break;
			case INCLUDE:
				{
				alt17=4;
				}
				break;
			case FUNCTION:
				{
				alt17=5;
				}
				break;
			case LIST:
				{
				alt17=6;
				}
				break;
			case VALUE:
				{
				alt17=7;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 17, 0, input);

					throw nvae;
				}
			}

			switch ( alt17 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:440:4: ^( PLUS exprCompiled[$gen] exprCompiled[$gen] )
				{
				Match(input,PLUS,Follow._PLUS_in_exprCompiled1031); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_exprCompiled1033);
				exprCompiled(gen);

				state._fsp--;

				PushFollow(Follow._exprCompiled_in_exprCompiled1036);
				exprCompiled(gen);

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							EmitAdd(gen);
				#endif
						

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:446:4: templateApplicationCompiled[$gen]
				{
				PushFollow(Follow._templateApplicationCompiled_in_exprCompiled1047);
				templateApplicationCompiled(gen);

				state._fsp--;


				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:447:4: attributeCompiled[$gen]
				{
				PushFollow(Follow._attributeCompiled_in_exprCompiled1053);
				attributeCompiled(gen);

				state._fsp--;


				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:448:4: templateIncludeCompiled[$gen]
				{
				PushFollow(Follow._templateIncludeCompiled_in_exprCompiled1059);
				templateIncludeCompiled(gen);

				state._fsp--;


				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:449:4: functionCompiled[$gen]
				{
				PushFollow(Follow._functionCompiled_in_exprCompiled1065);
				functionCompiled(gen);

				state._fsp--;


				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:450:4: listCompiled[$gen]
				{
				PushFollow(Follow._listCompiled_in_exprCompiled1071);
				listCompiled(gen);

				state._fsp--;


				}
				break;
			case 7:
				// Language\\ActionEvaluator.g3:451:4: ^( VALUE exprCompiled[$gen] )
				{
				Match(input,VALUE,Follow._VALUE_in_exprCompiled1078); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_exprCompiled1080);
				exprCompiled(gen);

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							EmitWriteToString(gen);
				#endif
						

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
	// $ANTLR end "exprCompiled"


	// $ANTLR start "exprFunctional"
	// Language\\ActionEvaluator.g3:459:0: exprFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ( ^( PLUS a= exprFunctional b= exprFunctional ) | templateApplicationFunctional | attributeFunctional | templateIncludeFunctional | functionFunctional | listFunctional | ^( VALUE a= exprFunctional ) );
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> a = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> b = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> templateApplicationFunctional16 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> attributeFunctional17 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> templateIncludeFunctional18 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> functionFunctional19 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> listFunctional20 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:460:4: ( ^( PLUS a= exprFunctional b= exprFunctional ) | templateApplicationFunctional | attributeFunctional | templateIncludeFunctional | functionFunctional | listFunctional | ^( VALUE a= exprFunctional ) )
			int alt18=7;
			switch ( input.LA(1) )
			{
			case PLUS:
				{
				alt18=1;
				}
				break;
			case APPLY:
			case MULTI_APPLY:
				{
				alt18=2;
				}
				break;
			case ANONYMOUS_TEMPLATE:
			case DOT:
			case ID:
			case INT:
			case STRING:
				{
				alt18=3;
				}
				break;
			case INCLUDE:
				{
				alt18=4;
				}
				break;
			case FUNCTION:
				{
				alt18=5;
				}
				break;
			case LIST:
				{
				alt18=6;
				}
				break;
			case VALUE:
				{
				alt18=7;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 18, 0, input);

					throw nvae;
				}
			}

			switch ( alt18 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:460:4: ^( PLUS a= exprFunctional b= exprFunctional )
				{
				Match(input,PLUS,Follow._PLUS_in_exprFunctional1102); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprFunctional_in_exprFunctional1106);
				a=exprFunctional();

				state._fsp--;

				PushFollow(Follow._exprFunctional_in_exprFunctional1110);
				b=exprFunctional();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							func = (chunk,self,writer) => chunk.Add(a(chunk,self,writer),b(chunk,self,writer));
				#endif
						

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:466:4: templateApplicationFunctional
				{
				PushFollow(Follow._templateApplicationFunctional_in_exprFunctional1120);
				templateApplicationFunctional16=templateApplicationFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = templateApplicationFunctional16;
				#endif
						

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:472:4: attributeFunctional
				{
				PushFollow(Follow._attributeFunctional_in_exprFunctional1129);
				attributeFunctional17=attributeFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = attributeFunctional17;
				#endif
						

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:478:4: templateIncludeFunctional
				{
				PushFollow(Follow._templateIncludeFunctional_in_exprFunctional1138);
				templateIncludeFunctional18=templateIncludeFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = templateIncludeFunctional18;
				#endif
						

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:484:4: functionFunctional
				{
				PushFollow(Follow._functionFunctional_in_exprFunctional1147);
				functionFunctional19=functionFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = functionFunctional19;
				#endif
						

				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:490:4: listFunctional
				{
				PushFollow(Follow._listFunctional_in_exprFunctional1156);
				listFunctional20=listFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
							func = listFunctional20;
				#endif
						

				}
				break;
			case 7:
				// Language\\ActionEvaluator.g3:496:4: ^( VALUE a= exprFunctional )
				{
				Match(input,VALUE,Follow._VALUE_in_exprFunctional1166); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprFunctional_in_exprFunctional1170);
				a=exprFunctional();

				state._fsp--;


				Match(input, TokenConstants.Up, null); 

				#if COMPILE_EXPRESSIONS
							func = (chunk,self,writer) =>
								{
									var value = a(chunk,self,writer);
									StringWriter buf = new StringWriter();
									IStringTemplateWriter sw = self.Group.GetStringTemplateWriter( buf );
									int n = chunk.WriteAttribute( self, value, sw );
									if ( n > 0 )
										return buf.ToString();
									return value;
								};
				#endif
						

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
		return func;
	}
	// $ANTLR end "exprFunctional"


	// $ANTLR start "templateApplicationCompiled"
	// Language\\ActionEvaluator.g3:513:0: templateApplicationCompiled[System.Reflection.Emit.ILGenerator gen] : ( ^( APPLY a= exprCompiled[$gen] ( templateCompiled[$gen] )+ ) | ^( MULTI_APPLY ( exprCompiled[$gen] )+ COLON ANONYMOUS_TEMPLATE ) );
	private void templateApplicationCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		StringTemplateAST ANONYMOUS_TEMPLATE21=null;


		#if COMPILE_EXPRESSIONS
			System.Reflection.Emit.LocalBuilder templates = null;
			System.Reflection.Emit.LocalBuilder attributes = null;
		#endif

		try
		{
			// Language\\ActionEvaluator.g3:521:4: ( ^( APPLY a= exprCompiled[$gen] ( templateCompiled[$gen] )+ ) | ^( MULTI_APPLY ( exprCompiled[$gen] )+ COLON ANONYMOUS_TEMPLATE ) )
			int alt21=2;
			int LA21_0 = input.LA(1);

			if ( (LA21_0==APPLY) )
			{
				alt21=1;
			}
			else if ( (LA21_0==MULTI_APPLY) )
			{
				alt21=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 21, 0, input);

				throw nvae;
			}
			switch ( alt21 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:521:4: ^( APPLY a= exprCompiled[$gen] ( templateCompiled[$gen] )+ )
				{
				Match(input,APPLY,Follow._APPLY_in_templateApplicationCompiled1194); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_templateApplicationCompiled1198);
				exprCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
								templates = EmitCreateList<StringTemplate>(gen);
				#endif
							
				// Language\\ActionEvaluator.g3:527:4: ( templateCompiled[$gen] )+
				int cnt19=0;
				for ( ; ; )
				{
					int alt19=2;
					int LA19_0 = input.LA(1);

					if ( (LA19_0==TEMPLATE) )
					{
						alt19=1;
					}


					switch ( alt19 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:527:6: templateCompiled[$gen]
						{
						PushFollow(Follow._templateCompiled_in_templateApplicationCompiled1211);
						templateCompiled(gen);

						state._fsp--;


						#if COMPILE_EXPRESSIONS
											EmitAddValueToList(gen, templates);
						#endif
										

						}
						break;

					default:
						if ( cnt19 >= 1 )
							goto loop19;

						EarlyExitException eee19 = new EarlyExitException( 19, input );
						throw eee19;
					}
					cnt19++;
				}
				loop19:
					;



				#if COMPILE_EXPRESSIONS
								EmitApplyAlternatingTemplates( gen, templates );
				#endif
							

				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:540:4: ^( MULTI_APPLY ( exprCompiled[$gen] )+ COLON ANONYMOUS_TEMPLATE )
				{
				Match(input,MULTI_APPLY,Follow._MULTI_APPLY_in_templateApplicationCompiled1240); 


				#if COMPILE_EXPRESSIONS
								attributes = EmitCreateList<object>(gen);
				#endif
							

				Match(input, TokenConstants.Down, null); 
				// Language\\ActionEvaluator.g3:546:4: ( exprCompiled[$gen] )+
				int cnt20=0;
				for ( ; ; )
				{
					int alt20=2;
					int LA20_0 = input.LA(1);

					if ( ((LA20_0>=ANONYMOUS_TEMPLATE && LA20_0<=APPLY)||LA20_0==DOT||(LA20_0>=FUNCTION && LA20_0<=INT)||LA20_0==LIST||LA20_0==MULTI_APPLY||LA20_0==PLUS||LA20_0==STRING||LA20_0==VALUE) )
					{
						alt20=1;
					}


					switch ( alt20 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:546:6: exprCompiled[$gen]
						{
						PushFollow(Follow._exprCompiled_in_templateApplicationCompiled1252);
						exprCompiled(gen);

						state._fsp--;


						#if COMPILE_EXPRESSIONS
											EmitAddValueToList(gen, attributes);
						#endif
										

						}
						break;

					default:
						if ( cnt20 >= 1 )
							goto loop20;

						EarlyExitException eee20 = new EarlyExitException( 20, input );
						throw eee20;
					}
					cnt20++;
				}
				loop20:
					;


				Match(input,COLON,Follow._COLON_in_templateApplicationCompiled1270); 
				ANONYMOUS_TEMPLATE21=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_templateApplicationCompiled1275); 

				#if COMPILE_EXPRESSIONS
								EmitApplyAnonymousTemplate( gen, ANONYMOUS_TEMPLATE21.StringTemplate, attributes );
				#endif
							

				Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "templateApplicationCompiled"


	// $ANTLR start "templateApplicationFunctional"
	// Language\\ActionEvaluator.g3:563:0: templateApplicationFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ( ^( APPLY a= exprFunctional ( templateFunctional[templateApplicators] )+ ) | ^( MULTI_APPLY (a= exprFunctional )+ COLON ANONYMOUS_TEMPLATE ) );
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> templateApplicationFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		StringTemplateAST ANONYMOUS_TEMPLATE22=null;
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> a = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);


			var  templateApplicators = new List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate>>();
		#if COMPILE_EXPRESSIONS
			List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>> attributes = new List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>>();
		#endif

		try
		{
			// Language\\ActionEvaluator.g3:571:4: ( ^( APPLY a= exprFunctional ( templateFunctional[templateApplicators] )+ ) | ^( MULTI_APPLY (a= exprFunctional )+ COLON ANONYMOUS_TEMPLATE ) )
			int alt24=2;
			int LA24_0 = input.LA(1);

			if ( (LA24_0==APPLY) )
			{
				alt24=1;
			}
			else if ( (LA24_0==MULTI_APPLY) )
			{
				alt24=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 24, 0, input);

				throw nvae;
			}
			switch ( alt24 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:571:4: ^( APPLY a= exprFunctional ( templateFunctional[templateApplicators] )+ )
				{
				Match(input,APPLY,Follow._APPLY_in_templateApplicationFunctional1306); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprFunctional_in_templateApplicationFunctional1310);
				a=exprFunctional();

				state._fsp--;

				// Language\\ActionEvaluator.g3:572:4: ( templateFunctional[templateApplicators] )+
				int cnt22=0;
				for ( ; ; )
				{
					int alt22=2;
					int LA22_0 = input.LA(1);

					if ( (LA22_0==TEMPLATE) )
					{
						alt22=1;
					}


					switch ( alt22 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:572:5: templateFunctional[templateApplicators]
						{
						PushFollow(Follow._templateFunctional_in_templateApplicationFunctional1316);
						templateFunctional(templateApplicators);

						state._fsp--;


						}
						break;

					default:
						if ( cnt22 >= 1 )
							goto loop22;

						EarlyExitException eee22 = new EarlyExitException( 22, input );
						throw eee22;
					}
					cnt22++;
				}
				loop22:
					;



				#if COMPILE_EXPRESSIONS
								func = (chunk,self,writer) =>
									{
										var templatesToApply =	( from applicator in templateApplicators
																  let st = applicator(chunk,self,writer)
																  where st != null
																  select st )
																.ToList();
										return chunk.ApplyListOfAlternatingTemplates( self, a(chunk,self,writer), templatesToApply );
									};
				#endif
							

				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:587:4: ^( MULTI_APPLY (a= exprFunctional )+ COLON ANONYMOUS_TEMPLATE )
				{
				Match(input,MULTI_APPLY,Follow._MULTI_APPLY_in_templateApplicationFunctional1335); 

				Match(input, TokenConstants.Down, null); 
				// Language\\ActionEvaluator.g3:588:4: (a= exprFunctional )+
				int cnt23=0;
				for ( ; ; )
				{
					int alt23=2;
					int LA23_0 = input.LA(1);

					if ( ((LA23_0>=ANONYMOUS_TEMPLATE && LA23_0<=APPLY)||LA23_0==DOT||(LA23_0>=FUNCTION && LA23_0<=INT)||LA23_0==LIST||LA23_0==MULTI_APPLY||LA23_0==PLUS||LA23_0==STRING||LA23_0==VALUE) )
					{
						alt23=1;
					}


					switch ( alt23 )
					{
					case 1:
						// Language\\ActionEvaluator.g3:588:6: a= exprFunctional
						{
						PushFollow(Follow._exprFunctional_in_templateApplicationFunctional1344);
						a=exprFunctional();

						state._fsp--;


						#if COMPILE_EXPRESSIONS
											attributes.Add(a);
						#endif
										

						}
						break;

					default:
						if ( cnt23 >= 1 )
							goto loop23;

						EarlyExitException eee23 = new EarlyExitException( 23, input );
						throw eee23;
					}
					cnt23++;
				}
				loop23:
					;


				Match(input,COLON,Follow._COLON_in_templateApplicationFunctional1361); 
				ANONYMOUS_TEMPLATE22=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_templateApplicationFunctional1363); 

				#if COMPILE_EXPRESSIONS
								StringTemplate anonymous = ANONYMOUS_TEMPLATE22.StringTemplate;
								func = (chunk,self,writer) =>
									{
										var attr =	from f in attributes
													select f(chunk,self,writer);
										return chunk.ApplyTemplateToListOfAttributes( self, attr.ToList(), anonymous );
									};
				#endif
							

				Match(input, TokenConstants.Up, null); 

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
		return func;
	}
	// $ANTLR end "templateApplicationFunctional"


	// $ANTLR start "attributeCompiled"
	// Language\\ActionEvaluator.g3:610:0: attributeCompiled[System.Reflection.Emit.ILGenerator gen] : ( ^( DOT exprCompiled[$gen] (id= ID | ^( VALUE exprCompiled[$gen] ) ) ) |id= ID | INT | STRING | ANONYMOUS_TEMPLATE );
	private void attributeCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		StringTemplateAST id=null;
		StringTemplateAST INT23=null;
		StringTemplateAST STRING24=null;
		StringTemplateAST ANONYMOUS_TEMPLATE25=null;

		try
		{
			// Language\\ActionEvaluator.g3:611:4: ( ^( DOT exprCompiled[$gen] (id= ID | ^( VALUE exprCompiled[$gen] ) ) ) |id= ID | INT | STRING | ANONYMOUS_TEMPLATE )
			int alt26=5;
			switch ( input.LA(1) )
			{
			case DOT:
				{
				alt26=1;
				}
				break;
			case ID:
				{
				alt26=2;
				}
				break;
			case INT:
				{
				alt26=3;
				}
				break;
			case STRING:
				{
				alt26=4;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt26=5;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 26, 0, input);

					throw nvae;
				}
			}

			switch ( alt26 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:611:4: ^( DOT exprCompiled[$gen] (id= ID | ^( VALUE exprCompiled[$gen] ) ) )
				{
				Match(input,DOT,Follow._DOT_in_attributeCompiled1386); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_attributeCompiled1388);
				exprCompiled(gen);

				state._fsp--;

				// Language\\ActionEvaluator.g3:612:4: (id= ID | ^( VALUE exprCompiled[$gen] ) )
				int alt25=2;
				int LA25_0 = input.LA(1);

				if ( (LA25_0==ID) )
				{
					alt25=1;
				}
				else if ( (LA25_0==VALUE) )
				{
					alt25=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 25, 0, input);

					throw nvae;
				}
				switch ( alt25 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:612:6: id= ID
					{
					id=(StringTemplateAST)Match(input,ID,Follow._ID_in_attributeCompiled1398); 

					#if COMPILE_EXPRESSIONS
										EmitLoadString(gen,(id!=null?id.Text:null));
					#endif
									

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:618:6: ^( VALUE exprCompiled[$gen] )
					{
					Match(input,VALUE,Follow._VALUE_in_attributeCompiled1412); 

					Match(input, TokenConstants.Down, null); 
					PushFollow(Follow._exprCompiled_in_attributeCompiled1414);
					exprCompiled(gen);

					state._fsp--;


					Match(input, TokenConstants.Up, null); 

					}
					break;

				}


				#if COMPILE_EXPRESSIONS
								EmitObjectProperty(gen);
				#endif
							

				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:627:4: id= ID
				{
				id=(StringTemplateAST)Match(input,ID,Follow._ID_in_attributeCompiled1438); 

				#if COMPILE_EXPRESSIONS
							EmitAttribute(gen,(id!=null?id.Text:null));
				#endif
						

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:634:4: INT
				{
				INT23=(StringTemplateAST)Match(input,INT,Follow._INT_in_attributeCompiled1448); 

				#if COMPILE_EXPRESSIONS
							EmitLoadIntAsObject(gen,int.Parse((INT23!=null?INT23.Text:null)));
				#endif
						

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:641:4: STRING
				{
				STRING24=(StringTemplateAST)Match(input,STRING,Follow._STRING_in_attributeCompiled1458); 

				#if COMPILE_EXPRESSIONS
							EmitLoadString(gen,(STRING24!=null?STRING24.Text:null));
				#endif
						

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:648:4: ANONYMOUS_TEMPLATE
				{
				ANONYMOUS_TEMPLATE25=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_attributeCompiled1468); 

				#if COMPILE_EXPRESSIONS
							EmitAnonymousTemplate(gen,(ANONYMOUS_TEMPLATE25!=null?ANONYMOUS_TEMPLATE25.Text:null));
				#endif
						

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
	// $ANTLR end "attributeCompiled"


	// $ANTLR start "attributeFunctional"
	// Language\\ActionEvaluator.g3:656:0: attributeFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ( ^( DOT a= exprFunctional (id= ID | ^( VALUE b= exprFunctional ) ) ) |id= ID | INT | STRING | ANONYMOUS_TEMPLATE );
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> attributeFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		StringTemplateAST id=null;
		StringTemplateAST INT26=null;
		StringTemplateAST STRING27=null;
		StringTemplateAST ANONYMOUS_TEMPLATE28=null;
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> a = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> b = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:657:4: ( ^( DOT a= exprFunctional (id= ID | ^( VALUE b= exprFunctional ) ) ) |id= ID | INT | STRING | ANONYMOUS_TEMPLATE )
			int alt28=5;
			switch ( input.LA(1) )
			{
			case DOT:
				{
				alt28=1;
				}
				break;
			case ID:
				{
				alt28=2;
				}
				break;
			case INT:
				{
				alt28=3;
				}
				break;
			case STRING:
				{
				alt28=4;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt28=5;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 28, 0, input);

					throw nvae;
				}
			}

			switch ( alt28 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:657:4: ^( DOT a= exprFunctional (id= ID | ^( VALUE b= exprFunctional ) ) )
				{
				Match(input,DOT,Follow._DOT_in_attributeFunctional1489); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprFunctional_in_attributeFunctional1493);
				a=exprFunctional();

				state._fsp--;

				// Language\\ActionEvaluator.g3:658:4: (id= ID | ^( VALUE b= exprFunctional ) )
				int alt27=2;
				int LA27_0 = input.LA(1);

				if ( (LA27_0==ID) )
				{
					alt27=1;
				}
				else if ( (LA27_0==VALUE) )
				{
					alt27=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 27, 0, input);

					throw nvae;
				}
				switch ( alt27 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:658:6: id= ID
					{
					id=(StringTemplateAST)Match(input,ID,Follow._ID_in_attributeFunctional1502); 

					#if COMPILE_EXPRESSIONS
										string text = (id!=null?id.Text:null);
										func = (chunk,self,writer) => chunk.GetObjectProperty( self, a(chunk,self,writer), text );
					#endif
									

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:665:6: ^( VALUE b= exprFunctional )
					{
					Match(input,VALUE,Follow._VALUE_in_attributeFunctional1516); 

					Match(input, TokenConstants.Down, null); 
					PushFollow(Follow._exprFunctional_in_attributeFunctional1520);
					b=exprFunctional();

					state._fsp--;


					Match(input, TokenConstants.Up, null); 

					#if COMPILE_EXPRESSIONS
										func = (chunk,self,writer) => chunk.GetObjectProperty( self, a(chunk,self,writer), b(chunk,self,writer) );
					#endif
									

					}
					break;

				}


				Match(input, TokenConstants.Up, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:674:4: id= ID
				{
				id=(StringTemplateAST)Match(input,ID,Follow._ID_in_attributeFunctional1544); 

				#if COMPILE_EXPRESSIONS
							string text = (id!=null?id.Text:null);
							func = (chunk,self,writer) => self.GetAttribute( text );
				#endif
						

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:682:4: INT
				{
				INT26=(StringTemplateAST)Match(input,INT,Follow._INT_in_attributeFunctional1554); 

				#if COMPILE_EXPRESSIONS
							int i = int.Parse((INT26!=null?INT26.Text:null));
							func = (chunk,self,writer) => i;
				#endif
						

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:690:4: STRING
				{
				STRING27=(StringTemplateAST)Match(input,STRING,Follow._STRING_in_attributeFunctional1564); 

				#if COMPILE_EXPRESSIONS
							string text = (STRING27!=null?STRING27.Text:null);
							func = (chunk,self,writer) => text;
				#endif
						

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:698:4: ANONYMOUS_TEMPLATE
				{
				ANONYMOUS_TEMPLATE28=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_attributeFunctional1574); 

				#if COMPILE_EXPRESSIONS
							string text = (ANONYMOUS_TEMPLATE28!=null?ANONYMOUS_TEMPLATE28.Text:null);
							if ( text != null )
							{
								func = (chunk,self,writer) =>
									new StringTemplate( self.Group, text )
									{
										EnclosingInstance = self,
										Name = "<anonymous template argument>"
									};
							}
							else
							{
								func = (chunk,self,writer) => null;
							}
				#endif
						

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
		return func;
	}
	// $ANTLR end "attributeFunctional"


	// $ANTLR start "templateIncludeCompiled"
	// Language\\ActionEvaluator.g3:719:0: templateIncludeCompiled[System.Reflection.Emit.ILGenerator gen] : ^( INCLUDE ( ID args= . | ^( VALUE exprCompiled[$gen] args= . ) ) ) ;
	private void templateIncludeCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		StringTemplateAST ID29=null;
		StringTemplateAST args=null;

		try
		{
			// Language\\ActionEvaluator.g3:720:4: ( ^( INCLUDE ( ID args= . | ^( VALUE exprCompiled[$gen] args= . ) ) ) )
			// Language\\ActionEvaluator.g3:720:4: ^( INCLUDE ( ID args= . | ^( VALUE exprCompiled[$gen] args= . ) ) )
			{
			Match(input,INCLUDE,Follow._INCLUDE_in_templateIncludeCompiled1592); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:721:4: ( ID args= . | ^( VALUE exprCompiled[$gen] args= . ) )
			int alt29=2;
			int LA29_0 = input.LA(1);

			if ( (LA29_0==ID) )
			{
				alt29=1;
			}
			else if ( (LA29_0==VALUE) )
			{
				alt29=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 29, 0, input);

				throw nvae;
			}
			switch ( alt29 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:721:6: ID args= .
				{
				ID29=(StringTemplateAST)Match(input,ID,Follow._ID_in_templateIncludeCompiled1599); 
				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				#if COMPILE_EXPRESSIONS
									EmitLoadString( gen, (ID29!=null?ID29.Text:null) );
				#endif
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:727:6: ^( VALUE exprCompiled[$gen] args= . )
				{
				Match(input,VALUE,Follow._VALUE_in_templateIncludeCompiled1618); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_templateIncludeCompiled1620);
				exprCompiled(gen);

				state._fsp--;

				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				Match(input, TokenConstants.Up, null); 

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

			#if COMPILE_EXPRESSIONS
						EmitTemplateInclude( gen, args );
			#endif
					

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
	// $ANTLR end "templateIncludeCompiled"


	// $ANTLR start "templateIncludeFunctional"
	// Language\\ActionEvaluator.g3:737:0: templateIncludeFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ^( INCLUDE ( ID args= . | ^( VALUE exprFunctional args= . ) ) ) ;
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> templateIncludeFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		StringTemplateAST ID30=null;
		StringTemplateAST args=null;
		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional31 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:738:4: ( ^( INCLUDE ( ID args= . | ^( VALUE exprFunctional args= . ) ) ) )
			// Language\\ActionEvaluator.g3:738:4: ^( INCLUDE ( ID args= . | ^( VALUE exprFunctional args= . ) ) )
			{
			Match(input,INCLUDE,Follow._INCLUDE_in_templateIncludeFunctional1657); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:739:4: ( ID args= . | ^( VALUE exprFunctional args= . ) )
			int alt30=2;
			int LA30_0 = input.LA(1);

			if ( (LA30_0==ID) )
			{
				alt30=1;
			}
			else if ( (LA30_0==VALUE) )
			{
				alt30=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 30, 0, input);

				throw nvae;
			}
			switch ( alt30 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:739:6: ID args= .
				{
				ID30=(StringTemplateAST)Match(input,ID,Follow._ID_in_templateIncludeFunctional1664); 
				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:740:6: ^( VALUE exprFunctional args= . )
				{
				Match(input,VALUE,Follow._VALUE_in_templateIncludeFunctional1677); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprFunctional_in_templateIncludeFunctional1679);
				exprFunctional31=exprFunctional();

				state._fsp--;

				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				Match(input, TokenConstants.Up, null); 

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

			#if COMPILE_EXPRESSIONS
						if ( ID30 != null )
						{
							string name = (ID30!=null?ID30.Text:null);
							if ( name != null )
							{
								func = (chunk,self,writer) =>
									{
										return chunk.GetTemplateInclude( self, name, args );
									};
							}
							else
							{
								func = (chunk,self,writer) => null;
							}
						}
						else
						{
							func = (chunk,self,writer) =>
								{
									var value = exprFunctional31(chunk,self,writer);
									if ( value == null )
										return null;
										
									string name = value.ToString();
									if ( name != null )
									{
										return chunk.GetTemplateInclude( self, name, args );
									}
									return null;
								};
						}
			#endif
					

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
		return func;
	}
	// $ANTLR end "templateIncludeFunctional"


	// $ANTLR start "functionCompiled"
	// Language\\ActionEvaluator.g3:780:0: functionCompiled[System.Reflection.Emit.ILGenerator gen] : ^( FUNCTION ( 'first' singleFunctionArgCompiled[$gen] | 'rest' singleFunctionArgCompiled[$gen] | 'last' singleFunctionArgCompiled[$gen] | 'length' singleFunctionArgCompiled[$gen] | 'strip' singleFunctionArgCompiled[$gen] | 'trunc' singleFunctionArgCompiled[$gen] ) ) ;
	private void functionCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:781:4: ( ^( FUNCTION ( 'first' singleFunctionArgCompiled[$gen] | 'rest' singleFunctionArgCompiled[$gen] | 'last' singleFunctionArgCompiled[$gen] | 'length' singleFunctionArgCompiled[$gen] | 'strip' singleFunctionArgCompiled[$gen] | 'trunc' singleFunctionArgCompiled[$gen] ) ) )
			// Language\\ActionEvaluator.g3:781:4: ^( FUNCTION ( 'first' singleFunctionArgCompiled[$gen] | 'rest' singleFunctionArgCompiled[$gen] | 'last' singleFunctionArgCompiled[$gen] | 'length' singleFunctionArgCompiled[$gen] | 'strip' singleFunctionArgCompiled[$gen] | 'trunc' singleFunctionArgCompiled[$gen] ) )
			{
			Match(input,FUNCTION,Follow._FUNCTION_in_functionCompiled1712); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:782:4: ( 'first' singleFunctionArgCompiled[$gen] | 'rest' singleFunctionArgCompiled[$gen] | 'last' singleFunctionArgCompiled[$gen] | 'length' singleFunctionArgCompiled[$gen] | 'strip' singleFunctionArgCompiled[$gen] | 'trunc' singleFunctionArgCompiled[$gen] )
			int alt31=6;
			switch ( input.LA(1) )
			{
			case FIRST:
				{
				alt31=1;
				}
				break;
			case REST:
				{
				alt31=2;
				}
				break;
			case LAST:
				{
				alt31=3;
				}
				break;
			case LENGTH:
				{
				alt31=4;
				}
				break;
			case STRIP:
				{
				alt31=5;
				}
				break;
			case TRUNC:
				{
				alt31=6;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 31, 0, input);

					throw nvae;
				}
			}

			switch ( alt31 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:782:6: 'first' singleFunctionArgCompiled[$gen]
				{
				Match(input,FIRST,Follow._FIRST_in_functionCompiled1719); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1722);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionFirst(gen);
				#endif
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:788:6: 'rest' singleFunctionArgCompiled[$gen]
				{
				Match(input,REST,Follow._REST_in_functionCompiled1736); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1739);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionRest(gen);
				#endif
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:794:6: 'last' singleFunctionArgCompiled[$gen]
				{
				Match(input,LAST,Follow._LAST_in_functionCompiled1753); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1756);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionLast(gen);
				#endif
								

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:800:6: 'length' singleFunctionArgCompiled[$gen]
				{
				Match(input,LENGTH,Follow._LENGTH_in_functionCompiled1770); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1772);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionLength(gen);
				#endif
								

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:806:6: 'strip' singleFunctionArgCompiled[$gen]
				{
				Match(input,STRIP,Follow._STRIP_in_functionCompiled1786); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1789);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionStrip(gen);
				#endif
								

				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:812:6: 'trunc' singleFunctionArgCompiled[$gen]
				{
				Match(input,TRUNC,Follow._TRUNC_in_functionCompiled1803); 
				PushFollow(Follow._singleFunctionArgCompiled_in_functionCompiled1806);
				singleFunctionArgCompiled(gen);

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									EmitFunctionTrunc(gen);
				#endif
								

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "functionCompiled"


	// $ANTLR start "functionFunctional"
	// Language\\ActionEvaluator.g3:822:0: functionFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ^( FUNCTION ( 'first' a= singleFunctionArgFunctional | 'rest' a= singleFunctionArgFunctional | 'last' a= singleFunctionArgFunctional | 'length' a= singleFunctionArgFunctional | 'strip' a= singleFunctionArgFunctional | 'trunc' a= singleFunctionArgFunctional ) ) ;
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> functionFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> a = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:823:4: ( ^( FUNCTION ( 'first' a= singleFunctionArgFunctional | 'rest' a= singleFunctionArgFunctional | 'last' a= singleFunctionArgFunctional | 'length' a= singleFunctionArgFunctional | 'strip' a= singleFunctionArgFunctional | 'trunc' a= singleFunctionArgFunctional ) ) )
			// Language\\ActionEvaluator.g3:823:4: ^( FUNCTION ( 'first' a= singleFunctionArgFunctional | 'rest' a= singleFunctionArgFunctional | 'last' a= singleFunctionArgFunctional | 'length' a= singleFunctionArgFunctional | 'strip' a= singleFunctionArgFunctional | 'trunc' a= singleFunctionArgFunctional ) )
			{
			Match(input,FUNCTION,Follow._FUNCTION_in_functionFunctional1839); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:824:4: ( 'first' a= singleFunctionArgFunctional | 'rest' a= singleFunctionArgFunctional | 'last' a= singleFunctionArgFunctional | 'length' a= singleFunctionArgFunctional | 'strip' a= singleFunctionArgFunctional | 'trunc' a= singleFunctionArgFunctional )
			int alt32=6;
			switch ( input.LA(1) )
			{
			case FIRST:
				{
				alt32=1;
				}
				break;
			case REST:
				{
				alt32=2;
				}
				break;
			case LAST:
				{
				alt32=3;
				}
				break;
			case LENGTH:
				{
				alt32=4;
				}
				break;
			case STRIP:
				{
				alt32=5;
				}
				break;
			case TRUNC:
				{
				alt32=6;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 32, 0, input);

					throw nvae;
				}
			}

			switch ( alt32 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:824:6: 'first' a= singleFunctionArgFunctional
				{
				Match(input,FIRST,Follow._FIRST_in_functionFunctional1846); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1851);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.First( a(chunk,self,writer) );
				#endif
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:830:6: 'rest' a= singleFunctionArgFunctional
				{
				Match(input,REST,Follow._REST_in_functionFunctional1864); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1869);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.Rest( a(chunk,self,writer) );
				#endif
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:836:6: 'last' a= singleFunctionArgFunctional
				{
				Match(input,LAST,Follow._LAST_in_functionFunctional1882); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1887);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.Last( a(chunk,self,writer) );
				#endif
								

				}
				break;
			case 4:
				// Language\\ActionEvaluator.g3:842:6: 'length' a= singleFunctionArgFunctional
				{
				Match(input,LENGTH,Follow._LENGTH_in_functionFunctional1900); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1904);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.Length( a(chunk,self,writer) );
				#endif
								

				}
				break;
			case 5:
				// Language\\ActionEvaluator.g3:848:6: 'strip' a= singleFunctionArgFunctional
				{
				Match(input,STRIP,Follow._STRIP_in_functionFunctional1917); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1922);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.Strip( a(chunk,self,writer) );
				#endif
								

				}
				break;
			case 6:
				// Language\\ActionEvaluator.g3:854:6: 'trunc' a= singleFunctionArgFunctional
				{
				Match(input,TRUNC,Follow._TRUNC_in_functionFunctional1935); 
				PushFollow(Follow._singleFunctionArgFunctional_in_functionFunctional1940);
				a=singleFunctionArgFunctional();

				state._fsp--;


				#if COMPILE_EXPRESSIONS
									func = (chunk,self,writer) => chunk.Trunc( a(chunk,self,writer) );
				#endif
								

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
		return func;
	}
	// $ANTLR end "functionFunctional"


	// $ANTLR start "listCompiled"
	// Language\\ActionEvaluator.g3:864:0: listCompiled[System.Reflection.Emit.ILGenerator gen] : ^( LIST ( exprCompiled[$gen] | NOTHING )+ ) ;
	private void listCompiled( System.Reflection.Emit.ILGenerator gen )
	{

		#if COMPILE_EXPRESSIONS
			var elements = EmitCreateList<object>(gen);
		#endif

		try
		{
			// Language\\ActionEvaluator.g3:871:4: ( ^( LIST ( exprCompiled[$gen] | NOTHING )+ ) )
			// Language\\ActionEvaluator.g3:871:4: ^( LIST ( exprCompiled[$gen] | NOTHING )+ )
			{
			Match(input,LIST,Follow._LIST_in_listCompiled1974); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:872:4: ( exprCompiled[$gen] | NOTHING )+
			int cnt33=0;
			for ( ; ; )
			{
				int alt33=3;
				int LA33_0 = input.LA(1);

				if ( ((LA33_0>=ANONYMOUS_TEMPLATE && LA33_0<=APPLY)||LA33_0==DOT||(LA33_0>=FUNCTION && LA33_0<=INT)||LA33_0==LIST||LA33_0==MULTI_APPLY||LA33_0==PLUS||LA33_0==STRING||LA33_0==VALUE) )
				{
					alt33=1;
				}
				else if ( (LA33_0==NOTHING) )
				{
					alt33=2;
				}


				switch ( alt33 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:872:6: exprCompiled[$gen]
					{
					PushFollow(Follow._exprCompiled_in_listCompiled1981);
					exprCompiled(gen);

					state._fsp--;


					#if COMPILE_EXPRESSIONS
										EmitAddValueToList(gen, elements);
					#endif
									

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:878:6: NOTHING
					{
					Match(input,NOTHING,Follow._NOTHING_in_listCompiled1995); 

					#if COMPILE_EXPRESSIONS
										EmitAddNothingToList(gen, elements);
					#endif
									

					}
					break;

				default:
					if ( cnt33 >= 1 )
						goto loop33;

					EarlyExitException eee33 = new EarlyExitException( 33, input );
					throw eee33;
				}
				cnt33++;
			}
			loop33:
				;



			Match(input, TokenConstants.Up, null); 

			#if COMPILE_EXPRESSIONS
						EmitCatList(gen,elements);
			#endif
					

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
	// $ANTLR end "listCompiled"


	// $ANTLR start "listFunctional"
	// Language\\ActionEvaluator.g3:893:0: listFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ^( LIST ( exprFunctional | NOTHING )+ ) ;
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> listFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional32 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);


			List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>> elements = new List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>>();

		try
		{
			// Language\\ActionEvaluator.g3:898:4: ( ^( LIST ( exprFunctional | NOTHING )+ ) )
			// Language\\ActionEvaluator.g3:898:4: ^( LIST ( exprFunctional | NOTHING )+ )
			{
			Match(input,LIST,Follow._LIST_in_listFunctional2037); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:899:4: ( exprFunctional | NOTHING )+
			int cnt34=0;
			for ( ; ; )
			{
				int alt34=3;
				int LA34_0 = input.LA(1);

				if ( ((LA34_0>=ANONYMOUS_TEMPLATE && LA34_0<=APPLY)||LA34_0==DOT||(LA34_0>=FUNCTION && LA34_0<=INT)||LA34_0==LIST||LA34_0==MULTI_APPLY||LA34_0==PLUS||LA34_0==STRING||LA34_0==VALUE) )
				{
					alt34=1;
				}
				else if ( (LA34_0==NOTHING) )
				{
					alt34=2;
				}


				switch ( alt34 )
				{
				case 1:
					// Language\\ActionEvaluator.g3:899:6: exprFunctional
					{
					PushFollow(Follow._exprFunctional_in_listFunctional2044);
					exprFunctional32=exprFunctional();

					state._fsp--;


					#if COMPILE_EXPRESSIONS
										if ( exprFunctional32 != null )
											elements.Add(exprFunctional32);
					#endif
									

					}
					break;
				case 2:
					// Language\\ActionEvaluator.g3:906:6: NOTHING
					{
					Match(input,NOTHING,Follow._NOTHING_in_listFunctional2057); 

					#if COMPILE_EXPRESSIONS
										elements.Add( (chunk,self,writer) => new ArrayList( new object[] { null } ).iterator() );
					#endif
									

					}
					break;

				default:
					if ( cnt34 >= 1 )
						goto loop34;

					EarlyExitException eee34 = new EarlyExitException( 34, input );
					throw eee34;
				}
				cnt34++;
			}
			loop34:
				;



			Match(input, TokenConstants.Up, null); 

			#if COMPILE_EXPRESSIONS
						func = (chunk,self,writer) =>
							new Cat(from f in elements
									let value = f(chunk,self,writer)
									where value != null
									select value
									);
			#endif
					

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
		return func;
	}
	// $ANTLR end "listFunctional"


	// $ANTLR start "singleFunctionArgCompiled"
	// Language\\ActionEvaluator.g3:926:0: singleFunctionArgCompiled[System.Reflection.Emit.ILGenerator gen] : ^( SINGLEVALUEARG exprCompiled[$gen] ) ;
	private void singleFunctionArgCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		try
		{
			// Language\\ActionEvaluator.g3:927:4: ( ^( SINGLEVALUEARG exprCompiled[$gen] ) )
			// Language\\ActionEvaluator.g3:927:4: ^( SINGLEVALUEARG exprCompiled[$gen] )
			{
			Match(input,SINGLEVALUEARG,Follow._SINGLEVALUEARG_in_singleFunctionArgCompiled2091); 

			Match(input, TokenConstants.Down, null); 
			PushFollow(Follow._exprCompiled_in_singleFunctionArgCompiled2093);
			exprCompiled(gen);

			state._fsp--;


			Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "singleFunctionArgCompiled"


	// $ANTLR start "singleFunctionArgFunctional"
	// Language\\ActionEvaluator.g3:930:0: singleFunctionArgFunctional returns [System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func] : ^( SINGLEVALUEARG exprFunctional ) ;
	private System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> singleFunctionArgFunctional(  )
	{

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> func = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object> exprFunctional33 = default(System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,object>);

		try
		{
			// Language\\ActionEvaluator.g3:931:4: ( ^( SINGLEVALUEARG exprFunctional ) )
			// Language\\ActionEvaluator.g3:931:4: ^( SINGLEVALUEARG exprFunctional )
			{
			Match(input,SINGLEVALUEARG,Follow._SINGLEVALUEARG_in_singleFunctionArgFunctional2113); 

			Match(input, TokenConstants.Down, null); 
			PushFollow(Follow._exprFunctional_in_singleFunctionArgFunctional2115);
			exprFunctional33=exprFunctional();

			state._fsp--;

			func = exprFunctional33;

			Match(input, TokenConstants.Up, null); 

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
		return func;
	}
	// $ANTLR end "singleFunctionArgFunctional"


	// $ANTLR start "templateCompiled"
	// Language\\ActionEvaluator.g3:934:0: templateCompiled[System.Reflection.Emit.ILGenerator gen] : ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= exprCompiled[$gen] args2= . ) ) ) ;
	private void templateCompiled( System.Reflection.Emit.ILGenerator gen )
	{
		StringTemplateAST anon=null;
		StringTemplateAST args=null;
		StringTemplateAST args2=null;

		try
		{
			// Language\\ActionEvaluator.g3:935:4: ( ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= exprCompiled[$gen] args2= . ) ) ) )
			// Language\\ActionEvaluator.g3:935:4: ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= exprCompiled[$gen] args2= . ) ) )
			{
			Match(input,TEMPLATE,Follow._TEMPLATE_in_templateCompiled2133); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:936:4: ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= exprCompiled[$gen] args2= . ) )
			int alt35=3;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt35=1;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt35=2;
				}
				break;
			case VALUE:
				{
				alt35=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 35, 0, input);

					throw nvae;
				}
			}

			switch ( alt35 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:936:6: ID args= .
				{
				Match(input,ID,Follow._ID_in_templateCompiled2140); 
				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				#if COMPILE_EXPRESSIONS
									throw new System.NotImplementedException();
				#endif
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:943:6: anon= ANONYMOUS_TEMPLATE
				{
				anon=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_templateCompiled2161); 

				#if COMPILE_EXPRESSIONS
									throw new System.NotImplementedException();
				#endif
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:950:6: ^( VALUE n= exprCompiled[$gen] args2= . )
				{
				Match(input,VALUE,Follow._VALUE_in_templateCompiled2177); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._exprCompiled_in_templateCompiled2181);
				exprCompiled(gen);

				state._fsp--;

				args2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				#if COMPILE_EXPRESSIONS
										throw new System.NotImplementedException();
				#endif
									

				Match(input, TokenConstants.Up, null); 

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "templateCompiled"


	// $ANTLR start "templateFunctional"
	// Language\\ActionEvaluator.g3:961:0: templateFunctional[List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate>> templateApplicators] : ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) ) ;
	private void templateFunctional( List<System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate>> templateApplicators )
	{
		StringTemplateAST anon=null;
		StringTemplateAST ID34=null;
		StringTemplateAST args=null;
		StringTemplateAST args2=null;
		object n = default(object);


			Map argumentContext = null;

		try
		{
			// Language\\ActionEvaluator.g3:966:4: ( ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) ) )
			// Language\\ActionEvaluator.g3:966:4: ^( TEMPLATE ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) ) )
			{
			Match(input,TEMPLATE,Follow._TEMPLATE_in_templateFunctional2227); 

			Match(input, TokenConstants.Down, null); 
			// Language\\ActionEvaluator.g3:967:4: ( ID args= . |anon= ANONYMOUS_TEMPLATE | ^( VALUE n= expr args2= . ) )
			int alt36=3;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt36=1;
				}
				break;
			case ANONYMOUS_TEMPLATE:
				{
				alt36=2;
				}
				break;
			case VALUE:
				{
				alt36=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 36, 0, input);

					throw nvae;
				}
			}

			switch ( alt36 )
			{
			case 1:
				// Language\\ActionEvaluator.g3:967:6: ID args= .
				{
				ID34=(StringTemplateAST)Match(input,ID,Follow._ID_in_templateFunctional2234); 
				args=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				#if COMPILE_EXPRESSIONS
									System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate> func =
										(chunk,self,writer) =>
										{
											string templateName = (ID34!=null?ID34.Text:null);
											StringTemplateGroup group = self.Group;
											StringTemplate embedded = group.GetEmbeddedInstanceOf(self, templateName);
											if ( embedded!=null )
											{
												embedded.ArgumentsAST = args;
											}
											return embedded;
										};
									templateApplicators.Add( func );
				#endif
								

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:986:6: anon= ANONYMOUS_TEMPLATE
				{
				anon=(StringTemplateAST)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_templateFunctional2255); 

				#if COMPILE_EXPRESSIONS
									System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate> func =
										(chunk,self,writer) =>
										{
											StringTemplate anonymous = anon.StringTemplate;
											// to properly see overridden templates, always set
											// anonymous' group to be self's group
											anonymous.Group = self.Group;
											return anonymous;
										};
									templateApplicators.Add( func );
				#endif
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:1002:6: ^( VALUE n= expr args2= . )
				{
				Match(input,VALUE,Follow._VALUE_in_templateFunctional2271); 

				Match(input, TokenConstants.Down, null); 
				PushFollow(Follow._expr_in_templateFunctional2275);
				n=expr();

				state._fsp--;

				args2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				#if COMPILE_EXPRESSIONS
										System.Func<ASTExpr,StringTemplate,IStringTemplateWriter,StringTemplate> func =
											(chunk,self,writer) =>
											{
												StringTemplate embedded = null;
												if ( n!=null )
												{
													string templateName = n.ToString();
													StringTemplateGroup group = self.Group;
													embedded = group.GetEmbeddedInstanceOf(self, templateName);
													if ( embedded!=null )
													{
														embedded.ArgumentsAST = args2;
													}
												}
												return embedded;
											};
										templateApplicators.Add( func );
				#endif
									

				Match(input, TokenConstants.Up, null); 

				}
				break;

			}


			Match(input, TokenConstants.Up, null); 

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
	// $ANTLR end "templateFunctional"
	#endregion Rules


	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _expr_in_action56 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_expr79 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_expr83 = new BitSet(new ulong[]{0x410428F0830UL});
		public static readonly BitSet _expr_in_expr87 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _templateApplication_in_expr95 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attribute_in_expr102 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templateInclude_in_expr109 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _function_in_expr116 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _list_in_expr123 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VALUE_in_expr131 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_expr135 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LIST_in_list167 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_list174 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _NOTHING_in_list187 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _INCLUDE_in_templateInclude229 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateInclude242 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _VALUE_in_templateInclude262 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateInclude266 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _APPLY_in_templateApplication316 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateApplication320 = new BitSet(new ulong[]{0x8000000000UL});
		public static readonly BitSet _template_in_templateApplication326 = new BitSet(new ulong[]{0x8000000008UL});
		public static readonly BitSet _MULTI_APPLY_in_templateApplication345 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateApplication350 = new BitSet(new ulong[]{0x410428F0930UL});
		public static readonly BitSet _COLON_in_templateApplication357 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateApplication364 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FUNCTION_in_function390 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _FIRST_in_function397 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function402 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REST_in_function411 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function416 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LAST_in_function425 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function430 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LENGTH_in_function439 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function443 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRIP_in_function452 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function457 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TRUNC_in_function466 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArg_in_function471 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SINGLEVALUEARG_in_singleFunctionArg499 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_singleFunctionArg501 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_template524 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_template531 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_template552 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_template568 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_template572 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _ifAtom_in_ifCondition617 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_ifCondition625 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ifAtom_in_ifCondition629 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _expr_in_ifAtom647 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_attribute671 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_attribute675 = new BitSet(new ulong[]{0x40000020000UL});
		public static readonly BitSet _ID_in_attribute684 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_attribute714 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_attribute718 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_attribute742 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_attribute754 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_attribute764 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_attribute776 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ARGS_in_argList808 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _argumentAssignment_in_argList811 = new BitSet(new ulong[]{0x1088UL});
		public static readonly BitSet _singleTemplateArg_in_argList821 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SINGLEVALUEARG_in_singleTemplateArg836 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_singleTemplateArg840 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_argumentAssignment860 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_argumentAssignment864 = new BitSet(new ulong[]{0x410428F0830UL});
		public static readonly BitSet _expr_in_argumentAssignment866 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOTDOTDOT_in_argumentAssignment877 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _exprCompiled_in_actionCompiled893 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _exprFunctional_in_actionFunctional915 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ifAtomCompiled_in_ifConditionCompiled933 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_ifConditionCompiled940 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ifAtomCompiled_in_ifConditionCompiled942 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ifAtomFunctional_in_ifConditionFunctional965 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_ifConditionFunctional975 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ifAtomFunctional_in_ifConditionFunctional977 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exprCompiled_in_ifAtomCompiled994 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _exprFunctional_in_ifAtomFunctional1014 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_exprCompiled1031 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_exprCompiled1033 = new BitSet(new ulong[]{0x410428F0830UL});
		public static readonly BitSet _exprCompiled_in_exprCompiled1036 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _templateApplicationCompiled_in_exprCompiled1047 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attributeCompiled_in_exprCompiled1053 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templateIncludeCompiled_in_exprCompiled1059 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _functionCompiled_in_exprCompiled1065 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _listCompiled_in_exprCompiled1071 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VALUE_in_exprCompiled1078 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_exprCompiled1080 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_in_exprFunctional1102 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_exprFunctional1106 = new BitSet(new ulong[]{0x410428F0830UL});
		public static readonly BitSet _exprFunctional_in_exprFunctional1110 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _templateApplicationFunctional_in_exprFunctional1120 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attributeFunctional_in_exprFunctional1129 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templateIncludeFunctional_in_exprFunctional1138 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _functionFunctional_in_exprFunctional1147 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _listFunctional_in_exprFunctional1156 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VALUE_in_exprFunctional1166 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_exprFunctional1170 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _APPLY_in_templateApplicationCompiled1194 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_templateApplicationCompiled1198 = new BitSet(new ulong[]{0x8000000000UL});
		public static readonly BitSet _templateCompiled_in_templateApplicationCompiled1211 = new BitSet(new ulong[]{0x8000000008UL});
		public static readonly BitSet _MULTI_APPLY_in_templateApplicationCompiled1240 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_templateApplicationCompiled1252 = new BitSet(new ulong[]{0x410428F0930UL});
		public static readonly BitSet _COLON_in_templateApplicationCompiled1270 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateApplicationCompiled1275 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _APPLY_in_templateApplicationFunctional1306 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_templateApplicationFunctional1310 = new BitSet(new ulong[]{0x8000000000UL});
		public static readonly BitSet _templateFunctional_in_templateApplicationFunctional1316 = new BitSet(new ulong[]{0x8000000008UL});
		public static readonly BitSet _MULTI_APPLY_in_templateApplicationFunctional1335 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_templateApplicationFunctional1344 = new BitSet(new ulong[]{0x410428F0930UL});
		public static readonly BitSet _COLON_in_templateApplicationFunctional1361 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateApplicationFunctional1363 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOT_in_attributeCompiled1386 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_attributeCompiled1388 = new BitSet(new ulong[]{0x40000020000UL});
		public static readonly BitSet _ID_in_attributeCompiled1398 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_attributeCompiled1412 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_attributeCompiled1414 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_attributeCompiled1438 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_attributeCompiled1448 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_attributeCompiled1458 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_attributeCompiled1468 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_attributeFunctional1489 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_attributeFunctional1493 = new BitSet(new ulong[]{0x40000020000UL});
		public static readonly BitSet _ID_in_attributeFunctional1502 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_attributeFunctional1516 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_attributeFunctional1520 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_attributeFunctional1544 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_attributeFunctional1554 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_attributeFunctional1564 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_attributeFunctional1574 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INCLUDE_in_templateIncludeCompiled1592 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateIncludeCompiled1599 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _VALUE_in_templateIncludeCompiled1618 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_templateIncludeCompiled1620 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _INCLUDE_in_templateIncludeFunctional1657 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateIncludeFunctional1664 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _VALUE_in_templateIncludeFunctional1677 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_templateIncludeFunctional1679 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _FUNCTION_in_functionCompiled1712 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _FIRST_in_functionCompiled1719 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1722 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REST_in_functionCompiled1736 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1739 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LAST_in_functionCompiled1753 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1756 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LENGTH_in_functionCompiled1770 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1772 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRIP_in_functionCompiled1786 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1789 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TRUNC_in_functionCompiled1803 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgCompiled_in_functionCompiled1806 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FUNCTION_in_functionFunctional1839 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _FIRST_in_functionFunctional1846 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1851 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REST_in_functionFunctional1864 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1869 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LAST_in_functionFunctional1882 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1887 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LENGTH_in_functionFunctional1900 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1904 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRIP_in_functionFunctional1917 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1922 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TRUNC_in_functionFunctional1935 = new BitSet(new ulong[]{0x800000000UL});
		public static readonly BitSet _singleFunctionArgFunctional_in_functionFunctional1940 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LIST_in_listCompiled1974 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_listCompiled1981 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _NOTHING_in_listCompiled1995 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _LIST_in_listFunctional2037 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_listFunctional2044 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _NOTHING_in_listFunctional2057 = new BitSet(new ulong[]{0x410628F0838UL});
		public static readonly BitSet _SINGLEVALUEARG_in_singleFunctionArgCompiled2091 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_singleFunctionArgCompiled2093 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SINGLEVALUEARG_in_singleFunctionArgFunctional2113 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprFunctional_in_singleFunctionArgFunctional2115 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_templateCompiled2133 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateCompiled2140 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateCompiled2161 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_templateCompiled2177 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _exprCompiled_in_templateCompiled2181 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _TEMPLATE_in_templateFunctional2227 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateFunctional2234 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateFunctional2255 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_templateFunctional2271 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateFunctional2275 = new BitSet(new ulong[]{0x1FFFFFFFFFF0UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.ST.Language
