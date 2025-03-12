namespace Better
{
    internal static class BinaryHelper
    {
        private static readonly int _minBit = 0;
        private static readonly int _maxBit = 16;
        
        static readonly int[] _power2;

        static BinaryHelper()
        {
            _power2 = new int[_maxBit - _minBit + 1];
            _power2[0] = 1;
            for (int i = _minBit + 1; i <= _maxBit; i++)
            {
                _power2[i] = 2 * _power2[i - 1];
            }
        }

        public static int GetBit(int length)
        {
            for (int i = 0; i <= _maxBit; ++i)
            {
                int size = GetPower2(i);

                if (length <= size) return i;
            }

            return -1;
        }

        public static int GetBitFromAlign(int alignSize)
        {
            for (int i = 0; i <= _maxBit; ++i)
            {
                int size = GetPower2(i);

                if (alignSize <= size)
                {
                    return alignSize == size ? i : -1;
                }
            }

            return -1;
        }

        public static int GetAlignSize(int length)
        {
            for (int i = 0; i <= _maxBit; ++i)
            {
                int size = GetPower2(i);

                if (length <= size) return size;
            }

            return length;
        }

        public static int GetPower2(int index)
        {
            if (index < 0) return 0;

            if (index >= _minBit && index <= _maxBit) return _power2[index];

            return _power2[index];
        }
    }
}