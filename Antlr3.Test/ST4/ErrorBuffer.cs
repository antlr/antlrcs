namespace AntlrUnitTests.ST4
{
    using StringTemplate;
    using Exception = System.Exception;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;

    public class ErrorBuffer : ITemplateErrorListener
    {
        StringBuilder errorOutput = new StringBuilder(500);
        int n = 0;

        public void Error(string msg)
        {
            Error(msg, null);
        }

        public void Error(string msg, Exception e)
        {
            n++;
            if (n > 1)
            {
                errorOutput.Append('\n');
            }
            if (e != null)
            {
                StringWriter duh = new StringWriter();
                duh.WriteLine(e.StackTrace);
                errorOutput.Append(msg + ": " + duh.ToString());
            }
            else
            {
                errorOutput.Append(msg);
            }
        }

        public void Warning(string msg)
        {
            n++;
            errorOutput.Append(msg);
        }

        public override bool Equals(object o)
        {
            string me = ToString();
            string them = o.ToString();
            return me.Equals(them);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return errorOutput.ToString();
        }
    }
}
