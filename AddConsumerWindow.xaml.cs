using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для AddConsumerWindow.xaml
    /// </summary>
    public partial class AddConsumerWindow : Window
    {
        private ObservableCollection<ConsumerList> ConsumerListProp { get; set; }
            = new ObservableCollection<ConsumerList>(ConnectDB.db.ConsumerList.ToList());
        private bool AddedNewConsumer { get; set; } = false;
        public AddConsumerWindow()
        {
            InitializeComponent();
        }

        public new ObservableCollection<ConsumerList> ShowDialog()
        {
            base.ShowDialog();
            return ConsumerListProp;
        }
        
        public new ObservableCollection<ConsumerList> ShowDialog(out bool addedNewConsumer)
        {
            base.ShowDialog();
            addedNewConsumer = AddedNewConsumer;
            return ConsumerListProp;
        }

        private void AddConsumerButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectDB.db.ConsumerList.Add(new ConsumerList()
            {
                surname = surnameTextbox.Text,
                name = nameTextbox.Text,
                patronymic = patronymicTextbox.Text
            });
            ConnectDB.db.SaveChanges();
            ConsumerListProp = new ObservableCollection<ConsumerList>(ConnectDB.db.ConsumerList.ToList());
            AddedNewConsumer = true;
            Close();
        }
    }
}
