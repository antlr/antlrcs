// $ANTLR 3.1.2 Grammars\\ANTLRTreePrinter.g3 2009-03-20 14:32:48

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
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Antlr3.Grammars
{
/** Print out a grammar (no pretty printing).
 *
 *  Terence Parr
 *  University of San Francisco
 *  August 19, 2003
 */
public partial class ANTLRTreePrinter : TreeParser
{
	public static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACTION", "ACTION_CHAR_LITERAL", "ACTION_ESC", "ACTION_STRING_LITERAL", "ALT", "AMPERSAND", "ARG", "ARG_ACTION", "ARGLIST", "ASSIGN", "BACKTRACK_SEMPRED", "BANG", "BLOCK", "CATCH", "CHAR_LITERAL", "CHAR_RANGE", "CLOSE_ELEMENT_OPTION", "CLOSURE", "COLON", "COMBINED_GRAMMAR", "COMMA", "COMMENT", "DIGIT", "DOC_COMMENT", "DOLLAR", "DOT", "DOUBLE_ANGLE_STRING_LITERAL", "DOUBLE_QUOTE_STRING_LITERAL", "EOA", "EOB", "EOR", "EPSILON", "ESC", "ETC", "FINALLY", "FORCED_ACTION", "FRAGMENT", "GATED_SEMPRED", "GRAMMAR", "ID", "IMPLIES", "IMPORT", "INITACTION", "INT", "LABEL", "LEXER", "LEXER_GRAMMAR", "LPAREN", "ML_COMMENT", "NESTED_ACTION", "NESTED_ARG_ACTION", "NOT", "OPEN_ELEMENT_OPTION", "OPTIONAL", "OPTIONS", "OR", "PARSER", "PARSER_GRAMMAR", "PLUS", "PLUS_ASSIGN", "POSITIVE_CLOSURE", "PRIVATE", "PROTECTED", "PUBLIC", "QUESTION", "RANGE", "RCURLY", "RET", "RETURNS", "REWRITE", "ROOT", "RPAREN", "RULE", "RULE_REF", "SCOPE", "SEMI", "SEMPRED", "SL_COMMENT", "SRC", "STAR", "STRAY_BRACKET", "STRING_LITERAL", "SYN_SEMPRED", "SYNPRED", "TEMPLATE", "THROWS", "TOKEN_REF", "TOKENS", "TREE", "TREE_BEGIN", "TREE_GRAMMAR", "WILDCARD", "WS", "WS_LOOP", "WS_OPT", "XDIGIT"
	};
	public const int EOF=-1;
	public const int ACTION=4;
	public const int ACTION_CHAR_LITERAL=5;
	public const int ACTION_ESC=6;
	public const int ACTION_STRING_LITERAL=7;
	public const int ALT=8;
	public const int AMPERSAND=9;
	public const int ARG=10;
	public const int ARG_ACTION=11;
	public const int ARGLIST=12;
	public const int ASSIGN=13;
	public const int BACKTRACK_SEMPRED=14;
	public const int BANG=15;
	public const int BLOCK=16;
	public const int CATCH=17;
	public const int CHAR_LITERAL=18;
	public const int CHAR_RANGE=19;
	public const int CLOSE_ELEMENT_OPTION=20;
	public const int CLOSURE=21;
	public const int COLON=22;
	public const int COMBINED_GRAMMAR=23;
	public const int COMMA=24;
	public const int COMMENT=25;
	public const int DIGIT=26;
	public const int DOC_COMMENT=27;
	public const int DOLLAR=28;
	public const int DOT=29;
	public const int DOUBLE_ANGLE_STRING_LITERAL=30;
	public const int DOUBLE_QUOTE_STRING_LITERAL=31;
	public const int EOA=32;
	public const int EOB=33;
	public const int EOR=34;
	public const int EPSILON=35;
	public const int ESC=36;
	public const int ETC=37;
	public const int FINALLY=38;
	public const int FORCED_ACTION=39;
	public const int FRAGMENT=40;
	public const int GATED_SEMPRED=41;
	public const int GRAMMAR=42;
	public const int ID=43;
	public const int IMPLIES=44;
	public const int IMPORT=45;
	public const int INITACTION=46;
	public const int INT=47;
	public const int LABEL=48;
	public const int LEXER=49;
	public const int LEXER_GRAMMAR=50;
	public const int LPAREN=51;
	public const int ML_COMMENT=52;
	public const int NESTED_ACTION=53;
	public const int NESTED_ARG_ACTION=54;
	public const int NOT=55;
	public const int OPEN_ELEMENT_OPTION=56;
	public const int OPTIONAL=57;
	public const int OPTIONS=58;
	public const int OR=59;
	public const int PARSER=60;
	public const int PARSER_GRAMMAR=61;
	public const int PLUS=62;
	public const int PLUS_ASSIGN=63;
	public const int POSITIVE_CLOSURE=64;
	public const int PRIVATE=65;
	public const int PROTECTED=66;
	public const int PUBLIC=67;
	public const int QUESTION=68;
	public const int RANGE=69;
	public const int RCURLY=70;
	public const int RET=71;
	public const int RETURNS=72;
	public const int REWRITE=73;
	public const int ROOT=74;
	public const int RPAREN=75;
	public const int RULE=76;
	public const int RULE_REF=77;
	public const int SCOPE=78;
	public const int SEMI=79;
	public const int SEMPRED=80;
	public const int SL_COMMENT=81;
	public const int SRC=82;
	public const int STAR=83;
	public const int STRAY_BRACKET=84;
	public const int STRING_LITERAL=85;
	public const int SYN_SEMPRED=86;
	public const int SYNPRED=87;
	public const int TEMPLATE=88;
	public const int THROWS=89;
	public const int TOKEN_REF=90;
	public const int TOKENS=91;
	public const int TREE=92;
	public const int TREE_BEGIN=93;
	public const int TREE_GRAMMAR=94;
	public const int WILDCARD=95;
	public const int WS=96;
	public const int WS_LOOP=97;
	public const int WS_OPT=98;
	public const int XDIGIT=99;

	// delegates
	// delegators

	public ANTLRTreePrinter( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ANTLRTreePrinter( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return ANTLRTreePrinter.tokenNames; }
	public override string GrammarFileName { get { return "Grammars\\ANTLRTreePrinter.g3"; } }


	#region Rules

	// $ANTLR start "toString"
	// Grammars\\ANTLRTreePrinter.g3:89:0: public toString[Grammar g, bool showActions] returns [string s=null] : ( grammar_ | rule | alternative | element | single_rewrite | EOR ) ;
	public string toString( Grammar g, bool showActions )
	{

		string s = null;


			grammar = g;
			this.showActions = showActions;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:95:4: ( ( grammar_ | rule | alternative | element | single_rewrite | EOR ) )
			// Grammars\\ANTLRTreePrinter.g3:95:4: ( grammar_ | rule | alternative | element | single_rewrite | EOR )
			{
			// Grammars\\ANTLRTreePrinter.g3:95:4: ( grammar_ | rule | alternative | element | single_rewrite | EOR )
			int alt1=6;
			switch ( input.LA(1) )
			{
			case COMBINED_GRAMMAR:
			case LEXER_GRAMMAR:
			case PARSER_GRAMMAR:
			case TREE_GRAMMAR:
				{
				alt1=1;
				}
				break;
			case RULE:
				{
				alt1=2;
				}
				break;
			case ALT:
				{
				alt1=3;
				}
				break;
			case ACTION:
			case ASSIGN:
			case BACKTRACK_SEMPRED:
			case BANG:
			case BLOCK:
			case CHAR_LITERAL:
			case CHAR_RANGE:
			case CLOSURE:
			case DOT:
			case EPSILON:
			case FORCED_ACTION:
			case GATED_SEMPRED:
			case LABEL:
			case NOT:
			case OPTIONAL:
			case PLUS_ASSIGN:
			case POSITIVE_CLOSURE:
			case RANGE:
			case ROOT:
			case RULE_REF:
			case SEMPRED:
			case STRING_LITERAL:
			case SYN_SEMPRED:
			case SYNPRED:
			case TOKEN_REF:
			case TREE_BEGIN:
			case WILDCARD:
				{
				alt1=4;
				}
				break;
			case REWRITE:
				{
				alt1=5;
				}
				break;
			case EOR:
				{
				alt1=6;
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
				// Grammars\\ANTLRTreePrinter.g3:95:6: grammar_
				{
				PushFollow(Follow._grammar__in_toString72);
				grammar_();

				state._fsp--;


				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:96:5: rule
				{
				PushFollow(Follow._rule_in_toString78);
				rule();

				state._fsp--;


				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:97:5: alternative
				{
				PushFollow(Follow._alternative_in_toString84);
				alternative();

				state._fsp--;


				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:98:5: element
				{
				PushFollow(Follow._element_in_toString90);
				element();

				state._fsp--;


				}
				break;
			case 5:
				// Grammars\\ANTLRTreePrinter.g3:99:5: single_rewrite
				{
				PushFollow(Follow._single_rewrite_in_toString96);
				single_rewrite();

				state._fsp--;


				}
				break;
			case 6:
				// Grammars\\ANTLRTreePrinter.g3:100:5: EOR
				{
				Match(input,EOR,Follow._EOR_in_toString102); 
				s="EOR";

				}
				break;

			}

			return normalize(buf.ToString());

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
		return s;
	}
	// $ANTLR end "toString"


	// $ANTLR start "grammar_"
	// Grammars\\ANTLRTreePrinter.g3:107:0: grammar_ : ( ^( LEXER_GRAMMAR grammarSpec[\"lexer \" ] ) | ^( PARSER_GRAMMAR grammarSpec[\"parser \"] ) | ^( TREE_GRAMMAR grammarSpec[\"tree \"] ) | ^( COMBINED_GRAMMAR grammarSpec[\"\"] ) );
	private void grammar_(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:108:4: ( ^( LEXER_GRAMMAR grammarSpec[\"lexer \" ] ) | ^( PARSER_GRAMMAR grammarSpec[\"parser \"] ) | ^( TREE_GRAMMAR grammarSpec[\"tree \"] ) | ^( COMBINED_GRAMMAR grammarSpec[\"\"] ) )
			int alt2=4;
			switch ( input.LA(1) )
			{
			case LEXER_GRAMMAR:
				{
				alt2=1;
				}
				break;
			case PARSER_GRAMMAR:
				{
				alt2=2;
				}
				break;
			case TREE_GRAMMAR:
				{
				alt2=3;
				}
				break;
			case COMBINED_GRAMMAR:
				{
				alt2=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

					throw nvae;
				}
			}

			switch ( alt2 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:108:4: ^( LEXER_GRAMMAR grammarSpec[\"lexer \" ] )
				{
				Match(input,LEXER_GRAMMAR,Follow._LEXER_GRAMMAR_in_grammar_127); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._grammarSpec_in_grammar_129);
				grammarSpec("lexer ");

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:109:4: ^( PARSER_GRAMMAR grammarSpec[\"parser \"] )
				{
				Match(input,PARSER_GRAMMAR,Follow._PARSER_GRAMMAR_in_grammar_139); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._grammarSpec_in_grammar_141);
				grammarSpec("parser ");

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:110:4: ^( TREE_GRAMMAR grammarSpec[\"tree \"] )
				{
				Match(input,TREE_GRAMMAR,Follow._TREE_GRAMMAR_in_grammar_151); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._grammarSpec_in_grammar_153);
				grammarSpec("tree ");

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:111:4: ^( COMBINED_GRAMMAR grammarSpec[\"\"] )
				{
				Match(input,COMBINED_GRAMMAR,Follow._COMBINED_GRAMMAR_in_grammar_163); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._grammarSpec_in_grammar_165);
				grammarSpec("");

				state._fsp--;


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
		return ;
	}
	// $ANTLR end "grammar_"


	// $ANTLR start "attrScope"
	// Grammars\\ANTLRTreePrinter.g3:114:0: attrScope : ^( 'scope' ID ( ruleAction )* ACTION ) ;
	private void attrScope(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:115:4: ( ^( 'scope' ID ( ruleAction )* ACTION ) )
			// Grammars\\ANTLRTreePrinter.g3:115:4: ^( 'scope' ID ( ruleAction )* ACTION )
			{
			Match(input,SCOPE,Follow._SCOPE_in_attrScope181); 

			Match(input, TokenConstants.DOWN, null); 
			Match(input,ID,Follow._ID_in_attrScope183); 
			// Grammars\\ANTLRTreePrinter.g3:115:18: ( ruleAction )*
			for ( ; ; )
			{
				int alt3=2;
				int LA3_0 = input.LA(1);

				if ( (LA3_0==AMPERSAND) )
				{
					alt3=1;
				}


				switch ( alt3 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:115:0: ruleAction
					{
					PushFollow(Follow._ruleAction_in_attrScope185);
					ruleAction();

					state._fsp--;


					}
					break;

				default:
					goto loop3;
				}
			}

			loop3:
				;


			Match(input,ACTION,Follow._ACTION_in_attrScope188); 

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
	// $ANTLR end "attrScope"


	// $ANTLR start "grammarSpec"
	// Grammars\\ANTLRTreePrinter.g3:118:0: grammarSpec[string gtype] : id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules ;
	private void grammarSpec( string gtype )
	{
		GrammarAST id=null;
		GrammarAST cmt=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:119:4: (id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules )
			// Grammars\\ANTLRTreePrinter.g3:119:4: id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules
			{
			id=(GrammarAST)Match(input,ID,Follow._ID_in_grammarSpec204); 
			@out(gtype+"grammar "+(id!=null?id.Text:null));
			// Grammars\\ANTLRTreePrinter.g3:120:3: (cmt= DOC_COMMENT )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==DOC_COMMENT) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:120:4: cmt= DOC_COMMENT
				{
				cmt=(GrammarAST)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammarSpec213); 
				@out((cmt!=null?cmt.Text:null)+"\n");

				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:121:3: ( optionsSpec )?
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( (LA5_0==OPTIONS) )
			{
				alt5=1;
			}
			switch ( alt5 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:121:4: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_grammarSpec223);
				optionsSpec();

				state._fsp--;


				}
				break;

			}

			@out(";\n");
			// Grammars\\ANTLRTreePrinter.g3:122:3: ( delegateGrammars )?
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==IMPORT) )
			{
				alt6=1;
			}
			switch ( alt6 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:122:4: delegateGrammars
				{
				PushFollow(Follow._delegateGrammars_in_grammarSpec232);
				delegateGrammars();

				state._fsp--;


				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:123:3: ( tokensSpec )?
			int alt7=2;
			int LA7_0 = input.LA(1);

			if ( (LA7_0==TOKENS) )
			{
				alt7=1;
			}
			switch ( alt7 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:123:4: tokensSpec
				{
				PushFollow(Follow._tokensSpec_in_grammarSpec239);
				tokensSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:124:3: ( attrScope )*
			for ( ; ; )
			{
				int alt8=2;
				int LA8_0 = input.LA(1);

				if ( (LA8_0==SCOPE) )
				{
					alt8=1;
				}


				switch ( alt8 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:124:4: attrScope
					{
					PushFollow(Follow._attrScope_in_grammarSpec246);
					attrScope();

					state._fsp--;


					}
					break;

				default:
					goto loop8;
				}
			}

			loop8:
				;


			// Grammars\\ANTLRTreePrinter.g3:125:3: ( actions )?
			int alt9=2;
			int LA9_0 = input.LA(1);

			if ( (LA9_0==AMPERSAND) )
			{
				alt9=1;
			}
			switch ( alt9 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:125:4: actions
				{
				PushFollow(Follow._actions_in_grammarSpec253);
				actions();

				state._fsp--;


				}
				break;

			}

			PushFollow(Follow._rules_in_grammarSpec259);
			rules();

			state._fsp--;


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
	// $ANTLR end "grammarSpec"


	// $ANTLR start "actions"
	// Grammars\\ANTLRTreePrinter.g3:129:0: actions : ( action )+ ;
	private void actions(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:130:4: ( ( action )+ )
			// Grammars\\ANTLRTreePrinter.g3:130:4: ( action )+
			{
			// Grammars\\ANTLRTreePrinter.g3:130:4: ( action )+
			int cnt10=0;
			for ( ; ; )
			{
				int alt10=2;
				int LA10_0 = input.LA(1);

				if ( (LA10_0==AMPERSAND) )
				{
					alt10=1;
				}


				switch ( alt10 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:130:6: action
					{
					PushFollow(Follow._action_in_actions272);
					action();

					state._fsp--;


					}
					break;

				default:
					if ( cnt10 >= 1 )
						goto loop10;

					EarlyExitException eee10 = new EarlyExitException( 10, input );
					throw eee10;
				}
				cnt10++;
			}
			loop10:
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
	// $ANTLR end "actions"


	// $ANTLR start "action"
	// Grammars\\ANTLRTreePrinter.g3:133:0: action : ^( AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) ) ;
	private void action(  )
	{
		GrammarAST id1=null;
		GrammarAST id2=null;
		GrammarAST a1=null;
		GrammarAST a2=null;


			string scope=null, name=null;
			string action=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:139:4: ( ^( AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) ) )
			// Grammars\\ANTLRTreePrinter.g3:139:4: ^( AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) )
			{
			Match(input,AMPERSAND,Follow._AMPERSAND_in_action293); 

			Match(input, TokenConstants.DOWN, null); 
			id1=(GrammarAST)Match(input,ID,Follow._ID_in_action297); 
			// Grammars\\ANTLRTreePrinter.g3:140:4: (id2= ID a1= ACTION |a2= ACTION )
			int alt11=2;
			int LA11_0 = input.LA(1);

			if ( (LA11_0==ID) )
			{
				alt11=1;
			}
			else if ( (LA11_0==ACTION) )
			{
				alt11=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 11, 0, input);

				throw nvae;
			}
			switch ( alt11 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:140:6: id2= ID a1= ACTION
				{
				id2=(GrammarAST)Match(input,ID,Follow._ID_in_action306); 
				a1=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_action310); 
				scope=(id1!=null?id1.Text:null); name=(a1!=null?a1.Text:null); action=(a1!=null?a1.Text:null);

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:142:6: a2= ACTION
				{
				a2=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_action325); 
				scope=null; name=(id1!=null?id1.Text:null); action=(a2!=null?a2.Text:null);

				}
				break;

			}


			Match(input, TokenConstants.UP, null); 

						if ( showActions )
						{
							@out("@"+(scope!=null?scope+"::":"")+name+action);
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
	// $ANTLR end "action"


	// $ANTLR start "optionsSpec"
	// Grammars\\ANTLRTreePrinter.g3:154:0: optionsSpec : ^( OPTIONS ( option )+ ) ;
	private void optionsSpec(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:155:4: ( ^( OPTIONS ( option )+ ) )
			// Grammars\\ANTLRTreePrinter.g3:155:4: ^( OPTIONS ( option )+ )
			{
			Match(input,OPTIONS,Follow._OPTIONS_in_optionsSpec357); 

			@out(" options {");

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:156:4: ( option )+
			int cnt12=0;
			for ( ; ; )
			{
				int alt12=2;
				int LA12_0 = input.LA(1);

				if ( (LA12_0==ASSIGN) )
				{
					alt12=1;
				}


				switch ( alt12 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:156:5: option
					{
					PushFollow(Follow._option_in_optionsSpec365);
					option();

					state._fsp--;

					@out("; ");

					}
					break;

				default:
					if ( cnt12 >= 1 )
						goto loop12;

					EarlyExitException eee12 = new EarlyExitException( 12, input );
					throw eee12;
				}
				cnt12++;
			}
			loop12:
				;


			@out("} ");

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
	// $ANTLR end "optionsSpec"


	// $ANTLR start "option"
	// Grammars\\ANTLRTreePrinter.g3:161:0: option : ^( ASSIGN id= ID optionValue ) ;
	private void option(  )
	{
		GrammarAST id=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:162:4: ( ^( ASSIGN id= ID optionValue ) )
			// Grammars\\ANTLRTreePrinter.g3:162:4: ^( ASSIGN id= ID optionValue )
			{
			Match(input,ASSIGN,Follow._ASSIGN_in_option391); 

			Match(input, TokenConstants.DOWN, null); 
			id=(GrammarAST)Match(input,ID,Follow._ID_in_option395); 
			@out((id!=null?id.Text:null)+"=");
			PushFollow(Follow._optionValue_in_option399);
			optionValue();

			state._fsp--;


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
	// $ANTLR end "option"


	// $ANTLR start "optionValue"
	// Grammars\\ANTLRTreePrinter.g3:165:0: optionValue : (id= ID |s= STRING_LITERAL |c= CHAR_LITERAL |i= INT );
	private void optionValue(  )
	{
		GrammarAST id=null;
		GrammarAST s=null;
		GrammarAST c=null;
		GrammarAST i=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:166:4: (id= ID |s= STRING_LITERAL |c= CHAR_LITERAL |i= INT )
			int alt13=4;
			switch ( input.LA(1) )
			{
			case ID:
				{
				alt13=1;
				}
				break;
			case STRING_LITERAL:
				{
				alt13=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt13=3;
				}
				break;
			case INT:
				{
				alt13=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

					throw nvae;
				}
			}

			switch ( alt13 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:166:4: id= ID
				{
				id=(GrammarAST)Match(input,ID,Follow._ID_in_optionValue414); 
				@out((id!=null?id.Text:null));

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:167:4: s= STRING_LITERAL
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_optionValue434); 
				@out((s!=null?s.Text:null));

				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:168:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_optionValue443); 
				@out((c!=null?c.Text:null));

				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:169:4: i= INT
				{
				i=(GrammarAST)Match(input,INT,Follow._INT_in_optionValue454); 
				@out((i!=null?i.Text:null));

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
	// $ANTLR end "optionValue"


	// $ANTLR start "delegateGrammars"
	// Grammars\\ANTLRTreePrinter.g3:185:0: delegateGrammars : ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) ;
	private void delegateGrammars(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:186:4: ( ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) )
			// Grammars\\ANTLRTreePrinter.g3:186:4: ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ )
			{
			Match(input,IMPORT,Follow._IMPORT_in_delegateGrammars484); 

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:186:16: ( ^( ASSIGN ID ID ) | ID )+
			int cnt14=0;
			for ( ; ; )
			{
				int alt14=3;
				int LA14_0 = input.LA(1);

				if ( (LA14_0==ASSIGN) )
				{
					alt14=1;
				}
				else if ( (LA14_0==ID) )
				{
					alt14=2;
				}


				switch ( alt14 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:186:18: ^( ASSIGN ID ID )
					{
					Match(input,ASSIGN,Follow._ASSIGN_in_delegateGrammars489); 

					Match(input, TokenConstants.DOWN, null); 
					Match(input,ID,Follow._ID_in_delegateGrammars491); 
					Match(input,ID,Follow._ID_in_delegateGrammars493); 

					Match(input, TokenConstants.UP, null); 

					}
					break;
				case 2:
					// Grammars\\ANTLRTreePrinter.g3:186:36: ID
					{
					Match(input,ID,Follow._ID_in_delegateGrammars498); 

					}
					break;

				default:
					if ( cnt14 >= 1 )
						goto loop14;

					EarlyExitException eee14 = new EarlyExitException( 14, input );
					throw eee14;
				}
				cnt14++;
			}
			loop14:
				;



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
	// $ANTLR end "delegateGrammars"


	// $ANTLR start "tokensSpec"
	// Grammars\\ANTLRTreePrinter.g3:189:0: tokensSpec : ^( TOKENS ( tokenSpec )+ ) ;
	private void tokensSpec(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:190:4: ( ^( TOKENS ( tokenSpec )+ ) )
			// Grammars\\ANTLRTreePrinter.g3:190:4: ^( TOKENS ( tokenSpec )+ )
			{
			Match(input,TOKENS,Follow._TOKENS_in_tokensSpec516); 

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:190:14: ( tokenSpec )+
			int cnt15=0;
			for ( ; ; )
			{
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( (LA15_0==ASSIGN||LA15_0==TOKEN_REF) )
				{
					alt15=1;
				}


				switch ( alt15 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:190:16: tokenSpec
					{
					PushFollow(Follow._tokenSpec_in_tokensSpec520);
					tokenSpec();

					state._fsp--;


					}
					break;

				default:
					if ( cnt15 >= 1 )
						goto loop15;

					EarlyExitException eee15 = new EarlyExitException( 15, input );
					throw eee15;
				}
				cnt15++;
			}
			loop15:
				;



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
	// $ANTLR end "tokensSpec"


	// $ANTLR start "tokenSpec"
	// Grammars\\ANTLRTreePrinter.g3:193:0: tokenSpec : ( TOKEN_REF | ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) ) );
	private void tokenSpec(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:194:4: ( TOKEN_REF | ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) ) )
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( (LA16_0==TOKEN_REF) )
			{
				alt16=1;
			}
			else if ( (LA16_0==ASSIGN) )
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
				// Grammars\\ANTLRTreePrinter.g3:194:4: TOKEN_REF
				{
				Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec536); 

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:195:4: ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_tokenSpec543); 

				Match(input, TokenConstants.DOWN, null); 
				Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec545); 
				if ( input.LA(1)==CHAR_LITERAL||input.LA(1)==STRING_LITERAL )
				{
					input.Consume();
					state.errorRecovery=false;
				}
				else
				{
					MismatchedSetException mse = new MismatchedSetException(null,input);
					throw mse;
				}


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
		return ;
	}
	// $ANTLR end "tokenSpec"


	// $ANTLR start "rules"
	// Grammars\\ANTLRTreePrinter.g3:198:0: rules : ( rule )+ ;
	private void rules(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:199:4: ( ( rule )+ )
			// Grammars\\ANTLRTreePrinter.g3:199:4: ( rule )+
			{
			// Grammars\\ANTLRTreePrinter.g3:199:4: ( rule )+
			int cnt17=0;
			for ( ; ; )
			{
				int alt17=2;
				int LA17_0 = input.LA(1);

				if ( (LA17_0==RULE) )
				{
					alt17=1;
				}


				switch ( alt17 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:199:6: rule
					{
					PushFollow(Follow._rule_in_rules566);
					rule();

					state._fsp--;


					}
					break;

				default:
					if ( cnt17 >= 1 )
						goto loop17;

					EarlyExitException eee17 = new EarlyExitException( 17, input );
					throw eee17;
				}
				cnt17++;
			}
			loop17:
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
	// $ANTLR end "rules"


	// $ANTLR start "rule"
	// Grammars\\ANTLRTreePrinter.g3:202:0: rule : ^( RULE id= ID ( modifier )? ^( ARG (arg= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ruleAction )* b= block[false] ( exceptionGroup )? EOR ) ;
	private void rule(  )
	{
		GrammarAST id=null;
		GrammarAST arg=null;
		GrammarAST ret=null;
		ANTLRTreePrinter.block_return b = default(ANTLRTreePrinter.block_return);

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:203:4: ( ^( RULE id= ID ( modifier )? ^( ARG (arg= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ruleAction )* b= block[false] ( exceptionGroup )? EOR ) )
			// Grammars\\ANTLRTreePrinter.g3:203:4: ^( RULE id= ID ( modifier )? ^( ARG (arg= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec )? ( ruleAction )* b= block[false] ( exceptionGroup )? EOR )
			{
			Match(input,RULE,Follow._RULE_in_rule582); 

			Match(input, TokenConstants.DOWN, null); 
			id=(GrammarAST)Match(input,ID,Follow._ID_in_rule586); 
			// Grammars\\ANTLRTreePrinter.g3:204:4: ( modifier )?
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( (LA18_0==FRAGMENT||(LA18_0>=PRIVATE && LA18_0<=PUBLIC)) )
			{
				alt18=1;
			}
			switch ( alt18 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:204:5: modifier
				{
				PushFollow(Follow._modifier_in_rule592);
				modifier();

				state._fsp--;


				}
				break;

			}

			@out((id!=null?id.Text:null));
			Match(input,ARG,Follow._ARG_in_rule605); 

			if ( input.LA(1)==TokenConstants.DOWN )
			{
				Match(input, TokenConstants.DOWN, null); 
				// Grammars\\ANTLRTreePrinter.g3:206:10: (arg= ARG_ACTION )?
				int alt19=2;
				int LA19_0 = input.LA(1);

				if ( (LA19_0==ARG_ACTION) )
				{
					alt19=1;
				}
				switch ( alt19 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:206:11: arg= ARG_ACTION
					{
					arg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule610); 
					@out("["+(arg!=null?arg.Text:null)+"]");

					}
					break;

				}


				Match(input, TokenConstants.UP, null); 
			}
			Match(input,RET,Follow._RET_in_rule623); 

			if ( input.LA(1)==TokenConstants.DOWN )
			{
				Match(input, TokenConstants.DOWN, null); 
				// Grammars\\ANTLRTreePrinter.g3:207:10: (ret= ARG_ACTION )?
				int alt20=2;
				int LA20_0 = input.LA(1);

				if ( (LA20_0==ARG_ACTION) )
				{
					alt20=1;
				}
				switch ( alt20 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:207:11: ret= ARG_ACTION
					{
					ret=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule628); 
					@out(" returns ["+(ret!=null?ret.Text:null)+"]");

					}
					break;

				}


				Match(input, TokenConstants.UP, null); 
			}
			// Grammars\\ANTLRTreePrinter.g3:208:4: ( throwsSpec )?
			int alt21=2;
			int LA21_0 = input.LA(1);

			if ( (LA21_0==THROWS) )
			{
				alt21=1;
			}
			switch ( alt21 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:208:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule641);
				throwsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:209:4: ( optionsSpec )?
			int alt22=2;
			int LA22_0 = input.LA(1);

			if ( (LA22_0==OPTIONS) )
			{
				alt22=1;
			}
			switch ( alt22 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:209:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_rule649);
				optionsSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:210:4: ( ruleScopeSpec )?
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0==SCOPE) )
			{
				alt23=1;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:210:5: ruleScopeSpec
				{
				PushFollow(Follow._ruleScopeSpec_in_rule657);
				ruleScopeSpec();

				state._fsp--;


				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:211:4: ( ruleAction )*
			for ( ; ; )
			{
				int alt24=2;
				int LA24_0 = input.LA(1);

				if ( (LA24_0==AMPERSAND) )
				{
					alt24=1;
				}


				switch ( alt24 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:211:5: ruleAction
					{
					PushFollow(Follow._ruleAction_in_rule665);
					ruleAction();

					state._fsp--;


					}
					break;

				default:
					goto loop24;
				}
			}

			loop24:
				;


			@out(" :");

							if ( input.LA(5) == NOT || input.LA(5) == ASSIGN )
								@out(" ");
						
			PushFollow(Follow._block_in_rule684);
			b=block(false);

			state._fsp--;

			// Grammars\\ANTLRTreePrinter.g3:218:4: ( exceptionGroup )?
			int alt25=2;
			int LA25_0 = input.LA(1);

			if ( (LA25_0==CATCH||LA25_0==FINALLY) )
			{
				alt25=1;
			}
			switch ( alt25 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:218:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_rule691);
				exceptionGroup();

				state._fsp--;


				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_rule698); 
			@out(";\n");

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
	// $ANTLR end "rule"


	// $ANTLR start "ruleAction"
	// Grammars\\ANTLRTreePrinter.g3:223:0: ruleAction : ^( AMPERSAND id= ID a= ACTION ) ;
	private void ruleAction(  )
	{
		GrammarAST id=null;
		GrammarAST a=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:224:4: ( ^( AMPERSAND id= ID a= ACTION ) )
			// Grammars\\ANTLRTreePrinter.g3:224:4: ^( AMPERSAND id= ID a= ACTION )
			{
			Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleAction716); 

			Match(input, TokenConstants.DOWN, null); 
			id=(GrammarAST)Match(input,ID,Follow._ID_in_ruleAction720); 
			a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_ruleAction724); 

			Match(input, TokenConstants.UP, null); 
			if ( showActions ) @out("@"+(id!=null?id.Text:null)+"{"+(a!=null?a.Text:null)+"}");

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
	// $ANTLR end "ruleAction"

	public class modifier_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "modifier"
	// Grammars\\ANTLRTreePrinter.g3:228:0: modifier : ( 'protected' | 'public' | 'private' | 'fragment' );
	private ANTLRTreePrinter.modifier_return modifier(  )
	{
		ANTLRTreePrinter.modifier_return retval = new ANTLRTreePrinter.modifier_return();
		retval.start = input.LT(1);

		@out(((GrammarAST)retval.start).Text); @out(" ");
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:231:4: ( 'protected' | 'public' | 'private' | 'fragment' )
			// Grammars\\ANTLRTreePrinter.g3:
			{
			if ( input.LA(1)==FRAGMENT||(input.LA(1)>=PRIVATE && input.LA(1)<=PUBLIC) )
			{
				input.Consume();
				state.errorRecovery=false;
			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				throw mse;
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
		return retval;
	}
	// $ANTLR end "modifier"


	// $ANTLR start "throwsSpec"
	// Grammars\\ANTLRTreePrinter.g3:237:0: throwsSpec : ^( 'throws' ( ID )+ ) ;
	private void throwsSpec(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:238:4: ( ^( 'throws' ( ID )+ ) )
			// Grammars\\ANTLRTreePrinter.g3:238:4: ^( 'throws' ( ID )+ )
			{
			Match(input,THROWS,Follow._THROWS_in_throwsSpec773); 

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:238:15: ( ID )+
			int cnt26=0;
			for ( ; ; )
			{
				int alt26=2;
				int LA26_0 = input.LA(1);

				if ( (LA26_0==ID) )
				{
					alt26=1;
				}


				switch ( alt26 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:238:0: ID
					{
					Match(input,ID,Follow._ID_in_throwsSpec775); 

					}
					break;

				default:
					if ( cnt26 >= 1 )
						goto loop26;

					EarlyExitException eee26 = new EarlyExitException( 26, input );
					throw eee26;
				}
				cnt26++;
			}
			loop26:
				;



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
	// $ANTLR end "throwsSpec"


	// $ANTLR start "ruleScopeSpec"
	// Grammars\\ANTLRTreePrinter.g3:241:0: ruleScopeSpec : ^( 'scope' ( ruleAction )* ( ACTION )? ( ID )* ) ;
	private void ruleScopeSpec(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:242:4: ( ^( 'scope' ( ruleAction )* ( ACTION )? ( ID )* ) )
			// Grammars\\ANTLRTreePrinter.g3:242:4: ^( 'scope' ( ruleAction )* ( ACTION )? ( ID )* )
			{
			Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec790); 

			if ( input.LA(1)==TokenConstants.DOWN )
			{
				Match(input, TokenConstants.DOWN, null); 
				// Grammars\\ANTLRTreePrinter.g3:242:15: ( ruleAction )*
				for ( ; ; )
				{
					int alt27=2;
					int LA27_0 = input.LA(1);

					if ( (LA27_0==AMPERSAND) )
					{
						alt27=1;
					}


					switch ( alt27 )
					{
					case 1:
						// Grammars\\ANTLRTreePrinter.g3:242:0: ruleAction
						{
						PushFollow(Follow._ruleAction_in_ruleScopeSpec792);
						ruleAction();

						state._fsp--;


						}
						break;

					default:
						goto loop27;
					}
				}

				loop27:
					;


				// Grammars\\ANTLRTreePrinter.g3:242:27: ( ACTION )?
				int alt28=2;
				int LA28_0 = input.LA(1);

				if ( (LA28_0==ACTION) )
				{
					alt28=1;
				}
				switch ( alt28 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:242:28: ACTION
					{
					Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec796); 

					}
					break;

				}

				// Grammars\\ANTLRTreePrinter.g3:242:37: ( ID )*
				for ( ; ; )
				{
					int alt29=2;
					int LA29_0 = input.LA(1);

					if ( (LA29_0==ID) )
					{
						alt29=1;
					}


					switch ( alt29 )
					{
					case 1:
						// Grammars\\ANTLRTreePrinter.g3:242:39: ID
						{
						Match(input,ID,Follow._ID_in_ruleScopeSpec802); 

						}
						break;

					default:
						goto loop29;
					}
				}

				loop29:
					;



				Match(input, TokenConstants.UP, null); 
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
	// $ANTLR end "ruleScopeSpec"

	public class block_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "block"
	// Grammars\\ANTLRTreePrinter.g3:245:0: block[bool forceParens] : ^( BLOCK ( optionsSpec )? alternative rewrite ( alternative rewrite )* EOB ) ;
	private ANTLRTreePrinter.block_return block( bool forceParens )
	{
		ANTLRTreePrinter.block_return retval = new ANTLRTreePrinter.block_return();
		retval.start = input.LT(1);


		int numAlts = countAltsForBlock(((GrammarAST)retval.start));

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:250:4: ( ^( BLOCK ( optionsSpec )? alternative rewrite ( alternative rewrite )* EOB ) )
			// Grammars\\ANTLRTreePrinter.g3:250:4: ^( BLOCK ( optionsSpec )? alternative rewrite ( alternative rewrite )* EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_block826); 


							if ( forceParens||numAlts>1 )
							{
								//for ( Antlr.Runtime.Tree.Tree parent = ((GrammarAST)retval.start).getParent(); parent != null && parent.getType() != RULE; parent = parent.getParent() )
								//{
								//	if ( parent.getType() == BLOCK && countAltsForBlock((GrammarAST)parent) > 1 )
								//	{
								//		@out(" ");
								//		break;
								//	}
								//}
								@out(" (");
							}
						

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:265:4: ( optionsSpec )?
			int alt30=2;
			int LA30_0 = input.LA(1);

			if ( (LA30_0==OPTIONS) )
			{
				alt30=1;
			}
			switch ( alt30 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:265:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_block837);
				optionsSpec();

				state._fsp--;

				@out(" :");

				}
				break;

			}

			PushFollow(Follow._alternative_in_block847);
			alternative();

			state._fsp--;

			PushFollow(Follow._rewrite_in_block849);
			rewrite();

			state._fsp--;

			// Grammars\\ANTLRTreePrinter.g3:266:24: ( alternative rewrite )*
			for ( ; ; )
			{
				int alt31=2;
				int LA31_0 = input.LA(1);

				if ( (LA31_0==ALT) )
				{
					alt31=1;
				}


				switch ( alt31 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:266:26: alternative rewrite
					{
					@out("|");
					PushFollow(Follow._alternative_in_block855);
					alternative();

					state._fsp--;

					PushFollow(Follow._rewrite_in_block857);
					rewrite();

					state._fsp--;


					}
					break;

				default:
					goto loop31;
				}
			}

			loop31:
				;


			Match(input,EOB,Follow._EOB_in_block865); 
			if ( forceParens||numAlts>1 ) @out(")");

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
		return retval;
	}
	// $ANTLR end "block"


	// $ANTLR start "alternative"
	// Grammars\\ANTLRTreePrinter.g3:271:0: alternative : ^( ALT ( element )+ EOA ) ;
	private void alternative(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:272:4: ( ^( ALT ( element )+ EOA ) )
			// Grammars\\ANTLRTreePrinter.g3:272:4: ^( ALT ( element )+ EOA )
			{
			Match(input,ALT,Follow._ALT_in_alternative887); 

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:272:11: ( element )+
			int cnt32=0;
			for ( ; ; )
			{
				int alt32=2;
				int LA32_0 = input.LA(1);

				if ( (LA32_0==ACTION||(LA32_0>=ASSIGN && LA32_0<=BLOCK)||(LA32_0>=CHAR_LITERAL && LA32_0<=CHAR_RANGE)||LA32_0==CLOSURE||LA32_0==DOT||LA32_0==EPSILON||LA32_0==FORCED_ACTION||LA32_0==GATED_SEMPRED||LA32_0==LABEL||LA32_0==NOT||LA32_0==OPTIONAL||(LA32_0>=PLUS_ASSIGN && LA32_0<=POSITIVE_CLOSURE)||LA32_0==RANGE||LA32_0==ROOT||LA32_0==RULE_REF||LA32_0==SEMPRED||(LA32_0>=STRING_LITERAL && LA32_0<=SYNPRED)||LA32_0==TOKEN_REF||LA32_0==TREE_BEGIN||LA32_0==WILDCARD) )
				{
					alt32=1;
				}


				switch ( alt32 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:272:12: element
					{
					PushFollow(Follow._element_in_alternative890);
					element();

					state._fsp--;


					}
					break;

				default:
					if ( cnt32 >= 1 )
						goto loop32;

					EarlyExitException eee32 = new EarlyExitException( 32, input );
					throw eee32;
				}
				cnt32++;
			}
			loop32:
				;


			Match(input,EOA,Follow._EOA_in_alternative894); 

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
	// $ANTLR end "alternative"


	// $ANTLR start "exceptionGroup"
	// Grammars\\ANTLRTreePrinter.g3:275:0: exceptionGroup : ( ( exceptionHandler )+ ( finallyClause )? | finallyClause );
	private void exceptionGroup(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:276:4: ( ( exceptionHandler )+ ( finallyClause )? | finallyClause )
			int alt35=2;
			int LA35_0 = input.LA(1);

			if ( (LA35_0==CATCH) )
			{
				alt35=1;
			}
			else if ( (LA35_0==FINALLY) )
			{
				alt35=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 35, 0, input);

				throw nvae;
			}
			switch ( alt35 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:276:4: ( exceptionHandler )+ ( finallyClause )?
				{
				// Grammars\\ANTLRTreePrinter.g3:276:4: ( exceptionHandler )+
				int cnt33=0;
				for ( ; ; )
				{
					int alt33=2;
					int LA33_0 = input.LA(1);

					if ( (LA33_0==CATCH) )
					{
						alt33=1;
					}


					switch ( alt33 )
					{
					case 1:
						// Grammars\\ANTLRTreePrinter.g3:276:6: exceptionHandler
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup909);
						exceptionHandler();

						state._fsp--;


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


				// Grammars\\ANTLRTreePrinter.g3:276:26: ( finallyClause )?
				int alt34=2;
				int LA34_0 = input.LA(1);

				if ( (LA34_0==FINALLY) )
				{
					alt34=1;
				}
				switch ( alt34 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:276:27: finallyClause
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup915);
					finallyClause();

					state._fsp--;


					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:277:4: finallyClause
				{
				PushFollow(Follow._finallyClause_in_exceptionGroup922);
				finallyClause();

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
	// $ANTLR end "exceptionGroup"


	// $ANTLR start "exceptionHandler"
	// Grammars\\ANTLRTreePrinter.g3:280:0: exceptionHandler : ^( 'catch' ARG_ACTION ACTION ) ;
	private void exceptionHandler(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:281:4: ( ^( 'catch' ARG_ACTION ACTION ) )
			// Grammars\\ANTLRTreePrinter.g3:281:4: ^( 'catch' ARG_ACTION ACTION )
			{
			Match(input,CATCH,Follow._CATCH_in_exceptionHandler934); 

			Match(input, TokenConstants.DOWN, null); 
			Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler936); 
			Match(input,ACTION,Follow._ACTION_in_exceptionHandler938); 

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
	// $ANTLR end "exceptionHandler"


	// $ANTLR start "finallyClause"
	// Grammars\\ANTLRTreePrinter.g3:284:0: finallyClause : ^( 'finally' ACTION ) ;
	private void finallyClause(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:285:4: ( ^( 'finally' ACTION ) )
			// Grammars\\ANTLRTreePrinter.g3:285:4: ^( 'finally' ACTION )
			{
			Match(input,FINALLY,Follow._FINALLY_in_finallyClause951); 

			Match(input, TokenConstants.DOWN, null); 
			Match(input,ACTION,Follow._ACTION_in_finallyClause953); 

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
	// $ANTLR end "finallyClause"


	// $ANTLR start "single_rewrite"
	// Grammars\\ANTLRTreePrinter.g3:288:0: single_rewrite : ^( REWRITE ( SEMPRED )? ( alternative | rewrite_template | ETC | ACTION ) ) ;
	private void single_rewrite(  )
	{
		GrammarAST SEMPRED1=null;
		GrammarAST ACTION2=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:289:4: ( ^( REWRITE ( SEMPRED )? ( alternative | rewrite_template | ETC | ACTION ) ) )
			// Grammars\\ANTLRTreePrinter.g3:289:4: ^( REWRITE ( SEMPRED )? ( alternative | rewrite_template | ETC | ACTION ) )
			{
			Match(input,REWRITE,Follow._REWRITE_in_single_rewrite967); 

			@out(" ->");

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:290:4: ( SEMPRED )?
			int alt36=2;
			int LA36_0 = input.LA(1);

			if ( (LA36_0==SEMPRED) )
			{
				alt36=1;
			}
			switch ( alt36 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:290:6: SEMPRED
				{
				SEMPRED1=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_single_rewrite976); 
				@out(" {"+(SEMPRED1!=null?SEMPRED1.Text:null)+"}?");

				}
				break;

			}

			// Grammars\\ANTLRTreePrinter.g3:292:4: ( alternative | rewrite_template | ETC | ACTION )
			int alt37=4;
			switch ( input.LA(1) )
			{
			case ALT:
				{
				alt37=1;
				}
				break;
			case TEMPLATE:
				{
				alt37=2;
				}
				break;
			case ETC:
				{
				alt37=3;
				}
				break;
			case ACTION:
				{
				alt37=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 37, 0, input);

					throw nvae;
				}
			}

			switch ( alt37 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:292:6: alternative
				{
				PushFollow(Follow._alternative_in_single_rewrite991);
				alternative();

				state._fsp--;


				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:293:6: rewrite_template
				{
				PushFollow(Follow._rewrite_template_in_single_rewrite998);
				rewrite_template();

				state._fsp--;


				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:294:6: ETC
				{
				Match(input,ETC,Follow._ETC_in_single_rewrite1005); 
				@out("...");

				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:295:6: ACTION
				{
				ACTION2=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_single_rewrite1014); 
				@out(" {"+(ACTION2!=null?ACTION2.Text:null)+"}");

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
	// $ANTLR end "single_rewrite"


	// $ANTLR start "rewrite_template"
	// Grammars\\ANTLRTreePrinter.g3:300:0: rewrite_template : ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) ;
	private void rewrite_template(  )
	{
		GrammarAST id=null;
		GrammarAST ind=null;
		GrammarAST arg=null;
		GrammarAST a=null;
		GrammarAST DOUBLE_QUOTE_STRING_LITERAL3=null;
		GrammarAST DOUBLE_ANGLE_STRING_LITERAL4=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:301:4: ( ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) )
			// Grammars\\ANTLRTreePrinter.g3:301:4: ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? )
			{
			Match(input,TEMPLATE,Follow._TEMPLATE_in_rewrite_template1038); 

			Match(input, TokenConstants.DOWN, null); 
			// Grammars\\ANTLRTreePrinter.g3:302:4: (id= ID |ind= ACTION )
			int alt38=2;
			int LA38_0 = input.LA(1);

			if ( (LA38_0==ID) )
			{
				alt38=1;
			}
			else if ( (LA38_0==ACTION) )
			{
				alt38=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 38, 0, input);

				throw nvae;
			}
			switch ( alt38 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:302:6: id= ID
				{
				id=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template1047); 
				@out(" "+(id!=null?id.Text:null));

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:303:6: ind= ACTION
				{
				ind=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template1058); 
				@out(" ({"+(ind!=null?ind.Text:null)+"})");

				}
				break;

			}

			Match(input,ARGLIST,Follow._ARGLIST_in_rewrite_template1072); 

			@out("(");

			if ( input.LA(1)==TokenConstants.DOWN )
			{
				Match(input, TokenConstants.DOWN, null); 
				// Grammars\\ANTLRTreePrinter.g3:307:5: ( ^( ARG arg= ID a= ACTION ) )*
				for ( ; ; )
				{
					int alt39=2;
					int LA39_0 = input.LA(1);

					if ( (LA39_0==ARG) )
					{
						alt39=1;
					}


					switch ( alt39 )
					{
					case 1:
						// Grammars\\ANTLRTreePrinter.g3:307:7: ^( ARG arg= ID a= ACTION )
						{
						Match(input,ARG,Follow._ARG_in_rewrite_template1088); 

						Match(input, TokenConstants.DOWN, null); 
						arg=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template1092); 
						@out((arg!=null?arg.Text:null)+"=");
						a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template1104); 
						@out((a!=null?a.Text:null));

						Match(input, TokenConstants.UP, null); 

						}
						break;

					default:
						goto loop39;
					}
				}

				loop39:
					;


				@out(")");

				Match(input, TokenConstants.UP, null); 
			}
			// Grammars\\ANTLRTreePrinter.g3:313:4: ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )?
			int alt40=3;
			int LA40_0 = input.LA(1);

			if ( (LA40_0==DOUBLE_QUOTE_STRING_LITERAL) )
			{
				alt40=1;
			}
			else if ( (LA40_0==DOUBLE_ANGLE_STRING_LITERAL) )
			{
				alt40=2;
			}
			switch ( alt40 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:313:6: DOUBLE_QUOTE_STRING_LITERAL
				{
				DOUBLE_QUOTE_STRING_LITERAL3=(GrammarAST)Match(input,DOUBLE_QUOTE_STRING_LITERAL,Follow._DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template1140); 
				@out(" "+(DOUBLE_QUOTE_STRING_LITERAL3!=null?DOUBLE_QUOTE_STRING_LITERAL3.Text:null));

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:314:6: DOUBLE_ANGLE_STRING_LITERAL
				{
				DOUBLE_ANGLE_STRING_LITERAL4=(GrammarAST)Match(input,DOUBLE_ANGLE_STRING_LITERAL,Follow._DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template1149); 
				@out(" "+(DOUBLE_ANGLE_STRING_LITERAL4!=null?DOUBLE_ANGLE_STRING_LITERAL4.Text:null));

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
	// $ANTLR end "rewrite_template"


	// $ANTLR start "rewrite"
	// Grammars\\ANTLRTreePrinter.g3:319:0: rewrite : ( single_rewrite )* ;
	private void rewrite(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:320:4: ( ( single_rewrite )* )
			// Grammars\\ANTLRTreePrinter.g3:320:4: ( single_rewrite )*
			{
			// Grammars\\ANTLRTreePrinter.g3:320:4: ( single_rewrite )*
			for ( ; ; )
			{
				int alt41=2;
				int LA41_0 = input.LA(1);

				if ( (LA41_0==REWRITE) )
				{
					alt41=1;
				}


				switch ( alt41 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:320:5: single_rewrite
					{
					PushFollow(Follow._single_rewrite_in_rewrite1173);
					single_rewrite();

					state._fsp--;


					}
					break;

				default:
					goto loop41;
				}
			}

			loop41:
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
	// $ANTLR end "rewrite"


	// $ANTLR start "element"
	// Grammars\\ANTLRTreePrinter.g3:323:0: element : ( ^( ROOT element ) | ^( BANG element ) | atom | ^( NOT element ) | ^( RANGE atom atom ) | ^( CHAR_RANGE atom atom ) | ^( ASSIGN id= ID element ) | ^( PLUS_ASSIGN id2= ID element ) | ebnf | tree_ | ^( SYNPRED block[true] ) |a= ACTION |a2= FORCED_ACTION |pred= SEMPRED |spred= SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) |gpred= GATED_SEMPRED | EPSILON );
	private void element(  )
	{
		GrammarAST id=null;
		GrammarAST id2=null;
		GrammarAST a=null;
		GrammarAST a2=null;
		GrammarAST pred=null;
		GrammarAST spred=null;
		GrammarAST gpred=null;

		try
		{
			// Grammars\\ANTLRTreePrinter.g3:324:4: ( ^( ROOT element ) | ^( BANG element ) | atom | ^( NOT element ) | ^( RANGE atom atom ) | ^( CHAR_RANGE atom atom ) | ^( ASSIGN id= ID element ) | ^( PLUS_ASSIGN id2= ID element ) | ebnf | tree_ | ^( SYNPRED block[true] ) |a= ACTION |a2= FORCED_ACTION |pred= SEMPRED |spred= SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) |gpred= GATED_SEMPRED | EPSILON )
			int alt43=18;
			switch ( input.LA(1) )
			{
			case ROOT:
				{
				alt43=1;
				}
				break;
			case BANG:
				{
				alt43=2;
				}
				break;
			case CHAR_LITERAL:
			case DOT:
			case LABEL:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
			case WILDCARD:
				{
				alt43=3;
				}
				break;
			case NOT:
				{
				alt43=4;
				}
				break;
			case RANGE:
				{
				alt43=5;
				}
				break;
			case CHAR_RANGE:
				{
				alt43=6;
				}
				break;
			case ASSIGN:
				{
				alt43=7;
				}
				break;
			case PLUS_ASSIGN:
				{
				alt43=8;
				}
				break;
			case BLOCK:
			case CLOSURE:
			case OPTIONAL:
			case POSITIVE_CLOSURE:
				{
				alt43=9;
				}
				break;
			case TREE_BEGIN:
				{
				alt43=10;
				}
				break;
			case SYNPRED:
				{
				alt43=11;
				}
				break;
			case ACTION:
				{
				alt43=12;
				}
				break;
			case FORCED_ACTION:
				{
				alt43=13;
				}
				break;
			case SEMPRED:
				{
				alt43=14;
				}
				break;
			case SYN_SEMPRED:
				{
				alt43=15;
				}
				break;
			case BACKTRACK_SEMPRED:
				{
				alt43=16;
				}
				break;
			case GATED_SEMPRED:
				{
				alt43=17;
				}
				break;
			case EPSILON:
				{
				alt43=18;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 43, 0, input);

					throw nvae;
				}
			}

			switch ( alt43 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:324:4: ^( ROOT element )
				{
				Match(input,ROOT,Follow._ROOT_in_element1187); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._element_in_element1189);
				element();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:325:4: ^( BANG element )
				{
				Match(input,BANG,Follow._BANG_in_element1196); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._element_in_element1198);
				element();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:326:4: atom
				{
				PushFollow(Follow._atom_in_element1204);
				atom();

				state._fsp--;


				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:327:4: ^( NOT element )
				{
				Match(input,NOT,Follow._NOT_in_element1210); 

				@out("~");

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._element_in_element1214);
				element();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 5:
				// Grammars\\ANTLRTreePrinter.g3:328:4: ^( RANGE atom atom )
				{
				Match(input,RANGE,Follow._RANGE_in_element1221); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._atom_in_element1223);
				atom();

				state._fsp--;

				@out("..");
				PushFollow(Follow._atom_in_element1227);
				atom();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 6:
				// Grammars\\ANTLRTreePrinter.g3:329:4: ^( CHAR_RANGE atom atom )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_element1234); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._atom_in_element1236);
				atom();

				state._fsp--;

				@out("..");
				PushFollow(Follow._atom_in_element1240);
				atom();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 7:
				// Grammars\\ANTLRTreePrinter.g3:330:4: ^( ASSIGN id= ID element )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_element1247); 

				Match(input, TokenConstants.DOWN, null); 
				id=(GrammarAST)Match(input,ID,Follow._ID_in_element1251); 
				@out((id!=null?id.Text:null)+"=");
				PushFollow(Follow._element_in_element1255);
				element();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 8:
				// Grammars\\ANTLRTreePrinter.g3:331:4: ^( PLUS_ASSIGN id2= ID element )
				{
				Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_element1262); 

				Match(input, TokenConstants.DOWN, null); 
				id2=(GrammarAST)Match(input,ID,Follow._ID_in_element1266); 
				@out((id2!=null?id2.Text:null)+"+=");
				PushFollow(Follow._element_in_element1270);
				element();

				state._fsp--;


				Match(input, TokenConstants.UP, null); 

				}
				break;
			case 9:
				// Grammars\\ANTLRTreePrinter.g3:332:4: ebnf
				{
				PushFollow(Follow._ebnf_in_element1276);
				ebnf();

				state._fsp--;


				}
				break;
			case 10:
				// Grammars\\ANTLRTreePrinter.g3:333:4: tree_
				{
				PushFollow(Follow._tree__in_element1281);
				tree_();

				state._fsp--;


				}
				break;
			case 11:
				// Grammars\\ANTLRTreePrinter.g3:334:4: ^( SYNPRED block[true] )
				{
				Match(input,SYNPRED,Follow._SYNPRED_in_element1288); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._block_in_element1290);
				block(true);

				state._fsp--;


				Match(input, TokenConstants.UP, null); 
				@out("=>");

				}
				break;
			case 12:
				// Grammars\\ANTLRTreePrinter.g3:335:4: a= ACTION
				{
				a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_element1302); 
				if ( showActions ) {@out("{"); @out((a!=null?a.Text:null)); @out("}");}

				}
				break;
			case 13:
				// Grammars\\ANTLRTreePrinter.g3:336:4: a2= FORCED_ACTION
				{
				a2=(GrammarAST)Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_element1312); 
				if ( showActions ) {@out("{{"); @out((a2!=null?a2.Text:null)); @out("}}");}

				}
				break;
			case 14:
				// Grammars\\ANTLRTreePrinter.g3:337:4: pred= SEMPRED
				{
				pred=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_element1322); 

							if ( showActions )
							{
								@out("{");
								@out((pred!=null?pred.Text:null));
								@out("}?");
							}
							else
							{
								@out("{...}?");
							}
						

				}
				break;
			case 15:
				// Grammars\\ANTLRTreePrinter.g3:350:4: spred= SYN_SEMPRED
				{
				spred=(GrammarAST)Match(input,SYN_SEMPRED,Follow._SYN_SEMPRED_in_element1333); 

							string name = (spred!=null?spred.Text:null);
							GrammarAST predAST=grammar.getSyntacticPredicate(name);
							block(predAST, true);
							@out("=>");
						

				}
				break;
			case 16:
				// Grammars\\ANTLRTreePrinter.g3:357:4: ^( BACKTRACK_SEMPRED ( . )* )
				{
				Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_element1343); 

				if ( input.LA(1)==TokenConstants.DOWN )
				{
					Match(input, TokenConstants.DOWN, null); 
					// Grammars\\ANTLRTreePrinter.g3:357:24: ( . )*
					for ( ; ; )
					{
						int alt42=2;
						int LA42_0 = input.LA(1);

						if ( ((LA42_0>=ACTION && LA42_0<=XDIGIT)) )
						{
							alt42=1;
						}
						else if ( (LA42_0==UP) )
						{
							alt42=2;
						}


						switch ( alt42 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:357:0: .
							{
							MatchAny(input); 

							}
							break;

						default:
							goto loop42;
						}
					}

					loop42:
						;



					Match(input, TokenConstants.UP, null); 
				}

				}
				break;
			case 17:
				// Grammars\\ANTLRTreePrinter.g3:358:4: gpred= GATED_SEMPRED
				{
				gpred=(GrammarAST)Match(input,GATED_SEMPRED,Follow._GATED_SEMPRED_in_element1355); 

						if ( showActions ) {@out("{"); @out((gpred!=null?gpred.Text:null)); @out("}? =>");}
						else {@out("{...}? =>");}
						

				}
				break;
			case 18:
				// Grammars\\ANTLRTreePrinter.g3:363:4: EPSILON
				{
				Match(input,EPSILON,Follow._EPSILON_in_element1364); 

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
	// $ANTLR end "element"


	// $ANTLR start "ebnf"
	// Grammars\\ANTLRTreePrinter.g3:366:0: ebnf : ( block[true] | ^( OPTIONAL block[true] ) | ^( CLOSURE block[true] ) | ^( POSITIVE_CLOSURE block[true] ) );
	private void ebnf(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:367:4: ( block[true] | ^( OPTIONAL block[true] ) | ^( CLOSURE block[true] ) | ^( POSITIVE_CLOSURE block[true] ) )
			int alt44=4;
			switch ( input.LA(1) )
			{
			case BLOCK:
				{
				alt44=1;
				}
				break;
			case OPTIONAL:
				{
				alt44=2;
				}
				break;
			case CLOSURE:
				{
				alt44=3;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt44=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 44, 0, input);

					throw nvae;
				}
			}

			switch ( alt44 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:367:4: block[true]
				{
				PushFollow(Follow._block_in_ebnf1375);
				block(true);

				state._fsp--;

				@out(" ");

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:368:4: ^( OPTIONAL block[true] )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_ebnf1385); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._block_in_ebnf1387);
				block(true);

				state._fsp--;


				Match(input, TokenConstants.UP, null); 
				@out("? ");

				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:369:4: ^( CLOSURE block[true] )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_ebnf1399); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._block_in_ebnf1401);
				block(true);

				state._fsp--;


				Match(input, TokenConstants.UP, null); 
				@out("* ");

				}
				break;
			case 4:
				// Grammars\\ANTLRTreePrinter.g3:370:4: ^( POSITIVE_CLOSURE block[true] )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_ebnf1414); 

				Match(input, TokenConstants.DOWN, null); 
				PushFollow(Follow._block_in_ebnf1416);
				block(true);

				state._fsp--;


				Match(input, TokenConstants.UP, null); 
				@out("+ ");

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
	// $ANTLR end "ebnf"


	// $ANTLR start "tree_"
	// Grammars\\ANTLRTreePrinter.g3:373:0: tree_ : ^( TREE_BEGIN element ( element )* ) ;
	private void tree_(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:374:4: ( ^( TREE_BEGIN element ( element )* ) )
			// Grammars\\ANTLRTreePrinter.g3:374:4: ^( TREE_BEGIN element ( element )* )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_1433); 

			@out(" ^(");

			Match(input, TokenConstants.DOWN, null); 
			PushFollow(Follow._element_in_tree_1437);
			element();

			state._fsp--;

			// Grammars\\ANTLRTreePrinter.g3:374:40: ( element )*
			for ( ; ; )
			{
				int alt45=2;
				int LA45_0 = input.LA(1);

				if ( (LA45_0==ACTION||(LA45_0>=ASSIGN && LA45_0<=BLOCK)||(LA45_0>=CHAR_LITERAL && LA45_0<=CHAR_RANGE)||LA45_0==CLOSURE||LA45_0==DOT||LA45_0==EPSILON||LA45_0==FORCED_ACTION||LA45_0==GATED_SEMPRED||LA45_0==LABEL||LA45_0==NOT||LA45_0==OPTIONAL||(LA45_0>=PLUS_ASSIGN && LA45_0<=POSITIVE_CLOSURE)||LA45_0==RANGE||LA45_0==ROOT||LA45_0==RULE_REF||LA45_0==SEMPRED||(LA45_0>=STRING_LITERAL && LA45_0<=SYNPRED)||LA45_0==TOKEN_REF||LA45_0==TREE_BEGIN||LA45_0==WILDCARD) )
				{
					alt45=1;
				}


				switch ( alt45 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:374:41: element
					{
					PushFollow(Follow._element_in_tree_1440);
					element();

					state._fsp--;


					}
					break;

				default:
					goto loop45;
				}
			}

			loop45:
				;


			@out(") ");

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
	// $ANTLR end "tree_"

	public class atom_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "atom"
	// Grammars\\ANTLRTreePrinter.g3:377:0: atom : ( ( ^( RULE_REF (rarg= ARG_ACTION )? ( ast_suffix )? ) | ^( TOKEN_REF (targ= ARG_ACTION )? ( ast_suffix )? ) | ^( CHAR_LITERAL ( ast_suffix )? ) | ^( STRING_LITERAL ( ast_suffix )? ) | ^( WILDCARD ( ast_suffix )? ) ) | LABEL | ^( DOT ID atom ) );
	private ANTLRTreePrinter.atom_return atom(  )
	{
		ANTLRTreePrinter.atom_return retval = new ANTLRTreePrinter.atom_return();
		retval.start = input.LT(1);

		GrammarAST rarg=null;
		GrammarAST targ=null;
		GrammarAST LABEL5=null;
		GrammarAST ID6=null;

		@out(" ");
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:380:4: ( ( ^( RULE_REF (rarg= ARG_ACTION )? ( ast_suffix )? ) | ^( TOKEN_REF (targ= ARG_ACTION )? ( ast_suffix )? ) | ^( CHAR_LITERAL ( ast_suffix )? ) | ^( STRING_LITERAL ( ast_suffix )? ) | ^( WILDCARD ( ast_suffix )? ) ) | LABEL | ^( DOT ID atom ) )
			int alt54=3;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
			case WILDCARD:
				{
				alt54=1;
				}
				break;
			case LABEL:
				{
				alt54=2;
				}
				break;
			case DOT:
				{
				alt54=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 54, 0, input);

					throw nvae;
				}
			}

			switch ( alt54 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:380:4: ( ^( RULE_REF (rarg= ARG_ACTION )? ( ast_suffix )? ) | ^( TOKEN_REF (targ= ARG_ACTION )? ( ast_suffix )? ) | ^( CHAR_LITERAL ( ast_suffix )? ) | ^( STRING_LITERAL ( ast_suffix )? ) | ^( WILDCARD ( ast_suffix )? ) )
				{
				// Grammars\\ANTLRTreePrinter.g3:380:4: ( ^( RULE_REF (rarg= ARG_ACTION )? ( ast_suffix )? ) | ^( TOKEN_REF (targ= ARG_ACTION )? ( ast_suffix )? ) | ^( CHAR_LITERAL ( ast_suffix )? ) | ^( STRING_LITERAL ( ast_suffix )? ) | ^( WILDCARD ( ast_suffix )? ) )
				int alt53=5;
				switch ( input.LA(1) )
				{
				case RULE_REF:
					{
					alt53=1;
					}
					break;
				case TOKEN_REF:
					{
					alt53=2;
					}
					break;
				case CHAR_LITERAL:
					{
					alt53=3;
					}
					break;
				case STRING_LITERAL:
					{
					alt53=4;
					}
					break;
				case WILDCARD:
					{
					alt53=5;
					}
					break;
				default:
					{
						NoViableAltException nvae = new NoViableAltException("", 53, 0, input);

						throw nvae;
					}
				}

				switch ( alt53 )
				{
				case 1:
					// Grammars\\ANTLRTreePrinter.g3:380:6: ^( RULE_REF (rarg= ARG_ACTION )? ( ast_suffix )? )
					{
					Match(input,RULE_REF,Follow._RULE_REF_in_atom1466); 

					@out(((GrammarAST)retval.start).ToString());

					if ( input.LA(1)==TokenConstants.DOWN )
					{
						Match(input, TokenConstants.DOWN, null); 
						// Grammars\\ANTLRTreePrinter.g3:381:5: (rarg= ARG_ACTION )?
						int alt46=2;
						int LA46_0 = input.LA(1);

						if ( (LA46_0==ARG_ACTION) )
						{
							alt46=1;
						}
						switch ( alt46 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:381:6: rarg= ARG_ACTION
							{
							rarg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1478); 
							@out("["+rarg.ToString()+"]");

							}
							break;

						}

						// Grammars\\ANTLRTreePrinter.g3:382:5: ( ast_suffix )?
						int alt47=2;
						int LA47_0 = input.LA(1);

						if ( (LA47_0==BANG||LA47_0==ROOT) )
						{
							alt47=1;
						}
						switch ( alt47 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:382:6: ast_suffix
							{
							PushFollow(Follow._ast_suffix_in_atom1489);
							ast_suffix();

							state._fsp--;


							}
							break;

						}


						Match(input, TokenConstants.UP, null); 
					}

					}
					break;
				case 2:
					// Grammars\\ANTLRTreePrinter.g3:384:5: ^( TOKEN_REF (targ= ARG_ACTION )? ( ast_suffix )? )
					{
					Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom1504); 

					@out(((GrammarAST)retval.start).ToString());

					if ( input.LA(1)==TokenConstants.DOWN )
					{
						Match(input, TokenConstants.DOWN, null); 
						// Grammars\\ANTLRTreePrinter.g3:385:5: (targ= ARG_ACTION )?
						int alt48=2;
						int LA48_0 = input.LA(1);

						if ( (LA48_0==ARG_ACTION) )
						{
							alt48=1;
						}
						switch ( alt48 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:385:6: targ= ARG_ACTION
							{
							targ=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1517); 
							@out("["+targ.ToString()+"]");

							}
							break;

						}

						// Grammars\\ANTLRTreePrinter.g3:386:5: ( ast_suffix )?
						int alt49=2;
						int LA49_0 = input.LA(1);

						if ( (LA49_0==BANG||LA49_0==ROOT) )
						{
							alt49=1;
						}
						switch ( alt49 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:386:6: ast_suffix
							{
							PushFollow(Follow._ast_suffix_in_atom1529);
							ast_suffix();

							state._fsp--;


							}
							break;

						}


						Match(input, TokenConstants.UP, null); 
					}

					}
					break;
				case 3:
					// Grammars\\ANTLRTreePrinter.g3:388:5: ^( CHAR_LITERAL ( ast_suffix )? )
					{
					Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom1544); 

					@out(((GrammarAST)retval.start).ToString());

					if ( input.LA(1)==TokenConstants.DOWN )
					{
						Match(input, TokenConstants.DOWN, null); 
						// Grammars\\ANTLRTreePrinter.g3:389:5: ( ast_suffix )?
						int alt50=2;
						int LA50_0 = input.LA(1);

						if ( (LA50_0==BANG||LA50_0==ROOT) )
						{
							alt50=1;
						}
						switch ( alt50 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:389:6: ast_suffix
							{
							PushFollow(Follow._ast_suffix_in_atom1553);
							ast_suffix();

							state._fsp--;


							}
							break;

						}


						Match(input, TokenConstants.UP, null); 
					}

					}
					break;
				case 4:
					// Grammars\\ANTLRTreePrinter.g3:391:5: ^( STRING_LITERAL ( ast_suffix )? )
					{
					Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_atom1568); 

					@out(((GrammarAST)retval.start).ToString());

					if ( input.LA(1)==TokenConstants.DOWN )
					{
						Match(input, TokenConstants.DOWN, null); 
						// Grammars\\ANTLRTreePrinter.g3:392:5: ( ast_suffix )?
						int alt51=2;
						int LA51_0 = input.LA(1);

						if ( (LA51_0==BANG||LA51_0==ROOT) )
						{
							alt51=1;
						}
						switch ( alt51 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:392:6: ast_suffix
							{
							PushFollow(Follow._ast_suffix_in_atom1577);
							ast_suffix();

							state._fsp--;


							}
							break;

						}


						Match(input, TokenConstants.UP, null); 
					}

					}
					break;
				case 5:
					// Grammars\\ANTLRTreePrinter.g3:394:5: ^( WILDCARD ( ast_suffix )? )
					{
					Match(input,WILDCARD,Follow._WILDCARD_in_atom1592); 

					@out(((GrammarAST)retval.start).ToString());

					if ( input.LA(1)==TokenConstants.DOWN )
					{
						Match(input, TokenConstants.DOWN, null); 
						// Grammars\\ANTLRTreePrinter.g3:395:5: ( ast_suffix )?
						int alt52=2;
						int LA52_0 = input.LA(1);

						if ( (LA52_0==BANG||LA52_0==ROOT) )
						{
							alt52=1;
						}
						switch ( alt52 )
						{
						case 1:
							// Grammars\\ANTLRTreePrinter.g3:395:6: ast_suffix
							{
							PushFollow(Follow._ast_suffix_in_atom1602);
							ast_suffix();

							state._fsp--;


							}
							break;

						}


						Match(input, TokenConstants.UP, null); 
					}

					}
					break;

				}

				@out(" ");

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:399:4: LABEL
				{
				LABEL5=(GrammarAST)Match(input,LABEL,Follow._LABEL_in_atom1622); 
				@out(" $"+(LABEL5!=null?LABEL5.Text:null));

				}
				break;
			case 3:
				// Grammars\\ANTLRTreePrinter.g3:400:4: ^( DOT ID atom )
				{
				Match(input,DOT,Follow._DOT_in_atom1631); 

				Match(input, TokenConstants.DOWN, null); 
				ID6=(GrammarAST)Match(input,ID,Follow._ID_in_atom1633); 
				@out((ID6!=null?ID6.Text:null)+".");
				PushFollow(Follow._atom_in_atom1637);
				atom();

				state._fsp--;


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
		return retval;
	}
	// $ANTLR end "atom"


	// $ANTLR start "ast_suffix"
	// Grammars\\ANTLRTreePrinter.g3:403:0: ast_suffix : ( ROOT | BANG );
	private void ast_suffix(  )
	{
		try
		{
			// Grammars\\ANTLRTreePrinter.g3:404:4: ( ROOT | BANG )
			int alt55=2;
			int LA55_0 = input.LA(1);

			if ( (LA55_0==ROOT) )
			{
				alt55=1;
			}
			else if ( (LA55_0==BANG) )
			{
				alt55=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 55, 0, input);

				throw nvae;
			}
			switch ( alt55 )
			{
			case 1:
				// Grammars\\ANTLRTreePrinter.g3:404:4: ROOT
				{
				Match(input,ROOT,Follow._ROOT_in_ast_suffix1650); 
				@out("^");

				}
				break;
			case 2:
				// Grammars\\ANTLRTreePrinter.g3:405:4: BANG
				{
				Match(input,BANG,Follow._BANG_in_ast_suffix1657); 
				@out("!");

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
	// $ANTLR end "ast_suffix"
	#endregion Rules

	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _grammar__in_toString72 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rule_in_toString78 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _alternative_in_toString84 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _element_in_toString90 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _single_rewrite_in_toString96 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EOR_in_toString102 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LEXER_GRAMMAR_in_grammar_127 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_129 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PARSER_GRAMMAR_in_grammar_139 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_141 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_GRAMMAR_in_grammar_151 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_153 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _COMBINED_GRAMMAR_in_grammar_163 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_165 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SCOPE_in_attrScope181 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScope183 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ruleAction_in_attrScope185 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ACTION_in_attrScope188 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_grammarSpec204 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammarSpec213 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _optionsSpec_in_grammarSpec223 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _delegateGrammars_in_grammarSpec232 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _tokensSpec_in_grammarSpec239 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _attrScope_in_grammarSpec246 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _actions_in_grammarSpec253 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _rules_in_grammarSpec259 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _action_in_actions272 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _AMPERSAND_in_action293 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_action297 = new BitSet(new ulong[]{0x80000000010UL});
		public static readonly BitSet _ID_in_action306 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_action310 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_action325 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _OPTIONS_in_optionsSpec357 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _option_in_optionsSpec365 = new BitSet(new ulong[]{0x2008UL});
		public static readonly BitSet _ASSIGN_in_option391 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_option395 = new BitSet(new ulong[]{0x880000040000UL,0x200000UL});
		public static readonly BitSet _optionValue_in_option399 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_optionValue414 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_optionValue434 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_optionValue443 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_optionValue454 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_in_delegateGrammars484 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ASSIGN_in_delegateGrammars489 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_delegateGrammars491 = new BitSet(new ulong[]{0x80000000000UL});
		public static readonly BitSet _ID_in_delegateGrammars493 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_delegateGrammars498 = new BitSet(new ulong[]{0x80000002008UL});
		public static readonly BitSet _TOKENS_in_tokensSpec516 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _tokenSpec_in_tokensSpec520 = new BitSet(new ulong[]{0x2008UL,0x4000000UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec536 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ASSIGN_in_tokenSpec543 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec545 = new BitSet(new ulong[]{0x40000UL,0x200000UL});
		public static readonly BitSet _set_in_tokenSpec547 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rule_in_rules566 = new BitSet(new ulong[]{0x400200008000202UL,0x8005000UL});
		public static readonly BitSet _RULE_in_rule582 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rule586 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_rule592 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_rule605 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule610 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RET_in_rule623 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule628 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _throwsSpec_in_rule641 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _optionsSpec_in_rule649 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _ruleScopeSpec_in_rule657 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _ruleAction_in_rule665 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _block_in_rule684 = new BitSet(new ulong[]{0x4400020000UL});
		public static readonly BitSet _exceptionGroup_in_rule691 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_rule698 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _AMPERSAND_in_ruleAction716 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_ruleAction720 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_ruleAction724 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_modifier746 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec773 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_throwsSpec775 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec790 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ruleAction_in_ruleScopeSpec792 = new BitSet(new ulong[]{0x80000000218UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec796 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _ID_in_ruleScopeSpec802 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _BLOCK_in_block826 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _optionsSpec_in_block837 = new BitSet(new ulong[]{0x100UL});
		public static readonly BitSet _alternative_in_block847 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _rewrite_in_block849 = new BitSet(new ulong[]{0x200000100UL});
		public static readonly BitSet _alternative_in_block855 = new BitSet(new ulong[]{0x200000100UL,0x200UL});
		public static readonly BitSet _rewrite_in_block857 = new BitSet(new ulong[]{0x200000100UL});
		public static readonly BitSet _EOB_in_block865 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_alternative887 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_alternative890 = new BitSet(new ulong[]{0x86810289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _EOA_in_alternative894 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup909 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup915 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup922 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler934 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler936 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler938 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FINALLY_in_finallyClause951 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_finallyClause953 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _REWRITE_in_single_rewrite967 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _SEMPRED_in_single_rewrite976 = new BitSet(new ulong[]{0x2000000110UL,0x1000000UL});
		public static readonly BitSet _alternative_in_single_rewrite991 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rewrite_template_in_single_rewrite998 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ETC_in_single_rewrite1005 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_single_rewrite1014 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_rewrite_template1038 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template1047 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ACTION_in_rewrite_template1058 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ARGLIST_in_rewrite_template1072 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_in_rewrite_template1088 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template1092 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_rewrite_template1104 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template1140 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template1149 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _single_rewrite_in_rewrite1173 = new BitSet(new ulong[]{0x2UL,0x200UL});
		public static readonly BitSet _ROOT_in_element1187 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element1189 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BANG_in_element1196 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element1198 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _atom_in_element1204 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_element1210 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element1214 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RANGE_in_element1221 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element1223 = new BitSet(new ulong[]{0x1000020040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element1227 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_RANGE_in_element1234 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element1236 = new BitSet(new ulong[]{0x1000020040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element1240 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_element1247 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element1251 = new BitSet(new ulong[]{0x86810289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element1255 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_ASSIGN_in_element1262 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element1266 = new BitSet(new ulong[]{0x86810289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element1270 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ebnf_in_element1276 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_element1281 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYNPRED_in_element1288 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_element1290 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_element1302 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FORCED_ACTION_in_element1312 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_element1322 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYN_SEMPRED_in_element1333 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_element1343 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _GATED_SEMPRED_in_element1355 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EPSILON_in_element1364 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_ebnf1375 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_ebnf1385 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1387 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_ebnf1399 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1401 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_ebnf1414 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1416 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_1433 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_tree_1437 = new BitSet(new ulong[]{0x86810289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _element_in_tree_1440 = new BitSet(new ulong[]{0x86810289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _RULE_REF_in_atom1466 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1478 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom1489 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom1504 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1517 = new BitSet(new ulong[]{0x8008UL,0x400UL});
		public static readonly BitSet _ast_suffix_in_atom1529 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom1544 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1553 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _STRING_LITERAL_in_atom1568 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1577 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _WILDCARD_in_atom1592 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ast_suffix_in_atom1602 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _LABEL_in_atom1622 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_atom1631 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_atom1633 = new BitSet(new ulong[]{0x1000020040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_atom1637 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ROOT_in_ast_suffix1650 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_ast_suffix1657 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
