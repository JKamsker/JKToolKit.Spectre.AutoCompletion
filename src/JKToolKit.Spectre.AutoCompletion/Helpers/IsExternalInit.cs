// Polyfill for netstandard2.0 to support C# 9+ record types and init-only properties
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit { }
}
#endif
