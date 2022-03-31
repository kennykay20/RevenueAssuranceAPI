using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace  RevAssuranceApi.Helper
{
    //   public static class SecurityEncryptPwd
    // {
    //     private static string key = "YTAnything*your";
    //     public static string Encrypt(this string text)
    //     {
    //         try
    //         {
    //             if (string.IsNullOrEmpty(key))
    //                 throw new ArgumentException("Key must have valid value.", nameof(key));
    //             if (string.IsNullOrEmpty(text))
    //                 throw new ArgumentException("The text must have valid value.", nameof(text));
    //             var buffer = Encoding.UTF8.GetBytes(text);
    //             var hash = new SHA512CryptoServiceProvider();
    //             var aesKey = new byte[24];
    //             Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

    //             using (var aes = Aes.Create())
    //             {
    //                 if (aes == null)
    //                     throw new ArgumentException("Parameter must not be null.", nameof(aes));

    //                 aes.Key = aesKey;

    //                 using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
    //                 using (var resultStream = new MemoryStream())
    //                 {
    //                     using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
    //                     using (var plainStream = new MemoryStream(buffer))
    //                     {
    //                         plainStream.CopyTo(aesStream);
    //                     }

    //                     var result = resultStream.ToArray();
    //                     var combined = new byte[aes.IV.Length + result.Length];
    //                     Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
    //                     Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

    //                     return Convert.ToBase64String(combined);
    //                 }
    //         }
    //     }
    //      catch(Exception ex) {
    //             var exMessage =  ex == null ? ex.InnerException.Message : ex.Message;
    //       }
    //       return "";
    // }

    // public static string Decrypt(this string encryptedText)
    // {
    //     try
    //     {
    //         if (string.IsNullOrEmpty(key))
    //             throw new ArgumentException("Key must have valid value.", nameof(key));
    //         if (string.IsNullOrEmpty(encryptedText))
    //             throw new ArgumentException("The encrypted text must have valid value.", nameof(encryptedText));

    //         var combined = Convert.FromBase64String(encryptedText);
    //         var buffer = new byte[combined.Length];
    //         var hash = new SHA512CryptoServiceProvider();
    //         var aesKey = new byte[24];
    //         Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

    //         using (var aes = Aes.Create())
    //         {
    //             if (aes == null)
    //                 throw new ArgumentException("Parameter must not be null.", nameof(aes));

    //             aes.Key = aesKey;

    //             var iv = new byte[aes.IV.Length];
    //             var ciphertext = new byte[buffer.Length - iv.Length];

    //             Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
    //             Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

    //             aes.IV = iv;

    //             using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
    //             using (var resultStream = new MemoryStream())
    //             {
    //                 using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
    //                 using (var plainStream = new MemoryStream(ciphertext))
    //                 {
    //                     plainStream.CopyTo(aesStream);
    //                 }
    //                 return Encoding.UTF8.GetString(resultStream.ToArray());
    //             }
    //       }  
    //     }
    //     catch(Exception ex) 
    //       {
    //             var exM =  ex == null ? ex.InnerException.Message : ex.Message;
    //       }
    //       return "";
    //   }
    // }


public static class SecurityEncryptPwd
{
    /// <summary>
    /// Size of salt.
    /// </summary>
    private const int SaltSize = 16;
    private static string MyHash = "$ANyGth$V1$";// "$MYHASH$V1$";

    /// <summary>
    /// Size of hash.
    /// </summary>
    private const int HashSize = 20;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="iterations">Number of iterations.</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password, int iterations)
    {
        // Create salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

        // Create hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        // Combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert to base64
        var base64Hash = Convert.ToBase64String(hashBytes);

        // Format hash with extra information

 
        return string.Format(MyHash+"{0}${1}", iterations, base64Hash);
        
  
        //MyHash
    }

    /// <summary>
    /// Creates a hash from a password with 10000 iterations
    /// </summary>
    /// <param name="password">The password.</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password)
    {
        return Hash(password, 10000);
    }

    /// <summary>
    /// Checks if hash is supported.
    /// </summary>
    /// <param name="hashString">The hash.</param>
    /// <returns>Is supported?</returns>
    public static bool IsHashSupported(string hashString)
    {
        return hashString.Contains(MyHash);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="hashedPassword">The hash.</param>
    /// <returns>Could be verified?</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        // Check hash
        if (!IsHashSupported(hashedPassword))
        {
            throw new NotSupportedException("The hashtype is not supported");
        }

        // Extract iteration and Base64 string
        var splittedHashString = hashedPassword.Replace(MyHash, "").Split('$');
        var iterations = int.Parse(splittedHashString[0]);
        var base64Hash = splittedHashString[1];

        // Get hash bytes
        var hashBytes = Convert.FromBase64String(base64Hash);

        // Get salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Get result
        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }
        return true;
    }

    // public void CreatePasswordHash(string Password, out byte[] passwordHash, out byte[] password)
    // {
    //     var hmac = new System.Security.Cryptography.HMAC

    // }
}
}