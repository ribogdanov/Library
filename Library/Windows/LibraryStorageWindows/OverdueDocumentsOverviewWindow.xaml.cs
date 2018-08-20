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
    /// Логика взаимодействия для OverdueDocumentsOverviewWindow.xaml
    /// </summary>
    public partial class OverdueDocumentsOverviewWindow : Window
    {
        public OverdueDocumentsOverviewWindow()
        {
            #region BooksDataGridQuery 
            const string BooksDataGridQuery = @"
select
    Book.Title,
	string_agg (Concat(Author.Surname, ' ', Author.Name, ' ', Author.Patronymic), ', ') as Authors,
    DocumentItem.DocumentItemID,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID,
	Customer.CustomerID,
	Concat(Customer.Surname, ' ', Customer.Name, ' ', Customer.Patronymic) as CustomerCredentials,
	Customer.Surname as CustomerSurname,
	Customer.Name as CustomerName,
	Customer.Patronymic as CustomerPatronymic
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
inner join [Coursework_2018].[dbo].[Customer]
		on [Coursework_2018].[dbo].[CustomerDocumentInteraction].CustomerID=[Coursework_2018].[dbo].[Customer].CustomerID
		where CustomerDocumentInteraction.Status='Overdue'
Group by
    Book.BookID,
	Book.Title,  
    DocumentItem.DocumentItemID,
	CustomerDocumentInteraction.Status,
	CustomerDocumentInteraction.DueDate,
	CustomerDocumentInteraction.CheckedOutDate,
	CustomerDocumentInteraction.FactReturnDate,
	CustomerDocumentInteraction.IfRenewed,
	CustomerDocumentInteraction.CDInteractionID,
	Customer.CustomerID,Customer.Surname,
	Customer.Name,
	Customer.Patronymic
";
            #endregion
            #region PeriodicalsDataGridQuery
            const string PeriodicalsDataGridQuery = @"
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
	CustomerDocumentInteraction.CDInteractionID,
	Customer.CustomerID,
	Concat(Customer.Surname, ' ', Customer.Name, ' ', Customer.Patronymic) as CustomerCredentials,
	Customer.Surname as CustomerSurname,
	Customer.Name as CustomerName,
	Customer.Patronymic as CustomerPatronymic
from [Coursework_2018].[dbo].[Periodical]
inner join [Coursework_2018].[dbo].[PeriodicalIssue]
        on [Coursework_2018].[dbo].[Periodical].PeriodicalID=[Coursework_2018].[dbo].[PeriodicalIssue].PeriodicalID
inner join [Coursework_2018].[dbo].[PeriodicalItem]
	    on [Coursework_2018].[dbo].[PeriodicalIssue].PeriodicalIssueID=[Coursework_2018].[dbo].[PeriodicalItem].PeriodicalIssueID
inner join [Coursework_2018].[dbo].[DocumentItem]
    	on [Coursework_2018].[dbo].[PeriodicalItem].DocumentItemID=[Coursework_2018].[dbo].[DocumentItem].DocumentItemID
inner join [Coursework_2018].[dbo].[CustomerDocumentInteraction]
	    on [Coursework_2018].[dbo].[DocumentItem].DocumentItemID=[Coursework_2018].[dbo].[CustomerDocumentInteraction].DocumentItemID
inner join [Coursework_2018].[dbo].[Customer]
		on [Coursework_2018].[dbo].[CustomerDocumentInteraction].CustomerID=[Coursework_2018].[dbo].[Customer].CustomerID
where CustomerDocumentInteraction.Status='Overdue'

";
            #endregion
            InitializeComponent();
            using (var db = new TheContext())
            {
                List<QueryResultClasses.OverdueDocumentsOverviewWindow_BooksDataGrid> books = db.Database.SqlQuery<QueryResultClasses.OverdueDocumentsOverviewWindow_BooksDataGrid>(BooksDataGridQuery).ToList();
                BooksDataGrid.ItemsSource = books;
                List<QueryResultClasses.OverdueDocumentsOverview_PeriodicalsDataGrid> periodicals = db.Database.SqlQuery<QueryResultClasses.OverdueDocumentsOverview_PeriodicalsDataGrid>(PeriodicalsDataGridQuery).ToList();
                PeriodicalsDataGrid.ItemsSource = periodicals;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов и формата представления даты:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                //Уникальные поля книг:
                case "Title":
                    e.Column.Header = "Название книги";
                    break;
                case "Authors":
                    e.Column.Header = "Автор(ы)";
                    break;
                //Уникальные поля периодики:
                case "PeriodicalName":
                    e.Column.Header = "Название периодического издания";
                    break;
                case "IssueNumber":
                    e.Column.Header = "Номер выпуска";
                    break;
                case "IssuePeriod":
                    e.Column.Header = "Период выпуска";
                    break;
                //Поля, общие для книг и периодики:
                case "DocumentItemID":
                    e.Column.Header = "ID экземпляра книги";
                    break;
                case "CustomerCredentials":
                    e.Column.Header = "ФИО читателя";
                    break;
                case "CustomerID":
                    e.Column.Header = "ID читателя";
                    break;
                case "Status":
                    e.Cancel = true;
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
                    e.Column.Header = "ID взаимодействия документ-читатель";
                    break;
                case "CustomerName":
                    e.Cancel = true;
                    break;
                case "CustomerSurname":
                    e.Cancel = true;
                    break;
                case "CustomerPatronymic":
                    e.Cancel = true;
                    break;
            }
        }
    }
}
