using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TirleaPaul2022
{
    /// <summary>
    /// Interaction logic for SuppliersWindow.xaml
    /// </summary>
    public partial class SuppliersWindow : UserControl
    {
        #region ================================== VARIABILE ==================================
        private TirleaPaul2022Context context = null;
        private List<Supplier> SupplierList = null;
        private CollectionViewSource supplierViewSource = null;
        #endregion

        #region ================================== CONSTRUCTOR ==================================
        public SuppliersWindow()
        {
            InitializeComponent();
            supplierViewSource = (CollectionViewSource)this.FindResource("supplierViewSource");
            LoadSupplierData();
        }
        #endregion

        #region ================================== METODE ==================================
        private void LoadSupplierData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Suppliers.Load();

                //contextul ajunge la lista care contine furnizorii
                SupplierList = context.Suppliers.ToList();

                // se atribuie lista la sursa elementului de tip  sfDataGrid
                mySfDataGrid_Supplier.ItemsSource = SupplierList;

                supplierViewSource.Source = SupplierList;
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

        private void ShowSupplierDetails()
        {

            // se face o serie de verificari asupra vederii de tip CollectionViewSource
            if (supplierViewSource == null || supplierViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.",
                                "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            Supplier currentSupplier = this.mySfDataGrid_Supplier.SelectedItem as Supplier;
            // se determina obiectul Supplier corespunzator randului curent din DataGrid
            if (currentSupplier != null)
            {
                // se deschide fereastra cu detalii folosind constructorul specializat
                SupplierDetailsWindow detailsWindow = new SupplierDetailsWindow(currentSupplier);

                // se seteaza pozitia initiala pe ecran 
                detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // se afiseaza detaliile unui Supplier intr-o fereastra-copil, separata
                detailsWindow.ShowDialog();
                if (detailsWindow.DialogResult == true)
                {
                    // se salveaza modificarile facute
                    SaveData(showConfiguration: false, showSuccess: true);

                    // se reincarca datele
                    LoadSupplierData();
                }
            }
            
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (supplierViewSource == null || supplierViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            if (showConfiguration)
            {
                // se afiseaza dialogul de configurare la care se raspunde "OK" sau "Cnacel"
                MessageBoxResult result =
                    MessageBox.Show("Doriti salvarea datelor actuale ?",
                                       "Save data", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                // daca utilizatorul a apasat Cancel, atunci operatia de salvare este abandoanata
                if (result == MessageBoxResult.Cancel) return;
            }
            try
            {
                context.SaveChanges();

                if (showSuccess)
                {
                    MessageBox.Show("Datele au fost salvate cu succes.", "Save data", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
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
                        message += string.Format("Proprietatea: {0} \n Mesaj de eroare: {1} \n\n", error.PropertyName, 
                            error.ErrorMessage);

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

        private void AddNew()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (supplierViewSource == null || supplierViewSource.View == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", 
                                "Add New", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se pregateste un nou obiect din clasa Supplier
            Supplier newSupplier = new Supplier();

            // se deschide o nouă fereastra cu detalii folosind constructorul specializat
            SupplierDetailsWindow window = new SupplierDetailsWindow(newSupplier);

            // se seteaza pozitia initiala pe ecran 
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se seteaza unele proprientati ale ferestrei 
            window.Title = "Add new ...";

            // se afiseaza fereastra ca un dialog modal
            // rezultatul dialogului poate fi 'true' sau 'false'
            // in fereastra, utilizatorul seteaza proprietatile pentru obiectul de tip Supplier
            window.ShowDialog();

            // daca fereasta a fost inchisa cu OK, atunci conteaza modificarile facute
            if (window.DialogResult == true)
            {
                context.Suppliers.Add(newSupplier);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);

                // se reincarca datele
                LoadSupplierData();
            }
        }

        private void DeleteCurrent()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (supplierViewSource == null || supplierViewSource.View == null)
            {
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            Supplier currentSupplier = this.mySfDataGrid_Supplier.SelectedItem as Supplier;

            if (currentSupplier != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.Suppliers.Remove(currentSupplier);

                    // se salveaza modificarile facute 
                    SaveData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadSupplierData();
                }
            }
        }
        #endregion

        #region ================================== EVENIMENTE ==================================
        private void buttonLoadSuppliers_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            LoadSupplierData();
            Cursor = Cursors.Arrow;

        }


        private void buttonAddNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void buttonDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrent();
        }

        private void buttonSaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveData();
            Cursor = Cursors.Arrow;
        }

        private void buttonShowSupplierDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowSupplierDetails();
        }
        #endregion
    }
}
