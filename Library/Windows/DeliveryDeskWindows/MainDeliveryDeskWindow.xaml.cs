using System;
using System.Collections.Generic;
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

namespace Library.Windows.DeliveryDeskWindows
{
    /// <summary>
    /// Логика взаимодействия для MainDeliveryDeskWindow.xaml
    /// </summary>
    public partial class MainDeliveryDeskWindow : Window
    {
        public CheckOutBookWindow checkOutBookWindow { get; set; }
        public CheckOutPeriodicalWindow checkOutPeriodicalWindow { get; set; }
        public ReturnRenewWindow returnRenewWindow { get; set; }
        public SearchWindows.MainSearchWindow mainSearchWindow { get; set; }
        public CustomerRegistrationWindow customerRegistrationWindow { get; set; }
        public MainDeliveryDeskWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tag = button.Tag as string;
            var tagInt = Convert.ToInt32(tag);
            switch (tagInt)
            {
                case 0:
                    //Нажата кнопка "Выдать книгу"
                    checkOutBookWindow = new CheckOutBookWindow();
                    checkOutBookWindow.Show();
                    break;
                case 1:
                    //Нажата кнопка "Выдать еприодическое издание"
                    checkOutPeriodicalWindow = new CheckOutPeriodicalWindow();
                    checkOutPeriodicalWindow.Show();
                    break;
                case 2:
                    //Нажата кнопка "Принять/продлить документ"
                    returnRenewWindow = new ReturnRenewWindow();
                    returnRenewWindow.Show();
                    break;
                case 3:
                    //Нажата кнопка "Найти документ в базе данных"
                    mainSearchWindow = new SearchWindows.MainSearchWindow();
                    mainSearchWindow.Show();
                    break;
                case 4:
                    //Нажата кнопка "Зарегистрировать читателя"
                    customerRegistrationWindow = new CustomerRegistrationWindow();
                    customerRegistrationWindow.Show();
                    break;
                case 5:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }
    }
}
