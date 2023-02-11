using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TirleaPaul2022.User_Control_Windows
{
    /// <summary>
    /// Interaction logic for OrderViewByCategory.xaml
    /// </summary>
    public partial class OrderViewByCategory : UserControl
    {
        public SeriesCollection SeriesCollectionPie { get; set; }
        private TirleaPaul2022Context context = null;
        private List<string> CategoryNameList = null;
        private List<View_Orders_By_Category> ViewOrdersCategories = null;
        private List<Category> CategoryList = null;

        public OrderViewByCategory()
        {
            InitializeComponent();
            LoadCategoriesData();
        }

        private void LoadCategoriesData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.Categories.Load();
                context.View_Orders_By_Category.Load();

                //contextul ajunge la lista care contine categoriile
                var quary = from category in context.Categories.Local
                            select category.CategoryName;
                CategoryNameList = quary.ToList();
                comboBoxCategory.ItemsSource = CategoryNameList;

                ViewOrdersCategories = context.View_Orders_By_Category.ToList();
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

        private void ShowDate()
        {
            var categoryChosen = comboBoxCategory.SelectedItem;
            List<View_Orders_By_Category> newView = new List<View_Orders_By_Category>();
            foreach (View_Orders_By_Category view in ViewOrdersCategories)
            {
                if (view.CategoryName == categoryChosen.ToString())
                {
                    newView.Add(view);
                }
            }
            mySfDataGrid_OrderView.ItemsSource = newView;

            SeriesCollectionPie = new SeriesCollection();
            foreach (View_Orders_By_Category newV in newView)
            {
                SeriesCollectionPie.Add(new PieSeries
                {
                    Title = newV.OrderDate.ToString(),
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)newV.Value) },
                    DataLabels = true
                });
            }
            this.DataContext = this;

            //reîncarcăm datele pentru că în controlul comboBoxAdv dispar opțiunile după selecție
            LoadCategoriesData();
        }

        private void loadTabelData_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            ShowDate();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxCategory.SelectedIndex = 1;
            ShowDate();
        }
    }
}
