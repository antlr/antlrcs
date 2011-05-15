/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr.Runtime.Misc
{
    using System.Collections.Generic;

    using Directory = System.IO.Directory;
    using Environment = System.Environment;
    using File = System.IO.File;
    using Math = System.Math;
    using Path = System.IO.Path;

    /** <summary>Stats routines needed by profiler etc...</summary>
     *
     *  <remarks>
     *  note that these routines return 0.0 if no values exist in the X[]
     *  which is not "correct", but it is useful so I don't generate NaN
     *  in my output
     *  </remarks>
     */
    public class Stats
    {
        public const string ANTLRWORKS_DIR = "antlrworks";

        /** <summary>Compute the sample (unbiased estimator) standard deviation following:</summary>
         *
         *  <remarks>
         *  Computing Deviations: Standard Accuracy
         *  Tony F. Chan and John Gregg Lewis
         *  Stanford University
         *  Communications of ACM September 1979 of Volume 22 the ACM Number 9
         *
         *  The "two-pass" method from the paper; supposed to have better
         *  numerical properties than the textbook summation/sqrt.  To me
         *  this looks like the textbook method, but I ain't no numerical
         *  methods guy.
         *  </remarks>
         */
        public static double Stddev( int[] X )
        {
            int m = X.Length;
            if ( m <= 1 )
            {
                return 0;
            }
            double xbar = Average(X);
            double s2 = 0.0;
            for ( int i = 0; i < m; i++ )
            {
                s2 += ( X[i] - xbar ) * ( X[i] - xbar );
            }
            s2 = s2 / ( m - 1 );
            return Math.Sqrt( s2 );
        }
        public static double Stddev( List<int> X )
        {
            int m = X.Count;
            if ( m <= 1 )
            {
                return 0;
            }
            double xbar = Average(X);
            double s2 = 0.0;
            for ( int i = 0; i < m; i++ )
            {
                s2 += ( X[i] - xbar ) * ( X[i] - xbar );
            }
            s2 = s2 / ( m - 1 );
            return Math.Sqrt( s2 );
        }

        /** <summary>Compute the sample mean</summary> */
        public static double Average(ICollection<int> X)
        {
            if (X.Count == 0)
                return 0.0;

            double sum = 0.0;
            foreach (int i in X)
                sum += i;

            return sum / X.Count;
        }

        public static void WriteReport( string filename, string data )
        {
            string absoluteFilename = GetAbsoluteFileName( filename );

            Directory.CreateDirectory( Path.GetDirectoryName( absoluteFilename ) );
            File.AppendAllText( absoluteFilename, data );
        }

        public static string GetAbsoluteFileName( string filename )
        {
            string personalFolder = Environment.GetFolderPath( Environment.SpecialFolder.Personal );
            return personalFolder + Path.DirectorySeparatorChar +
                        ANTLRWORKS_DIR + Path.DirectorySeparatorChar +
                        filename;
        }
    }
}
