// $ANTLR 3.1.2 Grammars\\ActionAnalysisLexer.g3 2009-04-16 20:58:18

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


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.Grammars
{
/** We need to set Rule.referencedPredefinedRuleAttributes before
 *  code generation.  This filter looks at an action in context of
 *  its rule and outer alternative number and figures out which
 *  rules have predefined prefs referenced.  I need this so I can
 *  remove unusued labels.  This also tracks, for labeled rules,
 *  which are referenced by actions.
 */
public partial class ActionAnalysisLexer : Lexer
{
	public const int EOF=-1;
	public const int ID=4;
	public const int X=5;
	public const int X_Y=6;
	public const int Y=7;

    // delegates
    // delegators

	public ActionAnalysisLexer() {}
	public ActionAnalysisLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ActionAnalysisLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "Grammars\\ActionAnalysisLexer.g3"; } }

	public override IToken NextToken()
	{
		for ( ; ;)
		{
			if ( input.LA(1)==CharStreamConstants.Eof )
			{
				return TokenConstants.EofToken;
			}
			state.token = null;
			state.channel = TokenConstants.DefaultChannel;
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
	}// $ANTLR start "X_Y"
	private void mX_Y()
	{
		try
		{
			int _type = X_Y;
			int _channel = DefaultTokenChannel;
			IToken x=null;
			IToken y=null;

			// Grammars\\ActionAnalysisLexer.g3:92:4: ( '$' x= ID '.' y= ID {...}?)
			// Grammars\\ActionAnalysisLexer.g3:92:4: '$' x= ID '.' y= ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int xStart57 = GetCharIndex();
			mID(); if (state.failed) return ;
			x = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, xStart57, GetCharIndex()-1);
			Match('.'); if (state.failed) return ;
			int yStart63 = GetCharIndex();
			mID(); if (state.failed) return ;
			y = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, yStart63, GetCharIndex()-1);
			if ( !((enclosingRule!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "X_Y", "enclosingRule!=null");
			}
			if ( state.backtracking==1 )
			{

							AttributeScope scope = null;
							string refdRuleName = null;
							if ( (x!=null?x.Text:null).Equals(enclosingRule.name) )
							{
								// ref to enclosing rule.
								refdRuleName = (x!=null?x.Text:null);
								scope = enclosingRule.GetLocalAttributeScope((y!=null?y.Text:null));
							}
							else if ( enclosingRule.GetRuleLabel((x!=null?x.Text:null))!=null )
							{
								// ref to rule label
								Grammar.LabelElementPair pair = enclosingRule.GetRuleLabel((x!=null?x.Text:null));
								pair.actionReferencesLabel = true;
								refdRuleName = pair.referencedRuleName;
								Rule refdRule = grammar.GetRule(refdRuleName);
								if ( refdRule!=null )
								{
									scope = refdRule.GetLocalAttributeScope((y!=null?y.Text:null));
								}
							}
							else if ( enclosingRule.GetRuleRefsInAlt(x.Text, outerAltNum)!=null )
							{
								// ref to rule referenced in this alt
								refdRuleName = (x!=null?x.Text:null);
								Rule refdRule = grammar.GetRule(refdRuleName);
								if ( refdRule!=null )
								{
									scope = refdRule.GetLocalAttributeScope((y!=null?y.Text:null));
								}
							}
							if ( scope!=null &&
								 (scope.isPredefinedRuleScope||scope.isPredefinedLexerRuleScope) )
							{
								grammar.ReferenceRuleLabelPredefinedAttribute(refdRuleName);
								//System.out.println("referenceRuleLabelPredefinedAttribute for "+refdRuleName);
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
	// $ANTLR end "X_Y"

	// $ANTLR start "X"
	private void mX()
	{
		try
		{
			int _type = X;
			int _channel = DefaultTokenChannel;
			IToken ID1=null;

			// Grammars\\ActionAnalysisLexer.g3:135:4: ( '$' ID {...}?)
			// Grammars\\ActionAnalysisLexer.g3:135:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID1Start84 = GetCharIndex();
			mID(); if (state.failed) return ;
			ID1 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ID1Start84, GetCharIndex()-1);
			if ( !((enclosingRule!=null && enclosingRule.GetRuleLabel((ID1!=null?ID1.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "X", "enclosingRule!=null && enclosingRule.GetRuleLabel($ID.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							Grammar.LabelElementPair pair = enclosingRule.GetRuleLabel((ID1!=null?ID1.Text:null));
							pair.actionReferencesLabel = true;
						
			}

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "X"

	// $ANTLR start "Y"
	private void mY()
	{
		try
		{
			int _type = Y;
			int _channel = DefaultTokenChannel;
			IToken ID2=null;

			// Grammars\\ActionAnalysisLexer.g3:144:4: ( '$' ID {...}?)
			// Grammars\\ActionAnalysisLexer.g3:144:4: '$' ID {...}?
			{
			Match('$'); if (state.failed) return ;
			int ID2Start106 = GetCharIndex();
			mID(); if (state.failed) return ;
			ID2 = new CommonToken(input, TokenConstants.InvalidTokenType, TokenConstants.DefaultChannel, ID2Start106, GetCharIndex()-1);
			if ( !((enclosingRule!=null && enclosingRule.GetLocalAttributeScope((ID2!=null?ID2.Text:null))!=null)) )
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				throw new FailedPredicateException(input, "Y", "enclosingRule!=null && enclosingRule.GetLocalAttributeScope($ID.text)!=null");
			}
			if ( state.backtracking==1 )
			{

							AttributeScope scope = enclosingRule.GetLocalAttributeScope((ID2!=null?ID2.Text:null));
							if ( scope!=null &&
								 (scope.isPredefinedRuleScope||scope.isPredefinedLexerRuleScope) )
							{
								grammar.ReferenceRuleLabelPredefinedAttribute(enclosingRule.name);
								//System.out.println("referenceRuleLabelPredefinedAttribute for "+(ID2!=null?ID2.Text:null));
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
	// $ANTLR end "Y"

	// $ANTLR start "ID"
	private void mID()
	{
		try
		{
			// Grammars\\ActionAnalysisLexer.g3:158:4: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
			// Grammars\\ActionAnalysisLexer.g3:158:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
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

			// Grammars\\ActionAnalysisLexer.g3:158:28: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( ((LA1_0>='0' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')) )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// Grammars\\ActionAnalysisLexer.g3:
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

		}
		finally
		{
		}
	}
	// $ANTLR end "ID"

	public override void mTokens()
	{
		// Grammars\\ActionAnalysisLexer.g3:1:41: ( X_Y | X | Y )
		int alt2=3;
		int LA2_0 = input.LA(1);

		if ( (LA2_0=='$') )
		{
			int LA2_1 = input.LA(2);

			if ( (EvaluatePredicate(synpred1_ActionAnalysisLexer_fragment)) )
			{
				alt2=1;
			}
			else if ( (EvaluatePredicate(synpred2_ActionAnalysisLexer_fragment)) )
			{
				alt2=2;
			}
			else if ( (true) )
			{
				alt2=3;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 2, 1, input);

				throw nvae;
			}
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
			// Grammars\\ActionAnalysisLexer.g3:1:41: X_Y
			{

			mX_Y(); if (state.failed) return ;

			}
			break;
		case 2:
			// Grammars\\ActionAnalysisLexer.g3:1:45: X
			{

			mX(); if (state.failed) return ;

			}
			break;
		case 3:
			// Grammars\\ActionAnalysisLexer.g3:1:47: Y
			{
			mY(); if (state.failed) return ;

			}
			break;

		}

	}

	// $ANTLR start synpred1_ActionAnalysisLexer
	public void synpred1_ActionAnalysisLexer_fragment()
	{
		// Grammars\\ActionAnalysisLexer.g3:1:0: ( X_Y )
		// Grammars\\ActionAnalysisLexer.g3:1:41: X_Y
		{
		mX_Y(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred1_ActionAnalysisLexer

	// $ANTLR start synpred2_ActionAnalysisLexer
	public void synpred2_ActionAnalysisLexer_fragment()
	{
		// Grammars\\ActionAnalysisLexer.g3:1:0: ( X )
		// Grammars\\ActionAnalysisLexer.g3:1:45: X
		{
		mX(); if (state.failed) return ;

		}
	}
	// $ANTLR end synpred2_ActionAnalysisLexer

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

	protected override void InitDFAs()
	{
		base.InitDFAs();
	}

 
	#endregion

}

} // namespace Antlr3.Grammars
