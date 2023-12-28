namespace Commandify.Enumerators;

public ref struct ReadOnlySpanSplitEnumerator
{
    private ReadOnlySpan<char> _span;

    private ReadOnlySpan<char> _previousSpan;

    public ReadOnlySpan<char> Current { get; private set; }

    public char Delimiter { get; private set; }

    internal ReadOnlySpanSplitEnumerator(ReadOnlySpan<char> span)
    {
        _span = span;
        _previousSpan = ReadOnlySpan<char>.Empty;
        Current = default;
    }

    public ReadOnlySpanSplitEnumerator GetEnumerator()
    {
        return this;
    }

    public bool IsEmpty
    {
        get
        {
            bool nextIsAvailable = MoveNext();

            if (nextIsAvailable)
                MoveBack();
            
            return nextIsAvailable;
        }
    }
    
    public bool MoveBack()
    {
        if (!_previousSpan.IsEmpty)
        {
            _span = _previousSpan;

            return true;
        }

        return false;
    }
    
    public bool MoveNext()
    {
        while (!_span.IsEmpty)
        {
            bool inQuotes = false;
            char quoteChar = default;
            int segmentStart = 0;

            for (int i = 0; i < _span.Length; i++)
            {
                char currentChar = _span[i];
                bool isQuote = currentChar == '\'' || currentChar == '\"';
                bool isStartOfSpan = i == 0;
                bool isEndOfSpan = i == _span.Length - 1;
                bool isSpaceBefore = isStartOfSpan || _span[i - 1] == ' ';
                bool isSpaceAfter = isEndOfSpan || _span[i + 1] == ' ';

                if (isQuote && !inQuotes && isSpaceBefore && !isSpaceAfter)
                {
                    inQuotes = true;
                    quoteChar = currentChar;
                    segmentStart = i;
                    continue;
                }

                if (isQuote && inQuotes && currentChar == quoteChar && isSpaceAfter)
                {
                    Current = _span.Slice(segmentStart + 1, i - segmentStart - 1);
                    Delimiter = currentChar;

                    _previousSpan = _span;
                    _span = _span.Slice(i + 1);
                    inQuotes = false;
                    return true;
                }

                if (!inQuotes && currentChar == ' ')
                {
                    if (i > segmentStart)
                    {
                        Current = _span.Slice(segmentStart, i - segmentStart);
                        Delimiter = currentChar;
                        _previousSpan = _span;
                        _span = _span.Slice(i + 1);
                        return true;
                    }

                    segmentStart = i + 1;
                }
            }

            if (segmentStart < _span.Length)
            {
                Current = _span.Slice(segmentStart);
                Delimiter = inQuotes ? quoteChar : ' ';
                _previousSpan = _span;
                _span = ReadOnlySpan<char>.Empty;
                return true;
            }

            return false;
        }

        return false;
    }

    public readonly ReadOnlySpan<char> AsSpan() => _span;
}