// $ANTLR 3.1.2 Grammars\\ANTLR.g3 2009-03-20 14:32:46

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

using ErrorManager = Antlr3.Tool.ErrorManager;
using Grammar = Antlr3.Tool.Grammar;
using GrammarAST = Antlr3.Tool.GrammarAST;
using IntSet = Antlr3.Misc.IIntSet;
using StringBuffer = System.Text.StringBuilder;
using TokenWithIndex = Antlr.Runtime.CommonToken;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;

using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;

namespace Antlr3.Grammars
{
/** Read in an ANTLR grammar and build an AST.  Try not to do
 *  any actions, just build the tree.
 *
 *  The phases are:
 *
 *		antlr.g (this file)
 *		assign.types.g
 *		define.g
 *		buildnfa.g
 *		antlr.print.g (optional)
 *		codegen.g
 *
 *  Terence Parr
 *  University of San Francisco
 *  2005
 */
public partial class ANTLRParser : Parser
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

	public ANTLRParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public ANTLRParser( ITokenStream input, RecognizerSharedState state )
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

	public override string[] GetTokenNames() { return ANTLRParser.tokenNames; }
	public override string GrammarFileName { get { return "Grammars\\ANTLR.g3"; } }


	#region Rules
	public class grammar__return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "grammar_"
	// Grammars\\ANTLR.g3:193:0: public grammar_[Grammar g] : ( ACTION )? (cmt= DOC_COMMENT )? gr= grammarType gid= id SEMI ( optionsSpec )? (ig= delegateGrammars )? (ts= tokensSpec )? scopes= attrScopes (a= actions )? r= rules EOF -> ^( $gr $gid ( $cmt)? ( optionsSpec )? ( $ig)? ( $ts)? ( $scopes)? ( $a)? $r) ;
	public ANTLRParser.grammar__return grammar_( Grammar g )
	{
		ANTLRParser.grammar__return retval = new ANTLRParser.grammar__return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken cmt=null;
		IToken ACTION1=null;
		IToken SEMI2=null;
		IToken EOF4=null;
		ANTLRParser.grammarType_return gr = default(ANTLRParser.grammarType_return);
		ANTLRParser.id_return gid = default(ANTLRParser.id_return);
		ANTLRParser.delegateGrammars_return ig = default(ANTLRParser.delegateGrammars_return);
		ANTLRParser.tokensSpec_return ts = default(ANTLRParser.tokensSpec_return);
		ANTLRParser.attrScopes_return scopes = default(ANTLRParser.attrScopes_return);
		ANTLRParser.actions_return a = default(ANTLRParser.actions_return);
		ANTLRParser.rules_return r = default(ANTLRParser.rules_return);
		ANTLRParser.optionsSpec_return optionsSpec3 = default(ANTLRParser.optionsSpec_return);

		GrammarAST cmt_tree=null;
		GrammarAST ACTION1_tree=null;
		GrammarAST SEMI2_tree=null;
		GrammarAST EOF4_tree=null;
		RewriteRuleITokenStream stream_ACTION=new RewriteRuleITokenStream(adaptor,"token ACTION");
		RewriteRuleITokenStream stream_DOC_COMMENT=new RewriteRuleITokenStream(adaptor,"token DOC_COMMENT");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleITokenStream stream_EOF=new RewriteRuleITokenStream(adaptor,"token EOF");
		RewriteRuleSubtreeStream stream_grammarType=new RewriteRuleSubtreeStream(adaptor,"rule grammarType");
		RewriteRuleSubtreeStream stream_id=new RewriteRuleSubtreeStream(adaptor,"rule id");
		RewriteRuleSubtreeStream stream_optionsSpec=new RewriteRuleSubtreeStream(adaptor,"rule optionsSpec");
		RewriteRuleSubtreeStream stream_delegateGrammars=new RewriteRuleSubtreeStream(adaptor,"rule delegateGrammars");
		RewriteRuleSubtreeStream stream_tokensSpec=new RewriteRuleSubtreeStream(adaptor,"rule tokensSpec");
		RewriteRuleSubtreeStream stream_attrScopes=new RewriteRuleSubtreeStream(adaptor,"rule attrScopes");
		RewriteRuleSubtreeStream stream_actions=new RewriteRuleSubtreeStream(adaptor,"rule actions");
		RewriteRuleSubtreeStream stream_rules=new RewriteRuleSubtreeStream(adaptor,"rule rules");

			this.Grammar = g;
			IDictionary<string, object> opts;

		try
		{
			// Grammars\\ANTLR.g3:204:3: ( ( ACTION )? (cmt= DOC_COMMENT )? gr= grammarType gid= id SEMI ( optionsSpec )? (ig= delegateGrammars )? (ts= tokensSpec )? scopes= attrScopes (a= actions )? r= rules EOF -> ^( $gr $gid ( $cmt)? ( optionsSpec )? ( $ig)? ( $ts)? ( $scopes)? ( $a)? $r) )
			// Grammars\\ANTLR.g3:204:3: ( ACTION )? (cmt= DOC_COMMENT )? gr= grammarType gid= id SEMI ( optionsSpec )? (ig= delegateGrammars )? (ts= tokensSpec )? scopes= attrScopes (a= actions )? r= rules EOF
			{
			// Grammars\\ANTLR.g3:204:3: ( ACTION )?
			int alt1=2;
			int LA1_0 = input.LA(1);

			if ( (LA1_0==ACTION) )
			{
				alt1=1;
			}
			switch ( alt1 )
			{
			case 1:
				// Grammars\\ANTLR.g3:204:5: ACTION
				{
				ACTION1=(IToken)Match(input,ACTION,Follow._ACTION_in_grammar_308); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ACTION.Add(ACTION1);


				}
				break;

			}

			// Grammars\\ANTLR.g3:205:3: (cmt= DOC_COMMENT )?
			int alt2=2;
			int LA2_0 = input.LA(1);

			if ( (LA2_0==DOC_COMMENT) )
			{
				alt2=1;
			}
			switch ( alt2 )
			{
			case 1:
				// Grammars\\ANTLR.g3:205:5: cmt= DOC_COMMENT
				{
				cmt=(IToken)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_grammar_319); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_DOC_COMMENT.Add(cmt);


				}
				break;

			}

			PushFollow(Follow._grammarType_in_grammar_329);
			gr=grammarType();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_grammarType.Add(gr.Tree);
			PushFollow(Follow._id_in_grammar_333);
			gid=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_id.Add(gid.Tree);
			if ( state.backtracking == 0 )
			{
				Grammar.setName((gid!=null?input.ToString(gid.start,gid.stop):null));
			}
			SEMI2=(IToken)Match(input,SEMI,Follow._SEMI_in_grammar_337); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI2);

			// Grammars\\ANTLR.g3:207:3: ( optionsSpec )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==OPTIONS) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Grammars\\ANTLR.g3:207:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_grammar_343);
				optionsSpec3=optionsSpec();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_optionsSpec.Add(optionsSpec3.Tree);
				if ( state.backtracking == 0 )
				{
					opts = (optionsSpec3!=null?optionsSpec3.opts:default(IDictionary<string, object>)); Grammar.setOptions(opts, (optionsSpec3!=null?((IToken)optionsSpec3.start):null));
				}

				}
				break;

			}

			// Grammars\\ANTLR.g3:209:3: (ig= delegateGrammars )?
			int alt4=2;
			int LA4_0 = input.LA(1);

			if ( (LA4_0==IMPORT) )
			{
				alt4=1;
			}
			switch ( alt4 )
			{
			case 1:
				// Grammars\\ANTLR.g3:209:4: ig= delegateGrammars
				{
				PushFollow(Follow._delegateGrammars_in_grammar_357);
				ig=delegateGrammars();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_delegateGrammars.Add(ig.Tree);

				}
				break;

			}

			// Grammars\\ANTLR.g3:210:3: (ts= tokensSpec )?
			int alt5=2;
			int LA5_0 = input.LA(1);

			if ( (LA5_0==TOKENS) )
			{
				alt5=1;
			}
			switch ( alt5 )
			{
			case 1:
				// Grammars\\ANTLR.g3:210:4: ts= tokensSpec
				{
				PushFollow(Follow._tokensSpec_in_grammar_366);
				ts=tokensSpec();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_tokensSpec.Add(ts.Tree);

				}
				break;

			}

			PushFollow(Follow._attrScopes_in_grammar_374);
			scopes=attrScopes();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_attrScopes.Add(scopes.Tree);
			// Grammars\\ANTLR.g3:212:3: (a= actions )?
			int alt6=2;
			int LA6_0 = input.LA(1);

			if ( (LA6_0==AMPERSAND) )
			{
				alt6=1;
			}
			switch ( alt6 )
			{
			case 1:
				// Grammars\\ANTLR.g3:212:4: a= actions
				{
				PushFollow(Follow._actions_in_grammar_381);
				a=actions();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_actions.Add(a.Tree);

				}
				break;

			}

			PushFollow(Follow._rules_in_grammar_389);
			r=rules();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rules.Add(r.Tree);
			EOF4=(IToken)Match(input,EOF,Follow._EOF_in_grammar_393); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_EOF.Add(EOF4);



			{
			// AST REWRITE
			// elements: gr, gid, cmt, optionsSpec, ig, ts, scopes, a, r
			// token labels: cmt
			// rule labels: gr, gid, ig, ts, scopes, a, r, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleITokenStream stream_cmt=new RewriteRuleITokenStream(adaptor,"token cmt",cmt);
			RewriteRuleSubtreeStream stream_gr=new RewriteRuleSubtreeStream(adaptor,"rule gr",gr!=null?gr.tree:null);
			RewriteRuleSubtreeStream stream_gid=new RewriteRuleSubtreeStream(adaptor,"rule gid",gid!=null?gid.tree:null);
			RewriteRuleSubtreeStream stream_ig=new RewriteRuleSubtreeStream(adaptor,"rule ig",ig!=null?ig.tree:null);
			RewriteRuleSubtreeStream stream_ts=new RewriteRuleSubtreeStream(adaptor,"rule ts",ts!=null?ts.tree:null);
			RewriteRuleSubtreeStream stream_scopes=new RewriteRuleSubtreeStream(adaptor,"rule scopes",scopes!=null?scopes.tree:null);
			RewriteRuleSubtreeStream stream_a=new RewriteRuleSubtreeStream(adaptor,"rule a",a!=null?a.tree:null);
			RewriteRuleSubtreeStream stream_r=new RewriteRuleSubtreeStream(adaptor,"rule r",r!=null?r.tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 215:3: -> ^( $gr $gid ( $cmt)? ( optionsSpec )? ( $ig)? ( $ts)? ( $scopes)? ( $a)? $r)
			{
				// Grammars\\ANTLR.g3:215:6: ^( $gr $gid ( $cmt)? ( optionsSpec )? ( $ig)? ( $ts)? ( $scopes)? ( $a)? $r)
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot(stream_gr.NextNode(), root_1);

				adaptor.AddChild(root_1, stream_gid.NextTree());
				// Grammars\\ANTLR.g3:215:18: ( $cmt)?
				if ( stream_cmt.HasNext )
				{
					adaptor.AddChild(root_1, stream_cmt.NextNode());

				}
				stream_cmt.Reset();
				// Grammars\\ANTLR.g3:215:23: ( optionsSpec )?
				if ( stream_optionsSpec.HasNext )
				{
					adaptor.AddChild(root_1, stream_optionsSpec.NextTree());

				}
				stream_optionsSpec.Reset();
				// Grammars\\ANTLR.g3:215:37: ( $ig)?
				if ( stream_ig.HasNext )
				{
					adaptor.AddChild(root_1, stream_ig.NextTree());

				}
				stream_ig.Reset();
				// Grammars\\ANTLR.g3:215:42: ( $ts)?
				if ( stream_ts.HasNext )
				{
					adaptor.AddChild(root_1, stream_ts.NextTree());

				}
				stream_ts.Reset();
				// Grammars\\ANTLR.g3:215:47: ( $scopes)?
				if ( stream_scopes.HasNext )
				{
					adaptor.AddChild(root_1, stream_scopes.NextTree());

				}
				stream_scopes.Reset();
				// Grammars\\ANTLR.g3:215:56: ( $a)?
				if ( stream_a.HasNext )
				{
					adaptor.AddChild(root_1, stream_a.NextTree());

				}
				stream_a.Reset();
				adaptor.AddChild(root_1, stream_r.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
			if ( state.backtracking == 0 )
			{

					cleanup( ((GrammarAST)retval.tree) );

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "grammar_"

	public class grammarType_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "grammarType"
	// Grammars\\ANTLR.g3:218:0: grammarType : ( 'lexer' gr= 'grammar' -> LEXER_GRAMMAR[$gr] | 'parser' gr= 'grammar' -> PARSER_GRAMMAR[$gr] | 'tree' gr= 'grammar' -> TREE_GRAMMAR[$gr] |gr= 'grammar' -> COMBINED_GRAMMAR[$gr] ) ;
	private ANTLRParser.grammarType_return grammarType(  )
	{
		ANTLRParser.grammarType_return retval = new ANTLRParser.grammarType_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken gr=null;
		IToken string_literal5=null;
		IToken string_literal6=null;
		IToken string_literal7=null;

		GrammarAST gr_tree=null;
		GrammarAST string_literal5_tree=null;
		GrammarAST string_literal6_tree=null;
		GrammarAST string_literal7_tree=null;
		RewriteRuleITokenStream stream_LEXER=new RewriteRuleITokenStream(adaptor,"token LEXER");
		RewriteRuleITokenStream stream_GRAMMAR=new RewriteRuleITokenStream(adaptor,"token GRAMMAR");
		RewriteRuleITokenStream stream_PARSER=new RewriteRuleITokenStream(adaptor,"token PARSER");
		RewriteRuleITokenStream stream_TREE=new RewriteRuleITokenStream(adaptor,"token TREE");

		try
		{
			// Grammars\\ANTLR.g3:219:4: ( ( 'lexer' gr= 'grammar' -> LEXER_GRAMMAR[$gr] | 'parser' gr= 'grammar' -> PARSER_GRAMMAR[$gr] | 'tree' gr= 'grammar' -> TREE_GRAMMAR[$gr] |gr= 'grammar' -> COMBINED_GRAMMAR[$gr] ) )
			// Grammars\\ANTLR.g3:219:4: ( 'lexer' gr= 'grammar' -> LEXER_GRAMMAR[$gr] | 'parser' gr= 'grammar' -> PARSER_GRAMMAR[$gr] | 'tree' gr= 'grammar' -> TREE_GRAMMAR[$gr] |gr= 'grammar' -> COMBINED_GRAMMAR[$gr] )
			{
			// Grammars\\ANTLR.g3:219:4: ( 'lexer' gr= 'grammar' -> LEXER_GRAMMAR[$gr] | 'parser' gr= 'grammar' -> PARSER_GRAMMAR[$gr] | 'tree' gr= 'grammar' -> TREE_GRAMMAR[$gr] |gr= 'grammar' -> COMBINED_GRAMMAR[$gr] )
			int alt7=4;
			switch ( input.LA(1) )
			{
			case LEXER:
				{
				alt7=1;
				}
				break;
			case PARSER:
				{
				alt7=2;
				}
				break;
			case TREE:
				{
				alt7=3;
				}
				break;
			case GRAMMAR:
				{
				alt7=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

					throw nvae;
				}
			}

			switch ( alt7 )
			{
			case 1:
				// Grammars\\ANTLR.g3:219:6: 'lexer' gr= 'grammar'
				{
				string_literal5=(IToken)Match(input,LEXER,Follow._LEXER_in_grammarType444); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LEXER.Add(string_literal5);

				gr=(IToken)Match(input,GRAMMAR,Follow._GRAMMAR_in_grammarType449); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GRAMMAR.Add(gr);

				if ( state.backtracking == 0 )
				{
					GrammarType=LEXER_GRAMMAR; Grammar.type = Grammar.LEXER;
				}


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 220:4: -> LEXER_GRAMMAR[$gr]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(LEXER_GRAMMAR, gr));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:221:5: 'parser' gr= 'grammar'
				{
				string_literal6=(IToken)Match(input,PARSER,Follow._PARSER_in_grammarType472); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PARSER.Add(string_literal6);

				gr=(IToken)Match(input,GRAMMAR,Follow._GRAMMAR_in_grammarType476); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GRAMMAR.Add(gr);

				if ( state.backtracking == 0 )
				{
					GrammarType=PARSER_GRAMMAR; Grammar.type = Grammar.PARSER;
				}


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 222:4: -> PARSER_GRAMMAR[$gr]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(PARSER_GRAMMAR, gr));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:223:5: 'tree' gr= 'grammar'
				{
				string_literal7=(IToken)Match(input,TREE,Follow._TREE_in_grammarType497); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TREE.Add(string_literal7);

				gr=(IToken)Match(input,GRAMMAR,Follow._GRAMMAR_in_grammarType503); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GRAMMAR.Add(gr);

				if ( state.backtracking == 0 )
				{
					GrammarType=TREE_GRAMMAR; Grammar.type = Grammar.TREE_PARSER;
				}


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 224:4: -> TREE_GRAMMAR[$gr]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(TREE_GRAMMAR, gr));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:225:8: gr= 'grammar'
				{
				gr=(IToken)Match(input,GRAMMAR,Follow._GRAMMAR_in_grammarType526); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GRAMMAR.Add(gr);

				if ( state.backtracking == 0 )
				{
					GrammarType=COMBINED_GRAMMAR; Grammar.type = Grammar.COMBINED;
				}


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 226:4: -> COMBINED_GRAMMAR[$gr]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(COMBINED_GRAMMAR, gr));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "grammarType"

	public class actions_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "actions"
	// Grammars\\ANTLR.g3:230:0: actions : ( action )+ ;
	private ANTLRParser.actions_return actions(  )
	{
		ANTLRParser.actions_return retval = new ANTLRParser.actions_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.action_return action8 = default(ANTLRParser.action_return);


		try
		{
			// Grammars\\ANTLR.g3:231:4: ( ( action )+ )
			// Grammars\\ANTLR.g3:231:4: ( action )+
			{
			root_0 = (GrammarAST)adaptor.Nil();

			// Grammars\\ANTLR.g3:231:4: ( action )+
			int cnt8=0;
			for ( ; ; )
			{
				int alt8=2;
				int LA8_0 = input.LA(1);

				if ( (LA8_0==AMPERSAND) )
				{
					alt8=1;
				}


				switch ( alt8 )
				{
				case 1:
					// Grammars\\ANTLR.g3:231:5: action
					{
					PushFollow(Follow._action_in_actions553);
					action8=action();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, action8.Tree);

					}
					break;

				default:
					if ( cnt8 >= 1 )
						goto loop8;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee8 = new EarlyExitException( 8, input );
					throw eee8;
				}
				cnt8++;
			}
			loop8:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "actions"

	public class action_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "action"
	// Grammars\\ANTLR.g3:235:0: action : AMPERSAND ( actionScopeName COLON COLON )? id ACTION ;
	private ANTLRParser.action_return action(  )
	{
		ANTLRParser.action_return retval = new ANTLRParser.action_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken AMPERSAND9=null;
		IToken COLON11=null;
		IToken COLON12=null;
		IToken ACTION14=null;
		ANTLRParser.actionScopeName_return actionScopeName10 = default(ANTLRParser.actionScopeName_return);
		ANTLRParser.id_return id13 = default(ANTLRParser.id_return);

		GrammarAST AMPERSAND9_tree=null;
		GrammarAST COLON11_tree=null;
		GrammarAST COLON12_tree=null;
		GrammarAST ACTION14_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:236:4: ( AMPERSAND ( actionScopeName COLON COLON )? id ACTION )
			// Grammars\\ANTLR.g3:236:4: AMPERSAND ( actionScopeName COLON COLON )? id ACTION
			{
			root_0 = (GrammarAST)adaptor.Nil();

			AMPERSAND9=(IToken)Match(input,AMPERSAND,Follow._AMPERSAND_in_action568); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			AMPERSAND9_tree = (GrammarAST)adaptor.Create(AMPERSAND9);
			root_0 = (GrammarAST)adaptor.BecomeRoot(AMPERSAND9_tree, root_0);
			}
			// Grammars\\ANTLR.g3:236:15: ( actionScopeName COLON COLON )?
			int alt9=2;
			switch ( input.LA(1) )
			{
			case TOKEN_REF:
				{
				int LA9_1 = input.LA(2);

				if ( (LA9_1==COLON) )
				{
					alt9=1;
				}
				}
				break;
			case RULE_REF:
				{
				int LA9_2 = input.LA(2);

				if ( (LA9_2==COLON) )
				{
					alt9=1;
				}
				}
				break;
			case LEXER:
			case PARSER:
				{
				alt9=1;
				}
				break;
			}

			switch ( alt9 )
			{
			case 1:
				// Grammars\\ANTLR.g3:236:16: actionScopeName COLON COLON
				{
				PushFollow(Follow._actionScopeName_in_action572);
				actionScopeName10=actionScopeName();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, actionScopeName10.Tree);
				COLON11=(IToken)Match(input,COLON,Follow._COLON_in_action574); if (state.failed) return retval;
				COLON12=(IToken)Match(input,COLON,Follow._COLON_in_action577); if (state.failed) return retval;

				}
				break;

			}

			PushFollow(Follow._id_in_action582);
			id13=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id13.Tree);
			ACTION14=(IToken)Match(input,ACTION,Follow._ACTION_in_action584); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ACTION14_tree = (GrammarAST)adaptor.Create(ACTION14);
			adaptor.AddChild(root_0, ACTION14_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "action"

	public class actionScopeName_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "actionScopeName"
	// Grammars\\ANTLR.g3:242:0: actionScopeName : ( id |l= 'lexer' -> ID[l] |p= 'parser' -> ID[p] );
	private ANTLRParser.actionScopeName_return actionScopeName(  )
	{
		ANTLRParser.actionScopeName_return retval = new ANTLRParser.actionScopeName_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken l=null;
		IToken p=null;
		ANTLRParser.id_return id15 = default(ANTLRParser.id_return);

		GrammarAST l_tree=null;
		GrammarAST p_tree=null;
		RewriteRuleITokenStream stream_LEXER=new RewriteRuleITokenStream(adaptor,"token LEXER");
		RewriteRuleITokenStream stream_PARSER=new RewriteRuleITokenStream(adaptor,"token PARSER");

		try
		{
			// Grammars\\ANTLR.g3:243:4: ( id |l= 'lexer' -> ID[l] |p= 'parser' -> ID[p] )
			int alt10=3;
			switch ( input.LA(1) )
			{
			case RULE_REF:
			case TOKEN_REF:
				{
				alt10=1;
				}
				break;
			case LEXER:
				{
				alt10=2;
				}
				break;
			case PARSER:
				{
				alt10=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 10, 0, input);

					throw nvae;
				}
			}

			switch ( alt10 )
			{
			case 1:
				// Grammars\\ANTLR.g3:243:4: id
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._id_in_actionScopeName597);
				id15=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id15.Tree);

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:244:4: l= 'lexer'
				{
				l=(IToken)Match(input,LEXER,Follow._LEXER_in_actionScopeName604); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LEXER.Add(l);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 245:3: -> ID[l]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ID, l));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:246:4: p= 'parser'
				{
				p=(IToken)Match(input,PARSER,Follow._PARSER_in_actionScopeName618); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PARSER.Add(p);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 247:3: -> ID[p]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ID, p));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "actionScopeName"

	public class optionsSpec_return : ParserRuleReturnScope
	{
		public IDictionary<string, object> opts=new Dictionary<string, object>();
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "optionsSpec"
	// Grammars\\ANTLR.g3:250:0: optionsSpec returns [IDictionary<string, object> opts=new Dictionary<string, object>()] : OPTIONS ( option[$opts] SEMI )+ RCURLY ;
	private ANTLRParser.optionsSpec_return optionsSpec(  )
	{
		ANTLRParser.optionsSpec_return retval = new ANTLRParser.optionsSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken OPTIONS16=null;
		IToken SEMI18=null;
		IToken RCURLY19=null;
		ANTLRParser.option_return option17 = default(ANTLRParser.option_return);

		GrammarAST OPTIONS16_tree=null;
		GrammarAST SEMI18_tree=null;
		GrammarAST RCURLY19_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:251:4: ( OPTIONS ( option[$opts] SEMI )+ RCURLY )
			// Grammars\\ANTLR.g3:251:4: OPTIONS ( option[$opts] SEMI )+ RCURLY
			{
			root_0 = (GrammarAST)adaptor.Nil();

			OPTIONS16=(IToken)Match(input,OPTIONS,Follow._OPTIONS_in_optionsSpec640); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			OPTIONS16_tree = (GrammarAST)adaptor.Create(OPTIONS16);
			root_0 = (GrammarAST)adaptor.BecomeRoot(OPTIONS16_tree, root_0);
			}
			// Grammars\\ANTLR.g3:251:13: ( option[$opts] SEMI )+
			int cnt11=0;
			for ( ; ; )
			{
				int alt11=2;
				int LA11_0 = input.LA(1);

				if ( (LA11_0==RULE_REF||LA11_0==TOKEN_REF) )
				{
					alt11=1;
				}


				switch ( alt11 )
				{
				case 1:
					// Grammars\\ANTLR.g3:251:14: option[$opts] SEMI
					{
					PushFollow(Follow._option_in_optionsSpec644);
					option17=option(retval.opts);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, option17.Tree);
					SEMI18=(IToken)Match(input,SEMI,Follow._SEMI_in_optionsSpec647); if (state.failed) return retval;

					}
					break;

				default:
					if ( cnt11 >= 1 )
						goto loop11;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee11 = new EarlyExitException( 11, input );
					throw eee11;
				}
				cnt11++;
			}
			loop11:
				;


			RCURLY19=(IToken)Match(input,RCURLY,Follow._RCURLY_in_optionsSpec652); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "optionsSpec"

	public class option_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "option"
	// Grammars\\ANTLR.g3:254:0: option[IDictionary<string, object> opts] : id ASSIGN optionValue ;
	private ANTLRParser.option_return option( IDictionary<string, object> opts )
	{
		ANTLRParser.option_return retval = new ANTLRParser.option_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken ASSIGN21=null;
		ANTLRParser.id_return id20 = default(ANTLRParser.id_return);
		ANTLRParser.optionValue_return optionValue22 = default(ANTLRParser.optionValue_return);

		GrammarAST ASSIGN21_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:255:4: ( id ASSIGN optionValue )
			// Grammars\\ANTLR.g3:255:4: id ASSIGN optionValue
			{
			root_0 = (GrammarAST)adaptor.Nil();

			PushFollow(Follow._id_in_option665);
			id20=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id20.Tree);
			ASSIGN21=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_option667); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			ASSIGN21_tree = (GrammarAST)adaptor.Create(ASSIGN21);
			root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN21_tree, root_0);
			}
			PushFollow(Follow._optionValue_in_option670);
			optionValue22=optionValue();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, optionValue22.Tree);
			if ( state.backtracking == 0 )
			{

							opts[(id20!=null?input.ToString(id20.start,id20.stop):null)] = (optionValue22!=null?optionValue22.value:default(object));
						
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "option"

	public class optionValue_return : ParserRuleReturnScope
	{
		public object value=null;
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "optionValue"
	// Grammars\\ANTLR.g3:261:0: optionValue returns [object value=null] : (x= id |s= STRING_LITERAL |c= CHAR_LITERAL |i= INT |ss= STAR -> STRING_LITERAL[$ss] );
	private ANTLRParser.optionValue_return optionValue(  )
	{
		ANTLRParser.optionValue_return retval = new ANTLRParser.optionValue_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken s=null;
		IToken c=null;
		IToken i=null;
		IToken ss=null;
		ANTLRParser.id_return x = default(ANTLRParser.id_return);

		GrammarAST s_tree=null;
		GrammarAST c_tree=null;
		GrammarAST i_tree=null;
		GrammarAST ss_tree=null;
		RewriteRuleITokenStream stream_STAR=new RewriteRuleITokenStream(adaptor,"token STAR");

		try
		{
			// Grammars\\ANTLR.g3:262:4: (x= id |s= STRING_LITERAL |c= CHAR_LITERAL |i= INT |ss= STAR -> STRING_LITERAL[$ss] )
			int alt12=5;
			switch ( input.LA(1) )
			{
			case RULE_REF:
			case TOKEN_REF:
				{
				alt12=1;
				}
				break;
			case STRING_LITERAL:
				{
				alt12=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt12=3;
				}
				break;
			case INT:
				{
				alt12=4;
				}
				break;
			case STAR:
				{
				alt12=5;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 12, 0, input);

					throw nvae;
				}
			}

			switch ( alt12 )
			{
			case 1:
				// Grammars\\ANTLR.g3:262:4: x= id
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._id_in_optionValue691);
				x=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, x.Tree);
				if ( state.backtracking == 0 )
				{
					retval.value = (x!=null?input.ToString(x.start,x.stop):null);
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:263:4: s= STRING_LITERAL
				{
				root_0 = (GrammarAST)adaptor.Nil();

				s=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_optionValue703); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				s_tree = (GrammarAST)adaptor.Create(s);
				adaptor.AddChild(root_0, s_tree);
				}
				if ( state.backtracking == 0 )
				{
					string vs = (s!=null?s.Text:null);
											  // remove the quotes:
											  retval.value =vs.Substring(1,vs.Length-2);
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:266:4: c= CHAR_LITERAL
				{
				root_0 = (GrammarAST)adaptor.Nil();

				c=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_optionValue712); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				c_tree = (GrammarAST)adaptor.Create(c);
				adaptor.AddChild(root_0, c_tree);
				}
				if ( state.backtracking == 0 )
				{
					string vs = (c!=null?c.Text:null);
											  // remove the quotes:
											  retval.value =vs.Substring(1,vs.Length-2);
				}

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:269:4: i= INT
				{
				root_0 = (GrammarAST)adaptor.Nil();

				i=(IToken)Match(input,INT,Follow._INT_in_optionValue723); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				i_tree = (GrammarAST)adaptor.Create(i);
				adaptor.AddChild(root_0, i_tree);
				}
				if ( state.backtracking == 0 )
				{
					retval.value = int.Parse((i!=null?i.Text:null));
				}

				}
				break;
			case 5:
				// Grammars\\ANTLR.g3:270:4: ss= STAR
				{
				ss=(IToken)Match(input,STAR,Follow._STAR_in_optionValue743); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STAR.Add(ss);

				if ( state.backtracking == 0 )
				{
					retval.value = "*";
				}


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 271:3: -> STRING_LITERAL[$ss]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(STRING_LITERAL, ss));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "optionValue"

	public class delegateGrammars_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "delegateGrammars"
	// Grammars\\ANTLR.g3:275:0: delegateGrammars : 'import' delegateGrammar ( COMMA delegateGrammar )* SEMI ;
	private ANTLRParser.delegateGrammars_return delegateGrammars(  )
	{
		ANTLRParser.delegateGrammars_return retval = new ANTLRParser.delegateGrammars_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal23=null;
		IToken COMMA25=null;
		IToken SEMI27=null;
		ANTLRParser.delegateGrammar_return delegateGrammar24 = default(ANTLRParser.delegateGrammar_return);
		ANTLRParser.delegateGrammar_return delegateGrammar26 = default(ANTLRParser.delegateGrammar_return);

		GrammarAST string_literal23_tree=null;
		GrammarAST COMMA25_tree=null;
		GrammarAST SEMI27_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:276:4: ( 'import' delegateGrammar ( COMMA delegateGrammar )* SEMI )
			// Grammars\\ANTLR.g3:276:4: 'import' delegateGrammar ( COMMA delegateGrammar )* SEMI
			{
			root_0 = (GrammarAST)adaptor.Nil();

			string_literal23=(IToken)Match(input,IMPORT,Follow._IMPORT_in_delegateGrammars768); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			string_literal23_tree = (GrammarAST)adaptor.Create(string_literal23);
			root_0 = (GrammarAST)adaptor.BecomeRoot(string_literal23_tree, root_0);
			}
			PushFollow(Follow._delegateGrammar_in_delegateGrammars771);
			delegateGrammar24=delegateGrammar();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, delegateGrammar24.Tree);
			// Grammars\\ANTLR.g3:276:30: ( COMMA delegateGrammar )*
			for ( ; ; )
			{
				int alt13=2;
				int LA13_0 = input.LA(1);

				if ( (LA13_0==COMMA) )
				{
					alt13=1;
				}


				switch ( alt13 )
				{
				case 1:
					// Grammars\\ANTLR.g3:276:31: COMMA delegateGrammar
					{
					COMMA25=(IToken)Match(input,COMMA,Follow._COMMA_in_delegateGrammars774); if (state.failed) return retval;
					PushFollow(Follow._delegateGrammar_in_delegateGrammars777);
					delegateGrammar26=delegateGrammar();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, delegateGrammar26.Tree);

					}
					break;

				default:
					goto loop13;
				}
			}

			loop13:
				;


			SEMI27=(IToken)Match(input,SEMI,Follow._SEMI_in_delegateGrammars781); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "delegateGrammars"

	public class delegateGrammar_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "delegateGrammar"
	// Grammars\\ANTLR.g3:279:0: delegateGrammar : (lab= id ASSIGN g= id |g2= id );
	private ANTLRParser.delegateGrammar_return delegateGrammar(  )
	{
		ANTLRParser.delegateGrammar_return retval = new ANTLRParser.delegateGrammar_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken ASSIGN28=null;
		ANTLRParser.id_return lab = default(ANTLRParser.id_return);
		ANTLRParser.id_return g = default(ANTLRParser.id_return);
		ANTLRParser.id_return g2 = default(ANTLRParser.id_return);

		GrammarAST ASSIGN28_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:280:4: (lab= id ASSIGN g= id |g2= id )
			int alt14=2;
			int LA14_0 = input.LA(1);

			if ( (LA14_0==TOKEN_REF) )
			{
				int LA14_1 = input.LA(2);

				if ( (LA14_1==ASSIGN) )
				{
					alt14=1;
				}
				else if ( (LA14_1==COMMA||LA14_1==SEMI) )
				{
					alt14=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 14, 1, input);

					throw nvae;
				}
			}
			else if ( (LA14_0==RULE_REF) )
			{
				int LA14_2 = input.LA(2);

				if ( (LA14_2==ASSIGN) )
				{
					alt14=1;
				}
				else if ( (LA14_2==COMMA||LA14_2==SEMI) )
				{
					alt14=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 14, 2, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

				throw nvae;
			}
			switch ( alt14 )
			{
			case 1:
				// Grammars\\ANTLR.g3:280:4: lab= id ASSIGN g= id
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._id_in_delegateGrammar795);
				lab=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, lab.Tree);
				ASSIGN28=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_delegateGrammar797); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				ASSIGN28_tree = (GrammarAST)adaptor.Create(ASSIGN28);
				root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN28_tree, root_0);
				}
				PushFollow(Follow._id_in_delegateGrammar802);
				g=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, g.Tree);
				if ( state.backtracking == 0 )
				{
					Grammar.importGrammar((g!=null?((GrammarAST)g.tree):null), (lab!=null?input.ToString(lab.start,lab.stop):null));
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:281:4: g2= id
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._id_in_delegateGrammar811);
				g2=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, g2.Tree);
				if ( state.backtracking == 0 )
				{
					Grammar.importGrammar((g2!=null?((GrammarAST)g2.tree):null),null);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "delegateGrammar"

	public class tokensSpec_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "tokensSpec"
	// Grammars\\ANTLR.g3:284:0: tokensSpec : TOKENS ( tokenSpec )+ RCURLY ;
	private ANTLRParser.tokensSpec_return tokensSpec(  )
	{
		ANTLRParser.tokensSpec_return retval = new ANTLRParser.tokensSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken TOKENS29=null;
		IToken RCURLY31=null;
		ANTLRParser.tokenSpec_return tokenSpec30 = default(ANTLRParser.tokenSpec_return);

		GrammarAST TOKENS29_tree=null;
		GrammarAST RCURLY31_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:285:4: ( TOKENS ( tokenSpec )+ RCURLY )
			// Grammars\\ANTLR.g3:285:4: TOKENS ( tokenSpec )+ RCURLY
			{
			root_0 = (GrammarAST)adaptor.Nil();

			TOKENS29=(IToken)Match(input,TOKENS,Follow._TOKENS_in_tokensSpec838); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			TOKENS29_tree = (GrammarAST)adaptor.Create(TOKENS29);
			root_0 = (GrammarAST)adaptor.BecomeRoot(TOKENS29_tree, root_0);
			}
			// Grammars\\ANTLR.g3:286:4: ( tokenSpec )+
			int cnt15=0;
			for ( ; ; )
			{
				int alt15=2;
				int LA15_0 = input.LA(1);

				if ( (LA15_0==TOKEN_REF) )
				{
					alt15=1;
				}


				switch ( alt15 )
				{
				case 1:
					// Grammars\\ANTLR.g3:286:6: tokenSpec
					{
					PushFollow(Follow._tokenSpec_in_tokensSpec846);
					tokenSpec30=tokenSpec();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, tokenSpec30.Tree);

					}
					break;

				default:
					if ( cnt15 >= 1 )
						goto loop15;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee15 = new EarlyExitException( 15, input );
					throw eee15;
				}
				cnt15++;
			}
			loop15:
				;


			RCURLY31=(IToken)Match(input,RCURLY,Follow._RCURLY_in_tokensSpec853); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "tokensSpec"

	public class tokenSpec_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "tokenSpec"
	// Grammars\\ANTLR.g3:290:0: tokenSpec : TOKEN_REF ( ASSIGN ( STRING_LITERAL | CHAR_LITERAL ) )? SEMI ;
	private ANTLRParser.tokenSpec_return tokenSpec(  )
	{
		ANTLRParser.tokenSpec_return retval = new ANTLRParser.tokenSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken TOKEN_REF32=null;
		IToken ASSIGN33=null;
		IToken set34=null;
		IToken SEMI35=null;

		GrammarAST TOKEN_REF32_tree=null;
		GrammarAST ASSIGN33_tree=null;
		GrammarAST set34_tree=null;
		GrammarAST SEMI35_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:291:4: ( TOKEN_REF ( ASSIGN ( STRING_LITERAL | CHAR_LITERAL ) )? SEMI )
			// Grammars\\ANTLR.g3:291:4: TOKEN_REF ( ASSIGN ( STRING_LITERAL | CHAR_LITERAL ) )? SEMI
			{
			root_0 = (GrammarAST)adaptor.Nil();

			TOKEN_REF32=(IToken)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_tokenSpec865); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			TOKEN_REF32_tree = (GrammarAST)adaptor.Create(TOKEN_REF32);
			adaptor.AddChild(root_0, TOKEN_REF32_tree);
			}
			// Grammars\\ANTLR.g3:291:14: ( ASSIGN ( STRING_LITERAL | CHAR_LITERAL ) )?
			int alt16=2;
			int LA16_0 = input.LA(1);

			if ( (LA16_0==ASSIGN) )
			{
				alt16=1;
			}
			switch ( alt16 )
			{
			case 1:
				// Grammars\\ANTLR.g3:291:16: ASSIGN ( STRING_LITERAL | CHAR_LITERAL )
				{
				ASSIGN33=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_tokenSpec869); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				ASSIGN33_tree = (GrammarAST)adaptor.Create(ASSIGN33);
				root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN33_tree, root_0);
				}
				set34=(IToken)input.LT(1);
				if ( input.LA(1)==CHAR_LITERAL||input.LA(1)==STRING_LITERAL )
				{
					input.Consume();
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(set34));
					state.errorRecovery=false;state.failed=false;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					MismatchedSetException mse = new MismatchedSetException(null,input);
					throw mse;
				}


				}
				break;

			}

			SEMI35=(IToken)Match(input,SEMI,Follow._SEMI_in_tokenSpec881); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "tokenSpec"

	public class attrScopes_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "attrScopes"
	// Grammars\\ANTLR.g3:294:0: attrScopes : ( attrScope )* ;
	private ANTLRParser.attrScopes_return attrScopes(  )
	{
		ANTLRParser.attrScopes_return retval = new ANTLRParser.attrScopes_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.attrScope_return attrScope36 = default(ANTLRParser.attrScope_return);


		try
		{
			// Grammars\\ANTLR.g3:295:4: ( ( attrScope )* )
			// Grammars\\ANTLR.g3:295:4: ( attrScope )*
			{
			root_0 = (GrammarAST)adaptor.Nil();

			// Grammars\\ANTLR.g3:295:4: ( attrScope )*
			for ( ; ; )
			{
				int alt17=2;
				int LA17_0 = input.LA(1);

				if ( (LA17_0==SCOPE) )
				{
					alt17=1;
				}


				switch ( alt17 )
				{
				case 1:
					// Grammars\\ANTLR.g3:295:5: attrScope
					{
					PushFollow(Follow._attrScope_in_attrScopes894);
					attrScope36=attrScope();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, attrScope36.Tree);

					}
					break;

				default:
					goto loop17;
				}
			}

			loop17:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "attrScopes"

	public class attrScope_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "attrScope"
	// Grammars\\ANTLR.g3:298:0: attrScope : 'scope' id ( ruleActions )? ACTION ;
	private ANTLRParser.attrScope_return attrScope(  )
	{
		ANTLRParser.attrScope_return retval = new ANTLRParser.attrScope_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal37=null;
		IToken ACTION40=null;
		ANTLRParser.id_return id38 = default(ANTLRParser.id_return);
		ANTLRParser.ruleActions_return ruleActions39 = default(ANTLRParser.ruleActions_return);

		GrammarAST string_literal37_tree=null;
		GrammarAST ACTION40_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:299:4: ( 'scope' id ( ruleActions )? ACTION )
			// Grammars\\ANTLR.g3:299:4: 'scope' id ( ruleActions )? ACTION
			{
			root_0 = (GrammarAST)adaptor.Nil();

			string_literal37=(IToken)Match(input,SCOPE,Follow._SCOPE_in_attrScope907); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			string_literal37_tree = (GrammarAST)adaptor.Create(string_literal37);
			root_0 = (GrammarAST)adaptor.BecomeRoot(string_literal37_tree, root_0);
			}
			PushFollow(Follow._id_in_attrScope910);
			id38=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id38.Tree);
			// Grammars\\ANTLR.g3:299:16: ( ruleActions )?
			int alt18=2;
			int LA18_0 = input.LA(1);

			if ( (LA18_0==AMPERSAND) )
			{
				alt18=1;
			}
			switch ( alt18 )
			{
			case 1:
				// Grammars\\ANTLR.g3:299:0: ruleActions
				{
				PushFollow(Follow._ruleActions_in_attrScope912);
				ruleActions39=ruleActions();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ruleActions39.Tree);

				}
				break;

			}

			ACTION40=(IToken)Match(input,ACTION,Follow._ACTION_in_attrScope915); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ACTION40_tree = (GrammarAST)adaptor.Create(ACTION40);
			adaptor.AddChild(root_0, ACTION40_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "attrScope"

	public class rules_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rules"
	// Grammars\\ANTLR.g3:302:0: rules : ( rule )+ ;
	private ANTLRParser.rules_return rules(  )
	{
		ANTLRParser.rules_return retval = new ANTLRParser.rules_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.rule_return rule41 = default(ANTLRParser.rule_return);


		try
		{
			// Grammars\\ANTLR.g3:303:4: ( ( rule )+ )
			// Grammars\\ANTLR.g3:303:4: ( rule )+
			{
			root_0 = (GrammarAST)adaptor.Nil();

			// Grammars\\ANTLR.g3:303:4: ( rule )+
			int cnt19=0;
			for ( ; ; )
			{
				int alt19=2;
				int LA19_0 = input.LA(1);

				if ( (LA19_0==DOC_COMMENT||LA19_0==FRAGMENT||(LA19_0>=PRIVATE && LA19_0<=PUBLIC)||LA19_0==RULE_REF||LA19_0==TOKEN_REF) )
				{
					alt19=1;
				}


				switch ( alt19 )
				{
				case 1:
					// Grammars\\ANTLR.g3:303:6: rule
					{
					PushFollow(Follow._rule_in_rules928);
					rule41=rule();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rule41.Tree);

					}
					break;

				default:
					if ( cnt19 >= 1 )
						goto loop19;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee19 = new EarlyExitException( 19, input );
					throw eee19;
				}
				cnt19++;
			}
			loop19:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rules"

	public class rule_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rule"
	// Grammars\\ANTLR.g3:308:0: public rule : ( (d= DOC_COMMENT )? (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )? ruleName= id ( BANG )? (aa= ARG_ACTION )? ( 'returns' rt= ARG_ACTION )? ( throwsSpec )? ( optionsSpec )? scopes= ruleScopeSpec ( ruleActions )? COLON altList[$optionsSpec.opts] SEMI (ex= exceptionGroup )? -> ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] ) ) ;
	public ANTLRParser.rule_return rule(  )
	{
		ANTLRParser.rule_return retval = new ANTLRParser.rule_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken d=null;
		IToken p1=null;
		IToken p2=null;
		IToken p3=null;
		IToken p4=null;
		IToken aa=null;
		IToken rt=null;
		IToken BANG42=null;
		IToken string_literal43=null;
		IToken COLON47=null;
		IToken SEMI49=null;
		ANTLRParser.id_return ruleName = default(ANTLRParser.id_return);
		ANTLRParser.ruleScopeSpec_return scopes = default(ANTLRParser.ruleScopeSpec_return);
		ANTLRParser.exceptionGroup_return ex = default(ANTLRParser.exceptionGroup_return);
		ANTLRParser.throwsSpec_return throwsSpec44 = default(ANTLRParser.throwsSpec_return);
		ANTLRParser.optionsSpec_return optionsSpec45 = default(ANTLRParser.optionsSpec_return);
		ANTLRParser.ruleActions_return ruleActions46 = default(ANTLRParser.ruleActions_return);
		ANTLRParser.altList_return altList48 = default(ANTLRParser.altList_return);

		GrammarAST d_tree=null;
		GrammarAST p1_tree=null;
		GrammarAST p2_tree=null;
		GrammarAST p3_tree=null;
		GrammarAST p4_tree=null;
		GrammarAST aa_tree=null;
		GrammarAST rt_tree=null;
		GrammarAST BANG42_tree=null;
		GrammarAST string_literal43_tree=null;
		GrammarAST COLON47_tree=null;
		GrammarAST SEMI49_tree=null;
		RewriteRuleITokenStream stream_DOC_COMMENT=new RewriteRuleITokenStream(adaptor,"token DOC_COMMENT");
		RewriteRuleITokenStream stream_PROTECTED=new RewriteRuleITokenStream(adaptor,"token PROTECTED");
		RewriteRuleITokenStream stream_PUBLIC=new RewriteRuleITokenStream(adaptor,"token PUBLIC");
		RewriteRuleITokenStream stream_PRIVATE=new RewriteRuleITokenStream(adaptor,"token PRIVATE");
		RewriteRuleITokenStream stream_FRAGMENT=new RewriteRuleITokenStream(adaptor,"token FRAGMENT");
		RewriteRuleITokenStream stream_BANG=new RewriteRuleITokenStream(adaptor,"token BANG");
		RewriteRuleITokenStream stream_ARG_ACTION=new RewriteRuleITokenStream(adaptor,"token ARG_ACTION");
		RewriteRuleITokenStream stream_RETURNS=new RewriteRuleITokenStream(adaptor,"token RETURNS");
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_id=new RewriteRuleSubtreeStream(adaptor,"rule id");
		RewriteRuleSubtreeStream stream_throwsSpec=new RewriteRuleSubtreeStream(adaptor,"rule throwsSpec");
		RewriteRuleSubtreeStream stream_optionsSpec=new RewriteRuleSubtreeStream(adaptor,"rule optionsSpec");
		RewriteRuleSubtreeStream stream_ruleScopeSpec=new RewriteRuleSubtreeStream(adaptor,"rule ruleScopeSpec");
		RewriteRuleSubtreeStream stream_ruleActions=new RewriteRuleSubtreeStream(adaptor,"rule ruleActions");
		RewriteRuleSubtreeStream stream_altList=new RewriteRuleSubtreeStream(adaptor,"rule altList");
		RewriteRuleSubtreeStream stream_exceptionGroup=new RewriteRuleSubtreeStream(adaptor,"rule exceptionGroup");

			GrammarAST eob=null;
			int start = ((CommonToken)LT(1)).TokenIndex;
			int startLine = LT(1).Line;

		try
		{
			// Grammars\\ANTLR.g3:316:2: ( ( (d= DOC_COMMENT )? (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )? ruleName= id ( BANG )? (aa= ARG_ACTION )? ( 'returns' rt= ARG_ACTION )? ( throwsSpec )? ( optionsSpec )? scopes= ruleScopeSpec ( ruleActions )? COLON altList[$optionsSpec.opts] SEMI (ex= exceptionGroup )? -> ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] ) ) )
			// Grammars\\ANTLR.g3:316:2: ( (d= DOC_COMMENT )? (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )? ruleName= id ( BANG )? (aa= ARG_ACTION )? ( 'returns' rt= ARG_ACTION )? ( throwsSpec )? ( optionsSpec )? scopes= ruleScopeSpec ( ruleActions )? COLON altList[$optionsSpec.opts] SEMI (ex= exceptionGroup )? -> ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] ) )
			{
			// Grammars\\ANTLR.g3:316:2: ( (d= DOC_COMMENT )? (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )? ruleName= id ( BANG )? (aa= ARG_ACTION )? ( 'returns' rt= ARG_ACTION )? ( throwsSpec )? ( optionsSpec )? scopes= ruleScopeSpec ( ruleActions )? COLON altList[$optionsSpec.opts] SEMI (ex= exceptionGroup )? -> ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] ) )
			// Grammars\\ANTLR.g3:316:4: (d= DOC_COMMENT )? (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )? ruleName= id ( BANG )? (aa= ARG_ACTION )? ( 'returns' rt= ARG_ACTION )? ( throwsSpec )? ( optionsSpec )? scopes= ruleScopeSpec ( ruleActions )? COLON altList[$optionsSpec.opts] SEMI (ex= exceptionGroup )?
			{
			// Grammars\\ANTLR.g3:316:4: (d= DOC_COMMENT )?
			int alt20=2;
			int LA20_0 = input.LA(1);

			if ( (LA20_0==DOC_COMMENT) )
			{
				alt20=1;
			}
			switch ( alt20 )
			{
			case 1:
				// Grammars\\ANTLR.g3:316:6: d= DOC_COMMENT
				{
				d=(IToken)Match(input,DOC_COMMENT,Follow._DOC_COMMENT_in_rule958); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_DOC_COMMENT.Add(d);


				}
				break;

			}

			// Grammars\\ANTLR.g3:318:3: (p1= 'protected' |p2= 'public' |p3= 'private' |p4= 'fragment' )?
			int alt21=5;
			switch ( input.LA(1) )
			{
			case PROTECTED:
				{
				alt21=1;
				}
				break;
			case PUBLIC:
				{
				alt21=2;
				}
				break;
			case PRIVATE:
				{
				alt21=3;
				}
				break;
			case FRAGMENT:
				{
				alt21=4;
				}
				break;
			}

			switch ( alt21 )
			{
			case 1:
				// Grammars\\ANTLR.g3:318:5: p1= 'protected'
				{
				p1=(IToken)Match(input,PROTECTED,Follow._PROTECTED_in_rule971); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PROTECTED.Add(p1);


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:319:5: p2= 'public'
				{
				p2=(IToken)Match(input,PUBLIC,Follow._PUBLIC_in_rule980); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PUBLIC.Add(p2);


				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:320:5: p3= 'private'
				{
				p3=(IToken)Match(input,PRIVATE,Follow._PRIVATE_in_rule990); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PRIVATE.Add(p3);


				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:321:5: p4= 'fragment'
				{
				p4=(IToken)Match(input,FRAGMENT,Follow._FRAGMENT_in_rule999); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_FRAGMENT.Add(p4);


				}
				break;

			}

			PushFollow(Follow._id_in_rule1011);
			ruleName=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_id.Add(ruleName.Tree);
			if ( state.backtracking == 0 )
			{

							currentRuleName=(ruleName!=null?input.ToString(ruleName.start,ruleName.stop):null);
							if ( GrammarType==LEXER_GRAMMAR && p4==null )
								Grammar.lexerRuleNamesInCombined.Add(currentRuleName);
						
			}
			// Grammars\\ANTLR.g3:329:3: ( BANG )?
			int alt22=2;
			int LA22_0 = input.LA(1);

			if ( (LA22_0==BANG) )
			{
				alt22=1;
			}
			switch ( alt22 )
			{
			case 1:
				// Grammars\\ANTLR.g3:329:5: BANG
				{
				BANG42=(IToken)Match(input,BANG,Follow._BANG_in_rule1021); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_BANG.Add(BANG42);


				}
				break;

			}

			// Grammars\\ANTLR.g3:330:3: (aa= ARG_ACTION )?
			int alt23=2;
			int LA23_0 = input.LA(1);

			if ( (LA23_0==ARG_ACTION) )
			{
				alt23=1;
			}
			switch ( alt23 )
			{
			case 1:
				// Grammars\\ANTLR.g3:330:5: aa= ARG_ACTION
				{
				aa=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule1032); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ARG_ACTION.Add(aa);


				}
				break;

			}

			// Grammars\\ANTLR.g3:331:3: ( 'returns' rt= ARG_ACTION )?
			int alt24=2;
			int LA24_0 = input.LA(1);

			if ( (LA24_0==RETURNS) )
			{
				alt24=1;
			}
			switch ( alt24 )
			{
			case 1:
				// Grammars\\ANTLR.g3:331:5: 'returns' rt= ARG_ACTION
				{
				string_literal43=(IToken)Match(input,RETURNS,Follow._RETURNS_in_rule1041); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RETURNS.Add(string_literal43);

				rt=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rule1045); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ARG_ACTION.Add(rt);


				}
				break;

			}

			// Grammars\\ANTLR.g3:332:3: ( throwsSpec )?
			int alt25=2;
			int LA25_0 = input.LA(1);

			if ( (LA25_0==THROWS) )
			{
				alt25=1;
			}
			switch ( alt25 )
			{
			case 1:
				// Grammars\\ANTLR.g3:332:5: throwsSpec
				{
				PushFollow(Follow._throwsSpec_in_rule1055);
				throwsSpec44=throwsSpec();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_throwsSpec.Add(throwsSpec44.Tree);

				}
				break;

			}

			// Grammars\\ANTLR.g3:333:3: ( optionsSpec )?
			int alt26=2;
			int LA26_0 = input.LA(1);

			if ( (LA26_0==OPTIONS) )
			{
				alt26=1;
			}
			switch ( alt26 )
			{
			case 1:
				// Grammars\\ANTLR.g3:333:5: optionsSpec
				{
				PushFollow(Follow._optionsSpec_in_rule1064);
				optionsSpec45=optionsSpec();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_optionsSpec.Add(optionsSpec45.Tree);

				}
				break;

			}

			PushFollow(Follow._ruleScopeSpec_in_rule1073);
			scopes=ruleScopeSpec();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_ruleScopeSpec.Add(scopes.Tree);
			// Grammars\\ANTLR.g3:335:3: ( ruleActions )?
			int alt27=2;
			int LA27_0 = input.LA(1);

			if ( (LA27_0==AMPERSAND) )
			{
				alt27=1;
			}
			switch ( alt27 )
			{
			case 1:
				// Grammars\\ANTLR.g3:335:4: ruleActions
				{
				PushFollow(Follow._ruleActions_in_rule1078);
				ruleActions46=ruleActions();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_ruleActions.Add(ruleActions46.Tree);

				}
				break;

			}

			COLON47=(IToken)Match(input,COLON,Follow._COLON_in_rule1084); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_COLON.Add(COLON47);

			PushFollow(Follow._altList_in_rule1088);
			altList48=altList((optionsSpec45!=null?optionsSpec45.opts:default(IDictionary<string, object>)));

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_altList.Add(altList48.Tree);
			SEMI49=(IToken)Match(input,SEMI,Follow._SEMI_in_rule1093); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI49);

			// Grammars\\ANTLR.g3:339:3: (ex= exceptionGroup )?
			int alt28=2;
			int LA28_0 = input.LA(1);

			if ( (LA28_0==CATCH||LA28_0==FINALLY) )
			{
				alt28=1;
			}
			switch ( alt28 )
			{
			case 1:
				// Grammars\\ANTLR.g3:339:5: ex= exceptionGroup
				{
				PushFollow(Follow._exceptionGroup_in_rule1101);
				ex=exceptionGroup();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_exceptionGroup.Add(ex.Tree);

				}
				break;

			}



			{
			// AST REWRITE
			// elements: ruleName, p1, p2, p3, p4, aa, rt, throwsSpec, optionsSpec, scopes, ruleActions, altList, ex
			// token labels: p1, p2, p3, p4, aa, rt
			// rule labels: ruleName, scopes, ex, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleITokenStream stream_p1=new RewriteRuleITokenStream(adaptor,"token p1",p1);
			RewriteRuleITokenStream stream_p2=new RewriteRuleITokenStream(adaptor,"token p2",p2);
			RewriteRuleITokenStream stream_p3=new RewriteRuleITokenStream(adaptor,"token p3",p3);
			RewriteRuleITokenStream stream_p4=new RewriteRuleITokenStream(adaptor,"token p4",p4);
			RewriteRuleITokenStream stream_aa=new RewriteRuleITokenStream(adaptor,"token aa",aa);
			RewriteRuleITokenStream stream_rt=new RewriteRuleITokenStream(adaptor,"token rt",rt);
			RewriteRuleSubtreeStream stream_ruleName=new RewriteRuleSubtreeStream(adaptor,"rule ruleName",ruleName!=null?ruleName.tree:null);
			RewriteRuleSubtreeStream stream_scopes=new RewriteRuleSubtreeStream(adaptor,"rule scopes",scopes!=null?scopes.tree:null);
			RewriteRuleSubtreeStream stream_ex=new RewriteRuleSubtreeStream(adaptor,"rule ex",ex!=null?ex.tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 340:3: -> ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] )
			{
				// Grammars\\ANTLR.g3:340:6: ^( RULE[\"rule\"] ^( $ruleName) ( $p1)? ( $p2)? ( $p3)? ( $p4)? ^( ARG[\"ARG\"] ( $aa)? ) ^( RET[\"RET\"] ( $rt)? ) ( throwsSpec )? ( optionsSpec )? ^( $scopes) ( ruleActions )? altList ( $ex)? EOR[$SEMI,\"<end-of-rule>\"] )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(RULE, "rule"), root_1);

				// Grammars\\ANTLR.g3:341:5: ^( $ruleName)
				{
				GrammarAST root_2 = (GrammarAST)adaptor.Nil();
				root_2 = (GrammarAST)adaptor.BecomeRoot(stream_ruleName.NextNode(), root_2);

				adaptor.AddChild(root_1, root_2);
				}
				// Grammars\\ANTLR.g3:343:6: ( $p1)?
				if ( stream_p1.HasNext )
				{
					adaptor.AddChild(root_1, stream_p1.NextNode());

				}
				stream_p1.Reset();
				// Grammars\\ANTLR.g3:343:11: ( $p2)?
				if ( stream_p2.HasNext )
				{
					adaptor.AddChild(root_1, stream_p2.NextNode());

				}
				stream_p2.Reset();
				// Grammars\\ANTLR.g3:343:16: ( $p3)?
				if ( stream_p3.HasNext )
				{
					adaptor.AddChild(root_1, stream_p3.NextNode());

				}
				stream_p3.Reset();
				// Grammars\\ANTLR.g3:343:21: ( $p4)?
				if ( stream_p4.HasNext )
				{
					adaptor.AddChild(root_1, stream_p4.NextNode());

				}
				stream_p4.Reset();
				// Grammars\\ANTLR.g3:344:5: ^( ARG[\"ARG\"] ( $aa)? )
				{
				GrammarAST root_2 = (GrammarAST)adaptor.Nil();
				root_2 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ARG, "ARG"), root_2);

				// Grammars\\ANTLR.g3:344:19: ( $aa)?
				if ( stream_aa.HasNext )
				{
					adaptor.AddChild(root_2, stream_aa.NextNode());

				}
				stream_aa.Reset();

				adaptor.AddChild(root_1, root_2);
				}
				// Grammars\\ANTLR.g3:345:5: ^( RET[\"RET\"] ( $rt)? )
				{
				GrammarAST root_2 = (GrammarAST)adaptor.Nil();
				root_2 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(RET, "RET"), root_2);

				// Grammars\\ANTLR.g3:345:19: ( $rt)?
				if ( stream_rt.HasNext )
				{
					adaptor.AddChild(root_2, stream_rt.NextNode());

				}
				stream_rt.Reset();

				adaptor.AddChild(root_1, root_2);
				}
				// Grammars\\ANTLR.g3:346:5: ( throwsSpec )?
				if ( stream_throwsSpec.HasNext )
				{
					adaptor.AddChild(root_1, stream_throwsSpec.NextTree());

				}
				stream_throwsSpec.Reset();
				// Grammars\\ANTLR.g3:347:5: ( optionsSpec )?
				if ( stream_optionsSpec.HasNext )
				{
					adaptor.AddChild(root_1, stream_optionsSpec.NextTree());

				}
				stream_optionsSpec.Reset();
				// Grammars\\ANTLR.g3:348:5: ^( $scopes)
				{
				GrammarAST root_2 = (GrammarAST)adaptor.Nil();
				root_2 = (GrammarAST)adaptor.BecomeRoot(stream_scopes.NextNode(), root_2);

				adaptor.AddChild(root_1, root_2);
				}
				// Grammars\\ANTLR.g3:349:5: ( ruleActions )?
				if ( stream_ruleActions.HasNext )
				{
					adaptor.AddChild(root_1, stream_ruleActions.NextTree());

				}
				stream_ruleActions.Reset();
				adaptor.AddChild(root_1, stream_altList.NextTree());
				// Grammars\\ANTLR.g3:351:6: ( $ex)?
				if ( stream_ex.HasNext )
				{
					adaptor.AddChild(root_1, stream_ex.NextTree());

				}
				stream_ex.Reset();
				adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOR, SEMI49, "<end-of-rule>"));

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{

						((GrammarAST)((GrammarAST)retval.tree).GetChild(0)).setBlockOptions( (optionsSpec45!=null?optionsSpec45.opts:default(IDictionary<string, object>)) );
					
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rule"

	public class ruleActions_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ruleActions"
	// Grammars\\ANTLR.g3:359:0: ruleActions : ( ruleAction )+ ;
	private ANTLRParser.ruleActions_return ruleActions(  )
	{
		ANTLRParser.ruleActions_return retval = new ANTLRParser.ruleActions_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.ruleAction_return ruleAction50 = default(ANTLRParser.ruleAction_return);


		try
		{
			// Grammars\\ANTLR.g3:360:4: ( ( ruleAction )+ )
			// Grammars\\ANTLR.g3:360:4: ( ruleAction )+
			{
			root_0 = (GrammarAST)adaptor.Nil();

			// Grammars\\ANTLR.g3:360:4: ( ruleAction )+
			int cnt29=0;
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
					// Grammars\\ANTLR.g3:360:5: ruleAction
					{
					PushFollow(Follow._ruleAction_in_ruleActions1243);
					ruleAction50=ruleAction();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ruleAction50.Tree);

					}
					break;

				default:
					if ( cnt29 >= 1 )
						goto loop29;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee29 = new EarlyExitException( 29, input );
					throw eee29;
				}
				cnt29++;
			}
			loop29:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ruleActions"

	public class ruleAction_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ruleAction"
	// Grammars\\ANTLR.g3:364:0: ruleAction : AMPERSAND id ACTION ;
	private ANTLRParser.ruleAction_return ruleAction(  )
	{
		ANTLRParser.ruleAction_return retval = new ANTLRParser.ruleAction_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken AMPERSAND51=null;
		IToken ACTION53=null;
		ANTLRParser.id_return id52 = default(ANTLRParser.id_return);

		GrammarAST AMPERSAND51_tree=null;
		GrammarAST ACTION53_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:365:4: ( AMPERSAND id ACTION )
			// Grammars\\ANTLR.g3:365:4: AMPERSAND id ACTION
			{
			root_0 = (GrammarAST)adaptor.Nil();

			AMPERSAND51=(IToken)Match(input,AMPERSAND,Follow._AMPERSAND_in_ruleAction1258); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			AMPERSAND51_tree = (GrammarAST)adaptor.Create(AMPERSAND51);
			root_0 = (GrammarAST)adaptor.BecomeRoot(AMPERSAND51_tree, root_0);
			}
			PushFollow(Follow._id_in_ruleAction1261);
			id52=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id52.Tree);
			ACTION53=(IToken)Match(input,ACTION,Follow._ACTION_in_ruleAction1263); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ACTION53_tree = (GrammarAST)adaptor.Create(ACTION53);
			adaptor.AddChild(root_0, ACTION53_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ruleAction"

	public class throwsSpec_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "throwsSpec"
	// Grammars\\ANTLR.g3:368:0: throwsSpec : 'throws' id ( COMMA id )* ;
	private ANTLRParser.throwsSpec_return throwsSpec(  )
	{
		ANTLRParser.throwsSpec_return retval = new ANTLRParser.throwsSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal54=null;
		IToken COMMA56=null;
		ANTLRParser.id_return id55 = default(ANTLRParser.id_return);
		ANTLRParser.id_return id57 = default(ANTLRParser.id_return);

		GrammarAST string_literal54_tree=null;
		GrammarAST COMMA56_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:369:4: ( 'throws' id ( COMMA id )* )
			// Grammars\\ANTLR.g3:369:4: 'throws' id ( COMMA id )*
			{
			root_0 = (GrammarAST)adaptor.Nil();

			string_literal54=(IToken)Match(input,THROWS,Follow._THROWS_in_throwsSpec1274); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			string_literal54_tree = (GrammarAST)adaptor.Create(string_literal54);
			root_0 = (GrammarAST)adaptor.BecomeRoot(string_literal54_tree, root_0);
			}
			PushFollow(Follow._id_in_throwsSpec1277);
			id55=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id55.Tree);
			// Grammars\\ANTLR.g3:369:17: ( COMMA id )*
			for ( ; ; )
			{
				int alt30=2;
				int LA30_0 = input.LA(1);

				if ( (LA30_0==COMMA) )
				{
					alt30=1;
				}


				switch ( alt30 )
				{
				case 1:
					// Grammars\\ANTLR.g3:369:19: COMMA id
					{
					COMMA56=(IToken)Match(input,COMMA,Follow._COMMA_in_throwsSpec1281); if (state.failed) return retval;
					PushFollow(Follow._id_in_throwsSpec1284);
					id57=id();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id57.Tree);

					}
					break;

				default:
					goto loop30;
				}
			}

			loop30:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "throwsSpec"

	public class ruleScopeSpec_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ruleScopeSpec"
	// Grammars\\ANTLR.g3:372:0: ruleScopeSpec : ( 'scope' ( ruleActions )? ACTION )? ( 'scope' idList SEMI )* -> ^( SCOPE[$start,\"scope\"] ( ruleActions )? ( ACTION )? ( idList )* ) ;
	private ANTLRParser.ruleScopeSpec_return ruleScopeSpec(  )
	{
		ANTLRParser.ruleScopeSpec_return retval = new ANTLRParser.ruleScopeSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal58=null;
		IToken ACTION60=null;
		IToken string_literal61=null;
		IToken SEMI63=null;
		ANTLRParser.ruleActions_return ruleActions59 = default(ANTLRParser.ruleActions_return);
		ANTLRParser.idList_return idList62 = default(ANTLRParser.idList_return);

		GrammarAST string_literal58_tree=null;
		GrammarAST ACTION60_tree=null;
		GrammarAST string_literal61_tree=null;
		GrammarAST SEMI63_tree=null;
		RewriteRuleITokenStream stream_SCOPE=new RewriteRuleITokenStream(adaptor,"token SCOPE");
		RewriteRuleITokenStream stream_ACTION=new RewriteRuleITokenStream(adaptor,"token ACTION");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_ruleActions=new RewriteRuleSubtreeStream(adaptor,"rule ruleActions");
		RewriteRuleSubtreeStream stream_idList=new RewriteRuleSubtreeStream(adaptor,"rule idList");
		try
		{
			// Grammars\\ANTLR.g3:373:4: ( ( 'scope' ( ruleActions )? ACTION )? ( 'scope' idList SEMI )* -> ^( SCOPE[$start,\"scope\"] ( ruleActions )? ( ACTION )? ( idList )* ) )
			// Grammars\\ANTLR.g3:373:4: ( 'scope' ( ruleActions )? ACTION )? ( 'scope' idList SEMI )*
			{
			// Grammars\\ANTLR.g3:373:4: ( 'scope' ( ruleActions )? ACTION )?
			int alt32=2;
			int LA32_0 = input.LA(1);

			if ( (LA32_0==SCOPE) )
			{
				int LA32_1 = input.LA(2);

				if ( (LA32_1==ACTION||LA32_1==AMPERSAND) )
				{
					alt32=1;
				}
			}
			switch ( alt32 )
			{
			case 1:
				// Grammars\\ANTLR.g3:373:6: 'scope' ( ruleActions )? ACTION
				{
				string_literal58=(IToken)Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec1300); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_SCOPE.Add(string_literal58);

				// Grammars\\ANTLR.g3:373:14: ( ruleActions )?
				int alt31=2;
				int LA31_0 = input.LA(1);

				if ( (LA31_0==AMPERSAND) )
				{
					alt31=1;
				}
				switch ( alt31 )
				{
				case 1:
					// Grammars\\ANTLR.g3:373:0: ruleActions
					{
					PushFollow(Follow._ruleActions_in_ruleScopeSpec1302);
					ruleActions59=ruleActions();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_ruleActions.Add(ruleActions59.Tree);

					}
					break;

				}

				ACTION60=(IToken)Match(input,ACTION,Follow._ACTION_in_ruleScopeSpec1305); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ACTION.Add(ACTION60);


				}
				break;

			}

			// Grammars\\ANTLR.g3:374:3: ( 'scope' idList SEMI )*
			for ( ; ; )
			{
				int alt33=2;
				int LA33_0 = input.LA(1);

				if ( (LA33_0==SCOPE) )
				{
					alt33=1;
				}


				switch ( alt33 )
				{
				case 1:
					// Grammars\\ANTLR.g3:374:5: 'scope' idList SEMI
					{
					string_literal61=(IToken)Match(input,SCOPE,Follow._SCOPE_in_ruleScopeSpec1314); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SCOPE.Add(string_literal61);

					PushFollow(Follow._idList_in_ruleScopeSpec1316);
					idList62=idList();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_idList.Add(idList62.Tree);
					SEMI63=(IToken)Match(input,SEMI,Follow._SEMI_in_ruleScopeSpec1318); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI63);


					}
					break;

				default:
					goto loop33;
				}
			}

			loop33:
				;




			{
			// AST REWRITE
			// elements: ruleActions, ACTION, idList
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 375:3: -> ^( SCOPE[$start,\"scope\"] ( ruleActions )? ( ACTION )? ( idList )* )
			{
				// Grammars\\ANTLR.g3:375:6: ^( SCOPE[$start,\"scope\"] ( ruleActions )? ( ACTION )? ( idList )* )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(SCOPE, ((IToken)retval.start), "scope"), root_1);

				// Grammars\\ANTLR.g3:375:30: ( ruleActions )?
				if ( stream_ruleActions.HasNext )
				{
					adaptor.AddChild(root_1, stream_ruleActions.NextTree());

				}
				stream_ruleActions.Reset();
				// Grammars\\ANTLR.g3:375:43: ( ACTION )?
				if ( stream_ACTION.HasNext )
				{
					adaptor.AddChild(root_1, stream_ACTION.NextNode());

				}
				stream_ACTION.Reset();
				// Grammars\\ANTLR.g3:375:51: ( idList )*
				while ( stream_idList.HasNext )
				{
					adaptor.AddChild(root_1, stream_idList.NextTree());

				}
				stream_idList.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ruleScopeSpec"

	public class block_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "block"
	// Grammars\\ANTLR.g3:379:0: block : (lp= LPAREN -> BLOCK[$lp,\"BLOCK\"] ) ( ( optionsSpec )? ( ruleActions )? COLON | ACTION COLON )? a= alternative r= rewrite ( OR a= alternative r= rewrite )* rp= RPAREN -> ^( $block ( optionsSpec )? ( ruleActions )? ( ACTION )? ( alternative )+ EOB[$rp,\"<end-of-block>\"] ) ;
	private ANTLRParser.block_return block(  )
	{
		ANTLRParser.block_return retval = new ANTLRParser.block_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken lp=null;
		IToken rp=null;
		IToken COLON66=null;
		IToken ACTION67=null;
		IToken COLON68=null;
		IToken OR69=null;
		ANTLRParser.alternative_return a = default(ANTLRParser.alternative_return);
		ANTLRParser.rewrite_return r = default(ANTLRParser.rewrite_return);
		ANTLRParser.optionsSpec_return optionsSpec64 = default(ANTLRParser.optionsSpec_return);
		ANTLRParser.ruleActions_return ruleActions65 = default(ANTLRParser.ruleActions_return);

		GrammarAST lp_tree=null;
		GrammarAST rp_tree=null;
		GrammarAST COLON66_tree=null;
		GrammarAST ACTION67_tree=null;
		GrammarAST COLON68_tree=null;
		GrammarAST OR69_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleITokenStream stream_ACTION=new RewriteRuleITokenStream(adaptor,"token ACTION");
		RewriteRuleITokenStream stream_OR=new RewriteRuleITokenStream(adaptor,"token OR");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_optionsSpec=new RewriteRuleSubtreeStream(adaptor,"rule optionsSpec");
		RewriteRuleSubtreeStream stream_ruleActions=new RewriteRuleSubtreeStream(adaptor,"rule ruleActions");
		RewriteRuleSubtreeStream stream_alternative=new RewriteRuleSubtreeStream(adaptor,"rule alternative");
		RewriteRuleSubtreeStream stream_rewrite=new RewriteRuleSubtreeStream(adaptor,"rule rewrite");

			GrammarAST save = currentBlockAST;
			Map opts=null;

		try
		{
			// Grammars\\ANTLR.g3:385:4: ( (lp= LPAREN -> BLOCK[$lp,\"BLOCK\"] ) ( ( optionsSpec )? ( ruleActions )? COLON | ACTION COLON )? a= alternative r= rewrite ( OR a= alternative r= rewrite )* rp= RPAREN -> ^( $block ( optionsSpec )? ( ruleActions )? ( ACTION )? ( alternative )+ EOB[$rp,\"<end-of-block>\"] ) )
			// Grammars\\ANTLR.g3:385:4: (lp= LPAREN -> BLOCK[$lp,\"BLOCK\"] ) ( ( optionsSpec )? ( ruleActions )? COLON | ACTION COLON )? a= alternative r= rewrite ( OR a= alternative r= rewrite )* rp= RPAREN
			{
			// Grammars\\ANTLR.g3:385:4: (lp= LPAREN -> BLOCK[$lp,\"BLOCK\"] )
			// Grammars\\ANTLR.g3:385:6: lp= LPAREN
			{
			lp=(IToken)Match(input,LPAREN,Follow._LPAREN_in_block1361); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(lp);



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

			root_0 = (GrammarAST)adaptor.Nil();
			// 386:4: -> BLOCK[$lp,\"BLOCK\"]
			{
				adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(BLOCK, lp, "BLOCK"));

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{
				currentBlockAST = (GrammarAST)((GrammarAST)retval.tree).GetChild(0);
			}
			// Grammars\\ANTLR.g3:389:3: ( ( optionsSpec )? ( ruleActions )? COLON | ACTION COLON )?
			int alt36=3;
			int LA36_0 = input.LA(1);

			if ( (LA36_0==AMPERSAND||LA36_0==COLON||LA36_0==OPTIONS) )
			{
				alt36=1;
			}
			else if ( (LA36_0==ACTION) )
			{
				int LA36_2 = input.LA(2);

				if ( (LA36_2==COLON) )
				{
					alt36=2;
				}
			}
			switch ( alt36 )
			{
			case 1:
				// Grammars\\ANTLR.g3:393:4: ( optionsSpec )? ( ruleActions )? COLON
				{
				// Grammars\\ANTLR.g3:393:4: ( optionsSpec )?
				int alt34=2;
				int LA34_0 = input.LA(1);

				if ( (LA34_0==OPTIONS) )
				{
					alt34=1;
				}
				switch ( alt34 )
				{
				case 1:
					// Grammars\\ANTLR.g3:393:5: optionsSpec
					{
					PushFollow(Follow._optionsSpec_in_block1399);
					optionsSpec64=optionsSpec();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_optionsSpec.Add(optionsSpec64.Tree);
					if ( state.backtracking == 0 )
					{
						((GrammarAST)((GrammarAST)retval.tree).GetChild(0)).setOptions(Grammar,(optionsSpec64!=null?optionsSpec64.opts:default(IDictionary<string, object>)));
					}

					}
					break;

				}

				// Grammars\\ANTLR.g3:394:4: ( ruleActions )?
				int alt35=2;
				int LA35_0 = input.LA(1);

				if ( (LA35_0==AMPERSAND) )
				{
					alt35=1;
				}
				switch ( alt35 )
				{
				case 1:
					// Grammars\\ANTLR.g3:394:6: ruleActions
					{
					PushFollow(Follow._ruleActions_in_block1410);
					ruleActions65=ruleActions();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_ruleActions.Add(ruleActions65.Tree);

					}
					break;

				}

				COLON66=(IToken)Match(input,COLON,Follow._COLON_in_block1418); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COLON.Add(COLON66);


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:396:5: ACTION COLON
				{
				ACTION67=(IToken)Match(input,ACTION,Follow._ACTION_in_block1424); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ACTION.Add(ACTION67);

				COLON68=(IToken)Match(input,COLON,Follow._COLON_in_block1426); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COLON.Add(COLON68);


				}
				break;

			}

			PushFollow(Follow._alternative_in_block1438);
			a=alternative();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_alternative.Add(a.Tree);
			PushFollow(Follow._rewrite_in_block1442);
			r=rewrite();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite.Add(r.Tree);
			if ( state.backtracking == 0 )
			{

							stream_alternative.Add( (r!=null?((GrammarAST)r.tree):null) );
							if ( LA(1)==OR || (LA(2)==QUESTION||LA(2)==PLUS||LA(2)==STAR) )
								prefixWithSynPred((a!=null?((GrammarAST)a.tree):null));
						
			}
			// Grammars\\ANTLR.g3:405:3: ( OR a= alternative r= rewrite )*
			for ( ; ; )
			{
				int alt37=2;
				int LA37_0 = input.LA(1);

				if ( (LA37_0==OR) )
				{
					alt37=1;
				}


				switch ( alt37 )
				{
				case 1:
					// Grammars\\ANTLR.g3:405:5: OR a= alternative r= rewrite
					{
					OR69=(IToken)Match(input,OR,Follow._OR_in_block1452); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_OR.Add(OR69);

					PushFollow(Follow._alternative_in_block1456);
					a=alternative();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_alternative.Add(a.Tree);
					PushFollow(Follow._rewrite_in_block1460);
					r=rewrite();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_rewrite.Add(r.Tree);
					if ( state.backtracking == 0 )
					{

										stream_alternative.Add( (r!=null?((GrammarAST)r.tree):null) );
										if (LA(1)==OR||(LA(2)==QUESTION||LA(2)==PLUS||LA(2)==STAR))
											prefixWithSynPred((a!=null?((GrammarAST)a.tree):null));
									
					}

					}
					break;

				default:
					goto loop37;
				}
			}

			loop37:
				;


			rp=(IToken)Match(input,RPAREN,Follow._RPAREN_in_block1477); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(rp);



			{
			// AST REWRITE
			// elements: block, optionsSpec, ruleActions, ACTION, alternative
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 414:3: -> ^( $block ( optionsSpec )? ( ruleActions )? ( ACTION )? ( alternative )+ EOB[$rp,\"<end-of-block>\"] )
			{
				// Grammars\\ANTLR.g3:414:6: ^( $block ( optionsSpec )? ( ruleActions )? ( ACTION )? ( alternative )+ EOB[$rp,\"<end-of-block>\"] )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot(stream_retval.NextNode(), root_1);

				// Grammars\\ANTLR.g3:414:15: ( optionsSpec )?
				if ( stream_optionsSpec.HasNext )
				{
					adaptor.AddChild(root_1, stream_optionsSpec.NextTree());

				}
				stream_optionsSpec.Reset();
				// Grammars\\ANTLR.g3:414:28: ( ruleActions )?
				if ( stream_ruleActions.HasNext )
				{
					adaptor.AddChild(root_1, stream_ruleActions.NextTree());

				}
				stream_ruleActions.Reset();
				// Grammars\\ANTLR.g3:414:41: ( ACTION )?
				if ( stream_ACTION.HasNext )
				{
					adaptor.AddChild(root_1, stream_ACTION.NextNode());

				}
				stream_ACTION.Reset();
				if ( !(stream_alternative.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_alternative.HasNext )
				{
					adaptor.AddChild(root_1, stream_alternative.NextTree());

				}
				stream_alternative.Reset();
				adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOB, rp, "<end-of-block>"));

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
			 currentBlockAST = save; 
		}
		return retval;
	}
	// $ANTLR end "block"

	public class altList_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "altList"
	// Grammars\\ANTLR.g3:418:0: altList[IDictionary<string, object> opts] : ( -> BLOCK[$start,\"BLOCK\"] ) (a1= alternative r1= rewrite -> $a1 ( $r1)? ) ( OR a2= alternative r2= rewrite -> $altList $a2 ( $r2)? )* -> ^( $altList EOB[\"<end-of-block>\"] ) ;
	private ANTLRParser.altList_return altList( IDictionary<string, object> opts )
	{
		ANTLRParser.altList_return retval = new ANTLRParser.altList_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken OR70=null;
		ANTLRParser.alternative_return a1 = default(ANTLRParser.alternative_return);
		ANTLRParser.rewrite_return r1 = default(ANTLRParser.rewrite_return);
		ANTLRParser.alternative_return a2 = default(ANTLRParser.alternative_return);
		ANTLRParser.rewrite_return r2 = default(ANTLRParser.rewrite_return);

		GrammarAST OR70_tree=null;
		RewriteRuleITokenStream stream_OR=new RewriteRuleITokenStream(adaptor,"token OR");
		RewriteRuleSubtreeStream stream_alternative=new RewriteRuleSubtreeStream(adaptor,"rule alternative");
		RewriteRuleSubtreeStream stream_rewrite=new RewriteRuleSubtreeStream(adaptor,"rule rewrite");

			GrammarAST blkRoot = null;
			GrammarAST save = currentBlockAST;

		try
		{
			// Grammars\\ANTLR.g3:424:4: ( ( -> BLOCK[$start,\"BLOCK\"] ) (a1= alternative r1= rewrite -> $a1 ( $r1)? ) ( OR a2= alternative r2= rewrite -> $altList $a2 ( $r2)? )* -> ^( $altList EOB[\"<end-of-block>\"] ) )
			// Grammars\\ANTLR.g3:424:4: ( -> BLOCK[$start,\"BLOCK\"] ) (a1= alternative r1= rewrite -> $a1 ( $r1)? ) ( OR a2= alternative r2= rewrite -> $altList $a2 ( $r2)? )*
			{
			// Grammars\\ANTLR.g3:424:4: ( -> BLOCK[$start,\"BLOCK\"] )
			// Grammars\\ANTLR.g3:424:6: 
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

			root_0 = (GrammarAST)adaptor.Nil();
			// 424:6: -> BLOCK[$start,\"BLOCK\"]
			{
				adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(BLOCK, ((IToken)retval.start), "BLOCK"));

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{

							blkRoot = (GrammarAST)((GrammarAST)retval.tree).GetChild(0);
							blkRoot.setBlockOptions( opts );
							currentBlockAST = blkRoot;
						
			}
			// Grammars\\ANTLR.g3:430:3: (a1= alternative r1= rewrite -> $a1 ( $r1)? )
			// Grammars\\ANTLR.g3:430:5: a1= alternative r1= rewrite
			{
			PushFollow(Follow._alternative_in_altList1541);
			a1=alternative();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_alternative.Add(a1.Tree);
			PushFollow(Follow._rewrite_in_altList1545);
			r1=rewrite();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite.Add(r1.Tree);
			if ( state.backtracking == 0 )
			{
				if (LA(1)==OR||(LA(2)==QUESTION||LA(2)==PLUS||LA(2)==STAR)) prefixWithSynPred((a1!=null?((GrammarAST)a1.tree):null));
			}


			{
			// AST REWRITE
			// elements: a1, r1
			// token labels: 
			// rule labels: a1, r1, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_a1=new RewriteRuleSubtreeStream(adaptor,"rule a1",a1!=null?a1.tree:null);
			RewriteRuleSubtreeStream stream_r1=new RewriteRuleSubtreeStream(adaptor,"rule r1",r1!=null?r1.tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 432:4: -> $a1 ( $r1)?
			{
				adaptor.AddChild(root_0, stream_a1.NextTree());
				// Grammars\\ANTLR.g3:432:12: ( $r1)?
				if ( stream_r1.HasNext )
				{
					adaptor.AddChild(root_0, stream_r1.NextTree());

				}
				stream_r1.Reset();

			}

			retval.tree = root_0;
			}
			}

			}

			// Grammars\\ANTLR.g3:434:3: ( OR a2= alternative r2= rewrite -> $altList $a2 ( $r2)? )*
			for ( ; ; )
			{
				int alt38=2;
				int LA38_0 = input.LA(1);

				if ( (LA38_0==OR) )
				{
					alt38=1;
				}


				switch ( alt38 )
				{
				case 1:
					// Grammars\\ANTLR.g3:434:5: OR a2= alternative r2= rewrite
					{
					OR70=(IToken)Match(input,OR,Follow._OR_in_altList1572); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_OR.Add(OR70);

					PushFollow(Follow._alternative_in_altList1576);
					a2=alternative();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_alternative.Add(a2.Tree);
					PushFollow(Follow._rewrite_in_altList1580);
					r2=rewrite();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_rewrite.Add(r2.Tree);
					if ( state.backtracking == 0 )
					{
						if (LA(1)==OR||(LA(2)==QUESTION||LA(2)==PLUS||LA(2)==STAR)) prefixWithSynPred((a2!=null?((GrammarAST)a2.tree):null));
					}


					{
					// AST REWRITE
					// elements: altList, a2, r2
					// token labels: 
					// rule labels: a2, r2, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.tree = root_0;
					RewriteRuleSubtreeStream stream_a2=new RewriteRuleSubtreeStream(adaptor,"rule a2",a2!=null?a2.tree:null);
					RewriteRuleSubtreeStream stream_r2=new RewriteRuleSubtreeStream(adaptor,"rule r2",r2!=null?r2.tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

					root_0 = (GrammarAST)adaptor.Nil();
					// 436:4: -> $altList $a2 ( $r2)?
					{
						adaptor.AddChild(root_0, stream_retval.NextTree());
						adaptor.AddChild(root_0, stream_a2.NextTree());
						// Grammars\\ANTLR.g3:436:21: ( $r2)?
						if ( stream_r2.HasNext )
						{
							adaptor.AddChild(root_0, stream_r2.NextTree());

						}
						stream_r2.Reset();

					}

					retval.tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop38;
				}
			}

			loop38:
				;




			{
			// AST REWRITE
			// elements: altList
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 438:3: -> ^( $altList EOB[\"<end-of-block>\"] )
			{
				// Grammars\\ANTLR.g3:438:6: ^( $altList EOB[\"<end-of-block>\"] )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot(blkRoot, root_1);

				adaptor.AddChild(root_1, stream_retval.NextTree());
				adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOB, "<end-of-block>"));

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
			 currentBlockAST = save; 
		}
		return retval;
	}
	// $ANTLR end "altList"

	public class alternative_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "alternative"
	// Grammars\\ANTLR.g3:442:0: alternative : ( -> EOA[\"<end-of-alt>\"] ) ( -> ALT[$start,\"ALT\"] ) ( (el+= element )+ -> ^( ( $el)+ ) | -> ^( EPSILON[$start,\"epsilon\"] ) ) ;
	private ANTLRParser.alternative_return alternative(  )
	{
		ANTLRParser.alternative_return retval = new ANTLRParser.alternative_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		List list_el=null;
		ANTLRParser.element_return el = default(ANTLRParser.element_return);
		RewriteRuleSubtreeStream stream_element=new RewriteRuleSubtreeStream(adaptor,"rule element");

			GrammarAST eoa = null;
			GrammarAST altRoot = null;

		try
		{
			// Grammars\\ANTLR.g3:448:4: ( ( -> EOA[\"<end-of-alt>\"] ) ( -> ALT[$start,\"ALT\"] ) ( (el+= element )+ -> ^( ( $el)+ ) | -> ^( EPSILON[$start,\"epsilon\"] ) ) )
			// Grammars\\ANTLR.g3:448:4: ( -> EOA[\"<end-of-alt>\"] ) ( -> ALT[$start,\"ALT\"] ) ( (el+= element )+ -> ^( ( $el)+ ) | -> ^( EPSILON[$start,\"epsilon\"] ) )
			{
			// Grammars\\ANTLR.g3:448:4: ( -> EOA[\"<end-of-alt>\"] )
			// Grammars\\ANTLR.g3:448:6: 
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

			root_0 = (GrammarAST)adaptor.Nil();
			// 448:6: -> EOA[\"<end-of-alt>\"]
			{
				adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(EOA, "<end-of-alt>"));

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{
				 eoa = ((GrammarAST)retval.tree); 
			}
			// Grammars\\ANTLR.g3:450:3: ( -> ALT[$start,\"ALT\"] )
			// Grammars\\ANTLR.g3:450:5: 
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

			root_0 = (GrammarAST)adaptor.Nil();
			// 450:5: -> ALT[$start,\"ALT\"]
			{
				adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ALT, ((IToken)retval.start), "ALT"));

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{
				 altRoot = ((GrammarAST)retval.tree); 
			}
			// Grammars\\ANTLR.g3:453:3: ( (el+= element )+ -> ^( ( $el)+ ) | -> ^( EPSILON[$start,\"epsilon\"] ) )
			int alt40=2;
			int LA40_0 = input.LA(1);

			if ( (LA40_0==ACTION||LA40_0==CHAR_LITERAL||LA40_0==FORCED_ACTION||LA40_0==LPAREN||LA40_0==NOT||LA40_0==RULE_REF||LA40_0==SEMPRED||LA40_0==STRING_LITERAL||LA40_0==TOKEN_REF||LA40_0==TREE_BEGIN||LA40_0==WILDCARD) )
			{
				alt40=1;
			}
			else if ( (LA40_0==OR||LA40_0==REWRITE||LA40_0==RPAREN||LA40_0==SEMI) )
			{
				alt40=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 40, 0, input);

				throw nvae;
			}
			switch ( alt40 )
			{
			case 1:
				// Grammars\\ANTLR.g3:453:5: (el+= element )+
				{
				// Grammars\\ANTLR.g3:453:5: (el+= element )+
				int cnt39=0;
				for ( ; ; )
				{
					int alt39=2;
					int LA39_0 = input.LA(1);

					if ( (LA39_0==ACTION||LA39_0==CHAR_LITERAL||LA39_0==FORCED_ACTION||LA39_0==LPAREN||LA39_0==NOT||LA39_0==RULE_REF||LA39_0==SEMPRED||LA39_0==STRING_LITERAL||LA39_0==TOKEN_REF||LA39_0==TREE_BEGIN||LA39_0==WILDCARD) )
					{
						alt39=1;
					}


					switch ( alt39 )
					{
					case 1:
						// Grammars\\ANTLR.g3:453:7: el+= element
						{
						PushFollow(Follow._element_in_alternative1676);
						el=element();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_element.Add(el.Tree);
						if (list_el==null) list_el=new ArrayList();
						list_el.Add(el.Tree);


						}
						break;

					default:
						if ( cnt39 >= 1 )
							goto loop39;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee39 = new EarlyExitException( 39, input );
						throw eee39;
					}
					cnt39++;
				}
				loop39:
					;




				{
				// AST REWRITE
				// elements: el
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: el
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);
				RewriteRuleSubtreeStream stream_el=new RewriteRuleSubtreeStream(adaptor,"token el",list_el);
				root_0 = (GrammarAST)adaptor.Nil();
				// 454:4: -> ^( ( $el)+ )
				{
					// Grammars\\ANTLR.g3:454:7: ^( ( $el)+ )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(altRoot, root_1);

					if ( !(stream_el.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_el.HasNext )
					{
						adaptor.AddChild(root_1, stream_el.NextTree());

					}
					stream_el.Reset();
					adaptor.AddChild(root_1, eoa);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:456:4: 
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

				root_0 = (GrammarAST)adaptor.Nil();
				// 456:4: -> ^( EPSILON[$start,\"epsilon\"] )
				{
					// Grammars\\ANTLR.g3:456:7: ^( EPSILON[$start,\"epsilon\"] )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(altRoot, root_1);

					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EPSILON, ((IToken)retval.start), "epsilon"));
					adaptor.AddChild(root_1, eoa);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "alternative"

	public class exceptionGroup_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "exceptionGroup"
	// Grammars\\ANTLR.g3:460:0: exceptionGroup : ( ( exceptionHandler )+ ( finallyClause )? | finallyClause );
	private ANTLRParser.exceptionGroup_return exceptionGroup(  )
	{
		ANTLRParser.exceptionGroup_return retval = new ANTLRParser.exceptionGroup_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.exceptionHandler_return exceptionHandler71 = default(ANTLRParser.exceptionHandler_return);
		ANTLRParser.finallyClause_return finallyClause72 = default(ANTLRParser.finallyClause_return);
		ANTLRParser.finallyClause_return finallyClause73 = default(ANTLRParser.finallyClause_return);


		try
		{
			// Grammars\\ANTLR.g3:461:4: ( ( exceptionHandler )+ ( finallyClause )? | finallyClause )
			int alt43=2;
			int LA43_0 = input.LA(1);

			if ( (LA43_0==CATCH) )
			{
				alt43=1;
			}
			else if ( (LA43_0==FINALLY) )
			{
				alt43=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 43, 0, input);

				throw nvae;
			}
			switch ( alt43 )
			{
			case 1:
				// Grammars\\ANTLR.g3:461:4: ( exceptionHandler )+ ( finallyClause )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				// Grammars\\ANTLR.g3:461:4: ( exceptionHandler )+
				int cnt41=0;
				for ( ; ; )
				{
					int alt41=2;
					int LA41_0 = input.LA(1);

					if ( (LA41_0==CATCH) )
					{
						alt41=1;
					}


					switch ( alt41 )
					{
					case 1:
						// Grammars\\ANTLR.g3:461:6: exceptionHandler
						{
						PushFollow(Follow._exceptionHandler_in_exceptionGroup1729);
						exceptionHandler71=exceptionHandler();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, exceptionHandler71.Tree);

						}
						break;

					default:
						if ( cnt41 >= 1 )
							goto loop41;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee41 = new EarlyExitException( 41, input );
						throw eee41;
					}
					cnt41++;
				}
				loop41:
					;


				// Grammars\\ANTLR.g3:461:26: ( finallyClause )?
				int alt42=2;
				int LA42_0 = input.LA(1);

				if ( (LA42_0==FINALLY) )
				{
					alt42=1;
				}
				switch ( alt42 )
				{
				case 1:
					// Grammars\\ANTLR.g3:461:28: finallyClause
					{
					PushFollow(Follow._finallyClause_in_exceptionGroup1736);
					finallyClause72=finallyClause();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, finallyClause72.Tree);

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:462:4: finallyClause
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._finallyClause_in_exceptionGroup1744);
				finallyClause73=finallyClause();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, finallyClause73.Tree);

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "exceptionGroup"

	public class exceptionHandler_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "exceptionHandler"
	// Grammars\\ANTLR.g3:465:0: exceptionHandler : 'catch' ARG_ACTION ACTION ;
	private ANTLRParser.exceptionHandler_return exceptionHandler(  )
	{
		ANTLRParser.exceptionHandler_return retval = new ANTLRParser.exceptionHandler_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal74=null;
		IToken ARG_ACTION75=null;
		IToken ACTION76=null;

		GrammarAST string_literal74_tree=null;
		GrammarAST ARG_ACTION75_tree=null;
		GrammarAST ACTION76_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:466:4: ( 'catch' ARG_ACTION ACTION )
			// Grammars\\ANTLR.g3:466:4: 'catch' ARG_ACTION ACTION
			{
			root_0 = (GrammarAST)adaptor.Nil();

			string_literal74=(IToken)Match(input,CATCH,Follow._CATCH_in_exceptionHandler1755); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			string_literal74_tree = (GrammarAST)adaptor.Create(string_literal74);
			root_0 = (GrammarAST)adaptor.BecomeRoot(string_literal74_tree, root_0);
			}
			ARG_ACTION75=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_exceptionHandler1758); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ARG_ACTION75_tree = (GrammarAST)adaptor.Create(ARG_ACTION75);
			adaptor.AddChild(root_0, ARG_ACTION75_tree);
			}
			ACTION76=(IToken)Match(input,ACTION,Follow._ACTION_in_exceptionHandler1760); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ACTION76_tree = (GrammarAST)adaptor.Create(ACTION76);
			adaptor.AddChild(root_0, ACTION76_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "exceptionHandler"

	public class finallyClause_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "finallyClause"
	// Grammars\\ANTLR.g3:469:0: finallyClause : 'finally' ACTION ;
	private ANTLRParser.finallyClause_return finallyClause(  )
	{
		ANTLRParser.finallyClause_return retval = new ANTLRParser.finallyClause_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken string_literal77=null;
		IToken ACTION78=null;

		GrammarAST string_literal77_tree=null;
		GrammarAST ACTION78_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:470:4: ( 'finally' ACTION )
			// Grammars\\ANTLR.g3:470:4: 'finally' ACTION
			{
			root_0 = (GrammarAST)adaptor.Nil();

			string_literal77=(IToken)Match(input,FINALLY,Follow._FINALLY_in_finallyClause1771); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			string_literal77_tree = (GrammarAST)adaptor.Create(string_literal77);
			root_0 = (GrammarAST)adaptor.BecomeRoot(string_literal77_tree, root_0);
			}
			ACTION78=(IToken)Match(input,ACTION,Follow._ACTION_in_finallyClause1774); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ACTION78_tree = (GrammarAST)adaptor.Create(ACTION78);
			adaptor.AddChild(root_0, ACTION78_tree);
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "finallyClause"

	public class element_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "element"
	// Grammars\\ANTLR.g3:473:0: element : elementNoOptionSpec ;
	private ANTLRParser.element_return element(  )
	{
		ANTLRParser.element_return retval = new ANTLRParser.element_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.elementNoOptionSpec_return elementNoOptionSpec79 = default(ANTLRParser.elementNoOptionSpec_return);


		try
		{
			// Grammars\\ANTLR.g3:474:4: ( elementNoOptionSpec )
			// Grammars\\ANTLR.g3:474:4: elementNoOptionSpec
			{
			root_0 = (GrammarAST)adaptor.Nil();

			PushFollow(Follow._elementNoOptionSpec_in_element1785);
			elementNoOptionSpec79=elementNoOptionSpec();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, elementNoOptionSpec79.Tree);

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "element"

	public class elementNoOptionSpec_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "elementNoOptionSpec"
	// Grammars\\ANTLR.g3:477:0: elementNoOptionSpec : ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )? |a= atom (sub2= ebnfSuffix[$a.tree,false] )? | ebnf | FORCED_ACTION | ACTION |p= SEMPRED ( IMPLIES )? |t3= tree_ ) ;
	private ANTLRParser.elementNoOptionSpec_return elementNoOptionSpec(  )
	{
		ANTLRParser.elementNoOptionSpec_return retval = new ANTLRParser.elementNoOptionSpec_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken p=null;
		IToken ASSIGN81=null;
		IToken PLUS_ASSIGN82=null;
		IToken FORCED_ACTION86=null;
		IToken ACTION87=null;
		IToken IMPLIES88=null;
		ANTLRParser.ebnfSuffix_return sub = default(ANTLRParser.ebnfSuffix_return);
		ANTLRParser.atom_return a = default(ANTLRParser.atom_return);
		ANTLRParser.ebnfSuffix_return sub2 = default(ANTLRParser.ebnfSuffix_return);
		ANTLRParser.tree__return t3 = default(ANTLRParser.tree__return);
		ANTLRParser.id_return id80 = default(ANTLRParser.id_return);
		ANTLRParser.atom_return atom83 = default(ANTLRParser.atom_return);
		ANTLRParser.block_return block84 = default(ANTLRParser.block_return);
		ANTLRParser.ebnf_return ebnf85 = default(ANTLRParser.ebnf_return);

		GrammarAST p_tree=null;
		GrammarAST ASSIGN81_tree=null;
		GrammarAST PLUS_ASSIGN82_tree=null;
		GrammarAST FORCED_ACTION86_tree=null;
		GrammarAST ACTION87_tree=null;
		GrammarAST IMPLIES88_tree=null;


			IntSet elements=null;

		try
		{
			// Grammars\\ANTLR.g3:482:4: ( ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )? |a= atom (sub2= ebnfSuffix[$a.tree,false] )? | ebnf | FORCED_ACTION | ACTION |p= SEMPRED ( IMPLIES )? |t3= tree_ ) )
			// Grammars\\ANTLR.g3:482:4: ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )? |a= atom (sub2= ebnfSuffix[$a.tree,false] )? | ebnf | FORCED_ACTION | ACTION |p= SEMPRED ( IMPLIES )? |t3= tree_ )
			{
			root_0 = (GrammarAST)adaptor.Nil();

			// Grammars\\ANTLR.g3:482:4: ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )? |a= atom (sub2= ebnfSuffix[$a.tree,false] )? | ebnf | FORCED_ACTION | ACTION |p= SEMPRED ( IMPLIES )? |t3= tree_ )
			int alt49=7;
			alt49 = dfa49.Predict(input);
			switch ( alt49 )
			{
			case 1:
				// Grammars\\ANTLR.g3:482:6: ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )?
				{
				// Grammars\\ANTLR.g3:482:6: ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) )
				// Grammars\\ANTLR.g3:482:8: id ( ASSIGN | PLUS_ASSIGN ) ( atom | block )
				{
				PushFollow(Follow._id_in_elementNoOptionSpec1805);
				id80=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id80.Tree);
				// Grammars\\ANTLR.g3:482:11: ( ASSIGN | PLUS_ASSIGN )
				int alt44=2;
				int LA44_0 = input.LA(1);

				if ( (LA44_0==ASSIGN) )
				{
					alt44=1;
				}
				else if ( (LA44_0==PLUS_ASSIGN) )
				{
					alt44=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 44, 0, input);

					throw nvae;
				}
				switch ( alt44 )
				{
				case 1:
					// Grammars\\ANTLR.g3:482:12: ASSIGN
					{
					ASSIGN81=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_elementNoOptionSpec1808); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ASSIGN81_tree = (GrammarAST)adaptor.Create(ASSIGN81);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN81_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:482:20: PLUS_ASSIGN
					{
					PLUS_ASSIGN82=(IToken)Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_elementNoOptionSpec1811); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					PLUS_ASSIGN82_tree = (GrammarAST)adaptor.Create(PLUS_ASSIGN82);
					root_0 = (GrammarAST)adaptor.BecomeRoot(PLUS_ASSIGN82_tree, root_0);
					}

					}
					break;

				}

				// Grammars\\ANTLR.g3:482:34: ( atom | block )
				int alt45=2;
				int LA45_0 = input.LA(1);

				if ( (LA45_0==CHAR_LITERAL||LA45_0==NOT||LA45_0==RULE_REF||LA45_0==STRING_LITERAL||LA45_0==TOKEN_REF||LA45_0==WILDCARD) )
				{
					alt45=1;
				}
				else if ( (LA45_0==LPAREN) )
				{
					alt45=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 45, 0, input);

					throw nvae;
				}
				switch ( alt45 )
				{
				case 1:
					// Grammars\\ANTLR.g3:482:35: atom
					{
					PushFollow(Follow._atom_in_elementNoOptionSpec1816);
					atom83=atom();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, atom83.Tree);

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:482:40: block
					{
					PushFollow(Follow._block_in_elementNoOptionSpec1818);
					block84=block();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, block84.Tree);

					}
					break;

				}


				}

				// Grammars\\ANTLR.g3:484:4: (sub= ebnfSuffix[root_0,false] )?
				int alt46=2;
				int LA46_0 = input.LA(1);

				if ( (LA46_0==PLUS||LA46_0==QUESTION||LA46_0==STAR) )
				{
					alt46=1;
				}
				switch ( alt46 )
				{
				case 1:
					// Grammars\\ANTLR.g3:484:6: sub= ebnfSuffix[root_0,false]
					{
					PushFollow(Follow._ebnfSuffix_in_elementNoOptionSpec1833);
					sub=ebnfSuffix(root_0, false);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{
						root_0 = (sub!=null?((GrammarAST)sub.tree):null);
					}

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:486:5: a= atom (sub2= ebnfSuffix[$a.tree,false] )?
				{
				PushFollow(Follow._atom_in_elementNoOptionSpec1851);
				a=atom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, a.Tree);
				// Grammars\\ANTLR.g3:487:4: (sub2= ebnfSuffix[$a.tree,false] )?
				int alt47=2;
				int LA47_0 = input.LA(1);

				if ( (LA47_0==PLUS||LA47_0==QUESTION||LA47_0==STAR) )
				{
					alt47=1;
				}
				switch ( alt47 )
				{
				case 1:
					// Grammars\\ANTLR.g3:487:6: sub2= ebnfSuffix[$a.tree,false]
					{
					PushFollow(Follow._ebnfSuffix_in_elementNoOptionSpec1860);
					sub2=ebnfSuffix((a!=null?((GrammarAST)a.tree):null), false);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{
						root_0=(sub2!=null?((GrammarAST)sub2.tree):null);
					}

					}
					break;

				}


				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:489:5: ebnf
				{
				PushFollow(Follow._ebnf_in_elementNoOptionSpec1876);
				ebnf85=ebnf();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ebnf85.Tree);

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:490:5: FORCED_ACTION
				{
				FORCED_ACTION86=(IToken)Match(input,FORCED_ACTION,Follow._FORCED_ACTION_in_elementNoOptionSpec1882); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				FORCED_ACTION86_tree = (GrammarAST)adaptor.Create(FORCED_ACTION86);
				adaptor.AddChild(root_0, FORCED_ACTION86_tree);
				}

				}
				break;
			case 5:
				// Grammars\\ANTLR.g3:491:5: ACTION
				{
				ACTION87=(IToken)Match(input,ACTION,Follow._ACTION_in_elementNoOptionSpec1888); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ACTION87_tree = (GrammarAST)adaptor.Create(ACTION87);
				adaptor.AddChild(root_0, ACTION87_tree);
				}

				}
				break;
			case 6:
				// Grammars\\ANTLR.g3:492:5: p= SEMPRED ( IMPLIES )?
				{
				p=(IToken)Match(input,SEMPRED,Follow._SEMPRED_in_elementNoOptionSpec1896); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				p_tree = (GrammarAST)adaptor.Create(p);
				adaptor.AddChild(root_0, p_tree);
				}
				// Grammars\\ANTLR.g3:492:15: ( IMPLIES )?
				int alt48=2;
				int LA48_0 = input.LA(1);

				if ( (LA48_0==IMPLIES) )
				{
					alt48=1;
				}
				switch ( alt48 )
				{
				case 1:
					// Grammars\\ANTLR.g3:492:17: IMPLIES
					{
					IMPLIES88=(IToken)Match(input,IMPLIES,Follow._IMPLIES_in_elementNoOptionSpec1900); if (state.failed) return retval;
					if ( state.backtracking == 0 )
					{
						p.Type = GATED_SEMPRED;
					}

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

								Grammar.blocksWithSemPreds.Add(currentBlockAST);
								
				}

				}
				break;
			case 7:
				// Grammars\\ANTLR.g3:496:5: t3= tree_
				{
				PushFollow(Follow._tree__in_elementNoOptionSpec1919);
				t3=tree_();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, t3.Tree);

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "elementNoOptionSpec"

	public class atom_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "atom"
	// Grammars\\ANTLR.g3:500:0: atom : ( range ( ROOT | BANG )? | (=> id w= WILDCARD ( terminal | ruleref ) | terminal | ruleref ) | notSet ( ROOT | BANG )? );
	private ANTLRParser.atom_return atom(  )
	{
		ANTLRParser.atom_return retval = new ANTLRParser.atom_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken w=null;
		IToken ROOT90=null;
		IToken BANG91=null;
		IToken ROOT98=null;
		IToken BANG99=null;
		ANTLRParser.range_return range89 = default(ANTLRParser.range_return);
		ANTLRParser.id_return id92 = default(ANTLRParser.id_return);
		ANTLRParser.terminal_return terminal93 = default(ANTLRParser.terminal_return);
		ANTLRParser.ruleref_return ruleref94 = default(ANTLRParser.ruleref_return);
		ANTLRParser.terminal_return terminal95 = default(ANTLRParser.terminal_return);
		ANTLRParser.ruleref_return ruleref96 = default(ANTLRParser.ruleref_return);
		ANTLRParser.notSet_return notSet97 = default(ANTLRParser.notSet_return);

		GrammarAST w_tree=null;
		GrammarAST ROOT90_tree=null;
		GrammarAST BANG91_tree=null;
		GrammarAST ROOT98_tree=null;
		GrammarAST BANG99_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:501:4: ( range ( ROOT | BANG )? | (=> id w= WILDCARD ( terminal | ruleref ) | terminal | ruleref ) | notSet ( ROOT | BANG )? )
			int alt54=3;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				int LA54_1 = input.LA(2);

				if ( (LA54_1==RANGE) )
				{
					alt54=1;
				}
				else if ( (LA54_1==ACTION||LA54_1==BANG||LA54_1==CHAR_LITERAL||LA54_1==FORCED_ACTION||LA54_1==LPAREN||(LA54_1>=NOT && LA54_1<=OPEN_ELEMENT_OPTION)||LA54_1==OR||LA54_1==PLUS||LA54_1==QUESTION||(LA54_1>=REWRITE && LA54_1<=RPAREN)||LA54_1==RULE_REF||(LA54_1>=SEMI && LA54_1<=SEMPRED)||LA54_1==STAR||LA54_1==STRING_LITERAL||LA54_1==TOKEN_REF||LA54_1==TREE_BEGIN||LA54_1==WILDCARD) )
				{
					alt54=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 54, 1, input);

					throw nvae;
				}
				}
				break;
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
			case WILDCARD:
				{
				alt54=2;
				}
				break;
			case NOT:
				{
				alt54=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 54, 0, input);

					throw nvae;
				}
			}

			switch ( alt54 )
			{
			case 1:
				// Grammars\\ANTLR.g3:501:4: range ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._range_in_atom1934);
				range89=range();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, range89.Tree);
				// Grammars\\ANTLR.g3:501:10: ( ROOT | BANG )?
				int alt50=3;
				int LA50_0 = input.LA(1);

				if ( (LA50_0==ROOT) )
				{
					alt50=1;
				}
				else if ( (LA50_0==BANG) )
				{
					alt50=2;
				}
				switch ( alt50 )
				{
				case 1:
					// Grammars\\ANTLR.g3:501:11: ROOT
					{
					ROOT90=(IToken)Match(input,ROOT,Follow._ROOT_in_atom1937); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT90_tree = (GrammarAST)adaptor.Create(ROOT90);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT90_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:501:17: BANG
					{
					BANG91=(IToken)Match(input,BANG,Follow._BANG_in_atom1940); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG91_tree = (GrammarAST)adaptor.Create(BANG91);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG91_tree, root_0);
					}

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:502:4: (=> id w= WILDCARD ( terminal | ruleref ) | terminal | ruleref )
				{
				root_0 = (GrammarAST)adaptor.Nil();

				// Grammars\\ANTLR.g3:502:4: (=> id w= WILDCARD ( terminal | ruleref ) | terminal | ruleref )
				int alt52=3;
				alt52 = dfa52.Predict(input);
				switch ( alt52 )
				{
				case 1:
					// Grammars\\ANTLR.g3:505:4: => id w= WILDCARD ( terminal | ruleref )
					{

					PushFollow(Follow._id_in_atom1980);
					id92=id();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id92.Tree);
					w=(IToken)Match(input,WILDCARD,Follow._WILDCARD_in_atom1984); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					w_tree = (GrammarAST)adaptor.Create(w);
					root_0 = (GrammarAST)adaptor.BecomeRoot(w_tree, root_0);
					}
					// Grammars\\ANTLR.g3:507:19: ( terminal | ruleref )
					int alt51=2;
					int LA51_0 = input.LA(1);

					if ( (LA51_0==CHAR_LITERAL||LA51_0==STRING_LITERAL||LA51_0==TOKEN_REF||LA51_0==WILDCARD) )
					{
						alt51=1;
					}
					else if ( (LA51_0==RULE_REF) )
					{
						alt51=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 51, 0, input);

						throw nvae;
					}
					switch ( alt51 )
					{
					case 1:
						// Grammars\\ANTLR.g3:507:20: terminal
						{
						PushFollow(Follow._terminal_in_atom1988);
						terminal93=terminal();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, terminal93.Tree);

						}
						break;
					case 2:
						// Grammars\\ANTLR.g3:507:29: ruleref
						{
						PushFollow(Follow._ruleref_in_atom1990);
						ruleref94=ruleref();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ruleref94.Tree);

						}
						break;

					}

					if ( state.backtracking == 0 )
					{
						w.Type = DOT;
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:508:5: terminal
					{
					PushFollow(Follow._terminal_in_atom1999);
					terminal95=terminal();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, terminal95.Tree);

					}
					break;
				case 3:
					// Grammars\\ANTLR.g3:509:5: ruleref
					{
					PushFollow(Follow._ruleref_in_atom2005);
					ruleref96=ruleref();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ruleref96.Tree);

					}
					break;

				}


				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:511:4: notSet ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._notSet_in_atom2014);
				notSet97=notSet();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, notSet97.Tree);
				// Grammars\\ANTLR.g3:511:11: ( ROOT | BANG )?
				int alt53=3;
				int LA53_0 = input.LA(1);

				if ( (LA53_0==ROOT) )
				{
					alt53=1;
				}
				else if ( (LA53_0==BANG) )
				{
					alt53=2;
				}
				switch ( alt53 )
				{
				case 1:
					// Grammars\\ANTLR.g3:511:12: ROOT
					{
					ROOT98=(IToken)Match(input,ROOT,Follow._ROOT_in_atom2017); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT98_tree = (GrammarAST)adaptor.Create(ROOT98);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT98_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:511:18: BANG
					{
					BANG99=(IToken)Match(input,BANG,Follow._BANG_in_atom2020); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG99_tree = (GrammarAST)adaptor.Create(BANG99);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG99_tree, root_0);
					}

					}
					break;

				}


				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "atom"

	public class ruleref_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ruleref"
	// Grammars\\ANTLR.g3:514:0: ruleref : rr= RULE_REF ( ARG_ACTION )? ( ROOT | BANG )? ;
	private ANTLRParser.ruleref_return ruleref(  )
	{
		ANTLRParser.ruleref_return retval = new ANTLRParser.ruleref_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken rr=null;
		IToken ARG_ACTION100=null;
		IToken ROOT101=null;
		IToken BANG102=null;

		GrammarAST rr_tree=null;
		GrammarAST ARG_ACTION100_tree=null;
		GrammarAST ROOT101_tree=null;
		GrammarAST BANG102_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:515:4: (rr= RULE_REF ( ARG_ACTION )? ( ROOT | BANG )? )
			// Grammars\\ANTLR.g3:515:4: rr= RULE_REF ( ARG_ACTION )? ( ROOT | BANG )?
			{
			root_0 = (GrammarAST)adaptor.Nil();

			rr=(IToken)Match(input,RULE_REF,Follow._RULE_REF_in_ruleref2036); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			rr_tree = (GrammarAST)adaptor.Create(rr);
			root_0 = (GrammarAST)adaptor.BecomeRoot(rr_tree, root_0);
			}
			// Grammars\\ANTLR.g3:515:17: ( ARG_ACTION )?
			int alt55=2;
			int LA55_0 = input.LA(1);

			if ( (LA55_0==ARG_ACTION) )
			{
				alt55=1;
			}
			switch ( alt55 )
			{
			case 1:
				// Grammars\\ANTLR.g3:515:19: ARG_ACTION
				{
				ARG_ACTION100=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_ruleref2041); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ARG_ACTION100_tree = (GrammarAST)adaptor.Create(ARG_ACTION100);
				adaptor.AddChild(root_0, ARG_ACTION100_tree);
				}

				}
				break;

			}

			// Grammars\\ANTLR.g3:515:33: ( ROOT | BANG )?
			int alt56=3;
			int LA56_0 = input.LA(1);

			if ( (LA56_0==ROOT) )
			{
				alt56=1;
			}
			else if ( (LA56_0==BANG) )
			{
				alt56=2;
			}
			switch ( alt56 )
			{
			case 1:
				// Grammars\\ANTLR.g3:515:34: ROOT
				{
				ROOT101=(IToken)Match(input,ROOT,Follow._ROOT_in_ruleref2047); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				ROOT101_tree = (GrammarAST)adaptor.Create(ROOT101);
				root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT101_tree, root_0);
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:515:40: BANG
				{
				BANG102=(IToken)Match(input,BANG,Follow._BANG_in_ruleref2050); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				BANG102_tree = (GrammarAST)adaptor.Create(BANG102);
				root_0 = (GrammarAST)adaptor.BecomeRoot(BANG102_tree, root_0);
				}

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ruleref"

	public class notSet_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "notSet"
	// Grammars\\ANTLR.g3:518:0: notSet : NOT ( notTerminal | block ) ;
	private ANTLRParser.notSet_return notSet(  )
	{
		ANTLRParser.notSet_return retval = new ANTLRParser.notSet_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken NOT103=null;
		ANTLRParser.notTerminal_return notTerminal104 = default(ANTLRParser.notTerminal_return);
		ANTLRParser.block_return block105 = default(ANTLRParser.block_return);

		GrammarAST NOT103_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:519:4: ( NOT ( notTerminal | block ) )
			// Grammars\\ANTLR.g3:519:4: NOT ( notTerminal | block )
			{
			root_0 = (GrammarAST)adaptor.Nil();

			NOT103=(IToken)Match(input,NOT,Follow._NOT_in_notSet2064); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			NOT103_tree = (GrammarAST)adaptor.Create(NOT103);
			root_0 = (GrammarAST)adaptor.BecomeRoot(NOT103_tree, root_0);
			}
			// Grammars\\ANTLR.g3:520:3: ( notTerminal | block )
			int alt57=2;
			int LA57_0 = input.LA(1);

			if ( (LA57_0==CHAR_LITERAL||LA57_0==STRING_LITERAL||LA57_0==TOKEN_REF) )
			{
				alt57=1;
			}
			else if ( (LA57_0==LPAREN) )
			{
				alt57=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 57, 0, input);

				throw nvae;
			}
			switch ( alt57 )
			{
			case 1:
				// Grammars\\ANTLR.g3:520:5: notTerminal
				{
				PushFollow(Follow._notTerminal_in_notSet2071);
				notTerminal104=notTerminal();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, notTerminal104.Tree);

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:521:5: block
				{
				PushFollow(Follow._block_in_notSet2077);
				block105=block();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, block105.Tree);

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "notSet"

	public class treeRoot_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "treeRoot"
	// Grammars\\ANTLR.g3:525:0: treeRoot : ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) | atom | block ) ;
	private ANTLRParser.treeRoot_return treeRoot(  )
	{
		ANTLRParser.treeRoot_return retval = new ANTLRParser.treeRoot_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken ASSIGN107=null;
		IToken PLUS_ASSIGN108=null;
		ANTLRParser.id_return id106 = default(ANTLRParser.id_return);
		ANTLRParser.atom_return atom109 = default(ANTLRParser.atom_return);
		ANTLRParser.block_return block110 = default(ANTLRParser.block_return);
		ANTLRParser.atom_return atom111 = default(ANTLRParser.atom_return);
		ANTLRParser.block_return block112 = default(ANTLRParser.block_return);

		GrammarAST ASSIGN107_tree=null;
		GrammarAST PLUS_ASSIGN108_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:526:4: ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) | atom | block ) )
			// Grammars\\ANTLR.g3:526:4: ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) | atom | block )
			{
			root_0 = (GrammarAST)adaptor.Nil();

			if ( state.backtracking == 0 )
			{
				atTreeRoot=true;
			}
			// Grammars\\ANTLR.g3:527:3: ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) | atom | block )
			int alt60=3;
			switch ( input.LA(1) )
			{
			case TOKEN_REF:
				{
				int LA60_1 = input.LA(2);

				if ( (LA60_1==ASSIGN||LA60_1==PLUS_ASSIGN) )
				{
					alt60=1;
				}
				else if ( (LA60_1==ACTION||LA60_1==ARG_ACTION||LA60_1==BANG||LA60_1==CHAR_LITERAL||LA60_1==FORCED_ACTION||LA60_1==LPAREN||(LA60_1>=NOT && LA60_1<=OPEN_ELEMENT_OPTION)||LA60_1==ROOT||LA60_1==RULE_REF||LA60_1==SEMPRED||LA60_1==STRING_LITERAL||LA60_1==TOKEN_REF||LA60_1==TREE_BEGIN||LA60_1==WILDCARD) )
				{
					alt60=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 60, 1, input);

					throw nvae;
				}
				}
				break;
			case RULE_REF:
				{
				int LA60_2 = input.LA(2);

				if ( (LA60_2==ASSIGN||LA60_2==PLUS_ASSIGN) )
				{
					alt60=1;
				}
				else if ( (LA60_2==ACTION||LA60_2==ARG_ACTION||LA60_2==BANG||LA60_2==CHAR_LITERAL||LA60_2==FORCED_ACTION||LA60_2==LPAREN||LA60_2==NOT||LA60_2==ROOT||LA60_2==RULE_REF||LA60_2==SEMPRED||LA60_2==STRING_LITERAL||LA60_2==TOKEN_REF||LA60_2==TREE_BEGIN||LA60_2==WILDCARD) )
				{
					alt60=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 60, 2, input);

					throw nvae;
				}
				}
				break;
			case CHAR_LITERAL:
			case NOT:
			case STRING_LITERAL:
			case WILDCARD:
				{
				alt60=2;
				}
				break;
			case LPAREN:
				{
				alt60=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 60, 0, input);

					throw nvae;
				}
			}

			switch ( alt60 )
			{
			case 1:
				// Grammars\\ANTLR.g3:527:5: id ( ASSIGN | PLUS_ASSIGN ) ( atom | block )
				{
				PushFollow(Follow._id_in_treeRoot2098);
				id106=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id106.Tree);
				// Grammars\\ANTLR.g3:527:8: ( ASSIGN | PLUS_ASSIGN )
				int alt58=2;
				int LA58_0 = input.LA(1);

				if ( (LA58_0==ASSIGN) )
				{
					alt58=1;
				}
				else if ( (LA58_0==PLUS_ASSIGN) )
				{
					alt58=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 58, 0, input);

					throw nvae;
				}
				switch ( alt58 )
				{
				case 1:
					// Grammars\\ANTLR.g3:527:9: ASSIGN
					{
					ASSIGN107=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_treeRoot2101); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ASSIGN107_tree = (GrammarAST)adaptor.Create(ASSIGN107);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN107_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:527:17: PLUS_ASSIGN
					{
					PLUS_ASSIGN108=(IToken)Match(input,PLUS_ASSIGN,Follow._PLUS_ASSIGN_in_treeRoot2104); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					PLUS_ASSIGN108_tree = (GrammarAST)adaptor.Create(PLUS_ASSIGN108);
					root_0 = (GrammarAST)adaptor.BecomeRoot(PLUS_ASSIGN108_tree, root_0);
					}

					}
					break;

				}

				// Grammars\\ANTLR.g3:527:31: ( atom | block )
				int alt59=2;
				int LA59_0 = input.LA(1);

				if ( (LA59_0==CHAR_LITERAL||LA59_0==NOT||LA59_0==RULE_REF||LA59_0==STRING_LITERAL||LA59_0==TOKEN_REF||LA59_0==WILDCARD) )
				{
					alt59=1;
				}
				else if ( (LA59_0==LPAREN) )
				{
					alt59=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 59, 0, input);

					throw nvae;
				}
				switch ( alt59 )
				{
				case 1:
					// Grammars\\ANTLR.g3:527:32: atom
					{
					PushFollow(Follow._atom_in_treeRoot2109);
					atom109=atom();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, atom109.Tree);

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:527:37: block
					{
					PushFollow(Follow._block_in_treeRoot2111);
					block110=block();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, block110.Tree);

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:528:5: atom
				{
				PushFollow(Follow._atom_in_treeRoot2118);
				atom111=atom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, atom111.Tree);

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:529:5: block
				{
				PushFollow(Follow._block_in_treeRoot2124);
				block112=block();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, block112.Tree);

				}
				break;

			}

			if ( state.backtracking == 0 )
			{
				atTreeRoot=false;
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "treeRoot"

	public class tree__return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "tree_"
	// Grammars\\ANTLR.g3:534:0: tree_ : TREE_BEGIN treeRoot ( element )+ RPAREN ;
	private ANTLRParser.tree__return tree_(  )
	{
		ANTLRParser.tree__return retval = new ANTLRParser.tree__return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken TREE_BEGIN113=null;
		IToken RPAREN116=null;
		ANTLRParser.treeRoot_return treeRoot114 = default(ANTLRParser.treeRoot_return);
		ANTLRParser.element_return element115 = default(ANTLRParser.element_return);

		GrammarAST TREE_BEGIN113_tree=null;
		GrammarAST RPAREN116_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:535:4: ( TREE_BEGIN treeRoot ( element )+ RPAREN )
			// Grammars\\ANTLR.g3:535:4: TREE_BEGIN treeRoot ( element )+ RPAREN
			{
			root_0 = (GrammarAST)adaptor.Nil();

			TREE_BEGIN113=(IToken)Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_tree_2143); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			TREE_BEGIN113_tree = (GrammarAST)adaptor.Create(TREE_BEGIN113);
			root_0 = (GrammarAST)adaptor.BecomeRoot(TREE_BEGIN113_tree, root_0);
			}
			PushFollow(Follow._treeRoot_in_tree_2148);
			treeRoot114=treeRoot();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, treeRoot114.Tree);
			// Grammars\\ANTLR.g3:536:12: ( element )+
			int cnt61=0;
			for ( ; ; )
			{
				int alt61=2;
				int LA61_0 = input.LA(1);

				if ( (LA61_0==ACTION||LA61_0==CHAR_LITERAL||LA61_0==FORCED_ACTION||LA61_0==LPAREN||LA61_0==NOT||LA61_0==RULE_REF||LA61_0==SEMPRED||LA61_0==STRING_LITERAL||LA61_0==TOKEN_REF||LA61_0==TREE_BEGIN||LA61_0==WILDCARD) )
				{
					alt61=1;
				}


				switch ( alt61 )
				{
				case 1:
					// Grammars\\ANTLR.g3:536:14: element
					{
					PushFollow(Follow._element_in_tree_2152);
					element115=element();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, element115.Tree);

					}
					break;

				default:
					if ( cnt61 >= 1 )
						goto loop61;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee61 = new EarlyExitException( 61, input );
					throw eee61;
				}
				cnt61++;
			}
			loop61:
				;


			RPAREN116=(IToken)Match(input,RPAREN,Follow._RPAREN_in_tree_2159); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "tree_"

	public class ebnf_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ebnf"
	// Grammars\\ANTLR.g3:541:0: ebnf : block ( QUESTION -> ^( OPTIONAL[$start,\"?\"] block ) | STAR -> ^( CLOSURE[$start,\"*\"] block ) | PLUS -> ^( POSITIVE_CLOSURE[$start,\"+\"] block ) | IMPLIES -> {GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0])}? ^( SYNPRED[$start,\"=>\"] block ) ->| ROOT -> ^( ROOT block ) | BANG -> ^( BANG block ) | -> block ) ;
	private ANTLRParser.ebnf_return ebnf(  )
	{
		ANTLRParser.ebnf_return retval = new ANTLRParser.ebnf_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken QUESTION118=null;
		IToken STAR119=null;
		IToken PLUS120=null;
		IToken IMPLIES121=null;
		IToken ROOT122=null;
		IToken BANG123=null;
		ANTLRParser.block_return block117 = default(ANTLRParser.block_return);

		GrammarAST QUESTION118_tree=null;
		GrammarAST STAR119_tree=null;
		GrammarAST PLUS120_tree=null;
		GrammarAST IMPLIES121_tree=null;
		GrammarAST ROOT122_tree=null;
		GrammarAST BANG123_tree=null;
		RewriteRuleITokenStream stream_QUESTION=new RewriteRuleITokenStream(adaptor,"token QUESTION");
		RewriteRuleITokenStream stream_STAR=new RewriteRuleITokenStream(adaptor,"token STAR");
		RewriteRuleITokenStream stream_PLUS=new RewriteRuleITokenStream(adaptor,"token PLUS");
		RewriteRuleITokenStream stream_IMPLIES=new RewriteRuleITokenStream(adaptor,"token IMPLIES");
		RewriteRuleITokenStream stream_ROOT=new RewriteRuleITokenStream(adaptor,"token ROOT");
		RewriteRuleITokenStream stream_BANG=new RewriteRuleITokenStream(adaptor,"token BANG");
		RewriteRuleSubtreeStream stream_block=new RewriteRuleSubtreeStream(adaptor,"rule block");
		try
		{
			// Grammars\\ANTLR.g3:542:4: ( block ( QUESTION -> ^( OPTIONAL[$start,\"?\"] block ) | STAR -> ^( CLOSURE[$start,\"*\"] block ) | PLUS -> ^( POSITIVE_CLOSURE[$start,\"+\"] block ) | IMPLIES -> {GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0])}? ^( SYNPRED[$start,\"=>\"] block ) ->| ROOT -> ^( ROOT block ) | BANG -> ^( BANG block ) | -> block ) )
			// Grammars\\ANTLR.g3:542:4: block ( QUESTION -> ^( OPTIONAL[$start,\"?\"] block ) | STAR -> ^( CLOSURE[$start,\"*\"] block ) | PLUS -> ^( POSITIVE_CLOSURE[$start,\"+\"] block ) | IMPLIES -> {GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0])}? ^( SYNPRED[$start,\"=>\"] block ) ->| ROOT -> ^( ROOT block ) | BANG -> ^( BANG block ) | -> block )
			{
			PushFollow(Follow._block_in_ebnf2173);
			block117=block();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_block.Add(block117.Tree);
			// Grammars\\ANTLR.g3:543:3: ( QUESTION -> ^( OPTIONAL[$start,\"?\"] block ) | STAR -> ^( CLOSURE[$start,\"*\"] block ) | PLUS -> ^( POSITIVE_CLOSURE[$start,\"+\"] block ) | IMPLIES -> {GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0])}? ^( SYNPRED[$start,\"=>\"] block ) ->| ROOT -> ^( ROOT block ) | BANG -> ^( BANG block ) | -> block )
			int alt62=7;
			switch ( input.LA(1) )
			{
			case QUESTION:
				{
				alt62=1;
				}
				break;
			case STAR:
				{
				alt62=2;
				}
				break;
			case PLUS:
				{
				alt62=3;
				}
				break;
			case IMPLIES:
				{
				alt62=4;
				}
				break;
			case ROOT:
				{
				alt62=5;
				}
				break;
			case BANG:
				{
				alt62=6;
				}
				break;
			case ACTION:
			case CHAR_LITERAL:
			case FORCED_ACTION:
			case LPAREN:
			case NOT:
			case OR:
			case REWRITE:
			case RPAREN:
			case RULE_REF:
			case SEMI:
			case SEMPRED:
			case STRING_LITERAL:
			case TOKEN_REF:
			case TREE_BEGIN:
			case WILDCARD:
				{
				alt62=7;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 62, 0, input);

					throw nvae;
				}
			}

			switch ( alt62 )
			{
			case 1:
				// Grammars\\ANTLR.g3:543:5: QUESTION
				{
				QUESTION118=(IToken)Match(input,QUESTION,Follow._QUESTION_in_ebnf2179); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_QUESTION.Add(QUESTION118);



				{
				// AST REWRITE
				// elements: block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 544:4: -> ^( OPTIONAL[$start,\"?\"] block )
				{
					// Grammars\\ANTLR.g3:544:7: ^( OPTIONAL[$start,\"?\"] block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(OPTIONAL, ((IToken)retval.start), "?"), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:545:5: STAR
				{
				STAR119=(IToken)Match(input,STAR,Follow._STAR_in_ebnf2197); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STAR.Add(STAR119);



				{
				// AST REWRITE
				// elements: block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 546:4: -> ^( CLOSURE[$start,\"*\"] block )
				{
					// Grammars\\ANTLR.g3:546:7: ^( CLOSURE[$start,\"*\"] block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(CLOSURE, ((IToken)retval.start), "*"), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:547:5: PLUS
				{
				PLUS120=(IToken)Match(input,PLUS,Follow._PLUS_in_ebnf2215); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PLUS.Add(PLUS120);



				{
				// AST REWRITE
				// elements: block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 548:4: -> ^( POSITIVE_CLOSURE[$start,\"+\"] block )
				{
					// Grammars\\ANTLR.g3:548:7: ^( POSITIVE_CLOSURE[$start,\"+\"] block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(POSITIVE_CLOSURE, ((IToken)retval.start), "+"), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:549:5: IMPLIES
				{
				IMPLIES121=(IToken)Match(input,IMPLIES,Follow._IMPLIES_in_ebnf2233); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_IMPLIES.Add(IMPLIES121);



				{
				// AST REWRITE
				// elements: block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 551:4: -> {GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0])}? ^( SYNPRED[$start,\"=>\"] block )
				if (GrammarType == COMBINED_GRAMMAR && char.IsUpper(currentRuleName[0]))
				{
					// Grammars\\ANTLR.g3:551:78: ^( SYNPRED[$start,\"=>\"] block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(SYNPRED, ((IToken)retval.start), "=>"), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}
				else // 553:4: ->
				{
					adaptor.AddChild(root_0, createSynSemPredFromBlock((block117!=null?((GrammarAST)block117.tree):null), SYN_SEMPRED));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 5:
				// Grammars\\ANTLR.g3:554:5: ROOT
				{
				ROOT122=(IToken)Match(input,ROOT,Follow._ROOT_in_ebnf2269); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ROOT.Add(ROOT122);



				{
				// AST REWRITE
				// elements: ROOT, block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 555:4: -> ^( ROOT block )
				{
					// Grammars\\ANTLR.g3:555:7: ^( ROOT block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(stream_ROOT.NextNode(), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 6:
				// Grammars\\ANTLR.g3:556:5: BANG
				{
				BANG123=(IToken)Match(input,BANG,Follow._BANG_in_ebnf2286); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_BANG.Add(BANG123);



				{
				// AST REWRITE
				// elements: BANG, block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 557:4: -> ^( BANG block )
				{
					// Grammars\\ANTLR.g3:557:7: ^( BANG block )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(stream_BANG.NextNode(), root_1);

					adaptor.AddChild(root_1, stream_block.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 7:
				// Grammars\\ANTLR.g3:559:4: 
				{



				{
				// AST REWRITE
				// elements: block
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 559:4: -> block
				{
					adaptor.AddChild(root_0, stream_block.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ebnf"

	public class range_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "range"
	// Grammars\\ANTLR.g3:563:0: range : c1= CHAR_LITERAL RANGE c2= CHAR_LITERAL -> ^( CHAR_RANGE[$c1,\"..\"] $c1 $c2) ;
	private ANTLRParser.range_return range(  )
	{
		ANTLRParser.range_return retval = new ANTLRParser.range_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken c1=null;
		IToken c2=null;
		IToken RANGE124=null;

		GrammarAST c1_tree=null;
		GrammarAST c2_tree=null;
		GrammarAST RANGE124_tree=null;
		RewriteRuleITokenStream stream_CHAR_LITERAL=new RewriteRuleITokenStream(adaptor,"token CHAR_LITERAL");
		RewriteRuleITokenStream stream_RANGE=new RewriteRuleITokenStream(adaptor,"token RANGE");

		try
		{
			// Grammars\\ANTLR.g3:564:4: (c1= CHAR_LITERAL RANGE c2= CHAR_LITERAL -> ^( CHAR_RANGE[$c1,\"..\"] $c1 $c2) )
			// Grammars\\ANTLR.g3:564:4: c1= CHAR_LITERAL RANGE c2= CHAR_LITERAL
			{
			c1=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_range2325); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_CHAR_LITERAL.Add(c1);

			RANGE124=(IToken)Match(input,RANGE,Follow._RANGE_in_range2327); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RANGE.Add(RANGE124);

			c2=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_range2331); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_CHAR_LITERAL.Add(c2);



			{
			// AST REWRITE
			// elements: c1, c2
			// token labels: c1, c2
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleITokenStream stream_c1=new RewriteRuleITokenStream(adaptor,"token c1",c1);
			RewriteRuleITokenStream stream_c2=new RewriteRuleITokenStream(adaptor,"token c2",c2);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 565:3: -> ^( CHAR_RANGE[$c1,\"..\"] $c1 $c2)
			{
				// Grammars\\ANTLR.g3:565:6: ^( CHAR_RANGE[$c1,\"..\"] $c1 $c2)
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(CHAR_RANGE, c1, ".."), root_1);

				adaptor.AddChild(root_1, stream_c1.NextNode());
				adaptor.AddChild(root_1, stream_c2.NextNode());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "range"

	public class terminal_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "terminal"
	// Grammars\\ANTLR.g3:568:0: terminal : (cl= CHAR_LITERAL ( elementOptions[$cl.tree] )? ( ROOT | BANG )? |tr= TOKEN_REF ( elementOptions[$tr.tree] )? ( ARG_ACTION )? ( ROOT | BANG )? |sl= STRING_LITERAL ( elementOptions[$sl.tree] )? ( ROOT | BANG )? |wi= WILDCARD ( ROOT | BANG )? );
	private ANTLRParser.terminal_return terminal(  )
	{
		ANTLRParser.terminal_return retval = new ANTLRParser.terminal_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken cl=null;
		IToken tr=null;
		IToken sl=null;
		IToken wi=null;
		IToken ROOT126=null;
		IToken BANG127=null;
		IToken ARG_ACTION129=null;
		IToken ROOT130=null;
		IToken BANG131=null;
		IToken ROOT133=null;
		IToken BANG134=null;
		IToken ROOT135=null;
		IToken BANG136=null;
		ANTLRParser.elementOptions_return elementOptions125 = default(ANTLRParser.elementOptions_return);
		ANTLRParser.elementOptions_return elementOptions128 = default(ANTLRParser.elementOptions_return);
		ANTLRParser.elementOptions_return elementOptions132 = default(ANTLRParser.elementOptions_return);

		GrammarAST cl_tree=null;
		GrammarAST tr_tree=null;
		GrammarAST sl_tree=null;
		GrammarAST wi_tree=null;
		GrammarAST ROOT126_tree=null;
		GrammarAST BANG127_tree=null;
		GrammarAST ARG_ACTION129_tree=null;
		GrammarAST ROOT130_tree=null;
		GrammarAST BANG131_tree=null;
		GrammarAST ROOT133_tree=null;
		GrammarAST BANG134_tree=null;
		GrammarAST ROOT135_tree=null;
		GrammarAST BANG136_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:569:4: (cl= CHAR_LITERAL ( elementOptions[$cl.tree] )? ( ROOT | BANG )? |tr= TOKEN_REF ( elementOptions[$tr.tree] )? ( ARG_ACTION )? ( ROOT | BANG )? |sl= STRING_LITERAL ( elementOptions[$sl.tree] )? ( ROOT | BANG )? |wi= WILDCARD ( ROOT | BANG )? )
			int alt71=4;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt71=1;
				}
				break;
			case TOKEN_REF:
				{
				alt71=2;
				}
				break;
			case STRING_LITERAL:
				{
				alt71=3;
				}
				break;
			case WILDCARD:
				{
				alt71=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 71, 0, input);

					throw nvae;
				}
			}

			switch ( alt71 )
			{
			case 1:
				// Grammars\\ANTLR.g3:569:4: cl= CHAR_LITERAL ( elementOptions[$cl.tree] )? ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				cl=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_terminal2359); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				cl_tree = (GrammarAST)adaptor.Create(cl);
				root_0 = (GrammarAST)adaptor.BecomeRoot(cl_tree, root_0);
				}
				// Grammars\\ANTLR.g3:569:21: ( elementOptions[$cl.tree] )?
				int alt63=2;
				int LA63_0 = input.LA(1);

				if ( (LA63_0==OPEN_ELEMENT_OPTION) )
				{
					alt63=1;
				}
				switch ( alt63 )
				{
				case 1:
					// Grammars\\ANTLR.g3:569:23: elementOptions[$cl.tree]
					{
					PushFollow(Follow._elementOptions_in_terminal2364);
					elementOptions125=elementOptions(cl_tree);

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				}

				// Grammars\\ANTLR.g3:569:52: ( ROOT | BANG )?
				int alt64=3;
				int LA64_0 = input.LA(1);

				if ( (LA64_0==ROOT) )
				{
					alt64=1;
				}
				else if ( (LA64_0==BANG) )
				{
					alt64=2;
				}
				switch ( alt64 )
				{
				case 1:
					// Grammars\\ANTLR.g3:569:53: ROOT
					{
					ROOT126=(IToken)Match(input,ROOT,Follow._ROOT_in_terminal2372); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT126_tree = (GrammarAST)adaptor.Create(ROOT126);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT126_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:569:59: BANG
					{
					BANG127=(IToken)Match(input,BANG,Follow._BANG_in_terminal2375); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG127_tree = (GrammarAST)adaptor.Create(BANG127);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG127_tree, root_0);
					}

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:571:4: tr= TOKEN_REF ( elementOptions[$tr.tree] )? ( ARG_ACTION )? ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				tr=(IToken)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_terminal2386); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				tr_tree = (GrammarAST)adaptor.Create(tr);
				root_0 = (GrammarAST)adaptor.BecomeRoot(tr_tree, root_0);
				}
				// Grammars\\ANTLR.g3:572:3: ( elementOptions[$tr.tree] )?
				int alt65=2;
				int LA65_0 = input.LA(1);

				if ( (LA65_0==OPEN_ELEMENT_OPTION) )
				{
					alt65=1;
				}
				switch ( alt65 )
				{
				case 1:
					// Grammars\\ANTLR.g3:572:5: elementOptions[$tr.tree]
					{
					PushFollow(Follow._elementOptions_in_terminal2393);
					elementOptions128=elementOptions(tr_tree);

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				}

				// Grammars\\ANTLR.g3:573:3: ( ARG_ACTION )?
				int alt66=2;
				int LA66_0 = input.LA(1);

				if ( (LA66_0==ARG_ACTION) )
				{
					alt66=1;
				}
				switch ( alt66 )
				{
				case 1:
					// Grammars\\ANTLR.g3:573:5: ARG_ACTION
					{
					ARG_ACTION129=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_terminal2404); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					ARG_ACTION129_tree = (GrammarAST)adaptor.Create(ARG_ACTION129);
					adaptor.AddChild(root_0, ARG_ACTION129_tree);
					}

					}
					break;

				}

				// Grammars\\ANTLR.g3:574:3: ( ROOT | BANG )?
				int alt67=3;
				int LA67_0 = input.LA(1);

				if ( (LA67_0==ROOT) )
				{
					alt67=1;
				}
				else if ( (LA67_0==BANG) )
				{
					alt67=2;
				}
				switch ( alt67 )
				{
				case 1:
					// Grammars\\ANTLR.g3:574:4: ROOT
					{
					ROOT130=(IToken)Match(input,ROOT,Follow._ROOT_in_terminal2413); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT130_tree = (GrammarAST)adaptor.Create(ROOT130);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT130_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:574:10: BANG
					{
					BANG131=(IToken)Match(input,BANG,Follow._BANG_in_terminal2416); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG131_tree = (GrammarAST)adaptor.Create(BANG131);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG131_tree, root_0);
					}

					}
					break;

				}


				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:576:4: sl= STRING_LITERAL ( elementOptions[$sl.tree] )? ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				sl=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_terminal2427); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				sl_tree = (GrammarAST)adaptor.Create(sl);
				root_0 = (GrammarAST)adaptor.BecomeRoot(sl_tree, root_0);
				}
				// Grammars\\ANTLR.g3:576:23: ( elementOptions[$sl.tree] )?
				int alt68=2;
				int LA68_0 = input.LA(1);

				if ( (LA68_0==OPEN_ELEMENT_OPTION) )
				{
					alt68=1;
				}
				switch ( alt68 )
				{
				case 1:
					// Grammars\\ANTLR.g3:576:25: elementOptions[$sl.tree]
					{
					PushFollow(Follow._elementOptions_in_terminal2432);
					elementOptions132=elementOptions(sl_tree);

					state._fsp--;
					if (state.failed) return retval;

					}
					break;

				}

				// Grammars\\ANTLR.g3:576:54: ( ROOT | BANG )?
				int alt69=3;
				int LA69_0 = input.LA(1);

				if ( (LA69_0==ROOT) )
				{
					alt69=1;
				}
				else if ( (LA69_0==BANG) )
				{
					alt69=2;
				}
				switch ( alt69 )
				{
				case 1:
					// Grammars\\ANTLR.g3:576:55: ROOT
					{
					ROOT133=(IToken)Match(input,ROOT,Follow._ROOT_in_terminal2440); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT133_tree = (GrammarAST)adaptor.Create(ROOT133);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT133_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:576:61: BANG
					{
					BANG134=(IToken)Match(input,BANG,Follow._BANG_in_terminal2443); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG134_tree = (GrammarAST)adaptor.Create(BANG134);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG134_tree, root_0);
					}

					}
					break;

				}


				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:578:4: wi= WILDCARD ( ROOT | BANG )?
				{
				root_0 = (GrammarAST)adaptor.Nil();

				wi=(IToken)Match(input,WILDCARD,Follow._WILDCARD_in_terminal2454); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				wi_tree = (GrammarAST)adaptor.Create(wi);
				adaptor.AddChild(root_0, wi_tree);
				}
				// Grammars\\ANTLR.g3:578:16: ( ROOT | BANG )?
				int alt70=3;
				int LA70_0 = input.LA(1);

				if ( (LA70_0==ROOT) )
				{
					alt70=1;
				}
				else if ( (LA70_0==BANG) )
				{
					alt70=2;
				}
				switch ( alt70 )
				{
				case 1:
					// Grammars\\ANTLR.g3:578:17: ROOT
					{
					ROOT135=(IToken)Match(input,ROOT,Follow._ROOT_in_terminal2457); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					ROOT135_tree = (GrammarAST)adaptor.Create(ROOT135);
					root_0 = (GrammarAST)adaptor.BecomeRoot(ROOT135_tree, root_0);
					}

					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:578:23: BANG
					{
					BANG136=(IToken)Match(input,BANG,Follow._BANG_in_terminal2460); if (state.failed) return retval;
					if ( state.backtracking == 0 ) {
					BANG136_tree = (GrammarAST)adaptor.Create(BANG136);
					root_0 = (GrammarAST)adaptor.BecomeRoot(BANG136_tree, root_0);
					}

					}
					break;

				}

				if ( state.backtracking == 0 )
				{

								if ( atTreeRoot )
								{
									ErrorManager.syntaxError(
										ErrorManager.MSG_WILDCARD_AS_ROOT,Grammar,wi,null,null);
								}
							
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "terminal"

	public class elementOptions_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "elementOptions"
	// Grammars\\ANTLR.g3:588:0: elementOptions[GrammarAST terminalAST] : ( OPEN_ELEMENT_OPTION defaultNodeOption[terminalAST] CLOSE_ELEMENT_OPTION | OPEN_ELEMENT_OPTION elementOption[terminalAST] ( SEMI elementOption[terminalAST] )* CLOSE_ELEMENT_OPTION );
	private ANTLRParser.elementOptions_return elementOptions( GrammarAST terminalAST )
	{
		ANTLRParser.elementOptions_return retval = new ANTLRParser.elementOptions_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken OPEN_ELEMENT_OPTION137=null;
		IToken CLOSE_ELEMENT_OPTION139=null;
		IToken OPEN_ELEMENT_OPTION140=null;
		IToken SEMI142=null;
		IToken CLOSE_ELEMENT_OPTION144=null;
		ANTLRParser.defaultNodeOption_return defaultNodeOption138 = default(ANTLRParser.defaultNodeOption_return);
		ANTLRParser.elementOption_return elementOption141 = default(ANTLRParser.elementOption_return);
		ANTLRParser.elementOption_return elementOption143 = default(ANTLRParser.elementOption_return);

		GrammarAST OPEN_ELEMENT_OPTION137_tree=null;
		GrammarAST CLOSE_ELEMENT_OPTION139_tree=null;
		GrammarAST OPEN_ELEMENT_OPTION140_tree=null;
		GrammarAST SEMI142_tree=null;
		GrammarAST CLOSE_ELEMENT_OPTION144_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:589:4: ( OPEN_ELEMENT_OPTION defaultNodeOption[terminalAST] CLOSE_ELEMENT_OPTION | OPEN_ELEMENT_OPTION elementOption[terminalAST] ( SEMI elementOption[terminalAST] )* CLOSE_ELEMENT_OPTION )
			int alt73=2;
			int LA73_0 = input.LA(1);

			if ( (LA73_0==OPEN_ELEMENT_OPTION) )
			{
				int LA73_1 = input.LA(2);

				if ( (LA73_1==TOKEN_REF) )
				{
					int LA73_2 = input.LA(3);

					if ( (LA73_2==CLOSE_ELEMENT_OPTION||LA73_2==WILDCARD) )
					{
						alt73=1;
					}
					else if ( (LA73_2==ASSIGN) )
					{
						alt73=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 73, 2, input);

						throw nvae;
					}
				}
				else if ( (LA73_1==RULE_REF) )
				{
					int LA73_3 = input.LA(3);

					if ( (LA73_3==CLOSE_ELEMENT_OPTION||LA73_3==WILDCARD) )
					{
						alt73=1;
					}
					else if ( (LA73_3==ASSIGN) )
					{
						alt73=2;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 73, 3, input);

						throw nvae;
					}
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 73, 1, input);

					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 73, 0, input);

				throw nvae;
			}
			switch ( alt73 )
			{
			case 1:
				// Grammars\\ANTLR.g3:589:4: OPEN_ELEMENT_OPTION defaultNodeOption[terminalAST] CLOSE_ELEMENT_OPTION
				{
				root_0 = (GrammarAST)adaptor.Nil();

				OPEN_ELEMENT_OPTION137=(IToken)Match(input,OPEN_ELEMENT_OPTION,Follow._OPEN_ELEMENT_OPTION_in_elementOptions2479); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				OPEN_ELEMENT_OPTION137_tree = (GrammarAST)adaptor.Create(OPEN_ELEMENT_OPTION137);
				root_0 = (GrammarAST)adaptor.BecomeRoot(OPEN_ELEMENT_OPTION137_tree, root_0);
				}
				PushFollow(Follow._defaultNodeOption_in_elementOptions2482);
				defaultNodeOption138=defaultNodeOption(terminalAST);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, defaultNodeOption138.Tree);
				CLOSE_ELEMENT_OPTION139=(IToken)Match(input,CLOSE_ELEMENT_OPTION,Follow._CLOSE_ELEMENT_OPTION_in_elementOptions2485); if (state.failed) return retval;

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:590:4: OPEN_ELEMENT_OPTION elementOption[terminalAST] ( SEMI elementOption[terminalAST] )* CLOSE_ELEMENT_OPTION
				{
				root_0 = (GrammarAST)adaptor.Nil();

				OPEN_ELEMENT_OPTION140=(IToken)Match(input,OPEN_ELEMENT_OPTION,Follow._OPEN_ELEMENT_OPTION_in_elementOptions2491); if (state.failed) return retval;
				if ( state.backtracking == 0 ) {
				OPEN_ELEMENT_OPTION140_tree = (GrammarAST)adaptor.Create(OPEN_ELEMENT_OPTION140);
				root_0 = (GrammarAST)adaptor.BecomeRoot(OPEN_ELEMENT_OPTION140_tree, root_0);
				}
				PushFollow(Follow._elementOption_in_elementOptions2494);
				elementOption141=elementOption(terminalAST);

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, elementOption141.Tree);
				// Grammars\\ANTLR.g3:590:52: ( SEMI elementOption[terminalAST] )*
				for ( ; ; )
				{
					int alt72=2;
					int LA72_0 = input.LA(1);

					if ( (LA72_0==SEMI) )
					{
						alt72=1;
					}


					switch ( alt72 )
					{
					case 1:
						// Grammars\\ANTLR.g3:590:53: SEMI elementOption[terminalAST]
						{
						SEMI142=(IToken)Match(input,SEMI,Follow._SEMI_in_elementOptions2498); if (state.failed) return retval;
						PushFollow(Follow._elementOption_in_elementOptions2501);
						elementOption143=elementOption(terminalAST);

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, elementOption143.Tree);

						}
						break;

					default:
						goto loop72;
					}
				}

				loop72:
					;


				CLOSE_ELEMENT_OPTION144=(IToken)Match(input,CLOSE_ELEMENT_OPTION,Follow._CLOSE_ELEMENT_OPTION_in_elementOptions2506); if (state.failed) return retval;

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "elementOptions"

	public class defaultNodeOption_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "defaultNodeOption"
	// Grammars\\ANTLR.g3:593:0: defaultNodeOption[GrammarAST terminalAST] : i= id ( WILDCARD i2= id )* ;
	private ANTLRParser.defaultNodeOption_return defaultNodeOption( GrammarAST terminalAST )
	{
		ANTLRParser.defaultNodeOption_return retval = new ANTLRParser.defaultNodeOption_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken WILDCARD145=null;
		ANTLRParser.id_return i = default(ANTLRParser.id_return);
		ANTLRParser.id_return i2 = default(ANTLRParser.id_return);

		GrammarAST WILDCARD145_tree=null;


		StringBuffer buf = new StringBuffer();

		try
		{
			// Grammars\\ANTLR.g3:598:4: (i= id ( WILDCARD i2= id )* )
			// Grammars\\ANTLR.g3:598:4: i= id ( WILDCARD i2= id )*
			{
			root_0 = (GrammarAST)adaptor.Nil();

			PushFollow(Follow._id_in_defaultNodeOption2526);
			i=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, i.Tree);
			if ( state.backtracking == 0 )
			{
				buf.Append((i!=null?input.ToString(i.start,i.stop):null));
			}
			// Grammars\\ANTLR.g3:598:32: ( WILDCARD i2= id )*
			for ( ; ; )
			{
				int alt74=2;
				int LA74_0 = input.LA(1);

				if ( (LA74_0==WILDCARD) )
				{
					alt74=1;
				}


				switch ( alt74 )
				{
				case 1:
					// Grammars\\ANTLR.g3:598:33: WILDCARD i2= id
					{
					WILDCARD145=(IToken)Match(input,WILDCARD,Follow._WILDCARD_in_defaultNodeOption2531); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					WILDCARD145_tree = (GrammarAST)adaptor.Create(WILDCARD145);
					adaptor.AddChild(root_0, WILDCARD145_tree);
					}
					PushFollow(Follow._id_in_defaultNodeOption2535);
					i2=id();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, i2.Tree);
					if ( state.backtracking == 0 )
					{
						buf.Append("."+(i2!=null?input.ToString(i2.start,i2.stop):null));
					}

					}
					break;

				default:
					goto loop74;
				}
			}

			loop74:
				;


			if ( state.backtracking == 0 )
			{
				terminalAST.setTerminalOption(Grammar,Grammar.defaultTokenOption,buf.ToString());
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "defaultNodeOption"

	public class elementOption_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "elementOption"
	// Grammars\\ANTLR.g3:602:0: elementOption[GrammarAST terminalAST] : a= id ASSIGN (b= id |s= STRING_LITERAL ) ;
	private ANTLRParser.elementOption_return elementOption( GrammarAST terminalAST )
	{
		ANTLRParser.elementOption_return retval = new ANTLRParser.elementOption_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken s=null;
		IToken ASSIGN146=null;
		ANTLRParser.id_return a = default(ANTLRParser.id_return);
		ANTLRParser.id_return b = default(ANTLRParser.id_return);

		GrammarAST s_tree=null;
		GrammarAST ASSIGN146_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:603:4: (a= id ASSIGN (b= id |s= STRING_LITERAL ) )
			// Grammars\\ANTLR.g3:603:4: a= id ASSIGN (b= id |s= STRING_LITERAL )
			{
			root_0 = (GrammarAST)adaptor.Nil();

			PushFollow(Follow._id_in_elementOption2557);
			a=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, a.Tree);
			ASSIGN146=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_elementOption2559); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			ASSIGN146_tree = (GrammarAST)adaptor.Create(ASSIGN146);
			root_0 = (GrammarAST)adaptor.BecomeRoot(ASSIGN146_tree, root_0);
			}
			// Grammars\\ANTLR.g3:603:17: (b= id |s= STRING_LITERAL )
			int alt75=2;
			int LA75_0 = input.LA(1);

			if ( (LA75_0==RULE_REF||LA75_0==TOKEN_REF) )
			{
				alt75=1;
			}
			else if ( (LA75_0==STRING_LITERAL) )
			{
				alt75=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 75, 0, input);

				throw nvae;
			}
			switch ( alt75 )
			{
			case 1:
				// Grammars\\ANTLR.g3:603:18: b= id
				{
				PushFollow(Follow._id_in_elementOption2565);
				b=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, b.Tree);

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:603:23: s= STRING_LITERAL
				{
				s=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_elementOption2569); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				s_tree = (GrammarAST)adaptor.Create(s);
				adaptor.AddChild(root_0, s_tree);
				}

				}
				break;

			}

			if ( state.backtracking == 0 )
			{

						object v = ((b!=null?((GrammarAST)b.tree):null)!=null)?(b!=null?input.ToString(b.start,b.stop):null):(s!=null?s.Text:null);
						terminalAST.setTerminalOption(Grammar,(a!=null?input.ToString(a.start,a.stop):null),v);
						
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "elementOption"

	public class ebnfSuffix_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "ebnfSuffix"
	// Grammars\\ANTLR.g3:610:0: ebnfSuffix[GrammarAST elemAST, bool inRewrite] : ( -> BLOCK[\"BLOCK\"] ) ( -> ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] ) ) ( QUESTION -> OPTIONAL[$elemAST.Token,\"?\"] | STAR -> CLOSURE[$elemAST.Token,\"*\"] | PLUS -> POSITIVE_CLOSURE[$elemAST.Token,\"+\"] ) -> ^( $ebnfSuffix ^( EOB[$elemAST.Token, \"<end-of-block>\"] ) ) ;
	private ANTLRParser.ebnfSuffix_return ebnfSuffix( GrammarAST elemAST, bool inRewrite )
	{
		ANTLRParser.ebnfSuffix_return retval = new ANTLRParser.ebnfSuffix_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken QUESTION147=null;
		IToken STAR148=null;
		IToken PLUS149=null;

		GrammarAST QUESTION147_tree=null;
		GrammarAST STAR148_tree=null;
		GrammarAST PLUS149_tree=null;
		RewriteRuleITokenStream stream_QUESTION=new RewriteRuleITokenStream(adaptor,"token QUESTION");
		RewriteRuleITokenStream stream_STAR=new RewriteRuleITokenStream(adaptor,"token STAR");
		RewriteRuleITokenStream stream_PLUS=new RewriteRuleITokenStream(adaptor,"token PLUS");


		GrammarAST blkRoot=null;
		GrammarAST alt=null;
		GrammarAST save = currentBlockAST;

		try
		{
			// Grammars\\ANTLR.g3:621:4: ( ( -> BLOCK[\"BLOCK\"] ) ( -> ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] ) ) ( QUESTION -> OPTIONAL[$elemAST.Token,\"?\"] | STAR -> CLOSURE[$elemAST.Token,\"*\"] | PLUS -> POSITIVE_CLOSURE[$elemAST.Token,\"+\"] ) -> ^( $ebnfSuffix ^( EOB[$elemAST.Token, \"<end-of-block>\"] ) ) )
			// Grammars\\ANTLR.g3:621:4: ( -> BLOCK[\"BLOCK\"] ) ( -> ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] ) ) ( QUESTION -> OPTIONAL[$elemAST.Token,\"?\"] | STAR -> CLOSURE[$elemAST.Token,\"*\"] | PLUS -> POSITIVE_CLOSURE[$elemAST.Token,\"+\"] )
			{
			// Grammars\\ANTLR.g3:621:4: ( -> BLOCK[\"BLOCK\"] )
			// Grammars\\ANTLR.g3:621:6: 
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

			root_0 = (GrammarAST)adaptor.Nil();
			// 621:6: -> BLOCK[\"BLOCK\"]
			{
				adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(BLOCK, "BLOCK"));

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{
				 blkRoot = (GrammarAST)((GrammarAST)retval.tree).GetChild(0); currentBlockAST = blkRoot; 
			}
			// Grammars\\ANTLR.g3:624:3: ( -> ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] ) )
			// Grammars\\ANTLR.g3:625:4: 
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

			root_0 = (GrammarAST)adaptor.Nil();
			// 625:4: -> ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] )
			{
				// Grammars\\ANTLR.g3:625:7: ^( ALT[\"ALT\"] EOA[\"<end-of-alt>\"] )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ALT, "ALT"), root_1);

				adaptor.AddChild(root_1, elemAST);
				adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOA, "<end-of-alt>"));

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			if ( state.backtracking == 0 )
			{

							alt = (GrammarAST)((GrammarAST)retval.tree).GetChild(0);
							if ( !inRewrite )
								prefixWithSynPred(alt);
						
			}
			// Grammars\\ANTLR.g3:632:3: ( QUESTION -> OPTIONAL[$elemAST.Token,\"?\"] | STAR -> CLOSURE[$elemAST.Token,\"*\"] | PLUS -> POSITIVE_CLOSURE[$elemAST.Token,\"+\"] )
			int alt76=3;
			switch ( input.LA(1) )
			{
			case QUESTION:
				{
				alt76=1;
				}
				break;
			case STAR:
				{
				alt76=2;
				}
				break;
			case PLUS:
				{
				alt76=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 76, 0, input);

					throw nvae;
				}
			}

			switch ( alt76 )
			{
			case 1:
				// Grammars\\ANTLR.g3:632:5: QUESTION
				{
				QUESTION147=(IToken)Match(input,QUESTION,Follow._QUESTION_in_ebnfSuffix2643); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_QUESTION.Add(QUESTION147);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 633:4: -> OPTIONAL[$elemAST.Token,\"?\"]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(OPTIONAL, elemAST.Token, "?"));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:634:5: STAR
				{
				STAR148=(IToken)Match(input,STAR,Follow._STAR_in_ebnfSuffix2657); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STAR.Add(STAR148);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 635:4: -> CLOSURE[$elemAST.Token,\"*\"]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(CLOSURE, elemAST.Token, "*"));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:636:5: PLUS
				{
				PLUS149=(IToken)Match(input,PLUS,Follow._PLUS_in_ebnfSuffix2671); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PLUS.Add(PLUS149);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 637:4: -> POSITIVE_CLOSURE[$elemAST.Token,\"+\"]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(POSITIVE_CLOSURE, elemAST.Token, "+"));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}



			{
			// AST REWRITE
			// elements: ebnfSuffix
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 639:3: -> ^( $ebnfSuffix ^( EOB[$elemAST.Token, \"<end-of-block>\"] ) )
			{
				// Grammars\\ANTLR.g3:639:6: ^( $ebnfSuffix ^( EOB[$elemAST.Token, \"<end-of-block>\"] ) )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot(stream_retval.NextNode(), root_1);

				// Grammars\\ANTLR.g3:639:20: ^( EOB[$elemAST.Token, \"<end-of-block>\"] )
				{
				GrammarAST root_2 = (GrammarAST)adaptor.Nil();
				root_2 = (GrammarAST)adaptor.BecomeRoot(blkRoot, root_2);

				adaptor.AddChild(root_2, alt);
				adaptor.AddChild(root_2, (GrammarAST)adaptor.Create(EOB, elemAST.Token, "<end-of-block>"));

				adaptor.AddChild(root_1, root_2);
				}

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
			if ( state.backtracking == 0 )
			{

				currentBlockAST = save;

			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "ebnfSuffix"

	public class notTerminal_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "notTerminal"
	// Grammars\\ANTLR.g3:642:0: notTerminal : (cl= CHAR_LITERAL |tr= TOKEN_REF | STRING_LITERAL );
	private ANTLRParser.notTerminal_return notTerminal(  )
	{
		ANTLRParser.notTerminal_return retval = new ANTLRParser.notTerminal_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken cl=null;
		IToken tr=null;
		IToken STRING_LITERAL150=null;

		GrammarAST cl_tree=null;
		GrammarAST tr_tree=null;
		GrammarAST STRING_LITERAL150_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:643:4: (cl= CHAR_LITERAL |tr= TOKEN_REF | STRING_LITERAL )
			int alt77=3;
			switch ( input.LA(1) )
			{
			case CHAR_LITERAL:
				{
				alt77=1;
				}
				break;
			case TOKEN_REF:
				{
				alt77=2;
				}
				break;
			case STRING_LITERAL:
				{
				alt77=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 77, 0, input);

					throw nvae;
				}
			}

			switch ( alt77 )
			{
			case 1:
				// Grammars\\ANTLR.g3:643:4: cl= CHAR_LITERAL
				{
				root_0 = (GrammarAST)adaptor.Nil();

				cl=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_notTerminal2714); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				cl_tree = (GrammarAST)adaptor.Create(cl);
				adaptor.AddChild(root_0, cl_tree);
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:644:4: tr= TOKEN_REF
				{
				root_0 = (GrammarAST)adaptor.Nil();

				tr=(IToken)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_notTerminal2721); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				tr_tree = (GrammarAST)adaptor.Create(tr);
				adaptor.AddChild(root_0, tr_tree);
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:645:4: STRING_LITERAL
				{
				root_0 = (GrammarAST)adaptor.Nil();

				STRING_LITERAL150=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_notTerminal2726); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				STRING_LITERAL150_tree = (GrammarAST)adaptor.Create(STRING_LITERAL150);
				adaptor.AddChild(root_0, STRING_LITERAL150_tree);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "notTerminal"

	public class idList_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "idList"
	// Grammars\\ANTLR.g3:648:0: idList : id ( COMMA id )* ;
	private ANTLRParser.idList_return idList(  )
	{
		ANTLRParser.idList_return retval = new ANTLRParser.idList_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken COMMA152=null;
		ANTLRParser.id_return id151 = default(ANTLRParser.id_return);
		ANTLRParser.id_return id153 = default(ANTLRParser.id_return);

		GrammarAST COMMA152_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:649:4: ( id ( COMMA id )* )
			// Grammars\\ANTLR.g3:649:4: id ( COMMA id )*
			{
			root_0 = (GrammarAST)adaptor.Nil();

			PushFollow(Follow._id_in_idList2737);
			id151=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id151.Tree);
			// Grammars\\ANTLR.g3:649:7: ( COMMA id )*
			for ( ; ; )
			{
				int alt78=2;
				int LA78_0 = input.LA(1);

				if ( (LA78_0==COMMA) )
				{
					alt78=1;
				}


				switch ( alt78 )
				{
				case 1:
					// Grammars\\ANTLR.g3:649:8: COMMA id
					{
					COMMA152=(IToken)Match(input,COMMA,Follow._COMMA_in_idList2740); if (state.failed) return retval;
					PushFollow(Follow._id_in_idList2743);
					id153=id();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, id153.Tree);

					}
					break;

				default:
					goto loop78;
				}
			}

			loop78:
				;



			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "idList"

	public class id_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "id"
	// Grammars\\ANTLR.g3:652:0: id : ( TOKEN_REF -> ID[$TOKEN_REF] | RULE_REF -> ID[$RULE_REF] );
	private ANTLRParser.id_return id(  )
	{
		ANTLRParser.id_return retval = new ANTLRParser.id_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken TOKEN_REF154=null;
		IToken RULE_REF155=null;

		GrammarAST TOKEN_REF154_tree=null;
		GrammarAST RULE_REF155_tree=null;
		RewriteRuleITokenStream stream_TOKEN_REF=new RewriteRuleITokenStream(adaptor,"token TOKEN_REF");
		RewriteRuleITokenStream stream_RULE_REF=new RewriteRuleITokenStream(adaptor,"token RULE_REF");

		try
		{
			// Grammars\\ANTLR.g3:653:4: ( TOKEN_REF -> ID[$TOKEN_REF] | RULE_REF -> ID[$RULE_REF] )
			int alt79=2;
			int LA79_0 = input.LA(1);

			if ( (LA79_0==TOKEN_REF) )
			{
				alt79=1;
			}
			else if ( (LA79_0==RULE_REF) )
			{
				alt79=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 79, 0, input);

				throw nvae;
			}
			switch ( alt79 )
			{
			case 1:
				// Grammars\\ANTLR.g3:653:4: TOKEN_REF
				{
				TOKEN_REF154=(IToken)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_id2756); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TOKEN_REF.Add(TOKEN_REF154);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 654:3: -> ID[$TOKEN_REF]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ID, TOKEN_REF154));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:655:4: RULE_REF
				{
				RULE_REF155=(IToken)Match(input,RULE_REF,Follow._RULE_REF_in_id2768); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RULE_REF.Add(RULE_REF155);



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

				root_0 = (GrammarAST)adaptor.Nil();
				// 656:3: -> ID[$RULE_REF]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ID, RULE_REF155));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "id"

	public class rewrite_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite"
	// Grammars\\ANTLR.g3:661:0: rewrite : ( ( rewrite_with_sempred )* REWRITE rewrite_alternative -> ( rewrite_with_sempred )* ^( REWRITE rewrite_alternative ) |);
	private ANTLRParser.rewrite_return rewrite(  )
	{
		ANTLRParser.rewrite_return retval = new ANTLRParser.rewrite_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken REWRITE157=null;
		ANTLRParser.rewrite_with_sempred_return rewrite_with_sempred156 = default(ANTLRParser.rewrite_with_sempred_return);
		ANTLRParser.rewrite_alternative_return rewrite_alternative158 = default(ANTLRParser.rewrite_alternative_return);

		GrammarAST REWRITE157_tree=null;
		RewriteRuleITokenStream stream_REWRITE=new RewriteRuleITokenStream(adaptor,"token REWRITE");
		RewriteRuleSubtreeStream stream_rewrite_with_sempred=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_with_sempred");
		RewriteRuleSubtreeStream stream_rewrite_alternative=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_alternative");
		try
		{
			// Grammars\\ANTLR.g3:662:4: ( ( rewrite_with_sempred )* REWRITE rewrite_alternative -> ( rewrite_with_sempred )* ^( REWRITE rewrite_alternative ) |)
			int alt81=2;
			int LA81_0 = input.LA(1);

			if ( (LA81_0==REWRITE) )
			{
				alt81=1;
			}
			else if ( (LA81_0==OR||LA81_0==RPAREN||LA81_0==SEMI) )
			{
				alt81=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 81, 0, input);

				throw nvae;
			}
			switch ( alt81 )
			{
			case 1:
				// Grammars\\ANTLR.g3:662:4: ( rewrite_with_sempred )* REWRITE rewrite_alternative
				{
				// Grammars\\ANTLR.g3:662:4: ( rewrite_with_sempred )*
				for ( ; ; )
				{
					int alt80=2;
					int LA80_0 = input.LA(1);

					if ( (LA80_0==REWRITE) )
					{
						int LA80_1 = input.LA(2);

						if ( (LA80_1==SEMPRED) )
						{
							alt80=1;
						}


					}


					switch ( alt80 )
					{
					case 1:
						// Grammars\\ANTLR.g3:662:0: rewrite_with_sempred
						{
						PushFollow(Follow._rewrite_with_sempred_in_rewrite2788);
						rewrite_with_sempred156=rewrite_with_sempred();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_rewrite_with_sempred.Add(rewrite_with_sempred156.Tree);

						}
						break;

					default:
						goto loop80;
					}
				}

				loop80:
					;


				REWRITE157=(IToken)Match(input,REWRITE,Follow._REWRITE_in_rewrite2793); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_REWRITE.Add(REWRITE157);

				PushFollow(Follow._rewrite_alternative_in_rewrite2795);
				rewrite_alternative158=rewrite_alternative();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rewrite_alternative.Add(rewrite_alternative158.Tree);


				{
				// AST REWRITE
				// elements: rewrite_with_sempred, REWRITE, rewrite_alternative
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 664:3: -> ( rewrite_with_sempred )* ^( REWRITE rewrite_alternative )
				{
					// Grammars\\ANTLR.g3:664:6: ( rewrite_with_sempred )*
					while ( stream_rewrite_with_sempred.HasNext )
					{
						adaptor.AddChild(root_0, stream_rewrite_with_sempred.NextTree());

					}
					stream_rewrite_with_sempred.Reset();
					// Grammars\\ANTLR.g3:664:28: ^( REWRITE rewrite_alternative )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(stream_REWRITE.NextNode(), root_1);

					adaptor.AddChild(root_1, stream_rewrite_alternative.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:666:2: 
				{
				root_0 = (GrammarAST)adaptor.Nil();



				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite"

	public class rewrite_with_sempred_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_with_sempred"
	// Grammars\\ANTLR.g3:668:0: rewrite_with_sempred : REWRITE SEMPRED rewrite_alternative ;
	private ANTLRParser.rewrite_with_sempred_return rewrite_with_sempred(  )
	{
		ANTLRParser.rewrite_with_sempred_return retval = new ANTLRParser.rewrite_with_sempred_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken REWRITE159=null;
		IToken SEMPRED160=null;
		ANTLRParser.rewrite_alternative_return rewrite_alternative161 = default(ANTLRParser.rewrite_alternative_return);

		GrammarAST REWRITE159_tree=null;
		GrammarAST SEMPRED160_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:669:4: ( REWRITE SEMPRED rewrite_alternative )
			// Grammars\\ANTLR.g3:669:4: REWRITE SEMPRED rewrite_alternative
			{
			root_0 = (GrammarAST)adaptor.Nil();

			REWRITE159=(IToken)Match(input,REWRITE,Follow._REWRITE_in_rewrite_with_sempred2822); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			REWRITE159_tree = (GrammarAST)adaptor.Create(REWRITE159);
			root_0 = (GrammarAST)adaptor.BecomeRoot(REWRITE159_tree, root_0);
			}
			SEMPRED160=(IToken)Match(input,SEMPRED,Follow._SEMPRED_in_rewrite_with_sempred2825); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			SEMPRED160_tree = (GrammarAST)adaptor.Create(SEMPRED160);
			adaptor.AddChild(root_0, SEMPRED160_tree);
			}
			PushFollow(Follow._rewrite_alternative_in_rewrite_with_sempred2827);
			rewrite_alternative161=rewrite_alternative();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_alternative161.Tree);

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_with_sempred"

	public class rewrite_block_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_block"
	// Grammars\\ANTLR.g3:672:0: rewrite_block : LPAREN rewrite_alternative RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] rewrite_alternative EOB[$RPAREN,\"<end-of-block>\"] ) ;
	private ANTLRParser.rewrite_block_return rewrite_block(  )
	{
		ANTLRParser.rewrite_block_return retval = new ANTLRParser.rewrite_block_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken LPAREN162=null;
		IToken RPAREN164=null;
		ANTLRParser.rewrite_alternative_return rewrite_alternative163 = default(ANTLRParser.rewrite_alternative_return);

		GrammarAST LPAREN162_tree=null;
		GrammarAST RPAREN164_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_rewrite_alternative=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_alternative");
		try
		{
			// Grammars\\ANTLR.g3:673:4: ( LPAREN rewrite_alternative RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] rewrite_alternative EOB[$RPAREN,\"<end-of-block>\"] ) )
			// Grammars\\ANTLR.g3:673:4: LPAREN rewrite_alternative RPAREN
			{
			LPAREN162=(IToken)Match(input,LPAREN,Follow._LPAREN_in_rewrite_block2838); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN162);

			PushFollow(Follow._rewrite_alternative_in_rewrite_block2842);
			rewrite_alternative163=rewrite_alternative();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite_alternative.Add(rewrite_alternative163.Tree);
			RPAREN164=(IToken)Match(input,RPAREN,Follow._RPAREN_in_rewrite_block2846); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN164);



			{
			// AST REWRITE
			// elements: rewrite_alternative
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 676:3: -> ^( BLOCK[$LPAREN,\"BLOCK\"] rewrite_alternative EOB[$RPAREN,\"<end-of-block>\"] )
			{
				// Grammars\\ANTLR.g3:676:6: ^( BLOCK[$LPAREN,\"BLOCK\"] rewrite_alternative EOB[$RPAREN,\"<end-of-block>\"] )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(BLOCK, LPAREN162, "BLOCK"), root_1);

				adaptor.AddChild(root_1, stream_rewrite_alternative.NextTree());
				adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOB, RPAREN164, "<end-of-block>"));

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_block"

	public class rewrite_alternative_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_alternative"
	// Grammars\\ANTLR.g3:679:0: rewrite_alternative options {k=1; } : ({...}? => rewrite_template |{...}? => ( rewrite_element )+ -> {!stream_rewrite_element.HasNext}? ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) -> ^( ALT[LT(1),\"ALT\"] ( rewrite_element )+ EOA[\"<end-of-alt>\"] ) | -> ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) |{...}? ETC );
	private ANTLRParser.rewrite_alternative_return rewrite_alternative(  )
	{
		ANTLRParser.rewrite_alternative_return retval = new ANTLRParser.rewrite_alternative_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken ETC167=null;
		ANTLRParser.rewrite_template_return rewrite_template165 = default(ANTLRParser.rewrite_template_return);
		ANTLRParser.rewrite_element_return rewrite_element166 = default(ANTLRParser.rewrite_element_return);

		GrammarAST ETC167_tree=null;
		RewriteRuleSubtreeStream stream_rewrite_element=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_element");
		try
		{
			// Grammars\\ANTLR.g3:681:4: ({...}? => rewrite_template |{...}? => ( rewrite_element )+ -> {!stream_rewrite_element.HasNext}? ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) -> ^( ALT[LT(1),\"ALT\"] ( rewrite_element )+ EOA[\"<end-of-alt>\"] ) | -> ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) |{...}? ETC )
			int alt83=4;
			alt83 = dfa83.Predict(input);
			switch ( alt83 )
			{
			case 1:
				// Grammars\\ANTLR.g3:681:4: {...}? => rewrite_template
				{
				root_0 = (GrammarAST)adaptor.Nil();

				if ( !((Grammar.BuildTemplate)) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "rewrite_alternative", "Grammar.BuildTemplate");
				}
				PushFollow(Follow._rewrite_template_in_rewrite_alternative2882);
				rewrite_template165=rewrite_template();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_template165.Tree);

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:683:4: {...}? => ( rewrite_element )+
				{
				if ( !((Grammar.BuildAST)) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "rewrite_alternative", "Grammar.BuildAST");
				}
				// Grammars\\ANTLR.g3:683:27: ( rewrite_element )+
				int cnt82=0;
				for ( ; ; )
				{
					int alt82=2;
					int LA82_0 = input.LA(1);

					if ( (LA82_0==ACTION||LA82_0==CHAR_LITERAL||LA82_0==DOLLAR||LA82_0==LPAREN||LA82_0==RULE_REF||LA82_0==STRING_LITERAL||LA82_0==TOKEN_REF||LA82_0==TREE_BEGIN) )
					{
						alt82=1;
					}


					switch ( alt82 )
					{
					case 1:
						// Grammars\\ANTLR.g3:683:29: rewrite_element
						{
						PushFollow(Follow._rewrite_element_in_rewrite_alternative2894);
						rewrite_element166=rewrite_element();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_rewrite_element.Add(rewrite_element166.Tree);

						}
						break;

					default:
						if ( cnt82 >= 1 )
							goto loop82;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee82 = new EarlyExitException( 82, input );
						throw eee82;
					}
					cnt82++;
				}
				loop82:
					;




				{
				// AST REWRITE
				// elements: rewrite_element
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 684:3: -> {!stream_rewrite_element.HasNext}? ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] )
				if (!stream_rewrite_element.HasNext)
				{
					// Grammars\\ANTLR.g3:684:41: ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ALT, LT(1), "ALT"), root_1);

					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EPSILON, "epsilon"));
					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOA, "<end-of-alt>"));

					adaptor.AddChild(root_0, root_1);
					}

				}
				else // 685:3: -> ^( ALT[LT(1),\"ALT\"] ( rewrite_element )+ EOA[\"<end-of-alt>\"] )
				{
					// Grammars\\ANTLR.g3:685:6: ^( ALT[LT(1),\"ALT\"] ( rewrite_element )+ EOA[\"<end-of-alt>\"] )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ALT, LT(1), "ALT"), root_1);

					if ( !(stream_rewrite_element.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_rewrite_element.HasNext )
					{
						adaptor.AddChild(root_1, stream_rewrite_element.NextTree());

					}
					stream_rewrite_element.Reset();
					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOA, "<end-of-alt>"));

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:688:3: 
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

				root_0 = (GrammarAST)adaptor.Nil();
				// 688:3: -> ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] )
				{
					// Grammars\\ANTLR.g3:688:6: ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ALT, LT(1), "ALT"), root_1);

					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EPSILON, "epsilon"));
					adaptor.AddChild(root_1, (GrammarAST)adaptor.Create(EOA, "<end-of-alt>"));

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:689:4: {...}? ETC
				{
				root_0 = (GrammarAST)adaptor.Nil();

				if ( !((Grammar.BuildAST)) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "rewrite_alternative", "Grammar.BuildAST");
				}
				ETC167=(IToken)Match(input,ETC,Follow._ETC_in_rewrite_alternative2955); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ETC167_tree = (GrammarAST)adaptor.Create(ETC167);
				adaptor.AddChild(root_0, ETC167_tree);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_alternative"

	public class rewrite_element_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_element"
	// Grammars\\ANTLR.g3:692:0: rewrite_element : ( (t= rewrite_atom -> $t) (subrule= ebnfSuffix[$t.tree,true] -> $subrule)? | rewrite_ebnf | (tr= rewrite_tree -> $tr) (subrule= ebnfSuffix[$tr.tree,true] -> $subrule)? );
	private ANTLRParser.rewrite_element_return rewrite_element(  )
	{
		ANTLRParser.rewrite_element_return retval = new ANTLRParser.rewrite_element_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		ANTLRParser.rewrite_atom_return t = default(ANTLRParser.rewrite_atom_return);
		ANTLRParser.ebnfSuffix_return subrule = default(ANTLRParser.ebnfSuffix_return);
		ANTLRParser.rewrite_tree_return tr = default(ANTLRParser.rewrite_tree_return);
		ANTLRParser.rewrite_ebnf_return rewrite_ebnf168 = default(ANTLRParser.rewrite_ebnf_return);

		RewriteRuleSubtreeStream stream_rewrite_atom=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_atom");
		RewriteRuleSubtreeStream stream_ebnfSuffix=new RewriteRuleSubtreeStream(adaptor,"rule ebnfSuffix");
		RewriteRuleSubtreeStream stream_rewrite_tree=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_tree");
		try
		{
			// Grammars\\ANTLR.g3:693:4: ( (t= rewrite_atom -> $t) (subrule= ebnfSuffix[$t.tree,true] -> $subrule)? | rewrite_ebnf | (tr= rewrite_tree -> $tr) (subrule= ebnfSuffix[$tr.tree,true] -> $subrule)? )
			int alt86=3;
			switch ( input.LA(1) )
			{
			case ACTION:
			case CHAR_LITERAL:
			case DOLLAR:
			case RULE_REF:
			case STRING_LITERAL:
			case TOKEN_REF:
				{
				alt86=1;
				}
				break;
			case LPAREN:
				{
				alt86=2;
				}
				break;
			case TREE_BEGIN:
				{
				alt86=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 86, 0, input);

					throw nvae;
				}
			}

			switch ( alt86 )
			{
			case 1:
				// Grammars\\ANTLR.g3:693:4: (t= rewrite_atom -> $t) (subrule= ebnfSuffix[$t.tree,true] -> $subrule)?
				{
				// Grammars\\ANTLR.g3:693:4: (t= rewrite_atom -> $t)
				// Grammars\\ANTLR.g3:693:6: t= rewrite_atom
				{
				PushFollow(Follow._rewrite_atom_in_rewrite_element2970);
				t=rewrite_atom();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rewrite_atom.Add(t.Tree);


				{
				// AST REWRITE
				// elements: t
				// token labels: 
				// rule labels: t, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_t=new RewriteRuleSubtreeStream(adaptor,"rule t",t!=null?t.tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 694:4: -> $t
				{
					adaptor.AddChild(root_0, stream_t.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:696:3: (subrule= ebnfSuffix[$t.tree,true] -> $subrule)?
				int alt84=2;
				int LA84_0 = input.LA(1);

				if ( (LA84_0==PLUS||LA84_0==QUESTION||LA84_0==STAR) )
				{
					alt84=1;
				}
				switch ( alt84 )
				{
				case 1:
					// Grammars\\ANTLR.g3:696:5: subrule= ebnfSuffix[$t.tree,true]
					{
					PushFollow(Follow._ebnfSuffix_in_rewrite_element2990);
					subrule=ebnfSuffix((t!=null?((GrammarAST)t.tree):null), true);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_ebnfSuffix.Add(subrule.Tree);


					{
					// AST REWRITE
					// elements: subrule
					// token labels: 
					// rule labels: subrule, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.tree = root_0;
					RewriteRuleSubtreeStream stream_subrule=new RewriteRuleSubtreeStream(adaptor,"rule subrule",subrule!=null?subrule.tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

					root_0 = (GrammarAST)adaptor.Nil();
					// 697:4: -> $subrule
					{
						adaptor.AddChild(root_0, stream_subrule.NextTree());

					}

					retval.tree = root_0;
					}
					}

					}
					break;

				}


				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:699:4: rewrite_ebnf
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._rewrite_ebnf_in_rewrite_element3009);
				rewrite_ebnf168=rewrite_ebnf();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_ebnf168.Tree);

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:700:4: (tr= rewrite_tree -> $tr) (subrule= ebnfSuffix[$tr.tree,true] -> $subrule)?
				{
				// Grammars\\ANTLR.g3:700:4: (tr= rewrite_tree -> $tr)
				// Grammars\\ANTLR.g3:700:6: tr= rewrite_tree
				{
				PushFollow(Follow._rewrite_tree_in_rewrite_element3018);
				tr=rewrite_tree();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rewrite_tree.Add(tr.Tree);


				{
				// AST REWRITE
				// elements: tr
				// token labels: 
				// rule labels: tr, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_tr=new RewriteRuleSubtreeStream(adaptor,"rule tr",tr!=null?tr.tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 701:4: -> $tr
				{
					adaptor.AddChild(root_0, stream_tr.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:703:3: (subrule= ebnfSuffix[$tr.tree,true] -> $subrule)?
				int alt85=2;
				int LA85_0 = input.LA(1);

				if ( (LA85_0==PLUS||LA85_0==QUESTION||LA85_0==STAR) )
				{
					alt85=1;
				}
				switch ( alt85 )
				{
				case 1:
					// Grammars\\ANTLR.g3:703:5: subrule= ebnfSuffix[$tr.tree,true]
					{
					PushFollow(Follow._ebnfSuffix_in_rewrite_element3038);
					subrule=ebnfSuffix((tr!=null?((GrammarAST)tr.tree):null), true);

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_ebnfSuffix.Add(subrule.Tree);


					{
					// AST REWRITE
					// elements: subrule
					// token labels: 
					// rule labels: subrule, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.tree = root_0;
					RewriteRuleSubtreeStream stream_subrule=new RewriteRuleSubtreeStream(adaptor,"rule subrule",subrule!=null?subrule.tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

					root_0 = (GrammarAST)adaptor.Nil();
					// 704:4: -> $subrule
					{
						adaptor.AddChild(root_0, stream_subrule.NextTree());

					}

					retval.tree = root_0;
					}
					}

					}
					break;

				}


				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_element"

	public class rewrite_atom_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_atom"
	// Grammars\\ANTLR.g3:708:0: rewrite_atom : ( (tr= TOKEN_REF -> $tr) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )? ( ARG_ACTION )? -> ^( $rewrite_atom ( ARG_ACTION )? ) |rr= RULE_REF | (cl= CHAR_LITERAL -> $cl) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )? | (sl= STRING_LITERAL -> $sl) (eo= elementOptions[(GrammarAST)$tree.GetChild(0)] )? |d= DOLLAR i= id -> LABEL[$i.start,$i.text] | ACTION );
	private ANTLRParser.rewrite_atom_return rewrite_atom(  )
	{
		ANTLRParser.rewrite_atom_return retval = new ANTLRParser.rewrite_atom_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken tr=null;
		IToken rr=null;
		IToken cl=null;
		IToken sl=null;
		IToken d=null;
		IToken ARG_ACTION170=null;
		IToken ACTION172=null;
		ANTLRParser.elementOptions_return eo = default(ANTLRParser.elementOptions_return);
		ANTLRParser.id_return i = default(ANTLRParser.id_return);
		ANTLRParser.elementOptions_return elementOptions169 = default(ANTLRParser.elementOptions_return);
		ANTLRParser.elementOptions_return elementOptions171 = default(ANTLRParser.elementOptions_return);

		GrammarAST tr_tree=null;
		GrammarAST rr_tree=null;
		GrammarAST cl_tree=null;
		GrammarAST sl_tree=null;
		GrammarAST d_tree=null;
		GrammarAST ARG_ACTION170_tree=null;
		GrammarAST ACTION172_tree=null;
		RewriteRuleITokenStream stream_TOKEN_REF=new RewriteRuleITokenStream(adaptor,"token TOKEN_REF");
		RewriteRuleITokenStream stream_ARG_ACTION=new RewriteRuleITokenStream(adaptor,"token ARG_ACTION");
		RewriteRuleITokenStream stream_CHAR_LITERAL=new RewriteRuleITokenStream(adaptor,"token CHAR_LITERAL");
		RewriteRuleITokenStream stream_STRING_LITERAL=new RewriteRuleITokenStream(adaptor,"token STRING_LITERAL");
		RewriteRuleITokenStream stream_DOLLAR=new RewriteRuleITokenStream(adaptor,"token DOLLAR");
		RewriteRuleSubtreeStream stream_elementOptions=new RewriteRuleSubtreeStream(adaptor,"rule elementOptions");
		RewriteRuleSubtreeStream stream_id=new RewriteRuleSubtreeStream(adaptor,"rule id");

		GrammarAST subrule=null;

		try
		{
			// Grammars\\ANTLR.g3:713:4: ( (tr= TOKEN_REF -> $tr) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )? ( ARG_ACTION )? -> ^( $rewrite_atom ( ARG_ACTION )? ) |rr= RULE_REF | (cl= CHAR_LITERAL -> $cl) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )? | (sl= STRING_LITERAL -> $sl) (eo= elementOptions[(GrammarAST)$tree.GetChild(0)] )? |d= DOLLAR i= id -> LABEL[$i.start,$i.text] | ACTION )
			int alt91=6;
			switch ( input.LA(1) )
			{
			case TOKEN_REF:
				{
				alt91=1;
				}
				break;
			case RULE_REF:
				{
				alt91=2;
				}
				break;
			case CHAR_LITERAL:
				{
				alt91=3;
				}
				break;
			case STRING_LITERAL:
				{
				alt91=4;
				}
				break;
			case DOLLAR:
				{
				alt91=5;
				}
				break;
			case ACTION:
				{
				alt91=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 91, 0, input);

					throw nvae;
				}
			}

			switch ( alt91 )
			{
			case 1:
				// Grammars\\ANTLR.g3:713:4: (tr= TOKEN_REF -> $tr) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )? ( ARG_ACTION )?
				{
				// Grammars\\ANTLR.g3:713:4: (tr= TOKEN_REF -> $tr)
				// Grammars\\ANTLR.g3:713:6: tr= TOKEN_REF
				{
				tr=(IToken)Match(input,TOKEN_REF,Follow._TOKEN_REF_in_rewrite_atom3072); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TOKEN_REF.Add(tr);



				{
				// AST REWRITE
				// elements: tr
				// token labels: tr
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleITokenStream stream_tr=new RewriteRuleITokenStream(adaptor,"token tr",tr);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 714:4: -> $tr
				{
					adaptor.AddChild(root_0, stream_tr.NextNode());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:716:3: ( elementOptions[(GrammarAST)$tree.GetChild(0)] )?
				int alt87=2;
				int LA87_0 = input.LA(1);

				if ( (LA87_0==OPEN_ELEMENT_OPTION) )
				{
					alt87=1;
				}
				switch ( alt87 )
				{
				case 1:
					// Grammars\\ANTLR.g3:716:4: elementOptions[(GrammarAST)$tree.GetChild(0)]
					{
					PushFollow(Follow._elementOptions_in_rewrite_atom3089);
					elementOptions169=elementOptions((GrammarAST)((GrammarAST)retval.tree).GetChild(0));

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_elementOptions.Add(elementOptions169.Tree);

					}
					break;

				}

				// Grammars\\ANTLR.g3:716:52: ( ARG_ACTION )?
				int alt88=2;
				int LA88_0 = input.LA(1);

				if ( (LA88_0==ARG_ACTION) )
				{
					alt88=1;
				}
				switch ( alt88 )
				{
				case 1:
					// Grammars\\ANTLR.g3:716:53: ARG_ACTION
					{
					ARG_ACTION170=(IToken)Match(input,ARG_ACTION,Follow._ARG_ACTION_in_rewrite_atom3095); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_ARG_ACTION.Add(ARG_ACTION170);


					}
					break;

				}



				{
				// AST REWRITE
				// elements: rewrite_atom, ARG_ACTION
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 717:3: -> ^( $rewrite_atom ( ARG_ACTION )? )
				{
					// Grammars\\ANTLR.g3:717:6: ^( $rewrite_atom ( ARG_ACTION )? )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot(stream_retval.NextNode(), root_1);

					// Grammars\\ANTLR.g3:717:22: ( ARG_ACTION )?
					if ( stream_ARG_ACTION.HasNext )
					{
						adaptor.AddChild(root_1, stream_ARG_ACTION.NextNode());

					}
					stream_ARG_ACTION.Reset();

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:718:4: rr= RULE_REF
				{
				root_0 = (GrammarAST)adaptor.Nil();

				rr=(IToken)Match(input,RULE_REF,Follow._RULE_REF_in_rewrite_atom3117); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				rr_tree = (GrammarAST)adaptor.Create(rr);
				adaptor.AddChild(root_0, rr_tree);
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:719:4: (cl= CHAR_LITERAL -> $cl) ( elementOptions[(GrammarAST)$tree.GetChild(0)] )?
				{
				// Grammars\\ANTLR.g3:719:4: (cl= CHAR_LITERAL -> $cl)
				// Grammars\\ANTLR.g3:719:6: cl= CHAR_LITERAL
				{
				cl=(IToken)Match(input,CHAR_LITERAL,Follow._CHAR_LITERAL_in_rewrite_atom3126); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_CHAR_LITERAL.Add(cl);



				{
				// AST REWRITE
				// elements: cl
				// token labels: cl
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleITokenStream stream_cl=new RewriteRuleITokenStream(adaptor,"token cl",cl);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 720:4: -> $cl
				{
					adaptor.AddChild(root_0, stream_cl.NextNode());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:722:3: ( elementOptions[(GrammarAST)$tree.GetChild(0)] )?
				int alt89=2;
				int LA89_0 = input.LA(1);

				if ( (LA89_0==OPEN_ELEMENT_OPTION) )
				{
					alt89=1;
				}
				switch ( alt89 )
				{
				case 1:
					// Grammars\\ANTLR.g3:722:4: elementOptions[(GrammarAST)$tree.GetChild(0)]
					{
					PushFollow(Follow._elementOptions_in_rewrite_atom3143);
					elementOptions171=elementOptions((GrammarAST)((GrammarAST)retval.tree).GetChild(0));

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_elementOptions.Add(elementOptions171.Tree);

					}
					break;

				}


				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:723:4: (sl= STRING_LITERAL -> $sl) (eo= elementOptions[(GrammarAST)$tree.GetChild(0)] )?
				{
				// Grammars\\ANTLR.g3:723:4: (sl= STRING_LITERAL -> $sl)
				// Grammars\\ANTLR.g3:723:6: sl= STRING_LITERAL
				{
				sl=(IToken)Match(input,STRING_LITERAL,Follow._STRING_LITERAL_in_rewrite_atom3155); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STRING_LITERAL.Add(sl);



				{
				// AST REWRITE
				// elements: sl
				// token labels: sl
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleITokenStream stream_sl=new RewriteRuleITokenStream(adaptor,"token sl",sl);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 724:4: -> $sl
				{
					adaptor.AddChild(root_0, stream_sl.NextNode());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:726:3: (eo= elementOptions[(GrammarAST)$tree.GetChild(0)] )?
				int alt90=2;
				int LA90_0 = input.LA(1);

				if ( (LA90_0==OPEN_ELEMENT_OPTION) )
				{
					alt90=1;
				}
				switch ( alt90 )
				{
				case 1:
					// Grammars\\ANTLR.g3:726:4: eo= elementOptions[(GrammarAST)$tree.GetChild(0)]
					{
					PushFollow(Follow._elementOptions_in_rewrite_atom3174);
					eo=elementOptions((GrammarAST)((GrammarAST)retval.tree).GetChild(0));

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_elementOptions.Add(eo.Tree);

					}
					break;

				}


				}
				break;
			case 5:
				// Grammars\\ANTLR.g3:727:4: d= DOLLAR i= id
				{
				d=(IToken)Match(input,DOLLAR,Follow._DOLLAR_in_rewrite_atom3184); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_DOLLAR.Add(d);

				PushFollow(Follow._id_in_rewrite_atom3188);
				i=id();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_id.Add(i.Tree);


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

				root_0 = (GrammarAST)adaptor.Nil();
				// 728:3: -> LABEL[$i.start,$i.text]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(LABEL, (i!=null?((IToken)i.start):null), (i!=null?input.ToString(i.start,i.stop):null)));

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 6:
				// Grammars\\ANTLR.g3:729:4: ACTION
				{
				root_0 = (GrammarAST)adaptor.Nil();

				ACTION172=(IToken)Match(input,ACTION,Follow._ACTION_in_rewrite_atom3201); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ACTION172_tree = (GrammarAST)adaptor.Create(ACTION172);
				adaptor.AddChild(root_0, ACTION172_tree);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_atom"

	public class rewrite_ebnf_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_ebnf"
	// Grammars\\ANTLR.g3:732:0: rewrite_ebnf : b= rewrite_block ( QUESTION -> ^( OPTIONAL[$b.start,\"?\"] $b) | STAR -> ^( CLOSURE[$b.start,\"*\"] $b) | PLUS -> ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b) ) ;
	private ANTLRParser.rewrite_ebnf_return rewrite_ebnf(  )
	{
		ANTLRParser.rewrite_ebnf_return retval = new ANTLRParser.rewrite_ebnf_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken QUESTION173=null;
		IToken STAR174=null;
		IToken PLUS175=null;
		ANTLRParser.rewrite_block_return b = default(ANTLRParser.rewrite_block_return);

		GrammarAST QUESTION173_tree=null;
		GrammarAST STAR174_tree=null;
		GrammarAST PLUS175_tree=null;
		RewriteRuleITokenStream stream_QUESTION=new RewriteRuleITokenStream(adaptor,"token QUESTION");
		RewriteRuleITokenStream stream_STAR=new RewriteRuleITokenStream(adaptor,"token STAR");
		RewriteRuleITokenStream stream_PLUS=new RewriteRuleITokenStream(adaptor,"token PLUS");
		RewriteRuleSubtreeStream stream_rewrite_block=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_block");
		try
		{
			// Grammars\\ANTLR.g3:733:4: (b= rewrite_block ( QUESTION -> ^( OPTIONAL[$b.start,\"?\"] $b) | STAR -> ^( CLOSURE[$b.start,\"*\"] $b) | PLUS -> ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b) ) )
			// Grammars\\ANTLR.g3:733:4: b= rewrite_block ( QUESTION -> ^( OPTIONAL[$b.start,\"?\"] $b) | STAR -> ^( CLOSURE[$b.start,\"*\"] $b) | PLUS -> ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b) )
			{
			PushFollow(Follow._rewrite_block_in_rewrite_ebnf3214);
			b=rewrite_block();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite_block.Add(b.Tree);
			// Grammars\\ANTLR.g3:734:3: ( QUESTION -> ^( OPTIONAL[$b.start,\"?\"] $b) | STAR -> ^( CLOSURE[$b.start,\"*\"] $b) | PLUS -> ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b) )
			int alt92=3;
			switch ( input.LA(1) )
			{
			case QUESTION:
				{
				alt92=1;
				}
				break;
			case STAR:
				{
				alt92=2;
				}
				break;
			case PLUS:
				{
				alt92=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 92, 0, input);

					throw nvae;
				}
			}

			switch ( alt92 )
			{
			case 1:
				// Grammars\\ANTLR.g3:734:5: QUESTION
				{
				QUESTION173=(IToken)Match(input,QUESTION,Follow._QUESTION_in_rewrite_ebnf3220); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_QUESTION.Add(QUESTION173);



				{
				// AST REWRITE
				// elements: b
				// token labels: 
				// rule labels: b, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_b=new RewriteRuleSubtreeStream(adaptor,"rule b",b!=null?b.tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 735:4: -> ^( OPTIONAL[$b.start,\"?\"] $b)
				{
					// Grammars\\ANTLR.g3:735:7: ^( OPTIONAL[$b.start,\"?\"] $b)
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(OPTIONAL, (b!=null?((IToken)b.start):null), "?"), root_1);

					adaptor.AddChild(root_1, stream_b.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:736:5: STAR
				{
				STAR174=(IToken)Match(input,STAR,Follow._STAR_in_rewrite_ebnf3239); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STAR.Add(STAR174);



				{
				// AST REWRITE
				// elements: b
				// token labels: 
				// rule labels: b, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_b=new RewriteRuleSubtreeStream(adaptor,"rule b",b!=null?b.tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 737:4: -> ^( CLOSURE[$b.start,\"*\"] $b)
				{
					// Grammars\\ANTLR.g3:737:7: ^( CLOSURE[$b.start,\"*\"] $b)
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(CLOSURE, (b!=null?((IToken)b.start):null), "*"), root_1);

					adaptor.AddChild(root_1, stream_b.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:738:5: PLUS
				{
				PLUS175=(IToken)Match(input,PLUS,Follow._PLUS_in_rewrite_ebnf3258); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PLUS.Add(PLUS175);



				{
				// AST REWRITE
				// elements: b
				// token labels: 
				// rule labels: b, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_b=new RewriteRuleSubtreeStream(adaptor,"rule b",b!=null?b.tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 739:4: -> ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b)
				{
					// Grammars\\ANTLR.g3:739:7: ^( POSITIVE_CLOSURE[$b.start,\"+\"] $b)
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(POSITIVE_CLOSURE, (b!=null?((IToken)b.start):null), "+"), root_1);

					adaptor.AddChild(root_1, stream_b.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}


			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_ebnf"

	public class rewrite_tree_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_tree"
	// Grammars\\ANTLR.g3:743:0: rewrite_tree : TREE_BEGIN rewrite_atom ( rewrite_element )* RPAREN ;
	private ANTLRParser.rewrite_tree_return rewrite_tree(  )
	{
		ANTLRParser.rewrite_tree_return retval = new ANTLRParser.rewrite_tree_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken TREE_BEGIN176=null;
		IToken RPAREN179=null;
		ANTLRParser.rewrite_atom_return rewrite_atom177 = default(ANTLRParser.rewrite_atom_return);
		ANTLRParser.rewrite_element_return rewrite_element178 = default(ANTLRParser.rewrite_element_return);

		GrammarAST TREE_BEGIN176_tree=null;
		GrammarAST RPAREN179_tree=null;

		try
		{
			// Grammars\\ANTLR.g3:744:4: ( TREE_BEGIN rewrite_atom ( rewrite_element )* RPAREN )
			// Grammars\\ANTLR.g3:744:4: TREE_BEGIN rewrite_atom ( rewrite_element )* RPAREN
			{
			root_0 = (GrammarAST)adaptor.Nil();

			TREE_BEGIN176=(IToken)Match(input,TREE_BEGIN,Follow._TREE_BEGIN_in_rewrite_tree3286); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			TREE_BEGIN176_tree = (GrammarAST)adaptor.Create(TREE_BEGIN176);
			root_0 = (GrammarAST)adaptor.BecomeRoot(TREE_BEGIN176_tree, root_0);
			}
			PushFollow(Follow._rewrite_atom_in_rewrite_tree3292);
			rewrite_atom177=rewrite_atom();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_atom177.Tree);
			// Grammars\\ANTLR.g3:745:17: ( rewrite_element )*
			for ( ; ; )
			{
				int alt93=2;
				int LA93_0 = input.LA(1);

				if ( (LA93_0==ACTION||LA93_0==CHAR_LITERAL||LA93_0==DOLLAR||LA93_0==LPAREN||LA93_0==RULE_REF||LA93_0==STRING_LITERAL||LA93_0==TOKEN_REF||LA93_0==TREE_BEGIN) )
				{
					alt93=1;
				}


				switch ( alt93 )
				{
				case 1:
					// Grammars\\ANTLR.g3:745:19: rewrite_element
					{
					PushFollow(Follow._rewrite_element_in_rewrite_tree3296);
					rewrite_element178=rewrite_element();

					state._fsp--;
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_element178.Tree);

					}
					break;

				default:
					goto loop93;
				}
			}

			loop93:
				;


			RPAREN179=(IToken)Match(input,RPAREN,Follow._RPAREN_in_rewrite_tree3303); if (state.failed) return retval;

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_tree"

	public class rewrite_template_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_template"
	// Grammars\\ANTLR.g3:761:0: public rewrite_template options {k=1; } : ({...}? => ( rewrite_template_head -> rewrite_template_head ) (st= DOUBLE_QUOTE_STRING_LITERAL |st= DOUBLE_ANGLE_STRING_LITERAL ) | rewrite_template_head | rewrite_indirect_template_head | ACTION );
	public ANTLRParser.rewrite_template_return rewrite_template(  )
	{
		ANTLRParser.rewrite_template_return retval = new ANTLRParser.rewrite_template_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken st=null;
		IToken ACTION183=null;
		ANTLRParser.rewrite_template_head_return rewrite_template_head180 = default(ANTLRParser.rewrite_template_head_return);
		ANTLRParser.rewrite_template_head_return rewrite_template_head181 = default(ANTLRParser.rewrite_template_head_return);
		ANTLRParser.rewrite_indirect_template_head_return rewrite_indirect_template_head182 = default(ANTLRParser.rewrite_indirect_template_head_return);

		GrammarAST st_tree=null;
		GrammarAST ACTION183_tree=null;
		RewriteRuleITokenStream stream_DOUBLE_QUOTE_STRING_LITERAL=new RewriteRuleITokenStream(adaptor,"token DOUBLE_QUOTE_STRING_LITERAL");
		RewriteRuleITokenStream stream_DOUBLE_ANGLE_STRING_LITERAL=new RewriteRuleITokenStream(adaptor,"token DOUBLE_ANGLE_STRING_LITERAL");
		RewriteRuleSubtreeStream stream_rewrite_template_head=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_template_head");
		try
		{
			// Grammars\\ANTLR.g3:764:3: ({...}? => ( rewrite_template_head -> rewrite_template_head ) (st= DOUBLE_QUOTE_STRING_LITERAL |st= DOUBLE_ANGLE_STRING_LITERAL ) | rewrite_template_head | rewrite_indirect_template_head | ACTION )
			int alt95=4;
			switch ( input.LA(1) )
			{
			case TOKEN_REF:
				{
				int LA95_1 = input.LA(2);

				if ( ((LT(1).Text.Equals("template"))) )
				{
					alt95=1;
				}
				else if ( (true) )
				{
					alt95=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 95, 1, input);

					throw nvae;
				}
				}
				break;
			case RULE_REF:
				{
				int LA95_2 = input.LA(2);

				if ( ((LT(1).Text.Equals("template"))) )
				{
					alt95=1;
				}
				else if ( (true) )
				{
					alt95=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 95, 2, input);

					throw nvae;
				}
				}
				break;
			case LPAREN:
				{
				alt95=3;
				}
				break;
			case ACTION:
				{
				alt95=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 95, 0, input);

					throw nvae;
				}
			}

			switch ( alt95 )
			{
			case 1:
				// Grammars\\ANTLR.g3:764:3: {...}? => ( rewrite_template_head -> rewrite_template_head ) (st= DOUBLE_QUOTE_STRING_LITERAL |st= DOUBLE_ANGLE_STRING_LITERAL )
				{
				if ( !((LT(1).Text.Equals("template"))) )
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					throw new FailedPredicateException(input, "rewrite_template", "LT(1).Text.Equals(\"template\")");
				}
				// Grammars\\ANTLR.g3:765:3: ( rewrite_template_head -> rewrite_template_head )
				// Grammars\\ANTLR.g3:765:5: rewrite_template_head
				{
				PushFollow(Follow._rewrite_template_head_in_rewrite_template3338);
				rewrite_template_head180=rewrite_template_head();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rewrite_template_head.Add(rewrite_template_head180.Tree);


				{
				// AST REWRITE
				// elements: rewrite_template_head
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 766:4: -> rewrite_template_head
				{
					adaptor.AddChild(root_0, stream_rewrite_template_head.NextTree());

				}

				retval.tree = root_0;
				}
				}

				}

				// Grammars\\ANTLR.g3:768:3: (st= DOUBLE_QUOTE_STRING_LITERAL |st= DOUBLE_ANGLE_STRING_LITERAL )
				int alt94=2;
				int LA94_0 = input.LA(1);

				if ( (LA94_0==DOUBLE_QUOTE_STRING_LITERAL) )
				{
					alt94=1;
				}
				else if ( (LA94_0==DOUBLE_ANGLE_STRING_LITERAL) )
				{
					alt94=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 94, 0, input);

					throw nvae;
				}
				switch ( alt94 )
				{
				case 1:
					// Grammars\\ANTLR.g3:768:5: st= DOUBLE_QUOTE_STRING_LITERAL
					{
					st=(IToken)Match(input,DOUBLE_QUOTE_STRING_LITERAL,Follow._DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template3357); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_DOUBLE_QUOTE_STRING_LITERAL.Add(st);


					}
					break;
				case 2:
					// Grammars\\ANTLR.g3:768:38: st= DOUBLE_ANGLE_STRING_LITERAL
					{
					st=(IToken)Match(input,DOUBLE_ANGLE_STRING_LITERAL,Follow._DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template3363); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_DOUBLE_ANGLE_STRING_LITERAL.Add(st);


					}
					break;

				}

				if ( state.backtracking == 0 )
				{
					 adaptor.AddChild( ((GrammarAST)retval.tree).GetChild(0), adaptor.Create(st) ); 
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:772:3: rewrite_template_head
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._rewrite_template_head_in_rewrite_template3378);
				rewrite_template_head181=rewrite_template_head();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_template_head181.Tree);

				}
				break;
			case 3:
				// Grammars\\ANTLR.g3:775:3: rewrite_indirect_template_head
				{
				root_0 = (GrammarAST)adaptor.Nil();

				PushFollow(Follow._rewrite_indirect_template_head_in_rewrite_template3387);
				rewrite_indirect_template_head182=rewrite_indirect_template_head();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rewrite_indirect_template_head182.Tree);

				}
				break;
			case 4:
				// Grammars\\ANTLR.g3:778:3: ACTION
				{
				root_0 = (GrammarAST)adaptor.Nil();

				ACTION183=(IToken)Match(input,ACTION,Follow._ACTION_in_rewrite_template3396); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				ACTION183_tree = (GrammarAST)adaptor.Create(ACTION183);
				adaptor.AddChild(root_0, ACTION183_tree);
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_template"

	public class rewrite_template_head_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_template_head"
	// Grammars\\ANTLR.g3:782:0: rewrite_template_head : id lp= LPAREN rewrite_template_args RPAREN -> ^( TEMPLATE[$lp,\"TEMPLATE\"] id rewrite_template_args ) ;
	private ANTLRParser.rewrite_template_head_return rewrite_template_head(  )
	{
		ANTLRParser.rewrite_template_head_return retval = new ANTLRParser.rewrite_template_head_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken lp=null;
		IToken RPAREN186=null;
		ANTLRParser.id_return id184 = default(ANTLRParser.id_return);
		ANTLRParser.rewrite_template_args_return rewrite_template_args185 = default(ANTLRParser.rewrite_template_args_return);

		GrammarAST lp_tree=null;
		GrammarAST RPAREN186_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_id=new RewriteRuleSubtreeStream(adaptor,"rule id");
		RewriteRuleSubtreeStream stream_rewrite_template_args=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_template_args");
		try
		{
			// Grammars\\ANTLR.g3:783:4: ( id lp= LPAREN rewrite_template_args RPAREN -> ^( TEMPLATE[$lp,\"TEMPLATE\"] id rewrite_template_args ) )
			// Grammars\\ANTLR.g3:783:4: id lp= LPAREN rewrite_template_args RPAREN
			{
			PushFollow(Follow._id_in_rewrite_template_head3409);
			id184=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_id.Add(id184.Tree);
			lp=(IToken)Match(input,LPAREN,Follow._LPAREN_in_rewrite_template_head3413); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(lp);

			PushFollow(Follow._rewrite_template_args_in_rewrite_template_head3417);
			rewrite_template_args185=rewrite_template_args();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite_template_args.Add(rewrite_template_args185.Tree);
			RPAREN186=(IToken)Match(input,RPAREN,Follow._RPAREN_in_rewrite_template_head3421); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN186);



			{
			// AST REWRITE
			// elements: id, rewrite_template_args
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 786:3: -> ^( TEMPLATE[$lp,\"TEMPLATE\"] id rewrite_template_args )
			{
				// Grammars\\ANTLR.g3:786:6: ^( TEMPLATE[$lp,\"TEMPLATE\"] id rewrite_template_args )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(TEMPLATE, lp, "TEMPLATE"), root_1);

				adaptor.AddChild(root_1, stream_id.NextTree());
				adaptor.AddChild(root_1, stream_rewrite_template_args.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_template_head"

	public class rewrite_indirect_template_head_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_indirect_template_head"
	// Grammars\\ANTLR.g3:790:0: rewrite_indirect_template_head : lp= LPAREN ACTION RPAREN LPAREN rewrite_template_args RPAREN -> ^( TEMPLATE[$lp,\"TEMPLATE\"] ACTION rewrite_template_args ) ;
	private ANTLRParser.rewrite_indirect_template_head_return rewrite_indirect_template_head(  )
	{
		ANTLRParser.rewrite_indirect_template_head_return retval = new ANTLRParser.rewrite_indirect_template_head_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken lp=null;
		IToken ACTION187=null;
		IToken RPAREN188=null;
		IToken LPAREN189=null;
		IToken RPAREN191=null;
		ANTLRParser.rewrite_template_args_return rewrite_template_args190 = default(ANTLRParser.rewrite_template_args_return);

		GrammarAST lp_tree=null;
		GrammarAST ACTION187_tree=null;
		GrammarAST RPAREN188_tree=null;
		GrammarAST LPAREN189_tree=null;
		GrammarAST RPAREN191_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_ACTION=new RewriteRuleITokenStream(adaptor,"token ACTION");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_rewrite_template_args=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_template_args");
		try
		{
			// Grammars\\ANTLR.g3:791:4: (lp= LPAREN ACTION RPAREN LPAREN rewrite_template_args RPAREN -> ^( TEMPLATE[$lp,\"TEMPLATE\"] ACTION rewrite_template_args ) )
			// Grammars\\ANTLR.g3:791:4: lp= LPAREN ACTION RPAREN LPAREN rewrite_template_args RPAREN
			{
			lp=(IToken)Match(input,LPAREN,Follow._LPAREN_in_rewrite_indirect_template_head3449); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(lp);

			ACTION187=(IToken)Match(input,ACTION,Follow._ACTION_in_rewrite_indirect_template_head3453); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_ACTION.Add(ACTION187);

			RPAREN188=(IToken)Match(input,RPAREN,Follow._RPAREN_in_rewrite_indirect_template_head3457); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN188);

			LPAREN189=(IToken)Match(input,LPAREN,Follow._LPAREN_in_rewrite_indirect_template_head3461); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN189);

			PushFollow(Follow._rewrite_template_args_in_rewrite_indirect_template_head3463);
			rewrite_template_args190=rewrite_template_args();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_rewrite_template_args.Add(rewrite_template_args190.Tree);
			RPAREN191=(IToken)Match(input,RPAREN,Follow._RPAREN_in_rewrite_indirect_template_head3465); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN191);



			{
			// AST REWRITE
			// elements: ACTION, rewrite_template_args
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 795:3: -> ^( TEMPLATE[$lp,\"TEMPLATE\"] ACTION rewrite_template_args )
			{
				// Grammars\\ANTLR.g3:795:6: ^( TEMPLATE[$lp,\"TEMPLATE\"] ACTION rewrite_template_args )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(TEMPLATE, lp, "TEMPLATE"), root_1);

				adaptor.AddChild(root_1, stream_ACTION.NextNode());
				adaptor.AddChild(root_1, stream_rewrite_template_args.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_indirect_template_head"

	public class rewrite_template_args_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_template_args"
	// Grammars\\ANTLR.g3:798:0: rewrite_template_args : ( rewrite_template_arg ( COMMA rewrite_template_arg )* -> ^( ARGLIST[\"ARGLIST\"] ( rewrite_template_arg )+ ) | -> ARGLIST[\"ARGLIST\"] );
	private ANTLRParser.rewrite_template_args_return rewrite_template_args(  )
	{
		ANTLRParser.rewrite_template_args_return retval = new ANTLRParser.rewrite_template_args_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken COMMA193=null;
		ANTLRParser.rewrite_template_arg_return rewrite_template_arg192 = default(ANTLRParser.rewrite_template_arg_return);
		ANTLRParser.rewrite_template_arg_return rewrite_template_arg194 = default(ANTLRParser.rewrite_template_arg_return);

		GrammarAST COMMA193_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_rewrite_template_arg=new RewriteRuleSubtreeStream(adaptor,"rule rewrite_template_arg");
		try
		{
			// Grammars\\ANTLR.g3:799:4: ( rewrite_template_arg ( COMMA rewrite_template_arg )* -> ^( ARGLIST[\"ARGLIST\"] ( rewrite_template_arg )+ ) | -> ARGLIST[\"ARGLIST\"] )
			int alt97=2;
			int LA97_0 = input.LA(1);

			if ( (LA97_0==RULE_REF||LA97_0==TOKEN_REF) )
			{
				alt97=1;
			}
			else if ( (LA97_0==RPAREN) )
			{
				alt97=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 97, 0, input);

				throw nvae;
			}
			switch ( alt97 )
			{
			case 1:
				// Grammars\\ANTLR.g3:799:4: rewrite_template_arg ( COMMA rewrite_template_arg )*
				{
				PushFollow(Follow._rewrite_template_arg_in_rewrite_template_args3489);
				rewrite_template_arg192=rewrite_template_arg();

				state._fsp--;
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rewrite_template_arg.Add(rewrite_template_arg192.Tree);
				// Grammars\\ANTLR.g3:799:25: ( COMMA rewrite_template_arg )*
				for ( ; ; )
				{
					int alt96=2;
					int LA96_0 = input.LA(1);

					if ( (LA96_0==COMMA) )
					{
						alt96=1;
					}


					switch ( alt96 )
					{
					case 1:
						// Grammars\\ANTLR.g3:799:26: COMMA rewrite_template_arg
						{
						COMMA193=(IToken)Match(input,COMMA,Follow._COMMA_in_rewrite_template_args3492); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA193);

						PushFollow(Follow._rewrite_template_arg_in_rewrite_template_args3494);
						rewrite_template_arg194=rewrite_template_arg();

						state._fsp--;
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_rewrite_template_arg.Add(rewrite_template_arg194.Tree);

						}
						break;

					default:
						goto loop96;
					}
				}

				loop96:
					;




				{
				// AST REWRITE
				// elements: rewrite_template_arg
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

				root_0 = (GrammarAST)adaptor.Nil();
				// 800:3: -> ^( ARGLIST[\"ARGLIST\"] ( rewrite_template_arg )+ )
				{
					// Grammars\\ANTLR.g3:800:6: ^( ARGLIST[\"ARGLIST\"] ( rewrite_template_arg )+ )
					{
					GrammarAST root_1 = (GrammarAST)adaptor.Nil();
					root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ARGLIST, "ARGLIST"), root_1);

					if ( !(stream_rewrite_template_arg.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_rewrite_template_arg.HasNext )
					{
						adaptor.AddChild(root_1, stream_rewrite_template_arg.NextTree());

					}
					stream_rewrite_template_arg.Reset();

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.tree = root_0;
				}
				}

				}
				break;
			case 2:
				// Grammars\\ANTLR.g3:802:3: 
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

				root_0 = (GrammarAST)adaptor.Nil();
				// 802:3: -> ARGLIST[\"ARGLIST\"]
				{
					adaptor.AddChild(root_0, (GrammarAST)adaptor.Create(ARGLIST, "ARGLIST"));

				}

				retval.tree = root_0;
				}
				}

				}
				break;

			}
			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_template_args"

	public class rewrite_template_arg_return : ParserRuleReturnScope
	{
		public GrammarAST tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "rewrite_template_arg"
	// Grammars\\ANTLR.g3:805:0: rewrite_template_arg : id a= ASSIGN ACTION -> ^( ARG[$a,\"ARG\"] id ACTION ) ;
	private ANTLRParser.rewrite_template_arg_return rewrite_template_arg(  )
	{
		ANTLRParser.rewrite_template_arg_return retval = new ANTLRParser.rewrite_template_arg_return();
		retval.start = input.LT(1);

		GrammarAST root_0 = null;

		IToken a=null;
		IToken ACTION196=null;
		ANTLRParser.id_return id195 = default(ANTLRParser.id_return);

		GrammarAST a_tree=null;
		GrammarAST ACTION196_tree=null;
		RewriteRuleITokenStream stream_ASSIGN=new RewriteRuleITokenStream(adaptor,"token ASSIGN");
		RewriteRuleITokenStream stream_ACTION=new RewriteRuleITokenStream(adaptor,"token ACTION");
		RewriteRuleSubtreeStream stream_id=new RewriteRuleSubtreeStream(adaptor,"rule id");
		try
		{
			// Grammars\\ANTLR.g3:806:4: ( id a= ASSIGN ACTION -> ^( ARG[$a,\"ARG\"] id ACTION ) )
			// Grammars\\ANTLR.g3:806:4: id a= ASSIGN ACTION
			{
			PushFollow(Follow._id_in_rewrite_template_arg3529);
			id195=id();

			state._fsp--;
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_id.Add(id195.Tree);
			a=(IToken)Match(input,ASSIGN,Follow._ASSIGN_in_rewrite_template_arg3533); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_ASSIGN.Add(a);

			ACTION196=(IToken)Match(input,ACTION,Follow._ACTION_in_rewrite_template_arg3535); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_ACTION.Add(ACTION196);



			{
			// AST REWRITE
			// elements: id, ACTION
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (GrammarAST)adaptor.Nil();
			// 807:3: -> ^( ARG[$a,\"ARG\"] id ACTION )
			{
				// Grammars\\ANTLR.g3:807:6: ^( ARG[$a,\"ARG\"] id ACTION )
				{
				GrammarAST root_1 = (GrammarAST)adaptor.Nil();
				root_1 = (GrammarAST)adaptor.BecomeRoot((GrammarAST)adaptor.Create(ARG, a, "ARG"), root_1);

				adaptor.AddChild(root_1, stream_id.NextTree());
				adaptor.AddChild(root_1, stream_ACTION.NextNode());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}
			}

			}

			retval.stop = input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.tree = (GrammarAST)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);
			}
		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (GrammarAST)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "rewrite_template_arg"

	// $ANTLR start synpred1_ANTLR
	public void synpred1_ANTLR_fragment()
	{
		// Grammars\\ANTLR.g3:505:4: ({...}? id WILDCARD ( terminal | ruleref ) )
		// Grammars\\ANTLR.g3:505:5: {...}? id WILDCARD ( terminal | ruleref )
		{
		if ( !((LT(1).CharPositionInLine+LT(1).Text.Length==LT(2).CharPositionInLine&&
					 LT(2).CharPositionInLine+1==LT(3).CharPositionInLine)) )
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			throw new FailedPredicateException(input, "synpred1_ANTLR", "LT(1).CharPositionInLine+LT(1).Text.Length==LT(2).CharPositionInLine&&\n\t\t\t LT(2).CharPositionInLine+1==LT(3).CharPositionInLine");
		}
		PushFollow(Follow._id_in_synpred1_ANTLR1964);
		id();

		state._fsp--;
		if (state.failed) return ;
		Match(input,WILDCARD,Follow._WILDCARD_in_synpred1_ANTLR1966); if (state.failed) return ;
		// Grammars\\ANTLR.g3:506:72: ( terminal | ruleref )
		int alt98=2;
		int LA98_0 = input.LA(1);

		if ( (LA98_0==CHAR_LITERAL||LA98_0==STRING_LITERAL||LA98_0==TOKEN_REF||LA98_0==WILDCARD) )
		{
			alt98=1;
		}
		else if ( (LA98_0==RULE_REF) )
		{
			alt98=2;
		}
		else
		{
			if (state.backtracking>0) {state.failed=true; return ;}
			NoViableAltException nvae = new NoViableAltException("", 98, 0, input);

			throw nvae;
		}
		switch ( alt98 )
		{
		case 1:
			// Grammars\\ANTLR.g3:506:73: terminal
			{
			PushFollow(Follow._terminal_in_synpred1_ANTLR1969);
			terminal();

			state._fsp--;
			if (state.failed) return ;

			}
			break;
		case 2:
			// Grammars\\ANTLR.g3:506:82: ruleref
			{
			PushFollow(Follow._ruleref_in_synpred1_ANTLR1971);
			ruleref();

			state._fsp--;
			if (state.failed) return ;

			}
			break;

		}


		}
	}
	// $ANTLR end synpred1_ANTLR
	#endregion Rules

	#region Synpreds
	public bool synpred1_ANTLR()
	{
		state.backtracking++;
		int start = input.Mark();
		try
		{
			synpred1_ANTLR_fragment(); // can never throw exception
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
	DFA49 dfa49;
	DFA52 dfa52;
	DFA83 dfa83;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa49 = new DFA49( this );
		dfa52 = new DFA52( this, new SpecialStateTransitionHandler( specialStateTransition52 ) );
		dfa83 = new DFA83( this, new SpecialStateTransitionHandler( specialStateTransition83 ) );
	}

	class DFA49 : DFA
	{

		const string DFA49_eotS =
			"\xA\xFFFF";
		const string DFA49_eofS =
			"\xA\xFFFF";
		const string DFA49_minS =
			"\x3\x4\x7\xFFFF";
		const string DFA49_maxS =
			"\x3\x5F\x7\xFFFF";
		const string DFA49_acceptS =
			"\x3\xFFFF\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x1";
		const string DFA49_specialS =
			"\xA\xFFFF}>";
		static readonly string[] DFA49_transitionS =
			{
				"\x1\x6\xD\xFFFF\x1\x3\x14\xFFFF\x1\x5\xB\xFFFF\x1\x4\x3\xFFFF\x1\x3"+
				"\x15\xFFFF\x1\x2\x2\xFFFF\x1\x7\x4\xFFFF\x1\x3\x4\xFFFF\x1\x1\x2\xFFFF"+
				"\x1\x8\x1\xFFFF\x1\x3",
				"\x1\x3\x6\xFFFF\x1\x3\x1\xFFFF\x1\x9\x1\xFFFF\x1\x3\x2\xFFFF\x1\x3\x14"+
				"\xFFFF\x1\x3\xB\xFFFF\x1\x3\x3\xFFFF\x2\x3\x2\xFFFF\x1\x3\x2\xFFFF\x1"+
				"\x3\x1\x9\x4\xFFFF\x1\x3\x4\xFFFF\x3\x3\x1\xFFFF\x1\x3\x1\xFFFF\x2\x3"+
				"\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3",
				"\x1\x3\x6\xFFFF\x1\x3\x1\xFFFF\x1\x9\x1\xFFFF\x1\x3\x2\xFFFF\x1\x3\x14"+
				"\xFFFF\x1\x3\xB\xFFFF\x1\x3\x3\xFFFF\x1\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1"+
				"\x3\x1\x9\x4\xFFFF\x1\x3\x4\xFFFF\x3\x3\x1\xFFFF\x1\x3\x1\xFFFF\x2\x3"+
				"\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3",
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};

		static readonly short[] DFA49_eot = DFA.UnpackEncodedString(DFA49_eotS);
		static readonly short[] DFA49_eof = DFA.UnpackEncodedString(DFA49_eofS);
		static readonly char[] DFA49_min = DFA.UnpackEncodedStringToUnsignedChars(DFA49_minS);
		static readonly char[] DFA49_max = DFA.UnpackEncodedStringToUnsignedChars(DFA49_maxS);
		static readonly short[] DFA49_accept = DFA.UnpackEncodedString(DFA49_acceptS);
		static readonly short[] DFA49_special = DFA.UnpackEncodedString(DFA49_specialS);
		static readonly short[][] DFA49_transition;

		static DFA49()
		{
			int numStates = DFA49_transitionS.Length;
			DFA49_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA49_transition[i] = DFA.UnpackEncodedString(DFA49_transitionS[i]);
			}
		}

		public DFA49( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 49;
			this.eot = DFA49_eot;
			this.eof = DFA49_eof;
			this.min = DFA49_min;
			this.max = DFA49_max;
			this.accept = DFA49_accept;
			this.special = DFA49_special;
			this.transition = DFA49_transition;
		}
		public override string GetDescription()
		{
			return "482:4: ( ( id ( ASSIGN | PLUS_ASSIGN ) ( atom | block ) ) (sub= ebnfSuffix[root_0,false] )? |a= atom (sub2= ebnfSuffix[$a.tree,false] )? | ebnf | FORCED_ACTION | ACTION |p= SEMPRED ( IMPLIES )? |t3= tree_ )";
		}
	}

	class DFA52 : DFA
	{

		const string DFA52_eotS =
			"\x12\xFFFF";
		const string DFA52_eofS =
			"\x12\xFFFF";
		const string DFA52_minS =
			"\x1\x12\x2\x4\x1\xFFFF\x2\x4\x1\xFFFF\xA\x0\x1\xFFFF";
		const string DFA52_maxS =
			"\x3\x5F\x1\xFFFF\x2\x5F\x1\xFFFF\xA\x0\x1\xFFFF";
		const string DFA52_acceptS =
			"\x3\xFFFF\x1\x2\x2\xFFFF\x1\x3\xA\xFFFF\x1\x1";
		const string DFA52_specialS =
			"\x7\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9"+
			"\x1\xFFFF}>";
		static readonly string[] DFA52_transitionS =
			{
				"\x1\x3\x3A\xFFFF\x1\x2\x7\xFFFF\x1\x3\x4\xFFFF\x1\x1\x4\xFFFF\x1\x3",
				"\x1\x3\x6\xFFFF\x1\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x14\xFFFF\x1\x3"+
				"\xB\xFFFF\x1\x3\x3\xFFFF\x2\x3\x2\xFFFF\x1\x3\x2\xFFFF\x1\x3\x5\xFFFF"+
				"\x1\x3\x4\xFFFF\x3\x3\x1\xFFFF\x1\x3\x1\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1"+
				"\xFFFF\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x4",
				"\x1\x6\x6\xFFFF\x1\x6\x3\xFFFF\x1\x6\x2\xFFFF\x1\x6\x14\xFFFF\x1\x6"+
				"\xB\xFFFF\x1\x6\x3\xFFFF\x1\x6\x3\xFFFF\x1\x6\x2\xFFFF\x1\x6\x5\xFFFF"+
				"\x1\x6\x4\xFFFF\x3\x6\x1\xFFFF\x1\x6\x1\xFFFF\x2\x6\x2\xFFFF\x1\x6\x1"+
				"\xFFFF\x1\x6\x4\xFFFF\x1\x6\x2\xFFFF\x1\x6\x1\xFFFF\x1\x5",
				"",
				"\x1\x3\xA\xFFFF\x1\x3\x2\xFFFF\x1\x7\x14\xFFFF\x1\x3\xB\xFFFF\x1\x3"+
				"\x3\xFFFF\x1\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x5\xFFFF\x1\x3\x4\xFFFF"+
				"\x3\x3\x1\xFFFF\x1\xB\x1\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x9\x4"+
				"\xFFFF\x1\x8\x2\xFFFF\x1\x3\x1\xFFFF\x1\xA",
				"\x1\x6\xA\xFFFF\x1\x6\x2\xFFFF\x1\xC\x14\xFFFF\x1\x6\xB\xFFFF\x1\x6"+
				"\x3\xFFFF\x1\x6\x3\xFFFF\x1\x6\x2\xFFFF\x1\x6\x5\xFFFF\x1\x6\x4\xFFFF"+
				"\x3\x6\x1\xFFFF\x1\x10\x1\xFFFF\x2\x6\x2\xFFFF\x1\x6\x1\xFFFF\x1\xE"+
				"\x4\xFFFF\x1\xD\x2\xFFFF\x1\x6\x1\xFFFF\x1\xF",
				"",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				""
			};

		static readonly short[] DFA52_eot = DFA.UnpackEncodedString(DFA52_eotS);
		static readonly short[] DFA52_eof = DFA.UnpackEncodedString(DFA52_eofS);
		static readonly char[] DFA52_min = DFA.UnpackEncodedStringToUnsignedChars(DFA52_minS);
		static readonly char[] DFA52_max = DFA.UnpackEncodedStringToUnsignedChars(DFA52_maxS);
		static readonly short[] DFA52_accept = DFA.UnpackEncodedString(DFA52_acceptS);
		static readonly short[] DFA52_special = DFA.UnpackEncodedString(DFA52_specialS);
		static readonly short[][] DFA52_transition;

		static DFA52()
		{
			int numStates = DFA52_transitionS.Length;
			DFA52_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA52_transition[i] = DFA.UnpackEncodedString(DFA52_transitionS[i]);
			}
		}

		public DFA52( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 52;
			this.eot = DFA52_eot;
			this.eof = DFA52_eof;
			this.min = DFA52_min;
			this.max = DFA52_max;
			this.accept = DFA52_accept;
			this.special = DFA52_special;
			this.transition = DFA52_transition;
		}
		public override string GetDescription()
		{
			return "502:4: (=> id w= WILDCARD ( terminal | ruleref ) | terminal | ruleref )";
		}
	}

	int specialStateTransition52( DFA dfa, int s, IIntStream _input )
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA52_7 = input.LA(1);


				int index52_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 3;}


				input.Seek(index52_7);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA52_8 = input.LA(1);


				int index52_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 3;}


				input.Seek(index52_8);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA52_9 = input.LA(1);


				int index52_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 3;}


				input.Seek(index52_9);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA52_10 = input.LA(1);


				int index52_10 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 3;}


				input.Seek(index52_10);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA52_11 = input.LA(1);


				int index52_11 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 3;}


				input.Seek(index52_11);
				if ( s>=0 ) return s;
				break;

			case 5:
				int LA52_12 = input.LA(1);


				int index52_12 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 6;}


				input.Seek(index52_12);
				if ( s>=0 ) return s;
				break;

			case 6:
				int LA52_13 = input.LA(1);


				int index52_13 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 6;}


				input.Seek(index52_13);
				if ( s>=0 ) return s;
				break;

			case 7:
				int LA52_14 = input.LA(1);


				int index52_14 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 6;}


				input.Seek(index52_14);
				if ( s>=0 ) return s;
				break;

			case 8:
				int LA52_15 = input.LA(1);


				int index52_15 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 6;}


				input.Seek(index52_15);
				if ( s>=0 ) return s;
				break;

			case 9:
				int LA52_16 = input.LA(1);


				int index52_16 = input.Index;
				input.Rewind();
				s = -1;
				if ( (synpred1_ANTLR()) ) {s = 17;}

				else if ( (true) ) {s = 6;}


				input.Seek(index52_16);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 52, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	class DFA83 : DFA
	{

		const string DFA83_eotS =
			"\xF\xFFFF";
		const string DFA83_eofS =
			"\xF\xFFFF";
		const string DFA83_minS =
			"\x1\x4\x4\x0\xA\xFFFF";
		const string DFA83_maxS =
			"\x1\x5D\x4\x0\xA\xFFFF";
		const string DFA83_acceptS =
			"\x5\xFFFF\x1\x2\x3\xFFFF\x1\x3\x3\xFFFF\x1\x4\x1\x1";
		const string DFA83_specialS =
			"\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\xA\xFFFF}>";
		static readonly string[] DFA83_transitionS =
			{
				"\x1\x4\xD\xFFFF\x1\x5\x9\xFFFF\x1\x5\x8\xFFFF\x1\xD\xD\xFFFF\x1\x3\x7"+
				"\xFFFF\x1\x9\xD\xFFFF\x1\x9\x1\xFFFF\x1\x9\x1\xFFFF\x1\x2\x1\xFFFF\x1"+
				"\x9\x5\xFFFF\x1\x5\x4\xFFFF\x1\x1\x2\xFFFF\x1\x5",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
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
				""
			};

		static readonly short[] DFA83_eot = DFA.UnpackEncodedString(DFA83_eotS);
		static readonly short[] DFA83_eof = DFA.UnpackEncodedString(DFA83_eofS);
		static readonly char[] DFA83_min = DFA.UnpackEncodedStringToUnsignedChars(DFA83_minS);
		static readonly char[] DFA83_max = DFA.UnpackEncodedStringToUnsignedChars(DFA83_maxS);
		static readonly short[] DFA83_accept = DFA.UnpackEncodedString(DFA83_acceptS);
		static readonly short[] DFA83_special = DFA.UnpackEncodedString(DFA83_specialS);
		static readonly short[][] DFA83_transition;

		static DFA83()
		{
			int numStates = DFA83_transitionS.Length;
			DFA83_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA83_transition[i] = DFA.UnpackEncodedString(DFA83_transitionS[i]);
			}
		}

		public DFA83( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base( specialStateTransition )	{
			this.recognizer = recognizer;
			this.decisionNumber = 83;
			this.eot = DFA83_eot;
			this.eof = DFA83_eof;
			this.min = DFA83_min;
			this.max = DFA83_max;
			this.accept = DFA83_accept;
			this.special = DFA83_special;
			this.transition = DFA83_transition;
		}
		public override string GetDescription()
		{
			return "679:0: rewrite_alternative options {k=1; } : ({...}? => rewrite_template |{...}? => ( rewrite_element )+ -> {!stream_rewrite_element.HasNext}? ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) -> ^( ALT[LT(1),\"ALT\"] ( rewrite_element )+ EOA[\"<end-of-alt>\"] ) | -> ^( ALT[LT(1),\"ALT\"] EPSILON[\"epsilon\"] EOA[\"<end-of-alt>\"] ) |{...}? ETC );";
		}
	}

	int specialStateTransition83( DFA dfa, int s, IIntStream _input )
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch ( s )
		{

			case 0:
				int LA83_0 = input.LA(1);


				int index83_0 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA83_0==TOKEN_REF) && ((((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||(Grammar.BuildTemplate)||(Grammar.BuildAST)||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))))) {s = 1;}

				else if ( (LA83_0==RULE_REF) && ((((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||(Grammar.BuildTemplate)||(Grammar.BuildAST)||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))))) {s = 2;}

				else if ( (LA83_0==LPAREN) && (((Grammar.BuildTemplate)||(Grammar.BuildAST)))) {s = 3;}

				else if ( (LA83_0==ACTION) && (((Grammar.BuildTemplate)||(Grammar.BuildAST)))) {s = 4;}

				else if ( (LA83_0==CHAR_LITERAL||LA83_0==DOLLAR||LA83_0==STRING_LITERAL||LA83_0==TREE_BEGIN) && ((Grammar.BuildAST))) {s = 5;}

				else if ( (LA83_0==OR||LA83_0==REWRITE||LA83_0==RPAREN||LA83_0==SEMI) ) {s = 9;}

				else if ( (LA83_0==ETC) ) {s = 13;}


				input.Seek(index83_0);
				if ( s>=0 ) return s;
				break;

			case 1:
				int LA83_1 = input.LA(1);


				int index83_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||(Grammar.BuildTemplate))) ) {s = 14;}

				else if ( ((Grammar.BuildAST)) ) {s = 5;}


				input.Seek(index83_1);
				if ( s>=0 ) return s;
				break;

			case 2:
				int LA83_2 = input.LA(1);


				int index83_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((((Grammar.BuildTemplate)&&(LT(1).Text.Equals("template")))||(Grammar.BuildTemplate))) ) {s = 14;}

				else if ( ((Grammar.BuildAST)) ) {s = 5;}


				input.Seek(index83_2);
				if ( s>=0 ) return s;
				break;

			case 3:
				int LA83_3 = input.LA(1);


				int index83_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((Grammar.BuildTemplate)) ) {s = 14;}

				else if ( ((Grammar.BuildAST)) ) {s = 5;}


				input.Seek(index83_3);
				if ( s>=0 ) return s;
				break;

			case 4:
				int LA83_4 = input.LA(1);


				int index83_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((Grammar.BuildTemplate)) ) {s = 14;}

				else if ( ((Grammar.BuildAST)) ) {s = 5;}


				input.Seek(index83_4);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking>0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.GetDescription(), 83, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}

	#endregion DFA

	#region Follow sets
	public static class Follow
	{
		public static readonly BitSet _ACTION_in_grammar_308 = new BitSet(new ulong[]{0x1002040008000000UL,0x10000000UL});
		public static readonly BitSet _DOC_COMMENT_in_grammar_319 = new BitSet(new ulong[]{0x1002040008000000UL,0x10000000UL});
		public static readonly BitSet _grammarType_in_grammar_329 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_grammar_333 = new BitSet(new ulong[]{0x0UL,0x8000UL});
		public static readonly BitSet _SEMI_in_grammar_337 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _optionsSpec_in_grammar_343 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _delegateGrammars_in_grammar_357 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _tokensSpec_in_grammar_366 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _attrScopes_in_grammar_374 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _actions_in_grammar_381 = new BitSet(new ulong[]{0x400210008000200UL,0xC00600EUL});
		public static readonly BitSet _rules_in_grammar_389 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _EOF_in_grammar_393 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LEXER_in_grammarType444 = new BitSet(new ulong[]{0x40000000000UL});
		public static readonly BitSet _GRAMMAR_in_grammarType449 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PARSER_in_grammarType472 = new BitSet(new ulong[]{0x40000000000UL});
		public static readonly BitSet _GRAMMAR_in_grammarType476 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TREE_in_grammarType497 = new BitSet(new ulong[]{0x40000000000UL});
		public static readonly BitSet _GRAMMAR_in_grammarType503 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _GRAMMAR_in_grammarType526 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _action_in_actions553 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _AMPERSAND_in_action568 = new BitSet(new ulong[]{0x1002000000000000UL,0x4002000UL});
		public static readonly BitSet _actionScopeName_in_action572 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _COLON_in_action574 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _COLON_in_action577 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_action582 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_action584 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_actionScopeName597 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LEXER_in_actionScopeName604 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PARSER_in_actionScopeName618 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPTIONS_in_optionsSpec640 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _option_in_optionsSpec644 = new BitSet(new ulong[]{0x0UL,0x8000UL});
		public static readonly BitSet _SEMI_in_optionsSpec647 = new BitSet(new ulong[]{0x0UL,0x4002040UL});
		public static readonly BitSet _RCURLY_in_optionsSpec652 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_option665 = new BitSet(new ulong[]{0x2000UL});
		public static readonly BitSet _ASSIGN_in_option667 = new BitSet(new ulong[]{0x800000040000UL,0x4282000UL});
		public static readonly BitSet _optionValue_in_option670 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_optionValue691 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_optionValue703 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_optionValue712 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INT_in_optionValue723 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STAR_in_optionValue743 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_in_delegateGrammars768 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _delegateGrammar_in_delegateGrammars771 = new BitSet(new ulong[]{0x1000000UL,0x8000UL});
		public static readonly BitSet _COMMA_in_delegateGrammars774 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _delegateGrammar_in_delegateGrammars777 = new BitSet(new ulong[]{0x1000000UL,0x8000UL});
		public static readonly BitSet _SEMI_in_delegateGrammars781 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_delegateGrammar795 = new BitSet(new ulong[]{0x2000UL});
		public static readonly BitSet _ASSIGN_in_delegateGrammar797 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_delegateGrammar802 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_delegateGrammar811 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKENS_in_tokensSpec838 = new BitSet(new ulong[]{0x0UL,0x4000000UL});
		public static readonly BitSet _tokenSpec_in_tokensSpec846 = new BitSet(new ulong[]{0x0UL,0x4000040UL});
		public static readonly BitSet _RCURLY_in_tokensSpec853 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_tokenSpec865 = new BitSet(new ulong[]{0x2000UL,0x8000UL});
		public static readonly BitSet _ASSIGN_in_tokenSpec869 = new BitSet(new ulong[]{0x40000UL,0x200000UL});
		public static readonly BitSet _set_in_tokenSpec872 = new BitSet(new ulong[]{0x0UL,0x8000UL});
		public static readonly BitSet _SEMI_in_tokenSpec881 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attrScope_in_attrScopes894 = new BitSet(new ulong[]{0x2UL,0x4000UL});
		public static readonly BitSet _SCOPE_in_attrScope907 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_attrScope910 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ruleActions_in_attrScope912 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_attrScope915 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rule_in_rules928 = new BitSet(new ulong[]{0x400210008000202UL,0xC00600EUL});
		public static readonly BitSet _DOC_COMMENT_in_rule958 = new BitSet(new ulong[]{0x10000000000UL,0x400200EUL});
		public static readonly BitSet _PROTECTED_in_rule971 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _PUBLIC_in_rule980 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _PRIVATE_in_rule990 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _FRAGMENT_in_rule999 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_rule1011 = new BitSet(new ulong[]{0x400000000408A00UL,0x2004100UL});
		public static readonly BitSet _BANG_in_rule1021 = new BitSet(new ulong[]{0x400000000400A00UL,0x2004100UL});
		public static readonly BitSet _ARG_ACTION_in_rule1032 = new BitSet(new ulong[]{0x400000000400200UL,0x2004100UL});
		public static readonly BitSet _RETURNS_in_rule1041 = new BitSet(new ulong[]{0x800UL});
		public static readonly BitSet _ARG_ACTION_in_rule1045 = new BitSet(new ulong[]{0x400000000400200UL,0x2004000UL});
		public static readonly BitSet _throwsSpec_in_rule1055 = new BitSet(new ulong[]{0x400000000400200UL,0x4000UL});
		public static readonly BitSet _optionsSpec_in_rule1064 = new BitSet(new ulong[]{0x400200UL,0x4000UL});
		public static readonly BitSet _ruleScopeSpec_in_rule1073 = new BitSet(new ulong[]{0x400200UL});
		public static readonly BitSet _ruleActions_in_rule1078 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _COLON_in_rule1084 = new BitSet(new ulong[]{0x888008000040010UL,0xA4212200UL});
		public static readonly BitSet _altList_in_rule1088 = new BitSet(new ulong[]{0x0UL,0x8000UL});
		public static readonly BitSet _SEMI_in_rule1093 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _exceptionGroup_in_rule1101 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ruleAction_in_ruleActions1243 = new BitSet(new ulong[]{0x202UL});
		public static readonly BitSet _AMPERSAND_in_ruleAction1258 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_ruleAction1261 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_ruleAction1263 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROWS_in_throwsSpec1274 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_throwsSpec1277 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _COMMA_in_throwsSpec1281 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_throwsSpec1284 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec1300 = new BitSet(new ulong[]{0x210UL});
		public static readonly BitSet _ruleActions_in_ruleScopeSpec1302 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_ruleScopeSpec1305 = new BitSet(new ulong[]{0x2UL,0x4000UL});
		public static readonly BitSet _SCOPE_in_ruleScopeSpec1314 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _idList_in_ruleScopeSpec1316 = new BitSet(new ulong[]{0x0UL,0x8000UL});
		public static readonly BitSet _SEMI_in_ruleScopeSpec1318 = new BitSet(new ulong[]{0x2UL,0x4000UL});
		public static readonly BitSet _LPAREN_in_block1361 = new BitSet(new ulong[]{0xC88008000440210UL,0xA4212A00UL});
		public static readonly BitSet _optionsSpec_in_block1399 = new BitSet(new ulong[]{0x400200UL});
		public static readonly BitSet _ruleActions_in_block1410 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _COLON_in_block1418 = new BitSet(new ulong[]{0x888008000040010UL,0xA4212A00UL});
		public static readonly BitSet _ACTION_in_block1424 = new BitSet(new ulong[]{0x400000UL});
		public static readonly BitSet _COLON_in_block1426 = new BitSet(new ulong[]{0x888008000040010UL,0xA4212A00UL});
		public static readonly BitSet _alternative_in_block1438 = new BitSet(new ulong[]{0x800000000000000UL,0xA00UL});
		public static readonly BitSet _rewrite_in_block1442 = new BitSet(new ulong[]{0x800000000000000UL,0x800UL});
		public static readonly BitSet _OR_in_block1452 = new BitSet(new ulong[]{0x888008000040010UL,0xA4212A00UL});
		public static readonly BitSet _alternative_in_block1456 = new BitSet(new ulong[]{0x800000000000000UL,0xA00UL});
		public static readonly BitSet _rewrite_in_block1460 = new BitSet(new ulong[]{0x800000000000000UL,0x800UL});
		public static readonly BitSet _RPAREN_in_block1477 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _alternative_in_altList1541 = new BitSet(new ulong[]{0x800000000000000UL,0x200UL});
		public static readonly BitSet _rewrite_in_altList1545 = new BitSet(new ulong[]{0x800000000000002UL});
		public static readonly BitSet _OR_in_altList1572 = new BitSet(new ulong[]{0x888008000040010UL,0xA4212200UL});
		public static readonly BitSet _alternative_in_altList1576 = new BitSet(new ulong[]{0x800000000000000UL,0x200UL});
		public static readonly BitSet _rewrite_in_altList1580 = new BitSet(new ulong[]{0x800000000000002UL});
		public static readonly BitSet _element_in_alternative1676 = new BitSet(new ulong[]{0x88008000040012UL,0xA4212000UL});
		public static readonly BitSet _exceptionHandler_in_exceptionGroup1729 = new BitSet(new ulong[]{0x4000020002UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup1736 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _finallyClause_in_exceptionGroup1744 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_exceptionHandler1755 = new BitSet(new ulong[]{0x800UL});
		public static readonly BitSet _ARG_ACTION_in_exceptionHandler1758 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_exceptionHandler1760 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FINALLY_in_finallyClause1771 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_finallyClause1774 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _elementNoOptionSpec_in_element1785 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_elementNoOptionSpec1805 = new BitSet(new ulong[]{0x8000000000002000UL});
		public static readonly BitSet _ASSIGN_in_elementNoOptionSpec1808 = new BitSet(new ulong[]{0x88000000040000UL,0x84202000UL});
		public static readonly BitSet _PLUS_ASSIGN_in_elementNoOptionSpec1811 = new BitSet(new ulong[]{0x88000000040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_elementNoOptionSpec1816 = new BitSet(new ulong[]{0x4000000000000002UL,0x80010UL});
		public static readonly BitSet _block_in_elementNoOptionSpec1818 = new BitSet(new ulong[]{0x4000000000000002UL,0x80010UL});
		public static readonly BitSet _ebnfSuffix_in_elementNoOptionSpec1833 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _atom_in_elementNoOptionSpec1851 = new BitSet(new ulong[]{0x4000000000000002UL,0x80010UL});
		public static readonly BitSet _ebnfSuffix_in_elementNoOptionSpec1860 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ebnf_in_elementNoOptionSpec1876 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FORCED_ACTION_in_elementNoOptionSpec1882 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_elementNoOptionSpec1888 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMPRED_in_elementNoOptionSpec1896 = new BitSet(new ulong[]{0x100000000002UL});
		public static readonly BitSet _IMPLIES_in_elementNoOptionSpec1900 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tree__in_elementNoOptionSpec1919 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _range_in_atom1934 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_atom1937 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_atom1940 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_atom1980 = new BitSet(new ulong[]{0x0UL,0x80000000UL});
		public static readonly BitSet _WILDCARD_in_atom1984 = new BitSet(new ulong[]{0x40000UL,0x84202000UL});
		public static readonly BitSet _terminal_in_atom1988 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ruleref_in_atom1990 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _terminal_in_atom1999 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ruleref_in_atom2005 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _notSet_in_atom2014 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_atom2017 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_atom2020 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _RULE_REF_in_ruleref2036 = new BitSet(new ulong[]{0x8802UL,0x400UL});
		public static readonly BitSet _ARG_ACTION_in_ruleref2041 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_ruleref2047 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_ruleref2050 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_in_notSet2064 = new BitSet(new ulong[]{0x8000000040000UL,0x4200000UL});
		public static readonly BitSet _notTerminal_in_notSet2071 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_notSet2077 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_treeRoot2098 = new BitSet(new ulong[]{0x8000000000002000UL});
		public static readonly BitSet _ASSIGN_in_treeRoot2101 = new BitSet(new ulong[]{0x88000000040000UL,0x84202000UL});
		public static readonly BitSet _PLUS_ASSIGN_in_treeRoot2104 = new BitSet(new ulong[]{0x88000000040000UL,0x84202000UL});
		public static readonly BitSet _atom_in_treeRoot2109 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_treeRoot2111 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _atom_in_treeRoot2118 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_treeRoot2124 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TREE_BEGIN_in_tree_2143 = new BitSet(new ulong[]{0x88000000040000UL,0x84202000UL});
		public static readonly BitSet _treeRoot_in_tree_2148 = new BitSet(new ulong[]{0x88008000040010UL,0xA4212000UL});
		public static readonly BitSet _element_in_tree_2152 = new BitSet(new ulong[]{0x88008000040010UL,0xA4212800UL});
		public static readonly BitSet _RPAREN_in_tree_2159 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _block_in_ebnf2173 = new BitSet(new ulong[]{0x4000100000008002UL,0x80410UL});
		public static readonly BitSet _QUESTION_in_ebnf2179 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STAR_in_ebnf2197 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_ebnf2215 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPLIES_in_ebnf2233 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ROOT_in_ebnf2269 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_ebnf2286 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_range2325 = new BitSet(new ulong[]{0x0UL,0x20UL});
		public static readonly BitSet _RANGE_in_range2327 = new BitSet(new ulong[]{0x40000UL});
		public static readonly BitSet _CHAR_LITERAL_in_range2331 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_terminal2359 = new BitSet(new ulong[]{0x100000000008002UL,0x400UL});
		public static readonly BitSet _elementOptions_in_terminal2364 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_terminal2372 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_terminal2375 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_terminal2386 = new BitSet(new ulong[]{0x100000000008802UL,0x400UL});
		public static readonly BitSet _elementOptions_in_terminal2393 = new BitSet(new ulong[]{0x8802UL,0x400UL});
		public static readonly BitSet _ARG_ACTION_in_terminal2404 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_terminal2413 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_terminal2416 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_terminal2427 = new BitSet(new ulong[]{0x100000000008002UL,0x400UL});
		public static readonly BitSet _elementOptions_in_terminal2432 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_terminal2440 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_terminal2443 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _WILDCARD_in_terminal2454 = new BitSet(new ulong[]{0x8002UL,0x400UL});
		public static readonly BitSet _ROOT_in_terminal2457 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BANG_in_terminal2460 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPEN_ELEMENT_OPTION_in_elementOptions2479 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _defaultNodeOption_in_elementOptions2482 = new BitSet(new ulong[]{0x100000UL});
		public static readonly BitSet _CLOSE_ELEMENT_OPTION_in_elementOptions2485 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPEN_ELEMENT_OPTION_in_elementOptions2491 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _elementOption_in_elementOptions2494 = new BitSet(new ulong[]{0x100000UL,0x8000UL});
		public static readonly BitSet _SEMI_in_elementOptions2498 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _elementOption_in_elementOptions2501 = new BitSet(new ulong[]{0x100000UL,0x8000UL});
		public static readonly BitSet _CLOSE_ELEMENT_OPTION_in_elementOptions2506 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_defaultNodeOption2526 = new BitSet(new ulong[]{0x2UL,0x80000000UL});
		public static readonly BitSet _WILDCARD_in_defaultNodeOption2531 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_defaultNodeOption2535 = new BitSet(new ulong[]{0x2UL,0x80000000UL});
		public static readonly BitSet _id_in_elementOption2557 = new BitSet(new ulong[]{0x2000UL});
		public static readonly BitSet _ASSIGN_in_elementOption2559 = new BitSet(new ulong[]{0x0UL,0x4202000UL});
		public static readonly BitSet _id_in_elementOption2565 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_elementOption2569 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _QUESTION_in_ebnfSuffix2643 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STAR_in_ebnfSuffix2657 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_ebnfSuffix2671 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_notTerminal2714 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_notTerminal2721 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_notTerminal2726 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_idList2737 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _COMMA_in_idList2740 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_idList2743 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _TOKEN_REF_in_id2756 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _RULE_REF_in_id2768 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_with_sempred_in_rewrite2788 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _REWRITE_in_rewrite2793 = new BitSet(new ulong[]{0x8002010040010UL,0x24202000UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite2795 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _REWRITE_in_rewrite_with_sempred2822 = new BitSet(new ulong[]{0x0UL,0x10000UL});
		public static readonly BitSet _SEMPRED_in_rewrite_with_sempred2825 = new BitSet(new ulong[]{0x8002010040010UL,0x24202000UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite_with_sempred2827 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_rewrite_block2838 = new BitSet(new ulong[]{0x8002010040010UL,0x24202800UL});
		public static readonly BitSet _rewrite_alternative_in_rewrite_block2842 = new BitSet(new ulong[]{0x0UL,0x800UL});
		public static readonly BitSet _RPAREN_in_rewrite_block2846 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_template_in_rewrite_alternative2882 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_element_in_rewrite_alternative2894 = new BitSet(new ulong[]{0x8000010040012UL,0x24202000UL});
		public static readonly BitSet _ETC_in_rewrite_alternative2955 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_element2970 = new BitSet(new ulong[]{0x4000000000000002UL,0x80010UL});
		public static readonly BitSet _ebnfSuffix_in_rewrite_element2990 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_ebnf_in_rewrite_element3009 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_tree_in_rewrite_element3018 = new BitSet(new ulong[]{0x4000000000000002UL,0x80010UL});
		public static readonly BitSet _ebnfSuffix_in_rewrite_element3038 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TOKEN_REF_in_rewrite_atom3072 = new BitSet(new ulong[]{0x100000000000802UL});
		public static readonly BitSet _elementOptions_in_rewrite_atom3089 = new BitSet(new ulong[]{0x802UL});
		public static readonly BitSet _ARG_ACTION_in_rewrite_atom3095 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _RULE_REF_in_rewrite_atom3117 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHAR_LITERAL_in_rewrite_atom3126 = new BitSet(new ulong[]{0x100000000000002UL});
		public static readonly BitSet _elementOptions_in_rewrite_atom3143 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_LITERAL_in_rewrite_atom3155 = new BitSet(new ulong[]{0x100000000000002UL});
		public static readonly BitSet _elementOptions_in_rewrite_atom3174 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOLLAR_in_rewrite_atom3184 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _id_in_rewrite_atom3188 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_rewrite_atom3201 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_block_in_rewrite_ebnf3214 = new BitSet(new ulong[]{0x4000000000000000UL,0x80010UL});
		public static readonly BitSet _QUESTION_in_rewrite_ebnf3220 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STAR_in_rewrite_ebnf3239 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_rewrite_ebnf3258 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TREE_BEGIN_in_rewrite_tree3286 = new BitSet(new ulong[]{0x10040010UL,0x4202000UL});
		public static readonly BitSet _rewrite_atom_in_rewrite_tree3292 = new BitSet(new ulong[]{0x8000010040010UL,0x24202800UL});
		public static readonly BitSet _rewrite_element_in_rewrite_tree3296 = new BitSet(new ulong[]{0x8000010040010UL,0x24202800UL});
		public static readonly BitSet _RPAREN_in_rewrite_tree3303 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_template_head_in_rewrite_template3338 = new BitSet(new ulong[]{0xC0000000UL});
		public static readonly BitSet _DOUBLE_QUOTE_STRING_LITERAL_in_rewrite_template3357 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOUBLE_ANGLE_STRING_LITERAL_in_rewrite_template3363 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_template_head_in_rewrite_template3378 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_indirect_template_head_in_rewrite_template3387 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_rewrite_template3396 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_rewrite_template_head3409 = new BitSet(new ulong[]{0x8000000000000UL});
		public static readonly BitSet _LPAREN_in_rewrite_template_head3413 = new BitSet(new ulong[]{0x0UL,0x4002800UL});
		public static readonly BitSet _rewrite_template_args_in_rewrite_template_head3417 = new BitSet(new ulong[]{0x0UL,0x800UL});
		public static readonly BitSet _RPAREN_in_rewrite_template_head3421 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_rewrite_indirect_template_head3449 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_rewrite_indirect_template_head3453 = new BitSet(new ulong[]{0x0UL,0x800UL});
		public static readonly BitSet _RPAREN_in_rewrite_indirect_template_head3457 = new BitSet(new ulong[]{0x8000000000000UL});
		public static readonly BitSet _LPAREN_in_rewrite_indirect_template_head3461 = new BitSet(new ulong[]{0x0UL,0x4002800UL});
		public static readonly BitSet _rewrite_template_args_in_rewrite_indirect_template_head3463 = new BitSet(new ulong[]{0x0UL,0x800UL});
		public static readonly BitSet _RPAREN_in_rewrite_indirect_template_head3465 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rewrite_template_arg_in_rewrite_template_args3489 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _COMMA_in_rewrite_template_args3492 = new BitSet(new ulong[]{0x0UL,0x4002000UL});
		public static readonly BitSet _rewrite_template_arg_in_rewrite_template_args3494 = new BitSet(new ulong[]{0x1000002UL});
		public static readonly BitSet _id_in_rewrite_template_arg3529 = new BitSet(new ulong[]{0x2000UL});
		public static readonly BitSet _ASSIGN_in_rewrite_template_arg3533 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _ACTION_in_rewrite_template_arg3535 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_synpred1_ANTLR1964 = new BitSet(new ulong[]{0x0UL,0x80000000UL});
		public static readonly BitSet _WILDCARD_in_synpred1_ANTLR1966 = new BitSet(new ulong[]{0x40000UL,0x84202000UL});
		public static readonly BitSet _terminal_in_synpred1_ANTLR1969 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ruleref_in_synpred1_ANTLR1971 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.Grammars
