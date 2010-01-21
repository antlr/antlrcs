/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace StringTemplate.Compiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Console = System.Console;
    using StringBuilder = System.Text.StringBuilder;

    public class CompiledTemplate
    {
        /** The original, immutable pattern (not really used again after
         *  initial "compilation"). Useful for debugging.
         */
        public string template;

        // for subtemplates:
        public int embeddedStart = -1;
        public int embeddedStop = -1;

        protected internal IDictionary<string, FormalArgument> formalArguments = FormalArgument.Unknown;

        protected internal List<CompiledTemplate> implicitlyDefinedTemplates;

        /** The group that holds this ST definition.  We use it to initiate
         *  interpretation via ST.toString().  From there, it becomes field 'group'
         *  in interpreter and is fixed until rendering completes.
         */
        public TemplateGroup nativeGroup = TemplateGroup.defaultGroup;

        /** Does this template come from a <@region>...<@end> embedded in
         *  another template?
         */
        protected internal bool isRegion;

        /** If someone refs <@r()> in template t, an implicit
         *
         *   @t.r() ::= ""
         *
         *  is defined, but you can overwrite this def by defining your
         *  own.  We need to prevent more than one manual def though.  Between
         *  this var and isEmbeddedRegion we can determine these cases.
         */
        protected internal Template.RegionType regionDefType;

        public string[] strings;
        public byte[] instrs;        // byte-addressable code memory.
        public int codeSize;
        public Interval[] sourceMap; // maps IP to range in template pattern

        [DebuggerHidden]
        public string Disassembly
        {
            get
            {
                BytecodeDisassembler dis = new BytecodeDisassembler(this);
                StringBuilder buffer = new StringBuilder();
                buffer.AppendLine(dis.Disassemble());
                buffer.AppendLine("Strings:");
                buffer.AppendLine(dis.Strings());
                buffer.AppendLine("Bytecode to template map:");
                buffer.AppendLine(dis.SourceMap());
                return buffer.ToString();
            }
        }

        public TemplateName Name
        {
            get;
            internal set;
        }

        public string Template
        {
            get
            {
                return template;
            }
        }

        public bool IsSubtemplate
        {
            get;
            set;
        }

        public virtual string Instructions()
        {
            var disassembler = new BytecodeDisassembler(this);
            return disassembler.Instructions();
        }

        public virtual void Dump()
        {
            var disassembler = new BytecodeDisassembler(this);
            Console.WriteLine(disassembler.Disassemble());
            Console.WriteLine("Strings:");
            Console.WriteLine(disassembler.Strings());
        }
    }
}
