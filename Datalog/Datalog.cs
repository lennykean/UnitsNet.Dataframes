using System.IO;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.KPro;

namespace HondataDotNet.Datalog
{
    public static class Datalog
    {
        private const int TYPE_IDENTIFIER_LENGTH = 6;

        public static IDatalog FromStream(Stream stream)
        {
            var buffer = new byte[TYPE_IDENTIFIER_LENGTH];
            stream.Read(buffer, 0, TYPE_IDENTIFIER_LENGTH);

            if (KProDatalog.IsValidIdentifier(buffer))
                return KProDatalog.FromStream(stream, preValidate: false);

            throw new InvalidDatalogFormatException($"Identifier \"{buffer}\" does not indicate a valid datalog");
        }

        public static IDatalog FromFile(string filename)
        {
            try
            {
                using var fileStream = File.OpenRead(filename);
                var datalog = FromStream(fileStream);

                return datalog;
            }
            catch (InvalidDatalogFormatException idfEx)
            {
                throw new InvalidDatalogFileException($"File {filename} is not a valid datalog file.", idfEx);
            }
        }
    }
}
