using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace InfoDefense.Algorithms
{
    public class Diffie_Hellman
    {
        #region Properties
        
        private BigInteger _Q { get; set; }
        public BigInteger Q { get => _Q; set { _Q = value; CalculateValues(); } }

        private BigInteger _N { get; set; }
        public BigInteger N { get => _N; set { _N = value; CalculateValues(); } }

        public BigInteger _X { get; private set; }
        public BigInteger _A { get; private set; } 
        public BigInteger _Y { get; private set; } 
        public BigInteger _B { get; private set; }
        public BigInteger _Kx { get; private set; }
        public BigInteger _Ky { get; private set; }
        #endregion
        
        public Diffie_Hellman(BigInteger q, BigInteger n)
        {
            _Q = q;
            _N = n;

            CalculateValues();
        }

        private void CalculateValues()
        {
            _X = DiffieHellmanUtils.GenerateBigInt(64);
            _Y = DiffieHellmanUtils.GenerateBigInt(64);
            _A = BigInteger.ModPow(_Q, _X, _N);

            _B = BigInteger.ModPow(_Q, _Y, _N);
            _Kx = BigInteger.ModPow(_B, _X, _N);
            _Ky = BigInteger.ModPow(_B, _Y, _N);
        }

        public string Encrypt(string message)
        {
            string res = "";
            foreach (var ch in message)
            {
                res += (char)(ch + _Kx);
            }

            return res;
        }

        public string Decrypt(string message)
        {
            string res = "";
            foreach (var ch in message)
            {
                res += (char)(ch - _Ky);
            }

            return res;
        }
    }

    public static class DiffieHellmanUtils
    {
        public static BigInteger Pow(this BigInteger a, BigInteger n)
        {
            var res = a;
            for (int i = 0; i < n; i++)
            {
                res *= a;
            }

            return res;
        }
        private static Random Gen = new Random();
        
        public static BigInteger GenerateBigInt(int bits)
        {
            while (true)
            {
                var rng = new RNGCryptoServiceProvider();
                byte[] bytes = new byte[bits / 8];
                rng.GetBytes(bytes);

                BigInteger p = new BigInteger(bytes);
                
                if(!p.IsEven && !p.IsPowerOfTwo)
                    return BigInteger.Abs(p);
            }
        }
    }
}
