// $ANTLR 3.1.2 Language\\ActionEvaluator.g3 2009-03-16 18:28:41

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
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ANONYMOUS_TEMPLATE", "APPLY", "ARGS", "ASSIGN", "COLON", "COMMA", "CONDITIONAL", "DOT", "DOTDOTDOT", "ELSEIF", "ESC_CHAR", "FIRST", "FUNCTION", "ID", "INCLUDE", "INT", "LAST", "LBRACK", "LENGTH", "LIST", "LPAREN", "MULTI_APPLY", "NESTED_ANONYMOUS_TEMPLATE", "NOT", "NOTHING", "PLUS", "RBRACK", "REST", "RPAREN", "SEMI", "SINGLEVALUEARG", "STRING", "STRIP", "SUPER", "TEMPLATE", "TEMPLATE_ARGS", "TRUNC", "VALUE", "WS", "WS_CHAR"
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
	public const int NOT=27;
	public const int NOTHING=28;
	public const int PLUS=29;
	public const int RBRACK=30;
	public const int REST=31;
	public const int RPAREN=32;
	public const int SEMI=33;
	public const int SINGLEVALUEARG=34;
	public const int STRING=35;
	public const int STRIP=36;
	public const int SUPER=37;
	public const int TEMPLATE=38;
	public const int TEMPLATE_ARGS=39;
	public const int TRUNC=40;
	public const int VALUE=41;
	public const int WS=42;
	public const int WS_CHAR=43;

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

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._expr_in_expr83);
				a=expr();

				state._fsp--;

				PushFollow(Follow._expr_in_expr87);
				b=expr();

				state._fsp--;

				value = chunk.Add(a,b);

				Match(input, TokenConstants.UP, null); 

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

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._expr_in_expr135);
				e=expr();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

							StringWriter buf = new StringWriter();
							IStringTemplateWriter sw = self.GetGroup().GetStringTemplateWriter(buf);
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

			Match(input, TokenConstants.DOWN, null); 
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



			Match(input, TokenConstants.UP, null); 
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

			Match(input, TokenConstants.DOWN, null); 
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

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._expr_in_templateInclude266);
				n=expr();

				state._fsp--;

				a2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

				Match(input, TokenConstants.UP, null); 
				if (n!=null) {name=n.ToString();} args=a2;

				}
				break;

			}


			Match(input, TokenConstants.UP, null); 

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

				Match(input, TokenConstants.DOWN, null); 
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

				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 2:
				// Language\\ActionEvaluator.g3:177:4: ^( MULTI_APPLY (a= expr )+ COLON anon= ANONYMOUS_TEMPLATE )
				{
				Match(input,MULTI_APPLY,Follow._MULTI_APPLY_in_templateApplication345); 

				Match(input, TokenConstants.DOWN, null); 
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
							

				Match(input, TokenConstants.UP, null); 

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

			Match(input, TokenConstants.DOWN, null); 
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


			Match(input, TokenConstants.UP, null); 

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

			Match(input, TokenConstants.DOWN, null); 
			PushFollow(Follow._expr_in_singleFunctionArg501);
			expr8=expr();

			state._fsp--;

			value = expr8;

			Match(input, TokenConstants.UP, null); 

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

			Match(input, TokenConstants.DOWN, null); 
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
									StringTemplateGroup group = self.GetGroup();
									StringTemplate embedded = group.GetEmbeddedInstanceOf(self, templateName);
									if ( embedded!=null )
									{
										embedded.SetArgumentsAST(args);
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
									anonymous.SetGroup(self.GetGroup());
									templatesToApply.Add(anonymous);
								

				}
				break;
			case 3:
				// Language\\ActionEvaluator.g3:232:6: ^( VALUE n= expr args2= . )
				{
				Match(input,VALUE,Follow._VALUE_in_template568); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._expr_in_template572);
				n=expr();

				state._fsp--;

				args2=(StringTemplateAST)input.LT(1);
				MatchAny(input); 

										StringTemplate embedded = null;
										if ( n!=null )
										{
											string templateName = n.ToString();
											StringTemplateGroup group = self.GetGroup();
											embedded = group.GetEmbeddedInstanceOf(self, templateName);
											if ( embedded!=null )
											{
												embedded.SetArgumentsAST(args2);
												templatesToApply.Add(embedded);
											}
										}
									

				Match(input, TokenConstants.UP, null); 

				}
				break;

			}


			Match(input, TokenConstants.UP, null); 

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

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._ifAtom_in_ifCondition629);
				a=ifAtom();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 
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

				Match(input, TokenConstants.DOWN, null); 
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

					Match(input, TokenConstants.DOWN, null); 
					PushFollow(Follow._expr_in_attribute718);
					e=expr();

					state._fsp--;


					Match(input, TokenConstants.UP, null); 
					if (e!=null) {propName=e;}

					}
					break;

				}


				Match(input, TokenConstants.UP, null); 
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
								StringTemplate valueST =new StringTemplate(self.GetGroup(), (at!=null?at.Text:null));
								valueST.SetEnclosingInstance(self);
								valueST.SetName("<anonymous template argument>");
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

				if ( input.LA(1)==TokenConstants.DOWN )
				{
					Match(input, TokenConstants.DOWN, null); 
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



					Match(input, TokenConstants.UP, null); 
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

			Match(input, TokenConstants.DOWN, null); 
			PushFollow(Follow._expr_in_singleTemplateArg840);
			e=expr();

			state._fsp--;


			Match(input, TokenConstants.UP, null); 

						if ( e!=null )
						{
							string soleArgName = null;
							// find the sole defined formal argument for embedded
							bool error = false;
							var formalArgs = embedded.GetFormalArguments();
							if ( formalArgs!=null )
							{
								var argNames = formalArgs.Select( fa => fa.name ).ToArray();
								if ( argNames.Length==1 )
								{
									soleArgName = (string)argNames.ToArray()[0];
									//System.out.println("sole formal arg of "+embedded.GetName()+" is "+soleArgName);
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
								self.Error("template "+embedded.GetName()+
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

				Match(input, TokenConstants.DOWN, null); 
				arg=(StringTemplateAST)Match(input,ID,Follow._ID_in_argumentAssignment864); 
				PushFollow(Follow._expr_in_argumentAssignment866);
				expr11=expr();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

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
	#endregion

	// Delegated rules

	#region Synpreds
	#endregion

	#region DFA

	protected override void InitDFAs()
	{
		base.InitDFAs();
	}

	#endregion

	#region Follow Sets
	public static class Follow
	{
		public static readonly BitSet _expr_in_action56 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_expr79 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_expr83 = new BitSet(new ulong[]{0x208228F0830UL});
		public static readonly BitSet _expr_in_expr87 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _templateApplication_in_expr95 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attribute_in_expr102 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _templateInclude_in_expr109 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _function_in_expr116 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _list_in_expr123 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VALUE_in_expr131 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_expr135 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LIST_in_list167 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_list174 = new BitSet(new ulong[]{0x208328F0838UL});
		public static readonly BitSet _NOTHING_in_list187 = new BitSet(new ulong[]{0x208328F0838UL});
		public static readonly BitSet _INCLUDE_in_templateInclude229 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_templateInclude242 = new BitSet(new ulong[]{0xFFFFFFFFFF0UL});
		public static readonly BitSet _VALUE_in_templateInclude262 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateInclude266 = new BitSet(new ulong[]{0xFFFFFFFFFF0UL});
		public static readonly BitSet _APPLY_in_templateApplication316 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateApplication320 = new BitSet(new ulong[]{0x4000000000UL});
		public static readonly BitSet _template_in_templateApplication326 = new BitSet(new ulong[]{0x4000000008UL});
		public static readonly BitSet _MULTI_APPLY_in_templateApplication345 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_templateApplication350 = new BitSet(new ulong[]{0x208228F0930UL});
		public static readonly BitSet _COLON_in_templateApplication357 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_templateApplication364 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FUNCTION_in_function390 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _FIRST_in_function397 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function402 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REST_in_function411 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function416 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LAST_in_function425 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function430 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LENGTH_in_function439 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function443 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRIP_in_function452 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function457 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TRUNC_in_function466 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _singleFunctionArg_in_function471 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SINGLEVALUEARG_in_singleFunctionArg499 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_singleFunctionArg501 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_template524 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_template531 = new BitSet(new ulong[]{0xFFFFFFFFFF0UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_template552 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _VALUE_in_template568 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_template572 = new BitSet(new ulong[]{0xFFFFFFFFFF0UL});
		public static readonly BitSet _ifAtom_in_ifCondition617 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_ifCondition625 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ifAtom_in_ifCondition629 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _expr_in_ifAtom647 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_attribute671 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _expr_in_attribute675 = new BitSet(new ulong[]{0x20000020000UL});
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
		public static readonly BitSet _ID_in_argumentAssignment864 = new BitSet(new ulong[]{0x208228F0830UL});
		public static readonly BitSet _expr_in_argumentAssignment866 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOTDOTDOT_in_argumentAssignment877 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion
}

} // namespace Antlr3.ST.Language
