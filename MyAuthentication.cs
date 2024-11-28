using System.Security.Cryptography;
using System.Text;
using DotPulsar.Abstractions;

namespace TuyaPulsar
{
    /// <summary>
    /// Implements token-based authentication for Tuya's Pulsar service.
    /// Handles secure credential generation and authentication data formatting.
    /// </summary>
    public class MyAuthentication : IAuthentication
    {
        private readonly string _authData;

        public MyAuthentication(string accessId, string accessKey)
        {
            _authData = BuildAuthData(accessId, accessKey);
        }

        /// <summary>
        /// Gets the authentication method identifier
        /// </summary>
        public string AuthenticationMethodName => "auth1";

        /// <summary>
        /// Provides the authentication data for the Pulsar connection
        /// </summary>
        public async ValueTask<byte[]> GetAuthenticationData(CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken);
            return Encoding.UTF8.GetBytes(_authData);
        }

        private static string BuildAuthData(string accessId, string accessKey)
        {
            string password = GeneratePassword(accessId, accessKey);
            return $"{{\"username\":\"{accessId}\", \"password\":\"{password}\"}}";
        }

        private static string GeneratePassword(string accessId, string accessKey)
        {
            string md5Key = ComputeMd5Hash(accessKey);
            string combinedString = accessId + md5Key;
            string finalHash = ComputeMd5Hash(combinedString);
            return finalHash.Substring(8, 16);
        }

        private static string ComputeMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            StringBuilder builder = new();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2").ToLower());
            }
            return builder.ToString();
        }
    }
}