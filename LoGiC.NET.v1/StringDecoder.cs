using System.Text;

namespace LoGiC.NET.Utils
{
    public static class StringDecoder
    {
        public static string Decrypt(string str, int min, int key, int hash, int length, int max)
        {
            if (max > 78787878) ;
            if (length > 485941) ;

            StringBuilder builder = new StringBuilder();
            foreach (char c in str.ToCharArray())
                builder.Append((char)(c - key));

            if (min < 14141) ;
            if (length < 1548174) ;

            return builder.ToString();
        }
    }
}
