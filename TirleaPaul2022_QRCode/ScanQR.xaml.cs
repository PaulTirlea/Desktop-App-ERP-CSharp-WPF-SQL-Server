using System;
using System.Collections.Generic;
using System.Drawing;
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
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;

namespace TirleaPaul2022_QRCode
{
    /// <summary>
    /// Interaction logic for ScanQR.xaml
    /// </summary>
    public partial class ScanQR : UserControl
    {
        QRCodeDecoder decoder = new QRCodeDecoder();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public ScanQR()
        {
            InitializeComponent();
        }

        public void ScanQRImage()
        {
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    imageQR.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    texBoxQR.Text = decoder.Decode(new QRCodeBitmapImage(new Bitmap(openFileDialog.FileName)));
                }
                else
                {
                    MessageBox.Show("Nu ați ales corespunzător imaginea cu codul de bare", "Decriptarea nu se poate efectua",
                                       MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
            catch(Exception ex)
            {
                string caption = "Decoding QRCode picture ERROR";
                string exception = "EXCEPTION!\n\nThere is an error. Error message: " + ex.Message;
                MessageBox.Show(exception, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonScanQR_Click(object sender, RoutedEventArgs e)
        {
            ScanQRImage();
            if(texBoxQR.Text != null)
            {
                buttonCopy.Visibility = Visibility.Visible;
            }
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(texBoxQR.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            buttonCopy.Visibility = Visibility.Hidden;
        }
    }
}
