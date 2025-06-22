using Azure;
using Optional;

public static class OptionExtensions
{
    public static Option<T> ToOption<T>(this NullableResponse<T> nullableResponse)
    {
        return nullableResponse.Value is not null ? Option.Some(nullableResponse.Value) : Option.None<T>();
    }
}
