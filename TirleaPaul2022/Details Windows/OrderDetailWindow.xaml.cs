using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
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
using System.Windows.Shapes;

namespace TirleaPaul2022.Details_Windows
{
    /// <summary>
    /// Interaction logic for OrderDetailWindow.xaml
    /// </summary>
    public partial class OrderDetailWindow : Window
    {
        private TirleaPaul2022Context context = null;
        private List<Order> OrderList = null;
        private CollectionViewSource orderViewSource = null;
        private List<OrderDetail> OrderDetailsList = null;
        private CollectionViewSource orderDetailsViewSource = null;

        private int currentOrderID = 0;
        private int firstOrderID = 0;
        private int lastOrderID = 0;
        private int indexOfCurrentOrder = 0;
        private Order firstOrder;
        private Order lastOrder;

        private Order order;
        private Order Order
        {
            get { return order; }
            set
            {
                order = value;
                this.DataContext = order;
            }
        }

        public OrderDetailWindow()
        {
            InitializeComponent();
        }

        public OrderDetailWindow(Order order)
        {
            InitializeComponent();
            orderDetailsViewSource = (CollectionViewSource)this.FindResource("orderDetailsViewSource");
            orderViewSource = (CollectionViewSource)this.FindResource("orderViewSource");
            this.Order = order;
            currentOrderID = order.OrderID;
            if (currentOrderID != 0)
            {
                stackPanelMovement.Visibility = Visibility.Visible;
                LoadOrders();

                for (int i = 0; i < OrderList.Count(); i++)
                {
                    if (OrderList[i].OrderID == currentOrderID)
                    {
                        indexOfCurrentOrder = i;
                        break;
                    }
                }
            }
            else
            {
                stackPanelMovement.Visibility = Visibility.Collapsed;
                OrderIDTextBox.IsReadOnly = true;
            }
        }

        private void LoadOrders()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.Orders.Load();
            context.OrderDetails.Load();

            // se pregateste interogarea (se poate introduce filtreare si ordonare)

            var quaryOrders = from o in context.Orders
                        select o;

            //contextul ajunge la lista care contine furnizorii
            this.OrderList = quaryOrders.ToList();

            firstOrder = OrderList[0];
            firstOrderID = firstOrder.OrderID;

            lastOrder = OrderList[OrderList.Count() - 1];
            lastOrderID = lastOrder.OrderID;

            this.orderViewSource.Source = this.OrderList;

            var quaryOrderDetails = from od in context.OrderDetails
                                    where od.OrderID == currentOrderID
                                    select od;
            this.OrderDetailsList = quaryOrderDetails.ToList();
            mySfDataGrid_OrderDetails.ItemsSource = OrderDetailsList;
            this.orderDetailsViewSource.Source = this.OrderDetailsList;

        }

        private void LoadOrderDetails()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.OrderDetails.Load();

            var quaryOrderDetails = from od in context.OrderDetails
                                    where od.OrderID == currentOrderID
                                    select od;
            this.OrderDetailsList = quaryOrderDetails.ToList();
            mySfDataGrid_OrderDetails.ItemsSource = OrderDetailsList;
        }

        private void SaveOrderDetailsData(bool showConfiguration = true, bool showSuccess = true)
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

        private void AddNewOrderDetail()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Add new", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            
            OrderDetail currentOrderDetail = this.mySfDataGrid_OrderDetails.SelectedItem as OrderDetail;
            context.OrderDetails.Add(currentOrderDetail);


            // se salveaza modificarile facute
            SaveOrderDetailsData(showConfiguration: true, showSuccess: true);

            // se reincarca datele
            LoadOrderDetails();

        }

        private void DeleteCurrent()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (orderViewSource == null || orderViewSource.View == null || orderDetailsViewSource == null || orderDetailsViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Delete current", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se determina obiectul corespunzator randului ales pentru Delete
            OrderDetail currentOrderDetail = this.mySfDataGrid_OrderDetails.SelectedItem as OrderDetail;

            if (currentOrderDetail != null)
            {
                // se cere confirmare la stergere
                string question = "Stergeti randul curent ?";
                string caption = " Confirmare";
                MessageBoxResult result = MessageBox.Show(question, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    context.OrderDetails.Remove(currentOrderDetail);

                    // se salveaza modificarile facute 
                    SaveOrderDetailsData(showConfiguration: false, showSuccess: false);

                    // se reincarca datele
                    LoadOrderDetails();
                }
            }
        }

        private void MoveToFirst()
        {
            if (currentOrderID != firstOrderID)
            {
                this.Close();
                OrderDetailWindow orderDetailsWindow = new OrderDetailWindow(firstOrder);
                orderDetailsWindow.ShowDialog();
            }
        }

        private void MoveToPrevious()
        {
            if (currentOrderID != firstOrderID)
            {
                Order previousOrder = OrderList[indexOfCurrentOrder - 1];
                this.Close();
                OrderDetailWindow orderDetailsWindow = new OrderDetailWindow(previousOrder);
                orderDetailsWindow.ShowDialog();

            }
        }

        private void MoveToNext()
        {
            if (lastOrderID != currentOrderID)
            {
                Order nextOrder = OrderList[indexOfCurrentOrder + 1];
                this.Close();
                OrderDetailWindow orderDetailsWindow = new OrderDetailWindow(nextOrder);
                orderDetailsWindow.ShowDialog();
            }

        }

        private void MoveToLast()
        {
            if (lastOrderID != currentOrderID)
            {
                this.Close();
                OrderDetailWindow orderDetailsWindow = new OrderDetailWindow(lastOrder);
                orderDetailsWindow.ShowDialog();
            }
        }
        private void OKAndCloseWindow()
        {
            DialogResult = true;
            Close();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonFirstOrder_Click(object sender, RoutedEventArgs e)
        {
            MoveToFirst();
        }

        private void buttonPreviosOrder_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void buttonNextOrder_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void buttonLastOrder_Click(object sender, RoutedEventArgs e)
        {
            MoveToLast();
        }

        private void buttonOK_CloseOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            OKAndCloseWindow();
        }


        private void buttonDeleteOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrent();
        }

        private void buttonAddNewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            AddNewOrderDetail();
        }

        private void gridTop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
