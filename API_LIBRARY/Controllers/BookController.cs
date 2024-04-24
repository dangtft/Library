using API_LIBRARY.Models;
using API_LIBRARY.Services;
using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.DTO;
using API_LIBRARY.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class BookController : ControllerBase
    {
        private LibaryDbContext _db;
        private readonly IBookRepository _bookRepository;
        private ILogger<BookController> _logger;
        public BookController(IBookRepository bookRepository,LibaryDbContext libaryDbContext, ILogger<BookController> logger)
        {
            _bookRepository = bookRepository;
            _db = libaryDbContext;
            _logger = logger;
        }

        [HttpGet("get-all-book")]
        public async Task<IActionResult> GetBooks([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
                               [FromQuery] string? sortBy, [FromQuery] bool isAscending,
                               [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");

            IQueryable<Book> query = _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author);

            // Apply filters if provided
            if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "title":
                        query = query.Where(b => b.Title.Contains(filterQuery));
                        break;
                    case "description":
                        query = query.Where(b => b.Description.Contains(filterQuery));
                        break;
                    
                    case "author":
                        query = query.Where(b => b.BookAuthors.Any(ba => ba.Author.FullName.Contains(filterQuery)));
                        break;
                    default:
                        break;
                }
            }

            // Apply sorting if provided
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "title":
                        query = isAscending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
                        break;
                    case "dateadded":
                        query = isAscending ? query.OrderBy(b => b.DateAdded) : query.OrderByDescending(b => b.DateAdded);
                        break;
                    case "id":
                        query = isAscending ? query.OrderBy(b => b.Id) : query.OrderByDescending(b => b.Id);
                        break;
                    default:
                        break;
                }
            }

            // Paginate the results
            var paginatedBooks = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (paginatedBooks == null || !paginatedBooks.Any())
            {
                return StatusCode(StatusCodes.Status204NoContent, "No books found matching the criteria.");
            }

            var bookDTOs = paginatedBooks.Select(book => new BookAuthorAndPublisher
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                isRead = book.isRead,
                DateRead = book.DateRead,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.BookAuthors.Select(author => author.Author.FullName).ToList()
            }).ToList();

            return StatusCode(StatusCodes.Status200OK, bookDTOs);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooks(int id)
        {
            var book = await _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, $"No book found for id: {id}");
            }

            var bookDTO = new BookAuthorAndPublisher
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                isRead = book.isRead,
                DateRead = book.DateRead,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.BookAuthors.Select(author => author.Author.FullName).ToList()
            };

            return StatusCode(StatusCodes.Status200OK, bookDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookDTO bookDTO)
        {
            var book = new Book
            {
                Title = bookDTO.Title,
                Description = bookDTO.Description,
                isRead = bookDTO.isRead,
                DateRead = bookDTO.DateRead,
                Genre = bookDTO.Genre,
                CoverUrl = bookDTO.CoverUrl,
                DateAdded = bookDTO.DateAdded,
                PublisherId = bookDTO.PublisherId,
                
            };
            book.BookAuthors = bookDTO.AuthorId.Select(authorId => new BookAuthor { AuthorId = authorId }).ToList();

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, "Books added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id,[FromBody] BookDTO bookDTO)
        {
            var book = await _db.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.Title = bookDTO.Title;
            book.Description = bookDTO.Description;
            book.isRead = bookDTO.isRead;
            book.DateRead = bookDTO.DateRead;
            book.Genre = bookDTO.Genre;
            book.CoverUrl = bookDTO.CoverUrl;
            book.DateAdded = bookDTO.DateAdded;
            book.PublisherId = bookDTO.PublisherId;

            _db.Entry(book).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status200OK);
        }
        private bool BookExists(int id)
        {
            return _db.Books.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, book);
        }
    }
}

