/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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
    using System.Linq;

    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;

    public class Cat : List<object>
    {
        public Cat( IEnumerable attributes )
            : base( Unfold( attributes.Cast<object>() ).ToArray() )
        {
        }

        static IEnumerable<object> Unfold( IEnumerable<object> items )
        {
            return items.SelectMany( ( item ) =>
            {
                item = ASTExpr.ConvertAnythingIteratableToIterator( item );
                if ( item is string )
                    return Enumerable.Repeat( item, 1 );
                else if ( item is IEnumerable )
                    return ( (IEnumerable)item ).Cast<object>();
                else if ( item is IEnumerator )
                    return Enumerate((IEnumerator)item);
                else
                    return Enumerable.Repeat( item, 1 );
            } );
        }

        private static IEnumerable<object> Enumerate(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
