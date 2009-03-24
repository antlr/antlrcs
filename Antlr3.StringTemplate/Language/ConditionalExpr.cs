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
#if COMPILE_EXPRESSIONS
    using DynamicMethod = System.Reflection.Emit.DynamicMethod;
    using OpCodes = System.Reflection.Emit.OpCodes;
    using ParameterAttributes = System.Reflection.ParameterAttributes;
    using Type = System.Type;
#endif

    /** <summary>A conditional reference to an embedded subtemplate.</summary> */
    public class ConditionalExpr : ASTExpr
    {
#if COMPILE_EXPRESSIONS
        System.Func<StringTemplate, IStringTemplateWriter, bool> EvaluateCondition;
#endif
        StringTemplate _subtemplate;
        List<ElseIfClauseData> _elseIfSubtemplates;
        StringTemplate _elseSubtemplate;

        protected class ElseIfClauseData
        {
#if COMPILE_EXPRESSIONS
            public System.Func<StringTemplate, IStringTemplateWriter, bool> EvaluateCondition;
#endif
            public ASTExpr expr;
            public StringTemplate st;
        }

        public ConditionalExpr( StringTemplate enclosingTemplate, ITree tree )
            : base( enclosingTemplate, tree, null )
        {
        }

        public StringTemplate Subtemplate
        {
            get
            {
                return _subtemplate;
            }
            set
            {
                _subtemplate = value;
            }
        }
        public StringTemplate ElseSubtemplate
        {
            get
            {
                return _elseSubtemplate;
            }
            set
            {
                _elseSubtemplate = value;
            }
        }

#if COMPILE_EXPRESSIONS
        public class HoldsConditionFuncAndChunk
        {
            public System.Func<ASTExpr, StringTemplate, IStringTemplateWriter, bool> func;
            public ASTExpr chunk;
        }
        public static bool CallFunctionalConditionEvaluator( HoldsConditionFuncAndChunk data, StringTemplate self, IStringTemplateWriter writer )
        {
            return data.func( data.chunk, self, writer );
        }

        static int _evaluatorNumber = 0;
#if CACHE_FUNCTORS
        static Dictionary<ITree, DynamicMethod> _methods = new Dictionary<ITree, DynamicMethod>();
#endif
        static System.Func<StringTemplate, IStringTemplateWriter, bool> GetEvaluator( ASTExpr chunk, ITree condition )
        {
            if ( EnableDynamicMethods )
            {
                try
                {
                    DynamicMethod method = null;
#if CACHE_FUNCTORS
                    if ( !_methods.TryGetValue( condition, out method ) )
#endif
                    {
                        Type[] parameterTypes = { typeof( ASTExpr ), typeof( StringTemplate ), typeof( IStringTemplateWriter ) };
                        method = new DynamicMethod( "ConditionEvaluator" + _evaluatorNumber, typeof( bool ), parameterTypes, typeof( ConditionalExpr ), true );
                        method.DefineParameter( 1, ParameterAttributes.None, "chunk" );
                        method.DefineParameter( 2, ParameterAttributes.None, "self" );
                        method.DefineParameter( 3, ParameterAttributes.None, "writer" );
                        _evaluatorNumber++;

                        var gen = method.GetILGenerator();
                        ActionEvaluator evalCompiled = new ActionEvaluator( null, chunk, null, condition );
                        evalCompiled.ifConditionCompiled( gen );
                        gen.Emit( OpCodes.Ret );
#if CACHE_FUNCTORS
                        _methods[condition] = method;
#endif
                    }

                    var dynamicEvaluator = (System.Func<StringTemplate, IStringTemplateWriter, bool>)method.CreateDelegate( typeof( System.Func<StringTemplate, IStringTemplateWriter, bool> ), chunk );
                    return dynamicEvaluator;
                }
                catch
                {
                    // fall back to functional (or interpreted) version
                }
            }

            if ( EnableFunctionalMethods )
            {
                try
                {
                    ActionEvaluator evalFunctional = new ActionEvaluator( null, chunk, null, condition );
                    var functionalEvaluator = evalFunctional.ifConditionFunctional();
                    HoldsConditionFuncAndChunk holder = new HoldsConditionFuncAndChunk()
                    {
                        func = functionalEvaluator,
                        chunk = chunk
                    };
                    return (System.Func<StringTemplate, IStringTemplateWriter, bool>)System.Delegate.CreateDelegate( typeof( System.Func<StringTemplate, IStringTemplateWriter, bool> ), holder, typeof( ConditionalExpr ).GetMethod( "CallFunctionalConditionEvaluator" ) );
                }
                catch
                {
                    // fall back to interpreted version
                }
            }

            return new System.Func<StringTemplate, IStringTemplateWriter, bool>( ( self, @out ) =>
            {
                ActionEvaluator eval = new ActionEvaluator( self, chunk, @out, condition );
                return eval.ifCondition();
            } );
        }
#endif

        public virtual void AddElseIfSubtemplate( ASTExpr conditionalTree,
                                         StringTemplate subtemplate )
        {
            if ( _elseIfSubtemplates == null )
            {
                _elseIfSubtemplates = new List<ElseIfClauseData>();
            }
            ElseIfClauseData d = new ElseIfClauseData()
            {
                expr = conditionalTree,
                st = subtemplate
            };
            _elseIfSubtemplates.Add( d );
        }

        /** <summary>
         *  To write out the value of a condition expr, invoke the evaluator in eval.g
         *  to walk the condition tree computing the boolean value.  If result
         *  is true, then write subtemplate.
         *  </summary>
         */
        public override int Write( StringTemplate self, IStringTemplateWriter @out )
        {
            if ( AST == null || self == null || @out == null )
            {
                return 0;
            }
            //System.Console.Out.WriteLine( "evaluating conditional tree: " + AST.ToStringTree() );
#if !COMPILE_EXPRESSIONS
            ActionEvaluator eval = null;
#endif
            int n = 0;
            try
            {
                bool testedTrue = false;
                // get conditional from tree and compute result
#if COMPILE_EXPRESSIONS
                if ( EvaluateCondition == null )
                    EvaluateCondition = GetEvaluator( this, AST.GetChild( 0 ) );
                bool includeSubtemplate = EvaluateCondition( self, @out ); // eval and write out tree
#else
                ITree cond = AST.GetChild( 0 );
                eval = new ActionEvaluator( self, this, @out, cond );
                // eval and write out trees
                bool includeSubtemplate = eval.ifCondition();
#endif
                //System.Console.Out.WriteLine( "subtemplate " + _subtemplate );
                // IF
                if ( includeSubtemplate )
                {
                    n = WriteSubTemplate( self, @out, _subtemplate );
                    testedTrue = true;
                }
                // ELSEIF
                else if ( _elseIfSubtemplates != null && _elseIfSubtemplates.Count > 0 )
                {
                    for ( int i = 0; i < _elseIfSubtemplates.Count; i++ )
                    {
                        ElseIfClauseData elseIfClause = _elseIfSubtemplates[i];
#if COMPILE_EXPRESSIONS
                        if ( elseIfClause.EvaluateCondition == null )
                            elseIfClause.EvaluateCondition = GetEvaluator( this, elseIfClause.expr.AST );
                        includeSubtemplate = elseIfClause.EvaluateCondition( self, @out );
#else
                        eval = new ActionEvaluator( self, this, @out, elseIfClause.expr.AST );
                        includeSubtemplate = eval.ifCondition();
#endif
                        if ( includeSubtemplate )
                        {
                            WriteSubTemplate( self, @out, elseIfClause.st );
                            testedTrue = true;
                            break;
                        }
                    }
                }
                // ELSE
                if ( !testedTrue && _elseSubtemplate != null )
                {
                    // evaluate ELSE clause if present and IF condition failed
                    StringTemplate s = _elseSubtemplate.GetInstanceOf();
                    s.EnclosingInstance = self;
                    s.Group = self.Group;
                    s.NativeGroup = self.NativeGroup;
                    n = s.Write( @out );
                }
            }
            catch ( RecognitionException re )
            {
                self.Error( "can't evaluate tree: " + AST.ToStringTree(), re );
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
            s.EnclosingInstance = self;
            // make sure we evaluate in context of enclosing template's
            // group so polymorphism works. :)
            s.Group = self.Group;
            s.NativeGroup = self.NativeGroup;
            return s.Write( @out );
        }
    }
}
