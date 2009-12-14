namespace AntlrUnitTests.ST4
{
    using System.Collections.Generic;
    using StringTemplate;
    using StringBuilder = System.Text.StringBuilder;

    public class ErrorBuffer : ITemplateErrorListener
    {
        private List<TemplateMessage> errors = new List<TemplateMessage>();

        public void CompileTimeError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public void RuntimeError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public void IOError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public void InternalError(TemplateMessage msg)
        {
            errors.Add(msg);
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            foreach (TemplateMessage m in errors)
            {
                buf.Append(m.ToString());
            }
            return buf.ToString();
        }
    }
}
