using System.Security.Cryptography;

namespace CourseWork.Application.Common;

public static class FileEncryptor
{
    public static async Task EncryptFile(Stream fileData, Stream destinationStream, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;

        aes.GenerateIV();
        
        // Сначала сохраняем IV
        await destinationStream.WriteAsync(aes.IV.AsMemory(0, aes.IV.Length));

        await using var cs = new CryptoStream(destinationStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await fileData.CopyToAsync(cs);
        cs.Close();
    }

    // Метод для расшифровки файла
    public static async Task<Stream> DecryptFile(Stream encryptedStream, byte[] key)
    {
        using var aes = Aes.Create();
        var result = new MemoryStream();
        aes.Key = key;
        // Чтение IV
        var iv = new byte[aes.IV.Length];
        await encryptedStream.ReadAsync(iv);
        aes.IV = iv;

        await using var cs = new CryptoStream(encryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await cs.CopyToAsync(result);
        await cs.FlushAsync();
        result.Position = 0;
        return result;
    }
}