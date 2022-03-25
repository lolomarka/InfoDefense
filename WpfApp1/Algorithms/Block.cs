using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDefense.Algorithms
{
    internal class Block
    {
        static int blockLength = 16;
        static int keyLength = 32;
        static int bitNums = 8;
        static int[] shifts = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
        static int rounds = 16;
        List<string> keys = new List<string>();

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

        List<string> BinaryToBlocks(string bs)
        {
            var blocks = new List<string>();

            for (int i = 0; i < bs.Length; i += blockLength)
            {
                blocks.Add(bs.Substring(i, i + blockLength));
            }

            return blocks;
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

        string BinaryToString(string b)
        {
            var codes = new List<int>();

            for (int i = 0; i < b.Length; i += bitNums)
            {
                var code = Convert.ToInt32(b.Substring(i, i + bitNums), 2);
                codes.Add(code);
            }

            return String.Join("", codes);
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
            var s1 = new List<char>();
            foreach (var row in matrix)
            {
                foreach (var elem in row)
                {
                    s1.Add(block[elem - 1]);
                }
            }

            return string.Join("", s1);
        }

        string Feistel(string r, string key)
        {
            var rExp = Permutation(r, Utils.ExpandMatrix);

            var bStr = XOR(rExp, key);
            var bVec = Utils.SplitToChunks(bStr, 6);

            var i = 0;
            var res = "";

            foreach (var bi in bVec)
            {
                var a = Convert.ToInt32(string.Concat(bi[0],bi[bi.Length - 1]), 2);
                var b = Convert.ToInt32(bi.Substring(1,bi.Length-1), 2);

                var sMatrix = Utils.selectS(i);

                var n = sMatrix[a][b];

                var binN = NormalizeToSize(Convert.ToString(n, 2), 4);

                res += binN;

                i++;
            }

            return Permutation(res, Utils.pPermutationMatrix);
        }

        string DecryptOnce(string bInput, string bKey)
        {
            var bLeft = bInput.Substring(0, bInput.Length / 2);
            var bRight = bInput.Substring(bInput.Length / 2);

            return bRight + XOR(bLeft, bRight);
        }

        string GenerateKey(string bKey)
        {
            var keyChunks = Utils.SplitToChunks(bKey, 8);

            for (int i = 0; i < keyChunks.Count; i++)
            {
                var oneCount = Utils.charsCount('1', keyChunks[i]);
                keyChunks[i] += oneCount % 2 == 0 ? '1' : '0';
            }

            bKey = Permutation(string.Join("", keyChunks), Utils.KeyPermutationMatrix1);

            return bKey;
        }

        CryptoResult Encrypt(string binaryData, string key)
        {
            var input = ExpandBinary(binaryData); 

            keys.Clear();

            var blocksBinary = BinaryToBlocks(input);

            for (int i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.PermutationMatrix1);
            }

            var cd = GenerateKey(StringToBinary(key));
            var cdSplit = Utils.SplitToChunks(cd, cd.Length / 2);

            var c = cdSplit[0];
            var d = cdSplit[1];

            for (int r = 0; r < rounds; r++)
            {
                c = ShiftsLeft(c, shifts[r]);
                d = ShiftsLeft(d, shifts[r]);

                cd = Permutation(c + d, Utils.CDPermutationMatrix);

                var roundKey = cd;

                keys.Add(roundKey);

                for (int b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = EncryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = NormalizeToSize(tmpBlock, blockLength);
                }
            }

            for (int i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.PermutationMatrix16);
            }

            key = BinaryToString(cd);

            return new CryptoResult(string.Join("",blocksBinary), key);

        }

        private string ShiftsLeft(string bKey, int shift)
        {
            for (int i = 0; i < shift; i++)
            {
                bKey = bKey + bKey[0];
                bKey = bKey.Substring(1);
            }

            return bKey;
        }

        CryptoResult Decrypt(string input, string encryptedKey)
        {
            var blocksBinary = BinaryToBlocks(input);
            for (int i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.PermutationMatrix1);
            }

            for (int r = 0; r < rounds; r++)
            {
                var roundKey = keys[rounds - r - 1];

                for(var b = 0; b < blocksBinary.Count; b++)
                {
                    var tmpBlock = DecryptOnce(blocksBinary[b], roundKey);
                    blocksBinary[b] = NormalizeToSize(tmpBlock, blockLength);
                }
            }

            for (int i = 0; i < blocksBinary.Count; i++)
            {
                blocksBinary[i] = Permutation(blocksBinary[i], Utils.PermutationMatrix16);
            }

            return new CryptoResult(string.Join("", blocksBinary), "");
        }
    }

    class CryptoResult
    {
        public CryptoResult(string value, string key)
        {
            this.value = value;
            this.key = key;
        }

        string value { get; set; }
        string key { get; set; }

    }

    class Utils
    {
        public static List<List<int>> PermutationMatrix1 = new List<List<int>>
        {
            new List<int> { 58, 50, 42, 34, 26, 18, 10, 2 },

            new List<int>{ 60, 52, 44, 36, 28, 20, 12, 4 },

            new List<int>{ 62, 54, 46, 38, 30, 22, 14, 6},

            new List<int>{ 64, 56, 48, 40, 32, 24, 16, 8 },

            new List<int>{ 57, 49, 41, 33, 25, 17, 9, 1 },

            new List<int>{ 59, 51, 43, 35, 27, 19, 11, 3 },

            new List<int>{ 61, 53, 45, 37, 29, 21, 13, 5 },

            new List<int>{ 63, 55, 47, 39, 31, 23, 15, 7 }
        };

        public static List<List<int>> PermutationMatrix16 = new List<List<int>>
        {
            new List<int> {40, 8, 48, 16, 56, 24, 64, 32 },
            new List<int>{ 39, 7, 47, 15, 55, 23, 63, 31},
            new List<int>{ 38, 6, 46, 14, 54, 22, 62, 30 },
            new List<int>{37, 5, 45, 13, 53, 21, 61, 29 },
            new List<int>{36, 4, 44, 12, 52, 20, 60, 28 },
            new List<int>{35, 3, 43, 11, 51, 19, 59, 27 },
            new List<int>{34, 2, 42, 10, 50, 18, 58, 26 },
            new List<int>{33, 1, 41, 9, 49, 17, 57, 25 }
        };

        public static List<List<int>> KeyPermutationMatrix1 = new List<List<int>>
        {
          new List<int>{57, 49, 41, 33, 25, 17, 9 },
          new List<int>{1, 58, 50, 42, 34, 26, 18 },
          new List<int>{10, 2, 59, 51, 43, 35, 27 },
          new List<int>{19, 11, 3, 60, 52, 44, 36 },
          new List<int>{63, 55, 47, 39, 31, 23, 15},
          new List<int>{7, 62, 54, 46, 38, 30, 22},
          new List<int>{14, 6, 61, 53, 45, 37, 29},
          new List<int>{21, 13, 5, 28, 20, 12, 4}
        };

        public static List<List<int>> ExpandMatrix = new List<List<int>>
        {
          new List<int>{32, 1, 2, 3, 4, 5},
          new List<int>{4, 5, 6, 7, 8, 9},
          new List<int>{8, 9, 10, 11, 12, 13},
          new List<int>{12, 13, 14, 15, 16, 17},
          new List<int>{16, 17, 18, 19, 20, 21},
          new List<int>{20, 21, 22, 23, 24, 25},
          new List<int>{24, 25, 26, 27, 28, 29},
          new List<int>{28, 29, 30, 31, 32, 1}
        };

        public static List<List<int>> pPermutationMatrix = new List<List<int>>
        {

            new List<int>{16, 7, 20, 21, 29, 12, 28, 17 },

            new List<int>{1, 15, 23, 26, 5, 18, 31, 10},

            new List<int>{2, 8, 24, 14, 32, 27, 3, 9},

            new List<int>{19, 13, 30, 6, 22, 11, 4, 25}
        };

        public static List<List<int>> CDPermutationMatrix = new List<List<int>>
        {
            new List<int>{14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4},

            new List<int>{26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40},

            new List<int>{51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32}
        };

        public static List<List<int>> s1 = new List<List<int>>
        {
          new List<int>{14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},


          new List<int>{0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},


          new List<int>{4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,},


          new List<int>{15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13},
        };

        public static List<List<int>> s2 = new List<List<int>>
        {
            new List<int>{15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},

            new List<int>{3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},

            new List<int>{0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},

            new List<int>{13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
        };

        public static List<List<int>> s3 = new List<List<int>>
        {
      
          new List<int>{10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
      
          new List<int>{13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
      
          new List<int>{13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
      
          new List<int>{1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
        };

        public static List<List<int>> s4 = new List<List<int>>
        {

          new List<int>{7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},

          new List<int>{13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},

          new List<int>{10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },

          new List<int>{3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
        };

        public static List<List<int>> s5 = new List<List<int>>
        {

            new List<int>{2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},

            new List<int>{14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},

            new List<int>{4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},

            new List<int>{11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3},
        };

        public static List<List<int>> s6 = new List<List<int>>
        {

            new List<int>{12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},

            new List<int>{10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},

            new List<int>{9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},

            new List<int>{4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
        };

        public static List<List<int>> s7 = new List<List<int>>
        {

            new List<int>{4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},

            new List<int>{13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},

            new List<int>{1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},

            new List<int>{6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
        };

        public static List<List<int>> s8 = new List<List<int>>
        {
            new List<int>{13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
            new List<int>{1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
            new List<int>{7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
            new List<int>{2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
        };

        public static List<List<int>> selectS(int sIdx)
        {
            return new List<List<List<int>>> { s1, s2, s3, s4, s5, s6, s7, s8 }[sIdx];
        }

        public static List<string> SplitToChunks(string s, int chunks)
        {
            var l = new List<string>();

            for (var i = 0; i < s.Length; i += chunks)
            {
                l.Add(s.Substring(i,  Math.Clamp((i + chunks), 0, s.Length)));
            }

            return l;
        }

        public static int charsCount(char ch, string str)
        {
            int c = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == ch) c++;
            }
            return c;
        }
    }

}
