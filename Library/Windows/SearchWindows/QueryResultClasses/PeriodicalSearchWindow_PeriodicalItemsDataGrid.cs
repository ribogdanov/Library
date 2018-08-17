using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.SearchWindows.QueryResultClasses
{
    class PeriodicalSearchWindow_PeriodicalItemsDataGrid
    {
        public int PeriodicalIssueID { get; set; }
        public int DocumentItemID { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDate { get; set; }
        public Nullable<DateTime> WrittenOffDate { get; set; }
        public string Comment { get; set; }
    }
}
