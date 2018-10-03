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
        private List<QueryResultClasses.ReturnRenewWindow_BooksDataGrid> BooksDataGridQueryResult { get; set; }
        #region BooksDataGridSqlQuery
        private const string BooksDataGridSqlQuery = @"
select
    Book.Title,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
	BookItem.ISBN,
    DocumentItem.DocumentItemID,
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
    DocumentItem.DocumentItemID,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID
";
        #endregion
        private List<QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid> PeriodicalsDataGridQueryResult { get; set; }
        #region PeriodicalsDataGridSqlQuery
        private const string PeriodicalsDataGridSqlQuery = @"
select 
	Periodical.PeriodicalName,
	PeriodicalIssue.IssueNumber,
	PeriodicalIssue.IssuePeriod,
    DocumentItem.DocumentItemID,
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

        private const string RenewDocumentQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set DueDate=@dueDate, IfRenewed=1 where CDInteractionID=@currentCDIID";
        private const string AddRenewalDateQuery = "insert [Coursework_2018].[dbo].[RenewalDates] (CDInteractionID, RenewalDate) values (@currentCDIID, getdate())";
        private const string ReturnDocumentQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set FactReturnDate=@factReturnDate, Status='Returned' where CDInteractionID=@currentCDIID";
        private const string SetCDIStatusTakenQuery = "update [Coursework_2018].[dbo].[CustomerDocumentInteraction] set Status = 'Taken' where CDInteractionID = @id";
        private const string SetDocumentStatusAvailableQuery = "update[Coursework_2018].[dbo].[DocumentItem] set Status = 'Available' where DocumentItemID = @id";

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
                    if (IDTextBox.Text != "")
                    {

                        //UserID = Convert.ToInt32(IDTextBox.Text);
                        using (var db = new TheContext())
                        {
                            List<Customer> customer = null;
                            bool flag = true;

                            try
                            {
                                //Вывод ФИО клиента, ID которого введен:
                                customer = db.Customers.SqlQuery(GetCustomerByIDQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                CustomerNameTextBox.Text = $"Читатель: {customer[0].Name} {customer[0].Patronymic} {customer[0].Surname}";
                            }
                            catch (Exception exception)
                            {
                                if (exception is ArgumentOutOfRangeException || exception is System.Data.Entity.Core.EntityCommandExecutionException)
                                {
                                    flag = false;
                                    BooksDataGrid.ItemsSource = null;
                                    PeriodicalsDataGrid.ItemsSource = null;
                                    MessageBox.Show("Пользователя с таким ID не существует.");
                                }
                            }

                            if (flag == true)
                            {
                                //Вывод книг и периодики этого клиента:
                                BooksDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;
                            }
                        }
                    }
                    else
                        MessageBox.Show("Введите ID читателя.");
                    break;
                case 1:
                    //Нажата кнопка "Принять книгу"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.ReturnRenewWindow_BooksDataGrid currentBook = (QueryResultClasses.ReturnRenewWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        if (currentBook.Status == "Taken" || currentBook.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                db.Database.ExecuteSqlCommand(ReturnDocumentQuery, new SqlParameter("@factReturnDate", DateTime.Now), new SqlParameter("currentCDIID", currentBook.CDInteractionID));
                                db.Database.ExecuteSqlCommand(SetDocumentStatusAvailableQuery, new SqlParameter("@id", currentBook.DocumentItemID));
                                
                                //Обновление данных в DataGrid:
                                BooksDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                BooksDataGrid.ItemsSource = null;
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;

                                MessageBox.Show("Книга принята.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выбранная книга уже возвращена в библиотеку.");
                        }
                    }
                    else
                        MessageBox.Show("Выберите книгу для приёма.");
                    break;
                case 2:
                    //Нажата кнопка "Продлить книгу"
                    if (BooksDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.ReturnRenewWindow_BooksDataGrid currentBook = (QueryResultClasses.ReturnRenewWindow_BooksDataGrid)BooksDataGrid.SelectedItem;
                        if (currentBook.Status == "Taken" || currentBook.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentBook.DueDate + month), new SqlParameter("currentCDIID", currentBook.CDInteractionID));
                                if (currentBook.DueDate + month > DateTime.Now)
                                    db.Database.ExecuteSqlCommand(SetCDIStatusTakenQuery, new SqlParameter("@id", currentBook.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentBook.CDInteractionID));

                                //Обновление данных в DataGrid:
                                BooksDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_BooksDataGrid>(BooksDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                BooksDataGrid.ItemsSource = null;
                                BooksDataGrid.ItemsSource = BooksDataGridQueryResult;

                                MessageBox.Show("Книга продлена.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выбранная книга уже возвращена в библиотеку.");
                        }
                    }
                    else
                        MessageBox.Show("Выберите книгу для продления.");
                    break;
                case 3:
                    //Нажата кнопка "Принять периодическое издание"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid currentPeriodical = (QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid)PeriodicalsDataGrid.SelectedItem;
                        if (currentPeriodical.Status == "Taken" || currentPeriodical.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                db.Database.ExecuteSqlCommand(ReturnDocumentQuery, new SqlParameter("@factReturnDate", DateTime.Now), new SqlParameter("currentCDIID", currentPeriodical.CDInteractionID));
                                db.Database.ExecuteSqlCommand(SetDocumentStatusAvailableQuery, new SqlParameter("@id", currentPeriodical.DocumentItemID));

                                //Обновление данных в DataGrid:
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                PeriodicalsDataGrid.ItemsSource = null;
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;

                                MessageBox.Show("Периодическое издание принято.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выбранное издание уже возвращено в библиотеку.");
                        }
                    }
                    else
                        MessageBox.Show("Выберите периодическое издание для приёма.");
                    break;
                case 4:
                    //Нажата кнопка "Продлить периодическое издание"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid currentPeriodical = (QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid)PeriodicalsDataGrid.SelectedItem;
                        if (currentPeriodical.Status == "Taken" || currentPeriodical.Status == "Overdue")
                        {
                            using (TheContext db = new TheContext())
                            {
                                TimeSpan month = new TimeSpan(31, 0, 0, 0);
                                db.Database.ExecuteSqlCommand(RenewDocumentQuery, new SqlParameter("@dueDate", currentPeriodical.DueDate + month), new SqlParameter("currentCDIID", currentPeriodical.CDInteractionID));
                                if (currentPeriodical.DueDate + month > DateTime.Now)
                                    db.Database.ExecuteSqlCommand(SetCDIStatusTakenQuery, new SqlParameter("@id", currentPeriodical.CDInteractionID));
                                db.Database.ExecuteSqlCommand(AddRenewalDateQuery, new SqlParameter("@currentCDIID", currentPeriodical.CDInteractionID));

                                //Обновление данных в DataGrid:
                                PeriodicalsDataGridQueryResult = db.Database.SqlQuery<QueryResultClasses.ReturnRenewWindow_PeriodicalsDataGrid>(PeriodicalsDataGridSqlQuery, new SqlParameter("@id", IDTextBox.Text)).ToList();
                                PeriodicalsDataGrid.ItemsSource = null;
                                PeriodicalsDataGrid.ItemsSource = PeriodicalsDataGridQueryResult;

                                MessageBox.Show("Периодическое издание продлено.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выбранное издание уже возвращено в библиотеку.");
                        }
                    }
                    else
                        MessageBox.Show("Выберите периодическое издание для продления.");
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
                case "DocumentItemID":
                    //Не отображать столбец в DataGrid:
                    e.Cancel = true;
                    break;
            }
        }
    }
}
