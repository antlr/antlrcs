namespace Antlr3.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr4.StringTemplate;
    using TextWriter = System.IO.TextWriter;

    public static class TemplateExtensions
    {
        public static Template SetAttribute(this Template template, string name, params object[] value)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Invalid attribute name", "name");

            if (name[name.Length - 1] == '}')
                return template.AddMany(name, value);
            else if (value.Length == 1)
                return template.Add(name, value[0]);
            else
                return template.Add(name, value);
        }

        public static void RemoveAttribute(this Template template, string name)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Invalid attribute name", "name");

            template.Remove(name);
        }

        public static Template GetInstanceOf(this Template template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            return new Template(template);
        }

        public static ITemplateWriter GetStringTemplateWriter(this TemplateGroup group, TextWriter writer)
        {
            if (group == null)
                throw new ArgumentNullException("group");
            if (writer == null)
                throw new ArgumentNullException("writer");

            return new AutoIndentWriter(writer);
        }
    }
}
