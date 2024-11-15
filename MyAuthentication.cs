using System;
using System.Threading;
using System.Threading.Tasks;
using DotPulsar.Abstractions;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Token-based authentication implementation.
/// </summary>
public class MyAuthentication : IAuthentication
{
    private readonly string _authData;

    public MyAuthentication(string accessId, string accessKey)
    {
        _authData = "{\"username\":\"" + accessId +"\", \"password\":\"" + GenPwd(accessId, accessKey) + "\"}";
    }

    /// <summary>
    /// The authentication method name
    /// </summary>
    public string AuthenticationMethodName => "auth1";

    /// <summary>
    /// Get the authentication data
    /// </summary>
    public async ValueTask<byte[]> GetAuthenticationData(CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);

        return System.Text.Encoding.UTF8.GetBytes(_authData);
    }


    // pwd
    private static string GenPwd(string accessId, string accessKey){
        string md5HexKey = Md5(accessKey);
        string mixStr = accessId + md5HexKey;
        String md5MixStr = Md5(mixStr);
        return md5MixStr.Substring(8,16);
    }

    // md5
    private static string Md5(string md5Str) {
        using (MD5 md5 = MD5.Create())
        {
            byte[] dataHash = md5.ComputeHash(Encoding.UTF8.GetBytes(md5Str));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in dataHash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
    }
}