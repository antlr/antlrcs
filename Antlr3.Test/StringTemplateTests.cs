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
    using System.Text;
    using Antlr.Runtime.JavaExtensions;
    using Antlr3.ST;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AngleBracketTemplateLexer = Antlr3.ST.Language.AngleBracketTemplateLexer;
    using DefaultTemplateLexer = Antlr3.ST.Language.TemplateLexer;
    using IDictionary = System.Collections.IDictionary;
    using IList = System.Collections.IList;
    using IOException = System.IO.IOException;
    using Path = System.IO.Path;
    using StreamReader = System.IO.StreamReader;
    using StreamWriter = System.IO.StreamWriter;
    using StringReader = System.IO.StringReader;
    using StringWriter = System.IO.StringWriter;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class StringTemplateTests
    {
        static readonly string newline = Environment.NewLine;

        class ErrorBuffer : IStringTemplateErrorListener
        {
            StringBuilder errorOutput = new StringBuilder( 500 );
            int n = 0;
            public void Error( string msg, Exception e )
            {
                n++;
                if ( n > 1 )
                {
                    errorOutput.Append( '\n' );
                }
                if ( e != null )
                {
                    StringWriter duh = new StringWriter();
                    e.PrintStackTrace( duh );
                    errorOutput.Append( msg + ": " + duh.ToString() );
                }
                else
                {
                    errorOutput.Append( msg );
                }
            }
            public void Warning( string msg )
            {
                n++;
                errorOutput.Append( msg );
            }
            public override bool Equals( object o )
            {
                string me = this.ToString();
                string them = o.ToString();
                return me.Equals( them );
            }
            public override int GetHashCode()
            {
                return errorOutput.ToString().GetHashCode();
            }
            public override string ToString()
            {
                return errorOutput.ToString();
            }
        }

        public StringTemplateTests()
        {
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void TestInterfaceFileFormat()
        {
            string groupI =
                    "interface test;" + newline +
                    "t();" + newline +
                    "bold(item);" + newline +
                    "optional duh(a,b,c);" + newline;
            StringTemplateGroupInterface I =
                    new StringTemplateGroupInterface( new StringReader( groupI ) );

            string expecting =
                "interface test;" + newline +
                "t();" + newline +
                "bold(item);" + newline +
                "optional duh(a, b, c);" + newline;
            Assert.AreEqual( expecting, I.ToString() );
        }

        [TestMethod]
        public void TestNoGroupLoader()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( null );

            string templates =
                "group testG implements blort;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            /*StringTemplateGroup group =*/
            new StringTemplateGroup( new StreamReader( System.IO.File.OpenRead( tmpdir + "/testG.stg" ) ), errors );

            string expecting = "no group loader registered";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestCannotFindInterfaceFile()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( new PathGroupLoader( tmpdir, errors ) );

            string templates =
                "group testG implements blort;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ), errors );

            string expecting = "no such interface file blort.sti";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestMultiDirGroupLoading()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            if ( !System.IO.Directory.Exists( System.IO.Path.Combine( tmpdir, "sub" ) ) )
            {
                try
                {
                    System.IO.Directory.CreateDirectory( System.IO.Path.Combine( tmpdir, "sub" ) );
                }
                catch
                { // create a subdir
                    Console.Error.WriteLine( "can't make subdir in test" );
                    return;
                }
            }
            StringTemplateGroup.RegisterGroupLoader(
                new PathGroupLoader( tmpdir + ":" + tmpdir + "/sub", errors )
            );

            string templates =
                "group testG2;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir + "/sub", "testG2.stg", templates );

            StringTemplateGroup group =
                StringTemplateGroup.LoadGroup( "testG2" );
            string expecting = "group testG2;" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline +
                "t() ::= <<foo>>" + newline;
            Assert.AreEqual( expecting, group.ToString() );
        }

        [TestMethod]
        public void TestGroupSatisfiesSingleInterface()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( new PathGroupLoader( tmpdir, errors ) );
            string groupI =
                    "interface testI;" + newline +
                    "t();" + newline +
                    "bold(item);" + newline +
                    "optional duh(a,b,c);" + newline;
            WriteFile( tmpdir, "testI.sti", groupI );

            string templates =
                "group testG implements testI;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                    new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ), errors );

            string expecting = ""; // should be no errors
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestGroupExtendsSuperGroup()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader(
                new PathGroupLoader( tmpdir, errors )
            );
            string superGroup =
                    "group superG;" + newline +
                    "bold(item) ::= <<*$item$*>>;\n" + newline;
            WriteFile( tmpdir, "superG.stg", superGroup );

            string templates =
                "group testG : superG;" + newline +
                "main(x) ::= <<$bold(x)$>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                    new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "main" );
            st.SetAttribute( "x", "foo" );

            string expecting = "*foo*";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestGroupExtendsSuperGroupWithAngleBrackets()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader(
                new PathGroupLoader( tmpdir, errors )
            );
            string superGroup =
                    "group superG;" + newline +
                    "bold(item) ::= <<*<item>*>>;\n" + newline;
            WriteFile( tmpdir, "superG.stg", superGroup );

            string templates =
                "group testG : superG;" + newline +
                "main(x) ::= \"<bold(x)>\"" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                    new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "main" );
            st.SetAttribute( "x", "foo" );

            string expecting = "*foo*";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestMissingInterfaceTemplate()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( new PathGroupLoader( tmpdir, errors ) );
            string groupI =
                    "interface testI;" + newline +
                    "t();" + newline +
                    "bold(item);" + newline +
                    "optional duh(a,b,c);" + newline;
            WriteFile( tmpdir, "testI.sti", groupI );

            string templates =
                "group testG implements testI;" + newline +
                "t() ::= <<foo>>" + newline +
                "duh(a,b,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                    new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ), errors );

            string expecting = "group testG does not satisfy interface testI: missing templates [bold]";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestMissingOptionalInterfaceTemplate()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( new PathGroupLoader( tmpdir, errors ) );
            string groupI =
                    "interface testI;" + newline +
                    "t();" + newline +
                    "bold(item);" + newline +
                    "optional duh(a,b,c);" + newline;
            WriteFile( tmpdir, "testI.sti", groupI );

            string templates =
                "group testG implements testI;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>";

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ), errors );

            string expecting = ""; // should be NO errors
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestMismatchedInterfaceTemplate()
        {
            // this also tests the group loader
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string tmpdir = Path.GetTempPath();
            StringTemplateGroup.RegisterGroupLoader( new PathGroupLoader( tmpdir, errors ) );
            string groupI =
                    "interface testI;" + newline +
                    "t();" + newline +
                    "bold(item);" + newline +
                    "optional duh(a,b,c);" + newline;
            WriteFile( tmpdir, "testI.sti", groupI );

            string templates =
                "group testG implements testI;" + newline +
                "t() ::= <<foo>>" + newline +
                "bold(item) ::= <<foo>>" + newline +
                "duh(a,c) ::= <<foo>>" + newline;

            WriteFile( tmpdir, "testG.stg", templates );

            StringTemplateGroup group =
                new StringTemplateGroup( new StreamReader( tmpdir + "/testG.stg" ), errors );

            string expecting = "group testG does not satisfy interface testI: mismatched arguments on these templates [optional duh(a, b, c)]";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestGroupFileFormat()
        {
            string templates =
                    "group test;" + newline +
                    "t() ::= \"literal template\"" + newline +
                    "bold(item) ::= \"<b>$item$</b>\"" + newline +
                    "duh() ::= <<" + newline + "xx" + newline + ">>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );

            string expecting = "group test;" + newline +
                    "bold(item) ::= <<<b>$item$</b>>>" + newline +
                    "duh() ::= <<xx>>" + newline +
                    "t() ::= <<literal template>>" + newline;
            Assert.AreEqual( expecting, group.ToString() );

            StringTemplate a = group.GetInstanceOf( "t" );
            expecting = "literal template";
            Assert.AreEqual( expecting, a.ToString() );

            StringTemplate b = group.GetInstanceOf( "bold" );
            b.SetAttribute( "item", "dork" );
            expecting = "<b>dork</b>";
            Assert.AreEqual( expecting, b.ToString() );
        }

        [TestMethod]
        public void TestEscapedTemplateDelimiters()
        {
            string templates =
                    "group test;" + newline +
                    "t() ::= <<$\"literal\":{a|$a$\\}}$ template\n>>" + newline +
                    "bold(item) ::= <<<b>$item$</b\\>>>" + newline +
                    "duh() ::= <<" + newline + "xx" + newline + ">>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );

            string expecting = "group test;" + newline +
                    "bold(item) ::= <<<b>$item$</b>>>" + newline +
                    "duh() ::= <<xx>>" + newline +
                    "t() ::= <<$\"literal\":{a|$a$\\}}$ template>>" + newline;
            Assert.AreEqual( expecting, group.ToString() );

            StringTemplate b = group.GetInstanceOf( "bold" );
            b.SetAttribute( "item", "dork" );
            expecting = "<b>dork</b>";
            Assert.AreEqual( expecting, b.ToString() );

            StringTemplate a = group.GetInstanceOf( "t" );
            expecting = "literal} template";
            Assert.AreEqual( expecting, a.ToString() );
        }

        /** Check syntax and setAttribute-time errors */
        [TestMethod]
        public void TestTemplateParameterDecls()
        {
            string templates =
                    "group test;" + newline +
                    "t() ::= \"no args but ref $foo$\"" + newline +
                    "t2(item) ::= \"decl but not used is ok\"" + newline +
                    "t3(a,b,c,d) ::= <<$a$ $d$>>" + newline +
                    "t4(a,b,c,d) ::= <<$a$ $b$ $c$ $d$>>" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );

            // check setting unknown arg in empty formal list
            StringTemplate a = group.GetInstanceOf( "t" );
            string error = null;
            try
            {
                a.SetAttribute( "foo", "x" ); // want NoSuchElementException
            }
            catch ( ArgumentException e )
            {
                error = e.Message;
            }
            string expecting = "no such attribute: foo in template context [t]";
            Assert.AreEqual( expecting, error );

            // check setting known arg
            a = group.GetInstanceOf( "t2" );
            a.SetAttribute( "item", "x" ); // shouldn't get exception

            // check setting unknown arg in nonempty list of formal args
            a = group.GetInstanceOf( "t3" );
            a.SetAttribute( "b", "x" );
        }

        [TestMethod]
        public void TestTemplateRedef()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"x\"" + newline +
                    "b() ::= \"y\"" + newline +
                    "a() ::= \"z\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group = new StringTemplateGroup( new StringReader( templates ), errors );
            string expecting = "redefinition of template: a";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestMissingInheritedAttribute()
        {
            string templates =
                    "group test;" + newline +
                    "page(title,font) ::= <<" + newline +
                    "<html>" + newline +
                    "<body>" + newline +
                    "$title$<br>" + newline +
                    "$body()$" + newline +
                    "</body>" + newline +
                    "</html>" + newline +
                    ">>" + newline +
                    "body() ::= \"<font face=$font$>my body</font>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            t.SetAttribute( "title", "my title" );
            t.SetAttribute( "font", "Helvetica" ); // body() will see it
            t.ToString(); // should be no problem
        }

        [TestMethod]
        public void TestFormalArgumentAssignment()
        {
            string templates =
                    "group test;" + newline +
                    "page() ::= <<$body(font=\"Times\")$>>" + newline +
                    "body(font) ::= \"<font face=$font$>my body</font>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            string expecting = "<font face=Times>my body</font>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestUndefinedArgumentAssignment()
        {
            string templates =
                    "group test;" + newline +
                    "page(x) ::= <<$body(font=x)$>>" + newline +
                    "body() ::= \"<font face=$font$>my body</font>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            t.SetAttribute( "x", "Times" );
            string error = "";
            try
            {
                t.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expecting = "template body has no such attribute: font in template context [page <invoke body arg context>]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestFormalArgumentAssignmentInApply()
        {
            string templates =
                    "group test;" + newline +
                    "page(name) ::= <<$name:bold(font=\"Times\")$>>" + newline +
                    "bold(font) ::= \"<font face=$font$><b>$it$</b></font>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            t.SetAttribute( "name", "Ter" );
            string expecting = "<font face=Times><b>Ter</b></font>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestUndefinedArgumentAssignmentInApply()
        {
            string templates =
                    "group test;" + newline +
                    "page(name,x) ::= <<$name:bold(font=x)$>>" + newline +
                    "bold() ::= \"<font face=$font$><b>$it$</b></font>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            t.SetAttribute( "x", "Times" );
            t.SetAttribute( "name", "Ter" );
            string error = "";
            try
            {
                t.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expecting = "template bold has no such attribute: font in template context [page <invoke bold arg context>]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestUndefinedAttributeReference()
        {
            string templates =
                    "group test;" + newline +
                    "page() ::= <<$bold()$>>" + newline +
                    "bold() ::= \"$name$\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            string error = "";
            try
            {
                t.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expecting = "no such attribute: name in template context [page bold]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestUndefinedDefaultAttributeReference()
        {
            string templates =
                    "group test;" + newline +
                    "page() ::= <<$bold()$>>" + newline +
                    "bold() ::= \"$it$\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate t = group.GetInstanceOf( "page" );
            string error = "";
            try
            {
                t.ToString();
            }
            catch ( ArgumentException nse )
            {
                error = nse.Message;
            }
            string expecting = "no such attribute: it in template context [page bold]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestAngleBracketsWithGroupFile()
        {
            string templates =
                    "group test;" + newline +
                    "a(s) ::= \"<s:{case <i> : <it> break;}>\"" + newline +
                    "b(t) ::= \"<t; separator=\\\",\\\">\"" + newline +
                    "c(t) ::= << <t; separator=\",\"> >>" + newline;
            // mainly testing to ensure we don't get parse errors of above
            StringTemplateGroup group =
                    new StringTemplateGroup(
                            new StringReader( templates ) );
            StringTemplate t = group.GetInstanceOf( "a" );
            t.SetAttribute( "s", "Test" );
            string expecting = "case 1 : Test break;";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestAngleBracketsNoGroup()
        {
            StringTemplate st = new StringTemplate(
                    "Tokens : <rules; separator=\"|\"> ;",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "rules", "A" );
            st.SetAttribute( "rules", "B" );
            string expecting = "Tokens : A|B ;";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestRegionRef()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X$@r()$Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmbeddedRegionRef()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X$@r$blort$@end$Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XblortY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionRefAngleBrackets()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r()>Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmbeddedRegionRefAngleBrackets()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r>blort<@end>Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XblortY";
            Assert.AreEqual( expecting, result );
        }

        // FIXME: This test fails due to inserted white space...
        [TestMethod]
        public void TestEmbeddedRegionRefWithNewlinesAngleBrackets()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r>" + newline +
                    "blort" + newline +
                    "<@end>" + newline +
                    "Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XblortY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionRefWithDefAngleBrackets()
        {
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r()>Y\"" + newline +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XfooY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionRefWithDefInConditional()
        {
            string templates =
                    "group test;" + newline +
                    "a(v) ::= \"X<if(v)>A<@r()>B<endif>Y\"" + newline +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.SetAttribute( "v", "true" );
            string result = st.ToString();
            string expecting = "XAfooBY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionRefWithImplicitDefInConditional()
        {
            string templates =
                    "group test;" + newline +
                    "a(v) ::= \"X<if(v)>A<@r>yo<@end>B<endif>Y\"" + newline +
                    "@a.r() ::= \"foo\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.SetAttribute( "v", "true" );
            string result = st.ToString();
            string expecting = "XAyoBY";
            Assert.AreEqual( expecting, result );

            string err_result = errors.ToString();
            string err_expecting = "group test line 3: redefinition of template region: @a.r";
            Assert.AreEqual( err_expecting, err_result );
        }

        [TestMethod]
        public void TestRegionOverride()
        {
            string templates1 =
                    "group super;" + newline +
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                    "group sub;" + newline +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            group );

            StringTemplate st = subGroup.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XfooY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionOverrideRefSuperRegion()
        {
            string templates1 =
                    "group super;" + newline +
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                    "group sub;" + newline +
                    "@a.r() ::= \"A<@super.r()>B\"" + newline;
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            group );

            StringTemplate st = subGroup.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XAfooBY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionOverrideRefSuperRegion3Levels()
        {
            // Bug: This was causing infinite recursion:
            // getInstanceOf(super::a)
            // getInstanceOf(sub::a)
            // getInstanceOf(subsub::a)
            // getInstanceOf(subsub::region__a__r)
            // getInstanceOf(subsub::super.region__a__r)
            // getInstanceOf(subsub::super.region__a__r)
            // getInstanceOf(subsub::super.region__a__r)
            // ...
            // Somehow, the ref to super in subsub is not moving up the chain
            // to the @super.r(); oh, i introduced a bug when i put setGroup
            // into STG.GetInstanceOf()!

            string templates1 =
                    "group super;" + newline +
                    "a() ::= \"X<@r()>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                    "group sub;" + newline +
                    "@a.r() ::= \"<@super.r()>2\"" + newline;
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            group );

            string templates3 =
                    "group subsub;" + newline +
                    "@a.r() ::= \"<@super.r()>3\"" + newline;
            StringTemplateGroup subSubGroup =
                    new StringTemplateGroup( new StringReader( templates3 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            subGroup );

            StringTemplate st = subSubGroup.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "Xfoo23Y";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRegionOverrideRefSuperImplicitRegion()
        {
            string templates1 =
                    "group super;" + newline +
                    "a() ::= \"X<@r>foo<@end>Y\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                    "group sub;" + newline +
                    "@a.r() ::= \"A<@super.r()>\"" + newline;
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            group );

            StringTemplate st = subGroup.GetInstanceOf( "a" );
            string result = st.ToString();
            string expecting = "XAfooY";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmbeddedRegionRedefError()
        {
            // cannot define an embedded template within group
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r>dork<@end>Y\"" +
                    "@a.r() ::= \"foo\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.ToString();
            string result = errors.ToString();
            string expecting = "group test line 2: redefinition of template region: @a.r";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestImplicitRegionRedefError()
        {
            // cannot define an implicitly-defined template more than once
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r()>Y\"" + newline +
                    "@a.r() ::= \"foo\"" + newline +
                    "@a.r() ::= \"bar\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.ToString();
            string result = errors.ToString();
            string expecting = "group test line 4: redefinition of template region: @a.r";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestImplicitOverriddenRegionRedefError()
        {
            string templates1 =
                "group super;" + newline +
                "a() ::= \"X<@r()>Y\"" +
                "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                "group sub;" + newline +
                "@a.r() ::= \"foo\"" + newline +
                "@a.r() ::= \"bar\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            errors,
                                            group );

            StringTemplate st = subGroup.GetInstanceOf( "a" );
            string result = errors.ToString();
            string expecting = "group sub line 3: redefinition of template region: @a.r";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestUnknownRegionDefError()
        {
            // cannot define an implicitly-defined template more than once
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r()>Y\"" + newline +
                    "@a.q() ::= \"foo\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.ToString();
            string result = errors.ToString();
            string expecting = "group test line 3: template a has no region called q";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSuperRegionRefError()
        {
            string templates1 =
                "group super;" + newline +
                "a() ::= \"X<@r()>Y\"" +
                "@a.r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                "group sub;" + newline +
                "@a.r() ::= \"A<@super.q()>B\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            errors,
                                            group );

            StringTemplate st = subGroup.GetInstanceOf( "a" );
            string result = errors.ToString();
            string expecting = "template a has no region called q";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMissingEndRegionError()
        {
            // cannot define an implicitly-defined template more than once
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X$@r$foo\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors,
                                            null );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.ToString();
            string result = errors.ToString();
            string expecting = "missing region r $@end$ tag";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMissingEndRegionErrorAngleBrackets()
        {
            // cannot define an implicitly-defined template more than once
            string templates =
                    "group test;" + newline +
                    "a() ::= \"X<@r>foo\"" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            errors );
            StringTemplate st = group.GetInstanceOf( "a" );
            st.ToString();
            string result = errors.ToString();
            string expecting = "missing region r <@end> tag";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSimpleInheritance()
        {
            // make a bold template in the super group that you can inherit from sub
            StringTemplateGroup supergroup = new StringTemplateGroup( "super" );
            StringTemplateGroup subgroup = new StringTemplateGroup( "sub" );
            StringTemplate bold = supergroup.DefineTemplate( "bold", "<b>$it$</b>" );
            subgroup.SuperGroup = supergroup;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            subgroup.ErrorListener = errors;
            supergroup.ErrorListener = errors;
            StringTemplate duh = new StringTemplate( subgroup, "$name:bold()$" );
            duh.SetAttribute( "name", "Terence" );
            string expecting = "<b>Terence</b>";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        [TestMethod]
        public void TestOverrideInheritance()
        {
            // make a bold template in the super group and one in sub group
            StringTemplateGroup supergroup = new StringTemplateGroup( "super" );
            StringTemplateGroup subgroup = new StringTemplateGroup( "sub" );
            supergroup.DefineTemplate( "bold", "<b>$it$</b>" );
            subgroup.DefineTemplate( "bold", "<strong>$it$</strong>" );
            subgroup.SuperGroup = supergroup;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            subgroup.ErrorListener = errors;
            supergroup.ErrorListener = errors;
            StringTemplate duh = new StringTemplate( subgroup, "$name:bold()$" );
            duh.SetAttribute( "name", "Terence" );
            string expecting = "<strong>Terence</strong>";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        [TestMethod]
        public void TestMultiLevelInheritance()
        {
            // must loop up two levels to find bold()
            StringTemplateGroup rootgroup = new StringTemplateGroup( "root" );
            StringTemplateGroup level1 = new StringTemplateGroup( "level1" );
            StringTemplateGroup level2 = new StringTemplateGroup( "level2" );
            rootgroup.DefineTemplate( "bold", "<b>$it$</b>" );
            level1.SuperGroup = rootgroup;
            level2.SuperGroup = level1;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            rootgroup.ErrorListener = errors;
            level1.ErrorListener = errors;
            level2.ErrorListener = errors;
            StringTemplate duh = new StringTemplate( level2, "$name:bold()$" );
            duh.SetAttribute( "name", "Terence" );
            string expecting = "<b>Terence</b>";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        [TestMethod]
        public void TestComplicatedInheritance()
        {
            // in super: decls invokes labels
            // in sub:   overridden decls which calls super.decls
            //           overridden labels
            // Bug: didn't see the overridden labels.  In other words,
            // the overridden decls called super which called labels, but
            // didn't get the subgroup overridden labels--it calls the
            // one in the superclass.  Ouput was "DL" not "DSL"; didn't
            // invoke sub's labels().
            string basetemplates =
                "group base;" + newline +
                "decls() ::= \"D<labels()>\"" + newline +
                "labels() ::= \"L\"" + newline
                ;
            StringTemplateGroup @base =
                new StringTemplateGroup( new StringReader( basetemplates ) );
            string subtemplates =
                "group sub;" + newline +
                "decls() ::= \"<super.decls()>\"" + newline +
                "labels() ::= \"SL\"" + newline
                ;
            StringTemplateGroup sub =
                    new StringTemplateGroup( new StringReader( subtemplates ) );
            sub.SuperGroup = @base;
            StringTemplate st = sub.GetInstanceOf( "decls" );
            string expecting = "DSL";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void Test3LevelSuperRef()
        {
            string templates1 =
                    "group super;" + newline +
                    "r() ::= \"foo\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates1 ) );

            string templates2 =
                    "group sub;" + newline +
                    "r() ::= \"<super.r()>2\"" + newline;
            StringTemplateGroup subGroup =
                    new StringTemplateGroup( new StringReader( templates2 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            group );

            string templates3 =
                    "group subsub;" + newline +
                    "r() ::= \"<super.r()>3\"" + newline;
            StringTemplateGroup subSubGroup =
                    new StringTemplateGroup( new StringReader( templates3 ),
                                            typeof( AngleBracketTemplateLexer ),
                                            null,
                                            subGroup );

            StringTemplate st = subSubGroup.GetInstanceOf( "r" );
            string result = st.ToString();
            string expecting = "foo23";
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestExprInParens()
        {
            // specify a template to apply to an attribute
            // Use a template group so we can specify the start/stop chars
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", "." );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate duh = new StringTemplate( group, "$(\"blort: \"+(list)):bold()$" );
            duh.SetAttribute( "list", "a" );
            duh.SetAttribute( "list", "b" );
            duh.SetAttribute( "list", "c" );
            // System.out.println(duh);
            string expecting = "<b>blort: abc</b>";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        [TestMethod]
        public void TestMultipleAdditions()
        {
            // specify a template to apply to an attribute
            // Use a template group so we can specify the start/stop chars
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", "." );
            group.DefineTemplate( "link", "<a href=\"$url$\"><b>$title$</b></a>" );
            StringTemplate duh =
                new StringTemplate( group,
                    "$link(url=\"/member/view?ID=\"+ID+\"&x=y\"+foo, title=\"the title\")$" );
            duh.SetAttribute( "ID", "3321" );
            duh.SetAttribute( "foo", "fubar" );
            string expecting = "<a href=\"/member/view?ID=3321&x=yfubar\"><b>the title</b></a>";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        [TestMethod]
        public void TestCollectionAttributes()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate t =
                new StringTemplate( group, "$data$, $data:bold()$, " +
                                          "$list:bold():bold()$, $array$, $a2$, $a3$, $a4$" );
            List<object> v = new List<object>();
            v.Add( "1" );
            v.Add( "2" );
            v.Add( "3" );
            IList list = new List<object>();
            list.Add( "a" );
            list.Add( "b" );
            list.Add( "c" );
            t.SetAttribute( "data", v );
            t.SetAttribute( "list", list );
            t.SetAttribute( "array", new string[] { "x", "y" } );
            t.SetAttribute( "a2", new int[] { 10, 20 } );
            t.SetAttribute( "a3", new float[] { 1.2f, 1.3f } );
            t.SetAttribute( "a4", new double[] { 8.7, 9.2 } );
            //System.out.println(t);
            string expecting = "123, <b>1</b><b>2</b><b>3</b>, " +
                "<b><b>a</b></b><b><b>b</b></b><b><b>c</b></b>, xy, 1020, 1.21.3, 8.79.2";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestParenthesizedExpression()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate t = new StringTemplate( group, "$(f+l):bold()$" );
            t.SetAttribute( "f", "Joe" );
            t.SetAttribute( "l", "Schmoe" );
            //System.out.println(t);
            string expecting = "<b>JoeSchmoe</b>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestApplyTemplateNameExpression()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "foobar", "foo$attr$bar" );
            StringTemplate t = new StringTemplate( group, "$data:(name+\"bar\")()$" );
            t.SetAttribute( "data", "Ter" );
            t.SetAttribute( "data", "Tom" );
            t.SetAttribute( "name", "foo" );
            //System.out.println(t);
            string expecting = "fooTerbarfooTombar";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestApplyTemplateNameTemplateEval()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate foobar = group.DefineTemplate( "foobar", "foo$it$bar" );
            StringTemplate a = group.DefineTemplate( "a", "$it$bar" );
            StringTemplate t = new StringTemplate( group, "$data:(\"foo\":a())()$" );
            t.SetAttribute( "data", "Ter" );
            t.SetAttribute( "data", "Tom" );
            //System.out.println(t);
            string expecting = "fooTerbarfooTombar";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestTemplateNameExpression()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate foo = group.DefineTemplate( "foo", "hi there!" );
            StringTemplate t = new StringTemplate( group, "$(name)()$" );
            t.SetAttribute( "name", "foo" );
            //System.out.println(t);
            string expecting = "hi there!";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestMissingEndDelimiter()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group, "stuff $a then more junk etc..." );
            //string expectingError = "problem parsing template 'anonymous': line 1:31: expecting '$', found '<EOF>'";
            string expectingError = "$...$ chunk lexer error: line 1:31: mismatched character '<EOF>' expecting '$':";
            //System.out.println("error: '"+errors+"'");
            //System.out.println("expecting: '"+expectingError+"'");
            Assert.IsTrue( errors.ToString().StartsWith( expectingError ) );
        }

        [TestMethod]
        public void TestSetButNotRefd()
        {
            StringTemplate.LintMode = true;
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate t = new StringTemplate( group, "$a$ then $b$ and $c$ refs." );
            t.SetAttribute( "a", "Terence" );
            t.SetAttribute( "b", "Terence" );
            t.SetAttribute( "cc", "Terence" ); // oops...should be 'c'
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            string expectingError = "anonymous: set but not used: cc";
            string result = t.ToString();    // result is irrelevant
            //System.out.println("result error: '"+errors+"'");
            //System.out.println("expecting: '"+expectingError+"'");
            StringTemplate.LintMode = false;
            Assert.AreEqual( expectingError, errors.ToString() );
        }

        [TestMethod]
        public void TestNullTemplateApplication()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group, "$names:bold(x=it)$" );
            t.SetAttribute( "names", "Terence" );

            string error = null;
            try
            {
                t.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expecting = "Can't find template bold.st; context is [anonymous]; group hierarchy is [test]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestNullTemplateToMultiValuedApplication()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group, "$names:bold(x=it)$" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "Tom" );
            //System.out.println(t);
            string error = null;
            try
            {
                t.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expecting = "Can't find template bold.st; context is [anonymous]; group hierarchy is [test]"; // bold not found...empty string
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestChangingAttrValueTemplateApplicationToVector()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate t = new StringTemplate( group, "$names:bold(x=it)$" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "Tom" );
            //System.out.println("'"+t.toString()+"'");
            string expecting = "<b>Terence</b><b>Tom</b>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestChangingAttrValueRepeatedTemplateApplicationToVector()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$item$</b>" );
            StringTemplate italics = group.DefineTemplate( "italics", "<i>$it$</i>" );
            StringTemplate members = new StringTemplate( group, "$members:bold(item=it):italics(it=it)$" );
            members.SetAttribute( "members", "Jim" );
            members.SetAttribute( "members", "Mike" );
            members.SetAttribute( "members", "Ashar" );
            //System.out.println("members="+members);
            string expecting = "<i><b>Jim</b></i><i><b>Mike</b></i><i><b>Ashar</b></i>";
            Assert.AreEqual( expecting, members.ToString() );
        }

        [TestMethod]
        public void TestAlternatingTemplateApplication()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate listItem = group.DefineTemplate( "listItem", "<li>$it$</li>" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate italics = group.DefineTemplate( "italics", "<i>$it$</i>" );
            StringTemplate item = new StringTemplate( group, "$item:bold(),italics():listItem()$" );
            item.SetAttribute( "item", "Jim" );
            item.SetAttribute( "item", "Mike" );
            item.SetAttribute( "item", "Ashar" );
            //System.out.println("ITEM="+item);
            string expecting = "<li><b>Jim</b></li><li><i>Mike</i></li><li><b>Ashar</b></li>";
            Assert.AreEqual( expecting, item.ToString() );
        }

        [TestMethod]
        public void TestExpressionAsRHSOfAssignment()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate hostname = group.DefineTemplate( "hostname", "$machine$.jguru.com" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate t = new StringTemplate( group, "$bold(x=hostname(machine=\"www\"))$" );
            string expecting = "<b>www.jguru.com</b>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestTemplateApplicationAsRHSOfAssignment()
        {
            StringTemplateGroup group = new StringTemplateGroup( "test" );
            StringTemplate hostname = group.DefineTemplate( "hostname", "$machine$.jguru.com" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate italics = group.DefineTemplate( "italics", "<i>$it$</i>" );
            StringTemplate t = new StringTemplate( group, "$bold(x=hostname(machine=\"www\"):italics())$" );
            string expecting = "<b><i>www.jguru.com</i></b>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestParameterAndAttributeScoping()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate italics = group.DefineTemplate( "italics", "<i>$x$</i>" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate t = new StringTemplate( group, "$bold(x=italics(x=name))$" );
            t.SetAttribute( "name", "Terence" );
            //System.out.println(t);
            string expecting = "<b><i>Terence</i></b>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestComplicatedSeparatorExpr()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bulletSeparator", "</li>$foo$<li>" );
            // make separator a complicated expression with args passed to included template
            StringTemplate t =
                new StringTemplate( group,
                                   "<ul>$name; separator=bulletSeparator(foo=\" \")+\"&nbsp;\"$</ul>" );
            t.SetAttribute( "name", "Ter" );
            t.SetAttribute( "name", "Tom" );
            t.SetAttribute( "name", "Mel" );
            //System.out.println(t);
            string expecting = "<ul>Ter</li> <li>&nbsp;Tom</li> <li>&nbsp;Mel</ul>";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestAttributeRefButtedUpAgainstEndifAndWhitespace()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate a = new StringTemplate( group,
                                                  "$if (!firstName)$$email$$endif$" );
            a.SetAttribute( "email", "parrt@jguru.com" );
            string expecting = "parrt@jguru.com";
            Assert.AreEqual( a.ToString(), expecting );
        }

        [TestMethod]
        public void TestStringCatenationOnSingleValuedAttributeViaTemplateLiteral()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            //StringTemplate a = new StringTemplate(group, "$\" Parr\":bold()$");
            StringTemplate b = new StringTemplate( group, "$bold(it={$name$ Parr})$" );
            //a.SetAttribute("name", "Terence");
            b.SetAttribute( "name", "Terence" );
            string expecting = "<b>Terence Parr</b>";
            //assertEquals(a.toString(), expecting);
            Assert.AreEqual( b.ToString(), expecting );
        }

        [TestMethod]
        public void TestStringCatenationOpOnArg()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate b = new StringTemplate( group, "$bold(it=name+\" Parr\")$" );
            //a.SetAttribute("name", "Terence");
            b.SetAttribute( "name", "Terence" );
            string expecting = "<b>Terence Parr</b>";
            //assertEquals(expecting, a.toString());
            Assert.AreEqual( expecting, b.ToString() );
        }

        [TestMethod]
        public void TestStringCatenationOpOnArgWithEqualsInString()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate b = new StringTemplate( group, "$bold(it=name+\" Parr=\")$" );
            //a.SetAttribute("name", "Terence");
            b.SetAttribute( "name", "Terence" );
            string expecting = "<b>Terence Parr=</b>";
            //assertEquals(expecting, a.toString());
            Assert.AreEqual( expecting, b.ToString() );
        }

        [TestMethod]
        public void TestApplyingTemplateFromDiskWithPrecompiledIF()
        {
            // Create a temporary working directory
            string tmpdir = Path.GetTempPath();
            string tmpWorkDir;
            int counter = ( new Random() ).Next() & 65535;
            do
            {
                counter++;
                StringBuilder name = new StringBuilder( "st-junit-" );
                name.Append( counter );
                tmpWorkDir = System.IO.Path.Combine( tmpdir, name.ToString() );
            } while ( System.IO.Directory.Exists( tmpWorkDir ) );
            System.IO.Directory.CreateDirectory( tmpWorkDir );

            try
            {
                // write the template files first to /tmp
                string pageFile = System.IO.Path.Combine( tmpWorkDir, "page.st" );
                try
                {
                    StreamWriter fw = new StreamWriter( pageFile );
                    fw.Write( "<html><head>" + newline );
                    //fw.write("  <title>PeerScope: $title$</title>"+newline);
                    fw.Write( "</head>" + newline );
                    fw.Write( "<body>" + newline );
                    fw.Write( "$if(member)$User: $member:terse()$$endif$" + newline );
                    fw.Write( "</body>" + newline );
                    fw.Write( "</head>" + newline );
                    fw.Close();

                    string terseFile = System.IO.Path.Combine( tmpWorkDir, "terse.st" );
                    try
                    {
                        fw = new StreamWriter( terseFile );
                        fw.Write( "$it.firstName$ $it.lastName$ (<tt>$it.email$</tt>)" + newline );
                        fw.Close();
                        // specify a template to apply to an attribute
                        // Use a template group so we can specify the start/stop chars
                        StringTemplateGroup group =
                                new StringTemplateGroup( "dummy", tmpWorkDir.ToString() );

                        StringTemplate a = group.GetInstanceOf( "page" );
                        a.SetAttribute( "member", new Connector() );
                        string expecting = "<html><head>" + newline +
                                "</head>" + newline +
                                "<body>" + newline +
                                "User: Terence Parr (<tt>parrt@jguru.com</tt>)" + newline +
                                "</body>" + newline +
                                "</head>";
                        //System.out.println("'"+a+"'");
                        Assert.AreEqual( expecting, a.ToString() );

                        // Cleanup the temp folder.
                    }
                    finally
                    {
                        System.IO.File.Delete( terseFile );
                    }
                }
                finally
                {
                    System.IO.File.Delete( pageFile );
                }
            }
            finally
            {
                System.IO.Directory.Delete( tmpWorkDir );
            }
        }

        [TestMethod]
        public void TestMultiValuedAttributeWithAnonymousTemplateUsingIndexVariableI()
        {
            StringTemplateGroup tgroup =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate( tgroup,
                                       " List:" + newline + "  " + newline + "foo" + newline + newline +
                                       "$names:{<br>$i$. $it$" + newline +
                                       "}$" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "Jim" );
            t.SetAttribute( "names", "Sriram" );
            //System.out.println(t);
            string expecting =
                    " List:" + newline +
                    "  " + newline +
                    "foo" + newline + newline +
                    "<br>1. Terence" + newline +
                    "<br>2. Jim" + newline +
                    "<br>3. Sriram" + newline;
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestFindTemplateInCLASSPATH()
        {
            // Look for templates in CLASSPATH as resources
            StringTemplateGroup mgroup =
                    new StringTemplateGroup( "method stuff",
                                            null,
                                            typeof( AngleBracketTemplateLexer ),
                                            System.Reflection.Assembly.GetExecutingAssembly() );
            StringTemplate m = mgroup.GetInstanceOf( "AntlrUnitTests/method" );
            // "method.st" references body() so "body.st" will be loaded too
            m.SetAttribute( "visibility", "public" );
            m.SetAttribute( "name", "foobar" );
            m.SetAttribute( "returnType", "void" );
            m.SetAttribute( "statements", "i=1;" ); // body inherits these from method
            m.SetAttribute( "statements", "x=i;" );
            string expecting =
                    "public void foobar() {" + newline +
                    "\t// start of a body" + newline +
                    "\ti=1;" + newline +
                    "\tx=i;" + newline +
                    "\t// end of a body" + newline +
                    "}";
            //System.out.println(m);
            Assert.AreEqual( expecting, m.ToString() );
        }

        [TestMethod]
        public void TestApplyTemplateToSingleValuedAttribute()
        {
            StringTemplateGroup group = new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate name = new StringTemplate( group, "$name:bold(x=name)$" );
            name.SetAttribute( "name", "Terence" );
            Assert.AreEqual( "<b>Terence</b>", name.ToString() );
        }

        [TestMethod]
        public void TestStringLiteralAsAttribute()
        {
            StringTemplateGroup group = new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate name = new StringTemplate( group, "$\"Terence\":bold()$" );
            Assert.AreEqual( "<b>Terence</b>", name.ToString() );
        }

        [TestMethod]
        public void TestApplyTemplateToSingleValuedAttributeWithDefaultAttribute()
        {
            StringTemplateGroup group = new StringTemplateGroup( "test" );
            StringTemplate bold = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate name = new StringTemplate( group, "$name:bold()$" );
            name.SetAttribute( "name", "Terence" );
            Assert.AreEqual( "<b>Terence</b>", name.ToString() );
        }

        [TestMethod]
        public void TestApplyAnonymousTemplateToSingleValuedAttribute()
        {
            // specify a template to apply to an attribute
            // Use a template group so we can specify the start/stop chars
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate item = new StringTemplate( group, "$item:{<li>$it$</li>}$" );
            item.SetAttribute( "item", "Terence" );
            Assert.AreEqual( "<li>Terence</li>", item.ToString() );
        }

        [TestMethod]
        public void TestApplyAnonymousTemplateToMultiValuedAttribute()
        {
            // specify a template to apply to an attribute
            // Use a template group so we can specify the start/stop chars
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate list = new StringTemplate( group, "<ul>$items$</ul>" );
            // demonstrate setting arg to anonymous subtemplate
            StringTemplate item = new StringTemplate( group, "$item:{<li>$it$</li>}; separator=\",\"$" );
            item.SetAttribute( "item", "Terence" );
            item.SetAttribute( "item", "Jim" );
            item.SetAttribute( "item", "John" );
            list.SetAttribute( "items", item ); // nested template
            string expecting = "<ul><li>Terence</li>,<li>Jim</li>,<li>John</li></ul>";
            Assert.AreEqual( expecting, list.ToString() );
        }

        [TestMethod]
        public void TestApplyAnonymousTemplateToAggregateAttribute()
        {
            StringTemplate st = new StringTemplate( "$items:{$it.lastName$, $it.firstName$\n}$" );
            // also testing wacky spaces in aggregate spec
            st.SetAttribute( "items.{ firstName ,lastName}", "Ter", "Parr" );
            st.SetAttribute( "items.{firstName, lastName }", "Tom", "Burns" );
            string expecting =
                    "Parr, Ter" + newline +
                    "Burns, Tom" + newline;
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestRepeatedApplicationOfTemplateToSingleValuedAttribute()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate search = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate item = new StringTemplate( group, "$item:bold():bold()$" );
            item.SetAttribute( "item", "Jim" );
            Assert.AreEqual( "<b><b>Jim</b></b>", item.ToString() );
        }

        [TestMethod]
        public void TestRepeatedApplicationOfTemplateToMultiValuedAttributeWithSeparator()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate search = group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate item = new StringTemplate( group, "$item:bold():bold(); separator=\",\"$" );
            item.SetAttribute( "item", "Jim" );
            item.SetAttribute( "item", "Mike" );
            item.SetAttribute( "item", "Ashar" );
            // first application of template must yield another vector!
            //System.out.println("ITEM="+item);
            string expecting = "<b><b>Jim</b></b>,<b><b>Mike</b></b>,<b><b>Ashar</b></b>";
            Assert.AreEqual( expecting, item.ToString() );
        }

        // ### NEED A TEST OF obj ASSIGNED TO ARG?

        [TestMethod]
        public void TestMultiValuedAttributeWithSeparator()
        {
            StringTemplate query;

            // if column can be multi-valued, specify a separator
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            query = new StringTemplate( group, "SELECT <distinct> <column; separator=\", \"> FROM <table>;" );
            query.SetAttribute( "column", "name" );
            query.SetAttribute( "column", "email" );
            query.SetAttribute( "table", "User" );
            // uncomment next line to make "DISTINCT" appear in output
            // query.SetAttribute("distince", "DISTINCT");
            // System.out.println(query);
            Assert.AreEqual( "SELECT  name, email FROM User;", query.ToString() );
        }

        [TestMethod]
        public void TestSingleValuedAttributes()
        {
            // all attributes are single-valued:
            StringTemplate query = new StringTemplate( "SELECT $column$ FROM $table$;" );
            query.SetAttribute( "column", "name" );
            query.SetAttribute( "table", "User" );
            Assert.AreEqual( "SELECT name FROM User;", query.ToString() );
        }

        [TestMethod]
        public void TestIFTemplate()
        {
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                new StringTemplate( group,
                          "SELECT <column> FROM PERSON " +
                          "<if(cond)>WHERE ID=<id><endif>;" );
            t.SetAttribute( "column", "name" );
            t.SetAttribute( "cond", "true" );
            t.SetAttribute( "id", "231" );
            Assert.AreEqual( "SELECT name FROM PERSON WHERE ID=231;", t.ToString() );
        }

        [TestMethod]
        public void TestIFCondWithParensTemplate()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t = new StringTemplate( group, "<if(map.(type))><type> <prop>=<map.(type)>;<endif>" );
            Dictionary<object, object> map = new Dictionary<object, object>();
            map["int"] = "0";
            t.SetAttribute( "map", map );
            t.SetAttribute( "prop", "x" );
            t.SetAttribute( "type", "int" );
            Assert.AreEqual( "int x=0;", t.ToString() );
        }

        [TestMethod]
        public void TestIFCondWithParensDollarDelimsTemplate()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate t = new StringTemplate( group, "$if(map.(type))$$type$ $prop$=$map.(type)$;$endif$" );
            Dictionary<object, object> map = new Dictionary<object, object>();
            map["int"] = "0";
            t.SetAttribute( "map", map );
            t.SetAttribute( "prop", "x" );
            t.SetAttribute( "type", "int" );
            Assert.AreEqual( "int x=0;", t.ToString() );
        }

        /** As of 2.0, you can test a boolean value */
        [TestMethod]
        public void TestIFBoolean()
        {
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                new StringTemplate( group,
                          "$if(b)$x$endif$ $if(!b)$y$endif$" );
            t.SetAttribute( "b", true );
            Assert.AreEqual( "x ", t.ToString() );

            t = t.GetInstanceOf();
            t.SetAttribute( "b", false );
            Assert.AreEqual( " y", t.ToString() );
        }

        [TestMethod]
        public void TestNestedIFTemplate()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                new StringTemplate( group,
                    "ack<if(a)>" + newline +
                    "foo" + newline +
                    "<if(!b)>stuff<endif>" + newline +
                    "<if(b)>no<endif>" + newline +
                    "junk" + newline +
                    "<endif>"
                );
            t.SetAttribute( "a", "blort" );
            // leave b as null
            //System.out.println("t="+t);
            string expecting =
                    "ackfoo" + newline +
                    "stuff" + newline +
                    "junk";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIFConditionWithTemplateApplication()
        {
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                new StringTemplate( group,
                          "$if(names:{$it$})$Fail!$endif$ $if(!names:{$it$})$Works!$endif$" );
            t.SetAttribute( "b", true );
            Assert.AreEqual( t.ToString(), " Works!" );
        }

        public class Connector
        {
            public int getID()
            {
                return 1;
            }
            public string getFirstName()
            {
                return "Terence";
            }
            public string getLastName()
            {
                return "Parr";
            }
            public string getEmail()
            {
                return "parrt@jguru.com";
            }
            public string getBio()
            {
                return "Superhero by night...";
            }
            /** As of 2.0, booleans work as you expect.  In 1.x,
             *  a missing value simulated a boolean.
             */
            public bool getCanEdit()
            {
                return false;
            }
        }

        public class Connector2
        {
            public int getID()
            {
                return 2;
            }
            public string getFirstName()
            {
                return "Tom";
            }
            public string getLastName()
            {
                return "Burns";
            }
            public string getEmail()
            {
                return "tombu@jguru.com";
            }
            public string getBio()
            {
                return "Superhero by day...";
            }
            public Boolean getCanEdit()
            {
                return true;
            }
        }

        [TestMethod]
        public void TestObjectPropertyReference()
        {
            //assertEquals(expecting, t.toString());
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "<b>Name: $p.firstName$ $p.lastName$</b><br>" + newline +
                            "<b>Email: $p.email$</b><br>" + newline +
                            "$p.bio$"
                    );
            t.SetAttribute( "p", new Connector() );
            //System.out.println("t is "+t.toString());
            string expecting =
                    "<b>Name: Terence Parr</b><br>" + newline +
                    "<b>Email: parrt@jguru.com</b><br>" + newline +
                    "Superhero by night...";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestApplyRepeatedAnonymousTemplateWithForeignTemplateRefToMultiValuedAttribute()
        {
            // specify a template to apply to an attribute
            // Use a template group so we can specify the start/stop chars
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", "." );
            group.DefineTemplate( "link", "<a href=\"$url$\"><b>$title$</b></a>" );
            StringTemplate duh =
                new StringTemplate( group,
            "start|$p:{$link(url=\"/member/view?ID=\"+it.ID, title=it.firstName)$ $if(it.canEdit)$canEdit$endif$}:" +
            "{$it$<br>\n}$|end" );
            duh.SetAttribute( "p", new Connector() );
            duh.SetAttribute( "p", new Connector2() );
            //System.out.println(duh);
            string expecting = "start|<a href=\"/member/view?ID=1\"><b>Terence</b></a> <br>" + newline +
                "<a href=\"/member/view?ID=2\"><b>Tom</b></a> canEdit<br>" + newline +
                "|end";
            Assert.AreEqual( expecting, duh.ToString() );
        }

        public class Tree
        {
            protected IList children = new List<object>();
            protected string text;
            public Tree( string t )
            {
                text = t;
            }
            public virtual string getText()
            {
                return text;
            }
            public virtual void addChild( Tree c )
            {
                children.Add( c );
            }
            public virtual Tree getFirstChild()
            {
                if ( children.Count == 0 )
                {
                    return null;
                }
                return (Tree)children[0];
            }
            public virtual IList getChildren()
            {
                return children;
            }
        }

        [TestMethod]
        public void TestRecursion()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            group.DefineTemplate( "tree",
            "<if(it.firstChild)>" +
              "( <it.text> <it.children:tree(); separator=\" \"> )" +
            "<else>" +
              "<it.text>" +
            "<endif>" );
            StringTemplate tree = group.GetInstanceOf( "tree" );
            // build ( a b (c d) e )
            Tree root = new Tree( "a" );
            root.addChild( new Tree( "b" ) );
            Tree subtree = new Tree( "c" );
            subtree.addChild( new Tree( "d" ) );
            root.addChild( subtree );
            root.addChild( new Tree( "e" ) );
            tree.SetAttribute( "it", root );
            string expecting = "( a b ( c d ) e )";
            Assert.AreEqual( expecting, tree.ToString() );
        }

        [TestMethod]
        public void TestNestedAnonymousTemplates()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A:{" + newline +
                              "<i>$it:{" + newline +
                                "<b>$it$</b>" + newline +
                              "}$</i>" + newline +
                            "}$"
                    );
            t.SetAttribute( "A", "parrt" );
            string expecting = newline +
                "<i>" + newline +
                "<b>parrt</b>" + newline +
                "</i>" + newline;
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestAnonymousTemplateAccessToEnclosingAttributes()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A:{" + newline +
                              "<i>$it:{" + newline +
                                "<b>$it$, $B$</b>" + newline +
                              "}$</i>" + newline +
                            "}$"
                    );
            t.SetAttribute( "A", "parrt" );
            t.SetAttribute( "B", "tombu" );
            string expecting = newline +
                "<i>" + newline +
                "<b>parrt, tombu</b>" + newline +
                "</i>" + newline;
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNestedAnonymousTemplatesAgain()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "<table>" + newline +
                            "$names:{<tr>$it:{<td>$it:{<b>$it$</b>}$</td>}$</tr>}$" + newline +
                            "</table>" + newline
                    );
            t.SetAttribute( "names", "parrt" );
            t.SetAttribute( "names", "tombu" );
            string expecting =
                    "<table>" + newline +
                    "<tr><td><b>parrt</b></td></tr><tr><td><b>tombu</b></td></tr>" + newline +
                    "</table>" + newline;
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestEscapes()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            group.DefineTemplate( "foo", "$x$ && $it$" );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A:foo(x=\"dog\\\"\\\"\")$" // $A:foo("dog\"\"")$
                    );
            StringTemplate u =
                    new StringTemplate(
                            group,
                            "$A:foo(x=\"dog\\\"g\")$" // $A:foo(x="dog\"g")$
                    );
            StringTemplate v =
                    new StringTemplate(
                            group,
                // $A:{$attr:foo(x="\{dog\}\"")$ is cool}$
                            "$A:{$it:foo(x=\"\\{dog\\}\\\"\")$ is cool}$"
                    );
            t.SetAttribute( "A", "ick" );
            u.SetAttribute( "A", "ick" );
            v.SetAttribute( "A", "ick" );
            //System.out.println("t is '"+t.toString()+"'");
            //System.out.println("u is '"+u.toString()+"'");
            //System.out.println("v is '"+v.toString()+"'");
            string expecting = "dog\"\" && ick";
            Assert.AreEqual( expecting, t.ToString() );
            expecting = "dog\"g && ick";
            Assert.AreEqual( expecting, u.ToString() );
            expecting = "{dog}\" && ick is cool";
            Assert.AreEqual( expecting, v.ToString() );
        }

        [TestMethod]
        public void TestEscapesOutsideExpressions()
        {
            StringTemplate b = new StringTemplate( "It\\'s ok...\\$; $a:{\\'hi\\', $it$}$" );
            b.SetAttribute( "a", "Ter" );
            string expecting = "It\\'s ok...$; \\'hi\\', Ter";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestElseClause()
        {
            StringTemplate e = new StringTemplate(
                    "$if(title)$" + newline +
                    "foo" + newline +
                    "$else$" + newline +
                    "bar" + newline +
                    "$endif$"
                );
            e.SetAttribute( "title", "sample" );
            string expecting = "foo";
            Assert.AreEqual( expecting, e.ToString() );

            e = e.GetInstanceOf();
            expecting = "bar";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestElseIfClause()
        {
            StringTemplate e = new StringTemplate(
                    "$if(x)$" + newline +
                    "foo" + newline +
                    "$elseif(y)$" + newline +
                    "bar" + newline +
                    "$endif$"
                );
            e.SetAttribute( "y", "yep" );
            string expecting = "bar";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestElseIfClauseAngleBrackets()
        {
            StringTemplate e = new StringTemplate(
                    "<if(x)>" + newline +
                    "foo" + newline +
                    "<elseif(y)>" + newline +
                    "bar" + newline +
                    "<endif>",
                    typeof( AngleBracketTemplateLexer )
                );
            e.SetAttribute( "y", "yep" );
            string expecting = "bar";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestElseIfClause2()
        {
            StringTemplate e = new StringTemplate(
                    "$if(x)$" + newline +
                    "foo" + newline +
                    "$elseif(y)$" + newline +
                    "bar" + newline +
                    "$elseif(z)$" + newline +
                    "blort" + newline +
                    "$endif$"
                );
            e.SetAttribute( "z", "yep" );
            string expecting = "blort";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestElseIfClauseAndElse()
        {
            StringTemplate e = new StringTemplate(
                    "$if(x)$" + newline +
                    "foo" + newline +
                    "$elseif(y)$" + newline +
                    "bar" + newline +
                    "$elseif(z)$" + newline +
                    "z" + newline +
                    "$else$" + newline +
                    "blort" + newline +
                    "$endif$"
                );
            string expecting = "blort";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestNestedIF()
        {
            StringTemplate e = new StringTemplate(
                    "$if(title)$" + newline +
                    "foo" + newline +
                    "$else$" + newline +
                    "$if(header)$" + newline +
                    "bar" + newline +
                    "$else$" + newline +
                    "blort" + newline +
                    "$endif$" + newline +
                    "$endif$"
                );
            e.SetAttribute( "title", "sample" );
            string expecting = "foo";
            Assert.AreEqual( expecting, e.ToString() );

            e = e.GetInstanceOf();
            e.SetAttribute( "header", "more" );
            expecting = "bar";
            Assert.AreEqual( expecting, e.ToString() );

            e = e.GetInstanceOf();
            expecting = "blort";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestEmbeddedMultiLineIF()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate main = new StringTemplate( group, "$sub$" );
            StringTemplate sub = new StringTemplate( group,
                    "begin" + newline +
                    "$if(foo)$" + newline +
                    "$foo$" + newline +
                    "$else$" + newline +
                    "blort" + newline +
                    "$endif$" + newline
                );
            sub.SetAttribute( "foo", "stuff" );
            main.SetAttribute( "sub", sub );
            string expecting =
                "begin" + newline +
                "stuff";
            Assert.AreEqual( expecting, main.ToString() );

            main = new StringTemplate( group, "$sub$" );
            sub = sub.GetInstanceOf();
            main.SetAttribute( "sub", sub );
            expecting =
                "begin" + newline +
                "blort";
            Assert.AreEqual( expecting, main.ToString() );
        }

        [TestMethod]
        public void TestSimpleIndentOfAttributeList()
        {
            string templates =
                    "group test;" + newline +
                    "list(names) ::= <<" +
                    "  $names; separator=\"\n\"$" + newline +
                    ">>" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate t = group.GetInstanceOf( "list" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "Jim" );
            t.SetAttribute( "names", "Sriram" );
            string expecting =
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndentOfMultilineAttributes()
        {
            string templates =
                    "group test;" + newline +
                    "list(names) ::= <<" +
                    "  $names; separator=\"\n\"$" + newline +
                    ">>" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate t = group.GetInstanceOf( "list" );
            t.SetAttribute( "names", "Terence\nis\na\nmaniac" );
            t.SetAttribute( "names", "Jim" );
            t.SetAttribute( "names", "Sriram\nis\ncool" );
            string expecting =
                    "  Terence" + newline +
                    "  is" + newline +
                    "  a" + newline +
                    "  maniac" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "  is" + newline +
                    "  cool";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndentOfMultipleBlankLines()
        {
            string templates =
                    "group test;" + newline +
                    "list(names) ::= <<" +
                    "  $names$" + newline +
                    ">>" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate t = group.GetInstanceOf( "list" );
            t.SetAttribute( "names", "Terence\n\nis a maniac" );
            string expecting =
                    "  Terence" + newline +
                    "" + newline + // no indent on blank line
                    "  is a maniac";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndentBetweenLeftJustifiedLiterals()
        {
            string templates =
                    "group test;" + newline +
                    "list(names) ::= <<" +
                    "Before:" + newline +
                    "  $names; separator=\"\\n\"$" + newline +
                    "after" + newline +
                    ">>" + newline;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate t = group.GetInstanceOf( "list" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "Jim" );
            t.SetAttribute( "names", "Sriram" );
            string expecting =
                    "Before:" + newline +
                    "  Terence" + newline +
                    "  Jim" + newline +
                    "  Sriram" + newline +
                    "after";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNestedIndent()
        {
            string templates =
                    "group test;" + newline +
                    "method(name,stats) ::= <<" +
                    "void $name$() {" + newline +
                    "\t$stats; separator=\"\\n\"$" + newline +
                    "}" + newline +
                    ">>" + newline +
                    "ifstat(expr,stats) ::= <<" + newline +
                    "if ($expr$) {" + newline +
                    "  $stats; separator=\"\\n\"$" + newline +
                    "}" +
                    ">>" + newline +
                    "assign(lhs,expr) ::= <<$lhs$=$expr$;>>" + newline
                    ;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate t = group.GetInstanceOf( "method" );
            t.SetAttribute( "name", "foo" );
            StringTemplate s1 = group.GetInstanceOf( "assign" );
            s1.SetAttribute( "lhs", "x" );
            s1.SetAttribute( "expr", "0" );
            StringTemplate s2 = group.GetInstanceOf( "ifstat" );
            s2.SetAttribute( "expr", "x>0" );
            StringTemplate s2a = group.GetInstanceOf( "assign" );
            s2a.SetAttribute( "lhs", "y" );
            s2a.SetAttribute( "expr", "x+y" );
            StringTemplate s2b = group.GetInstanceOf( "assign" );
            s2b.SetAttribute( "lhs", "z" );
            s2b.SetAttribute( "expr", "4" );
            s2.SetAttribute( "stats", s2a );
            s2.SetAttribute( "stats", s2b );
            t.SetAttribute( "stats", s1 );
            t.SetAttribute( "stats", s2 );
            string expecting =
                    "void foo() {" + newline +
                    "\tx=0;" + newline +
                    "\tif (x>0) {" + newline +
                    "\t  y=x+y;" + newline +
                    "\t  z=4;" + newline +
                    "\t}" + newline +
                    "}";
            Assert.AreEqual( expecting, t.ToString() );
        }

        class AlternativeWriter : IStringTemplateWriter
        {
            StringBuilder _buffer;

            public AlternativeWriter( StringBuilder buffer )
            {
                _buffer = buffer;
            }

            public void PushIndentation( string indent )
            {
            }
            public string PopIndentation()
            {
                return null;
            }
            public void PushAnchorPoint()
            {
            }
            public void PopAnchorPoint()
            {
            }
            public void SetLineWidth( int lineWidth )
            {
            }
            public int Write( string str )
            {
                _buffer.Append( str );
                return str.Length;
            }
            public int Write( string str, string wrap )
            {
                return 0;
            }
            public int WriteWrapSeparator( string wrap )
            {
                return 0;
            }
            public int WriteSeparator( string str )
            {
                return Write( str );
            }
        }

        [TestMethod]
        public void TestAlternativeWriter()
        {
            StringBuilder buf = new StringBuilder();
            IStringTemplateWriter w = new AlternativeWriter( buf );
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            group.DefineTemplate( "bold", "<b>$x$</b>" );
            StringTemplate name = new StringTemplate( group, "$name:bold(x=name)$" );
            name.SetAttribute( "name", "Terence" );
            name.Write( w );
            Assert.AreEqual( "<b>Terence</b>", buf.ToString() );
        }

        [TestMethod]
        public void TestApplyAnonymousTemplateToMapAndSet()
        {
            StringTemplate st =
                    new StringTemplate( "$items:{<li>$it$</li>}$" );
            IDictionary m = new SortedList<object, object>();
            m["a"] = "1";
            m["b"] = "2";
            m["c"] = "3";
            st.SetAttribute( "items", m );
            string expecting = "<li>1</li><li>2</li><li>3</li>";
            Assert.AreEqual( expecting, st.ToString() );

            st = st.GetInstanceOf();
            HashSet<object> s = new HashSet<object>();
            s.Add( "1" );
            s.Add( "2" );
            s.Add( "3" );
            st.SetAttribute( "items", s );
            //expecting = "<li>3</li><li>2</li><li>1</li>";
            expecting = "<li>1</li><li>2</li><li>3</li>";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestDumpMapAndSet()
        {
            StringTemplate st =
                    new StringTemplate( "$items; separator=\",\"$" );
            IDictionary m = new SortedList<object, object>();
            m["a"] = "1";
            m["b"] = "2";
            m["c"] = "3";
            st.SetAttribute( "items", m );
            string expecting = "1,2,3";
            Assert.AreEqual( expecting, st.ToString() );

            st = st.GetInstanceOf();
            HashSet<object> s = new HashSet<object>();
            s.Add( "1" );
            s.Add( "2" );
            s.Add( "3" );
            st.SetAttribute( "items", s );
            //expecting = "3,2,1";
            expecting = "1,2,3";
            Assert.AreEqual( expecting, st.ToString() );
        }

        public class Connector3
        {
            public int[] getValues()
            {
                return new int[] { 1, 2, 3 };
            }
            public IDictionary getStuff()
            {
                IDictionary m = new SortedList<object, object>();
                m["a"] = "1";
                m["b"] = "2";
                return m;
            }
        }

        [TestMethod]
        public void TestApplyAnonymousTemplateToArrayAndMapProperty()
        {
            StringTemplate st =
                    new StringTemplate( "$x.values:{<li>$it$</li>}$" );
            st.SetAttribute( "x", new Connector3() );
            string expecting = "<li>1</li><li>2</li><li>3</li>";
            Assert.AreEqual( expecting, st.ToString() );

            st = new StringTemplate( "$x.stuff:{<li>$it$</li>}$" );
            st.SetAttribute( "x", new Connector3() );
            expecting = "<li>1</li><li>2</li>";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestSuperTemplateRef()
        {
            // you can refer to a template defined in a super group via super.t()
            StringTemplateGroup group = new StringTemplateGroup( "super" );
            StringTemplateGroup subGroup = new StringTemplateGroup( "sub" );
            subGroup.SuperGroup = group;
            group.DefineTemplate( "page", "$font()$:text" );
            group.DefineTemplate( "font", "Helvetica" );
            subGroup.DefineTemplate( "font", "$super.font()$ and Times" );
            StringTemplate st = subGroup.GetInstanceOf( "page" );
            string expecting =
                    "Helvetica and Times:text";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestApplySuperTemplateRef()
        {
            StringTemplateGroup group = new StringTemplateGroup( "super" );
            StringTemplateGroup subGroup = new StringTemplateGroup( "sub" );
            subGroup.SuperGroup = group;
            group.DefineTemplate( "bold", "<b>$it$</b>" );
            subGroup.DefineTemplate( "bold", "<strong>$it$</strong>" );
            subGroup.DefineTemplate( "page", "$name:super.bold()$" );
            StringTemplate st = subGroup.GetInstanceOf( "page" );
            st.SetAttribute( "name", "Ter" );
            string expecting =
                    "<b>Ter</b>";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestLazyEvalOfSuperInApplySuperTemplateRef()
        {
            StringTemplateGroup group = new StringTemplateGroup( "base" );
            StringTemplateGroup subGroup = new StringTemplateGroup( "sub" );
            subGroup.SuperGroup = group;
            group.DefineTemplate( "bold", "<b>$it$</b>" );
            subGroup.DefineTemplate( "bold", "<strong>$it$</strong>" );
            // this is the same as testApplySuperTemplateRef() test
            // 'cept notice that here the supergroup defines page
            // As long as you create the instance via the subgroup,
            // "super." will evaluate lazily (i.e., not statically
            // during template compilation) to the templates
            // getGroup().superGroup value.  If I create instance
            // of page in group not subGroup, however, I will get
            // an error as superGroup is null for group "group".
            group.DefineTemplate( "page", "$name:super.bold()$" );
            StringTemplate st = subGroup.GetInstanceOf( "page" );
            st.SetAttribute( "name", "Ter" );
            string error = null;
            try
            {
                st.ToString();
            }
            catch ( ArgumentException iae )
            {
                error = iae.Message;
            }
            string expectingError = "base has no super group; invalid template: super.bold";
            Assert.AreEqual( expectingError, error );
        }

        [TestMethod]
        public void TestTemplatePolymorphism()
        {
            StringTemplateGroup group = new StringTemplateGroup( "super" );
            StringTemplateGroup subGroup = new StringTemplateGroup( "sub" );
            subGroup.SuperGroup = group;
            // bold is defined in both super and sub
            // if you create an instance of page via the subgroup,
            // then bold() should evaluate to the subgroup not the super
            // even though page is defined in the super.  Just like polymorphism.
            group.DefineTemplate( "bold", "<b>$it$</b>" );
            group.DefineTemplate( "page", "$name:bold()$" );
            subGroup.DefineTemplate( "bold", "<strong>$it$</strong>" );
            StringTemplate st = subGroup.GetInstanceOf( "page" );
            st.SetAttribute( "name", "Ter" );
            string expecting =
                    "<strong>Ter</strong>";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestListOfEmbeddedTemplateSeesEnclosingAttributes()
        {
            string templates =
                    "group test;" + newline +
                    "output(cond,items) ::= <<page: $items$>>" + newline +
                    "mybody() ::= <<$font()$stuff>>" + newline +
                    "font() ::= <<$if(cond)$this$else$that$endif$>>"
                    ;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ),
                                            errors );
            StringTemplate outputST = group.GetInstanceOf( "output" );
            StringTemplate bodyST1 = group.GetInstanceOf( "mybody" );
            StringTemplate bodyST2 = group.GetInstanceOf( "mybody" );
            StringTemplate bodyST3 = group.GetInstanceOf( "mybody" );
            outputST.SetAttribute( "items", bodyST1 );
            outputST.SetAttribute( "items", bodyST2 );
            outputST.SetAttribute( "items", bodyST3 );
            string expecting = "page: thatstuffthatstuffthatstuff";
            Assert.AreEqual( expecting, outputST.ToString() );
        }

        [TestMethod]
        public void TestInheritArgumentFromRecursiveTemplateApplication()
        {
            // do not inherit attributes through formal args
            string templates =
                    "group test;" + newline +
                    "block(stats) ::= \"<stats>\"" +
                    "ifstat(stats) ::= \"IF true then <stats>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "block" );
            b.SetAttribute( "stats", group.GetInstanceOf( "ifstat" ) );
            b.SetAttribute( "stats", group.GetInstanceOf( "ifstat" ) );
            string expecting = "IF true then IF true then ";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }


        [TestMethod]
        public void TestDeliberateRecursiveTemplateApplication()
        {
            // This test will cause infinite loop.  block contains a stat which
            // contains the same block.  Must be in lintMode to detect
            string templates =
                    "group test;" + newline +
                    "block(stats) ::= \"<stats>\"" +
                    "ifstat(stats) ::= \"IF true then <stats>\"" + newline
                    ;
            StringTemplate.LintMode = true;
            StringTemplate.ResetTemplateCounter();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "block" );
            StringTemplate ifstat = group.GetInstanceOf( "ifstat" );
            b.SetAttribute( "stats", ifstat ); // block has if stat
            ifstat.SetAttribute( "stats", b ); // but make "if" contain block
            string expectingError =
                    "infinite recursion to <ifstat([stats])@4> referenced in <block([stats])@3>; stack trace:" + newline +
                    "<ifstat([stats])@4>, attributes=[stats=<block()@3>]>" + newline +
                    "<block([stats])@3>, attributes=[stats=<ifstat()@4>], references=[stats]>" + newline +
                    "<ifstat([stats])@4> (start of recursive cycle)" + newline +
                    "...";
            // note that attributes attribute doesn't show up in ifstat() because
            // recursion detection traps the problem before it writes out the
            // infinitely-recursive template; I set the attributes attribute right
            // before I render.
            string errors = "";
            try
            {
                string result = b.ToString();
            }
            catch ( InvalidOperationException ise )
            {
                errors = ise.Message;
            }
            //System.err.println("errors="+errors+"'");
            //System.err.println("expecting="+expectingError+"'");
            StringTemplate.LintMode = false;
            Assert.AreEqual( expectingError, errors );
        }


        [TestMethod]
        public void TestImmediateTemplateAsAttributeLoop()
        {
            // even though block has a stats value that refers to itself,
            // there is no recursion because each instance of block hides
            // the stats value from above since it's a formal arg.
            string templates =
                    "group test;" + newline +
                    "block(stats) ::= \"{<stats>}\""
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "block" );
            b.SetAttribute( "stats", group.GetInstanceOf( "block" ) );
            string expecting = "{{}}";
            string result = b.ToString();
            //System.err.println(result);
            Assert.AreEqual( expecting, result );
        }


        [TestMethod]
        public void TestTemplateAlias()
        {
            string templates =
                    "group test;" + newline +
                    "page(name) ::= \"name is <name>\"" +
                    "other ::= page" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "other" );  // alias for page
            b.SetAttribute( "name", "Ter" );
            string expecting = "name is Ter";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestTemplateGetPropertyGetsAttribute()
        {
            // This test will cause infinite loop if missing attribute no
            // properly caught in getAttribute
            string templates =
                    "group test;" + newline +
                    "Cfile(funcs) ::= <<" + newline +
                    "#include \\<stdio.h>" + newline +
                    "<funcs:{public void <it.name>(<it.args>);}; separator=\"\\n\">" + newline +
                    "<funcs; separator=\"\\n\">" + newline +
                    ">>" + newline +
                    "func(name,args,body) ::= <<" + newline +
                    "public void <name>(<args>) {<body>}" + newline +
                    ">>" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "Cfile" );
            StringTemplate f1 = group.GetInstanceOf( "func" );
            StringTemplate f2 = group.GetInstanceOf( "func" );
            f1.SetAttribute( "name", "f" );
            f1.SetAttribute( "args", "" );
            f1.SetAttribute( "body", "i=1;" );
            f2.SetAttribute( "name", "g" );
            f2.SetAttribute( "args", "int arg" );
            f2.SetAttribute( "body", "y=1;" );
            b.SetAttribute( "funcs", f1 );
            b.SetAttribute( "funcs", f2 );
            string expecting = "#include <stdio.h>" + newline +
                    "public void f();" + newline +
                    "public void g(int arg);" + newline +
                    "public void f() {i=1;}" + newline +
                    "public void g(int arg) {y=1;}";
            Assert.AreEqual( expecting, b.ToString() );
        }

        public class Decl
        {
            string _name;
            string _type;
            public Decl( string name, string type )
            {
                _name = name;
                _type = type;
            }
            public string Name
            {
                get
                {
                    return _name;
                }
            }
            public string Type
            {
                get
                {
                    return _type;
                }
            }
        }

        [TestMethod]
        public void TestComplicatedIndirectTemplateApplication()
        {
            string templates =
                    "group Java;" + newline +
                    "" + newline +
                    "file(variables) ::= <<" +
                    "<variables:{ v | <v.decl:(v.format)()>}; separator=\"\\n\">" + newline +
                    ">>" + newline +
                    "intdecl(decl) ::= \"int <decl.name> = 0;\"" + newline +
                    "intarray(decl) ::= \"int[] <decl.name> = null;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "file" );
            f.SetAttribute( "variables.{decl,format}", new Decl( "i", "int" ), "intdecl" );
            f.SetAttribute( "variables.{decl,format}", new Decl( "a", "int-array" ), "intarray" );
            //System.out.println("f='"+f+"'");
            string expecting = "int i = 0;" + newline +
                    "int[] a = null;";
            Assert.AreEqual( expecting, f.ToString() );
        }

        [TestMethod]
        public void TestIndirectTemplateApplication()
        {
            string templates =
                    "group dork;" + newline +
                    "" + newline +
                    "test(name) ::= <<" +
                    "<(name)()>" + newline +
                    ">>" + newline +
                    "first() ::= \"the first\"" + newline +
                    "second() ::= \"the second\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "test" );
            f.SetAttribute( "name", "first" );
            string expecting = "the first";
            Assert.AreEqual( expecting, f.ToString() );
        }

        [TestMethod]
        public void TestIndirectTemplateWithArgsApplication()
        {
            string templates =
                    "group dork;" + newline +
                    "" + newline +
                    "test(name) ::= <<" +
                    "<(name)(a=\"foo\")>" + newline +
                    ">>" + newline +
                    "first(a) ::= \"the first: <a>\"" + newline +
                    "second(a) ::= \"the second <a>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "test" );
            f.SetAttribute( "name", "first" );
            string expecting = "the first: foo";
            Assert.AreEqual( expecting, f.ToString() );
        }

        [TestMethod]
        public void TestNullIndirectTemplateApplication()
        {
            string templates =
                    "group dork;" + newline +
                    "" + newline +
                    "test(names,t) ::= <<" +
                    "<names:(t)()>" + newline + // t null be must be defined else error: null attr w/o formal def
                    ">>" + newline +
                    "ind() ::= \"[<it>]\"" + newline;
            ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "test" );
            f.SetAttribute( "names", "me" );
            f.SetAttribute( "names", "you" );
            string expecting = "";
            Assert.AreEqual( expecting, f.ToString() );
        }

        [TestMethod]
        public void TestNullIndirectTemplate()
        {
            string templates =
                    "group dork;" + newline +
                    "" + newline +
                    "test(name) ::= <<" +
                    "<(name)()>" + newline +
                    ">>" + newline +
                    "first() ::= \"the first\"" + newline +
                    "second() ::= \"the second\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "test" );
            //f.SetAttribute("name", "first");
            string expecting = "";
            Assert.AreEqual( expecting, f.ToString() );
        }

        [TestMethod]
        public void TestHashMapPropertyFetch()
        {
            StringTemplate a = new StringTemplate( "$stuff.prop$" );
            Dictionary<object, object> map = new Dictionary<object, object>();
            a.SetAttribute( "stuff", map );
            map["prop"] = "Terence";
            string results = a.ToString();
            //System.out.println(results);
            string expecting = "Terence";
            Assert.AreEqual( expecting, results );
        }

        [TestMethod]
        public void TestHashMapPropertyFetchEmbeddedStringTemplate()
        {
            StringTemplate a = new StringTemplate( "$stuff.prop$" );
            Dictionary<object, object> map = new Dictionary<object, object>();
            a.SetAttribute( "stuff", map );
            a.SetAttribute( "title", "ST rocks" );
            map["prop"] = new StringTemplate( "embedded refers to $title$" );
            string results = a.ToString();
            //System.out.println(results);
            string expecting = "embedded refers to ST rocks";
            Assert.AreEqual( expecting, results );
        }

        [TestMethod]
        public void TestEmbeddedComments()
        {
            StringTemplate st = new StringTemplate(
                    "Foo $! ignore !$bar" + newline
                    );
            string expecting = "Foo bar" + newline;
            string result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo $! ignore" + newline +
                    " and a line break!$" + newline +
                    "bar" + newline
                    );
            expecting = "Foo " + newline + "bar" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "$! start of line $ and $! ick" + newline +
                    "!$boo" + newline
                    );
            expecting = "boo" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                "$! start of line !$" + newline +
                "$! another to ignore !$" + newline +
                "$! ick" + newline +
                "!$boo" + newline
            );
            expecting = "boo" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                "$! back !$$! to back !$" + newline + // can't detect; leaves \n
                "$! ick" + newline +
                "!$boo" + newline
            );
            expecting = newline + "boo" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmbeddedCommentsAngleBracketed()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <! ignore !>bar" + newline,
                    typeof( AngleBracketTemplateLexer )
                    );
            string expecting = "Foo bar" + newline;
            string result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo <! ignore" + newline +
                    " and a line break!>" + newline +
                    "bar" + newline,
                    typeof( AngleBracketTemplateLexer )
                    );
            expecting = "Foo " + newline + "bar" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "<! start of line $ and <! ick" + newline +
                    "!>boo" + newline,
                    typeof( AngleBracketTemplateLexer )
                    );
            expecting = "boo" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                "<! start of line !>" +
                "<! another to ignore !>" +
                "<! ick" + newline +
                "!>boo" + newline,
                    typeof( AngleBracketTemplateLexer )
            );
            expecting = "boo" + newline;
            result = st.ToString();
            //System.out.println(result);
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                "<! back !><! to back !>" + newline + // can't detect; leaves \n
                "<! ick" + newline +
                "!>boo" + newline,
                    typeof( AngleBracketTemplateLexer )
            );
            expecting = newline + "boo" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestLineBreak()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <\\\\>" + newline +
                    "  \t  bar" + newline,
                    typeof(AngleBracketTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreak2()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <\\\\>       " + newline +
                    "  \t  bar" + newline,
                    typeof(AngleBracketTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreakNoWhiteSpace()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <\\\\>" + newline +
                    "bar" + newline,
                    typeof(AngleBracketTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreakDollar()
        {
            StringTemplate st = new StringTemplate(
                    "Foo $\\\\$" + newline +
                    "  \t  bar" + newline,
                    typeof(DefaultTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreakDollar2()
        {
            StringTemplate st = new StringTemplate(
                    "Foo $\\\\$          " + newline +
                    "  \t  bar" + newline,
                    typeof(DefaultTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestLineBreakNoWhiteSpaceDollar()
        {
            StringTemplate st = new StringTemplate(
                    "Foo $\\\\$" + newline +
                    "bar" + newline,
                    typeof(DefaultTemplateLexer)
                    );
            StringWriter sw = new StringWriter();
            st.Write(new AutoIndentWriter(sw, "\n")); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo bar\n";     // expect \n in output
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestCharLiterals()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <\\r\\n><\\n><\\t> bar" + newline,
                    typeof( AngleBracketTemplateLexer )
                    );
            StringWriter sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\n" ) ); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo \n\n\t bar\n";     // expect \n in output
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo $\\n$$\\t$ bar" + newline );
            sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\n" ) ); // force \n as newline
            expecting = "Foo \n\t bar\n";     // expect \n in output
            result = sw.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo$\\ $bar$\\n$" );
            sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\n" ) ); // force \n as newline
            result = sw.ToString();
            expecting = "Foo bar\n"; // force \n
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestNewlineNormalizationInTemplateString()
        {
            StringTemplate st = new StringTemplate(
                    "Foo\r\n" +
                    "Bar\n",
                    typeof( AngleBracketTemplateLexer )
                    );
            StringWriter sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\n" ) ); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo\nBar\n";     // expect \n in output
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestNewlineNormalizationInTemplateStringPC()
        {
            StringTemplate st = new StringTemplate(
                    "Foo\r\n" +
                    "Bar\n",
                    typeof( AngleBracketTemplateLexer )
                    );
            StringWriter sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\r\n" ) ); // force \r\n as newline
            string result = sw.ToString();
            string expecting = "Foo\r\nBar\r\n";     // expect \r\n in output
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestNewlineNormalizationInAttribute()
        {
            StringTemplate st = new StringTemplate(
                    "Foo\r\n" +
                    "<name>\n",
                    typeof( AngleBracketTemplateLexer )
                    );
            st.SetAttribute( "name", "a\nb\r\nc" );
            StringWriter sw = new StringWriter();
            st.Write( new AutoIndentWriter( sw, "\n" ) ); // force \n as newline
            string result = sw.ToString();
            string expecting = "Foo\na\nb\nc\n";     // expect \n in output
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestUnicodeLiterals()
        {
            StringTemplate st = new StringTemplate(
                    "Foo <\\uFEA5\\n\\u00C2> bar" + newline,
                    typeof( AngleBracketTemplateLexer )
                    );
            string expecting = "Foo \xfea5" + newline + "\x00C2 bar" + newline;
            string result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo $\\uFEA5\\n\\u00C2$ bar" + newline );
            expecting = "Foo \xfea5" + newline + "\x00C2 bar" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );

            st = new StringTemplate(
                    "Foo$\\ $bar$\\n$" );
            expecting = "Foo bar" + newline;
            result = st.ToString();
            Assert.AreEqual( expecting, result );
        }


        [TestMethod]
        public void TestEmptyIteratedValueGetsSeparator()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group, "$names; separator=\",\"$" );
            t.SetAttribute( "names", "Terence" );
            t.SetAttribute( "names", "" );
            t.SetAttribute( "names", "" );
            t.SetAttribute( "names", "Tom" );
            t.SetAttribute( "names", "Frank" );
            t.SetAttribute( "names", "" );
            // empty values get separator still
            string expecting = "Terence,,,Tom,Frank,";
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMissingIteratedConditionalValueGetsNoSeparator()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "$users:{$if(it.ok)$$it.name$$endif$}; separator=\",\"$" );
            t.SetAttribute( "users.{name,ok}", "Terence", ( true ) );
            t.SetAttribute( "users.{name,ok}", "Tom", ( false ) );
            t.SetAttribute( "users.{name,ok}", "Frank", ( true ) );
            t.SetAttribute( "users.{name,ok}", "Johnny", ( false ) );
            // empty conditional values get no separator
            string expecting = "Terence,Frank";
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMissingIteratedConditionalValueGetsNoSeparator2()
        {
            StringTemplateGroup group = new StringTemplateGroup("test");
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate(group, "$users:{$if(it.ok)$$it.name$$endif$}; separator=\",\"$");
            t.SetAttribute("users.{name,ok}", "Terence", true);
            t.SetAttribute("users.{name,ok}", "Tom", false);
            t.SetAttribute("users.{name,ok}", "Frank", false);
            t.SetAttribute("users.{name,ok}", "Johnny", false);
            // empty conditional values get no separator
            string expecting = "Terence";
            string result = t.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestMissingIteratedDoubleConditionalValueGetsNoSeparator()
        {
            StringTemplateGroup group = new StringTemplateGroup("test");
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate(group, "$users:{$if(it.ok)$$it.name$$endif$$if(it.ok)$$it.name$$endif$}; separator=\",\"$");
            t.SetAttribute("users.{name,ok}", "Terence", false);
            t.SetAttribute("users.{name,ok}", "Tom", true);
            t.SetAttribute("users.{name,ok}", "Frank", true);
            t.SetAttribute("users.{name,ok}", "Johnny", true);
            // empty conditional values get no separator
            string expecting = "TomTom,FrankFrank,JohnnyJohnny";
            string result = t.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestIteratedConditionalWithEmptyElseValueGetsSeparator()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "$users:{$if(it.ok)$$it.name$$else$$endif$}; separator=\",\"$" );
            t.SetAttribute( "users.{name,ok}", "Terence", true );
            t.SetAttribute( "users.{name,ok}", "Tom", false );
            t.SetAttribute( "users.{name,ok}", "Frank", true );
            t.SetAttribute( "users.{name,ok}", "Johnny", false );
            // empty conditional values get no separator
            string expecting = "Terence,,Frank,";
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestWhiteSpaceAtEndOfTemplate()
        {
            StringTemplateGroup group = new StringTemplateGroup( "group", System.Reflection.Assembly.GetExecutingAssembly() );
            StringTemplate pageST = group.GetInstanceOf( "AntlrUnitTests/page" );
            StringTemplate listST = group.GetInstanceOf( "AntlrUnitTests/users_list" );
            // users.list references row.st which has a single blank line at the end.
            // I.e., there are 2 \n in a row at the end
            // ST should eat all whitespace at end
            listST.SetAttribute( "users", new Connector() );
            listST.SetAttribute( "users", new Connector2() );
            pageST.SetAttribute( "title", "some title" );
            pageST.SetAttribute( "body", listST );
            string expecting = "some title" + newline +
                "Terence parrt@jguru.comTom tombu@jguru.com";
            string result = pageST.ToString();
            //System.out.println("'"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        class Duh
        {
            public IList users = new List<object>();
        }

        [TestMethod]
        public void TestSizeZeroButNonNullListGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "$duh.users:{name: $it$}; separator=\", \"$\n" +
                "end\n" );
            t.SetAttribute( "duh", new Duh() );
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestNullListGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "$users:{name: $it$}; separator=\", \"$\n" +
                "end\n" );
            //t.SetAttribute("users", new Duh());
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyListGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "$users:{name: $it$}; separator=\", \"$\n" +
                "end\n" );
            t.SetAttribute( "users", new List<object>() );
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyListNoIteratorGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "$users; separator=\", \"$\n" +
                "end\n" );
            t.SetAttribute( "users", new List<object>() );
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyExprAsFirstLineGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            group.DefineTemplate( "bold", "<b>$it$</b>" );
            StringTemplate t = new StringTemplate( group,
                "$users$\n" +
                "end\n" );
            string expecting = "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSizeZeroOnLineByItselfGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "$name$\n" +
                "$users:{name: $it$}$\n" +
                "$users:{name: $it$}; separator=\", \"$\n" +
                "end\n" );
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSizeZeroOnLineWithIndentGetsNoOutput()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "begin\n" +
                "  $name$\n" +
                "	$users:{name: $it$}$\n" +
                "	$users:{name: $it$$\\n$}$\n" +
                "end\n" );
            string expecting = "begin" + newline + "end" + newline;
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSimpleAutoIndent()
        {
            StringTemplate a = new StringTemplate(
                "$title$: {\n" +
                "	$name; separator=\"\n\"$\n" +
                "}" );
            a.SetAttribute( "title", "foo" );
            a.SetAttribute( "name", "Terence" );
            a.SetAttribute( "name", "Frank" );
            string results = a.ToString();
            //System.out.println(results);
            string expecting =
                "foo: {" + newline +
                "\tTerence" + newline +
                "\tFrank" + newline +
                "}";
            Assert.AreEqual( expecting, results );
        }

        [TestMethod]
        public void TestComputedPropertyName()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            IStringTemplateErrorListener errors = new ErrorBuffer();
            group.ErrorListener = errors;
            StringTemplate t = new StringTemplate( group,
                "variable property $propName$=$v.(propName)$" );
            t.SetAttribute( "v", new Decl( "i", "int" ) );
            t.SetAttribute( "propName", "type" );
            string expecting = "variable property type=int";
            string result = t.ToString();
            Assert.AreEqual( "", errors.ToString() );
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestNonNullButEmptyIteratorTestsFalse()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate t = new StringTemplate( group,
                "$if(users)$\n" +
                "Users: $users:{$it.name$ }$\n" +
                "$endif$" );
            t.SetAttribute( "users", new LinkedList<object>() );
            string expecting = "";
            string result = t.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDoNotInheritAttributesThroughFormalArgs()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= \"<stat()>\"" + newline +
                    "stat(name) ::= \"x=y; // <name>\"" + newline
                    ;
            // name is not visible in stat because of the formal arg called name.
            // somehow, it must be set.
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=y; // ";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestArgEvaluationContext()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= \"<stat(name=name)>\"" + newline +
                    "stat(name) ::= \"x=y; // <name>\"" + newline
                    ;
            // attribute name is not visible in stat because of the formal
            // arg called name in template stat.  However, we can set it's value
            // with an explicit name=name.  This looks weird, but makes total
            // sense as the rhs is evaluated in the context of method and the lhs
            // is evaluated in the context of stat's arg list.
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=y; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestPassThroughAttributes()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= \"<stat(...)>\"" + newline +
                    "stat(name) ::= \"x=y; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=y; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestPassThroughAttributes2()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= <<" + newline +
                    "<stat(value=\"34\",...)>" + newline +
                    ">>" + newline +
                    "stat(name,value) ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=34; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDefaultArgument()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=99; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDefaultArgument2()
        {
            string templates =
                    "group test;" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "stat" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=99; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        public class Field
        {
            public string name = "parrt";
            public int n = 0;

            public override string ToString()
            {
                return "Field";
            }
        }

        [TestMethod]
        public void TestDefaultArgumentManuallySet()
        {
            string templates =
                    "group test;" + newline +
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat(f=f)>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;

            StringTemplateGroup group = new StringTemplateGroup(new StringReader(templates));
            StringTemplate m = group.GetInstanceOf("method");
            m.SetAttribute("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.ToString();
            Assert.AreEqual(expecting, result);
        }

        /** This fails because checkNullAttributeAgainstFormalArguments looks
         *  for a formal argument at the current level not of the original embedded
         *  template. We have defined it all the way in the embedded, but there is
         *  no value so we try to look upwards ala dynamic scoping. When it reaches
         *  the top, it doesn't find a value but it will miss the
         *  formal argument down in the embedded.
         *
         *  By definition, though, the formal parameter exists if we have
         *  a default value. look up the value to see if it's null without
         *  checking checkNullAttributeAgainstFormalArguments.
         */
        [TestMethod]
        public void TestDefaultArgumentImplicitlySet()
        {
            string templates =
                    "group test;" + newline +
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <stat(...)>}>" + newline +
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;

            StringTemplateGroup group = new StringTemplateGroup(new StringReader(templates));
            StringTemplate m = group.GetInstanceOf("method");
            m.SetAttribute("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentImplicitlySet2()
        {
            string templates =
                    "group test;" + newline +
                    "method(fields) ::= <<" + newline +
                    "<fields:{f | <f:stat()>}>" + newline +  // THIS SHOULD BE ERROR; >1 arg?
                    ">>" + newline +
                    "stat(f,value={<f.name>}) ::= \"x=<value>; // <f.name>\"" + newline
                    ;

            StringTemplateGroup group = new StringTemplateGroup(new StringReader(templates));
            StringTemplate m = group.GetInstanceOf("method");
            m.SetAttribute("fields", new Field());
            string expecting = "x=parrt; // parrt";
            string result = m.ToString();
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate()
        {
            string templates =
                    "group test;" + newline +
                    "method(name,size) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value={<name>}) ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            b.SetAttribute( "size", "2" );
            string expecting = "x=foo; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDefaultArgumentAsTemplate2()
        {
            string templates =
                    "group test;" + newline +
                    "method(name,size) ::= <<" + newline +
                    "<stat(...)>" + newline +
                    ">>" + newline +
                    "stat(name,value={ [<name>] }) ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group = new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            b.SetAttribute( "size", "2" );
            string expecting = "x= [foo] ; // foo";
            string result = b.ToString();
            //System.err.println("result='"+result+"'");
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDoNotUseDefaultArgument()
        {
            string templates =
                    "group test;" + newline +
                    "method(name) ::= <<" + newline +
                    "<stat(value=\"34\",...)>" + newline +
                    ">>" + newline +
                    "stat(name,value=\"99\") ::= \"x=<value>; // <name>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            string expecting = "x=34; // foo";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        private class Counter
        {
            int n = 0;
            public override string ToString()
            {
                return (n++).ToString();
            }
        }

        [TestMethod]
        public void TestDefaultArgumentInParensToEvalEarly()
        {
            string templates =
                    "group test;" + newline +
                    "A(x) ::= \"<B()>\"" + newline +
                    "B(y={<(x)>}) ::= \"<y> <x> <x> <y>\"" + newline
                    ;

            StringTemplateGroup group = new StringTemplateGroup(new StringReader(templates));
            StringTemplate b = group.GetInstanceOf("A");
            b.SetAttribute("x", new Counter());
            string expecting = "0 1 2 0";
            string result = b.ToString();
            //Console.Error.WriteLine("result='" + result + "'");
            Assert.AreEqual(expecting, result);
        }

        [TestMethod]
        public void TestArgumentsAsTemplates()
        {
            string templates =
                    "group test;" + newline +
                    "method(name,size) ::= <<" + newline +
                    "<stat(value={<size>})>" + newline +
                    ">>" + newline +
                    "stat(value) ::= \"x=<value>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            b.SetAttribute( "size", "34" );
            string expecting = "x=34;";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestTemplateArgumentEvaluatedInSurroundingContext()
        {
            string templates =
                    "group test;" + newline +
                    "file(m,size) ::= \"<m>\"" + newline +
                    "method(name) ::= <<" + newline +
                    "<stat(value={<size>.0})>" + newline +
                    ">>" + newline +
                    "stat(value) ::= \"x=<value>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate f = group.GetInstanceOf( "file" );
            f.SetAttribute( "size", "34" );
            StringTemplate m = group.GetInstanceOf( "method" );
            m.SetAttribute( "name", "foo" );
            f.SetAttribute( "m", m );
            string expecting = "x=34.0;";
            string result = m.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestArgumentsAsTemplatesDefaultDelimiters()
        {
            string templates =
                    "group test;" + newline +
                    "method(name,size) ::= <<" + newline +
                    "$stat(value={$size$})$" + newline +
                    ">>" + newline +
                    "stat(value) ::= \"x=$value$;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate b = group.GetInstanceOf( "method" );
            b.SetAttribute( "name", "foo" );
            b.SetAttribute( "size", "34" );
            string expecting = "x=34;";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestDefaultArgsWhenNotInvoked()
        {
            string templates =
                    "group test;" + newline +
                    "b(name=\"foo\") ::= \".<name>.\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate b = group.GetInstanceOf( "b" );
            string expecting = ".foo.";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        public class DateRenderer : IAttributeRenderer
        {
            public string ToString( object o )
            {
                return ( (DateTime)o ).ToString( "yyyy.MM.dd" );
                //SimpleDateFormat f = new SimpleDateFormat( "yyyy.MM.dd" );
                //return f.format( ( (Calendar)o ).getTime() );
            }
            public string ToString( object o, string formatString )
            {
                return ToString( o );
            }
        }

        public class DateRenderer2 : IAttributeRenderer
        {
            public string ToString( object o )
            {
                return ( (DateTime)o ).ToString( "MM/dd/yyyy" );
                //SimpleDateFormat f = new SimpleDateFormat ("MM/dd/yyyy");
                //return f.format(((Calendar)o).getTime());
            }
            public string ToString( object o, string formatString )
            {
                return ToString( o );
            }
        }

        public class DateRenderer3 : IAttributeRenderer
        {
            public string ToString( object o )
            {
                return ( (DateTime)o ).ToString( "MM/dd/yyyy" );
                //SimpleDateFormat f = new SimpleDateFormat ("MM/dd/yyyy");
                //return f.format(((Calendar)o).getTime());
            }
            public string ToString( object o, string formatString )
            {
                return ( (DateTime)o ).ToString( formatString );
                //SimpleDateFormat f = new SimpleDateFormat (formatString);
                //return f.format(((Calendar)o).getTime());
            }
        }

        public class StringRenderer : IAttributeRenderer
        {
            public virtual string ToString( object o )
            {
                return (string)o;
            }
            public virtual string ToString( object o, string formatString )
            {
                if ( formatString.Equals( "upper" ) )
                {
                    return ( (string)o ).ToUpperInvariant();
                }
                return ToString( o );
            }
        }

        [TestMethod]
        public void TestRendererForST()
        {
            StringTemplate st = new StringTemplate(
                    "date: <created>",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "created",
                            new DateTime( 2005, 07, 05 ) );
            st.RegisterRenderer( typeof( DateTime ), new DateRenderer() );
            string expecting = "date: 2005.07.05";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRendererWithFormat()
        {
            StringTemplate st = new StringTemplate(
                    "date: <created; format=\"yyyy.MM.dd\">",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "created",
                            new DateTime( 2005, 07, 05 ) );
            st.RegisterRenderer( typeof( DateTime ), new DateRenderer3() );
            string expecting = "date: 2005.07.05";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRendererWithFormatAndList()
        {
            StringTemplate st = new StringTemplate(
                    "The names: <names; format=\"upper\">",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "names", "ter" );
            st.SetAttribute( "names", "tom" );
            st.SetAttribute( "names", "sriram" );
            st.RegisterRenderer( typeof( string ), new StringRenderer() );
            string expecting = "The names: TERTOMSRIRAM";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRendererWithFormatAndSeparator()
        {
            StringTemplate st = new StringTemplate(
                    "The names: <names; separator=\" and \", format=\"upper\">",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "names", "ter" );
            st.SetAttribute( "names", "tom" );
            st.SetAttribute( "names", "sriram" );
            st.RegisterRenderer( typeof( string ), new StringRenderer() );
            string expecting = "The names: TER and TOM and SRIRAM";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRendererWithFormatAndSeparatorAndNull()
        {
            StringTemplate st = new StringTemplate(
                    "The names: <names; separator=\" and \", null=\"n/a\", format=\"upper\">",
                    typeof( AngleBracketTemplateLexer ) );
            IList names = new List<object>();
            names.Add( "ter" );
            names.Add( null );
            names.Add( "sriram" );
            st.SetAttribute( "names", names );
            st.RegisterRenderer( typeof( string ), new StringRenderer() );
            string expecting = "The names: TER and N/A and SRIRAM";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmbeddedRendererSeesEnclosing()
        {
            // st is embedded in outer; set renderer on outer, st should
            // still see it.
            StringTemplate outer = new StringTemplate(
                    "X: <x>",
                    typeof( AngleBracketTemplateLexer ) );
            StringTemplate st = new StringTemplate(
                    "date: <created>",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "created",
                            new DateTime( 2005, 07, 05 ) );
            outer.SetAttribute( "x", st );
            outer.RegisterRenderer( typeof( DateTime ), new DateRenderer() );
            string expecting = "X: date: 2005.07.05";
            string result = outer.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestRendererForGroup()
        {
            string templates =
                    "group test;" + newline +
                    "dateThing(created) ::= \"date: <created>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "dateThing" );
            st.SetAttribute( "created",
                            new DateTime( 2005, 07, 05 ) );
            group.RegisterRenderer( typeof( DateTime ), new DateRenderer() );
            string expecting = "date: 2005.07.05";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestOverriddenRenderer()
        {
            string templates =
                    "group test;" + newline +
                    "dateThing(created) ::= \"date: <created>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "dateThing" );
            st.SetAttribute( "created",
                            new DateTime( 2005, 07, 05 ) );
            group.RegisterRenderer( typeof( DateTime ), new DateRenderer() );
            st.RegisterRenderer( typeof( DateTime ), new DateRenderer2() );
            string expecting = "date: 07/05/2005";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMap()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "int" );
            st.SetAttribute( "name", "x" );
            string expecting = "int x = 0;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapValuesAreTemplates()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0<w>\", \"float\":\"0.0<w>\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "w", "L" );
            st.SetAttribute( "type", "int" );
            st.SetAttribute( "name", "x" );
            string expecting = "int x = 0L;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapKeyLookupViaTemplate()
        {
            // ST doesn't do a toString on .(key) values, it just uses the value
            // of key rather than key itself as the key.  But, if you compute a
            // key via a template
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0<w>\", \"float\":\"0.0<w>\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "w", "L" );
            st.SetAttribute( "type", new StringTemplate( "int" ) );
            st.SetAttribute( "name", "x" );
            string expecting = "int x = 0L;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapMissingDefaultValueIsEmpty()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "w", "L" );
            st.SetAttribute( "type", "double" ); // double not in typeInit map
            st.SetAttribute( "name", "x" );
            string expecting = "double x = ;"; // weird, but tests default value is key
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapHiddenByFormalArg()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "var(typeInit,type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "int" );
            st.SetAttribute( "name", "x" );
            string expecting = "int x = ;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapEmptyValueAndAngleBracketStrings()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":, \"double\":<<0.0L>>] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "float" );
            st.SetAttribute( "name", "x" );
            string expecting = "float x = ;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapDefaultValue()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "UserRecord" );
            st.SetAttribute( "name", "x" );
            string expecting = "UserRecord x = null;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapEmptyDefaultValue()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", default:] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "UserRecord" );
            st.SetAttribute( "name", "x" );
            string expecting = "UserRecord x = ;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapDefaultValueIsKey()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", default:key] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "UserRecord" );
            st.SetAttribute( "name", "x" );
            string expecting = "UserRecord x = UserRecord;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        /**
         * Test that a map can have only the default entry.
         * <p>
         * Bug ref: JIRA bug ST-15 (Fixed)
         */
        [TestMethod]
        public void TestMapDefaultStringAsKey()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"default\":\"foo\"] " + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "var" );
            st.SetAttribute( "type", "default" );
            st.SetAttribute( "name", "x" );
            string expecting = "default x = foo;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        /**
         * Test that a map can return a <b>string</b> with the word: default.
         * <p>
         * Bug ref: JIRA bug ST-15 (Fixed)
         */
        [TestMethod]
        public void TestMapDefaultIsDefaultString()
        {
            string templates =
                    "group test;" + newline +
                    "map ::= [default: \"default\"] " + newline +
                    "t1() ::= \"<map.(1)>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "t1" );
            string expecting = "default";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapViaEnclosingTemplates()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(type,name) ::= \"<var(...)>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate st = group.GetInstanceOf( "intermediate" );
            st.SetAttribute( "type", "int" );
            st.SetAttribute( "name", "x" );
            string expecting = "int x = 0;";
            string result = st.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestMapViaEnclosingTemplates2()
        {
            string templates =
                    "group test;" + newline +
                    "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                    "intermediate(stuff) ::= \"<stuff>\"" + newline +
                    "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate interm = group.GetInstanceOf( "intermediate" );
            StringTemplate var = group.GetInstanceOf( "var" );
            var.SetAttribute( "type", "int" );
            var.SetAttribute( "name", "x" );
            interm.SetAttribute( "stuff", var );
            string expecting = "int x = 0;";
            string result = interm.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyGroupTemplate()
        {
            string templates =
                    "group test;" + newline +
                    "foo() ::= \"\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate a = group.GetInstanceOf( "foo" );
            string expecting = "";
            string result = a.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyStringAndEmptyAnonTemplateAsParameterUsingAngleBracketLexer()
        {
            string templates =
                    "group test;" + newline +
                    "top() ::= <<<x(a=\"\", b={})\\>>>" + newline +
                    "x(a,b) ::= \"a=<a>, b=<b>\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate a = group.GetInstanceOf( "top" );
            string expecting = "a=, b=";
            string result = a.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestEmptyStringAndEmptyAnonTemplateAsParameterUsingDollarLexer()
        {
            string templates =
                    "group test;" + newline +
                    "top() ::= <<$x(a=\"\", b={})$>>" + newline +
                    "x(a,b) ::= \"a=$a$, b=$b$\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate a = group.GetInstanceOf( "top" );
            string expecting = "a=, b=";
            string result = a.ToString();
            Assert.AreEqual( expecting, result );
        }

        /**
         *  FIXME: Dannish does not work if typed directly in with default file
         *  encoding on windows. The character needs to be escaped as bellow.
         *  Please correct to escape the correct charcter.
         */
        [TestMethod]
        public void Test8BitEuroChars()
        {
            StringTemplate e = new StringTemplate(
                    "Danish: \x0143 char"
                );
            e = e.GetInstanceOf();
            string expecting = "Danish: \x0143 char";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void Test16BitUnicodeChar()
        {
            StringTemplate e = new StringTemplate(
                    "DINGBAT CIRCLED SANS-SERIF DIGIT ONE: \x2780"
                );
            e = e.GetInstanceOf();
            string expecting = "DINGBAT CIRCLED SANS-SERIF DIGIT ONE: \x2780";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestFirstOp()
        {
            StringTemplate e = new StringTemplate(
                    "$first(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Ter";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestTruncOp()
        {
            StringTemplate e = new StringTemplate(
                    "$trunc(names); separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Ter, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestRestOp()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names); separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Tom, Sriram";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestRestOpEmptyList()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names); separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", new List<object>() );
            string expecting = "";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestReUseOfRestResult()
        {
            string templates =
                "group test;" + newline +
                "a(names) ::= \"<b(rest(names))>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "a" );
            IList names = new List<object>();
            names.Add( "Ter" );
            names.Add( "Tom" );
            e.SetAttribute( "names", names );
            string expecting = "Tom, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLastOp()
        {
            StringTemplate e = new StringTemplate(
                    "$last(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Sriram";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCombinedOp()
        {
            // replace first of yours with first of mine
            StringTemplate e = new StringTemplate(
                    "$[first(mine),rest(yours)]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "mine", "1" );
            e.SetAttribute( "mine", "2" );
            e.SetAttribute( "mine", "3" );
            e.SetAttribute( "yours", "a" );
            e.SetAttribute( "yours", "b" );
            string expecting = "1, b";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatListAndSingleAttribute()
        {
            // replace first of yours with first of mine
            StringTemplate e = new StringTemplate(
                    "$[mine,yours]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "mine", "1" );
            e.SetAttribute( "mine", "2" );
            e.SetAttribute( "mine", "3" );
            e.SetAttribute( "yours", "a" );
            string expecting = "1, 2, 3, a";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestReUseOfCat()
        {
            string templates =
                "group test;" + newline +
                "a(mine,yours) ::= \"<b([mine,yours])>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "a" );
            IList mine = new List<object>();
            mine.Add( "Ter" );
            mine.Add( "Tom" );
            e.SetAttribute( "mine", mine );
            IList yours = new List<object>();
            yours.Add( "Foo" );
            e.SetAttribute( "yours", yours );
            string expecting = "TerTomFoo, TerTomFoo";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatListAndEmptyAttributes()
        {
            // + is overloaded to be cat strings and cat lists so the
            // two operands (from left to right) determine which way it
            // goes.  In this case, x+mine is a list so everything from their
            // to the right becomes list cat.
            StringTemplate e = new StringTemplate(
                    "$[x,mine,y,yours,z]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "mine", "1" );
            e.SetAttribute( "mine", "2" );
            e.SetAttribute( "mine", "3" );
            e.SetAttribute( "yours", "a" );
            string expecting = "1, 2, 3, a";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestNestedOp()
        {
            StringTemplate e = new StringTemplate(
                    "$first(rest(names))$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestFirstWithOneAttributeOp()
        {
            StringTemplate e = new StringTemplate(
                    "$first(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            string expecting = "Ter";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLastWithOneAttributeOp()
        {
            StringTemplate e = new StringTemplate(
                    "$last(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            string expecting = "Ter";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLastWithLengthOneListAttributeOp()
        {
            StringTemplate e = new StringTemplate(
                    "$last(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", new List<object>( new object[] { "Ter" } ) );
            string expecting = "Ter";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestRestWithOneAttributeOp()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            string expecting = "";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestRestWithLengthOneListAttributeOp()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", new List<object>( new object[] { "Ter" } ) );
            string expecting = "";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestRepeatedRestOp()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names)$, $rest(names)$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "Tom, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        /** If an iterator is sent into ST, it must be cannot be reset after each
         *  use so repeated refs yield empty values.  This would
         *  work if we passed in a List not an iterator.  Avoid sending in iterators
         *  if you ref it twice.
         */
        [TestMethod]
        public void TestRepeatedIteratedAttrFromArg()
        {
            string templates =
                    "group test;" + newline +
                    "root(names) ::= \"$other(names)$\"" + newline +
                    "other(x) ::= \"$x$, $x$\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "root" );
            IList names = new List<object>();
            names.Add( "Ter" );
            names.Add( "Tom" );
            e.SetAttribute( "names", names.iterator() );
            string expecting = "TerTom, ";  // This does not give TerTom twice!!
            Assert.AreEqual( expecting, e.ToString() );
        }

#if false
        /** FIXME: BUG! Iterator is not reset from first to second $x$
         *  Either reset the iterator or pass an attribute that knows to get
         *  the iterator each time.  Seems like first, tail do not
         *  have same problem as they yield objects.
         *
         *  Maybe make a RestIterator like I have CatIterator.
         */
        [TestMethod]
        public void TestRepeatedRestOpAsArg()
        {
            String templates =
                    "group test;" + newline +
                    "root(names) ::= \"$other(rest(names))$\"" + newline +
                    "other(x) ::= \"$x$, $x$\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "root" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            String expecting = "Tom, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }
#endif

        [TestMethod]
        public void TestIncomingLists()
        {
            StringTemplate e = new StringTemplate(
                    "$rest(names)$, $rest(names)$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "Tom, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestIncomingListsAreNotModified()
        {
            StringTemplate e = new StringTemplate(
                    "$names; separator=\", \"$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            IList names = new List<object>();
            names.Add( "Ter" );
            names.Add( "Tom" );
            e.SetAttribute( "names", names );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Ter, Tom, Sriram";
            Assert.AreEqual( expecting, e.ToString() );

            Assert.AreEqual( names.Count, 2 );
        }

        [TestMethod]
        public void TestIncomingListsAreNotModified2()
        {
            StringTemplate e = new StringTemplate(
                    "$names; separator=\", \"$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            IList names = new List<object>();
            names.Add( "Ter" );
            names.Add( "Tom" );
            e.SetAttribute( "names", "Sriram" ); // single element first now
            e.SetAttribute( "names", names );
            string expecting = "Sriram, Ter, Tom";
            Assert.AreEqual( expecting, e.ToString() );

            Assert.AreEqual( names.Count, 2 );
        }

        [TestMethod]
        public void TestIncomingArraysAreOk()
        {
            StringTemplate e = new StringTemplate(
                    "$names; separator=\", \"$" // gets 2nd element
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", new string[] { "Ter", "Tom" } );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "Ter, Tom, Sriram";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestMultipleRefsToListAttribute()
        {
            string templates =
                    "group test;" + newline +
                    "f(x) ::= \"<x> <x>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "f" );
            e.SetAttribute( "x", "Ter" );
            e.SetAttribute( "x", "Tom" );
            string expecting = "TerTom TerTom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestApplyTemplateWithSingleFormalArgs()
        {
            string templates =
                    "group test;" + newline +
                    "test(names) ::= <<<names:bold(item=it); separator=\", \"> >>" + newline +
                    "bold(item) ::= <<*<item>*>>" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "*Ter*, *Tom* ";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestApplyTemplateWithNoFormalArgs()
        {
            string templates =
                    "group test;" + newline +
                    "test(names) ::= <<<names:bold(); separator=\", \"> >>" + newline +
                    "bold() ::= <<*<it>*>>" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "*Ter*, *Tom* ";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestAnonTemplateArgs()
        {
            StringTemplate e = new StringTemplate(
                    "$names:{n| $n$}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "Ter, Tom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestAnonTemplateWithArgHasNoITArg()
        {
            StringTemplate e = new StringTemplate(
                    "$names:{n| $n$:$it$}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string error = null;
            try
            {
                e.ToString();
            }
            catch ( ArgumentException nse )
            {
                error = nse.Message;
            }
            string expecting = "no such attribute: it in template context [anonymous anonymous]";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestAnonTemplateArgs2()
        {
            StringTemplate e = new StringTemplate(
                    "$names:{n| .$n$.}:{ n | _$n$_}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "_.Ter._, _.Tom._";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestFirstWithCatAttribute()
        {
            StringTemplate e = new StringTemplate(
                    "$first([names,phones])$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "Ter";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestFirstWithListOfMaps()
        {
            StringTemplate e = new StringTemplate(
                    "$first(maps).Ter$"
                );
            e = e.GetInstanceOf();
            IDictionary m1 = new Dictionary<object, object>();
            IDictionary m2 = new Dictionary<object, object>();
            m1["Ter"] = "x5707";
            e.SetAttribute( "maps", m1 );
            m2["Tom"] = "x5332";
            e.SetAttribute( "maps", m2 );
            string expecting = "x5707";
            Assert.AreEqual( expecting, e.ToString() );

            e = e.GetInstanceOf();
            IList list = new List<object>( new object[] { m1, m2 } );
            e.SetAttribute( "maps", list );
            expecting = "x5707";
            Assert.AreEqual( expecting, e.ToString() );
        }

#if false
        // this FAILS!
        [TestMethod]
        public void TestFirstWithListOfMaps2()
        {
            StringTemplate e = new StringTemplate(
                    "$first(maps):{ m | $m.Ter$ }$"
                );
            IDictionary m1 = new Dictionary<object, object>();
            IDictionary m2 = new Dictionary<object, object>();
            m1.Add( "Ter", "x5707" );
            e.SetAttribute( "maps", m1 );
            m2.Add( "Tom", "x5332" );
            e.SetAttribute( "maps", m2 );
            string expecting = "x5707";
            Assert.AreEqual( expecting, e.ToString() );

            e = e.GetInstanceOf();
            IList list = new List<object>() { m1, m2 };
            e.SetAttribute( "maps", list );
            expecting = "x5707";
            Assert.AreEqual( expecting, e.ToString() );
        }
#endif

        [TestMethod]
        public void TestJustCat()
        {
            StringTemplate e = new StringTemplate(
                    "$[names,phones]$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "TerTom12";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCat2Attributes()
        {
            StringTemplate e = new StringTemplate(
                    "$[names,phones]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "Ter, Tom, 1, 2";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCat2AttributesWithApply()
        {
            StringTemplate e = new StringTemplate(
                    "$[names,phones]:{a|$a$.}$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "Ter.Tom.1.2.";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCat3Attributes()
        {
            StringTemplate e = new StringTemplate(
                    "$[names,phones,salaries]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            e.SetAttribute( "salaries", "huge" );
            string expecting = "Ter, Tom, 1, 2, big, huge";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatWithTemplateApplicationAsElement()
        {
            StringTemplate e = new StringTemplate(
                    "$[names:{$it$!},phones]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "Ter!, Tom!, 1, 2";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatWithIFAsElement()
        {
            StringTemplate e = new StringTemplate(
                    "$[{$if(names)$doh$endif$},phones]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "doh, 1, 2";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatWithNullTemplateApplicationAsElement()
        {
            StringTemplate e = new StringTemplate(
                    "$[names:{$it$!},\"foo\"]:{x}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "x";  // only one since template application gives nothing
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestCatWithNestedTemplateApplicationAsElement()
        {
            StringTemplate e = new StringTemplate(
                    "$[names, [\"foo\",\"bar\"]:{$it$!},phones]; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "Ter, Tom, foo!, bar!, 1, 2";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestListAsTemplateArgument()
        {
            string templates =
                    "group test;" + newline +
                    "test(names,phones) ::= \"<foo([names,phones])>\"" + newline +
                    "foo(items) ::= \"<items:{a | *<a>*}>\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            string expecting = "*Ter**Tom**1**2*";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSingleExprTemplateArgument()
        {
            string templates =
                    "group test;" + newline +
                    "test(name) ::= \"<bold(name)>\"" + newline +
                    "bold(item) ::= \"*<item>*\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "name", "Ter" );
            string expecting = "*Ter*";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSingleExprTemplateArgumentInApply()
        {
            // when you specify a single arg on a template application
            // it overrides the setting of the iterated value "it" to that
            // same single formal arg.  Your arg hides the implicitly set "it".
            string templates =
                    "group test;" + newline +
                    "test(names,x) ::= \"<names:bold(x)>\"" + newline +
                    "bold(item) ::= \"*<item>*\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "x", "ick" );
            string expecting = "*ick**ick*";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSoleFormalTemplateArgumentInMultiApply()
        {
            string templates =
                    "group test;" + newline +
                    "test(names) ::= \"<names:bold(),italics()>\"" + newline +
                    "bold(x) ::= \"*<x>*\"" + newline +
                    "italics(y) ::= \"_<y>_\"" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            string expecting = "*Ter*_Tom_";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestSingleExprTemplateArgumentError()
        {
            string templates =
                    "group test;" + newline +
                    "test(name) ::= \"<bold(name)>\"" + newline +
                    "bold(item,ick) ::= \"*<item>*\"" + newline
                    ;
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                            typeof( AngleBracketTemplateLexer ), errors );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "name", "Ter" );
            string result = e.ToString();
            string expecting = "template bold must have exactly one formal arg in template context [test <invoke bold arg context>]";
            Assert.AreEqual( expecting, errors.ToString() );
        }

        [TestMethod]
        public void TestInvokeIndirectTemplateWithSingleFormalArgs()
        {
            string templates =
                    "group test;" + newline +
                    "test(templateName,arg) ::= \"<(templateName)(arg)>\"" + newline +
                    "bold(x) ::= <<*<x>*>>" + newline +
                    "italics(y) ::= <<_<y>_>>" + newline
                    ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "test" );
            e.SetAttribute( "templateName", "italics" );
            e.SetAttribute( "arg", "Ter" );
            string expecting = "_Ter_";
            string result = e.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestParallelAttributeIteration()
        {
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p,s | $n$@$p$: $s$\n}$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            e.SetAttribute( "salaries", "huge" );
            string expecting = "Ter@1: big" + newline + "Tom@2: huge" + newline;
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithNullValue()
        {
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p,s | $n$@$p$: $s$\n}$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            e.SetAttribute( "phones", new List<object>( new object[] { "1", null, "3" } ) );
            e.SetAttribute( "salaries", "big" );
            e.SetAttribute( "salaries", "huge" );
            e.SetAttribute( "salaries", "enormous" );
            string expecting = "Ter@1: big" + newline +
                               "Tom@: huge" + newline +
                               "Sriram@3: enormous" + newline;
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationHasI()
        {
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p,s | $i0$. $n$@$p$: $s$\n}$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            e.SetAttribute( "salaries", "huge" );
            string expecting = "0. Ter@1: big" + newline + "1. Tom@2: huge" + newline;
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizes()
        {
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p,s | $n$@$p$: $s$}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            string expecting = "Ter@1: big, Tom@2: , Sriram@: ";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithSingletons()
        {
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p,s | $n$@$p$: $s$}; separator=\", \"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "salaries", "big" );
            string expecting = "Ter@1: big";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithMismatchArgListSizes()
        {
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{n,p | $n$@$p$}; separator=\", \"$"
                );
            e.ErrorListener = errors;
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            string expecting = "Ter@1, Tom@2";
            Assert.AreEqual( expecting, e.ToString() );
            string errorExpecting = "number of arguments [n, p] mismatch between attribute list and anonymous template in context [anonymous]";
            Assert.AreEqual( errorExpecting, errors.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithMissingArgs()
        {
            IStringTemplateErrorListener errors = new ErrorBuffer();
            StringTemplate e = new StringTemplate(
                    "$names,phones,salaries:{$n$@$p$}; separator=\", \"$"
                );
            e.ErrorListener = errors;
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "phones", "2" );
            e.SetAttribute( "salaries", "big" );
            e.ToString(); // generate the error
            string errorExpecting = "missing arguments in anonymous template in context [anonymous]";
            Assert.AreEqual( errorExpecting, errors.ToString() );
        }

        [TestMethod]
        public void TestParallelAttributeIterationWithDifferentSizesTemplateRefInsideToo()
        {
            string templates =
                    "group test;" + newline +
                    "page(names,phones,salaries) ::= " + newline +
                    "	<<$names,phones,salaries:{n,p,s | $value(n)$@$value(p)$: $value(s)$}; separator=\", \"$>>" + newline +
                    "value(x=\"n/a\") ::= \"$x$\"" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ),
                                            typeof( DefaultTemplateLexer ) );
            StringTemplate p = group.GetInstanceOf( "page" );
            p.SetAttribute( "names", "Ter" );
            p.SetAttribute( "names", "Tom" );
            p.SetAttribute( "names", "Sriram" );
            p.SetAttribute( "phones", "1" );
            p.SetAttribute( "phones", "2" );
            p.SetAttribute( "salaries", "big" );
            string expecting = "Ter@1: big, Tom@2: n/a, Sriram@n/a: n/a";
            Assert.AreEqual( expecting, p.ToString() );
        }

        [TestMethod]
        public void TestAnonTemplateOnLeftOfApply()
        {
            StringTemplate e = new StringTemplate(
                    "${foo}:{($it$)}$"
                );
            string expecting = "(foo)";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestOverrideThroughConditional()
        {
            string templates =
                "group base;" + newline +
                "body(ick) ::= \"<if(ick)>ick<f()><else><f()><endif>\"" +
                "f() ::= \"foo\"" + newline
                ;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );
            string templates2 =
                    "group sub;" + newline +
                    "f() ::= \"bar\"" + newline
                ;
            StringTemplateGroup subgroup =
                new StringTemplateGroup( new StringReader( templates2 ),
                                        typeof( AngleBracketTemplateLexer ),
                                        null,
                                        group );

            StringTemplate b = subgroup.GetInstanceOf( "body" );
            string expecting = "bar";
            string result = b.ToString();
            Assert.AreEqual( expecting, result );
        }

        public class NonPublicProperty
        {
            public int foo = 9;
            public int getBar()
            {
                return 34;
            }
        }

        [TestMethod]
        public void TestNonPublicPropertyAccess()
        {
            StringTemplate st =
                    new StringTemplate( "$x.foo$:$x.bar$" );
            object o = new NonPublicProperty();

            st.SetAttribute( "x", o );
            string expecting = "9:34";
            Assert.AreEqual( expecting, st.ToString() );
        }

        [TestMethod]
        public void TestIndexVar()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A:{$i$. $it$}; separator=\"\\n\"$"
                    );
            t.SetAttribute( "A", "parrt" );
            t.SetAttribute( "A", "tombu" );
            string expecting =
                "1. parrt" + newline +
                "2. tombu";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndex0Var()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A:{$i0$. $it$}; separator=\"\\n\"$"
                    );
            t.SetAttribute( "A", "parrt" );
            t.SetAttribute( "A", "tombu" );
            string expecting =
                "0. parrt" + newline +
                "1. tombu";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndexVarWithMultipleExprs()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A,B:{a,b|$i$. $a$@$b$}; separator=\"\\n\"$"
                    );
            t.SetAttribute( "A", "parrt" );
            t.SetAttribute( "A", "tombu" );
            t.SetAttribute( "B", "x5707" );
            t.SetAttribute( "B", "x5000" );
            string expecting =
                "1. parrt@x5707" + newline +
                "2. tombu@x5000";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestIndex0VarWithMultipleExprs()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "dummy", "." );
            StringTemplate t =
                    new StringTemplate(
                            group,
                            "$A,B:{a,b|$i0$. $a$@$b$}; separator=\"\\n\"$"
                    );
            t.SetAttribute( "A", "parrt" );
            t.SetAttribute( "A", "tombu" );
            t.SetAttribute( "B", "x5707" );
            t.SetAttribute( "B", "x5000" );
            string expecting =
                "0. parrt@x5707" + newline +
                "1. tombu@x5000";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestArgumentContext()
        {
            // t is referenced within foo and so will be evaluated in that
            // context.  it can therefore see name.
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate main = group.DefineTemplate( "main", "$foo(t={Hi, $name$}, name=\"parrt\")$" );
            StringTemplate foo = group.DefineTemplate( "foo", "$t$" );
            string expecting = "Hi, parrt";
            Assert.AreEqual( expecting, main.ToString() );
        }

        [TestMethod]
        public void TestNoDotsInAttributeNames()
        {
            StringTemplateGroup group = new StringTemplateGroup( "dummy", "." );
            StringTemplate t = new StringTemplate( group, "$user.Name$" );
            string error = null;
            try
            {
                t.SetAttribute( "user.Name", "Kunle" );
            }
            catch ( ArgumentException e )
            {
                error = e.Message;
            }
            string expecting = "cannot have '.' in attribute names";
            Assert.AreEqual( expecting, error );
        }

        [TestMethod]
        public void TestNoDotsInTemplateNames()
        {
            IStringTemplateErrorListener errors = new ErrorBuffer();
            string templates =
                    "group test;" + newline +
                    "a.b() ::= <<foo>>" + newline;

            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates ),
                                        typeof( DefaultTemplateLexer ),
                                        errors );
            //string expecting = "template group parse error: line 2:1: unexpected token:";
            string expecting = "template group parse error: line 2:1: required (...)+ loop did not match anything at input 'a':";
            Assert.IsTrue( errors.ToString().StartsWith( expecting ) );
        }

        [TestMethod]
        public void TestLineWrap()
        {
            string templates =
                    "group test;" + newline +
                    "array(values) ::= <<int[] a = { <values; wrap=\"\\n\", separator=\",\"> };>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                            4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
                            3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5} );
            string expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + newline +
                "2,1,6,32,5,6,77,4,9,20,2,1,4,63,9,20,2,1," + newline +
                "4,6,32,5,6,77,6,32,5,6,77,3,9,20,2,1,4,6," + newline +
                "32,5,6,77,888,1,6,32,5 };";
            Assert.AreEqual( expecting, a.ToString( 40 ) );
        }

        [TestMethod]
        public void TestLineWrapWithNormalizedNewlines()
        {
            string templates =
                    "group test;" + newline +
                    "array(values) ::= <<int[] a = { <values; wrap=\"\\r\\n\", separator=\",\"> };>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                            4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
                            3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5} );
            string expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888,\n" + // wrap is \r\n, normalize to \n
                "2,1,6,32,5,6,77,4,9,20,2,1,4,63,9,20,2,1,\n" +
                "4,6,32,5,6,77,6,32,5,6,77,3,9,20,2,1,4,6,\n" +
                "32,5,6,77,888,1,6,32,5 };";

            StringWriter sw = new StringWriter();
            IStringTemplateWriter stw = new AutoIndentWriter( sw, "\n" ); // force \n as newline
            stw.SetLineWidth( 40 );
            a.Write( stw );
            string result = sw.ToString();
            Assert.AreEqual( expecting, result );
        }

        [TestMethod]
        public void TestLineWrapAnchored()
        {
            string templates =
                    "group test;" + newline +
                    "array(values) ::= <<int[] a = { <values; anchor, wrap=\"\\n\", separator=\",\"> };>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                            4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
                            3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5} );
            string expecting =
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + newline +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + newline +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + newline +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + newline +
                "            5,6,77,888,1,6,32,5 };";
            Assert.AreEqual( expecting, a.ToString( 40 ) );
        }

        [TestMethod]
        public void TestSubtemplatesAnchorToo()
        {
            String templates =
                    "group test;" + newline +
                    "array(values) ::= <<{ <values; anchor, separator=\", \"> }>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate x = new StringTemplate( group, "<\\n>{ <stuff; anchor, separator=\",\\n\"> }<\\n>" );
            x.SetAttribute( "stuff", "1" );
            x.SetAttribute( "stuff", "2" );
            x.SetAttribute( "stuff", "3" );
            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values", new object[] { "a", x, "b" } );
            String expecting =
                "{ a, " + newline +
                "  { 1," + newline +
                "    2," + newline +
                "    3 }" + newline +
                "  , b }";
            Assert.AreEqual( expecting, a.ToString( 40 ) );
        }

        [TestMethod]
        public void TestFortranLineWrap()
        {
            string templates =
                    "group test;" + newline +
                    "func(args) ::= <<       FUNCTION line( <args; wrap=\"\\n      c\", separator=\",\"> )>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "func" );
            a.SetAttribute( "args",
                           new string[] { "a", "b", "c", "d", "e", "f" } );
            string expecting =
                "       FUNCTION line( a,b,c,d," + newline +
                "      ce,f )";
            Assert.AreEqual( expecting, a.ToString( 30 ) );
        }

        [TestMethod]
        public void TestLineWrapWithDiffAnchor()
        {
            string templates =
                    "group test;" + newline +
                    "array(values) ::= <<int[] a = { <{1,9,2,<values; wrap, separator=\",\">}; anchor> };>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                            4,9,20,2,1,4,63,9,20,2,1,4,6} );
            string expecting =
                "int[] a = { 1,9,2,3,9,20,2,1,4," + newline +
                "            6,32,5,6,77,888,2," + newline +
                "            1,6,32,5,6,77,4,9," + newline +
                "            20,2,1,4,63,9,20,2," + newline +
                "            1,4,6 };";
            Assert.AreEqual( expecting, a.ToString( 30 ) );
        }

        [TestMethod]
        public void TestLineWrapEdgeCase()
        {
            string templates =
                    "group test;" + newline +
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "chars", new string[] { "a", "b", "c", "d", "e" } );
            // lineWidth==3 implies that we can have 3 characters at most
            string expecting =
                "abc" + newline +
                "de";
            Assert.AreEqual( expecting, a.ToString( 3 ) );
        }

        [TestMethod]
        public void TestLineWrapLastCharIsNewline()
        {
            string templates =
                    "group test;" + newline +
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "chars", new string[] { "a", "b", "\n", "d", "e" } );
            // don't do \n if it's last element anyway
            string expecting =
                "ab" + newline +
                "de";
            Assert.AreEqual( expecting, a.ToString( 3 ) );
        }

        [TestMethod]
        public void TestLineWrapCharAfterWrapIsNewline()
        {
            string templates =
                    "group test;" + newline +
                    "duh(chars) ::= <<<chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "chars", new string[] { "a", "b", "c", "\n", "d", "e" } );
            // Once we wrap, we must dump chars as we see them.  A newline right
            // after a wrap is just an "unfortunate" event.  People will expect
            // a newline if it's in the data.
            string expecting =
                "abc" + newline +
                "" + newline +
                "de";
            Assert.AreEqual( expecting, a.ToString( 3 ) );
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplate()
        {
            string templates =
                    "group test;" + newline +
                    "duh(data) ::= <<!<data:{v|[<v>]}; wrap>!>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            string expecting =
                "![1][2][3]" + newline + // width=9 is the 3 char; don't break til after ]
                "[4][5][6]" + newline +
                "[7][8][9]!";
            Assert.AreEqual( expecting, a.ToString( 9 ) );
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateAnchored()
        {
            string templates =
                    "group test;" + newline +
                    "duh(data) ::= <<!<data:{v|[<v>]}; anchor, wrap>!>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            string expecting =
                "![1][2][3]" + newline +
                " [4][5][6]" + newline +
                " [7][8][9]!";
            Assert.AreEqual( expecting, a.ToString( 9 ) );
        }

        [TestMethod]
        public void TestLineWrapForAnonTemplateComplicatedWrap()
        {
            string templates =
                    "group test;" + newline +
                    "top(s) ::= <<  <s>.>>" +
                    "str(data) ::= <<!<data:{v|[<v>]}; wrap=\"!+\\n!\">!>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate t = group.GetInstanceOf( "top" );
            StringTemplate s = group.GetInstanceOf( "str" );
            s.SetAttribute( "data", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            t.SetAttribute( "s", s );
            string expecting =
                "  ![1][2]!+" + newline +
                "  ![3][4]!+" + newline +
                "  ![5][6]!+" + newline +
                "  ![7][8]!+" + newline +
                "  ![9]!.";
            Assert.AreEqual( expecting, t.ToString( 9 ) );
        }

        [TestMethod]
        public void TestIndentBeyondLineWidth()
        {
            string templates =
                    "group test;" + newline +
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "chars", new string[] { "a", "b", "c", "d", "e" } );
            //
            string expecting =
                "    a" + newline +
                "    b" + newline +
                "    c" + newline +
                "    d" + newline +
                "    e";
            Assert.AreEqual( expecting, a.ToString( 2 ) );
        }

        [TestMethod]
        public void TestIndentedExpr()
        {
            string templates =
                    "group test;" + newline +
                    "duh(chars) ::= <<    <chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "duh" );
            a.SetAttribute( "chars", new string[] { "a", "b", "c", "d", "e" } );
            //
            string expecting =
                "    ab" + newline +
                "    cd" + newline +
                "    e";
            // width=4 spaces + 2 char.
            Assert.AreEqual( expecting, a.ToString( 6 ) );
        }

        [TestMethod]
        public void TestNestedIndentedExpr()
        {
            string templates =
                    "group test;" + newline +
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<  <chars; wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate top = group.GetInstanceOf( "top" );
            StringTemplate duh = group.GetInstanceOf( "duh" );
            duh.SetAttribute( "chars", new string[] { "a", "b", "c", "d", "e" } );
            top.SetAttribute( "d", duh );
            string expecting =
                "    ab" + newline +
                "    cd" + newline +
                "    e!";
            // width=4 spaces + 2 char.
            Assert.AreEqual( expecting, top.ToString( 6 ) );
        }

        [TestMethod]
        public void TestNestedWithIndentAndTrackStartOfExpr()
        {
            string templates =
                    "group test;" + newline +
                    "top(d) ::= <<  <d>!>>" + newline +
                    "duh(chars) ::= <<x: <chars; anchor, wrap=\"\\n\"\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate top = group.GetInstanceOf( "top" );
            StringTemplate duh = group.GetInstanceOf( "duh" );
            duh.SetAttribute( "chars", new string[] { "a", "b", "c", "d", "e" } );
            top.SetAttribute( "d", duh );

            string expecting =
                "  x: ab" + newline +
                "     cd" + newline +
                "     e!";
            Assert.AreEqual( expecting, top.ToString( 7 ) );
        }

        [TestMethod]
        public void TestLineDoesNotWrapDueToLiteral()
        {
            string templates =
                    "group test;" + newline +
                    "m(args,body) ::= <<public void foo(<args; wrap=\"\\n\",separator=\", \">) throws Ick { <body> }>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate a = group.GetInstanceOf( "m" );
            a.SetAttribute( "args",
                           new string[] { "a", "b", "c" } );
            a.SetAttribute( "body", "i=3;" );
            // make it wrap because of ") throws Ick { " literal
            int n = "public void foo(a, b, c".Length;
            string expecting =
                "public void foo(a, b, c) throws Ick { i=3; }";
            Assert.AreEqual( expecting, a.ToString( n ) );
        }

        [TestMethod]
        public void TestSingleValueWrap()
        {
            string templates =
                    "group test;" + newline +
                    "m(args,body) ::= <<{ <body; anchor, wrap=\"\\n\"> }>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate m = group.GetInstanceOf( "m" );
            m.SetAttribute( "body", "i=3;" );
            // make it wrap because of ") throws Ick { " literal
            string expecting =
                "{ " + newline +
                "  i=3; }";
            Assert.AreEqual( expecting, m.ToString( 2 ) );
        }

        [TestMethod]
        public void TestLineWrapInNestedExpr()
        {
            string templates =
                    "group test;" + newline +
                    "top(arrays) ::= <<Arrays: <arrays>done>>" + newline +
                    "array(values) ::= <<int[] a = { <values; anchor, wrap=\"\\n\", separator=\",\"> };<\\n\\>>>" + newline;
            StringTemplateGroup group =
                    new StringTemplateGroup( new StringReader( templates ) );

            StringTemplate top = group.GetInstanceOf( "top" );
            StringTemplate a = group.GetInstanceOf( "array" );
            a.SetAttribute( "values",
                           new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                            4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
                            3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5} );
            top.SetAttribute( "arrays", a );
            top.SetAttribute( "arrays", a ); // add twice
            string expecting =
                "Arrays: int[] a = { 3,9,20,2,1,4,6,32,5," + newline +
                "                    6,77,888,2,1,6,32,5," + newline +
                "                    6,77,4,9,20,2,1,4,63," + newline +
                "                    9,20,2,1,4,6,32,5,6," + newline +
                "                    77,6,32,5,6,77,3,9,20," + newline +
                "                    2,1,4,6,32,5,6,77,888," + newline +
                "                    1,6,32,5 };" + newline +
                "int[] a = { 3,9,20,2,1,4,6,32,5,6,77,888," + newline +
                "            2,1,6,32,5,6,77,4,9,20,2,1,4," + newline +
                "            63,9,20,2,1,4,6,32,5,6,77,6," + newline +
                "            32,5,6,77,3,9,20,2,1,4,6,32," + newline +
                "            5,6,77,888,1,6,32,5 };" + newline +
                "done";
            Assert.AreEqual( expecting, top.ToString( 40 ) );
        }

        [TestMethod]
        public void TestBackslash()
        {
            StringTemplateGroup group = new StringTemplateGroup("test");
            StringTemplate t = group.DefineTemplate("t", "\\");
            string expecting = "\\";
            Assert.AreEqual(expecting, t.ToString());
        }

        [TestMethod]
        public void TestBackslash2()
        {
            StringTemplateGroup group = new StringTemplateGroup("test");
            StringTemplate t = group.DefineTemplate("t", "\\ ");
            string expecting = "\\ ";
            Assert.AreEqual(expecting, t.ToString());
        }

        [TestMethod]
        public void TestEscapeEscape()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate t = group.DefineTemplate( "t", "\\\\$v$" );
            t.SetAttribute( "v", "Joe" );
            //System.out.println(t);
            string expecting = "\\Joe";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestEscapeEscapeNestedAngle()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t = group.DefineTemplate( "t", "<v:{a|\\\\<a>}>" );
            t.SetAttribute( "v", "Joe" );
            //System.out.println(t);
            string expecting = "\\Joe";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestListOfIntArrays()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data:array()>" );
            group.DefineTemplate( "array", "[<it:element(); separator=\",\">]" );
            group.DefineTemplate( "element", "<it>" );
            IList data = new List<object>();
            data.Add( new int[] { 1, 2, 3 } );
            data.Add( new int[] { 10, 20, 30 } );
            t.SetAttribute( "data", data );
            //System.out.println(t);
            string expecting = "[1,2,3][10,20,30]";
            Assert.AreEqual( expecting, t.ToString() );
        }

        // Test null option

        [TestMethod]
        public void TestNullOptionSingleNullValue()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data; null=\"0\">" );
            //System.out.println(t);
            string expecting = "0";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullOptionHasEmptyNullValue()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data; null=\"\", separator=\", \">" );
            IList data = new List<object>();
            data.Add( null );
            data.Add( 1 );
            t.SetAttribute( "data", data );
            string expecting = ", 1";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullOptionSingleNullValueInList()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data; null=\"0\">" );
            IList data = new List<object>();
            data.Add( null );
            t.SetAttribute( "data", data );
            //System.out.println(t);
            string expecting = "0";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullValueInList()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data; null=\"-1\", separator=\", \">" );
            IList data = new List<object>();
            data.Add( null );
            data.Add( 1 );
            data.Add( null );
            data.Add( 3 );
            data.Add( 4 );
            data.Add( null );
            t.SetAttribute( "data", data );
            //System.out.println(t);
            string expecting = "-1, 1, -1, 3, 4, -1";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullValueInListNoNullOption()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data; separator=\", \">" );
            IList data = new List<object>();
            data.Add( null );
            data.Add( 1 );
            data.Add( null );
            data.Add( 3 );
            data.Add( 4 );
            data.Add( null );
            t.SetAttribute( "data", data );
            //System.out.println(t);
            string expecting = "1, 3, 4";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullValueInListWithTemplateApply()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data:array(); null=\"-1\", separator=\", \">" );
            group.DefineTemplate( "array", "<it>" );
            IList data = new List<object>();
            data.Add( 0 );
            data.Add( null );
            data.Add( 2 );
            data.Add( null );
            t.SetAttribute( "data", data );
            string expecting = "0, -1, 2, -1";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullValueInListWithTemplateApplyNullFirstValue()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data:array(); null=\"-1\", separator=\", \">" );
            group.DefineTemplate( "array", "<it>" );
            IList data = new List<object>();
            data.Add( null );
            data.Add( 0 );
            data.Add( null );
            data.Add( 2 );
            t.SetAttribute( "data", data );
            string expecting = "-1, 0, -1, 2";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullSingleValueInListWithTemplateApply()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data:array(); null=\"-1\", separator=\", \">" );
            group.DefineTemplate( "array", "<it>" );
            IList data = new List<object>();
            data.Add( null );
            t.SetAttribute( "data", data );
            string expecting = "-1";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestNullSingleValueWithTemplateApply()
        {
            StringTemplateGroup group =
                    new StringTemplateGroup( "test", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                group.DefineTemplate( "t", "<data:array(); null=\"-1\", separator=\", \">" );
            group.DefineTemplate( "array", "<it>" );
            string expecting = "-1";
            Assert.AreEqual( expecting, t.ToString() );
        }

        [TestMethod]
        public void TestLengthOp()
        {
            StringTemplate e = new StringTemplate(
                    "$length(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "names", "Tom" );
            e.SetAttribute( "names", "Sriram" );
            string expecting = "3";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpWithMap()
        {
            StringTemplate e = new StringTemplate(
                    "$length(names)$"
                );
            e = e.GetInstanceOf();
            IDictionary m = new Dictionary<object, object>();
            m["Tom"] = "foo";
            m["Sriram"] = "foo";
            m["Doug"] = "foo";
            e.SetAttribute( "names", m );
            string expecting = "3";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpWithSet()
        {
            StringTemplate e = new StringTemplate(
                    "$length(names)$"
                );
            e = e.GetInstanceOf();
            HashSet<object> m = new HashSet<object>();
            m.Add( "Tom" );
            m.Add( "Sriram" );
            m.Add( "Doug" );
            e.SetAttribute( "names", m );
            string expecting = "3";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpNull()
        {
            StringTemplate e = new StringTemplate(
                    "$length(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", null );
            string expecting = "0";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpSingleValue()
        {
            StringTemplate e = new StringTemplate(
                    "$length(names)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            string expecting = "1";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpPrimitive()
        {
            StringTemplate e = new StringTemplate(
                    "$length(ints)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "ints", new int[] { 1, 2, 3, 4 } );
            string expecting = "4";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpOfListWithNulls()
        {
            StringTemplate e = new StringTemplate(
                    "$length(data)$"
                );
            e = e.GetInstanceOf();
            IList data = new List<object>();
            data.Add( "Hi" );
            data.Add( null );
            data.Add( "mom" );
            data.Add( null );
            e.SetAttribute( "data", data );
            string expecting = "4"; // nulls are counted
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestStripOpOfListWithNulls()
        {
            StringTemplate e = new StringTemplate(
                    "$strip(data)$"
                );
            e = e.GetInstanceOf();
            IList data = new List<object>();
            data.Add( "Hi" );
            data.Add( null );
            data.Add( "mom" );
            data.Add( null );
            e.SetAttribute( "data", data );
            string expecting = "Himom"; // nulls are skipped
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestStripOpOfListOfListsWithNulls()
        {
            StringTemplate e = new StringTemplate(
                    "$strip(data):{list | $strip(list)$}; separator=\",\"$"
                );
            e = e.GetInstanceOf();
            IList data = new List<object>();
            IList dataOne = new List<object>();
            dataOne.Add( "Hi" );
            dataOne.Add( "mom" );
            data.Add( dataOne );
            data.Add( null );
            IList dataTwo = new List<object>();
            dataTwo.Add( "Hi" );
            dataTwo.Add( null );
            dataTwo.Add( "dad" );
            dataTwo.Add( null );
            data.Add( dataTwo );
            e.SetAttribute( "data", data );
            string expecting = "Himom,Hidad"; // nulls are skipped
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestStripOpOfSingleAlt()
        {
            StringTemplate e = new StringTemplate(
                    "$strip(data)$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "data", "hi" );
            string expecting = "hi"; // nulls are skipped
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestStripOpOfNull()
        {
            StringTemplate e = new StringTemplate(
                    "$strip(data)$"
                );
            e = e.GetInstanceOf();
            string expecting = ""; // nulls are skipped
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestReUseOfStripResult()
        {
            string templates =
                "group test;" + newline +
                "a(names) ::= \"<b(strip(names))>\"" + newline +
                "b(x) ::= \"<x>, <x>\"" + newline
                ;
            StringTemplateGroup group =
                new StringTemplateGroup( new StringReader( templates ) );
            StringTemplate e = group.GetInstanceOf( "a" );
            IList names = new List<object>();
            names.Add( "Ter" );
            names.Add( null );
            names.Add( "Tom" );
            e.SetAttribute( "names", names );
            string expecting = "TerTom, TerTom";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpOfStrippedListWithNulls()
        {
            StringTemplate e = new StringTemplate(
                    "$length(strip(data))$"
                );
            e = e.GetInstanceOf();
            IList data = new List<object>();
            data.Add( "Hi" );
            data.Add( null );
            data.Add( "mom" );
            data.Add( null );
            e.SetAttribute( "data", data );
            string expecting = "2"; // nulls are counted
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestLengthOpOfStrippedListWithNullsFrontAndBack()
        {
            StringTemplate e = new StringTemplate(
                    "$length(strip(data))$"
                );
            e = e.GetInstanceOf();
            IList data = new List<object>();
            data.Add( null );
            data.Add( null );
            data.Add( null );
            data.Add( "Hi" );
            data.Add( null );
            data.Add( null );
            data.Add( null );
            data.Add( "mom" );
            data.Add( null );
            data.Add( null );
            data.Add( null );
            e.SetAttribute( "data", data );
            string expecting = "2"; // nulls are counted
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestMapKeys()
        {
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                new StringTemplate( group,
                    "<aMap.keys:{k|<k>:<aMap.(k)>}; separator=\", \">" );
            IDictionary map = new SortedList<object, object>();
            map["int"] = "0";
            map["float"] = "0.0";
            t.SetAttribute( "aMap", map );
            // either order of enumerating the dictionary is allowed
            string result = t.ToString();
            if ( result.StartsWith( "int" ) )
                Assert.AreEqual( "int:0, float:0.0", t.ToString() );
            else
                Assert.AreEqual( "float:0.0, int:0", t.ToString() );
        }

        [TestMethod]
        public void TestMapValues()
        {
            StringTemplateGroup group =
                new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                new StringTemplate( group,
                    "<aMap.values; separator=\", \"> <aMap.(\"i\"+\"nt\")>" );
            IDictionary map = new SortedList<object, object>();
            map["int"] = "0";
            map["float"] = "0.0";
            t.SetAttribute( "aMap", map );

            // either order of enumerating the dictionary is allowed
            string result = t.ToString();
            if ( result == "0, 0.0 0" )
                Assert.AreEqual( "0, 0.0 0", t.ToString() );
            else
                Assert.AreEqual( "0.0, 0 0", t.ToString() );
        }

        [TestMethod]
        public void TestMapKeysWithIntegerType()
        {
            // must get back an Integer from keys not a toString()'d version
            StringTemplateGroup group = new StringTemplateGroup( "dummy", ".", typeof( AngleBracketTemplateLexer ) );
            StringTemplate t =
                new StringTemplate( group,
                    "<aMap.keys:{k|<k>:<aMap.(k)>}; separator=\", \">" );
            IDictionary map = new Dictionary<object, object>();
            map.Add( 1, new List<object>( new object[] { "ick", "foo" } ) );
            map.Add( 2, new List<object>( new object[] { "x", "y" } ) );
            t.SetAttribute( "aMap", map );
            string result = t.ToString();
            if ( result.StartsWith( "1" ) )
                Assert.AreEqual( "1:ickfoo, 2:xy", t.ToString() );
            else
                Assert.AreEqual( "2:xy, 1:ickfoo", t.ToString() );
        }

#if false
        /** Use when super.attr name is implemented
         */
        [TestMethod]
        public void TestArgumentContext2()
        {
            // t is referenced within foo and so will be evaluated in that
            // context.  it can therefore see name.
            StringTemplateGroup group =
                    new StringTemplateGroup( "test" );
            StringTemplate main = group.DefineTemplate( "main", "$foo(t={Hi, $super.name$}, name=\"parrt\")$" );
            main.SetAttribute( "name", "tombu" );
            StringTemplate foo = group.DefineTemplate( "foo", "$t$" );
            string expecting = "Hi, parrt";
            Assert.AreEqual( expecting, main.ToString() );
        }
#endif

#if false
        /**
         * Check what happens when a semicolon is  appended to a single line template
         * Should fail with a parse error(?) and not a missing template error.
         * FIXME: This should generate a warning or error about that semi colon.
         * <p>
         * Bug ref: JIRA bug ST-2
         */
        [TestMethod]
        public void TestGroupTrailingSemiColon()
        {
            //try {
                string templates =
                        "group test;" + newline +
                        "t1()::=\"R1\"; " + newline +
                        "t2() ::= \"R2\"" + newline
                        ;
                StringTemplateGroup group =
                        new StringTemplateGroup( new StringReader( templates ) );

                StringTemplate st = group.GetInstanceOf( "t1" );
                Assert.AreEqual( "R1", st.ToString() );

                st = group.GetInstanceOf( "t2" );
                Assert.AreEqual( "R2", st.ToString() );

                Assert.Fail( "A parse error should have been generated" );
            //} catch (ParseError??) {            
            //}
        }
#endif

        [TestMethod]
        public void TestSuperReferenceInIfClause()
        {
            string superGroupString =
                "group super;" + newline +
                "a(x) ::= \"super.a\"" + newline +
                "b(x) ::= \"<c()>super.b\"" + newline +
                "c() ::= \"super.c\""
                ;
            StringTemplateGroup superGroup = new StringTemplateGroup(
                new StringReader( superGroupString ), typeof( AngleBracketTemplateLexer ) );
            string subGroupString =
                "group sub;\n" +
                "a(x) ::= \"<if(x)><super.a()><endif>\"" + newline +
                "b(x) ::= \"<if(x)><else><super.b()><endif>\"" + newline +
                "c() ::= \"sub.c\""
                ;
            StringTemplateGroup subGroup = new StringTemplateGroup(
                new StringReader( subGroupString ), typeof( AngleBracketTemplateLexer ) );
            subGroup.SuperGroup = superGroup;
            StringTemplate a = subGroup.GetInstanceOf( "a" );
            a.SetAttribute( "x", "foo" );
            Assert.AreEqual( "super.a", a.ToString() );
            StringTemplate b = subGroup.GetInstanceOf( "b" );
            Assert.AreEqual( "sub.csuper.b", b.ToString() );
            StringTemplate c = subGroup.GetInstanceOf( "c" );
            Assert.AreEqual( "sub.c", c.ToString() );
        }

        /** Added feature for ST-21 */
        [TestMethod]
        public void TestListLiteralWithEmptyElements()
        {
            StringTemplate e = new StringTemplate(
                    "$[\"Ter\",,\"Jesse\"]:{n | $i$:$n$}; separator=\", \", null=\"\"$"
                );
            e = e.GetInstanceOf();
            e.SetAttribute( "names", "Ter" );
            e.SetAttribute( "phones", "1" );
            e.SetAttribute( "salaries", "big" );
            string expecting = "1:Ter, 2:, 3:Jesse";
            Assert.AreEqual( expecting, e.ToString() );
        }

        [TestMethod]
        public void TestTemplateApplicationAsOptionValue()
        {
            StringTemplate st = new StringTemplate(
                    "Tokens : <rules; separator=names:{<it>}> ;",
                    typeof( AngleBracketTemplateLexer ) );
            st.SetAttribute( "rules", "A" );
            st.SetAttribute( "rules", "B" );
            st.SetAttribute( "names", "Ter" );
            st.SetAttribute( "names", "Tom" );
            string expecting = "Tokens : ATerTomB ;";
            Assert.AreEqual( expecting, st.ToString() );
        }

        #region C# port tests only

        [TestMethod]
        public void TestDateTimeFormatting()
        {
            StringTemplate e = new StringTemplate(
                    "<p>&copy; 2008- $Today.Year$ . All rights reserved.</p>"
                );
            e = e.GetInstanceOf();
            e.SetAttribute("Today", new DateTime(2009, 8, 4));
            string expecting = "<p>&copy; 2008- 2009 . All rights reserved.</p>";
            Assert.AreEqual(expecting, e.ToString());
        }

        #endregion

        public static void WriteFile( string dir, string fileName, string content )
        {
            try
            {
                System.IO.File.WriteAllText( System.IO.Path.Combine( dir, fileName ), content );
            }
            catch ( IOException ioe )
            {
                Console.Error.WriteLine( "can't write file" );
                ioe.PrintStackTrace( Console.Error );
            }
        }
    }
}
