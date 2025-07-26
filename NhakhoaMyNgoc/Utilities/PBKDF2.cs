using System.Security.Cryptography;

namespace NhakhoaMyNgoc.Utilities
{

    public static class PBKDF2
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = GenerateSalt();
            byte[] hashBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256).GetBytes(32);
            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256).GetBytes(32);
            string computedHash = Convert.ToBase64String(hashBytes);
            return computedHash == storedHash;
        }
    }

}
