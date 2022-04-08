using InfoDefense.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for RLEPage.xaml
    /// </summary>
    public partial class RLEPage : Page
    {
        RLE rle = new RLE();
        Huffman hf = new Huffman();

        int bitsInput = -1;
        int bitsRLE = -1;
        int bitsHuffman = -1;

        public RLEPage()
        {
            InitializeComponent();
        }

        private string BytesArrayToString(byte[] bytes)
        {
            return string.Join(" ", bytes.Select((b) => b.ToString()));
        }

        private int GetInputBits(string input)
        {
            return input.Length * 8;
        }

        private int GetRLEBits(byte[] input)
        {
            return input.Length * 8;
        }

        private int GetHuffmanBits(string binary)
        {
            return binary.Length;
        }

        private void SetCompressRate()
        {
            double total = -1;
            if (bitsInput != -1 && bitsHuffman != -1)
            {
                total = (double)bitsInput / bitsHuffman;
            }
            compressRate.Content = $"Input bits = {bitsInput}, RLE bits = {bitsRLE}, Huffman bits = {bitsHuffman}, Compress rate = {total}";
        }

        private void OnRLEClick(object sender, RoutedEventArgs e)
        {
            if (InputBox.Text == "") return;

            var rleCompressed = rle.Compress(Encoding.ASCII.GetBytes(InputBox.Text));
            RLEBox.Text = BytesArrayToString(rleCompressed);

            bitsInput = GetInputBits(InputBox.Text);
            bitsRLE = GetRLEBits(rleCompressed);
            SetCompressRate();

        }

        private void OnHuffmanClick(object sender, RoutedEventArgs e)
        {
            if (RLEBox.Text == "") return;

            var bytes = RLEBox.Text.Split(' ').Select((s) => byte.Parse(s)).ToArray();
            var (hfCompressed, codes) = hf.Compress(bytes);
            var asStr = string.Join(", ", codes);
            HuffmanBox.Text = hfCompressed;
            codesLabel.Content = $"Codes: {asStr}";

            bitsHuffman = GetHuffmanBits(hfCompressed);
            SetCompressRate();
        }
    }

}
