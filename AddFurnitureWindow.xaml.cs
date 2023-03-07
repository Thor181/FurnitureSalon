using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для AddFurnitureWindow.xaml
    /// </summary>
    public partial class AddFurnitureWindow : Window
    {
        private ObservableCollection<FurnitureType> FurnitureTypesProp { get; set; } 
            = new ObservableCollection<FurnitureType>(ConnectDB.db.FurnitureType.ToList());

        private ObservableCollection<FurnitureName> FurnitureNamesProp { get; set; }
            = new ObservableCollection<FurnitureName>(ConnectDB.db.FurnitureName.ToList());

        private ObservableCollection<FurnitureList> FurnitureListProp { get;set; }
            = new ObservableCollection<FurnitureList>(ConnectDB.db.FurnitureList.ToList());

        private bool EditProcess { get; set; } = false;
        private FurnitureList EditableItem { get; set; }

        public AddFurnitureWindow()
        {
            InitializeComponent();
            InitializeNew();
        }

        public AddFurnitureWindow(FurnitureList editableItem)
        {
            InitializeComponent();
            InitializeNew();
            EditableItem = editableItem;
            EditProcess = true;
            Title = "Редактирование";
            headingLabel.Text = "Редактирование мебели в базе";
            typeFurnitureCombobox.SelectedItem = editableItem.FurnitureType;
            nameFurnitureCombobox.SelectedItem = editableItem.FurnitureName;
            priceTextbox.Text = string.Format($"{editableItem.price:#.##}");
            
            PlaceForPhoto.Source = ConvertImage(editableItem);
            
        }
        private BitmapImage ConvertImage(FurnitureList editableItem)
        {
            if (editableItem.photo != null)
            {
                System.Drawing.Image image = (Bitmap)((new ImageConverter()).ConvertFrom(editableItem.photo));
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            return null;
        }

        private void InitializeNew()
        {
            typeFurnitureCombobox.ItemsSource = FurnitureTypesProp;
            nameFurnitureCombobox.ItemsSource = FurnitureNamesProp;
        }
        public new ObservableCollection<FurnitureList> ShowDialog()
        {
            base.ShowDialog();
            return FurnitureListProp;
        }

        private byte[] _imageData;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog loadPhoto = new OpenFileDialog();
            loadPhoto.Filter = "Файлы изображений|*.bmp;*.jpg;*.gif;*.png;*.tif|Все файлы|*.*";
            if ((bool)loadPhoto.ShowDialog())
            {
                var image = new BitmapImage(new Uri(loadPhoto.FileName));
                if (true)
                {
                    PlaceForPhoto.Source = image;
                    _imageData = SaveImage(loadPhoto.FileName);
                }
            }
        }

        private byte[] SaveImage(string fileImage)
        {
            byte[] imageData;
            using (System.IO.FileStream fs = new System.IO.FileStream(fileImage, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }
            return imageData;
        }

        private void AddFurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditProcess == true)
            {
                if (_imageData == null)
                {
                    _imageData = EditableItem.photo;
                }
            }
            bool otherTypeAdded = false;
            bool otherNameAdded = false;

            if (enabledOtherTypeCheckbox.IsChecked == true)
            {
                otherTypeAdded = true;
                ConnectDB.db.FurnitureType.Add(new FurnitureType() { furniture_type = typeFurnitureTextbox.Text});
                ConnectDB.db.SaveChanges();
            }

            if (enabledOtherNameCheckbox.IsChecked == true)
            {
                otherNameAdded = true;
                ConnectDB.db.FurnitureName.Add(new FurnitureName() { furniture_name = nameFurnitureTextbox.Text });
                ConnectDB.db.SaveChanges();
            }

            try
            {
                if (EditProcess == true)
                {
                    EditableItem.furniture_type_id = otherTypeAdded == true ? ConnectDB.db.FurnitureType.ToList().Last().id : (typeFurnitureCombobox.SelectedItem as FurnitureType).id;
                    EditableItem.furniture_name_id = otherNameAdded == true ? ConnectDB.db.FurnitureName.ToList().Last().id : (nameFurnitureCombobox.SelectedItem as FurnitureName).id;
                    EditableItem.price = priceTextbox.Text == "" ? 0 : decimal.Parse(priceTextbox.Text);
                    EditableItem.photo = _imageData;
                }
                else
                {
                    ConnectDB.db.FurnitureList.Add(new FurnitureList()
                    {
                        furniture_type_id = otherTypeAdded == true ? ConnectDB.db.FurnitureType.ToList().Last().id : (typeFurnitureCombobox.SelectedItem as FurnitureType).id,
                        furniture_name_id = otherNameAdded == true ? ConnectDB.db.FurnitureName.ToList().Last().id : (nameFurnitureCombobox.SelectedItem as FurnitureName).id,
                        price = priceTextbox.Text == "" ? 0 : decimal.Parse(priceTextbox.Text),
                        photo = _imageData
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {                
                ConnectDB.db.SaveChanges();
                FurnitureListProp = new ObservableCollection<FurnitureList>(ConnectDB.db.FurnitureList.ToList());
                Close();
            }            
        }
    }
}
