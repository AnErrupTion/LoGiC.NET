using System;
using System.Security.Cryptography;
using System.Text;

namespace LoGiC.NET.Utils
{
    /// <summary>
    /// This class is the one that generates random integers and strings.
    /// </summary>
    public class Randomizer
    {
        private static readonly RandomNumberGenerator csp = RandomNumberGenerator.Create();
        private static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 !:;,ù^$*&é\"'(-è_çà)=?./§%¨£µ1234567890°+".ToCharArray();

        public static string Generated;

        public static void Initialize()
        {
            byte[] data = RandomBytes(8);
            Generated = chars[BitConverter.ToUInt32(data, 4) % chars.Length].ToString();
        }

        public static int Next(int maxValue, int minValue = 0)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            long diff = (long) maxValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;
            uint ui;
            do { ui = RandomUInt(); } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        public static string String(int size)
        {
            byte[] data = RandomBytes(4 * size);

            StringBuilder sb = new StringBuilder(size);
            for (int i = 0; i < size; i++)
                sb.Append(chars[BitConverter.ToUInt32(data, i * 4) % chars.Length]);

            return sb.ToString();
        }

        public static int Next()
        {
            return BitConverter.ToInt32(RandomBytes(sizeof(int)), 0);
        }

        private static uint RandomUInt()
        {
            return BitConverter.ToUInt32(RandomBytes(sizeof(uint)), 0);
        }

        private static byte[] RandomBytes(int bytes)
        {
            byte[] buffer = new byte[bytes];
            csp.GetBytes(buffer);
            return buffer;
        }
    }
}
