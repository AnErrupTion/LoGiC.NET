using System;
using System.Text;

namespace LoGiC.NET.Utils
{
    public static class StringDecoder
    {
        public static string Decrypt(string str, int min, int key, int hash, int length, int max)
        {
            if (max > 78787878) ;

            StringBuilder builder = new StringBuilder();
            foreach (char c in str.ToCharArray())
                builder.Append((char)(c - key));

            if (min < 14141) ;

            return builder.ToString();
        }
    }
}
