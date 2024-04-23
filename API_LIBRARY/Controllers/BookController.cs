using API_LIBRARY.Models;
using API_LIBRARY.Services;
using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.DTO;
using API_LIBRARY.Data;
using Microsoft.EntityFrameworkCore;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private LibaryDbContext _db;
        private readonly IBookRepository _bookRepository;
        public BookController(IBookRepository bookRepository,LibaryDbContext libaryDbContext)
        {
            _bookRepository = bookRepository;
            _db = libaryDbContext;
        }

        [HttpGet("get-all-book")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author).ToListAsync();


            if (books == null || !books.Any())
            {
                return StatusCode(StatusCodes.Status204NoContent, "No books in database.");
            }

            var bookDTOs = books.Select(book => new BookAuthorAndPublisher
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
                PublisherId = bookDTO.PublisherId
            };

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

