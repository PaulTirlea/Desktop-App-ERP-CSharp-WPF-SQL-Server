using TirleaPaul2022.Details_Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

namespace TirleaPaul2022.User_Control_Windows
{
    /// <summary>
    /// Interaction logic for StockWindow.xaml
    /// </summary>
    public partial class StockWindow : UserControl
    {
        private TirleaPaul2022Context context = null;
        private List<Product> ProductList = null;

        public StockWindow()
        {
            InitializeComponent();
            LoadProductData();
        }

        private void LoadProductData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Products.Load();

                //contextul ajunge la lista care contine produsele
                ProductList = context.Products.ToList();

                // se atribuie lista la sursa elementului de tip  sfDataGrid
                mySfDataGrid_Products.ItemsSource = ProductList;
            }
            catch (Exception ex)
            {
                string caption = "Loading data ...";

                string message = null;
                string exception = null;
                string innerException = null;
                string connectionString = null;

                exception = "EXCEPTION!\n" + ex.Message;
                if (innerException != null)
                {
                    innerException = "INNER EXCEPTION: \n" + ex.InnerException.Message +
                                    "SOURCE: \n" + ex.InnerException.Source;
                    connectionString = "CONNECTION STRING: \n" +
                                    context.Database.Connection.ConnectionString;

                    message = $"{exception}\n\n{innerException}\n\n{connectionString}";

                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }

        }

        private void ShowProductDetails()
        {

            // se face o serie de verificari asupra listei cu produse
            if (ProductList == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            Product currentProduct = this.mySfDataGrid_Products.SelectedItem as Product;
            // se determina obiectul Customer corespunzator randului curent din DataGrid
            if (currentProduct != null)
            {
                // se deschide fereastra cu detalii folosind constructorul specializat
                StockDetailsWindow detailsWindow = new StockDetailsWindow(currentProduct);

                // se seteaza pozitia initiala pe ecran 
                detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // se afiseaza detaliile unui Customer intr-o fereastra-copil, separata
                detailsWindow.ShowDialog();

                if (detailsWindow.DialogResult == true)
                {
                    // se salveaza modificarile facute
                    SaveData(showConfiguration: false, showSuccess: true);

                    // se reincarca datele
                    LoadProductData();
                }
            }
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra listei cu produsele
            if (ProductList == null )
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            if (showConfiguration)
            {
                // se afiseaza dialogul de configurare la care se raspunde "OK" sau "Cnacel"
                MessageBoxResult result =
                    MessageBox.Show("Doriti salvarea datelor actuale?",
                                       "Save data", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                // daca utilizatorul a apasat Cancel, atunci operatia de salvare este abandoanata
                if (result == MessageBoxResult.Cancel) return;
            }
            try
            {
                context.SaveChanges();

                if (showSuccess)
                {
                    MessageBox.Show("Datele au fost salvate cu succes.", "Save data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                string caption = "Save data ...";
                string message = "Erori la validarea datelor: \n\n";

                foreach (DbEntityValidationResult entityErr in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError error in entityErr.ValidationErrors)
                    {
                        message += string.Format("Proprietatea: {0} \n Mesaj de eroare: {1} \n\n", error.PropertyName, error.ErrorMessage);

                    }
                }
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string caption = "Save data ...";

                string message = null;
                string exception = null;
                string innerException = null;
                string connectionString = null;

                exception = "EXCEPTION!\n" + ex.Message;
                if (innerException != null)
                {
                    innerException = "INNER EXCEPTION: \n" + ex.InnerException.Message +
                                    "\nSOURCE: \n" + ex.InnerException.Source;
                    connectionString = "CONNECTION STRING: \n" +
                                    context.Database.Connection.ConnectionString;

                    message = $"{exception}\n\n{innerException}\n\n{connectionString}";

                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        private void AddNewProduct()
        {
            // se fac o serie de verificari asupra listei cu produsele
            if (ProductList == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se pregateste un nou obiect din clasa Customer
            Product newProduct = new Product();

            // se deschide o nou fereastra cu detalii folosind constructorul specializat
            StockDetailsWindow window = new StockDetailsWindow(newProduct);

            // se seteaza pozitia initiala pe ecran 
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se seteaza unele proprientati ale ferestrei 
            window.Title = "Add new ...";

            // se afiseaza fereastra ca un dialog modal
            // rezultatul dialogului poate fi 'true' sau 'false'
            // in fereastra, utilizatorul seteaza proprietatile pentru obiectul de tip Customer
            window.ShowDialog();

            // daca fereasta a fost inchisa cu OK, atunci conteaza modificarile facute
            if (window.DialogResult == true)
            {
                context.Products.Add(newProduct);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);

                // se reincarca datele
                LoadProductData();
            }
        }

        private void DeleteCurrent()
        {
            // se fac o serie de verificari asupra listei cu produsele
            if (ProductList == null)
            {
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            Product currentProduct = this.mySfDataGrid_Products.SelectedItem as Product;

            if (currentProduct != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.Products.Remove(currentProduct);

                    // se salveaza modificarile facute 
                    SaveData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadProductData();
                }
            }
        }

        private void buttonLoadproduscts_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            LoadProductData();
            Cursor = Cursors.Arrow;
        }

        private void buttonAddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddNewProduct();
        }

        private void buttonSaveProducts_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveData();
            Cursor = Cursors.Arrow;
        }

        private void buttonDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrent();
        }

        private void buttonShowProductDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowProductDetails();
        }

        private void buttonShowChartDetails_Click(object sender, RoutedEventArgs e)
        {
            StockGraphsWindow stockGraphsWindow = new StockGraphsWindow();
            stockGraphsWindow.Show();
        }
    }
}
