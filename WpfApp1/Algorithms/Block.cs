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
        const int blockLength = 64;
        const int keyLength = 56;
        const int bitNums = 8;
        int[] shifts = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        const int rounds = 16;

        List<string> keys = new List<string>();

        string _expandBinary(string bs)
        {
            var d = bs.Length % blockLength;
            bs = bs + string.Join("", Enumerable.Repeat("0", blockLength - d).ToArray());
            Trace.WriteLine(bs.Length);
            return bs;
        }

        public String bytesToBinary(byte[] bytes)
        {
            List<string> binaryList = new List<string>();
            foreach (var item in bytes)
            {
                binaryList.Add(_normalizeToSize(Convert.ToString(item, 2), bitNums));
            }
            return string.Join("", binaryList.ToArray());
        }

        List<string> _binaryToBlocks(string bs)
        {
            var blocks = new List<string>();
            for (var i = 0; i < bs.Length; i += blockLength)
            {
                blocks.Add(bs.Substring(i, blockLength));
            }
            return blocks;
        }

        string stringToBinary(string s)
        {
            var codes = Encoding.ASCII.GetBytes(s);
            var binStr = "";
            foreach (var code in codes)
            {
                var binCode = Convert.ToString(code, 2);
                binCode = _normalizeToSize(binCode, bitNums);
                binStr += binCode;
            }
            return binStr;
        }

        public String binaryToString(String b)
        {
            var codes = new List<byte>();
            for (var i = 0; i < b.Length; i += bitNums)
            {
                var code = Convert.ToInt32(b.Substring(i, bitNums), 2);
                codes.Add((byte)code);
            }
            return Encoding.ASCII.GetString(codes.ToArray());
        }
        /// XOR
        String _xor(String s1, String s2)
        {
            var xorString = new List<string>();
            for (var i = 0; i < s1.Length; i++)
            {
                var b1 = s1[i] == '1' ? true : false;
                var b2 = s2[i] == '1' ? true : false;
                xorString.Add(b1 ^ b2 ? "1" : "0");
            }
            return string.Join("", xorString.ToArray());
        }
        String _encryptOnce(String bInput, String bKey)
        {
            var bLeft = bInput.Substring(0, bInput.Length / 2);
            var bRight = bInput.Substring(bInput.Length / 2);
            return bRight + _xor(bLeft, feistel(bRight, bKey));
        }
        String _decryptOnce(String bInput, String bKey)
        {
            var bLeft = bInput.Substring(0, bInput.Length / 2);
            var bRight = bInput.Substring(bInput.Length / 2);
            return _xor(feistel(bLeft, bKey), bRight) + bLeft;
        }
        String _shiftLeft(String bKey, int shift)
        {
            for (var i = 0; i < shift; i++)
            {
                bKey = bKey + bKey[0];
                bKey = bKey.Substring(1);
            }
            return bKey;
        }
        String permutation(string block, List<List<int>> matrix)
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
        String _normalizeToSize(String bBlock, int size)
        {
            var zerosToAdd = size - bBlock.Length;
            return string.Join("", Enumerable.Repeat("0", zerosToAdd).ToList().ToArray()) + bBlock;
        }
        String feistel(String r, String key)
        {
            var rExp = permutation(r, Utils.Utils.expandMatrix);
            var bStr = _xor(rExp, key);
            var bVec = Utils.Utils.splitToChunks(bStr, 6);
            var i = 0;
            var res = "";
            foreach (var bi in bVec)
            {
                var a = Convert.ToInt32(bi[0].ToString() + bi[bi.Length - 1].ToString(), 2);
                var b = Convert.ToInt32(bi.Substring(1, bi.Length - 2), 2);
                var sMatrix = Utils.Utils.selectS(i);
                var n = sMatrix[a][b];
                var binN = _normalizeToSize(Convert.ToString(n, 2), 4);
                res += binN;
                i++;
            }
            return permutation(res, Utils.Utils.pPermutationMatrix);
        }
        String generateKey(String bKey)
        {
            var keyChunks = Utils.Utils.splitToChunks(bKey, 8);
            for (var i = 0; i < keyChunks.Count; i++)
            {
                var oneCount = Utils.Utils.charsCount('1', keyChunks[i]);
                keyChunks[i] += oneCount % 2 == 0 ? '1' : '0';
            }
            bKey = permutation(string.Join("", keyChunks.ToArray()), Utils.Utils.keyPermutationMatrix1);
            return bKey;
        }
        public CryptoResult encrypt(String binaryData, String key)
        {
            var input = _expandBinary(binaryData);
            keys.Clear();
            var blocksBinary = _binaryToBlocks(input);
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = permutation(blocksBinary[i], Utils.Utils.permutationMatrix1);
            }
            var cd = generateKey(stringToBinary(key));
            var cdSplit = Utils.Utils.splitToChunks(cd, cd.Length / 2);
            var c = cdSplit[0];
            var d = cdSplit[1];
            for (var r = 0; r < rounds; r++)
            {
                c = _shiftLeft(c, shifts[r]);
                d = _shiftLeft(d, shifts[r]);
                cd = permutation(c + d, Utils.Utils.cdPermutationMatrix);
                var roundKey = cd;
                keys.Add(roundKey);

                for (var b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = _encryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = _normalizeToSize(tmpBlock, blockLength);
                }
            }

            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = permutation(blocksBinary[i], Utils.Utils.permutationMatrix16);
            }
            key = binaryToString(cd);
            return new CryptoResult(string.Join("", blocksBinary.ToArray()), key);
        }

        public CryptoResult decrypt(String input, String encryptedKey)
        {
            var blocksBinary = _binaryToBlocks(input);
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = permutation(blocksBinary[i], Utils.Utils.permutationMatrix1);
            }
            for (var r = 0; r < rounds; r++)
            {
                var roundKey = keys[rounds - r - 1];

                for (var b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = _decryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = _normalizeToSize(tmpBlock, blockLength);
                }
            }
            for (var i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = permutation(blocksBinary[i], Utils.Utils.permutationMatrix16);
            }
            return new CryptoResult(string.Join("", blocksBinary.ToArray()), "");
        }

        public byte[] binaryToBytes(String b)
        {
            var codes = new List<byte>();
            for (var i = 0; i < b.Length; i += bitNums)
            {
                var code = Convert.ToInt32(b.Substring(i, bitNums), 2);
                codes.Add((byte)code);
            }

            return codes.ToArray();
        }
    }

    public class CryptoResult
    {
        public string value;
        public string key;
        public CryptoResult(string _value, string _key)
        {
            value = _value;
            key = _key;
        }

    }

}


