/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr3.Tool
{
    using System.Collections.Generic;

    using ANTLRFileStream = Antlr.Runtime.ANTLRFileStream;
    using ANTLRInputStream = Antlr.Runtime.ANTLRInputStream;
    using ANTLRLexer = Antlr3.Grammars.ANTLRLexer;
    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using ICharStream = Antlr.Runtime.ICharStream;
    using CommonTree = Antlr.Runtime.Tree.CommonTree;
    using Console = System.Console;
    using IToken = Antlr.Runtime.IToken;
    using TokenRewriteStream = Antlr.Runtime.TokenRewriteStream;
    using ITreeAdaptor = Antlr.Runtime.Tree.ITreeAdaptor;
    using TreeWizard = Antlr.Runtime.Tree.TreeWizard;

    /** A basic action stripper. */
    public class Strip
    {
        protected string filename;
        protected TokenRewriteStream tokens;
        protected bool tree_option = false;
        protected string[] args;

#if BUILD_STRIPTOOL
        public static void Main( string[] args )
        {
            Strip s = new Strip( args );
            s.ParseAndRewrite();
            Console.Out.WriteLine( s.tokens );
        }
#endif

        public Strip( string[] args )
        {
            this.args = args;
        }

        public virtual TokenRewriteStream GetTokenStream()
        {
            return tokens;
        }

        public virtual void ParseAndRewrite()
        {
            ProcessArgs( args );
            ICharStream input = null;
            if ( filename != null )
                input = new ANTLRFileStream( filename );
            else
                input = new ANTLRInputStream( Console.In );
            // BUILD AST
            ANTLRLexer lex = new ANTLRLexer( input );
            tokens = new TokenRewriteStream( lex );
            ANTLRParser g = new ANTLRParser( tokens );
            Grammar grammar = new Grammar();
            var r = g.grammar_( grammar );
            CommonTree t = (CommonTree)r.Tree;
            if ( tree_option )
                Console.Out.WriteLine( t.ToStringTree() );
            Rewrite( g.TreeAdaptor, t, g.GetTokenNames() );
        }

        public virtual void Rewrite( ITreeAdaptor adaptor, CommonTree t, string[] tokenNames )
        {
            TreeWizard wiz = new TreeWizard( adaptor, tokenNames );

            // ACTIONS STUFF
            wiz.Visit( t, ANTLRParser.ACTION, ( tree ) =>
            {
                ACTION( tokens, (CommonTree)tree );
            } );

            // ^('@' id ACTION) rule actions
            wiz.Visit( t, ANTLRParser.AMPERSAND, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                CommonTree action = null;
                if ( a.ChildCount == 2 )
                    action = (CommonTree)a.GetChild( 1 );
                else if ( a.ChildCount == 3 )
                    action = (CommonTree)a.GetChild( 2 );
                if ( action.Type == ANTLRParser.ACTION )
                {
                    tokens.Delete( a.TokenStartIndex, a.TokenStopIndex );
                    KillTrailingNewline( tokens, action.TokenStopIndex );
                }
            } );

            wiz.Visit( t, ANTLRParser.ACTION, ( tree ) =>
            {
            } );

            // wipe rule arguments
            wiz.Visit( t, ANTLRParser.ARG, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                a = (CommonTree)a.GetChild( 0 );
                tokens.Delete( a.token.TokenIndex );
                KillTrailingNewline( tokens, a.token.TokenIndex );
            } );

            // wipe rule return declarations
            wiz.Visit( t, ANTLRParser.RET, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                CommonTree ret = (CommonTree)a.GetChild( 0 );
                tokens.Delete( a.token.TokenIndex, ret.token.TokenIndex );
            } );

            // comment out semantic predicates
            wiz.Visit( t, ANTLRParser.SEMPRED, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                tokens.Replace( a.token.TokenIndex, "/*" + a.Text + "*/" );
            } );

            // comment out semantic predicates
            wiz.Visit( t, ANTLRParser.GATED_SEMPRED, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                string text = tokens.ToString( a.TokenStartIndex, a.TokenStopIndex );
                tokens.Replace( a.TokenStartIndex, a.TokenStopIndex, "/*" + text + "*/" );
            } );

            // comment scope specs
            wiz.Visit( t, ANTLRParser.SCOPE, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                tokens.Delete( a.TokenStartIndex, a.TokenStopIndex );
                KillTrailingNewline( tokens, a.TokenStopIndex );
            } );

            // args r[x,y] -> ^(r [x,y])
            wiz.Visit( t, ANTLRParser.ARG_ACTION, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                if ( a.Parent.Type == ANTLRParser.RULE_REF )
                    tokens.Delete( a.TokenStartIndex, a.TokenStopIndex );
            } );

#if false // what is this token type in the C# ported version of the V3 grammar?
            // ^('=' id ^(RULE_REF [arg])), ...
            wiz.Visit( t, ANTLRParser.LABEL_ASSIGN, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                if ( !a.HasAncestor( ANTLRParser.OPTIONS ) )
                {
                    // avoid options
                    CommonTree child = (CommonTree)a.GetChild( 0 );
                    // kill "id="
                    tokens.Delete( a.token.TokenIndex );
                    tokens.Delete( child.token.TokenIndex );
                }
            } );
#endif

#if false // what is this token type in the C# ported version of the V3 grammar?
            // ^('+=' id ^(RULE_REF [arg])), ...
            wiz.Visit( t, ANTLRParser.LIST_LABEL_ASSIGN, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                CommonTree child = (CommonTree)a.GetChild( 0 );
                // kill "id+="
                tokens.Delete( a.token.TokenIndex );
                tokens.Delete( child.token.TokenIndex );
            } );
#endif

            // AST STUFF

            wiz.Visit( t, ANTLRParser.REWRITE, ( tree ) =>
            {
                CommonTree a = (CommonTree)t;
                CommonTree child = (CommonTree)a.GetChild( 0 );
                int stop = child.TokenStopIndex;
                if ( child.Type == ANTLRParser.SEMPRED )
                {
                    CommonTree rew = (CommonTree)a.GetChild( 1 );
                    stop = rew.TokenStopIndex;
                }
                tokens.Delete( a.token.TokenIndex, stop );
                KillTrailingNewline( tokens, stop );
            } );

            wiz.Visit( t, ANTLRParser.ROOT, ( tree ) =>
            {
                tokens.Delete( ( (CommonTree)t ).token.TokenIndex );
            } );

            wiz.Visit( t, ANTLRParser.BANG, ( tree ) =>
            {
                tokens.Delete( ( (CommonTree)t ).token.TokenIndex );
            } );
        }

        public static void ACTION( TokenRewriteStream tokens, CommonTree t )
        {
            CommonTree parent = (CommonTree)t.Parent;
            int ptype = parent.Type;
            if ( ptype == ANTLRParser.SCOPE || // we have special rules for these
                 ptype == ANTLRParser.AMPERSAND )
            {
                return;
            }
            //Console.Out.WriteLine( "ACTION: " + t.Text );
            CommonTree root = (CommonTree)t.GetAncestor( ANTLRParser.RULE );
            if ( root != null )
            {
                CommonTree rule = (CommonTree)root.GetChild( 0 );
                //Console.Out.WriteLine( "rule: " + rule );
                if ( !char.IsUpper( rule.Text[0] ) )
                {
                    tokens.Delete( t.TokenStartIndex, t.TokenStopIndex );
                    KillTrailingNewline( tokens, t.token.TokenIndex );
                }
            }
        }

        private static void KillTrailingNewline( TokenRewriteStream tokens, int index )
        {
            IList<IToken> all = tokens.GetTokens();
            IToken tok = all[index];
            IToken after = all[index + 1];
            string ws = after.Text;
            if ( ws.StartsWith( "\n" ) )
            {
                //Console.Out.WriteLine( "killing WS after action" );
                if ( ws.Length > 1 )
                {
                    int space = ws.IndexOf( ' ' );
                    int tab = ws.IndexOf( '\t' );
                    if ( ws.StartsWith( "\n" ) && space >= 0 || tab >= 0 )
                    {
                        return; // do nothing if \n + indent
                    }
                    // otherwise kill all \n
                    ws = ws.Replace( "\n", "" );
                    tokens.Replace( after.TokenIndex, ws );
                }
                else
                {
                    tokens.Delete( after.TokenIndex );
                }
            }
        }

        public virtual void ProcessArgs( string[] args )
        {
            if ( args == null || args.Length == 0 )
            {
                Help();
                return;
            }
            for ( int i = 0; i < args.Length; i++ )
            {
                if ( args[i].Equals( "-tree" ) )
                    tree_option = true;
                else
                {
                    if ( args[i][0] != '-' )
                    {
                        // Must be the grammar file
                        filename = args[i];
                    }
                }
            }
        }

        private static void Help()
        {
            Console.Error.WriteLine( "usage: java org.antlr.tool.Strip [args] file.g" );
            Console.Error.WriteLine( "  -tree      print out ANTLR grammar AST" );
        }
    }
}
