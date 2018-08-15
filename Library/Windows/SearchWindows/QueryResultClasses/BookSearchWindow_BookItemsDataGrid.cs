using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.SearchWindows.QueryResultClasses
{
    class BookSearchWindow_BookItemsDataGrid
    {
        public int DocumentItemID { get; set; }
        public int BookID { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishDate { get; set; }
        public string Publisher { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDate { get; set; }
        public Nullable<DateTime> WrittenOffDate { get; set; }
        public string Comment { get; set; }

    }
}
