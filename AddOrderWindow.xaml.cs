using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для AddOrderWindow.xaml
    /// </summary>
    public partial class AddOrderWindow : Window
    {
        private ObservableCollection<ConsumerList> ConsumersListProp { get; set; }
            = new ObservableCollection<ConsumerList>(ConnectDB.db.ConsumerList.ToList().OrderBy(x => x.surname));

        private ObservableCollection<FurnitureList> FurnitureListProp { get; set; }
            = new ObservableCollection<FurnitureList>(ConnectDB.db.FurnitureList.ToList().OrderBy(x => x.FurnitureType.furniture_type));

        private decimal TotalSum { get; set; }
        private double Discount { get; set; } = 0;

        private decimal DeliverPrice { get; set; } = 0;
        private decimal InstallationPrice { get; set; } = 0;

        private bool NewOrderAdded { get; set; } = false;
        public AddOrderWindow()
        {
            InitializeComponent();
            ConsumerListCombobox.ItemsSource = ConsumersListProp;
            FurnitureCombobox.ItemsSource = FurnitureListProp;
            FurnitureCountCombobox.ItemsSource = new List<int>(Enumerable.Range(1, 10).ToList());            
        }

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return NewOrderAdded;
        }

        private void NewConsumerButton_Click(object sender, RoutedEventArgs e)
        {
            AddConsumerWindow addConsumerWindow = new AddConsumerWindow() { Owner = this};
            bool addedNewConsumer;
            ConsumersListProp = addConsumerWindow.ShowDialog(out addedNewConsumer);
            if (addedNewConsumer == true)
            {
                ConsumerListCombobox.ItemsSource = ConsumersListProp;
                ConsumerListCombobox.SelectedItem = ConsumersListProp.Last();
            }         
        }
        #region CalculateTotalSum
        private void FurnitureCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FurnitureCountCombobox.SelectedIndex == -1)
            {
                FurnitureCountCombobox.SelectedIndex = 0;
            }
            var totalSumFurniture = ((sender as ComboBox).SelectedItem as FurnitureList).price * (int)FurnitureCountCombobox.SelectedItem;
            totalSumFurnitureBox.Text = string.Format($"{totalSumFurniture:C2}");
            DisplayTotalSum();                
        }

        private void FurnitureCountCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FurnitureCombobox.SelectedIndex != -1)
            {
                var totalSumFurniture = (FurnitureCombobox.SelectedItem as FurnitureList).price * (int)FurnitureCountCombobox.SelectedItem;
                totalSumFurnitureBox.Text = string.Format($"{totalSumFurniture:C2}");
            }
            DisplayTotalSum();
        }

        private void discountOtherTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            discount0RadioButton.IsChecked = discount10RadioButton.IsChecked = discount15RadioButton.IsChecked = false;
            double otherDiscount = 0; double.TryParse(discountOtherTextbox.Text, out otherDiscount);
            Discount = otherDiscount;
            DisplayTotalSum();
        }

        private void SetDiscount(object sender, RoutedEventArgs e)
        {
            switch ((sender as Control).Name)
            {
                case "discount0RadioButton":
                    Discount = 0;
                    break;
                case "discount10RadioButton":
                    Discount = 10;
                    break;
                case "discount15RadioButton":
                    Discount = 15;
                    break;
                default:
                    break;
            }
            DisplayTotalSum();
        }


        private void deliverPriceTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal deliverPrice = 0; decimal.TryParse(deliverPriceTextbox.Text, out deliverPrice);
            DeliverPrice = deliverPrice;
            DisplayTotalSum();
        }

        private void installationPriceTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal installationPrice = 0; decimal.TryParse(installationPriceTextbox.Text, out installationPrice);
            InstallationPrice = installationPrice;
            DisplayTotalSum();
        }
        private void DisplayTotalSum()
        {
            if (FurnitureCombobox.SelectedIndex == -1)
            {
                return;
            }
            decimal furniturePrice = (FurnitureCombobox.SelectedItem as FurnitureList).price;
            decimal furnitureCount = (int)FurnitureCountCombobox.SelectedItem;
            decimal deliverAndInstallPrice = (decimal)(DeliverPrice + InstallationPrice);
            var previouslySum = (furniturePrice * furnitureCount);
            double discount = Discount;
            
            if (discount != 0)
            {
                TotalSum = previouslySum - (previouslySum * (decimal)discount / 100);
            }
            else
            {
                TotalSum = previouslySum;
            }
            TotalSum += deliverAndInstallPrice;
            TotalOrderSumBox.Text = String.Format($"{TotalSum:C2}");
        }
        #endregion
        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectDB.db.OrderList.Add(new OrderList()
                {
                    date_selling = DateTime.Now,
                    furniture_id = (FurnitureCombobox.SelectedItem as FurnitureList).id,
                    consumer_id = (ConsumerListCombobox.SelectedItem as ConsumerList).id,
                    count_product = (int)FurnitureCountCombobox.SelectedItem,
                    deliver_price = DeliverPrice,
                    installation_price = InstallationPrice,
                    discount = (int)Discount,
                    total_sum = TotalSum
                });
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                ConnectDB.db.SaveChanges();
                NewOrderAdded = true;
                Close();
            }
        }
    }
}
