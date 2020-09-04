using System.Security.Cryptography;
using System.Text;

namespace Shared.Security
{
    public static class Criptografia
    {
        public static string SenhaCriptografada(string senha)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA512Managed sha512hasher = new SHA512Managed();

            byte[] hashedDataBytes = sha512hasher.ComputeHash(encoder.GetBytes(senha));

            StringBuilder output = new StringBuilder("");

            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                output.Append(hashedDataBytes[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}
