using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using InfoDefense.Algorithms;


namespace InfoDefense.Pages
{
    /// <summary>
    /// Interaction logic for CRC16Page.xaml
    /// </summary>
    public partial class CRC16Page : Page
    {
        static CRC16 crc16 = new CRC16();
        public CRC16Page()
        {
            InitializeComponent();
        }

        public static string ToBinary(byte[] data)
        {
            return string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        public static byte[] ToBytes(string binary)
        {
            int numOfBytes = binary.Length / 8;
            byte[] bytes = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(binary.Substring(8 * i, 8), 2);
            }
            return bytes;
        }

        private ushort calcChecksum(byte[] bytes)
        {
            return crc16.Checksum(bytes);
        }

        private void onSendMessage(object sender, RoutedEventArgs e)
        {
            var sendMessage = messageToSendTextBox.Text;
            if (sendMessage != string.Empty)
            {
                var bytes = Encoding.ASCII.GetBytes(sendMessage);
                var recievedMessageBinary = ToBinary(bytes);
                messageToRecieveTextBox.Text = recievedMessageBinary;

            }
        }

        private void onChecksumSend(object sender, RoutedEventArgs e)
        {
            var sendMessage = messageToSendTextBox.Text;
            var bytes = Encoding.ASCII.GetBytes(sendMessage);
            var chs = calcChecksum(bytes);
            firstChecksumTextBox.Text = $"0x{chs:X}";

        }

        private void onChecksumRecieve(object sender, RoutedEventArgs e)
        {
            var recievedMessageBinary = messageToRecieveTextBox.Text;
            var recievedBytes = ToBytes(recievedMessageBinary);
            var chs = calcChecksum(recievedBytes);
            secondSendTextBox.Text = $"0x{chs:X}";

        }
    }

}
