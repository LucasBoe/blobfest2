using System;


    public static class GUIDHelper
    {
        public static long GenerateNew()
        {
            return BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);
        }
    }