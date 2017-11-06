/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Grammar conversion to ANTLR v3 and C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *	notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *	notice, this list of conditions and the following disclaimer in the
 *	documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *	derived from this software without specific prior written permission.
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
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

#if COMPILE_EXPRESSIONS
    using System.Collections.Generic;

    using ArrayList = System.Collections.ArrayList;
    using ICollection = System.Collections.ICollection;
    using ILGenerator = System.Reflection.Emit.ILGenerator;
    using LocalBuilder = System.Reflection.Emit.LocalBuilder;
    using MethodInfo = System.Reflection.MethodInfo;
    using OpCodes = System.Reflection.Emit.OpCodes;
    using PropertyInfo = System.Reflection.PropertyInfo;
    using StringWriter = System.IO.StringWriter;
#endif

    partial class ActionEvaluator
    {
        public class NameValuePair
        {
            public string name;
            public object value;
        }

        protected StringTemplate self = null;
        protected IStringTemplateWriter writer = null;
        protected ASTExpr chunk = null;

        public ActionEvaluator( StringTemplate self, ASTExpr chunk, IStringTemplateWriter writer, ITree input )
            : base( new CommonTreeNodeStream( new StringTemplateTreeAdaptor(), input ) )
        {
            this.self = self;
            this.chunk = chunk;
            this.writer = writer;
        }

        public override void ReportError( RecognitionException e )
        {
            self.Error( "eval tree parse error", e );
        }

#if COMPILE_EXPRESSIONS
        static void EmitLoadChunk( ILGenerator gen )
        {
            // Stack behavior: ... => ..., (ASTExpr)chunk
            gen.Emit( OpCodes.Ldarg_0 );
        }
        static void EmitLoadSelf( ILGenerator gen )
        {
            // Stack behavior: ... => ..., (StringTemplate)self
            gen.Emit( OpCodes.Ldarg_1 );
        }
        static void EmitLoadWriter( ILGenerator gen )
        {
            // Stack behavior: ... => ..., (IStringTemplateWriter)writer
            gen.Emit( OpCodes.Ldarg_2 );
        }

        static void EmitNot( ILGenerator gen )
        {
            // Stack behavior: ..., (int32)value => ..., (int32)(value == 0 ? 1 : 0)
            gen.Emit( OpCodes.Ldc_I4_0 );
            gen.Emit( OpCodes.Ceq );
        }
        static void EmitTest( ILGenerator gen )
        {
            // Stack behavior: ..., (object)a => ..., (bool)result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object a, ASTExpr chunk ) => chunk.TestAttributeTrue( a ) ) );
        }
        static void EmitAdd( ILGenerator gen )
        {
            // Stack behavior: ..., (object)a, (object)b => ..., (object)result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object a, object b, ASTExpr chunk ) => chunk.Add( a, b ) ) );
        }
        static void EmitWriteToString( ILGenerator gen )
        {
            // Stack behavior: ..., (object)value => ..., (string)result

            //System.Func<object, StringTemplate, object> write = ( value, self ) =>
            //{
            //    StringWriter buf = new StringWriter();
            //    IStringTemplateWriter sw = self.Group.GetStringTemplateWriter( buf );
            //    int n = chunk.WriteAttribute( self, value, sw );
            //    if ( n > 0 )
            //        return buf.ToString();
            //    return value;
            //};

            var value = gen.DeclareLocal( typeof( object ) );
            var buf = gen.DeclareLocal( typeof( StringWriter ) );
            var sw = gen.DeclareLocal( typeof( IStringTemplateWriter ) );

            var preserveValue = gen.DefineLabel();
            var endOfWrite = gen.DefineLabel();

            gen.Emit( OpCodes.Stloc, value );

            gen.Emit( OpCodes.Newobj, typeof( StringWriter ) );
            gen.Emit( OpCodes.Stloc, buf );

            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( StringTemplate self ) => self.Group ) );
            gen.Emit( OpCodes.Ldloc, buf );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( StringTemplateGroup group, System.IO.TextWriter writer ) => group.GetStringTemplateWriter( writer ) ) );
            gen.Emit( OpCodes.Stloc, sw );

            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, value );
            gen.Emit( OpCodes.Ldloc, sw );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( ASTExpr chunk, StringTemplate self, object o, IStringTemplateWriter writer ) => chunk.WriteAttribute( self, o, writer ) ) );
            gen.Emit( OpCodes.Ldc_I4_0 );
            gen.Emit( OpCodes.Ble, preserveValue );

            gen.Emit( OpCodes.Ldloc, buf );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( StringWriter writer ) => writer.ToString() ) );
            gen.Emit( OpCodes.Br_S, endOfWrite );

            gen.MarkLabel( preserveValue );
            gen.Emit( OpCodes.Ldloc, value );

            gen.MarkLabel( endOfWrite );
        }
        static void EmitObjectProperty( ILGenerator gen )
        {
            // Stack behavior: ..., (object)o, (object)propertyName => ..., (object)result
            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object o, object propertyName, ASTExpr chunk, StringTemplate self ) => chunk.GetObjectProperty( self, o, propertyName ) ) );
        }
        static void EmitAttribute( ILGenerator gen, string attribute )
        {
            // Stack behavior: ... => ..., (object)result

            //$value=self.GetAttribute($i3.text);
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldstr, attribute );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( StringTemplate self, string attr ) => self.GetAttribute( attr ) ) );
        }
        static void EmitLoadIntAsObject( ILGenerator gen, int value )
        {
            // Stack behavior: ... => ..., (object)result
            gen.Emit( OpCodes.Ldc_I4, value );
            gen.Emit( OpCodes.Box, typeof( int ) );
        }
        static void EmitLoadString( ILGenerator gen, string value )
        {
            // Stack behavior: ... => ..., (string)result
            gen.Emit( OpCodes.Ldstr, value );
        }
        static void EmitAnonymousTemplate( ILGenerator gen, string value )
        {
            // Stack behavior: ... => ..., (StringTemplate)result

            //System.Func<StringTemplate, string, object> loadTemplate = ( self, text ) =>
            //{
            //    if ( text != null )
            //    {
            //        StringTemplate valueST = new StringTemplate( self.Group, text );
            //        valueST.EnclosingInstance = self;
            //        valueST.Name = "<anonymous template argument>";
            //        return valueST;
            //    }
            //    return null;
            //};

            if ( value != null )
            {
                EmitLoadSelf( gen );
                gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( StringTemplate self ) => (object)self.Group ) );
                gen.Emit( OpCodes.Ldstr, value );
                gen.Emit( OpCodes.Newobj, typeof( StringTemplate ).GetConstructor( new System.Type[] { typeof( StringTemplateGroup ), typeof( string ) } ) );
                // copies for set EnclosingInstance, set Name, and one left on the evaluation stack
                gen.Emit( OpCodes.Dup );
                gen.Emit( OpCodes.Dup );
                EmitLoadSelf( gen );
                gen.Emit( OpCodes.Call, GetActionMethodInfo( ( StringTemplate v, StringTemplate self ) => v.EnclosingInstance = self ) );
                gen.Emit( OpCodes.Call, GetActionMethodInfo( ( StringTemplate v ) => v.Name = "<anonymous template argument>" ) );
            }
            else
            {
                gen.Emit( OpCodes.Ldnull );
            }
        }
        static List<StringTemplate> _anonymousTemplates = new List<StringTemplate>();
        static void EmitApplyAnonymousTemplate( ILGenerator gen, StringTemplate anonymous, LocalBuilder attributes )
        {
            // Stack behavior: ... => ..., (StringTemplate)result

            int index;
            lock ( _anonymousTemplates )
            {
                index = _anonymousTemplates.Count;
                _anonymousTemplates.Add( anonymous );
            }

            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, attributes );
            gen.Emit( OpCodes.Ldc_I4, index );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( int i ) => _anonymousTemplates[i] ) );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo(
                ( ASTExpr chunk, StringTemplate self, List<object> attr, StringTemplate anon ) =>
                    chunk.ApplyTemplateToListOfAttributes( self, attr, anon ) ) );
        }
        static void EmitTemplateInclude( ILGenerator gen, StringTemplateAST args )
        {
            var name = gen.DeclareLocal( typeof( string ) );

            var ldnull = gen.DefineLabel();
            var endinclude = gen.DefineLabel();

            gen.Emit( OpCodes.Dup );
            gen.Emit( OpCodes.Brfalse_S, endinclude ); // the dup of a null object already loaded null back on the stack
            gen.Emit( OpCodes.Call, GetFuncMethodInfo<object, string>( ( o ) => o.ToString() ) );
            // at this point, the name is the top item on the evaluation stack
            gen.Emit( OpCodes.Dup );
            gen.Emit( OpCodes.Brfalse_S, endinclude );
            gen.Emit( OpCodes.Stloc, name );

            // $value = chunk.GetTemplateInclude(self, name, args);
            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, name );

            // TODO: handle args
            throw new System.NotImplementedException();

            //gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "GetTemplateInclude", new System.Type[] { typeof( StringTemplate ), typeof( string ), typeof( StringTemplateAST ) } ) );

            //gen.MarkLabel( endinclude );
        }

        static void EmitWriteAttribute( ILGenerator gen )
        {
            // Stack behavior: ..., (object)value => ..., (int32)result

            // $numCharsWritten = chunk.WriteAttribute(self,$expr.value,writer);
            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            EmitLoadWriter( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo(
                ( object o, ASTExpr chunk, StringTemplate self, IStringTemplateWriter writer ) => chunk.WriteAttribute( self, o, writer )
                ) );
        }

        static void EmitFunctionFirst( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.First( value ) ) );
        }
        static void EmitFunctionRest( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.Rest( value ) ) );
        }
        static void EmitFunctionLast( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.Last( value ) ) );
        }
        static void EmitFunctionLength( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.Length( value ) ) );
        }
        static void EmitFunctionStrip( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.Strip( value ) ) );
        }
        static void EmitFunctionTrunc( ILGenerator gen )
        {
            // Stack behavior: ..., value => ..., result
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( object value, ASTExpr chunk ) => chunk.Trunc( value ) ) );
        }

        static LocalBuilder EmitCreateList<T>( ILGenerator gen )
        {
            var local = gen.DeclareLocal( typeof( List<T> ) );
            gen.Emit( OpCodes.Newobj, typeof( List<T> ).GetConstructor( new System.Type[0] ) );
            gen.Emit( OpCodes.Stloc, local );
            return local;
        }
        static void EmitAddValueToList( ILGenerator gen, LocalBuilder local )
        {
            var label1 = gen.DefineLabel();
            var label2 = gen.DefineLabel();
            gen.Emit( OpCodes.Dup );
            gen.Emit( OpCodes.Brfalse_S, label1 );
            gen.Emit( OpCodes.Ldloc, local );
            gen.Emit( OpCodes.Call, GetActionMethodInfo( ( object value, List<object> list ) => list.Add( value ) ) );
            gen.Emit( OpCodes.Br_S, label2 );
            gen.MarkLabel( label1 );
            gen.Emit( OpCodes.Pop );
            gen.MarkLabel( label2 );
        }
        static void EmitAddNothingToList( ILGenerator gen, LocalBuilder local )
        {
            gen.Emit( OpCodes.Ldc_I4_1 );
            gen.Emit( OpCodes.Newarr, typeof( object ) );
            gen.Emit( OpCodes.Newobj, typeof( ArrayList ).GetConstructor( new System.Type[] { typeof( ICollection ) } ) );
            gen.Emit( OpCodes.Ldloc, local );
            gen.Emit( OpCodes.Call, GetActionMethodInfo( ( object value, List<object> list ) => list.Add( value ) ) );
        }
        static void EmitCatList( ILGenerator gen, LocalBuilder local )
        {
            gen.Emit( OpCodes.Ldloc, local );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo( ( List<object> list ) => new Cat( list ) ) );
        }

        static void EmitApplyAlternatingTemplates( ILGenerator gen, LocalBuilder local )
        {
            //{$value = chunk.ApplyListOfAlternatingTemplates(self,$a.value,templatesToApply);}
            gen.Emit( OpCodes.Ldloc, local );
            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Call, GetFuncMethodInfo(
                ( object value, List<StringTemplate> templates, ASTExpr chunk, StringTemplate self ) => chunk.ApplyListOfAlternatingTemplates( self, value, templates )
                ) );
        }

        static MethodInfo GetActionMethodInfo( System.Action method )
        {
            return method.Method;
        }
        static MethodInfo GetActionMethodInfo<T>( System.Action<T> method )
        {
            return method.Method;
        }
        static MethodInfo GetActionMethodInfo<T1, T2>( System.Action<T1, T2> method )
        {
            return method.Method;
        }

        static MethodInfo GetFuncMethodInfo<TResult>( System.Func<TResult> method )
        {
            return method.Method;
        }
        static MethodInfo GetFuncMethodInfo<T, TResult>( System.Func<T, TResult> method )
        {
            return method.Method;
        }
        static MethodInfo GetFuncMethodInfo<T1, T2, TResult>( System.Func<T1, T2, TResult> method )
        {
            return method.Method;
        }
        static MethodInfo GetFuncMethodInfo<T1, T2, T3, TResult>( System.Func<T1, T2, T3, TResult> method )
        {
            return method.Method;
        }
        static MethodInfo GetFuncMethodInfo<T1, T2, T3, T4, TResult>( System.Func<T1, T2, T3, T4, TResult> method )
        {
            return method.Method;
        }
#endif
    }

}
