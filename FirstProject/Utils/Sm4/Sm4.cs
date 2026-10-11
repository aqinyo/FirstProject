using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace FirstProject.Utils;

public sealed class Sm4
{
        private const int BlockSize = 16;
        private readonly byte[] _key;


        // 从配置中读取并校验 SM4 密钥
        public Sm4(IConfiguration configuration)
        {
            var keyHex = configuration["Crypto:Sm4Key"]
                ?? throw new InvalidOperationException("未配置 Crypto:Sm4Key。");

            try
            {
                _key = Convert.FromHexString(keyHex);
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException("Crypto:Sm4Key 必须是 32 位十六进制字符串。", exception);
            }

            if (_key.Length != BlockSize)
            {
                throw new InvalidOperationException("Crypto:Sm4Key 必须是 32 位十六进制字符串（16 字节）。");
            }
        }


        // 加密：使用 SM4-CBC 和 PKCS7 填充加密明文，结果为 Base64 编码的随机 IV 与密文（注：每次加密都会生成新的 IV，因此同一明文的密文通常不同）
        public string Encrypt(string plaintext)
        {
            ArgumentNullException.ThrowIfNull(plaintext);

            var iv = RandomNumberGenerator.GetBytes(BlockSize);
            var input = Encoding.UTF8.GetBytes(plaintext);
            var cipher = CreateCipher(encrypt: true, iv);
            var output = new byte[cipher.GetOutputSize(input.Length)];
            var length = cipher.ProcessBytes(input, 0, input.Length, output, 0);
            length += cipher.DoFinal(output, length);

            var result = new byte[BlockSize + length];
            Buffer.BlockCopy(iv, 0, result, 0, BlockSize);
            Buffer.BlockCopy(output, 0, result, BlockSize, length);
            return Convert.ToBase64String(result);
        }


        // 解密：从密文中提取 IV 并使用 SM4-CBC 解密。
        private string Decrypt(string encryptedText)
        {
            var input = Convert.FromBase64String(encryptedText);
            if (input.Length < BlockSize * 2 || (input.Length - BlockSize) % BlockSize != 0)
            {
                throw new FormatException("SM4 密文格式无效。");
            }

            var iv = input[..BlockSize];
            var ciphertext = input[BlockSize..];
            var cipher = CreateCipher(encrypt: false, iv);
            var output = new byte[cipher.GetOutputSize(ciphertext.Length)];
            var length = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, output, 0);
            length += cipher.DoFinal(output, length);
            return Encoding.UTF8.GetString(output, 0, length);
        }

    
        // 密码验证：解密存储的密文并与输入明文进行定时安全比较
        public bool Verify(string plaintext, string encryptedText)
        {
            try
            {
                var decrypted = Decrypt(encryptedText);
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(plaintext),
                    Encoding.UTF8.GetBytes(decrypted));
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCipherTextException)
            {
                return false;
            }
        }
    

        // 创建并初始化 SM4-CBC/PKCS7 加解密器
        private PaddedBufferedBlockCipher CreateCipher(bool encrypt, byte[] iv)
        {
            var cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new SM4Engine()),
                new Pkcs7Padding());
            cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(_key), iv));
            return cipher;
        }

}
