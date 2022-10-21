using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProFrameCollection : IReadOnlyCollection<KProFrame>
    {
        private readonly SortedSet<KProFrame> _frames;
        private readonly KProDatalog _datalog;

        internal KProFrameCollection(KProDatalog datalog)
        {
            _frames = new(new FrameComparer());
            _datalog = datalog;
        }

        public int Count => _frames.Count;

        public void Add(KProFrame item)
        {
            _frames.Add(item);

            item.Datalog = _datalog;
        }

        public bool Remove(KProFrame item)
        {
            var removed = _frames.Remove(item);
            if (removed)
                item.Datalog = null;

            return removed;
        }

        public void Clear()
        {
            foreach (var frame in _frames)
            {
                frame.Datalog = null;
            }
            _frames.Clear();
        }

        public bool Contains(KProFrame item)
        {
            return _frames.Contains(item);
        }

        public void CopyTo(KProFrame[] array, int arrayIndex)
        {
            _frames.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KProFrame> GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Save(Stream stream, int frameSize)
        {
            foreach (var (frame, frameNumber) in _frames.Select((frame, frameNumber) => (frame, frameNumber)))
            {
                frame.Save(stream, frameNumber, frameSize);
            }
        }

        internal static KProFrameCollection ReadFramesFromStream(Stream stream, KProDatalog datalog, int frameCount, int frameSize)
        {
            var frames = new KProFrameCollection(datalog);

            for (var i = 0; i < frameCount; i++)
            {
                frames.Add(KProFrame.ReadFromStream(stream, frameSize));
            }
            return frames;
        }
    }
}
