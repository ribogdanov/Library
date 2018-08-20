using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.LibraryStorageWindows.QueryResultClasses
{
    class OverdueDocumentsOverviewWindow_BooksDataGrid
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public int DocumentItemID { get; set; }
        public string CustomerCredentials { get; set; }
        public int CustomerID { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CheckedOutDate { get; set; }
        public Nullable<DateTime> FactReturnDate { get; set; }
        public bool IfRenewed { get; set; }
        public int CDInteractionID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerPatronymic { get; set; }
    }
}
