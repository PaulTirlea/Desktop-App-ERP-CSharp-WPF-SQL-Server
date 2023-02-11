using System;
using System.Collections.Generic;
using System.IO;
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
using MessagingToolkit.Barcode;
using Microsoft.Win32;

namespace TirleaPaul2022_QRCode
{
    /// <summary>
    /// Interaction logic for ScanBarcode.xaml
    /// </summary>
    public partial class ScanBarcode : UserControl
    {
        BarcodeDecoder decoder = new BarcodeDecoder();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public ScanBarcode()
        {
            InitializeComponent();
        }

        public void ScanBarcodeImage()
        {
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    imageBarcode.Source = bitmapImage;
                    WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);
                    Result result = decoder.Decode(writeableBitmap);
                    textBlockBarcode.Text = result.ToString();
                }
                else
                {
                    MessageBox.Show("Nu ați ales corespunzător imaginea cu codul de bare", "Decriptarea nu se poate efectua",
                                       MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
            catch (Exception ex)
            {
                string caption = "Decoding barcode picture ERROR";
                string exception = "EXCEPTION!\n\nThere is an error. Error message: " + ex.Message;
                MessageBox.Show(exception, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonScanBarcode_Click(object sender, RoutedEventArgs e)
        {
            ScanBarcodeImage();
            if(textBlockBarcode.Text != null)
            {
                buttonCopy.Visibility = Visibility.Visible;
            }
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBlockBarcode.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            buttonCopy.Visibility = Visibility.Hidden;
        }
    }
}
