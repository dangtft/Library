using API_LIBRARY.Models;

namespace API_LIBRARY.Services
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync(); // GET All Books
        Task<Book> GetBookAsync(int id); // Get Single Book
        Task<Book> AddBookAsync(Book book); // POST New Book
        Task<Book> UpdateBookAsync(Book book); // PUT Book
        Task<(bool, string)> DeleteBookAsync(Book book); // DELETE Book

        /////// author

        Task<List<Author>> GetAuthorsAsync(); // GET All Authors
        Task<Author> GetAuthorAsync(int id, bool includeBooks = false); // GET Single Author
        Task<Author> AddAuthorAsync(Author author); // POST New Author
        Task<Author> UpdateAuthorAsync(Author author); // PUT Author
        Task<(bool, string)> DeleteAuthorAsync(Author author); // DELETE Author

        //Pulisher
        Task<List<Publisher>> GetPublishersAsync(); // GET All Publishers
        Task<Publisher> GetPublisherAsync(int id, bool includeBooks = false); // GET Single Publisher
        Task<Publisher> AddPublisherAsync(Publisher publisher); // POST New Publisher
        Task<Publisher> UpdatePublisherAsync(Publisher publisher); // PUT Publisher
        Task<(bool, string)> DeletePublisherAsync(Publisher publisher); // DELETE Publisher

    }
}
