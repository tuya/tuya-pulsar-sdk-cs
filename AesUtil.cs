using System.Text;
using System.Security.Cryptography;
using DotPulsar.Internal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DotPulsar.Abstractions;
using DotPulsar;


class AesUtil {

    public static string DecryptMessage(IMessage message, string accessKey) {
        message.Properties.TryGetValue("em", out var decryptModel);
        Console.WriteLine($"Received: {decryptModel}");
        var data = Encoding.UTF8.GetString(message.Data);
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        var payloadJson = (JObject?)JsonConvert.DeserializeObject(data);
        if (payloadJson == null) return string.Empty;
        
        return decryptModel == "aes_gcm" ?
            DecryptByGcm(payloadJson["data"].ToString(),accessKey.Substring(8,16)).Replace("\f","").Replace("\r","").Replace("\n","").Replace("\t","").Replace("\v","").Replace("\b","")
            : DecryptByEcb(payloadJson["data"].ToString(),accessKey.Substring(8,16)).Replace("\f","").Replace("\r","").Replace("\n","").Replace("\t","").Replace("\v","").Replace("\b","");
    }

    //decrypt_by_gcm
    private static string DecryptByGcm(string decryptStr, string key)
    {
        var encryptedBytes = Convert.FromBase64String(decryptStr);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        
        var nonce = new byte[12];
        Array.Copy(encryptedBytes, 0, nonce, 0, nonce.Length);
        
        var ciphertext = new byte[encryptedBytes.Length - nonce.Length - 16];
        Array.Copy(encryptedBytes, nonce.Length, ciphertext, 0, ciphertext.Length);
        
        var tag = new byte[16];
        Array.Copy(encryptedBytes, encryptedBytes.Length - 16, tag, 0, tag.Length);

        var plaintext = new byte[ciphertext.Length];
        using var aesGcm = new AesGcm(keyBytes, 16);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    //decrypt_by_aes
    private static string? DecryptByEcb(string decryptStr, string key)
    {
        try
        {
            var encryptedBytes = Convert.FromBase64String(decryptStr);
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
            Console.WriteLine(ex);
            return null;
        }
    }
}