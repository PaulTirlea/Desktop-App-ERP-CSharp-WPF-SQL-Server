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

namespace TirleaPaul2022
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : UserControl
    {
        private TirleaPaul2022Context context = null;
        private List<Customer> CustomerList = null;
        private CollectionViewSource customerViewSource = null;

        public CustomerWindow()
        {
            InitializeComponent();
            customerViewSource = (CollectionViewSource)this.FindResource("customerViewSource");
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Customers.Load();

                //contextul ajunge la lista care contine furnizorii
                CustomerList = context.Customers.ToList();

                // se atribuie lista la sursa elementului de tip  sfDataGrid
                mySfDataGrid_Customer.ItemsSource = CustomerList;

                customerViewSource.Source = CustomerList;
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

        private void ShowCustomerDetails()
        {

            // se face o serie de verificari asupra vederii de tip CollectionViewSource
            if (customerViewSource == null || customerViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            Customer currentCustomer = this.mySfDataGrid_Customer.SelectedItem as Customer;
            // se determina obiectul Customer corespunzator randului curent din DataGrid
            if (currentCustomer != null)
            {
                // se deschide fereastra cu detalii folosind constructorul specializat
                CustomerDetailWindow detailsWindow = new CustomerDetailWindow(currentCustomer);

                // se seteaza pozitia initiala pe ecran 
                detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // se afiseaza detaliile unui Customer intr-o fereastra-copil, separata
                detailsWindow.ShowDialog();

                if (detailsWindow.DialogResult == true)
                {
                    // se salveaza modificarile facute
                    SaveData(showConfiguration: false, showSuccess: true);

                    // se reincarca datele
                    LoadCustomerData();
                }
            }
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (customerViewSource == null || customerViewSource.View == null)
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

        private void AddNewCustomer()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (customerViewSource == null || customerViewSource.View == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se pregateste un nou obiect din clasa Customer
            Customer newCustomer = new Customer();

            // se deschide o nou fereastra cu detalii folosind constructorul specializat
            CustomerDetailWindow window = new CustomerDetailWindow(newCustomer);

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
                context.Customers.Add(newCustomer);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);

                // se reincarca datele
                LoadCustomerData();
            }
        }

        private void DeleteCurrent()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (customerViewSource == null || customerViewSource.View == null)
            {
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            Customer currentCustomer = this.mySfDataGrid_Customer.SelectedItem as Customer;

            if (currentCustomer != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.Customers.Remove(currentCustomer);

                    // se salveaza modificarile facute 
                    SaveData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadCustomerData();
                }
            }
        }

        private void buttonLoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            LoadCustomerData();
            Cursor = Cursors.Arrow;

        }

        private void buttonAddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddNewCustomer();
        }

        private void buttonSaveCustomers_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveData();
            Cursor = Cursors.Arrow;
        }

        private void buttonDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrent();
        }

        private void buttonShowCustomerDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomerDetails();
        }

    }
}
