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

namespace Library.Windows.LibraryStorageWindows.DocumentWriteOffWindows
{
    /// <summary>
    /// Логика взаимодействия для MainDocumentWriteOffWindow.xaml
    /// </summary>
    public partial class MainDocumentWriteOffWindow : Window
    {
        public BookWriteOffWindow bookWriteOffWindow { get; set; }
        public PeriodicalWriteOffWindow periodicalWriteOffWindow { get; set; }

        public MainDocumentWriteOffWindow()
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
                    //Нажата кнопка "Списать книгу"
                    bookWriteOffWindow = new BookWriteOffWindow();
                    bookWriteOffWindow.ShowDialog();
                    break;
                case 1:
                    //Нажата кнопка "Списать периодическое издание"
                    periodicalWriteOffWindow = new PeriodicalWriteOffWindow();
                    periodicalWriteOffWindow.ShowDialog();
                    break;
                case 2:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }
    }
}
