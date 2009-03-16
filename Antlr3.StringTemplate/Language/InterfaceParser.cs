// $ANTLR 3.1.2 Language\\Interface.g3 2009-03-07 08:51:17

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

namespace Antlr3.ST.Language
{
public partial class InterfaceParser : Parser
{
	public static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "COLON", "COMMA", "ID", "LPAREN", "ML_COMMENT", "RPAREN", "SEMI", "SL_COMMENT", "WS", "'interface'", "'optional'"
	};
	public const int EOF=-1;
	public const int T__13=13;
	public const int T__14=14;
	public const int COLON=4;
	public const int COMMA=5;
	public const int ID=6;
	public const int LPAREN=7;
	public const int ML_COMMENT=8;
	public const int RPAREN=9;
	public const int SEMI=10;
	public const int SL_COMMENT=11;
	public const int WS=12;

	// delegates
	// delegators

	public InterfaceParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public InterfaceParser( ITokenStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] GetTokenNames() { return InterfaceParser.tokenNames; }
	public override string GrammarFileName { get { return "Language\\Interface.g3"; } }


	#region Rules

	// $ANTLR start "groupInterface"
	// Language\\Interface.g3:113:0: public groupInterface[StringTemplateGroupInterface groupI] : 'interface' name= ID SEMI ( template[groupI] )+ EOF ;
	public void groupInterface( StringTemplateGroupInterface groupI )
	{
		IToken name=null;


			this.groupI = groupI;

		try
		{
			// Language\\Interface.g3:118:4: ( 'interface' name= ID SEMI ( template[groupI] )+ EOF )
			// Language\\Interface.g3:118:4: 'interface' name= ID SEMI ( template[groupI] )+ EOF
			{
			Match(input,13,Follow._13_in_groupInterface67); 
			name=(IToken)Match(input,ID,Follow._ID_in_groupInterface71); 
			groupI.setName((name!=null?name.Text:null));
			Match(input,SEMI,Follow._SEMI_in_groupInterface75); 
			// Language\\Interface.g3:119:3: ( template[groupI] )+
			int cnt1=0;
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( (LA1_0==ID||LA1_0==14) )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// Language\\Interface.g3:119:5: template[groupI]
					{
					PushFollow(Follow._template_in_groupInterface81);
					template(groupI);

					state._fsp--;


					}
					break;

				default:
					if ( cnt1 >= 1 )
						goto loop1;

					EarlyExitException eee1 = new EarlyExitException( 1, input );
					throw eee1;
				}
				cnt1++;
			}
			loop1:
				;


			Match(input,EOF,Follow._EOF_in_groupInterface89); 

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
	// $ANTLR end "groupInterface"


	// $ANTLR start "template"
	// Language\\Interface.g3:124:0: template[StringTemplateGroupInterface groupI] : (opt= 'optional' )? name= ID LPAREN (formalArgs= args )? RPAREN SEMI ;
	private void template( StringTemplateGroupInterface groupI )
	{
		IToken opt=null;
		IToken name=null;
		System.Collections.Generic.SortedList<string, FormalArgument> formalArgs = default(System.Collections.Generic.SortedList<string, FormalArgument>);


			string templateName = null;

		try
		{
			// Language\\Interface.g3:129:4: ( (opt= 'optional' )? name= ID LPAREN (formalArgs= args )? RPAREN SEMI )
			// Language\\Interface.g3:129:4: (opt= 'optional' )? name= ID LPAREN (formalArgs= args )? RPAREN SEMI
			{
			// Language\\Interface.g3:129:4: (opt= 'optional' )?
			int alt2=2;
			int LA2_0 = input.LA(1);

			if ( (LA2_0==14) )
			{
				alt2=1;
			}
			switch ( alt2 )
			{
			case 1:
				// Language\\Interface.g3:129:5: opt= 'optional'
				{
				opt=(IToken)Match(input,14,Follow._14_in_template110); 

				}
				break;

			}

			name=(IToken)Match(input,ID,Follow._ID_in_template116); 
			Match(input,LPAREN,Follow._LPAREN_in_template118); 
			// Language\\Interface.g3:129:37: (formalArgs= args )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( (LA3_0==ID) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// Language\\Interface.g3:129:38: formalArgs= args
				{
				PushFollow(Follow._args_in_template123);
				formalArgs=args();

				state._fsp--;


				}
				break;

			}

			Match(input,RPAREN,Follow._RPAREN_in_template127); 
			Match(input,SEMI,Follow._SEMI_in_template129); 

						templateName = (name!=null?name.Text:null);
						groupI.defineTemplate( templateName, formalArgs ?? new System.Collections.Generic.SortedList<string, FormalArgument>(), opt!=null );
					

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
	// Language\\Interface.g3:136:0: args returns [System.Collections.Generic.SortedList<string, FormalArgument> args] : a= ID ( COMMA b= ID )* ;
	private System.Collections.Generic.SortedList<string, FormalArgument> args(  )
	{

		System.Collections.Generic.SortedList<string, FormalArgument> args = default(System.Collections.Generic.SortedList<string, FormalArgument>);

		IToken a=null;
		IToken b=null;


			args = new System.Collections.Generic.SortedList<string, FormalArgument>();

		try
		{
			// Language\\Interface.g3:141:4: (a= ID ( COMMA b= ID )* )
			// Language\\Interface.g3:141:4: a= ID ( COMMA b= ID )*
			{
			a=(IToken)Match(input,ID,Follow._ID_in_args154); 
			args[(a!=null?a.Text:null)] = new FormalArgument((a!=null?a.Text:null));
			// Language\\Interface.g3:142:3: ( COMMA b= ID )*
			for ( ; ; )
			{
				int alt4=2;
				int LA4_0 = input.LA(1);

				if ( (LA4_0==COMMA) )
				{
					alt4=1;
				}


				switch ( alt4 )
				{
				case 1:
					// Language\\Interface.g3:142:5: COMMA b= ID
					{
					Match(input,COMMA,Follow._COMMA_in_args162); 
					b=(IToken)Match(input,ID,Follow._ID_in_args166); 
					args[(b!=null?b.Text:null)] = new FormalArgument((b!=null?b.Text:null));

					}
					break;

				default:
					goto loop4;
				}
			}

			loop4:
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
		return args;
	}
	// $ANTLR end "args"
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
		public static readonly BitSet _13_in_groupInterface67 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _ID_in_groupInterface71 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _SEMI_in_groupInterface75 = new BitSet(new ulong[]{0x4040UL});
		public static readonly BitSet _template_in_groupInterface81 = new BitSet(new ulong[]{0x4040UL});
		public static readonly BitSet _EOF_in_groupInterface89 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _14_in_template110 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _ID_in_template116 = new BitSet(new ulong[]{0x80UL});
		public static readonly BitSet _LPAREN_in_template118 = new BitSet(new ulong[]{0x240UL});
		public static readonly BitSet _args_in_template123 = new BitSet(new ulong[]{0x200UL});
		public static readonly BitSet _RPAREN_in_template127 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _SEMI_in_template129 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_args154 = new BitSet(new ulong[]{0x22UL});
		public static readonly BitSet _COMMA_in_args162 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _ID_in_args166 = new BitSet(new ulong[]{0x22UL});

	}
	#endregion
}

} // namespace Antlr3.ST.Language
