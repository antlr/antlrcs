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

namespace AntlrUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using IList = System.Collections.IList;
    using IntervalSet = Antlr3.Misc.IntervalSet;
    using Label = Antlr3.Analysis.Label;

    [TestClass]
    public class TestIntervalSet : BaseTest
    {

        /** Public default constructor used by TestRig */
        public TestIntervalSet()
        {
        }

        [TestMethod]
        public void TestSingleElement() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 99 );
            string expecting = "99";
            assertEquals( s.ToString(), expecting );
        }

        [TestMethod]
        public void TestIsolatedElements() /*throws Exception*/ {
            IntervalSet s = new IntervalSet();
            s.add( 1 );
            s.add( 'z' );
            s.add( '\uFFF0' );
            string expecting = "{1, 122, 65520}";
            assertEquals( s.ToString(), expecting );
        }

        [TestMethod]
        public void TestMixedRangesAndElements() /*throws Exception*/ {
            IntervalSet s = new IntervalSet();
            s.add( 1 );
            s.add( 'a', 'z' );
            s.add( '0', '9' );
            string expecting = "{1, 48..57, 97..122}";
            assertEquals( s.ToString(), expecting );
        }

        [TestMethod]
        public void TestSimpleAnd() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 13, 15 );
            string expecting = "13..15";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestRangeAndIsolatedElement() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 'a', 'z' );
            IntervalSet s2 = IntervalSet.of( 'd' );
            string expecting = "100";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestEmptyIntersection() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 'a', 'z' );
            IntervalSet s2 = IntervalSet.of( '0', '9' );
            string expecting = "{}";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestEmptyIntersectionSingleElements() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 'a' );
            IntervalSet s2 = IntervalSet.of( 'd' );
            string expecting = "{}";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestNotSingleElement() /*throws Exception*/ {
            IntervalSet vocabulary = IntervalSet.of( 1, 1000 );
            vocabulary.add( 2000, 3000 );
            IntervalSet s = IntervalSet.of( 50, 50 );
            string expecting = "{1..49, 51..1000, 2000..3000}";
            string result = ( s.complement( vocabulary ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestNotSet() /*throws Exception*/ {
            IntervalSet vocabulary = IntervalSet.of( 1, 1000 );
            IntervalSet s = IntervalSet.of( 50, 60 );
            s.add( 5 );
            s.add( 250, 300 );
            string expecting = "{1..4, 6..49, 61..249, 301..1000}";
            string result = ( s.complement( vocabulary ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestNotEqualSet() /*throws Exception*/ {
            IntervalSet vocabulary = IntervalSet.of( 1, 1000 );
            IntervalSet s = IntervalSet.of( 1, 1000 );
            string expecting = "{}";
            string result = ( s.complement( vocabulary ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestNotSetEdgeElement() /*throws Exception*/ {
            IntervalSet vocabulary = IntervalSet.of( 1, 2 );
            IntervalSet s = IntervalSet.of( 1 );
            string expecting = "2";
            string result = ( s.complement( vocabulary ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestNotSetFragmentedVocabulary() /*throws Exception*/ {
            IntervalSet vocabulary = IntervalSet.of( 1, 255 );
            vocabulary.add( 1000, 2000 );
            vocabulary.add( 9999 );
            IntervalSet s = IntervalSet.of( 50, 60 );
            s.add( 3 );
            s.add( 250, 300 );
            s.add( 10000 ); // this is outside range of vocab and should be ignored
            string expecting = "{1..2, 4..49, 61..249, 1000..2000, 9999}";
            string result = ( s.complement( vocabulary ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSubtractOfCompletelyContainedRange() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 12, 15 );
            string expecting = "{10..11, 16..20}";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSubtractOfOverlappingRangeFromLeft() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 5, 11 );
            string expecting = "12..20";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );

            IntervalSet s3 = IntervalSet.of( 5, 10 );
            expecting = "11..20";
            result = ( s.subtract( s3 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSubtractOfOverlappingRangeFromRight() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 15, 25 );
            string expecting = "10..14";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );

            IntervalSet s3 = IntervalSet.of( 20, 25 );
            expecting = "10..19";
            result = ( s.subtract( s3 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSubtractOfCompletelyCoveredRange() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 1, 25 );
            string expecting = "{}";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSubtractOfRangeSpanningMultipleRanges() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            s.add( 30, 40 );
            s.add( 50, 60 ); // s has 3 ranges now: 10..20, 30..40, 50..60
            IntervalSet s2 = IntervalSet.of( 5, 55 ); // covers one and touches 2nd range
            string expecting = "56..60";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );

            IntervalSet s3 = IntervalSet.of( 15, 55 ); // touches both
            expecting = "{10..14, 56..60}";
            result = ( s.subtract( s3 ) ).ToString();
            assertEquals( result, expecting );
        }

        /** The following was broken:
            {0..113, 115..65534}-{0..115, 117..65534}=116..65534
         */
        [TestMethod]
        public void TestSubtractOfWackyRange() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 0, 113 );
            s.add( 115, 200 );
            IntervalSet s2 = IntervalSet.of( 0, 115 );
            s2.add( 117, 200 );
            string expecting = "116";
            string result = ( s.subtract( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSimpleEquals() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 10, 20 );
            Boolean expecting = true;
            Boolean result = s.Equals( s2 );
            assertEquals( result, expecting );

            IntervalSet s3 = IntervalSet.of( 15, 55 );
            expecting = false;
            result = s.Equals( s3 );
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestEquals() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            s.add( 2 );
            s.add( 499, 501 );
            IntervalSet s2 = IntervalSet.of( 10, 20 );
            s2.add( 2 );
            s2.add( 499, 501 );
            Boolean expecting = true;
            Boolean result = s.Equals( s2 );
            assertEquals( result, expecting );

            IntervalSet s3 = IntervalSet.of( 10, 20 );
            s3.add( 2 );
            expecting = false;
            result = s.Equals( s3 );
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSingleElementMinusDisjointSet() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 15, 15 );
            IntervalSet s2 = IntervalSet.of( 1, 5 );
            s2.add( 10, 20 );
            string expecting = "{}"; // 15 - {1..5, 10..20} = {}
            string result = s.subtract( s2 ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestMembership() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 15, 15 );
            s.add( 50, 60 );
            assertTrue( !s.member( 0 ) );
            assertTrue( !s.member( 20 ) );
            assertTrue( !s.member( 100 ) );
            assertTrue( s.member( 15 ) );
            assertTrue( s.member( 55 ) );
            assertTrue( s.member( 50 ) );
            assertTrue( s.member( 60 ) );
        }

        // {2,15,18} & 10..20
        [TestMethod]
        public void TestIntersectionWithTwoContainedElements() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 2, 2 );
            s2.add( 15 );
            s2.add( 18 );
            string expecting = "{15, 18}";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestIntersectionWithTwoContainedElementsReversed() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 10, 20 );
            IntervalSet s2 = IntervalSet.of( 2, 2 );
            s2.add( 15 );
            s2.add( 18 );
            string expecting = "{15, 18}";
            string result = ( s2.and( s ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestComplement() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 100, 100 );
            s.add( 101, 101 );
            IntervalSet s2 = IntervalSet.of( 100, 102 );
            string expecting = "102";
            string result = ( s.complement( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestComplement2() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 100, 101 );
            IntervalSet s2 = IntervalSet.of( 100, 102 );
            string expecting = "102";
            string result = ( s.complement( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestComplement3() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 1, 96 );
            s.add( 99, Label.MAX_CHAR_VALUE );
            string expecting = "97..98";
            string result = ( s.complement( 1, Label.MAX_CHAR_VALUE ) ).ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestMergeOfRangesAndSingleValues() /*throws Exception*/ {
            // {0..41, 42, 43..65534}
            IntervalSet s = IntervalSet.of( 0, 41 );
            s.add( 42 );
            s.add( 43, 65534 );
            string expecting = "0..65534";
            string result = s.ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestMergeOfRangesAndSingleValuesReverse() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 43, 65534 );
            s.add( 42 );
            s.add( 0, 41 );
            string expecting = "0..65534";
            string result = s.ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestMergeWhereAdditionMergesTwoExistingIntervals() /*throws Exception*/ {
            // 42, 10, {0..9, 11..41, 43..65534}
            IntervalSet s = IntervalSet.of( 42 );
            s.add( 10 );
            s.add( 0, 9 );
            s.add( 43, 65534 );
            s.add( 11, 41 );
            string expecting = "0..65534";
            string result = s.ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestMergeWithDoubleOverlap() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 1, 10 );
            s.add( 20, 30 );
            s.add( 5, 25 ); // overlaps two!
            string expecting = "1..30";
            string result = s.ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestSize() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 20, 30 );
            s.add( 50, 55 );
            s.add( 5, 19 );
            string expecting = "32";
            string result = s.size().ToString();
            assertEquals( result, expecting );
        }

        [TestMethod]
        public void TestToList() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 20, 25 );
            s.add( 50, 55 );
            s.add( 5, 5 );
            string expecting = "[5, 20, 21, 22, 23, 24, 25, 50, 51, 52, 53, 54, 55]";
            IList foo = new List<object>();
            //String result = String.valueOf( s.toList() );
            string result = "[" + string.Join( ", ", s.toArray().Select( i => i.ToString() ).ToArray() ) + "]";
            assertEquals( result, expecting );
        }

        /** The following was broken:
            {'\u0000'..'s', 'u'..'\uFFFE'} & {'\u0000'..'q', 's'..'\uFFFE'}=
            {'\u0000'..'q', 's'}!!!! broken...
            'q' is 113 ascii
            'u' is 117
        */
        [TestMethod]
        public void TestNotRIntersectionNotT() /*throws Exception*/ {
            IntervalSet s = IntervalSet.of( 0, 's' );
            s.add( 'u', 200 );
            IntervalSet s2 = IntervalSet.of( 0, 'q' );
            s2.add( 's', 200 );
            string expecting = "{0..113, 115, 117..200}";
            string result = ( s.and( s2 ) ).ToString();
            assertEquals( result, expecting );
        }

    }
}
