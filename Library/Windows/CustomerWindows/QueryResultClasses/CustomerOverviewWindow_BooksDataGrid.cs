﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.CustomerWindows.QueryResultClasses
{
    class CustomerOverviewWindow_BooksDataGrid
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public string ISBN { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CheckedOutDate { get; set; }
        public Nullable<DateTime> FactReturnDate { get; set; }
        public bool IfRenewed { get; set; }
        public int CDInteractionID { get; set; }
    }
}
