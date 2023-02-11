using TirleaPaul2022.Details_Windows;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : UserControl
    {

        private TirleaPaul2022Context context = null;
        private List<Order> OrderList = null;
        private CollectionViewSource orderViewSource = null;
        private List<OrderDetail> OrderDetailsList = null;
        private CollectionViewSource orderDetailsViewSource = null;

        public OrderWindow()
        {
            InitializeComponent();
            orderDetailsViewSource = (CollectionViewSource)this.FindResource("orderDetailsViewSource");
            orderViewSource = (CollectionViewSource)this.FindResource("orderViewSource");
            LoadOrderData();
        }

        private void LoadOrderData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Orders.Load();
                context.OrderDetails.Load();

                //contextul ajunge la lista care contine comenzile
                OrderList = context.Orders.ToList();
                OrderDetailsList = context.OrderDetails.ToList();
                //DataSet dataSet = new DataSet();

                // se atribuie lista la sursa elementului de tip  sfDataGrid
                mySfDataGrid_Order.ItemsSource = OrderList;
                mySfDataGrid_OrderDetails.ItemsSource = OrderDetailsList;

                orderViewSource.Source = OrderList;
                this.DataContext = orderViewSource;
                orderDetailsViewSource.Source = OrderDetailsList;
                this.DataContext = orderDetailsViewSource;
                this.mySfDataGrid_Order.DetailsViewExpanding += mySfDataGrid_Order_DetailsViewExpanding;
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

        private void ShowOrderDetails()
        {

            // se face o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            Order currentOrder = this.mySfDataGrid_Order.SelectedItem as Order;
            // se determina obiectul Supplier corespunzator randului curent din DataGrid
            if (currentOrder != null)
            {
                // se deschide fereastra cu detalii folosind constructorul specializat
                OrderDetailWindow detailsWindow = new OrderDetailWindow(currentOrder);

                // se seteaza pozitia initiala pe ecran 
                detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // se afiseaza detaliile unui Supplier intr-o fereastra-copil, separata
                detailsWindow.ShowDialog();

                if (detailsWindow.DialogResult == true)
                {
                    // se salveaza modificarile facute
                    SaveData(showConfiguration: false, showSuccess: true);

                    // se reincarca datele
                    LoadOrderData ();
                }
            }
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Save Data", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void AddNewOrder()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se pregateste un nou obiect din clasa Order
            Order newOrder = new Order();

            // se deschide o nou fereastra cu detalii folosind constructorul specializat
            OrderDetailWindow window = new OrderDetailWindow(newOrder);

            // se seteaza pozitia initiala pe ecran 
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se seteaza unele proprientati ale ferestrei 
            window.Title = "Add new ...";

            // se afiseaza fereastra ca un dialog modal
            // rezultatul dialogului poate fi 'true' sau 'false'
            // in fereastra, utilizatorul seteaza proprietatile pentru obiectul de tip Order
            window.ShowDialog();

            // daca fereasta a fost inchisa cu OK, atunci conteaza modificarile facute
            if (window.DialogResult == true)
            {
                context.Orders.Add(newOrder);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);

                // se reincarca datele
                LoadOrderData();
            }
        }

        private void DeleteCurrent()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            Order CurrentOrder = this.mySfDataGrid_Order.SelectedItem as Order;

            if (CurrentOrder != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.Orders.Remove(CurrentOrder);

                    // se salveaza modificarile facute 
                    SaveData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadOrderData();
                }
            }
        }

        private void mySfDataGrid_Order_DetailsViewExpanding(object sender, Syncfusion.UI.Xaml.Grid.GridDetailsViewExpandingEventArgs e)
        {
            var orderDetails = e.Record as Order;
            var orderViewModel = context.OrderDetails.Where(order => order.OrderID == orderDetails.OrderID);
            e.DetailsViewItemsSource["odersDetailsRelation"] = orderViewModel;
        }

        private void buttonShowOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowOrderDetails();
        }

        private void buttonLoadOrders_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            LoadOrderData();
            Cursor = Cursors.Arrow;

        }

        private void buttonAddNewOrder_Click(object sender, RoutedEventArgs e)
        {
            AddNewOrder();
        }

        private void buttonSaveOrders_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveData();
            Cursor = Cursors.Arrow;
        }

        private void buttonDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrent();
        }

        private void buttonShowOrderGraphs_Click(object sender, RoutedEventArgs e)
        {
            OrderGraphsWindow detailsWindow = new OrderGraphsWindow();

            // se seteaza pozitia initiala pe ecran 
            detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se afiseaza detaliile unui Supplier intr-o fereastra-copil, separata
            detailsWindow.ShowDialog();
        }
    }
}
