using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicDataframes;

internal class DynamicDataframeEnumerable<TObject, TMetadataAttribute, TMetadata> : IDynamicDataframeEnumerable<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly ConcurrentDictionary<TObject, TObject> _proxyCache = new();

    private readonly IEnumerable _dataframes;
    private readonly DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframeEnumerable(
        IEnumerable dataframes,
        DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));
        _dynamicMetadataProvider = dynamicMetadataProvider ?? throw new ArgumentNullException(nameof(dynamicMetadataProvider));
    }

    public IEnumerator<TObject> GetEnumerator()
    {
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(_dynamicMetadataProvider);

        var proxyGenerator = new ProxyGenerator();
        var interceptor = new DynamicQuantityInterceptor<TObject, TMetadataAttribute, TMetadata>(_dynamicMetadataProvider);

        Func<object, TObject> createProxy = typeof(TObject).IsInterface
            ? proxyObj => (TObject)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TObject), proxyObj, options, interceptor)
            : proxyObj => (TObject)proxyGenerator.CreateClassProxyWithTarget(typeof(TObject), proxyObj, options, interceptor);

        foreach (TObject dataframe in _dataframes)
            yield return _proxyCache.GetOrAdd(dataframe, _ => createProxy(dataframe));
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        return _dynamicMetadataProvider.TryGetMetadata(property, out metadata, culture);
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        _dynamicMetadataProvider.ValidateMetadata(property);
    }

    public IMetadataDictionary<TMetadata> GetObjectMetadata(CultureInfo? culture)
    {
        var dynamicMetadataProvider = _dynamicMetadataProvider as IMetadataProvider<TMetadataAttribute, TMetadata>;

        dynamicMetadataProvider.ValidateAllMetadata(typeof(TObject));

        return new MetadataDictionary<TMetadata>(dynamicMetadataProvider.GetMetadata(typeof(TObject), culture));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
