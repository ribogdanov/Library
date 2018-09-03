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
    /// Логика взаимодействия для AddPeriodicalWindow.xaml
    /// </summary>
    public partial class AddPeriodicalWindow : Window
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;
        public AddPeriodicalWindow()
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
                    //Нажата кнопка "Назад"
                    Close();
                    break;

                case 1:
                    //Нажата кнопка "Добавить периодическое издание"
                    const string CreatePeriodicalQuery = "insert into [Coursework_2018].[dbo].[Periodical] (PeriodicalName, IssueRegularity, Language, Comment) values(@periodicalName, @issueRegularity, @language, @comment)";
                    if (PeriodicalNameTextBox.Text != "" && IssueRegularityTextBox.Text != "" && LanguageTextBox.Text != "")
                    {
                        using (var db = new TheContext())
                        {
                            db.Database.ExecuteSqlCommand(CreatePeriodicalQuery,
                                new SqlParameter("@periodicalName", PeriodicalNameTextBox.Text),
                                new SqlParameter("@issueRegularity", IssueRegularityTextBox.Text),
                                new SqlParameter("@language", LanguageTextBox.Text), 
                                new SqlParameter("@comment", CommentTextBox.Text));
                        }
                        DataChanged?.Invoke(this, new EventArgs());
                        MessageBox.Show("Периодическое издание добавлено.");
                    }
                    else
                        MessageBox.Show("Необходимо заполнить все поля кроме \"Комментарий\"");
                    break;
            }
        }
    }
}
