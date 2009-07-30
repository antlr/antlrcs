// $ANTLR 3.1.2 Language\\Template.g3 2009-07-30 16:39:08

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
using Antlr.Runtime.JavaExtensions;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Antlr3.ST.Language
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.1.2")]
[System.CLSCompliant(false)]
public partial class TemplateParser : Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACTION", "COMMENT", "ELSE", "ELSEIF", "ENDIF", "ESC", "ESC_CHAR", "EXPR", "HEX", "IF", "IF_EXPR", "INDENT", "LINE_BREAK", "LITERAL", "NESTED_PARENS", "NEWLINE", "REGION_DEF", "REGION_REF", "SUBTEMPLATE", "TEMPLATE"
	};
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
	public const int LINE_BREAK=16;
	public const int LITERAL=17;
	public const int NESTED_PARENS=18;
	public const int NEWLINE=19;
	public const int REGION_DEF=20;
	public const int REGION_REF=21;
	public const int SUBTEMPLATE=22;
	public const int TEMPLATE=23;

	// delegates
	// delegators

	public TemplateParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public TemplateParser( ITokenStream input, RecognizerSharedState state )
		: base( input, state )
	{
	}
		

	public override string[] TokenNames { get { return TemplateParser.tokenNames; } }
	public override string GrammarFileName { get { return "Language\\Template.g3"; } }


	#region Rules

	// $ANTLR start "template"
	// Language\\Template.g3:114:0: public template[StringTemplate self] : (s= LITERAL |nl= NEWLINE | action[self] )* ( EOF )? ;
	public void template( StringTemplate self )
	{
		IToken s=null;
		IToken nl=null;


			this.self = self;

		try
		{
			// Language\\Template.g3:119:4: ( (s= LITERAL |nl= NEWLINE | action[self] )* ( EOF )? )
			// Language\\Template.g3:119:4: (s= LITERAL |nl= NEWLINE | action[self] )* ( EOF )?
			{
			// Language\\Template.g3:119:4: (s= LITERAL |nl= NEWLINE | action[self] )*
			for ( ; ; )
			{
				int alt1=4;
				switch ( input.LA(1) )
				{
				case LITERAL:
					{
					alt1=1;
					}
					break;
				case NEWLINE:
					{
					alt1=2;
					}
					break;
				case ACTION:
				case IF:
				case REGION_DEF:
				case REGION_REF:
					{
					alt1=3;
					}
					break;

				}

				switch ( alt1 )
				{
				case 1:
					// Language\\Template.g3:119:6: s= LITERAL
					{
					s=(IToken)Match(input,LITERAL,Follow._LITERAL_in_template71); 
					self.AddChunk(new StringRef(self,(s!=null?s.Text:null)));

					}
					break;
				case 2:
					// Language\\Template.g3:120:5: nl= NEWLINE
					{
					nl=(IToken)Match(input,NEWLINE,Follow._NEWLINE_in_template82); 

									int next = input.LA(1);
									if ( next!=ELSE && next!=ENDIF )
									{
										self.AddChunk(new NewlineRef(self,(nl!=null?nl.Text:null)));
									}
								

					}
					break;
				case 3:
					// Language\\Template.g3:128:5: action[self]
					{
					PushFollow(Follow._action_in_template93);
					action(self);

					state._fsp--;


					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;


			// Language\\Template.g3:130:3: ( EOF )?
			int alt2=2;
			int LA2_0 = input.LA(1);

			if ( (LA2_0==EOF) )
			{
				alt2=1;
			}
			switch ( alt2 )
			{
			case 1:
				// Language\\Template.g3:130:0: EOF
				{
				Match(input,EOF,Follow._EOF_in_template103); 

				}
				break;

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
	// $ANTLR end "template"


	// $ANTLR start "action"
	// Language\\Template.g3:133:0: action[StringTemplate self] : (a= ACTION |i= IF template[subtemplate] (ei= ELSEIF template[elseIfSubtemplate] )* ( ELSE template[elseSubtemplate] )? ENDIF |rr= REGION_REF |rd= REGION_DEF );
	private void action( StringTemplate self )
	{
		IToken a=null;
		IToken i=null;
		IToken ei=null;
		IToken rr=null;
		IToken rd=null;

		try
		{
			// Language\\Template.g3:134:4: (a= ACTION |i= IF template[subtemplate] (ei= ELSEIF template[elseIfSubtemplate] )* ( ELSE template[elseSubtemplate] )? ENDIF |rr= REGION_REF |rd= REGION_DEF )
			int alt5=4;
			switch ( input.LA(1) )
			{
			case ACTION:
				{
				alt5=1;
				}
				break;
			case IF:
				{
				alt5=2;
				}
				break;
			case REGION_REF:
				{
				alt5=3;
				}
				break;
			case REGION_DEF:
				{
				alt5=4;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 5, 0, input);

					throw nvae;
				}
			}

			switch ( alt5 )
			{
			case 1:
				// Language\\Template.g3:134:4: a= ACTION
				{
				a=(IToken)Match(input,ACTION,Follow._ACTION_in_action118); 

							string indent = ((ChunkToken)a).Indentation;
							ASTExpr c = self.ParseAction((a!=null?a.Text:null));
							c.Indentation = indent;
							self.AddChunk(c);
						

				}
				break;
			case 2:
				// Language\\Template.g3:142:4: i= IF template[subtemplate] (ei= ELSEIF template[elseIfSubtemplate] )* ( ELSE template[elseSubtemplate] )? ENDIF
				{
				i=(IToken)Match(input,IF,Follow._IF_in_action130); 

							ConditionalExpr c = (ConditionalExpr)self.ParseAction((i!=null?i.Text:null));
							// create and precompile the subtemplate
							StringTemplate subtemplate = new StringTemplate(self.Group, null);
							subtemplate.EnclosingInstance = self;
							subtemplate.Name = (i!=null?i.Text:null) + "_subtemplate";
							self.AddChunk(c);
						
				PushFollow(Follow._template_in_action139);
				template(subtemplate);

				state._fsp--;

				if ( c!=null ) c.Subtemplate = subtemplate;
				// Language\\Template.g3:154:3: (ei= ELSEIF template[elseIfSubtemplate] )*
				for ( ; ; )
				{
					int alt3=2;
					int LA3_0 = input.LA(1);

					if ( (LA3_0==ELSEIF) )
					{
						alt3=1;
					}


					switch ( alt3 )
					{
					case 1:
						// Language\\Template.g3:154:5: ei= ELSEIF template[elseIfSubtemplate]
						{
						ei=(IToken)Match(input,ELSEIF,Follow._ELSEIF_in_action151); 

										ASTExpr ec = self.ParseAction((ei!=null?ei.Text:null));
										// create and precompile the subtemplate
										StringTemplate elseIfSubtemplate = new StringTemplate(self.Group, null);
										elseIfSubtemplate.EnclosingInstance = self;
										elseIfSubtemplate.Name = (ei!=null?ei.Text:null) + "_subtemplate";
									
						PushFollow(Follow._template_in_action162);
						template(elseIfSubtemplate);

						state._fsp--;

						if ( c!=null ) c.AddElseIfSubtemplate(ec, elseIfSubtemplate);

						}
						break;

					default:
						goto loop3;
					}
				}

				loop3:
					;


				// Language\\Template.g3:168:3: ( ELSE template[elseSubtemplate] )?
				int alt4=2;
				int LA4_0 = input.LA(1);

				if ( (LA4_0==ELSE) )
				{
					alt4=1;
				}
				switch ( alt4 )
				{
				case 1:
					// Language\\Template.g3:168:5: ELSE template[elseSubtemplate]
					{
					Match(input,ELSE,Follow._ELSE_in_action181); 

									// create and precompile the subtemplate
									StringTemplate elseSubtemplate = new StringTemplate(self.Group, null);
									elseSubtemplate.EnclosingInstance = self;
									elseSubtemplate.Name = "else_subtemplate";
								
					PushFollow(Follow._template_in_action192);
					template(elseSubtemplate);

					state._fsp--;

					if ( c!=null ) c.ElseSubtemplate = elseSubtemplate;

					}
					break;

				}

				Match(input,ENDIF,Follow._ENDIF_in_action208); 

				}
				break;
			case 3:
				// Language\\Template.g3:182:4: rr= REGION_REF
				{
				rr=(IToken)Match(input,REGION_REF,Follow._REGION_REF_in_action216); 

							// define implicit template and
							// convert <@r()> to <region__enclosingTemplate__r()>
							string regionName = (rr!=null?rr.Text:null);
							string mangledRef = null;
							bool err = false;
							// watch out for <@super.r()>; that does NOT def implicit region
							// convert to <super.region__enclosingTemplate__r()>
							if ( regionName.StartsWith("super.") )
							{
								//System.Console.Out.WriteLine( "super region ref " + regionName );
								string regionRef = regionName.substring("super.".Length,regionName.Length);
								string templateScope = self.Group.GetUnMangledTemplateName(self.Name);
								StringTemplate scopeST = self.Group.LookupTemplate(templateScope);
								if ( scopeST==null )
								{
									self.Group.Error("reference to region within undefined template: "+templateScope);
									err=true;
								}
								if ( !scopeST.ContainsRegionName(regionRef) )
								{
									self.Group.Error("template "+templateScope+" has no region called "+regionRef);
									err=true;
								}
								else
								{
									mangledRef = self.Group.GetMangledRegionName(templateScope,regionRef);
									mangledRef = "super."+mangledRef;
								}
							}
							else
							{
								//System.out.println("region ref "+regionName);
								StringTemplate regionST = self.Group.DefineImplicitRegionTemplate(self,regionName);
								mangledRef = regionST.Name;
							}

							if ( !err )
							{
								// treat as regular action: mangled template include
								string indent = ((ChunkToken)rr).Indentation;
								ASTExpr c = self.ParseAction(mangledRef+"()");
								c.Indentation = indent;
								self.AddChunk(c);
							}
						

				}
				break;
			case 4:
				// Language\\Template.g3:230:4: rd= REGION_DEF
				{
				rd=(IToken)Match(input,REGION_DEF,Follow._REGION_DEF_in_action228); 

							string combinedNameTemplateStr = (rd!=null?rd.Text:null);
							int indexOfDefSymbol = combinedNameTemplateStr.IndexOf("::=");
							if ( indexOfDefSymbol>=1 )
							{
								string regionName = combinedNameTemplateStr.substring(0,indexOfDefSymbol);
								string template = combinedNameTemplateStr.substring(indexOfDefSymbol+3, combinedNameTemplateStr.Length);
								StringTemplate regionST = self.Group.DefineRegionTemplate(self,regionName,template,RegionType.Embedded);
								// treat as regular action: mangled template include
								string indent = ((ChunkToken)rd).Indentation;
								ASTExpr c = self.ParseAction(regionST.Name+"()");
								c.Indentation = indent;
								self.AddChunk(c);
							}
							else
							{
								self.Error("embedded region definition screwed up");
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
	// $ANTLR end "action"
	#endregion Rules


	#region Follow sets
	static class Follow
	{
		public static readonly BitSet _LITERAL_in_template71 = new BitSet(new ulong[]{0x3A2012UL});
		public static readonly BitSet _NEWLINE_in_template82 = new BitSet(new ulong[]{0x3A2012UL});
		public static readonly BitSet _action_in_template93 = new BitSet(new ulong[]{0x3A2012UL});
		public static readonly BitSet _EOF_in_template103 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ACTION_in_action118 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IF_in_action130 = new BitSet(new ulong[]{0x3A2010UL});
		public static readonly BitSet _template_in_action139 = new BitSet(new ulong[]{0x1C0UL});
		public static readonly BitSet _ELSEIF_in_action151 = new BitSet(new ulong[]{0x3A2010UL});
		public static readonly BitSet _template_in_action162 = new BitSet(new ulong[]{0x1C0UL});
		public static readonly BitSet _ELSE_in_action181 = new BitSet(new ulong[]{0x3A2010UL});
		public static readonly BitSet _template_in_action192 = new BitSet(new ulong[]{0x100UL});
		public static readonly BitSet _ENDIF_in_action208 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _REGION_REF_in_action216 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _REGION_DEF_in_action228 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Antlr3.ST.Language
