using API_LIBRARY.Data;
using API_LIBRARY.DTO;
using API_LIBRARY.Interfaces;
using API_LIBRARY.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly LibaryDbContext libraryDbContext;
        public AuthorController(IBookRepository bookRepository,LibaryDbContext libaryDbContext)
        {
            this.bookRepository = bookRepository;
            this.libraryDbContext = libaryDbContext;
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
        public async Task<ActionResult<Author>> AddAuthor(AuthorDTO authorDTO)
        {
            var author = new Author
            {
                FullName = authorDTO.FullName
            };

            libraryDbContext.Authors.Add(author);
            await libraryDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, "Author added successfully");
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateAuthor(int id,[FromBody] AuthorDTO authorDTO)
        {
            var author = await libraryDbContext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            author.FullName = authorDTO.FullName;

            libraryDbContext.Entry(author).State = EntityState.Modified;

            try
            {
                await libraryDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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
        private bool AuthorExists(int id)
        {
            return libraryDbContext.Authors.Any(e => e.Id == id);
        }
        [Authorize("Write,Read")]
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await libraryDbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            libraryDbContext.Remove(author);
            await libraryDbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, author);
        }
    }
}
