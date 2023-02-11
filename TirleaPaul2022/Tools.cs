using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TirleaPaul2022
{
    public class Tools
    {
        public string GetFilenameToOpen(string defaultFilename = null)
        {
            // se creeaza un nou dialog standard de tip Opnefile
            OpenFileDialog fileDialog = new OpenFileDialog(); // using Microsoft.Win32

            fileDialog.Title = "OPEN FILE";

            // se stabileste folderul initial (de ex. Desktop, MyDocuments sau altul)
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // se precizeaza un filtru pentru lista de fisiere afisate
            string separator = "|";
            fileDialog.Filter =
                "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif" + separator +
                "JPG files (*.jpg, *.jpeg)|*.jpg; *.jpeg" + separator +
                "PNG files (*.png)|*.png" + separator +
                "GIF files (*.gif)|*.gif" + separator +
                "ALL files (*.*)|*.*";

            // se precizeaza filtrul implicit prin numarul de ordine (incepand cu 1 !!!)
            fileDialog.FilterIndex = 1;

            // se stabileste daca se pot selecta mai multe nume de fisiere, sau nu
            fileDialog.Multiselect = false;

            // se indica numele implicit al fisierului (daca este indicat in apelul functiei)
            if (defaultFilename != null) fileDialog.FileName = defaultFilename;

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName; // calea completa catre fisier
            }
            else
            {
                return null; // utilizatorul a apasat Cnacel
            }
        }

        public string GetFilenameToSave(string defaultFilename = null)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.Title = "SAVE FILE";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string separator = "|";
            fileDialog.Filter =
                "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif" + separator +
                "JPG files (*.jpg, *.jpeg)|*.jpg; *.jpeg" + separator +
                "PNG files (*.png)|*.png" + separator +
                "GIF files (*.gif)|*.gif" + separator +
                "ALL files (*.*)|*.*";

            fileDialog.FilterIndex = 1;

            if (defaultFilename != null) fileDialog.FileName = defaultFilename;

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public BitmapImage ReadBitmapFromFile(string pathToFile)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(pathToFile, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            return bi;
        }
    }
}
