using Microsoft.Win32;
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

namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for BlockPage.xaml
    /// </summary>
    public partial class BlockPage : Page
    {
        public BlockPage()
        {
            InitializeComponent();
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
    }
}