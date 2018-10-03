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
    /// Логика взаимодействия для AddPeriodicalIssueWindow.xaml
    /// </summary>
    public partial class AddPeriodicalIssueWindow : Window
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;
        Periodical Periodical;
        public AddPeriodicalIssueWindow(Periodical periodical)
        {
            InitializeComponent();
            Periodical = periodical;
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
                    //Нажата кнопка "Добавить выпуск периодического издания"
                    const string CreatePeriodicalIssueQuery = @"
insert into[Coursework_2018].[dbo].[PeriodicalIssue] (PeriodicalID, IssueNumber, IssuePeriod, StoredItemsNumber, Publisher, Comment) 
values (@periodicalID, @issueNumber, @issuePeriod, 0, @publisher, @comment)";
                    const string GetIssuesByPeriodicalID = "select * from [Coursework_2018].[dbo].[PeriodicalIssue] where PeriodicalID = @periodicalID";

                    if (IssueNumberTextBox.Text != "" && IssuePeriodTextBox.Text != "" && PublisherTextBox.Text != "")
                    {
                        int issueNumber = 0;
                        bool issueNumberCorrectFlag = true;

                        try
                        {
                            issueNumber = Convert.ToInt32(IssueNumberTextBox.Text);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Номер издания должен быть числом.");
                            issueNumberCorrectFlag = false;
                        }

                        if (issueNumberCorrectFlag == true)
                        {
                            using (var db = new TheContext())
                            {
                                bool noRepetitionsIssueNumberFlag = true;
                                bool noRepetitionsIssuePeriodFlag = true;
                                var issuesOfThePeriodical = db.PeriodicalIssues.SqlQuery(GetIssuesByPeriodicalID, new SqlParameter("@periodicalID", Periodical.PeriodicalID)).ToList();

                                //Проверка, есть ли в БД записи с введенными пользователем номером и периодом выпуска.
                                foreach (var item in issuesOfThePeriodical)
                                {
                                    if (item.IssueNumber == issueNumber)
                                    {
                                        noRepetitionsIssueNumberFlag = false;
                                        break;
                                    }
                                }

                                foreach (var item in issuesOfThePeriodical)
                                {
                                    if (item.IssuePeriod == IssuePeriodTextBox.Text)
                                    {
                                        noRepetitionsIssuePeriodFlag = false;
                                        break;
                                    }
                                }

                                if (noRepetitionsIssueNumberFlag == true && noRepetitionsIssuePeriodFlag == true)
                                {
                                    db.Database.ExecuteSqlCommand(CreatePeriodicalIssueQuery,
                                        new SqlParameter("@periodicalID", Periodical.PeriodicalID),
                                        new SqlParameter("@issueNumber", IssueNumberTextBox.Text),
                                        //Здесь будет обработка исключения если не число
                                        new SqlParameter("@issuePeriod", IssuePeriodTextBox.Text),
                                        new SqlParameter("@publisher", PublisherTextBox.Text),
                                        new SqlParameter("@comment", CommentTextBox.Text));
                                    DataChanged?.Invoke(this, new EventArgs());

                                    //Очиска заполняемых полей:
                                    IssueNumberTextBox.Text = "";
                                    IssuePeriodTextBox.Text = "";
                                    PublisherTextBox.Text = "";
                                    CommentTextBox.Text = "";

                                    MessageBox.Show("Выпуск периодического издания добавлен.");
                                }
                                else
                                {
                                    if (noRepetitionsIssueNumberFlag == false && noRepetitionsIssuePeriodFlag == false)
                                        MessageBox.Show("Выпуск с таким периодом выпуска и номером уже есть в базе данных, дублирование недопустимо.");
                                    if (noRepetitionsIssueNumberFlag == false && noRepetitionsIssuePeriodFlag == true)
                                        MessageBox.Show("Выпуск с таким номером уже есть в базе данных, дублирование недопустимо.");
                                    if (noRepetitionsIssueNumberFlag == true && noRepetitionsIssuePeriodFlag == false)
                                        MessageBox.Show("Выпуск с таким периодом выпуска уже есть в базе данных, дублирование недопустимо.");
                                }
                            }
                        }
                    }
                    else
                        MessageBox.Show("Необходимо заполнить все поля кроме \"Комментарий\"");
                    break;
            }
        }
    }
}
