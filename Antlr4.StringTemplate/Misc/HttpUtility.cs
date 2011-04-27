/* Copyright Microsoft Corporation.
 *
 * This code is specifically included in this project to remove a dependency
 * on System.Web (a standard .NET assembly). System.Web is not part of the
 * .NET Framework Client Profile, so including the dependency is expensive in
 * terms of end user requirements.
 */

namespace Antlr4.StringTemplate.Misc
{
    using ArgumentNullException = System.ArgumentNullException;
    using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
    using Encoding = System.Text.Encoding;

    internal static class HttpUtility
    {
        internal static string UrlEncode(string str)
        {
            if (str == null)
                return null;

            return UrlEncode(str, Encoding.UTF8);
        }

        internal static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return null;

            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        internal static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            byte[] bytes = e.GetBytes(str);
            return UrlEncode(bytes, 0, bytes.Length, false);
        }

        internal static byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
        {
            byte[] buffer = UrlEncode(bytes, offset, count);
            if (alwaysCreateNewReturnValue && buffer != null && buffer == bytes)
                return (byte[])buffer.Clone();

            return buffer;
        }

        internal static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(bytes, offset, count))
                return null;

            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (ch == ' ')
                    num++;
                else if (!IsUrlSafeChar(ch))
                    num2++;
            }

            if (num == 0 && num2 == 0)
                return bytes;

            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsUrlSafeChar(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }

            return buffer;
        }

        internal static bool IsUrlSafeChar(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                return true;

            switch (ch)
            {
            case '(':
            case ')':
            case '*':
            case '-':
            case '.':
            case '_':
            case '!':
                return true;
            }

            return false;
        }

        internal static char IntToHex(int n)
        {
            if (n <= 9)
                return (char)(n + 0x30);

            return (char)((n - 10) + 0x61);
        }

        private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
        {
            if (bytes == null && count == 0)
                return false;

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (offset < 0 || offset > bytes.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || (offset + count) > bytes.Length)
                throw new ArgumentOutOfRangeException("count");

            return true;
        }
    }
}
