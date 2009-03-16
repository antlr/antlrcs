/*
 * [The "BSD licence"]
 * Copyright (c) 2003-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.ST
{
    using System;

    /** <summary>
     *  When group files derive from another group, we have to know how to
     *  load that group and its supergroups.  This interface also knows how
     *  to load interfaces.
     *  </summary>
     */
    public interface IStringTemplateGroupLoader
    {
        /** <summary>
         *  Load the group called groupName from somewhere.  Return null
         *  if no group is found.
         *  </summary>
         */
        StringTemplateGroup LoadGroup( string groupName );

        /** <summary>
         *  Load a group with a specified superGroup.  Groups with
         *  region definitions must know their supergroup to find templates
         *  during parsing.
         *  </summary>
         */
        StringTemplateGroup LoadGroup( string groupName,
                                             StringTemplateGroup superGroup );


        /** <summary>
         *  Specify the template lexer to use for parsing templates.  If null,
         *  it assumes angle brackets &lt;...>.
         *  </summary>
         */
        StringTemplateGroup LoadGroup( string groupName,
                                             Type templateLexer,
                                             StringTemplateGroup superGroup );

        /** <summary>
         *  Load the interface called interfaceName from somewhere.  Return null
         *  if no interface is found.
         *  </summary>
         */
        StringTemplateGroupInterface LoadInterface( string interfaceName );
    }
}
