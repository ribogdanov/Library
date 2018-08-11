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

namespace Library.Windows.CustomerWindows
{
    /// <summary>
    /// Логика взаимодействия для CustomerOverview.xaml
    /// </summary>
    public partial class CustomerOverview : Window
    {
        public SearchWindows.MainSearchWindow SearchWindow { get; set; }
        private List<QueryResultClasses.CustomerOverviewWindow_BooksDataGrid> BooksDataGridQueryResult { get; set; }
        #region BooksDataGridSqlQuery
        private const string BooksDataGridSqlQuery = @"
        select
    Book.Title,
	STRING_AGG(Concat(Author.Surname, ' ', Author.Name), ', ') as Authors,
	BookItem.ISBN,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID
from[Coursework_2018].[dbo].[Book]
inner join[Coursework_2018].[dbo].[Author_Book]
        on[Coursework_2018].[dbo].[Book].BookID=[Coursework_2018].[dbo].[Author_Book].BookID
inner join[Coursework_2018].[dbo].[Author]
        on[Coursework_2018].[dbo].[Author_Book].AuthorID=[Coursework_2018].[dbo].[Author].AuthorID
inner join[Coursework_2018].[dbo].[BookItem]
        on[Coursework_2018].[dbo].[BookItem].BookID=[Coursework_2018].[dbo].[Book].BookID
inner join[Coursework_2018].[dbo].[DocumentItem]
        on[Coursework_2018].[dbo].[DocumentItem].DocumentItemID=[Coursework_2018].[dbo].[BookItem].DocumentItemID
inner join[Coursework_2018].[dbo].[CustomerDocumentInteraction]
        on[Coursework_2018].[dbo].[CustomerDocumentInteraction].DocumentItemID=[Coursework_2018].[dbo].[DocumentItem].DocumentItemID
where CustomerDocumentInteraction.CustomerID=@id
Group by
    Book.BookID,
	Book.Title,  
	BookItem.ISBN,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID
";
    #endregion
        private List<QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid> PeriodicalsDataGridQueryResult { get; set; }
        #region PeriodicalsDataGridSqlQuery
        private const string PeriodicalsDataGridSqlQuery = @"
select 
	Periodical.PeriodicalName,
	PeriodicalIssue.IssueNumber,
	PeriodicalIssue.IssuePeriod,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID
from [Coursework_2018].[dbo].[Periodical]
inner join [Coursework_2018].[dbo].[PeriodicalIssue]
        on [Coursework_2018].[dbo].[Periodical].PeriodicalID=[Coursework_2018].[dbo].[PeriodicalIssue].PeriodicalID
inner join [Coursework_2018].[dbo].[PeriodicalItem]
	    on [Coursework_2018].[dbo].[PeriodicalIssue].PeriodicalIssueID=[Coursework_2018].[dbo].[PeriodicalItem].PeriodicalIssueID
inner join [Coursework_2018].[dbo].[DocumentItem]
    	on [Coursework_2018].[dbo].[PeriodicalItem].DocumentItemID=[Coursework_2018].[dbo].[DocumentItem].DocumentItemID
inner join [Coursework_2018].[dbo].[CustomerDocumentInteraction]
	    on [Coursework_2018].[dbo].[DocumentItem].DocumentItemID=[Coursework_2018].[dbo].[CustomerDocumentInteraction].DocumentItemID
where CustomerDocumentInteraction.CustomerID=@id
";
        #endregion
        private int UserID { get; set; }
        public CustomerOverview(Customer user)
        {
            InitializeComponent();
            GreetingTextBox.Text = "Здравствуйте, "+user.Name + " "+user.Patronymic;
            IdTextBox.Text = "Ваш ID: "+user.CustomerID.ToString();
            UserID = user.CustomerID;
            using (TheContext db = new TheContext())
            {
                BooksDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.CustomerOverviewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;              
                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            const string RenewDocumentQuery = @"update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set DueDate=@dueDate, IfRenewed=1 where CDInteractionID=@currentCDIID";
            const string AddRenewalDateQuery = @"insert [Coursework_2018].[dbo].[RenewalDates] (CDInteractionID, RenewalDate) values (@currentCDIID, getdate())";
            var button = sender as Button;
            var tag = button.Tag as string;
            var tagInt = Convert.ToInt32(tag);
            switch (tagInt)
            {
                case 0:
                    //Нажата кнопка "Продлить книгу"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.CustomerOverviewWindow_BooksDataGrid currentBook = (QueryResultClasses.CustomerOverviewWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        if (currentBook.Status == "Taken")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentBook.DueDate + month), new SqlParameter("currentCDIID", currentBook.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentBook.CDInteractionID));

                                //Обновление данных в DataGrid:
                                BooksDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.CustomerOverviewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                BooksDataGrid.ItemsSource = null;
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;
                            }
                        }
                    }
                    break;
                case 1:
                    //Нажата кнопка "Продлить периодическое издание"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid currentPeriodical = (QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid)PeriodicalsDataGrid.SelectedItem;
                        if (currentPeriodical.Status == "Taken")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentPeriodical.DueDate + month), new SqlParameter("currentCDIID", currentPeriodical.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentPeriodical.CDInteractionID));

                                //Обновление данных в DataGrid:
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                PeriodicalsDataGrid.ItemsSource = null;
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
                            }
                        }
                    }
                    break;
                case 2:
                    //Нажата кнопка "Искать документ в базе данных"
                    SearchWindow = new SearchWindows.MainSearchWindow();
                    SearchWindow.Show();
                    break;
                case 3:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение формата представления даты:
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
            //Изменение заголовков столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                //Уникальные колонки книг:
                case "Title":
                    e.Column.Header = "Название книги";
                    break;
                case "Authors":
                    e.Column.Header = "Автор(ы)";
                    break;
                //Уникальные колонки периодических изданий:
                case "PeriodicalName":
                    e.Column.Header = "Название периодики";
                    break;
                case "IssueNumber":
                    e.Column.Header = "Номер выпуска";
                    break;
                case "IssuePeriod":
                    e.Column.Header = "Период выпуска";
                    break;
                //Колонки, общие для книг и периодики:
                case "Status":
                    e.Column.Header = "Статус";
                    break;
                case "DueDate":
                    e.Column.Header = "Вернуть до";
                    break;
                case "CheckedOutDate":
                    e.Column.Header = "Дата выдачи";
                    break;
                case "FactReturnDate":
                    e.Column.Header = "Дата возврата";
                    break;
                case "IfRenewed":
                    e.Column.Header = "Документ продлен?";
                    break;
                case "CDInteractionID":
                    e.Cancel = true;
                    break;
            }
        }
    }
}
