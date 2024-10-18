using System;

namespace Engine
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input[0].ToString().ToUpper() + input.Substring(1)
            };

        public static bool ContainsAny(this string self, string[] array)
        {
            foreach (var toCheck in array)
            {
                if (self.Contains(toCheck))
                    return true;
            }

            return false;
        }
        public static int ParseIntFast(this string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char letter = value[i];
                result = 10 * result + (letter - 48);
            }
            return result;
        }

        public static int ParseIntFast(this char value)
        {
            int result = 0;
            result = 10 * result + (value - 48);
            return result;
        }
    }
}
