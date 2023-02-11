using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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

namespace TirleaPaul2022
{
    /// <summary>
    /// Interaction logic for CategoryDetailsWindow.xaml
    /// </summary>
    public partial class CategoryDetailsWindow : Window
    {
        private Tools tools = new Tools();
        private byte[] oldPhoto = null;
        private byte[] newphoto = null;

        private TirleaPaul2022Context context = null;
        private List<Category> CategoryList = null;
        private CollectionViewSource categoryViewSource = null;

        private int currentCategoryID = 0;
        private int firstCategoryID = 0;
        private int lastCategoryID = 0;
        private int indexOfCurrentCategory = 0;
        private Category firstCategory;
        private Category lastCategory;

        private Category category;
        private Category Category
        {
            get {return category;}
            set
            {
                category = value;
                this.DataContext = category;
            }
           
        }

        public CategoryDetailsWindow()
        {
            InitializeComponent();
        }

        public CategoryDetailsWindow(Category category)
        {
            InitializeComponent();
            

            categoryViewSource = (CollectionViewSource)this.FindResource("categoryViewSource");
            this.Category = category;
            currentCategoryID = category.CategoryID;
            if (currentCategoryID != 0)
            {
                stackPanelMovement.Visibility = Visibility.Visible;
                LoadCategories();

                for (int i = 0; i < CategoryList.Count(); i++)
                {
                    if (CategoryList[i].CategoryID == currentCategoryID)
                    {
                        indexOfCurrentCategory = i;
                        break;
                    }
                }
            }
            else
            {
                stackPanelMovement.Visibility = Visibility.Collapsed;
                CategoryIDTextBox.IsReadOnly = true;
            }
        }

        private void LoadCategories()
        {
            if (context == null) context = new TirleaPaul2022Context();
            // se incarca datele in context
            context.Categories.Load();

            // se pregateste interogarea (se poate introduce filtreare si ordonare)

            var quary = from c in context.Categories
                        select c;

            //contextul ajunge la lista care contine furnizorii
            this.CategoryList = quary.ToList();

            firstCategory = CategoryList[0];
            firstCategoryID = firstCategory.CategoryID;

            lastCategory = CategoryList[CategoryList.Count() - 1];
            lastCategoryID = lastCategory.CategoryID;

            this.categoryViewSource.Source = this.CategoryList;
        }

        private void MoveToFirst()
        {
            if (currentCategoryID != firstCategoryID)
            {
                this.Close();
                CategoryDetailsWindow detailsWindow = new CategoryDetailsWindow(firstCategory);
                detailsWindow.ShowDialog();
            }
        }

        private void MoveToPrevious()
        {
            if (currentCategoryID != firstCategoryID)
            {
                Category previousCategory = CategoryList[indexOfCurrentCategory - 1];
                this.Close();
                CategoryDetailsWindow detailsWindow = new CategoryDetailsWindow(previousCategory);
                detailsWindow.ShowDialog();

            }
        }

        private void MoveToNext()
        {
            if (lastCategoryID != currentCategoryID)
            {
                Category nextCategory= CategoryList[indexOfCurrentCategory + 1];
                this.Close();
                CategoryDetailsWindow detailsWindow = new CategoryDetailsWindow(nextCategory);
                detailsWindow.ShowDialog();
            }

        }

        private void MoveToLast()
        {
            if (lastCategoryID != currentCategoryID)
            {
                this.Close();
                CategoryDetailsWindow detailsWindow = new CategoryDetailsWindow(lastCategory);
                detailsWindow.ShowDialog();
            }
        }

        private void ChangeCategoryImage()
        {
            // se afiseaza un dialog OPEN FILE pentru a obtine calea completa catre fisier
            string pathToFile = tools.GetFilenameToOpen();

            // daca filename este null, atunci operatia se anuleaza
            //altfel, operatia continua
            if (File.Exists(pathToFile) == false)
            {
                string msg = $"Nu gasesc pe disc fisierul indicat: \n{pathToFile}";
                MessageBox.Show(msg, "Change Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // se face schimbarea imagini folosind fisierul indicat si ferificat
            ChangeCategoryImageUsingfile(pathToFile);
        }

        private void ChangeCategoryImageUsingfile(string pathToFile)
        {
            try
            {
                // se memoreaza vechea imagine din sursa controlului Image (ca byte[])
                oldPhoto = category.Picture;

                //continutul fisierului indicat se incarca in conrolul Image (ca BitmapImage)
                BitmapImage bi = tools.ReadBitmapFromFile(pathToFile);
                categoryImage.Source = bi;

                //continutul fisierului indicat se incarca intr-un array de bytes (byte[])
                // daca se va confirma ulterior pastrarea imaginii, atunci ea va fi salvata
                newphoto = File.ReadAllBytes(pathToFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"EROARE la citirea fisierului \n {pathToFile}\n\n { ex.Message}",
                                "Change image using a file ...",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OkAndCloseWindow()
        {
            if (newphoto != null)
            {
                // se cere o confirmare daca ramane definitiva noua imagine 
                MessageBoxResult confirm = MessageBox.Show("Ramane acesta imagine ? \n\n" +
                    "Daca alegeti NU, atunci se pastreaza imaginea anterioara", "Change image", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    // se salveaza noua imagine in sursa controlului Image
                    category.Picture = newphoto;
                }
                else
                {
                    category.Picture = oldPhoto;
                }
            }
            if (String.IsNullOrEmpty(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Nu ați introdus corespunzător datele. \n" + "Operatia nu poate continua.\n" + "Numele categoriei nu poate fi null!", "Show Details", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // operatia nu poate continua
            }
            else
            {
                DialogResult = true;
                Close();
            }

        }

        private void categoryImage_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void categoryImage_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var filenames = data as string[];
                if (filenames.Length > 0)
                {
                    string pathToFile = filenames[0];
                    ChangeCategoryImageUsingfile(pathToFile);
                }
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonOK_CloseCategoryDetails_Click(object sender, RoutedEventArgs e)
        {
            OkAndCloseWindow();
        }

        private void buttonChangeCategoryImage_Click(object sender, RoutedEventArgs e)
        {
            ChangeCategoryImage();
        }

        private void buttonFirstCategory_Click(object sender, RoutedEventArgs e)
        {
            MoveToFirst();
        }

        private void buttonPreviosCategory_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void buttonNextCategory_Click(object sender, RoutedEventArgs e)
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
    }
}
