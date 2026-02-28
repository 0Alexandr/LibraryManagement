using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public string ISBN { get; set; }
        public int QuantityInStock { get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        // Вспомогательные свойства для отображения в таблице (DataGrid)
        public string AuthorsDisplay => string.Join(", ", Authors.Select(a => a.LastName));
        public string GenresDisplay => string.Join(", ", Genres.Select(g => g.Name));
    }
}
