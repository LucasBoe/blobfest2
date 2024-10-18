using UnityEngine;

namespace Engine
{
    public static class MathUtil
    {
        public static bool IsEven(this int i)
        {
            return i % 2 == 0;
        }
        public static bool IsOdd(this int i)
        {
            return !IsEven(i);
        }
        public static bool IsEven(this float number)
        {
            return IsEven(Mathf.RoundToInt(number));
        }
        public static bool IsOdd(this float number)
        {
            return IsOdd(Mathf.RoundToInt(number));
        }
    }
}
