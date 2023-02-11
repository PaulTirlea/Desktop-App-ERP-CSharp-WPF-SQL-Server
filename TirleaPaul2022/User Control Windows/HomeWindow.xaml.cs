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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using TirleaPaul2022.Details_Windows;

namespace TirleaPaul2022.User_Control_Windows
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private TirleaPaul2022Context context = null;
        private List<OrderDetail> OrderDetailsList = null;
        private List<Order> OrderList = null;
        public bool ConnectionStatus = false;

        public HomeWindow()
        {
            InitializeComponent();
        }

        public HomeWindow(bool LogInStatus = true)
        {
            InitializeComponent();
            if (LogInStatus)
            {
                IsLogged();
            }
        }

        private void LoadData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Products.Load();
                context.Employees.Load();
                context.OrderDetails.Load();
                context.Orders.Load();


                int products = context.Products.Count();
                textBlockProducts.Text = products.ToString();

                int employees = context.Employees.Count();
                textBlockEmployees.Text = employees.ToString();

                OrderDetailsList = context.OrderDetails.ToList();
                decimal sum = 0;
                foreach (OrderDetail od in OrderDetailsList)
                {
                    sum += od.UnitPrice * (decimal)od.Quantity;
                }
                textBlockValue.Text = sum.ToString();

                OrderList = context.Orders.ToList();
                int orders = 0;
                foreach (Order o in OrderList)
                {
                    if (o.ShippedDate == null)
                    {
                        orders++;
                    }
                }
                textBlockOrders.Text = orders.ToString();

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

        private void LoadChart()
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Cifra de afaceri",
                    Values = new ChartValues<double> { 455014, 513645, 626688, 696058, 707815, 695697, 1019868, 950424, 996738, 1299034, 1631494 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Profit Net",
                Values = new ChartValues<double> { 1607, -20325, 2962, 1220, 16909, 18701, 4577, 13778, 6165, 19248, 7352 }
            });

            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Datorii",
                Values = new ChartValues<double> { 282792, 259131, 266947, 192690, 228752, 245185, 217200, 247402, 251307, 288753, 502760 }
            });

            //also adding values updates and animates the chart automatically
            SeriesCollection[1].Values.Add(48d);

            Labels = new[] { "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020" };
            Formatter = value => value.ToString("N");

            DataContext = this;
        }

        private void IsLogged()
        {
            connectionStatustextBlox.Text = "Status: Connected";
            disconnectedIcon.Visibility = Visibility.Hidden;
            connectedIcon.Visibility = Visibility.Visible;
            buttonShowLoginWindow.Content = "LogOut";
            buttonShowLoginWindow.ToolTip = "Logging Out";
            ConnectionStatus = true;
        }

        private void LogInVerify()
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.ShowDialog();
            if (logInWindow.DialogResult == true)
            {

                IsLogged();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).ButtonOpenMenu.Visibility = Visibility.Visible;
                        (window as MainWindow).buttonMaximize.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void buttonShowLoginWindow_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionStatus == false)
            {
                LogInVerify();
            }
            else if(ConnectionStatus)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            
        }

        private void buttonQRCode_Click(object sender, RoutedEventArgs e)
        {
            TirleaPaul2022_QRCode.MainWindow mainWindow = new TirleaPaul2022_QRCode.MainWindow();
            mainWindow.Show();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadChart();
            
        }
    }
}
