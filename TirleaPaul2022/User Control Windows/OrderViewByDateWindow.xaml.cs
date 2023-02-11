using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace TirleaPaul2022.User_Control_Windows
{
    /// <summary>
    /// Interaction logic for OrderViewByDateWindow.xaml
    /// </summary>
    public partial class OrderViewByDateWindow : UserControl
    {
        #region ================================== VARIABILE ==================================
        public SeriesCollection SeriesCollectionRow { get; set; }
        public SeriesCollection SeriesCollectionPie { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private TirleaPaul2022Context context = null;
        private List<View_Orders_20> ViewOrders2020List = null;
        private List<View_Orders_21> ViewOrders2021List = null;
        private List<View_Orders_22> ViewOrders2022List = null;
        private OrdersByYear ordersByYear;
        #endregion

        #region ================================== CONSTRUCTOR ==================================
        public OrderViewByDateWindow()
        {
            InitializeComponent();
            LoadData();
            ShowRowChart();
        }
        #endregion

        #region ================================== METODE ==================================
        private void LoadData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.View_Orders_20.Load();
                context.View_Orders_21.Load();
                context.View_Orders_22.Load();

                //contextul ajunge la lista 
                ViewOrders2020List = context.View_Orders_20.ToList();
                ViewOrders2021List = context.View_Orders_21.ToList();
                ViewOrders2022List = context.View_Orders_22.ToList();

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
        private void PrepareChartsData(string title1, string[] labels, int[] values1)
        {
            SeriesCollectionRow = new SeriesCollection();

            SeriesCollectionRow.Add(new RowSeries
            {
                Title = title1,
                Values = new ChartValues<int>(values1)
            });
            Labels = labels;
            Formatter = value1 => value1.ToString("N");

        }


        private void ShowRowChart()
        {
            LoadData();

            string title1 = "NUMBER OF ORDERS BY YEAR";

            // se pregatesc datele pentru grafic

            int orders2020 = ViewOrders2020List.Count();
            int orders2021 = ViewOrders2021List.Count();
            int orders2022 = ViewOrders2022List.Count();

            double valueOrders2020 = 0;
            foreach(View_Orders_20 view_Orders_2020 in ViewOrders2020List)
            {
                valueOrders2020 += (double)view_Orders_2020.Value;
            }

            double valueOrders2021 = 0;
            foreach (View_Orders_21 view_Orders_2021 in ViewOrders2021List)
            {
                valueOrders2021 += (double)view_Orders_2021.Value;
            }

            double valueOrders2022 = 0;
            foreach (View_Orders_22 view_Orders_2022 in ViewOrders2022List)
            {
                valueOrders2022 += (double)view_Orders_2022.Value;
            }

            List<OrdersByYear> OrdersByYearList = new List<OrdersByYear>();
           

            ordersByYear = new OrdersByYear
            {
                OrdersCount = orders2020,
                Year = "2020",
                OrderValue = valueOrders2020
            };
            OrdersByYearList.Add(ordersByYear);

            ordersByYear = new OrdersByYear
            {
                OrdersCount = orders2021,
                Year = "2021",
                OrderValue = valueOrders2021
            };
            OrdersByYearList.Add(ordersByYear);

            ordersByYear = new OrdersByYear
            {
                OrdersCount = orders2022,
                Year = "2022",
                OrderValue = valueOrders2022
            };
            OrdersByYearList.Add(ordersByYear);


            int yearsCount = OrdersByYearList.Count;
            string[] labels = new string[yearsCount];
            int[] values1 = new int[yearsCount];
            decimal[] values2 = new decimal[yearsCount];

            int index = -1;
            foreach (OrdersByYear oby in OrdersByYearList)
            {
                index++;
                labels[index] = $"Anul {oby.Year}";
                values1[index] = (int)oby.OrdersCount;
            }

            PrepareChartsData(title1, labels, values1);
            SeriesCollectionPie = new SeriesCollection();

            SeriesCollectionPie.Add(new PieSeries
            {
                Title = "2020",
                Values = new ChartValues<ObservableValue> { new ObservableValue(valueOrders2020) },
                DataLabels = true
            });
            SeriesCollectionPie.Add(new PieSeries
            {
                Title = "2021",
                Values = new ChartValues<ObservableValue> { new ObservableValue(valueOrders2021) },
                DataLabels = true
            });
            SeriesCollectionPie.Add(new PieSeries
            {
                Title = "2022",
                Values = new ChartValues<ObservableValue> { new ObservableValue(valueOrders2022) },
                DataLabels = true
            });

            DataContext = this;
        }
        #endregion

        #region ================================== EVENIMENTE ==================================
        private void button2020_Click(object sender, RoutedEventArgs e)
        {
            mySfDataGrid_OrderView.ItemsSource = ViewOrders2020List;
            button2020.Focus();
        }

        private void button2021_Click(object sender, RoutedEventArgs e)
        {
            mySfDataGrid_OrderView.ItemsSource = ViewOrders2021List;
            button2021.Focus();
        }

        private void button2022_Click(object sender, RoutedEventArgs e)
        {
            mySfDataGrid_OrderView.ItemsSource = ViewOrders2022List;
            button2022.Focus();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            mySfDataGrid_OrderView.ItemsSource = ViewOrders2020List;
            button2020.Focus();
        }
        #endregion
    }
}
