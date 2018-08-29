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
    /// Логика взаимодействия для AddBookMainWindow.xaml
    /// </summary>
    public partial class AddBookMainWindow : Window
    {
        AddBookWindow addBookWindow;
        AddBookItemWindow addBookItemWindow;
        #region GetBooksNAuthorsQuery 
        const string GetBooksNAuthorsQuery = @"
select 
    Book.BookID,
	Book.Title,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	Book.StoredItemsNumber,
	Book.Language,
	Book.Comment
from [Coursework_2018].[dbo].[Book]
inner join[Coursework_2018].[dbo].[Author_Book]
        on[Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join[Coursework_2018].[dbo].[Author]
        on[Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
		group by 
		Title,
		StoredItemsNumber,
		Language,
		Book.Comment,
        Book.BookID
";
        #endregion
        public AddBookMainWindow()
        {
            InitializeComponent();
            UpdateDataGrid();
        }

        public void UpdateDataGrid()
        {
            using (var db = new TheContext())
            {
                List<AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid> books = db.Database.SqlQuery<AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid>(GetBooksNAuthorsQuery).ToList();
                BooksDataGrid.ItemsSource = books;
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
                    //Нажата кнопка "Добавить экземпляр выбранной книги"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid currentBook = (AddBooksWindows.QueryResultClasses.AddBookMainWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        addBookItemWindow = new AddBookItemWindow(currentBook);
                        addBookItemWindow.DataChanged += AddBookItemWindow_DataChanged;
                        addBookItemWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Выберите экземпляр книги.");
                    }
                    break;
                case 1:
                    //Нажата кнопка "Добавить новую книгу в базу данных"
                    addBookWindow = new AddBookWindow();
                    addBookWindow.DataChanged += AddBookWindow_DataChanged;
                    addBookWindow.Show();
                    break;
                case 2:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }

        private void BooksDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "Title":
                    e.Column.Header = "Название книги";
                    break;
                case "Authors":
                    e.Column.Header = "Автор(ы)";
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
                case "BookID":
                    e.Cancel = true;
                    break;
            }
        }

        private void AddBookItemWindow_DataChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        private void AddBookWindow_DataChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
    }
}
