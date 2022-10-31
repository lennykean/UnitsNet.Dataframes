using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.KPro
{
    public sealed partial class KProDatalogFrameComment : IDatalogFrameComment
    {
        private Metadata _metadata;

        public KProDatalogFrameComment(TimeSpan offset, string comment)
        {
            _metadata.Offset = offset.TotalSeconds;
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private KProDatalogFrameComment()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public TimeSpan Offset => TimeSpan.FromSeconds(_metadata.Offset);

        public string Comment { get; private set; }

        public override string ToString()
        {
            return $"{nameof(KProDatalogFrameComment)}({Offset}, \"{Comment}\")";
        }

        internal static KProDatalogFrameComment ReadFromStream(Stream stream)
        {
            var datalogComment = new KProDatalogFrameComment
            {
                _metadata = stream.ReadStruct<Metadata>()
            };

            var comment = new byte[datalogComment._metadata.Length];
            stream.Read(comment, 0, comment.Length);
            datalogComment.Comment = Encoding.ASCII.GetString(comment);

            return datalogComment;
        }

        internal void Save(Stream stream)
        {
            _metadata.Length = Comment.Length;

            stream.WriteStruct(_metadata);
            stream.Write(Encoding.ASCII.GetBytes(Comment));
        }
    }
}
