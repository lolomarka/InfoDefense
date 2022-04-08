using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDefense.Algorithms
{
    public class HNode
    {
        public int probability;
        public byte value;
        public HNode left;
        public HNode right;
        public string code = "";

        public HNode(int probability, byte value, HNode left = null, HNode right = null)
        {
            this.probability = probability;
            this.value = value;
            this.left = left;
            this.right = right;
        }
    }
    class Huffman
    {
        private Dictionary<byte, int> Probs(byte[] bytes)
        {
            var probsMap = new Dictionary<byte, int>();

            foreach (var b in bytes)
            {
                if (probsMap.ContainsKey(b))
                {
                    probsMap[b]++;
                }
                else
                {
                    probsMap[b] = 1;
                }
            }

            return probsMap;
        }

        private Dictionary<byte, string> Codes(HNode n, string code, Dictionary<byte, string> codes)
        {
            string nCode = code + n.code;

            if (n.left != null)
            {
                codes = Codes(n.left, nCode, codes);
            }
            if (n.right != null)
            {
                codes = Codes(n.right, nCode, codes);
            }

            if (n.left == null && n.right == null)
            {
                codes[n.value] = nCode;
            }

            return codes;
        }

        private string Encode(byte[] bytes, Dictionary<byte, string> codes)
        {
            List<string> enc = new List<string>();

            foreach (var b in bytes)
            {
                enc.Add(codes[b]);
            }

            return string.Join("", enc);

        }

        public (string, Dictionary<byte, string>) Compress(byte[] bytes)
        {
            var probs = Probs(bytes);
            List<HNode> nodes = new List<HNode>();

            foreach (var p in probs)
            {
                nodes.Add(new HNode(p.Value, p.Key));
            }

            while (nodes.Count > 1)
            {
                nodes.Sort((x, y) => x.probability - y.probability);

                var r = nodes[0];
                var l = nodes[1];

                r.code = "1";
                l.code = "0";

                var newNode = new HNode(l.probability + r.probability, 0, l, r);

                nodes.RemoveAt(0);
                nodes.RemoveAt(0);
                nodes.Add(newNode);
            }

            var codes = Codes(nodes[0], "", new Dictionary<byte, string>());
            var encoded = Encode(bytes, codes);
            return (encoded, codes);
        }
    }

}
