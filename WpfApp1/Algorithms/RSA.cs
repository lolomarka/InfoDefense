using System;
using System.Collections.Generic;
using System.Numerics;

namespace InfoDefence.Algorithms
{
    public class RSA
    {
        public BigInteger mod;
        public BigInteger fi;
        public BigInteger e;
        public BigInteger d;

        public RSA(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;
            mod = p * q;
            fi = (p - 1) * (q - 1);
            e = FindE(fi);
            e.TryModInverse(fi, out d);
        }



        public List<BigInteger> Encrypt(string message)
        {
            var codes = message.ToCharArray();
            var encryptedMsg = new List<BigInteger>();
            for (int i = 0; i < codes.Length; i++)
            {
                var code = codes[i];
                BigInteger crypdetCode = BigInteger.ModPow((BigInteger)code, e, mod);
                encryptedMsg.Add(crypdetCode);
            }

            return encryptedMsg;
        }

        public string Decrypt(List<BigInteger> message)
        {
            var decrypted = "";
            for (int i = 0; i < message.Count; i++)
            {
                var code = message[i];
                BigInteger decryptedCode = BigInteger.ModPow(code, d, mod);
                decrypted += (char)decryptedCode;
            }

            return decrypted;
        }

        public string Decrypt(string cryptedMessage)
        {
            var tmp = cryptedMessage.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<BigInteger> semiRes = new List<BigInteger>();
            foreach (var item in tmp)
            {
                if (!BigInteger.TryParse(item, out BigInteger res))
                    return "Ошибка";
                semiRes.Add(res);
            }

            return Decrypt(semiRes);
        }

        private BigInteger FindE(BigInteger fi)
        {
            do
            {
                e = GetNextPrime(e);
                if (fi % e != 0)
                    return e;
            } while (e < fi);
            return 0;
        }

        BigInteger GetNextPrime(BigInteger n)
        {
            bool isPrimeNumber = false;

            do
            {
                n++;
                isPrimeNumber = IsPrime(n);
            } while (!isPrimeNumber);
            return n;
        }

        public static bool IsPrime(BigInteger n)
        {
            if (n <= 1) return false;
            if (n <= 3) return true;
            if (n % 2 == 0 || n % 3 == 0)
                return false;

            for (BigInteger i = 5; i * i < n; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
    }

    internal static class BigIntegerExt
    {
        public static BigInteger ModInverse(this BigInteger step, BigInteger m)
        {
            return (1 / step) % m;
        }

        public static bool TryModInverse(this BigInteger number, BigInteger modulo, out BigInteger result)
        {
            if (number < 1) throw new ArgumentOutOfRangeException(nameof(number));
            if (modulo < 2) throw new ArgumentOutOfRangeException(nameof(modulo));
            BigInteger n = number;
            BigInteger m = modulo, v = 0, d = 1;
            while (n > 0)
            {
                BigInteger t = m / n, x = n;
                n = m % x;
                m = x;
                x = d;
                d = checked(v - t * x); // Just in case
                v = x;
            }
            result = v % modulo;
            if (result < 0) result += modulo;
            if ((long)number * result % modulo == 1L) return true;
            result = default;
            return false;
        }
    }
}
