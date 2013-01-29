/*
 * [The "BSD licence"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Pixel Mine, Inc.
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

namespace Antlr3.Tool
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using Antlr3.Codegen;
    using Antlr3.Extensions;
    using Antlr3.Grammars;

    using Exception = System.Exception;
    using StringBuilder = System.Text.StringBuilder;
    using StringTemplate = Antlr4.StringTemplate.Template;
    using TemplateGroup = Antlr4.StringTemplate.TemplateGroup;
    using TemplateGroupFile = Antlr4.StringTemplate.TemplateGroupFile;

    [System.CLSCompliant(false)]
    public class LeftRecursiveRuleAnalyzer : LeftRecursiveRuleWalker
    {
        public enum ASSOC
        {
            left,
            right
        }

        public Grammar g;
        public CodeGenerator generator;
        public string ruleName;
        IDictionary<int, int> tokenToPrec = new Dictionary<int, int>();
        public Dictionary<int, string> binaryAlts = new Dictionary<int, string>();
        public Dictionary<int, string> ternaryAlts = new Dictionary<int, string>();
        public Dictionary<int, string> suffixAlts = new Dictionary<int, string>();
        public List<string> prefixAlts = new List<string>();
        public List<string> otherAlts = new List<string>();

        private static readonly SortedList<string, TemplateGroup> recRuleTemplatesCache =
            new SortedList<string, TemplateGroup>();

        public string language;
        private TemplateGroup recRuleTemplates;

        public IDictionary<int, ASSOC> altAssociativity = new Dictionary<int, ASSOC>();

        public LeftRecursiveRuleAnalyzer(ITreeNodeStream input, Grammar g, string ruleName)
            : base(input)
        {
            this.g = g;
            this.ruleName = ruleName;
            language = (string)g.GetOption("language");
            generator = new CodeGenerator(g.Tool, g, language);
            generator.LoadTemplates(language);
            recRuleTemplates = LoadPrecRuleTemplates(g.Tool);
        }

        private static TemplateGroup LoadPrecRuleTemplates(AntlrTool tool)
        {
            string templateDirs = tool.TemplatesDirectory;
            TemplateGroup group;
            if (!recRuleTemplatesCache.TryGetValue(templateDirs, out group))
            {
                string fileName = CodeGenerator.FindTemplateFile(templateDirs.Split(':'), "LeftRecursiveRules.stg");
                group = new ToolTemplateGroupFile(fileName);
                if (!group.IsDefined("recRuleName"))
                {
                    recRuleTemplatesCache[templateDirs] = group;
                }
                else
                {
                    ErrorManager.Error(ErrorManager.MSG_MISSING_CODE_GEN_TEMPLATES, "PrecRules");
                    return null;
                }
            }

            return group;
        }

        public override void SetTokenPrec(GrammarAST t, int alt)
        {
            int ttype = g.GetTokenType(t.Text);
            //tokenToPrec.Add(ttype, alt);
            tokenToPrec[ttype] = alt;

            ASSOC assoc = ASSOC.left;
            if (t.terminalOptions != null)
            {
                object o;
                t.terminalOptions.TryGetValue("assoc", out o);
                string a = o as string;
                if (a != null)
                {
                    if (a.Equals(ASSOC.right.ToString()))
                    {
                        assoc = ASSOC.right;
                    }
                    else
                    {
                        ErrorManager.Error(ErrorManager.MSG_ILLEGAL_OPTION_VALUE, "assoc", assoc);
                    }
                }
            }

            ASSOC currentAssociativity;
            if (altAssociativity.TryGetValue(alt, out currentAssociativity))
            {
                if (currentAssociativity != assoc)
                    ErrorManager.Error(ErrorManager.MSG_ALL_OPS_NEED_SAME_ASSOC, alt);
            }
            else
            {
                altAssociativity.Add(alt, assoc);
            }

            //System.out.println("op " + alt + ": " + t.getText()+", assoc="+assoc);
        }

        public override void BinaryAlt(GrammarAST altTree, GrammarAST rewriteTree, int alt)
        {
            altTree = GrammarAST.DupTree(altTree);
            rewriteTree = GrammarAST.DupTree(rewriteTree);

            StripSynPred(altTree);
            StripLeftRecursion(altTree);

            // rewrite e to be e_[rec_arg]
            int nextPrec = NextPrecedence(alt);
            StringTemplate refST = recRuleTemplates.GetInstanceOf("recRuleRef");
            refST.SetAttribute("ruleName", ruleName);
            refST.SetAttribute("arg", nextPrec);
            altTree = ReplaceRuleRefs(altTree, refST.Render());

            string altText = Text(altTree);
            altText = altText.Trim();
            altText += "{}"; // add empty alt to prevent pred hoisting
            StringTemplate nameST = recRuleTemplates.GetInstanceOf("recRuleName");
            nameST.SetAttribute("ruleName", ruleName);
            rewriteTree = ReplaceRuleRefs(rewriteTree, "$" + nameST.Render());
            string rewriteText = Text(rewriteTree);
            binaryAlts.Add(alt, altText + (rewriteText != null ? " " + rewriteText : ""));
            //System.out.println("binaryAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
        }

        /** Convert e ? e : e  ->  ? e : e_[nextPrec] */
        public override void TernaryAlt(GrammarAST altTree, GrammarAST rewriteTree, int alt)
        {
            altTree = GrammarAST.DupTree(altTree);
            rewriteTree = GrammarAST.DupTree(rewriteTree);

            StripSynPred(altTree);
            StripLeftRecursion(altTree);

            int nextPrec = NextPrecedence(alt);
            StringTemplate refST = recRuleTemplates.GetInstanceOf("recRuleRef");
            refST.SetAttribute("ruleName", ruleName);
            refST.SetAttribute("arg", nextPrec);
            altTree = ReplaceLastRuleRef(altTree, refST.Render());

            string altText = Text(altTree);
            altText = altText.Trim();
            altText += "{}"; // add empty alt to prevent pred hoisting
            StringTemplate nameST = recRuleTemplates.GetInstanceOf("recRuleName");
            nameST.SetAttribute("ruleName", ruleName);
            rewriteTree = ReplaceRuleRefs(rewriteTree, "$" + nameST.Render());
            string rewriteText = Text(rewriteTree);
            ternaryAlts.Add(alt, altText + (rewriteText != null ? " " + rewriteText : ""));
            //System.out.println("ternaryAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
        }

        public override void PrefixAlt(GrammarAST altTree, GrammarAST rewriteTree, int alt)
        {
            altTree = GrammarAST.DupTree(altTree);
            rewriteTree = GrammarAST.DupTree(rewriteTree);

            StripSynPred(altTree);

            int nextPrec = Precedence(alt);
            // rewrite e to be e_[rec_arg]
            StringTemplate refST = recRuleTemplates.GetInstanceOf("recRuleRef");
            refST.SetAttribute("ruleName", ruleName);
            refST.SetAttribute("arg", nextPrec);
            altTree = ReplaceRuleRefs(altTree, refST.Render());
            string altText = Text(altTree);
            altText = altText.Trim();
            altText += "{}"; // add empty alt to prevent pred hoisting

            StringTemplate nameST = recRuleTemplates.GetInstanceOf("recRuleName");
            nameST.SetAttribute("ruleName", ruleName);
            rewriteTree = ReplaceRuleRefs(rewriteTree, nameST.Render());
            string rewriteText = Text(rewriteTree);

            prefixAlts.Add(altText + (rewriteText != null ? " " + rewriteText : ""));
            //System.out.println("prefixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
        }

        public override void SuffixAlt(GrammarAST altTree, GrammarAST rewriteTree, int alt)
        {
            altTree = GrammarAST.DupTree(altTree);
            rewriteTree = GrammarAST.DupTree(rewriteTree);
            StripSynPred(altTree);
            StripLeftRecursion(altTree);
            StringTemplate nameST = recRuleTemplates.GetInstanceOf("recRuleName");
            nameST.SetAttribute("ruleName", ruleName);
            rewriteTree = ReplaceRuleRefs(rewriteTree, "$" + nameST.Render());
            string rewriteText = Text(rewriteTree);
            string altText = Text(altTree);
            altText = altText.Trim();
            suffixAlts.Add(alt, altText + (rewriteText != null ? " " + rewriteText : ""));
            //		System.out.println("suffixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
        }

        public override void OtherAlt(GrammarAST altTree, GrammarAST rewriteTree, int alt)
        {
            altTree = GrammarAST.DupTree(altTree);
            rewriteTree = GrammarAST.DupTree(rewriteTree);
            StripSynPred(altTree);
            StripLeftRecursion(altTree);
            string altText = Text(altTree);

            string rewriteText = Text(rewriteTree);
            otherAlts.Add(altText + (rewriteText != null ? " " + rewriteText : ""));
            //System.out.println("otherAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
        }

        public string GetArtificialOpPrecRule()
        {
            StringTemplate ruleST = recRuleTemplates.GetInstanceOf("recRule");
            ruleST.SetAttribute("ruleName", ruleName);
            ruleST.SetAttribute("buildAST", grammar.BuildAST);
            StringTemplate argDefST =
                generator.Templates.GetInstanceOf("recRuleDefArg");
            ruleST.SetAttribute("argDef", argDefST);
            StringTemplate ruleArgST =
                generator.Templates.GetInstanceOf("recRuleArg");
            ruleST.SetAttribute("argName", ruleArgST);
            StringTemplate setResultST =
                generator.Templates.GetInstanceOf("recRuleSetResultAction");
            ruleST.SetAttribute("setResultAction", setResultST);

            IDictionary<int, string> opPrecRuleAlts = binaryAlts.Concat(ternaryAlts).Concat(suffixAlts).ToDictionary(i => i.Key, i => i.Value);
            foreach (var pair in opPrecRuleAlts)
            {
                int alt = pair.Key;
                string altText = pair.Value;
                StringTemplate altST = recRuleTemplates.GetInstanceOf("recRuleAlt");
                StringTemplate predST =
                    generator.Templates.GetInstanceOf("recRuleAltPredicate");
                predST.SetAttribute("opPrec", Precedence(alt));
                predST.SetAttribute("ruleName", ruleName);
                altST.SetAttribute("pred", predST);
                altST.SetAttribute("alt", altText);
                ruleST.SetAttribute("alts", altST);
            }

            //Console.WriteLine(ruleST);

            return ruleST.Render();
        }

        public string GetArtificialPrimaryRule()
        {
            StringTemplate ruleST = recRuleTemplates.GetInstanceOf("recPrimaryRule");
            ruleST.SetAttribute("ruleName", ruleName);
            ruleST.SetAttribute("alts", prefixAlts);
            ruleST.SetAttribute("alts", otherAlts);
            //Console.WriteLine(ruleST);
            return ruleST.Render();
        }

        public string GetArtificialPrecStartRule()
        {
            StringTemplate ruleST = recRuleTemplates.GetInstanceOf("recRuleStart");
            ruleST.SetAttribute("ruleName", ruleName);
            ruleST.SetAttribute("maxPrec", 0);

            //Console.WriteLine("start: " + ruleST);
            return ruleST.Render();
        }

        public GrammarAST ReplaceRuleRefs(GrammarAST t, string name)
        {
            if (t == null)
                return null;

            foreach (GrammarAST rref in t.FindAllType(RULE_REF))
            {
                if (rref.Text.Equals(ruleName))
                    rref.Text = name;
            }

            return t;
        }

        public GrammarAST ReplaceLastRuleRef(GrammarAST t, string name)
        {
            if (t == null)
                return null;
            GrammarAST last = null;
            foreach (GrammarAST rref in t.FindAllType(RULE_REF))
            {
                last = rref;
            }
            if (last != null && last.Text.Equals(ruleName))
                last.Text = name;
            return t;
        }

        public void StripSynPred(GrammarAST altAST)
        {
            GrammarAST t = (GrammarAST)altAST.GetChild(0);
            if (t.Type == ANTLRParser.BACKTRACK_SEMPRED ||
                 t.Type == ANTLRParser.SYNPRED ||
                 t.Type == ANTLRParser.SYN_SEMPRED)
            {
                altAST.DeleteChild(0);
            }
        }

        public void StripLeftRecursion(GrammarAST altAST)
        {
            GrammarAST rref = (GrammarAST)altAST.GetChild(0);
            if (rref.Type == ANTLRParser.RULE_REF && rref.Text.Equals(ruleName))
            {
                // remove rule ref
                altAST.DeleteChild(0);

                // reset index so it prints properly
                GrammarAST newFirstChild = (GrammarAST)altAST.GetChild(0);
                altAST.TokenStartIndex = newFirstChild.TokenStartIndex;
            }
        }

        public string Text(GrammarAST t)
        {
            if (t == null)
                return null;

            try
            {
                ITreeNodeStream input = new CommonTreeNodeStream(new ANTLRParser.grammar_Adaptor(null), t);
                ANTLRTreePrinter printer = new ANTLRTreePrinter(input);
                return printer.toString(grammar, true);
            }
            catch (Exception e)
            {
                if (e.IsCritical())
                    throw;

                ErrorManager.Error(ErrorManager.MSG_BAD_AST_STRUCTURE, e);
                return null;
            }
        }

        public int Precedence(int alt)
        {
            return numAlts - alt + 1;
        }

        public int NextPrecedence(int alt)
        {
            int p = Precedence(alt);
            ASSOC assoc;
            altAssociativity.TryGetValue(alt, out assoc);
            if (assoc == ASSOC.left)
                p++;

            return p;
        }

        public override string ToString()
        {
            return "PrecRuleOperatorCollector{" +
                   "binaryAlts=" + binaryAlts +
                   ", rec=" + tokenToPrec +
                   ", ternaryAlts=" + ternaryAlts +
                   ", suffixAlts=" + suffixAlts +
                   ", prefixAlts=" + prefixAlts +
                   ", otherAlts=" + otherAlts +
                   '}';
        }

        private static string ToOriginalString(Antlr.Runtime.CommonTokenStream tokenStream, int start, int end)
        {
            StringBuilder buf = new StringBuilder();
            for (int i = start; i >= 0 && i <= end && i < tokenStream.Count; i++)
            {
                string s = tokenStream.Get(i).Text;
                if (tokenStream.Get(i).Type == ANTLRParser.BLOCK)
                    s = "(";

                buf.Append(s);
            }

            return buf.ToString();
        }
    }
}
