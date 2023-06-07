using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using Newtonsoft.Json;

using NUnit.Framework;

using UnitsNet.Dataframes.Tests.TestData;

namespace UnitsNet.Dataframes.Tests.DataframeExtensions;

[TestFixture]
public class GetDataframeMetadata
{
    [TestCase(TestName = "{c} (gets metadata)")]
    public void GetDataframeMetadataTests()
    {
        var box = new Box();

        var metadata = box.GetDataframeMetadata();

        var json = JsonConvert.SerializeObject(metadata);

    }
}
