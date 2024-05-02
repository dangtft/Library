using API_LIBRARY.DTO;
using API_LIBRARY.Models;

namespace API_LIBRARY.Interfaces
{
    public interface IBookRepository
    {
        Task<List<BookAuthorAndPublisher>> GetBooksAsync(string? filterOn, string? filterQuery,
            string? sortBy, bool isAscending, int pageNumber = 1, int pageSize = 100);

        Task<BookAuthorAndPublisher> GetBookAsync(int id);

        Task<bool> AddBookAsync(BookDTO bookDTO);

        Task<bool> UpdateBookAsync(int id, BookDTO bookDTO);

        Task<bool> DeleteBookAsync(int id);

        /////// author

        Task<List<Author>> GetAuthorsAsync();

        Task<Author> GetAuthorAsync(int id, bool includeBooks = false);

        Task<Author> AddAuthorAsync(AddAuthor authorDTO);

        Task<bool> UpdateAuthorAsync(int id, AuthorDTO authorDTO);

        Task<bool> DeleteAuthorAsync(int id);

        //Pulisher
        Task<List<Publisher>> GetPublishersAsync();

        Task<Publisher> GetPublisherAsync(int id, bool includeBooks = false);

        Task<Publisher> AddPublisherAsync(PulisherDTO publisherDTO);

        Task<bool> UpdatePublisherAsync(int id, PulisherDTO publisherDTO);

        Task<bool> DeletePublisherAsync(int id);

    }
}
