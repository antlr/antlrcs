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

    partial class ActionParser
    {
        protected StringTemplate self = null;

        protected ActionParser( ITokenSource lexer, int k )
            : this( new CommonTokenStream( lexer ) )
        {
        }
        public ActionParser( ITokenSource lexer, StringTemplate self )
            : this( lexer, 2 )
        {
            //this( new CommonTokenStream( lexer ), 2 );
            this.self = self;
        }

        protected override void Initialize()
        {
            TreeAdaptor = new StringTemplateTreeAdaptor();
            base.Initialize();
        }

        public override void ReportError( RecognitionException e )
        {
            StringTemplateGroup group = self.getGroup();
            if ( group == StringTemplate.defaultGroup )
            {
                self.error( "action parse error; template context is " + self.getEnclosingInstanceStackString(), e );
            }
            else
            {
                self.error( "action parse error in group " + self.getGroup().Name + " line " + self.getGroupFileLine() + "; template context is " + self.getEnclosingInstanceStackString(), e );
            }
        }
    }
}
