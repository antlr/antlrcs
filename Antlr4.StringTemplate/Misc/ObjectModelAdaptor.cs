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

namespace Antlr4.StringTemplate.Misc
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ArgumentNullException = System.ArgumentNullException;
    using FieldInfo = System.Reflection.FieldInfo;
    using MethodInfo = System.Reflection.MethodInfo;
    using PropertyInfo = System.Reflection.PropertyInfo;
    using Type = System.Type;

    public class ObjectModelAdaptor : IModelAdaptor
    {
        private static readonly Dictionary<Type, Dictionary<string, System.Func<object, object>>> _memberAccessors =
            new Dictionary<Type, Dictionary<string, System.Func<object, object>>>();

        public virtual object GetProperty(Template self, object o, object property, string propertyName)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            Type c = o.GetType();
            if (property == null)
                throw new TemplateNoSuchPropertyException(string.Format("{0}.{1}", c.FullName, propertyName ?? "null"));

            object value = null;
            var accessor = FindMember(c, propertyName);
            if (accessor != null)
            {
                value = accessor(o);
            }
            else
            {
                throw new TemplateNoSuchPropertyException(string.Format("{0}.{1}", c.FullName, propertyName));
            }

            return value;
        }

        private static System.Func<object, object> FindMember(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            lock (_memberAccessors)
            {
                Dictionary<string, System.Func<object, object>> members;
                System.Func<object, object> accessor = null;

                if (_memberAccessors.TryGetValue(type, out members))
                {
                    if (members.TryGetValue(name, out accessor))
                        return accessor;
                }
                else
                {
                    members = new Dictionary<string, System.Func<object, object>>();
                    _memberAccessors[type] = members;
                }

                // must look up using reflection
                string methodSuffix = char.ToUpperInvariant(name[0]) + name.Substring(1);
                bool checkOriginalName = !string.Equals(methodSuffix, name);

                MethodInfo method = null;
                if (method == null)
                {
                    PropertyInfo p = type.GetProperty(methodSuffix);
                    if (p == null && checkOriginalName)
                        p = type.GetProperty(name);

                    if (p != null)
                        method = p.GetGetMethod();
                }

                if (method == null)
                {
                    method = type.GetMethod("Get" + methodSuffix, Type.EmptyTypes);
                    if (method == null && checkOriginalName)
                        method = type.GetMethod("Get" + name, Type.EmptyTypes);
                }

                if (method == null)
                {
                    method = type.GetMethod("get_" + methodSuffix, Type.EmptyTypes);
                    if (method == null && checkOriginalName)
                        method = type.GetMethod("get_" + name, Type.EmptyTypes);
                }

                if (method != null)
                {
                    accessor = BuildAccessor(method);
                }
                else
                {
                    // try for an indexer
                    method = type.GetMethod("get_Item", new Type[] { typeof(string) });
                    if (method == null)
                    {
                        var property = type.GetProperties().FirstOrDefault(IsIndexer);
                        if (property != null)
                            method = property.GetGetMethod();
                    }

                    if (method != null)
                    {
                        accessor = BuildAccessor(method, name);
                    }
                    else
                    {
                        // try for a visible field
                        FieldInfo field = type.GetField(name);
                        // also check .NET naming convention for fields
                        if (field == null)
                            field = type.GetField("_" + name);

                        if (field != null)
                            accessor = BuildAccessor(field);
                    }
                }

                members[name] = accessor;

                return accessor;
            }
        }

        private static bool IsIndexer(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var indexParameters = propertyInfo.GetIndexParameters();
            return indexParameters != null
                && indexParameters.Length > 0
                && indexParameters[0].ParameterType == typeof(string);
        }

        private static System.Func<object, object> BuildAccessor(MethodInfo method)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method),
                    typeof(object)),
                obj);

            return expr.Compile();
        }

        /// <summary>
        /// Builds an accessor for an indexer property that returns a takes a string argument.
        /// </summary>
        private static System.Func<object, object> BuildAccessor(MethodInfo method, string argument)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method,
                        Expression.Constant(argument)),
                    typeof(object)),
                obj);

            return expr.Compile();
        }

        private static System.Func<object, object> BuildAccessor(FieldInfo field)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
                Expression.Convert(
                    Expression.Field(
                        Expression.Convert(obj, field.DeclaringType),
                        field),
                    typeof(object)),
                obj);

            return expr.Compile();
        }
    }
}
