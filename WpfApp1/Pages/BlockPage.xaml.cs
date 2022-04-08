using InfoDefense.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InfoDefense.Algorithms;
using System.Linq;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for BlockPage.xaml
    /// </summary>
    public partial class BlockPage : Page
    {
        DefaultDialogService DialogService = new DefaultDialogService();
        DefaultFileService FileService = new DefaultFileService();

        Block crypter;

        public BlockPage()
        {

            InitializeComponent();
            Key_TextBox.Text = "asdfghj";
            crypter = new Block();
        }

        private void Path_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = true; 

            if(dialog.ShowDialog() == true)
            {
                Path_TextBlock.Text = dialog.FileName;
            }
        }

        private async  void OpenFile()
        {
            if (DialogService.OpenFileDialog())
            {
                SourceTextBox.Text =  await FileService.OpenAsync(DialogService.FilePath);
            }
        }

        private void ReadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] codes = Encoding.ASCII.GetBytes(SourceTextBox.Text);
            string binaryCodes = crypter.bytesToBinary(codes);
            CryptoResult cresult = crypter.encrypt(binaryCodes, Key_TextBox.Text);
            EncryptedTextBox.Text = cresult.value;
        }

        

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string bin = crypter.decrypt(EncryptedTextBox.Text, Key_TextBox.Text).value;
            DecryptedTextBox.Text = crypter.binaryToString(bin);
        }
    }
}