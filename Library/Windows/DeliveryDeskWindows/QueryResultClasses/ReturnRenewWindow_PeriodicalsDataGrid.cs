using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.DeliveryDeskWindows.QueryResultClasses
{
    class ReturnRenewWindow_PeriodicalsDataGrid
    {
        public string PeriodicalName { get; set; }
        public int IssueNumber { get; set; }
        public string IssuePeriod { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CheckedOutDate { get; set; }
        public Nullable<DateTime> FactReturnDate { get; set; }
        public bool IfRenewed { get; set; }
        public int CDInteractionID { get; set; }
        public int DocumentItemID { get; set; }
    }
}
