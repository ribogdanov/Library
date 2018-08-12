using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Windows.CustomerWindows.CustomerLogin customerLoginWindow { get; set; }
        public Windows.DeliveryDeskWindows.DeliveryDeskLogin deliveryDeskLoginWindow { get; set; }
        public Windows.LibraryStorageWindows.StorageLogin storageLoginWindow { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CheckIfOverdue();
        }

        public void CheckIfOverdue()
        {
            const string GetCDIQuery = "select * from[Coursework_2018].[dbo].[CustomerDocumentInteraction]";
            const string SetStatusOverdueQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set Status = 'Overdue' where CDInteractionID = @id";
            using (var db = new TheContext())
            {
                var customerDocumentInteractions = db.CustomerDocumentInteractions.SqlQuery(GetCDIQuery).ToList();
                foreach (var item in customerDocumentInteractions)
                {
                    if (item.DueDate < DateTime.Now)
                        db.Database.ExecuteSqlCommand(SetStatusOverdueQuery, new SqlParameter("@id", item.CDInteractionID));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tag = button.Tag as string;
            var tagInt = Convert.ToInt32(tag);
            switch (tagInt)
            {
                case 0:
                    //Нажата кнопка "Войти в модуль "Читатель"
                    customerLoginWindow = new Windows.CustomerWindows.CustomerLogin();
                    customerLoginWindow.Show();
                    break;
                case 1:
                    //Нажата кнопка "Войти в модуль "Абонемент"
                    deliveryDeskLoginWindow = new Windows.DeliveryDeskWindows.DeliveryDeskLogin();
                    deliveryDeskLoginWindow.Show();
                    break;
                case 2:
                    //Нажата кнопка "Войти в модуль "Хранилище"
                    storageLoginWindow = new Windows.LibraryStorageWindows.StorageLogin();
                    storageLoginWindow.Show();
                    break;
                case 3:
                    //Нажата кнопка "Выйти"
                    Close();
                    break;
            }
        }
    }
}
