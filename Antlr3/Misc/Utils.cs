/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
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

#if DEBUG

namespace Antlr3.Misc
{
    using System;

    public static class Utils
    {
        /** Integer objects are immutable so share all Integers with the
         *  same value up to some max size.  Use an array as a perfect hash.
         *  Return shared object for 0..INTEGER_POOL_MAX_VALUE or a new
         *  Integer object with x in it.
         */
        [Obsolete]
        public static int integer( int x )
        {
            //if ( x<0 || x>INTEGER_POOL_MAX_VALUE ) {
            //    return new Integer(x);
            //}
            //if ( ints[x]==null ) {
            //    ints[x] = new Integer(x);
            //}
            //return ints[x];
            return x;
        }

        /** Given a source string, src,
            a string to replace, replacee,
            and a string to replace with, replacer,
            return a new string w/ the replacing done.
            You can use replacer==null to remove replacee from the string.

            This should be faster than Java's String.replaceAll as that one
            uses regex (I only want to play with strings anyway).
        */
        [Obsolete]
        public static string replace( string src, string replacee, string replacer )
        {
            return src.Replace( replacee, replacer );
        }
    }
}

#endif
