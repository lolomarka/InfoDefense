using InfoDefense.Algorithms;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for WheatStoneAlghoritmPage.xaml
    /// </summary>
    public partial class WheatstoneAlghoritmPage : Page
    {
        private WheatstoneEncryption wheatstoneEncryption;

        public WheatstoneAlghoritmPage()
        {
            InitializeComponent();
            wheatstoneEncryption = new WheatstoneEncryption();


            LeftTable.ItemsSource = GetViewFromArray(wheatstoneEncryption.LeftTable);
            RightTable.ItemsSource = GetViewFromArray(wheatstoneEncryption.RightTable);
        }

        private DataView GetViewFromArray(char[,] arr)
        {
            DataTable table = new DataTable();

            for (int i = 0; i < 8; i++)
            {
                table.Columns.Add();
            }
            for (int i = 0; i < 9; i++)
            {
                object[] rowData = new object[8];
                for (int j = 0; j < 8; j++)
                {
                    rowData[j] = arr[j, i];
                }
                table.Rows.Add(rowData);
            }
            return table.DefaultView;
        }

        private void ConfirmInputButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputBox.Text;
            string outputText = wheatstoneEncryption.Encrypt(inputText);

            OutputBlock.Text = outputText;
        }

        private void ConfirmDecryptInputButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = EncryptedInputBox.Text;
            string outputText = wheatstoneEncryption.Encrypt(inputText);

            DecryptedOutputBlock.Text = wheatstoneEncryption.Decrypt(inputText);
        }
    }
}
