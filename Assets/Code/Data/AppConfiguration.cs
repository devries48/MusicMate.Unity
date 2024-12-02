using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Create App Configuration", fileName = "Configuration (app)")]
public class AppConfiguration : ScriptableObject
{
    public string ApiServiceUrl;
    public string User;

    [SerializeField, HideInInspector] private string encryptedPassword;

    public void SetPassword(string password, string key) => encryptedPassword = Encrypt(password, key);

    public string GetPassword(string key) => Decrypt(encryptedPassword, key);

    private static string Encrypt(string text, string key)
    {
        using Aes aes = Aes.Create();
        var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
        aes.Key = keyBytes;
        aes.IV = new byte[16]; // Default IV (all zeros)

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] inputBytes = Encoding.UTF8.GetBytes(text);
        byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        return System.Convert.ToBase64String(encrypted);
    }

    private static string Decrypt(string encryptedText, string key)
    {
        using Aes aes = Aes.Create();
        var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));
        aes.Key = keyBytes;
        aes.IV = new byte[16]; // Default IV (all zeros)

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
        byte[] decrypted = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(decrypted);
    }
}
