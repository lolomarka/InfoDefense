using InfoDefense.Pages;

using System.Windows;

namespace InfoDefence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WheatstoneAlgButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new WheatstoneAlghoritmPage();
        }

        private void RSAButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new RSAPage();
        }

        private void DeffieHellmanButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new DeffieHellmanPage();
        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new BlockPage();
        }

        private void CRCButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new CRC16Page();
        }

        private void RLEButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new RLEPage();
        }

        private void HashButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new HashPage();
        }

        private void GammaButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.RemoveBackEntry();
            MainFrame.Content = new GammaPage();
        }
    }
}
