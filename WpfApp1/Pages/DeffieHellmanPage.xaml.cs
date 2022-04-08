using System.Collections.Generic;
using System.Numerics;
using System.Windows.Controls;
using InfoDefense.Algorithms;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Логика взаимодействия для DeffieHellmanPage.xaml
    /// </summary>
    public partial class DeffieHellmanPage : Page
    {
        Diffie_Hellman alghoritm;

        List<BigInteger> lastCryptedMessage;

        public DeffieHellmanPage()
        {
            InitializeComponent();

            var q = DiffieHellmanUtils.GenerateBigInt(12);
            var n = DiffieHellmanUtils.GenerateBigInt(12);
            alghoritm = new Diffie_Hellman(q, n);

            QInputTextBox.Text = alghoritm.Q.ToString();
            NInputTextBox.Text = alghoritm.N.ToString();
            XInputTextBox.Text = alghoritm._X.ToString();
            YInputTextBox.Text = alghoritm._Y.ToString();
            KxInputTextBox.Text = alghoritm._Kx.ToString(); 
            KyInputTextBox.Text = alghoritm._Kx.ToString(); 
            AInputTextBox.Text = alghoritm._A.ToString();
            BInputTextBox.Text = alghoritm._B.ToString();
        }

        private void EncryptButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var t = alghoritm.Encrypt(InputTextBox.Text);
            EncryptedTextBox.Text = t.Item1;
            lastCryptedMessage = t.Item2;
        }

        private void DecryptButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DecryptedTextBox.Text = alghoritm.Decrypt(lastCryptedMessage);
        }

        private void QInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(BigInteger.TryParse(QInputTextBox.Text, out BigInteger value))
            {
                alghoritm.Q = value;
            }
        }

        private void NInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BigInteger.TryParse(NInputTextBox.Text, out BigInteger value))
            {
                alghoritm.N = value;
            }
        }
    }
}
