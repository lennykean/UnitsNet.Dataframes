using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata> : IDynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata>
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    private readonly ConcurrentDictionary<TDataframe, TDataframe> _proxyCache = new();

    private readonly IEnumerable _dataframes;
    private readonly DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframeEnumerable(
        IEnumerable dataframes,
        DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));
        _dynamicMetadataProvider = dynamicMetadataProvider ?? throw new ArgumentNullException(nameof(dynamicMetadataProvider));
    }

    public IEnumerator<TDataframe> GetEnumerator()
    {
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(_dynamicMetadataProvider);

        var proxyGenerator = new ProxyGenerator();
        var interceptor = new DynamicQuantityInterceptor<TDataframe, TMetadataAttribute, TMetadata>(_dynamicMetadataProvider);

        Func<object, TDataframe> createProxy = typeof(TDataframe).IsInterface
            ? dataframe => (TDataframe)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor)
            : dataframe => (TDataframe)proxyGenerator.CreateClassProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor);

        foreach (TDataframe dataframe in _dataframes)
            yield return _proxyCache.GetOrAdd(dataframe, _ => createProxy(dataframe));
    }

    DataframeMetadata<TMetadataAttribute, TMetadata> IDynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata>.GetDataframeMetadata(CultureInfo? culture)
    {
        return new(_dynamicMetadataProvider.GetAllMetadata(culture));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerable<TMetadata> IDataframeMetadataProvider<TMetadataAttribute, TMetadata>.GetAllMetadata(CultureInfo? culture)
    {
        return _dynamicMetadataProvider.GetAllMetadata(culture);
    }

    bool IDataframeMetadataProvider<TMetadataAttribute, TMetadata>.TryGetMetadata(PropertyInfo property, [NotNullWhen(true)]out TMetadata? metadata, CultureInfo? culture)
    {
        return _dynamicMetadataProvider.TryGetMetadata(property, out metadata, culture);
    }

    public void ValidateAllMetadata()
    {
        _dynamicMetadataProvider.ValidateAllMetadata();
    }
}
