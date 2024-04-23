using System.ComponentModel.DataAnnotations;

namespace API_LIBRARY.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
