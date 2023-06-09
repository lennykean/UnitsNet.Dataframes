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
    private readonly DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframeEnumerable(
        IEnumerable dataframes,
        DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
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

    public DataframeMetadata<TMetadataAttribute, TMetadata> GetDataframeMetadata(CultureInfo? culture)
    {
        ((IDataframeMetadataProvider<TMetadataAttribute, TMetadata>)_dynamicMetadataProvider).ValidateAllMetadata(typeof(TDataframe));

        return new(((IDataframeMetadataProvider<TMetadataAttribute, TMetadata>)_dynamicMetadataProvider).GetAllMetadata(typeof(TDataframe), culture));
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        return _dynamicMetadataProvider.TryGetMetadata(property, out metadata, culture);
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        _dynamicMetadataProvider.ValidateMetadata(property);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
