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

namespace StringTemplate
{
    public sealed class ErrorType
    {
        // RUNTIME SEMANTIC ERRORS
        public static readonly ErrorType NoSuchTemplate = new ErrorType("no such template: {0}");
        public static readonly ErrorType NoImportedTemplate = new ErrorType("no such template: super.{0}");
        //public static readonly ErrorType NoSuchProperty = new ErrorType("{0} doesn't have a {1} property");
        public static readonly ErrorType ExpectingSingleArgument = new ErrorType("expecting single arg in template reference {0} (not {1})");
        public static readonly ErrorType MissingFormalArguments = new ErrorType("missing argument definitions");
        public static readonly ErrorType ArgumentCountMismatch = new ErrorType("iterating through {0} arguments but parallel map has {1} formal arguments");
        public static readonly ErrorType ExpectingString = new ErrorType("function {0} expects a string not {1}");

        // COMPILE-TIME SYNTAX/SEMANTIC ERRORS
        public static readonly ErrorType SyntaxError = new ErrorType("{0}");
        public static readonly ErrorType TemplateRedefinition = new ErrorType("redefinition of template {0}");
        public static readonly ErrorType EmbeddedRegionRedefinition = new ErrorType("region {0} is embedded and thus already implicitly defined");
        public static readonly ErrorType RegionRedefinition = new ErrorType("redefinition of region {0}");
        public static readonly ErrorType MapRedefinition = new ErrorType("redefinition of dictionary {0}");
        public static readonly ErrorType TemplateRedefinitionAsMap = new ErrorType("redefinition of template {0} as a map");
        public static readonly ErrorType LexerError = new ErrorType("lexer there are add character {0}");
        public static readonly ErrorType NoDefaultValue = new ErrorType("missing dictionary default value");
        public static readonly ErrorType NoSuchFunction = new ErrorType("no such function: {0}");
        public static readonly ErrorType NoSuchOption = new ErrorType("no such option: {0}");

        // IO ERRORS
        public static readonly ErrorType WriteIoError = new ErrorType("error writing output");
        public static readonly ErrorType CantLoadGroupFile = new ErrorType("can't load group file {0}");
        public static readonly ErrorType CantLoadTemplateFile = new ErrorType("can't load template file {0}");
        public static readonly ErrorType InvalidBytecode = new ErrorType("invalid bytecode {0} at IP {1}");

        public static readonly ErrorType GuiError = new ErrorType("GUI error");

        private ErrorType(string messageFormat)
        {
            this.MessageFormat = messageFormat;
        }

        public string MessageFormat
        {
            get;
            private set;
        }
    }
}
