using System.Security.Cryptography;
using System.Text;

/// <summary>
/// MD5Algorithm to get Hash
/// </summary>
class MD5Algorithm
{
    public static readonly MD5 Instance = MD5.Create();

    /// <summary>
    /// Get MD5 Hash
    /// </summary>
    protected static string GetMd5Hash(MD5 md5Hash, string input)
    {
        // Convert the input string to a byte array and compute the hash. 
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes 
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string. 
        return sBuilder.ToString();
    }
    public static string GetMd5Hash(string input)
    {
        return GetMd5Hash(Instance, input);
    }
}