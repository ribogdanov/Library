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

namespace Library.Windows.SearchWindows
{
    /// <summary>
    /// Логика взаимодействия для PeriodicalSearchWindow.xaml
    /// </summary>
    public partial class PeriodicalSearchWindow : Window
    {
        public PeriodicalSearchWindow()
        {
            InitializeComponent();
            AllPeriodicals();
        }

        public void AllPeriodicals()
        {
            using (var db = new TheContext())
            {
                var allPeriodicals = db.Periodicals.SqlQuery("SELECT * FROM [Coursework_2018].[dbo].[Periodical]").ToList();
                PeriodicalsDataGrid.ItemsSource = allPeriodicals;
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
                    //Нажата кнопка "Искать периодическое издание по названию"
                    if (PeriodicalNameTextBox.Text != "")
                    {
                        using (TheContext db = new TheContext())
                        {
                            var periodicals = db.Periodicals.SqlQuery(
                                "select * from[Coursework_2018].[dbo].[Periodical] where PeriodicalName like @name",
                                new SqlParameter("@name", $"%{PeriodicalNameTextBox.Text}%")
                            ).ToList();
                            PeriodicalsDataGrid.ItemsSource = periodicals;
                        }
                    }
                    else
                    {
                        //Вывод окна с сообщением и вывод в DataGrid всей периодики из БД
                        MessageBoxResult result = MessageBox.Show("Введите данные для поиска.");
                        AllPeriodicals();
                    }
                    PeriodicalItemsDataGrid.ItemsSource = null;
                    PeriodicalIssuesDataGrid.ItemsSource = null;
                    break;
                case 1:
                    //Нажата кнопка "Искать выпуск периодического издания по номеру"
                    if (IssueNumberTextBox.Text != "")
                    {
                        if (PeriodicalsDataGrid.SelectedItem != null)
                        {
                            using (TheContext db = new TheContext())
                            {
                                Periodical currentPeriodical = (Periodical)PeriodicalsDataGrid.SelectedItem;
                                var periodicalIssues = db.PeriodicalIssues.SqlQuery(
                                    "select * from [Coursework_2018].[dbo].[PeriodicalIssue] where PeriodicalID=@periodicalID and IssueNumber=@issueNumber",
                                    new SqlParameter("@periodicalID", currentPeriodical.PeriodicalID),
                                    new SqlParameter("@issueNumber", Convert.ToInt32(IssueNumberTextBox.Text))
                                ).ToList();
                                PeriodicalIssuesDataGrid.ItemsSource = periodicalIssues;
                            }
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Выберите периодическое издание.");
                        }
                    }
                    else
                    {
                        //Вывод окна с сообщением и вывод в DataGrid всей периодики из БД
                        MessageBoxResult result = MessageBox.Show("Введите данные для поиска.");
                        AllPeriodicals();
                    }
                    PeriodicalItemsDataGrid.ItemsSource = null;
                    break;
                case 2:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }

        private void PeriodicalsDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов и отмена отображения некоторых столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "PeriodicalID":
                    e.Cancel=true;
                    break;
                case "PeriodicalName":
                    e.Column.Header = "Название издания";
                    break;
                case "IssueRegularity":
                    e.Column.Header = "Частота выпусков";
                    break;
                case "Language":
                    e.Column.Header = "Язык";
                    break;
                case "Comment":
                    e.Column.Header = "Комметарий";
                    break;
                case "PeriodicalIssues":
                    e.Cancel = true;
                    break;
            }
        }

        private void PeriodicalsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            const string GetIssuesByPeriodicalIDSqlQuery = "select * from [Coursework_2018].[dbo].[PeriodicalIssue] where PeriodicalID=@id";
            if (PeriodicalsDataGrid.SelectedItem != null)
            {
                Periodical currentPeriodical = (Periodical)PeriodicalsDataGrid.SelectedItem;
                using (TheContext db = new TheContext())
                {
                    var periodicalIssues = db.PeriodicalIssues.SqlQuery(GetIssuesByPeriodicalIDSqlQuery, new SqlParameter("@id", currentPeriodical.PeriodicalID)).ToList();
                    PeriodicalIssuesDataGrid.ItemsSource = periodicalIssues;
                }
                PeriodicalItemsDataGrid.ItemsSource = null;
            }
        }

        private void PeriodicalIssuesDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов и отмена отображения некоторых столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "PeriodicalID":
                    e.Cancel = true;
                    break;
                case "PeriodicalIssueID":
                    e.Cancel = true;
                    break;
                case "IssueNumber":
                    e.Column.Header = "Номер выпуска";
                    break;
                case "IssuePeriod":
                    e.Column.Header = "Период выпуска";
                    break;
                case "StoredItemsNumber":
                    e.Column.Header = "Количество экземпляров в наличии";
                    break;
                case "Publisher":
                    e.Column.Header = "Издатель";
                    break;
                case "Comment":
                    e.Column.Header = "Комментарий";
                    break;
                case "Periodical":
                    e.Cancel = true;
                    break;
                case "DocumentItems":
                    e.Cancel = true;
                    break;
            }
        }

        private void PeriodicalIssuesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region GetPeriodicalItemsByIssueIDSqlQuery 
            const string GetPeriodicalItemsByIssueIDSqlQuery = @"
select 
	PeriodicalItem.PeriodicalIssueID,
	PeriodicalItem.DocumentItemID,
	DocumentItem.Status,
	DocumentItem.ReceivedDate,
	DocumentItem.WrittenOffDate,
	DocumentItem.Comment
from [Coursework_2018].[dbo].[PeriodicalItem]
inner join [Coursework_2018].[dbo].[DocumentItem]
on [Coursework_2018].[dbo].[PeriodicalItem].DocumentItemID=[Coursework_2018].[dbo].[DocumentItem].DocumentItemID
where PeriodicalIssueID=@id
";
            #endregion
            if (PeriodicalIssuesDataGrid.SelectedItem != null)
            {
                var currentIssue = (PeriodicalIssue)PeriodicalIssuesDataGrid.SelectedItem;
                using (TheContext db = new TheContext())
                {
                    List<QueryResultClasses.PeriodicalSearchWindow_PeriodicalItemsDataGrid> periodicalItems = db.Database.SqlQuery<QueryResultClasses.PeriodicalSearchWindow_PeriodicalItemsDataGrid>(GetPeriodicalItemsByIssueIDSqlQuery, new SqlParameter("@id", currentIssue.PeriodicalIssueID)).ToList();
                    PeriodicalItemsDataGrid.ItemsSource = periodicalItems;
                }
            }
        }

        private void PeriodicalItemsDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Изменение заголовков столбцов и отмена отображения некоторых столбцов:
            string header = e.Column.Header.ToString();
            switch (header)
            {
                case "PeriodicalIssueID":
                    e.Cancel = true;
                    break;
                case "DocumentItemID":
                    e.Column.Header = "ID экземпляра";
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
