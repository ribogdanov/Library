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
using System.Windows.Shapes;

namespace Library.Windows.DeliveryDeskWindows
{
    /// <summary>
    /// Логика взаимодействия для CustomerRegistrationWindow.xaml
    /// </summary>
    public partial class CustomerRegistrationWindow : Window
    {
        private const string CreateUserSqlQuery = "insert into[Coursework_2018].[dbo].[Customer] (Name, Surname, Patronymic, Password) values(@name, @surname, @patronymic, @password)";
        private const string GetUserID = "select @@identity as ID";
        private const string CreateUserGetIdQuery = "insert into[Coursework_2018].[dbo].[Customer] (Name, Surname, Patronymic, Password) values(@name, @surname, @patronymic, @password); select @@identity as ID;";
        private List<QueryResultClasses.CustomerRegistration_GetUserID> UserIDObject;
        decimal UserID;
        public CustomerRegistrationWindow()
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
                    //Нажата кнопка "Зарегистрировать"
                    if (NameTextBox.Text != "" && PasswordTextBox.Text != "")
                    {
                        using (TheContext db = new TheContext())
                        {
                            UserIDObject = db.Database.SqlQuery<QueryResultClasses.CustomerRegistration_GetUserID>(CreateUserGetIdQuery, new SqlParameter("@name", NameTextBox.Text), new SqlParameter("@surname", SurnameTextBox.Text), new SqlParameter("@patronymic", PatronymicTextBox.Text), new SqlParameter("@password", PasswordTextBox.Text)).ToList();
                            UserID = UserIDObject[0].ID;
                        }
                        MessageBoxResult result = MessageBox.Show($"Создана запись пользователя {NameTextBox.Text} {PatronymicTextBox.Text} {SurnameTextBox.Text}\n ID={UserID}");
                    }
                    break;
                case 1:
                    //Нажата кнопка "Выйти"
                    Close();
                    break;
            }
        }
    }
}
