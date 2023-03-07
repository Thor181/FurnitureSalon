using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для ConsumerListWindow.xaml
    /// </summary>
    public partial class ConsumerListWindow : Window
    {
        private ObservableCollection<ConsumerList> ConsumerListProp { get; set; }
            = new ObservableCollection<ConsumerList>(ConnectDB.db.ConsumerList.ToList());

        public ConsumerListWindow()
        {
            InitializeComponent();
            ConsumerListDatagrid.ItemsSource = ConsumerListProp;
        }

        private void AddConsumerButton_Click(object sender, RoutedEventArgs e)
        {
            AddConsumerWindow addConsumerWindow = new AddConsumerWindow() { Owner = this};
            ConsumerListProp = addConsumerWindow.ShowDialog();
            ConsumerListDatagrid.ItemsSource = ConsumerListProp;
        }

        private void DeleteConsumerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConsumerListDatagrid.SelectedItem is ConsumerList deletableItem)
            {
                ConnectDB.db.ConsumerList.Remove(deletableItem);
                ConnectDB.db.SaveChanges();
                ConsumerListProp = new ObservableCollection<ConsumerList>(ConnectDB.db.ConsumerList.ToList());
                ConsumerListDatagrid.ItemsSource = ConsumerListProp;
            }
        }

        private void searchConsumerTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var findableItem = searchConsumerTextbox.Text;
            var foundItems = ConnectDB.db.ConsumerList.Where(x => x.surname.Contains(findableItem)
                                                               || x.name.Contains(findableItem)
                                                               || x.patronymic.Contains(findableItem)).ToList();
            ConsumerListDatagrid.ItemsSource = foundItems;
        }
    }
}
