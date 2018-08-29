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
    /// Логика взаимодействия для AddPeriodicalMainWindow.xaml
    /// </summary>
    public partial class AddPeriodicalMainWindow : Window
    {
        AddPeriodicalWindow addPeriodicalWindow;
        AddPeriodicalIssueWindow addPeriodicalIssueWindow;
        AddPeriodicalItemWindow addPeriodicalItemWindow;
        public AddPeriodicalMainWindow()
        {
            InitializeComponent();
            UpdatePeriodicalsDataGrid();
        }

        public void UpdatePeriodicalsDataGrid()
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
                    //Нажата кнопка "Добавить новое издание"
                    addPeriodicalWindow = new AddPeriodicalWindow();
                    addPeriodicalWindow.DataChanged += AddPeriodicalWindow_DataChanged;
                    addPeriodicalWindow.Show();
                    break;
                case 1:
                    //Нажата кнопка "Добавить новый выпуск для выбранного издания"
                    if (PeriodicalsDataGrid.SelectedItem != null)
                    {
                        Periodical currentPeriodical = (Periodical)PeriodicalsDataGrid.SelectedItem;
                        addPeriodicalIssueWindow = new AddPeriodicalIssueWindow(currentPeriodical);
                        addPeriodicalIssueWindow.DataChanged += AddPeriodicalIssueWindow_DataChanged;
                        addPeriodicalIssueWindow.Show();
                    }
                    else
                        MessageBox.Show("Выберите периодическое издание в таблице слева");
                    break;
                case 2:
                    //Нажата кнопка "Добавить экземпляр для выбранного издания и выпуска"
                    if (PeriodicalIssuesDataGrid.SelectedItem != null)
                    {
                        PeriodicalIssue currentPeriodicalIssue = (PeriodicalIssue)PeriodicalIssuesDataGrid.SelectedItem;
                        addPeriodicalItemWindow = new AddPeriodicalItemWindow(currentPeriodicalIssue);
                        addPeriodicalItemWindow.DataChanged += AddPeriodicalItemWindow_DataChanged;
                        addPeriodicalItemWindow.Show();
                    }
                    else
                        MessageBox.Show("Выберите периодическое издание в таблице слева и выпуск периодического издания в таблице справа");
                    break;
                case 3:
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
                    e.Cancel = true;
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
                UpdatePeriodicalIssuesDataGrid();            
        }

        private void UpdatePeriodicalIssuesDataGrid()
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

        private void AddPeriodicalWindow_DataChanged(object sender, EventArgs e)
        {
            UpdatePeriodicalsDataGrid();
            PeriodicalIssuesDataGrid.ItemsSource = null;
        }

        private void AddPeriodicalIssueWindow_DataChanged(object sender, EventArgs e)
        {
            UpdatePeriodicalIssuesDataGrid();
        }

        private void AddPeriodicalItemWindow_DataChanged(object sender, EventArgs e)
        {
            UpdatePeriodicalIssuesDataGrid();
        }
    }
}
