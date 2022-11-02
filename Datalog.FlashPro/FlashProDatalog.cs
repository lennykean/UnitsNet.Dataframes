using System;
using System.Collections.Generic;
using System.IO;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed partial class FlashProDatalog : IFlashProDatalog
    {
        public TimeSpan Duration => throw new NotImplementedException();
        public Version Version => throw new NotImplementedException();
        public IReadWriteCollection<FlashProDatalogFrame> Frames => throw new NotImplementedException();
        IReadOnlyCollection<IDatalogFrame> IDatalog.Frames => throw new NotImplementedException();
        public IReadWriteCollection<FlashProDatalogFrameComment> Comments => throw new NotImplementedException();
        IReadOnlyCollection<IDatalogFrameComment> IDatalog.Comments => throw new NotImplementedException();

        public static FlashProDatalog FromStream(Stream stream, bool preValidate = true)
        {
            throw new NotImplementedException();
        }

        public static FlashProDatalog FromFile(string filename)
        {
            try
            {
                using var fileStream = File.OpenRead(filename);
                var datalog = FromStream(fileStream);

                return datalog;
            }
            catch (InvalidDatalogFormatException idfEx)
            {
                throw new InvalidDatalogFileException($"File {filename} is not a valid KPro datalog file.", idfEx);
            }
        }

        public static bool HasValidIdentifier(byte[] buffer)
        {
            return false;
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
