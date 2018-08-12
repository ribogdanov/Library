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
    /// Логика взаимодействия для ReturnRenewWindow.xaml
    /// </summary>
    public partial class ReturnRenewWindow : Window
    {
        //Для получения результатов sql-запросов использую классы, определенные в пространстве имен CustomerWindows. 
        //Запросы, для которых изначально создавались эти классы, идентичны используемым здесь.
        private List<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid> BooksDataGridQueryResult { get; set; }
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
        private List<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid> PeriodicalsDataGridQueryResult { get; set; }
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

        public int UserID { get; set; }

        private const string GetCustomerByIDQuery = "select * from[Coursework_2018].[dbo].[Customer] where CustomerID = @id";

        const string RenewDocumentQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set DueDate=@dueDate, IfRenewed=1 where CDInteractionID=@currentCDIID";
        const string AddRenewalDateQuery = "insert [Coursework_2018].[dbo].[RenewalDates] (CDInteractionID, RenewalDate) values (@currentCDIID, getdate())";
        const string ReturnDocumentQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set FactReturnDate=@factReturnDate, Status='Returned' where CDInteractionID=@currentCDIID";

        public ReturnRenewWindow()
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
                    //Нажата кнопка "Показать документы для читателя"
                    if (IDTextBox.Text!="")
                    {
                        UserID = Convert.ToInt32(IDTextBox.Text);
                        using (var db = new TheContext())
                        {
                            //Вывод ФИО клиента, ID которого введен:
                            var customer = db.Customers.SqlQuery(GetCustomerByIDQuery, new SqlParameter("@id", UserID)).ToList();
                            CustomerNameTextBox.Text = $"Читатель: {customer[0].Name} {customer[0].Patronymic} {customer[0].Surname}";
                            //Вывод книг и периодики этого клиента:
                            BooksDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                            BooksDataGrid.ItemsSource = BooksDataGridQueryResult;
                            PeriodicalsDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                            PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
                        }
                    }
                    break;
                case 1:
                    //Нажата кнопка "Принять книгу"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid currentBook = (CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        if (currentBook.Status == "Taken" || currentBook.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                db.Database.ExecuteSqlCommand(ReturnDocumentQuery, new SqlParameter("@factReturnDate", DateTime.Now), new SqlParameter("currentCDIID", currentBook.CDInteractionID));
                                
                                //Обновление данных в DataGrid:
                                BooksDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                BooksDataGrid.ItemsSource = null;
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;
                            }
                        }
                    }
                        break;
                case 2:
                    //Нажата кнопка "Продлить книгу"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid currentBook = (CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        if (currentBook.Status == "Taken" || currentBook.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentBook.DueDate + month), new SqlParameter("currentCDIID", currentBook.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentBook.CDInteractionID));

                                //Обновление данных в DataGrid:
                                BooksDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                BooksDataGrid.ItemsSource = null;
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;
                            }
                        }
                    }
                    break;
                case 3:
                    //Нажата кнопка "Принять периодическое издание"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid currentPeriodical = (CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid)PeriodicalsDataGrid.SelectedItem;
                        if (currentPeriodical.Status == "Taken" || currentPeriodical.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                db.Database.ExecuteSqlCommand(ReturnDocumentQuery, new SqlParameter("@factReturnDate", DateTime.Now), new SqlParameter("currentCDIID", currentPeriodical.CDInteractionID));

                                //Обновление данных в DataGrid:
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                PeriodicalsDataGrid.ItemsSource = null;
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
                            }
                        }
                    }
                    break;
                case 4:
                    //Нажата кнопка "Продлить периодическое издание"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid currentPeriodical = (CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid)PeriodicalsDataGrid.SelectedItem;
                        if (currentPeriodical.Status == "Taken" || currentPeriodical.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentPeriodical.DueDate + month), new SqlParameter("currentCDIID", currentPeriodical.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentPeriodical.CDInteractionID));

                                //Обновление данных в DataGrid:
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<CustomerWindows.QueryResultClasses.CustomerOverviewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", UserID)).ToList();
                                PeriodicalsDataGrid.ItemsSource = null;
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
                            }
                        }
                    }
                    break;
                case 5:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов и изменение представления дат:
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
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "CheckedOutDate":
                    e.Column.Header = "Дата выдачи";
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "FactReturnDate":
                    e.Column.Header = "Дата возврата";
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                    break;
                case "IfRenewed":
                    e.Column.Header = "Документ продлен?";
                    break;
                case "CDInteractionID":
                    //Не отображать столбец в DataGrid:
                    e.Cancel = true;
                    break;
            }
        }
    }
}
