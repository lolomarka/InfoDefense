using System;
using System.Collections.Generic;
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
using InfoDefense.Algorithms;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for HashPage.xaml
    /// </summary>
    public partial class HashPage : Page
    {
        public String Key { get; set; }
        private Random rnd = new Random();
        private byte[] bytesResult;
        StringBuilder chars = new StringBuilder("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890");
        private string hash;

        private Algorithms.Block Cr { get; set; }

        public string Hash
        {
            get => hash;
            set
            {
                hash = value;
            }
        }

        public HashPage()
        {
            InitializeComponent();
            Key = "asdfghjoigyui";
            Cr = new Algorithms.Block(new List<string>() { Key });
        }

        private void GetHash()
        {
            if (Key == "") return;

            var psw = Key;
            if (psw.Length > 13)
            {
                Key = psw.Substring(0, 13);
            }
            else if (psw.Length < 13)
            {
                Key = psw + GenerateRandomString(13 - psw.Length);
            }
            psw = Key;
            var pswKey = psw.Length > 7 ? psw.Substring(0, 7) : psw;

            byte[] codes = Encoding.Unicode.GetBytes(Key);
            string binaryCodes = Cr.BytesToBinary(codes);
            var res = Cr.Encrypt(binaryCodes, pswKey);
            bytesResult = Cr.BinaryToBytes(res.value);

            Hash = ConvertToCharsCode(bytesResult);
        }

        private string GenerateRandomString(int length)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {
                str += chars[rnd.Next(0, chars.Length)];
            }
            return str;
        }

        private string ConvertToCharsCode(byte[] bytes)
        {
            string str = "";
            foreach (var item in bytes)
            {
                str += Convert.ToChar(item);
            }
            return str;
        }

        private void HashButton_Click(object sender, RoutedEventArgs e)
        {
            Key = InputBox.Text;
            GetHash();
            OutputBox.Text = hash;
            //ByteBlock.Text = bytesResult;
        }
    }
}
