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

        public static int Next(int maxValue, int minValue = 0)
        {
            if (minValue >= maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
            long diff = (long)maxValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;
            uint ui;
            do { ui = RandomUInt(); } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        public static string GenerateRandomString(int size)
        {
            byte[] data = new byte[4 * size];
            csp.GetBytes(data);
            StringBuilder sb = new StringBuilder(size);
            for (int i = 0; i < size; i++) sb.Append(chars[BitConverter.ToUInt32(data, i * 4) % chars.Length]);
            return sb.ToString();
        }

        private static uint RandomUInt()
        {
            byte[] randomBytes = RandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private static byte[] RandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes(buffer);
            return buffer;
        }
    }
}
