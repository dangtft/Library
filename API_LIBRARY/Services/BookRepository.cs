using API_LIBRARY.Data;
using API_LIBRARY.Models;
using Microsoft.EntityFrameworkCore;
namespace API_LIBRARY.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly LibaryDbContext _db;
        public BookRepository(LibaryDbContext context)
        {
            _db = context;
        }

        #region Books

        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                return await _db.Books.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Book> GetBookAsync(int id)
        {
            try
            {
                return await _db.Books.FindAsync(id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            try
            {
                await _db.Books.AddAsync(book);
                await _db.SaveChangesAsync();
                return await _db.Books.FindAsync(book.Id); // Auto ID from DB
            }
            catch (Exception ex)
            {
                return null; // An error occured
            }
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            try
            {
                _db.Entry(book).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return book;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool, string)> DeleteBookAsync(Book book)
        {
            try
            {
                var dbBook = await _db.Books.FindAsync(book.Id);

                if (dbBook == null)
                {
                    return (false, "Book could not be found.");
                }

                _db.Books.Remove(book);
                await _db.SaveChangesAsync();

                return (true, "Book got deleted.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occured. Error Message: {ex.Message}");
            }
        }

        #endregion Books

        #region Authors

        public async Task<List<Author>> GetAuthorsAsync()
        {
            try
            {
                return await _db.Authors.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Author> GetAuthorAsync(int id, bool includeBooks)
        {
            try
            {
                var query = _db.Authors.AsQueryable();
                if (includeBooks)
                {
                    query = query.Include(a => a.BookAuthors).ThenInclude(ba => ba.Book);
                }
                return await query.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            try
            {
                await _db.Authors.AddAsync(author);
                await _db.SaveChangesAsync();
                return await _db.Authors.FindAsync(author.Id); // Auto ID from DB
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            try
            {
                _db.Entry(author).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return author;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool, string)> DeleteAuthorAsync(Author author)
        {
            try
            {
                var dbAuthor = await _db.Authors.FindAsync(author.Id);

                if (dbAuthor == null)
                {
                    return (false, "Author could not be found");
                }

                _db.Authors.Remove(author);
                await _db.SaveChangesAsync();

                return (true, "Author got deleted.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occured. Error Message: {ex.Message}");
            }
        }
        #endregion Author

        #region Pulishers
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await _db.Publisher.ToListAsync();
        }

        public async Task<Publisher> GetPublisherAsync(int id, bool includeBooks = false)
        {
            var query = _db.Publisher.AsQueryable();
            if (includeBooks)
            {
                query = query.Include(p => p.Books);
            }
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Publisher> AddPublisherAsync(Publisher publisher)
        {
            _db.Publisher.Add(publisher);
            await _db.SaveChangesAsync();
            return publisher;
        }

        public async Task<Publisher> UpdatePublisherAsync(Publisher publisher)
        {
            _db.Entry(publisher).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return publisher;
        }

        public async Task<(bool, string)> DeletePublisherAsync(Publisher publisher)
        {
            try
            {
                _db.Publisher.Remove(publisher);
                await _db.SaveChangesAsync();
                return (true, "Publisher deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting publisher: {ex.Message}");
            }
        }

        #endregion Pulisher
    }
}
