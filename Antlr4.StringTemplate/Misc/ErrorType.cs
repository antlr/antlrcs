/*
 * [The "BSD license"]
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

namespace Antlr4.StringTemplate.Misc
{
    using DebuggerDisplayAttribute = System.Diagnostics.DebuggerDisplayAttribute;

    /** All the errors that can happen and how to generate a message */
    [DebuggerDisplay("{Message}")]
    public sealed class ErrorType
    {
        // RUNTIME SEMANTIC ERRORS
        public static readonly ErrorType NO_SUCH_TEMPLATE = new ErrorType("no such template: {0}");
        public static readonly ErrorType NO_IMPORTED_TEMPLATE = new ErrorType("no such template: super.{0}");
        public static readonly ErrorType NO_SUCH_ATTRIBUTE = new ErrorType("attribute {0} isn't defined");
        public static readonly ErrorType NO_SUCH_ATTRIBUTE_PASS_THROUGH = new ErrorType("could not pass through undefined attribute {0}");
        public static readonly ErrorType REF_TO_IMPLICIT_ATTRIBUTE_OUT_OF_SCOPE = new ErrorType("implicitly-defined attribute {0} not visible");
        public static readonly ErrorType MISSING_FORMAL_ARGUMENTS = new ErrorType("missing argument definitions");
        public static readonly ErrorType NO_SUCH_PROPERTY = new ErrorType("no such property or can't access: {0}");
        public static readonly ErrorType MAP_ARGUMENT_COUNT_MISMATCH = new ErrorType("iterating through {0} values in zip map but template has {1} declared arguments");
        public static readonly ErrorType ARGUMENT_COUNT_MISMATCH = new ErrorType("passed {0} arg(s) to template {1} with {2} declared arg(s)");
        public static readonly ErrorType EXPECTING_STRING = new ErrorType("function {0} expects a string not {1}");
        public static readonly ErrorType CANT_IMPORT = new ErrorType("can't find template(s) in import \"{0}\"");

        // COMPILE-TIME SYNTAX/SEMANTIC ERRORS
        public static readonly ErrorType SYNTAX_ERROR = new ErrorType("{0}");
        public static readonly ErrorType TEMPLATE_REDEFINITION = new ErrorType("redefinition of template {0}");
        public static readonly ErrorType EMBEDDED_REGION_REDEFINITION = new ErrorType("region {0} is embedded and thus already implicitly defined");
        public static readonly ErrorType REGION_REDEFINITION = new ErrorType("redefinition of region {0}");
        public static readonly ErrorType HIDDEN_EMBEDDED_REGION_DEFINITION = new ErrorType("the explicit definition of region {0} hides an embedded definition in the same group");
        public static readonly ErrorType MAP_REDEFINITION = new ErrorType("redefinition of dictionary {0}");
        public static readonly ErrorType ALIAS_TARGET_UNDEFINED = new ErrorType("cannot alias {0} to undefined template: {1}");
        public static readonly ErrorType TEMPLATE_REDEFINITION_AS_MAP = new ErrorType("redefinition of template {0} as a map");
        public static readonly ErrorType LEXER_ERROR = new ErrorType("{0}");
        public static readonly ErrorType NO_DEFAULT_VALUE = new ErrorType("missing dictionary default value");
        public static readonly ErrorType NO_SUCH_FUNCTION = new ErrorType("no such function: {0}");
        public static readonly ErrorType NO_SUCH_REGION = new ErrorType("template {0} doesn't have a region called {1}");
        public static readonly ErrorType NO_SUCH_OPTION = new ErrorType("no such option: {0}");
        public static readonly ErrorType INVALID_TEMPLATE_NAME = new ErrorType("invalid template name or path: {0}");
        public static readonly ErrorType ANON_ARGUMENT_MISMATCH = new ErrorType("anonymous template has {0} arg(s) but mapped across {1} value(s)");
        public static readonly ErrorType REQUIRED_PARAMETER_AFTER_OPTIONAL = new ErrorType("Optional parameters must appear after all required parameters");
        public static readonly ErrorType INVALID_DELIMITER = new ErrorType("Invalid template delimiter: \"{0}\"");

        // INTERNAL ERRORS
        public static readonly ErrorType INTERNAL_ERROR = new ErrorType("{0}");
        public static readonly ErrorType WRITE_IO_ERROR = new ErrorType("error writing output caused by");
        public static readonly ErrorType CANT_LOAD_GROUP_FILE = new ErrorType("can't Load group file {0}");

        private readonly string _message;

        private ErrorType(string m)
        {
            _message = m;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
