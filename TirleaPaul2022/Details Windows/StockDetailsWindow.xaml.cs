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
    /// Interaction logic for StockDetailsWindow.xaml
    /// </summary>
    public partial class StockDetailsWindow : Window
    {
        private TirleaPaul2022Context context = null;
        private List<Product> ProductList = null;

        private int currentProductID = 0;
        private int firstProductID = 0;
        private int lastProductID = 0;
        private int indexOfCurrentProduct = 0;
        private Product firstProduct;
        private Product lastProduct;

        private Product product;
        private Product Product
        {
            get { return product; }
            set
            {
                product = value;
                this.DataContext = product;
            }
        }
        public StockDetailsWindow()
        {
            InitializeComponent();
        }

        public StockDetailsWindow(Product product)
        {
            InitializeComponent();
            this.Product = product;

            currentProductID = product.ProductID;
            if (currentProductID != 0)
            {
                stackPanelMovement.Visibility = Visibility.Visible;
                LoadProducts();

                for (int i = 0; i < ProductList.Count(); i++)
                {
                    if (ProductList[i].ProductID == currentProductID)
                    {
                        indexOfCurrentProduct = i;
                        break;
                    }
                }
            }
            else
            {
                stackPanelMovement.Visibility = Visibility.Collapsed;
                ProductIDTextBox.IsReadOnly = true;
            }
        }
        private void LoadProducts()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.Products.Load();

            // se pregateste interogarea (se poate introduce filtreare si ordonare)

            var quary = from p in context.Products
                        select p;

            //contextul ajunge la lista care contine furnizorii
            this.ProductList = quary.ToList();

            firstProduct = ProductList[0];
            firstProductID = firstProduct.ProductID;

            lastProduct = ProductList[ProductList.Count() - 1];
            lastProductID = lastProduct.ProductID;
        }

        private void MoveToFirst()
        {
            if (currentProductID != firstProductID)
            {
                this.Close();
                StockDetailsWindow DetailsWindow = new StockDetailsWindow(firstProduct);
                DetailsWindow.ShowDialog();
                //this.DataContext = firstSupplier;
                //indexOfCurrentSuppiler = 0;
            }
        }

        private void MoveToPrevious()
        {
            if (currentProductID != firstProductID)
            {
                Product previousProduct = ProductList[indexOfCurrentProduct - 1];

                //if (indexOfCurrentSuppiler != 1)
                //{
                //    indexOfCurrentSuppiler -= 1;
                //}
                //this.DataContext = previousSupplier;

                this.Close();
                StockDetailsWindow DetailsWindow = new StockDetailsWindow(previousProduct);
                DetailsWindow.ShowDialog();


            }
        }

        private void MoveToNext()
        {
            if (lastProductID != currentProductID)
            {
                Product nextProduct = ProductList[indexOfCurrentProduct + 1];
                //if (indexOfCurrentSuppiler != SupplierList.Count() - 2 )
                //{
                //    indexOfCurrentSuppiler += 1;
                //}
                //this.DataContext = nextSupplier;
                this.Close();
                StockDetailsWindow DetailsWindow = new StockDetailsWindow(nextProduct);
                DetailsWindow.ShowDialog();
            }

        }

        private void MoveToLast()
        {
            if (lastProductID != currentProductID)
            {
                this.Close();
                StockDetailsWindow DetailsWindow = new StockDetailsWindow(lastProduct);
                DetailsWindow.ShowDialog();
                //this.DataContext = lastSupplier;
                //indexOfCurrentSuppiler = SupplierList.Count() - 1;
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOK_CloseProductDetails_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonFirstProduct_Click(object sender, RoutedEventArgs e)
        {
            MoveToFirst();
        }

        private void buttonPreviosProduct_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void buttonNextProduct_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void buttonLastProduct_Click(object sender, RoutedEventArgs e)
        {
            MoveToLast();
        }

        private void gridTop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
