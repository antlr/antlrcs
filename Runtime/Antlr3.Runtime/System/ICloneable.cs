#if PORTABLE || NO_BINARY_SERIALIZATION

namespace System
{
    internal interface ICloneable
    {
        object Clone();
    }
}

#endif
