using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr3.ST;

using File = System.IO.File;
using Path = System.IO.Path;

namespace AntlrUnitTests
{
    public abstract class RuntimeTestHarness : IRuntimeTestHarness
    {
        public bool Debug
        {
            get;
            set;
        }
        public string TempPath
        {
            get;
            set;
        }

        public abstract string TestFileName
        {
            get;
        }

        public abstract StringTemplate GetParserCreationTemplate();

        public abstract StringTemplate GetCombinedTestFileTemplate();
        public abstract StringTemplate GetLexerTestFileTemplate();
        public abstract StringTemplate GetTreeTestFileTemplate();
        public abstract StringTemplate GetTreeAndTreeTestFileTemplate();
        public abstract StringTemplate GetTemplateTestFileTemplate();

        public virtual bool Compile( string fileName )
        {
            throw new NotImplementedException();
        }

        public virtual void WriteLexerTestFile( string lexerName, bool debug )
        {
            StringTemplate outputFileST = GetLexerTestFileTemplate();

            outputFileST.SetAttribute( "lexerName", lexerName );

            WriteTestFile( outputFileST.ToString() );
        }
        public virtual void WriteCombinedTestFile( string parserName, string lexerName, string parserStartRuleName )
        {
            StringTemplate outputFileST = GetCombinedTestFileTemplate();
            StringTemplate createParserST = GetParserCreationTemplate();

            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );

            WriteTestFile( outputFileST.ToString() );
        }
        public virtual void WriteTreeTestFile( string parserName, string treeParserName, string lexerName, string parserStartRuleName, string treeParserStartRuleName )
        {
            StringTemplate outputFileST = GetTreeTestFileTemplate();
            StringTemplate createParserST = GetParserCreationTemplate();

            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "treeParserName", treeParserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            outputFileST.SetAttribute( "treeParserStartRuleName", treeParserStartRuleName );

            WriteTestFile( outputFileST.ToString() );
        }
        /** Parser creates trees and so does the tree parser */
        public virtual void WriteTreeAndTreeTestFile( string parserName,
                                                string treeParserName,
                                                string lexerName,
                                                string parserStartRuleName,
                                                string treeParserStartRuleName )
        {
            StringTemplate outputFileST = GetTreeAndTreeTestFileTemplate();
            StringTemplate createParserST = GetParserCreationTemplate();

            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "treeParserName", treeParserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );
            outputFileST.SetAttribute( "treeParserStartRuleName", treeParserStartRuleName );

            WriteTestFile( outputFileST.ToString() );
        }
        public virtual void WriteTemplateTestFile( string parserName, string lexerName, string parserStartRuleName )
        {
            StringTemplate outputFileST = GetTemplateTestFileTemplate();
            StringTemplate createParserST = GetParserCreationTemplate();

            outputFileST.SetAttribute( "createParser", createParserST );
            outputFileST.SetAttribute( "parserName", parserName );
            outputFileST.SetAttribute( "lexerName", lexerName );
            outputFileST.SetAttribute( "parserStartRuleName", parserStartRuleName );

            WriteTestFile( outputFileST.ToString() );
        }

        protected virtual void WriteTestFile( string content )
        {
            File.WriteAllText( Path.Combine( TempPath, TestFileName ), content );
        }
    }
}
