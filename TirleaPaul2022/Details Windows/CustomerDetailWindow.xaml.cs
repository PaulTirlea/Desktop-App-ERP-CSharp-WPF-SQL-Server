using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for CustomerDetailWindow.xaml
    /// </summary>
    public partial class CustomerDetailWindow : Window
    {
        private TirleaPaul2022Context context = null;
        private List<Customer> CustomerList = null;
        private CollectionViewSource customerViewSource = null;
        private CollectionViewSource productViewSource = null;
        private List<OrderDetail> OrderDetailsList = new List<OrderDetail>();
        private List<Product> ProductList = new List<Product>();

        private string currentCustomerID = null;
        private string firstCustomerID = null;
        private string lastCustomerID = null;
        private int indexOfCurrentCustomer = 0;
        private Customer firstCustomer;
        private Customer lastCustomer;

        private Customer customer;
        private Customer Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                this.DataContext = customer;
            }
        }

        public CustomerDetailWindow()
        {
            InitializeComponent();
        }

        public CustomerDetailWindow(Customer customer)
        {
            InitializeComponent();
            customerViewSource = (CollectionViewSource)this.FindResource("customerViewSource");
            productViewSource = (CollectionViewSource)this.FindResource("productViewSource");
            this.Customer = customer;
            currentCustomerID = customer.CustomerID;
            if (String.IsNullOrEmpty(currentCustomerID))
            {
                stackPanelMovement.Visibility = Visibility.Collapsed;
            }
            else
            {
                stackPanelMovement.Visibility = Visibility.Visible;
                LoadCustomers();

                for (int i = 0; i < CustomerList.Count(); i++)
                {
                    if (CustomerList[i].CustomerID == currentCustomerID)
                    {
                        indexOfCurrentCustomer = i;
                        break;
                    }
                }
            }
        }

        private void LoadCustomers()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.Customers.Load();
            context.Products.Load();
            context.Orders.Load();
            context.OrderDetails.Load();
            // se pregateste interogarea (se poate introduce filtreare si ordonare)

            var quaryCustomers = from c in context.Customers
                                 select c;

            //contextul ajunge la lista care contine clientii
            this.CustomerList = quaryCustomers.ToList();

            firstCustomer = CustomerList[0];
            firstCustomerID = firstCustomer.CustomerID;

            lastCustomer = CustomerList[CustomerList.Count() - 1];
            lastCustomerID = lastCustomer.CustomerID;

            this.customerViewSource.Source = this.CustomerList;

            var quaryOrders = from o in context.Orders
                              where o.CustomerID == customer.CustomerID
                              select o;
            List<Order> OrderList = quaryOrders.ToList();
            foreach(Order order in OrderList)
            {
                var quaryOrderDetails = from od in context.OrderDetails
                                        where od.OrderID == order.OrderID
                                        select od;
                if (quaryOrderDetails != null)
                {

                    List<OrderDetail> newOrderDetailsList = quaryOrderDetails.ToList();
                    OrderDetailsList.AddRange(newOrderDetailsList);
                    foreach(OrderDetail newOrderDetail in newOrderDetailsList)
                    {
                        OrderDetailsList.Add(newOrderDetail);
                    }

                    foreach (OrderDetail orderDetail in OrderDetailsList)
                    {
                        var quaryProduct = from p in context.Products
                                           where p.ProductID == orderDetail.ProductID
                                           select p;
                        if (quaryProduct != null)
                        {

                            List<Product> newProductList = quaryProduct.ToList();
                            foreach(Product newProduct in newProductList)
                            {
                                ProductList.Add(newProduct);
                            }
                            this.mySfDataGrid_Products.ItemsSource = ProductList.Distinct();
                            this.productViewSource.Source = this.ProductList;
                        }
                    }
                }
            }
            
        }

        private void MoveToFirst()
        {
            if (currentCustomerID != firstCustomerID)
            {
                this.Close();
                CustomerDetailWindow customerDetailsWindow = new CustomerDetailWindow(firstCustomer);
                customerDetailsWindow.ShowDialog();
            }
        }

        private void MoveToPrevious()
        {
            if (currentCustomerID != firstCustomerID)
            {
                Customer previousCustomer = CustomerList[indexOfCurrentCustomer - 1];
                this.Close();
                CustomerDetailWindow customerDetailsWindow = new CustomerDetailWindow(previousCustomer);
                customerDetailsWindow.ShowDialog();

            }
        }

        private void MoveToNext()
        {
            if (lastCustomerID != currentCustomerID)
            {
                Customer nextCustomer = CustomerList[indexOfCurrentCustomer + 1];
                this.Close();
                CustomerDetailWindow customerDetailsWindow = new CustomerDetailWindow(nextCustomer);
                customerDetailsWindow.ShowDialog();
            }

        }

        private void MoveToLast()
        {
            if (lastCustomerID != currentCustomerID)
            {
                this.Close();
                CustomerDetailWindow customerDetailsWindow = new CustomerDetailWindow(lastCustomer);
                customerDetailsWindow.ShowDialog();
            }
        }
        private void OkAndCloseWindow()
        {
            if (String.IsNullOrEmpty(CustomerIDTextBox.Text) || String.IsNullOrEmpty(CompanyNameTextBox.Text))
            {
                MessageBox.Show("Nu ați introdus corespunzător datele. \n" + "Operatia nu poate continua.\n" + "ID - ul nu poate fi null!" +
                    "Numele companiei nu poate fi null!", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }
            else
            {
                DialogResult = true;
                Close();
            }

        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOK_CloseCustomerDetails_Click(object sender, RoutedEventArgs e)
        {
            OkAndCloseWindow();
        }

        private void buttonFirstCustomer_Click(object sender, RoutedEventArgs e)
        {
            MoveToFirst();
        }

        private void buttonPreviosCustomer_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void buttonNextCustomer_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void buttonLastCustomer_Click(object sender, RoutedEventArgs e)
        {
            MoveToLast();
        }

        private void gridTop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
