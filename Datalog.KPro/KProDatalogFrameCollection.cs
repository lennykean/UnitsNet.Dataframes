using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProDatalogFrameCollection : IReadWriteCollection<KProDatalogFrame>
    {
        private readonly SortedSet<KProDatalogFrame> _frames;
        private readonly KProDatalog _datalog;

        internal KProDatalogFrameCollection(KProDatalog datalog)
        {
            _frames = new(new TimeSeriesComparer());
            _datalog = datalog;
        }

        public int Count => _frames.Count;

        public bool IsReadOnly => false;

        public void Add(KProDatalogFrame item)
        {
            _frames.Add(item);

            item.Datalog = _datalog;
        }

        public bool Remove(KProDatalogFrame item)
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

        public bool Contains(KProDatalogFrame item)
        {
            return _frames.Contains(item);
        }

        public void CopyTo(KProDatalogFrame[] array, int arrayIndex)
        {
            _frames.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KProDatalogFrame> GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static KProDatalogFrameCollection ReadFromStream(Stream stream, KProDatalog datalog, int frameCount, int frameSize)
        {
            var frames = new KProDatalogFrameCollection(datalog);

            for (var i = 0; i < frameCount; i++)
            {
                frames.Add(KProDatalogFrame.ReadFromStream(stream, frameSize));
            }
            return frames;
        }

        internal void Save(Stream stream, int frameSize)
        {
            foreach (var (frame, frameNumber) in _frames.Select((frame, frameNumber) => (frame, frameNumber)))
            {
                frame.Save(stream, frameNumber, frameSize);
            }
        }
    }
}
