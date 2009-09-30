// $ANTLR 3.1.2 Grammars\\DefineGrammarItemsWalker.g3 2009-09-30 13:28:45

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

using System;

using Antlr3.Tool;
#if DEBUG
using Utils = Antlr3.Misc.Utils;
#endif


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace Antlr3.Grammars
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class DefineGrammarItemsWalker : TreeParser
{
	internal static readonly string[] tokenNames = new string[] {
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

	protected class AttributeScopeActions_scope
	{
		public Dictionary<GrammarAST, GrammarAST> actions;
	}
	partial void AttributeScopeActions_scopeInit( AttributeScopeActions_scope scope );partial void AttributeScopeActions_scopeAfter( AttributeScopeActions_scope scope );protected Stack<AttributeScopeActions_scope> AttributeScopeActions_stack = new Stack<AttributeScopeActions_scope>();

	public DefineGrammarItemsWalker( ITreeNodeStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public DefineGrammarItemsWalker( ITreeNodeStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] TokenNames { get { return DefineGrammarItemsWalker.tokenNames; } }
	public override string GrammarFileName { get { return "Grammars\\DefineGrammarItemsWalker.g3"; } }


	#region Rules
	public class grammar__return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "grammar_"
	// Grammars\\DefineGrammarItemsWalker.g3:91:0: public grammar_[Grammar g] : ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) );
	public DefineGrammarItemsWalker.grammar__return grammar_( Grammar g )
	{
		DefineGrammarItemsWalker.grammar__return retval = new DefineGrammarItemsWalker.grammar__return();
		retval.start = input.LT(1);


		grammar = g;
		root = ((GrammarAST)retval.Start);

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:101:4: ( ^( LEXER_GRAMMAR grammarSpec ) | ^( PARSER_GRAMMAR grammarSpec ) | ^( TREE_GRAMMAR grammarSpec ) | ^( COMBINED_GRAMMAR grammarSpec ) )
			int alt1=4;
			switch ( input.LA(1) )
			{
			case LEXER_GRAMMAR:
				{
				alt1=1;
				}
				break;
			case PARSER_GRAMMAR:
				{
				alt1=2;
				}
				break;
			case TREE_GRAMMAR:
				{
				alt1=3;
				}
				break;
			case COMBINED_GRAMMAR:
				{
				alt1=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

					throw nvae;
				}
			}

			switch ( alt1 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:101:4: ^( LEXER_GRAMMAR grammarSpec )
				{
				Match(input,LEXER_GRAMMAR,Follow._LEXER_GRAMMAR_in_grammar_77); if (state.failed) return retval;

				if ( state.backtracking == 0 )
				{
					grammar.type = GrammarType.Lexer;
				}

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._grammarSpec_in_grammar_83);
				grammarSpec();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:102:4: ^( PARSER_GRAMMAR grammarSpec )
				{
				Match(input,PARSER_GRAMMAR,Follow._PARSER_GRAMMAR_in_grammar_92); if (state.failed) return retval;

				if ( state.backtracking == 0 )
				{
					grammar.type = GrammarType.Parser;
				}

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._grammarSpec_in_grammar_97);
				grammarSpec();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:103:4: ^( TREE_GRAMMAR grammarSpec )
				{
				Match(input,TREE_GRAMMAR,Follow._TREE_GRAMMAR_in_grammar_106); if (state.failed) return retval;

				if ( state.backtracking == 0 )
				{
					grammar.type = GrammarType.TreeParser;
				}

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._grammarSpec_in_grammar_111);
				grammarSpec();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 4:
				// Grammars\\DefineGrammarItemsWalker.g3:104:4: ^( COMBINED_GRAMMAR grammarSpec )
				{
				Match(input,COMBINED_GRAMMAR,Follow._COMBINED_GRAMMAR_in_grammar_120); if (state.failed) return retval;

				if ( state.backtracking == 0 )
				{
					grammar.type = GrammarType.Combined;
				}

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._grammarSpec_in_grammar_125);
				grammarSpec();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;

			}
			if ( state.backtracking == 0 )
			{

				Finish();

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
	// $ANTLR end "grammar_"


	// $ANTLR start "attrScope"
	// Grammars\\DefineGrammarItemsWalker.g3:107:0: attrScope : ^( 'scope' name= ID ( attrScopeAction )* attrs= ACTION ) ;
	private void attrScope(  )
	{
		AttributeScopeActions_stack.Push(new AttributeScopeActions_scope());AttributeScopeActions_scopeInit(AttributeScopeActions_stack.Peek());

		GrammarAST name=null;
		GrammarAST attrs=null;


			((AttributeScopeActions_scope)AttributeScopeActions_stack.Peek()).actions = new Dictionary<GrammarAST, GrammarAST>();

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:113:4: ( ^( 'scope' name= ID ( attrScopeAction )* attrs= ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:113:4: ^( 'scope' name= ID ( attrScopeAction )* attrs= ACTION )
			{
			Match(input,SCOPE,Follow._SCOPE_in_attrScope150); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			name=(GrammarAST)Match(input,ID,Follow._ID_in_attrScope154); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:113:23: ( attrScopeAction )*
			for ( ; ; )
			{
				int alt2=2;
				int LA2_0 = input.LA(1);

				if ( (LA2_0==AMPERSAND) )
				{
					alt2=1;
				}


				switch ( alt2 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:113:0: attrScopeAction
					{
					PushFollow(Follow._attrScopeAction_in_attrScope156);
					attrScopeAction();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					goto loop2;
				}
			}

			loop2:
				;


			attrs=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_attrScope161); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							AttributeScope scope = grammar.DefineGlobalScope((name!=null?name.Text:null),attrs.token);
							scope.isDynamicGlobalScope = true;
							scope.AddAttributes((attrs!=null?attrs.Text:null), ';');
							foreach ( var action in ((AttributeScopeActions_scope)AttributeScopeActions_stack.Peek()).actions )
								scope.DefineNamedAction( action.Key, action.Value );
						
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
			AttributeScopeActions_scopeAfter(AttributeScopeActions_stack.Peek());AttributeScopeActions_stack.Pop();

		}
		return ;
	}
	// $ANTLR end "attrScope"


	// $ANTLR start "attrScopeAction"
	// Grammars\\DefineGrammarItemsWalker.g3:123:0: attrScopeAction : ^( AMPERSAND ID ACTION ) ;
	private void attrScopeAction(  )
	{
		GrammarAST ID1=null;
		GrammarAST ACTION2=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:124:4: ( ^( AMPERSAND ID ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:124:4: ^( AMPERSAND ID ACTION )
			{
			Match(input,AMPERSAND,Follow._AMPERSAND_in_attrScopeAction179); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			ID1=(GrammarAST)Match(input,ID,Follow._ID_in_attrScopeAction181); if (state.failed) return ;
			ACTION2=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_attrScopeAction183); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

							((AttributeScopeActions_scope)AttributeScopeActions_stack.Peek()).actions.Add( ID1, ACTION2 );
						
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
	// $ANTLR end "attrScopeAction"


	// $ANTLR start "grammarSpec"
	// Grammars\\DefineGrammarItemsWalker.g3:130:0: grammarSpec : id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules ;
	private void grammarSpec(  )
	{
		GrammarAST id=null;
		GrammarAST cmt=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:131:4: (id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules )
			// Grammars\\DefineGrammarItemsWalker.g3:131:4: id= ID (cmt= DOC_COMMENT )? ( optionsSpec )? ( delegateGrammars )? ( tokensSpec )? ( attrScope )* ( actions )? rules
			{
			id=(GrammarAST)Match(input,ID,Follow._ID_in_grammarSpec201); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:132:3: (cmt= DOC_COMMENT )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==DOC_COMMENT) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:132:4: cmt= DOC_COMMENT
				{
				cmt=(GrammarAST)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammarSpec208); if (state.failed) return ;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:133:3: ( optionsSpec )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==OPTIONS) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:133:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_grammarSpec216);
				optionsSpec();

				state._fsp--;
				if (state.failed) return ;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:134:3: ( delegateGrammars )?
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( (LA5_0==IMPORT) )
			{
				alt5=1;
			}
			switch ( alt5 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:134:4: delegateGrammars
				{
				PushFollow(Follow._delegateGrammars_in_grammarSpec224);
				delegateGrammars();

				state._fsp--;
				if (state.failed) return ;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:135:3: ( tokensSpec )?
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==TOKENS) )
			{
				alt6=1;
			}
			switch ( alt6 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:135:4: tokensSpec
				{
				PushFollow(Follow._tokensSpec_in_grammarSpec231);
				tokensSpec();

				state._fsp--;
				if (state.failed) return ;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:136:3: ( attrScope )*
			for ( ; ; )
			{
				int alt7=2;
				int LA7_0 = input.LA(1);

				if ( (LA7_0==SCOPE) )
				{
					alt7=1;
				}


				switch ( alt7 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:136:4: attrScope
					{
					PushFollow(Follow._attrScope_in_grammarSpec238);
					attrScope();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					goto loop7;
				}
			}

			loop7:
				;


			// Grammars\\DefineGrammarItemsWalker.g3:137:3: ( actions )?
			int alt8=2;
			int LA8_0 = input.LA(1);

			if ( (LA8_0==AMPERSAND) )
			{
				alt8=1;
			}
			switch ( alt8 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:137:4: actions
				{
				PushFollow(Follow._actions_in_grammarSpec245);
				actions();

				state._fsp--;
				if (state.failed) return ;

				}
				break;

			}

			PushFollow(Follow._rules_in_grammarSpec251);
			rules();

			state._fsp--;
			if (state.failed) return ;

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
	// Grammars\\DefineGrammarItemsWalker.g3:141:0: actions : ( action )+ ;
	private void actions(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:142:4: ( ( action )+ )
			// Grammars\\DefineGrammarItemsWalker.g3:142:4: ( action )+
			{
			// Grammars\\DefineGrammarItemsWalker.g3:142:4: ( action )+
			int cnt9=0;
			for ( ; ; )
			{
				int alt9=2;
				int LA9_0 = input.LA(1);

				if ( (LA9_0==AMPERSAND) )
				{
					alt9=1;
				}


				switch ( alt9 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:142:6: action
					{
					PushFollow(Follow._action_in_actions264);
					action();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					if ( cnt9 >= 1 )
						goto loop9;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee9 = new EarlyExitException( 9, input );
					throw eee9;
				}
				cnt9++;
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
	// $ANTLR end "actions"


	// $ANTLR start "action"
	// Grammars\\DefineGrammarItemsWalker.g3:145:0: action : ^(amp= AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) ) ;
	private void action(  )
	{
		GrammarAST amp=null;
		GrammarAST id1=null;
		GrammarAST id2=null;
		GrammarAST a1=null;
		GrammarAST a2=null;


			string scope=null;
			GrammarAST nameAST=null, actionAST=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:151:4: ( ^(amp= AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) ) )
			// Grammars\\DefineGrammarItemsWalker.g3:151:4: ^(amp= AMPERSAND id1= ID (id2= ID a1= ACTION |a2= ACTION ) )
			{
			amp=(GrammarAST)Match(input,AMPERSAND,Follow._AMPERSAND_in_action286); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			id1=(GrammarAST)Match(input,ID,Follow._ID_in_action290); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:152:4: (id2= ID a1= ACTION |a2= ACTION )
			int alt10=2;
			int LA10_0 = input.LA(1);

			if ( (LA10_0==ID) )
			{
				alt10=1;
			}
			else if ( (LA10_0==ACTION) )
			{
				alt10=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 10, 0, input);

				throw nvae;
			}
			switch ( alt10 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:152:6: id2= ID a1= ACTION
				{
				id2=(GrammarAST)Match(input,ID,Follow._ID_in_action299); if (state.failed) return ;
				a1=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_action303); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					scope=(id1!=null?id1.Text:null); nameAST=id2; actionAST=a1;
				}

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:154:6: a2= ACTION
				{
				a2=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_action319); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{
					scope=null; nameAST=id1; actionAST=a2;
				}

				}
				break;

			}


			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{

						 grammar.DefineNamedAction(amp,scope,nameAST,actionAST);
						 
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
	// Grammars\\DefineGrammarItemsWalker.g3:163:0: optionsSpec : ^( OPTIONS ( . )* ) ;
	private void optionsSpec(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:164:4: ( ^( OPTIONS ( . )* ) )
			// Grammars\\DefineGrammarItemsWalker.g3:164:4: ^( OPTIONS ( . )* )
			{
			Match(input,OPTIONS,Follow._OPTIONS_in_optionsSpec353); if (state.failed) return ;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				// Grammars\\DefineGrammarItemsWalker.g3:164:14: ( . )*
				for ( ; ; )
				{
					int alt11=2;
					int LA11_0 = input.LA(1);

					if ( ((LA11_0>=ACTION && LA11_0<=XDIGIT)) )
					{
						alt11=1;
					}
					else if ( (LA11_0==UP) )
					{
						alt11=2;
					}


					switch ( alt11 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:164:0: .
						{
						MatchAny(input); if (state.failed) return ;

						}
						break;

					default:
						goto loop11;
					}
				}

				loop11:
					;



				Match(input, TokenTypes.Up, null); if (state.failed) return ;
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
	// $ANTLR end "optionsSpec"


	// $ANTLR start "delegateGrammars"
	// Grammars\\DefineGrammarItemsWalker.g3:167:0: delegateGrammars : ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) ;
	private void delegateGrammars(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:168:4: ( ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ ) )
			// Grammars\\DefineGrammarItemsWalker.g3:168:4: ^( 'import' ( ^( ASSIGN ID ID ) | ID )+ )
			{
			Match(input,IMPORT,Follow._IMPORT_in_delegateGrammars370); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:168:16: ( ^( ASSIGN ID ID ) | ID )+
			int cnt12=0;
			for ( ; ; )
			{
				int alt12=3;
				int LA12_0 = input.LA(1);

				if ( (LA12_0==ASSIGN) )
				{
					alt12=1;
				}
				else if ( (LA12_0==ID) )
				{
					alt12=2;
				}


				switch ( alt12 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:168:18: ^( ASSIGN ID ID )
					{
					Match(input,ASSIGN,Follow._ASSIGN_in_delegateGrammars375); if (state.failed) return ;

					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					Match(input,ID,Follow._ID_in_delegateGrammars377); if (state.failed) return ;
					Match(input,ID,Follow._ID_in_delegateGrammars379); if (state.failed) return ;

					Match(input, TokenTypes.Up, null); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\DefineGrammarItemsWalker.g3:168:36: ID
					{
					Match(input,ID,Follow._ID_in_delegateGrammars384); if (state.failed) return ;

					}
					break;

				default:
					if ( cnt12 >= 1 )
						goto loop12;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee12 = new EarlyExitException( 12, input );
					throw eee12;
				}
				cnt12++;
			}
			loop12:
				;



			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// Grammars\\DefineGrammarItemsWalker.g3:171:0: tokensSpec : ^( TOKENS ( tokenSpec )+ ) ;
	private void tokensSpec(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:172:4: ( ^( TOKENS ( tokenSpec )+ ) )
			// Grammars\\DefineGrammarItemsWalker.g3:172:4: ^( TOKENS ( tokenSpec )+ )
			{
			Match(input,TOKENS,Follow._TOKENS_in_tokensSpec402); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:172:14: ( tokenSpec )+
			int cnt13=0;
			for ( ; ; )
			{
				int alt13=2;
				int LA13_0 = input.LA(1);

				if ( (LA13_0==ASSIGN||LA13_0==TOKEN_REF) )
				{
					alt13=1;
				}


				switch ( alt13 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:172:16: tokenSpec
					{
					PushFollow(Follow._tokenSpec_in_tokensSpec406);
					tokenSpec();

					state._fsp--;
					if (state.failed) return ;

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



			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// Grammars\\DefineGrammarItemsWalker.g3:175:0: tokenSpec : (t= TOKEN_REF | ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) ) );
	private void tokenSpec(  )
	{
		GrammarAST t=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:176:4: (t= TOKEN_REF | ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) ) )
			int alt14=2;
			int LA14_0 = input.LA(1);

			if ( (LA14_0==TOKEN_REF) )
			{
				alt14=1;
			}
			else if ( (LA14_0==ASSIGN) )
			{
				alt14=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

				throw nvae;
			}
			switch ( alt14 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:176:4: t= TOKEN_REF
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec424); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:177:4: ^( ASSIGN TOKEN_REF ( STRING_LITERAL | CHAR_LITERAL ) )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_tokenSpec431); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec436); if (state.failed) return ;
				if ( input.LA(1)==CHAR_LITERAL||input.LA(1)==STRING_LITERAL )
				{
					input.Consume();
					state.errorRecovery=false;state.failed=false;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					MismatchedSetException mse = new MismatchedSetException(null,input);
					throw mse;
				}


				Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// Grammars\\DefineGrammarItemsWalker.g3:185:0: rules : ( rule )+ ;
	private void rules(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:186:4: ( ( rule )+ )
			// Grammars\\DefineGrammarItemsWalker.g3:186:4: ( rule )+
			{
			// Grammars\\DefineGrammarItemsWalker.g3:186:4: ( rule )+
			int cnt15=0;
			for ( ; ; )
			{
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( (LA15_0==RULE) )
				{
					alt15=1;
				}


				switch ( alt15 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:186:0: rule
					{
					PushFollow(Follow._rule_in_rules471);
					rule();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					if ( cnt15 >= 1 )
						goto loop15;

					if (state.backtracking>0) {state.failed=true; return ;}
					EarlyExitException eee15 = new EarlyExitException( 15, input );
					throw eee15;
				}
				cnt15++;
			}
			loop15:
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

	public class rule_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rule"
	// Grammars\\DefineGrammarItemsWalker.g3:189:0: rule : ^( RULE id= ID ( modifier )? ^( ARG (args= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec[r] )? ( ruleAction[r] )* b= block ( exceptionGroup )? EOR ) ;
	private DefineGrammarItemsWalker.rule_return rule(  )
	{
		DefineGrammarItemsWalker.rule_return retval = new DefineGrammarItemsWalker.rule_return();
		retval.start = input.LT(1);

		GrammarAST id=null;
		GrammarAST args=null;
		GrammarAST ret=null;
		GrammarAST RULE3=null;
		DefineGrammarItemsWalker.block_return b = default(DefineGrammarItemsWalker.block_return);
		DefineGrammarItemsWalker.modifier_return modifier4 = default(DefineGrammarItemsWalker.modifier_return);
		HashSet<string> throwsSpec5 = default(HashSet<string>);


			string name=null;
			IDictionary<string,object> opts=null;
			Rule r = null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:196:5: ( ^( RULE id= ID ( modifier )? ^( ARG (args= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec[r] )? ( ruleAction[r] )* b= block ( exceptionGroup )? EOR ) )
			// Grammars\\DefineGrammarItemsWalker.g3:196:5: ^( RULE id= ID ( modifier )? ^( ARG (args= ARG_ACTION )? ) ^( RET (ret= ARG_ACTION )? ) ( throwsSpec )? ( optionsSpec )? ( ruleScopeSpec[r] )? ( ruleAction[r] )* b= block ( exceptionGroup )? EOR )
			{
			RULE3=(GrammarAST)Match(input,RULE,Follow._RULE_in_rule491); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_rule495); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{
				opts = RULE3.BlockOptions;
			}
			// Grammars\\DefineGrammarItemsWalker.g3:197:4: ( modifier )?
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( (LA16_0==FRAGMENT||(LA16_0>=PRIVATE && LA16_0<=PUBLIC)) )
			{
				alt16=1;
			}
			switch ( alt16 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:197:5: modifier
				{
				PushFollow(Follow._modifier_in_rule503);
				modifier4=modifier();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,ARG,Follow._ARG_in_rule512); if (state.failed) return retval;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				// Grammars\\DefineGrammarItemsWalker.g3:198:11: (args= ARG_ACTION )?
				int alt17=2;
				int LA17_0 = input.LA(1);

				if ( (LA17_0==ARG_ACTION) )
				{
					alt17=1;
				}
				switch ( alt17 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:198:12: args= ARG_ACTION
					{
					args=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule517); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			}
			Match(input,RET,Follow._RET_in_rule528); if (state.failed) return retval;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				// Grammars\\DefineGrammarItemsWalker.g3:199:11: (ret= ARG_ACTION )?
				int alt18=2;
				int LA18_0 = input.LA(1);

				if ( (LA18_0==ARG_ACTION) )
				{
					alt18=1;
				}
				switch ( alt18 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:199:12: ret= ARG_ACTION
					{
					ret=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule533); if (state.failed) return retval;

					}
					break;

				}


				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			}
			// Grammars\\DefineGrammarItemsWalker.g3:200:4: ( throwsSpec )?
			int alt19=2;
			int LA19_0 = input.LA(1);

			if ( (LA19_0==THROWS) )
			{
				alt19=1;
			}
			switch ( alt19 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:200:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule543);
				throwsSpec5=throwsSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:201:4: ( optionsSpec )?
			int alt20=2;
			int LA20_0 = input.LA(1);

			if ( (LA20_0==OPTIONS) )
			{
				alt20=1;
			}
			switch ( alt20 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:201:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_rule551);
				optionsSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

								name = (id!=null?id.Text:null);
								currentRuleName = name;
								if ( char.IsUpper(name[0]) && grammar.type==GrammarType.Combined )
								{
									// a merged grammar spec, track lexer rules and send to another grammar
									grammar.DefineLexerRuleFoundInParser(id.token, ((GrammarAST)retval.Start));
								}
								else
								{
									int numAlts = CountAltsForRule(((GrammarAST)retval.Start));
									grammar.DefineRule(id.Token, (modifier4!=null?modifier4.mod:default(string)), opts, ((GrammarAST)retval.Start), args, numAlts);
									r = grammar.GetRule(name);
									if ( args!=null )
									{
										r.parameterScope = grammar.CreateParameterScope(name,args.Token);
										r.parameterScope.AddAttributes((args!=null?args.Text:null), ',');
									}
									if ( ret!=null )
									{
										r.returnScope = grammar.CreateReturnScope(name,ret.token);
										r.returnScope.AddAttributes((ret!=null?ret.Text:null), ',');
									}
									if ( throwsSpec5 != null )
									{
										foreach ( string exception in throwsSpec5 )
											r.throwsSpec.Add( exception );
									}
								}
							
			}
			// Grammars\\DefineGrammarItemsWalker.g3:232:4: ( ruleScopeSpec[r] )?
			int alt21=2;
			int LA21_0 = input.LA(1);

			if ( (LA21_0==SCOPE) )
			{
				alt21=1;
			}
			switch ( alt21 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:232:5: ruleScopeSpec[r]
				{
				PushFollow(Follow._ruleScopeSpec_in_rule564);
				ruleScopeSpec(r);

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:233:4: ( ruleAction[r] )*
			for ( ; ; )
			{
				int alt22=2;
				int LA22_0 = input.LA(1);

				if ( (LA22_0==AMPERSAND) )
				{
					alt22=1;
				}


				switch ( alt22 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:233:5: ruleAction[r]
					{
					PushFollow(Follow._ruleAction_in_rule573);
					ruleAction(r);

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				default:
					goto loop22;
				}
			}

			loop22:
				;


			if ( state.backtracking == 0 )
			{
				 this.blockLevel=0; 
			}
			PushFollow(Follow._block_in_rule588);
			b=block();

			state._fsp--;
			if (state.failed) return retval;
			// Grammars\\DefineGrammarItemsWalker.g3:236:4: ( exceptionGroup )?
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0==CATCH||LA23_0==FINALLY) )
			{
				alt23=1;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:236:5: exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_rule594);
				exceptionGroup();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			Match(input,EOR,Follow._EOR_in_rule601); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

								// copy rule options into the block AST, which is where
								// the analysis will look for k option etc...
								(b!=null?((GrammarAST)b.Start):null).BlockOptions = opts;
							
			}

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "rule"


	// $ANTLR start "ruleAction"
	// Grammars\\DefineGrammarItemsWalker.g3:246:0: ruleAction[Rule r] : ^(amp= AMPERSAND id= ID a= ACTION ) ;
	private void ruleAction( Rule r )
	{
		GrammarAST amp=null;
		GrammarAST id=null;
		GrammarAST a=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:247:4: ( ^(amp= AMPERSAND id= ID a= ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:247:4: ^(amp= AMPERSAND id= ID a= ACTION )
			{
			amp=(GrammarAST)Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleAction625); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_ruleAction629); if (state.failed) return ;
			a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_ruleAction633); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				if (r!=null) r.DefineNamedAction(amp,id,a);
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
	// $ANTLR end "ruleAction"

	public class modifier_return : TreeRuleReturnScope
	{
		public string mod;
	}

	// $ANTLR start "modifier"
	// Grammars\\DefineGrammarItemsWalker.g3:250:0: modifier returns [string mod] : ( 'protected' | 'public' | 'private' | 'fragment' );
	private DefineGrammarItemsWalker.modifier_return modifier(  )
	{
		DefineGrammarItemsWalker.modifier_return retval = new DefineGrammarItemsWalker.modifier_return();
		retval.start = input.LT(1);


			retval.mod = ((GrammarAST)retval.Start).Token.Text;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:255:4: ( 'protected' | 'public' | 'private' | 'fragment' )
			// Grammars\\DefineGrammarItemsWalker.g3:
			{
			if ( input.LA(1)==FRAGMENT||(input.LA(1)>=PRIVATE && input.LA(1)<=PUBLIC) )
			{
				input.Consume();
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
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
	// Grammars\\DefineGrammarItemsWalker.g3:261:0: throwsSpec returns [HashSet<string> exceptions] : ^( 'throws' ( ID )+ ) ;
	private HashSet<string> throwsSpec(  )
	{
		HashSet<string> exceptions = default(HashSet<string>);

		GrammarAST ID6=null;


			exceptions = new HashSet<string>();

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:266:4: ( ^( 'throws' ( ID )+ ) )
			// Grammars\\DefineGrammarItemsWalker.g3:266:4: ^( 'throws' ( ID )+ )
			{
			Match(input,THROWS,Follow._THROWS_in_throwsSpec693); if (state.failed) return exceptions;

			Match(input, TokenTypes.Down, null); if (state.failed) return exceptions;
			// Grammars\\DefineGrammarItemsWalker.g3:266:15: ( ID )+
			int cnt24=0;
			for ( ; ; )
			{
				int alt24=2;
				int LA24_0 = input.LA(1);

				if ( (LA24_0==ID) )
				{
					alt24=1;
				}


				switch ( alt24 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:266:16: ID
					{
					ID6=(GrammarAST)Match(input,ID,Follow._ID_in_throwsSpec696); if (state.failed) return exceptions;
					if ( state.backtracking == 0 )
					{
						exceptions.Add((ID6!=null?ID6.Text:null));
					}

					}
					break;

				default:
					if ( cnt24 >= 1 )
						goto loop24;

					if (state.backtracking>0) {state.failed=true; return exceptions;}
					EarlyExitException eee24 = new EarlyExitException( 24, input );
					throw eee24;
				}
				cnt24++;
			}
			loop24:
				;



			Match(input, TokenTypes.Up, null); if (state.failed) return exceptions;

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
		return exceptions;
	}
	// $ANTLR end "throwsSpec"


	// $ANTLR start "ruleScopeSpec"
	// Grammars\\DefineGrammarItemsWalker.g3:269:0: ruleScopeSpec[Rule r] : ^( 'scope' ( ( attrScopeAction )* attrs= ACTION )? (uses= ID )* ) ;
	private void ruleScopeSpec( Rule r )
	{
		AttributeScopeActions_stack.Push(new AttributeScopeActions_scope());AttributeScopeActions_scopeInit(AttributeScopeActions_stack.Peek());

		GrammarAST attrs=null;
		GrammarAST uses=null;


			((AttributeScopeActions_scope)AttributeScopeActions_stack.Peek()).actions = new Dictionary<GrammarAST, GrammarAST>();

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:275:4: ( ^( 'scope' ( ( attrScopeAction )* attrs= ACTION )? (uses= ID )* ) )
			// Grammars\\DefineGrammarItemsWalker.g3:275:4: ^( 'scope' ( ( attrScopeAction )* attrs= ACTION )? (uses= ID )* )
			{
			Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec726); if (state.failed) return ;

			if ( input.LA(1)==TokenTypes.Down )
			{
				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				// Grammars\\DefineGrammarItemsWalker.g3:276:4: ( ( attrScopeAction )* attrs= ACTION )?
				int alt26=2;
				int LA26_0 = input.LA(1);

				if ( (LA26_0==ACTION||LA26_0==AMPERSAND) )
				{
					alt26=1;
				}
				switch ( alt26 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:276:6: ( attrScopeAction )* attrs= ACTION
					{
					// Grammars\\DefineGrammarItemsWalker.g3:276:6: ( attrScopeAction )*
					for ( ; ; )
					{
						int alt25=2;
						int LA25_0 = input.LA(1);

						if ( (LA25_0==AMPERSAND) )
						{
							alt25=1;
						}


						switch ( alt25 )
						{
						case 1:
							// Grammars\\DefineGrammarItemsWalker.g3:276:0: attrScopeAction
							{
							PushFollow(Follow._attrScopeAction_in_ruleScopeSpec733);
							attrScopeAction();

							state._fsp--;
							if (state.failed) return ;

							}
							break;

						default:
							goto loop25;
						}
					}

					loop25:
						;


					attrs=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec738); if (state.failed) return ;
					if ( state.backtracking == 0 )
					{

											r.ruleScope = grammar.CreateRuleScope(r.name,attrs.token);
											r.ruleScope.isDynamicRuleScope = true;
											r.ruleScope.AddAttributes((attrs!=null?attrs.Text:null), ';');
											foreach ( var action in ((AttributeScopeActions_scope)AttributeScopeActions_stack.Peek()).actions )
												r.ruleScope.DefineNamedAction( action.Key, action.Value );
										
					}

					}
					break;

				}

				// Grammars\\DefineGrammarItemsWalker.g3:285:4: (uses= ID )*
				for ( ; ; )
				{
					int alt27=2;
					int LA27_0 = input.LA(1);

					if ( (LA27_0==ID) )
					{
						alt27=1;
					}


					switch ( alt27 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:285:6: uses= ID
						{
						uses=(GrammarAST)Match(input,ID,Follow._ID_in_ruleScopeSpec759); if (state.failed) return ;
						if ( state.backtracking == 0 )
						{

												if ( grammar.GetGlobalScope((uses!=null?uses.Text:null))==null ) {
												ErrorManager.GrammarError(ErrorManager.MSG_UNKNOWN_DYNAMIC_SCOPE,
												grammar,
												uses.token,
												(uses!=null?uses.Text:null));
												}
												else {
												if ( r.useScopes==null ) {r.useScopes=new List<string>();}
												r.useScopes.Add((uses!=null?uses.Text:null));
												}
											
						}

						}
						break;

					default:
						goto loop27;
					}
				}

				loop27:
					;



				Match(input, TokenTypes.Up, null); if (state.failed) return ;
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
			AttributeScopeActions_scopeAfter(AttributeScopeActions_stack.Peek());AttributeScopeActions_stack.Pop();

		}
		return ;
	}
	// $ANTLR end "ruleScopeSpec"

	public class block_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "block"
	// Grammars\\DefineGrammarItemsWalker.g3:302:0: block : ^( BLOCK ( optionsSpec )? ( blockAction )* ( alternative rewrite )+ EOB ) ;
	private DefineGrammarItemsWalker.block_return block(  )
	{
		DefineGrammarItemsWalker.block_return retval = new DefineGrammarItemsWalker.block_return();
		retval.start = input.LT(1);


			// must run during backtracking
			this.blockLevel++;
			if ( blockLevel == 1 )
				this.outerAltNum=1;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:310:4: ( ^( BLOCK ( optionsSpec )? ( blockAction )* ( alternative rewrite )+ EOB ) )
			// Grammars\\DefineGrammarItemsWalker.g3:310:4: ^( BLOCK ( optionsSpec )? ( blockAction )* ( alternative rewrite )+ EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_block793); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			// Grammars\\DefineGrammarItemsWalker.g3:311:4: ( optionsSpec )?
			int alt28=2;
			int LA28_0 = input.LA(1);

			if ( (LA28_0==OPTIONS) )
			{
				alt28=1;
			}
			switch ( alt28 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:311:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_block799);
				optionsSpec();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;

			}

			// Grammars\\DefineGrammarItemsWalker.g3:312:4: ( blockAction )*
			for ( ; ; )
			{
				int alt29=2;
				int LA29_0 = input.LA(1);

				if ( (LA29_0==AMPERSAND) )
				{
					alt29=1;
				}


				switch ( alt29 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:312:5: blockAction
					{
					PushFollow(Follow._blockAction_in_block807);
					blockAction();

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				default:
					goto loop29;
				}
			}

			loop29:
				;


			// Grammars\\DefineGrammarItemsWalker.g3:313:4: ( alternative rewrite )+
			int cnt30=0;
			for ( ; ; )
			{
				int alt30=2;
				int LA30_0 = input.LA(1);

				if ( (LA30_0==ALT) )
				{
					alt30=1;
				}


				switch ( alt30 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:313:6: alternative rewrite
					{
					PushFollow(Follow._alternative_in_block816);
					alternative();

					state._fsp--;
					if (state.failed) return retval;
					PushFollow(Follow._rewrite_in_block818);
					rewrite();

					state._fsp--;
					if (state.failed) return retval;

										if ( this.blockLevel == 1 )
											this.outerAltNum++;
									

					}
					break;

				default:
					if ( cnt30 >= 1 )
						goto loop30;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee30 = new EarlyExitException( 30, input );
					throw eee30;
				}
				cnt30++;
			}
			loop30:
				;


			Match(input,EOB,Follow._EOB_in_block835); if (state.failed) return retval;

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;

			}

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			 blockLevel--; 
		}
		return retval;
	}
	// $ANTLR end "block"


	// $ANTLR start "blockAction"
	// Grammars\\DefineGrammarItemsWalker.g3:325:0: blockAction : ^(amp= AMPERSAND id= ID a= ACTION ) ;
	private void blockAction(  )
	{
		GrammarAST amp=null;
		GrammarAST id=null;
		GrammarAST a=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:326:4: ( ^(amp= AMPERSAND id= ID a= ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:326:4: ^(amp= AMPERSAND id= ID a= ACTION )
			{
			amp=(GrammarAST)Match(input,AMPERSAND,Follow._AMPERSAND_in_blockAction859); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			id=(GrammarAST)Match(input,ID,Follow._ID_in_blockAction863); if (state.failed) return ;
			a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_blockAction867); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "blockAction"

	public class alternative_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "alternative"
	// Grammars\\DefineGrammarItemsWalker.g3:329:0: alternative : ^( ALT ( element )+ EOA ) ;
	private DefineGrammarItemsWalker.alternative_return alternative(  )
	{
		DefineGrammarItemsWalker.alternative_return retval = new DefineGrammarItemsWalker.alternative_return();
		retval.start = input.LT(1);


			if ( state.backtracking == 0 )
			{
				if ( grammar.type!=GrammarType.Lexer && grammar.GetOption("output")!=null && blockLevel==1 )
				{
					GrammarAST aRewriteNode = ((GrammarAST)retval.Start).FindFirstType(REWRITE); // alt itself has rewrite?
					GrammarAST rewriteAST = (GrammarAST)((GrammarAST)retval.Start).Parent.GetChild(((GrammarAST)retval.Start).ChildIndex + 1);
					// we have a rewrite if alt uses it inside subrule or this alt has one
					// but don't count -> ... rewrites, which mean "do default auto construction"
					if ( aRewriteNode!=null||
						 (rewriteAST!=null &&
						  rewriteAST.Type==REWRITE &&
						  rewriteAST.GetChild(0)!=null &&
						  rewriteAST.GetChild(0).Type!=ETC) )
					{
						Rule r = grammar.GetRule(currentRuleName);
						r.TrackAltsWithRewrites(((GrammarAST)retval.Start),this.outerAltNum);
					}
				}
			}

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:352:4: ( ^( ALT ( element )+ EOA ) )
			// Grammars\\DefineGrammarItemsWalker.g3:352:4: ^( ALT ( element )+ EOA )
			{
			Match(input,ALT,Follow._ALT_in_alternative888); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			// Grammars\\DefineGrammarItemsWalker.g3:352:11: ( element )+
			int cnt31=0;
			for ( ; ; )
			{
				int alt31=2;
				int LA31_0 = input.LA(1);

				if ( (LA31_0==ACTION||(LA31_0>=ASSIGN && LA31_0<=BLOCK)||(LA31_0>=CHAR_LITERAL && LA31_0<=CHAR_RANGE)||LA31_0==CLOSURE||LA31_0==DOT||LA31_0==EPSILON||LA31_0==FORCED_ACTION||LA31_0==GATED_SEMPRED||LA31_0==NOT||LA31_0==OPTIONAL||(LA31_0>=PLUS_ASSIGN && LA31_0<=POSITIVE_CLOSURE)||LA31_0==RANGE||LA31_0==ROOT||LA31_0==RULE_REF||LA31_0==SEMPRED||(LA31_0>=STRING_LITERAL && LA31_0<=SYNPRED)||LA31_0==TOKEN_REF||LA31_0==TREE_BEGIN||LA31_0==WILDCARD) )
				{
					alt31=1;
				}


				switch ( alt31 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:352:12: element
					{
					PushFollow(Follow._element_in_alternative891);
					element();

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				default:
					if ( cnt31 >= 1 )
						goto loop31;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee31 = new EarlyExitException( 31, input );
					throw eee31;
				}
				cnt31++;
			}
			loop31:
				;


			Match(input,EOA,Follow._EOA_in_alternative895); if (state.failed) return retval;

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;

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
	// $ANTLR end "alternative"


	// $ANTLR start "exceptionGroup"
	// Grammars\\DefineGrammarItemsWalker.g3:355:0: exceptionGroup : ( ( exceptionHandler )+ ( finallyClause )? | finallyClause );
	private void exceptionGroup(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:356:4: ( ( exceptionHandler )+ ( finallyClause )? | finallyClause )
			int alt34=2;
			int LA34_0 = input.LA(1);

			if ( (LA34_0==CATCH) )
			{
				alt34=1;
			}
			else if ( (LA34_0==FINALLY) )
			{
				alt34=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 34, 0, input);

				throw nvae;
			}
			switch ( alt34 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:356:4: ( exceptionHandler )+ ( finallyClause )?
				{
				// Grammars\\DefineGrammarItemsWalker.g3:356:4: ( exceptionHandler )+
				int cnt32=0;
				for ( ; ; )
				{
					int alt32=2;
					int LA32_0 = input.LA(1);

					if ( (LA32_0==CATCH) )
					{
						alt32=1;
					}


					switch ( alt32 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:356:6: exceptionHandler
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup910);
						exceptionHandler();

						state._fsp--;
						if (state.failed) return ;

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


				// Grammars\\DefineGrammarItemsWalker.g3:356:26: ( finallyClause )?
				int alt33=2;
				int LA33_0 = input.LA(1);

				if ( (LA33_0==FINALLY) )
				{
					alt33=1;
				}
				switch ( alt33 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:356:27: finallyClause
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup916);
					finallyClause();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:357:4: finallyClause
				{
				PushFollow(Follow._finallyClause_in_exceptionGroup923);
				finallyClause();

				state._fsp--;
				if (state.failed) return ;

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
	// Grammars\\DefineGrammarItemsWalker.g3:360:0: exceptionHandler : ^( 'catch' ARG_ACTION ACTION ) ;
	private void exceptionHandler(  )
	{
		GrammarAST ACTION7=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:361:6: ( ^( 'catch' ARG_ACTION ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:361:6: ^( 'catch' ARG_ACTION ACTION )
			{
			Match(input,CATCH,Follow._CATCH_in_exceptionHandler937); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler939); if (state.failed) return ;
			ACTION7=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_exceptionHandler941); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				TrackInlineAction(ACTION7);
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
	// $ANTLR end "exceptionHandler"


	// $ANTLR start "finallyClause"
	// Grammars\\DefineGrammarItemsWalker.g3:364:0: finallyClause : ^( 'finally' ACTION ) ;
	private void finallyClause(  )
	{
		GrammarAST ACTION8=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:365:7: ( ^( 'finally' ACTION ) )
			// Grammars\\DefineGrammarItemsWalker.g3:365:7: ^( 'finally' ACTION )
			{
			Match(input,FINALLY,Follow._FINALLY_in_finallyClause959); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			ACTION8=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_finallyClause961); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			if ( state.backtracking == 0 )
			{
				TrackInlineAction(ACTION8);
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
	// $ANTLR end "finallyClause"

	public class element_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "element"
	// Grammars\\DefineGrammarItemsWalker.g3:368:0: element : ( ^( ROOT element ) | ^( BANG element ) | atom[null] | ^( NOT element ) | ^( RANGE atom[null] atom[null] ) | ^( CHAR_RANGE atom[null] atom[null] ) | ^( ASSIGN id= ID el= element ) | ^( PLUS_ASSIGN id2= ID a2= element ) | ebnf | tree_ | ^( SYNPRED block ) |act= ACTION |act2= FORCED_ACTION | SEMPRED | SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) | GATED_SEMPRED | EPSILON );
	private DefineGrammarItemsWalker.element_return element(  )
	{
		DefineGrammarItemsWalker.element_return retval = new DefineGrammarItemsWalker.element_return();
		retval.start = input.LT(1);

		GrammarAST id=null;
		GrammarAST id2=null;
		GrammarAST act=null;
		GrammarAST act2=null;
		GrammarAST SEMPRED9=null;
		GrammarAST GATED_SEMPRED10=null;
		DefineGrammarItemsWalker.element_return el = default(DefineGrammarItemsWalker.element_return);
		DefineGrammarItemsWalker.element_return a2 = default(DefineGrammarItemsWalker.element_return);

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:369:6: ( ^( ROOT element ) | ^( BANG element ) | atom[null] | ^( NOT element ) | ^( RANGE atom[null] atom[null] ) | ^( CHAR_RANGE atom[null] atom[null] ) | ^( ASSIGN id= ID el= element ) | ^( PLUS_ASSIGN id2= ID a2= element ) | ebnf | tree_ | ^( SYNPRED block ) |act= ACTION |act2= FORCED_ACTION | SEMPRED | SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) | GATED_SEMPRED | EPSILON )
			int alt36=18;
			alt36 = dfa36.Predict(input);
			switch ( alt36 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:369:6: ^( ROOT element )
				{
				Match(input,ROOT,Follow._ROOT_in_element978); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element980);
				element();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:370:6: ^( BANG element )
				{
				Match(input,BANG,Follow._BANG_in_element989); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element991);
				element();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:371:6: atom[null]
				{
				PushFollow(Follow._atom_in_element999);
				atom(null);

				state._fsp--;
				if (state.failed) return retval;

				}
				break;
			case 4:
				// Grammars\\DefineGrammarItemsWalker.g3:372:6: ^( NOT element )
				{
				Match(input,NOT,Follow._NOT_in_element1008); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._element_in_element1010);
				element();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 5:
				// Grammars\\DefineGrammarItemsWalker.g3:373:6: ^( RANGE atom[null] atom[null] )
				{
				Match(input,RANGE,Follow._RANGE_in_element1019); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._atom_in_element1021);
				atom(null);

				state._fsp--;
				if (state.failed) return retval;
				PushFollow(Follow._atom_in_element1024);
				atom(null);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 6:
				// Grammars\\DefineGrammarItemsWalker.g3:374:6: ^( CHAR_RANGE atom[null] atom[null] )
				{
				Match(input,CHAR_RANGE,Follow._CHAR_RANGE_in_element1034); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._atom_in_element1036);
				atom(null);

				state._fsp--;
				if (state.failed) return retval;
				PushFollow(Follow._atom_in_element1039);
				atom(null);

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 7:
				// Grammars\\DefineGrammarItemsWalker.g3:375:4: ^( ASSIGN id= ID el= element )
				{
				Match(input,ASSIGN,Follow._ASSIGN_in_element1048); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				id=(GrammarAST)Match(input,ID,Follow._ID_in_element1052); if (state.failed) return retval;
				PushFollow(Follow._element_in_element1056);
				el=element();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

									GrammarAST e = (el!=null?((GrammarAST)el.Start):null);
									if ( e.Type==ANTLRParser.ROOT || e.Type==ANTLRParser.BANG )
									{
										e = (GrammarAST)e.GetChild(0);
									}
									if ( e.Type==RULE_REF)
									{
										grammar.DefineRuleRefLabel(currentRuleName,id.token,e);
									}
									else if ( e.Type==WILDCARD && grammar.type==GrammarType.TreeParser )
									{
										grammar.DefineWildcardTreeLabel(currentRuleName,id.token,e);
									}
									else
									{
										grammar.DefineTokenRefLabel(currentRuleName,id.Token,e);
									}
								
				}

				}
				break;
			case 8:
				// Grammars\\DefineGrammarItemsWalker.g3:395:4: ^( PLUS_ASSIGN id2= ID a2= element )
				{
				Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_element1069); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				id2=(GrammarAST)Match(input,ID,Follow._ID_in_element1073); if (state.failed) return retval;
				PushFollow(Follow._element_in_element1077);
				a2=element();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

									GrammarAST a = (a2!=null?((GrammarAST)a2.Start):null);
									if ( a.Type==ANTLRParser.ROOT || a.Type==ANTLRParser.BANG )
									{
										a = (GrammarAST)a.GetChild(0);
									}
									if ( a.Type==RULE_REF )
									{
										grammar.DefineRuleListLabel(currentRuleName,id2.Token,a);
									}
									else if ( a.Type == WILDCARD && grammar.type == GrammarType.TreeParser )
									{
										grammar.DefineWildcardTreeListLabel( currentRuleName, id2.token, a );
									}
									else
									{
										grammar.DefineTokenListLabel(currentRuleName,id2.Token,a);
									}
								
				}

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 9:
				// Grammars\\DefineGrammarItemsWalker.g3:416:6: ebnf
				{
				PushFollow(Follow._ebnf_in_element1094);
				ebnf();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;
			case 10:
				// Grammars\\DefineGrammarItemsWalker.g3:417:6: tree_
				{
				PushFollow(Follow._tree__in_element1101);
				tree_();

				state._fsp--;
				if (state.failed) return retval;

				}
				break;
			case 11:
				// Grammars\\DefineGrammarItemsWalker.g3:418:6: ^( SYNPRED block )
				{
				Match(input,SYNPRED,Follow._SYNPRED_in_element1110); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._block_in_element1112);
				block();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 12:
				// Grammars\\DefineGrammarItemsWalker.g3:419:6: act= ACTION
				{
				act=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_element1123); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								act.outerAltNum = this.outerAltNum;
								TrackInlineAction(act);
							
				}

				}
				break;
			case 13:
				// Grammars\\DefineGrammarItemsWalker.g3:424:6: act2= FORCED_ACTION
				{
				act2=(GrammarAST)Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_element1136); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								act2.outerAltNum = this.outerAltNum;
								TrackInlineAction(act2);
							
				}

				}
				break;
			case 14:
				// Grammars\\DefineGrammarItemsWalker.g3:429:6: SEMPRED
				{
				SEMPRED9=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_element1147); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								SEMPRED9.outerAltNum = this.outerAltNum;
								TrackInlineAction(SEMPRED9);
							
				}

				}
				break;
			case 15:
				// Grammars\\DefineGrammarItemsWalker.g3:434:6: SYN_SEMPRED
				{
				Match(input,SYN_SEMPRED,Follow._SYN_SEMPRED_in_element1158); if (state.failed) return retval;

				}
				break;
			case 16:
				// Grammars\\DefineGrammarItemsWalker.g3:435:6: ^( BACKTRACK_SEMPRED ( . )* )
				{
				Match(input,BACKTRACK_SEMPRED,Follow._BACKTRACK_SEMPRED_in_element1166); if (state.failed) return retval;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\DefineGrammarItemsWalker.g3:435:26: ( . )*
					for ( ; ; )
					{
						int alt35=2;
						int LA35_0 = input.LA(1);

						if ( ((LA35_0>=ACTION && LA35_0<=XDIGIT)) )
						{
							alt35=1;
						}
						else if ( (LA35_0==UP) )
						{
							alt35=2;
						}


						switch ( alt35 )
						{
						case 1:
							// Grammars\\DefineGrammarItemsWalker.g3:435:0: .
							{
							MatchAny(input); if (state.failed) return retval;

							}
							break;

						default:
							goto loop35;
						}
					}

					loop35:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
				}

				}
				break;
			case 17:
				// Grammars\\DefineGrammarItemsWalker.g3:436:6: GATED_SEMPRED
				{
				GATED_SEMPRED10=(GrammarAST)Match(input,GATED_SEMPRED,Follow._GATED_SEMPRED_in_element1177); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								GATED_SEMPRED10.outerAltNum = this.outerAltNum;
								TrackInlineAction(GATED_SEMPRED10);
							
				}

				}
				break;
			case 18:
				// Grammars\\DefineGrammarItemsWalker.g3:441:6: EPSILON
				{
				Match(input,EPSILON,Follow._EPSILON_in_element1188); if (state.failed) return retval;

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
	// $ANTLR end "element"


	// $ANTLR start "ebnf"
	// Grammars\\DefineGrammarItemsWalker.g3:444:0: ebnf : (=> dotLoop | block | ^( OPTIONAL block ) | ^( CLOSURE block ) | ^( POSITIVE_CLOSURE block ) );
	private void ebnf(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:445:4: (=> dotLoop | block | ^( OPTIONAL block ) | ^( CLOSURE block ) | ^( POSITIVE_CLOSURE block ) )
			int alt37=5;
			alt37 = dfa37.Predict(input);
			switch ( alt37 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:445:4: => dotLoop
				{

				PushFollow(Follow._dotLoop_in_ebnf1206);
				dotLoop();

				state._fsp--;
				if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:446:4: block
				{
				PushFollow(Follow._block_in_ebnf1212);
				block();

				state._fsp--;
				if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:447:4: ^( OPTIONAL block )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_ebnf1219); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._block_in_ebnf1221);
				block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 4:
				// Grammars\\DefineGrammarItemsWalker.g3:448:4: ^( CLOSURE block )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_ebnf1230); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._block_in_ebnf1232);
				block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 5:
				// Grammars\\DefineGrammarItemsWalker.g3:449:4: ^( POSITIVE_CLOSURE block )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_ebnf1241); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._block_in_ebnf1243);
				block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

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

	public class dotLoop_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "dotLoop"
	// Grammars\\DefineGrammarItemsWalker.g3:454:0: dotLoop : ( ^( CLOSURE dotBlock ) | ^( POSITIVE_CLOSURE dotBlock ) ) ;
	private DefineGrammarItemsWalker.dotLoop_return dotLoop(  )
	{
		DefineGrammarItemsWalker.dotLoop_return retval = new DefineGrammarItemsWalker.dotLoop_return();
		retval.start = input.LT(1);

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:455:4: ( ( ^( CLOSURE dotBlock ) | ^( POSITIVE_CLOSURE dotBlock ) ) )
			// Grammars\\DefineGrammarItemsWalker.g3:455:4: ( ^( CLOSURE dotBlock ) | ^( POSITIVE_CLOSURE dotBlock ) )
			{
			// Grammars\\DefineGrammarItemsWalker.g3:455:4: ( ^( CLOSURE dotBlock ) | ^( POSITIVE_CLOSURE dotBlock ) )
			int alt38=2;
			int LA38_0 = input.LA(1);

			if ( (LA38_0==CLOSURE) )
			{
				alt38=1;
			}
			else if ( (LA38_0==POSITIVE_CLOSURE) )
			{
				alt38=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 38, 0, input);

				throw nvae;
			}
			switch ( alt38 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:455:6: ^( CLOSURE dotBlock )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_dotLoop1262); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._dotBlock_in_dotLoop1264);
				dotBlock();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:456:5: ^( POSITIVE_CLOSURE dotBlock )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_dotLoop1274); if (state.failed) return retval;

				Match(input, TokenTypes.Down, null); if (state.failed) return retval;
				PushFollow(Follow._dotBlock_in_dotLoop1276);
				dotBlock();

				state._fsp--;
				if (state.failed) return retval;

				Match(input, TokenTypes.Up, null); if (state.failed) return retval;

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

							GrammarAST block = (GrammarAST)((GrammarAST)retval.Start).GetChild(0);
							IDictionary<string, object> opts=new Dictionary<string, object>();
							opts["greedy"] = "false";
							if ( grammar.type!=GrammarType.Lexer )
							{
								// parser grammars assume k=1 for .* loops
								// otherwise they (analysis?) look til EOF!
								opts["k"] = 1;
							}
							block.SetOptions(grammar,opts);
						
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
	// $ANTLR end "dotLoop"


	// $ANTLR start "dotBlock"
	// Grammars\\DefineGrammarItemsWalker.g3:472:0: dotBlock : ^( BLOCK ^( ALT WILDCARD EOA ) EOB ) ;
	private void dotBlock(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:473:4: ( ^( BLOCK ^( ALT WILDCARD EOA ) EOB ) )
			// Grammars\\DefineGrammarItemsWalker.g3:473:4: ^( BLOCK ^( ALT WILDCARD EOA ) EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_dotBlock1299); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,ALT,Follow._ALT_in_dotBlock1303); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			Match(input,WILDCARD,Follow._WILDCARD_in_dotBlock1305); if (state.failed) return ;
			Match(input,EOA,Follow._EOA_in_dotBlock1307); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;
			Match(input,EOB,Follow._EOB_in_dotBlock1311); if (state.failed) return ;

			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "dotBlock"


	// $ANTLR start "tree_"
	// Grammars\\DefineGrammarItemsWalker.g3:476:0: tree_ : ^( TREE_BEGIN ( element )+ ) ;
	private void tree_(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:477:4: ( ^( TREE_BEGIN ( element )+ ) )
			// Grammars\\DefineGrammarItemsWalker.g3:477:4: ^( TREE_BEGIN ( element )+ )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_1325); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:477:17: ( element )+
			int cnt39=0;
			for ( ; ; )
			{
				int alt39=2;
				int LA39_0 = input.LA(1);

				if ( (LA39_0==ACTION||(LA39_0>=ASSIGN && LA39_0<=BLOCK)||(LA39_0>=CHAR_LITERAL && LA39_0<=CHAR_RANGE)||LA39_0==CLOSURE||LA39_0==DOT||LA39_0==EPSILON||LA39_0==FORCED_ACTION||LA39_0==GATED_SEMPRED||LA39_0==NOT||LA39_0==OPTIONAL||(LA39_0>=PLUS_ASSIGN && LA39_0<=POSITIVE_CLOSURE)||LA39_0==RANGE||LA39_0==ROOT||LA39_0==RULE_REF||LA39_0==SEMPRED||(LA39_0>=STRING_LITERAL && LA39_0<=SYNPRED)||LA39_0==TOKEN_REF||LA39_0==TREE_BEGIN||LA39_0==WILDCARD) )
				{
					alt39=1;
				}


				switch ( alt39 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:477:0: element
					{
					PushFollow(Follow._element_in_tree_1327);
					element();

					state._fsp--;
					if (state.failed) return ;

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



			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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


	// $ANTLR start "atom"
	// Grammars\\DefineGrammarItemsWalker.g3:480:0: atom[GrammarAST scope_] : ( ^(rr= RULE_REF (rarg= ARG_ACTION )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL | WILDCARD | ^( DOT ID atom[$ID] ) );
	private void atom( GrammarAST scope_ )
	{
		GrammarAST rr=null;
		GrammarAST rarg=null;
		GrammarAST t=null;
		GrammarAST targ=null;
		GrammarAST c=null;
		GrammarAST s=null;
		GrammarAST ID11=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:481:4: ( ^(rr= RULE_REF (rarg= ARG_ACTION )? ) | ^(t= TOKEN_REF (targ= ARG_ACTION )? ) |c= CHAR_LITERAL |s= STRING_LITERAL | WILDCARD | ^( DOT ID atom[$ID] ) )
			int alt42=6;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt42=1;
				}
				break;
			case TOKEN_REF:
				{
				alt42=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt42=3;
				}
				break;
			case STRING_LITERAL:
				{
				alt42=4;
				}
				break;
			case WILDCARD:
				{
				alt42=5;
				}
				break;
			case DOT:
				{
				alt42=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 42, 0, input);

					throw nvae;
				}
			}

			switch ( alt42 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:481:4: ^(rr= RULE_REF (rarg= ARG_ACTION )? )
				{
				rr=(GrammarAST)Match(input,RULE_REF,Follow._RULE_REF_in_atom1345); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\DefineGrammarItemsWalker.g3:481:19: (rarg= ARG_ACTION )?
					int alt40=2;
					int LA40_0 = input.LA(1);

					if ( (LA40_0==ARG_ACTION) )
					{
						alt40=1;
					}
					switch ( alt40 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:481:20: rarg= ARG_ACTION
						{
						rarg=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1350); if (state.failed) return ;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}
				if ( state.backtracking == 0 )
				{

								grammar.AltReferencesRule( currentRuleName, scope_, rr, this.outerAltNum );
								if ( rarg != null )
								{
									rarg.outerAltNum = this.outerAltNum;
									TrackInlineAction(rarg);
								}
							
				}

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:490:4: ^(t= TOKEN_REF (targ= ARG_ACTION )? )
				{
				t=(GrammarAST)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_atom1367); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\DefineGrammarItemsWalker.g3:490:19: (targ= ARG_ACTION )?
					int alt41=2;
					int LA41_0 = input.LA(1);

					if ( (LA41_0==ARG_ACTION) )
					{
						alt41=1;
					}
					switch ( alt41 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:490:20: targ= ARG_ACTION
						{
						targ=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_atom1372); if (state.failed) return ;

						}
						break;

					}


					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}
				if ( state.backtracking == 0 )
				{

								if ( targ != null )
								{
									targ.outerAltNum = this.outerAltNum;
									TrackInlineAction(targ);
								}
								if ( grammar.type == GrammarType.Lexer )
								{
									grammar.AltReferencesRule( currentRuleName, scope_, t, this.outerAltNum );
								}
								else
								{
									grammar.AltReferencesTokenID( currentRuleName, t, this.outerAltNum );
								}
							
				}

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:506:4: c= CHAR_LITERAL
				{
				c=(GrammarAST)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_atom1388); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type != GrammarType.Lexer )
								{
									Rule rule = grammar.GetRule(currentRuleName);
									if ( rule != null )
										rule.TrackTokenReferenceInAlt(c, outerAltNum);
								}
							
				}

				}
				break;
			case 4:
				// Grammars\\DefineGrammarItemsWalker.g3:515:4: s= STRING_LITERAL
				{
				s=(GrammarAST)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_atom1399); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								if ( grammar.type != GrammarType.Lexer )
								{
									Rule rule = grammar.GetRule(currentRuleName);
									if ( rule!=null )
										rule.TrackTokenReferenceInAlt(s, outerAltNum);
								}
							
				}

				}
				break;
			case 5:
				// Grammars\\DefineGrammarItemsWalker.g3:524:4: WILDCARD
				{
				Match(input,WILDCARD,Follow._WILDCARD_in_atom1409); if (state.failed) return ;

				}
				break;
			case 6:
				// Grammars\\DefineGrammarItemsWalker.g3:525:4: ^( DOT ID atom[$ID] )
				{
				Match(input,DOT,Follow._DOT_in_atom1415); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				ID11=(GrammarAST)Match(input,ID,Follow._ID_in_atom1417); if (state.failed) return ;
				PushFollow(Follow._atom_in_atom1419);
				atom(ID11);

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "atom"


	// $ANTLR start "ast_suffix"
	// Grammars\\DefineGrammarItemsWalker.g3:528:0: ast_suffix : ( ROOT | BANG );
	private void ast_suffix(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:529:4: ( ROOT | BANG )
			// Grammars\\DefineGrammarItemsWalker.g3:
			{
			if ( input.LA(1)==BANG||input.LA(1)==ROOT )
			{
				input.Consume();
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
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
		return ;
	}
	// $ANTLR end "ast_suffix"

	public class rewrite_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rewrite"
	// Grammars\\DefineGrammarItemsWalker.g3:533:0: rewrite : ( ^( REWRITE (pred= SEMPRED )? rewrite_alternative ) )* ;
	private DefineGrammarItemsWalker.rewrite_return rewrite(  )
	{
		DefineGrammarItemsWalker.rewrite_return retval = new DefineGrammarItemsWalker.rewrite_return();
		retval.start = input.LT(1);

		GrammarAST pred=null;


			currentRewriteRule = ((GrammarAST)retval.Start); // has to execute during backtracking
			if ( state.backtracking == 0 )
			{
				if ( grammar.BuildAST )
					((GrammarAST)retval.Start).rewriteRefsDeep = new HashSet<GrammarAST>();
			}

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:543:4: ( ( ^( REWRITE (pred= SEMPRED )? rewrite_alternative ) )* )
			// Grammars\\DefineGrammarItemsWalker.g3:543:4: ( ^( REWRITE (pred= SEMPRED )? rewrite_alternative ) )*
			{
			// Grammars\\DefineGrammarItemsWalker.g3:543:4: ( ^( REWRITE (pred= SEMPRED )? rewrite_alternative ) )*
			for ( ; ; )
			{
				int alt44=2;
				int LA44_0 = input.LA(1);

				if ( (LA44_0==REWRITE) )
				{
					alt44=1;
				}


				switch ( alt44 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:544:4: ^( REWRITE (pred= SEMPRED )? rewrite_alternative )
					{
					Match(input,REWRITE,Follow._REWRITE_in_rewrite1461); if (state.failed) return retval;

					Match(input, TokenTypes.Down, null); if (state.failed) return retval;
					// Grammars\\DefineGrammarItemsWalker.g3:544:15: (pred= SEMPRED )?
					int alt43=2;
					int LA43_0 = input.LA(1);

					if ( (LA43_0==SEMPRED) )
					{
						alt43=1;
					}
					switch ( alt43 )
					{
					case 1:
						// Grammars\\DefineGrammarItemsWalker.g3:544:16: pred= SEMPRED
						{
						pred=(GrammarAST)Match(input,SEMPRED,Follow._SEMPRED_in_rewrite1466); if (state.failed) return retval;

						}
						break;

					}

					PushFollow(Follow._rewrite_alternative_in_rewrite1470);
					rewrite_alternative();

					state._fsp--;
					if (state.failed) return retval;

					Match(input, TokenTypes.Up, null); if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{

										if ( pred != null )
										{
											pred.outerAltNum = this.outerAltNum;
											TrackInlineAction(pred);
										}
									
					}

					}
					break;

				default:
					goto loop44;
				}
			}

			loop44:
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
		return retval;
	}
	// $ANTLR end "rewrite"

	public class rewrite_block_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rewrite_block"
	// Grammars\\DefineGrammarItemsWalker.g3:556:0: rewrite_block : ^( BLOCK rewrite_alternative EOB ) ;
	private DefineGrammarItemsWalker.rewrite_block_return rewrite_block(  )
	{
		DefineGrammarItemsWalker.rewrite_block_return retval = new DefineGrammarItemsWalker.rewrite_block_return();
		retval.start = input.LT(1);


			GrammarAST enclosingBlock = currentRewriteBlock;
			if ( state.backtracking == 0 )
			{
				// don't do if guessing
				currentRewriteBlock=((GrammarAST)retval.Start); // pts to BLOCK node
				currentRewriteBlock.rewriteRefsShallow = new HashSet<GrammarAST>();
				currentRewriteBlock.rewriteRefsDeep = new HashSet<GrammarAST>();
			}

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:568:6: ( ^( BLOCK rewrite_alternative EOB ) )
			// Grammars\\DefineGrammarItemsWalker.g3:568:6: ^( BLOCK rewrite_alternative EOB )
			{
			Match(input,BLOCK,Follow._BLOCK_in_rewrite_block1505); if (state.failed) return retval;

			Match(input, TokenTypes.Down, null); if (state.failed) return retval;
			PushFollow(Follow._rewrite_alternative_in_rewrite_block1507);
			rewrite_alternative();

			state._fsp--;
			if (state.failed) return retval;
			Match(input,EOB,Follow._EOB_in_rewrite_block1509); if (state.failed) return retval;

			Match(input, TokenTypes.Up, null); if (state.failed) return retval;
			if ( state.backtracking == 0 )
			{

							// copy the element refs in this block to the surrounding block
							if ( enclosingBlock != null )
							{
								foreach ( var item in currentRewriteBlock.rewriteRefsShallow )
									enclosingBlock.rewriteRefsDeep.Add( item );
							}
							//currentRewriteBlock = enclosingBlock; // restore old BLOCK ptr
						
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
			 currentRewriteBlock = enclosingBlock; 
		}
		return retval;
	}
	// $ANTLR end "rewrite_block"


	// $ANTLR start "rewrite_alternative"
	// Grammars\\DefineGrammarItemsWalker.g3:582:0: rewrite_alternative : ({...}? => ^(a= ALT ( ( rewrite_element )+ | EPSILON ) EOA ) |{...}? => rewrite_template | ETC {...}?);
	private void rewrite_alternative(  )
	{
		GrammarAST a=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:583:4: ({...}? => ^(a= ALT ( ( rewrite_element )+ | EPSILON ) EOA ) |{...}? => rewrite_template | ETC {...}?)
			int alt47=3;
			int LA47_0 = input.LA(1);

			if ( (LA47_0==ALT) && (((grammar.BuildAST)||(grammar.BuildTemplate))))
			{
				int LA47_1 = input.LA(2);

				if ( (LA47_1==DOWN) && (((grammar.BuildAST)||(grammar.BuildTemplate))))
				{
					int LA47_4 = input.LA(3);

					if ( (LA47_4==EPSILON) && (((grammar.BuildTemplate)||(grammar.BuildAST))))
					{
						int LA47_5 = input.LA(4);

						if ( (LA47_5==EOA) && (((grammar.BuildTemplate)||(grammar.BuildAST))))
						{
							int LA47_7 = input.LA(5);

							if ( (LA47_7==UP) && (((grammar.BuildTemplate)||(grammar.BuildAST))))
							{
								int LA47_8 = input.LA(6);

								if ( ((grammar.BuildAST)) )
								{
									alt47=1;
								}
								else if ( ((grammar.BuildTemplate)) )
								{
									alt47=2;
								}
								else
								{
									if (state.backtracking>0) {state.failed=true; return ;}
									NoViableAltException nvae = new NoViableAltException("", 47, 8, input);

									throw nvae;
								}
							}
							else
							{
								if (state.backtracking>0) {state.failed=true; return ;}
								NoViableAltException nvae = new NoViableAltException("", 47, 7, input);

								throw nvae;
							}
						}
						else
						{
							if (state.backtracking>0) {state.failed=true; return ;}
							NoViableAltException nvae = new NoViableAltException("", 47, 5, input);

							throw nvae;
						}
					}
					else if ( (LA47_4==ACTION||LA47_4==CHAR_LITERAL||LA47_4==CLOSURE||LA47_4==LABEL||LA47_4==OPTIONAL||LA47_4==POSITIVE_CLOSURE||LA47_4==RULE_REF||LA47_4==STRING_LITERAL||LA47_4==TOKEN_REF||LA47_4==TREE_BEGIN) && ((grammar.BuildAST)))
					{
						alt47=1;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						NoViableAltException nvae = new NoViableAltException("", 47, 4, input);

						throw nvae;
					}
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 47, 1, input);

					throw nvae;
				}
			}
			else if ( (LA47_0==ACTION||LA47_0==TEMPLATE) && ((grammar.BuildTemplate)))
			{
				alt47=2;
			}
			else if ( (LA47_0==ETC) )
			{
				alt47=3;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return ;}
				NoViableAltException nvae = new NoViableAltException("", 47, 0, input);

				throw nvae;
			}
			switch ( alt47 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:583:4: {...}? => ^(a= ALT ( ( rewrite_element )+ | EPSILON ) EOA )
				{
				if ( !((grammar.BuildAST)) )
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					throw new FailedPredicateException(input, "rewrite_alternative", "grammar.BuildAST");
				}
				a=(GrammarAST)Match(input,ALT,Follow._ALT_in_rewrite_alternative1541); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				// Grammars\\DefineGrammarItemsWalker.g3:583:36: ( ( rewrite_element )+ | EPSILON )
				int alt46=2;
				int LA46_0 = input.LA(1);

				if ( (LA46_0==ACTION||LA46_0==CHAR_LITERAL||LA46_0==CLOSURE||LA46_0==LABEL||LA46_0==OPTIONAL||LA46_0==POSITIVE_CLOSURE||LA46_0==RULE_REF||LA46_0==STRING_LITERAL||LA46_0==TOKEN_REF||LA46_0==TREE_BEGIN) )
				{
					alt46=1;
				}
				else if ( (LA46_0==EPSILON) )
				{
					alt46=2;
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
					// Grammars\\DefineGrammarItemsWalker.g3:583:38: ( rewrite_element )+
					{
					// Grammars\\DefineGrammarItemsWalker.g3:583:38: ( rewrite_element )+
					int cnt45=0;
					for ( ; ; )
					{
						int alt45=2;
						int LA45_0 = input.LA(1);

						if ( (LA45_0==ACTION||LA45_0==CHAR_LITERAL||LA45_0==CLOSURE||LA45_0==LABEL||LA45_0==OPTIONAL||LA45_0==POSITIVE_CLOSURE||LA45_0==RULE_REF||LA45_0==STRING_LITERAL||LA45_0==TOKEN_REF||LA45_0==TREE_BEGIN) )
						{
							alt45=1;
						}


						switch ( alt45 )
						{
						case 1:
							// Grammars\\DefineGrammarItemsWalker.g3:583:40: rewrite_element
							{
							PushFollow(Follow._rewrite_element_in_rewrite_alternative1547);
							rewrite_element();

							state._fsp--;
							if (state.failed) return ;

							}
							break;

						default:
							if ( cnt45 >= 1 )
								goto loop45;

							if (state.backtracking>0) {state.failed=true; return ;}
							EarlyExitException eee45 = new EarlyExitException( 45, input );
							throw eee45;
						}
						cnt45++;
					}
					loop45:
						;



					}
					break;
				case 2:
					// Grammars\\DefineGrammarItemsWalker.g3:583:61: EPSILON
					{
					Match(input,EPSILON,Follow._EPSILON_in_rewrite_alternative1554); if (state.failed) return ;

					}
					break;

				}

				Match(input,EOA,Follow._EOA_in_rewrite_alternative1558); if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:584:4: {...}? => rewrite_template
				{
				if ( !((grammar.BuildTemplate)) )
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					throw new FailedPredicateException(input, "rewrite_alternative", "grammar.BuildTemplate");
				}
				PushFollow(Follow._rewrite_template_in_rewrite_alternative1569);
				rewrite_template();

				state._fsp--;
				if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:585:4: ETC {...}?
				{
				Match(input,ETC,Follow._ETC_in_rewrite_alternative1574); if (state.failed) return ;
				if ( !((this.blockLevel==1)) )
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					throw new FailedPredicateException(input, "rewrite_alternative", "this.blockLevel==1");
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
		return ;
	}
	// $ANTLR end "rewrite_alternative"


	// $ANTLR start "rewrite_element"
	// Grammars\\DefineGrammarItemsWalker.g3:588:0: rewrite_element : ( rewrite_atom | rewrite_ebnf | rewrite_tree );
	private void rewrite_element(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:589:4: ( rewrite_atom | rewrite_ebnf | rewrite_tree )
			int alt48=3;
			switch ( input.LA(1) )
			{
			case ACTION:
			case CHAR_LITERAL:
			case LABEL:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
				{
				alt48=1;
				}
				break;
			case CLOSURE:
			case OPTIONAL:
			case POSITIVE_CLOSURE:
				{
				alt48=2;
				}
				break;
			case TREE_BEGIN:
				{
				alt48=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 48, 0, input);

					throw nvae;
				}
			}

			switch ( alt48 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:589:4: rewrite_atom
				{
				PushFollow(Follow._rewrite_atom_in_rewrite_element1588);
				rewrite_atom();

				state._fsp--;
				if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:590:4: rewrite_ebnf
				{
				PushFollow(Follow._rewrite_ebnf_in_rewrite_element1593);
				rewrite_ebnf();

				state._fsp--;
				if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:591:4: rewrite_tree
				{
				PushFollow(Follow._rewrite_tree_in_rewrite_element1598);
				rewrite_tree();

				state._fsp--;
				if (state.failed) return ;

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
	// $ANTLR end "rewrite_element"


	// $ANTLR start "rewrite_ebnf"
	// Grammars\\DefineGrammarItemsWalker.g3:594:0: rewrite_ebnf : ( ^( OPTIONAL rewrite_block ) | ^( CLOSURE rewrite_block ) | ^( POSITIVE_CLOSURE rewrite_block ) );
	private void rewrite_ebnf(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:595:4: ( ^( OPTIONAL rewrite_block ) | ^( CLOSURE rewrite_block ) | ^( POSITIVE_CLOSURE rewrite_block ) )
			int alt49=3;
			switch ( input.LA(1) )
			{
			case OPTIONAL:
				{
				alt49=1;
				}
				break;
			case CLOSURE:
				{
				alt49=2;
				}
				break;
			case POSITIVE_CLOSURE:
				{
				alt49=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 49, 0, input);

					throw nvae;
				}
			}

			switch ( alt49 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:595:4: ^( OPTIONAL rewrite_block )
				{
				Match(input,OPTIONAL,Follow._OPTIONAL_in_rewrite_ebnf1611); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1613);
				rewrite_block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:596:4: ^( CLOSURE rewrite_block )
				{
				Match(input,CLOSURE,Follow._CLOSURE_in_rewrite_ebnf1622); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1624);
				rewrite_block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:597:4: ^( POSITIVE_CLOSURE rewrite_block )
				{
				Match(input,POSITIVE_CLOSURE,Follow._POSITIVE_CLOSURE_in_rewrite_ebnf1633); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				PushFollow(Follow._rewrite_block_in_rewrite_ebnf1635);
				rewrite_block();

				state._fsp--;
				if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "rewrite_ebnf"


	// $ANTLR start "rewrite_tree"
	// Grammars\\DefineGrammarItemsWalker.g3:600:0: rewrite_tree : ^( TREE_BEGIN rewrite_atom ( rewrite_element )* ) ;
	private void rewrite_tree(  )
	{
		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:601:6: ( ^( TREE_BEGIN rewrite_atom ( rewrite_element )* ) )
			// Grammars\\DefineGrammarItemsWalker.g3:601:6: ^( TREE_BEGIN rewrite_atom ( rewrite_element )* )
			{
			Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_rewrite_tree1652); if (state.failed) return ;

			Match(input, TokenTypes.Down, null); if (state.failed) return ;
			PushFollow(Follow._rewrite_atom_in_rewrite_tree1654);
			rewrite_atom();

			state._fsp--;
			if (state.failed) return ;
			// Grammars\\DefineGrammarItemsWalker.g3:601:33: ( rewrite_element )*
			for ( ; ; )
			{
				int alt50=2;
				int LA50_0 = input.LA(1);

				if ( (LA50_0==ACTION||LA50_0==CHAR_LITERAL||LA50_0==CLOSURE||LA50_0==LABEL||LA50_0==OPTIONAL||LA50_0==POSITIVE_CLOSURE||LA50_0==RULE_REF||LA50_0==STRING_LITERAL||LA50_0==TOKEN_REF||LA50_0==TREE_BEGIN) )
				{
					alt50=1;
				}


				switch ( alt50 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:601:35: rewrite_element
					{
					PushFollow(Follow._rewrite_element_in_rewrite_tree1658);
					rewrite_element();

					state._fsp--;
					if (state.failed) return ;

					}
					break;

				default:
					goto loop50;
				}
			}

			loop50:
				;



			Match(input, TokenTypes.Up, null); if (state.failed) return ;

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
	// $ANTLR end "rewrite_tree"

	public class rewrite_atom_return : TreeRuleReturnScope
	{
	}

	// $ANTLR start "rewrite_atom"
	// Grammars\\DefineGrammarItemsWalker.g3:604:0: rewrite_atom : ( RULE_REF | ( ^( TOKEN_REF ( ARG_ACTION )? ) | CHAR_LITERAL | STRING_LITERAL ) | LABEL | ACTION );
	private DefineGrammarItemsWalker.rewrite_atom_return rewrite_atom(  )
	{
		DefineGrammarItemsWalker.rewrite_atom_return retval = new DefineGrammarItemsWalker.rewrite_atom_return();
		retval.start = input.LT(1);

		GrammarAST ARG_ACTION12=null;
		GrammarAST ACTION13=null;


			if ( state.backtracking == 0 )
			{
				Rule r = grammar.GetRule(currentRuleName);
				var tokenRefsInAlt = r.GetTokenRefsInAlt(outerAltNum);
				bool imaginary =
					((GrammarAST)retval.Start).Type==TOKEN_REF &&
					!tokenRefsInAlt.Contains(((GrammarAST)retval.Start).Text);
				if ( !imaginary && grammar.BuildAST &&
					 (((GrammarAST)retval.Start).Type==RULE_REF ||
					  ((GrammarAST)retval.Start).Type==LABEL ||
					  ((GrammarAST)retval.Start).Type==TOKEN_REF ||
					  ((GrammarAST)retval.Start).Type==CHAR_LITERAL ||
					  ((GrammarAST)retval.Start).Type==STRING_LITERAL) )
				{
					// track per block and for entire rewrite rule
					if ( currentRewriteBlock!=null )
					{
						currentRewriteBlock.rewriteRefsShallow.Add(((GrammarAST)retval.Start));
						currentRewriteBlock.rewriteRefsDeep.Add(((GrammarAST)retval.Start));
					}
					currentRewriteRule.rewriteRefsDeep.Add(((GrammarAST)retval.Start));
				}
			}

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:631:4: ( RULE_REF | ( ^( TOKEN_REF ( ARG_ACTION )? ) | CHAR_LITERAL | STRING_LITERAL ) | LABEL | ACTION )
			int alt53=4;
			switch ( input.LA(1) )
			{
			case RULE_REF:
				{
				alt53=1;
				}
				break;
			case CHAR_LITERAL:
			case STRING_LITERAL:
			case TOKEN_REF:
				{
				alt53=2;
				}
				break;
			case LABEL:
				{
				alt53=3;
				}
				break;
			case ACTION:
				{
				alt53=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 53, 0, input);

					throw nvae;
				}
			}

			switch ( alt53 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:631:4: RULE_REF
				{
				Match(input,RULE_REF,Follow._RULE_REF_in_rewrite_atom1679); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:632:4: ( ^( TOKEN_REF ( ARG_ACTION )? ) | CHAR_LITERAL | STRING_LITERAL )
				{
				// Grammars\\DefineGrammarItemsWalker.g3:632:4: ( ^( TOKEN_REF ( ARG_ACTION )? ) | CHAR_LITERAL | STRING_LITERAL )
				int alt52=3;
				switch ( input.LA(1) )
				{
				case TOKEN_REF:
					{
					alt52=1;
					}
					break;
				case CHAR_LITERAL:
					{
					alt52=2;
					}
					break;
				case STRING_LITERAL:
					{
					alt52=3;
					}
					break;
				default:
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 52, 0, input);

						throw nvae;
					}
				}

				switch ( alt52 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:632:6: ^( TOKEN_REF ( ARG_ACTION )? )
					{
					Match(input,TOKEN_REF,Follow._TOKEN_REF_in_rewrite_atom1689); if (state.failed) return retval;

					if ( input.LA(1)==TokenTypes.Down )
					{
						Match(input, TokenTypes.Down, null); if (state.failed) return retval;
						// Grammars\\DefineGrammarItemsWalker.g3:633:5: ( ARG_ACTION )?
						int alt51=2;
						int LA51_0 = input.LA(1);

						if ( (LA51_0==ARG_ACTION) )
						{
							alt51=1;
						}
						switch ( alt51 )
						{
						case 1:
							// Grammars\\DefineGrammarItemsWalker.g3:633:7: ARG_ACTION
							{
							ARG_ACTION12=(GrammarAST)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rewrite_atom1697); if (state.failed) return retval;
							if ( state.backtracking == 0 )
							{

														ARG_ACTION12.outerAltNum = this.outerAltNum;
														TrackInlineAction(ARG_ACTION12);
													
							}

							}
							break;

						}


						Match(input, TokenTypes.Up, null); if (state.failed) return retval;
					}

					}
					break;
				case 2:
					// Grammars\\DefineGrammarItemsWalker.g3:640:5: CHAR_LITERAL
					{
					Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_rewrite_atom1722); if (state.failed) return retval;

					}
					break;
				case 3:
					// Grammars\\DefineGrammarItemsWalker.g3:641:5: STRING_LITERAL
					{
					Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_rewrite_atom1728); if (state.failed) return retval;

					}
					break;

				}


				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:643:4: LABEL
				{
				Match(input,LABEL,Follow._LABEL_in_rewrite_atom1737); if (state.failed) return retval;

				}
				break;
			case 4:
				// Grammars\\DefineGrammarItemsWalker.g3:644:4: ACTION
				{
				ACTION13=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_atom1742); if (state.failed) return retval;
				if ( state.backtracking == 0 )
				{

								ACTION13.outerAltNum = this.outerAltNum;
								TrackInlineAction(ACTION13);
							
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
		return retval;
	}
	// $ANTLR end "rewrite_atom"


	// $ANTLR start "rewrite_template"
	// Grammars\\DefineGrammarItemsWalker.g3:651:0: rewrite_template : ( ^( ALT EPSILON EOA ) | ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) |act= ACTION );
	private void rewrite_template(  )
	{
		GrammarAST id=null;
		GrammarAST ind=null;
		GrammarAST arg=null;
		GrammarAST a=null;
		GrammarAST act=null;

		try
		{
			// Grammars\\DefineGrammarItemsWalker.g3:652:4: ( ^( ALT EPSILON EOA ) | ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? ) |act= ACTION )
			int alt57=3;
			switch ( input.LA(1) )
			{
			case ALT:
				{
				alt57=1;
				}
				break;
			case TEMPLATE:
				{
				alt57=2;
				}
				break;
			case ACTION:
				{
				alt57=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 57, 0, input);

					throw nvae;
				}
			}

			switch ( alt57 )
			{
			case 1:
				// Grammars\\DefineGrammarItemsWalker.g3:652:4: ^( ALT EPSILON EOA )
				{
				Match(input,ALT,Follow._ALT_in_rewrite_template1759); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				Match(input,EPSILON,Follow._EPSILON_in_rewrite_template1761); if (state.failed) return ;
				Match(input,EOA,Follow._EOA_in_rewrite_template1763); if (state.failed) return ;

				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 2:
				// Grammars\\DefineGrammarItemsWalker.g3:653:4: ^( TEMPLATE (id= ID |ind= ACTION ) ^( ARGLIST ( ^( ARG arg= ID a= ACTION ) )* ) ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )? )
				{
				Match(input,TEMPLATE,Follow._TEMPLATE_in_rewrite_template1772); if (state.failed) return ;

				Match(input, TokenTypes.Down, null); if (state.failed) return ;
				// Grammars\\DefineGrammarItemsWalker.g3:653:16: (id= ID |ind= ACTION )
				int alt54=2;
				int LA54_0 = input.LA(1);

				if ( (LA54_0==ID) )
				{
					alt54=1;
				}
				else if ( (LA54_0==ACTION) )
				{
					alt54=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return ;}
					NoViableAltException nvae = new NoViableAltException("", 54, 0, input);

					throw nvae;
				}
				switch ( alt54 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:653:17: id= ID
					{
					id=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template1777); if (state.failed) return ;

					}
					break;
				case 2:
					// Grammars\\DefineGrammarItemsWalker.g3:653:23: ind= ACTION
					{
					ind=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template1781); if (state.failed) return ;

					}
					break;

				}

				Match(input,ARGLIST,Follow._ARGLIST_in_rewrite_template1789); if (state.failed) return ;

				if ( input.LA(1)==TokenTypes.Down )
				{
					Match(input, TokenTypes.Down, null); if (state.failed) return ;
					// Grammars\\DefineGrammarItemsWalker.g3:655:5: ( ^( ARG arg= ID a= ACTION ) )*
					for ( ; ; )
					{
						int alt55=2;
						int LA55_0 = input.LA(1);

						if ( (LA55_0==ARG) )
						{
							alt55=1;
						}


						switch ( alt55 )
						{
						case 1:
							// Grammars\\DefineGrammarItemsWalker.g3:655:7: ^( ARG arg= ID a= ACTION )
							{
							Match(input,ARG,Follow._ARG_in_rewrite_template1799); if (state.failed) return ;

							Match(input, TokenTypes.Down, null); if (state.failed) return ;
							arg=(GrammarAST)Match(input,ID,Follow._ID_in_rewrite_template1803); if (state.failed) return ;
							a=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template1807); if (state.failed) return ;

							Match(input, TokenTypes.Up, null); if (state.failed) return ;
							if ( state.backtracking == 0 )
							{

														a.outerAltNum = this.outerAltNum;
														TrackInlineAction(a);
													
							}

							}
							break;

						default:
							goto loop55;
						}
					}

					loop55:
						;



					Match(input, TokenTypes.Up, null); if (state.failed) return ;
				}
				if ( state.backtracking == 0 )
				{

									if ( ind!=null )
									{
										ind.outerAltNum = this.outerAltNum;
										TrackInlineAction(ind);
									}
								
				}
				// Grammars\\DefineGrammarItemsWalker.g3:669:4: ( DOUBLE_QUOTE_STRING_LITERAL | DOUBLE_ANGLE_STRING_LITERAL )?
				int alt56=2;
				int LA56_0 = input.LA(1);

				if ( ((LA56_0>=DOUBLE_ANGLE_STRING_LITERAL && LA56_0<=DOUBLE_QUOTE_STRING_LITERAL)) )
				{
					alt56=1;
				}
				switch ( alt56 )
				{
				case 1:
					// Grammars\\DefineGrammarItemsWalker.g3:
					{
					if ( (input.LA(1)>=DOUBLE_ANGLE_STRING_LITERAL && input.LA(1)<=DOUBLE_QUOTE_STRING_LITERAL) )
					{
						input.Consume();
						state.errorRecovery=false;state.failed=false;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return ;}
						MismatchedSetException mse = new MismatchedSetException(null,input);
						throw mse;
					}


					}
					break;

				}


				Match(input, TokenTypes.Up, null); if (state.failed) return ;

				}
				break;
			case 3:
				// Grammars\\DefineGrammarItemsWalker.g3:673:4: act= ACTION
				{
				act=(GrammarAST)Match(input,ACTION,Follow._ACTION_in_rewrite_template1864); if (state.failed) return ;
				if ( state.backtracking == 0 )
				{

								act.outerAltNum = this.outerAltNum;
								TrackInlineAction(act);
							
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
		return ;
	}
	// $ANTLR end "rewrite_template"

	// $ANTLR start synpred1_DefineGrammarItemsWalker
	public void synpred1_DefineGrammarItemsWalker_fragment()
	{
		// Grammars\\DefineGrammarItemsWalker.g3:445:4: ( dotLoop )
		// Grammars\\DefineGrammarItemsWalker.g3:445:5: dotLoop
		{
		PushFollow(Follow._dotLoop_in_synpred1_DefineGrammarItemsWalker1201);
		dotLoop();

		state._fsp--;
		if (state.failed) return ;

		}
	}
	// $ANTLR end synpred1_DefineGrammarItemsWalker
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
	DFA36 dfa36;
	DFA37 dfa37;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa36 = new DFA36( this );
		dfa37 = new DFA37( this, new SpecialStateTransitionHandler( specialStateTransition37 ) );
	}

	class DFA36 : DFA
	{

		const string DFA36_eotS =
			"\x13\xFFFF";
		const string DFA36_eofS =
			"\x13\xFFFF";
		const string DFA36_minS =
			"\x1\x4\x12\xFFFF";
		const string DFA36_maxS =
			"\x1\x5F\x12\xFFFF";
		const string DFA36_acceptS =
			"\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9\x1\xA"+
			"\x1\xB\x1\xC\x1\xD\x1\xE\x1\xF\x1\x10\x1\x11\x1\x12";
		const string DFA36_specialS =
			"\x13\xFFFF}>";
		static readonly string[] DFA36_transitionS =
			{
				"\x1\xC\x8\xFFFF\x1\x7\x1\x10\x1\x2\x1\x9\x1\xFFFF\x1\x3\x1\x6\x1\xFFFF"+
				"\x1\x9\x7\xFFFF\x1\x3\x5\xFFFF\x1\x12\x3\xFFFF\x1\xD\x1\xFFFF\x1\x11"+
				"\xD\xFFFF\x1\x4\x1\xFFFF\x1\x9\x5\xFFFF\x1\x8\x1\x9\x4\xFFFF\x1\x5\x4"+
				"\xFFFF\x1\x1\x2\xFFFF\x1\x3\x2\xFFFF\x1\xE\x4\xFFFF\x1\x3\x1\xF\x1\xB"+
				"\x2\xFFFF\x1\x3\x2\xFFFF\x1\xA\x1\xFFFF\x1\x3",
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
				""
			};

		static readonly short[] DFA36_eot = DFA.UnpackEncodedString(DFA36_eotS);
		static readonly short[] DFA36_eof = DFA.UnpackEncodedString(DFA36_eofS);
		static readonly char[] DFA36_min = DFA.UnpackEncodedStringToUnsignedChars(DFA36_minS);
		static readonly char[] DFA36_max = DFA.UnpackEncodedStringToUnsignedChars(DFA36_maxS);
		static readonly short[] DFA36_accept = DFA.UnpackEncodedString(DFA36_acceptS);
		static readonly short[] DFA36_special = DFA.UnpackEncodedString(DFA36_specialS);
		static readonly short[][] DFA36_transition;

		static DFA36()
		{
			int numStates = DFA36_transitionS.Length;
			DFA36_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA36_transition[i] = DFA.UnpackEncodedString(DFA36_transitionS[i]);
			}
		}

		public DFA36( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 36;
			this.eot = DFA36_eot;
			this.eof = DFA36_eof;
			this.min = DFA36_min;
			this.max = DFA36_max;
			this.accept = DFA36_accept;
			this.special = DFA36_special;
			this.transition = DFA36_transition;
		}
		public override string GetDescription()
		{
			return "368:0: element : ( ^( ROOT element ) | ^( BANG element ) | atom[null] | ^( NOT element ) | ^( RANGE atom[null] atom[null] ) | ^( CHAR_RANGE atom[null] atom[null] ) | ^( ASSIGN id= ID el= element ) | ^( PLUS_ASSIGN id2= ID a2= element ) | ebnf | tree_ | ^( SYNPRED block ) |act= ACTION |act2= FORCED_ACTION | SEMPRED | SYN_SEMPRED | ^( BACKTRACK_SEMPRED ( . )* ) | GATED_SEMPRED | EPSILON );";
		}
	}

	class DFA37 : DFA
	{

		const string DFA37_eotS =
			"\x1E\xFFFF";
		const string DFA37_eofS =
			"\x1E\xFFFF";
		const string DFA37_minS =
			"\x1\x10\x2\x2\x2\xFFFF\x2\x10\x2\x2\x2\x8\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF"+
			"\x4\x4\x2\x3\x2\x8\x4\x3\x2\x0\x1\xFFFF";
		const string DFA37_maxS =
			"\x1\x40\x2\x2\x2\xFFFF\x2\x10\x2\x2\x2\x3A\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF"+
			"\x4\x5F\x2\x3\x2\x49\x4\x3\x2\x0\x1\xFFFF";
		const string DFA37_acceptS =
			"\x3\xFFFF\x1\x2\x1\x3\x7\xFFFF\x1\x4\x1\xFFFF\x1\x5\xE\xFFFF\x1\x1";
		const string DFA37_specialS =
			"\x1B\xFFFF\x1\x0\x1\x1\x1\xFFFF}>";
		static readonly string[] DFA37_transitionS =
			{
				"\x1\x3\x4\xFFFF\x1\x1\x23\xFFFF\x1\x4\x6\xFFFF\x1\x2",
				"\x1\x5",
				"\x1\x6",
				"",
				"",
				"\x1\x7",
				"\x1\x8",
				"\x1\x9",
				"\x1\xA",
				"\x1\xB\x1\xC\x30\xFFFF\x1\xC",
				"\x1\xD\x1\xE\x30\xFFFF\x1\xE",
				"\x1\xF",
				"",
				"\x1\x10",
				"",
				"\x1\xC\x8\xFFFF\x4\xC\x1\xFFFF\x2\xC\x1\xFFFF\x1\xC\x7\xFFFF\x1\xC\x5"+
				"\xFFFF\x1\xC\x3\xFFFF\x1\xC\x1\xFFFF\x1\xC\xD\xFFFF\x1\xC\x1\xFFFF\x1"+
				"\xC\x5\xFFFF\x2\xC\x4\xFFFF\x1\xC\x4\xFFFF\x1\xC\x2\xFFFF\x1\xC\x2\xFFFF"+
				"\x1\xC\x4\xFFFF\x3\xC\x2\xFFFF\x1\xC\x2\xFFFF\x1\xC\x1\xFFFF\x1\x11",
				"\x1\xE\x8\xFFFF\x4\xE\x1\xFFFF\x2\xE\x1\xFFFF\x1\xE\x7\xFFFF\x1\xE\x5"+
				"\xFFFF\x1\xE\x3\xFFFF\x1\xE\x1\xFFFF\x1\xE\xD\xFFFF\x1\xE\x1\xFFFF\x1"+
				"\xE\x5\xFFFF\x2\xE\x4\xFFFF\x1\xE\x4\xFFFF\x1\xE\x2\xFFFF\x1\xE\x2\xFFFF"+
				"\x1\xE\x4\xFFFF\x3\xE\x2\xFFFF\x1\xE\x2\xFFFF\x1\xE\x1\xFFFF\x1\x12",
				"\x1\xC\x8\xFFFF\x4\xC\x1\xFFFF\x2\xC\x1\xFFFF\x1\xC\x7\xFFFF\x1\xC\x2"+
				"\xFFFF\x1\x13\x2\xFFFF\x1\xC\x3\xFFFF\x1\xC\x1\xFFFF\x1\xC\xD\xFFFF"+
				"\x1\xC\x1\xFFFF\x1\xC\x5\xFFFF\x2\xC\x4\xFFFF\x1\xC\x4\xFFFF\x1\xC\x2"+
				"\xFFFF\x1\xC\x2\xFFFF\x1\xC\x4\xFFFF\x3\xC\x2\xFFFF\x1\xC\x2\xFFFF\x1"+
				"\xC\x1\xFFFF\x1\xC",
				"\x1\xE\x8\xFFFF\x4\xE\x1\xFFFF\x2\xE\x1\xFFFF\x1\xE\x7\xFFFF\x1\xE\x2"+
				"\xFFFF\x1\x14\x2\xFFFF\x1\xE\x3\xFFFF\x1\xE\x1\xFFFF\x1\xE\xD\xFFFF"+
				"\x1\xE\x1\xFFFF\x1\xE\x5\xFFFF\x2\xE\x4\xFFFF\x1\xE\x4\xFFFF\x1\xE\x2"+
				"\xFFFF\x1\xE\x2\xFFFF\x1\xE\x4\xFFFF\x3\xE\x2\xFFFF\x1\xE\x2\xFFFF\x1"+
				"\xE\x1\xFFFF\x1\xE",
				"\x1\x15",
				"\x1\x16",
				"\x1\xC\x18\xFFFF\x1\x17\x27\xFFFF\x1\xC",
				"\x1\xE\x18\xFFFF\x1\x18\x27\xFFFF\x1\xE",
				"\x1\x19",
				"\x1\x1A",
				"\x1\x1B",
				"\x1\x1C",
				"\x1\xFFFF",
				"\x1\xFFFF",
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
			return "444:0: ebnf : (=> dotLoop | block | ^( OPTIONAL block ) | ^( CLOSURE block ) | ^( POSITIVE_CLOSURE block ) );";
		}
	}

	int specialStateTransition37( DFA dfa, int s, IIntStream _input )
	{
		ITreeNodeStream input = (ITreeNodeStream)_input;
		int _s = s;
		switch ( s )
		{
			case 0:
				int LA37_27 = input.LA(1);


				int index37_27 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_DefineGrammarItemsWalker_fragment)) ) {s = 29;}

				else if ( (true) ) {s = 12;}


				input.Seek(index37_27);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA37_28 = input.LA(1);


				int index37_28 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred1_DefineGrammarItemsWalker_fragment)) ) {s = 29;}

				else if ( (true) ) {s = 14;}


				input.Seek(index37_28);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 37, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}

	#endregion DFA

	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _LEXER_GRAMMAR_in_grammar_77 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_83 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PARSER_GRAMMAR_in_grammar_92 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_97 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_GRAMMAR_in_grammar_106 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_111 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _COMBINED_GRAMMAR_in_grammar_120 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _grammarSpec_in_grammar_125 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _SCOPE_in_attrScope150 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScope154 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _attrScopeAction_in_attrScope156 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ACTION_in_attrScope161 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _AMPERSAND_in_attrScopeAction179 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_attrScopeAction181 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_attrScopeAction183 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_grammarSpec201 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammarSpec208 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _optionsSpec_in_grammarSpec216 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _delegateGrammars_in_grammarSpec224 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _tokensSpec_in_grammarSpec231 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _attrScope_in_grammarSpec238 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _actions_in_grammarSpec245 = new BitSet(new ulong[]{0x400200008000200UL,0x8005000UL});
		public static readonly BitSet _rules_in_grammarSpec251 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _action_in_actions264 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _AMPERSAND_in_action286 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_action290 = new BitSet(new ulong[]{0x80000000010UL});
		public static readonly BitSet _ID_in_action299 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_action303 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_action319 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _OPTIONS_in_optionsSpec353 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _IMPORT_in_delegateGrammars370 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ASSIGN_in_delegateGrammars375 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_delegateGrammars377 = new BitSet(new ulong[]{0x80000000000UL});
		public static readonly BitSet _ID_in_delegateGrammars379 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ID_in_delegateGrammars384 = new BitSet(new ulong[]{0x80000002008UL});
		public static readonly BitSet _TOKENS_in_tokensSpec402 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _tokenSpec_in_tokensSpec406 = new BitSet(new ulong[]{0x2008UL,0x4000000UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec424 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ASSIGN_in_tokenSpec431 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec436 = new BitSet(new ulong[]{0x40000UL,0x200000UL});
		public static readonly BitSet _set_in_tokenSpec441 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rule_in_rules471 = new BitSet(new ulong[]{0x400200008000202UL,0x8005000UL});
		public static readonly BitSet _RULE_in_rule491 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rule495 = new BitSet(new ulong[]{0x10000000400UL,0xEUL});
		public static readonly BitSet _modifier_in_rule503 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _ARG_in_rule512 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule517 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RET_in_rule528 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rule533 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _throwsSpec_in_rule543 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _optionsSpec_in_rule551 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _ruleScopeSpec_in_rule564 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _ruleAction_in_rule573 = new BitSet(new ulong[]{0x400000000010200UL,0x4000UL});
		public static readonly BitSet _block_in_rule588 = new BitSet(new ulong[]{0x4400020000UL});
		public static readonly BitSet _exceptionGroup_in_rule594 = new BitSet(new ulong[]{0x400000000UL});
		public static readonly BitSet _EOR_in_rule601 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _AMPERSAND_in_ruleAction625 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_ruleAction629 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_ruleAction633 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_modifier657 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec693 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_throwsSpec696 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec726 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _attrScopeAction_in_ruleScopeSpec733 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec738 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _ID_in_ruleScopeSpec759 = new BitSet(new ulong[]{0x80000000008UL});
		public static readonly BitSet _BLOCK_in_block793 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _optionsSpec_in_block799 = new BitSet(new ulong[]{0x300UL});
		public static readonly BitSet _blockAction_in_block807 = new BitSet(new ulong[]{0x300UL});
		public static readonly BitSet _alternative_in_block816 = new BitSet(new ulong[]{0x200000300UL,0x200UL});
		public static readonly BitSet _rewrite_in_block818 = new BitSet(new ulong[]{0x200000300UL});
		public static readonly BitSet _EOB_in_block835 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _AMPERSAND_in_blockAction859 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_blockAction863 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_blockAction867 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_alternative888 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_alternative891 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _EOA_in_alternative895 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup910 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup916 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup923 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler937 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler939 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler941 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _FINALLY_in_finallyClause959 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ACTION_in_finallyClause961 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ROOT_in_element978 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element980 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BANG_in_element989 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element991 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _atom_in_element999 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_element1008 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_element1010 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _RANGE_in_element1019 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element1021 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element1024 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_RANGE_in_element1034 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _atom_in_element1036 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_element1039 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ASSIGN_in_element1048 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element1052 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element1056 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _PLUS_ASSIGN_in_element1069 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_element1073 = new BitSet(new ulong[]{0x86800289202DE210UL,0xA4E16421UL});
		public static readonly BitSet _element_in_element1077 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ebnf_in_element1094 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_element1101 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYNPRED_in_element1110 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_element1112 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_element1123 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FORCED_ACTION_in_element1136 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_element1147 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SYN_SEMPRED_in_element1158 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BACKTRACK_SEMPRED_in_element1166 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _GATED_SEMPRED_in_element1177 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EPSILON_in_element1188 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _dotLoop_in_ebnf1206 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_ebnf1212 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_ebnf1219 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1221 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_ebnf1230 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1232 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_ebnf1241 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _block_in_ebnf1243 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_dotLoop1262 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _dotBlock_in_dotLoop1264 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_dotLoop1274 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _dotBlock_in_dotLoop1276 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BLOCK_in_dotBlock1299 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ALT_in_dotBlock1303 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _WILDCARD_in_dotBlock1305 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_dotBlock1307 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _EOB_in_dotBlock1311 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_1325 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _element_in_tree_1327 = new BitSet(new ulong[]{0x86800289202DE218UL,0xA4E16421UL});
		public static readonly BitSet _RULE_REF_in_atom1345 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1350 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TOKEN_REF_in_atom1367 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_atom1372 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_atom1388 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_atom1399 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _WILDCARD_in_atom1409 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_atom1415 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_atom1417 = new BitSet(new ulong[]{0x20040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_atom1419 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_ast_suffix1433 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _REWRITE_in_rewrite1461 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _SEMPRED_in_rewrite1466 = new BitSet(new ulong[]{0x2000000110UL,0x1000000UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite1470 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _BLOCK_in_rewrite_block1505 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite_block1507 = new BitSet(new ulong[]{0x200000000UL});
		public static readonly BitSet _EOB_in_rewrite_block1509 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ALT_in_rewrite_alternative1541 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_element_in_rewrite_alternative1547 = new BitSet(new ulong[]{0x201000100240010UL,0x24202001UL});
		public static readonly BitSet _EPSILON_in_rewrite_alternative1554 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_rewrite_alternative1558 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _rewrite_template_in_rewrite_alternative1569 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ETC_in_rewrite_alternative1574 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_element1588 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_ebnf_in_rewrite_element1593 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_tree_in_rewrite_element1598 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONAL_in_rewrite_ebnf1611 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1613 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CLOSURE_in_rewrite_ebnf1622 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1624 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _POSITIVE_CLOSURE_in_rewrite_ebnf1633 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf1635 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TREE_BEGIN_in_rewrite_tree1652 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_tree1654 = new BitSet(new ulong[]{0x201000100240018UL,0x24202001UL});
		public static readonly BitSet _rewrite_element_in_rewrite_tree1658 = new BitSet(new ulong[]{0x201000100240018UL,0x24202001UL});
		public static readonly BitSet _RULE_REF_in_rewrite_atom1679 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_rewrite_atom1689 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_ACTION_in_rewrite_atom1697 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _CHAR_LITERAL_in_rewrite_atom1722 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_rewrite_atom1728 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LABEL_in_rewrite_atom1737 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_rewrite_atom1742 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ALT_in_rewrite_template1759 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _EPSILON_in_rewrite_template1761 = new BitSet(new ulong[]{0x100000000UL});
		public static readonly BitSet _EOA_in_rewrite_template1763 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _TEMPLATE_in_rewrite_template1772 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template1777 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ACTION_in_rewrite_template1781 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _ARGLIST_in_rewrite_template1789 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ARG_in_rewrite_template1799 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _ID_in_rewrite_template1803 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_rewrite_template1807 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _set_in_rewrite_template1838 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ACTION_in_rewrite_template1864 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _dotLoop_in_synpred1_DefineGrammarItemsWalker1201 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
