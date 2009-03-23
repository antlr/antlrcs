// $ANTLR 3.1.2 Language\\AngleBracketTemplateLexer.g3 2009-03-23 17:53:05

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

using StringBuffer = System.Text.StringBuilder;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.ST.Language
{
public partial class AngleBracketTemplateLexer : Lexer
{
	public const int EOF=-1;
	public const int ACTION=4;
	public const int COMMENT=5;
	public const int ELSE=6;
	public const int ELSEIF=7;
	public const int ENDIF=8;
	public const int ESC=9;
	public const int ESC_CHAR=10;
	public const int EXPR=11;
	public const int HEX=12;
	public const int IF=13;
	public const int IF_EXPR=14;
	public const int INDENT=15;
	public const int LITERAL=16;
	public const int NESTED_PARENS=17;
	public const int NEWLINE=18;
	public const int REGION_DEF=19;
	public const int REGION_REF=20;
	public const int SUBTEMPLATE=21;
	public const int TEMPLATE=22;

    // delegates
    // delegators

	public AngleBracketTemplateLexer() {}
	public AngleBracketTemplateLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public AngleBracketTemplateLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Language\\AngleBracketTemplateLexer.g3"; } }

	// $ANTLR start "NEWLINE"
	private void mNEWLINE()
	{
		try
		{
			int _type = NEWLINE;
			int _channel = DefaultTokenChannel;
			// Language\\AngleBracketTemplateLexer.g3:80:4: ( ( '\\r' )? '\\n' )
			// Language\\AngleBracketTemplateLexer.g3:80:4: ( '\\r' )? '\\n'
			{
			// Language\\AngleBracketTemplateLexer.g3:80:4: ( '\\r' )?
			int alt1=2;
			int LA1_0 = input.LA(1);

			if ( (LA1_0=='\r') )
			{
				alt1=1;
			}
			switch ( alt1 )
			{
			case 1:
				// Language\\AngleBracketTemplateLexer.g3:80:5: '\\r'
				{
				Match('\r'); if (state.failed) return ;

				}
				break;

			}

			Match('\n'); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				currentIndent=null;
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "NEWLINE"

	// $ANTLR start "ACTION"
	private void mACTION()
	{
		try
		{
			int _type = ACTION;
			int _channel = DefaultTokenChannel;
			IToken exp=null;
			int ch;


				int startCol = CharPositionInLine;
				System.Text.StringBuilder buf = null;
				string subtext = string.Empty;
				char uc = '\0';
				System.Text.StringBuilder builder = null;
				bool atLeft = false;
				string t = null;

			// Language\\AngleBracketTemplateLexer.g3:95:4: (=> '<' ( ESC_CHAR[out uc] )+ '>' |=> COMMENT | ( options {k=1; } :=> '<if' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<elseif' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<else>' ( ( '\\r' )? '\\n' )? |=> '<endif>' ({...}? => ( '\\r' )? '\\n' )? |=> '<@' (ch=~ ( '>' | '(' ) )+ ( '()>' | '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )? ) | '<' EXPR[out subtext] '>' ) )
			int alt26=3;
			int LA26_0 = input.LA(1);

			if ( (LA26_0=='<') )
			{
				int LA26_1 = input.LA(2);

				if ( (synpred1_AngleBracketTemplateLexer()) )
				{
					alt26=1;
				}
				else if ( (synpred2_AngleBracketTemplateLexer()) )
				{
					alt26=2;
				}
				else if ( (true) )
				{
					alt26=3;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 26, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 26, 0, input);

				throw nvae;
			}
			switch ( alt26 )
			{
			case 1:
				// Language\\AngleBracketTemplateLexer.g3:95:4: => '<' ( ESC_CHAR[out uc] )+ '>'
				{

				if ( state.backtracking == 0 )
				{

								buf = new System.Text.StringBuilder();
								uc = '\0';
							
				}
				Match('<'); if (state.failed) return ;
				// Language\\AngleBracketTemplateLexer.g3:101:7: ( ESC_CHAR[out uc] )+
				int cnt2=0;
				for ( ; ; )
				{
					int alt2=2;
					int LA2_0 = input.LA(1);

					if ( (LA2_0=='\\') )
					{
						alt2=1;
					}


					switch ( alt2 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:101:8: ESC_CHAR[out uc]
						{
						mESC_CHAR(out uc); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							buf.Append(uc);
						}

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


				Match('>'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								Text = buf.ToString();
								_type = LITERAL;
							
				}

				}
				break;
			case 2:
				// Language\\AngleBracketTemplateLexer.g3:106:4: => COMMENT
				{

				mCOMMENT(); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					_channel = Hidden;
				}

				}
				break;
			case 3:
				// Language\\AngleBracketTemplateLexer.g3:107:4: ( options {k=1; } :=> '<if' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<elseif' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<else>' ( ( '\\r' )? '\\n' )? |=> '<endif>' ({...}? => ( '\\r' )? '\\n' )? |=> '<@' (ch=~ ( '>' | '(' ) )+ ( '()>' | '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )? ) | '<' EXPR[out subtext] '>' )
				{
				// Language\\AngleBracketTemplateLexer.g3:107:4: ( options {k=1; } :=> '<if' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<elseif' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )? |=> '<else>' ( ( '\\r' )? '\\n' )? |=> '<endif>' ({...}? => ( '\\r' )? '\\n' )? |=> '<@' (ch=~ ( '>' | '(' ) )+ ( '()>' | '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )? ) | '<' EXPR[out subtext] '>' )
				int alt25=6;
				int LA25_0 = input.LA(1);

				if ( (LA25_0=='<') )
				{
					int LA25_1 = input.LA(2);

					if ( (synpred3_AngleBracketTemplateLexer()) )
					{
						alt25=1;
					}
					else if ( (synpred4_AngleBracketTemplateLexer()) )
					{
						alt25=2;
					}
					else if ( (synpred5_AngleBracketTemplateLexer()) )
					{
						alt25=3;
					}
					else if ( (synpred6_AngleBracketTemplateLexer()) )
					{
						alt25=4;
					}
					else if ( (synpred7_AngleBracketTemplateLexer()) )
					{
						alt25=5;
					}
					else if ( (true) )
					{
						alt25=6;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 25, 1, input);

						throw nvae;
					}
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 25, 0, input);

					throw nvae;
				}
				switch ( alt25 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:108:4: => '<if' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )?
					{

					Match("<if"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:109:10: ( ' ' )*
					for ( ; ; )
					{
						int alt3=2;
						int LA3_0 = input.LA(1);

						if ( (LA3_0==' ') )
						{
							alt3=1;
						}


						switch ( alt3 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:109:11: ' '
							{
							Match(' '); if (state.failed) return ;

							}
							break;

						default:
							goto loop3;
						}
					}

					loop3:
						;


					Match('('); if (state.failed) return ;
					int expStart171 = GetCharIndex();
					mIF_EXPR(); if (state.failed) return ;
					exp = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, expStart171, GetCharIndex()-1);
					Match(")>"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:110:4: ( ( '\\r' )? '\\n' )?
					int alt5=2;
					int LA5_0 = input.LA(1);

					if ( (LA5_0=='\n'||LA5_0=='\r') )
					{
						alt5=1;
					}
					switch ( alt5 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:110:6: ( '\\r' )? '\\n'
						{
						// Language\\AngleBracketTemplateLexer.g3:110:6: ( '\\r' )?
						int alt4=2;
						int LA4_0 = input.LA(1);

						if ( (LA4_0=='\r') )
						{
							alt4=1;
						}
						switch ( alt4 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:110:7: '\\r'
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

										Text = "if(" + (exp!=null?exp.Text:null) + ")";
										_type = TemplateParser.IF;
									
					}

					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:115:5: => '<elseif' ( ' ' )* '(' exp= IF_EXPR ')>' ( ( '\\r' )? '\\n' )?
					{

					Match("<elseif"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:116:14: ( ' ' )*
					for ( ; ; )
					{
						int alt6=2;
						int LA6_0 = input.LA(1);

						if ( (LA6_0==' ') )
						{
							alt6=1;
						}


						switch ( alt6 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:116:15: ' '
							{
							Match(' '); if (state.failed) return ;

							}
							break;

						default:
							goto loop6;
						}
					}

					loop6:
						;


					Match('('); if (state.failed) return ;
					int expStart220 = GetCharIndex();
					mIF_EXPR(); if (state.failed) return ;
					exp = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, expStart220, GetCharIndex()-1);
					Match(")>"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:117:4: ( ( '\\r' )? '\\n' )?
					int alt8=2;
					int LA8_0 = input.LA(1);

					if ( (LA8_0=='\n'||LA8_0=='\r') )
					{
						alt8=1;
					}
					switch ( alt8 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:117:6: ( '\\r' )? '\\n'
						{
						// Language\\AngleBracketTemplateLexer.g3:117:6: ( '\\r' )?
						int alt7=2;
						int LA7_0 = input.LA(1);

						if ( (LA7_0=='\r') )
						{
							alt7=1;
						}
						switch ( alt7 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:117:7: '\\r'
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

										Text = "elseif(" + (exp!=null?exp.Text:null) + ")";
										_type = TemplateParser.ELSEIF;
									
					}

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:122:5: => '<else>' ( ( '\\r' )? '\\n' )?
					{

					Match("<else>"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:124:4: ( ( '\\r' )? '\\n' )?
					int alt10=2;
					int LA10_0 = input.LA(1);

					if ( (LA10_0=='\n'||LA10_0=='\r') )
					{
						alt10=1;
					}
					switch ( alt10 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:124:6: ( '\\r' )? '\\n'
						{
						// Language\\AngleBracketTemplateLexer.g3:124:6: ( '\\r' )?
						int alt9=2;
						int LA9_0 = input.LA(1);

						if ( (LA9_0=='\r') )
						{
							alt9=1;
						}
						switch ( alt9 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:124:7: '\\r'
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

										Text = "else";
										_type = TemplateParser.ELSE;
									
					}

					}
					break;
				case 4:
					// Language\\AngleBracketTemplateLexer.g3:129:5: => '<endif>' ({...}? => ( '\\r' )? '\\n' )?
					{

					Match("<endif>"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:131:4: ({...}? => ( '\\r' )? '\\n' )?
					int alt12=2;
					int LA12_0 = input.LA(1);

					if ( (LA12_0=='\n'||LA12_0=='\r') && ((startCol==0)))
					{
						alt12=1;
					}
					switch ( alt12 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:131:6: {...}? => ( '\\r' )? '\\n'
						{
						if ( !((startCol==0)) )
						{
							if (state.backtracking>0) {state.failed=true; return ;}
							throw new FailedPredicateException(input, "ACTION", "startCol==0");
						}
						// Language\\AngleBracketTemplateLexer.g3:131:24: ( '\\r' )?
						int alt11=2;
						int LA11_0 = input.LA(1);

						if ( (LA11_0=='\r') )
						{
							alt11=1;
						}
						switch ( alt11 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:131:25: '\\r'
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

										Text = "endif";
										_type = TemplateParser.ENDIF;
									
					}

					}
					break;
				case 5:
					// Language\\AngleBracketTemplateLexer.g3:138:4: => '<@' (ch=~ ( '>' | '(' ) )+ ( '()>' | '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )? )
					{

					if ( state.backtracking == 0 )
					{

										builder = new System.Text.StringBuilder();
									
					}
					Match("<@"); if (state.failed) return ;

					// Language\\AngleBracketTemplateLexer.g3:143:4: (ch=~ ( '>' | '(' ) )+
					int cnt13=0;
					for ( ; ; )
					{
						int alt13=2;
						int LA13_0 = input.LA(1);

						if ( ((LA13_0>='\u0000' && LA13_0<='\'')||(LA13_0>=')' && LA13_0<='=')||(LA13_0>='?' && LA13_0<='\uFFFF')) )
						{
							alt13=1;
						}


						switch ( alt13 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:143:6: ch=~ ( '>' | '(' )
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
							if ( cnt13 >= 1 )
								goto loop13;

							if (state.backtracking>0) {state.failed=true; return ;}
							EarlyExitException eee13 = new EarlyExitException( 13, input );
							throw eee13;
						}
						cnt13++;
					}
					loop13:
						;


					if ( state.backtracking == 0 )
					{
						 t = builder.ToString(); 
					}
					// Language\\AngleBracketTemplateLexer.g3:145:4: ( '()>' | '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )? )
					int alt24=2;
					int LA24_0 = input.LA(1);

					if ( (LA24_0=='(') )
					{
						alt24=1;
					}
					else if ( (LA24_0=='>') )
					{
						alt24=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 24, 0, input);

						throw nvae;
					}
					switch ( alt24 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:145:6: '()>'
						{
						Match("()>"); if (state.failed) return ;

						if ( state.backtracking == 0 )
						{

												_type = TemplateParser.REGION_REF;
											
						}

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:149:6: '>' (=> ( '\\r' )? '\\n' )? ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+ (=> ( '\\r' )? '\\n' )? ( '<@end>' | . ) ({...}? => ( '\\r' )? '\\n' )?
						{
						Match('>'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{

												_type = TemplateParser.REGION_DEF;
												builder.Append("::=");
											
						}
						// Language\\AngleBracketTemplateLexer.g3:154:5: (=> ( '\\r' )? '\\n' )?
						int alt15=2;
						int LA15_0 = input.LA(1);

						if ( (LA15_0=='\r') )
						{
							int LA15_1 = input.LA(2);

							if ( (LA15_1=='\n') )
							{
								int LA15_4 = input.LA(3);

								if ( (synpred8_AngleBracketTemplateLexer()) )
								{
									alt15=1;
								}
							}
						}
						else if ( (LA15_0=='\n') )
						{
							int LA15_2 = input.LA(2);

							if ( (synpred8_AngleBracketTemplateLexer()) )
							{
								alt15=1;
							}
						}
						switch ( alt15 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:154:7: => ( '\\r' )? '\\n'
							{

							// Language\\AngleBracketTemplateLexer.g3:154:22: ( '\\r' )?
							int alt14=2;
							int LA14_0 = input.LA(1);

							if ( (LA14_0=='\r') )
							{
								alt14=1;
							}
							switch ( alt14 )
							{
							case 1:
								// Language\\AngleBracketTemplateLexer.g3:154:23: '\\r'
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

												atLeft = false;
											
						}
						// Language\\AngleBracketTemplateLexer.g3:158:5: ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+
						int cnt18=0;
						for ( ; ; )
						{
							int alt18=2;
							alt18 = dfa18.Predict(input);
							switch ( alt18 )
							{
							case 1:
								// Language\\AngleBracketTemplateLexer.g3:158:7: {...}? => (=> ( '\\r' )? '\\n' |ch= . )
								{
								if ( !((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) )
								{
									if (state.backtracking>0) {state.failed=true; return ;}
									throw new FailedPredicateException(input, "ACTION", "!(UpcomingAtEND(1) || ( input.LA(1) == '\\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\\r' && input.LA(2) == '\\n' && UpcomingAtEND(3) ))");
								}
								// Language\\AngleBracketTemplateLexer.g3:159:6: (=> ( '\\r' )? '\\n' |ch= . )
								int alt17=2;
								int LA17_0 = input.LA(1);

								if ( (LA17_0=='\r') )
								{
									int LA17_1 = input.LA(2);

									if ( (synpred9_AngleBracketTemplateLexer()) )
									{
										alt17=1;
									}
									else if ( (true) )
									{
										alt17=2;
									}
									else
									{
										if (state.backtracking>0) {state.failed=true; return ;}
										NoViableAltException nvae = new NoViableAltException("", 17, 1, input);

										throw nvae;
									}
								}
								else if ( (LA17_0=='\n') )
								{
									int LA17_2 = input.LA(2);

									if ( (synpred9_AngleBracketTemplateLexer()) )
									{
										alt17=1;
									}
									else if ( (true) )
									{
										alt17=2;
									}
									else
									{
										if (state.backtracking>0) {state.failed=true; return ;}
										NoViableAltException nvae = new NoViableAltException("", 17, 2, input);

										throw nvae;
									}
								}
								else if ( ((LA17_0>='\u0000' && LA17_0<='\t')||(LA17_0>='\u000B' && LA17_0<='\f')||(LA17_0>='\u000E' && LA17_0<='\uFFFF')) )
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
									// Language\\AngleBracketTemplateLexer.g3:159:8: => ( '\\r' )? '\\n'
									{

									// Language\\AngleBracketTemplateLexer.g3:159:23: ( '\\r' )?
									int alt16=2;
									int LA16_0 = input.LA(1);

									if ( (LA16_0=='\r') )
									{
										alt16=1;
									}
									switch ( alt16 )
									{
									case 1:
										// Language\\AngleBracketTemplateLexer.g3:159:24: '\\r'
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
										builder.Append('\n'); atLeft = true;
									}

									}
									break;
								case 2:
									// Language\\AngleBracketTemplateLexer.g3:160:8: ch= .
									{
									ch = input.LA(1);
									MatchAny(); if (state.failed) return ;
									if ( state.backtracking == 0 )
									{
										builder.Append((char)ch); atLeft = false;
									}

									}
									break;

								}


								}
								break;

							default:
								if ( cnt18 >= 1 )
									goto loop18;

								if (state.backtracking>0) {state.failed=true; return ;}
								EarlyExitException eee18 = new EarlyExitException( 18, input );
								throw eee18;
							}
							cnt18++;
						}
						loop18:
							;


						// Language\\AngleBracketTemplateLexer.g3:163:5: (=> ( '\\r' )? '\\n' )?
						int alt20=2;
						int LA20_0 = input.LA(1);

						if ( (LA20_0=='\r') )
						{
							int LA20_1 = input.LA(2);

							if ( (LA20_1=='\n') )
							{
								int LA20_4 = input.LA(3);

								if ( (LA20_4=='<') && (synpred10_AngleBracketTemplateLexer()))
								{
									alt20=1;
								}
								else if ( ((LA20_4>='\u0000' && LA20_4<=';')||(LA20_4>='=' && LA20_4<='\uFFFF')) && (synpred10_AngleBracketTemplateLexer()))
								{
									alt20=1;
								}
							}
						}
						else if ( (LA20_0=='\n') )
						{
							int LA20_2 = input.LA(2);

							if ( (LA20_2=='<') && (synpred10_AngleBracketTemplateLexer()))
							{
								alt20=1;
							}
							else if ( (LA20_2=='\r') )
							{
								int LA20_6 = input.LA(3);

								if ( (synpred10_AngleBracketTemplateLexer()) )
								{
									alt20=1;
								}
							}
							else if ( (LA20_2=='\n') )
							{
								int LA20_7 = input.LA(3);

								if ( (synpred10_AngleBracketTemplateLexer()) )
								{
									alt20=1;
								}
							}
							else if ( ((LA20_2>='\u0000' && LA20_2<='\t')||(LA20_2>='\u000B' && LA20_2<='\f')||(LA20_2>='\u000E' && LA20_2<=';')||(LA20_2>='=' && LA20_2<='\uFFFF')) && (synpred10_AngleBracketTemplateLexer()))
							{
								alt20=1;
							}
						}
						switch ( alt20 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:163:7: => ( '\\r' )? '\\n'
							{

							// Language\\AngleBracketTemplateLexer.g3:163:22: ( '\\r' )?
							int alt19=2;
							int LA19_0 = input.LA(1);

							if ( (LA19_0=='\r') )
							{
								alt19=1;
							}
							switch ( alt19 )
							{
							case 1:
								// Language\\AngleBracketTemplateLexer.g3:163:23: '\\r'
								{
								Match('\r'); if (state.failed) return ;

								}
								break;

							}

							Match('\n'); if (state.failed) return ;
							if ( state.backtracking == 0 )
							{
								atLeft = true;
							}

							}
							break;

						}

						// Language\\AngleBracketTemplateLexer.g3:164:5: ( '<@end>' | . )
						int alt21=2;
						int LA21_0 = input.LA(1);

						if ( (LA21_0=='<') )
						{
							int LA21_1 = input.LA(2);

							if ( (LA21_1=='@') )
							{
								alt21=1;
							}
							else
							{
								alt21=2;}
						}
						else if ( ((LA21_0>='\u0000' && LA21_0<=';')||(LA21_0>='=' && LA21_0<='\uFFFF')) )
						{
							alt21=2;
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return ;}
							NoViableAltException nvae = new NoViableAltException("", 21, 0, input);

							throw nvae;
						}
						switch ( alt21 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:164:7: '<@end>'
							{
							Match("<@end>"); if (state.failed) return ;


							}
							break;
						case 2:
							// Language\\AngleBracketTemplateLexer.g3:165:7: .
							{
							MatchAny(); if (state.failed) return ;
							if ( state.backtracking == 0 )
							{
								self.Error("missing region "+t+" <@end> tag");
							}

							}
							break;

						}

						// Language\\AngleBracketTemplateLexer.g3:167:5: ({...}? => ( '\\r' )? '\\n' )?
						int alt23=2;
						int LA23_0 = input.LA(1);

						if ( (LA23_0=='\n'||LA23_0=='\r') && ((atLeft)))
						{
							alt23=1;
						}
						switch ( alt23 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:167:7: {...}? => ( '\\r' )? '\\n'
							{
							if ( !((atLeft)) )
							{
								if (state.backtracking>0) {state.failed=true; return ;}
								throw new FailedPredicateException(input, "ACTION", "atLeft");
							}
							// Language\\AngleBracketTemplateLexer.g3:167:20: ( '\\r' )?
							int alt22=2;
							int LA22_0 = input.LA(1);

							if ( (LA22_0=='\r') )
							{
								alt22=1;
							}
							switch ( alt22 )
							{
							case 1:
								// Language\\AngleBracketTemplateLexer.g3:167:21: '\\r'
								{
								Match('\r'); if (state.failed) return ;

								}
								break;

							}

							Match('\n'); if (state.failed) return ;

							}
							break;

						}


						}
						break;

					}

					if ( state.backtracking == 0 )
					{

										Text = builder.ToString();
									
					}

					}
					break;
				case 6:
					// Language\\AngleBracketTemplateLexer.g3:172:5: '<' EXPR[out subtext] '>'
					{
					Match('<'); if (state.failed) return ;
					mEXPR(out subtext); if (state.failed) return ;
					Match('>'); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{
						 Text = subtext; 
					}

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

								//ChunkToken t = new ChunkToken(_type, Text, currentIndent);
								//state.token = t; //$ setToken(t);
								state.token = new ChunkToken(_type, Text, currentIndent);
							
				}

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION"

	// $ANTLR start "LITERAL"
	private void mLITERAL()
	{
		try
		{
			int _type = LITERAL;
			int _channel = DefaultTokenChannel;
			IToken ind=null;
			int ch;


				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				int loopStartIndex = 0;
				int col = 0;

			// Language\\AngleBracketTemplateLexer.g3:189:4: ( ( ( '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) ) ) |ind= INDENT |ch=~ ( '\\\\' | ' ' | '\\t' | '<' | '\\r' | '\\n' ) )+ )
			// Language\\AngleBracketTemplateLexer.g3:189:4: ( ( '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) ) ) |ind= INDENT |ch=~ ( '\\\\' | ' ' | '\\t' | '<' | '\\r' | '\\n' ) )+
			{
			// Language\\AngleBracketTemplateLexer.g3:189:4: ( ( '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) ) ) |ind= INDENT |ch=~ ( '\\\\' | ' ' | '\\t' | '<' | '\\r' | '\\n' ) )+
			int cnt28=0;
			for ( ; ; )
			{
				int alt28=4;
				int LA28_0 = input.LA(1);

				if ( (LA28_0=='\\') )
				{
					alt28=1;
				}
				else if ( (LA28_0=='\t'||LA28_0==' ') )
				{
					alt28=2;
				}
				else if ( ((LA28_0>='\u0000' && LA28_0<='\b')||(LA28_0>='\u000B' && LA28_0<='\f')||(LA28_0>='\u000E' && LA28_0<='\u001F')||(LA28_0>='!' && LA28_0<=';')||(LA28_0>='=' && LA28_0<='[')||(LA28_0>=']' && LA28_0<='\uFFFF')) )
				{
					alt28=3;
				}


				switch ( alt28 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:189:6: ( '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) ) )
					{
					// Language\\AngleBracketTemplateLexer.g3:189:6: ( '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) ) )
					// Language\\AngleBracketTemplateLexer.g3:189:8: '\\\\' ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) )
					{
					Match('\\'); if (state.failed) return ;
					// Language\\AngleBracketTemplateLexer.g3:190:5: ( '<' | '>' | '\\\\' |ch=~ ( '<' | '>' | '\\\\' ) )
					int alt27=4;
					int LA27_0 = input.LA(1);

					if ( (LA27_0=='<') )
					{
						alt27=1;
					}
					else if ( (LA27_0=='>') )
					{
						alt27=2;
					}
					else if ( (LA27_0=='\\') )
					{
						alt27=3;
					}
					else if ( ((LA27_0>='\u0000' && LA27_0<=';')||LA27_0=='='||(LA27_0>='?' && LA27_0<='[')||(LA27_0>=']' && LA27_0<='\uFFFF')) )
					{
						alt27=4;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 27, 0, input);

						throw nvae;
					}
					switch ( alt27 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:190:7: '<'
						{
						Match('<'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							 builder.Append('<'); 
						}

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:191:7: '>'
						{
						Match('>'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							 builder.Append('>'); 
						}

						}
						break;
					case 3:
						// Language\\AngleBracketTemplateLexer.g3:192:7: '\\\\'
						{
						Match('\\'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							 builder.Append('\\'); 
						}

						}
						break;
					case 4:
						// Language\\AngleBracketTemplateLexer.g3:193:7: ch=~ ( '<' | '>' | '\\\\' )
						{
						ch= input.LA(1);
						input.Consume();
						state.failed=false;
						if ( state.backtracking == 0 )
						{
							 builder.Append( "\\" + (char)ch ); 
						}

						}
						break;

					}


					}


					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:196:5: ind= INDENT
					{
					int indStart686 = GetCharIndex();
					mINDENT(); if (state.failed) return ;
					ind = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, indStart686, GetCharIndex()-1);
					if ( state.backtracking == 0 )
					{

										loopStartIndex = builder.Length;
										col = CharPositionInLine - (ind!=null?ind.Text:null).Length;

										builder.Append( (ind!=null?ind.Text:null) );
										if ( col==0 && input.LA(1)=='<' )
										{
											// store indent in ASTExpr not in a literal
											currentIndent=(ind!=null?ind.Text:null);
											builder.Length = loopStartIndex; // reset length to wack text
										}
										else
										{
											currentIndent=null;
										}
									
					}

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:213:5: ch=~ ( '\\\\' | ' ' | '\\t' | '<' | '\\r' | '\\n' )
					{
					ch= input.LA(1);
					input.Consume();
					state.failed=false;
					if ( state.backtracking == 0 )
					{
						 builder.Append( (char)ch ); 
					}

					}
					break;

				default:
					if ( cnt28 >= 1 )
						goto loop28;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee28 = new EarlyExitException( 28, input );
					throw eee28;
				}
				cnt28++;
			}
			loop28:
				;


			if ( state.backtracking == 0 )
			{

							Text = builder.ToString();
							if ( Text.Length == 0 )
							{
								_channel = Hidden;
							}
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LITERAL"

	// $ANTLR start "INDENT"
	private void mINDENT()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:226:4: ( ( ' ' | '\\t' )+ )
			// Language\\AngleBracketTemplateLexer.g3:226:4: ( ' ' | '\\t' )+
			{
			// Language\\AngleBracketTemplateLexer.g3:226:4: ( ' ' | '\\t' )+
			int cnt29=0;
			for ( ; ; )
			{
				int alt29=2;
				int LA29_0 = input.LA(1);

				if ( (LA29_0=='\t'||LA29_0==' ') )
				{
					alt29=1;
				}


				switch ( alt29 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt29 >= 1 )
						goto loop29;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee29 = new EarlyExitException( 29, input );
					throw eee29;
				}
				cnt29++;
			}
			loop29:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "INDENT"

	// $ANTLR start "EXPR"
	private void mEXPR(out string _text)
	{
		try
		{
			IToken st=null;
			IToken ESC1=null;
			int ch;


				string subtext = string.Empty;
				_text = string.Empty;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\AngleBracketTemplateLexer.g3:237:4: ( ( ESC |st= SUBTEMPLATE | ( '=' | '+' ) ( TEMPLATE[out subtext] |st= SUBTEMPLATE |ch=~ ( '\"' | '<' | '{' ) ) |ch=~ ( '\\\\' | '{' | '=' | '+' | '>' ) )+ )
			// Language\\AngleBracketTemplateLexer.g3:237:4: ( ESC |st= SUBTEMPLATE | ( '=' | '+' ) ( TEMPLATE[out subtext] |st= SUBTEMPLATE |ch=~ ( '\"' | '<' | '{' ) ) |ch=~ ( '\\\\' | '{' | '=' | '+' | '>' ) )+
			{
			// Language\\AngleBracketTemplateLexer.g3:237:4: ( ESC |st= SUBTEMPLATE | ( '=' | '+' ) ( TEMPLATE[out subtext] |st= SUBTEMPLATE |ch=~ ( '\"' | '<' | '{' ) ) |ch=~ ( '\\\\' | '{' | '=' | '+' | '>' ) )+
			int cnt32=0;
			for ( ; ; )
			{
				int alt32=5;
				int LA32_0 = input.LA(1);

				if ( (LA32_0=='\\') )
				{
					alt32=1;
				}
				else if ( (LA32_0=='{') )
				{
					alt32=2;
				}
				else if ( (LA32_0=='+'||LA32_0=='=') )
				{
					alt32=3;
				}
				else if ( ((LA32_0>='\u0000' && LA32_0<='*')||(LA32_0>=',' && LA32_0<='<')||(LA32_0>='?' && LA32_0<='[')||(LA32_0>=']' && LA32_0<='z')||(LA32_0>='|' && LA32_0<='\uFFFF')) )
				{
					alt32=4;
				}


				switch ( alt32 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:237:6: ESC
					{
					int ESC1Start766 = GetCharIndex();
					mESC(); if (state.failed) return ;
					ESC1 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ESC1Start766, GetCharIndex()-1);
					if ( state.backtracking == 0 )
					{
						builder.Append((ESC1!=null?ESC1.Text:null));
					}

					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:238:5: st= SUBTEMPLATE
					{
					int stStart784 = GetCharIndex();
					mSUBTEMPLATE(); if (state.failed) return ;
					st = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, stStart784, GetCharIndex()-1);
					if ( state.backtracking == 0 )
					{
						builder.Append((st!=null?st.Text:null));
					}

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:239:5: ( '=' | '+' ) ( TEMPLATE[out subtext] |st= SUBTEMPLATE |ch=~ ( '\"' | '<' | '{' ) )
					{
					// Language\\AngleBracketTemplateLexer.g3:239:5: ( '=' | '+' )
					int alt30=2;
					int LA30_0 = input.LA(1);

					if ( (LA30_0=='=') )
					{
						alt30=1;
					}
					else if ( (LA30_0=='+') )
					{
						alt30=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 30, 0, input);

						throw nvae;
					}
					switch ( alt30 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:239:7: '='
						{
						Match('='); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							builder.Append('=');
						}

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:240:6: '+'
						{
						Match('+'); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							builder.Append('+');
						}

						}
						break;

					}

					// Language\\AngleBracketTemplateLexer.g3:242:4: ( TEMPLATE[out subtext] |st= SUBTEMPLATE |ch=~ ( '\"' | '<' | '{' ) )
					int alt31=3;
					int LA31_0 = input.LA(1);

					if ( (LA31_0=='\"'||LA31_0=='<') )
					{
						alt31=1;
					}
					else if ( (LA31_0=='{') )
					{
						alt31=2;
					}
					else if ( ((LA31_0>='\u0000' && LA31_0<='!')||(LA31_0>='#' && LA31_0<=';')||(LA31_0>='=' && LA31_0<='z')||(LA31_0>='|' && LA31_0<='\uFFFF')) )
					{
						alt31=3;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 31, 0, input);

						throw nvae;
					}
					switch ( alt31 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:242:6: TEMPLATE[out subtext]
						{
						mTEMPLATE(out subtext); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{
							builder.Append(subtext);
						}

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:243:6: st= SUBTEMPLATE
						{
						int stStart850 = GetCharIndex();
						mSUBTEMPLATE(); if (state.failed) return ;
						st = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, stStart850, GetCharIndex()-1);
						if ( state.backtracking == 0 )
						{
							builder.Append((st!=null?st.Text:null));
						}

						}
						break;
					case 3:
						// Language\\AngleBracketTemplateLexer.g3:244:6: ch=~ ( '\"' | '<' | '{' )
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

					}


					}
					break;
				case 4:
					// Language\\AngleBracketTemplateLexer.g3:246:5: ch=~ ( '\\\\' | '{' | '=' | '+' | '>' )
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
					if ( cnt32 >= 1 )
						goto loop32;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee32 = new EarlyExitException( 32, input );
					throw eee32;
				}
				cnt32++;
			}
			loop32:
				;


			if ( state.backtracking == 0 )
			{
				_text = builder.ToString();
			}

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "EXPR"

	// $ANTLR start "TEMPLATE"
	private void mTEMPLATE(out string _text)
	{
		try
		{
			IToken ESC2=null;
			int ch;


				_text = string.Empty;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

			// Language\\AngleBracketTemplateLexer.g3:258:4: ( '\"' ( ESC |ch=~ ( '\\\\' | '\"' ) )* '\"' | '<<' (=> ( '\\r' )? '\\n' )? ( options {k=2; } :=> ( '\\r' )? '\\n' |=>ch= . )* '>>' )
			int alt38=2;
			int LA38_0 = input.LA(1);

			if ( (LA38_0=='\"') )
			{
				alt38=1;
			}
			else if ( (LA38_0=='<') )
			{
				alt38=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 38, 0, input);

				throw nvae;
			}
			switch ( alt38 )
			{
			case 1:
				// Language\\AngleBracketTemplateLexer.g3:258:4: '\"' ( ESC |ch=~ ( '\\\\' | '\"' ) )* '\"'
				{
				Match('\"'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					builder.Append('"');
				}
				// Language\\AngleBracketTemplateLexer.g3:260:3: ( ESC |ch=~ ( '\\\\' | '\"' ) )*
				for ( ; ; )
				{
					int alt33=3;
					int LA33_0 = input.LA(1);

					if ( (LA33_0=='\\') )
					{
						alt33=1;
					}
					else if ( ((LA33_0>='\u0000' && LA33_0<='!')||(LA33_0>='#' && LA33_0<='[')||(LA33_0>=']' && LA33_0<='\uFFFF')) )
					{
						alt33=2;
					}


					switch ( alt33 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:260:5: ESC
						{
						int ESC2Start943 = GetCharIndex();
						mESC(); if (state.failed) return ;
						ESC2 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ESC2Start943, GetCharIndex()-1);
						if ( state.backtracking == 0 )
						{
							builder.Append((ESC2!=null?ESC2.Text:null));
						}

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:261:5: ch=~ ( '\\\\' | '\"' )
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
						goto loop33;
					}
				}

				loop33:
					;


				Match('\"'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								builder.Append('"');
								_text = builder.ToString();
							
				}

				}
				break;
			case 2:
				// Language\\AngleBracketTemplateLexer.g3:268:4: '<<' (=> ( '\\r' )? '\\n' )? ( options {k=2; } :=> ( '\\r' )? '\\n' |=>ch= . )* '>>'
				{
				Match("<<"); if (state.failed) return ;

				if ( state.backtracking == 0 )
				{

								builder.Append("<<");
							
				}
				// Language\\AngleBracketTemplateLexer.g3:272:3: (=> ( '\\r' )? '\\n' )?
				int alt35=2;
				int LA35_0 = input.LA(1);

				if ( (LA35_0=='\r') )
				{
					int LA35_1 = input.LA(2);

					if ( (LA35_1=='\n') )
					{
						int LA35_2 = input.LA(3);

						if ( (synpred11_AngleBracketTemplateLexer()) )
						{
							alt35=1;
						}
					}
				}
				else if ( (LA35_0=='\n') )
				{
					int LA35_2 = input.LA(2);

					if ( (synpred11_AngleBracketTemplateLexer()) )
					{
						alt35=1;
					}
				}
				switch ( alt35 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:272:5: => ( '\\r' )? '\\n'
					{

					// Language\\AngleBracketTemplateLexer.g3:272:20: ( '\\r' )?
					int alt34=2;
					int LA34_0 = input.LA(1);

					if ( (LA34_0=='\r') )
					{
						alt34=1;
					}
					switch ( alt34 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:272:21: '\\r'
						{
						Match('\r'); if (state.failed) return ;

						}
						break;

					}

					Match('\n'); if (state.failed) return ;

					}
					break;

				}

				// Language\\AngleBracketTemplateLexer.g3:273:3: ( options {k=2; } :=> ( '\\r' )? '\\n' |=>ch= . )*
				for ( ; ; )
				{
					int alt37=3;
					alt37 = dfa37.Predict(input);
					switch ( alt37 )
					{
					case 1:
						// Language\\AngleBracketTemplateLexer.g3:274:4: => ( '\\r' )? '\\n'
						{

						// Language\\AngleBracketTemplateLexer.g3:274:22: ( '\\r' )?
						int alt36=2;
						int LA36_0 = input.LA(1);

						if ( (LA36_0=='\r') )
						{
							alt36=1;
						}
						switch ( alt36 )
						{
						case 1:
							// Language\\AngleBracketTemplateLexer.g3:274:23: '\\r'
							{
							Match('\r'); if (state.failed) return ;

							}
							break;

						}

						Match('\n'); if (state.failed) return ;

						}
						break;
					case 2:
						// Language\\AngleBracketTemplateLexer.g3:275:5: =>ch= .
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
						goto loop37;
					}
				}

				loop37:
					;


				Match(">>"); if (state.failed) return ;

				if ( state.backtracking == 0 )
				{

								builder.Append(">>");
								_text = builder.ToString();
							
				}

				}
				break;

			}
		}
		finally
		{
		}
	}
	// $ANTLR end "TEMPLATE"

	// $ANTLR start "IF_EXPR"
	private void mIF_EXPR()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:286:4: ( ( ESC | SUBTEMPLATE | NESTED_PARENS |~ ( '\\\\' | '{' | '(' | ')' ) )+ )
			// Language\\AngleBracketTemplateLexer.g3:286:4: ( ESC | SUBTEMPLATE | NESTED_PARENS |~ ( '\\\\' | '{' | '(' | ')' ) )+
			{
			// Language\\AngleBracketTemplateLexer.g3:286:4: ( ESC | SUBTEMPLATE | NESTED_PARENS |~ ( '\\\\' | '{' | '(' | ')' ) )+
			int cnt39=0;
			for ( ; ; )
			{
				int alt39=5;
				int LA39_0 = input.LA(1);

				if ( (LA39_0=='\\') )
				{
					alt39=1;
				}
				else if ( (LA39_0=='{') )
				{
					alt39=2;
				}
				else if ( (LA39_0=='(') )
				{
					alt39=3;
				}
				else if ( ((LA39_0>='\u0000' && LA39_0<='\'')||(LA39_0>='*' && LA39_0<='[')||(LA39_0>=']' && LA39_0<='z')||(LA39_0>='|' && LA39_0<='\uFFFF')) )
				{
					alt39=4;
				}


				switch ( alt39 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:286:6: ESC
					{
					mESC(); if (state.failed) return ;

					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:287:5: SUBTEMPLATE
					{
					mSUBTEMPLATE(); if (state.failed) return ;

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:288:5: NESTED_PARENS
					{
					mNESTED_PARENS(); if (state.failed) return ;

					}
					break;
				case 4:
					// Language\\AngleBracketTemplateLexer.g3:289:5: ~ ( '\\\\' | '{' | '(' | ')' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt39 >= 1 )
						goto loop39;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee39 = new EarlyExitException( 39, input );
					throw eee39;
				}
				cnt39++;
			}
			loop39:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "IF_EXPR"

	// $ANTLR start "ESC_CHAR"
	private void mESC_CHAR(out char uc)
	{
		try
		{
			IToken a=null;
			IToken b=null;
			IToken c=null;
			IToken d=null;


				uc = '\0';

			// Language\\AngleBracketTemplateLexer.g3:299:4: ( '\\\\' ( 'n' | 'r' | 't' | ' ' | 'u' a= HEX b= HEX c= HEX d= HEX ) )
			// Language\\AngleBracketTemplateLexer.g3:299:4: '\\\\' ( 'n' | 'r' | 't' | ' ' | 'u' a= HEX b= HEX c= HEX d= HEX )
			{
			Match('\\'); if (state.failed) return ;
			// Language\\AngleBracketTemplateLexer.g3:300:3: ( 'n' | 'r' | 't' | ' ' | 'u' a= HEX b= HEX c= HEX d= HEX )
			int alt40=5;
			switch ( input.LA(1) )
			{
			case 'n':
				{
				alt40=1;
				}
				break;
			case 'r':
				{
				alt40=2;
				}
				break;
			case 't':
				{
				alt40=3;
				}
				break;
			case ' ':
				{
				alt40=4;
				}
				break;
			case 'u':
				{
				alt40=5;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 40, 0, input);

					throw nvae;
				}
			}

			switch ( alt40 )
			{
			case 1:
				// Language\\AngleBracketTemplateLexer.g3:300:5: 'n'
				{
				Match('n'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					uc = '\n';
				}

				}
				break;
			case 2:
				// Language\\AngleBracketTemplateLexer.g3:301:5: 'r'
				{
				Match('r'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					uc = '\r';
				}

				}
				break;
			case 3:
				// Language\\AngleBracketTemplateLexer.g3:302:5: 't'
				{
				Match('t'); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					uc = '\t';
				}

				}
				break;
			case 4:
				// Language\\AngleBracketTemplateLexer.g3:303:5: ' '
				{
				Match(' '); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					uc = ' ';
				}

				}
				break;
			case 5:
				// Language\\AngleBracketTemplateLexer.g3:304:5: 'u' a= HEX b= HEX c= HEX d= HEX
				{
				Match('u'); if (state.failed) return ;
				int aStart1191 = GetCharIndex();
				mHEX(); if (state.failed) return ;
				a = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, aStart1191, GetCharIndex()-1);
				int bStart1195 = GetCharIndex();
				mHEX(); if (state.failed) return ;
				b = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, bStart1195, GetCharIndex()-1);
				int cStart1199 = GetCharIndex();
				mHEX(); if (state.failed) return ;
				c = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, cStart1199, GetCharIndex()-1);
				int dStart1203 = GetCharIndex();
				mHEX(); if (state.failed) return ;
				d = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, dStart1203, GetCharIndex()-1);
				if ( state.backtracking == 0 )
				{
					 uc = (char)int.Parse((a!=null?a.Text:null)+(b!=null?b.Text:null)+(c!=null?c.Text:null)+(d!=null?d.Text:null), System.Globalization.NumberStyles.AllowHexSpecifier); 
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

	// $ANTLR start "ESC"
	private void mESC()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:312:4: ( '\\\\' . )
			// Language\\AngleBracketTemplateLexer.g3:312:4: '\\\\' .
			{
			Match('\\'); if (state.failed) return ;
			MatchAny(); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ESC"

	// $ANTLR start "HEX"
	private void mHEX()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:317:4: ( '0' .. '9' | 'A' .. 'F' | 'a' .. 'f' )
			// Language\\AngleBracketTemplateLexer.g3:
			{
			if ( (input.LA(1)>='0' && input.LA(1)<='9')||(input.LA(1)>='A' && input.LA(1)<='F')||(input.LA(1)>='a' && input.LA(1)<='f') )
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
		finally
		{
		}
	}
	// $ANTLR end "HEX"

	// $ANTLR start "SUBTEMPLATE"
	private void mSUBTEMPLATE()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:322:4: ( '{' ( SUBTEMPLATE | ESC |~ ( '{' | '\\\\' | '}' ) )* '}' )
			// Language\\AngleBracketTemplateLexer.g3:322:4: '{' ( SUBTEMPLATE | ESC |~ ( '{' | '\\\\' | '}' ) )* '}'
			{
			Match('{'); if (state.failed) return ;
			// Language\\AngleBracketTemplateLexer.g3:323:3: ( SUBTEMPLATE | ESC |~ ( '{' | '\\\\' | '}' ) )*
			for ( ; ; )
			{
				int alt41=4;
				int LA41_0 = input.LA(1);

				if ( (LA41_0=='{') )
				{
					alt41=1;
				}
				else if ( (LA41_0=='\\') )
				{
					alt41=2;
				}
				else if ( ((LA41_0>='\u0000' && LA41_0<='[')||(LA41_0>=']' && LA41_0<='z')||LA41_0=='|'||(LA41_0>='~' && LA41_0<='\uFFFF')) )
				{
					alt41=3;
				}


				switch ( alt41 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:323:5: SUBTEMPLATE
					{
					mSUBTEMPLATE(); if (state.failed) return ;

					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:324:5: ESC
					{
					mESC(); if (state.failed) return ;

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:325:5: ~ ( '{' | '\\\\' | '}' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop41;
				}
			}

			loop41:
				;


			Match('}'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "SUBTEMPLATE"

	// $ANTLR start "NESTED_PARENS"
	private void mNESTED_PARENS()
	{
		try
		{
			// Language\\AngleBracketTemplateLexer.g3:332:4: ( '(' ( NESTED_PARENS | ESC |~ ( '(' | '\\\\' | ')' ) )+ ')' )
			// Language\\AngleBracketTemplateLexer.g3:332:4: '(' ( NESTED_PARENS | ESC |~ ( '(' | '\\\\' | ')' ) )+ ')'
			{
			Match('('); if (state.failed) return ;
			// Language\\AngleBracketTemplateLexer.g3:333:3: ( NESTED_PARENS | ESC |~ ( '(' | '\\\\' | ')' ) )+
			int cnt42=0;
			for ( ; ; )
			{
				int alt42=4;
				int LA42_0 = input.LA(1);

				if ( (LA42_0=='(') )
				{
					alt42=1;
				}
				else if ( (LA42_0=='\\') )
				{
					alt42=2;
				}
				else if ( ((LA42_0>='\u0000' && LA42_0<='\'')||(LA42_0>='*' && LA42_0<='[')||(LA42_0>=']' && LA42_0<='\uFFFF')) )
				{
					alt42=3;
				}


				switch ( alt42 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:333:5: NESTED_PARENS
					{
					mNESTED_PARENS(); if (state.failed) return ;

					}
					break;
				case 2:
					// Language\\AngleBracketTemplateLexer.g3:334:5: ESC
					{
					mESC(); if (state.failed) return ;

					}
					break;
				case 3:
					// Language\\AngleBracketTemplateLexer.g3:335:5: ~ ( '(' | '\\\\' | ')' )
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt42 >= 1 )
						goto loop42;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee42 = new EarlyExitException( 42, input );
					throw eee42;
				}
				cnt42++;
			}
			loop42:
				;


			Match(')'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "NESTED_PARENS"

	// $ANTLR start "COMMENT"
	private void mCOMMENT()
	{
		try
		{

			    int startCol = CharPositionInLine;

			// Language\\AngleBracketTemplateLexer.g3:346:4: ( '<!' ( . )* '!>' ({...}? => ( '\\r' )? '\\n' )? )
			// Language\\AngleBracketTemplateLexer.g3:346:4: '<!' ( . )* '!>' ({...}? => ( '\\r' )? '\\n' )?
			{
			Match("<!"); if (state.failed) return ;

			// Language\\AngleBracketTemplateLexer.g3:346:9: ( . )*
			for ( ; ; )
			{
				int alt43=2;
				int LA43_0 = input.LA(1);

				if ( (LA43_0=='!') )
				{
					int LA43_1 = input.LA(2);

					if ( (LA43_1=='>') )
					{
						alt43=2;
					}
					else if ( ((LA43_1>='\u0000' && LA43_1<='=')||(LA43_1>='?' && LA43_1<='\uFFFF')) )
					{
						alt43=1;
					}


				}
				else if ( ((LA43_0>='\u0000' && LA43_0<=' ')||(LA43_0>='\"' && LA43_0<='\uFFFF')) )
				{
					alt43=1;
				}


				switch ( alt43 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:346:0: .
					{
					MatchAny(); if (state.failed) return ;

					}
					break;

				default:
					goto loop43;
				}
			}

			loop43:
				;


			Match("!>"); if (state.failed) return ;

			// Language\\AngleBracketTemplateLexer.g3:347:3: ({...}? => ( '\\r' )? '\\n' )?
			int alt45=2;
			int LA45_0 = input.LA(1);

			if ( (LA45_0=='\n'||LA45_0=='\r') && ((startCol==0)))
			{
				alt45=1;
			}
			switch ( alt45 )
			{
			case 1:
				// Language\\AngleBracketTemplateLexer.g3:347:5: {...}? => ( '\\r' )? '\\n'
				{
				if ( !((startCol==0)) )
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					throw new FailedPredicateException(input, "COMMENT", "startCol==0");
				}
				// Language\\AngleBracketTemplateLexer.g3:347:23: ( '\\r' )?
				int alt44=2;
				int LA44_0 = input.LA(1);

				if ( (LA44_0=='\r') )
				{
					alt44=1;
				}
				switch ( alt44 )
				{
				case 1:
					// Language\\AngleBracketTemplateLexer.g3:347:24: '\\r'
					{
					Match('\r'); if (state.failed) return ;

					}
					break;

				}

				Match('\n'); if (state.failed) return ;

				}
				break;

			}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "COMMENT"

	public override void mTokens()
	{
		// Language\\AngleBracketTemplateLexer.g3:1:10: ( NEWLINE | ACTION | LITERAL )
		int alt46=3;
		int LA46_0 = input.LA(1);

		if ( (LA46_0=='\n'||LA46_0=='\r') )
		{
			alt46=1;
		}
		else if ( (LA46_0=='<') )
		{
			alt46=2;
		}
		else if ( ((LA46_0>='\u0000' && LA46_0<='\t')||(LA46_0>='\u000B' && LA46_0<='\f')||(LA46_0>='\u000E' && LA46_0<=';')||(LA46_0>='=' && LA46_0<='\uFFFF')) )
		{
			alt46=3;
		}
		else
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			NoViableAltException nvae = new NoViableAltException("", 46, 0, input);

			throw nvae;
		}
		switch ( alt46 )
		{
		case 1:
			// Language\\AngleBracketTemplateLexer.g3:1:10: NEWLINE
			{
			mNEWLINE(); if (state.failed) return ;

			}
			break;
		case 2:
			// Language\\AngleBracketTemplateLexer.g3:1:18: ACTION
			{
			mACTION(); if (state.failed) return ;

			}
			break;
		case 3:
			// Language\\AngleBracketTemplateLexer.g3:1:25: LITERAL
			{
			mLITERAL(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_AngleBracketTemplateLexer
	public void synpred1_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:95:4: ( '<\\\\' )
		// Language\\AngleBracketTemplateLexer.g3:95:5: '<\\\\'
		{
		Match("<\\"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred1_AngleBracketTemplateLexer

	// $ANTLR start synpred10_AngleBracketTemplateLexer
	public void synpred10_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:163:7: ( '\\r' | '\\n' )
		// Language\\AngleBracketTemplateLexer.g3:
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
	// $ANTLR end synpred10_AngleBracketTemplateLexer

	// $ANTLR start synpred11_AngleBracketTemplateLexer
	public void synpred11_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:272:5: ( '\\r' | '\\n' )
		// Language\\AngleBracketTemplateLexer.g3:
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
	// $ANTLR end synpred11_AngleBracketTemplateLexer

	// $ANTLR start synpred12_AngleBracketTemplateLexer
	public void synpred12_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:274:4: ( ( '\\r' )? '\\n>>' )
		// Language\\AngleBracketTemplateLexer.g3:274:5: ( '\\r' )? '\\n>>'
		{
		// Language\\AngleBracketTemplateLexer.g3:274:5: ( '\\r' )?
		int alt47=2;
		int LA47_0 = input.LA(1);

		if ( (LA47_0=='\r') )
		{
			alt47=1;
		}
		switch ( alt47 )
		{
		case 1:
			// Language\\AngleBracketTemplateLexer.g3:274:0: '\\r'
			{
			Match('\r'); if (state.failed) return ;

			}
			break;

		}

		Match("\n>>"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred12_AngleBracketTemplateLexer

	// $ANTLR start synpred13_AngleBracketTemplateLexer
	public void synpred13_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:275:5: ( '>' ~ '>' |~ '>' )
		int alt48=2;
		int LA48_0 = input.LA(1);

		if ( (LA48_0=='>') )
		{
			alt48=1;
		}
		else if ( ((LA48_0>='\u0000' && LA48_0<='=')||(LA48_0>='?' && LA48_0<='\uFFFF')) )
		{
			alt48=2;
		}
		else
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			NoViableAltException nvae = new NoViableAltException("", 48, 0, input);

			throw nvae;
		}
		switch ( alt48 )
		{
		case 1:
			// Language\\AngleBracketTemplateLexer.g3:275:6: '>' ~ '>'
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
			// Language\\AngleBracketTemplateLexer.g3:275:17: ~ '>'
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
	// $ANTLR end synpred13_AngleBracketTemplateLexer

	// $ANTLR start synpred2_AngleBracketTemplateLexer
	public void synpred2_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:106:4: ( '<!' )
		// Language\\AngleBracketTemplateLexer.g3:106:5: '<!'
		{
		Match("<!"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred2_AngleBracketTemplateLexer

	// $ANTLR start synpred3_AngleBracketTemplateLexer
	public void synpred3_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:108:4: ( '<if' )
		// Language\\AngleBracketTemplateLexer.g3:108:5: '<if'
		{
		Match("<if"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred3_AngleBracketTemplateLexer

	// $ANTLR start synpred4_AngleBracketTemplateLexer
	public void synpred4_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:115:5: ( '<elseif' )
		// Language\\AngleBracketTemplateLexer.g3:115:6: '<elseif'
		{
		Match("<elseif"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred4_AngleBracketTemplateLexer

	// $ANTLR start synpred5_AngleBracketTemplateLexer
	public void synpred5_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:122:5: ( '<else' )
		// Language\\AngleBracketTemplateLexer.g3:122:6: '<else'
		{
		Match("<else"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred5_AngleBracketTemplateLexer

	// $ANTLR start synpred6_AngleBracketTemplateLexer
	public void synpred6_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:129:5: ( '<endif' )
		// Language\\AngleBracketTemplateLexer.g3:129:6: '<endif'
		{
		Match("<endif"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred6_AngleBracketTemplateLexer

	// $ANTLR start synpred7_AngleBracketTemplateLexer
	public void synpred7_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:138:4: ( '<@' )
		// Language\\AngleBracketTemplateLexer.g3:138:5: '<@'
		{
		Match("<@"); if (state.failed) return ;


		}
	}
	// $ANTLR end synpred7_AngleBracketTemplateLexer

	// $ANTLR start synpred8_AngleBracketTemplateLexer
	public void synpred8_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:154:7: ( '\\r' | '\\n' )
		// Language\\AngleBracketTemplateLexer.g3:
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
	// $ANTLR end synpred8_AngleBracketTemplateLexer

	// $ANTLR start synpred9_AngleBracketTemplateLexer
	public void synpred9_AngleBracketTemplateLexer_fragment()
	{
		// Language\\AngleBracketTemplateLexer.g3:159:8: ( '\\r' | '\\n' )
		// Language\\AngleBracketTemplateLexer.g3:
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
	// $ANTLR end synpred9_AngleBracketTemplateLexer

	public bool synpred8_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred8_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred9_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred9_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred10_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred10_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred3_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred3_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred4_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred4_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred5_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred5_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred6_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred6_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred7_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred7_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred1_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred1_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred2_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred2_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred11_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred11_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred12_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred12_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	public bool synpred13_AngleBracketTemplateLexer()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred13_AngleBracketTemplateLexer_fragment(); // can never throw exception
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
	DFA18 dfa18;
	DFA37 dfa37;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa18 = new DFA18( this, new SpecialStateTransitionHandler( specialStateTransition18 ) );
		dfa37 = new DFA37( this, new SpecialStateTransitionHandler( specialStateTransition37 ) );
	}

	class DFA18 : DFA
	{

		const string DFA18_eotS =
			"\x3\xFFFF\x2\x6\x2\xFFFF\x1\x5\x2\xFFFF\x3\x5\x1\xFFFF";
		const string DFA18_eofS =
			"\xE\xFFFF";
		const string DFA18_minS =
			"\x5\x0\x2\xFFFF\x1\x65\x2\x0\x1\x6E\x1\x64\x1\x3E\x1\x0";
		const string DFA18_maxS =
			"\x1\xFFFF\x2\x0\x2\xFFFF\x2\xFFFF\x1\x65\x2\x0\x1\x6E\x1\x64\x1\x3E\x1"+
			"\x0";
		const string DFA18_acceptS =
			"\x5\xFFFF\x1\x1\x1\x2\x7\xFFFF";
		const string DFA18_specialS =
			"\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x2\xFFFF\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9"+
			"\x1\xA\x1\xB}>";
		static readonly string[] DFA18_transitionS =
			{
				"\xA\x4\x1\x2\x2\x4\x1\x1\x2E\x4\x1\x3\xFFC3\x4",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\xA\x5\x1\x9\x2\x5\x1\x8\x32\x5\x1\x7\xFFBF\x5",
				"\xA\x5\x1\x9\x2\x5\x1\x8\xFFF2\x5",
				"",
				"",
				"\x1\xA",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xB",
				"\x1\xC",
				"\x1\xD",
				"\x1\xFFFF"
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

		public DFA18( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
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
			return "()+ loopback of 158:5: ({...}? => (=> ( '\\r' )? '\\n' |ch= . ) )+";
		}
	}

	int specialStateTransition18( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA18_0 = input.LA(1);

				s = -1;
				if ( (LA18_0=='\r') ) {s = 1;}

				else if ( (LA18_0=='\n') ) {s = 2;}

				else if ( (LA18_0=='<') ) {s = 3;}

				else if ( ((LA18_0>='\u0000' && LA18_0<='\t')||(LA18_0>='\u000B' && LA18_0<='\f')||(LA18_0>='\u000E' && LA18_0<=';')||(LA18_0>='=' && LA18_0<='\uFFFF')) ) {s = 4;}

				if ( s>=0 ) return s;
				break;

			case 1:
				int LA18_1 = input.LA(1);


				int index18_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) ) {s = 5;}

				else if ( (true) ) {s = 6;}


				input.Seek(index18_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA18_2 = input.LA(1);


				int index18_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) ) {s = 5;}

				else if ( (true) ) {s = 6;}


				input.Seek(index18_2);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA18_3 = input.LA(1);


				int index18_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_3=='@') ) {s = 7;}

				else if ( (LA18_3=='\r') ) {s = 8;}

				else if ( (LA18_3=='\n') ) {s = 9;}

				else if ( ((LA18_3>='\u0000' && LA18_3<='\t')||(LA18_3>='\u000B' && LA18_3<='\f')||(LA18_3>='\u000E' && LA18_3<='?')||(LA18_3>='A' && LA18_3<='\uFFFF')) && ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) ))))) {s = 5;}

				else s = 6;


				input.Seek(index18_3);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA18_4 = input.LA(1);


				int index18_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_4=='\r') ) {s = 8;}

				else if ( (LA18_4=='\n') ) {s = 9;}

				else if ( ((LA18_4>='\u0000' && LA18_4<='\t')||(LA18_4>='\u000B' && LA18_4<='\f')||(LA18_4>='\u000E' && LA18_4<='\uFFFF')) && ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) ))))) {s = 5;}

				else s = 6;


				input.Seek(index18_4);
				if ( s>=0 ) return s;
				break;

			case 5:
				int LA18_7 = input.LA(1);


				int index18_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_7=='e') ) {s = 10;}

				else s = 5;


				input.Seek(index18_7);
				if ( s>=0 ) return s;
				break;

			case 6:
				int LA18_8 = input.LA(1);


				int index18_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) ) {s = 5;}

				else if ( (true) ) {s = 6;}


				input.Seek(index18_8);
				if ( s>=0 ) return s;
				break;

			case 7:
				int LA18_9 = input.LA(1);


				int index18_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) ) {s = 5;}

				else if ( (true) ) {s = 6;}


				input.Seek(index18_9);
				if ( s>=0 ) return s;
				break;

			case 8:
				int LA18_10 = input.LA(1);


				int index18_10 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_10=='n') ) {s = 11;}

				else s = 5;


				input.Seek(index18_10);
				if ( s>=0 ) return s;
				break;

			case 9:
				int LA18_11 = input.LA(1);


				int index18_11 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_11=='d') ) {s = 12;}

				else s = 5;


				input.Seek(index18_11);
				if ( s>=0 ) return s;
				break;

			case 10:
				int LA18_12 = input.LA(1);


				int index18_12 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA18_12=='>') ) {s = 13;}

				else s = 5;


				input.Seek(index18_12);
				if ( s>=0 ) return s;
				break;

			case 11:
				int LA18_13 = input.LA(1);


				int index18_13 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((!(UpcomingAtEND(1) || ( input.LA(1) == '\n' && UpcomingAtEND(2) ) || ( input.LA(1) == '\r' && input.LA(2) == '\n' && UpcomingAtEND(3) )))) ) {s = 5;}

				else if ( (true) ) {s = 6;}


				input.Seek(index18_13);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 18, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA37 : DFA
	{

		const string DFA37_eotS =
			"\x13\xFFFF";
		const string DFA37_eofS =
			"\x13\xFFFF";
		const string DFA37_minS =
			"\x4\x0\x1\xFFFF\x1\x0\xD\xFFFF";
		const string DFA37_maxS =
			"\x2\xFFFF\x2\x0\x1\xFFFF\x1\x0\xD\xFFFF";
		const string DFA37_acceptS =
			"\x4\xFFFF\x1\x2\x1\xFFFF\x3\x2\x4\xFFFF\x1\x1\x4\xFFFF\x1\x3";
		const string DFA37_specialS =
			"\x1\x0\x1\x1\x1\x2\x1\x3\x1\xFFFF\x1\x4\xD\xFFFF}>";
		static readonly string[] DFA37_transitionS =
			{
				"\xA\x4\x1\x3\x2\x4\x1\x2\x30\x4\x1\x1\xFFC1\x4",
				"\xA\x8\x1\x7\x2\x8\x1\x6\x30\x8\x1\x5\xFFC1\x8",
				"\x1\xFFFF",
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
				"",
				"",
				"",
				"",
				""
			};

		static readonly short[] DFA37_eot = DFA.UnpackEncodedString(DFA37_eotS);
		static readonly short[] DFA37_eof = DFA.UnpackEncodedString(DFA37_eofS);
		static readonly char[] DFA37_min = DFA.UnpackEncodedStringToUnsignedChars(DFA37_minS);
		static readonly char[] DFA37_max = DFA.UnpackEncodedStringToUnsignedChars(DFA37_maxS);
		static readonly short[] DFA37_accept = DFA.UnpackEncodedString(DFA37_acceptS);
		static readonly short[] DFA37_special = DFA.UnpackEncodedString(DFA37_specialS);
		static readonly short[][] DFA37_transition;

		static DFA37()
		{
			int numStates = DFA37_transitionS.Length;
			DFA37_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA37_transition[i] = DFA.UnpackEncodedString(DFA37_transitionS[i]);
			}
		}

		public DFA37( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 37;
			this.eot = DFA37_eot;
			this.eof = DFA37_eof;
			this.min = DFA37_min;
			this.max = DFA37_max;
			this.accept = DFA37_accept;
			this.special = DFA37_special;
			this.transition = DFA37_transition;
		}
		public override string GetDescription()
		{
			return "()* loopback of 273:3: ( options {k=2; } :=> ( '\\r' )? '\\n' |=>ch= . )*";
		}
	}

	int specialStateTransition37( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA37_0 = input.LA(1);


				int index37_0 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA37_0=='>') ) {s = 1;}

				else if ( (LA37_0=='\r') ) {s = 2;}

				else if ( (LA37_0=='\n') ) {s = 3;}

				else if ( ((LA37_0>='\u0000' && LA37_0<='\t')||(LA37_0>='\u000B' && LA37_0<='\f')||(LA37_0>='\u000E' && LA37_0<='=')||(LA37_0>='?' && LA37_0<='\uFFFF')) && (synpred13_AngleBracketTemplateLexer())) {s = 4;}


				input.Seek(index37_0);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA37_1 = input.LA(1);


				int index37_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA37_1=='>') ) {s = 5;}

				else if ( (LA37_1=='\r') && (synpred13_AngleBracketTemplateLexer())) {s = 6;}

				else if ( (LA37_1=='\n') && (synpred13_AngleBracketTemplateLexer())) {s = 7;}

				else if ( ((LA37_1>='\u0000' && LA37_1<='\t')||(LA37_1>='\u000B' && LA37_1<='\f')||(LA37_1>='\u000E' && LA37_1<='=')||(LA37_1>='?' && LA37_1<='\uFFFF')) && (synpred13_AngleBracketTemplateLexer())) {s = 8;}


				input.Seek(index37_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA37_2 = input.LA(1);


				int index37_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred12_AngleBracketTemplateLexer()) ) {s = 13;}

				else if ( (synpred13_AngleBracketTemplateLexer()) ) {s = 8;}


				input.Seek(index37_2);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA37_3 = input.LA(1);


				int index37_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred12_AngleBracketTemplateLexer()) ) {s = 13;}

				else if ( (synpred13_AngleBracketTemplateLexer()) ) {s = 8;}


				input.Seek(index37_3);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA37_5 = input.LA(1);


				int index37_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred13_AngleBracketTemplateLexer()) ) {s = 8;}

				else if ( (true) ) {s = 18;}


				input.Seek(index37_5);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 37, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
 
	#endregion

}

} // namespace Antlr3.ST.Language
