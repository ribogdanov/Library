using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Windows.LibraryStorageWindows.AddBooksWindows.QueryResultClasses
{
    public class AddBookMainWindow_BooksDataGrid
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public int StoredItemsNumber { get; set; }
        public string Language { get; set; }
        public string Comment { get; set; }
        public int BookID { get; set; }
    }
}
