/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
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

namespace Antlr4.StringTemplate.Debug
{
    public class EvalExprEvent : InterpEvent
    {
        // template pattern location
        private readonly int exprStart;
        private readonly int exprStop;
        private readonly string expr;

        public EvalExprEvent(DebugST self, int start, int stop, int exprStart, int exprStop)
            : base(self, start, stop)
        {
            this.exprStart = exprStart;
            this.exprStop = exprStop;
            if (exprStart >= 0 && exprStop >= 0)
            {
                expr = self.impl.template.Substring(exprStart, exprStop + 1 - exprStart);
            }
        }

        public int ExprStart
        {
            get
            {
                return exprStart;
            }
        }

        public int ExprStop
        {
            get
            {
                return exprStop;
            }
        }

        public string Expr
        {
            get
            {
                return expr;
            }
        }

        public override string ToString()
        {
            return GetType().Name + "{" +
                   "self=" + Self +
                 //", attr=" + self.attributes +
                   ", start=" + Start +
                   ", stop=" + Stop +
                   ", expr=" + expr +
                   '}';
        }
    }
}
