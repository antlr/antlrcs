// $ANTLR 3.1.2 Grammars\\ActionTranslator.g3 2009-04-18 13:36:22

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

using Antlr3.Tool;
using StringTemplate = Antlr3.ST.StringTemplate;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.Grammars
{
public partial class ActionTranslator : Lexer
{
	public const int EOF=-1;
	public const int ACTION=4;
	public const int ARG=5;
	public const int ATTR_VALUE_EXPR=6;
	public const int DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR=7;
	public const int DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR=8;
	public const int DYNAMIC_SCOPE_ATTR=9;
	public const int ENCLOSING_RULE_SCOPE_ATTR=10;
	public const int ERROR_SCOPED_XY=11;
	public const int ERROR_X=12;
	public const int ERROR_XY=13;
	public const int ESC=14;
	public const int ID=15;
	public const int INDIRECT_TEMPLATE_INSTANCE=16;
	public const int INT=17;
	public const int ISOLATED_DYNAMIC_SCOPE=18;
	public const int ISOLATED_LEXER_RULE_REF=19;
	public const int ISOLATED_TOKEN_REF=20;
	public const int LABEL_REF=21;
	public const int LOCAL_ATTR=22;
	public const int RULE_SCOPE_ATTR=23;
	public const int SCOPE_INDEX_EXPR=24;
	public const int SET_ATTRIBUTE=25;
	public const int SET_DYNAMIC_SCOPE_ATTR=26;
	public const int SET_ENCLOSING_RULE_SCOPE_ATTR=27;
	public const int SET_EXPR_ATTRIBUTE=28;
	public const int SET_LOCAL_ATTR=29;
	public const int SET_RULE_SCOPE_ATTR=30;
	public const int SET_TOKEN_SCOPE_ATTR=31;
	public const int TEMPLATE_EXPR=32;
	public const int TEMPLATE_INSTANCE=33;
	public const int TEXT=34;
	public const int TOKEN_SCOPE_ATTR=35;
	public const int UNKNOWN_SYNTAX=36;
	public const int WS=37;

    // delegates
    // delegators

	public ActionTranslator() {}
	public ActionTranslator( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ActionTranslator( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Grammars\\ActionTranslator.g3"; } }

	public override IToken NextToken()
	{
		for ( ; ;)
		{
			if ( input.LA(1)==CharStreamConstants.EndOfFile )
			{
				return Tokens.EndOfFile;
			}
			state.token = null;
			state.channel = TokenChannels.Default;
			state.tokenStartCharIndex = input.Index;
			state.tokenStartCharPositionInLine = input.CharPositionInLine;
			state.tokenStartLine = input.Line;
			state.text = null;
			try
			{
				int m = input.Mark();
				state.backtracking=1;
				state.failed=false;
				mTokens();
				state.backtracking=0;

				if ( state.failed )
				{
					input.Rewind(m);
					input.Consume(); 
				}
				else
				{
					Emit();
					return state.token;
				}
			}
			catch ( RecognitionException re )
			{
				// shouldn't happen in backtracking mode, but...
				ReportError(re);
				Recover(re);
			}
		}
	}

	public override void Memoize( IIntStream input, int ruleIndex, int ruleStartIndex )
	{
		if ( state.backtracking > 1 )
			base.Memoize( input, ruleIndex, ruleStartIndex );
	}

	public override bool AlreadyParsedRule(IIntStream input, int ruleIndex)
	{
		if ( state.backtracking > 1 )
			return base.AlreadyParsedRule(input, ruleIndex);

		return false;
	}// $ANTLR start "SET_ENCLOSING_RULE_SCOPE_ATTR"
	private void mSET_ENCLOSING_RULE_SCOPE_ATTR()
	{
		try
		{
			int _type = SET_ENCLOSING_RULE_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;
			IToken expr=null;

			// Grammars\\ActionTranslator.g3:91:4: ( '$' x= ID '.' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?)
			// Grammars\\ActionTranslator.g3:91:4: '$' x= ID '.' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart60 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart60, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart66 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart66, CharIndex-1);
			// Grammars\\ActionTranslator.g3:91:22: ( WS )?
			int alt1=2;
			int LA1_0 = input.LA(1);

			if ( ((LA1_0>='\t' && LA1_0<='\n')||LA1_0=='\r'||LA1_0==' ') )
			{
				alt1=1;
			}
			switch ( alt1 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:91:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			int exprStart75 = CharIndex;
			mATTR_VALUE_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart75, CharIndex-1);
			Match(';'); if (state.failed) return ;
			if ( !((enclosingRule!=null &&
				                         (x!=null?x.Text:null).Equals(enclosingRule.name) &&
				                         enclosingRule.GetLocalAttributeScope((y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_ENCLOSING_RULE_SCOPE_ATTR", "enclosingRule!=null &&\r\n\t                         $x.text.Equals(enclosingRule.name) &&\r\n\t                         enclosingRule.GetLocalAttributeScope($y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							StringTemplate st = null;
							AttributeScope scope = enclosingRule.GetLocalAttributeScope((y!=null?y.Text:null));
							if ( scope.isPredefinedRuleScope )
							{
								if ( (y!=null?y.Text:null).Equals("st") || (y!=null?y.Text:null).Equals("tree") )
								{
									st = Template("ruleSetPropertyRef_"+(y!=null?y.Text:null));
									grammar.ReferenceRuleLabelPredefinedAttribute((x!=null?x.Text:null));
									st.SetAttribute("scope", (x!=null?x.Text:null));
									st.SetAttribute("attr", (y!=null?y.Text:null));
									st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
								}
								else
								{
									ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR,
															  grammar,
															  actionToken,
															  (x!=null?x.Text:null),
															  (y!=null?y.Text:null));
								}
							}
							else if ( scope.isPredefinedLexerRuleScope )
							{
					    		// this is a better message to emit than the previous one...
								ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR,
														  grammar,
														  actionToken,
														  (x!=null?x.Text:null),
														  (y!=null?y.Text:null));
							}
							else if ( scope.isParameterScope )
							{
								st = Template("parameterSetAttributeRef");
								st.SetAttribute("attr", scope.GetAttribute((y!=null?y.Text:null)));
								st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
							}
							else
							{ // must be return value
								st = Template("returnSetAttributeRef");
								st.SetAttribute("ruleDescriptor", enclosingRule);
								st.SetAttribute("attr", scope.GetAttribute((y!=null?y.Text:null)));
								st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
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
	// $ANTLR end "SET_ENCLOSING_RULE_SCOPE_ATTR"

	// $ANTLR start "ENCLOSING_RULE_SCOPE_ATTR"
	private void mENCLOSING_RULE_SCOPE_ATTR()
	{
		try
		{
			int _type = ENCLOSING_RULE_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:143:4: ( '$' x= ID '.' y= ID {...}?)
			// Grammars\\ActionTranslator.g3:143:4: '$' x= ID '.' y= ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart107 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart107, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart113 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart113, CharIndex-1);
			if ( !((enclosingRule!=null &&
				                         (x!=null?x.Text:null).Equals(enclosingRule.name) &&
				                         enclosingRule.GetLocalAttributeScope((y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "ENCLOSING_RULE_SCOPE_ATTR", "enclosingRule!=null &&\r\n\t                         $x.text.Equals(enclosingRule.name) &&\r\n\t                         enclosingRule.GetLocalAttributeScope($y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							if ( IsRuleRefInAlt((x!=null?x.Text:null)) )
							{
								ErrorManager.GrammarError(ErrorManager.MSG_RULE_REF_AMBIG_WITH_RULE_IN_ALT,
														  grammar,
														  actionToken,
														  (x!=null?x.Text:null));
							}
							StringTemplate st = null;
							AttributeScope scope = enclosingRule.GetLocalAttributeScope((y!=null?y.Text:null));
							if ( scope.isPredefinedRuleScope )
							{
								st = Template("rulePropertyRef_"+(y!=null?y.Text:null));
								grammar.ReferenceRuleLabelPredefinedAttribute((x!=null?x.Text:null));
								st.SetAttribute("scope", (x!=null?x.Text:null));
								st.SetAttribute("attr", (y!=null?y.Text:null));
							}
							else if ( scope.isPredefinedLexerRuleScope )
							{
								// perhaps not the most precise error message to use, but...
								ErrorManager.GrammarError(ErrorManager.MSG_RULE_HAS_NO_ARGS,
														  grammar,
														  actionToken,
														  (x!=null?x.Text:null));
							}
							else if ( scope.isParameterScope )
							{
								st = Template("parameterAttributeRef");
								st.SetAttribute("attr", scope.GetAttribute((y!=null?y.Text:null)));
							}
							else
							{ // must be return value
								st = Template("returnAttributeRef");
								st.SetAttribute("ruleDescriptor", enclosingRule);
								st.SetAttribute("attr", scope.GetAttribute((y!=null?y.Text:null)));
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
	// $ANTLR end "ENCLOSING_RULE_SCOPE_ATTR"

	// $ANTLR start "SET_TOKEN_SCOPE_ATTR"
	private void mSET_TOKEN_SCOPE_ATTR()
	{
		try
		{
			int _type = SET_TOKEN_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:188:4: ( '$' x= ID '.' y= ID ( WS )? '=' {...}?)
			// Grammars\\ActionTranslator.g3:188:4: '$' x= ID '.' y= ID ( WS )? '=' {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart139 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart139, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart145 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart145, CharIndex-1);
			// Grammars\\ActionTranslator.g3:188:22: ( WS )?
			int alt2=2;
			int LA2_0 = input.LA(1);

			if ( ((LA2_0>='\t' && LA2_0<='\n')||LA2_0=='\r'||LA2_0==' ') )
			{
				alt2=1;
			}
			switch ( alt2 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:188:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			if ( !((enclosingRule!=null && input.LA(1)!='=' &&
				                         (enclosingRule.GetTokenLabel((x!=null?x.Text:null))!=null||
				                          IsTokenRefInAlt((x!=null?x.Text:null))) &&
				                         AttributeScope.tokenScope.GetAttribute((y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_TOKEN_SCOPE_ATTR", "enclosingRule!=null && input.LA(1)!='=' &&\r\n\t                         (enclosingRule.GetTokenLabel($x.text)!=null||\r\n\t                          IsTokenRefInAlt($x.text)) &&\r\n\t                         AttributeScope.tokenScope.GetAttribute($y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR,
													  grammar,
													  actionToken,
													  (x!=null?x.Text:null),
													  (y!=null?y.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SET_TOKEN_SCOPE_ATTR"

	// $ANTLR start "TOKEN_SCOPE_ATTR"
	private void mTOKEN_SCOPE_ATTR()
	{
		try
		{
			int _type = TOKEN_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:207:4: ( '$' x= ID '.' y= ID {...}?)
			// Grammars\\ActionTranslator.g3:207:4: '$' x= ID '.' y= ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart184 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart184, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart190 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart190, CharIndex-1);
			if ( !((enclosingRule!=null &&
				                         (enclosingRule.GetTokenLabel((x!=null?x.Text:null))!=null||
				                          IsTokenRefInAlt((x!=null?x.Text:null))) &&
				                         AttributeScope.tokenScope.GetAttribute((y!=null?y.Text:null))!=null &&
				                         (grammar.type!=Grammar.LEXER ||
				                         GetElementLabel((x!=null?x.Text:null)).elementRef.token.Type==ANTLRParser.TOKEN_REF ||
				                         GetElementLabel((x!=null?x.Text:null)).elementRef.token.Type==ANTLRParser.STRING_LITERAL))) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "TOKEN_SCOPE_ATTR", "enclosingRule!=null &&\r\n\t                         (enclosingRule.GetTokenLabel($x.text)!=null||\r\n\t                          IsTokenRefInAlt($x.text)) &&\r\n\t                         AttributeScope.tokenScope.GetAttribute($y.text)!=null &&\r\n\t                         (grammar.type!=Grammar.LEXER ||\r\n\t                         GetElementLabel($x.text).elementRef.token.Type==ANTLRParser.TOKEN_REF ||\r\n\t                         GetElementLabel($x.text).elementRef.token.Type==ANTLRParser.STRING_LITERAL)");
			}
			if ( state.backtracking==1 )
			{

							string label = (x!=null?x.Text:null);
							if ( enclosingRule.GetTokenLabel((x!=null?x.Text:null))==null )
							{
								// $tokenref.attr  gotta get old label or compute new one
								CheckElementRefUniqueness((x!=null?x.Text:null), true);
								label = enclosingRule.GetElementLabel((x!=null?x.Text:null), outerAltNum, generator);
								if ( label==null )
								{
									ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF,
															  grammar,
															  actionToken,
															  "$"+(x!=null?x.Text:null)+"."+(y!=null?y.Text:null));
									label = (x!=null?x.Text:null);
								}
							}
							StringTemplate st = Template("tokenLabelPropertyRef_"+(y!=null?y.Text:null));
							st.SetAttribute("scope", label);
							st.SetAttribute("attr", AttributeScope.tokenScope.GetAttribute((y!=null?y.Text:null)));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TOKEN_SCOPE_ATTR"

	// $ANTLR start "SET_RULE_SCOPE_ATTR"
	private void mSET_RULE_SCOPE_ATTR()
	{
		try
		{
			int _type = SET_RULE_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;


			Grammar.LabelElementPair pair=null;
			string refdRuleName=null;

			// Grammars\\ActionTranslator.g3:248:4: ( '$' x= ID '.' y= ID ( WS )? '=' {...}?{...}?)
			// Grammars\\ActionTranslator.g3:248:4: '$' x= ID '.' y= ID ( WS )? '=' {...}?{...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart221 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart221, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart227 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart227, CharIndex-1);
			// Grammars\\ActionTranslator.g3:248:22: ( WS )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( ((LA3_0>='\t' && LA3_0<='\n')||LA3_0=='\r'||LA3_0==' ') )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:248:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			if ( !((enclosingRule!=null && input.LA(1)!='=')) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_RULE_SCOPE_ATTR", "enclosingRule!=null && input.LA(1)!='='");
			}
			if ( state.backtracking==1 )
			{

							pair = enclosingRule.GetRuleLabel((x!=null?x.Text:null));
							refdRuleName = (x!=null?x.Text:null);
							if ( pair!=null )
							{
								refdRuleName = pair.referencedRuleName;
							}
						
			}
			if ( !(((enclosingRule.GetRuleLabel((x!=null?x.Text:null))!=null || IsRuleRefInAlt((x!=null?x.Text:null))) &&
				      GetRuleLabelAttribute(enclosingRule.GetRuleLabel((x!=null?x.Text:null))!=null?enclosingRule.GetRuleLabel((x!=null?x.Text:null)).referencedRuleName:(x!=null?x.Text:null),(y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_RULE_SCOPE_ATTR", "(enclosingRule.GetRuleLabel($x.text)!=null || IsRuleRefInAlt($x.text)) &&\r\n\t      GetRuleLabelAttribute(enclosingRule.GetRuleLabel($x.text)!=null?enclosingRule.GetRuleLabel($x.text).referencedRuleName:$x.text,$y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR,
													  grammar,
													  actionToken,
													  (x!=null?x.Text:null),
													  (y!=null?y.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SET_RULE_SCOPE_ATTR"

	// $ANTLR start "RULE_SCOPE_ATTR"
	private void mRULE_SCOPE_ATTR()
	{
		try
		{
			int _type = RULE_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;


				Grammar.LabelElementPair pair=null;
				string refdRuleName=null;

			// Grammars\\ActionTranslator.g3:279:4: ( '$' x= ID '.' y= ID {...}?{...}?)
			// Grammars\\ActionTranslator.g3:279:4: '$' x= ID '.' y= ID {...}?{...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart280 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart280, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart286 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart286, CharIndex-1);
			if ( !((enclosingRule!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "RULE_SCOPE_ATTR", "enclosingRule!=null");
			}
			if ( state.backtracking==1 )
			{

						pair = enclosingRule.GetRuleLabel((x!=null?x.Text:null));
						refdRuleName = (x!=null?x.Text:null);
						if ( pair!=null ) {
							refdRuleName = pair.referencedRuleName;
						}
						
			}
			if ( !(((enclosingRule.GetRuleLabel((x!=null?x.Text:null))!=null || IsRuleRefInAlt((x!=null?x.Text:null))) &&
				      GetRuleLabelAttribute(enclosingRule.GetRuleLabel((x!=null?x.Text:null))!=null?enclosingRule.GetRuleLabel((x!=null?x.Text:null)).referencedRuleName:(x!=null?x.Text:null),(y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "RULE_SCOPE_ATTR", "(enclosingRule.GetRuleLabel($x.text)!=null || IsRuleRefInAlt($x.text)) &&\r\n\t      GetRuleLabelAttribute(enclosingRule.GetRuleLabel($x.text)!=null?enclosingRule.GetRuleLabel($x.text).referencedRuleName:$x.text,$y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							string label = (x!=null?x.Text:null);
							if ( pair==null )
							{
								// $ruleref.attr  gotta get old label or compute new one
								CheckElementRefUniqueness((x!=null?x.Text:null), false);
								label = enclosingRule.GetElementLabel((x!=null?x.Text:null), outerAltNum, generator);
								if ( label==null )
								{
									ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF,
															  grammar,
															  actionToken,
															  "$"+(x!=null?x.Text:null)+"."+(y!=null?y.Text:null));
									label = (x!=null?x.Text:null);
								}
							}
							StringTemplate st;
							Rule refdRule = grammar.GetRule(refdRuleName);
							AttributeScope scope = refdRule.GetLocalAttributeScope((y!=null?y.Text:null));
							if ( scope.isPredefinedRuleScope )
							{
								st = Template("ruleLabelPropertyRef_"+(y!=null?y.Text:null));
								grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
								st.SetAttribute("scope", label);
								st.SetAttribute("attr", (y!=null?y.Text:null));
							}
							else if ( scope.isPredefinedLexerRuleScope )
							{
								st = Template("lexerRuleLabelPropertyRef_"+(y!=null?y.Text:null));
								grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
								st.SetAttribute("scope", label);
								st.SetAttribute("attr", (y!=null?y.Text:null));
							}
							else if ( scope.isParameterScope )
							{
								// TODO: error!
							}
							else
							{
								st = Template("ruleLabelRef");
								st.SetAttribute("referencedRule", refdRule);
								st.SetAttribute("scope", label);
								st.SetAttribute("attr", scope.GetAttribute((y!=null?y.Text:null)));
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
	// $ANTLR end "RULE_SCOPE_ATTR"

	// $ANTLR start "LABEL_REF"
	private void mLABEL_REF()
	{
		try
		{
			int _type = LABEL_REF;
			int _channel = DefaultTokenChannel;
			IToken ID1=null;

			// Grammars\\ActionTranslator.g3:343:4: ( '$' ID {...}?)
			// Grammars\\ActionTranslator.g3:343:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID1Start328 = CharIndex;
			mID(); if (state.failed) return ;
			ID1 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID1Start328, CharIndex-1);
			if ( !((enclosingRule!=null &&
				            GetElementLabel((ID1!=null?ID1.Text:null))!=null &&
					        enclosingRule.GetRuleLabel((ID1!=null?ID1.Text:null))==null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "LABEL_REF", "enclosingRule!=null &&\r\n\t            GetElementLabel($ID.text)!=null &&\r\n\t\t        enclosingRule.GetRuleLabel($ID.text)==null");
			}
			if ( state.backtracking==1 )
			{

							StringTemplate st;
							Grammar.LabelElementPair pair = GetElementLabel((ID1!=null?ID1.Text:null));
							if ( pair.type==Grammar.RULE_LIST_LABEL ||
								  pair.type==Grammar.TOKEN_LIST_LABEL ||
								  pair.type == Grammar.WILDCARD_TREE_LIST_LABEL )
							{
								st = Template("listLabelRef");
							}
							else
							{
								st = Template("tokenLabelRef");
							}
							st.SetAttribute("label", (ID1!=null?ID1.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LABEL_REF"

	// $ANTLR start "ISOLATED_TOKEN_REF"
	private void mISOLATED_TOKEN_REF()
	{
		try
		{
			int _type = ISOLATED_TOKEN_REF;
			int _channel = DefaultTokenChannel;
			IToken ID2=null;

			// Grammars\\ActionTranslator.g3:366:4: ( '$' ID {...}?)
			// Grammars\\ActionTranslator.g3:366:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID2Start352 = CharIndex;
			mID(); if (state.failed) return ;
			ID2 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID2Start352, CharIndex-1);
			if ( !((grammar.type!=Grammar.LEXER && enclosingRule!=null && IsTokenRefInAlt((ID2!=null?ID2.Text:null)))) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "ISOLATED_TOKEN_REF", "grammar.type!=Grammar.LEXER && enclosingRule!=null && IsTokenRefInAlt($ID.text)");
			}
			if ( state.backtracking==1 )
			{

							string label = enclosingRule.GetElementLabel((ID2!=null?ID2.Text:null), outerAltNum, generator);
							CheckElementRefUniqueness((ID2!=null?ID2.Text:null), true);
							if ( label==null )
							{
								ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF,
														  grammar,
														  actionToken,
														  (ID2!=null?ID2.Text:null));
							}
							else
							{
								StringTemplate st = Template("tokenLabelRef");
								st.SetAttribute("label", label);
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
	// $ANTLR end "ISOLATED_TOKEN_REF"

	// $ANTLR start "ISOLATED_LEXER_RULE_REF"
	private void mISOLATED_LEXER_RULE_REF()
	{
		try
		{
			int _type = ISOLATED_LEXER_RULE_REF;
			int _channel = DefaultTokenChannel;
			IToken ID3=null;

			// Grammars\\ActionTranslator.g3:388:4: ( '$' ID {...}?)
			// Grammars\\ActionTranslator.g3:388:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID3Start376 = CharIndex;
			mID(); if (state.failed) return ;
			ID3 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID3Start376, CharIndex-1);
			if ( !((grammar.type==Grammar.LEXER &&
				             enclosingRule!=null &&
				             IsRuleRefInAlt((ID3!=null?ID3.Text:null)))) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "ISOLATED_LEXER_RULE_REF", "grammar.type==Grammar.LEXER &&\r\n\t             enclosingRule!=null &&\r\n\t             IsRuleRefInAlt($ID.text)");
			}
			if ( state.backtracking==1 )
			{

							string label = enclosingRule.GetElementLabel((ID3!=null?ID3.Text:null), outerAltNum, generator);
							CheckElementRefUniqueness((ID3!=null?ID3.Text:null), false);
							if ( label==null )
							{
								ErrorManager.GrammarError(ErrorManager.MSG_FORWARD_ELEMENT_REF,
														  grammar,
														  actionToken,
														  (ID3!=null?ID3.Text:null));
							}
							else
							{
								StringTemplate st = Template("lexerRuleLabel");
								st.SetAttribute("label", label);
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
	// $ANTLR end "ISOLATED_LEXER_RULE_REF"

	// $ANTLR start "SET_LOCAL_ATTR"
	private void mSET_LOCAL_ATTR()
	{
		try
		{
			int _type = SET_LOCAL_ATTR;
			int _channel = DefaultTokenChannel;
			IToken expr=null;
			IToken ID4=null;

			// Grammars\\ActionTranslator.g3:422:4: ( '$' ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?)
			// Grammars\\ActionTranslator.g3:422:4: '$' ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID4Start400 = CharIndex;
			mID(); if (state.failed) return ;
			ID4 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID4Start400, CharIndex-1);
			// Grammars\\ActionTranslator.g3:422:11: ( WS )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( ((LA4_0>='\t' && LA4_0<='\n')||LA4_0=='\r'||LA4_0==' ') )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:422:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			int exprStart409 = CharIndex;
			mATTR_VALUE_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart409, CharIndex-1);
			Match(';'); if (state.failed) return ;
			if ( !((enclosingRule!=null
																&& enclosingRule.GetLocalAttributeScope((ID4!=null?ID4.Text:null))!=null
																&& !enclosingRule.GetLocalAttributeScope((ID4!=null?ID4.Text:null)).isPredefinedLexerRuleScope)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_LOCAL_ATTR", "enclosingRule!=null\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t&& enclosingRule.GetLocalAttributeScope($ID.text)!=null\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t&& !enclosingRule.GetLocalAttributeScope($ID.text).isPredefinedLexerRuleScope");
			}
			if ( state.backtracking==1 )
			{

							StringTemplate st;
							AttributeScope scope = enclosingRule.GetLocalAttributeScope((ID4!=null?ID4.Text:null));
							if ( scope.isPredefinedRuleScope )
							{
								if ((ID4!=null?ID4.Text:null).Equals("tree") || (ID4!=null?ID4.Text:null).Equals("st"))
								{
									st = Template("ruleSetPropertyRef_"+(ID4!=null?ID4.Text:null));
									grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.name);
									st.SetAttribute("scope", enclosingRule.name);
									st.SetAttribute("attr", (ID4!=null?ID4.Text:null));
									st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
								}
								else
								{
									ErrorManager.GrammarError(ErrorManager.MSG_WRITE_TO_READONLY_ATTR,
															 grammar,
															 actionToken,
															 (ID4!=null?ID4.Text:null),
															 "");
								}
							}
							else if ( scope.isParameterScope )
							{
								st = Template("parameterSetAttributeRef");
								st.SetAttribute("attr", scope.GetAttribute((ID4!=null?ID4.Text:null)));
								st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
							}
							else
							{
								st = Template("returnSetAttributeRef");
								st.SetAttribute("ruleDescriptor", enclosingRule);
								st.SetAttribute("attr", scope.GetAttribute((ID4!=null?ID4.Text:null)));
								st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
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
	// $ANTLR end "SET_LOCAL_ATTR"

	// $ANTLR start "LOCAL_ATTR"
	private void mLOCAL_ATTR()
	{
		try
		{
			int _type = LOCAL_ATTR;
			int _channel = DefaultTokenChannel;
			IToken ID5=null;

			// Grammars\\ActionTranslator.g3:464:4: ( '$' ID {...}?)
			// Grammars\\ActionTranslator.g3:464:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID5Start432 = CharIndex;
			mID(); if (state.failed) return ;
			ID5 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID5Start432, CharIndex-1);
			if ( !((enclosingRule!=null && enclosingRule.GetLocalAttributeScope((ID5!=null?ID5.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "LOCAL_ATTR", "enclosingRule!=null && enclosingRule.GetLocalAttributeScope($ID.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							StringTemplate st;
							AttributeScope scope = enclosingRule.GetLocalAttributeScope((ID5!=null?ID5.Text:null));
							if ( scope.isPredefinedRuleScope )
							{
								st = Template("rulePropertyRef_"+(ID5!=null?ID5.Text:null));
								grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.name);
								st.SetAttribute("scope", enclosingRule.name);
								st.SetAttribute("attr", (ID5!=null?ID5.Text:null));
							}
							else if ( scope.isPredefinedLexerRuleScope )
							{
								st = Template("lexerRulePropertyRef_"+(ID5!=null?ID5.Text:null));
								st.SetAttribute("scope", enclosingRule.name);
								st.SetAttribute("attr", (ID5!=null?ID5.Text:null));
							}
							else if ( scope.isParameterScope )
							{
								st = Template("parameterAttributeRef");
								st.SetAttribute("attr", scope.GetAttribute((ID5!=null?ID5.Text:null)));
							}
							else
							{
								st = Template("returnAttributeRef");
								st.SetAttribute("ruleDescriptor", enclosingRule);
								st.SetAttribute("attr", scope.GetAttribute((ID5!=null?ID5.Text:null)));
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
	// $ANTLR end "LOCAL_ATTR"

	// $ANTLR start "SET_DYNAMIC_SCOPE_ATTR"
	private void mSET_DYNAMIC_SCOPE_ATTR()
	{
		try
		{
			int _type = SET_DYNAMIC_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;
			IToken expr=null;

			// Grammars\\ActionTranslator.g3:509:4: ( '$' x= ID '::' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?)
			// Grammars\\ActionTranslator.g3:509:4: '$' x= ID '::' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart458 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart458, CharIndex-1);
			Match("::"); if (state.failed) return ;

			int yStart464 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart464, CharIndex-1);
			// Grammars\\ActionTranslator.g3:509:23: ( WS )?
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( ((LA5_0>='\t' && LA5_0<='\n')||LA5_0=='\r'||LA5_0==' ') )
			{
				alt5=1;
			}
			switch ( alt5 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:509:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			int exprStart473 = CharIndex;
			mATTR_VALUE_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart473, CharIndex-1);
			Match(';'); if (state.failed) return ;
			if ( !((ResolveDynamicScope((x!=null?x.Text:null))!=null &&
									     ResolveDynamicScope((x!=null?x.Text:null)).GetAttribute((y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "SET_DYNAMIC_SCOPE_ATTR", "ResolveDynamicScope($x.text)!=null &&\r\n\t\t\t\t\t\t     ResolveDynamicScope($x.text).GetAttribute($y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							AttributeScope scope = ResolveDynamicScope((x!=null?x.Text:null));
							if ( scope!=null )
							{
								StringTemplate st = Template("scopeSetAttributeRef");
								st.SetAttribute("scope", (x!=null?x.Text:null));
								st.SetAttribute("attr",  scope.GetAttribute((y!=null?y.Text:null)));
								st.SetAttribute("expr",  TranslateAction((expr!=null?expr.Text:null)));
							}
							else
							{
								// error: invalid dynamic attribute
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
	// $ANTLR end "SET_DYNAMIC_SCOPE_ATTR"

	// $ANTLR start "DYNAMIC_SCOPE_ATTR"
	private void mDYNAMIC_SCOPE_ATTR()
	{
		try
		{
			int _type = DYNAMIC_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:530:4: ( '$' x= ID '::' y= ID {...}?)
			// Grammars\\ActionTranslator.g3:530:4: '$' x= ID '::' y= ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart508 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart508, CharIndex-1);
			Match("::"); if (state.failed) return ;

			int yStart514 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart514, CharIndex-1);
			if ( !((ResolveDynamicScope((x!=null?x.Text:null))!=null &&
									     ResolveDynamicScope((x!=null?x.Text:null)).GetAttribute((y!=null?y.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "DYNAMIC_SCOPE_ATTR", "ResolveDynamicScope($x.text)!=null &&\r\n\t\t\t\t\t\t     ResolveDynamicScope($x.text).GetAttribute($y.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							AttributeScope scope = ResolveDynamicScope((x!=null?x.Text:null));
							if ( scope!=null )
							{
								StringTemplate st = Template("scopeAttributeRef");
								st.SetAttribute("scope", (x!=null?x.Text:null));
								st.SetAttribute("attr",  scope.GetAttribute((y!=null?y.Text:null)));
							}
							else
							{
								// error: invalid dynamic attribute
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
	// $ANTLR end "DYNAMIC_SCOPE_ATTR"

	// $ANTLR start "ERROR_SCOPED_XY"
	private void mERROR_SCOPED_XY()
	{
		try
		{
			int _type = ERROR_SCOPED_XY;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:551:4: ( '$' x= ID '::' y= ID )
			// Grammars\\ActionTranslator.g3:551:4: '$' x= ID '::' y= ID
			{
			Match('$'); if (state.failed) return ;
			int xStart548 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart548, CharIndex-1);
			Match("::"); if (state.failed) return ;

			int yStart554 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart554, CharIndex-1);
			if ( state.backtracking==1 )
			{

						chunks.Add(Text);
						generator.IssueInvalidScopeError((x!=null?x.Text:null),(y!=null?y.Text:null),
						                                 enclosingRule,actionToken,
						                                 outerAltNum);		
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ERROR_SCOPED_XY"

	// $ANTLR start "DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR"
	private void mDYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR()
	{
		try
		{
			int _type = DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken expr=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:569:4: ( '$' x= ID '[' '-' expr= SCOPE_INDEX_EXPR ']' '::' y= ID )
			// Grammars\\ActionTranslator.g3:569:4: '$' x= ID '[' '-' expr= SCOPE_INDEX_EXPR ']' '::' y= ID
			{
			Match('$'); if (state.failed) return ;
			int xStart576 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart576, CharIndex-1);
			Match('['); if (state.failed) return ;
			Match('-'); if (state.failed) return ;
			int exprStart584 = CharIndex;
			mSCOPE_INDEX_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart584, CharIndex-1);
			Match(']'); if (state.failed) return ;
			Match("::"); if (state.failed) return ;

			int yStart592 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart592, CharIndex-1);
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("scopeAttributeRef");
							st.SetAttribute("scope",    (x!=null?x.Text:null));
							st.SetAttribute("attr",     ResolveDynamicScope((x!=null?x.Text:null)).GetAttribute((y!=null?y.Text:null)));
							st.SetAttribute("negIndex", (expr!=null?expr.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR"

	// $ANTLR start "DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR"
	private void mDYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR()
	{
		try
		{
			int _type = DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken expr=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:580:4: ( '$' x= ID '[' expr= SCOPE_INDEX_EXPR ']' '::' y= ID )
			// Grammars\\ActionTranslator.g3:580:4: '$' x= ID '[' expr= SCOPE_INDEX_EXPR ']' '::' y= ID
			{
			Match('$'); if (state.failed) return ;
			int xStart616 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart616, CharIndex-1);
			Match('['); if (state.failed) return ;
			int exprStart622 = CharIndex;
			mSCOPE_INDEX_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart622, CharIndex-1);
			Match(']'); if (state.failed) return ;
			Match("::"); if (state.failed) return ;

			int yStart630 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart630, CharIndex-1);
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("scopeAttributeRef");
							st.SetAttribute("scope", (x!=null?x.Text:null));
							st.SetAttribute("attr",  ResolveDynamicScope((x!=null?x.Text:null)).GetAttribute((y!=null?y.Text:null)));
							st.SetAttribute("index", (expr!=null?expr.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR"

	// $ANTLR start "SCOPE_INDEX_EXPR"
	private void mSCOPE_INDEX_EXPR()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:592:4: ( (~ ']' )+ )
			// Grammars\\ActionTranslator.g3:592:4: (~ ']' )+
			{
			// Grammars\\ActionTranslator.g3:592:4: (~ ']' )+
			int cnt6=0;
			for ( ; ; )
			{
				int alt6=2;
				int LA6_0 = input.LA(1);

				if ( ((LA6_0>='\u0000' && LA6_0<='\\')||(LA6_0>='^' && LA6_0<='\uFFFF')) )
				{
					alt6=1;
				}


				switch ( alt6 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt6 >= 1 )
						goto loop6;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee6 = new EarlyExitException( 6, input );
					throw eee6;
				}
				cnt6++;
			}
			loop6:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "SCOPE_INDEX_EXPR"

	// $ANTLR start "ISOLATED_DYNAMIC_SCOPE"
	private void mISOLATED_DYNAMIC_SCOPE()
	{
		try
		{
			int _type = ISOLATED_DYNAMIC_SCOPE;
			int _channel = DefaultTokenChannel;
			IToken ID6=null;

			// Grammars\\ActionTranslator.g3:601:4: ( '$' ID {...}?)
			// Grammars\\ActionTranslator.g3:601:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID6Start673 = CharIndex;
			mID(); if (state.failed) return ;
			ID6 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID6Start673, CharIndex-1);
			if ( !((ResolveDynamicScope((ID6!=null?ID6.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "ISOLATED_DYNAMIC_SCOPE", "ResolveDynamicScope($ID.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("isolatedDynamicScopeRef");
							st.SetAttribute("scope", (ID6!=null?ID6.Text:null));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ISOLATED_DYNAMIC_SCOPE"

	// $ANTLR start "TEMPLATE_INSTANCE"
	private void mTEMPLATE_INSTANCE()
	{
		try
		{
			int _type = TEMPLATE_INSTANCE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ActionTranslator.g3:614:4: ( '%' ID '(' ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )? ')' )
			// Grammars\\ActionTranslator.g3:614:4: '%' ID '(' ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )? ')'
			{
			Match('%'); if (state.failed) return ;
			mID(); if (state.failed) return ;
			Match('('); if (state.failed) return ;
			// Grammars\\ActionTranslator.g3:614:15: ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )?
			int alt11=2;
			int LA11_0 = input.LA(1);

			if ( ((LA11_0>='\t' && LA11_0<='\n')||LA11_0=='\r'||LA11_0==' '||(LA11_0>='A' && LA11_0<='Z')||LA11_0=='_'||(LA11_0>='a' && LA11_0<='z')) )
			{
				alt11=1;
			}
			switch ( alt11 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:614:17: ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )?
				{
				// Grammars\\ActionTranslator.g3:614:17: ( WS )?
				int alt7=2;
				int LA7_0 = input.LA(1);

				if ( ((LA7_0>='\t' && LA7_0<='\n')||LA7_0=='\r'||LA7_0==' ') )
				{
					alt7=1;
				}
				switch ( alt7 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:614:0: WS
					{
					mWS(); if (state.failed) return ;

					}
					break;

				}

				mARG(); if (state.failed) return ;
				// Grammars\\ActionTranslator.g3:614:25: ( ',' ( WS )? ARG )*
				for ( ; ; )
				{
					int alt9=2;
					int LA9_0 = input.LA(1);

					if ( (LA9_0==',') )
					{
						alt9=1;
					}


					switch ( alt9 )
					{
					case 1:
						// Grammars\\ActionTranslator.g3:614:26: ',' ( WS )? ARG
						{
						Match(','); if (state.failed) return ;
						// Grammars\\ActionTranslator.g3:614:30: ( WS )?
						int alt8=2;
						int LA8_0 = input.LA(1);

						if ( ((LA8_0>='\t' && LA8_0<='\n')||LA8_0=='\r'||LA8_0==' ') )
						{
							alt8=1;
						}
						switch ( alt8 )
						{
						case 1:
							// Grammars\\ActionTranslator.g3:614:0: WS
							{
							mWS(); if (state.failed) return ;

							}
							break;

						}

						mARG(); if (state.failed) return ;

						}
						break;

					default:
						goto loop9;
					}
				}

				loop9:
					;


				// Grammars\\ActionTranslator.g3:614:40: ( WS )?
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( ((LA10_0>='\t' && LA10_0<='\n')||LA10_0=='\r'||LA10_0==' ') )
				{
					alt10=1;
				}
				switch ( alt10 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:614:0: WS
					{
					mWS(); if (state.failed) return ;

					}
					break;

				}


				}
				break;

			}

			Match(')'); if (state.failed) return ;
			if ( state.backtracking==1 )
			{

							string action = Text.Substring( 1, Text.Length - 1 );
							string ruleName = "<outside-of-rule>";
							if ( enclosingRule!=null )
							{
								ruleName = enclosingRule.name;
							}
							StringTemplate st =
								generator.TranslateTemplateConstructor(ruleName,
																	   outerAltNum,
																	   actionToken,
																	   action);
							if ( st!=null )
							{
								chunks.Add(st);
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
	// $ANTLR end "TEMPLATE_INSTANCE"

	// $ANTLR start "INDIRECT_TEMPLATE_INSTANCE"
	private void mINDIRECT_TEMPLATE_INSTANCE()
	{
		try
		{
			int _type = INDIRECT_TEMPLATE_INSTANCE;
			int _channel = DefaultTokenChannel;
			// Grammars\\ActionTranslator.g3:637:4: ( '%' '(' ACTION ')' '(' ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )? ')' )
			// Grammars\\ActionTranslator.g3:637:4: '%' '(' ACTION ')' '(' ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )? ')'
			{
			Match('%'); if (state.failed) return ;
			Match('('); if (state.failed) return ;
			mACTION(); if (state.failed) return ;
			Match(')'); if (state.failed) return ;
			Match('('); if (state.failed) return ;
			// Grammars\\ActionTranslator.g3:637:27: ( ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )? )?
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( ((LA16_0>='\t' && LA16_0<='\n')||LA16_0=='\r'||LA16_0==' '||(LA16_0>='A' && LA16_0<='Z')||LA16_0=='_'||(LA16_0>='a' && LA16_0<='z')) )
			{
				alt16=1;
			}
			switch ( alt16 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:637:29: ( WS )? ARG ( ',' ( WS )? ARG )* ( WS )?
				{
				// Grammars\\ActionTranslator.g3:637:29: ( WS )?
				int alt12=2;
				int LA12_0 = input.LA(1);

				if ( ((LA12_0>='\t' && LA12_0<='\n')||LA12_0=='\r'||LA12_0==' ') )
				{
					alt12=1;
				}
				switch ( alt12 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:637:0: WS
					{
					mWS(); if (state.failed) return ;

					}
					break;

				}

				mARG(); if (state.failed) return ;
				// Grammars\\ActionTranslator.g3:637:37: ( ',' ( WS )? ARG )*
				for ( ; ; )
				{
					int alt14=2;
					int LA14_0 = input.LA(1);

					if ( (LA14_0==',') )
					{
						alt14=1;
					}


					switch ( alt14 )
					{
					case 1:
						// Grammars\\ActionTranslator.g3:637:38: ',' ( WS )? ARG
						{
						Match(','); if (state.failed) return ;
						// Grammars\\ActionTranslator.g3:637:42: ( WS )?
						int alt13=2;
						int LA13_0 = input.LA(1);

						if ( ((LA13_0>='\t' && LA13_0<='\n')||LA13_0=='\r'||LA13_0==' ') )
						{
							alt13=1;
						}
						switch ( alt13 )
						{
						case 1:
							// Grammars\\ActionTranslator.g3:637:0: WS
							{
							mWS(); if (state.failed) return ;

							}
							break;

						}

						mARG(); if (state.failed) return ;

						}
						break;

					default:
						goto loop14;
					}
				}

				loop14:
					;


				// Grammars\\ActionTranslator.g3:637:52: ( WS )?
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( ((LA15_0>='\t' && LA15_0<='\n')||LA15_0=='\r'||LA15_0==' ') )
				{
					alt15=1;
				}
				switch ( alt15 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:637:0: WS
					{
					mWS(); if (state.failed) return ;

					}
					break;

				}


				}
				break;

			}

			Match(')'); if (state.failed) return ;
			if ( state.backtracking==1 )
			{

							string action = Text.Substring( 1, Text.Length - 1 );
							StringTemplate st =
								generator.TranslateTemplateConstructor(enclosingRule.name,
																	   outerAltNum,
																	   actionToken,
																	   action);
							chunks.Add(st);
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "INDIRECT_TEMPLATE_INSTANCE"

	// $ANTLR start "ARG"
	private void mARG()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:651:7: ( ID '=' ACTION )
			// Grammars\\ActionTranslator.g3:651:7: ID '=' ACTION
			{
			mID(); if (state.failed) return ;
			Match('='); if (state.failed) return ;
			mACTION(); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ARG"

	// $ANTLR start "SET_EXPR_ATTRIBUTE"
	private void mSET_EXPR_ATTRIBUTE()
	{
		try
		{
			int _type = SET_EXPR_ATTRIBUTE;
			int _channel = DefaultTokenChannel;
			IToken a=null;
			IToken expr=null;
			IToken ID7=null;

			// Grammars\\ActionTranslator.g3:656:4: ( '%' a= ACTION '.' ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' )
			// Grammars\\ActionTranslator.g3:656:4: '%' a= ACTION '.' ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';'
			{
			Match('%'); if (state.failed) return ;
			int aStart823 = CharIndex;
			mACTION(); if (state.failed) return ;
			a = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, aStart823, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int ID7Start827 = CharIndex;
			mID(); if (state.failed) return ;
			ID7 = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, ID7Start827, CharIndex-1);
			// Grammars\\ActionTranslator.g3:656:24: ( WS )?
			int alt17=2;
			int LA17_0 = input.LA(1);

			if ( ((LA17_0>='\t' && LA17_0<='\n')||LA17_0=='\r'||LA17_0==' ') )
			{
				alt17=1;
			}
			switch ( alt17 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:656:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			int exprStart836 = CharIndex;
			mATTR_VALUE_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart836, CharIndex-1);
			Match(';'); if (state.failed) return ;
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("actionSetAttribute");
							string action = (a!=null?a.Text:null);
							action = action.Substring( 1, action.Length - 2 ); // stuff inside {...}
							st.SetAttribute("st", TranslateAction(action));
							st.SetAttribute("attrName", (ID7!=null?ID7.Text:null));
							st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SET_EXPR_ATTRIBUTE"

	// $ANTLR start "SET_ATTRIBUTE"
	private void mSET_ATTRIBUTE()
	{
		try
		{
			int _type = SET_ATTRIBUTE;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;
			IToken expr=null;

			// Grammars\\ActionTranslator.g3:673:4: ( '%' x= ID '.' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' )
			// Grammars\\ActionTranslator.g3:673:4: '%' x= ID '.' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';'
			{
			Match('%'); if (state.failed) return ;
			int xStart863 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart863, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart869 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart869, CharIndex-1);
			// Grammars\\ActionTranslator.g3:673:22: ( WS )?
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( ((LA18_0>='\t' && LA18_0<='\n')||LA18_0=='\r'||LA18_0==' ') )
			{
				alt18=1;
			}
			switch ( alt18 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:673:0: WS
				{
				mWS(); if (state.failed) return ;

				}
				break;

			}

			Match('='); if (state.failed) return ;
			int exprStart878 = CharIndex;
			mATTR_VALUE_EXPR(); if (state.failed) return ;
			expr = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, exprStart878, CharIndex-1);
			Match(';'); if (state.failed) return ;
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("actionSetAttribute");
							st.SetAttribute("st", (x!=null?x.Text:null));
							st.SetAttribute("attrName", (y!=null?y.Text:null));
							st.SetAttribute("expr", TranslateAction((expr!=null?expr.Text:null)));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "SET_ATTRIBUTE"

	// $ANTLR start "ATTR_VALUE_EXPR"
	private void mATTR_VALUE_EXPR()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:686:4: (~ '=' (~ ';' )* )
			// Grammars\\ActionTranslator.g3:686:4: ~ '=' (~ ';' )*
			{
			if ( (input.LA(1)>='\u0000' && input.LA(1)<='<')||(input.LA(1)>='>' && input.LA(1)<='\uFFFF') )
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

			// Grammars\\ActionTranslator.g3:686:9: (~ ';' )*
			for ( ; ; )
			{
				int alt19=2;
				int LA19_0 = input.LA(1);

				if ( ((LA19_0>='\u0000' && LA19_0<=':')||(LA19_0>='<' && LA19_0<='\uFFFF')) )
				{
					alt19=1;
				}


				switch ( alt19 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop19;
				}
			}

			loop19:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ATTR_VALUE_EXPR"

	// $ANTLR start "TEMPLATE_EXPR"
	private void mTEMPLATE_EXPR()
	{
		try
		{
			int _type = TEMPLATE_EXPR;
			int _channel = DefaultTokenChannel;
			IToken a=null;

			// Grammars\\ActionTranslator.g3:691:4: ( '%' a= ACTION )
			// Grammars\\ActionTranslator.g3:691:4: '%' a= ACTION
			{
			Match('%'); if (state.failed) return ;
			int aStart927 = CharIndex;
			mACTION(); if (state.failed) return ;
			a = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, aStart927, CharIndex-1);
			if ( state.backtracking==1 )
			{

							StringTemplate st = Template("actionStringConstructor");
							string action = (a!=null?a.Text:null);
							action = action.Substring( 1, action.Length - 2 ); // stuff inside {...}
							st.SetAttribute("stringExpr", TranslateAction(action));
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TEMPLATE_EXPR"

	// $ANTLR start "ACTION"
	private void mACTION()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:703:4: ( '{' ( options {greedy=false; } : . )* '}' )
			// Grammars\\ActionTranslator.g3:703:4: '{' ( options {greedy=false; } : . )* '}'
			{
			Match('{'); if (state.failed) return ;
			// Grammars\\ActionTranslator.g3:703:8: ( options {greedy=false; } : . )*
			for ( ; ; )
			{
				int alt20=2;
				int LA20_0 = input.LA(1);

				if ( (LA20_0=='}') )
				{
					alt20=2;
				}
				else if ( ((LA20_0>='\u0000' && LA20_0<='|')||(LA20_0>='~' && LA20_0<='\uFFFF')) )
				{
					alt20=1;
				}


				switch ( alt20 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:703:33: .
					{
					MatchAny(); if (state.failed) return ;

					}
					break;

				default:
					goto loop20;
				}
			}

			loop20:
				;


			Match('}'); if (state.failed) return ;

			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ACTION"

	// $ANTLR start "ESC"
	private void mESC()
	{
		try
		{
			int _type = ESC;
			int _channel = DefaultTokenChannel;
			// Grammars\\ActionTranslator.g3:707:4: ( '\\\\' '$' | '\\\\' '%' | '\\\\' ~ ( '$' | '%' ) )
			int alt21=3;
			int LA21_0 = input.LA(1);

			if ( (LA21_0=='\\') )
			{
				int LA21_1 = input.LA(2);

				if ( (LA21_1=='$') )
				{
					alt21=1;
				}
				else if ( (LA21_1=='%') )
				{
					alt21=2;
				}
				else if ( ((LA21_1>='\u0000' && LA21_1<='#')||(LA21_1>='&' && LA21_1<='\uFFFF')) )
				{
					alt21=3;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 21, 1, input);

					throw nvae;
				}
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
				// Grammars\\ActionTranslator.g3:707:4: '\\\\' '$'
				{
				Match('\\'); if (state.failed) return ;
				Match('$'); if (state.failed) return ;
				if ( state.backtracking==1 )
				{
					chunks.Add("$");
				}

				}
				break;
			case 2:
				// Grammars\\ActionTranslator.g3:708:4: '\\\\' '%'
				{
				Match('\\'); if (state.failed) return ;
				Match('%'); if (state.failed) return ;
				if ( state.backtracking==1 )
				{
					chunks.Add("%");
				}

				}
				break;
			case 3:
				// Grammars\\ActionTranslator.g3:709:4: '\\\\' ~ ( '$' | '%' )
				{
				Match('\\'); if (state.failed) return ;
				input.Consume();
				state.failed=false;
				if ( state.backtracking==1 )
				{
					chunks.Add(Text);
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
	// $ANTLR end "ESC"

	// $ANTLR start "ERROR_XY"
	private void mERROR_XY()
	{
		try
		{
			int _type = ERROR_XY;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionTranslator.g3:713:4: ( '$' x= ID '.' y= ID )
			// Grammars\\ActionTranslator.g3:713:4: '$' x= ID '.' y= ID
			{
			Match('$'); if (state.failed) return ;
			int xStart1016 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart1016, CharIndex-1);
			Match('.'); if (state.failed) return ;
			int yStart1022 = CharIndex;
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, yStart1022, CharIndex-1);
			if ( state.backtracking==1 )
			{

							chunks.Add(Text);
							generator.IssueInvalidAttributeError((x!=null?x.Text:null),(y!=null?y.Text:null),
																 enclosingRule,actionToken,
																 outerAltNum);
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ERROR_XY"

	// $ANTLR start "ERROR_X"
	private void mERROR_X()
	{
		try
		{
			int _type = ERROR_X;
			int _channel = DefaultTokenChannel;
			IToken x=null;

			// Grammars\\ActionTranslator.g3:723:4: ( '$' x= ID )
			// Grammars\\ActionTranslator.g3:723:4: '$' x= ID
			{
			Match('$'); if (state.failed) return ;
			int xStart1042 = CharIndex;
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenTypes.Invalid, TokenChannels.Default, xStart1042, CharIndex-1);
			if ( state.backtracking==1 )
			{

							chunks.Add(Text);
							generator.IssueInvalidAttributeError((x!=null?x.Text:null),
																 enclosingRule,actionToken,
																 outerAltNum);
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "ERROR_X"

	// $ANTLR start "UNKNOWN_SYNTAX"
	private void mUNKNOWN_SYNTAX()
	{
		try
		{
			int _type = UNKNOWN_SYNTAX;
			int _channel = DefaultTokenChannel;
			// Grammars\\ActionTranslator.g3:733:4: ( '$' | '%' ( ID | '.' | '(' | ')' | ',' | '{' | '}' | '\"' )* )
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0=='$') )
			{
				alt23=1;
			}
			else if ( (LA23_0=='%') )
			{
				alt23=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 23, 0, input);

				throw nvae;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\ActionTranslator.g3:733:4: '$'
				{
				Match('$'); if (state.failed) return ;
				if ( state.backtracking==1 )
				{

								chunks.Add(Text);
								// shouldn't need an error here.  Just accept $ if it doesn't look like anything
							
				}

				}
				break;
			case 2:
				// Grammars\\ActionTranslator.g3:738:4: '%' ( ID | '.' | '(' | ')' | ',' | '{' | '}' | '\"' )*
				{
				Match('%'); if (state.failed) return ;
				// Grammars\\ActionTranslator.g3:738:8: ( ID | '.' | '(' | ')' | ',' | '{' | '}' | '\"' )*
				for ( ; ; )
				{
					int alt22=9;
					alt22 = dfa22.Predict(input);
					switch ( alt22 )
					{
					case 1:
						// Grammars\\ActionTranslator.g3:738:9: ID
						{
						mID(); if (state.failed) return ;

						}
						break;
					case 2:
						// Grammars\\ActionTranslator.g3:738:12: '.'
						{
						Match('.'); if (state.failed) return ;

						}
						break;
					case 3:
						// Grammars\\ActionTranslator.g3:738:16: '('
						{
						Match('('); if (state.failed) return ;

						}
						break;
					case 4:
						// Grammars\\ActionTranslator.g3:738:20: ')'
						{
						Match(')'); if (state.failed) return ;

						}
						break;
					case 5:
						// Grammars\\ActionTranslator.g3:738:24: ','
						{
						Match(','); if (state.failed) return ;

						}
						break;
					case 6:
						// Grammars\\ActionTranslator.g3:738:28: '{'
						{
						Match('{'); if (state.failed) return ;

						}
						break;
					case 7:
						// Grammars\\ActionTranslator.g3:738:32: '}'
						{
						Match('}'); if (state.failed) return ;

						}
						break;
					case 8:
						// Grammars\\ActionTranslator.g3:738:36: '\"'
						{
						Match('\"'); if (state.failed) return ;

						}
						break;

					default:
						goto loop22;
					}
				}

				loop22:
					;


				if ( state.backtracking==1 )
				{

								chunks.Add(Text);
								ErrorManager.GrammarError(ErrorManager.MSG_INVALID_TEMPLATE_ACTION,
														  grammar,
														  actionToken,
														  Text);
							
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
	// $ANTLR end "UNKNOWN_SYNTAX"

	// $ANTLR start "TEXT"
	private void mTEXT()
	{
		try
		{
			int _type = TEXT;
			int _channel = DefaultTokenChannel;
			// Grammars\\ActionTranslator.g3:749:4: ( (~ ( '$' | '%' | '\\\\' ) )+ )
			// Grammars\\ActionTranslator.g3:749:4: (~ ( '$' | '%' | '\\\\' ) )+
			{
			// Grammars\\ActionTranslator.g3:749:4: (~ ( '$' | '%' | '\\\\' ) )+
			int cnt24=0;
			for ( ; ; )
			{
				int alt24=2;
				int LA24_0 = input.LA(1);

				if ( ((LA24_0>='\u0000' && LA24_0<='#')||(LA24_0>='&' && LA24_0<='[')||(LA24_0>=']' && LA24_0<='\uFFFF')) )
				{
					alt24=1;
				}


				switch ( alt24 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt24 >= 1 )
						goto loop24;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee24 = new EarlyExitException( 24, input );
					throw eee24;
				}
				cnt24++;
			}
			loop24:
				;


			if ( state.backtracking==1 )
			{
				chunks.Add(Text);
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "TEXT"

	// $ANTLR start "ID"
	private void mID()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:754:4: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			// Grammars\\ActionTranslator.g3:754:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
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

			// Grammars\\ActionTranslator.g3:754:28: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			for ( ; ; )
			{
				int alt25=2;
				int LA25_0 = input.LA(1);

				if ( ((LA25_0>='0' && LA25_0<='9')||(LA25_0>='A' && LA25_0<='Z')||LA25_0=='_'||(LA25_0>='a' && LA25_0<='z')) )
				{
					alt25=1;
				}


				switch ( alt25 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					goto loop25;
				}
			}

			loop25:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "ID"

	// $ANTLR start "INT"
	private void mINT()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:759:4: ( ( '0' .. '9' )+ )
			// Grammars\\ActionTranslator.g3:759:4: ( '0' .. '9' )+
			{
			// Grammars\\ActionTranslator.g3:759:4: ( '0' .. '9' )+
			int cnt26=0;
			for ( ; ; )
			{
				int alt26=2;
				int LA26_0 = input.LA(1);

				if ( ((LA26_0>='0' && LA26_0<='9')) )
				{
					alt26=1;
				}


				switch ( alt26 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt26 >= 1 )
						goto loop26;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee26 = new EarlyExitException( 26, input );
					throw eee26;
				}
				cnt26++;
			}
			loop26:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "INT"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			// Grammars\\ActionTranslator.g3:764:4: ( ( ' ' | '\\t' | '\\n' | '\\r' )+ )
			// Grammars\\ActionTranslator.g3:764:4: ( ' ' | '\\t' | '\\n' | '\\r' )+
			{
			// Grammars\\ActionTranslator.g3:764:4: ( ' ' | '\\t' | '\\n' | '\\r' )+
			int cnt27=0;
			for ( ; ; )
			{
				int alt27=2;
				int LA27_0 = input.LA(1);

				if ( ((LA27_0>='\t' && LA27_0<='\n')||LA27_0=='\r'||LA27_0==' ') )
				{
					alt27=1;
				}


				switch ( alt27 )
				{
				case 1:
					// Grammars\\ActionTranslator.g3:
					{
					input.Consume();
					state.failed=false;

					}
					break;

				default:
					if ( cnt27 >= 1 )
						goto loop27;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee27 = new EarlyExitException( 27, input );
					throw eee27;
				}
				cnt27++;
			}
			loop27:
				;



			}

		}
		finally
		{
		}
	}
	// $ANTLR end "WS"

	public override void mTokens()
	{
		// Grammars\\ActionTranslator.g3:1:41: ( SET_ENCLOSING_RULE_SCOPE_ATTR | ENCLOSING_RULE_SCOPE_ATTR | SET_TOKEN_SCOPE_ATTR | TOKEN_SCOPE_ATTR | SET_RULE_SCOPE_ATTR | RULE_SCOPE_ATTR | LABEL_REF | ISOLATED_TOKEN_REF | ISOLATED_LEXER_RULE_REF | SET_LOCAL_ATTR | LOCAL_ATTR | SET_DYNAMIC_SCOPE_ATTR | DYNAMIC_SCOPE_ATTR | ERROR_SCOPED_XY | DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR | DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR | ISOLATED_DYNAMIC_SCOPE | TEMPLATE_INSTANCE | INDIRECT_TEMPLATE_INSTANCE | SET_EXPR_ATTRIBUTE | SET_ATTRIBUTE | TEMPLATE_EXPR | ESC | ERROR_XY | ERROR_X | UNKNOWN_SYNTAX | TEXT )
		int alt28=27;
		alt28 = dfa28.Predict(input);
		switch ( alt28 )
		{
		case 1:
			// Grammars\\ActionTranslator.g3:1:41: SET_ENCLOSING_RULE_SCOPE_ATTR
			{

			mSET_ENCLOSING_RULE_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 2:
			// Grammars\\ActionTranslator.g3:1:71: ENCLOSING_RULE_SCOPE_ATTR
			{

			mENCLOSING_RULE_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 3:
			// Grammars\\ActionTranslator.g3:1:97: SET_TOKEN_SCOPE_ATTR
			{

			mSET_TOKEN_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 4:
			// Grammars\\ActionTranslator.g3:1:118: TOKEN_SCOPE_ATTR
			{

			mTOKEN_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 5:
			// Grammars\\ActionTranslator.g3:1:135: SET_RULE_SCOPE_ATTR
			{

			mSET_RULE_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 6:
			// Grammars\\ActionTranslator.g3:1:155: RULE_SCOPE_ATTR
			{

			mRULE_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 7:
			// Grammars\\ActionTranslator.g3:1:171: LABEL_REF
			{

			mLABEL_REF(); if (state.failed) return ;

			}
			break;
		case 8:
			// Grammars\\ActionTranslator.g3:1:181: ISOLATED_TOKEN_REF
			{

			mISOLATED_TOKEN_REF(); if (state.failed) return ;

			}
			break;
		case 9:
			// Grammars\\ActionTranslator.g3:1:200: ISOLATED_LEXER_RULE_REF
			{

			mISOLATED_LEXER_RULE_REF(); if (state.failed) return ;

			}
			break;
		case 10:
			// Grammars\\ActionTranslator.g3:1:224: SET_LOCAL_ATTR
			{

			mSET_LOCAL_ATTR(); if (state.failed) return ;

			}
			break;
		case 11:
			// Grammars\\ActionTranslator.g3:1:239: LOCAL_ATTR
			{

			mLOCAL_ATTR(); if (state.failed) return ;

			}
			break;
		case 12:
			// Grammars\\ActionTranslator.g3:1:250: SET_DYNAMIC_SCOPE_ATTR
			{

			mSET_DYNAMIC_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 13:
			// Grammars\\ActionTranslator.g3:1:273: DYNAMIC_SCOPE_ATTR
			{

			mDYNAMIC_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 14:
			// Grammars\\ActionTranslator.g3:1:292: ERROR_SCOPED_XY
			{

			mERROR_SCOPED_XY(); if (state.failed) return ;

			}
			break;
		case 15:
			// Grammars\\ActionTranslator.g3:1:308: DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR
			{

			mDYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 16:
			// Grammars\\ActionTranslator.g3:1:344: DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR
			{

			mDYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR(); if (state.failed) return ;

			}
			break;
		case 17:
			// Grammars\\ActionTranslator.g3:1:380: ISOLATED_DYNAMIC_SCOPE
			{

			mISOLATED_DYNAMIC_SCOPE(); if (state.failed) return ;

			}
			break;
		case 18:
			// Grammars\\ActionTranslator.g3:1:403: TEMPLATE_INSTANCE
			{

			mTEMPLATE_INSTANCE(); if (state.failed) return ;

			}
			break;
		case 19:
			// Grammars\\ActionTranslator.g3:1:421: INDIRECT_TEMPLATE_INSTANCE
			{

			mINDIRECT_TEMPLATE_INSTANCE(); if (state.failed) return ;

			}
			break;
		case 20:
			// Grammars\\ActionTranslator.g3:1:448: SET_EXPR_ATTRIBUTE
			{

			mSET_EXPR_ATTRIBUTE(); if (state.failed) return ;

			}
			break;
		case 21:
			// Grammars\\ActionTranslator.g3:1:467: SET_ATTRIBUTE
			{

			mSET_ATTRIBUTE(); if (state.failed) return ;

			}
			break;
		case 22:
			// Grammars\\ActionTranslator.g3:1:481: TEMPLATE_EXPR
			{

			mTEMPLATE_EXPR(); if (state.failed) return ;

			}
			break;
		case 23:
			// Grammars\\ActionTranslator.g3:1:495: ESC
			{

			mESC(); if (state.failed) return ;

			}
			break;
		case 24:
			// Grammars\\ActionTranslator.g3:1:499: ERROR_XY
			{

			mERROR_XY(); if (state.failed) return ;

			}
			break;
		case 25:
			// Grammars\\ActionTranslator.g3:1:508: ERROR_X
			{

			mERROR_X(); if (state.failed) return ;

			}
			break;
		case 26:
			// Grammars\\ActionTranslator.g3:1:516: UNKNOWN_SYNTAX
			{

			mUNKNOWN_SYNTAX(); if (state.failed) return ;

			}
			break;
		case 27:
			// Grammars\\ActionTranslator.g3:1:531: TEXT
			{
			mTEXT(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_ActionTranslator
	public void synpred1_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_ENCLOSING_RULE_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:41: SET_ENCLOSING_RULE_SCOPE_ATTR
		{
		mSET_ENCLOSING_RULE_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred1_ActionTranslator

	// $ANTLR start synpred10_ActionTranslator
	public void synpred10_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_LOCAL_ATTR )
		// Grammars\\ActionTranslator.g3:1:224: SET_LOCAL_ATTR
		{
		mSET_LOCAL_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred10_ActionTranslator

	// $ANTLR start synpred11_ActionTranslator
	public void synpred11_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( LOCAL_ATTR )
		// Grammars\\ActionTranslator.g3:1:239: LOCAL_ATTR
		{
		mLOCAL_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred11_ActionTranslator

	// $ANTLR start synpred12_ActionTranslator
	public void synpred12_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_DYNAMIC_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:250: SET_DYNAMIC_SCOPE_ATTR
		{
		mSET_DYNAMIC_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred12_ActionTranslator

	// $ANTLR start synpred13_ActionTranslator
	public void synpred13_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( DYNAMIC_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:273: DYNAMIC_SCOPE_ATTR
		{
		mDYNAMIC_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred13_ActionTranslator

	// $ANTLR start synpred14_ActionTranslator
	public void synpred14_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ERROR_SCOPED_XY )
		// Grammars\\ActionTranslator.g3:1:292: ERROR_SCOPED_XY
		{
		mERROR_SCOPED_XY(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred14_ActionTranslator

	// $ANTLR start synpred15_ActionTranslator
	public void synpred15_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:308: DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR
		{
		mDYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred15_ActionTranslator

	// $ANTLR start synpred16_ActionTranslator
	public void synpred16_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:344: DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR
		{
		mDYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred16_ActionTranslator

	// $ANTLR start synpred17_ActionTranslator
	public void synpred17_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ISOLATED_DYNAMIC_SCOPE )
		// Grammars\\ActionTranslator.g3:1:380: ISOLATED_DYNAMIC_SCOPE
		{
		mISOLATED_DYNAMIC_SCOPE(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred17_ActionTranslator

	// $ANTLR start synpred18_ActionTranslator
	public void synpred18_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( TEMPLATE_INSTANCE )
		// Grammars\\ActionTranslator.g3:1:403: TEMPLATE_INSTANCE
		{
		mTEMPLATE_INSTANCE(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred18_ActionTranslator

	// $ANTLR start synpred19_ActionTranslator
	public void synpred19_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( INDIRECT_TEMPLATE_INSTANCE )
		// Grammars\\ActionTranslator.g3:1:421: INDIRECT_TEMPLATE_INSTANCE
		{
		mINDIRECT_TEMPLATE_INSTANCE(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred19_ActionTranslator

	// $ANTLR start synpred2_ActionTranslator
	public void synpred2_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ENCLOSING_RULE_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:71: ENCLOSING_RULE_SCOPE_ATTR
		{
		mENCLOSING_RULE_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred2_ActionTranslator

	// $ANTLR start synpred20_ActionTranslator
	public void synpred20_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_EXPR_ATTRIBUTE )
		// Grammars\\ActionTranslator.g3:1:448: SET_EXPR_ATTRIBUTE
		{
		mSET_EXPR_ATTRIBUTE(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred20_ActionTranslator

	// $ANTLR start synpred21_ActionTranslator
	public void synpred21_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_ATTRIBUTE )
		// Grammars\\ActionTranslator.g3:1:467: SET_ATTRIBUTE
		{
		mSET_ATTRIBUTE(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred21_ActionTranslator

	// $ANTLR start synpred22_ActionTranslator
	public void synpred22_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( TEMPLATE_EXPR )
		// Grammars\\ActionTranslator.g3:1:481: TEMPLATE_EXPR
		{
		mTEMPLATE_EXPR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred22_ActionTranslator

	// $ANTLR start synpred24_ActionTranslator
	public void synpred24_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ERROR_XY )
		// Grammars\\ActionTranslator.g3:1:499: ERROR_XY
		{
		mERROR_XY(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred24_ActionTranslator

	// $ANTLR start synpred25_ActionTranslator
	public void synpred25_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ERROR_X )
		// Grammars\\ActionTranslator.g3:1:508: ERROR_X
		{
		mERROR_X(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred25_ActionTranslator

	// $ANTLR start synpred26_ActionTranslator
	public void synpred26_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( UNKNOWN_SYNTAX )
		// Grammars\\ActionTranslator.g3:1:516: UNKNOWN_SYNTAX
		{
		mUNKNOWN_SYNTAX(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred26_ActionTranslator

	// $ANTLR start synpred3_ActionTranslator
	public void synpred3_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_TOKEN_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:97: SET_TOKEN_SCOPE_ATTR
		{
		mSET_TOKEN_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred3_ActionTranslator

	// $ANTLR start synpred4_ActionTranslator
	public void synpred4_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( TOKEN_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:118: TOKEN_SCOPE_ATTR
		{
		mTOKEN_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred4_ActionTranslator

	// $ANTLR start synpred5_ActionTranslator
	public void synpred5_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( SET_RULE_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:135: SET_RULE_SCOPE_ATTR
		{
		mSET_RULE_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred5_ActionTranslator

	// $ANTLR start synpred6_ActionTranslator
	public void synpred6_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( RULE_SCOPE_ATTR )
		// Grammars\\ActionTranslator.g3:1:155: RULE_SCOPE_ATTR
		{
		mRULE_SCOPE_ATTR(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred6_ActionTranslator

	// $ANTLR start synpred7_ActionTranslator
	public void synpred7_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( LABEL_REF )
		// Grammars\\ActionTranslator.g3:1:171: LABEL_REF
		{
		mLABEL_REF(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred7_ActionTranslator

	// $ANTLR start synpred8_ActionTranslator
	public void synpred8_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ISOLATED_TOKEN_REF )
		// Grammars\\ActionTranslator.g3:1:181: ISOLATED_TOKEN_REF
		{
		mISOLATED_TOKEN_REF(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred8_ActionTranslator

	// $ANTLR start synpred9_ActionTranslator
	public void synpred9_ActionTranslator_fragment()
	{
		// Grammars\\ActionTranslator.g3:1:0: ( ISOLATED_LEXER_RULE_REF )
		// Grammars\\ActionTranslator.g3:1:200: ISOLATED_LEXER_RULE_REF
		{
		mISOLATED_LEXER_RULE_REF(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred9_ActionTranslator

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
	DFA22 dfa22;
	DFA28 dfa28;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa22 = new DFA22( this );
		dfa28 = new DFA28( this, new SpecialStateTransitionHandler( specialStateTransition28 ) );
	}

	class DFA22 : DFA
	{

		const string DFA22_eotS =
			"\x1\x1\x9\xFFFF";
		const string DFA22_eofS =
			"\xA\xFFFF";
		const string DFA22_minS =
			"\x1\x22\x9\xFFFF";
		const string DFA22_maxS =
			"\x1\x7D\x9\xFFFF";
		const string DFA22_acceptS =
			"\x1\xFFFF\x1\x9\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8";
		const string DFA22_specialS =
			"\xA\xFFFF}>";
		static readonly string[] DFA22_transitionS =
			{
				"\x1\x9\x5\xFFFF\x1\x4\x1\x5\x2\xFFFF\x1\x6\x1\xFFFF\x1\x3\x12\xFFFF"+
				"\x1A\x2\x4\xFFFF\x1\x2\x1\xFFFF\x1A\x2\x1\x7\x1\xFFFF\x1\x8",
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

		static readonly short[] DFA22_eot = DFA.UnpackEncodedString(DFA22_eotS);
		static readonly short[] DFA22_eof = DFA.UnpackEncodedString(DFA22_eofS);
		static readonly char[] DFA22_min = DFA.UnpackEncodedStringToUnsignedChars(DFA22_minS);
		static readonly char[] DFA22_max = DFA.UnpackEncodedStringToUnsignedChars(DFA22_maxS);
		static readonly short[] DFA22_accept = DFA.UnpackEncodedString(DFA22_acceptS);
		static readonly short[] DFA22_special = DFA.UnpackEncodedString(DFA22_specialS);
		static readonly short[][] DFA22_transition;

		static DFA22()
		{
			int numStates = DFA22_transitionS.Length;
			DFA22_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA22_transition[i] = DFA.UnpackEncodedString(DFA22_transitionS[i]);
			}
		}

		public DFA22( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 22;
			this.eot = DFA22_eot;
			this.eof = DFA22_eof;
			this.min = DFA22_min;
			this.max = DFA22_max;
			this.accept = DFA22_accept;
			this.special = DFA22_special;
			this.transition = DFA22_transition;
		}
		public override string GetDescription()
		{
			return "()* loopback of 738:8: ( ID | '.' | '(' | ')' | ',' | '{' | '}' | '\"' )*";
		}
	}

	class DFA28 : DFA
	{

		const string DFA28_eotS =
			"\x1E\xFFFF";
		const string DFA28_eofS =
			"\x1E\xFFFF";
		const string DFA28_minS =
			"\x2\x0\x14\xFFFF\x1\x0\x7\xFFFF";
		const string DFA28_maxS =
			"\x1\xFFFF\x1\x0\x14\xFFFF\x1\x0\x7\xFFFF";
		const string DFA28_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9\x1\xA"+
			"\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x10\x1\x11\x1\x18\x1\x19\x1\x1A\x1"+
			"\xFFFF\x1\x12\x1\x13\x1\x14\x1\x15\x1\x16\x1\x17\x1\x1B";
		const string DFA28_specialS =
			"\x1\x0\x1\x1\x14\xFFFF\x1\x2\x7\xFFFF}>";
		static readonly string[] DFA28_transitionS =
			{
				"\x24\x1D\x1\x1\x1\x16\x36\x1D\x1\x1C\xFFA3\x1D",
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
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xFFFF",
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};

		static readonly short[] DFA28_eot = DFA.UnpackEncodedString(DFA28_eotS);
		static readonly short[] DFA28_eof = DFA.UnpackEncodedString(DFA28_eofS);
		static readonly char[] DFA28_min = DFA.UnpackEncodedStringToUnsignedChars(DFA28_minS);
		static readonly char[] DFA28_max = DFA.UnpackEncodedStringToUnsignedChars(DFA28_maxS);
		static readonly short[] DFA28_accept = DFA.UnpackEncodedString(DFA28_acceptS);
		static readonly short[] DFA28_special = DFA.UnpackEncodedString(DFA28_specialS);
		static readonly short[][] DFA28_transition;

		static DFA28()
		{
			int numStates = DFA28_transitionS.Length;
			DFA28_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA28_transition[i] = DFA.UnpackEncodedString(DFA28_transitionS[i]);
			}
		}

		public DFA28( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 28;
			this.eot = DFA28_eot;
			this.eof = DFA28_eof;
			this.min = DFA28_min;
			this.max = DFA28_max;
			this.accept = DFA28_accept;
			this.special = DFA28_special;
			this.transition = DFA28_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens options {k=1; backtrack=true; } : ( SET_ENCLOSING_RULE_SCOPE_ATTR | ENCLOSING_RULE_SCOPE_ATTR | SET_TOKEN_SCOPE_ATTR | TOKEN_SCOPE_ATTR | SET_RULE_SCOPE_ATTR | RULE_SCOPE_ATTR | LABEL_REF | ISOLATED_TOKEN_REF | ISOLATED_LEXER_RULE_REF | SET_LOCAL_ATTR | LOCAL_ATTR | SET_DYNAMIC_SCOPE_ATTR | DYNAMIC_SCOPE_ATTR | ERROR_SCOPED_XY | DYNAMIC_NEGATIVE_INDEXED_SCOPE_ATTR | DYNAMIC_ABSOLUTE_INDEXED_SCOPE_ATTR | ISOLATED_DYNAMIC_SCOPE | TEMPLATE_INSTANCE | INDIRECT_TEMPLATE_INSTANCE | SET_EXPR_ATTRIBUTE | SET_ATTRIBUTE | TEMPLATE_EXPR | ESC | ERROR_XY | ERROR_X | UNKNOWN_SYNTAX | TEXT );";
		}
	}

	int specialStateTransition28( DFA dfa, int s, IIntStream _input )
	{
		IIntStream input = _input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA28_0 = input.LA(1);

				s = -1;
				if ( (LA28_0=='$') ) {s = 1;}

				else if ( (LA28_0=='%') ) {s = 22;}

				else if ( (LA28_0=='\\') ) {s = 28;}

				else if ( ((LA28_0>='\u0000' && LA28_0<='#')||(LA28_0>='&' && LA28_0<='[')||(LA28_0>=']' && LA28_0<='\uFFFF')) ) {s = 29;}

				if ( s>=0 ) return s;
				break;

			case 1:
				int LA28_1 = input.LA(1);


				int index28_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_ActionTranslator_fragment)) ) {s = 2;}

				else if ( (EvaluatePredicate(synpred2_ActionTranslator_fragment)) ) {s = 3;}

				else if ( (EvaluatePredicate(synpred3_ActionTranslator_fragment)) ) {s = 4;}

				else if ( (EvaluatePredicate(synpred4_ActionTranslator_fragment)) ) {s = 5;}

				else if ( (EvaluatePredicate(synpred5_ActionTranslator_fragment)) ) {s = 6;}

				else if ( (EvaluatePredicate(synpred6_ActionTranslator_fragment)) ) {s = 7;}

				else if ( (EvaluatePredicate(synpred7_ActionTranslator_fragment)) ) {s = 8;}

				else if ( (EvaluatePredicate(synpred8_ActionTranslator_fragment)) ) {s = 9;}

				else if ( (EvaluatePredicate(synpred9_ActionTranslator_fragment)) ) {s = 10;}

				else if ( (EvaluatePredicate(synpred10_ActionTranslator_fragment)) ) {s = 11;}

				else if ( (EvaluatePredicate(synpred11_ActionTranslator_fragment)) ) {s = 12;}

				else if ( (EvaluatePredicate(synpred12_ActionTranslator_fragment)) ) {s = 13;}

				else if ( (EvaluatePredicate(synpred13_ActionTranslator_fragment)) ) {s = 14;}

				else if ( (EvaluatePredicate(synpred14_ActionTranslator_fragment)) ) {s = 15;}

				else if ( (EvaluatePredicate(synpred15_ActionTranslator_fragment)) ) {s = 16;}

				else if ( (EvaluatePredicate(synpred16_ActionTranslator_fragment)) ) {s = 17;}

				else if ( (EvaluatePredicate(synpred17_ActionTranslator_fragment)) ) {s = 18;}

				else if ( (EvaluatePredicate(synpred24_ActionTranslator_fragment)) ) {s = 19;}

				else if ( (EvaluatePredicate(synpred25_ActionTranslator_fragment)) ) {s = 20;}

				else if ( (EvaluatePredicate(synpred26_ActionTranslator_fragment)) ) {s = 21;}


				input.Seek(index28_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA28_22 = input.LA(1);


				int index28_22 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred18_ActionTranslator_fragment)) ) {s = 23;}

				else if ( (EvaluatePredicate(synpred19_ActionTranslator_fragment)) ) {s = 24;}

				else if ( (EvaluatePredicate(synpred20_ActionTranslator_fragment)) ) {s = 25;}

				else if ( (EvaluatePredicate(synpred21_ActionTranslator_fragment)) ) {s = 26;}

				else if ( (EvaluatePredicate(synpred22_ActionTranslator_fragment)) ) {s = 27;}

				else if ( (EvaluatePredicate(synpred26_ActionTranslator_fragment)) ) {s = 21;}


				input.Seek(index28_22);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 28, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
 
	#endregion

}

} // namespace Antlr3.Grammars
