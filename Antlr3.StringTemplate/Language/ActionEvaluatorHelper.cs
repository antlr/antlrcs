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
    using ILGenerator = System.Reflection.Emit.ILGenerator;
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
            gen.Emit( OpCodes.Ldarg_0 );
        }
        static void EmitLoadSelf( ILGenerator gen )
        {
            gen.Emit( OpCodes.Ldarg_1 );
        }
        static void EmitLoadWriter( ILGenerator gen )
        {
            gen.Emit( OpCodes.Ldarg_2 );
        }

        static void EmitNot( ILGenerator gen )
        {
            gen.Emit( OpCodes.Ldc_I4_0 );
            gen.Emit( OpCodes.Ceq );
        }
        static void EmitTest( ILGenerator gen )
        {
            var local = gen.DeclareLocal( typeof( object ) );
            gen.Emit( OpCodes.Stloc, local );
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Ldloc, local );
            gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "TestAttributeTrue", new System.Type[] { typeof( object ) } ) );
        }
        static void EmitAdd( ILGenerator gen )
        {
            //System.Func<object, object, object> add = ( a, b ) => chunk.Add( a, b );
            var local1 = gen.DeclareLocal( typeof( object ) );
            var local2 = gen.DeclareLocal( typeof( object ) );
            gen.Emit( OpCodes.Stloc, local2 );
            gen.Emit( OpCodes.Stloc, local1 );
            EmitLoadChunk( gen );
            gen.Emit( OpCodes.Call, typeof( ASTExpr ).GetMethod( "Add", new System.Type[] { typeof( object ), typeof( object ) } ) );
        }
        static void EmitWriteToString( ILGenerator gen )
        {
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
            gen.Emit( OpCodes.Callvirt, typeof( StringTemplate ).GetProperty( "Group" ).GetGetMethod() );
            gen.Emit( OpCodes.Ldloc, buf );
            gen.Emit( OpCodes.Callvirt, typeof( StringTemplateGroup ).GetMethod( "GetStringTemplateWriter", new System.Type[] { typeof( System.IO.TextWriter ) } ) );
            gen.Emit( OpCodes.Stloc, sw );

            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, value );
            gen.Emit( OpCodes.Ldloc, sw );
            gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "WriteAttribute", new System.Type[] { typeof( StringTemplate ), typeof( object ), typeof( IStringTemplateWriter ) } ) );
            gen.Emit( OpCodes.Ldc_I4_0 );
            gen.Emit( OpCodes.Ble, preserveValue );

            gen.Emit( OpCodes.Ldloc, buf );
            gen.Emit( OpCodes.Callvirt, typeof( StringWriter ).GetMethod( "ToString", new System.Type[0] ) );
            gen.Emit( OpCodes.Br_S, endOfWrite );

            gen.MarkLabel( preserveValue );
            gen.Emit( OpCodes.Ldloc, value );

            gen.MarkLabel( endOfWrite );
        }
        static void EmitObjectProperty( ILGenerator gen )
        {
            var local2 = gen.DeclareLocal( typeof( object ) );
            gen.Emit( OpCodes.Stloc, local2 );
            var local1 = gen.DeclareLocal( typeof( object ) );
            gen.Emit( OpCodes.Stloc, local1 );

            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, local1 );
            gen.Emit( OpCodes.Ldloc, local2 );
            gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "GetObjectProperty", new System.Type[] { typeof( StringTemplate ), typeof( object ), typeof( object ) } ) );
        }
        static void EmitAttribute( ILGenerator gen, string attribute )
        {
            //$value=self.GetAttribute($i3.text);
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldstr, attribute );
            gen.Emit( OpCodes.Callvirt, typeof( StringTemplate ).GetMethod( "GetAttribute", new System.Type[] { typeof( string ) } ) );
        }
        static void EmitLoadIntAsObject( ILGenerator gen, int value )
        {
            gen.Emit( OpCodes.Ldc_I4, value );
            gen.Emit( OpCodes.Box, typeof( int ) );
        }
        static void EmitLoadString( ILGenerator gen, string value )
        {
            gen.Emit( OpCodes.Ldstr, value );
        }
        static void EmitAnonymousTemplate( ILGenerator gen, string value )
        {
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
                var valueST = gen.DeclareLocal( typeof( StringTemplate ) );

                EmitLoadSelf( gen );
                gen.Emit( OpCodes.Callvirt, typeof( StringTemplate ).GetProperty( "Group" ).GetGetMethod() );
                gen.Emit( OpCodes.Ldstr, value );
                gen.Emit( OpCodes.Newobj, typeof( StringTemplate ).GetConstructor( new System.Type[] { typeof( StringTemplateGroup ), typeof( string ) } ) );
                // copies for store, set EnclosingInstance, set Name, and one left on the evaluation stack
                gen.Emit( OpCodes.Dup );
                gen.Emit( OpCodes.Dup );
                gen.Emit( OpCodes.Dup );
                gen.Emit( OpCodes.Stloc, valueST );
                EmitLoadSelf( gen );
                gen.Emit( OpCodes.Callvirt, typeof( StringTemplate ).GetProperty( "EnclosingInstance" ).GetSetMethod() );
                gen.Emit( OpCodes.Ldstr, "<anonymous template argument>" );
                gen.Emit( OpCodes.Callvirt, typeof( StringTemplate ).GetProperty( "Name" ).GetSetMethod() );
            }
            else
            {
                gen.Emit( OpCodes.Ldnull );
            }
        }
        static void EmitTemplateInclude( ILGenerator gen, StringTemplateAST args )
        {
            var name = gen.DeclareLocal( typeof( string ) );

            var ldnull = gen.DefineLabel();
            var endinclude = gen.DefineLabel();

            gen.Emit( OpCodes.Dup );
            gen.Emit( OpCodes.Brfalse_S, endinclude ); // the dup of a null object already loaded null back on the stack
            gen.Emit( OpCodes.Callvirt, typeof( object ).GetMethod( "ToString" ) );
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

            gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "GetTemplateInclude", new System.Type[] { typeof( StringTemplate ), typeof( string ), typeof( StringTemplateAST ) } ) );

            gen.MarkLabel( endinclude );
        }
        static void EmitWriteAttribute( ILGenerator gen )
        {
            // $numCharsWritten = chunk.WriteAttribute(self,$expr.value,writer);

            var value = gen.DeclareLocal( typeof( object ) );
            gen.Emit( OpCodes.Stloc, value );

            EmitLoadChunk( gen );
            EmitLoadSelf( gen );
            gen.Emit( OpCodes.Ldloc, value );
            EmitLoadWriter( gen );
            gen.Emit( OpCodes.Callvirt, typeof( ASTExpr ).GetMethod( "WriteAttribute", new System.Type[] { typeof( StringTemplate ), typeof( object ), typeof( IStringTemplateWriter ) } ) );
        }
        static void EmitFunctionFirst( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
        static void EmitFunctionRest( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
        static void EmitFunctionLast( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
        static void EmitFunctionLength( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
        static void EmitFunctionStrip( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
        static void EmitFunctionTrunc( ILGenerator gen )
        {
            throw new System.NotImplementedException();
        }
#endif
    }

}
