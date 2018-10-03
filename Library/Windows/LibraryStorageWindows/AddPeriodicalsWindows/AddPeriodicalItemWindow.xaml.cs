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
    /// Логика взаимодействия для AddPeriodicalItemWindow.xaml
    /// </summary>
    public partial class AddPeriodicalItemWindow : Window
    {
        public delegate void DataChangedEventHandler(object sender, EventArgs e);
        public event DataChangedEventHandler DataChanged;

        PeriodicalIssue PeriodicalIssue;

        public AddPeriodicalItemWindow(PeriodicalIssue periodicalIssue)
        {
            InitializeComponent();
            PeriodicalIssue = periodicalIssue;
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
                    //Нажата кнопка "Добавить экземпляр периодического издания"
                    const string CreateDocumentItemGetIDQuery = "insert into [Coursework_2018].[dbo].[DocumentItem] (Status, ReceivedDate, Comment, Type) values ('Available', getdate(), @comment, 'Periodical') select @@identity as ID";
                    const string CreatePeriodicalItemQuery = "insert into [Coursework_2018].[dbo].[PeriodicalItem] (PeriodicalIssueID, DocumentItemID) values (@periodicalIssueID, @documentIssueID)";
                    const string UpdatePeriodicalIssueQuery = "update [Coursework_2018].[dbo].[PeriodicalIssue] set StoredItemsNumber = StoredItemsNumber+1 where PeriodicalIssueID = @periodicalIssueID";

                    using (var db = new TheContext())
                    {
                        List<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID> DocumentIssueIDObject = db.Database.SqlQuery<AddBooksWindows.QueryResultClasses.AddBookItemWindow_DocumentItemID>(CreateDocumentItemGetIDQuery, new SqlParameter("@comment", CommentTextBox.Text)).ToList();
                        decimal DocumentIssueID = DocumentIssueIDObject[0].ID;
                        db.Database.ExecuteSqlCommand(CreatePeriodicalItemQuery, new SqlParameter("@periodicalIssueID", PeriodicalIssue.PeriodicalIssueID), new SqlParameter("@documentIssueID", DocumentIssueID));
                        db.Database.ExecuteSqlCommand(UpdatePeriodicalIssueQuery, new SqlParameter("@periodicalIssueID", PeriodicalIssue.PeriodicalIssueID));
                    }

                    DataChanged?.Invoke(this, new EventArgs());

                    //Очистка заполняемого поля:
                    CommentTextBox.Text = "";

                    MessageBox.Show("Экземпляр периодического издания добавлен.");
                    break;
            }
        }
    }
}
