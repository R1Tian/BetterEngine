using UnityEngine;

namespace Better.Collections
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

        /// <summary>
        /// 比 length 大的最小的 2 的幂
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int GetAlignSize(int length)
        {
            for (int i = 0; i < _power2.Length; i++)
            {
                if (length <= _power2[i]) return _power2[i];
            }

            return length;
        }
    }
}