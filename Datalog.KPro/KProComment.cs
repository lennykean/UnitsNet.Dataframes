using System;
using System.IO;
using System.Text;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProComment : IComment
    {
        public KProComment(TimeSpan offset, string comment)
        {
            this.offset = offset;
            Comment = comment;
        }

        public TimeSpan offset { get; private set; }

        public string Comment {get; private set;}

        internal static KProComment ReadFromFromBuffer(BinaryReader reader)
        {
            var offset = reader.ReadDouble();
            var length = reader.ReadInt32();
            var commentBuffer = reader.ReadBytes(length);
            var comment = Encoding.ASCII.GetString(commentBuffer);

            return new KProComment(TimeSpan.FromMilliseconds(offset), comment);
        }
    }
}
