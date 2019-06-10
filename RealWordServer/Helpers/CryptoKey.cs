using System;
using System.Collections;
using System.Security.Cryptography;

namespace RealWordServer.Helpers
{
    public class SaltHash
    {
        public string Salt { get; set; }
        public string Hash { get; set; }
    }
    // Based upon: http://stackoverflow.com/a/32191537/2183503
    public class CryptoKey
    {
        // Note: These shouldn't be changed (obviously!) because changing them
        // will invalidate all the hashes created with the previous settings
        private const int SaltSize = 64;
        private const int HashSize = 128;
        private const int TokenSize = 64;
        private const int Iterations = 256;

        public string GenerateToken()
        {
            var saltBytes = CreateNewSalt();
            var hashBytes = CreateHash(Guid.NewGuid().ToString(), saltBytes, TokenSize);

            return Convert.ToBase64String(hashBytes);
        }

        public SaltHash HashPassword(string password)
        {
            var saltBytes = CreateNewSalt();
            var hashBytes = CreateHash(password, saltBytes, HashSize);

            return new SaltHash
            {
                Salt = Convert.ToBase64String(saltBytes),
                Hash = Convert.ToBase64String(hashBytes)
            };
        }

        public bool Verify(string password, SaltHash saltHash)
        {
            var saltBytes = Convert.FromBase64String(saltHash.Salt);
            var hashBytes = Convert.FromBase64String(saltHash.Hash);

            var newHashBytes = CreateHash(password, saltBytes, HashSize);

            return StructuralComparisons.StructuralEqualityComparer.Equals(newHashBytes, hashBytes);
        }

        private byte[] CreateNewSalt()
        {
            var saltBytes = new byte[SaltSize];
            new RNGCryptoServiceProvider().GetBytes(saltBytes);

            return saltBytes;
        }

        private byte[] CreateHash(string plainText, byte[] saltBytes, int hashSize)
        {
            var derivedBytes = new Rfc2898DeriveBytes(plainText, saltBytes, Iterations);

            return derivedBytes.GetBytes(hashSize);
        }
    }
}

