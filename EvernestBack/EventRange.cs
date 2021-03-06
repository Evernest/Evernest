﻿using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EvernestBack
{
    public class EventRange : IEnumerable<LowLevelEvent>
    {
        private readonly byte[] _src;
        private readonly int _offset;
        public int Size { get; private set; }

        /// <summary>
        /// Construct an EventRange with a byte array, an offset, and the number of bytes.
        /// </summary>
        /// <param name="src">The byte array.</param>
        /// <param name="offset">The offset where the data should be read.</param>
        /// <param name="size">The number of bytes to read.</param>
        internal EventRange(byte[] src, int offset, int size)
        {
            _src = src;
            _offset = offset;
            Size = size;
        }

        public EventRangeEnumerator GetEnumerator(int position)
        {
            return new EventRangeEnumerator(_src, _offset, Size, position);
        }

        public EventRangeEnumerator GetEnumerator()
        {
            return new EventRangeEnumerator(_src, _offset, Size);
        }

        IEnumerator<LowLevelEvent> IEnumerable<LowLevelEvent>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Try to create an EventRange containing the specified events.
        /// </summary>
        /// <param name="firstId">The first event's id.</param>
        /// <param name="lastId">The last event's id.</param>
        /// <param name="subRange">The range to be retrieved.</param>
        /// <returns>True if the range has successfully been created, false otherwise.</returns>
        public bool MakeSubRange(long firstId, long lastId, out EventRange subRange)
        {
            EventRangeEnumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext() && enumerator.CurrentId < firstId) {}
            if (enumerator.CurrentId != firstId)
            {
                subRange = null;
                return false;
            }
            subRange = enumerator.NextRange();
            enumerator = subRange.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.CurrentId < lastId) {}
            if (enumerator.CurrentId != lastId)
            {
                subRange = null;
                return false;
            }
            subRange = enumerator.PreviousRange();
            return true;
        }

    }

    public class EventRangeEnumerator : IEnumerator<LowLevelEvent>
    {
        private readonly byte[] _src;
        private readonly int _startOffset;
        private readonly int _endPosition;
        private int _position;
        private int _currentMessageLength;

        /// <summary>
        /// The RequestId of the current LowLevelEvent.
        /// Faster than Current.RequestId.
        /// </summary>
        public long CurrentId { get; private set; }

        /// <summary>
        /// The size of the bitwise representation of the current LowLevelEvent in bytes.
        /// </summary>
        public int CurrentSize
        {
            get { return sizeof (long) + sizeof (int) + _currentMessageLength; }
        }

        public LowLevelEvent Current
        {
            get
            {
                return new LowLevelEvent(Encoding.Unicode.GetString(_src, _position - _currentMessageLength, _currentMessageLength), CurrentId);
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        internal EventRangeEnumerator(byte[] src, int offset, int count, int position)
        {
            _src = src;
            _startOffset = offset;
            _endPosition = offset + count;
            _position = _startOffset+position;
            CurrentId = -1;
        }

        internal EventRangeEnumerator(byte[] src, int offset, int count)
        {
            _src = src;
            _startOffset = offset;
            _endPosition = offset + count;
            _position = _startOffset;
            CurrentId = -1;
        }

        public bool MoveNext()
        {
            if ( _position + sizeof (long) + sizeof (int) <= _endPosition)
            {
                CurrentId = Util.ToLong(_src, _position);
                _position += sizeof (long) + sizeof (int);
                return _position <= _endPosition
                       &&
                       (_position += (_currentMessageLength = Util.ToInt(_src, _position - sizeof (int)))) <=
                       _endPosition;
            }
            return false;
        }

        /// <summary>
        /// Create an EventRange starting with the current event.
        /// Undefined behaviour if the current event isn't properly defined.
        /// </summary>
        /// <returns>Described EventRange.</returns>
        public EventRange NextRange()
        {
            int offset = _position - (sizeof (int) + sizeof (long) + _currentMessageLength);
            return new EventRange(_src, offset, _endPosition - offset);
        }

        /// <summary>
        /// Create an EventRange ending with the current event.
        /// Undefined behaviour if the current event isn't properly defined.
        /// </summary>
        /// <returns>Described EventRange.</returns>
        public EventRange PreviousRange()
        {
            return new EventRange(_src, _startOffset, _position-_startOffset);
        }

        public void Reset()
        {
            _position = _startOffset;
            _currentMessageLength = 0;
            CurrentId = -1;
        }

        public void Dispose()
        { }
    }
}
