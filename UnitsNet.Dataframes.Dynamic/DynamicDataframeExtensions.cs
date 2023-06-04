namespace UnitsNet.Dataframes.Dynamic;

public static class DynamicDataframeExtensions
{
    public static DynamicDataframesBuilder<TDataframe> AsDynamicDataframes<TDataframe>(this IEnumerable<TDataframe> dataframes) where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe>(dataframes);
    }

    public static DynamicDataframesBuilder<TDataframe> AsDynamicDataframe<TDataframe>(this TDataframe dataframe) where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe>(new[] { dataframe });
    }
}
