using TirleaPaul2022.User_Control_Windows;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for OrderGraphsWindow.xaml
    /// </summary>
    public partial class OrderGraphsWindow : Window
    {
        public OrderGraphsWindow()
        {
            InitializeComponent();
        }




        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            switch (index)
            {
                case 0:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new OrderViewByDateWindow());
                    txtBlockDate.Foreground = new SolidColorBrush(Color.FromArgb(200, 123, 31, 162));
                    iconDate.Foreground = new SolidColorBrush(Color.FromArgb(200, 123, 31, 162));
                    txtBlocCategory.Foreground = new SolidColorBrush(Color.FromArgb(225, 225, 225, 225));
                    iconCategory.Foreground = new SolidColorBrush(Color.FromArgb(225, 225, 225, 225));
                    break;
                case 1:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new OrderViewByCategory());
                    txtBlocCategory.Foreground = new SolidColorBrush(Color.FromArgb(200, 123, 31, 162));
                    iconCategory.Foreground = new SolidColorBrush(Color.FromArgb(200, 123, 31, 162));
                    txtBlockDate.Foreground = new SolidColorBrush(Color.FromArgb(225, 225, 225, 225));
                    iconDate.Foreground = new SolidColorBrush(Color.FromArgb(225, 225, 225, 225));
                    break;
                case 2:
                    //GridContent.Children.Clear();
                    //GridContent.Children.Add(new CreateBarcode());
                    break;
                case 3:
                    //GridContent.Children.Clear();
                    //GridContent.Children.Add(new ScanBarcode());
                    break;
            }
        }

        private void DragGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GridContent.Children.Clear();
            GridContent.Children.Add(new OrderViewByDateWindow());
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
