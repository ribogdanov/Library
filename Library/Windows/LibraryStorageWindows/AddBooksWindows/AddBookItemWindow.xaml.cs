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
    /// Логика взаимодействия для AddBookItemWindow.xaml
    /// </summary>
    public partial class AddBookItemWindow : Window
    {
        AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid CurrentBook;
        private const string InsertDocumentItemGetIDQuery = "insert into [Coursework_2018].[dbo].[DocumentItem] (Status, ReceivedDate, Comment, Type) values('Available', getdate(), @comment,'Book') select @@identity as ID";
        private const string InsertBookItemQuery = "insert into [Coursework_2018].[dbo].[BookItem] (DocumentItemID, BookID, ISBN, PublishDate, Publisher) values(@documentItemID, @bookID, @isbn, @publishDate, @publisher)";
        private const string UpdateBookQuery = "update [Coursework_2018].[dbo].[Book] set StoredItemsNumber = StoredItemsNumber+1 where BookID = @bookID";
        private List<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID> DocumentItemIDObject;
        decimal DocumentItemID;
        
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;


        public AddBookItemWindow(AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid currentBook)
        {
            InitializeComponent();
            CurrentBook = currentBook;
            HeaderTextBox.Text = $"Добавить экземпляр книги \"{CurrentBook.Title}\"";
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
                    //Нажата кнопка "Добавить экземпляр книги в базу данных"
                    if (ISBNTextBox.Text != "" && PublishDateDatePicker.SelectedDate != null && PublisherTextBox.Text != "")
                    {
                        if (PublishDateDatePicker.SelectedDate > DateTime.Now)
                        {
                            MessageBox.Show("Укажите дату публикации не больше сегодняшнего дня");
                        }
                        else
                        {
                            using (TheContext db = new TheContext())
                            {
                                DocumentItemIDObject = db.Database.SqlQuery<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID>(InsertDocumentItemGetIDQuery, new SqlParameter("@comment", CommentTextBox.Text)).ToList();
                                DocumentItemID = DocumentItemIDObject[0].ID;
                                db.Database.ExecuteSqlCommand(InsertBookItemQuery,
                                    new SqlParameter("@documentItemID", DocumentItemID),
                                    new SqlParameter("@bookID", CurrentBook.BookID),
                                    new SqlParameter("@isbn", ISBNTextBox.Text),
                                    new SqlParameter("@publishDate", PublishDateDatePicker.SelectedDate),
                                    new SqlParameter("@publisher", PublisherTextBox.Text));
                                db.Database.ExecuteSqlCommand(UpdateBookQuery, new SqlParameter("@bookID", CurrentBook.BookID));
                            }
                            //Обновление данных в окне-родителе:
                            DataChanged?.Invoke(this, new EventArgs());

                            //Очистка заполняемых полей:
                            ISBNTextBox.Text = "";
                            PublishDateDatePicker.SelectedDate = null;
                            PublisherTextBox.Text = "";
                            CommentTextBox.Text = "";

                            MessageBox.Show($"Добавлен экземпляр книги.\nID={DocumentItemID}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Все поля кроме \"Комментарий\" необходимо заполнить");
                    }
                    break;
            }
        }

    }
}
