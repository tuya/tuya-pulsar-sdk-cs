using System.Text;
using System.Security.Cryptography;
using DotPulsar.Internal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DotPulsar.Abstractions;
using DotPulsar;


class AesUtil {

    public static string DecryptMessage(IMessage message, string accessKey) {
        message.Properties.TryGetValue("em", out var decrypt_model);
//        Console.WriteLine($"Received: {decrypt_model}");
        string data = Encoding.UTF8.GetString(message.Data);
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        JObject payloadJson = (JObject)JsonConvert.DeserializeObject(data);

        if (decrypt_model == "aes_gcm") {
            return DecryptByGcm(payloadJson["data"].ToString(),accessKey.Substring(8,16)).Replace("\f","").Replace("\r","").Replace("\n","").Replace("\t","").Replace("\v","").Replace("\b","");
        } else {
            return DecryptByEcb(payloadJson["data"].ToString(),accessKey.Substring(8,16)).Replace("\f","").Replace("\r","").Replace("\n","").Replace("\t","").Replace("\v","").Replace("\b","");
        }
    }

    //decrypt_by_gcm
    private static string DecryptByGcm(string decryptStr, string Key) {
        byte[] cadenaBytes = Convert.FromBase64String(decryptStr);
        byte[] claveBytes = Encoding.UTF8.GetBytes(Key);
        // The first 12 bytes are the nonce
        byte[] nonce = new byte[12];
        Array.Copy(cadenaBytes, 0, nonce, 0, nonce.Length);

        // The data to decrypt (excluding nonce and tag)
        byte[] ciphertext = new byte[cadenaBytes.Length - nonce.Length - 16];
        Array.Copy(cadenaBytes, nonce.Length, ciphertext, 0, ciphertext.Length);

        // The last 16 bytes are the authentication tag
        byte[] tag = new byte[16];
        Array.Copy(cadenaBytes, cadenaBytes.Length - 16, tag, 0, tag.Length);

        using (AesGcm aesGcm = new AesGcm(claveBytes))
        {
            byte[] plaintext = new byte[ciphertext.Length];
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
            return System.Text.Encoding.UTF8.GetString(plaintext);
        }
    }

    //decrypt_by_aes
    private static string DecryptByEcb(string decryptStr, string Key) {
        try{
            byte[] cadenaBytes = Convert.FromBase64String(decryptStr);
            byte[] claveBytes = Encoding.UTF8.GetBytes(Key);

            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.ECB;
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.Padding = PaddingMode.Zeros;
            ICryptoTransform desencriptador;
            desencriptador = rijndaelManaged.CreateDecryptor(claveBytes, rijndaelManaged.IV);
            MemoryStream memStream = new MemoryStream(cadenaBytes);
            CryptoStream cryptoStream;
            cryptoStream = new CryptoStream(memStream, desencriptador, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);
            string resultStr = streamReader.ReadToEnd();

            memStream.Close();
            cryptoStream.Close();
            return resultStr;
        }catch (Exception ex){
            Console.WriteLine(ex);
            return null;
        }
    }
}