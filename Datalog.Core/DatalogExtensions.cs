using System.IO;

namespace HondataDotNet.Datalog.Core
{
    public static class DatalogExtensions
    {
        public static void Save(this IDatalog datalog, string filename)
        {
            using var fileStream = File.Create(filename);
            datalog.Save(fileStream);
        }
    }
}
