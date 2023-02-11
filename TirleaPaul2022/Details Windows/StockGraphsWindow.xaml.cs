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
using System.Windows.Shapes;

namespace TirleaPaul2022.Details_Windows
{
    /// <summary>
    /// Interaction logic for StockGraphsWindow.xaml
    /// </summary>
    public partial class StockGraphsWindow : Window
    {
        public SeriesCollection SeriesCollectionPie { get; set; }
        private TirleaPaul2022Context context = null;
        private List<View_Products_In_Stock> StockList = null;
        private List<Category> CategoryList = null;

        public StockGraphsWindow()
        {
            InitializeComponent();
            LoadStockData();
            ShowPieChart();
        }

        private void LoadStockData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.View_Products_In_Stock.Load();
                context.Categories.Load();

                //contextul ajunge la lista care contine stocul
                StockList = context.View_Products_In_Stock.ToList();
                mySfDataGrid_StockView.ItemsSource = StockList;

                CategoryList = context.Categories.ToList();
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

        private void ShowPieChart()
        {
            List<double> numbersOfProducts = new List<double>();
            foreach (Category category in CategoryList)
            {
                var stockByCategory = StockList.Where(x => x.CategoryID == category.CategoryID);

                var sumStockByCategory = stockByCategory.Sum(x => x.UnitsInStock);
                numbersOfProducts.Add((double)sumStockByCategory);
            }
            var zipList = CategoryList.Zip(numbersOfProducts, (c, n) => new { Cat = c, Num = n });
            SeriesCollectionPie = new SeriesCollection();
            foreach (var zip in zipList)
            {
                SeriesCollectionPie.Add(new PieSeries
                {
                    Title = zip.Cat.CategoryName.ToString(),
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)zip.Num) },
                    DataLabels = true
                });
            }
            this.DataContext = this;
        }   

        private void DragGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }
    }
}

