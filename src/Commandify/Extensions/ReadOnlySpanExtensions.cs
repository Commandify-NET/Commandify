using Commandify.Enumerators;

namespace Commandify.Extensions;

public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpanSplitEnumerator Split(this ReadOnlySpan<char> source) => new(source);
}