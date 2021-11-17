using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryModel.Models.LibraryViewModels
{
    public class PublishedBookData
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
    }
}
