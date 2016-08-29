namespace Antlr.Runtime.Misc
{
    using System.Text.RegularExpressions;

#if PORTABLE || NET_STANDARD
    using System;
#endif

    internal static class RegexOptionsHelper
    {
        public static readonly RegexOptions Compiled;

        static RegexOptionsHelper()
        {
#if !PORTABLE && !NO_REGEXOPTIONS_COMPILED
            Compiled = RegexOptions.Compiled;
#else
            if (!Enum.TryParse("Compiled", out Compiled))
                Compiled = RegexOptions.None;
#endif
        }
    }
}
