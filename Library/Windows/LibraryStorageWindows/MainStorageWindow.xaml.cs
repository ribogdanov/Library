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
    /// Логика взаимодействия для MainStorageWindow.xaml
    /// </summary>
    public partial class MainStorageWindow : Window
    {
        public AddBookMainWindow addBookMainWindow { get; set; }
        public AddPeriodicalMainWindow addPeriodicalMainWindow { get; set; }
        public DocumentWriteOffWindows.MainDocumentWriteOffWindow documentWriteOffWindow { get; set; }
        public OverdueDocumentsOverviewWindow overdueDocumentsOverviewWindow { get; set; }
        public SearchWindows.MainSearchWindow mainSearchwindow { get; set; }
        public MainStorageWindow()
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
                    addBookMainWindow = new AddBookMainWindow();
                    addBookMainWindow.Show();
                    break;
                case 1:
                    addPeriodicalMainWindow = new AddPeriodicalMainWindow();
                    addPeriodicalMainWindow.Show();
                    break;
                case 2:
                    documentWriteOffWindow = new DocumentWriteOffWindows.MainDocumentWriteOffWindow();
                    documentWriteOffWindow.Show();
                    break;
                case 3:
                    overdueDocumentsOverviewWindow = new OverdueDocumentsOverviewWindow();
                    overdueDocumentsOverviewWindow.Show();
                    break;
                case 4:
                    mainSearchwindow = new SearchWindows.MainSearchWindow();
                    mainSearchwindow.Show();
                    break;
                case 5:
                    Close();
                    break;
            }
        }
    }
}
