using MessagingToolkit.Barcode;
using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TirleaPaul2022_QRCode
{
    /// <summary>
    /// Interaction logic for CreateBarcode.xaml
    /// </summary>
    public partial class CreateBarcode : UserControl
    {
        #region ================================== VARIABILE ==================================
        BarcodeEncoder encoder = new BarcodeEncoder();
        WriteableBitmap writeableBitmap;
        SaveFileDialog saveFile = new SaveFileDialog();
        #endregion

        #region ================================== CONSTRUCTOR ==================================
        public CreateBarcode()
        {
            InitializeComponent();
        }
        #endregion

        #region ================================== METODE ==================================
        public void CreateEncodedBarcode()
        {
            try
            {
                if (String.IsNullOrEmpty(textBoxBarcode.Text))
                {
                    string caption = "Atenție !!!";
                    string message = "Prima dată introduceți textul pe care doriți să-l reprezentați ca și cod de bare...";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage image = MessageBoxImage.Warning;

                    MessageBox.Show(message, caption, button, image);
                }
                else
                {
                    writeableBitmap = encoder.Encode(BarcodeFormat.Code128, textBoxBarcode.Text);
                    imageBarcode.Source = writeableBitmap;
                }
            }
            catch(Exception ex)
            {
                string caption = "Barcode generator ERROR";

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

        public void SaveEncodedBarcode()
        {
            try
            {
                if (writeableBitmap != null)
                {
                    saveFile.Filter = "PNG|*.png";
                    saveFile.FileName = textBoxBarcode.Text;
                    if (saveFile.ShowDialog() == true)
                    {
                        Tools.SaveWriteableBitmap(writeableBitmap, string.Concat(saveFile.FileName, ".png"));

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
                    string message = "Prima dată introduceți textul pe care doriți să-l reprezentați ca și cod de bare și generați codul";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage image = MessageBoxImage.Warning;

                    MessageBox.Show(message, caption, button, image);
                }
            }
            catch(Exception ex)
            {
                string caption = "Save barcode picture ERROR";
                string exception = "EXCEPTION!\n\nThere is an error. Error message: " + ex.Message;
                MessageBox.Show(exception, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        #endregion

        #region ================================== EVENIMENTE ==================================
        private void buttonCreateBarcode_Click(object sender, RoutedEventArgs e)
        {
            CreateEncodedBarcode();
        }

        private void buttonSaveBarcode_Click(object sender, RoutedEventArgs e)
        {
            SaveEncodedBarcode();
        }

        private void textBoxBarcode_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
    }
}
