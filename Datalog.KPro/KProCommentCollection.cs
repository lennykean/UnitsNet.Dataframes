using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProCommentCollection : IReadWriteCollection<KProComment>
    {
        private readonly List<KProComment> _comments;

        private KProCommentCollection(List<KProComment> comments)
        {
            _comments = comments;
        }

        public int Count => _comments.Count;

        public bool IsReadOnly => false;

        public void Add(KProComment item)
        {
            _comments.Add(item);
        }

        public void Clear()
        {
            _comments.Clear();
        }

        public bool Contains(KProComment item)
        {
            return _comments.Contains(item);
        }

        public void CopyTo(KProComment[] array, int arrayIndex)
        {
            _comments.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KProComment> GetEnumerator()
        {
            return _comments.GetEnumerator();
        }

        public bool Remove(KProComment item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static KProCommentCollection FromFooter(byte[] footer, int length)
        {
            using var stream = new MemoryStream(footer);
            using var reader = new BinaryReader(stream);
            
            reader.ReadBytes(4);

            var comments = new List<KProComment>(length);

            for (var i = 0; i < length; i++)
            {
                comments.Add(KProComment.ReadFromFromBuffer(reader));
            }
            return new KProCommentCollection(comments);
        }
    }
}
