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

    /** A tree of grammars */
    public class CompositeGrammarTree
    {
        protected internal IList<CompositeGrammarTree> children;
        public Grammar grammar;

        /** Who is the parent node of this node; if null, implies node is root */
        public CompositeGrammarTree parent;

        public CompositeGrammarTree( Grammar g )
        {
            grammar = g;
        }

        public virtual void AddChild( CompositeGrammarTree t )
        {
            //Console.Out.WriteLine("add "+t.toStringTree()+" as child to "+this.toStringTree());
            if ( t == null )
            {
                return; // do nothing upon addChild(null)
            }
            if ( children == null )
            {
                children = new List<CompositeGrammarTree>();
            }
            children.Add( t );
            t.parent = this;
        }

        /** Find a rule by looking in current grammar then down towards the
         *  delegate grammars.
         */
        public virtual Rule GetRule( string ruleName )
        {
            Rule r = grammar.GetLocallyDefinedRule( ruleName );
            for ( int i = 0; r == null && children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                r = child.GetRule( ruleName );
            }
            return r;
        }

        /** Find an option by looking up towards the root grammar rather than down */
        public virtual object GetOption( string key )
        {
            if (key == "language" && grammar.Tool != null && grammar.Tool.ForcedLanguageOption != null)
                return grammar.Tool.ForcedLanguageOption;

            object o = grammar.GetLocallyDefinedOption( key );
            if ( o != null )
            {
                return o;
            }
            if ( parent != null )
            {
                return parent.GetOption( key );
            }
            return null; // not found
        }

        public virtual CompositeGrammarTree FindNode( Grammar g )
        {
            if ( g == null )
            {
                return null;
            }
            if ( this.grammar == g )
            {
                return this;
            }
            CompositeGrammarTree n = null;
            for ( int i = 0; n == null && children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                n = child.FindNode( g );
            }
            return n;
        }

        public virtual CompositeGrammarTree FindNode( string grammarName )
        {
            if ( grammarName == null )
            {
                return null;
            }
            if ( grammarName.Equals( this.grammar.name ) )
            {
                return this;
            }
            CompositeGrammarTree n = null;
            for ( int i = 0; n == null && children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                n = child.FindNode( grammarName );
            }
            return n;
        }

        /** Return a postorder list of grammars; root is last in list */
        public IList<Grammar> GetPostOrderedGrammarList()
        {
            IList<Grammar> grammars = new List<Grammar>();
            GetPostOrderedGrammarListCore( grammars );
            return grammars;
        }

        /** work for getPostOrderedGrammarList */
        protected virtual void GetPostOrderedGrammarListCore( IList<Grammar> grammars )
        {
            for ( int i = 0; children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                child.GetPostOrderedGrammarListCore( grammars );
            }
            grammars.Add( this.grammar );
        }

        /** Return a preorder list of grammars; root is first in list */
        public IList<Grammar> GetPreOrderedGrammarList()
        {
            IList<Grammar> grammars = new List<Grammar>();
            GetPreOrderedGrammarListCore( grammars );
            return grammars;
        }

        protected virtual void GetPreOrderedGrammarListCore( IList<Grammar> grammars )
        {
            grammars.Add( this.grammar );
            for ( int i = 0; children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                child.GetPreOrderedGrammarListCore( grammars );
            }
        }

        public virtual void TrimLexerImportsIntoCombined()
        {
            CompositeGrammarTree p = this;
            if ( p.grammar.type == GrammarType.Lexer && p.parent != null &&
                 p.parent.grammar.type == GrammarType.Combined )
            {
                //System.Console.Out.WriteLine( "wacking " + p.grammar.name + " from " + p.parent.grammar.name );
                p.parent.children.Remove( this );
            }
            for ( int i = 0; children != null && i < children.Count; i++ )
            {
                CompositeGrammarTree child = children[i];
                child.TrimLexerImportsIntoCombined();
            }
        }
    }
}
