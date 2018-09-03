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
    /// Логика взаимодействия для DeliveryDeskLogin.xaml
    /// </summary>
    public partial class DeliveryDeskLogin : Window
    {
        public MainDeliveryDeskWindow mainDeliveryDeskWindow { get; set; }
        public DeliveryDeskLogin()
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
                    //Нажата кнопка "Войти в абонемент"
                    if (PasswordTextBox.Text != "")
                        if (PasswordTextBox.Text == "12345")
                        {
                            mainDeliveryDeskWindow = new MainDeliveryDeskWindow();
                            mainDeliveryDeskWindow.Show();
                            Close();
                        }
                        else
                            MessageBox.Show("Введенный пароль неверен.");
                    else
                        MessageBox.Show("Введите пароль.");
                    break;
                case 1:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }
    }
}
