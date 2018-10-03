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

namespace Library.Windows.LibraryStorageWindows
{
    /// <summary>
    /// Логика взаимодействия для StorageLogin.xaml
    /// </summary>
    public partial class StorageLogin : Window
    {
        public MainStorageWindow mainStorageWindow { get; set; }
        public StorageLogin()
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
                    //Нажата кнопка "Войти в хранилище"
                    if (PasswordTextBox.Text != "")
                        if (PasswordTextBox.Text == "12345")
                        {
                            mainStorageWindow = new MainStorageWindow();
                            Close();
                            mainStorageWindow.ShowDialog();
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
