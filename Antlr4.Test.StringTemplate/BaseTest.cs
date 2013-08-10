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

namespace Antlr4.Test.StringTemplate
{
    using Antlr.Runtime;
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Compiler;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ArgumentException = System.ArgumentException;
    using CultureInfo = System.Globalization.CultureInfo;
    using DateTime = System.DateTime;
    using Directory = System.IO.Directory;
    using Environment = System.Environment;
    using File = System.IO.File;
    using Path = System.IO.Path;
    using StringBuilder = System.Text.StringBuilder;
    using Thread = System.Threading.Thread;

    [TestClass]
    public abstract class BaseTest
    {
        public static string tmpdir;
        public static readonly string newline = Environment.NewLine;

        public TestContext TestContext
        {
            get;
            set;
        }

        [TestInitialize]
        public void setUp()
        {
            setUpImpl();
        }

        protected virtual void setUpImpl()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");
            TemplateGroup.DefaultGroup = new TemplateGroup();
            TemplateCompiler.subtemplateCount = 0;

            // new output dir for each test
            tmpdir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "st4-" + currentTimeMillis()));
        }

        [TestCleanup]
        public void tearDown()
        {
            // Remove tmpdir if no error. how?
            if (TestContext != null && TestContext.CurrentTestOutcome == UnitTestOutcome.Passed)
                eraseTempDir();
        }

        protected virtual void eraseTempDir()
        {
            if (Directory.Exists(tmpdir))
                Directory.Delete(tmpdir, true);
        }

        public static long currentTimeMillis()
        {
            return DateTime.Now.ToFileTime() / 10000;
        }

        public static void writeFile(string dir, string fileName, string content)
        {
            if (Path.IsPathRooted(fileName))
                throw new ArgumentException();

            string fullPath = Path.GetFullPath(Path.Combine(dir, fileName));
            dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(fullPath, content);
        }

        public void checkTokens(string template, string expected)
        {
            checkTokens(template, expected, '<', '>');
        }


        public void checkTokens(string template, string expected, char delimiterStartChar, char delimiterStopChar)
        {
            TemplateLexer lexer =
                new TemplateLexer(TemplateGroup.DefaultErrorManager,
                            new ANTLRStringStream(template),
                            null,
                            delimiterStartChar,
                            delimiterStopChar);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int i = 1;
            IToken t = tokens.LT(i);
            while (t.Type != CharStreamConstants.EndOfFile)
            {
                if (i > 1)
                    buf.Append(", ");
                buf.Append(t);
                i++;
                t = tokens.LT(i);
            }
            buf.Append("]");
            string result = buf.ToString();
            Assert.AreEqual(expected, result);
        }

        public class User
        {
            public int id;
            public string name;

            public User(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public virtual bool IsManager
            {
                get
                {
                    return true;
                }
            }

            public virtual bool HasParkingSpot
            {
                get
                {
                    return true;
                }
            }

            public virtual string Name
            {
                get
                {
                    return name;
                }
            }

            public static string StaticMethod()
            {
                return "method_result";
            }

            public static string StaticProperty
            {
                get { return "property_result"; } 
            }
        }

        public class HashableUser : User
        {
            public HashableUser(int id, string name) : base(id, name)
            {
            }

            public override int GetHashCode()
            {
                return id;
            }

            public override bool Equals(object o)
            {
                HashableUser hu = o as HashableUser;
                if (hu != null)
                    return this.id == hu.id && string.Equals(this.name, hu.name);

                return false;
            }
        }

#if false
        public static string getRandomDir()
        {
            string randomDir = tmpdir + "dir" + String.valueOf((int)(Math.random() * 100000));
            File f = new File(randomDir);
            f.mkdirs();
            return randomDir;
        }
#endif
    }
}
