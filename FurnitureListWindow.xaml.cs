using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для FurnitureListWindow.xaml
    /// </summary>
    public partial class FurnitureListWindow : Window
    {
        private ObservableCollection<FurnitureList> FurnitureListProp { get; set; } 
            = new ObservableCollection<FurnitureList>(ConnectDB.db.FurnitureList.ToList());

        public FurnitureListWindow()
        {
            InitializeComponent();
            FurnitureListDataGrid.ItemsSource = FurnitureListProp;
        }

        private void AddFurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            AddFurnitureWindow addFurnitureWindow = new AddFurnitureWindow()  { Owner = this };
            if (addFurnitureWindow.ShowDialog() is ObservableCollection<FurnitureList> newItem)
            {
                FurnitureListProp = newItem;
                FurnitureListDataGrid.ItemsSource = FurnitureListProp;
            }
        }

        private void DeleteFurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            if (FurnitureListDataGrid.SelectedItem is FurnitureList deleteItem)
            {
                try
                {
                    ConnectDB.db.FurnitureList.Remove(deleteItem);
                }
                catch (Exception)
                {
                }
                finally
                {
                    ConnectDB.db.SaveChanges();
                    FurnitureListProp = new ObservableCollection<FurnitureList>(ConnectDB.db.FurnitureList.ToList());
                    FurnitureListDataGrid.ItemsSource = FurnitureListProp;
                }
            }
        }

        private void EditFurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            if (FurnitureListDataGrid.SelectedItem is FurnitureList selectedItem)
            {
                AddFurnitureWindow editFurnitureWindow = new AddFurnitureWindow(selectedItem) { Owner = this, Title = "Редактировать мебель"};
                if (editFurnitureWindow.ShowDialog() is ObservableCollection<FurnitureList> editedItem)
                {
                    FurnitureListProp = editedItem;
                    FurnitureListDataGrid.ItemsSource = FurnitureListProp;
                }
            }
        }

        private void searchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextbox.Text;
            var foundItem = ConnectDB.db.FurnitureList.Where(x => x.FurnitureName.furniture_name.Contains(searchText)
                                                               || x.FurnitureType.furniture_type.Contains(searchText)
                                                               || x.price.ToString().Contains(searchText)).ToList();
            FurnitureListProp = new ObservableCollection<FurnitureList>(foundItem);
            FurnitureListDataGrid.ItemsSource = FurnitureListProp;
        }
    }
}
