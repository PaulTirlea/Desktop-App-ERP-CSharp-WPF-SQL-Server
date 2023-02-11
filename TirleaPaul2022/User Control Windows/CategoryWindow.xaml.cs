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
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : UserControl
    {
        private TirleaPaul2022Context context = null;
        private List<Category> CategoryList = null;
        private CollectionViewSource categoryViewSource = null;

        public CategoryWindow()
        {
            InitializeComponent();
            categoryViewSource = (CollectionViewSource)this.FindResource("categoryViewSource");
            LoadCategoriesData();
        }

        private void LoadCategoriesData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Categories.Load();

                //contextul ajunge la lista care contine categoriile
                CategoryList = context.Categories.ToList();

                // se atribuie lista la sursa elementului de tip  sfDataGrid
                mySfDataGrid_Category.ItemsSource = CategoryList;

                categoryViewSource.Source = CategoryList;
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

        private void ShowCategoryDetails()
        {

            // se face o serie de verificari asupra vederii de tip CollectionViewSource
            if (categoryViewSource == null || categoryViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            Category currentCategory = this.mySfDataGrid_Category.SelectedItem as Category;
            // se determina obiectul Category corespunzator randului curent din DataGrid
            if (currentCategory != null)
            {
                // se deschide fereastra cu detalii folosind constructorul specializat
                CategoryDetailsWindow detailsWindow = new CategoryDetailsWindow(currentCategory);

                // se seteaza pozitia initiala pe ecran 
                detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // se afiseaza detaliile unui Category intr-o fereastra-copil, separata
                detailsWindow.ShowDialog();

                if (detailsWindow.DialogResult == true)
                {
                    // se salveaza modificarile facute
                    SaveData(showConfiguration: false, showSuccess: true);

                    // se reincarca datele
                    LoadCategoriesData();
                }
            }
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (categoryViewSource == null || categoryViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            if (showConfiguration)
            {
                // se afiseaza dialogul de configurare la care se raspunde "OK" sau "Cnacel"
                MessageBoxResult result =
                    MessageBox.Show("Doriti salvarea datelor actuale (inclusiv imaginile) ?",
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

        private void AddNewCategory()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (categoryViewSource == null || categoryViewSource.View == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }
            // se pregateste un nou element din clasa Category
            Category newCategory = new Category();
            // se deschide o nou fereastra cu detalii folosind constructorul specializat
            CategoryDetailsWindow window = new CategoryDetailsWindow(newCategory);

            // se seteaza pozitia initiala pe ecran 
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se seteaza unele proprientati ale ferestrei 
            window.Title = "Add new ...";

            // se afiseaza fereastra ca un dialog modal
            // rezultatul dialogului poate fi 'true' sau 'false'
            // in fereastra, utilizatorul seteaza proprietatile pentru obiectul de tip Category
            window.ShowDialog();

            // daca fereasta a fost inchisa cu OK, atunci conteaza modificarile facute
            if (window.DialogResult == true)
            {
                context.Categories.Add(newCategory);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);

                // se reincarca datele
                LoadCategoriesData();
            }
        }

        private void DeleteCurrentCategory()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (categoryViewSource == null || categoryViewSource.View == null)
            {
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            Category currentCategory = this.mySfDataGrid_Category.SelectedItem as Category;

            if (currentCategory != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.Categories.Remove(currentCategory);

                    // se salveaza modificarile facute 
                    SaveData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadCategoriesData();
                }
            }
        }

        private void buttonLoadCategories_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            LoadCategoriesData();
            Cursor = Cursors.Arrow;
        }

        private void buttonAddNewCategory_Click(object sender, RoutedEventArgs e)
        {
            AddNewCategory();
        }

        private void buttonSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveData();
            Cursor = Cursors.Arrow;
        }

        private void buttonDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrentCategory();
        }

        private void buttonShowCategoryDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowCategoryDetails();
        }
    }
}
