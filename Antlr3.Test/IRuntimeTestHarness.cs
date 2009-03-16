using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntlrUnitTests
{
    public interface IRuntimeTestHarness
    {
        bool Debug
        {
            get;
            set;
        }
        string TempPath
        {
            get;
            set;
        }

        void WriteLexerTestFile( string lexerName, bool debug );
        void WriteCombinedTestFile( string parserName, string lexerName, string parserStartRuleName );
        void WriteTreeTestFile( string parserName, string treeParserName, string lexerName, string parserStartRuleName, string treeParserStartRuleName );
        void WriteTreeAndTreeTestFile( string parserName,
                                                string treeParserName,
                                                string lexerName,
                                                string parserStartRuleName,
                                                string treeParserStartRuleName );
        void WriteTemplateTestFile( string parserName, string lexerName, string parserStartRuleName );
    }
}
