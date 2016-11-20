
using System;
using System.Collections.Generic;

namespace wav2
{
    public static class rsa
    {
        private static int p;
        private static int q;
        public static Key pubKey;
        public static Key privKey;

        private static void createPrimeNumbers()
        {
            p = 233;
            q = 293;
        }

        public static void createKeys()
        {
            createPrimeNumbers();
            int euler = (p - 1) * (q - 1);
            int module = p * q;
            int e = relativelyPrime(euler);
            int d = eulerInverse(e, euler);
            pubKey = new Key(e, module);
            privKey = new Key(d, module);
        }

        public static void setPublicKey(int e, int n)
        {
            pubKey = new Key(e, n);
        }

        public static void setPrivateKey(int d, int n)
        {
            privKey = new Key(d, n);
        }

        public static byte[] encrypt(byte[] bytes)
        {
            List<byte> result = new List<byte>();
            int value = 0;
            string hexArray = "";
            for (int j = 0; j < bytes.Length; j += 2)
            {
                for (int i = j; i < j + 2; i++)
                {
                    if (i >= bytes.Length)
                        hexArray = hexArray + "00";
                    else
                    {
                        hexArray = hexArray + bytes[i].ToString("X2");
                    }
                }
                value = Convert.ToInt32(hexArray, 16);
                //  Console.WriteLine("VAL: "+value);
                value = powerModulo(value, pubKey.e, pubKey.n);
                hexArray = value.ToString("X6");
                string text = "";
                for (int i = 0; i < hexArray.Length; i += 2)
                {
                    text = hexArray[i].ToString() + hexArray[i + 1].ToString();
                    result.Add(Convert.ToByte(text, 16));
                }
                hexArray = "";
            }

            return result.ToArray();
        }

        public static byte[] decrypt(byte[] bytes)
        {
            List<byte> result = new List<byte>();
            int value = 0;
            string hexArray = "";
            for (int j = 0; j < bytes.Length; j += 3)
            {
                for (int i = j; i < j + 3; i++)
                {
                    if (i >= bytes.Length)
                        hexArray = hexArray + "00";
                    else
                    {
                        hexArray = hexArray + bytes[i].ToString("X2");
                    }
                }
                value = Convert.ToInt32(hexArray, 16);
                //  Console.WriteLine("VAL: " + value);
                value = powerModulo(value, privKey.e, privKey.n);
                hexArray = value.ToString("X4");
                string text = "";
                for (int i = 0; i < hexArray.Length; i += 2)
                {
                    text = hexArray[i].ToString() + hexArray[i + 1].ToString();
                    result.Add(Convert.ToByte(text, 16));
                }
                hexArray = "";
            }

            return result.ToArray();
        }

        private static int relativelyPrime(int a)
        {
            int e;
            for (e = 3; GCD(e, a) != 1; e += 2) ;
            return e;
        }

        private static int eulerInverse(int e, int baza)
        {
            int result = 0;
            int rest = 0;
            while (rest!=1)
            {
                ++result;
                rest = (e * result) % baza;
            }
            return result;
        }

        private static int GCD(int a, int b) //Największy wspólny dzielnik
        {
            int t;

            while (b != 0)
            {
                t = b;
                b = a % b;
                a = t;
            };
            return a;
        }

        private static int powerModulo(int a, int b, int m)
        {
            int i;
            long result = 1;
            long x = a % m;

            for (i = 1; i <= b; i <<= 1)
            {
                x %= m;
                if ((b & i) != 0)
                {
                    result *= x;
                    result %= m;
                }
                x *= x;
            }

            return (int)result;
        }
    }


    public struct Key
    {
        public int e { get; private set; }
        public int n { get; private set; }
        public Key(int e, int n)
        {
            this.e = e;
            this.n = n;
        }
    }
}
