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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Antlr3.Analysis;

    using StringBuilder = System.Text.StringBuilder;

    public class DgmlGenerator : IGraphGenerator
    {
        private const string EpsilonLinkLabel = "ε";
        private readonly Grammar _grammar;
        private string _groupId;
        private readonly Dictionary<State, XElement> _nodes = new Dictionary<State, XElement>();
        private readonly Dictionary<KeyValuePair<State, Transition>, XElement> _links = new Dictionary<KeyValuePair<State, Transition>, XElement>();
        private readonly List<XElement> _extraNodes = new List<XElement>();
        private readonly List<XElement> _extraLinks = new List<XElement>();
        private readonly HashSet<int> _markedStates = new HashSet<int>();
        private readonly HashSet<int> _canSkipStates = new HashSet<int>();

        public DgmlGenerator(Grammar grammar)
        {
            if (grammar == null)
                throw new ArgumentNullException("grammar");

            _grammar = grammar;
        }

        public string FileExtension
        {
            get
            {
                return ".dgml";
            }
        }

        public bool GroupNodes
        {
            get;
            set;
        }

        public bool StripNonreducedStates
        {
            get;
            set;
        }

        public string GenerateGraph(State state)
        {
            _nodes.Clear();
            _links.Clear();
            _extraNodes.Clear();
            _extraLinks.Clear();
            _markedStates.Clear();
            _canSkipStates.Clear();

            DFAState dfaState = state as DFAState;
            if (GroupNodes)
            {
                if (dfaState != null)
                {
                    _groupId = "decision_";
                    _extraNodes.Add(new XElement(Elements.Node,
                        new XAttribute(Attributes.Id, _groupId),
                        new XAttribute(Attributes.Label, dfaState.StateNumber.ToString()),
                        new XAttribute(Attributes.Group, "Collapsed")));
                }
                else
                {
                    NFAState nfaState = (NFAState)state;
                    _groupId = "rule_" + nfaState.enclosingRule.Name;
                    _extraNodes.Add(new XElement(Elements.Node,
                        new XAttribute(Attributes.Id, _groupId),
                        new XAttribute(Attributes.Label, nfaState.enclosingRule.Name),
                        new XAttribute(Attributes.Group, "Collapsed")));
                }
            }

            if (dfaState != null)
            {
                WalkCreatingDfaDgml(dfaState);
            }
            else
            {
                WalkRuleNfaCreatingDgml(state);
                LocateVerboseStates(state);
            }

            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Elements.DirectedGraph,
                    new XAttribute(Attributes.GraphDirection, GraphDirection.TopToBottom),
                    new XAttribute(Attributes.Layout, Layout.Sugiyama),
                    GetNodes(),
                    GetLinks(),
                    GetCategories(),
                    GetProperties(),
                    GetStyles()));

            return document.ToString();
        }

        private void WalkCreatingDfaDgml(DFAState dfaState)
        {
            if (!_markedStates.Add(dfaState.StateNumber))
                return;

            // first add this node
            string nodeCategory;
            if (dfaState.IsAcceptState)
            {
                nodeCategory = Categories.StopState;
            }
            else
            {
                nodeCategory = Categories.State;
            }

            XElement node = new XElement(Elements.Node,
                new XAttribute(Attributes.Id, "state_" + dfaState.StateNumber),
                new XAttribute(Attributes.Label, GetStateLabel(dfaState)),
                new XAttribute(Attributes.Category, nodeCategory));

            _nodes.Add(dfaState, node);
            if (GroupNodes)
                _extraLinks.Add(CreateContainmentLink(_groupId, "state_" + dfaState.StateNumber));

            // make an edge for each transition
            for (int i = 0; i < dfaState.NumberOfTransitions; i++)
            {
                Transition edge = dfaState.GetTransition(i);
                if (StripNonreducedStates)
                {
                    DFAState target = edge.Target as DFAState;
                    // don't generate nodes for terminal states
                    if (target != null && target.AcceptStateReachable != Reachable.Yes)
                        continue;
                }

                string edgeCategory = Categories.Edge;
                XElement edgeElement = new XElement(Elements.Link,
                    new XAttribute(Attributes.Source, "state_" + dfaState.StateNumber),
                    new XAttribute(Attributes.Target, "state_" + edge.Target.StateNumber),
                    new XAttribute(Attributes.Category, edgeCategory),
                    new XAttribute(Attributes.Label, GetEdgeLabel(edge)));

                _links.Add(new KeyValuePair<State, Transition>(dfaState, edge), edgeElement);
                WalkCreatingDfaDgml((DFAState)edge.Target);
            }
        }

        private XElement CreateContainmentLink(string source, string target)
        {
            return new XElement(Elements.Link,
                new XAttribute(Attributes.Source, source),
                new XAttribute(Attributes.Target, target),
                new XAttribute(Attributes.Category, Categories.Contains));
        }

        private void WalkRuleNfaCreatingDgml(State state)
        {
            if (!_markedStates.Add(state.StateNumber))
                return;

            NFAState nfaState = state as NFAState;

            // create the node
            string nodeCategory;
            if (state.IsAcceptState)
                nodeCategory = Categories.StopState;
            else if (nfaState != null && nfaState.IsDecisionState)
                nodeCategory = Categories.DecisionState;
            else
                nodeCategory = Categories.State;

            XElement node = new XElement(Elements.Node,
                new XAttribute(Attributes.Id, "state_" + state.StateNumber),
                new XAttribute(Attributes.Label, GetStateLabel(state)),
                new XAttribute(Attributes.Category, nodeCategory));

            if (nfaState != null && nfaState.IsDecisionState)
            {
                string baseFileName = _grammar.name;
                if (_grammar.implicitLexer)
                    baseFileName += Grammar.grammarTypeToFileNameSuffix[(int)_grammar.type];

                string decisionPath = string.Format("{0}.dec-{1}.dgml", baseFileName, nfaState.DecisionNumber);
                node.Add(new XAttribute(Attributes.Reference, decisionPath));
            }

            _nodes.Add(state, node);
            if (GroupNodes)
                _extraLinks.Add(CreateContainmentLink(_groupId, "state_" + state.StateNumber));

            // don't go past end of rule
            if (state.IsAcceptState)
                return;

            // create links for each transition
            for (int i = 0; i < state.NumberOfTransitions; i++)
            {
                Transition edge = state.GetTransition(i);
                RuleClosureTransition rr = edge as RuleClosureTransition;
                if (rr != null)
                {
                    string label;

                    if (rr.Rule.Grammar != _grammar)
                        label = string.Format("<{0}.{1}>", rr.Rule.Grammar.name, rr.Rule.Name);
                    else
                        label = string.Format("<{0}>", rr.Rule.Name);

                    XElement link = new XElement(Elements.Link,
                        new XAttribute(Attributes.Source, "state_" + state.StateNumber),
                        new XAttribute(Attributes.Target, "state_" + rr.FollowState),
                        new XAttribute(Attributes.Category, Categories.RuleClosureEdge),
                        new XAttribute(Attributes.Label, label));

                    _links.Add(new KeyValuePair<State, Transition>(state, edge), link);
                    WalkRuleNfaCreatingDgml(rr.FollowState);
                }
                else
                {
                    string edgeCategory;

                    if (edge.IsAction)
                        edgeCategory = Categories.ActionEdge;
                    else if (edge.IsEpsilon)
                        edgeCategory = Categories.EpsilonEdge;
                    else if (edge.Label.IsSet || edge.Label.IsAtom)
                        edgeCategory = Categories.AtomEdge;
                    else
                        edgeCategory = Categories.Edge;

                    XElement link = new XElement(Elements.Link,
                        new XAttribute(Attributes.Source, "state_" + state.StateNumber),
                        new XAttribute(Attributes.Target, "state_" + edge.Target.StateNumber),
                        new XAttribute(Attributes.Category, edgeCategory),
                        new XAttribute(Attributes.Label, GetEdgeLabel(edge)));

                    _links.Add(new KeyValuePair<State, Transition>(state, edge), link);
                    WalkRuleNfaCreatingDgml(edge.Target);
                }
            }
        }

        private void LocateVerboseStates(State startState)
        {
            Dictionary<State, XElement> verboseNodes = _nodes.Where(IsVerboseNode).ToDictionary(i => i.Key, i => i.Value);
            verboseNodes.Remove(startState);

            foreach (var verboseNode in verboseNodes)
            {
                verboseNode.Value.Add(new XElement(Elements.Category,
                    new XAttribute(Attributes.Ref, Categories.VerboseNode)));
            }

            Dictionary<State, List<State>> shortcuts = new Dictionary<State, List<State>>();
            foreach (var link in _links)
            {
                State source = link.Key.Key;
                State target = link.Key.Value.Target;
                if (!verboseNodes.ContainsKey(source) && verboseNodes.ContainsKey(target))
                {
                    List<State> newTargets = new List<State>();
                    Stack<State> remaining = new Stack<State>();
                    remaining.Push(target);
                    while (remaining.Count > 0)
                    {
                        State current = remaining.Pop();
                        if (!verboseNodes.ContainsKey(current))
                        {
                            if (_nodes.ContainsKey(current))
                                newTargets.Add(current);
                        }
                        else
                        {
                            for (int i = 0; i < current.NumberOfTransitions; i++)
                            {
                                remaining.Push(current.GetTransition(i).Target);
                            }
                        }
                    }

                    foreach (var newTarget in newTargets)
                    {
                        XElement newLink = new XElement(link.Value);
                        newLink.Attribute(Attributes.Target).Remove();
                        newLink.Add(new XAttribute(Attributes.Target, "state_" + newTarget.StateNumber));
                        newLink.Add(new XElement(Elements.Category, new XAttribute(Attributes.Ref, Categories.OptimizedEdge)));

                        _extraLinks.Add(newLink);
                    }
                }
            }
        }

        private bool IsVerboseNode(KeyValuePair<State, XElement> node)
        {
            NFAState state = node.Key as NFAState;
            if (state == null)
                return false;

            // Cannot skip first state
            if (state.StateNumber == 0)
                return false;

            // Cannot skip accepted state
            if (state.IsAcceptState)
                return false;

            // Cannot skip alternative state
            if (state.DecisionNumber > 0)
                return false;

            // Cannot skip start of block state
            if (state.endOfBlockStateNumber != State.INVALID_STATE_NUMBER)
                return false;

            // Cannot skip state with more than one incoming transition (often an end-of-alternative state)
            if (NumberOfIncomingTransition(state) > 1)
                return false;

            return HasOneOrMoreEpsilonTransitionOnly(state);
        }

        private int NumberOfIncomingTransition(NFAState state)
        {
            return _links.Count(i => i.Key.Value.Target == state);
        }

        private static bool HasOneOrMoreEpsilonTransitionOnly(NFAState state)
        {
            for (int t = 0; t < state.NumberOfTransitions; t++)
            {
                Transition transition = state.transition[t];
                if (!transition.IsEpsilon)
                    return false;
            }

            return state.NumberOfTransitions > 0;
        }

        private string GetEdgeLabel(Transition edge)
        {
            string label = edge.Label.ToString(_grammar);
            label = string.Join(" ", label.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
            if (label.Equals(Label.EPSILON_STR))
            {
                label = EpsilonLinkLabel;
            }
            State target = edge.Target;
            if (!edge.IsSemanticPredicate && target is DFAState)
            {
                // look for gated predicates; don't add gated to simple sempred edges
                SemanticContext preds =
                    ((DFAState)target).GetGatedPredicatesInNFAConfigurations();
                if (preds != null)
                {
                    string predsStr = "";
                    predsStr = "&&{" +
                        preds.GenExpr(_grammar.generator,
                                      _grammar.generator.Templates, null).ToString()
                        + "}?";
                    label += predsStr;
                }
            }

            if (label.Length > 43)
                label = label.Substring(0, 40) + "...";

            return label;
        }

        private string GetStateLabel(State state)
        {
            if (state == null)
                return "null";

            string stateLabel = state.StateNumber.ToString();
            DFAState dfaState = state as DFAState;
            NFAState nfaState = state as NFAState;
            if (dfaState != null)
            {
                StringBuilder builder = new StringBuilder(250);
                builder.Append('s');
                builder.Append(state.StateNumber);
                if (AntlrTool.internalOption_ShowNFAConfigsInDFA)
                {
                    if (dfaState.AbortedDueToRecursionOverflow)
                    {
                        builder.AppendLine();
                        builder.AppendLine("AbortedDueToRecursionOverflow");
                    }

                    var alts = dfaState.AltSet;
                    if (alts != null)
                    {
                        builder.AppendLine();
                        List<int> altList = alts.OrderBy(i => i).ToList();
                        ICollection<NFAConfiguration> configurations = dfaState.NfaConfigurations;
                        for (int i = 0; i < altList.Count; i++)
                        {
                            int alt = altList[i];
                            if (i > 0)
                                builder.AppendLine();
                            builder.AppendFormat("alt{0}:", alt);
                            // get a list of configs for just this alt
                            // it will help us print better later
                            List<NFAConfiguration> configsInAlt = new List<NFAConfiguration>();
                            foreach (NFAConfiguration c in configurations)
                            {
                                if (c.Alt != alt)
                                    continue;

                                configsInAlt.Add(c);
                            }

                            int n = 0;
                            for (int cIndex = 0; cIndex < configsInAlt.Count; cIndex++)
                            {
                                NFAConfiguration c = configsInAlt[cIndex];
                                n++;
                                builder.Append(c.ToString(false));
                                if ((cIndex + 1) < configsInAlt.Count)
                                {
                                    builder.Append(", ");
                                }
                                if (n % 5 == 0 && (configsInAlt.Count - cIndex) > 3)
                                {
                                    builder.Append("\\n");
                                }
                            }
                        }
                    }
                }

                if (dfaState.IsAcceptState)
                {
                    builder.Append("⇒" + dfaState.GetUniquelyPredictedAlt());
                }

                stateLabel = builder.ToString();
            }
            else if (nfaState != null)
            {
                if (nfaState.IsDecisionState)
                    stateLabel += ",d=" + nfaState.DecisionNumber;

                if (nfaState.endOfBlockStateNumber != State.INVALID_STATE_NUMBER)
                    stateLabel += ",eob=" + nfaState.endOfBlockStateNumber;
            }

            return stateLabel;
        }

        private XElement GetNodes()
        {
            return new XElement(Elements.Nodes, _nodes.Values.Concat(_extraNodes));
        }

        private XElement GetLinks()
        {
            return new XElement(Elements.Links, _links.Values.Concat(_extraLinks));
        }

        private static XElement GetCategories()
        {
            return new XElement(Elements.Categories,
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.GrammarRule),
                    new XAttribute(Attributes.FontFamily, "Consolas")),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.LexerIdentifier),
                    new XAttribute(Attributes.BasedOn, Categories.LexerRule),
                    new XAttribute(Attributes.Foreground, Colors.Blue)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.LexerLiteral),
                    new XAttribute(Attributes.BasedOn, Categories.LexerRule),
                    new XAttribute(Attributes.Foreground, Colors.DarkGreen)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.LexerRule),
                    new XAttribute(Attributes.BasedOn, Categories.GrammarRule),
                    new XAttribute(Attributes.NodeRadius, 0)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.ParserRule),
                    new XAttribute(Attributes.BasedOn, Categories.GrammarRule),
                    new XAttribute(Attributes.Foreground, Colors.Purple)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.State)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.DecisionState),
                    new XAttribute(Attributes.BasedOn, Categories.State)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.StopState),
                    new XAttribute(Attributes.BasedOn, Categories.State)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.Edge)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.ActionEdge),
                    new XAttribute(Attributes.BasedOn, Categories.Edge),
                    new XAttribute(Attributes.FontFamily, "Consolas")),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.EpsilonEdge),
                    new XAttribute(Attributes.FontFamily, "Times New Roman"),
                    new XAttribute(Attributes.FontStyle, "Italic")),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.RuleClosureEdge),
                    new XAttribute(Attributes.BasedOn, Categories.Edge)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.AtomEdge),
                    new XAttribute(Attributes.BasedOn, Categories.Edge)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.OptimizedEdge),
                    new XAttribute(Attributes.Stroke, Colors.Red),
                    new XAttribute(Attributes.Visibility, "Collapsed")),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.VerboseNode)),
                new XElement(Elements.Category,
                    new XAttribute(Attributes.Id, Categories.Contains),
                    new XAttribute(Attributes.IsContainment, true),
                    new XAttribute(Attributes.Label, Categories.Contains),
                    new XAttribute(Attributes.CanBeDataDriven, false),
                    new XAttribute(Attributes.CanLinkedNodesBeDataDriven, true),
                    new XAttribute(Attributes.IncomingActionLabel, "Contained By"),
                    new XAttribute(Attributes.OutgoingActionLabel, "Contains")));
        }

        private static XElement GetProperties()
        {
            return new XElement(Elements.Properties,
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.FontFamily),
                    new XAttribute(Attributes.DataType, "System.Windows.Media.FontFamily")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.Foreground),
                    new XAttribute(Attributes.Label, "Foreground"),
                    new XAttribute(Attributes.Description, "The foreground color"),
                    new XAttribute(Attributes.DataType, "System.Windows.Media.Brush")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.GraphDirection),
                    new XAttribute(Attributes.DataType, "Microsoft.VisualStudio.Progression.Layout.GraphDirection")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.Label),
                    new XAttribute(Attributes.Label, "Label"),
                    new XAttribute(Attributes.Description, "Displayable label of an Annotatable object"),
                    new XAttribute(Attributes.DataType, "System.String")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.Layout),
                    new XAttribute(Attributes.DataType, "System.String")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.NodeRadius),
                    new XAttribute(Attributes.DataType, "System.Double")),
                new XElement(Elements.Property,
                    new XAttribute(Attributes.Id, Attributes.Shape),
                    new XAttribute(Attributes.DataType, "System.String")));
        }

        private static XElement GetStyles()
        {
            return new XElement(Elements.Styles,
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Node"),
                    new XAttribute(Attributes.GroupLabel, "Verbose State"),
                    new XAttribute(Attributes.ValueLabel, "Verbose State"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.VerboseNode))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.Background),
                        new XAttribute(Attributes.Value, Colors.LightYellow))),
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Node"),
                    new XAttribute(Attributes.GroupLabel, "Stop State"),
                    new XAttribute(Attributes.ValueLabel, "Stop State"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.StopState))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.Stroke),
                        new XAttribute(Attributes.Value, Colors.Black)),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.StrokeThickness),
                        new XAttribute(Attributes.Value, 2))),
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Node"),
                    new XAttribute(Attributes.GroupLabel, "Decision State"),
                    new XAttribute(Attributes.ValueLabel, "Decision State"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.DecisionState))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.Stroke),
                        new XAttribute(Attributes.Value, Colors.Black)),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.StrokeThickness),
                        new XAttribute(Attributes.Value, 1)),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.StrokeDashArray),
                        new XAttribute(Attributes.Value, "2,2"))),
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Link"),
                    new XAttribute(Attributes.GroupLabel, "Epsilon Edge"),
                    new XAttribute(Attributes.ValueLabel, "Epsilon Edge"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.EpsilonEdge))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.StrokeDashArray),
                        new XAttribute(Attributes.Value, "2,2"))),
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Link"),
                    new XAttribute(Attributes.GroupLabel, "Rule Closure Edge"),
                    new XAttribute(Attributes.ValueLabel, "Rule Closure Edge"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.RuleClosureEdge))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.Stroke),
                        new XAttribute(Attributes.Value, Colors.Purple)),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.FontFamily),
                        new XAttribute(Attributes.Value, "Consolas"))),
                new XElement(Elements.Style,
                    new XAttribute(Attributes.TargetType, "Link"),
                    new XAttribute(Attributes.GroupLabel, "Atom Edge"),
                    new XAttribute(Attributes.ValueLabel, "Atom Edge"),
                    new XElement(Elements.Condition,
                        new XAttribute(Attributes.Expression, string.Format("HasCategory('{0}')", Categories.AtomEdge))),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.Stroke),
                        new XAttribute(Attributes.Value, Colors.DarkBlue)),
                    new XElement(Elements.Setter,
                        new XAttribute(Attributes.Property, Attributes.FontFamily),
                        new XAttribute(Attributes.Value, "Consolas"))));

        }

        private static class GraphDirection
        {
            public const string LeftToRight = "LeftToRight";
            public const string TopToBottom = "TopToBottom";
            public const string RightToLeft = "TopToBottom";
            public const string BottomToTop = "BottomToTop";
        }

        private static class Layout
        {
            public const string None = "None";
            public const string Sugiyama = "Sugiyama";
            public const string ForceDirected = "ForceDirected";
            public const string DependencyMatrix = "DependencyMatrix";
        }

        private static class Elements
        {
            private static readonly XNamespace ns = "http://schemas.microsoft.com/vs/2009/dgml";
            public static readonly XName DirectedGraph = ns + "DirectedGraph";
            public static readonly XName Nodes = ns + "Nodes";
            public static readonly XName Node = ns + "Node";
            public static readonly XName Links = ns + "Links";
            public static readonly XName Link = ns + "Link";
            public static readonly XName Categories = ns + "Categories";
            public static readonly XName Category = ns + "Category";
            public static readonly XName Properties = ns + "Properties";
            public static readonly XName Property = ns + "Property";
            public static readonly XName Styles = ns + "Styles";
            public static readonly XName Style = ns + "Style";
            public static readonly XName Condition = ns + "Condition";
            public static readonly XName Setter = ns + "Setter";
        }

        private static class Attributes
        {
            public const string Id = "Id";
            public const string GraphDirection = "GraphDirection";
            public const string Layout = "Layout";
            public const string FontFamily = "FontFamily";
            public const string BasedOn = "BasedOn";
            public const string Background = "Background";
            public const string Foreground = "Foreground";
            public const string NodeRadius = "NodeRadius";
            public const string DataType = "DataType";
            public const string Label = "Label";
            public const string Source = "Source";
            public const string Target = "Target";
            public const string Category = "Category";
            public const string Shape = "Shape";
            public const string Description = "Description";
            public const string FontStyle = "FontStyle";
            public const string Ref = "Ref";
            public const string Stroke = "Stroke";
            public const string StrokeThickness = "StrokeThickness";
            public const string StrokeDashArray = "StrokeDashArray";
            public const string Visibility = "Visibility";
            public const string Expression = "Expression";
            public const string Property = "Property";
            public const string Value = "Value";
            public const string TargetType = "TargetType";
            public const string GroupLabel = "GroupLabel";
            public const string ValueLabel = "ValueLabel";
            public const string Reference = "Reference";
            public const string IsContainment = "IsContainment";
            public const string CanBeDataDriven = "CanBeDataDriven";
            public const string CanLinkedNodesBeDataDriven = "CanLinkedNodesBeDataDriven";
            public const string IncomingActionLabel = "IncomingActionLabel";
            public const string OutgoingActionLabel = "OutgoingActionLabel";
            public const string Group = "Group";
        }

        private static class Categories
        {
            public const string GrammarRule = "GrammarRule";
            public const string LexerIdentifier = "LexerIdentifier";
            public const string LexerLiteral = "LexerLiteral";
            public const string LexerRule = "LexerRule";
            public const string ParserRule = "ParserRule";
            public const string OptimizedEdge = "OptimizedEdge";
            public const string VerboseNode = "VerboseNode";
            public const string EpsilonEdge = "EpsilonEdge";
            public const string ActionEdge = "ActionEdge";
            public const string RuleClosureEdge = "RuleClosureEdge";
            public const string AtomEdge = "AtomEdge";
            public const string Edge = "Edge";
            public const string State = "State";
            public const string DecisionState = "DecisionState";
            public const string StopState = "StopState";
            public const string Contains = "Contains";
        }

        private static class Colors
        {
            public const string DarkBlue = "DarkBlue";
            public const string DarkGreen = "#FF008000";
            public const string Purple = "#FF800080";
            public const string Blue = "#FF00008B";
            public const string Red = "#FFFF0000";
            public const string Black = "#FF000000";
            public const string LightYellow = "LightYellow";
        }

        private static class Shapes
        {
            public const string None = "None";
            public const string Rectangle = "Rectangle";
        }
    }
}
