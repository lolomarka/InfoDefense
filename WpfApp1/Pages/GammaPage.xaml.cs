using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for GammaPage.xaml
    /// </summary>
    public partial class GammaPage : Page
    {
        private string alph = "1234567890qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNMёйцукенгшщзхъждлорпавыфячсмитьбюЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЮБЬТИМСЧЯ ";

        private List<int> _codes(string data)
        {
            List<int> codes = new List<int>();
            foreach (var b in data.ToCharArray())
            {
                codes.Add(alph.IndexOf(b));
            }
            return codes;
        }

        private string _str(List<int> codes)
        {
            List<char> data = new List<char>();
            foreach (var c in codes)
            {
                data.Add(alph[c]);
            }
            return String.Concat<char>(data);
        }

        private string encrypt(string data, string key)
        {
            var codes = _codes(data);
            var KeyCodes = _codes(key);

            List<int> eCodes = new List<int>();
            for (int i = 0; i < codes.Count; i++) eCodes.Add(0);

            for (var i = 0; i < codes.Count; i++)
            {
                eCodes[i] = (codes[i] ^ KeyCodes[i % KeyCodes.Count]);
            }

            return _str(eCodes);
        }

        public GammaPage()
        {
            InitializeComponent();
        }

        private void OnEncryptClick(object sender, RoutedEventArgs e)
        {
            OutputBox.Text = encrypt(InputBox.Text, KeyBox.Text);
        }

        private void OnDecryptClick(object sender, RoutedEventArgs e)
        {
            DecryptBox.Text = encrypt(OutputBox.Text, KeyBox.Text);
        }
    }
}
