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

namespace AntlrUnitTests.ST4
{
    public class TestDollarDelimiters : StringTemplateTestBase
    {
        /*
        @Test public void testSimpleAttr() throws Exception {
            String template = "hi $name$";
            List<Chunk> chunks = new Chunkifier(template, '$', '$').chunkify();
            String expected = "[1:0..2:hi , 1:4..7:name]";
            String result = chunks.toString();
            assertEquals(expected, result);
        }
    
        @Test public void testString() throws Exception {
            String template = "hi $foo(a=\"$\")$";
            List<Chunk> chunks = new Chunkifier(template, '$', '$').chunkify();
            String expected = "[1:0..2:hi , 1:4..13:foo(a=\"$\")]";
            String result = chunks.toString();
            assertEquals(expected, result);
        }

        @Test public void testEscInString() throws Exception {
            String template = "hi $foo(a=\"$\\\"\")$";
            List<Chunk> chunks = new Chunkifier(template, '$', '$').chunkify();
            String expected = "[1:0..2:hi , 1:4..15:foo(a=\"$\\\"\")]";
            String result = chunks.toString();
            assertEquals(expected, result);
        }

        @Test public void testSubtemplate() throws Exception {
            String template = "hi $names:{n | $n$}$";
            List<Chunk> chunks = new Chunkifier(template, '$', '$').chunkify();
            String expected = "[1:0..2:hi , 1:4..18:names:{n | $n$}]";
            String result = chunks.toString();
            assertEquals(expected, result);
        }

        @Test public void testNestedSubtemplate() throws Exception {
            String template = "hi $names:{n | $n:{$it$}$}$";
            List<Chunk> chunks = new Chunkifier(template, '$', '$').chunkify();
            String expected = "[1:0..2:hi , 1:4..25:names:{n | $n:{$it$}$}]";
            String result = chunks.toString();
            assertEquals(expected, result);
        }
        */
    }
}
