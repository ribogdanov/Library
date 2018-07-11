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

namespace Library.Windows.CustomerWindows
{
    /// <summary>
    /// Логика взаимодействия для CustomerLogin.xaml
    /// </summary>
    public partial class CustomerLogin : Window
    {
        public CustomerOverview customerOverviewWindow { get; set; }
        public CustomerLogin()
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
                    customerOverviewWindow = new CustomerOverview();
                    customerOverviewWindow.Show();
                    break;
                case 1:
                    Close();
                    break;
            }
        }
    }
}
