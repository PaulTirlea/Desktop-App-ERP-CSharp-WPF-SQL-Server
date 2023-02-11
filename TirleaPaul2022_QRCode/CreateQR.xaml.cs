using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
    /// Interaction logic for CreateQR.xaml
    /// </summary>
    public partial class CreateQR : UserControl
    {
        QRCodeEncoder encoder = new QRCodeEncoder();
        Bitmap bitmap;
        SaveFileDialog saveFile = new SaveFileDialog();
        public CreateQR()
        {
            InitializeComponent();
        }

        public void CreateEncodedQR()
        {
            try
            {
                if (String.IsNullOrEmpty(texBoxQR.Text))
                {
                    string caption = "Atenție !!!";
                    string message = "Prima dată introduceți textul pe care doriți să-l reprezentați ca și cod QR...";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage image = MessageBoxImage.Warning;

                    MessageBox.Show(message, caption, button, image);
                }
                else
                {
                    encoder.QRCodeScale = 8;
                    bitmap = encoder.Encode(texBoxQR.Text);
                    imageQR.Source = Tools.ToBitmapImage(bitmap);
                }
            }
            catch(Exception ex)
            {
                string caption = "QRCode generator ERROR";

                string message = null;
                string exception = null;
                string innerException = null;

                exception = "EXCEPTION!\n\n" + ex.Message;
                if (innerException != null)
                {
                    innerException = "INNER EXCEPTION: \n" + ex.InnerException.Message +
                                    "SOURCE: \n" + ex.InnerException.Source;

                    message = $"{exception}\n\n{innerException}";

                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {
                    MessageBox.Show(exception, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void SaveEncodedQR()
        {
            try
            {
                if(bitmap != null)
                {
                    saveFile.Filter = "PNG|*.png";
                    saveFile.FileName = texBoxQR.Text;
                    if (saveFile.ShowDialog() == true)
                    {
                        bitmap.Save(string.Concat(saveFile.FileName, ".png"), ImageFormat.Png);
                    }
                    else
                    {
                        MessageBox.Show("Nu ați ales locul corespunzător datele ce țin de salvare.", "Salvarea nu se poate efectua",
                                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                }
                else
                {
                    string caption = "Atenție !!!";
                    string message = "Prima dată introduceți textul pe care doriți să-l reprezentați ca și cod QR și generați codul";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage image = MessageBoxImage.Warning;

                    MessageBox.Show(message, caption, button, image);
                }
            }
            catch(Exception ex)
            {
                string caption = "Save QRCode picture ERROR";
                string exception = "EXCEPTION!\n\nThere is an error. Error message: " + ex.Message;
                MessageBox.Show(exception, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonCreateQR_Click(object sender, RoutedEventArgs e)
        {
            CreateEncodedQR();
        }

        private void buttonSaveQR_Click(object sender, RoutedEventArgs e)
        {
            SaveEncodedQR();
        }
    }
}
