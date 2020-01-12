using System;
using System.Text;

namespace LoGiC.NET.Utils
{
    public static class StringEncoder
    {
        public static string Decrypt(string str)
        {
            char[] chars = "*$,;:!ù^*&é\"'(-è_çà)".ToCharArray();
            foreach (char c in chars) str = str.Replace(c.ToString(), string.Empty);
            return Encoding.UTF32.GetString(Convert.FromBase64String(str));
        }
    }
}
