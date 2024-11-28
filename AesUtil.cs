using System.Security.Cryptography;
using System.Text;
using DotPulsar.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TuyaPulsar
{
    /// <summary>
    /// Provides encryption and decryption utilities for Tuya's message format
    /// Supporting both AES-GCM and AES-ECB modes
    /// </summary>
    public static class AesUtil
    {
        /// <summary>
        /// Decrypts a message based on its encryption model
        /// </summary>
        /// <param name="message">The Pulsar message to decrypt</param>
        /// <param name="accessKey">The access key for decryption</param>
        /// <returns>Decrypted message content</returns>
        public static string DecryptMessage(IMessage message, string accessKey)
        {
            // Extract encryption model from message properties
            message.Properties.TryGetValue("em", out var encryptionModel);
            
            // Parse message data
            var data = Encoding.UTF8.GetString(message.Data);
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            var payloadJson = (JObject?)JsonConvert.DeserializeObject(data);

            var encryptedData = payloadJson?["data"]?.ToString();
            if (encryptedData == null)
                return string.Empty;
            var decryptionKey = accessKey.Substring(8, 16);

            // Decrypt based on encryption model
            var decryptedData = encryptionModel == "aes_gcm" 
                ? DecryptUsingGcm(encryptedData, decryptionKey)
                : DecryptUsingEcb(encryptedData, decryptionKey);

            // Clean up control characters
            return CleanControlCharacters(decryptedData);
        }

        /// <summary>
        /// Decrypts data using AES-GCM mode
        /// </summary>
        private static string DecryptUsingGcm(string encryptedData, string key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            
            // Extract components
            var nonce = new byte[12];
            Array.Copy(encryptedBytes, 0, nonce, 0, nonce.Length);
            
            var ciphertext = new byte[encryptedBytes.Length - nonce.Length - 16];
            Array.Copy(encryptedBytes, nonce.Length, ciphertext, 0, ciphertext.Length);
            
            var tag = new byte[16];
            Array.Copy(encryptedBytes, encryptedBytes.Length - 16, tag, 0, tag.Length);

            // Decrypt
            var plaintext = new byte[ciphertext.Length];
            using var aesGcm = new AesGcm(keyBytes, 16);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }

        /// <summary>
        /// Decrypts data using AES-ECB mode
        /// </summary>
        private static string DecryptUsingEcb(string encryptedData, string key)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedData);
                var keyBytes = Encoding.UTF8.GetBytes(key);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.ECB;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                using var decryptor = aes.CreateDecryptor(keyBytes, null);
                using var memStream = new MemoryStream(encryptedBytes);
                using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);
                
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Removes control characters from the decrypted string
        /// </summary>
        private static string CleanControlCharacters(string input)
        {
            return input
                .Replace("\f", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Replace("\v", "")
                .Replace("\b", "");
        }
    }
}