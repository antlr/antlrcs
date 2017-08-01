using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

[assembly: CLSCompliant(false)]

namespace DotnetValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new GrammarLexer(new ANTLRStringStream("text"));
            var parser = new GrammarParser(new CommonTokenStream(lexer));
            var tree = parser.compilationUnit();

            Action<string> writeLine;

#if LEGACY_PCL
            var writeLineMethod = typeof(int).Assembly.GetType("Console").GetMethod("WriteLine", new[] { typeof(string) });
            writeLine = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), writeLineMethod);
#else
            writeLine = Console.WriteLine;
#endif

            writeLine(new DotTreeGenerator().ToDot((ITree)tree.Tree));
        }
    }
}
