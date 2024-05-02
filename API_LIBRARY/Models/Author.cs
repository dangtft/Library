using System.ComponentModel.DataAnnotations;

namespace API_LIBRARY.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        [Required]
        public List<BookAuthor> BookAuthors { get; set; }
    }
}
