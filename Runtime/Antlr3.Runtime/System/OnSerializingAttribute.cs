#if NETSTANDARD

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class OnSerializingAttribute : Attribute
    {
    }
}

#endif
