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

namespace Library.Windows.LibraryStorageWindows
{
    /// <summary>
    /// Логика взаимодействия для AddAuthorWindow.xaml
    /// </summary>
    public partial class AddAuthorWindow : Window
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public AddAuthorWindow()
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
                    //Нажата кнопка "Назад"
                    Close();
                    break;
                case 1:
                    //Нажата кнопка "Добавить автора"
                    const string CreateAuthorQuery = "insert into [Coursework_2018].[dbo].[Author] (Name, Surname, Comment, Patronymic) values(@name, @surname, @comment, @patronymic)";
                    if (NameTextBox.Text != "" && SurnameTextBox.Text != "")
                    {
                        using (var db = new TheContext())
                        {
                            db.Database.ExecuteSqlCommand(CreateAuthorQuery, 
                                new SqlParameter("@name", NameTextBox.Text), 
                                new SqlParameter("@surname", SurnameTextBox.Text), 
                                new SqlParameter("@comment", CommentTextBox.Text), 
                                new SqlParameter("@patronymic", PatronymicTextBox.Text));
                        }
                        DataChanged?.Invoke(this, new EventArgs());

                        //Очистка заполняемых полей:
                        NameTextBox.Text = "";
                        SurnameTextBox.Text = "";
                        CommentTextBox.Text = "";
                        PatronymicTextBox.Text = "";

                        MessageBox.Show("Автор добавлен.");
                    }
                    else
                        MessageBox.Show("Необходимо заполнить поля 'Имя' и 'Фамилия'");
                    break;
            }
        }
    }
}
