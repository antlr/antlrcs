/*
 * [The "BSD licence"]
 * Copyright (c) 2005-2008 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2008-2009 Sam Harwell
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

namespace StringTemplate
{
    using ArgumentException = System.ArgumentException;
    using ArgumentNullException = System.ArgumentNullException;
    using System.Text.RegularExpressions;

    public sealed class TemplateName
    {
        public const char TemplateDirectorySeparator = '/';
        public static readonly TemplateName Root = new TemplateName("/");

        private static readonly Regex FullNameValidator = new Regex(@"^(?:[a-z_][a-z0-9_\-]*)?(?:/[a-z_][a-z0-9_\-]*)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _name;

        public TemplateName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0 || (name != TemplateDirectorySeparator.ToString() && !FullNameValidator.IsMatch(name)))
                throw new ArgumentException("name is not in the correct format", "name");

            this._name = name;
        }

        public string FullName
        {
            get
            {
                return _name;
            }
        }

        public bool IsRooted
        {
            get
            {
                return _name[0] == TemplateDirectorySeparator;
            }
        }

        public string Name
        {
            get
            {
                int lastSeparator = _name.LastIndexOf(TemplateDirectorySeparator);
                if (lastSeparator < 0)
                    return _name;

                return _name.Substring(lastSeparator + 1);
            }
        }

        public static TemplateName Combine(TemplateName left, string right)
        {
            return Combine(left, new TemplateName(right));
        }

        public static TemplateName Combine(TemplateName left, TemplateName right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            if (left == Root)
                return right.IsRooted ? right : new TemplateName(TemplateDirectorySeparator + right.FullName);

            if (right.IsRooted)
                return new TemplateName(left.FullName + right.FullName);
            else
                return new TemplateName(left.FullName + TemplateDirectorySeparator + right.FullName);
        }

        public static bool operator ==(TemplateName left, TemplateName right)
        {
            if (object.ReferenceEquals(left, null))
                return object.ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(TemplateName left, TemplateName right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            TemplateName other = obj as TemplateName;
            if (other == null)
                return false;

            return other._name == _name;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
