using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace TirleaPaul2022
{
    /// <summary>
    /// Interaction logic for SupplierDetailsWindow.xaml
    /// </summary>
    public partial class SupplierDetailsWindow : Window
    {
        #region ================================== VARIABILE ==================================
        private TirleaPaul2022Context context = null;
        private List<Supplier> SupplierList = null;
        private CollectionViewSource supplierViewSource = null;

        private int currentSupplierID = 0;
        private int firstSupplierID = 0;
        private int lastSupplierID = 0;
        private int indexOfCurrentSuppiler = 0;
        private Supplier firstSupplier;
        private Supplier lastSupplier;

        private Supplier supplier;
        public Supplier Supplier
        {
            get { return supplier; }
            set
            {
                supplier = value;
                this.DataContext = supplier;
            }
        }
        #endregion

        #region ================================== CONSTRUCTORI ==================================
        public SupplierDetailsWindow()
        {
            InitializeComponent();
        }

        public SupplierDetailsWindow(Supplier supplier)
        {
            InitializeComponent();
            supplierViewSource = (CollectionViewSource)this.FindResource("supplierViewSource");
            this.Supplier = supplier;
            currentSupplierID = supplier.SupplierID;
            if (currentSupplierID != 0)
            {
                stackPanelMovement.Visibility = Visibility.Visible;
                LoadSuppliers();

                for (int i = 0; i < SupplierList.Count(); i++)
                {
                    if (SupplierList[i].SupplierID == currentSupplierID)
                    {
                        indexOfCurrentSuppiler = i;
                        break;
                    }
                }
            }
            else
            {
                stackPanelMovement.Visibility = Visibility.Collapsed;
                SupplierIDTextBox.IsReadOnly = true;
            }
        }
        #endregion

        #region ================================== METODE ==================================
        private void LoadSuppliers()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.Suppliers.Load();

            // se pregateste interogarea (se poate introduce filtreare si ordonare)

            var quary = from s in context.Suppliers
                        select s;

            //contextul ajunge la lista care contine furnizorii
            this.SupplierList = quary.ToList();

            firstSupplier = SupplierList[0];
            firstSupplierID = firstSupplier.SupplierID;

            lastSupplier = SupplierList[SupplierList.Count() - 1];
            lastSupplierID = lastSupplier.SupplierID;

            this.supplierViewSource.Source = this.SupplierList;
        }

        private void MoveToFirst()
        {
            if (currentSupplierID != firstSupplierID)
            {
                this.Close();
                SupplierDetailsWindow supplierDetailsWindow = new SupplierDetailsWindow(firstSupplier);
                supplierDetailsWindow.ShowDialog();
                //this.DataContext = firstSupplier;
                //indexOfCurrentSuppiler = 0;
            }
        }

        private void MoveToPrevious()
        {
            if (currentSupplierID != firstSupplierID)
            {
                Supplier previousSupplier = SupplierList[indexOfCurrentSuppiler - 1];

                //if (indexOfCurrentSuppiler != 1)
                //{
                //    indexOfCurrentSuppiler -= 1;
                //}
                //this.DataContext = previousSupplier;

                this.Close();
                SupplierDetailsWindow supplierDetailsWindow = new SupplierDetailsWindow(previousSupplier);
                supplierDetailsWindow.ShowDialog();
                

            }
        }

        private void MoveToNext()
        {
            if(lastSupplierID != currentSupplierID)
            {
                Supplier nextSupplier = SupplierList[indexOfCurrentSuppiler + 1];
                //if (indexOfCurrentSuppiler != SupplierList.Count() - 2 )
                //{
                //    indexOfCurrentSuppiler += 1;
                //}
                //this.DataContext = nextSupplier;
                this.Close();
                SupplierDetailsWindow supplierDetailsWindow = new SupplierDetailsWindow(nextSupplier);
                supplierDetailsWindow.ShowDialog();
            }

        }

        private void MoveToLast()
        {
            if(lastSupplierID != currentSupplierID)
            {
                this.Close();
                SupplierDetailsWindow supplierDetailsWindow = new SupplierDetailsWindow(lastSupplier);
                supplierDetailsWindow.ShowDialog();
                //this.DataContext = lastSupplier;
                //indexOfCurrentSuppiler = SupplierList.Count() - 1;
            }
        }
        private void OKAndCloseWindow()
        {
            if (String.IsNullOrEmpty(CompanyNameTextBox.Text))
            {
                MessageBox.Show("Nu ați introdus corespunzător datele. \n" + "Operatia nu poate continua.\n" 
                    +"Numele companiei nu poate fi null!" , "Show Details", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }
            else
            {
                DialogResult = true;
                Close();
            }

        }
        #endregion

        #region ================================== EVENIMENTE ==================================
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOK_CloseSupplierDetails_Click(object sender, RoutedEventArgs e)
        {
            OKAndCloseWindow();
        }

        private void buttonFirstSupplier_Click(object sender, RoutedEventArgs e)
        {
            MoveToFirst();
        }

        private void buttonPreviosSupplier_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void buttonNextSupplier_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void buttonLastSupplier_Click(object sender, RoutedEventArgs e)
        {
            MoveToLast();
        }

        private void gridTop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }
}
