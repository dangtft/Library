using API_LIBRARY.Models;
using API_LIBRARY.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : Controller
    {
        private readonly IBookRepository bookRepository;
        public AuthorController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }
        [HttpGet("get-all-author")]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await bookRepository.GetAuthorsAsync();

            if (authors == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, "No authors in database");
            }

            return StatusCode(StatusCodes.Status200OK, authors);
        }

        [HttpGet("get-author-by-id/{id}")]
        public async Task<IActionResult> GetAuthor(int id, bool includeBooks = false)
        {
            Author author = await bookRepository.GetAuthorAsync(id, includeBooks);

            if (author == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, $"No Author found for id: {id}");
            }

            return StatusCode(StatusCodes.Status200OK, author);
        }

        [HttpPost]
        public async Task<ActionResult<Author>> AddAuthor(Author author)
        {
            var dbAuthor = await bookRepository.AddAuthorAsync(author);

            if (dbAuthor == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{author.FullName} could not be added.");
            }

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            Author dbAuthor = await bookRepository.UpdateAuthorAsync(author);

            if (dbAuthor == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{author.FullName} could not be updated");
            }

            return NoContent();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await bookRepository.GetAuthorAsync(id, false);
            (bool status, string message) = await bookRepository.DeleteAuthorAsync(author);

            if (status == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return StatusCode(StatusCodes.Status200OK, author);
        }
    }
}
