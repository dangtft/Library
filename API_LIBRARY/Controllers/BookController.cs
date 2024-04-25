using API_LIBRARY.Models;
using Microsoft.AspNetCore.Mvc;
using API_LIBRARY.DTO;
using API_LIBRARY.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API_LIBRARY.Interfaces;

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

            var bookDTOs = await _bookRepository.GetBooksAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            if (bookDTOs == null || !bookDTOs.Any())
            {
                return StatusCode(StatusCodes.Status204NoContent, "No books found matching the criteria.");
            }

            return StatusCode(StatusCodes.Status200OK, bookDTOs);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooks(int id)
        {
            var bookDTO = await _bookRepository.GetBookAsync(id);

            if (bookDTO == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, $"No book found for id: {id}");
            }

            return StatusCode(StatusCodes.Status200OK, bookDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(BookDTO bookDTO)
        {
            var result = await _bookRepository.AddBookAsync(bookDTO);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add book.");
            }

            return StatusCode(StatusCodes.Status200OK, "Books added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id,[FromBody] BookDTO bookDTO)
        {
            var result = await _bookRepository.UpdateBookAsync(id, bookDTO);

            if (!result)
            {
                return NotFound();
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
            var result = await _bookRepository.DeleteBookAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return StatusCode(StatusCodes.Status200OK, "Book deleted successfully");
        }
    }
}

