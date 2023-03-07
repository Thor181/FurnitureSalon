using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureSalon
{
    /// <summary>
    /// Логика взаимодействия для OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        private ObservableCollection<OrderList> OrderListProp { get; set; }
            = new ObservableCollection<OrderList>(ConnectDB.db.OrderList.ToList());

        private DateTime? ReportStartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        private DateTime? ReportEndDate { get; set; } = DateTime.Now;

        public OrderListWindow()
        {
            InitializeComponent();
            OrdersListDataGrid.ItemsSource = OrderListProp; 
            DateStartPicker.SelectedDate = ReportStartDate;
            DateEndPicker.SelectedDate = ReportEndDate;
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            AddOrderWindow addOrderWindow = new AddOrderWindow() { Owner = this};
            bool newOrderAdded = addOrderWindow.ShowDialog();
            if (newOrderAdded == true)
            {
                OrderListProp = new ObservableCollection<OrderList>(ConnectDB.db.OrderList.ToList());
                OrdersListDataGrid.ItemsSource = OrderListProp;
            }
        }

        private void searchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = searchTextbox.Text.ToLower();
            if (OrderListProp.Count == 0)
            {
                OrderListProp = new ObservableCollection<OrderList>(ConnectDB.db.OrderList.ToList());
            }
            else if (searchText == "")
            {
                OrderListProp = new ObservableCollection<OrderList>(ConnectDB.db.OrderList.ToList());
            }
            var fountItems = OrderListProp.Where(x => x.date_selling.ToString().ToLower().Contains(searchText)
                                                            || x.FurnitureList.FurnitureType.furniture_type.ToLower().Contains(searchText)
                                                            || x.FurnitureList.FurnitureName.furniture_name.ToLower().Contains(searchText)
                                                            || x.ConsumerList.surname.ToLower().Contains(searchText)
                                                            || x.ConsumerList.name.ToLower().Contains(searchText)
                                                            || x.ConsumerList.patronymic.ToLower().Contains(searchText)
                                                            || x.count_product.ToString().ToLower().Contains(searchText)
                                                            || x.deliver_price.ToString().ToLower().Contains(searchText)
                                                            || x.installation_price.ToString().ToLower().Contains(searchText)
                                                            || x.discount.ToString().ToLower().Contains(searchText)
                                                            || x.total_sum.ToString().ToLower().Contains(searchText)).ToList();
            OrderListProp = new ObservableCollection<OrderList>(fountItems);
            OrdersListDataGrid.ItemsSource = OrderListProp;                                            
        }

        private void DateStartPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ReportStartDate = DateStartPicker.SelectedDate;
        }

        private void DateEndPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ReportEndDate = DateEndPicker.SelectedDate;
        }

        private void OrdersOfDateButton_Click(object sender, RoutedEventArgs e)
        {
            var foundItems = ConnectDB.db.OrderList.Where(x => x.date_selling >= ReportStartDate && x.date_selling <= ReportEndDate).ToList();
            OrderListProp = new ObservableCollection<OrderList>(foundItems);
            OrdersListDataGrid.ItemsSource = OrderListProp;
            searchTextbox.Text = "";
        }

        enum ModePrint
        {
            CurrentReport = 1,
            ConsumerReport = 2
        }

        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {
            decimal? totalSumDeliver = 0;
            decimal? totalSumInstallation = 0;
            decimal? totalSumOrders = 0;
            foreach (var item in OrderListProp)
            {
                totalSumDeliver += item.deliver_price;
                totalSumInstallation += item.installation_price;
                totalSumOrders += item.total_sum;
            }
            string ttf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 8, Font.NORMAL);
            PdfPTable table = new PdfPTable(10);
            table.AddCell(new Paragraph($"№ п/п", font));
            table.AddCell(new Paragraph($"Учетный номер продажи", font));
            for (int i = 0; i < 8; i++)
            {
                table.AddCell(new Paragraph($"{OrdersListDataGrid.Columns[i].Header}", font));
            }
            for (int i = 0; i < OrderListProp.Count; i++)
            {
                table.AddCell(new Paragraph($"{i+1}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].id}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].date_selling}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].FurnitureList.FurnitureType.furniture_type}: {OrderListProp[i].FurnitureList.FurnitureName.furniture_name}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].count_product}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].ConsumerList.surname} {OrderListProp[i].ConsumerList.name} {OrderListProp[i].ConsumerList.patronymic}", font));
                if (OrderListProp[i].deliver_price == null)
                {
                    table.AddCell(new Paragraph($"0", font));
                }
                else
                {
                    table.AddCell(new Paragraph($"{OrderListProp[i].deliver_price:#.##}", font));
                }
                if (OrderListProp[i].installation_price == null)
                {
                    table.AddCell(new Paragraph($"0", font));
                }
                else
                {
                    table.AddCell(new Paragraph($"{OrderListProp[i].installation_price:#.##}", font));
                }
                table.AddCell(new Paragraph($"{OrderListProp[i].discount}", font));
                table.AddCell(new Paragraph($"{OrderListProp[i].total_sum:#.##}", font));
            }

            table.AddCell(new Paragraph($"ИТОГО", font));

            for (int i = 0; i < 5; i++)
            {
                table.AddCell(new Paragraph($"   ", font));
            }
            table.AddCell(new Paragraph($"{totalSumDeliver:#.##}", font));
            table.AddCell(new Paragraph($"{totalSumInstallation:#.##}", font));
            table.AddCell(new Paragraph($"    ", font));
            table.AddCell(new Paragraph($"{totalSumOrders:#.##}", font));
            Printer("OrdersList.pdf", table, ModePrint.CurrentReport);
        }

        private void PrintReportConsumerButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, decimal> dictionaryTotalSumConsumers = new Dictionary<string, decimal>();
            foreach (var consumer in ConnectDB.db.ConsumerList.ToList().OrderBy(x => x.surname))
            {
                var currentConsumer = $"{consumer.surname} {consumer.name} {consumer.patronymic}";
                var currentIDConsumer = consumer.id;
                try
                {
                    dictionaryTotalSumConsumers.Add(currentConsumer, ConnectDB.db.OrderList.Where(x => x.consumer_id == currentIDConsumer).Sum(x => x.total_sum));
                }
                catch (Exception)
                {
                }
            }
            string ttf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 8, Font.NORMAL);
            PdfPTable table = new PdfPTable(3);
            table.AddCell(new Paragraph($"№ п/п", font));
            table.AddCell(new Paragraph($"ФИО покупателя", font));
            table.AddCell(new Paragraph($"Всего потрачено", font));
            int i = 1;
            foreach (var item in dictionaryTotalSumConsumers)
            {
                table.AddCell(new Paragraph($"{i++}", font));
                table.AddCell(new Paragraph($"{item.Key}", font));
                table.AddCell(new Paragraph($"{item.Value:#.##}", font));
            }            
            Printer("ConsumerReport.pdf", table, ModePrint.ConsumerReport);
        }

        private void PrintReportFurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            decimal? totalSumOrders = 0;
            foreach (var item in OrderListProp)
            {
                totalSumOrders += item.total_sum;
            }
            string ttf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 8, Font.NORMAL);
            var headFont = new Font(baseFont, 15, Font.NORMAL);
            FileStream fs = new FileStream("FurnitureReport.pdf", FileMode.Create);
            Document document = new Document(PageSize.A4, 50, 10, 10, 10);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            document.Add(new iTextSharp.text.Paragraph($"ПОМЕСЯЧНЫЙ ОТЧЕТ", headFont));

            int yearStart = ConnectDB.db.OrderList.OrderBy(x => x.date_selling).First().date_selling.Year;
            int yearEnd = ConnectDB.db.OrderList.OrderByDescending(x => x.date_selling).First().date_selling.Year;

            for (int currentYear = yearStart; currentYear <= yearEnd; currentYear++)
            {
                for (int currentMonth = 1; currentMonth <= 12; currentMonth++)
                {
                    if (ConnectDB.db.OrderList.Any(x => x.date_selling.Month == new DateTime(currentYear, currentMonth, 1).Month && x.date_selling.Year == new DateTime(currentYear, currentMonth, 1).Year))
                    {
                        int i = 1;
                        document.Add(new Paragraph($"{new DateTime(currentYear, currentMonth, 1):MMMM} {currentYear}", font));

                        foreach (var furniture in ConnectDB.db.FurnitureList.ToList().OrderBy(x => x.FurnitureType.furniture_type))
                        {
                            string currentFurniture = $"{furniture.FurnitureType.furniture_type}: {furniture.FurnitureName.furniture_name}";
                            int currentIDFurniture = furniture.id;

                            try
                            {
                                var totalSumCurrentFurniture = ConnectDB.db.OrderList.Where(x => x.furniture_id == currentIDFurniture && x.date_selling >= new DateTime(currentYear, currentMonth, 1) && x.date_selling <= new DateTime(currentYear, currentMonth, 31)).Sum(x => x.total_sum);
                                document.Add(new Paragraph($"      {i++}. {currentFurniture} = {totalSumCurrentFurniture:C2}", font));
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            document.Add(new Paragraph($"      ", font));
            document.Add(new Paragraph($"ИТОГО: {totalSumOrders:C2}", font));
            document.Close();
            writer.Close();
            fs.Close();
            Opener.OpenCreatedFile("FurnitureReport.pdf");
        }

        private void Printer(string nameFile, PdfPTable table, ModePrint mode)
        {
            string ttf = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 8, Font.NORMAL);
            FileStream fs = new FileStream(nameFile, FileMode.Create);
            Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            if (mode == ModePrint.CurrentReport)
            {
                document.Add(new Paragraph($"         {DateTime.Now}", font));
                
            }
            else if(mode == ModePrint.ConsumerReport)
            {
                document.Add(new Paragraph($"         {DateTime.Now}        Расходы покупателей", font));
            }
            document.Add(new Paragraph($"    ", font));
            document.Add(table);
            document.Close();
            writer.Close();
            fs.Close();
            Opener.OpenCreatedFile(nameFile);
        }
    }
}
