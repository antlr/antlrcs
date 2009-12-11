namespace AntlrUnitTests.ST4
{
    using Antlr.Runtime;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringTemplate;
    using Directory = System.IO.Directory;
    using Environment = System.Environment;
    using File = System.IO.File;
    using IOException = System.IO.IOException;
    using Path = System.IO.Path;
    using Random = System.Random;
    using StringBuilder = System.Text.StringBuilder;

    public abstract class StringTemplateTestBase
    {
        public static readonly string tmpdir = Path.GetTempPath();
        public static readonly string newline = Environment.NewLine;

        private Random random = new Random();

        public TestContext TestContext
        {
            get;
            private set;
        }

        [ClassInitialize]
        public void ClassSetUp(TestContext testContext)
        {
            TestContext = testContext;
        }

        public void WriteFile(string dir, string fileName, string content)
        {
            try
            {
                string f = Path.Combine(dir, fileName);
                if (!Directory.Exists(Path.GetDirectoryName(f)))
                    Directory.CreateDirectory(Path.GetDirectoryName(f));

                File.WriteAllText(f, content);
            }
            catch (IOException ioe)
            {
                TestContext.WriteLine("can't write file");
                TestContext.WriteLine(ioe.StackTrace);
            }
        }

        public void CheckTokens(string template, string expected)
        {
            CheckTokens(template, expected, '<', '>');
        }


        public void CheckTokens(string template, string expected, char delimiterStartChar, char delimiterStopChar)
        {
            TemplateLexer lexer = new TemplateLexer(new ANTLRStringStream(template), delimiterStartChar, delimiterStopChar);
            UnbufferedTokenStream tokens = new UnbufferedTokenStream(lexer);
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

            public string Name
            {
                get
                {
                    return name;
                }
            }
        }

        public class HashableUser : User
        {
            public HashableUser(int id, string name)
                : base(id, name)
            {
            }

            public override int GetHashCode()
            {
                return id;
            }

            public override bool Equals(object o)
            {
                if (o is HashableUser)
                {
                    HashableUser hu = (HashableUser)o;
                    return this.id == hu.id && this.name.Equals(hu.name);
                }

                return false;
            }
        }

        protected string GetRandomDir()
        {
            string randomDir = Path.Combine(tmpdir, "dir" + random.Next(100000));
            Directory.CreateDirectory(randomDir);
            return randomDir;
        }
    }
}
