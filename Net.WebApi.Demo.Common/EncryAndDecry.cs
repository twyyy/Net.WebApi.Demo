using System.Security.Cryptography;
using System.Text;

namespace Net.WebApi.Demo.Common;

/// <summary>
/// 加解密
/// </summary>
public class EncryAndDecry
{
    /// <summary>
    /// MD5 32位加密
    /// </summary>
    /// <param name="plaintext">明文</param>
    /// <param name="capital">是否大写</param>
    /// <returns>密文</returns>
    public static string Md5X32(string plaintext, bool capital = false)
    {
        var inputBytes = Encoding.UTF8.GetBytes(plaintext);
        var hashBytes = MD5.HashData(inputBytes);

        var sb = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString($"{(capital ? "X" : "x")}2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// BcryptHash加密
    /// </summary>
    /// <param name="plaintext">明文</param>
    /// <param name="workFactor">工作因子 (建议10-15)</param>
    /// <param name="hash">hash算法</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string BcryptHash(string plaintext, int workFactor = 10, HashAlgorithmName? hash = null)
    {
        // 检查工作因子, 默认10-15(比较合理的值), 越小越容易被破解, 越大越耗费计算性能
        workFactor = workFactor switch
        {
            < 10 => 10,
            > 15 => 15,
            _ => workFactor
        };

        // 生成盐值
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        var saltString = Convert.ToBase64String(salt);

        // 生成哈希值
        using var hasher = new Rfc2898DeriveBytes(plaintext, salt, workFactor, hash ?? HashAlgorithmName.SHA256);
        var hashBytes = hasher.GetBytes(60);

        // 返回加密后的字符串
        return $"{saltString}${workFactor}${hash.GetHashCode}${Convert.ToBase64String(hashBytes)}";
    }

    /// <summary>
    /// 比较明文与BcryptHash加密的密文
    /// </summary>
    /// <param name="plaintext">明文</param>
    /// <param name="ciphertext">盐值$工作因子$hash算法$哈希值 格式的密文</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool BcryptVerify(string plaintext, string ciphertext)
    {
        // 拆分密文
        var parts = ciphertext.Split('$');
        if (parts.Length != 4) throw new ArgumentException("BcryptVerify加密的内容格式不正确", nameof(ciphertext));

        // 获取盐值/工作因子/hash算法/哈希值
        var salt = Convert.FromBase64String(parts[0]);
        if (!int.TryParse(parts[1], out var workFactor)) throw new ArgumentException("BcryptVerify加密的内容格式不正确", nameof(ciphertext));
        var hash = Enum.Parse<HashAlgorithmName>(parts[2]);
        var hashBytes = Convert.FromBase64String(parts[3]);

        // 生成哈希值
        using var hasher = new Rfc2898DeriveBytes(plaintext, salt, workFactor, hash);
        var newHashBytes = hasher.GetBytes(60);

        // 比较哈希值
        return hashBytes.SequenceEqual(newHashBytes);
    }
}