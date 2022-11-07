using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed class FlashProDatalogFrameCollection : IReadWriteCollection<FlashProDatalogFrame>
    {
        private readonly SortedSet<FlashProDatalogFrame> _frames;
        private readonly FlashProDatalog _datalog;

        internal FlashProDatalogFrameCollection(FlashProDatalog datalog)
        {
            _frames = new(new TimeSeriesComparer());
            _datalog = datalog;
        }

        public int Count => _frames.Count;

        public bool IsReadOnly => false;

        public void Add(FlashProDatalogFrame item)
        {
            _frames.Add(item);

            item.Datalog = _datalog;
        }

        public bool Remove(FlashProDatalogFrame item)
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

        public bool Contains(FlashProDatalogFrame item)
        {
            return _frames.Contains(item);
        }

        public void CopyTo(FlashProDatalogFrame[] array, int arrayIndex)
        {
            _frames.CopyTo(array, arrayIndex);
        }

        public IEnumerator<FlashProDatalogFrame> GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal static FlashProDatalogFrameCollection ReadFromStream(Stream stream, FlashProDatalog datalog, int frameCount, int frameSize)
        {
            var frames = new FlashProDatalogFrameCollection(datalog);

            for (var i = 0; i < frameCount; i++)
            {
                frames.Add(FlashProDatalogFrame.ReadFromStream(stream, frameSize));
            }
            return frames;
        }

        internal int Save(Stream stream, int frameSize)
        {
            var bytesSaved = 0;
            foreach (var (frame, frameNumber) in _frames.Select((frame, frameNumber) => (frame, frameNumber)))
            {
                bytesSaved += frame.Save(stream, frameNumber, frameSize);
            }
            return bytesSaved;
        }
    }
}
