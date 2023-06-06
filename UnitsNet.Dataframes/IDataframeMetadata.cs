//using System.Collections.Generic;
//using System.Globalization;
//using System.Reflection;

//using UnitsNet.Dataframes.Attributes;

//namespace UnitsNet.Dataframes;

//public interface IDataframeMetadata<TMetadataAttribute, TMetadata> : IEnumerable<TMetadata>
//    where TMetadataAttribute : QuantityAttribute, IDataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
//    where TMetadata : QuantityMetadata
//{
//    public interface IMetadataFactory 
//    {
//        TMetadata Create(PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null);
//    }
//}
