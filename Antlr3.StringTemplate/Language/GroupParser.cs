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

using HashMap = System.Collections.Hashtable;
using Map = System.Collections.IDictionary;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Antlr3.ST.Language
{
public partial class GroupParser : Parser
{
	public static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ANONYMOUS_TEMPLATE", "ANYCHAR", "ASSIGN", "AT", "BIGSTRING", "CLOSE_ANON_TEMPLATE", "CLOSE_BIG_STRING", "CLOSE_BLOCK_COMMENT", "COLON", "COMMA", "DEFINED_TO_BE", "DOT", "ID", "KWDEFAULT", "KWGROUP", "KWIMPLEMENTS", "LBRACK", "LPAREN", "ML_COMMENT", "OPTIONAL", "PLUS", "RBRACK", "RPAREN", "SEMI", "SL_COMMENT", "STAR", "STRING", "WS"
	};
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

	public GroupParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public GroupParser( ITokenStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return GroupParser.tokenNames; }
	public override string GrammarFileName { get { return "Language\\Group.g3"; } }


	#region Rules

	// $ANTLR start "group"
	// Language\\Group.g3:123:0: public group[StringTemplateGroup g] : 'group' name= ID ( COLON s= ID )? ( 'implements' i= ID ( COMMA i2= ID )* )? SEMI ( template[g] | mapdef[g] )+ EOF ;
	public void group( StringTemplateGroup g )
	{
		IToken name=null;
		IToken s=null;
		IToken i=null;
		IToken i2=null;


		this._group = g;

		try
		{
			// Language\\Group.g3:128:4: ( 'group' name= ID ( COLON s= ID )? ( 'implements' i= ID ( COMMA i2= ID )* )? SEMI ( template[g] | mapdef[g] )+ EOF )
			// Language\\Group.g3:128:4: 'group' name= ID ( COLON s= ID )? ( 'implements' i= ID ( COMMA i2= ID )* )? SEMI ( template[g] | mapdef[g] )+ EOF
			{
			Match(input,KWGROUP,Follow._KWGROUP_in_group93); 
			name=(IToken)Match(input,ID,Follow._ID_in_group97); 
			g.Name = (name!=null?name.Text:null);
			// Language\\Group.g3:129:3: ( COLON s= ID )?
			int alt1=2;
			int LA1_0 = input.LA(1);

			if ( (LA1_0==COLON) )
			{
				alt1=1;
			}
			switch ( alt1 )
			{
			case 1:
				// Language\\Group.g3:129:5: COLON s= ID
				{
				Match(input,COLON,Follow._COLON_in_group105); 
				s=(IToken)Match(input,ID,Follow._ID_in_group109); 
				g.setSuperGroup((s!=null?s.Text:null));

				}
				break;

			}

			// Language\\Group.g3:130:3: ( 'implements' i= ID ( COMMA i2= ID )* )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==KWIMPLEMENTS) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Language\\Group.g3:130:5: 'implements' i= ID ( COMMA i2= ID )*
				{
				Match(input,KWIMPLEMENTS,Follow._KWIMPLEMENTS_in_group120); 
				i=(IToken)Match(input,ID,Follow._ID_in_group124); 
				g.implementInterface((i!=null?i.Text:null));
				// Language\\Group.g3:131:4: ( COMMA i2= ID )*
				for ( ; ; )
				{
					int alt2=2;
					int LA2_0 = input.LA(1);

					if ( (LA2_0==COMMA) )
					{
						alt2=1;
					}


					switch ( alt2 )
					{
					case 1:
						// Language\\Group.g3:131:5: COMMA i2= ID
						{
						Match(input,COMMA,Follow._COMMA_in_group132); 
						i2=(IToken)Match(input,ID,Follow._ID_in_group136); 
						g.implementInterface((i2!=null?i2.Text:null));

						}
						break;

					default:
						goto loop2;
					}
				}

				loop2:
					;



				}
				break;

			}

			Match(input,SEMI,Follow._SEMI_in_group150); 
			// Language\\Group.g3:134:3: ( template[g] | mapdef[g] )+
			int cnt4=0;
			for ( ; ; )
			{
				int alt4=3;
				int LA4_0 = input.LA(1);

				if ( (LA4_0==AT) )
				{
					alt4=1;
				}
				else if ( (LA4_0==ID) )
				{
					int LA4_3 = input.LA(2);

					if ( (LA4_3==DEFINED_TO_BE) )
					{
						int LA4_4 = input.LA(3);

						if ( (LA4_4==ID) )
						{
							alt4=1;
						}
						else if ( (LA4_4==LBRACK) )
						{
							alt4=2;
						}


					}
					else if ( (LA4_3==LPAREN) )
					{
						alt4=1;
					}


				}


				switch ( alt4 )
				{
				case 1:
					// Language\\Group.g3:134:5: template[g]
					{
					PushFollow(Follow._template_in_group156);
					template(g);

					state._fsp--;


					}
					break;
				case 2:
					// Language\\Group.g3:134:19: mapdef[g]
					{
					PushFollow(Follow._mapdef_in_group161);
					mapdef(g);

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


			Match(input,EOF,Follow._EOF_in_group169); 

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
	// $ANTLR end "group"


	// $ANTLR start "template"
	// Language\\Group.g3:138:0: template[StringTemplateGroup g] : ( ( AT scope2= ID DOT region= ID |name= ID ) LPAREN ( args[st] |) RPAREN DEFINED_TO_BE (t= STRING |bt= BIGSTRING ) |alias= ID DEFINED_TO_BE target= ID );
	private void template( StringTemplateGroup g )
	{
		IToken scope2=null;
		IToken region=null;
		IToken name=null;
		IToken t=null;
		IToken bt=null;
		IToken alias=null;
		IToken target=null;


			Map formalArgs = null;
			StringTemplate st = null;
			bool ignore = false;
			string templateName=null;
			int line = input.LT(1).Line;

		try
		{
			// Language\\Group.g3:147:4: ( ( AT scope2= ID DOT region= ID |name= ID ) LPAREN ( args[st] |) RPAREN DEFINED_TO_BE (t= STRING |bt= BIGSTRING ) |alias= ID DEFINED_TO_BE target= ID )
			int alt8=2;
			int LA8_0 = input.LA(1);

			if ( (LA8_0==AT) )
			{
				alt8=1;
			}
			else if ( (LA8_0==ID) )
			{
				int LA8_2 = input.LA(2);

				if ( (LA8_2==DEFINED_TO_BE) )
				{
					alt8=2;
				}
				else if ( (LA8_2==LPAREN) )
				{
					alt8=1;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 8, 2, input);

					throw nvae;
				}
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 8, 0, input);

				throw nvae;
			}
			switch ( alt8 )
			{
			case 1:
				// Language\\Group.g3:147:4: ( AT scope2= ID DOT region= ID |name= ID ) LPAREN ( args[st] |) RPAREN DEFINED_TO_BE (t= STRING |bt= BIGSTRING )
				{
				// Language\\Group.g3:147:4: ( AT scope2= ID DOT region= ID |name= ID )
				int alt5=2;
				int LA5_0 = input.LA(1);

				if ( (LA5_0==AT) )
				{
					alt5=1;
				}
				else if ( (LA5_0==ID) )
				{
					alt5=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 5, 0, input);

					throw nvae;
				}
				switch ( alt5 )
				{
				case 1:
					// Language\\Group.g3:147:6: AT scope2= ID DOT region= ID
					{
					Match(input,AT,Follow._AT_in_template188); 
					scope2=(IToken)Match(input,ID,Follow._ID_in_template192); 
					Match(input,DOT,Follow._DOT_in_template194); 
					region=(IToken)Match(input,ID,Follow._ID_in_template198); 

									templateName=g.getMangledRegionName((scope2!=null?scope2.Text:null),(region!=null?region.Text:null));
									if ( g.isDefinedInThisGroup(templateName) )
									{
										g.error("group "+g.Name+" line "+line+": redefinition of template region: @"+
											(scope2!=null?scope2.Text:null)+"."+(region!=null?region.Text:null));
										st = new StringTemplate(); // create bogus template to fill in
									}
									else
									{
										bool err = false;
										// @template.region() ::= "..."
										StringTemplate scopeST = g.lookupTemplate((scope2!=null?scope2.Text:null));
										if ( scopeST == null )
										{
											g.error("group "+g.Name+" line "+line+": reference to region within undefined template: "+
												(scope2!=null?scope2.Text:null));
											err=true;
										}
										else if ( !scopeST.containsRegionName((region!=null?region.Text:null)) )
										{
											g.error("group "+g.Name+" line "+line+": template "+(scope2!=null?scope2.Text:null)+" has no region called "+
												(region!=null?region.Text:null));
											err=true;
										}
										if ( err )
										{
											st = new StringTemplate();
										}
										else
										{
											st = g.defineRegionTemplate((scope2!=null?scope2.Text:null),
																		(region!=null?region.Text:null),
																		null,
																		StringTemplate.REGION_EXPLICIT);
										}
									}
								

					}
					break;
				case 2:
					// Language\\Group.g3:186:5: name= ID
					{
					name=(IToken)Match(input,ID,Follow._ID_in_template211); 
					templateName = (name!=null?name.Text:null);

									if ( g.isDefinedInThisGroup(templateName) )
									{
										g.error("redefinition of template: "+templateName);
										st = new StringTemplate(); // create bogus template to fill in
									}
									else
									{
										st = g.defineTemplate(templateName, null);
									}
								

					}
					break;

				}

				if ( st!=null ) {st.setGroupFileLine(line);}
				Match(input,LPAREN,Follow._LPAREN_in_template230); 
				// Language\\Group.g3:201:4: ( args[st] |)
				int alt6=2;
				int LA6_0 = input.LA(1);

				if ( (LA6_0==ID) )
				{
					alt6=1;
				}
				else if ( (LA6_0==RPAREN) )
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
					// Language\\Group.g3:201:5: args[st]
					{
					PushFollow(Follow._args_in_template236);
					args(st);

					state._fsp--;


					}
					break;
				case 2:
					// Language\\Group.g3:201:14: 
					{
					st.defineEmptyFormalArgumentList();

					}
					break;

				}

				Match(input,RPAREN,Follow._RPAREN_in_template244); 
				Match(input,DEFINED_TO_BE,Follow._DEFINED_TO_BE_in_template248); 
				// Language\\Group.g3:204:3: (t= STRING |bt= BIGSTRING )
				int alt7=2;
				int LA7_0 = input.LA(1);

				if ( (LA7_0==STRING) )
				{
					alt7=1;
				}
				else if ( (LA7_0==BIGSTRING) )
				{
					alt7=2;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

					throw nvae;
				}
				switch ( alt7 )
				{
				case 1:
					// Language\\Group.g3:204:5: t= STRING
					{
					t=(IToken)Match(input,STRING,Follow._STRING_in_template256); 
					st.setTemplate((t!=null?t.Text:null));

					}
					break;
				case 2:
					// Language\\Group.g3:205:5: bt= BIGSTRING
					{
					bt=(IToken)Match(input,BIGSTRING,Follow._BIGSTRING_in_template270); 
					st.setTemplate((bt!=null?bt.Text:null));

					}
					break;

				}


				}
				break;
			case 2:
				// Language\\Group.g3:208:6: alias= ID DEFINED_TO_BE target= ID
				{
				alias=(IToken)Match(input,ID,Follow._ID_in_template286); 
				Match(input,DEFINED_TO_BE,Follow._DEFINED_TO_BE_in_template288); 
				target=(IToken)Match(input,ID,Follow._ID_in_template292); 
				g.defineTemplateAlias((alias!=null?alias.Text:null), (target!=null?target.Text:null));

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
	// $ANTLR end "template"


	// $ANTLR start "args"
	// Language\\Group.g3:212:0: args[StringTemplate st] : arg[st] ( COMMA arg[st] )* ;
	private void args( StringTemplate st )
	{
		try
		{
			// Language\\Group.g3:213:4: ( arg[st] ( COMMA arg[st] )* )
			// Language\\Group.g3:213:4: arg[st] ( COMMA arg[st] )*
			{
			PushFollow(Follow._arg_in_args308);
			arg(st);

			state._fsp--;

			// Language\\Group.g3:213:12: ( COMMA arg[st] )*
			for ( ; ; )
			{
				int alt9=2;
				int LA9_0 = input.LA(1);

				if ( (LA9_0==COMMA) )
				{
					alt9=1;
				}


				switch ( alt9 )
				{
				case 1:
					// Language\\Group.g3:213:14: COMMA arg[st]
					{
					Match(input,COMMA,Follow._COMMA_in_args313); 
					PushFollow(Follow._arg_in_args315);
					arg(st);

					state._fsp--;


					}
					break;

				default:
					goto loop9;
				}
			}

			loop9:
				;



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
	// $ANTLR end "args"


	// $ANTLR start "arg"
	// Language\\Group.g3:216:0: arg[StringTemplate st] : name= ID ( ASSIGN s= STRING | ASSIGN bs= ANONYMOUS_TEMPLATE )? ;
	private void arg( StringTemplate st )
	{
		IToken name=null;
		IToken s=null;
		IToken bs=null;


			StringTemplate defaultValue = null;

		try
		{
			// Language\\Group.g3:221:4: (name= ID ( ASSIGN s= STRING | ASSIGN bs= ANONYMOUS_TEMPLATE )? )
			// Language\\Group.g3:221:4: name= ID ( ASSIGN s= STRING | ASSIGN bs= ANONYMOUS_TEMPLATE )?
			{
			name=(IToken)Match(input,ID,Follow._ID_in_arg338); 
			// Language\\Group.g3:222:3: ( ASSIGN s= STRING | ASSIGN bs= ANONYMOUS_TEMPLATE )?
			int alt10=3;
			int LA10_0 = input.LA(1);

			if ( (LA10_0==ASSIGN) )
			{
				int LA10_1 = input.LA(2);

				if ( (LA10_1==STRING) )
				{
					alt10=1;
				}
				else if ( (LA10_1==ANONYMOUS_TEMPLATE) )
				{
					alt10=2;
				}
			}
			switch ( alt10 )
			{
			case 1:
				// Language\\Group.g3:222:5: ASSIGN s= STRING
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_arg344); 
				s=(IToken)Match(input,STRING,Follow._STRING_in_arg348); 

								defaultValue=new StringTemplate("$_val_$");
								defaultValue.setAttribute("_val_", (s!=null?s.Text:null));
								defaultValue.defineFormalArgument("_val_");
								defaultValue.Name = "<"+st.getName()+"'s arg "+(name!=null?name.Text:null)+" default value subtemplate>";
							

				}
				break;
			case 2:
				// Language\\Group.g3:229:5: ASSIGN bs= ANONYMOUS_TEMPLATE
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_arg359); 
				bs=(IToken)Match(input,ANONYMOUS_TEMPLATE,Follow._ANONYMOUS_TEMPLATE_in_arg363); 

								defaultValue=new StringTemplate(st.getGroup(), (bs!=null?bs.Text:null));
								defaultValue.Name = "<"+st.getName()+"'s arg "+(name!=null?name.Text:null)+" default value subtemplate>";
							

				}
				break;

			}

			st.defineFormalArgument((name!=null?name.Text:null), defaultValue);

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
	// $ANTLR end "arg"


	// $ANTLR start "mapdef"
	// Language\\Group.g3:238:0: mapdef[StringTemplateGroup g] : name= ID DEFINED_TO_BE m= map ;
	private void mapdef( StringTemplateGroup g )
	{
		IToken name=null;
		Map m = default(Map);

		try
		{
			// Language\\Group.g3:239:4: (name= ID DEFINED_TO_BE m= map )
			// Language\\Group.g3:239:4: name= ID DEFINED_TO_BE m= map
			{
			name=(IToken)Match(input,ID,Follow._ID_in_mapdef391); 
			Match(input,DEFINED_TO_BE,Follow._DEFINED_TO_BE_in_mapdef395); 
			PushFollow(Follow._map_in_mapdef399);
			m=map();

			state._fsp--;


						if ( g.getMap((name!=null?name.Text:null))!=null )
						{
							g.error("redefinition of map: "+(name!=null?name.Text:null));
						}
						else if ( g.isDefinedInThisGroup((name!=null?name.Text:null)) )
						{
							g.error("redefinition of template as map: "+(name!=null?name.Text:null));
						}
						else
						{
							g.defineMap((name!=null?name.Text:null), m);
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
	// $ANTLR end "mapdef"


	// $ANTLR start "map"
	// Language\\Group.g3:257:0: map returns [Map mapping=new HashMap()] : LBRACK mapPairs[mapping] RBRACK ;
	private Map map(  )
	{

		Map mapping = new HashMap();

		try
		{
			// Language\\Group.g3:258:6: ( LBRACK mapPairs[mapping] RBRACK )
			// Language\\Group.g3:258:6: LBRACK mapPairs[mapping] RBRACK
			{
			Match(input,LBRACK,Follow._LBRACK_in_map420); 
			PushFollow(Follow._mapPairs_in_map422);
			mapPairs(mapping);

			state._fsp--;

			Match(input,RBRACK,Follow._RBRACK_in_map425); 

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
		return mapping;
	}
	// $ANTLR end "map"


	// $ANTLR start "mapPairs"
	// Language\\Group.g3:261:0: mapPairs[Map mapping] : ( keyValuePair[mapping] ( COMMA keyValuePair[mapping] )* ( COMMA defaultValuePair[mapping] )? | defaultValuePair[mapping] );
	private void mapPairs( Map mapping )
	{
		try
		{
			// Language\\Group.g3:262:4: ( keyValuePair[mapping] ( COMMA keyValuePair[mapping] )* ( COMMA defaultValuePair[mapping] )? | defaultValuePair[mapping] )
			int alt13=2;
			int LA13_0 = input.LA(1);

			if ( (LA13_0==STRING) )
			{
				alt13=1;
			}
			else if ( (LA13_0==KWDEFAULT) )
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
				// Language\\Group.g3:262:4: keyValuePair[mapping] ( COMMA keyValuePair[mapping] )* ( COMMA defaultValuePair[mapping] )?
				{
				PushFollow(Follow._keyValuePair_in_mapPairs438);
				keyValuePair(mapping);

				state._fsp--;

				// Language\\Group.g3:262:26: ( COMMA keyValuePair[mapping] )*
				for ( ; ; )
				{
					int alt11=2;
					int LA11_0 = input.LA(1);

					if ( (LA11_0==COMMA) )
					{
						int LA11_1 = input.LA(2);

						if ( (LA11_1==STRING) )
						{
							alt11=1;
						}


					}


					switch ( alt11 )
					{
					case 1:
						// Language\\Group.g3:262:27: COMMA keyValuePair[mapping]
						{
						Match(input,COMMA,Follow._COMMA_in_mapPairs442); 
						PushFollow(Follow._keyValuePair_in_mapPairs444);
						keyValuePair(mapping);

						state._fsp--;


						}
						break;

					default:
						goto loop11;
					}
				}

				loop11:
					;


				// Language\\Group.g3:263:3: ( COMMA defaultValuePair[mapping] )?
				int alt12=2;
				int LA12_0 = input.LA(1);

				if ( (LA12_0==COMMA) )
				{
					alt12=1;
				}
				switch ( alt12 )
				{
				case 1:
					// Language\\Group.g3:263:4: COMMA defaultValuePair[mapping]
					{
					Match(input,COMMA,Follow._COMMA_in_mapPairs452); 
					PushFollow(Follow._defaultValuePair_in_mapPairs454);
					defaultValuePair(mapping);

					state._fsp--;


					}
					break;

				}


				}
				break;
			case 2:
				// Language\\Group.g3:264:4: defaultValuePair[mapping]
				{
				PushFollow(Follow._defaultValuePair_in_mapPairs462);
				defaultValuePair(mapping);

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
		return ;
	}
	// $ANTLR end "mapPairs"


	// $ANTLR start "defaultValuePair"
	// Language\\Group.g3:267:0: defaultValuePair[Map mapping] : 'default' COLON v= keyValue ;
	private void defaultValuePair( Map mapping )
	{
		StringTemplate v = default(StringTemplate);

		try
		{
			// Language\\Group.g3:268:4: ( 'default' COLON v= keyValue )
			// Language\\Group.g3:268:4: 'default' COLON v= keyValue
			{
			Match(input,KWDEFAULT,Follow._KWDEFAULT_in_defaultValuePair476); 
			Match(input,COLON,Follow._COLON_in_defaultValuePair478); 
			PushFollow(Follow._keyValue_in_defaultValuePair482);
			v=keyValue();

			state._fsp--;

			mapping[ASTExpr.DEFAULT_MAP_VALUE_NAME] = v;

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
	// $ANTLR end "defaultValuePair"


	// $ANTLR start "keyValuePair"
	// Language\\Group.g3:272:0: keyValuePair[Map mapping] : key= STRING COLON v= keyValue ;
	private void keyValuePair( Map mapping )
	{
		IToken key=null;
		StringTemplate v = default(StringTemplate);

		try
		{
			// Language\\Group.g3:273:4: (key= STRING COLON v= keyValue )
			// Language\\Group.g3:273:4: key= STRING COLON v= keyValue
			{
			key=(IToken)Match(input,STRING,Follow._STRING_in_keyValuePair500); 
			Match(input,COLON,Follow._COLON_in_keyValuePair502); 
			PushFollow(Follow._keyValue_in_keyValuePair506);
			v=keyValue();

			state._fsp--;

			mapping[(key!=null?key.Text:null)] = v;

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
	// $ANTLR end "keyValuePair"


	// $ANTLR start "keyValue"
	// Language\\Group.g3:276:0: keyValue returns [StringTemplate value=null] : (s1= BIGSTRING |s2= STRING |k= ID {...}?|);
	private StringTemplate keyValue(  )
	{

		StringTemplate value = null;

		IToken s1=null;
		IToken s2=null;
		IToken k=null;

		try
		{
			// Language\\Group.g3:277:4: (s1= BIGSTRING |s2= STRING |k= ID {...}?|)
			int alt14=4;
			switch ( input.LA(1) )
			{
			case BIGSTRING:
				{
				alt14=1;
				}
				break;
			case STRING:
				{
				alt14=2;
				}
				break;
			case ID:
				{
				alt14=3;
				}
				break;
			case COMMA:
			case RBRACK:
				{
				alt14=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

					throw nvae;
				}
			}

			switch ( alt14 )
			{
			case 1:
				// Language\\Group.g3:277:4: s1= BIGSTRING
				{
				s1=(IToken)Match(input,BIGSTRING,Follow._BIGSTRING_in_keyValue525); 
				value = new StringTemplate(_group,(s1!=null?s1.Text:null));

				}
				break;
			case 2:
				// Language\\Group.g3:278:4: s2= STRING
				{
				s2=(IToken)Match(input,STRING,Follow._STRING_in_keyValue534); 
				value = new StringTemplate(_group,(s2!=null?s2.Text:null));

				}
				break;
			case 3:
				// Language\\Group.g3:279:4: k= ID {...}?
				{
				k=(IToken)Match(input,ID,Follow._ID_in_keyValue544); 
				if ( !(((k!=null?k.Text:null) == "key")) )
				{
					throw new FailedPredicateException(input, "keyValue", "$k.text == \"key\"");
				}
				value = ASTExpr.MAP_KEY_VALUE;

				}
				break;
			case 4:
				// Language\\Group.g3:281:8: 
				{
				value = null;

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
	// $ANTLR end "keyValue"
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
		public static readonly BitSet _KWGROUP_in_group93 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_group97 = new BitSet(new ulong[]{0x8081000UL});
		public static readonly BitSet _COLON_in_group105 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_group109 = new BitSet(new ulong[]{0x8080000UL});
		public static readonly BitSet _KWIMPLEMENTS_in_group120 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_group124 = new BitSet(new ulong[]{0x8002000UL});
		public static readonly BitSet _COMMA_in_group132 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_group136 = new BitSet(new ulong[]{0x8002000UL});
		public static readonly BitSet _SEMI_in_group150 = new BitSet(new ulong[]{0x10080UL});
		public static readonly BitSet _template_in_group156 = new BitSet(new ulong[]{0x10080UL});
		public static readonly BitSet _mapdef_in_group161 = new BitSet(new ulong[]{0x10080UL});
		public static readonly BitSet _EOF_in_group169 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _AT_in_template188 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_template192 = new BitSet(new ulong[]{0x8000UL});
		public static readonly BitSet _DOT_in_template194 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_template198 = new BitSet(new ulong[]{0x200000UL});
		public static readonly BitSet _ID_in_template211 = new BitSet(new ulong[]{0x200000UL});
		public static readonly BitSet _LPAREN_in_template230 = new BitSet(new ulong[]{0x4010000UL});
		public static readonly BitSet _args_in_template236 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _RPAREN_in_template244 = new BitSet(new ulong[]{0x4000UL});
		public static readonly BitSet _DEFINED_TO_BE_in_template248 = new BitSet(new ulong[]{0x40000100UL});
		public static readonly BitSet _STRING_in_template256 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BIGSTRING_in_template270 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_template286 = new BitSet(new ulong[]{0x4000UL});
		public static readonly BitSet _DEFINED_TO_BE_in_template288 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _ID_in_template292 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _arg_in_args308 = new BitSet(new ulong[]{0x2002UL});
		public static readonly BitSet _COMMA_in_args313 = new BitSet(new ulong[]{0x10000UL});
		public static readonly BitSet _arg_in_args315 = new BitSet(new ulong[]{0x2002UL});
		public static readonly BitSet _ID_in_arg338 = new BitSet(new ulong[]{0x42UL});
		public static readonly BitSet _ASSIGN_in_arg344 = new BitSet(new ulong[]{0x40000000UL});
		public static readonly BitSet _STRING_in_arg348 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ASSIGN_in_arg359 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ANONYMOUS_TEMPLATE_in_arg363 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_mapdef391 = new BitSet(new ulong[]{0x4000UL});
		public static readonly BitSet _DEFINED_TO_BE_in_mapdef395 = new BitSet(new ulong[]{0x100000UL});
		public static readonly BitSet _map_in_mapdef399 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LBRACK_in_map420 = new BitSet(new ulong[]{0x40020000UL});
		public static readonly BitSet _mapPairs_in_map422 = new BitSet(new ulong[]{0x2000000UL});
		public static readonly BitSet _RBRACK_in_map425 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _keyValuePair_in_mapPairs438 = new BitSet(new ulong[]{0x2002UL});
		public static readonly BitSet _COMMA_in_mapPairs442 = new BitSet(new ulong[]{0x40000000UL});
		public static readonly BitSet _keyValuePair_in_mapPairs444 = new BitSet(new ulong[]{0x2002UL});
		public static readonly BitSet _COMMA_in_mapPairs452 = new BitSet(new ulong[]{0x40020000UL});
		public static readonly BitSet _defaultValuePair_in_mapPairs454 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _defaultValuePair_in_mapPairs462 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _KWDEFAULT_in_defaultValuePair476 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _COLON_in_defaultValuePair478 = new BitSet(new ulong[]{0x40010100UL});
		public static readonly BitSet _keyValue_in_defaultValuePair482 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_keyValuePair500 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _COLON_in_keyValuePair502 = new BitSet(new ulong[]{0x40010100UL});
		public static readonly BitSet _keyValue_in_keyValuePair506 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BIGSTRING_in_keyValue525 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_keyValue534 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_keyValue544 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion
}

} // namespace Antlr3.ST.Language
