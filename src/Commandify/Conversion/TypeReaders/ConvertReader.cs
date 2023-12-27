using Commandify.Abstractions.Conversion.TypeReaders;
using Commandify.Abstractions.Types.Results;

namespace Commandify.Conversion.TypeReaders;

public class ConvertReader : ITypeReader
{
    public bool IsSupported(Type type)
    {
        var code = Type.GetTypeCode(type);

        if (code is TypeCode.Empty or TypeCode.Object or TypeCode.DBNull or TypeCode.Object)
        {
            return false;
        }

        return true;
    }

    public TypeReadResult Read(ReadOnlySpan<char> input, Type type)
    {
        try
        {
            var result = Convert.ChangeType(input.ToString(), type);

            if (result is not { })
            {
                return new TypeReadResult(true, result!);
            }
        }
        catch
        {
            return new TypeReadResult(false, null!);
        }
        
        return new TypeReadResult(false, null!);
    }
}