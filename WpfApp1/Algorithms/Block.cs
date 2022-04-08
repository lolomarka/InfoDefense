using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDefense.Algorithms
{
    public class Block
    {
        static int blockLength = 64;
        static int keyLength = 56;
        static int bitNums = 8;
        static int[] shifts = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
        static int rounds = 16;
        List<string> keys = new List<string>();

        public List<string> Keys { get => keys; set => keys = value; }

        public Block(List<string> keys)
        {
            this.Keys = keys;
        }

        static string internals()
        {
            return $"Длина блока: {blockLength}, длина ключа {keyLength}, кол-во бит на символ: {bitNums}, величина сдвига: {shifts}, раунды: {rounds}";
        }

        string ExpandBinary(String bs)
        {
            var d = bs.Length % blockLength;

            bs = bs + new string('0', blockLength - d);
            return bs;
        }

        public String BytesToBinary(byte[] bytes)
        {
            List<string> binaryList = new List<string>();
            foreach (var item in bytes)
            {
                binaryList.Add(NormalizeToSize(Convert.ToString(item, 2), bitNums));
            }
            return string.Join("", binaryList.ToArray());
        }

        List<string> BinaryToBlocks(string bs)
        {
            var blocks = new List<string>();

            for (int i = 0; i < bs.Length; i += blockLength)
            {
                blocks.Add(bs.Substring(i, blockLength));
            }

            return blocks;
        }
        public byte[] BinaryToBytes(String b)
        {
            var codes = new List<byte>();
            for (var i = 0; i < b.Length; i += bitNums)
            {
                var code = Convert.ToInt32(b.Substring(i, bitNums), 2);
                codes.Add((byte)code);
            }

            return codes.ToArray();
        }

        string NormalizeToSize(string bBlock, int size)
        {
            var zerosToAdd = size - bBlock.Length;
            return new string('0', zerosToAdd) + bBlock;
        }

        string StringToBinary(string s)
        {
            var binStr = "";

            foreach (var code in s)
            {
                var binCode = Convert.ToString(code, 2);
                binCode = NormalizeToSize(binCode, bitNums);
                binStr += binCode;
            }
            return binStr;
        }

        public string BinaryToString(string b)
        {
            var codes = new List<byte>();
            for (var i = 0; i < b.Length; i += bitNums)
            {
                var code = Convert.ToInt32(b.Substring(i, bitNums), 2);
                codes.Add((byte)code);
            }
            return Encoding.ASCII.GetString(codes.ToArray());
        }

        

        string XOR(string s1, string s2)
        {
            var xorString = new List<string>();

            for (int i = 0; i < s1.Length; i++)
            {
                var b1 = s1[i] == '1';
                var b2 = s2[i] == '1';
                xorString.Add(b1 ^ b2 ? "1" : "0");
            }
            return string.Join("", xorString);
        }

        string EncryptOnce(string bInput, string bKey)
        {
            var bLeft = bInput.Substring(0, bInput.Length / 2);
            var bRight = bInput.Substring(bInput.Length / 2);

            return bRight + XOR(bLeft, bRight);
        }

        string Permutation(string block, List<List<int>> matrix)
        {
            var sl = new List<string>();
            foreach (var row in matrix)
            {
                foreach (var elem in row)
                {
                    sl.Add(block[elem - 1].ToString());
                }
            }
            return string.Join("", sl.ToArray());
        }

        string Feistel(string r, string key)
        {
            var rExp = Permutation(r, Utils.Utils.ExpandMatrix);

            var bStr = XOR(rExp, key);
            var bVec = Utils.Utils.SplitToChunks(bStr, 6);

            var i = 0;
            var res = "";

            foreach (var bi in bVec)
            {
                var a = Convert.ToInt32(string.Concat(bi[0], bi[bi.Length - 1]), 2);
                var b = Convert.ToInt32(bi.Substring(1, bi.Length - 1), 2);

                var sMatrix = Utils.Utils.selectS(i);

                var n = sMatrix[a][b];

                var binN = NormalizeToSize(Convert.ToString(n, 2), 4);

                res += binN;

                i++;
            }

            return Permutation(res, Utils.Utils.pPermutationMatrix);
        }

        string DecryptOnce(string bInput, string bKey)
        {
            var bLeft = bInput.Substring(0, bInput.Length / 2);
            var bRight = bInput.Substring(bInput.Length / 2);

            return bRight + XOR(bLeft, bRight);
        }

        string GenerateKey(string bKey)
        {
            var keyChunks = Utils.Utils.SplitToChunks(bKey, 8);
            for (var i = 0; i < keyChunks.Count; i++)
            {
                var oneCount = Utils.Utils.CharsCount('1', keyChunks[i]);
                keyChunks[i] += oneCount % 2 == 0 ? '1' : '0';
            }
            bKey = Permutation(string.Join("", keyChunks.ToArray()), Utils.Utils.KeyPermutationMatrix1);
            return bKey;
        }

        public CryptoResult Encrypt(string binaryData, string key)
        {
            var input = ExpandBinary(binaryData);
            keys.Clear();
            var blocksBinary = BinaryToBlocks(input);
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.Utils.PermutationMatrix1);
            }
            var cd = GenerateKey(StringToBinary(key));
            var cdSplit = Utils.Utils.SplitToChunks(cd, cd.Length / 2);
            var c = cdSplit[0];
            var d = cdSplit[1];
            for (var r = 0; r < rounds; r++)
            {
                c = ShiftLeft(c, shifts[r]);
                d = ShiftLeft(d, shifts[r]);
                cd = Permutation(c + d, Utils.Utils.CDPermutationMatrix);
                var roundKey = cd;
                keys.Add(roundKey);

                for (var b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = EncryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = NormalizeToSize(tmpBlock, blockLength);
                }
            }

            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.Utils.PermutationMatrix16);
            }
            key = BinaryToString(cd);
            return new CryptoResult(string.Join("", blocksBinary.ToArray()), key);
        }

        private string ShiftLeft(string bKey, int shift)
        {
            for (int i = 0; i < shift; i++)
            {
                bKey = bKey + bKey[0];
                bKey = bKey.Substring(1);
            }

            return bKey;
        }

        public CryptoResult Decrypt(string input, string encryptedKey)
        {
            var blocksBinary = BinaryToBlocks(input);
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.Utils.PermutationMatrix1);
            }
            for (var r = 0; r < rounds; r++)
            {
                var roundKey = keys[rounds - r - 1];

                for (var b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = DecryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = NormalizeToSize(tmpBlock, blockLength);
                }
            }
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.Utils.PermutationMatrix16);
            }
            return new CryptoResult(string.Join("", blocksBinary.ToArray()), "");
        }
    }

    public class CryptoResult
    {
        public CryptoResult(string value, string key)
        {
            this.value = value;
            this.key = key;
        }

        public string value { get; set; }
        public string key { get; set; }

    }

}


