using ShortenURL.utils;
using System.Text;
using System.Security.Cryptography;

namespace ShortenURL.utils
{
    public class Encoder: IBaseEncoder
    {
        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string Encode(string longUrl)
        {
            string trimmedUrl = longUrl.Replace("https://","")
                .Replace("http://","")
                .Trim();

            using var sha256 = SHA256.Create();
            
            var numericValue = BitConverter.ToUInt64(sha256.ComputeHash(Encoding.UTF8.GetBytes(trimmedUrl)), 0);

            return Base62Encode(numericValue);
            
        }
        public static string Base62Encode(ulong  numericValue)
        {
            if (numericValue == 0) return "0";

            StringBuilder sb = new StringBuilder();
            while (numericValue > 0)
            {
                sb.Insert(0, Base62Chars[(int)(numericValue % 62)]);
                numericValue /= 62;
            }
            return sb.ToString();
        }

        public static long Decode(string base62)
        {
            long result = 0;
            foreach (char c in base62)
            {
                result = result * 62 + Base62Chars.IndexOf(c);
            }
            return result;
        }
    }
}


