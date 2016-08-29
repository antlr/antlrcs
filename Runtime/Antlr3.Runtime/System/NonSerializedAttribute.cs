#if PORTABLE || NO_BINARY_SERIALIZATION

namespace System 
{
    [AttributeUsage(AttributeTargets.Field, Inherited=false)]
    internal sealed class NonSerializedAttribute : Attribute 
    {
    }
}

#endif
