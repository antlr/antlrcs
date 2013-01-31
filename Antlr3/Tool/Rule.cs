/*
 * [The "BSD license"]
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
    using Antlr3.Extensions;

    using ANTLRParser = Antlr3.Grammars.ANTLRParser;
    using ArgumentException = System.ArgumentException;
    using CodeGenerator = Antlr3.Codegen.CodeGenerator;
    using CommonToken = Antlr.Runtime.CommonToken;
    using IToken = Antlr.Runtime.IToken;
    using NFAState = Antlr3.Analysis.NFAState;

    /** Combine the info associated with a rule. */
    public class Rule
    {
        private readonly string _name;
        private int _index;
        private string _modifier;
        private NFAState _startState;
        private NFAState _stopState;

        /// <summary>
        /// This rule's options.
        /// </summary>
        private IDictionary<object, object> _options;

        public static readonly HashSet<string> legalOptions = new HashSet<string>()
            {
                "k",
                "greedy",
                "memoize",
                "backtrack"
            };

        /// <summary>
        /// The AST representing the whole rule.
        /// </summary>
        private GrammarAST _tree;

        /// <summary>
        /// To which grammar does this belong?
        /// </summary>
        private Grammar _grammar;

        /// <summary>
        /// For convenience, track the argument def AST action node if any.
        /// </summary>
        private GrammarAST _argActionAST;

        private GrammarAST _endOfRuleNode;

        /// <summary>
        /// The return values of a rule and predefined rule attributes.
        /// </summary>
        private AttributeScope _returnScope;

        private AttributeScope _parameterScope;

        /// <summary>
        /// the attributes defined with "scope {...}" inside a rule
        /// </summary>
        private AttributeScope _ruleScope;

        /// <summary>
        /// A list of scope names used by this rule
        /// </summary>
        private List<string> _useScopes;

        /// <summary>
        /// Exceptions that this rule can throw
        /// </summary>
        private HashSet<string> _throwsSpec;

        /// <summary>
        /// A list of all LabelElementPair attached to tokens like id=ID
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _tokenLabels;

        /// <summary>
        /// A list of all LabelElementPair attached to tokens like x=. in tree grammar
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _wildcardTreeLabels;

        /// <summary>
        /// A list of all LabelElementPair attached to tokens like x+=. in tree grammar
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _wildcardTreeListLabels;

        /// <summary>
        /// A list of all LabelElementPair attached to single char literals like x='a'
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _charLabels;

        /// <summary>
        /// A list of all LabelElementPair attached to char literals like x+='a'
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _charListLabels;

        /// <summary>
        /// A list of all LabelElementPair attached to rule references like f=field
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _ruleLabels;

        /// <summary>
        /// A list of all Token list LabelElementPair like ids+=ID
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _tokenListLabels;

        /// <summary>
        /// A list of all rule ref list LabelElementPair like ids+=expr
        /// </summary>
        private Dictionary<string, Grammar.LabelElementPair> _ruleListLabels;

        /// <summary>
        /// All labels go in here (plus being split per the above lists) to
        /// catch dup label and label type mismatches.
        /// </summary>
        private  IDictionary<string, Grammar.LabelElementPair> _labelNameSpace =
            new Dictionary<string, Grammar.LabelElementPair>();

        /// <summary>
        /// Map a name to an action for this rule.  Currently init is only
        /// one we use, but we can add more in future.
        /// The code generator will use this to fill holes in the rule template.
        /// I track the AST node for the action in case I need the line number
        /// for errors.  A better name is probably namedActions, but I don't
        /// want everyone to have to change their code gen templates now.
        /// </summary>
        private readonly IDictionary<string, object> _actions = new Dictionary<string, object>();

        /// <summary>
        /// Track all executable actions other than named actions like @init.
        /// Also tracks exception handlers, predicates, and rewrite rewrites.
        /// We need to examine these actions before code generation so
        /// that we can detect refs to $rule.attr etc...
        /// </summary>
        private readonly IList<GrammarAST> _inlineActions = new List<GrammarAST>();

        private readonly int _numberOfAlts;

        /// <summary>
        /// Each alt has a Map&lt;tokenRefName,List&lt;tokenRefAST&gt;&gt;; range 1..numberOfAlts.
        /// So, if there are 3 ID refs in a rule's alt number 2, you'll have
        /// altToTokenRef[2].get("ID").size()==3.  This is used to see if $ID is ok.
        /// There must be only one ID reference in the alt for $ID to be ok in
        /// an action--must be unique.
        ///
        /// This also tracks '+' and "int" literal token references
        /// (if not in LEXER).
        ///
        /// Rewrite rules force tracking of all tokens.
        /// </summary>
        private IDictionary<string, IList<GrammarAST>>[] _altToTokenRefMap;

        /// <summary>
        /// Each alt has a Map&lt;ruleRefName,List&lt;ruleRefAST&gt;&gt;; range 1..numberOfAlts
        /// So, if there are 3 expr refs in a rule's alt number 2, you'll have
        /// altToRuleRef[2].get("expr").size()==3.  This is used to see if $expr is ok.
        /// There must be only one expr reference in the alt for $expr to be ok in
        /// an action--must be unique.
        ///
        /// Rewrite rules force tracking of all rule result ASTs. 1..n
        /// </summary>
        private IDictionary<string, IList<GrammarAST>>[] _altToRuleRefMap;

        /// <summary>
        /// Do not generate start, stop etc... in a return value struct unless
        /// somebody references $r.start somewhere.
        /// </summary>
        private bool _referencedPredefinedRuleAttributes = false;

        private bool _isSynPred = false;

        private bool _imported = false;

        public Rule( Grammar grammar,
                    string ruleName,
                    int ruleIndex,
                    int numberOfAlts )
        {
            this._name = ruleName;
            this._index = ruleIndex;
            this._numberOfAlts = numberOfAlts;
            this._grammar = grammar;
            _throwsSpec = new HashSet<string>() { "RecognitionException" };
            _altToTokenRefMap = new IDictionary<string, IList<GrammarAST>>[numberOfAlts + 1]; //new Map[numberOfAlts + 1];
            _altToRuleRefMap = new IDictionary<string, IList<GrammarAST>>[numberOfAlts + 1]; //new Map[numberOfAlts + 1];
            for ( int alt = 1; alt <= numberOfAlts; alt++ )
            {
                _altToTokenRefMap[alt] = new Dictionary<string, IList<GrammarAST>>();
                _altToRuleRefMap[alt] = new Dictionary<string, IList<GrammarAST>>();
            }
        }

        #region Properties

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

        public NFAState StartState
        {
            get
            {
                return _startState;
            }

            set
            {
                _startState = value;
            }
        }

        public NFAState StopState
        {
            get
            {
                return _stopState;
            }

            set
            {
                _stopState = value;
            }
        }

        public GrammarAST EORNode
        {
            get
            {
                return _endOfRuleNode;
            }

            set
            {
                _endOfRuleNode = value;
            }
        }

        public IDictionary<string, object> Actions
        {
            get
            {
                return _actions;
            }
        }

        /** If a rule has no user-defined return values and nobody references
         *  it's start/stop (predefined attributes), then there is no need to
         *  define a struct; otherwise for now we assume a struct.  A rule also
         *  has multiple return values if you are building trees or templates.
         */
        public bool HasMultipleReturnValues
        {
            get
            {
                return _referencedPredefinedRuleAttributes
                    || Grammar.BuildAST
                    || Grammar.BuildTemplate
                    || (ReturnScope != null && ReturnScope.Attributes.Count > 1);
            }
        }

        public bool HasReturnValue
        {
            get
            {
                return _referencedPredefinedRuleAttributes
                    || Grammar.BuildAST
                    || Grammar.BuildTemplate
                    || (ReturnScope != null && ReturnScope.Attributes.Count > 0);
            }
        }

        public bool HasSingleReturnValue
        {
            get
            {
                return !(_referencedPredefinedRuleAttributes || Grammar.BuildAST || Grammar.BuildTemplate)
                    && (ReturnScope != null && ReturnScope.Attributes.Count == 1);
            }
        }

        protected internal IDictionary<string, Grammar.LabelElementPair> LabelNameSpace
        {
            get
            {
                return _labelNameSpace;
            }

            set
            {
                _labelNameSpace = value;
            }
        }

        public bool Imported
        {
            get
            {
                return _imported;
            }

            set
            {
                _imported = value;
            }
        }

        public ICollection<GrammarAST> InlineActions
        {
            get
            {
                return _inlineActions;
            }
        }

        public bool IsSynPred
        {
            get
            {
                return _isSynPred;
            }

            set
            {
                _isSynPred = value;
            }
        }

        public string Modifier
        {
            get
            {
                return _modifier;
            }

            set
            {
                _modifier = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int NumberOfAlts
        {
            get
            {
                return _numberOfAlts;
            }
        }

        public bool ReferencedPredefinedRuleAttributes
        {
            get
            {
                return _referencedPredefinedRuleAttributes;
            }

            set
            {
                _referencedPredefinedRuleAttributes = value;
            }
        }

        public IDictionary<string, Grammar.LabelElementPair> RuleLabels
        {
            get
            {
                return _ruleLabels;
            }
        }

        protected IDictionary<object, object> Options
        {
            get
            {
                return _options;
            }

            set
            {
                _options = value;
            }
        }

        public GrammarAST Tree
        {
            get
            {
                return _tree;
            }

            set
            {
                _tree = value;
            }
        }

        public Grammar Grammar
        {
            get
            {
                return _grammar;
            }

            set
            {
                _grammar = value;
            }
        }

        public GrammarAST ArgActionAST
        {
            get
            {
                return _argActionAST;
            }

            set
            {
                _argActionAST = value;
            }
        }

        public AttributeScope ReturnScope
        {
            get
            {
                return _returnScope;
            }

            set
            {
                _returnScope = value;
            }
        }

        public AttributeScope ParameterScope
        {
            get
            {
                return _parameterScope;
            }

            set
            {
                _parameterScope = value;
            }
        }

        public AttributeScope RuleScope
        {
            get
            {
                return _ruleScope;
            }

            set
            {
                _ruleScope = value;
            }
        }

        public List<string> UseScopes
        {
            get
            {
                return _useScopes;
            }

            set
            {
                _useScopes = value;
            }
        }

        public HashSet<string> ThrowsSpec
        {
            get
            {
                return _throwsSpec;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> TokenLabels
        {
            get
            {
                return _tokenLabels;
            }

            set
            {
                _tokenLabels = value;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> WildcardTreeLabels
        {
            get
            {
                return _wildcardTreeLabels;
            }

            set
            {
                _wildcardTreeLabels = value;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> WildcardTreeListLabels
        {
            get
            {
                return _wildcardTreeListLabels;
            }

            set
            {
                _wildcardTreeListLabels = value;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> CharLabels
        {
            get
            {
                return _charLabels;
            }

            set
            {
                _charLabels = value;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> CharListLabels
        {
            get
            {
                return _charListLabels;
            }

            set
            {
                _charListLabels = value;
            }
        }

        public Dictionary<string, Grammar.LabelElementPair> TokenListLabels
        {
            get
            {
                return _tokenListLabels;
            }

            set
            {
                _tokenListLabels = value;
            }
        }

        public IDictionary<string, Grammar.LabelElementPair> RuleListLabels
        {
            get
            {
                return _ruleListLabels;
            }
        }

        public RuleType RuleType
        {
            get
            {
                return GetRuleType(Name);
            }
        }

        public string SingleValueReturnName
        {
            get
            {
                return GetSingleValueReturnName();
            }
        }

        public string SingleValueReturnType
        {
            get
            {
                return GetSingleValueReturnType();
            }
        }

        #endregion

        public static RuleType GetRuleType(string name)
        {
            return char.IsUpper(name[0]) ? RuleType.Lexer : RuleType.Parser;
        }

        public virtual void DefineLabel( IToken label, GrammarAST elementRef, LabelType type )
        {
            Grammar.LabelElementPair pair = new Grammar.LabelElementPair( Grammar, label, elementRef );
            pair.type = type;
            LabelNameSpace[label.Text] = pair;
            switch (type)
            {
            case LabelType.Token:
                TokenLabels = TokenLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                TokenLabels[label.Text] = pair;
                break;

            case LabelType.WildcardTree:
                WildcardTreeLabels = WildcardTreeLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                WildcardTreeLabels[label.Text] = pair;
                break;

            case LabelType.WildcardTreeList:
                WildcardTreeListLabels = WildcardTreeListLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                WildcardTreeListLabels[label.Text] = pair;
                break;

            case LabelType.Rule:
                _ruleLabels = _ruleLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                _ruleLabels[label.Text] = pair;
                break;

            case LabelType.TokenList:
                TokenListLabels = TokenListLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                TokenListLabels[label.Text] = pair;
                break;

            case LabelType.RuleList:
                _ruleListLabels = _ruleListLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                _ruleListLabels[label.Text] = pair;
                break;

            case LabelType.Char:
                CharLabels = CharLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                CharLabels[label.Text] = pair;
                break;

            case LabelType.CharList:
                CharListLabels = CharListLabels ?? new Dictionary<string, Grammar.LabelElementPair>();
                CharListLabels[label.Text] = pair;
                break;

            default:
                throw new ArgumentException(string.Format("Unexpected label type {0}.", type), "type");
            }
        }

        public virtual Grammar.LabelElementPair GetLabel( string name )
        {
            Grammar.LabelElementPair pair;
            LabelNameSpace.TryGetValue(name, out pair);
            return pair;
        }

        public virtual Grammar.LabelElementPair GetTokenLabel( string name )
        {
            Grammar.LabelElementPair pair = null;
            if (TokenLabels != null)
                TokenLabels.TryGetValue(name, out pair);

            return pair;
        }

        public virtual Grammar.LabelElementPair GetRuleLabel( string name )
        {
            Grammar.LabelElementPair pair = null;
            if ( RuleLabels != null )
                RuleLabels.TryGetValue(name, out pair);

            return pair;
        }

        public virtual Grammar.LabelElementPair GetTokenListLabel( string name )
        {
            Grammar.LabelElementPair pair = null;
            if (TokenListLabels != null)
                TokenListLabels.TryGetValue(name, out pair);

            return pair;
        }

        public virtual Grammar.LabelElementPair GetRuleListLabel( string name )
        {
            Grammar.LabelElementPair pair = null;
            if (RuleListLabels != null)
                RuleListLabels.TryGetValue(name, out pair);

            return pair;
        }

        /** Track a token ID or literal like '+' and "void" as having been referenced
         *  somewhere within the alts (not rewrite sections) of a rule.
         *
         *  This differs from Grammar.altReferencesTokenID(), which tracks all
         *  token IDs to check for token IDs without corresponding lexer rules.
         */
        public virtual void TrackTokenReferenceInAlt( GrammarAST refAST, int outerAltNum )
        {
            IList<GrammarAST> refs;
            _altToTokenRefMap[outerAltNum].TryGetValue(refAST.Text, out refs);
            if ( refs == null )
            {
                refs = new List<GrammarAST>();
                _altToTokenRefMap[outerAltNum][refAST.Text] = refs;
            }
            refs.Add( refAST );
        }

        public virtual IList<GrammarAST> GetTokenRefsInAlt( string @ref, int outerAltNum )
        {
            if ( _altToTokenRefMap[outerAltNum] != null )
            {
                IList<GrammarAST> tokenRefASTs;
                _altToTokenRefMap[outerAltNum].TryGetValue(@ref, out tokenRefASTs);
                return tokenRefASTs;
            }

            return null;
        }

        public virtual void TrackRuleReferenceInAlt( GrammarAST refAST, int outerAltNum )
        {
            IList<GrammarAST> refs;
            _altToRuleRefMap[outerAltNum].TryGetValue(refAST.Text, out refs);
            if ( refs == null )
            {
                refs = new List<GrammarAST>();
                _altToRuleRefMap[outerAltNum][refAST.Text] = refs;
            }

            refs.Add( refAST );
        }

        public virtual IList<GrammarAST> GetRuleRefsInAlt( string @ref, int outerAltNum )
        {
            if ( _altToRuleRefMap[outerAltNum] != null )
            {
                IList<GrammarAST> ruleRefASTs;
                _altToRuleRefMap[outerAltNum].TryGetValue(@ref, out ruleRefASTs);
                return ruleRefASTs;
            }

            return null;
        }

        public virtual ICollection<string> GetTokenRefsInAlt( int altNum )
        {
            return _altToTokenRefMap[altNum].Keys;
        }

        /** For use with rewrite rules, we must track all tokens matched on the
         *  left-hand-side; so we need Lists.  This is a unique list of all
         *  token types for which the rule needs a list of tokens.  This
         *  is called from the rule template not directly by the code generator.
         */
        public virtual ICollection<string> GetAllTokenRefsInAltsWithRewrites()
        {
            string output = (string)Grammar.GetOption( "output" );
            ICollection<string> tokens = new HashSet<string>();
            if ( output == null || !output.Equals( "AST" ) )
            {
                // return nothing if not generating trees; i.e., don't do for templates
                return tokens;
            }

            //System.out.println("blk "+tree.findFirstType(ANTLRParser.BLOCK).toStringTree());
            for (int i = 1; i <= _numberOfAlts; i++)
            {
                if ( HasRewrite(i) )
                {
                    foreach ( string tokenName in _altToTokenRefMap[i].Keys )
                    {
                        // convert token name like ID to ID, "void" to 31
                        int ttype = Grammar.GetTokenType( tokenName );
                        string label = Grammar.generator.GetTokenTypeAsTargetLabel( ttype );
                        tokens.Add( label );
                    }
                }
            }

            return tokens;
        }

        public virtual ICollection<string> GetRuleRefsInAlt( int outerAltNum )
        {
            return _altToRuleRefMap[outerAltNum].Keys;
        }

        /** For use with rewrite rules, we must track all rule AST results on the
         *  left-hand-side; so we need Lists.  This is a unique list of all
         *  rule results for which the rule needs a list of results.
         */
        public virtual ICollection<string> GetAllRuleRefsInAltsWithRewrites()
        {
            var rules = from i in Enumerable.Range( 1, _numberOfAlts )
                        where HasRewrite(i)
                        select _altToRuleRefMap[i].Keys;

            return new HashSet<string>( rules.SelectMany( r => r ) );
        }

        public virtual bool HasRewrite( int i )
        {
            GrammarAST blk = Tree.FindFirstType(ANTLRParser.BLOCK);
            GrammarAST alt = blk.GetBlockAlt(i);
            GrammarAST rew = (GrammarAST)alt.Parent.GetChild(alt.ChildIndex + 1);
            if (rew != null && rew.Type == ANTLRParser.REWRITES)
                return true;

            if (alt.FindFirstType(ANTLRParser.REWRITES) != null)
                return true;

            return false;
        }

        /** Return the scope containing name */
        public virtual AttributeScope GetAttributeScope( string name )
        {
            AttributeScope scope = GetLocalAttributeScope( name );
            if ( scope != null )
            {
                return scope;
            }
            if ( RuleScope != null && RuleScope.GetAttribute( name ) != null )
            {
                scope = RuleScope;
            }
            return scope;
        }

        /** Get the arg, return value, or predefined property for this rule */
        public virtual AttributeScope GetLocalAttributeScope( string name )
        {
            AttributeScope scope = null;
            if ( ReturnScope != null && ReturnScope.GetAttribute( name ) != null )
            {
                scope = ReturnScope;
            }
            else if ( ParameterScope != null && ParameterScope.GetAttribute( name ) != null )
            {
                scope = ParameterScope;
            }
            else
            {
                AttributeScope rulePropertiesScope =
                    RuleLabelScope.grammarTypeToRulePropertiesScope[(int)Grammar.type];
                if ( rulePropertiesScope.GetAttribute( name ) != null )
                {
                    scope = rulePropertiesScope;
                }
            }
            return scope;
        }

        /** For references to tokens rather than by label such as $ID, we
         *  need to get the existing label for the ID ref or create a new
         *  one.
         */
        public virtual string GetElementLabel( string refdSymbol,
                                      int outerAltNum,
                                      CodeGenerator generator )
        {
            GrammarAST uniqueRefAST;
            if ( Grammar.type != GrammarType.Lexer &&
                 Rule.GetRuleType(refdSymbol) == RuleType.Lexer )
            {
                // symbol is a token
                IList<GrammarAST> tokenRefs = GetTokenRefsInAlt( refdSymbol, outerAltNum );
                uniqueRefAST = tokenRefs[0];
            }
            else
            {
                // symbol is a rule
                IList<GrammarAST> ruleRefs = GetRuleRefsInAlt( refdSymbol, outerAltNum );
                uniqueRefAST = ruleRefs[0];
            }
            if ( uniqueRefAST.code == null )
            {
                // no code?  must not have gen'd yet; forward ref
                return null;
            }
            string labelName = null;
            string existingLabelName =
                (string)uniqueRefAST.code.GetAttribute( "label" );
            // reuse any label or list label if it exists
            if ( existingLabelName != null )
            {
                labelName = existingLabelName;
            }
            else
            {
                // else create new label
                labelName = generator.CreateUniqueLabel( refdSymbol );
                CommonToken label = new CommonToken( ANTLRParser.ID, labelName );
                if ( Grammar.type != GrammarType.Lexer &&
                     Rule.GetRuleType(refdSymbol) == Tool.RuleType.Lexer )
                {
                    Grammar.DefineTokenRefLabel( Name, label, uniqueRefAST );
                }
                else
                {
                    Grammar.DefineRuleRefLabel( Name, label, uniqueRefAST );
                }
                uniqueRefAST.code.SetAttribute( "label", labelName );
            }
            return labelName;
        }

        public virtual string GetSingleValueReturnType()
        {
            if ( ReturnScope != null && ReturnScope.Attributes.Count == 1 )
            {
                return ReturnScope.Attributes[0].Type;
                //ICollection<Attribute> retvalAttrs = returnScope.attributes.Values;
                //return retvalAttrs.First().Type;

                //object[] javaSucks = retvalAttrs.toArray();
                //return ( (Attribute)javaSucks[0] ).type;
            }
            return null;
        }

        public virtual string GetSingleValueReturnName()
        {
            if ( ReturnScope != null && ReturnScope.Attributes.Count == 1 )
            {
                return ReturnScope.Attributes[0].Name;
            }
            return null;
        }

        /** Given @scope::name {action} define it for this grammar.  Later,
         *  the code generator will ask for the actions table.
         */
        public virtual void DefineNamedAction( GrammarAST ampersandAST,
                                      GrammarAST nameAST,
                                      GrammarAST actionAST )
        {
            //JSystem.@out.println("rule @"+nameAST.getText()+"{"+actionAST.getText()+"}");
            string actionName = nameAST.Text;
            object actionsObject;
            _actions.TryGetValue(actionName, out actionsObject);
            GrammarAST a = (GrammarAST)actionsObject;
            if ( a != null )
            {
                ErrorManager.GrammarError(
                    ErrorManager.MSG_ACTION_REDEFINITION, Grammar,
                    nameAST.Token, nameAST.Text );
            }
            else
            {
                _actions[actionName] = actionAST;
            }
        }

        public virtual void TrackInlineAction( GrammarAST actionAST )
        {
            _inlineActions.Add( actionAST );
        }

        /** Save the option key/value pair and process it; return the key
         *  or null if invalid option.
         */
        public virtual string SetOption( string key, object value, IToken optionsStartToken )
        {
            if ( !legalOptions.Contains( key ) )
            {
                ErrorManager.GrammarError( ErrorManager.MSG_ILLEGAL_OPTION,
                                          Grammar,
                                          optionsStartToken,
                                          key );
                return null;
            }
            if ( Options == null )
            {
                Options = new Dictionary<object, object>();
            }
            if ( key.Equals( "memoize" ) && value.ToString().Equals( "true" ) )
            {
                Grammar.composite.RootGrammar.atLeastOneRuleMemoizes = true;
            }
            if ( key == "backtrack" && value.ToString() == "true" )
            {
                Grammar.composite.RootGrammar.atLeastOneBacktrackOption = true;
            }
            if ( key.Equals( "k" ) )
            {
                Grammar.numberOfManualLookaheadOptions++;
            }
            Options[key] = value;
            return key;
        }

        public virtual void SetOptions( IDictionary<string, object> options, IToken optionsStartToken )
        {
            if ( options == null )
            {
                this.Options = null;
                return;
            }

            foreach ( var option in options.ToArray() )
            {
                string optionName = option.Key;
                object optionValue = option.Value;
                string stored = SetOption( optionName, optionValue, optionsStartToken );
                if ( stored == null )
                    options.Remove( optionName );
            }
        }

#if false
        /** Used during grammar imports to see if sets of rules intersect... This
         *  method and hashCode use the String name as the key for Rule objects.
         */
        public override bool Equals( object other )
        {
            return this.name.Equals( ( (Rule)other ).name );
        }

        /** Used during grammar imports to see if sets of rules intersect... */
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
#endif

        public override string ToString()
        {
            // used for testing
            return "[" + Grammar.name + "." + Name + ",index=" + Index + ",line=" + Tree.Token.Line + "]";
        }
    }
}
