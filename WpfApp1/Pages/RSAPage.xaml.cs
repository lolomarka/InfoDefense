using InfoDefense.Algorithms;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for RSAPage.xaml
    /// </summary>
    public partial class RSAPage : Page
    {
        RSA rsa;

        BigInteger standartP = 10000000000007031337;
        BigInteger standartQ = 10000000000007031389;

        Random Random = new Random();

        List<BigInteger> lastEncryptedMessage;
        public RSAPage()
        {
            InitializeComponent();
            rsa = new RSA(standartP, standartQ);
        }

        private BigInteger GenerateQ()
        {
            BigInteger num;
            do
            {
                num = standartQ * Random.Next(4) - Random.Next(9);
            } while (!RSA.IsPrime(num));

            return num;
        }

        private BigInteger GenerateP()
        {
            BigInteger num;
            do
            {
                num = standartP * Random.Next(4) - Random.Next(9);
            } while (!RSA.IsPrime(num));

            return num;
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            lastEncryptedMessage = rsa.Encrypt(EncryptTextBox.Text);
            StringBuilder str = new StringBuilder();
            foreach (var item in lastEncryptedMessage)
            {
                str.Append(item.ToString() + ' ');
            }
            OutputBlock.Text = str.ToString();
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastEncryptedMessage == null) return;
            OutputBlock.Text = rsa.Decrypt(EncryptTextBox.Text);
        }

        private void InternalsButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("p = " + rsa.P);
            stringBuilder.AppendLine("q = " + rsa.Q);
            stringBuilder.AppendLine("fi = " + rsa.fi);
            stringBuilder.AppendLine("mod =" + rsa.mod);
            stringBuilder.AppendLine("e =" + rsa.e);
            stringBuilder.AppendLine("d = " + rsa.d);

            OutputBlock.Text = stringBuilder.ToString();
        }
    }
}
