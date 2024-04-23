using API_LIBRARY.Models;

namespace API_LIBRARY.DTO
{
    public class BookAuthorAndPublisher
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? isRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime? DateAdded { get; set; }
        public string? PublisherName { get; set; }
        public List<string> AuthorNames { get; set; }

    }
}
