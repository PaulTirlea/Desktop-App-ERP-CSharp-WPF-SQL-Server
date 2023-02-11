using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TirleaPaul2022.User_Control_Windows;

namespace TirleaPaul2022
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ================================== CONSTRUCTOR ==================================
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion
        #region ================================== METODE ==================================
        private void SwitchMenuItem()
        {
            int index = ListViewMenu.SelectedIndex;
            switch (index)
            {
                case 0:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new HomeWindow(LogInStatus: true));
                    break;
                case 1:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new SuppliersWindow());
                    break;
                case 2:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new CategoryWindow());
                    break;
                case 3:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new CustomerWindow());
                    break;
                case 4:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new OrderWindow());
                    break;
                case 5:
                    GridContent.Children.Clear();
                    GridContent.Children.Add(new StockWindow());
                    break;

            }
        }
        #endregion

        #region ================================== EVENIMENTE ==================================
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SwitchMenuItem();
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            GridContent.Height = GridContent.Height + 40;
            GridContent.Width = GridContent.Width + 260;
            GridContent.Margin = new Thickness(0, 0, 0, 0);
            gridButtons.Background = new SolidColorBrush(Color.FromArgb(201, 74, 20, 140));
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
            GridContent.Height = 500;
            GridContent.Width = 770;
            GridContent.Margin = new Thickness(-240, -50, 0, 0);
            gridButtons.Background = new SolidColorBrush(Color.FromArgb(200, 124, 77, 255));
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void buttonMaximize_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            WindowState = WindowState.Maximized;

            gridMain.Width = gridAllContent.Width;  // = 1274;
            gridMain.Margin = new Thickness(250, 0, 0, 0);
            gridMenu.Margin = new Thickness(0, 0, 0, 0);

            GridContent.Width = gridMain.Width; // = 1274;
            GridContent.Height = 750;
            buttonMaximize.Visibility = Visibility.Hidden;
            buttonNormalSize.Visibility = Visibility.Visible;
            gridButtons.Background = new SolidColorBrush(Color.FromArgb(200, 124, 77, 255));
        }

        private void buttonNormalSize_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Visible;
            WindowState = WindowState.Normal;
            gridMain.Width = 1024;
            gridMain.Margin = new Thickness(0, 0, 0, 0);
            gridMenu.Margin = new Thickness(-250, 0, 0, 0);
            GridContent.Width = 1000;
            GridContent.Height = 500;
            buttonMaximize.Visibility = Visibility.Visible;
            buttonNormalSize.Visibility = Visibility.Hidden;
            gridButtons.Background = new SolidColorBrush(Color.FromArgb(201, 74, 20, 140));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gridButtons.Background = new SolidColorBrush(Color.FromArgb(201, 74, 20, 140));
            GridContent.Height = GridContent.Height + 40;
            GridContent.Width = GridContent.Width + 250;
            GridContent.Children.Clear();
            GridContent.Children.Add(new HomeWindow());
        }

        private void gridButtons_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }
}
