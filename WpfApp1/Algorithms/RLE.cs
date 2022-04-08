using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDefense.Algorithms
{
    public class RLE
    {
        public byte[] Compress(byte[] data)
        {
            List<byte> compressed = new List<byte>();
            int i = 0;
            int len = data.Length;

            while (i <= len - 1)
            {
                byte counter = 1;
                byte current = data[i];
                var j = i;
                while (j < len - 1)
                {
                    if (data[j] == data[j + 1] && counter != 255)
                    {
                        counter++;
                        j++;
                    }
                    else
                    {
                        break;
                    }
                }
                compressed.Add(counter);
                compressed.Add(current);
                i = j + 1;
            }

            return compressed.ToArray();

        }
    }

}
