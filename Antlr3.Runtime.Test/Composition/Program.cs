namespace Antlr3.Runtime.Test.Composition
{
    using System;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    internal class Program
    {
        private static void _Main(string[] args)
        {
            // input "x = 2*(3+3)"

            ICharStream input;
            if (args.Length > 0)
            {
                if (args[0].Equals("-i"))
                {
                    if (args.Length > 1)
                    {
                        input = new ANTLRFileStream(args[1]);
                    }
                    else
                    {
                        throw new Exception("No input file specified.");
                    }
                }
                else
                {
                    input = new ANTLRStringStream(args[0]);
                }
            }
            else
            {
                input = new ANTLRInputStream(Console.OpenStandardInput());
            }

            var lex = new VecMathLexer(input);
            var tokens = new CommonTokenStream(lex);
            var g = new VecMathParser(tokens);
            IAstRuleReturnScope<CommonTree> r = g.prog();
            CommonTree t = r.Tree;
            Console.WriteLine("Original tree:   " + t.ToStringTree());

            var simplify = new Simplify(new CommonTreeNodeStream(t));
            t = (CommonTree)simplify.Downup(t);

            var reduce = new Reduce(new CommonTreeNodeStream(t));
            t = (CommonTree)reduce.Downup(t);

            Console.WriteLine("Simplified tree: " + t.ToStringTree());
            Console.ReadKey();
        }
    }
}
