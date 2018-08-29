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
    /// Логика взаимодействия для AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        AddAuthorWindow addAuthorWindow;
        List<AddBooksWindows.QueryResultClasses.AddBookWindow_Author> selectedAuthors;
        List<AddBooksWindows.QueryResultClasses.AddBookWindow_Author> allAuthors;

        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        public AddBookWindow()
        {
            InitializeComponent();
            UpdateAllAuthorsDataGrid();
        }

        private void UpdateAllAuthorsDataGrid()
        {
            using (TheContext db = new TheContext())
            {
                allAuthors = db.Database.SqlQuery<AddBooksWindows.QueryResultClasses.AddBookWindow_Author>("select * from [Coursework_2018].[dbo].[Author]").ToList();
                AllAuthorsDataGrid.ItemsSource = allAuthors;
            }
            selectedAuthors = new List<AddBooksWindows.QueryResultClasses.AddBookWindow_Author>();
        }
        
        private void AddAuthorWindow_DataChanged(object sender, EventArgs e)
        {
            UpdateAllAuthorsDataGrid();
            AuthorsOfTheBookDataGrid.ItemsSource = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tag = button.Tag as string;
            var tagInt = Convert.ToInt32(tag);
            switch (tagInt)
            {
                case 0:
                    //Нажата кнопка "Отметить выбранного автора как автора этой книги"
                    if (AllAuthorsDataGrid.SelectedItem != null)
                    {
                        AddBooksWindows.QueryResultClasses.AddBookWindow_Author allAuthorsDataGridSelectedItem = (AddBooksWindows.QueryResultClasses.AddBookWindow_Author)AllAuthorsDataGrid.SelectedItem;

                        selectedAuthors.Add(allAuthorsDataGridSelectedItem);
                        AuthorsOfTheBookDataGrid.ItemsSource = null;
                        AuthorsOfTheBookDataGrid.ItemsSource = selectedAuthors;

                        allAuthors.Remove(allAuthorsDataGridSelectedItem);
                        AllAuthorsDataGrid.ItemsSource = null;
                        AllAuthorsDataGrid.ItemsSource = allAuthors;
                    }
                    else
                        MessageBox.Show("Выберите автора в таблице слева.");
                    break;
                case 1:
                    //Нажата кнопка "Убрать с выбранного автора отметку аб авторстве этой книги"
                    if (AuthorsOfTheBookDataGrid.SelectedItem != null)
                    {
                        AddBooksWindows.QueryResultClasses.AddBookWindow_Author authorsOfTheBookDataGridSelectedItem = (AddBooksWindows.QueryResultClasses.AddBookWindow_Author)AuthorsOfTheBookDataGrid.SelectedItem;

                        selectedAuthors.Remove(authorsOfTheBookDataGridSelectedItem);
                        AuthorsOfTheBookDataGrid.ItemsSource = null;
                        AuthorsOfTheBookDataGrid.ItemsSource = selectedAuthors;

                        allAuthors.Add(authorsOfTheBookDataGridSelectedItem);
                        AllAuthorsDataGrid.ItemsSource = null;
                        AllAuthorsDataGrid.ItemsSource = allAuthors;
                    }
                    else
                        MessageBox.Show("Выберите автора в таблице справа.");
                    break;
                case 2:
                    //Нажата кнопка "Добавить нового автора в базу данных"
                    addAuthorWindow = new AddAuthorWindow();
                    addAuthorWindow.DataChanged += AddAuthorWindow_DataChanged;
                    addAuthorWindow.Show();
                    break;
                case 3:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
                case 4:
                    //Нажата кнопка "Добавить книгу в базу данных"
                    if (TitleTextBox.Text != "" && LanguageTextBox.Text != "")
                    {
                        if (selectedAuthors.Count > 0)
                        {
                            const string AddBookGetIDQuery = "insert into [Coursework_2018].[dbo].[Book] (Title, StoredItemsNumber, Language, Comment) values (@title, 0, @language, @comment) select @@identity as ID";
                            const string AddAuthorBookConnection = "insert into [Coursework_2018].[dbo].[Author_Book] (BookID, AuthorID) values(@bookID, @authorID)";

                            using (var db = new TheContext())
                            {
                                //Класс, используемый в следующей строке, создавался для использования в другом месте, но подходит и сюда.
                                List<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID> bookIDObject = 
                                    db.Database.SqlQuery<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID>(AddBookGetIDQuery, 
                                    new SqlParameter("@title", TitleTextBox.Text), 
                                    new SqlParameter("@language", LanguageTextBox.Text), 
                                    new SqlParameter("@comment", CommentTextBox.Text)).ToList();
                                decimal bookID = bookIDObject[0].ID;

                                foreach (var item in selectedAuthors)
                                {
                                    db.Database.ExecuteSqlCommand(AddAuthorBookConnection, new SqlParameter("bookID", bookID), new SqlParameter("authorID", item.AuthorID));
                                }
                            }
                            DataChanged?.Invoke(this, new EventArgs());
                            MessageBox.Show("Книга добавлена");
                        }
                        else
                            MessageBox.Show("Выберите минимум одного автора");
                    }
                    else
                        MessageBox.Show("Необходимо заполнить поля 'Название' и 'Язык'");
                        break;
            }
        }

        private void AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "AuthorID":
                    e.Cancel=true;
                    break;
                case "Name":
                    e.Column.Header = "Имя";
                    break;
                case "Surname":
                    e.Column.Header = "Фамилия";
                    break;

                case "Patronymic":
                    e.Column.Header = "Отчество";
                    break;
                case "Comment":
                    e.Column.Header = "Комментарий";
                    break;
            }
        }
    }
}
