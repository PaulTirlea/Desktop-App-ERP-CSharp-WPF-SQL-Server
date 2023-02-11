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
    /// Interaction logic for CreateNewAccount.xaml
    /// </summary>
    public partial class CreateNewAccount : Window
    {
        private LogIn logIn;
        private LogIn LogIn
        {
            get { return logIn; }
            set
            {
                logIn = value;
                this.DataContext = logIn;


            }
        }
        
        public CreateNewAccount()
        {
            InitializeComponent();
            
        }

        public CreateNewAccount(LogIn logIn)
        {
            InitializeComponent();
            this.LogIn = logIn;
            
        }
        private void AddNewAccount()
        {
            if (String.IsNullOrEmpty(lastNameTextBox.Text) || String.IsNullOrEmpty(firstNameTextBox.Text) ||
                String.IsNullOrEmpty(titleTextBox.Text) || String.IsNullOrEmpty(usernameTextBox.Text) ||
                String.IsNullOrEmpty(passwordBox.Password) || String.IsNullOrEmpty(password2Box.Password))
            {
                MessageBox.Show("Nu ați introdus corespunzător datele. \n" + "Operatia nu poate continua.\n" + "Trebuie să completați toate câmpurile!", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }
            else
            {
                string password = passwordBox.Password.Trim();
                string password1 = password2Box.Password.Trim();
                if (password == password1)
                {

                    DialogResult = true;

                    string message = "Ati reusit sa introduceti un nou cont!";
                    string caption = "Felicitări!";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;

                    // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);
                    Close();
                }
                else
                {
                    string message = "Introduceti aceasi parola";
                    string caption = "Atenție!";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;

                    // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);
                }
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void createNewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewAccount();
        }

        private void password2Box_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            logIn.Password = passwordBox.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lastNameTextBox.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void lastNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                firstNameTextBox.Focus();
            }
        }

        private void firstNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                titleTextBox.Focus();
            }
        }

        private void titleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                usernameTextBox.Focus();
            }
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                passwordBox.Focus();
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                password2Box.Focus();
            }
        }

        private void password2Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddNewAccount();
            }
        }
    }
}
