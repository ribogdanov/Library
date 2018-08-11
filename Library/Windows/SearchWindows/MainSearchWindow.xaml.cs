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

namespace Library.Windows.SearchWindows
{
    /// <summary>
    /// Логика взаимодействия для MainSearchWindow.xaml
    /// </summary>
    public partial class MainSearchWindow : Window
    {
        public BookSearchWindow bookSearchWindow { get; set; }
        public PeriodicalSearchWindow periodicalSearchWindow { get; set; }
        public MainSearchWindow()
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
                    //Нажата кнопка "Искать книгу"
                    bookSearchWindow = new BookSearchWindow();
                    bookSearchWindow.Show();
                    break;
                case 1:
                    //Нажата кнопка "Искать периодическое издание"
                    periodicalSearchWindow = new PeriodicalSearchWindow();
                    periodicalSearchWindow.Show();
                    break;
                case 2:
                    //Нажата кнопка "Назад"
                    Close();
                    break;
            }
        }
    }
}
