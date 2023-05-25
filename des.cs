 /// <summary>
    /// 创佳的加密算法实现类
    /// </summary>
    public class LocstarCryptHelper
    {
        #region 适配创佳现在的蓝牙密钥des算法

        /// <summary>
        /// Des加密创佳蓝牙密钥
        /// </summary>
        /// <param name="encryptKeyHexStr">加密用的key</param>
        /// <param name="bluetoothKeyHexStr">要加密的蓝牙钥匙</param>
        /// <param name="isDecrypt">是否为解密</param>
        /// <returns>加密后的蓝牙钥匙</returns>
        public static string DesEncryptBluetooth(string encryptKeyHexStr, string bluetoothKeyHexStr, bool isDecrypt = false)
        {
            if (string.IsNullOrEmpty(encryptKeyHexStr))
            {
                throw new ArgumentNullException(nameof(encryptKeyHexStr), "请指定加密key的十六进制字符串");
            }
            if (encryptKeyHexStr.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(encryptKeyHexStr), "加密key只能是8字节");
            }
            if (string.IsNullOrEmpty(bluetoothKeyHexStr))
            {
                throw new ArgumentNullException(nameof(bluetoothKeyHexStr), "请指定蓝牙钥匙");
            }
            if (bluetoothKeyHexStr.Length % 16 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bluetoothKeyHexStr), "蓝牙钥匙的长度必须是8字节的倍数");
            }

            var result = ImmutableArray.Create<byte>();
            var key = ImmutableArrayHelper.ParseFromHexString(encryptKeyHexStr).ToArray();
            var times = bluetoothKeyHexStr.Length / 16;
            for (var i = 0; i < times; i++)
            {
                var sourceHexStr = bluetoothKeyHexStr.Substring(i * 16, 16);
                var source = ImmutableArrayHelper.ParseFromHexString(sourceHexStr).ToArray();
                InitPermutation(source);
                Console.WriteLine(ToHexStr(source));
                var target = DesData(!isDecrypt, source, key);
                result = result.AddRange(target);
            }
            return result.ToHexString();
        }


        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexStr(byte[] bytes, int start = 0)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = start; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 初始化数组置换
        /// </summary>
        /// <param name="inData">要初始化的原始数组</param>
        public static void InitPermutation(byte[] inData)
        {
            var newData = new byte[8];
            for (var i = 0; i < 64; i++)
            {
                if ((inData[BitIP[i] >> 3] & (1 << (7 - (BitIP[i] & 0x07)))) != 0)
                {
                    newData[i >> 3] = Convert.ToByte(newData[i >> 3] | (1 << (7 - (i & 0x07))));
                }
            }
            for (var i = 0; i < 8; i++)
            {
                inData[i] = newData[i];
            }
        }

        /// <summary>
        /// 根据加密key计算后续使用的密钥多维数组
        /// </summary>
        /// <param name="inKey">原加密key</param>
        /// <returns>后续使用的密钥多维数组</returns>
        private static byte[,] MakeKey(byte[] inKey)
        {
            var outKey = new byte[8, 8];
            for (var i = 0; i < 8; i++)
            {
                var newData = new byte[8];
                for (var j = 0; j < 64; j++)
                {
                    if ((inKey[BitPMC[i, j] >> 3] & (1 << (7 - (BitPMC[i, j] & 0x07)))) != 0)
                    {
                        newData[j >> 3] = Convert.ToByte(newData[j >> 3] | (1 << (7 - (j & 0x07))));
                    }
                }
                for (var k = 0; k < 8; k++)
                {
                    outKey[i, k] = newData[k];
                }
            }
            return outKey;
        }

        public static string MultidimensionalArrayToHexString(byte[,] array)
        {
            StringBuilder hexString = new StringBuilder();
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            for (int i = 0; i < rows; i++) for (int j = 0; j < cols; j++)
                    hexString.Append(array[i, j]);
            return hexString.ToString();
        }


        /// <summary>
        /// 进行Des加密解密
        /// </summary>
        /// <param name="isEncrypt">是加密还是解密</param>
        /// <param name="inData">原始数据</param>
        /// <param name="key">加解密key</param>
        /// <returns>加解密后的数据</returns>
        public static byte[] DesData(bool isEncrypt, byte[] inData, byte[] key)
        {
            var subKey = MakeKey(key);
            const int len = 8;
            var outData = new byte[len];
            if (isEncrypt)
            {
                for (var i = 0; i < len; i++)
                {
                    for (var j = 0; j < len; j++)
                    {
                        inData[j] = Convert.ToByte(inData[j] ^ subKey[i, j]);
                    }
                }
                for (var j = 0; j < 8; j++)
                {
                    outData[j] = inData[j];
                }
            }
            else
            {
                for (var i = 7; i >= 0; i--)
                {
                    for (var j = 0; j < 8; j++)
                    {
                        inData[j] = Convert.ToByte(inData[j] ^ subKey[i, j]);
                    }
                }
                for (var j = 0; j < 8; j++)
                {
                    outData[j] = inData[j];
                }
            }
            return outData;
        }

        /// <summary>
        /// 进行Des加密解密
        /// </summary>
        /// <param name="isEncrypt">是加密还是解密</param>
        /// <param name="inData">原始数据</param>
        /// <param name="key">加解密key</param>
        /// <returns>加解密后的数据</returns>
        public static byte[] DesData1(bool isEncrypt, byte[] inData, byte[] key)
        {
            var subKey = MakeKey(key);
            Console.WriteLine($"{MultidimensionalArrayToHexString(subKey)}");
            int len = inData.Length;
            var outData = new byte[len];
            if (isEncrypt)
            {
                for (var i = 0; i < len; i++)
                {
                    for (var j = 0; j < len; j++)
                    {
                        inData[j] = Convert.ToByte(inData[j] ^ subKey[i, j]);
                    }
                }
                for (var j = 0; j < len; j++)
                {
                    outData[j] = inData[j];
                }
            }
            else
            {
                for (var i = len-1; i >= 0; i--)
                {
                    for (var j = 0; j < len; j++)
                    {
                        inData[j] = Convert.ToByte(inData[j] ^ subKey[i, j]);
                    }
                }
                for (var j = 0; j < len; j++)
                {
                    outData[j] = inData[j];
                }
            }
            return outData;
        }


        #region 加密常量定义

        private static byte[] BitIP = new byte[] {25, 17, 54, 33, 9,  38, 34, 1,
  11, 48, 29, 56, 27, 50, 51, 40,
  19, 58, 21, 5,  44, 31, 45, 7,
  61, 47, 13, 57, 23, 15, 53, 46,
  24, 16, 8,  39, 0,  26, 18, 10,
  2,  49, 12, 4,  36, 30, 14, 6,
  41, 59, 63, 22, 62, 32, 37, 42,
  28, 20, 43, 52, 3,  35, 60, 55 };

        private static byte[] BitCP = new byte[] {36, 7,  40, 60, 43, 19, 47, 23,
  34, 4,  39, 8,  42, 26, 46, 29,
  33, 1,  38, 16, 57, 18, 51, 28,
  32, 0,  37, 12, 56, 10, 45, 21,
  53, 3,  6,  61, 44, 54, 5,  35,
  15, 48, 55, 58, 20, 22, 31, 25,
  9,  41, 13, 14, 59, 30, 2,  63,
  11, 27, 17, 49, 62, 24, 52, 50 };

        private static byte[,] BitPMC = new byte[,] {
        { 56,0,53,29,17,44,24,8,20,23,43,16,7,46,36,57,2,19,42,35,32,15,31,26,54,60,33,9,38,11,61,30,10,47,
  40,5,52,25,41,27,62,63,6,58,13,21,3,28,18,49,55,59,39,51,12,37,14,1,4,34,22,45,48,50 },

  {63,10,47,58,39,38,51,42,23,54,3,21,14,55,49,29,37,28,56,40,61,43,60,18,16,57,26,9,30,34,
  11,33,1,27,53,12,36,48,52,22,46,8,45,44,59,15,5,6,13,24,35,31,2,62,41,0,4,25,50,20,7,17,32,19},

  {8,5,46,4,39,44,63,52,2,54,56,62,21,32,50,48,20,22,47,57,60,37,12,34,9,41,27,11,6,18,33,14,
  24,31,28,55,36,23,16,40,51,25,61,43,17,3,35,53,0,7,10,58,15,1,13,19,38,45,29,42,49,26,59,30},

  {26,52,25,7,48,49,56,30,27,11,22,47,8,16,40,10,9,24,50,62,57,44,34,14,4,55,59,5,39,23,17,58,12,
  3,63,43,6,20,51,42,45,28,31,54,53,1,41,35,13,60,21,61,19,2,46,15,36,33,18,37,0,32,38,29},

  { 42,48,16,38,41,57,53,3,52,14,61,33,26,19,32,58,10,1,9,24,43,8,15,5,56,2,40,36,7,0,17,20,45,37,6,
  13,25,34,11,27,30,12,63,31,28,47,4,51,62,22,55,44,29,35,59,23,46,50,39,60,49,18,21,54},

  { 51,44,45,12,10,19,9,57,53,0,49,8,29,7,22,36,13,58,35,15,50,23,59,52,63,4,30,43,26,33,42,1,14,24,
  55,38,5,32,48,28,21,31,17,46,41,47,60,25,20,11,61,3,6,16,2,40,39,18,62,37,34,27,56,54},

  { 19,61,20,15,0,59,60,12,10,16,35,36,34,5,27,8,43,3,54,7,57,58,26,56,13,1,23,50,11,6,25,22,31,44,
  62,53,14,33,39,48,52,2,40,41,29,17,18,4,47,28,63,24,9,32,21,46,49,30,38,37,51,45,42,55},

  { 33,24,29,28,30,51,20,25,0,57,22,34,13,44,31,17,49,16,18,50,4,48,5,38,41,12,63,26,55,37,52,60,27,9,
  21,19,45,39,54,15,53,7,43,46,62,11,14,36,56,1,10,23,42,61,8,35,40,59,47,32,2,58,6,3}
        };

        #endregion 加密常量定义

        #endregion 适配创佳现在的蓝牙密钥des算法
    }

    /// <summary>
    /// 不可变数组辅助类
    /// </summary>
    public static class ImmutableArrayHelper
    {

        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        /// <param name="data">要转换的数据</param>
        /// <returns>对应的十六进制字符串</returns>
        public static string ToHexString(this ImmutableArray<byte> data)
        {
            var resultBuilder = new StringBuilder(data.Length * 2);
            foreach (var d in data)
            {
                var str = Convert.ToString(d, 16).PadLeft(2, '0').ToUpper();
                resultBuilder.Append(str);
            }
            return resultBuilder.ToString();
        }

        /// <summary>
        /// 将十六进制字符串转换为对应的byte数组
        /// </summary>
        /// <param name="hexString">要转换的十六进制字符串</param>
        /// <returns>对应的byte数组</returns>
        public static ImmutableArray<byte> ParseFromHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString) || !IsValidHexString(hexString))
            {
                return ImmutableArray.Create<byte>();
            }
            var builder = ImmutableArray.CreateBuilder<byte>();
            var length = hexString.Length;
            if (length % 2 != 0)
            {
                //如果长度不是2的整数倍数，则前面补0
                hexString = $"0{hexString}";
            }
            for (var i = 0; i < length; i += 2)
            {
                var str = hexString.Substring(i, 2);
                var b = Convert.ToByte(str, 16);
                builder.Add(b);
            }
            return builder.ToImmutable();
        }

        /// <summary>
        /// 是否是有效的16进制字符串
        /// </summary>
        /// <param name="hexString">要检查的16进制字符串</param>
        /// <returns>true:有效，false:无效</returns>
        public static bool IsValidHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return false;
            }
            hexString = hexString.ToLower();
            var validChars = "0123456789abcdef";
            var result = true;
            foreach (char c in hexString)
            {
                if (!validChars.Contains(c))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
