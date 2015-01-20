using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EvernestBack
{

    public class EventRange : IEnumerable<LowLevelEvent>
    {
        internal byte[] _src;
        internal int _count;
        internal int _offset;

        internal EventRange(byte[] src, int offset, int count)
        {
            _src = src;
            _offset = offset;
            _count = count;
        }

        public EventRangeEnumerator GetEnumerator(int position)
        {
            return new EventRangeEnumerator(_src, _offset, _count, position);
        }

        public EventRangeEnumerator GetEnumerator()
        {
            return new EventRangeEnumerator(_src, _offset, _count);
        }

        IEnumerator<LowLevelEvent> IEnumerable<LowLevelEvent>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
        /// The RequestID of the current LowLevelEvent.
        /// Faster than Current.RequestID.
        /// </summary>
        public long CurrentID { get; private set; }

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
                return new LowLevelEvent(Encoding.Unicode.GetString(_src, _position - _currentMessageLength, _currentMessageLength), CurrentID);
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
        }

        internal EventRangeEnumerator(byte[] src, int offset, int count)
        {
            _src = src;
            _startOffset = offset;
            _endPosition = offset + count;
            _position = _startOffset;
        }

        public bool MoveNext()
        {
            if ( _position + sizeof (long) < _endPosition)
            {
                CurrentID = Util.ToLong(_src, _position);
                _position += sizeof (long) + sizeof (int);
                return _position < _endPosition
                       &&
                       (_position += (_currentMessageLength = Util.ToInt(_src, _position - sizeof (int)))) <
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
        /// Create an EventRange ending with the current event (excluded).
        /// Undefined behaviour if the current event isn't properly defined.
        /// </summary>
        /// <returns>Described EventRange.</returns>
        public EventRange PreviousRange()
        {
            return new EventRange(_src, _startOffset, _position - (sizeof(int) + sizeof(long) + _currentMessageLength));
        }

        public void Reset()
        {
            _position = _startOffset;
            _currentMessageLength = 0;
        }

        public void Dispose()
        { }
    }

    public class ControlledEventRange : EventRange
    {
        public long FirstId { get; private set; }
        public long LastId { get; private set; }

        public ControlledEventRange(byte[] src, int offset, int count, int firstId, int lastId):
            base(src, offset, count)
        {
            FirstId = firstId;
            LastId = lastId;
        }
    }
}
