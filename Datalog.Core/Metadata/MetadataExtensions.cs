using System.Collections.Generic;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Metadata;

namespace UnitsNet.Metadata
{
    public static class MetadataExtensions
    {
        public static DatalogFrameMetadata GetDatalogFrameMetadata<TDatalogFrame>(this TDatalogFrame _) where TDatalogFrame : IDatalogFrame
        {
            return new DatalogFrameMetadata(typeof(TDatalogFrame));
        }

        public static DatalogFrameMetadata GetDatalogFrameMetadata<TDatalogFrame>(this IEnumerable<TDatalogFrame> _) where TDatalogFrame : IDatalogFrame
        {
            return new DatalogFrameMetadata(typeof(TDatalogFrame));
        }
    }
}