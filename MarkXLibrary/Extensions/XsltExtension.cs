using System.Security.Cryptography;

namespace MarkXLibrary
{
    public class XsltExtension
    {
        public static string Hash(string text)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }

        public static string UnescapeUri(string text)
        {
            return Uri.UnescapeDataString(text);
        }
    }
}
