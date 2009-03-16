/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
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

namespace Antlr3.ST.Language
{
    using System.Collections.Generic;
    using Antlr.Runtime.JavaExtensions;

    using ITree = Antlr.Runtime.Tree.ITree;
    using RecognitionException = Antlr.Runtime.RecognitionException;

    /** <summary>A conditional reference to an embedded subtemplate.</summary> */
    public class ConditionalExpr : ASTExpr
    {
        StringTemplate subtemplate = null;
        List<ElseIfClauseData> elseIfSubtemplates = null;
        StringTemplate elseSubtemplate = null;

        protected class ElseIfClauseData
        {
            public ASTExpr expr;
            public StringTemplate st;
        }

        public ConditionalExpr( StringTemplate enclosingTemplate, ITree tree ) :
            base( enclosingTemplate, tree, null )
        {
        }

        public virtual void SetSubtemplate( StringTemplate subtemplate )
        {
            this.subtemplate = subtemplate;
        }

        public virtual void AddElseIfSubtemplate( ASTExpr conditionalTree,
                                         StringTemplate subtemplate )
        {
            if ( elseIfSubtemplates == null )
            {
                elseIfSubtemplates = new List<ElseIfClauseData>();
            }
            ElseIfClauseData d = new ElseIfClauseData()
            {
                expr = conditionalTree,
                st = subtemplate
            };
            elseIfSubtemplates.Add( d );
        }

        public virtual StringTemplate GetSubtemplate()
        {
            return subtemplate;
        }

        public virtual StringTemplate GetElseSubtemplate()
        {
            return elseSubtemplate;
        }

        public virtual void SetElseSubtemplate( StringTemplate elseSubtemplate )
        {
            this.elseSubtemplate = elseSubtemplate;
        }

        /** <summary>
         *  To write out the value of a condition expr, invoke the evaluator in eval.g
         *  to walk the condition tree computing the boolean value.  If result
         *  is true, then write subtemplate.
         *  </summary>
         */
        public override int Write( StringTemplate self, IStringTemplateWriter @out )
        {
            if ( exprTree == null || self == null || @out == null )
            {
                return 0;
            }
            // System.out.println("evaluating conditional tree: "+exprTree.toStringList());
            ActionEvaluator eval = null;
            int n = 0;
            try
            {
                bool testedTrue = false;
                // get conditional from tree and compute result
                ITree cond = exprTree.GetChild( 0 );
                eval = new ActionEvaluator( self, this, @out, cond );
                bool includeSubtemplate = eval.ifCondition(); // eval and write out tree
                // System.out.println("subtemplate "+subtemplate);
                // IF
                if ( includeSubtemplate )
                {
                    n = WriteSubTemplate( self, @out, subtemplate );
                    testedTrue = true;
                }
                // ELSEIF
                else if ( elseIfSubtemplates != null && elseIfSubtemplates.Count > 0 )
                {
                    for ( int i = 0; i < elseIfSubtemplates.Count; i++ )
                    {
                        ElseIfClauseData elseIfClause = elseIfSubtemplates[i];
                        eval = new ActionEvaluator( self, this, @out, elseIfClause.expr.exprTree );
                        includeSubtemplate = eval.ifCondition();
                        if ( includeSubtemplate )
                        {
                            WriteSubTemplate( self, @out, elseIfClause.st );
                            testedTrue = true;
                            break;
                        }
                    }
                }
                // ELSE
                if ( !testedTrue && elseSubtemplate != null )
                {
                    // evaluate ELSE clause if present and IF condition failed
                    StringTemplate s = elseSubtemplate.GetInstanceOf();
                    s.SetEnclosingInstance( self );
                    s.SetGroup( self.GetGroup() );
                    s.SetNativeGroup( self.GetNativeGroup() );
                    n = s.Write( @out );
                }
            }
            catch ( RecognitionException re )
            {
                self.Error( "can't evaluate tree: " + exprTree.ToStringTree(), re );
            }
            return n;
        }

        protected virtual int WriteSubTemplate( StringTemplate self,
                                       IStringTemplateWriter @out,
                                       StringTemplate subtemplate )
        {
            /* To evaluate the IF chunk, make a new instance whose enclosingInstance
             * points at 'self' so get attribute works.  Otherwise, enclosingInstance
             * points at the template used to make the precompiled code.  We need a
             * new template instance every time we exec this chunk to get the new
             * "enclosing instance" pointer.
             */
            StringTemplate s = subtemplate.GetInstanceOf();
            s.SetEnclosingInstance( self );
            // make sure we evaluate in context of enclosing template's
            // group so polymorphism works. :)
            s.SetGroup( self.GetGroup() );
            s.SetNativeGroup( self.GetNativeGroup() );
            return s.Write( @out );
        }
    }
}
