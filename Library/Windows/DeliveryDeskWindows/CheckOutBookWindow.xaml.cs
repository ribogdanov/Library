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
    /// Логика взаимодействия для CheckOutBookWindow.xaml
    /// </summary>
    public partial class CheckOutBookWindow : Window
    {
        public CheckOutBookWindow()
        {
            InitializeComponent();
            AllBooks();
        }

        public void AllBooks()
        {
            //Метод выводит в BooksDataGrid все имеющиеся в БД книги с авторами, авторы каждой книги представлены в одной строке.
            #region GetBooksWithAuthorsSqlQuery
            const string GetBooksWithAuthorsSqlQuery = @"
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Surname, ', ') as AuthorSurnames,
	string_agg (Author.Name, ', ') as AuthorNames,
	string_agg (Author.Patronymic, ', ') as AuthorPatronymics
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment
";
            #endregion
            using (TheContext db = new TheContext())
            {
                List<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid> AllBooks = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(GetBooksWithAuthorsSqlQuery).ToList();
                BooksDataGrid.ItemsSource = AllBooks;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid> books;

            var button = sender as Button;
            var tag = button.Tag as string;
            var tagInt = Convert.ToInt32(tag);
            switch (tagInt)
            {
                case 0:
                    //Нажата кнопка "Искать книгу по автору"

                    //Поиск по имени, фамилии, отчеству:
                    if (NameTextBox.Text != "" && SurnameTextBox.Text != "" && PatronymicTextBox.Text != "")
                    {
                        #region GetBookByAuthorNameSurnamePatronymicSqlQuery
                        const string GetBookByAuthorNameSurnamePatronymicSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.Authors like @line
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorNameSurnamePatronymicSqlQuery,
                                new SqlParameter("@line", $"%{SurnameTextBox.Text} {NameTextBox.Text} {PatronymicTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по фамилии и отчеству:
                    if (NameTextBox.Text == "" && SurnameTextBox.Text != "" && PatronymicTextBox.Text != "")
                    {
                        #region GetBookByAuthorSurnamePatronymicSqlQuery
                        const string GetBookByAuthorSurnamePatronymicSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Surname, ', ') as AuthorSurnames,
	string_agg (Author.Patronymic, ', ') as AuthorPatronymics
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorSurnames like @surname and x.AuthorPatronymics like @patronymic
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorSurnamePatronymicSqlQuery,
                                new SqlParameter("@surname", $"%{SurnameTextBox.Text}%"),
                                new SqlParameter("@patronymic", $"%{PatronymicTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по имени и отчеству:
                    if (NameTextBox.Text != "" && SurnameTextBox.Text == "" && PatronymicTextBox.Text != "")
                    {
                        #region GetBookByAuthorNamePatronymicSqlQuery
                        const string GetBookByAuthorNamePatronymicSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Name, ', ') as AuthorNames,
	string_agg (Author.Patronymic, ', ') as AuthorPatronymics
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorNames like @name and x.AuthorPatronymics like @patronymic
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorNamePatronymicSqlQuery,
                                new SqlParameter("@name", $"%{NameTextBox.Text}%"),
                                new SqlParameter("@patronymic", $"%{PatronymicTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по имени и фамилии:
                    if (NameTextBox.Text != "" && SurnameTextBox.Text != "" && PatronymicTextBox.Text == "")
                    {
                        #region GetBookByAuthorNameSurnameSqlQuery
                        const string GetBookByAuthorNameSurnameSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Surname, ', ') as AuthorSurnames,
	string_agg (Author.Name, ', ') as AuthorNames
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorNames like @name and x.AuthorSurnames like @surname
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorNameSurnameSqlQuery,
                                new SqlParameter("@name", $"%{NameTextBox.Text}%"),
                                new SqlParameter("@surname", $"%{SurnameTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по имени:
                    if (NameTextBox.Text != "" && SurnameTextBox.Text == "" && PatronymicTextBox.Text == "")
                    {
                        #region GetBookByAuthorNameSqlQuery
                        const string GetBookByAuthorNameSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Name, ', ') as AuthorNames
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorNames like @name
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorNameSqlQuery,
                                new SqlParameter("@name", $"%{NameTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по фамилии:
                    if (NameTextBox.Text == "" && SurnameTextBox.Text != "" && PatronymicTextBox.Text == "")
                    {
                        #region GetBookByAuthorSurnameSqlQuery
                        const string GetBookByAuthorSurnameSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Surname, ', ') as AuthorSurnames
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorSurnames like @surname
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorSurnameSqlQuery,
                                new SqlParameter("@surname", $"%{SurnameTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Поиск по отчеству
                    if (NameTextBox.Text == "" && SurnameTextBox.Text == "" && PatronymicTextBox.Text != "")
                    {
                        #region GetBookByAuthorPatronymicSqlQuery
                        const string GetBookByAuthorPatronymicSqlQuery = @"
select * from (
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	string_agg (Author.Patronymic, ', ') as AuthorPatronymics
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment) as x
where x.AuthorPatronymics like @patronymic
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByAuthorPatronymicSqlQuery,
                                new SqlParameter("@patronymic", $"%{PatronymicTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }

                    //Попытка поиска при отсутствии имени, фамилии, отчества
                    if (NameTextBox.Text == "" && SurnameTextBox.Text == "" && PatronymicTextBox.Text == "")
                    {
                        //Вывод окна с сообщением и вывод в DataGrid всех книг из БД
                        MessageBoxResult result = MessageBox.Show("Введите данные для поиска.");
                        AllBooks();
                    }

                    BookItemsDataGrid.ItemsSource = null;
                    break;
                case 1:
                    //Нажата кнопка "Искать книгу по названию"
                    if (TitleTextBox.Text != "")
                    {
                        #region GetBookByTitleSqlQuery
                        const string GetBookByTitleSqlQuery = @"
select 
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors
from [Coursework_2018].[dbo].[Book]
inner join [Coursework_2018].[dbo].[Author_Book]
on [Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join [Coursework_2018].[dbo].[Author]
on [Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
where Title like @title
group by
    Book.BookID,
	Book.Title,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment
";
                        #endregion
                        using (TheContext db = new TheContext())
                        {
                            books = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid>(
                                GetBookByTitleSqlQuery,
                                new SqlParameter("@title", $"%{TitleTextBox.Text}%")
                            ).ToList();
                            BooksDataGrid.ItemsSource = books;
                        }
                    }
                    else
                    {
                        //Вывод окна с сообщением и вывод в DataGrid всех книг из БД
                        MessageBoxResult result = MessageBox.Show("Введите данные для поиска.");
                        AllBooks();
                    }
                    BookItemsDataGrid.ItemsSource = null;
                    break;
                case 2:
                    //Нажата кнопка "Выдать выбранный экземпляр книги читателю"
                    if (CustomerIDTextBox.Text != "")
                    {
                        if (BookItemsDataGrid.SelectedItem != null)
                        {
                            using (var db = new TheContext())
                            {
                                SearchWindows.QueryResultClasses.BookSearchWindow_BookItemsDataGrid currentBookItem = (SearchWindows.QueryResultClasses.BookSearchWindow_BookItemsDataGrid)BookItemsDataGrid.SelectedItem;

                                if (currentBookItem.Status == "Available")
                                {
                                    const string CreateCustomerDocumentInteractionQuery = @"insert into [Coursework_2018].[dbo].[CustomerDocumentInteraction] (CustomerID, DocumentItemID, CheckedOutDate, DueDate, IfRenewed, Status) values (@customerID, @documentItemID, getdate(), dateadd(month, 1, getdate()), 0, 'Taken')";
                                    const string SetDocumentStatusUnavailableQuery = "update[Coursework_2018].[dbo].[DocumentItem] set Status = 'Unavailable' where DocumentItemID = @documentItemId";

                                    db.Database.ExecuteSqlCommand(CreateCustomerDocumentInteractionQuery, new SqlParameter("@customerID", CustomerIDTextBox.Text), new SqlParameter("@documentItemID", currentBookItem.DocumentItemID));
                                    db.Database.ExecuteSqlCommand(SetDocumentStatusUnavailableQuery, new SqlParameter("@documentItemID", currentBookItem.DocumentItemID));

                                    //Обновление данных в BookItemsDataGrid:
                                    BookItemsByBook();
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Книга используется читателем библиотеки, выдача невозможна.");
                                }
                            }
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Выберите экземпляр книги.");
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Введите ID читателя.");
                    }
                    break;
                case 3:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }

        private void BooksDataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "BookID":
                    e.Cancel = true;
                    break;
                case "Title":
                    e.Column.Header = "Название книги";
                    break;
                case "StoredItemsNumber":
                    e.Column.Header = "Количество экземпляров в наличии";
                    break;
                case "Language":
                    e.Column.Header = "Язык";
                    break;
                case "Comment":
                    e.Column.Header = "Комментарий";
                    break;
                case "Authors":
                    e.Column.Header = "Автор(ы)";
                    break;
                case "AuthorSurnames":
                    e.Cancel = true;
                    break;
                case "AuthorNames":
                    e.Cancel = true;
                    break;
                case "AuthorPatronymics":
                    e.Cancel = true;
                    break;
            }
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookItemsByBook();
        }

        public void BookItemsByBook()
        {
            #region GetBookItemsSqlQuery 
            const string GetBookItemsSqlQuery = @"
select 
	BookItem.DocumentItemID, 
	BookItem.BookID,
	BookItem.ISBN,
	BookItem.PublishDate,
	BookItem.Publisher,
	DocumentItem.Status,
	DocumentItem.ReceivedDate, 
	DocumentItem.WrittenOffDate,
	DocumentItem.Comment
from [Coursework_2018].[dbo].[BookItem]
inner join [Coursework_2018].[dbo].[DocumentItem]
on [Coursework_2018].[dbo].[BookItem].DocumentItemID=[Coursework_2018].[dbo].[DocumentItem].DocumentItemID
where BookID=@id
";
            #endregion
            if (BooksDataGrid.SelectedItem != null)
            {
                SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid currentBook = (SearchWindows.QueryResultClasses.BookSearchWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                using (TheContext db = new TheContext())
                {
                    List<SearchWindows.QueryResultClasses.BookSearchWindow_BookItemsDataGrid> bookItems = db.Database.SqlQuery<SearchWindows.QueryResultClasses.BookSearchWindow_BookItemsDataGrid>(GetBookItemsSqlQuery, new SqlParameter("@id", currentBook.BookID)).ToList();
                    BookItemsDataGrid.ItemsSource = bookItems;
                }
            }
        }

        private void BookItemsDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "DocumentItemID":
                    e.Column.Header = "ID экземпляра";
                    break;
                case "BookID":
                    e.Cancel = true;
                    break;
                case "ISBN":
                    break;
                case "PublishDate":
                    e.Column.Header = "Дата печати";
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "Publisher":
                    e.Column.Header = "Издатель";
                    break;
                case "Status":
                    e.Column.Header = "Статус";
                    break;
                case "ReceivedDate":
                    e.Column.Header = "Дата поступления в абонемент";
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "WrittenOffDate":
                    e.Column.Header = "Дата списания из абонемента";
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "Comment":
                    e.Column.Header = "Комментарий";
                    break;
            }
        }
    }
}
