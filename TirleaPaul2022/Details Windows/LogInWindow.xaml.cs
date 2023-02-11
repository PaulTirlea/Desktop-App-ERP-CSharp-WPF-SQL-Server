using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
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
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        #region ================================== VARIABILE ==================================
        // creeam variabilele
        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private TirleaPaul2022Context context = null;
        private CollectionViewSource loginViewSource = null;
        private List<LogIn> LogInsList = null;
        #endregion
        public LogInWindow()
        {
            InitializeComponent();
            loginViewSource = (CollectionViewSource)this.FindResource("loginViewSource");
        }
        private void LoadClientData()
        {
            try
            {
                if (context == null) context = new TirleaPaul2022Context();

                // se incarca datele in context
                context.LogIns.Load();

                //contextul ajunge la lista care contine furnizorii
                LogInsList = context.LogIns.ToList();

                loginViewSource.Source = LogInsList;
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
        private void SearhCredentials()
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordBox.Password.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                string message = "Nu uita sa introduci credentialiele";
                string caption = "Atenție!";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;

                // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
                MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);
            }
            else
            {
                bool control = false;
                foreach (LogIn li in LogInsList)
                {
                    if (username == li.UserName && password == li.Password)
                    {
                        control = true;
                    }

                }
                if (control == true)
                {
                    string message = "V-ati conecetat cu succes!";
                    string caption = "Felicitari!";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                   
                    // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);

                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    string message = "Credențialele nu au fost introduse corect";
                    string caption = "Atenție!";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;

                    // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
                    MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);
                }
            }
            
        }

        private void SaveData(bool showConfiguration = true, bool showSuccess = true)
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (loginViewSource == null || loginViewSource.View == null)
            {
                MessageBox.Show("Nu exista date in tabel. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void AddNew()
        {
            // se fac o serie de verificari asupra vederii de tip CollectionViewSource
            if (loginViewSource == null || loginViewSource.View == null)
            {
                MessageBox.Show("Mai intai incarcati datele. \n " + "Operatia nu poate continua.", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }

            // se pregateste un nou obiect din clasa LogIn
            LogIn newLogIn = new LogIn();

            // se deschide o nou fereastra cu detalii folosind constructorul specializat
            CreateNewAccount window = new CreateNewAccount(newLogIn);

            // se seteaza pozitia initiala pe ecran 
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // se seteaza unele proprientati ale ferestrei 
            window.Title = "Add new ...";

            // se afiseaza fereastra ca un dialog modal
            // rezultatul dialogului poate fi 'true' sau 'false'
            // in fereastra, utilizatorul seteaza proprietatile pentru obiectul de tip Supplier
            window.ShowDialog();

            // daca fereasta a fost inchisa cu OK, atunci conteaza modificarile facute
            if (window.DialogResult == true)
            {
                context.LogIns.Add(newLogIn);
                // se salveaza modificarile facute
                SaveData(showConfiguration: false, showSuccess: true);
                LoadClientData();
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            //string message = "Doriti inchiderea ferestrei de login?";
            //string caption = "Inchidere!";
            //MessageBoxButton buttons = MessageBoxButton.OKCancel;
            //MessageBoxImage icon = MessageBoxImage.Question;

            //// se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
            //MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);

            //// daca utilizatorul a apasat "Cancel" atunci fereastra nu se mai inchide 
            //if (result == MessageBoxResult.Cancel)
            //{
            //    return;
            //}
            //else
            //{
            //    Close();
            //}

            Close();
        }

        private void Ajutor_Click(object sender, RoutedEventArgs e)
        {
            string message = "Introduceți numele de utilizator și parola pentru a efectua conectarea \n\n" +
                             "Dacă ați uitat credențialele sau nu ați mai accesat aplicația luați legătura cu dezvoltatorul aplicației pe adresa de email: \n"+
                             "tirlea.paul1@gmail.com";
            string caption = "Aveti nevoie de ajutor?";
            MessageBoxButton buttons = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            // se afiseaza dialogul de confirmare la care se raspunde "OK" sau "Cancel"
            MessageBox.Show(message, caption, buttons, icon);
        }

        private void myToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            // preluam tema in variabila de tip Itheme
            ITheme theme = paletteHelper.GetTheme();

            // creeam o conditie ce verifica tema 
            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                // setam tema ca fiind Light
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                // setam tema Dark
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            // pentru a aplica schimbarile, utilisam functia SetTheme
            paletteHelper.SetTheme(theme);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClientData();
            usernameTextBox.Focus();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            SearhCredentials();
        }

        private void createNewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
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
                SearhCredentials();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
