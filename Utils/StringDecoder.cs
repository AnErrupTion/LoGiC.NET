using System;
using System.Text;

namespace LoGiC.NET.Utils
{
    public static class StringDecoder
    {
        public static string Decrypt(string str)
        {
            char[] chars = "*$,;:!ù^*&é\"'(-è_çà)".ToCharArray();
            foreach (char c in chars) str = str.Replace(c.ToString(), string.Empty);
            return Encoding.UTF32.GetString(Convert.FromBase64String(str)); /*If you used UTF8 or Unicode
            for example in the Encrypt function, be sure to also replace it here.*/
        }
    }
}
