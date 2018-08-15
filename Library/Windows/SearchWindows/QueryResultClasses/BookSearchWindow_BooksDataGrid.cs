using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.SearchWindows.QueryResultClasses
{
    class BookSearchWindow_BooksDataGrid
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public int StoredItemsNumber { get; set; }
        public string Language { get; set; }
        public string Comment { get; set; }
        public string Authors { get; set; }
        public string AuthorSurnames { get; set; }
        public string AuthorNames { get; set; }
        public string AuthorPatronymics { get; set; }
    }
}
