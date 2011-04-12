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

namespace Antlr4.StringTemplate.Visualizer.Extensions
{
    using System.Collections;
    using Antlr4.StringTemplate;
    using ArgumentNullException = System.ArgumentNullException;
    using CultureInfo = System.Globalization.CultureInfo;
    using IEnumerable = System.Collections.IEnumerable;

    internal static class ListExtensions
    {
        private static TemplateGroup _listRendererTemplateGroup;

        public static string ToListString(this IEnumerable list)
        {
            Template st = GetListRendererTemplate();
            st.Add("list", list);
            return st.Render();
        }

        public static string GetDescription(this AttributeViewModel attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            Template template = GetAttributeTemplate();
            template.Add("attr", attribute);
            return template.Render().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0");
        }

        private static Template GetAttributeTemplate()
        {
            return GetListRendererTemplateGroup().GetInstanceOf("attribute");
        }

        private static Template GetListRendererTemplate()
        {
            return GetListRendererTemplateGroup().GetInstanceOf("listTemplate");
        }

        private static Template GetAggregateRendererTemplate()
        {
            return GetListRendererTemplateGroup().GetInstanceOf("aggregateTemplate");
        }

        private static Template GetDictionaryRendererTemplate()
        {
            return GetListRendererTemplateGroup().GetInstanceOf("dictionaryTemplate");
        }

        private static TemplateGroup GetListRendererTemplateGroup()
        {
            if (_listRendererTemplateGroup == null)
            {
                _listRendererTemplateGroup = new TemplateGroupString("AttributeRendererTemplates", Properties.Resources.AttributeRendererTemplates);
                _listRendererTemplateGroup.RegisterRenderer(typeof(IEnumerable), new CollectionRenderer());
                _listRendererTemplateGroup.RegisterTypeProxyFactory(typeof(IDictionary), new DictionaryTypeProxyFactory());
                _listRendererTemplateGroup.RegisterTypeProxyFactory(typeof(Misc.Aggregate), new AggregateProxyFactory());
                _listRendererTemplateGroup.RegisterTypeProxyFactory(typeof(Template), new TemplateProxyFactory(_listRendererTemplateGroup));
            }

            return _listRendererTemplateGroup;
        }

        private class CollectionRenderer : IAttributeRenderer
        {
            public string ToString(object o, string formatString, CultureInfo culture)
            {
                string s = o as string;
                if (s != null)
                    return s;

                return ((IEnumerable)o).ToListString();
            }
        }

        private class DictionaryTypeProxyFactory : ITypeProxyFactory
        {
            public object CreateProxy(TemplateFrame frame, object obj)
            {
                if (frame.Template.Name == "dictionaryTemplate")
                    return obj;

                Template template = GetDictionaryRendererTemplate();
                template.Add("dict", obj);
                return template;
            }
        }

        private class AggregateProxyFactory : ITypeProxyFactory
        {
            public object CreateProxy(TemplateFrame frame, object obj)
            {
                if (frame.Template.Name == "dictionaryTemplate")
                    return obj;

                Template template = GetAggregateRendererTemplate();
                template.Add("aggr", obj);
                return template;
            }
        }

        private class TemplateProxyFactory : ITypeProxyFactory
        {
            private readonly TemplateGroup _ignoreGroup;

            public TemplateProxyFactory(TemplateGroup ignoreGroup)
            {
                _ignoreGroup = ignoreGroup;
            }

            public object CreateProxy(TemplateFrame frame, object obj)
            {
                Template template = obj as Template;
                if (template != null && template.Group != _ignoreGroup)
                    return new TemplateProxy(template);

                return obj;
            }
        }

        private class TemplateProxy
        {
            private readonly Template _template;

            public TemplateProxy(Template template)
            {
                if (template == null)
                    throw new ArgumentNullException("template");

                _template = template;
            }

            public override string ToString()
            {
                if (_template.impl == null)
                    return _template.ToString();

                return _template.impl.template;
            }
        }
    }
}
