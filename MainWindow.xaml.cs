using System.Windows;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void FurnitureListButton_Click(object sender, RoutedEventArgs e)
        {
            FurnitureListWindow furnitureListWindow = new FurnitureListWindow() { Owner = this };
            furnitureListWindow.Show();
        }

        private void ConsumerListButton_Click(object sender, RoutedEventArgs e)
        {
            ConsumerListWindow consumerListWindow = new ConsumerListWindow() { Owner = this };
            consumerListWindow.Show();
        }

        private void OrderListButton_Click(object sender, RoutedEventArgs e)
        {
            OrderListWindow orderListWindow = new OrderListWindow() { Owner = this };
            orderListWindow.Show();
        }
    }
}
