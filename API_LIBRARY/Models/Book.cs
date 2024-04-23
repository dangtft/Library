using System.ComponentModel.DataAnnotations;

namespace API_LIBRARY.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? isRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime? DateAdded { get; set; }
        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        public List<BookAuthor>? BookAuthors { get; set; }
    }
}
