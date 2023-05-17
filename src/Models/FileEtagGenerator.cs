using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

namespace BookBrowser.Models;

public static class FileEtagGenerator
{
    /// <summary>
    /// Produces an etag based on the last write time, filename, and the file length.
    /// This should be sufficient to prevent the majority of collisions.
    /// </summary>
    /// <param name="filePath">The file for which we should get an etag.</param>
    /// <returns>The etag based on "cheap" file attributes.</returns>
    public static string EtagForPath(this string filePath)
    {
        var fi = new FileInfo(filePath);
        var md5 = MD5.HashData(Encoding.UTF8.GetBytes($"{fi.Length}-{fi.LastWriteTimeUtc}-{filePath}"));
        return Convert.ToBase64String(md5);
    }
}